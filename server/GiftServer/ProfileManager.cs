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
        public static class ProfileManager
        {
            public static string ProfilePage(User user)
            {
                // Add Side Navigation Bar (From Dashboard)
                HtmlDocument profile = new HtmlDocument();
                profile.LoadHtml(NavigationManager.NavigationBar(user) + Resources.profile);
                // Set src of image:
                HtmlNode img = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userImage \")]");
                img.Attributes["src"].Value = user.GetImage();
                HtmlNode name = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userName \")]");
                name.InnerHtml = HttpUtility.HtmlEncode(user.firstName + " " + user.lastName);
                HtmlNode timeMember = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" timeMember \")]");
                timeMember.InnerHtml = HttpUtility.HtmlEncode("Member since " + user.dateJoined.ToString("MMMM d, yyyy"));
                HtmlNode email = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" email \")]");
                email.InnerHtml = HttpUtility.HtmlEncode("Email: " + user.email);
                HtmlNode id = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@name), \" \"), \" userID \")]");
                id.Attributes["value"].Value = user.Id.ToString();
                if (user.birthMonth != 0)
                {
                    DateTime dob = new DateTime(1999, user.birthMonth, user.birthDay);
                    HtmlNode birthday = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" birthday \")]");
                    birthday.InnerHtml = HttpUtility.HtmlEncode("Birthday: " + dob.ToString("MMMM d"));
                }
                else
                {
                    HtmlNode birthday = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" birthday \")]");
                    birthday.InnerHtml = HttpUtility.HtmlEncode("Birthday: " + "Not Set");
                }
                HtmlNode theme = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" theme \")]");
                switch (user.theme)
                {
                    case 1:
                        theme.Attributes["style"].Value = "background: red;";
                        break;
                    case 2:
                        theme.Attributes["style"].Value = "background: blue;";
                        break;
                    default:
                        theme.Attributes["style"].Value = "background: red;";
                        break;
                }
                HtmlNode bio = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" bio \")]");
                bio.InnerHtml = HttpUtility.HtmlEncode(user.bio);

                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    // Get events
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT EventUserID FROM events_users WHERE UserID = @id;";
                        cmd.Parameters.AddWithValue("@id", user.Id);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            HtmlNode events = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" events \")]");
                            while (reader.Read())
                            {
                                EventUser evnt = new EventUser(Convert.ToInt64(reader["EventUserID"]));
                                HtmlNode eventEntry = HtmlNode.CreateNode("<tr id=\"event" + evnt.EventUserID + "\"><td><h3><span id=\"eventCloser" + evnt.EventUserID + "\" class=\"glyphicon glyphicon-remove event-closer\"></span></h3></td>"
                                                                        + "<td class=\"event-name\"><h3>" + HttpUtility.HtmlEncode(evnt.Name) + " </h3></td></tr>");
                                events.AppendChild(eventEntry);
                            }
                        }
                    }
                    // Get groups
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT GroupID FROM groups_users WHERE groups_users.UserID = @id;";
                        cmd.Parameters.AddWithValue("@id", user.Id);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // For each one, create a group
                            HtmlNode groups = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" groups \")]");
                            while (reader.Read())
                            {
                                // Create a group
                                Group group = new Group(Convert.ToInt64(reader["GroupID"]));
                                HtmlNode groupEntry = HtmlNode.CreateNode("<tr id=\"group" + group.GroupID + "\"><td><h3><span id=\"groupCloser" + group.GroupID + "\" class=\"glyphicon glyphicon-remove group-closer\"></span></h3></td>"
                                                                        + "<td class=\"group-name\"><h3>" + HttpUtility.HtmlEncode(group.Name) + " </h3></td></tr>");
                                groups.AppendChild(groupEntry);
                            }
                        }
                    }
                    
                }
                return profile.DocumentNode.OuterHtml;
            }
        }
    }
}