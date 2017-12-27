using GiftServer.Data;
using GiftServer.Properties;
using GiftServer.Server;
using HtmlAgilityPack;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Resources;
using System.Threading;
using System.Web;

namespace GiftServer
{
    namespace Html
    {
        public class ListManager
        {
            private ResourceManager HtmlManager;
            private ResourceManager StringManager;
            private NavigationManager NavigationManager;

            public ListManager(Controller controller)
            {
                Thread.CurrentThread.CurrentUICulture = controller.Culture;
                Thread.CurrentThread.CurrentCulture = controller.Culture;
                HtmlManager = new ResourceManager("GiftServer.HtmlTemplates", typeof(ListManager).Assembly);
                StringManager = new ResourceManager("GiftServer.Strings", typeof(ListManager).Assembly);
                NavigationManager = controller.NavigationManager;
            }
            public string PublicList(User user)
            {
                return GiftList(user);
            }
            public string GiftList(User user)
            {
                HtmlDocument myList = new HtmlDocument();
                myList.LoadHtml(NavigationManager.NavigationBar(user) + HtmlManager.GetString("list"));
                // Add category options to new and edit:
                HtmlNode categoryEdit = myList.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" editGiftCategory \")]");
                HtmlNode categoryNew = myList.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" newGiftCategory \")]");
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT categories.CategoryName FROM categories ORDER BY CategoryName ASC;";
                        cmd.Prepare();
                        using (MySqlDataReader Reader = cmd.ExecuteReader())
                        {
                            while (Reader.Read())
                            {
                                string catName = Convert.ToString(Reader["CategoryName"]);
                                HtmlNode entry = HtmlNode.CreateNode("<option value=\"" + catName + "\"></option>");
                                entry.InnerHtml = HttpUtility.HtmlEncode(catName);
                                categoryEdit.PrependChild(entry);
                                categoryNew.PrependChild(entry);
                            }
                        }
                    }
                }
                HtmlNode userName = myList.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userName \")]");
                userName.InnerHtml = user.UserName + "'s " + StringManager.GetString("giftList");
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
                    }
                }
                return myList.DocumentNode.OuterHtml;
            }
        }
    }
}