using System;
using HtmlAgilityPack;
using GiftServer.Properties;
using GiftServer.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Web;
using System.Resources;
using GiftServer.Server;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace GiftServer
{
    namespace HtmlManager
    {
        /// <summary>
        /// HTML Manager for the dashboard
        /// </summary>
        public class DashboardManager
        {
            private ResourceManager ResourceManager;
            private NavigationManager NavigationManager;
            /// <summary>
            /// Create a new Dashboard Manager
            /// </summary>
            /// <param name="controller">The controller for this thread</param>
            public DashboardManager(Controller controller)
            {
                Thread.CurrentThread.CurrentUICulture = controller.Culture;
                Thread.CurrentThread.CurrentCulture = controller.Culture;
                ResourceManager = new ResourceManager("GiftServer.HtmlTemplates", typeof(DashboardManager).Assembly);
                NavigationManager = controller.NavigationManager;
            }
            /// <summary>
            /// Update the events for a given page
            /// </summary>
            /// <remarks>
            /// Use this for a partially complete dashboard
            /// </remarks>
            /// <param name="user">The user viewing the page</param>
            /// <param name="page">The partially formed Dashboard</param>
            /// <returns>The HTML for the updated dashboard</returns>
            public string UpdateEvents(User user, string page)
            {
                HtmlDocument dash = new HtmlDocument();
                dash.LoadHtml(page);
                HtmlNode eventHolder = dash.DocumentNode.SelectSingleNode(@"//*[contains(concat("" "", normalize-space(@id), "" ""), "" eventHolder "")]");

                List<Event> events = new List<Event>();
                foreach (Group group in user.Groups)
                {
                    // Get all events associated with that group, add
                    events.AddRange(group.Events);
                }
                // Events should be ordered by the first future date
                events = events.Distinct().OrderBy(e => e.GetNearestOccurrence(DateTime.Today).Date).ToList();
                // For each event (up to some amount), list the name of the user and say 
                // Group by groups, drop to user, then to event?
                // Do it for next three months?
                for (DateTime m = DateTime.Today; m < DateTime.Today.AddMonths(3); m = m.AddMonths(1))
                {
                    // First print the month name, then print a ul for the events
                    HtmlNode monthHeader = HtmlNode.CreateNode("<h2></h2>");
                    monthHeader.AddClass("month-header");
                    monthHeader.InnerHtml = DateTime.Today.Month.ToString("MMMM");
                    eventHolder.AppendChild(monthHeader);
                    HtmlNode monthEventsHolder = HtmlNode.CreateNode("<ul></ul>");
                    monthEventsHolder.AddClass("month-events");
                    int counter = 0;
                    foreach (Occurrence o in Event.PoolOrder(events, m, new DateTime(m.Year, m.Month, 1).AddMonths(1).AddDays(-1)))
                    {
                        // Pretty print the event
                        HtmlNode eventNode = HtmlNode.CreateNode("<li></li>");
                        eventNode.AddClass("event-record");
                        /* REPLACE WITH STRING MANAGER */
                        eventNode.InnerHtml = "<a href=\"" + o.Event.User.UserUrl + "\">" + HttpUtility.HtmlEncode(o.Event.User.UserName)
                                            + "</a> is celebrating " + HttpUtility.HtmlEncode(o.Event.Name)
                                            + " on " + HttpUtility.HtmlEncode(o.Date.ToString("M"));
                        if (counter > 4)
                        {
                            // Add hidden class
                            eventNode.AddClass("hidden");
                        }
                        if (counter == 5)
                        {
                            // Add down arrow
                            HtmlNode downArrow = HtmlNode.CreateNode("<i></i>");
                            downArrow.AddClass("fa fa-angle-down text-center event-expander");
                            monthEventsHolder.AppendChild(downArrow);
                        }
                        monthEventsHolder.AppendChild(eventNode);
                        counter++;
                    }
                }
                return dash.DocumentNode.OuterHtml;
                
                /* using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        // Get all events that have anything to do with anyone in any group with our user.
                        // look at template
                        cmd.CommandText = "SELECT groups_events.EventID, users.UserName, users.UserEmail, users.UserURL "
                                        + "FROM groups_events "
                                        + "INNER JOIN user_events ON groups_events.EventID = user_events.EventID "
                                        + "INNER JOIN users ON user_events.UserID = users.UserID "
                                        + "WHERE user_events.UserID IN ( "
                                            + "SELECT UserID "
                                            + "FROM gift_registry_db.groups_users "
                                            + "WHERE GroupID IN ( "
                                                + "SELECT GroupID "
                                                + "FROM gift_registry_db.groups_users "
                                                + "WHERE gift_registry_db.groups_users.UserID = @uid "
                                            + ") "
                                        + ") "
                                        + "AND groups_events.GroupID IN ( "
                                            + "SELECT GroupID "
                                            + "FROM gift_registry_db.groups_users "
                                            + "WHERE GroupID IN ( "
                                                + "SELECT GroupID "
                                                + "FROM gift_registry_db.groups_users "
                                                + "WHERE gift_registry_db.groups_users.UserID = @uid "
                                            + ") "
                                        + ") "
                                        + "ORDER BY user_events.EventName;";
                        cmd.Parameters.AddWithValue("@uid", user.UserId);
                        cmd.Prepare();
                        int eventNum = 0;
                        using (MySqlDataReader Reader = cmd.ExecuteReader())
                        {
                            while (Reader.Read())
                            {
                                // Soonest is first. Loop until we reach a different event!
                                string eName = Convert.ToString(Reader["EventName"]);
                                // Create HtmlNode for this event:
                                // <li><h5>(ENAME) - (MM/DD) (CHEVRON_RIGHT)</h5><ul></ul></li>
                                HtmlNode eNode = HtmlNode.CreateNode(@"<li><h5 class=""event-header menu-dropdown"" data-toggle=""collapse"" data-target=""#EventNumber" + eventNum + @""">"
                                                                    + HttpUtility.HtmlEncode(Convert.ToString(Reader["EventName"]))
                                                                    + " - ("
                                                                    + Convert.ToInt32(Reader["EventMonth"]) + "/"
                                                                    + Convert.ToInt32(Reader["EventDay"])
                                                                    + @") <span class=""glyphicon glyphicon-chevron-right""></span></h5></li>");
                                HtmlNode users = HtmlNode.CreateNode(@"<ul id=""EventNumber" + eventNum + @""" class=""collapse""></ul>");
                                eNode.AppendChild(users);
                                while (eName.Equals(Convert.ToString(Reader["EventName"])))
                                {
                                    // Add HtmlNode for this person:
                                    // <li><a>(NAME) (ARROW_RIGHT)</a></li>
                                    HtmlNode userInfo = HtmlNode.CreateNode(@"<li><a href=""" + Constants.URL + "?dest=list&user=" + Convert.ToString(Reader["UserURL"]) + @""">"
                                                                            + HttpUtility.HtmlEncode(Convert.ToString(Reader["UserName"])) + " "
                                                                            + @"<span class=""glyphicon glyphicon-arrow-right""></span></a></li>");
                                    users.AppendChild(userInfo);
                                    if (!Reader.Read())
                                    {
                                        break;
                                    }
                                }
                                eventHolder.AppendChild(eNode);
                                eventNum++;
                            }
                        }
                    }
                }
                return dash.DocumentNode.OuterHtml;
                */
            }
            /// <summary>
            /// Creates a new dashboard for the given user
            /// </summary>
            /// <param name="user">The viewer</param>
            /// <returns>A New HTML dashboard</returns>
            public string UpdateEvents(User user)
            {
                return UpdateEvents(user, NavigationManager.NavigationBar(user) + ResourceManager.GetString("dashboard"));
            }
            /// <summary>
            /// Updates the Feed for this user
            /// </summary>
            /// <param name="user">The viewer</param>
            /// <param name="page">The current dashboard page</param>
            /// <returns>Updated Feed HTML</returns>
            public string UpdateFeed(User user, string page)
            {
                // TODO
                return page;
            }
            /// <summary>
            /// Creates a new dashboard and updates the feed
            /// </summary>
            /// <param name="user"></param>
            /// <returns>Dashboard HTML with feed</returns>
            public string UpdateFeed(User user)
            {
                return UpdateFeed(user, NavigationManager.NavigationBar(user) + ResourceManager.GetString("dashboard"));
            }
            /// <summary>
            /// Creates a new dashboard for the given user and updates feed and events
            /// </summary>
            /// <param name="user">The viewer</param>
            /// <returns>Complete dashboard HTML</returns>
            public string Dashboard(User user)
            {
                return UpdateFeed(user, UpdateEvents(user));
            }
        }
    }
}
