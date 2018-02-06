using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GiftServer.Data;
using GiftServer.Exceptions;
using System.Net.Mail;
using GiftServer.Security;
using MySql.Data.MySqlClient;
using System.Configuration;

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
            User user = new User(10);
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


        [TestCategory("User"), TestCategory("Property"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserProperty_NullEmail_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"));
            user.Email = null;
        }
        [TestCategory("User"), TestCategory("Property"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserProperty_NullName_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"));
            user.Name = null;
        }
        [TestCategory("User"), TestCategory("Property"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserProperty_EmptyName_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"));
            user.Name = "";
        }
        [TestCategory("User"), TestCategory("Property"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UserProperty_SpaceName_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"));
            user.Name = "   ";
        }



        [TestCategory("User"), TestCategory("Create"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserCreate_NoName_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@yahoo.com"), new Password("HelloWorld123"));
            user.Create();
        }
        [TestCategory("User"), TestCategory("Create"), TestCategory("ExceptionThrown")]
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
        [TestCategory("User"), TestCategory("Create"), TestCategory("Successful")]
        [TestMethod]
        public void UserCreate_ValidData_NewUser()
        {
            User user = new User(new MailAddress("alexhrao@hotmail.com"), new Password("HelloWorld123"))
            {
                Name = "Alejandro"
            };
            Assert.IsFalse(user.DateJoined.HasValue);
            user.Create();
            Assert.AreNotEqual(user.ID, 0L, "UserID was not updated after creation");
            Assert.IsTrue(user.DateJoined.HasValue, "User Timestamp not updated");
        }
        // ADD THESE TO UPDATE
        [TestCategory("User"), TestCategory("Create"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(DuplicateUserException))]
        public void UserCreate_DuplicateGoogleID_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"));
            user.GoogleId = "12345";
        }
        [TestCategory("User"), TestCategory("Create"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(DuplicateUserException))]
        public void UserCreate_DuplicateFacebookID_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"));
            user.FacebookId = "12345";
        }


        [TestCategory("User"), TestCategory("Update"), TestCategory("Successful")]
        [TestMethod]
        public void UserUpdate_ValidUser_NewName()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"));
            user.Name = "Alejandro";
            user.Update();
            User tester = new User(new MailAddress("alexhrao@gmail.com"));
            Assert.AreEqual("Alejandro", tester.Name, "Name not updated");
        }
        [TestCategory("User"), TestCategory("Update"), TestCategory("Successful")]
        [TestMethod]
        public void UserUpdate_ValidUser_NewEmail()
        {
            User user = new User(new MailAddress("alexhrao@gatech.edu"));
            user.Email = new MailAddress("alexhrao@outlook.com");
            user.Update();
            User tester = new User(new MailAddress("alexhrao@outlook.com"));
            Assert.AreEqual("alexhrao@outlook.com", tester.Email.Address, "Email not updated not updated");
        }
        [TestCategory("User"), TestCategory("Update"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(DuplicateUserException))]
        public void UserUpdate_DuplicateEmail_ExceptionThrown()
        {
            User tester = new User(new MailAddress("arao81@gatech.edu"));
            tester.Email = new MailAddress("alexhrao@gmail.com");
            tester.Update();
        }
        [TestCategory("User"), TestCategory("Update"), TestCategory("Successful")]
        [TestMethod]
        public void UserUpdate_ValidUser_NewPassword()
        {
            User user = new User(new MailAddress("raeedah.choudhury@gmail.com"));
            user.UpdatePassword("HelloWorld2.0", new Action<MailAddress, User>((a, b) => { }));
            User tester = new User(new MailAddress("raeedah.choudhury@gmail.com"));
            Assert.IsTrue(tester.Password.Verify("HelloWorld2.0"), "Password not updated in DB");
            Assert.IsTrue(user.Password.Verify("HelloWorld2.0"), "Password not updated locally");
        }
        [TestCategory("User"), TestCategory("Update"), TestCategory("Successful")]
        [TestMethod]
        public void UserUpdate_ValidUser_NewBirthDay()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"));
            user.BirthDay = 1;
            user.BirthMonth = 2;
            user.Update();
        }
        [TestCategory("User"), TestCategory("Update"), TestCategory("Successful")]
        [TestMethod]
        public void UserUpdate_ValidUser_RemoveBirth()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"));
            user.BirthDay = 0;
            user.BirthMonth = 0;
            user.Update();
        }
        [TestCategory("User"), TestCategory("Update"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserUpdate_BirthMonthWithoutDay_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"));
            user.BirthMonth = 1;
            user.BirthDay = 0;
            user.Update();
        }
        [TestCategory("User"), TestCategory("Update"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UserUpdate_BirthDayWithoutMonth_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"));
            user.BirthMonth = 0;
            user.BirthDay = 1;
            user.Update();
        }



        [TestCategory("User"), TestCategory("Delete"), TestCategory("Successful")]
        [TestMethod]
        public void UserDelete_ValidUser_NoUser()
        {
            User user = new User(new MailAddress("alexhrao@hotmail.com"));
            user.Delete();
            Assert.AreEqual<ulong>(user.ID, 0L, "UserID was not reset to 0");
        }
        [TestCategory("User"), TestCategory("Delete"), TestCategory("Successful")]
        [TestMethod]
        public void UserDelete_DeleteTwice_NoUser()
        {
            User user = new User(new MailAddress("alexhrao@yahoo.com"), new Password("HelloWorld123"))
            {
                Name = "Hello World"
            };
            Assert.AreEqual<ulong>(user.ID, 0L, "UserID was not set to 0");
            user.Create();
            user.Delete();
            user.Delete();
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
