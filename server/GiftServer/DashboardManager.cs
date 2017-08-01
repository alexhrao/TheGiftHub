using System;
using HtmlAgilityPack;
using GiftServer.Properties;
using MySql.Data.MySqlClient;
using System.Configuration;

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
                HtmlNode eventHolder = dash.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                eventHolder.RemoveAllChildren();
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        // Get all events that have anything to do with anyone in any group with our user.
                        // look at template
                        cmd.CommandText = "SELECT eventsusersgroups.EventUserID "
                                        + "FROM eventsusersgroups "
                                        + "INNER JOIN eventsusers ON eventsusersgroups.EventUserID = eventsusers.EventUserID "
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
                                        + ");";
                        cmd.Parameters.AddWithValue("@uid", userID);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Add HtmlNode to our document?
                            }
                        }
                    }
                }
                return page;
            }
            public static string UpdateEvents(long userID)
            {
                return UpdateEvents(userID, Resources.dashboard);
            }
            private static string getEvent(long eventID)
            {
                return "";
            }
        }
    }
}
