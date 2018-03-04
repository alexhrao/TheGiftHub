using System;
using System.Configuration;
using System.Net.Mail;
using System.Xml;
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
            // Assert that Preference were created as well!
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
            // Assert that Preference were created as well!
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



        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void PreferencesEquals_NullObject_False()
        {
            User user = new User(1);
            Assert.IsFalse(user.Preferences.Equals((object)null), "Null object compares true");
        }

        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void PreferencesEquals_NonPreferences_False()
        {
            User user = new User(1);
            Assert.IsFalse(user.Preferences.Equals(new User(1)), "User object compares true");
        }

        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void PreferencesEquals_NullPreferences_False()
        {
            User user = new User(1);
            Assert.IsFalse(user.Preferences.Equals((Preference)null), "Null Preferences compares true");
        }

        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void PreferencesEquals_DifferentPreferences_False()
        {
            User user = new User(1);
            User tester = new User(2);
            Assert.IsFalse(user.Preferences.Equals(tester.Preferences), "Different Preferences compare true");
        }

        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void PreferencesEquals_DifferentPreferencesObject_False()
        {
            User user = new User(1);
            User tester = new User(2);
            object pref = tester.Preferences;
            Assert.IsFalse(user.Preferences.Equals(pref), "Different Preferences object compares true");
        }

        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void PreferencesEquals_SamePreferences_True()
        {
            User user = new User(1);
            User tester = new User(1);
            Assert.IsTrue(user.Preferences.Equals(tester.Preferences), "Same preferences compare false");
        }

        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void PreferencesEquals_IdenticalPreferences_True()
        {
            User user = new User(1);
            Assert.IsTrue(user.Preferences.Equals(user.Preferences), "Identical Preferences compare false");
        }

        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void PreferencesEquals_SamePreferencesAsObject_True()
        {
            User user = new User(1);
            User tester = new User(1);
            object pref = tester.Preferences;
            Assert.IsTrue(user.Preferences.Equals(pref), "Same Preferences compare false");
        }

        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void PreferencesEquals_IdenticalPreferencesAsObject_True()
        {
            User user = new User(1);
            Assert.IsTrue(user.Preferences.Equals((object)user.Preferences), "Identical Preferences as object compare false");
        }



        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Fetch"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PreferencesFetch_InvalidUser_ExceptionThrown()
        {
            User user = new User(new MailAddress("fdsa@fdsafdsa.com"), new Password("Hello World"), "He");
            user.Preferences.Fetch();
        }

        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Fetch"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PreferencesFetch_ZeroID_ExceptionThrown()
        {
            User user = new User(1);
            user.Preferences.Delete();
            try
            {
                user.Preferences.Fetch();
            }
            catch (InvalidOperationException e)
            {
                TestManager.Reset().Wait();
                throw e;
            }
        }

        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Fetch"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PreferencesFetch_InvalidUserWithViewer_ExceptionThrown()
        {
            User user = new User(new MailAddress("fdsa@fdsafdsa.com"), new Password("Hello World"), "He");
            user.Preferences.Fetch(new User(1));
        }

        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Fetch"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void PreferencesFetch_ZeroIDWithViewer_ExceptionThrown()
        {
            User user = new User(1);
            user.Preferences.Delete();
            try
            {
                user.Preferences.Fetch(new User(1));
            }
            catch (InvalidOperationException e)
            {
                TestManager.Reset().Wait();
                throw e;
            }
        }

        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Fetch"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PreferencesFetch_NullViewer_ExceptionThrown()
        {
            User user = new User(1);
            user.Preferences.Fetch(null);
        }

        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Fetch"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PreferencesFetch_InvalidViewer_ExceptionThrown()
        {
            User user = new User(1);
            user.Preferences.Fetch(new User(new MailAddress("Hello@goodbye.com"), new Password("Hi"), "hello"));
        }

        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Fetch"), TestCategory("Successful")]
        [TestMethod]
        public void PreferencesFetch_ValidData_PreferencesFetched()
        {
            User user = new User(1);
            XmlDocument doc = user.Preferences.Fetch();
            Assert.AreEqual("preferences", doc.DocumentElement.Name, "Name mismatch");
            XmlElement id = (XmlElement)doc.GetElementsByTagName("preferenceId")[0];
            Assert.AreEqual(user.Preferences.ID.ToString(), id.InnerText, "Preference ID mismatch");
            XmlElement culture = (XmlElement)doc.GetElementsByTagName("culture")[0];
            Assert.AreEqual(user.Preferences.Culture, culture.InnerText, "Preference Culture mismatch");
        }

        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Fetch"), TestCategory("Successful")]
        [TestMethod]
        public void PreferencesFetch_UnauthorizedViewer_BareFetch()
        {
            User user = new User(1);
            XmlDocument doc = user.Preferences.Fetch(new User(2));
            Assert.AreEqual("preferences", doc.DocumentElement.Name, "Name mismatch");
        }

        [TestCategory("Preferences"), TestCategory("Method"), TestCategory("Fetch"), TestCategory("Successful")]
        [TestMethod]
        public void PreferencesFetch_ValidViewer_CompleteFetch()
        {
            User user = new User(1);
            XmlDocument doc = user.Preferences.Fetch(user);
            Assert.AreEqual("preferences", doc.DocumentElement.Name, "Name mismatch");
            XmlElement id = (XmlElement)doc.GetElementsByTagName("preferenceId")[0];
            Assert.AreEqual(user.Preferences.ID.ToString(), id.InnerText, "Preference ID mismatch");
            XmlElement culture = (XmlElement)doc.GetElementsByTagName("culture")[0];
            Assert.AreEqual(user.Preferences.Culture, culture.InnerText, "Preference Culture mismatch");
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
