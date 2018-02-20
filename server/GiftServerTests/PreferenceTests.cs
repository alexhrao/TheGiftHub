using System;
using System.Configuration;
using System.Net.Mail;
using GiftServer.Data;
using GiftServer.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;

namespace GiftServerTests
{
    [TestClass]
    public class PreferenceTests
    {
        [TestCategory("Preferences"), TestCategory("Initialize"), TestCategory("Success")]
        [TestMethod]
        public void Preferences_ValidUserWithPreferences_Success()
        {
            User user = new User(1);
            Assert.AreNotEqual(user.Preferences.ID, 0UL, "Valid preferences has invalid ID");
        }

        [TestCategory("Preferences"), TestCategory("Initialize"), TestCategory("Success")]
        [TestMethod]
        public void Preferences_InvalidUser_NoException()
        {
            User user = new User(new MailAddress("fdsaasdf@fdsaasdf.com"), new Password("hello world"), "My Name");
            Assert.AreEqual(0UL, user.Preferences.ID);
        }



        [TestCategory("Preferences"), TestCategory("Property"), TestCategory("Get"), TestCategory("Successful")]
        [TestMethod]
        public void PreferencesProperty_Locale_Success()
        {
            User user = new User(1);
            Assert.AreEqual("en-US", user.Preferences.Culture, "Culture mismatch");
        }

        [TestCategory("Preferences"), TestCategory("Property"), TestCategory("Set"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PreferencesProperty_LocaleNull_ExceptionThrown()
        {
            User user = new User(1);
            user.Preferences.Culture = null;
        }

        [TestCategory("Preferences"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void PreferencesProperty_LocaleEmpty_DefaultValue()
        {
            User user = new User(1);
            user.Preferences.Culture = "";
            Assert.AreEqual("en-US", user.Preferences.Culture, "Culture not updated correctly");
        }

        [TestCategory("Preferences"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void PreferencesProperty_LocaleInvalid_DefaultValue()
        {
            User user = new User(1);
            user.Preferences.Culture = "fdsasafdfdsaasdf";
            Assert.AreEqual("en-US", user.Preferences.Culture, "Culture not updated correctly");
        }

        [TestCategory("Preferences"), TestCategory("Property"), TestCategory("Set"), TestCategory("Successful")]
        [TestMethod]
        public void PreferencesProperty_LocaleValid_SetValue()
        {
            User user = new User(1);
            user.Preferences.Culture = "fr-FR";
            Assert.AreEqual("fr-FR", user.Preferences.Culture, "Culture not updated correctly");
        }



        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Create"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PreferencesCreate_InvalidUser_ExceptionThrown()
        {
            User user = new User(new MailAddress("fdsa@asdfasdf.com"), new Password("Hello"), "Hi");
            user.Preferences.Create();
        }

        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Create"), TestCategory("Successful")]
        [TestMethod]
        public void PreferencesCreate_ValidUser_PreferencesCreated()
        {
            User user = new User(new MailAddress("fdsa@asdfasdf.com"), new Password("Hello"), "Hi");
            user.Preferences.Culture = "fr-FR";
            user.Create();
            // Assert that Preferences were created as well!
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT * FROM preferences WHERE UserID = @uid;";
                    cmd.Parameters.AddWithValue("@uid", user.ID);
                    cmd.Prepare();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        int counter = 0;
                        while (reader.Read())
                        {
                            counter++;
                            // Check data
                            Assert.AreEqual("fr-FR", reader["UserCulture"].ToString(), "Culture incorrect");
                        }
                        Assert.AreEqual(1, counter, "More than one row returned for a single user");
                    }
                }
            }
        }



        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Create"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PreferencesUpdate_InvalidUser_ExceptionThrown()
        {
            User user = new User(new MailAddress("fdsa@asdfasdf.com"), new Password("Hello"), "Hi");
            user.Preferences.Update();
        }

        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Update"), TestCategory("Successful")]
        [TestMethod]
        public void PreferencesUpdate_ValidUser_PreferencesCreated()
        {
            User user = new User(1);
            user.Preferences.Culture = "fr-FR";
            user.Update();
            // Assert that Preferences were created as well!
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT * FROM preferences WHERE UserID = @uid;";
                    cmd.Parameters.AddWithValue("@uid", user.ID);
                    cmd.Prepare();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        int counter = 0;
                        while (reader.Read())
                        {
                            counter++;
                            // Check data
                            Assert.AreEqual("fr-FR", reader["UserCulture"].ToString(), "Culture incorrect");
                        }
                        Assert.AreEqual(1, counter, "More than one row returned for a single user");
                    }
                }
            }
            user.Preferences.Culture = "en-GB";
            user.Preferences.Update();
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT * FROM preferences WHERE UserID = @uid;";
                    cmd.Parameters.AddWithValue("@uid", user.ID);
                    cmd.Prepare();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        int counter = 0;
                        while (reader.Read())
                        {
                            counter++;
                            // Check data
                            Assert.AreEqual("en-GB", reader["UserCulture"].ToString(), "Culture incorrect");
                        }
                        Assert.AreEqual(1, counter, "More than one row returned for a single user");
                    }
                }
            }
        }



        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Delete"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PreferencesDelete_InvalidUser_ExceptionThrown()
        {
            User user = new User(new MailAddress("fdsa@asdfasdf.com"), new Password("Hello"), "Hi");
            user.Preferences.Delete();
        }

        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Delete"), TestCategory("Successful")]
        [TestMethod]
        public void PreferencesDelete_ValidUser_Successful()
        {
            User user = new User(10);
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT * FROM preferences WHERE UserID = @uid;";
                    cmd.Parameters.AddWithValue("@uid", user.ID);
                    cmd.Prepare();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Assert.IsFalse(reader.HasRows, "Preferences found after deleted");
                    }
                }
            }
            user.Preferences.Create();
            user.Delete();
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT * FROM preferences WHERE UserID = @uid;";
                    cmd.Parameters.AddWithValue("@uid", user.ID);
                    cmd.Prepare();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Assert.IsFalse(reader.HasRows, "Preferences found after user deleted");
                    }
                }
            }
            TestManager.Reset().Wait();
        }
        [ClassInitialize]
        public static void Initialize(TestContext ctx)
        {
            TestManager.Reset().Wait();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            TestManager.Reset().Wait();
        }
    }
}
