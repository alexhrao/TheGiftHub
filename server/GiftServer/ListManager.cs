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
                    HtmlNode giftRow = HtmlNode.CreateNode("<tr></tr>");
                    HtmlNode pic = HtmlNode.CreateNode("<td><img class=\"img-thumbnail img-responsive centered\" src=\"" + gift.GetImage() + "\" /></td>");
                    HtmlNode rating = HtmlNode.CreateNode("<td><h3>TODO: Create Rating</h3></td>");
                    HtmlNode name = HtmlNode.CreateNode("<td><h3>" + gift.Name + "</h3></td>");
                    HtmlNode quant = HtmlNode.CreateNode("<td><h3>" + gift.Quantity + "</h3></td>");
                    HtmlNode cost = HtmlNode.CreateNode("<td><h3>" + gift.Cost.ToString("C") + "</h3></td>");
                    giftRow.AppendChild(pic);
                    giftRow.AppendChild(rating);
                    giftRow.AppendChild(name);
                    giftRow.AppendChild(quant);
                    giftRow.AppendChild(cost);

                    giftTable.AppendChild(giftRow);
                }
                // Get all gifts associated with me

                return myList.DocumentNode.OuterHtml;
            }
        }
    }
}