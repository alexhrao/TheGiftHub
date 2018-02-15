using System;
using GiftServer.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GiftServerTests
{
    [TestClass]
    public class PasswordTests
    {

        [TestCategory("Password"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PasswordTest_NullHash_ExceptionThrown()
        {
            new Password(null, "hello", 1);
        }

        [TestCategory("Password"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PasswordTest_NullSalt_ExceptionThrown()
        {
            new Password("helloWorld", null, 1);
        }

        [TestCategory("Password"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PasswordTest_ZeroIterations_ExceptionThrown()
        {
            new Password("helloWorld", "wassup", 0);
        }

        [TestCategory("Password"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PasswordTest_NegativeIterations_ExceptionThrown()
        {
            new Password("helloWorld", "wassup", -5);
        }

        [TestCategory("Password"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PasswordTest_EmptyHash_ExceptionThrown()
        {
            new Password("", "wassup", -5);
        }

        [TestCategory("Password"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PasswordTest_EmptySalt_ExceptionThrown()
        {
            new Password("helloWorld", "", -5);
        }

        [TestCategory("Password"), TestCategory("Instantiate"), TestCategory("Successful")]
        [TestMethod]
        public void PasswordTest_ValidHashSaltIterations_Successful()
        {
            new Password("YotmshG9KKnabJwve9LWmcFGxK0=", "xyUPC/lbG9NUTuFqqRuqnw==", 10000);
        }

        [TestCategory("Password"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PasswordTest_EmptyString_ExceptionThrown()
        {
            new Password("");
        }

        [TestCategory("Password"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PasswordTest_ZeroIterationsString_ExceptionThrown()
        {
            new Password("Hello World", 0);
        }

        [TestCategory("Password"), TestCategory("Instantiate"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PasswordTest_NegativeIterationsString_ExceptionThrown()
        {
            new Password("Goodbye", -5);
        }

        [TestCategory("Password"), TestCategory("Instantiate"), TestCategory("Successful")]
        [TestMethod]
        public void PasswordTest_ValidString_Successful()
        {
            Password p = new Password("HelloWorld");
        }

        [TestCategory("Password"), TestCategory("Instantiate"), TestCategory("Successful")]
        [TestMethod]
        public void PasswordTest_ValidStringValidIter_Successful()
        {
            Password p = new Password("HelloWorld", 1000);
        }



        [TestCategory("Password"), TestCategory("Property"), TestCategory("Salt"), TestCategory("Successful")]
        [TestMethod]
        public void PasswordTest_SaltSize_16()
        {
            Assert.AreEqual(16, Password.SaltSize, "Invalid salt size");
        }

        [TestCategory("Password"), TestCategory("Property"), TestCategory("Salt"), TestCategory("Successful")]
        [TestMethod]
        public void PasswordTest_HashSize_20()
        {
            Assert.AreEqual(20, Password.HashSize, "Invalid hash size");
        }


        [TestCategory("Password"), TestCategory("Method"), TestCategory("Verify"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PasswordTest_VerifyNull_ExceptionThrown()
        {
            Password p = new Password("HelloWorld");
            p.Verify(null);
        }

        [TestCategory("Password"), TestCategory("Method"), TestCategory("Verify"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PasswordTest_VerifyEmpty_ExceptionThrown()
        {
            Password p = new Password("HelloWorld");
            p.Verify("");
        }

        [TestCategory("Password"), TestCategory("Method"), TestCategory("Verify"), TestCategory("Successful")]
        [TestMethod]
        public void PasswordTest_VerifyCorrect_True()
        {
            Password p = new Password("HelloWorld");
            Assert.IsTrue(p.Verify("HelloWorld"), "HelloWorld not validated");
        }

        [TestCategory("Password"), TestCategory("Method"), TestCategory("Verify"), TestCategory("Successful")]
        [TestMethod]
        public void PasswordTest_VerifyIncorrect_False()
        {
            Password p = new Password("HelloWorld");
            Assert.IsFalse(p.Verify("WorldHello"), "WorldHell validated as HelloWorld");
        }
    }
}
