using System;
using System.Collections.Generic;
using System.Net.Mail;
using GiftServer.Data;
using GiftServer.Exceptions;
using GiftServer.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GiftServerTests
{
    [TestClass]
    public class GroupTests
    {
        [TestCategory("Group"), TestCategory("Initialize")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GroupInitialize_ZeroID_ExceptionThrown()
        {
            Group group = new Group(0);
        }

        [TestCategory("Group"), TestCategory("Initialize")]
        [TestMethod]
        [ExpectedException(typeof(GroupNotFoundException))]
        public void GroupInitialize_InvalidID_ExceptionThrown()
        {
            Group group = new Group(100);
        }

        [TestCategory("Group"), TestCategory("Initialize")]
        [TestMethod]
        public void GroupInitialize_ValidID_Success()
        {
            Group group = new Group(1);
        }

        [TestCategory("Group"), TestCategory("Initialize")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GroupInitialize_NullAdmin_ExceptionThrown()
        {
            Group group = new Group(null, "HelloWorld");
        }

        [TestCategory("Group"), TestCategory("Initialize")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GroupInitialize_NullName_ExceptionThrown()
        {
            Group group = new Group(new User(1), null);
        }

        [TestCategory("Group"), TestCategory("Initialize")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GroupInitialize_EmptyName_ExceptionThrown()
        {
            Group group = new Group(new User(1), "");
        }

        [TestCategory("Group"), TestCategory("Initialize")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GroupInitialize_SpaceName_ExceptionThrown()
        {
            Group group = new Group(new User(1), "     ");
        }

        [TestCategory("Group"), TestCategory("Initialize")]
        [TestMethod]
        public void GroupInitialize_ValidData_NewGroup()
        {
            Group group = new Group(new User(1), "Hello World");
            Assert.AreEqual("Hello World", group.Name, "Name not set correctly");
            Assert.AreEqual(0UL, group.ID, "GroupID Not correctly initialized");
            Assert.AreEqual(new User(1), group.Admin, "Admin not correctly set");
            Assert.AreEqual(1, group.Members.Count, "User Count is not correct");
        }

        [TestCategory("Group"), TestCategory("Initialize")]
        [TestMethod]
        public void GroupInitialize_NullUsers_NewGroup()
        {
            Group group = new Group(new User(1), "Hello World", null);
            Assert.AreEqual("Hello World", group.Name, "Name not set correctly");
            Assert.AreEqual(0UL, group.ID, "GroupID Not correctly initialized");
            Assert.AreEqual(new User(1), group.Admin, "Admin not correctly set");
            Assert.AreEqual(1, group.Members.Count, "User Count is not correct");
        }

        [TestCategory("Group"), TestCategory("Initialize")]
        [TestMethod]
        public void GroupInitialize_ZeroUsers_NewGroup()
        {
            Group group = new Group(new User(1), "Hello World", new List<Member>());
            Assert.AreEqual("Hello World", group.Name, "Name not set correctly");
            Assert.AreEqual(0UL, group.ID, "GroupID Not correctly initialized");
            Assert.AreEqual(new User(1), group.Admin, "Admin not correctly set");
            Assert.AreEqual(1, group.Members.Count, "User Count is not correct");
        }

        [TestCategory("Group"), TestCategory("Initialize")]
        [TestMethod]
        public void GroupInitialize_TryAddAdminAsUser_NewGroup()
        {
            List<Member> list = new List<Member>
            {
                new User(1),
                new User(2),
                new User(3)
            };
            Group group = new Group(new User(1), "Hello World", list);
            Assert.AreEqual("Hello World", group.Name, "Name not set correctly");
            Assert.AreEqual(0UL, group.ID, "GroupID Not correctly initialized");
            Assert.AreEqual(new User(1), group.Admin, "Admin not correctly set");
            Assert.AreEqual(3, group.Members.Count, "User Count is not correct");
        }

        [TestCategory("Group"), TestCategory("Initialize")]
        [TestMethod]
        public void GroupInitialize_ValidListOfUsers_NewGroup()
        {
            List<Member> list = new List<Member>
            {
                new User(2),
                new User(3)
            };
            Group group = new Group(new User(1), "Hello World", list);
            Assert.AreEqual("Hello World", group.Name, "Name not set correctly");
            Assert.AreEqual(0UL, group.ID, "GroupID Not correctly initialized");
            Assert.AreEqual(new User(1), group.Admin, "Admin not correctly set");
            Assert.AreEqual(3, group.Members.Count, "User Count is not correct");
        }

        [TestCategory("Group"), TestCategory("Property")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Group_NullName_ExceptionThrown()
        {
            Group group = new Group(1)
            {
                Name = null
            };
        }

        [TestCategory("Group"), TestCategory("Property")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Group_EmptyName_ExceptionThrown()
        {
            Group group = new Group(1)
            {
                Name = ""
            };
        }

        [TestCategory("Group"), TestCategory("Property")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Group_SpaceName_ExceptionThrown()
        {
            Group group = new Group(1)
            {
                Name = "   "
            };
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
