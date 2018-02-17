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
using System.Xml;

namespace GiftServerTests
{
    [TestClass]
    public class UserTests
    {

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(UserNotFoundException))]
        public void UserInstantiate_ZeroID_ExceptionThrown()
        {
            User user = new User(0);
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(UserNotFoundException))]
        public void UserInstantiate_InvalidID_ExceptionThrown()
        {
            // User should not exist
            User user = new User(1000);
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
            User user = new User(new MailAddress("alexhrao@gmail.com"), null, "hello");
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserInstantiate_NullName_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"), new Password("Hello World"), null);
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserInstantiate_EmptyName_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"), new Password("Hello World"), "");
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserInstantiate_SpaceName_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"), new Password("Hello World"), "   ");
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserInstantiate_NullStringPassword_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"), null);
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
        /*
         * This should be handled in GoogleUser, FacebookUser tests, respectively
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
        */

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown"), TestCategory("OAuth")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserInstantiate_NullOAuth_ExceptionThrown()
        {
            User user = new User((OAuthUser)null, (Action<MailAddress>)null);
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown"), TestCategory("OAuth")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserInstantiate_NullOAuthValidPass_ExceptionThrown()
        {
            new User((OAuthUser)null, "HelloWorld123");
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown"), TestCategory("OAuth")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserInstantiate_NullOAuthNullPass_ExceptionThrown()
        {
            new User((OAuthUser)null, (string)null);
        }

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("ExceptionThrown"), TestCategory("OAuth")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserInstantiate_ValidOAuthNullPass_ExceptionThrown()
        {
            // Construct valid GoogleOAuthToken?
            new User((OAuthUser)null, (string)null);
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
            Assert.AreEqual(user.Name, "Ale-Alejandro", "Name was not changed");
        }

        [TestCategory("User"), TestCategory("Property"), TestCategory("Successful")]
        [TestMethod]
        public void UserProperty_Gifts_GiftReturned()
        {
            User user = new User(7);
            List<Gift> gifts = user.Gifts;
            Assert.IsTrue(gifts.Count == 1, "Fetched too many gifts; 1 expected, got " + gifts.Count);
            Assert.AreEqual(4UL, gifts[0].ID, "Wrong gift fetched");
        }

        [TestCategory("User"), TestCategory("Property"), TestCategory("Successful")]
        [TestMethod]
        public void UserProperty_Gifts_NoGifts()
        {
            User user = new User(3);
            List<Gift> gifts = user.Gifts;
            Assert.IsTrue(gifts.Count == 0, "Gifts returned when none should have been");
        }

        [TestCategory("User"), TestCategory("Property"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserProperty_GiftsNoID_ExceptionThrown()
        {
            User user = new User(new MailAddress("hellofdsa@gmail.com"), new Password("Hello"), "fdsa");
            List<Gift> g = user.Gifts;
        }

        [TestCategory("User"), TestCategory("Property"), TestCategory("Successful")]
        [TestMethod]
        public void UserProperty_Groups_GroupsReturned()
        {
            User user = new User(1);
            List<Group> groups = user.Groups;
            Assert.IsTrue(groups.Count == 3, "Expected 3 groups, got " + groups.Count);
            Assert.IsTrue(groups.Exists(g => g.ID == 1), "Group 1 was not fetched");
            Assert.IsTrue(groups.Exists(g => g.ID == 2), "Group 2 was not fetched");
            Assert.IsTrue(groups.Exists(g => g.ID == 3), "Group 3 was not fetched");
        }

        [TestCategory("User"), TestCategory("Property"), TestCategory("Successful")]
        [TestMethod]
        public void UserProperty_Groupts_NoGroups()
        {
            User user = new User(7);
            Assert.IsTrue(user.Groups.Count == 0, "Groups fetched when none should have been");
        }

        [TestCategory("User"), TestCategory("Property"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserProperty_GroupsNoID_ExceptionThrown()
        {
            User user = new User(new MailAddress("hellofdsa@gmail.com"), new Password("Hello"), "fdsa");
            List<Group> g = user.Groups;
        }

        [TestCategory("User"), TestCategory("Property"), TestCategory("Successful")]
        [TestMethod]
        public void UserProperty_Events_EventsReturned()
        {
            User user = new User(1);
            List<Event> events = user.Events;
            Assert.IsTrue(events.Count == 4, "Incorrect number of events fetched; expected 4, got " + events.Count);
            Assert.IsTrue(events.Exists(e => e.ID == 1), "Event 1 was not fetched");
            Assert.IsTrue(events.Exists(e => e.ID == 2), "Event 2 was not fetched");
            Assert.IsTrue(events.Exists(e => e.ID == 3), "Event 3 was not fetched");
            Assert.IsTrue(events.Exists(e => e.ID == 4), "Event 4 was not fetched");
        }

        [TestCategory("User"), TestCategory("Property"), TestCategory("Successful")]
        [TestMethod]
        public void UserProperty_Events_NoEvents()
        {
            User user = new User(6);
            Assert.IsTrue(user.Events.Count == 0, "Events fetched when none should have");
        }

        [TestCategory("User"), TestCategory("Property"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserProperty_EventsNoID_ExceptionThrown()
        {
            User user = new User(new MailAddress("hellofdsa@gmail.com"), new Password("Hello"), "fdsa");
            List<Event> e = user.Events;
        }

        [TestCategory("User"), TestCategory("Property"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserProperty_ReservationsNoID_ExceptionThrown()
        {
            User user = new User(new MailAddress("fdsafdsa@fdsafda.fdsa"), new Password("hfdsa"), "Ffdsa");
            List<Reservation> res = user.Reservations;
        }

        [TestCategory("User"), TestCategory("Property"), TestCategory("Successful")]
        [TestMethod]
        public void UserProperty_Reservations_NoReservations()
        {
            User user = new User(8);
            List<Reservation> res = user.Reservations;
            Assert.IsTrue(res.Count == 0, "Fetched non-existent reservations");
        }
        [TestCategory("User"), TestCategory("Property"), TestCategory("Successful")]
        [TestMethod]
        public void UserProperty_Reservations_Reservations()
        {
            User user = new User(9);
            List<Reservation> res = user.Reservations;
            Assert.AreEqual(1, res.Count, "Fetched a different number of records; expected 1, got " + res.Count);
        }



        [TestCategory("User"), TestCategory("Method"), TestCategory("Create"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(DuplicateUserException))]
        public void UserCreate_DuplicateEmail_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"), new Password("HelloWorld123"), "alejandro")
            {
                Name = "Alejandro"
            };
            user.Create();
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Create"), TestCategory("Successful")]
        [TestMethod]
        public void UserCreate_ValidData_NewUser()
        {
            User user = new User(new MailAddress("alexhrao@gatech.com"), new Password("HelloWorld123"), "Alejandro");
            Assert.IsFalse(user.DateJoined.HasValue);
            user.Create();
            Assert.AreNotEqual(0L, user.ID, "UserID was not updated after creation");
            Assert.IsTrue(user.DateJoined.HasValue, "User Timestamp not updated");
            User tester = new User(user.ID);
            Assert.IsNotNull(tester.Password, "Password was not sent");
            Assert.IsNotNull(tester.Email, "Email was not sent");
            Assert.IsNotNull(tester.Name, "Name was not sent");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Create"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(DuplicateUserException))]
        public void UserCreate_DuplicateGoogleID_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@thegifthub.org"), new Password("Helllllllo"), "Hi")
            {
                GoogleId = "12345"
            };
            user.Create();
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Create"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(DuplicateUserException))]
        public void UserCreate_DuplicateFacebookID_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@thegifthub.org"), new Password("Helllllllo"), "tester")
            {
                FacebookId = "12345"
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


        [TestCategory("User"), TestCategory("Method"), TestCategory("Delete"), TestCategory("Exception Thrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserDelete_Admin_ExceptionThrown()
        {
            User user = new User(1);
            user.Delete();
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Delete"), TestCategory("Successful")]
        [TestMethod]
        public void UserDelete_ValidUser_NoUser()
        {
            User user = new User(5);

            user.SaveImage(TestManager.Image);
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
            Assert.AreEqual(0UL, user.ID, "UserID was not reset to 0");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Delete"), TestCategory("Successful")]
        [TestMethod]
        public void UserDelete_DeleteTwice_NoUser()
        {
            User user = new User(new MailAddress("alexhrao@yahoo.edu"), new Password("HelloWorld123"), "Hello World");
            user.Create();
            user.Delete();
            Assert.AreEqual(0UL, user.ID, "UserID was not set to 0");
            user.Delete();
        }



        [TestCategory("User"), TestCategory("Method"), TestCategory("SaveImage"), TestCategory("Successful")]
        [TestMethod]
        public void SaveImage_NullInput_ImageRemoved()
        {
            User user = new User(4);
            user.SaveImage(TestManager.Image);
            user.SaveImage(null);
            Assert.IsFalse(File.Exists(Directory.GetCurrentDirectory() + "/resources/images/users/User" + user.ID + ".png"), "User Image not deleted");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("SaveImage"), TestCategory("Successful")]
        [TestMethod]
        public void SaveImage_EmptyInput_ImageRemoved()
        {
            User user = new User(3);
            user.SaveImage(TestManager.Image);
            byte[] arr = new byte[0];
            user.SaveImage(arr);
            Assert.IsFalse(File.Exists(Directory.GetCurrentDirectory() + "/resources/images/users/User" + user.ID + ".png"), "User Image not deleted");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("SaveImage"), TestCategory("Successful")]
        [TestMethod]
        public void SaveImage_ByteInput_ImageSaved()
        {
            User user = new User(2);
            user.SaveImage(TestManager.Image);
            Assert.IsTrue(File.Exists(Directory.GetCurrentDirectory() + "/resources/images/users/User" + user.ID + ".png"), "User Image not saved");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("SaveImage"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SaveImage_DeletedUser_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@code.com"), new Password("Hello World"), "Hello");
            user.SaveImage(TestManager.Image);
        }



        [TestCategory("User"), TestCategory("Method"), TestCategory("RemoveImage"), TestCategory("Successful")]
        [TestMethod]
        public void RemoveImage_NoSavedImage_NoException()
        {
            User user = new User(6);
            user.RemoveImage();
            Assert.IsFalse(File.Exists(Directory.GetCurrentDirectory() + "/resources/images/users/User" + user.ID + ".png"), "User Image not deleted");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("RemoveImage"), TestCategory("Successful")]
        [TestMethod]
        public void RemoveImage_SavedImage_ImageRemoved()
        {
            User user = new User(7);
            user.RemoveImage();
            Assert.IsFalse(File.Exists(Directory.GetCurrentDirectory() + "/resources/images/users/User" + user.ID + ".png"), "User Image not deleted");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("RemoveImage"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveImage_DeletedUser_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@code.com"), new Password("Hello World"), "Hello");
            user.RemoveImage();
        }



        [TestCategory("User"), TestCategory("Method"), TestCategory("GetImage"), TestCategory("Successful")]
        [TestMethod]
        public void GetImage_ValidUser_CustomImage()
        {
            User user = new User(6);
            user.SaveImage(TestManager.Image);
            string path = user.GetImage();
            Assert.AreEqual(Path.GetFileNameWithoutExtension(path), "User6", "Expected custom image, got " + Path.GetFileNameWithoutExtension(path));
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("GetImage"), TestCategory("Successful")]
        [TestMethod]
        public void GetImage_ValidUser_DefaultImage()
        {
            User user = new User(7);
            user.RemoveImage();
            string path = user.GetImage();
            Assert.AreEqual("default", Path.GetFileNameWithoutExtension(path), "Expected default image, got " + Path.GetFileNameWithoutExtension(path));
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("GetImage"), TestCategory("Successful")]
        [TestMethod]
        public void GetImage_ValidID_CustomImage()
        {
            User user = new User(1);
            user.SaveImage(TestManager.Image);
            string path = User.GetImage(1);
            Assert.AreEqual("User1", Path.GetFileNameWithoutExtension(path), "Expected custom image, got " + Path.GetFileNameWithoutExtension(path));
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("GetImage"), TestCategory("Successful")]
        [TestMethod]
        public void GetImage_ValidID_DefaultImage()
        {
            User user = new User(2);
            user.RemoveImage();
            string path = User.GetImage(2);
            Assert.AreEqual("default", Path.GetFileNameWithoutExtension(path), "Expected default image, got " + Path.GetFileNameWithoutExtension(path));
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("GetImage"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetImage_ZeroID_ExceptionThrown()
        {
            User.GetImage(0);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("GetImage"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetImage_DeletedUser_ExceptionThrown()
        {
            User user = new User(new MailAddress("fdsa@asdf.edu"), new Password("Hello World!"), "hello world");
            user.Create();
            user.Delete();
            user.GetImage();
        }



        [TestCategory("User"), TestCategory("Method"), TestCategory("Reserve"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReserveOne_NullGift_ExceptionThrown()
        {
            User user = new User(1);
            user.Reserve(null);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Reserve"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReserveMany_NullGift_ExceptionThrown()
        {
            User user = new User(1);
            user.Reserve(null, 10);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Reserve"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ReservationOverflowException))]
        public void ReserveOne_FullReservations_ExceptionThrown()
        {
            User user = new User(3);
            user.Reserve(new Gift(6));
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Reserve"), TestCategory("Successful")]
        [TestMethod]
        public void ReserveMany_FullReservations_ZeroReturned()
        {
            User user = new User(3);
            int res = user.Reserve(new Gift(6), 5);
            Assert.AreEqual(0, res, "Expected no reservations, " + res + " made");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Reserve"), TestCategory("Successful")]
        [TestMethod]
        public void ReserveOne_ValidGift_GiftReserved()
        {
            User user = new User(4);
            user.Reserve(new Gift(7));
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Reserve"), TestCategory("Successful")]
        [TestMethod]
        public void ReserveThree_ValidGift_GiftReserved()
        {
            User user = new User(1);
            int numReserve = user.Reserve(new Gift(7), 3);
            Assert.AreEqual(3, numReserve, "Expected 3 gifts; " + numReserve + " were reserved");
        }



        [TestCategory("User"), TestCategory("Method"), TestCategory("Release"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReleaseOne_NullGift_ExceptionThrown()
        {
            User user = new User(1);
            user.Release(null);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Release"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReleaseMany_NullGift_ExceptionThrown()
        {
            User user = new User(1);
            user.Release(null, 10);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Release"), TestCategory("Successful")]
        [TestMethod]
        public void ReleaseOne_ValidGift_NoReservation()
        {
            User user = new User(1);
            Gift gift = new Gift(7);
            user.Release(gift);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Release"), TestCategory("Successful")]
        [TestMethod]
        public void ReleaseOne_NoReservations_NoChange()
        {
            User user = new User(1);
            Gift gift = new Gift(8);
            user.Release(gift);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Release"), TestCategory("Successful")]
        [TestMethod]
        public void ReleaseMany_ValidGift_AllReleased()
        {
            User user = new User(4);
            Gift gift = new Gift(5);
            int released = user.Release(gift, 5);
            Assert.AreEqual(5, released, "Expected release of 5 gifts; only released " + released);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Release"), TestCategory("Successful")]
        [TestMethod]
        public void ReleaseMany_ValidGift_PartialReleased()
        {
            User user = new User(1);
            Gift gift = new Gift(3);
            int released = user.Release(gift, 5);
            Assert.AreEqual(3, released, "Expected release of 3 gifts; got " + released);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Release"), TestCategory("Successful")]
        [TestMethod]
        public void ReleaseMany_NoReservations_NoChange()
        {
            User user = new User(1);
            Gift gift = new Gift(8);
            int released = user.Release(gift, 10);
            Assert.AreEqual(0, released, "Expected no releases, got " + released);
        }



        [TestCategory("User"), TestCategory("Method"), TestCategory("Purchase"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PurchaseOne_NullGift_ExceptionThrown()
        {
            User user = new User(1);
            user.Purchase(null);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Purchase"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PurchaseMany_NullGift_ExceptionThrown()
        {
            User user = new User(1);
            user.Purchase(null, 3);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Purchase"), TestCategory("Successful")]
        [TestMethod]
        public void PurchaseOne_ValidGift_OnePurchase()
        {
            User user = new User(6);
            user.Purchase(new Gift(7));
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Purchase"), TestCategory("Successful")]
        [TestMethod]
        public void PurchaseMany_ValidGift_FullPurchase()
        {
            User user = new User(3);
            int purchased = user.Purchase(new Gift(2), 3);
            Assert.AreEqual(3, purchased, "Did not purchase enough gifts");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Purchase"), TestCategory("Successful")]
        [TestMethod]
        public void PurchaseMany_ValidGift_PartialPurchase()
        {
            User user = new User(1);
            int purchased = user.Purchase(new Gift(6), 5);
            Assert.AreEqual(1, purchased, "Purchased " + purchased + " When one was expected");
        }



        [TestCategory("User"), TestCategory("Method"), TestCategory("Return"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReturnOne_NullGift_ExceptionThrown()
        {
            User user = new User(1);
            user.Return(null);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Return"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReturnMany_NullGift_ExceptionThrown()
        {
            User user = new User(1);
            user.Return(null, 10);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Return"), TestCategory("Successful")]
        [TestMethod]
        public void ReturnOne_ValidGift_OneReturn()
        {
            User user = new User(2);
            Gift gift = new Gift(8);
            user.Return(gift);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Return"), TestCategory("Successful")]
        [TestMethod]
        public void ReturnMany_ValidGift_FullReturn()
        {
            User user = new User(2);
            Gift gift = new Gift(7);
            int returned = user.Return(gift, 4);
            Assert.AreEqual(4, returned, "Expected to return 4, returned " + returned);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Return"), TestCategory("Successful")]
        [TestMethod]
        public void ReturnMany_ValidGift_PartialReturn()
        {
            User user = new User(1);
            Gift gift = new Gift(2);
            int returned = user.Return(gift, 7);
            Assert.AreEqual(3, returned, "Expected to return 3, returned " + returned);
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
            Assert.AreEqual(2UL, gifts[0].ID, "Incorrect ID of " + gifts[0].ID + " was fetched instead");
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
            Assert.AreEqual(3, events.Count, events.Count + " events were fetched; 3 expected");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("GetEvents"), TestCategory("Successful")]
        [TestMethod]
        public void GetEvents_ValidUser_NoEvents()
        {
            User user = new User(1);
            User target = new User(6);
            List<Event> events = user.GetEvents(target);
            Assert.AreEqual(0, events.Count, "Events were fetched");
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



        [TestCategory("User"), TestCategory("Method"), TestCategory("AddOAuth"), TestCategory("OAuth"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddOAuth_NullToken_ExceptionThrown()
        {
            User user = new User(9);
            user.AddOAuth(null);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("AddOAuth"), TestCategory("OAuth"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AddOAuth_ZeroID_ExceptionThrown()
        {
            User user = new User(new MailAddress("wasssup@gmail.com"), new Password("hello world"), "hello")
            {
                Name = "hello"
            };
            // Find way to get valid OAuth Token
            // user.AddOAuth(GoogleUser);
            throw new InvalidOperationException();
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("AddOAuth"), TestCategory("OAuth"), TestCategory("Successful")]
        [TestMethod]
        public void AddOAuth_ValidOAuth_OAuthAdded()
        {
            User user = new User(9);
            // user.AddOAuth(new GoogleUser(TOKEN));
        }



        [TestCategory("User"), TestCategory("Method"), TestCategory("RemoveOAuth"), TestCategory("OAuth"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveOAuth_NullToken_ExceptionThrown()
        {
            User user = new User(9);
            user.RemoveOAuth(null);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("RemoveOAuth"), TestCategory("OAuth"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveOAuth_ZeroID_ExceptionThrown()
        {
            User user = new User(new MailAddress("wasssup@gmail.com"), new Password("hello world"), "Hello");
            // Find way to get valid OAuth Token
            // user.RemoveOAuth(null);
            throw new InvalidOperationException();
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("RemoveOAuth"), TestCategory("OAuth"), TestCategory("Successful")]
        [TestMethod]
        public void RemoveOAuth_ValidOAuth_OAuthAdded()
        {
            User user = new User(9);
            // user.RemoveOAuth(new GoogleUser(TOKEN));
        }



        [TestCategory("User"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void UserEquals_NullObject_False()
        {
            User user = new User(1);
            Assert.IsFalse(user.Equals((object)null), "Null Object reported equal to User");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void UserEquals_NonUser_False()
        {
            User user = new User(1);
            Assert.IsFalse(user.Equals(1), "Non-User reported equal to User");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void UserEquals_NullUser_False()
        {
            User user = new User(1);
            Assert.IsFalse(user.Equals((User)null), "Null User reported equal to User");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void UserEquals_DifferentUser_False()
        {
            User user = new User(1);
            User target = new User(2);
            Assert.IsFalse(user.Equals(target), "Different Users reported equal");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void UserEquals_SameUser_True()
        {
            User user = new User(1);
            Assert.IsTrue(user.Equals(user), "User is not equal to itself");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void UserEquals_IdenticalUsers_True()
        {
            User user = new User(1);
            User target = new User(1);
            Assert.IsTrue(user.Equals(target), "User is not equal to copy of itself");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void UserEquals_ThisZeroTargetValid_False()
        {
            User user = new User(new MailAddress("alexhrao@thewinners.com"), new Password("Hello World"), "fdsa");
            User target = new User(1);
            Assert.IsFalse(user.Equals(target), "Zero user equals non-zero user");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void UserEquals_ThisValidTargetZero_False()
        {
            User user = new User(1);
            User target = new User(new MailAddress("alexhrao@thewinners.com"), new Password("Hello World"), "fdsa");
            Assert.IsFalse(user.Equals(target), "Non-zero user equals zero user");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void UserEquals_ThisZeroTargetZero_False()
        {
            User user = new User(new MailAddress("alexhrao@thewinners.com"), new Password("Hello World"), "fdsa");
            User target = new User(new MailAddress("alexhrao@thewinners.com"), new Password("Hello World"), "fdsa");
            Assert.IsFalse(user.Equals(target), "Zero users are reported equal");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void UserEquals_SameZeroUser_False()
        {
            User user = new User(new MailAddress("alexhrao@thewinners.com"), new Password("Hello World"), "fdsa");
            Assert.IsFalse(user.Equals(user), "Same 0 user reported as equal to itself");
        }



        [TestCategory("User"), TestCategory("Method"), TestCategory("GetHashCode"), TestCategory("Successful")]
        [TestMethod]
        public void UserHash_IdenticalUsers_SameHash()
        {
            User user1 = new User(1);
            User user2 = new User(1);
            Assert.AreEqual(user1.GetHashCode(), user2.GetHashCode(), "Identical users have different hash codes");
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("GetHashCode"), TestCategory("Successful")]
        [TestMethod]
        public void UserHash_SameUser_SameHash()
        {
            User user = new User(1);
            Assert.AreEqual(user.GetHashCode(), user.GetHashCode(), "Same user gives different hash codes");
        }

        

        [TestCategory("User"), TestCategory("Method"), TestCategory("Fetch"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserFetch_ZeroID_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@thedailywtf.com"), new Password("hello world"), "hello");
            XmlDocument doc = user.Fetch();
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Fetch"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void UserFetch_ViewerZeroID_ExceptionThrown()
        {
            User target = new User(1);
            User user = new User(new MailAddress("alexhrao@thedailywtf.com"), new Password("hello world"), "hello");
            XmlDocument doc = target.Fetch(user);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Fetch"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserFetch_NullViewer_ExceptionThrown()
        {
            User user = new User(1);
            XmlDocument doc = user.Fetch(null);
        }

        [TestCategory("User"), TestCategory("Method"), TestCategory("Fetch"), TestCategory("Successful")]
        [TestMethod]
        public void UserFetch_ValidUser_DataPresent()
        {
            // Test that all fields are present and correct - except for iterated fields, which will be tested in other Tests
            User target = new User(1)
            {
                Name = "Alex Rao"
            };
            XmlDocument doc = target.Fetch();

            // Make sure has following fields:
            /// This method returns an XML Document with the following fields:
            ///     - userId: The user's ID
            ///     - userName: The user's name
            ///     - email: The User's email
            ///     - birthMonth: The User's birth month
            ///     - birthYear: The User's birth year
            ///     - bio: The User's biography
            ///     - dateJoined: The date this user joined, encoded as "yyyy-MM-dd"
            ///     - image: The qualified path for this user's image
            ///     - groups: An expanded list of all this user's groups.
            ///         - Note that each child of _groups_ is a _group_ element.
            ///     - gifts: An expanded list of all this user's gifts.
            ///         - Note that each child of _gifts_ is a _gift_ element.
            ///     - events: An expanded list of all this user's events.
            ///         - Note that each child of _events_ is an _event_ element.
            ///     - preferences: This user's preferences
            ///     
            /// This is all contained within a _user_ container.
            /// 
            XmlElement id = (XmlElement)doc.GetElementsByTagName("userId")[0];
            Assert.AreEqual(target.ID.ToString(), id.InnerText, "ID mismatch");
            XmlElement name = (XmlElement)doc.GetElementsByTagName("userName")[0];
            Assert.AreEqual(target.Name, name.InnerText, "Name mismatch");
            XmlElement email = (XmlElement)doc.GetElementsByTagName("email")[0];
            Assert.AreEqual(target.Email.Address, email.InnerText, "Email mismatch");
            XmlElement birthMonth = (XmlElement)doc.GetElementsByTagName("birthMonth")[0];
            Assert.AreEqual(target.BirthMonth.ToString(), birthMonth.InnerText, "Month mismatch");
            XmlElement birthDay = (XmlElement)doc.GetElementsByTagName("birthDay")[0];
            Assert.AreEqual(target.BirthDay.ToString(), birthDay.InnerText, "Day mismatch");
            XmlElement bio = (XmlElement)doc.GetElementsByTagName("bio")[0];
            Assert.AreEqual(target.Bio, bio.InnerText, "Bio Mismatch");
            XmlElement dateJoined = (XmlElement)doc.GetElementsByTagName("dateJoined")[0];
            Assert.AreEqual(DateTime.Today.ToString("yyyy-MM-dd"), dateJoined.InnerText, "Date Joined mismatch");
            XmlElement img = (XmlElement)doc.GetElementsByTagName("image")[0];
            Assert.AreEqual(target.GetImage(), img.InnerText, "Image path mismatch");
            // Just check for group element and count; checking of group element is done by GroupTests
            XmlElement groups = (XmlElement)doc.GetElementsByTagName("groups")[0];
            // There should be same num of groups as target thinks
            Assert.AreEqual(target.Groups.Count, groups.ChildNodes.Count, "Group count mismatch");
            XmlElement events = (XmlElement)doc.GetElementsByTagName("events")[0];
            Assert.AreEqual(target.Events.Count, events.ChildNodes.Count, "EVent count mismatch");
            XmlElement gifts = (XmlElement)doc.GetElementsByTagName("gifts")[0];
            Assert.AreEqual(target.Gifts.Count, gifts.ChildNodes.Count, "Gift count mismatch");
            XmlElement pref = (XmlElement)doc.GetElementsByTagName("preferences")[0];
            Assert.IsNotNull(pref, "Preferences were null");
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
                    cmd.CommandText = "CALL gift_registry_db_test.setup();";
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
