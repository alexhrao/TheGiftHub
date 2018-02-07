using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GiftServer.Data;
using GiftServer.Exceptions;
using System.Net.Mail;
using GiftServer.Security;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.IO;

namespace GiftServerTests
{
    [TestClass]
    public class UserTests
    {
        private static byte[] Image
        {
            get
            {
                string[] imgBytes = ("89 50 4e 47 0d 0a 1a 0a 00 00 00 0d 49 48 44 52 00 00 00 01 00 00 00 01 01 00 00 00 00 37 6e f9 24 00 00 00 10 49 44 41 54 78 " +
                    "9c 62 60 01 00 00 00 ff ff 03 00 00 06 00 05 57 bf ab d4 00 00 00 00 49 45 4e 44 ae 42 60 82").Split(' ');
                byte[] img = new byte[imgBytes.Length];
                for (int i = 0; i < imgBytes.Length; i++)
                {
                    // Fill in corr:
                    img[i] = Convert.ToByte(imgBytes[i], 16);
                }
                return img;
            }
        }
        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(UserNotFoundException))]
        public void UserInstantiate_ZeroID_ExceptionThrown()
        {
            User user = new User(0UL);
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(UserNotFoundException))]
        public void UserInstantiate_InvalidID_ExceptionThrown()
        {
            // User should not exist
            User user = new User(10UL);
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(UserNotFoundException))]
        public void UserInstantiate_InvalidHash_ExceptionThrown()
        {
            User user = new User("123445");
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(UserNotFoundException))]
        public void UserInstantiate_InvalidEmail_ExceptionThrown()
        {
            User user = new User(new MailAddress("myTest@hotmail.com"));
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidPasswordException))]
        public void UserInstantiate_InvalidPassword_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"), "HelloWorld");
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserInstantiate_NullEmail_ExceptionThrown()
        {
            User user = new User((MailAddress) null);
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserInstantiate_NullPassword_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"), (Password) null);
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserInstantiate_NullStringPassword_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"), (string) null);
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserInstantiate_EmptyPassword_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"), "");
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserInstantiate_NullHash_ExceptionThrown()
        {
            User user = new User((string) null);
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserInstantiate_EmptyHash_ExceptionThrown()
        {
            User user = new User("");
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("Successful")]
        [TestMethod]
        public void UserInstantiate_ValidCredentials_ExistingUser()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"), "MyPassword");
            Assert.IsNotNull(user, "Valid User was not initialized");
            Assert.AreEqual("Alex Rao", user.Name, "Incorrect User was fetched!");
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("Successful"), TestCategory("OAuth")]
        [TestMethod]
        public void UserInstantiate_ValidGoogleData_ExistingUser()
        {
            // Need valid (fake) GoogleUserID
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("Successful"), TestCategory("OAuth")]
        [TestMethod]
        public void UserInstantiate_ValidFacebookData_ExistingUser()
        {
            // Need valid (fake) FacebookUserID
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown"), TestCategory("OAuth")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserInstantiate_InvalidGoogleData_ExceptionThrown()
        {
            GoogleUser user = new GoogleUser("12345");
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown"), TestCategory("OAuth")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserInstantiate_InvalidFacebookData_ExceptionThrown()
        {
            FacebookUser user = new FacebookUser("12345");
        }



        [TestCategory("User"), TestCategory("Property"), TestCategory("Email"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserProperty_NullEmail_ExceptionThrown()
        {
            User user = new User(1)
            {
                Email = null
            };
        }

        [TestCategory("User"), TestCategory("Property"), TestCategory("Email"), TestCategory("Successful")]
        [TestMethod]
        public void UserProperty_ValidEmail_EmailChanged()
        {
            User user = new User(new MailAddress("alexhrao@google.com"))
            {
                Email = new MailAddress("alex.rao@southernco.edu")
            };
            Assert.AreEqual("alex.rao@southernco.edu", user.Email.Address, "Email was not changed!");
        }



        [TestCategory("User"), TestCategory("Property"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserProperty_NullName_ExceptionThrown()
        {
            User user = new User(1)
            {
                Name = null
            };
        }

        [TestCategory("User"), TestCategory("Property"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserProperty_EmptyName_ExceptionThrown()
        {
            User user = new User(1)
            {
                Name = ""
            };
        }

        [TestCategory("User"), TestCategory("Property"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserProperty_SpaceName_ExceptionThrown()
        {
            User user = new User(1)
            {
                Name = "   "
            };
        }

        [TestCategory("User"), TestCategory("Property"), TestCategory("Successful")]
        [TestMethod]
        public void UserProperty_ValidName_NameChanged()
        {
            User user = new User(1)
            {
                Name = "Ale-Alejandro"
            };
            Assert.AreEqual("Ale-Alejandro", user.Name, "Name was not changed");
        }



        [TestCategory("User"), TestCategory("Method"), TestCategory("Create"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserCreate_NoName_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@yahoo.com"), new Password("HelloWorld123"));
            user.Create();
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Create"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(DuplicateUserException))]
        public void UserCreate_DuplicateEmail_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"), new Password("HelloWorld123"))
            {
                Name = "Alejandro"
            };
            user.Create();
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Create"), TestCategory("Successful")]
        [TestMethod]
        public void UserCreate_ValidData_NewUser()
        {
            User user = new User(new MailAddress("alexhrao@gatech.com"), new Password("HelloWorld123"))
            {
                Name = "Alejandro"
            };
            Assert.IsFalse(user.DateJoined.HasValue);
            user.Create();
            Assert.AreNotEqual(user.ID, 0L, "UserID was not updated after creation");
            Assert.IsTrue(user.DateJoined.HasValue, "User Timestamp not updated");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Create"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(DuplicateUserException))]
        public void UserCreate_DuplicateGoogleID_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@thegifthub.org"), new Password("Helllllllo"))
            {
                GoogleId = "12345",
                Name = "tester"
            };
            user.Create();
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Create"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(DuplicateUserException))]
        public void UserCreate_DuplicateFacebookID_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@thegifthub.org"), new Password("Helllllllo"))
            {
                FacebookId = "12345",
                Name = "tester"
            };
            user.Create();
        }



        [TestCategory("User"), TestCategory("Method"), TestCategory("Update"), TestCategory("Successful")]
        [TestMethod]
        public void UserUpdate_ValidUser_NewName()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"))
            {
                Name = "Alejandro"
            };
            user.Update();
            User tester = new User(new MailAddress("alexhrao@gmail.com"));
            Assert.AreEqual("Alejandro", tester.Name, "Name not updated");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Update"), TestCategory("Successful")]
        [TestMethod]
        public void UserUpdate_ValidUser_NewEmail()
        {
            User user = new User(new MailAddress("alexhrao@gatech.edu"))
            {
                Email = new MailAddress("alexhrao@outlook.com")
            };
            user.Update();
            User tester = new User(new MailAddress("alexhrao@outlook.com"));
            Assert.AreEqual("alexhrao@outlook.com", tester.Email.Address, "Email not updated");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Update"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(DuplicateUserException))]
        public void UserUpdate_DuplicateEmail_ExceptionThrown()
        {
            User tester = new User(new MailAddress("arao81@gatech.edu"))
            {
                Email = new MailAddress("alexhrao@gmail.com")
            };
            tester.Update();
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Update"), TestCategory("Successful")]
        [TestMethod]
        public void UserUpdate_ValidUser_NewPassword()
        {
            User user = new User(2);
            user.UpdatePassword("HelloWorld2.0", new Action<MailAddress, User>((a, b) => { }));
            User tester = new User(2);
            Assert.IsTrue(tester.Password.Verify("HelloWorld2.0"), "Password not updated in DB");
            Assert.IsTrue(user.Password.Verify("HelloWorld2.0"), "Password not updated locally");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Update"), TestCategory("Successful")]
        [TestMethod]
        public void UserUpdate_ValidUser_NewBirthDay()
        {
            User user = new User(1)
            {
                BirthDay = 1,
                BirthMonth = 2
            };
            user.Update();
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Update"), TestCategory("Successful")]
        [TestMethod]
        public void UserUpdate_ValidUser_RemoveBirth()
        {
            User user = new User(1)
            {
                BirthDay = 0,
                BirthMonth = 0
            };
            user.Update();
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Update"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserUpdate_BirthMonthWithoutDay_ExceptionThrown()
        {
            User user = new User(1)
            {
                BirthMonth = 1,
                BirthDay = 0
            };
            user.Update();
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Update"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserUpdate_BirthDayWithoutMonth_ExceptionThrown()
        {
            User user = new User(1)
            {
                BirthMonth = 0,
                BirthDay = 1
            };
            user.Update();
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Update"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(DuplicateUserException))]
        public void UserUpdate_DuplicateGoogleID_ExceptionThrown()
        {
            User user = new User(6)
            {
                GoogleId = "12345"
            };
            user.Update();
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Create"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(DuplicateUserException))]
        public void UserUpdate_DuplicateFacebookID_ExceptionThrown()
        {
            User user = new User(6)
            {
                FacebookId = "12345"
            };
            user.Update();
        }



        [TestCategory("User"), TestCategory("Method"), TestCategory("Delete"), TestCategory("Successful")]
        [TestMethod]
        public void UserDelete_ValidUser_NoUser()
        {
            User user = new User(5);

            user.SaveImage(Image);
            user.Delete();
            // Make sure file DNE
            Assert.IsFalse(File.Exists(Directory.GetCurrentDirectory() + "/resources/images/users/User5.png"), "User Image not deleted");
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT UserID FROM users WHERE UserID = @uid OR UserEmail = @eml OR UserFacebookID = @fid OR UserGoogleID = @gid;";
                    cmd.Parameters.AddWithValue("@uid", 5);
                    cmd.Parameters.AddWithValue("@eml", user.Email.Address);
                    cmd.Parameters.AddWithValue("@fid", user.FacebookId);
                    cmd.Parameters.AddWithValue("@gid", user.GoogleId);
                    cmd.Prepare();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Assert.IsFalse(reader.HasRows, "User Information not fully deleted");
                    }
                }
            }
            Assert.AreEqual(user.ID, 0UL, "UserID was not reset to 0");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Delete"), TestCategory("Successful")]
        [TestMethod]
        public void UserDelete_DeleteTwice_NoUser()
        {
            User user = new User(new MailAddress("alexhrao@yahoo.edu"), new Password("HelloWorld123"))
            {
                Name = "Hello World"
            };
            user.Create();
            user.Delete();
            Assert.AreEqual(user.ID, 0UL, "UserID was not set to 0");
            user.Delete();
        }



        [TestCategory("User"), TestCategory("Method"), TestCategory("GetGroups"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetGroups_NullUser_ExceptionThrown()
        {
            User user = new User(1);
            user.GetGroups(null);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("GetGroups"), TestCategory("Successful")]
        [TestMethod]
        public void GetGroups_ValidCombination_GroupsFound()
        {
            User user = new User(1);
            User target = new User(2);
            List<Group> groups = user.GetGroups(target);
            // The following groups SHOULD be present:
            // GroupID 1
            // GroupID 2
            // GroupID 3
            Assert.IsTrue(groups.FindAll(g => g.ID == 1UL).Count == 1, "Unable to find Group with ID 1");
            Assert.IsTrue(groups.FindAll(g => g.ID == 2UL).Count == 1, "Unable to find Group with ID 2");
            Assert.IsTrue(groups.FindAll(g => g.ID == 3UL).Count == 1, "Unable to find Group with ID 3");
            foreach (Group g in groups)
            {
                Assert.IsTrue(g.ID == 1UL || g.ID == 2UL || g.ID == 3UL, "Group with ID " + g.ID + " was fetched");
            }
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("GetGroups"), TestCategory("Successful")]
        [TestMethod]
        public void GetGroups_ValidCombination_NoGroupsFound()
        {
            User user = new User(1);
            User target = new User(6UL);
            List<Group> groups = user.GetGroups(target);
            // groups should be empty
            foreach (Group g in groups)
            {
                Assert.Fail("Group with ID " + g.ID + " Was fetched when no groups are in common");
            }
        }



        [TestCategory("User"), TestCategory("Method"), TestCategory("GetGifts"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetGifts_NullUser_ExceptionThrown()
        {
            User user = new User(1);
            user.GetGifts((User)null);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("GetGifts"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetGifts_NullGroup_ExceptionThrown()
        {
            User user = new User(1);
            user.GetGifts((Group)null);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("GetGifts"), TestCategory("Successful")]
        [TestMethod]
        public void GetGifts_ValidUser_GiftFound()
        {
            User user = new User(1);
            User target = new User(2);
            List<Gift> gifts = user.GetGifts(target);
            // gifts should contain 1 element and it's ID is 2
            Assert.IsTrue(gifts.Count == 1, "Fetched " + gifts.Count + " Gifts; Should only fetch 1");
            Assert.AreEqual(gifts[0].ID, 2UL, "Incorrect ID of " + gifts[0].ID + " was fetched instead");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("GetGifts"), TestCategory("Successful")]
        [TestMethod]
        public void GetGifts_ValidUser_NoGifts()
        {
            User user = new User(1);
            User target = new User(4);
            List<Gift> gifts = user.GetGifts(target);
            // gifts should contain no elements
            Assert.IsTrue(gifts.Count == 0, gifts.Count + " Gifts fetched; expected 0");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("GetGifts"), TestCategory("Successful")]
        [TestMethod]
        public void GetGifts_ValidGroup_GiftsFound()
        {
            User user = new User(1);
            Group viewer = new Group(1);
            List<Gift> gifts = user.GetGifts(viewer);

            Assert.IsTrue(gifts.Count == 1, "Expected 1 gift; " + gifts.Count + " found");
            Assert.IsTrue(gifts[0].ID == 1, "Expected gift 1; Got gift " + gifts[0].ID);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("GetGifts"), TestCategory("Successful")]
        [TestMethod]
        public void GetGifts_ValidGroup_NoGifts()
        {
            User user = new User(2);
            Group viewer = new Group(4);
            List<Gift> gifts = user.GetGifts(viewer);

            Assert.IsTrue(gifts.Count == 0, "Expected no gifts, Got " + gifts.Count);
        }



        [TestCategory("User"), TestCategory("Method"), TestCategory("GetEvents"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetEvents_NullUser_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"));
            user.GetEvents((User) null);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("GetEvents"), TestCategory("Successful")]
        [TestMethod]
        public void GetEvents_ValidUser_EventFound()
        {
            User user = new User(1);
            User target = new User(2);
            List<Event> events = user.GetEvents(target);
            // Should get events 5, 6, 7
            Assert.IsTrue(events.FindAll(e => e.ID == 5UL).Count == 1, "Event 5 was not fetched");
            Assert.IsTrue(events.FindAll(e => e.ID == 6UL).Count == 1, "Event 6 was not fetched");
            Assert.IsTrue(events.FindAll(e => e.ID == 7UL).Count == 1, "Event 7 was not fetched");
            Assert.AreEqual(events.Count, 3, events.Count + " events were fetched; 3 expected");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("GetEvents"), TestCategory("Successful")]
        [TestMethod]
        public void GetEvents_ValidUser_NoEvents()
        {
            User user = new User(1);
            User target = new User(6);
            List<Event> events = user.GetEvents(target);
            Assert.AreEqual(events.Count, 0, "Events were fetched");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("GetEvents"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetEvents_NullGroup_ExceptionThrown()
        {
            User user = new User(1);
            user.GetEvents((Group) null);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("GetEvents"), TestCategory("Successful")]
        [TestMethod]
        public void GetEvents_ValidGroup_EventsFound()
        {
            User user = new User(1);
            Group target = new Group(1);
            List<Event> events = user.GetEvents(target);
            // Should find events 1-3
            Assert.IsTrue(events.Exists(e => e.ID == 1), "Event with ID 1 was not found");
            Assert.IsTrue(events.Exists(e => e.ID == 2), "Event with ID 2 was not found");
            Assert.IsTrue(events.Exists(e => e.ID == 3), "Event with ID 3 was not found");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("GetEvents"), TestCategory("Successful")]
        [TestMethod]
        public void GetEvents_ValidGroup_NoEvents()
        {
            User user = new User(1);
            Group viewer = new Group(4);
            List<Event> events = user.GetEvents(viewer);
            Assert.IsTrue(events.Count == 0, "Event count should be 0; got " + events.Count);
        }



        [TestCategory("User"), TestCategory("Method"), TestCategory("SaveImage"), TestCategory("Successful")]
        [TestMethod]
        public void SaveImage_NullInput_ImageRemoved()
        {
            User user = new User(4UL);
            user.SaveImage(Image);
            user.SaveImage(null);
            Assert.IsFalse(File.Exists(Directory.GetCurrentDirectory() + "/resources/images/users/User" + user.ID + ".png"), "User Image not deleted");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("SaveImage"), TestCategory("Successful")]
        [TestMethod]
        public void SaveImage_EmptyInput_ImageRemoved()
        {
            User user = new User(3UL);
            user.SaveImage(Image);
            byte[] arr = new byte[0];
            user.SaveImage(arr);
            Assert.IsFalse(File.Exists(Directory.GetCurrentDirectory() + "/resources/images/users/User" + user.ID + ".png"), "User Image not deleted");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("SaveImage"), TestCategory("Successful")]
        [TestMethod]
        public void SaveImage_ByteInput_ImageSaved()
        {
            User user = new User(2UL);
            user.SaveImage(Image);
            Assert.IsTrue(File.Exists(Directory.GetCurrentDirectory() + "/resources/images/users/User" + user.ID + ".png"), "User Image not saved");
        }



        [TestCategory("User"), TestCategory("Method"), TestCategory("RemoveImage"), TestCategory("Successful")]
        [TestMethod]
        public void RemoveImage_NoSavedImage_NoException()
        {
            User user = new User(6UL);
            user.RemoveImage();
            Assert.IsFalse(File.Exists(Directory.GetCurrentDirectory() + "/resources/images/users/User" + user.ID + ".png"), "User Image not deleted");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("RemoveImage"), TestCategory("Successful")]
        [TestMethod]
        public void RemoveImage_SavedImage_ImageRemoved()
        {
            User user = new User(7UL);
            user.RemoveImage();
            Assert.IsFalse(File.Exists(Directory.GetCurrentDirectory() + "/resources/images/users/User" + user.ID + ".png"), "User Image not deleted");
        }



        [ClassCleanup]
        public static void UserCleanup()
        {
            Reset();
        }

        [ClassInitialize]
        public static void UserInitialize(TestContext ctx)
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
                    cmd.CommandText = "CALL gift_registry_db_test.delete();";
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "CALL gift_registry_db_test.load();";
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
