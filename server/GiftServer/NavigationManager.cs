using GiftServer.Properties;
using System;
using GiftServer.Data;
using HtmlAgilityPack;
using System.Resources;

namespace GiftServer
{
    namespace Html
    {
        public class NavigationManager
        {
            public static string NavigationBar(User user)
            {
                HtmlDocument bar = new HtmlDocument();
                ResourceManager resource = new ResourceManager("GiftServer.HtmlTemplates", typeof(NavigationManager).Assembly);
                bar.LoadHtml(resource.GetString("header") + resource.GetString("navigationBar"));
                HtmlNode logo = bar.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" navbar-brand \")]");
                logo.Attributes["href"].Value = Constants.URL;
                return bar.DocumentNode.OuterHtml;
            }
        }
    }
}
