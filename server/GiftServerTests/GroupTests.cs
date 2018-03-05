using GiftServer.Data;
using GiftServer.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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
            Assert.AreEqual(3, group.Members.Count, "Incorrect number of members");
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

        [TestCategory("Group"), TestCategory("Property"), TestCategory("Name")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Group_NullName_ExceptionThrown()
        {
            Group group = new Group(1)
            {
                Name = null
            };
        }

        [TestCategory("Group"), TestCategory("Property"), TestCategory("Name")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Group_EmptyName_ExceptionThrown()
        {
            Group group = new Group(1)
            {
                Name = ""
            };
        }

        [TestCategory("Group"), TestCategory("Property"), TestCategory("Name")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Group_SpaceName_ExceptionThrown()
        {
            Group group = new Group(1)
            {
                Name = "   "
            };
        }

        [TestCategory("Group"), TestCategory("Property"), TestCategory("Name")]
        [TestMethod]
        public void Group_ValidName_NameChanged()
        {
            Group group = new Group(1)
            {
                Name = "Tester123"
            };
            Assert.AreEqual("Tester123", group.Name, "Name mismatch");
        }

        [TestCategory("Group"), TestCategory("Property"), TestCategory("Event")]
        [TestMethod]
        public void Group_ValidID_OnlyAdminReturned()
        {
            Group group = new Group(5);
            List<Member> list = group.Members;
            Assert.AreEqual(1, list.Count, "Invalid number of users fetched");
            Assert.AreEqual(10UL, list[0].ID, "Wrong Member fetched");
        }

        [TestCategory("Group"), TestCategory("Property"), TestCategory("User")]
        [TestMethod]
        public void Group_ValidID_OneMemberReturned()
        {
            Group group = new Group(6);
            List<Member> members = group.Members;
            Assert.AreEqual(2, members.Count, "Wrong number of users fetched");
            Assert.IsTrue(members.Exists(m => m.ID == 10), "Admin not fetched");
            Assert.IsTrue(members.Exists(m => m.ID == 9), "Member not fetched");
        }

        [TestCategory("Group"), TestCategory("Property"), TestCategory("User")]
        [TestMethod]
        public void Group_ValidID_MembersReturned()
        {
            Group group = new Group(2);
            List<Member> list = group.Members;
            Assert.AreEqual(6, list.Count, "Invalid number of members fetched");
            Assert.IsTrue(list.Exists(m => m.ID == 1), "Member 1 DNE");
            Assert.IsTrue(list.Exists(m => m.ID == 2), "Member 2 DNE");
            Assert.IsTrue(list.Exists(m => m.ID == 3), "Member 3 DNE");
            Assert.IsTrue(list.Exists(m => m.ID == 4), "Member 4 DNE");
            Assert.IsTrue(list.Exists(m => m.ID == 5), "Member 5 DNE");
            Assert.IsTrue(list.Exists(m => m.ID == 7), "Member 7 DNE");
        }

        [TestCategory("Group"), TestCategory("Property"), TestCategory("Event")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Group_ZeroIDEvents_ExceptionThrown()
        {
            Group group = new Group(new User(1), "Hello World");
            List<Event> list = group.Events;
        }

        [TestCategory("Group"), TestCategory("Property"), TestCategory("Event")]
        [TestMethod]
        public void Group_ValidID_NoEventReturned()
        {
            Group group = new Group(3);
            List<Event> list = group.Events;
            Assert.AreEqual(0, list.Count, "Invalid number of events fetched");
        }

        [TestCategory("Group"), TestCategory("Property"), TestCategory("Event")]
        [TestMethod]
        public void Group_ValidID_OneEventReturned()
        {
            Group group = new Group(4);
            List<Event> list = group.Events;
            Assert.AreEqual(1, list.Count, "Invalid number of events fetched");
            Assert.AreEqual(8UL, list[0].ID, "Wrong event fetched");
        }

        [TestCategory("Group"), TestCategory("Property"), TestCategory("Event")]
        [TestMethod]
        public void Group_ValidID_EventsReturned()
        {
            Group group = new Group(2);
            List<Event> list = group.Events;
            Assert.AreEqual(3, list.Count, "Invalid number of events fetched");
            Assert.IsTrue(list.Exists(e => e.ID == 4), "Event 4 DNE");
            Assert.IsTrue(list.Exists(e => e.ID == 6), "Event 6 DNE");
            Assert.IsTrue(list.Exists(e => e.ID == 7), "Event 7 DNE");
        }

        [TestCategory("Group"), TestCategory("Property"), TestCategory("Gift")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Group_ZeroIDGifts_ExceptionThrown()
        {
            Group group = new Group(new User(1), "Hello World");
            List<Gift> list = group.Gifts;
        }

        [TestCategory("Group"), TestCategory("Property"), TestCategory("Gift")]
        [TestMethod]
        public void Group_ValidID_NoGiftsReturned()
        {
            Group group = new Group(3);
            List<Gift> list = group.Gifts;
            Assert.AreEqual(0, list.Count, "Invalid number of gifts fetched");
        }

        [TestCategory("Group"), TestCategory("Property"), TestCategory("Gift")]
        [TestMethod]
        public void Group_ValidID_OneGiftReturned()
        {
            Group group = new Group(4);
            List<Gift> list = group.Gifts;
            Assert.AreEqual(1, list.Count, "Invalid number of gifts fetched");
            Assert.AreEqual(7UL, list[0].ID, "Wrong gift fetched");
        }

        [TestCategory("Group"), TestCategory("Property"), TestCategory("Gift")]
        [TestMethod]
        public void Group_ValidID_GiftsReturned()
        {
            Group group = new Group(2);
            List<Gift> list = group.Gifts;
            Assert.AreEqual(4, list.Count, "Invalid number of gifts fetched");
            Assert.IsTrue(list.Exists(e => e.ID == 1), "Gift 1 DNE");
            Assert.IsTrue(list.Exists(e => e.ID == 2), "Gift 2 DNE");
            Assert.IsTrue(list.Exists(e => e.ID == 5), "Gift 5 DNE");
            Assert.IsTrue(list.Exists(e => e.ID == 8), "Gift 8 DNE");
        }

        [TestCategory("Group"), TestCategory("Method"), TestCategory("Create")]
        [TestMethod]
        public void GroupCreate_OnlyAdmin_NewGroup()
        {
            Group group = new Group(new User(11), "Only the Admin!");
            group.Create();
            Assert.AreNotEqual(0UL, group.ID, "ID Not updated");
            Group tester = new Group(group.ID);
            Assert.AreEqual(new User(11), tester.Admin, "Admin not updated");
            Assert.AreEqual(1, tester.Members.Count, "Members not updated correctly");
            Assert.AreEqual(new Member(new User(11), false), tester.Members[0], "Incorrect member fetched");
        }

        [TestCategory("Group"), TestCategory("Method"), TestCategory("Create")]
        [TestMethod]
        public void GroupCreate_AdminPlusMembers_NewGroup()
        {
            Group group = new Group(new User(11), "Members Too!", new List<Member>()
            {
                new User(1),
                new User(2)
            });
            group.Create();
            Assert.AreNotEqual(0UL, group.ID, "ID Not Updated");
            Group tester = new Group(group.ID);
            Assert.AreEqual(new User(11), tester.Admin, "Admin not updated");
            Assert.AreEqual(3, tester.Members.Count, "Members not updated correctly");
            Assert.IsTrue(tester.Members.Exists(m => m.User.ID == 1), "User 1 not updated");
            Assert.IsTrue(tester.Members.Exists(m => m.User.ID == 2), "User 2 not updated");
        }

        [TestCategory("Group"), TestCategory("Method"), TestCategory("Update")]
        [TestMethod]
        public void GroupUpdate_ChangeData_DataChanged()
        {
            new Group(5)
            {
                Name = "wazzup"
            }.Update();
            Group tester = new Group(5);
            Assert.AreEqual("wazzup", tester.Name, "Name not updated");
            new Group(4).TransferAdmin(new User(5));
            // Checking TransferAdmin happens later...
            Group adminTester = new Group(4);
            Assert.AreEqual(new User(5), adminTester.Admin, "Admin not updated");
        }

        [TestCategory("Group"), TestCategory("Method"), TestCategory("Delete")]
        [TestMethod]
        public void GroupDelete_ValidGroup_GroupDeleted()
        {
            Group group = new Group(7);
            group.Delete();
            // TODO: Add checking for deleted event, gift, and user records
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
