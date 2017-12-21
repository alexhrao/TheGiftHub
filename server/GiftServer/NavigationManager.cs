using GiftServer.Properties;
using System;
using GiftServer.Data;
using HtmlAgilityPack;
using System.Resources;
using GiftServer.Server;

namespace GiftServer
{
    namespace Html
    {
        public class NavigationManager
        {
            ResourceManager ResourceManager;
            public NavigationManager(Controller controller)
            {
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
