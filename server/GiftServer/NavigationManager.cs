using GiftServer.Properties;
using System;
using GiftServer.Data;
using HtmlAgilityPack;
using System.Resources;
using GiftServer.Server;
using System.Threading;

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
                return bar.DocumentNode.OuterHtml;
            }
        }
    }
}
