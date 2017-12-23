using System;
using HtmlAgilityPack;
using GiftServer.Properties;
using GiftServer.Data;
using System.Web;
using System.Resources;
using GiftServer.Server;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Threading;

namespace GiftServer
{
    namespace Html
    {
        public class ProfileManager
        {
            private ResourceManager ResourceManager;
            private NavigationManager NavigationManager;
            public ProfileManager(Controller controller)
            {
                Thread.CurrentThread.CurrentUICulture = controller.Culture;
                Thread.CurrentThread.CurrentCulture = controller.Culture;
                ResourceManager = new ResourceManager("GiftServer.HtmlTemplates", typeof(ProfileManager).Assembly);
                NavigationManager = controller.NavigationManager;
            }
            public string ProfilePage(User user)
            {
                // Add Side Navigation Bar (From Dashboard)
                HtmlDocument profile = new HtmlDocument();
                profile.LoadHtml(NavigationManager.NavigationBar(user) + ResourceManager.GetString("profile"));
                // Set src of image:
                HtmlNode img = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userImage \")]");
                img.Attributes["src"].Value = user.GetImage();
                HtmlNode name = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userName \")]");
                name.InnerHtml = HttpUtility.HtmlEncode(user.UserName);
                HtmlNode timeMember = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" timeMember \")]");
                timeMember.InnerHtml = HttpUtility.HtmlEncode("Member since " + user.DateJoined.ToString("m") + ", " + user.DateJoined.ToString("yyyy"));
                HtmlNode email = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" email \")]");
                email.InnerHtml = HttpUtility.HtmlEncode("Email: " + user.Email);
                HtmlNode id = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@name), \" \"), \" userID \")]");
                id.Attributes["value"].Value = user.UserId.ToString();
                if (user.BirthMonth != 0)
                {
                    DateTime dob = new DateTime(1999, user.BirthMonth, user.BirthDay);
                    HtmlNode birthday = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" birthday \")]");
                    birthday.InnerHtml = HttpUtility.HtmlEncode("Birthday: " + dob.ToString("m"));
                }
                else
                {
                    HtmlNode birthday = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" birthday \")]");
                    birthday.InnerHtml = HttpUtility.HtmlEncode("Birthday: " + "Not Set");
                }
                /*
                HtmlNode theme = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" theme \")]");
                switch (user.Theme)
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
                */
                HtmlNode cultures = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userCulture \")]");
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT * FROM cultures ORDER BY CultureDesc ASC;";
                        cmd.Prepare();
                        bool isFound = false;
                        using (MySqlDataReader Reader = cmd.ExecuteReader())
                        {
                            while (Reader.Read())
                            {
                                // Create option node
                                HtmlNode culture;
                                string cultureCode = Convert.ToString(Reader["CultureLanguage"]) + "-" + Convert.ToString(Reader["CultureLocation"]);
                                if (!isFound && cultureCode.ToLower().Equals(user.Preferences.Culture.ToLower()))
                                {
                                    culture = HtmlNode.CreateNode("<option selected value=\"" + cultureCode + "\"></option>");
                                    isFound = true;
                                }
                                else
                                {
                                    culture = HtmlNode.CreateNode("<option value=\"" + cultureCode + "\"></option>");
                                }
                                culture.InnerHtml = HttpUtility.HtmlEncode(Convert.ToString(Reader["CultureDesc"]));
                                cultures.AppendChild(culture);
                            }
                        }
                    }
                }

                HtmlNode bio = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" bio \")]");
                bio.InnerHtml = HttpUtility.HtmlEncode(user.Bio);

                HtmlNode events = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" events \")]");
                foreach (EventUser evnt in user.Events)
                {
                    HtmlNode eventEntry = HtmlNode.CreateNode("<tr id=\"event" + evnt.EventUserId + "\"><td><h3><span id=\"eventCloser" + evnt.EventUserId + "\" class=\"glyphicon glyphicon-remove event-closer\"></span></h3></td>"
                                                            + "<td class=\"event-name\"><h3>" + HttpUtility.HtmlEncode(evnt.Name) + " </h3></td></tr>");
                    events.AppendChild(eventEntry);
                }
                HtmlNode addEvent = HtmlNode.CreateNode("<tr id=\"eventAdder\" data-toggle=\"modal\" href=\"#addEvent\"><td><h3><span class=\"glyphicon glyphicon-plus\"></span></h3></td><td></td></tr>");
                events.AppendChild(addEvent);

                HtmlNode groups = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" groups \")]");
                foreach (Group group in user.Groups)
                {
                    HtmlNode groupEntry = HtmlNode.CreateNode("<tr id=\"group" + group.GroupId + "\"><td><h3><span id=\"groupCloser" + group.GroupId + "\" class=\"glyphicon glyphicon-remove group-closer\"></span></h3></td>"
                                               + "<td class=\"group-name\"><h3>" + HttpUtility.HtmlEncode(group.Name) + " </h3></td></tr>");
                    groups.AppendChild(groupEntry);
                }
                HtmlNode addGroup = HtmlNode.CreateNode("<tr id=\"groupAdder\" type=\"button\" data-toggle=\"modal\" href=\"#addGroup\"><td><h3><span class=\"glyphicon glyphicon-plus\"></span></h3></td><td></td></tr>");
                groups.AppendChild(addGroup);
                return profile.DocumentNode.OuterHtml;
            }
        }
    }
}