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
                events.RemoveAll(e => e.GetNearestOccurrence(DateTime.Today) == null);
                events = events.Distinct().OrderBy(e => e.GetNearestOccurrence(DateTime.Today)).ToList();

                // For each event (up to some amount), list the name of the user and say 
                // Group by groups, drop to user, then to event?
                // Do it for next three months?
                DateTime low = DateTime.Today;
                DateTime stop = new DateTime(low.Year, low.Month, 1).AddMonths(3);
                for (DateTime m = new DateTime(low.Year, low.Month, 1); m < stop; m = m.AddMonths(1))
                {
                    // First print the month name, then print a ul for the events
                    HtmlNode monthHeader = HtmlNode.CreateNode("<h2></h2>");
                    monthHeader.AddClass("month-header");
                    monthHeader.InnerHtml = m.ToString("MMMM");
                    HtmlNode monthExpander = HtmlNode.CreateNode("<i></i>");
                    monthExpander.AddClass("fa fa-angle-left month-expander");
                    monthHeader.AppendChild(monthExpander);
                    eventHolder.AppendChild(monthHeader);
                    HtmlNode monthEventsHolder = HtmlNode.CreateNode("<ul></ul>");
                    monthEventsHolder.AddClass("month-events");
                    int counter = 0;
                    foreach (Occurrence o in Event.PoolOrder(events, m > low ? m : low, new DateTime(m.Year, m.Month, 1).AddMonths(1).AddDays(-1)))
                    {
                        // Pretty print the event
                        HtmlNode eventNode = HtmlNode.CreateNode("<li></li>");
                        eventNode.AddClass("event-record");
                        /* REPLACE WITH STRING MANAGER */
                        eventNode.InnerHtml = "<a href=\"" + Constants.URL + "/?dest=list&user=" + o.Event.User.UserUrl + "\">" + HttpUtility.HtmlEncode(o.Event.User.UserName)
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
                    eventHolder.AppendChild(monthEventsHolder);
                }
                return dash.DocumentNode.OuterHtml;
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
