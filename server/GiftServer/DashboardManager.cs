using System;
using HtmlAgilityPack;
using GiftServer.Properties;
using GiftServer.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Web;

namespace GiftServer
{
    namespace Html
    {
        public class DashboardManager
        {
            public static string UpdateEvents(User user, string page)
            {
                HtmlDocument dash = new HtmlDocument();
                dash.LoadHtml(page);
                HtmlNode eventHolder = dash.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" eventHolder \")]");
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        // Get all events that have anything to do with anyone in any group with our user.
                        // look at template
                        cmd.CommandText = "SELECT events_users_groups.EventUserID, users.FirstName, users.LastName, users.UserEmail, "
                                        + "events_users.EventName, events_users.EventDay, events_users.EventMonth, events_users.EventYear, events_users.EventRecurs, events_users.EventDescription "
                                        + "FROM events_users_groups "
                                        + "INNER JOIN events_users ON events_users_groups.EventUserID = events_users.EventUserID "
                                        + "INNER JOIN users ON events_users.UserID = users.UserID "
                                        + "WHERE events_users.UserID IN ( "
                                            + "SELECT UserID "
                                            + "FROM gift_registry_db.groups_users "
                                            + "WHERE GroupID IN ( "
                                                + "SELECT GroupID "
                                                + "FROM gift_registry_db.groups_users "
                                                + "WHERE gift_registry_db.groups_users.UserID = @uid "
                                            + ") "
                                        + ") "
                                        + "AND events_users_groups.GroupID IN ( "
                                            + "SELECT GroupID "
                                            + "FROM gift_registry_db.groups_users "
                                            + "WHERE GroupID IN ( "
                                                + "SELECT GroupID "
                                                + "FROM gift_registry_db.groups_users "
                                                + "WHERE gift_registry_db.groups_users.UserID = @uid "
                                            + ") "
                                        + ") "
                                        + "ORDER BY events_users.EventMonth ASC, events_users.EventDay ASC, events_users.EventName;";
                        cmd.Parameters.AddWithValue("@uid", user.Id);
                        cmd.Prepare();
                        int eventNum = 0;
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Soonest is first. Loop until we reach a different event!
                                string eName = Convert.ToString(reader["EventName"]);
                                // Create HtmlNode for this event:
                                // <li><h5>(ENAME) - (MM/DD) (CHEVRON_RIGHT)</h5><ul></ul></li>
                                HtmlNode eNode = HtmlNode.CreateNode("<li><h5 id=\"menu-dropdown\" data-toggle=\"collapse\" data-target=\"#EventNumber" + eventNum + "\">"
                                                                    + HttpUtility.HtmlEncode(Convert.ToString(reader["EventName"]))
                                                                    + " - ("
                                                                    + Convert.ToInt32(reader["EventMonth"]) + "/"
                                                                    + Convert.ToInt32(reader["EventDay"])
                                                                    + ") <span class=\"glyphicon glyphicon-chevron-right\"></span></h5></li>");
                                HtmlNode users = HtmlNode.CreateNode("<ul id=\"EventNumber" + eventNum + "\" class=\"collapse\"></ul>");
                                eNode.AppendChild(users);
                                while (eName.Equals(Convert.ToString(reader["EventName"])))
                                {
                                    // Add HtmlNode for this person:
                                    // <li><a>(FIRSTNAME) (LASTNAME) (ARROW_RIGHT)</a></li>
                                    HtmlNode userInfo = HtmlNode.CreateNode("<li><a href=\"\">"
                                                                            + HttpUtility.HtmlEncode(Convert.ToString(reader["FirstName"])) + " "
                                                                            + HttpUtility.HtmlEncode(Convert.ToString(reader["LastName"])) + " "
                                                                            + "<span class=\"glyphicon glyphicon-arrow-right\"></span></a></li>");
                                    users.AppendChild(userInfo);
                                    if (!reader.Read())
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
            }
            public static string UpdateEvents(User user)
            {
                return UpdateEvents(user, NavigationManager.NavigationBar(user) + Resources.dashboard);
            }

            public static string UpdateFeed(User user, string page)
            {
                return page;
            }
            public static string UpdateFeed(User user)
            {
                return UpdateFeed(user, NavigationManager.NavigationBar(user) + Resources.dashboard);
            }

            public static string Dashboard(User user)
            {
                string page = UpdateEvents(user);
                page = UpdateFeed(user, page);
                return page;
            }
        }
    }
}
