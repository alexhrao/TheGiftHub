using System;
using System.Configuration;
using GiftServer.Data;
using GiftServer.Exceptions;
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
            Gift gift = new Gift(null);
        }

        [TestCategory("Gift"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GiftInstantiate_EmptyName_ExceptionThrown()
        {
            Gift gift = new Gift("");
        }

        [TestCategory("Gift"), TestCategory("Instantiate"), TestCategory("Successful")]
        [TestMethod]
        public void GiftInstantiate_ValidID_NewGift()
        {
            Gift gift = new Gift(1);
            Assert.AreEqual(1UL, gift.ID, "ID Mismatch");
        }

        [TestCategory("Gift"), TestCategory("Instantiate"), TestCategory("Successful")]
        [TestMethod]
        public void GiftInstantiate_ValidName_NewGift()
        {
            Gift gift = new Gift("Hello world");
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
