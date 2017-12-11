using GiftServer.Data;
using GiftServer.Properties;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Web;

namespace GiftServer
{
    namespace Html
    {
        public static class ListManager
        {
            public static string GiftList(User user)
            {
                HtmlDocument myList = new HtmlDocument();
                myList.LoadHtml(NavigationManager.NavigationBar(user) + Resources.list);

                // Translate into correct culture

                HtmlNode giftTable = myList.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" giftHolder \")]");
                HtmlNode giftTableMicro = myList.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" xsGiftHolder \")]");
                List<Gift> gifts = user.Gifts;
                //int i = 0;
                foreach (Gift gift in gifts)
                {
                    // Print gift information
                    if (gift.DateReceived == DateTime.MinValue)
                    {
                        HtmlNode giftRow = HtmlNode.CreateNode("<tr id=\"" + gift.GiftId + "\" class=\"gift-row\"></tr>");

                        HtmlNode pict = HtmlNode.CreateNode("<td><div class=\"parent\"><img class=\"img-thumbnail img-gift img-responsive child\" src=\"" + gift.GetImage() + "\" /></div></td>");
                        HtmlNode rate = HtmlNode.CreateNode("<td><div class=\"parent\"><p class=\"child\"><input class=\"star-rating\" data-show-clear=\"false\" data-show-caption=\"false\" value=\"" + gift.Rating.ToString("N2") + "\" /></p></div></td>");
                        HtmlNode name = HtmlNode.CreateNode("<td><div class=\"parent\"><p class=\"child\">" + HttpUtility.HtmlEncode(gift.Name) + "</p></div></td>");
                        HtmlNode quan = HtmlNode.CreateNode("<td><div class=\"parent\"><p class=\"child\">" + gift.Quantity + "</p></div></td>");
                        HtmlNode cost = HtmlNode.CreateNode("<td><div class=\"parent\"><p class=\"child\">" + gift.Cost.ToString("C") + "</p></div></td>");
                        HtmlNode desc = HtmlNode.CreateNode("<td><div class=\"parent\"><p class=\"description child\">" + HttpUtility.HtmlEncode(gift.Description) + "</p></div></td>");
                        giftRow.AppendChild(pict);
                        giftRow.AppendChild(rate);
                        giftRow.AppendChild(name);
                        giftRow.AppendChild(quan);
                        giftRow.AppendChild(cost);
                        giftRow.AppendChild(desc);

                        giftTable.AppendChild(giftRow);

                        HtmlNode item = HtmlNode.CreateNode("<tr id=\"" + gift.GiftId + "\" class=\"gift-row\">" +
                                                            "<td><div class=\"parent\"><img class=\"img-thumbnail img-responsive child\" src=\"" + gift.GetImage() + "\" /></div>" +
                                                            "<div class=\"parent\"><p class=\"child\"><input class=\"star-rating\" data-show-clear=\"false\" data-show-caption=\"false\" value=\"" + gift.Rating.ToString("N2") + "\" /></p></div></td>" +
                                                            "<td><div class=\"parent\"><p classs\"child\">" + HttpUtility.HtmlEncode(gift.Name) + "</p></div></td>" +
                                                            "</tr>");
                        giftTableMicro.AppendChild(item);
                        //if (i++ >= 9)
                        //{
                        //    break;
                        //}
                    }
                }
                return myList.DocumentNode.OuterHtml;
            }
        }
    }
}