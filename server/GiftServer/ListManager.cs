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
            /// 
            public string GiftList(User viewer, User target)
            {
                List<Gift> gifts = viewer.GetGifts(target);
                return GiftList(viewer, target, gifts);
            }
            private string GiftList(User viewer, User target, List<Gift> gifts)
            {
                HtmlDocument list = new HtmlDocument();
                list.LoadHtml(NavigationManager.NavigationBar(target) + HtmlManager.GetString("publicList"));
                // Get all gifts that are visible to THIS USER
                HtmlNode userId = list.DocumentNode.
                    SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" thisUserId \")]");
                userId.SetAttributeValue("data-user-id", viewer.ID.ToString());
                HtmlNode userName = list.DocumentNode.
                    SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userName \")]");
                userName.InnerHtml = "<a href=\"" + Constants.URL + "/?dest=user&user=" + target.Url + "\">" + 
                    HttpUtility.HtmlEncode(target.Name) + "</a>'s " + StringManager.GetString("giftList");
                HtmlNode title = list.DocumentNode.
                    SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" title \")]");
                title.InnerHtml = target.Name + "'s " + StringManager.GetString("giftList");
                HtmlNode giftTable = list.DocumentNode.
                    SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" giftHolder \")]");
                HtmlNode giftTableMicro = list.DocumentNode.
                    SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" xsGiftHolder \")]");
                HtmlNode giftTableReceived = list.DocumentNode.
                    SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" receivedGiftHolder \")]");
                HtmlNode giftTableMicroReceived = list.DocumentNode.
                    SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" xsReceivedGiftHolder \")]");

                foreach (Gift gift in gifts)
                {
                    // Print gift information
                    // If a reservation is by my, color orange

                    HtmlNode giftRow = HtmlNode.CreateNode("<tr></tr>");

                    if (gift.Reservations.Exists(r => r.User.ID == viewer.ID))
                    {
                        giftRow.AddClass("gift-reserved");
                    }
                    else if (gift.Reservations.Count >= gift.Quantity)
                    {
                        giftRow.AddClass("gift-full");
                    }
                    giftRow.AddClass("gift-row");
                    giftRow.Attributes.Add("data-gift-id", gift.ID.ToString());

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

                    HtmlNode microRow = HtmlNode.CreateNode("<tr></tr>");
                    microRow.Attributes.Add("data-gift-id", gift.ID.ToString());
                    microRow.AddClass("gift-row");
                    HtmlNode microPic = HtmlNode.CreateNode("<td></td>");
                    // Two rows, first pic, then rating
                    HtmlNode microPicParent = pictParent.Clone();
                    microPicParent.AddClass("row text-center");
                    microPic.AppendChild(microPicParent);
                    HtmlNode microRateParent = rateParent.Clone();
                    microRateParent.AddClass("row text-center");
                    microPic.AppendChild(microRateParent);

                    microRow.AppendChild(microPic);
                    microRow.AppendChild(nameCell.Clone());

                    if (!gift.DateReceived.HasValue)
                    {
                        giftTable.AppendChild(giftRow);
                        giftTableMicro.AppendChild(microRow);
                    }
                    else
                    {
                        // Attach to other table
                        giftTableReceived.AppendChild(giftRow);
                        giftTableMicroReceived.AppendChild(microRow);
                    }
                }
                return list.DocumentNode.OuterHtml;
            }
            /// <summary>
            /// Get a list of gift reservations for this user
            /// </summary>
            /// <param name="target">The user who is viewing their own reservations </param>
            /// <returns></returns>
            public string GiftReservations(User target)
            {
                List<Gift> gifts = new List<Gift>();
                // For each reservation, if gift already added, continue, otherwise, add
                foreach (Reservation res in target.Reservations)
                {
                    if (!gifts.Exists(g => res.Gift.ID == g.ID))
                    {
                        // Add it
                        gifts.Add(res.Gift);
                    }
                }
                string page = GiftList(target, target, gifts);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(page);
                HtmlNode userName = doc.DocumentNode.
                    SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userName \")]");
                // Change to my own Reservations:
                userName.InnerHtml = "<a href=\"" + Constants.URL + "/?dest=user&user=" + target.Url + "\">" + 
                    HttpUtility.HtmlEncode(target.Name) + "</a>'s " + StringManager.GetString("reservations");
                HtmlNode giftTable = doc.DocumentNode.
                    SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" giftHolder \")]");
                HtmlNode giftTableMicro = doc.DocumentNode.
                    SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" xsGiftHolder \")]");
                giftTable.AddClass("table-reserved");
                giftTableMicro.AddClass("table-reserved");
                return doc.DocumentNode.OuterHtml;
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
                HtmlNode userId = myList.DocumentNode.
                    SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" thisUserId \")]");
                userId.SetAttributeValue("data-user-id", target.ID.ToString());
                HtmlNode categoryEdit = myList.DocumentNode.
                    SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" editGiftCategory \")]");
                HtmlNode categoryNew = myList.DocumentNode.
                    SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" newGiftCategory \")]");
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT categories.CategoryName FROM categories ORDER BY CategoryName ASC;";
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string catName = Convert.ToString(reader["CategoryName"]);
                                HtmlNode entry = HtmlNode.CreateNode("<option></option>");
                                entry.Attributes.Add("value", HttpUtility.HtmlAttributeEncode(catName));
                                entry.InnerHtml = HttpUtility.HtmlEncode(catName);
                                categoryEdit.PrependChild(entry);
                                categoryNew.PrependChild(entry.Clone());
                            }
                        }
                    }
                }
                // Add group options to new and edit:
                HtmlNode groupsEdit = myList.DocumentNode.
                    SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" editSharedGroups \")]");
                foreach (Group group in target.Groups)
                {
                    HtmlNode entry = HtmlNode.CreateNode("<label></label>");
                    entry.AddClass("checkbox-inline col-xs-5");
                    entry.Attributes.Add("data-group-id", group.ID.ToString());
                    HtmlNode inp = HtmlNode.CreateNode("<input />");
                    inp.Attributes.Add("type", "checkbox");
                    inp.Attributes.Add("value", "");
                    inp.Attributes.Add("data-group-id", group.ID.ToString());
                    entry.AppendChild(inp);
                    HtmlNode node = HtmlNode.CreateNode("<p></p>");
                    node.InnerHtml = HttpUtility.HtmlEncode(group.Name);
                    entry.AppendChild(node);
                    groupsEdit.AppendChild(entry);
                    groupsEdit.AppendChild(HtmlNode.CreateNode("<div class=\"col-xs-1\"></div>"));
                }
                HtmlNode userName = myList.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userName \")]");
                userName.InnerHtml = "<a href=\"" + Constants.URL + "/?dest=user&user=" + 
                    target.Url + "\">" + HttpUtility.HtmlEncode(target.Name) + "</a>'s "
                    + StringManager.GetString("giftList");
                HtmlNode giftTable = myList.DocumentNode.
                    SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" giftHolder \")]");
                HtmlNode giftTableMicro = myList.DocumentNode.
                    SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" xsGiftHolder \")]");
                HtmlNode giftTableReceived = myList.DocumentNode.
                    SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" receivedGiftHolder \")]");
                HtmlNode giftTableMicroReceived = myList.DocumentNode.
                    SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" xsReceivedGiftHolder \")]");

                foreach (Gift gift in target.Gifts)
                {
                    // Print gift information
                    // If a reservation is by my, color orange

                    HtmlNode giftRow = HtmlNode.CreateNode("<tr></tr>");

                    giftRow.AddClass("gift-row");
                    giftRow.Attributes.Add("data-gift-id", gift.ID.ToString());

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

                    HtmlNode microRow = HtmlNode.CreateNode("<tr></tr>");
                    microRow.Attributes.Add("data-gift-id", gift.ID.ToString());
                    microRow.AddClass("gift-row");
                    HtmlNode microPic = HtmlNode.CreateNode("<td></td>");
                    // Two rows, first pic, then rating
                    HtmlNode microPicParent = pictParent.Clone();
                    microPicParent.AddClass("row text-center");
                    microPic.AppendChild(microPicParent);
                    HtmlNode microRateParent = rateParent.Clone();
                    microRateParent.AddClass("row text-center");
                    microPic.AppendChild(microRateParent);

                    microRow.AppendChild(microPic);
                    microRow.AppendChild(nameCell.Clone());

                    if (!gift.DateReceived.HasValue)
                    {
                        giftTable.AppendChild(giftRow);
                        giftTableMicro.AppendChild(microRow);
                    }
                    else
                    {
                        // Attach to other table
                        giftTableReceived.AppendChild(giftRow);
                        giftTableMicroReceived.AppendChild(microRow);
                    }
                }
                return myList.DocumentNode.OuterHtml;
            }
        }
    }
}