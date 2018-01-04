using GiftServer.Properties;
using GiftServer.Data;
using HtmlAgilityPack;
using System.Resources;
using GiftServer.Server;
using System.Threading;
using System.Web;

namespace GiftServer
{
    namespace Html
    {
        public class NavigationManager
        {
            ResourceManager ResourceManager;
            public NavigationManager(Controller controller)
            {
                Thread.CurrentThread.CurrentUICulture = controller.Culture;
                Thread.CurrentThread.CurrentCulture = controller.Culture;
                ResourceManager = new ResourceManager("GiftServer.HtmlTemplates", typeof(NavigationManager).Assembly);
            }
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
                    HtmlNode groupMenu = HtmlNode.CreateNode("<li class=\"dropdown-submenu\"></li>");

                    HtmlNode groupName = HtmlNode.CreateNode("<p></p>");
                    groupName.InnerHtml = HttpUtility.HtmlEncode(group.Name) + " <span class=\"glyphicon glyphicon-arrow-right text-right\"></span>";
                    groupMenu.AppendChild(groupName);

                    HtmlNode users = HtmlNode.CreateNode("<ul class=\"dropdown-menu\"></ul>");
                    foreach (User member in group.Users)
                    {
                        // Add user to navbar
                        if (user.UserId != member.UserId)
                        {
                            HtmlNode userNode = HtmlNode.CreateNode("<li></li>");
                            HtmlNode userLink = HtmlNode.CreateNode("<a></a>");
                            userLink.SetAttributeValue("href", Constants.URL + "/?dest=user&user=" + user.UserUrl);
                            userLink.InnerHtml = HttpUtility.HtmlEncode(member.UserName);
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
