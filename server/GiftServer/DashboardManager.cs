using System;
using HtmlAgilityPack;
using GiftServer.Properties;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Web;

namespace GiftServer
{
    namespace Html
    {
        public class DashboardManager
        {
            public static string UpdateEvents(long userID, string page)
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
                        cmd.CommandText = "SELECT eventsusersgroups.EventUserID, users.FirstName, users.LastName, users.UserEmail, "
                                        + "eventsusers.EventName, eventsusers.EventDay, eventsusers.EventMonth, eventsusers.EventYear, eventsusers.EventRecurs, eventsusers.EventDescription "
                                        + "FROM eventsusersgroups "
                                        + "INNER JOIN eventsusers ON eventsusersgroups.EventUserID = eventsusers.EventUserID "
                                        + "INNER JOIN users ON eventsusers.UserID = users.UserID "
                                        + "WHERE eventsusers.UserID IN ( "
                                            + "SELECT UserID "
                                            + "FROM gift_registry_db.groupsusers "
                                            + "WHERE GroupID IN ( "
                                                + "SELECT GroupID "
                                                + "FROM gift_registry_db.groupsusers "
                                                + "WHERE gift_registry_db.groupsusers.UserID = @uid "
                                            + ") "
                                        + ") "
                                        + "AND eventsusersgroups.GroupID IN ( "
                                            + "SELECT GroupID "
                                            + "FROM gift_registry_db.groupsusers "
                                            + "WHERE GroupID IN ( "
                                                + "SELECT GroupID "
                                                + "FROM gift_registry_db.groupsusers "
                                                + "WHERE gift_registry_db.groupsusers.UserID = @uid "
                                            + ") "
                                        + ") "
                                        + "ORDER BY eventsusers.EventMonth ASC, eventsusers.EventDay ASC, eventsusers.EventName;";
                        cmd.Parameters.AddWithValue("@uid", userID);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Soonest is first. Loop until we reach a different event!
                                string eName = Convert.ToString(reader["EventName"]);
                                // Create HtmlNode for this event:
                                // <li><h5>(ENAME) - (MM/DD)</h5><ul></ul></li>
                                HtmlNode eNode = HtmlNode.CreateNode("<li><h5>" + HttpUtility.HtmlEncode(Convert.ToString(reader["EventName"]))
                                                                    + " - ("
                                                                    + Convert.ToInt32(reader["EventMonth"]) + "/"
                                                                    + Convert.ToInt32(reader["EventDay"]) + ")<span class=\"glyphicon glyphicon-chevron-right\"></span></h5></li>");
                                HtmlNode users = HtmlNode.CreateNode("<ul></ul>");
                                eNode.AppendChild(users);
                                while (eName.Equals(Convert.ToString(reader["EventName"])))
                                {
                                    // Add HtmlNode for this person:
                                    // <li><a>(FIRSTNAME) (LASTNAME) (CHEVRON_RIGHT)</a></li>
                                    HtmlNode userInfo = HtmlNode.CreateNode("<li><a href=\"\">"
                                                                            + HttpUtility.HtmlEncode(Convert.ToString(reader["FirstName"])) + " "
                                                                            + HttpUtility.HtmlEncode(Convert.ToString(reader["LastName"])) + " "
                                                                            + "<span class=\"glyphicon glyphicon-chevron-right\"></span></a></li>");
                                    users.AppendChild(userInfo);
                                    if (!reader.Read())
                                    {
                                        break;
                                    }
                                }
                                eventHolder.AppendChild(eNode);
                            }
                        }
                    }
                }
                return dash.DocumentNode.OuterHtml;
            }
            public static string UpdateEvents(long userID)
            {
                return UpdateEvents(userID, Resources.header + Resources.navigationBar + Resources.dashboard);
            }

            public static string UpdateFeed(long userID)
            {
                return UpdateFeed(userID, Resources.header + Resources.navigationBar + Resources.dashboard);
            }
            public static string UpdateFeed(long userID, string page)
            {
                return page;
            }

            public static string Dashboard(long userID)
            {
                string page = UpdateEvents(userID);
                page = UpdateFeed(userID, page);
                return page;
            }
        }
    }
}
