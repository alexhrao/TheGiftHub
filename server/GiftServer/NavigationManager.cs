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
                            userLink.SetAttributeValue("href", Constants.URL + "/?dest=user&user=" + member.UserUrl);
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
        }
    }
}
