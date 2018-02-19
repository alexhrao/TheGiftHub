using System;
using GiftServer.Data;
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
