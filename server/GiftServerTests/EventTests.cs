using System;
using System.Net.Mail;
using GiftServer.Data;
using GiftServer.Exceptions;
using GiftServer.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GiftServerTests
{
    [TestClass]
    public class EventTests
    {
        [TestCategory("Event")]
        [TestMethod]
        public void TestMethod1()
        {

        }

        [TestCategory("Event"), TestCategory("Initialize")]
        [TestMethod]
        [ExpectedException(typeof(EventNotFoundException))]
        public void EventInitialize_ZeroID_ExceptionThrown()
        {
            Event e = new Event(0);
        }

        [TestCategory("Event"), TestCategory("Initialize")]
        [TestMethod]
        [ExpectedException(typeof(EventNotFoundException))]
        public void EventInitialize_InvalidID_ExceptionThrown()
        {
            Event e = new Event(100);
        }

        [TestCategory("Event"), TestCategory("Initialize")]
        [TestMethod]
        public void EventInitialize_ValidID_EventFetched()
        {
            Event e = new Event(1);
        }

        [TestCategory("Event"), TestCategory("Initialize")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EventInitialize_NullName_ExceptionThrown()
        {
            Event e = new Event(null, DateTime.Now, new User(1), null);
        }

        [TestCategory("Event"), TestCategory("Initialize")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EventInitialize_EmptyName_ExceptionThrown()
        {
            Event e = new Event("", DateTime.Now, new User(1), null);
        }

        [TestCategory("Event"), TestCategory("Initialize")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EventInitialize_SpaceName_ExceptionThrown()
        {
            Event e = new Event("   ", DateTime.Now, new User(1), null);
        }

        [TestCategory("Event"), TestCategory("Initialize")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void EventInitialize_NullUser_ExceptionThrown()
        {
            Event e = new Event("   ", DateTime.Now, null, null);
        }

        [TestCategory("Event"), TestCategory("Initialize")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EventInitialize_InvalidUser_ExceptionThrown()
        {
            User user = new User(new MailAddress("alex.h.rao@gmail.com"), new Password("Hello"), "hi");
            Event e = new Event("   ", DateTime.Now, user, null);
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
