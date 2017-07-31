using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GiftServer.Exceptions;
using GiftServer.Security;
namespace GiftServerTester
{
    [TestClass]
    public class PasswordHashTester
    {

        [TestMethod]
        [ExpectedException(typeof(InvalidPasswordException), "Null Password did not throw correctly")]
        public void HashTest_Null()
        {
            PasswordHash.Hash(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPasswordException), "Password that is too short was allowed")]
        public void HashTest_ShortLength()
        {
            PasswordHash.Hash("123");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPasswordException), "Password that is too short was allowed")]
        public void HashTest_ZeroLength()
        {
            PasswordHash.Hash("");
        }

        [TestMethod]
        public void VerifyTest_Equal()
        {
            string tester = "HelloWorld!";
            string hashed = PasswordHash.Hash(tester);
            Assert.IsTrue(PasswordHash.Verify(tester, hashed));
        }

        [TestMethod]
        public void VerifyTest_Unequal()
        {
            string tester = "HelloWorld!!!";
            string hashed = PasswordHash.Hash("GoodbyeWorld");
            Assert.IsFalse(PasswordHash.Verify(tester, hashed));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPasswordException), "Null Hash was allowed")]
        public void VerifyTest_NullHash()
        {
            PasswordHash.Verify("testing123", null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPasswordException), "Null Password was allowed")]
        public void VerifyTest_NullPass()
        {
            string hashed = PasswordHash.Hash("HelloWorld");
            PasswordHash.Verify(null, hashed);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidPasswordException), "Nulls were allowed")]
        public void VerifyTest_NullBoth()
        {
            PasswordHash.Verify(null, null);
        }
    }
}
