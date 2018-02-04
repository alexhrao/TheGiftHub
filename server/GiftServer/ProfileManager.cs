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
    namespace HtmlManager
    {
        /// <summary>
        /// Manages all HTML related to a user's Profile
        /// </summary>
        public class ProfileManager
        {
            private ResourceManager ResourceManager;
            private NavigationManager NavigationManager;
            /// <summary>
            /// Instantiate a new ProfileManager
            /// </summary>
            /// <param name="controller">The controller for this thread</param>
            public ProfileManager(Controller controller)
            {
                Thread.CurrentThread.CurrentUICulture = controller.Culture;
                Thread.CurrentThread.CurrentCulture = controller.Culture;
                ResourceManager = new ResourceManager("GiftServer.HtmlTemplates", typeof(ProfileManager).Assembly);
                NavigationManager = controller.NavigationManager;
            }
            /// <summary>
            /// Create a profile page for a user to be viewable by another
            /// </summary>
            /// <remarks>
            /// As with other methods, viewer is viewing the target's page
            /// </remarks>
            /// <param name="viewer">The viewer (usually, but not always, this)</param>
            /// <param name="target">The target</param>
            /// <returns>HTML for the viewable page</returns>
            public string ProfilePage(User viewer, User target)
            {
                HtmlDocument profile = new HtmlDocument();
                profile.LoadHtml(NavigationManager.NavigationBar(viewer) + ResourceManager.GetString("publicProfile"));
                // Set src of image:
                HtmlNode img = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userImage \")]");
                img.Attributes["src"].Value = target.GetImage();
                HtmlNode name = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userName \")]");
                name.InnerHtml = HttpUtility.HtmlEncode(target.UserName);
                HtmlNode timeMember = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" timeMember \")]");
                timeMember.InnerHtml = HttpUtility.HtmlEncode("Member since " + target.DateJoined.ToString("m") + ", " + target.DateJoined.ToString("yyyy"));
                HtmlNode listLink = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" listLink \")]");
                listLink.SetAttributeValue("href", Constants.URL + "?dest=list&user=" + target.UserUrl);
                HtmlNode email = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" email \")]");
                email.InnerHtml = HttpUtility.HtmlEncode("Email: " + target.Email);
                if (viewer.BirthMonth != 0)
                {
                    DateTime dob = new DateTime(2004, target.BirthMonth, target.BirthDay);
                    HtmlNode birthday = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" birthday \")]");
                    birthday.InnerHtml = HttpUtility.HtmlEncode("Birthday: " + dob.ToString("m"));
                }
                else
                {
                    HtmlNode birthday = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" birthday \")]");
                    birthday.InnerHtml = HttpUtility.HtmlEncode("Birthday: " + "Not Set");
                }

                HtmlNode bio = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" bio \")]");
                bio.InnerHtml = HttpUtility.HtmlEncode(target.Bio);

                HtmlNode events = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" events \")]");
                foreach (Event evnt in viewer.GetEvents(target))
                {
                    HtmlNode eventRow = HtmlNode.CreateNode("<tr></tr>");
                    eventRow.Attributes.Add("data-event-id", evnt.ID.ToString());
                    HtmlNode eventName = HtmlNode.CreateNode("<td></td>");
                    eventName.AddClass("event-name");
                    eventName.AppendChild(HtmlNode.CreateNode("<h3>" + HttpUtility.HtmlEncode(evnt.Name) + "</h3>"));
                    eventRow.AppendChild(HtmlNode.CreateNode("<td></td>"));
                    eventRow.AppendChild(eventName);
                    
                    events.AppendChild(eventRow);
                }

                HtmlNode groups = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" groups \")]");
                foreach (Group group in viewer.GetGroups(target))
                {
                    HtmlNode groupRow = HtmlNode.CreateNode("<tr></tr>");
                    groupRow.Attributes.Add("data-group-id", group.ID.ToString());
                    HtmlNode groupName = HtmlNode.CreateNode("<td></td>");
                    groupName.AddClass("group-name");
                    groupName.AppendChild(HtmlNode.CreateNode("<h3>" + HttpUtility.HtmlEncode(group.Name) + "</h3>"));
                    groupRow.AppendChild(HtmlNode.CreateNode("<td></td>"));
                    groupRow.AppendChild(groupName);
                    groups.AppendChild(groupRow);
                }
                return profile.DocumentNode.OuterHtml;
            }
            /// <summary>
            /// Generate a private profile page for the specified user
            /// </summary>
            /// <param name="user">The viewer (and target)</param>
            /// <returns>HTML markup for a privately viewable profile page</returns>
            public string ProfilePage(User user)
            {
                // Add Side Navigation Bar (From Dashboard)
                HtmlDocument profile = new HtmlDocument();
                profile.LoadHtml(NavigationManager.NavigationBar(user) + ResourceManager.GetString("profile"));
                // Set src of image:
                HtmlNode img = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userImage \")]");
                img.Attributes["src"].Value = user.GetImage();
                HtmlNode name = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userName \")]");
                // HtmlNode nameChange = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userNameChange \")]");
                // nameChange.SetAttributeValue("placeholder", HttpUtility.HtmlEncode(user.UserName));
                name.InnerHtml = HttpUtility.HtmlEncode(user.UserName);
                HtmlNode timeMember = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" timeMember \")]");
                timeMember.InnerHtml = HttpUtility.HtmlEncode("Member since " + user.DateJoined.ToString("m") + ", " + user.DateJoined.ToString("yyyy"));
                HtmlNode email = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" email \")]");
                // HtmlNode emailChange = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userEmailChange \")]");
                // emailChange.SetAttributeValue("placeholder", HttpUtility.HtmlEncode(user.Email));
                email.InnerHtml = HttpUtility.HtmlEncode("Email: " + user.Email.Address);
                email.Attributes.Add("data-user-email", user.Email.Address);
                HtmlNode id = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@name), \" \"), \" userID \")]");
                id.Attributes["value"].Value = user.ID.ToString();
                if (user.BirthMonth != 0)
                {
                    DateTime dob = new DateTime(2004, user.BirthMonth, user.BirthDay);
                    HtmlNode birthday = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" birthday \")]");
                    birthday.InnerHtml = HttpUtility.HtmlEncode("Birthday: " + dob.ToString("m"));
                }
                else
                {
                    HtmlNode birthday = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" birthday \")]");
                    birthday.InnerHtml = HttpUtility.HtmlEncode("Birthday: " + "Not Set");
                }
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
                                    culture = HtmlNode.CreateNode("<option></option>");
                                    culture.Attributes.Add("selected", "");
                                    culture.Attributes.Add("value", cultureCode);
                                    isFound = true;
                                }
                                else
                                {
                                    culture = HtmlNode.CreateNode("<option></option>");
                                    culture.Attributes.Add("value", cultureCode);
                                }
                                culture.InnerHtml = HttpUtility.HtmlEncode(Convert.ToString(Reader["CultureDesc"]));
                                cultures.AppendChild(culture);
                            }
                        }
                    }
                }

                HtmlNode bio = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" bio \")]");
                bio.InnerHtml = HttpUtility.HtmlEncode(user.Bio);
                HtmlNode bioChange = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userBioChange \")]");
                bioChange.InnerHtml = HttpUtility.HtmlEncode(user.Bio);

                HtmlNode facebookLoginStatus = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" facebookLoginStatus \")]");
                if (user.FacebookId != null)
                {
                    // Checkmark
                    facebookLoginStatus.RemoveAllChildren();
                    HtmlNode checkMark = HtmlNode.CreateNode("<span></span>");
                    checkMark.AddClass("fas fa-times oauth-confirmed");
                    checkMark.Id = "facebookConfirmed";
                    facebookLoginStatus.RemoveClass("text-center");
                    facebookLoginStatus.AppendChild(checkMark);
                }

                HtmlNode googleLoginStatus = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" googleLoginStatus \")]");
                if (user.GoogleId != null)
                {
                    // Checkmark
                    googleLoginStatus.RemoveAllChildren();
                    HtmlNode checkMark = HtmlNode.CreateNode("<span></span>");
                    checkMark.AddClass("fas fa-times oauth-confirmed");
                    checkMark.Id = "googleConfirmed";
                    googleLoginStatus.RemoveClass("text-center");
                    googleLoginStatus.AppendChild(checkMark);
                }



                // Fill groups:
                HtmlNode newEventGroupLeft = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" newEventGroups \")]").FirstChild;
                HtmlNode newEventGroupRight = newEventGroupLeft.NextSibling;
                bool alt = true;
                foreach (Group group in user.Groups)
                {
                    // Left
                    HtmlNode groupCheck = HtmlNode.CreateNode("<label></label>");
                    groupCheck.AddClass("checkbox-inline");
                    groupCheck.Attributes.Add("data-group-id", HttpUtility.HtmlEncode(group.ID));
                    HtmlNode check = HtmlNode.CreateNode("<input />");
                    check.AddClass("group-check");
                    check.Attributes.Add("type", "checkbox");
                    check.Attributes.Add("value", "");
                    check.Attributes.Add("data-group-id", HttpUtility.HtmlEncode(group.ID));
                    check.Attributes.Add("data-group-name", HttpUtility.HtmlEncode(group.Name));
                    groupCheck.AppendChild(check);
                    HtmlNode groupName = HtmlNode.CreateNode("<p></p>");
                    groupName.InnerHtml = HttpUtility.HtmlEncode(group.Name);
                    groupCheck.AppendChild(groupName);
                    // Alternate
                    if (alt)
                    {
                        newEventGroupLeft.AppendChild(groupCheck);
                    }
                    else
                    {
                        // Right
                        newEventGroupRight.AppendChild(groupCheck);
                    }
                    alt = !alt;
                }

                HtmlNode events = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" events \")]");
                foreach (Event evnt in user.Events)
                {
                    HtmlNode eventRow = HtmlNode.CreateNode("<tr></tr>");
                    eventRow.Attributes.Add("data-event-id", evnt.ID.ToString());
                    HtmlNode eventCloser = HtmlNode.CreateNode("<i></i>");
                    eventCloser.AddClass("event-closer fas fa-times");
                    eventCloser.Attributes.Add("data-event-id", evnt.ID.ToString());
                    HtmlNode eventCloserTd = HtmlNode.CreateNode("<td></td>");
                    eventCloserTd.AppendChild(eventCloser);
                    HtmlNode eventName = HtmlNode.CreateNode("<td></td>");
                    eventName.Attributes.Add("data-event-id", evnt.ID.ToString());
                    eventName.AddClass("event-name");
                    eventName.AppendChild(HtmlNode.CreateNode("<h3>" + HttpUtility.HtmlEncode(evnt.Name) + "</h3>"));
                    eventRow.AppendChild(eventCloserTd);
                    eventRow.AppendChild(eventName);

                    events.AppendChild(eventRow);
                }
                {
                    HtmlNode addRow = HtmlNode.CreateNode("<tr></tr>");
                    addRow.Id = "eventAdder";
                    addRow.Attributes.Add("data-toggle", "modal");
                    addRow.Attributes.Add("href", "#addEvent");
                    HtmlNode adder = HtmlNode.CreateNode("<i></i>");
                    adder.AddClass("fas fa-plus");
                    HtmlNode adderTd = HtmlNode.CreateNode("<td></td>");
                    adderTd.AppendChild(HtmlNode.CreateNode("<h3></h3>"));
                    adderTd.FirstChild.AppendChild(adder);
                    addRow.AppendChild(adderTd);
                    addRow.AppendChild(HtmlNode.CreateNode("<td></td>"));
                    events.AppendChild(addRow);
                }

                HtmlNode groups = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" groups \")]");
                foreach (Group group in user.Groups)
                {
                    HtmlNode groupRow = HtmlNode.CreateNode("<tr></tr>");
                    groupRow.Attributes.Add("data-group-id", group.ID.ToString());
                    HtmlNode groupCloser = HtmlNode.CreateNode("<i></i>");
                    groupCloser.AddClass("group-closer fas fa-times");
                    groupCloser.Attributes.Add("data-group-id", group.ID.ToString());
                    HtmlNode groupCloserTd = HtmlNode.CreateNode("<td></td>");
                    groupCloserTd.AppendChild(groupCloser);
                    HtmlNode groupName = HtmlNode.CreateNode("<td></td>");
                    groupName.AddClass("group-name");
                    groupName.Attributes.Add("data-group-id", group.ID.ToString());
                    groupName.AppendChild(HtmlNode.CreateNode("<h3>" + HttpUtility.HtmlEncode(group.Name) + "</h3>"));
                    groupRow.AppendChild(groupCloserTd);
                    groupRow.AppendChild(groupName);
                    groups.AppendChild(groupRow);
                }
                {
                    HtmlNode addRow = HtmlNode.CreateNode("<tr></tr>");
                    addRow.Id = "groupAdder";
                    addRow.Attributes.Add("data-toggle", "modal");
                    addRow.Attributes.Add("href", "#addGroup");
                    HtmlNode adder = HtmlNode.CreateNode("<i></i>");
                    adder.AddClass("fas fa-plus");
                    HtmlNode adderTd = HtmlNode.CreateNode("<td></td>");
                    adderTd.AppendChild(HtmlNode.CreateNode("<h3></h3>"));
                    adderTd.FirstChild.AppendChild(adder);
                    addRow.AppendChild(adderTd);
                    addRow.AppendChild(HtmlNode.CreateNode("<td></td>"));
                    groups.AppendChild(addRow);
                }
                return profile.DocumentNode.OuterHtml;
            }
        }
    }
}