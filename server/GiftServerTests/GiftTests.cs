using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using GiftServer.Data;
using GiftServer.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;

namespace GiftServerTests
{
    [TestClass]
    public class GiftTests
    {
        [TestCategory("Gift"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GiftInstantiate_ZeroID_ExceptionThrown()
        {
            Gift gift = new Gift(0);
        }

        [TestCategory("Gift"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GiftInstantiate_InvalidID_ExceptionThrown()
        {
            Gift gift = new Gift(100);
        }

        [TestCategory("Gift"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GiftInstantiate_NullName_ExceptionThrown()
        {
            Gift gift = new Gift(null, new User(1));
        }

        [TestCategory("Gift"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GiftInstantiate_EmptyName_ExceptionThrown()
        {
            Gift gift = new Gift("", new User(1));
        }

        [TestCategory("Gift"), TestCategory("Instantiate"), TestCategory("Successful")]
        [TestMethod]
        public void GiftInstantiate_ValidID_NewGift()
        {
            Gift gift = new Gift(1);
            Assert.AreEqual(1UL, gift.ID, "ID Mismatch");
        }

        [TestCategory("Gift"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GiftInstantiate_NullUser_ExceptionThrown()
        {
            Gift gift = new Gift("Hello", null);
        }

        [TestCategory("Gift"), TestCategory("Instantiate"), TestCategory("Successful")]
        [TestMethod]
        public void GiftInstantiate_ValidName_NewGift()
        {
            Gift gift = new Gift("Hello world", new User(1));
            Assert.AreEqual(0UL, gift.ID, "New Gift has non-zero ID");
        }



        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GiftProperty_NullName_ExceptionThrown()
        {
            Gift gift = new Gift(1)
            {
                Name = null
            };
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GiftProperty_EmptyName_ExceptionThrown()
        {
            Gift gift = new Gift(1)
            {
                Name = ""
            };
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GiftProperty_SpaceName_ExceptionThrown()
        {
            Gift gift = new Gift(1)
            {
                Name = "   "
            };
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_ValidName_NameChanged()
        {
            Gift gift = new Gift(1)
            {
                Name = "Hello World!"
            };
            Assert.AreEqual("Hello World!", gift.Name, "Name was not changed correctly!");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_NullDescription_NoDescription()
        {
            Gift gift = new Gift(1)
            {
                Description = null
            };
            Assert.IsTrue(null != gift.Description && String.IsNullOrEmpty(gift.Description), "Description has not been correctly removed");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_EmptyDescription_NoDescription()
        {
            Gift gift = new Gift(1)
            {
                Description = ""
            };
            Assert.IsTrue(null != gift.Description && String.IsNullOrEmpty(gift.Description), "Description has not been correctly removed");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_WhiteSpaceDescription_DescriptionChanged()
        {
            Gift gift = new Gift(1)
            {
                Description = "    "
            };
            Assert.IsTrue(null != gift.Description && gift.Description == "    ", "Description has not been correctly modified");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_ValidDescription_DescriptionChanged()
        {
            Gift gift = new Gift(1)
            {
                Description = "Hello world"
            };
            Assert.IsTrue(null != gift.Description && gift.Description == "Hello world", "Description has not been correctly modified");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_NullURL_EmptyURL()
        {
            Gift gift = new Gift(1)
            {
                Url = null
            };
            Assert.IsTrue(gift.Url != null && String.IsNullOrEmpty(gift.Url), "Gift URL was not converted properly");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_EmptyURL_EmptyURL()
        {
            Gift gift = new Gift(1)
            {
                Url = ""
            };
            Assert.IsTrue(gift.Url != null && String.IsNullOrEmpty(gift.Url), "Gift URL was not converted properly");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_ValidURL_Url()
        {
            Gift gift = new Gift(1)
            {
                Url = "https:\\google.com"
            };
            Assert.IsTrue(gift.Url != null && gift.Url == "https:\\google.com", "Gift URL was not converted properly");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GiftProperty_NegativeCost_ExceptionThrown()
        {
            Gift gift = new Gift(1)
            {
                Cost = -5
            };
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_ZeroCost_ZeroCost()
        {
            Gift gift = new Gift(1)
            {
                Cost = 0
            };
            Assert.AreEqual(0d, gift.Cost, "Costs are not equal");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_NonZeroCost_NonZeroCost()
        {
            Gift gift = new Gift(1)
            {
                Cost = 5.02
            };
            Assert.AreEqual(5.02d, gift.Cost, "Costs were not maintained");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_NullStores_EmptyStores()
        {
            Gift gift = new Gift(1)
            {
                Stores = null
            };
            Assert.IsTrue(gift.Stores != null && String.IsNullOrEmpty(gift.Stores), "Gift Stores was not converted properly");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_EmptyStores_EmptyStores()
        {
            Gift gift = new Gift(1)
            {
                Stores = ""
            };
            Assert.IsTrue(gift.Stores != null && String.IsNullOrEmpty(gift.Stores), "Gift Stores was not converted properly");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GiftProperty_ZeroQuantity_ExceptionThrown()
        {
            Gift gift = new Gift(1)
            {
                Quantity = 0
            };
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_ValidQuantity_QuantityChanged()
        {
            Gift gift = new Gift(1)
            {
                Quantity = 65
            };
            Assert.AreEqual(65U, gift.Quantity, "Quantity not changed");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GiftProperty_ShortColor_ExceptionThrown()
        {
            Gift gift = new Gift(1)
            {
                Color = "1234"
            };
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GiftProperty_LongColor_ExceptionThrown()
        {
            Gift gift = new Gift(1)
            {
                Color = "123456789"
            };
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_NullColor_BlackColor()
        {
            Gift gift = new Gift(1)
            {
                Color = null
            };
            Assert.AreEqual("000000", gift.Color, "Null color was not converted to black");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_EmptyColor_BlackColor()
        {
            Gift gift = new Gift(1)
            {
                Color = ""
            };
            Assert.AreEqual("000000", gift.Color, "Empty color was not converted to black");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_SpaceColor_BlackColor()
        {
            Gift gift = new Gift(1)
            {
                Color = " "
            };
            Assert.AreEqual("000000", gift.Color, "Space color was not converted to black");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_OnlyHashtag_BlackColor()
        {
            Gift gift = new Gift(1)
            {
                Color = "#"
            };
            Assert.AreEqual("000000", gift.Color, "Hashtag color was not converted to black");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_HashtagSpace_BlackColor()
        {
            Gift gift = new Gift(1)
            {
                Color = "# "
            };
            Assert.AreEqual("000000", gift.Color, "Hashtag Space color was not converted to black");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_SixSpace_BlackColor()
        {
            Gift gift = new Gift(1)
            {
                Color = "      "
            };
            Assert.AreEqual("000000", gift.Color, "Null color was not converted to black");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GiftProperty_FiveCharOneSpace_ExceptionThrown()
        {
            Gift gift = new Gift(1)
            {
                Color = "12345 "
            };
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GiftProperty_FiveCharOneSpaceHash_ExceptionThrown()
        {
            Gift gift = new Gift(1)
            {
                Color = "#12345 "
            };
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_ValidColorWithHashtag_ValidColor()
        {
            Gift gift = new Gift(1)
            {
                Color = "#ffabe1"
            };
            Assert.AreEqual("FFABE1", gift.Color, "Hashtag color not converted correctly");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_ValidColor_ValidColor()
        {
            Gift gift = new Gift(1)
            {
                Color = "ffabe1"
            };
            Assert.AreEqual("FFABE1", gift.Color, "Color was not set correctly");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_NullColorText_EmptyColorText()
        {
            Gift gift = new Gift(1)
            {
                ColorText = null
            };
            Assert.AreEqual("", gift.ColorText, "ColorText was not set correctly");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_EmptyColorText_EmptyColorText()
        {
            Gift gift = new Gift(1)
            {
                ColorText = ""
            };
            Assert.AreEqual("", gift.ColorText, "ColorText was not set correctly");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_ValidColorText_ValidColorText()
        {
            Gift gift = new Gift(1)
            {
                ColorText = "Green"
            };
            Assert.AreEqual("Green", gift.ColorText, "ColorText was not set correctly");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_NullSize_EmptySize()
        {
            Gift gift = new Gift(1)
            {
                Size = null
            };
            Assert.AreEqual("", gift.Size, "Size was not set correctly");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_EmptySize_EmptySize()
        {
            Gift gift = new Gift(1)
            {
                Size = ""
            };
            Assert.AreEqual("", gift.Size, "Size was not set correctly");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_ValidSize_ValidSize()
        {
            Gift gift = new Gift(1)
            {
                Size = "Large"
            };
            Assert.AreEqual("Large", gift.Size, "Size was not set correctly");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_NullCategory_DefaultCategory()
        {
            Gift gift = new Gift(1)
            {
                Category = null
            };
            Assert.AreEqual(new Category(1), gift.Category, "Null category was not converted to default");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_ValidCategory_ValidCategory()
        {
            Gift gift = new Gift(1)
            {
                Category = new Category(2)
            };
            Assert.AreEqual(new Category(2), gift.Category, "Category not successfully changed");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GiftProperty_NegativeRating_ExceptionThrown()
        {
            Gift gift = new Gift(1)
            {
                Rating = -1d
            };
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GiftProperty_LargerRating_ExceptionThrown()
        {
            Gift gift = new Gift(1)
            {
                Rating = 7d
            };
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_ZeroRating_ZeroRating()
        {
            Gift gift = new Gift(1)
            {
                Rating = 0
            };
            Assert.AreEqual(0d, gift.Rating, "Rating is not 0");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_FiveRating_FiveRating()
        {
            Gift gift = new Gift(1)
            {
                Rating = 5
            };
            Assert.AreEqual(5d, gift.Rating, "Rating is not 5");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_DecimalRating_DecimalRating()
        {
            Gift gift = new Gift(1)
            {
                Rating = 2.5
            };
            Assert.AreEqual(2.5d, gift.Rating, "Rating is not 2.5");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Get"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_NewGift_NullTimestamp()
        {
            Gift gift = new Gift("Tester", new User(1));
            Assert.IsNull(gift.TimeStamp, "Non Null Timestamp for new gift");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Get"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_ExistingGift_NonNullTimestamp()
        {
            Gift gift = new Gift(1);
            Assert.IsNotNull(gift.TimeStamp, "Null Timestamp for existing gift");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Get"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_NotReceived_NullReceivedDate()
        {
            Gift gift = new Gift(1);
            Assert.IsNull(gift.DateReceived, "Date Received is not null");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void GiftProperty_SetReceived_NonNullReceivedDate()
        {
            Gift gift = new Gift(1)
            {
                DateReceived = DateTime.Today
            };
            Assert.IsNotNull(gift.DateReceived, "Date Received not set");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Get"), TestCategory("Successful"), TestCategory("Reservation")]
        [TestMethod]
        public void GiftProperty_Reservations_NoReservations()
        {
            Gift gift = new Gift(1);
            List<Reservation> res = gift.Reservations;
            Assert.AreEqual(0, res.Count, "More than 0 reservations fetched");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Get"), TestCategory("Successful"), TestCategory("Reservation")]
        [TestMethod]
        public void GiftProperty_Reservations_OneReservation()
        {
            Gift gift = new Gift(8);
            List<Reservation> res = gift.Reservations;
            Assert.AreEqual(1, res.Count, "Incorrect number of reservations");
            Assert.AreEqual(8UL, res[0].Gift.ID, "Incorrect GiftID fetched");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Get"), TestCategory("Successful"), TestCategory("Reservation")]
        [TestMethod]
        public void GiftProperty_Reservations_ManyReservations()
        {
            Gift gift = new Gift(3);
            List<Reservation> res = gift.Reservations;
            Assert.AreEqual(3, res.Count, "Incorrect number of reservations");
            Assert.IsTrue(res[0].Gift.ID == 3, "Wrong reservation fetched");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Get"), TestCategory("ExceptionThrown"), TestCategory("Reservation")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GiftProperty_ReservationsZeroID_ExceptionThrown()
        {
            Gift gift = new Gift("Hello!", new User(1));
            List<Reservation> res = gift.Reservations;
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Get"), TestCategory("ExceptionThrown"), TestCategory("Group")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GiftProperty_GroupsZeroID_ExceptionThrown()
        {
            Gift gift = new Gift("Hello!", new User(1));
            List<Group> groups = gift.Groups;
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Get"), TestCategory("Successful"), TestCategory("Group")]
        public void GiftProperty_Groups_NoGroups()
        {
            Gift gift = new Gift(3);
            List<Group> groups = gift.Groups;
            Assert.AreEqual(0, groups.Count, "Invalid number of groups fetched");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Get"), TestCategory("Successful"), TestCategory("Group")]
        public void GiftProperty_Groups_OneGroup()
        {
            Gift gift = new Gift(2);
            List<Group> groups = gift.Groups;
            Assert.AreEqual(1, groups.Count, "Invalid number of groups fetched");
            Assert.AreEqual(1L, groups[0].ID, "Wrong Group tie fetched");
        }

        [TestCategory("Gift"), TestCategory("Property"), TestCategory("Get"), TestCategory("Successful"), TestCategory("Group")]
        public void GiftProperty_Groups_ManyGroups()
        {
            Gift gift = new Gift(1);
            List<Group> groups = gift.Groups;
            Assert.AreEqual(2, groups.Count, "Invalid number of groups fetched");
            Assert.AreEqual(2, groups.FindAll(g => g.ID == 1 || g.ID == 2).Count, "Wrong Groups fetched");
            Assert.AreNotEqual(groups[0].ID, groups[1].ID, "Same group fetched twice");
        }



        [TestCategory("Gift"), TestCategory("Method"), TestCategory("Create"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GiftCreate_ZeroIDUser_ExceptionThrown()
        {
            Gift gift = new Gift("Hello", new User(new MailAddress("wassup@wassup.com"), new Password("hello"), "hello"));
            gift.Create();
        }

        [TestCategory("Gift"), TestCategory("Method"), TestCategory("Create"), TestCategory("Successful")]
        [TestMethod]
        public void GiftCreate_MinimalData_NewGift()
        {
            Gift gift = new Gift("What is up", new User(1));
            gift.Create();
            Assert.AreNotEqual(0UL, gift.ID, "Gift's ID not changed");
            Gift tester = new Gift(gift.ID);
            Assert.AreEqual("What is up", tester.Name, "Gift Name mismatch");
            Assert.AreEqual(1UL, tester.Owner.ID, "Gift owner mismatch");
        }

        [TestCategory("Gift"), TestCategory("Method"), TestCategory("Create"), TestCategory("Successful")]
        [TestMethod]
        public void GiftCreate_AllData_NewGift()
        {
            Gift gift = new Gift("Test123", new User(2))
            {
                Category = new Category(2),
                Color = "123456",
                ColorText = "Blue",
                Cost = 5.04d,
                DateReceived = DateTime.Today,
                Description = "My Beautiful gift",
                Quantity = 89,
                Rating = 2.5,
                Size = "Large",
                Stores = "Target, Walmart, etc.",
                Url = "https://www.google.com"
            };
            gift.Create();
            Assert.AreNotEqual(0UL, gift.ID, "Gift ID not updated");
            Gift tester = new Gift(gift.ID);
            // check all info
            Assert.AreEqual("Test123", tester.Name, "Name not added");
            Assert.AreEqual(2UL, tester.Owner.ID, "Owner not added");
            Assert.AreEqual(new Category(2), tester.Category, "Category not added");
            Assert.AreEqual("123456", tester.Color, "Color not added");
            Assert.AreEqual("Blue", tester.ColorText, "Color Text not added");
            Assert.AreEqual(5.04d, tester.Cost, "Cost not added");
            Assert.AreEqual(DateTime.Today, tester.DateReceived, "Dates do not match up");
            Assert.AreEqual("My Beautiful gift", tester.Description, "Description not added");
            Assert.AreEqual(89U, tester.Quantity, "Quantity not added");
            Assert.AreEqual(2.5d, tester.Rating, "Rating not added");
            Assert.AreEqual("Large", tester.Size, "Size not added");
            Assert.AreEqual("Target, Walmart, etc.", tester.Stores, "Stores not added");
            Assert.AreEqual("https://www.google.com", tester.Url, "URL not added");
        }

        [TestCategory("Gift"), TestCategory("Method"), TestCategory("Update")]
        [TestMethod]
        public void GiftUpdate_AllData_UpdatedData()
        {
            Gift gift = new Gift(9)
            {
                Category = new Category(2),
                Color = "123456",
                ColorText = "Blue",
                Cost = 5.04d,
                DateReceived = DateTime.Today,
                Description = "My Beautiful gift",
                Quantity = 89,
                Rating = 2.5,
                Size = "Large",
                Stores = "Target, Walmart, etc.",
                Url = "https://www.google.com"
            };
            gift.Update();
            Assert.AreEqual(9UL, gift.ID, "Gift ID changed");
            Gift tester = new Gift(gift.ID);
            // check all info
            Assert.AreEqual("Test123", tester.Name, "Name not updated");
            Assert.AreEqual(2UL, tester.Owner.ID, "Owner not updated");
            Assert.AreEqual(new Category(2), tester.Category, "Category not updated");
            Assert.AreEqual("123456", tester.Color, "Color not updated");
            Assert.AreEqual("Blue", tester.ColorText, "Color Text not updated");
            Assert.AreEqual(5.04d, tester.Cost, "Cost not updated");
            Assert.AreEqual(DateTime.Today, tester.DateReceived, "Dates do not match up");
            Assert.AreEqual("My Beautiful gift", tester.Description, "Description not updated");
            Assert.AreEqual(89U, tester.Quantity, "Quantity not updated");
            Assert.AreEqual(2.5d, tester.Rating, "Rating not updated");
            Assert.AreEqual("Large", tester.Size, "Size not updated");
            Assert.AreEqual("Target, Walmart, etc.", tester.Stores, "Stores not updated");
            Assert.AreEqual("https://www.google.com", tester.Url, "URL not updated");
        }

        [TestCategory("Gift"), TestCategory("Method"), TestCategory("Update"), TestCategory("Successful")]
        [TestMethod]
        public void GiftUpdate_ZeroID_GiftCreated()
        {
            Gift gift = new Gift("Test123", new User(2))
            {
                Category = new Category(2),
                Color = "123456",
                ColorText = "Blue",
                Cost = 5.04d,
                DateReceived = DateTime.Today,
                Description = "My Beautiful gift",
                Quantity = 89,
                Rating = 2.5,
                Size = "Large",
                Stores = "Target, Walmart, etc.",
                Url = "https://www.google.com"
            };
            gift.Update();
            Assert.AreNotEqual(0UL, gift.ID, "Gift ID not updated");
            Gift tester = new Gift(gift.ID);
            // check all info
            Assert.AreEqual("Test123", tester.Name, "Name not added");
            Assert.AreEqual(2UL, tester.Owner.ID, "Owner not added");
            Assert.AreEqual(new Category(2), tester.Category, "Category not added");
            Assert.AreEqual("123456", tester.Color, "Color not added");
            Assert.AreEqual("Blue", tester.ColorText, "Color Text not added");
            Assert.AreEqual(5.04d, tester.Cost, "Cost not added");
            Assert.AreEqual(DateTime.Today, tester.DateReceived, "Dates do not match up");
            Assert.AreEqual("My Beautiful gift", tester.Description, "Description not added");
            Assert.AreEqual(89U, tester.Quantity, "Quantity not added");
            Assert.AreEqual(2.5d, tester.Rating, "Rating not added");
            Assert.AreEqual("Large", tester.Size, "Size not added");
            Assert.AreEqual("Target, Walmart, etc.", tester.Stores, "Stores not added");
            Assert.AreEqual("https://www.google.com", tester.Url, "URL not added");
        }



        [TestCategory("Gift"), TestCategory("Delete"), TestCategory("Method"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GiftDelete_ZeroID_ExceptionThrown()
        {
            Gift gift = new Gift("hello", new User(1));
            gift.Delete();
        }

        [TestCategory("Gift"), TestCategory("Delete"), TestCategory("Method"), TestCategory("Successful")]
        [TestMethod]
        public void GiftDelete_ValidGift_GiftDeleted()
        {
            Gift gift = new Gift(10);
            ulong giftID = gift.ID;
            gift.Delete();
            // Check image
            Assert.IsFalse(File.Exists(Directory.GetCurrentDirectory() + "/resources/images/gifts/Gift" + giftID + ".png"), "Gift Image not deleted");
            // Ensure no reservations exist in DB:
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT ReservationID FROM reservations WHERE GiftID = @gid;";
                    cmd.Parameters.AddWithValue("@gid", giftID);
                    cmd.Prepare();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Assert.IsFalse(reader.HasRows, "Not all reservations deleted");
                    }
                }
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT GroupID FROM groups_gifts WHERE GiftID = @gid;";
                    cmd.Parameters.AddWithValue("@gid", giftID);
                    cmd.Prepare();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Assert.IsFalse(reader.HasRows, "Not all groups removed");
                    }
                }
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT GiftID FROM gifts WHERE GiftID = @gid;";
                    cmd.Parameters.AddWithValue("@gid", giftID);
                    cmd.Prepare();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Assert.IsFalse(reader.HasRows, "Gift not deleted");
                    }
                }
            }
        }


        [TestCategory("Gift"), TestCategory("Method"), TestCategory("SaveImage"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GiftSaveImage_ZeroID_ExceptionThrown()
        {
            Gift gift = new Gift("hello", new User(1));
            gift.SaveImage(TestManager.Image);
        }

        [TestCategory("Gift"), TestCategory("Method"), TestCategory("SaveImage"), TestCategory("Successful")]
        [TestMethod]
        public void GiftSaveImage_ValidGift_ImageSaved()
        {
            Gift gift = new Gift(5);
            gift.SaveImage(TestManager.Image);
            Assert.IsTrue(File.Exists(Directory.GetCurrentDirectory() + "/resources/images/gifts/Gift5.png"), "Gift Image not created");
        }

        [TestCategory("Gift"), TestCategory("Method"), TestCategory("SaveImage"), TestCategory("Successful")]
        [TestMethod]
        public void GiftSaveImage_NullInput_ImageDeleted()
        {
            Gift gift = new Gift(6);
            gift.SaveImage(TestManager.Image);
            Assert.IsTrue(File.Exists(Directory.GetCurrentDirectory() + "/resources/images/gifts/Gift6.png"), "Gift Image not created");
            gift.SaveImage(null);
            Assert.IsFalse(File.Exists(Directory.GetCurrentDirectory() + "/resources/images/gifts/Gift6.png"), "Gift Image not removed");
        }

        [TestCategory("Gift"), TestCategory("Method"), TestCategory("SaveImage"), TestCategory("Successful")]
        [TestMethod]
        public void GiftSaveImage_EmptyInput_ImageDeleted()
        {
            Gift gift = new Gift(7);
            gift.SaveImage(TestManager.Image);
            Assert.IsTrue(File.Exists(Directory.GetCurrentDirectory() + "/resources/images/gifts/Gift7.png"), "Gift Image not created");
            gift.SaveImage(new byte[0]);
            Assert.IsFalse(File.Exists(Directory.GetCurrentDirectory() + "/resources/images/gifts/Gift7.png"), "Gift Image not removed");
        }



        [TestCategory("Gift"), TestCategory("Method"), TestCategory("GetImage"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GiftGetImage_ZeroID_ExceptionThrown()
        {
            Gift gift = new Gift("hi", new User(1));
            string path = gift.Image;
        }

        [TestCategory("Gift"), TestCategory("Method"), TestCategory("GetImage"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GiftGetImage_ZeroIDInput_ExceptionThrown()
        {
            Gift.GetImage(0);
        }



        [ClassInitialize]
        public static void Initialize(TestContext ctx)
        {
            Reset();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            Reset();
        }

        private static void Reset()
        {
            // Initiate DELETE and LOAD
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "CALL gift_registry_db_test.setup();";
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
