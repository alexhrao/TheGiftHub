﻿using System;
using GiftServer.Data;
using GiftServer.Properties;
using HtmlAgilityPack;
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
                myList.LoadHtml(Resources.header + NavigationManager.NavigationBar(user) + Resources.list);
                HtmlNode giftTable = myList.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" giftHolder \")]");
                HtmlNode giftTableMicro = myList.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" xsGiftHolder \")]");
                List<Gift> gifts = user.Gifts;
                foreach (Gift gift in gifts)
                {
                    // Print gift information
                    HtmlNode giftRow = HtmlNode.CreateNode("<tr></tr>");

                    HtmlNode pict = HtmlNode.CreateNode("<td><div class=\"parent\"><img class=\"img-thumbnail img-responsive child\" src=\"" + gift.GetImage() + "\" /></div></td>");
                    HtmlNode rate = HtmlNode.CreateNode("<td><div class=\"parent\"><h3 class=\"child\"><input class=\"star-rating\" data-show-clear=\"false\" data-show-caption=\"false\" value=\"" + gift.Rating.ToString("N2") + "\" /></h3></div></td>");
                    HtmlNode name = HtmlNode.CreateNode("<td><div class=\"parent\"><h3 class=\"child\">" + HttpUtility.HtmlEncode(gift.Name) + "</h3></div></td>");
                    HtmlNode quan = HtmlNode.CreateNode("<td><div class=\"parent\"><h3 class=\"child\">" + gift.Quantity + "</h3></div></td>");
                    HtmlNode cost = HtmlNode.CreateNode("<td><div class=\"parent\"><h3 class=\"child\">" + gift.Cost.ToString("C") + "</h3></div></td>");
                    HtmlNode desc = HtmlNode.CreateNode("<td><div class=\"parent\"><h4 class=\"child\">" + HttpUtility.HtmlEncode(gift.Description) + "</h4></div></td>");
                    giftRow.AppendChild(pict);
                    giftRow.AppendChild(rate);
                    giftRow.AppendChild(name);
                    giftRow.AppendChild(quan);
                    giftRow.AppendChild(cost);
                    giftRow.AppendChild(desc);

                    giftTable.AppendChild(giftRow);

                    HtmlNode item = HtmlNode.CreateNode("<tr>" +
                                                        "<td><div class=\"parent\"><img class=\"img-thumbnail img-responsive child\" src=\"" + gift.GetImage() + "\" /></div>" +
                                                        "<div class=\"parent\"><h3 class=\"child\"><input class=\"star-rating\" data-size=\"xs\" data-show-clear=\"false\" data-show-caption=\"false\" value=\"" + gift.Rating.ToString("N2") + "\" /></h3></div></td>" +
                                                        "<td><div class=\"parent\"><h3 classs\"child\">" + HttpUtility.HtmlEncode(gift.Name) + "</h3></div></td>" +
                                                        "</tr>");

                    //HtmlNode item = HtmlNode.CreateNode("<tr>" +
                     //                                   "<td><img class=\"img-responsive text-center img-thumbnail text-center\" src=\"" + 
                      //                                  gift.GetImage() + "\" /><h4 class=\"text-center\">" + gift.Rating + "</h4></td>" +
                       //                                 "<td><h3 class=\"text-center\">" + HttpUtility.HtmlEncode(gift.Name) + "<h3></td>" +
                        //                                "</tr>");
                    giftTableMicro.AppendChild(item);

                }
                // Get all gifts associated with me

                return myList.DocumentNode.OuterHtml;
            }
        }
    }
}