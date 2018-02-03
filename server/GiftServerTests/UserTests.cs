using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GiftServer.Data;
using GiftServer.Exceptions;
using System.Net.Mail;
using GiftServer.Security;

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

        [TestCategory("User"), TestCategory("Instantiate"), TestCategory("Successful")]
        [TestMethod]
        public void UserInstantiate_ValidCredentials_NewUser()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"), "MyPassword");
            Assert.IsNotNull(user, "Valid User was not initialized");
            Assert.AreEqual("Alex Rao", user.UserName, "Incorrect User was fetched!");
        }

        [TestCategory("User"), TestCategory("Create"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(DuplicateUserException))]
        public void UserCreate_DuplicateEmail_ExceptionThrown()
        {
            User user = new User(new MailAddress("alexhrao@gmail.com"), new Password("HelloWorld123"));
            user.Create();
        }

        [TestCategory("User"), TestCategory("Create"), TestCategory("Successful")]
        [TestMethod]
        public void UserCreate_ValidData_NewUser()
        {
            User user = new User(new MailAddress("alexhrao@hotmail.com"), new Password("HelloWorld123"));
            bool res = user.Create();
            Assert.IsTrue(res, "Valid Create() returned false");
            Assert.AreNotEqual(user.UserId, 0, "UserID was not updated after creation");
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            // Delete user alexhrao@hotmail.com
            User user = new User(new MailAddress("alexhrao@hotmail.com"));
            user.Delete();
        }
    }
}
