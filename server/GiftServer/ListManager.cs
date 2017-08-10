using System;
using GiftServer.Data;
using GiftServer.Properties;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace GiftServer
{
    namespace Html
    {
        public static class ListManager
        {
            public static string MyList(User user)
            {
                HtmlDocument myList = new HtmlDocument();
                myList.LoadHtml(Resources.header + NavigationManager.NavigationBar(user) + Resources.list);
                HtmlNode giftTable = myList.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" giftHolder \")]");
                List<Gift> gifts = user.Gifts;
                foreach (Gift gift in gifts)
                {
                    // Print gift information
                    HtmlNode giftRow = HtmlNode.CreateNode("<tr><td>.....</td></tr>");
                    giftTable.AppendChild(giftRow);
                }
                // Get all gifts associated with me

                return myList.DocumentNode.OuterHtml;
            }
        }
    }
}