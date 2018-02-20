using System;
using System.Net.Mail;
using GiftServer.Data;
using GiftServer.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
