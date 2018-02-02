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
    namespace HtmlManager
    {
        /// <summary>
        /// A Manager for List HTML
        /// </summary>
        public class ListManager
        {
            private ResourceManager HtmlManager;
            private ResourceManager StringManager;
            private NavigationManager NavigationManager;
            /// <summary>
            /// Create a new ListManager
            /// </summary>
            /// <param name="controller">The controller for this thread</param>
            public ListManager(Controller controller)
            {
                Thread.CurrentThread.CurrentUICulture = controller.Culture;
                Thread.CurrentThread.CurrentCulture = controller.Culture;
                HtmlManager = new ResourceManager("GiftServer.HtmlTemplates", typeof(ListManager).Assembly);
                StringManager = new ResourceManager("GiftServer.Strings", typeof(ListManager).Assembly);
                NavigationManager = controller.NavigationManager;
            }
            /// <summary>
            /// Generate a gift list for this user to be viewed by another user
            /// </summary>
            /// <param name="viewer">The viewer of this list</param>
            /// <param name="target">The owner of this list</param>
            /// <returns>Complete HTML markup for this public list</returns>
            public string GiftList(User viewer, User target)
            {
                HtmlDocument list = new HtmlDocument();
                list.LoadHtml(NavigationManager.NavigationBar(target) + HtmlManager.GetString("publicList"));
                // Get all gifts that are visible to THIS USER
                HtmlNode userId = list.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" thisUserId \")]");
                userId.SetAttributeValue("data-user-id", viewer.UserId.ToString());
                List<Gift> gifts = viewer.GetGifts(target);
                HtmlNode userName = list.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userName \")]");
                userName.InnerHtml = "<a href=\"" + Constants.URL + "/?dest=user&user=" + target.UserUrl + "\">" + HttpUtility.HtmlEncode(target.UserName) + "</a>'s " + StringManager.GetString("giftList");
                HtmlNode title = list.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" title \")]");
                title.InnerHtml = target.UserName + "'s " + StringManager.GetString("giftList");
                HtmlNode giftTable = list.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" giftHolder \")]");
                HtmlNode giftTableMicro = list.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" xsGiftHolder \")]");

                foreach (Gift gift in gifts)
                {
                    // Print gift information
                    if (gift.DateReceived == DateTime.MinValue)
                    {
                        HtmlNode giftRow = HtmlNode.CreateNode("<tr id=\"" + gift.GiftId + "\" class=\"gift-row\"></tr>");

                        giftRow.AddClass("gift-row");
                        giftRow.Attributes.Add("data-gift-id", gift.GiftId.ToString());

                        HtmlNode parent = HtmlNode.CreateNode("<div></div>");
                        parent.AddClass("parent");

                        HtmlNode child = HtmlNode.CreateNode("<p></p>");
                        child.AddClass("child");

                        HtmlNode cell = HtmlNode.CreateNode("<td></td>");

                        // Picture
                        HtmlNode pict = HtmlNode.CreateNode("<img />");
                        pict.AddClass("img-thumbnail img-gift img-responsive child");
                        pict.Attributes.Add("src", gift.GetImage());

                        HtmlNode pictParent = parent.Clone();
                        pictParent.AppendChild(pict);
                        HtmlNode pictCell = cell.Clone();
                        pictCell.AppendChild(pictParent);

                        // Rating
                        HtmlNode rate = HtmlNode.CreateNode("<input />");
                        rate.AddClass("star-rating");
                        rate.Attributes.Add("data-show-clear", "false");
                        rate.Attributes.Add("data-show-caption", "false");
                        rate.Attributes.Add("value", gift.Rating.ToString("N2"));

                        HtmlNode rateCell = cell.Clone();
                        HtmlNode rateParent = parent.Clone();
                        HtmlNode rateChild = child.Clone();
                        rateChild.AppendChild(rate);
                        rateParent.AppendChild(rateChild);
                        rateCell.AppendChild(rateParent);

                        // Name
                        HtmlNode name = child.Clone();
                        name.InnerHtml = HttpUtility.HtmlEncode(gift.Name);

                        HtmlNode nameCell = cell.Clone();
                        HtmlNode nameParent = parent.Clone();
                        nameParent.AppendChild(name);
                        nameCell.AppendChild(nameParent);

                        // Quantity
                        HtmlNode quan = child.Clone();
                        quan.InnerHtml = gift.Quantity.ToString();

                        HtmlNode quanCell = cell.Clone();
                        HtmlNode quanParent = parent.Clone();
                        quanParent.AppendChild(quan);
                        quanCell.AppendChild(quanParent);

                        // Cost
                        HtmlNode cost = child.Clone();
                        cost.InnerHtml = gift.Cost.ToString("C");

                        HtmlNode costCell = cell.Clone();
                        HtmlNode costParent = parent.Clone();
                        costParent.AppendChild(cost);
                        costCell.AppendChild(costParent);

                        // Description
                        HtmlNode desc = child.Clone();
                        desc.InnerHtml = HttpUtility.HtmlEncode(gift.Description);

                        HtmlNode descCell = cell.Clone();
                        HtmlNode descParent = parent.Clone();
                        descParent.AppendChild(desc);
                        descCell.AppendChild(descParent);
                        
                        giftRow.AppendChild(pictCell);
                        giftRow.AppendChild(rateCell);
                        giftRow.AppendChild(nameCell);
                        giftRow.AppendChild(quanCell);
                        giftRow.AppendChild(costCell);
                        giftRow.AppendChild(descCell);

                        giftTable.AppendChild(giftRow);

                        HtmlNode microRow = HtmlNode.CreateNode("<tr></tr>");
                        microRow.Attributes.Add("data-gift-id", gift.GiftId.ToString());
                        microRow.AddClass("gift-row");

                        microRow.AppendChild(pictCell.Clone());
                        microRow.AppendChild(rateCell.Clone());
                        microRow.AppendChild(nameCell.Clone());

                        giftTableMicro.AppendChild(microRow);
                    }
                }
                return list.DocumentNode.OuterHtml;
            }
            /// <summary>
            /// Return a private gift list for this user.
            /// </summary>
            /// <param name="target">The owner of this list</param>
            /// <returns>Complete HTML markup for this private list</returns>
            public string GiftList(User target)
            {
                HtmlDocument myList = new HtmlDocument();
                myList.LoadHtml(NavigationManager.NavigationBar(target) + HtmlManager.GetString("list"));
                // Add category options to new and edit:
                HtmlNode userId = myList.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" thisUserId \")]");
                userId.SetAttributeValue("data-user-id", target.UserId.ToString());
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
                                // WHY OH WHY CAN I NOT PREPEND THIS ELEMENT TO BOTH LISTS??????
                                string catName = Convert.ToString(Reader["CategoryName"]);
                                HtmlNode entry = HtmlNode.CreateNode("<option value=\"" + HttpUtility.HtmlEncode(catName) + "\"></option>");
                                entry.InnerHtml = HttpUtility.HtmlEncode(catName);
                                categoryEdit.PrependChild(entry);
                                entry = HtmlNode.CreateNode("<option value=\"" + HttpUtility.HtmlEncode(catName) + "\"></option>");
                                entry.InnerHtml = HttpUtility.HtmlEncode(catName);
                                categoryNew.PrependChild(entry);
                            }
                        }
                    }
                }
                // Add group options to new and edit:
                HtmlNode groupsEdit = myList.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" editSharedGroups \")]");
                foreach (Group group in target.Groups)
                {
                    HtmlNode entry = HtmlNode.CreateNode("<label class=\"checkbox-inline col-xs-5\" data-group-id=\"" + HttpUtility.HtmlEncode(group.GroupId) + "\"></label>");
                    entry.AppendChild(HtmlNode.CreateNode("<input type=\"checkbox\" value=\"\" data-group-id=\"" + HttpUtility.HtmlEncode(group.GroupId) + "\" />"));
                    HtmlNode node = HtmlNode.CreateNode("<p></p>");
                    node.InnerHtml = HttpUtility.HtmlEncode(group.Name);
                    entry.AppendChild(node);
                    groupsEdit.AppendChild(entry);
                    groupsEdit.AppendChild(HtmlNode.CreateNode("<div class=\"col-xs-1\"></div>"));
                }
                HtmlNode userName = myList.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userName \")]");
                userName.InnerHtml = "<a href=\"" + Constants.URL + "/?dest=user&user=" + target.UserUrl + "\">" + HttpUtility.HtmlEncode(target.UserName) + "</a>'s " + StringManager.GetString("giftList");
                HtmlNode giftTable = myList.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" giftHolder \")]");
                HtmlNode giftTableMicro = myList.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" xsGiftHolder \")]");
                List<Gift> gifts = target.Gifts;

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