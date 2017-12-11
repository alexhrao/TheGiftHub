﻿using GiftServer.Properties;
using System;
using GiftServer.Data;
using HtmlAgilityPack;
namespace GiftServer
{
    namespace Html
    {
        public class NavigationManager
        {
            public static string NavigationBar(User user)
            {
                HtmlDocument bar = new HtmlDocument();
                bar.LoadHtml(HtmlTemplates.header + HtmlTemplates.navigationBar);
                HtmlNode logo = bar.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" navbar-brand \")]");
                logo.Attributes["href"].Value = HtmlTemplates.URL;
                return bar.DocumentNode.OuterHtml;
            }
        }
    }
}
