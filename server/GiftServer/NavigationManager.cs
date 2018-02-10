using GiftServer.Properties;
using GiftServer.Data;
using HtmlAgilityPack;
using System.Resources;
using GiftServer.Server;
using System.Threading;
using System.Web;

namespace GiftServer
{
    namespace HtmlManager
    {
        /// <summary>
        /// Manages the Navigation Bar (at the top of almost every screen)
        /// </summary>
        public class NavigationManager
        {
            ResourceManager ResourceManager;
            /// <summary>
            /// Creates a new NavigationManager with the given controller
            /// </summary>
            /// <param name="controller">The settings for this thread</param>
            public NavigationManager(Controller controller)
            {
                Thread.CurrentThread.CurrentUICulture = controller.Culture;
                Thread.CurrentThread.CurrentCulture = controller.Culture;
                ResourceManager = new ResourceManager("GiftServer.HtmlTemplates", typeof(NavigationManager).Assembly);
            }
            /// <summary>
            /// Create a complete navigation bar for this User.
            /// </summary>
            /// <param name="user">The viewer</param>
            /// <returns>Navigation Bar HTML</returns>
            public string NavigationBar(User user)
            {
                HtmlDocument bar = new HtmlDocument();
                bar.LoadHtml(ResourceManager.GetString("header") + ResourceManager.GetString("navigationBar"));
                HtmlNode logo = bar.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" navbar-brand \")]");
                logo.Attributes["href"].Value = Constants.URL;
                // Get all groups attached to this user:
                HtmlNode groupHolder = bar.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" groupHolder \")]");
                foreach (Group group in user.Groups)
                {
                    HtmlNode groupMenu = HtmlNode.CreateNode("<li></li>");
                    groupMenu.AddClass("dropdown-submenu");
                    HtmlNode groupName = HtmlNode.CreateNode("<p></p>");
                    HtmlNode rightArrow = HtmlNode.CreateNode("<i></i>");
                    rightArrow.AddClass("fas fa-long-arrow-alt-right text-right");
                    groupName.InnerHtml = HttpUtility.HtmlEncode(group.Name);
                    groupMenu.AppendChild(groupName);
                    groupMenu.AppendChild(rightArrow);
                    HtmlNode users = HtmlNode.CreateNode("<ul></ul>");
                    users.AddClass("dropdown-menu");
                    foreach (User member in group.Users)
                    {
                        // Add user to navbar
                        if (user.ID != member.ID)
                        {
                            HtmlNode userNode = HtmlNode.CreateNode("<li></li>");
                            HtmlNode userLink = HtmlNode.CreateNode("<a></a>");
                            userLink.SetAttributeValue("href", Constants.URL + "/?dest=user&user=" + member.Url);
                            userLink.InnerHtml = HttpUtility.HtmlEncode(member.Name);
                            userNode.AppendChild(userLink);
                            users.AppendChild(userNode);
                        }
                    }
                    groupMenu.AppendChild(users);

                    groupHolder.AppendChild(groupMenu);
                }
                return bar.DocumentNode.OuterHtml;
            }
            /// <summary>
            /// Create the sidebar navigator for the user
            /// </summary>
            /// <param name="navigator">The navigator node</param>
            /// <param name="user">The user viewing it</param>
            /// <returns>The modified HtmlNode</returns>
            public HtmlNode Navigator(HtmlNode navigator, User user)
            {
                navigator.RemoveAll();
                navigator.AddClass("panel");

                HtmlNode heading = HtmlNode.CreateNode("<div></div>");
                heading.AddClass("text-center panel-heading");
                heading.InnerHtml = "Quick Links";
                navigator.AppendChild(heading);

                HtmlNode body = HtmlNode.CreateNode("<div></div>");
                body.AddClass("panel-body");

                HtmlNode nav = HtmlNode.CreateNode("<ul></ul>");
                nav.AddClass("nav nav-pills nav-stacked");

                HtmlNode dash = HtmlNode.CreateNode("<li></li>");
                HtmlNode dashLink = HtmlNode.CreateNode("<a></a>");
                dashLink.Attributes.Add("href", ".?dest=dashboard");
                dashLink.Id = "dashboardLink";
                dashLink.InnerHtml = "Dashboard";
                dash.AppendChild(dashLink);
                nav.AppendChild(dash);

                HtmlNode profile = HtmlNode.CreateNode("<li></li>");
                HtmlNode profileLink = HtmlNode.CreateNode("<a></a>");
                profileLink.Attributes.Add("href", ".?dest=profile");
                profileLink.Id = "profileLink";
                profileLink.InnerHtml = "My Account";
                profile.AppendChild(profileLink);
                nav.AppendChild(profile);

                HtmlNode list = HtmlNode.CreateNode("<li></li>");
                HtmlNode listLink = HtmlNode.CreateNode("<a></a>");
                listLink.Attributes.Add("href", ".?dest=myList");
                listLink.InnerHtml = "My List";
                listLink.Id = "listLink";
                list.AppendChild(listLink);
                nav.AppendChild(list);

                HtmlNode res = HtmlNode.CreateNode("<li></li>");
                HtmlNode resLink = HtmlNode.CreateNode("<a></a>");
                resLink.Attributes.Add("href", ".?dest=reservations");
                resLink.InnerHtml = "My Reservations";
                resLink.Id = "reservationsLink";
                res.AppendChild(resLink);
                nav.AppendChild(res);

                // Groups ?
                body.AppendChild(nav);
                navigator.AppendChild(body);
                return navigator;
            }
        }
    }
}
