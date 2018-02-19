using System;
using System.Collections.Generic;
using System.Configuration;
using GiftServer.Data;
using GiftServer.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;

namespace GiftServerTests
{
    [TestClass]
    public class CategoryTests
    {
        [TestCategory("Category"), TestCategory("Initialize"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(CategoryNotFoundException))]
        public void CategoryInitialize_ZeroID_ExceptionThrown()
        {
            Category cat = new Category(0);
        }

        [TestCategory("Category"), TestCategory("Initialize"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(CategoryNotFoundException))]
        public void CategoryInitialize_InvalidID_ExceptionThrown()
        {
            Category cat = new Category(10);
        }

        [TestCategory("Category"), TestCategory("Initialize"), TestCategory("Success")]
        [TestMethod]
        public void CategoryInitialize_ValidID_CategoryInitialized()
        {
            Category cat = new Category(1);
            Assert.AreEqual(cat.Name, "Electronics", "Name not correct");
        }

        [TestCategory("Category"), TestCategory("Initialize"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CategoryInitialize_NullName_ExceptionThrown()
        {
            Category cat = new Category(null);
        }

        [TestCategory("Category"), TestCategory("Initialize"), TestCategory("ExceptionThrown")]
        [TestMethod]
        [ExpectedException(typeof(CategoryNotFoundException))]
        public void CategoryInitialize_NameNotFound_ExceptionThrown()
        {
            Category cat = new Category("");
        }

        [TestCategory("Category"), TestCategory("Initialize"), TestCategory("Success")]
        [TestMethod]
        public void CategoryInitialize_ValidName_CategoryFound()
        {
            Category cat = new Category("Electronics");
            Assert.AreEqual(1UL, cat.ID, "ID not correct");
        }

        [TestCategory("Category"), TestCategory("Property"), TestCategory("Get")]
        [TestMethod]
        public void CategoryProperty_Name_Name()
        {
            Category cat = new Category(1);
            Assert.AreEqual("Electronics", cat.Name, "Name not correct");
        }

        [TestCategory("Category"), TestCategory("Property"), TestCategory("Get")]
        [TestMethod]
        public void CategoryProperty_ID_ID()
        {
            Category cat = new Category("Electronics");
            Assert.AreEqual(1UL, cat.ID, "ID not correct");
        }

        [TestCategory("Category"), TestCategory("Property"), TestCategory("Get")]
        [TestMethod]
        public void CategoryProperty_Categories()
        {
            List<Category> cats = Category.Categories;
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT * FROM categories;";
                    cmd.Prepare();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Check that it maps to ONE and ONLY ONE category
                            Assert.AreEqual(1, cats.RemoveAll(c => c.Name == reader["CategoryName"].ToString()), "Removed more than one");
                        }
                    }
                }
            }
        }


        [TestCategory("Category"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void CategoryEquals_NullObject_False()
        {
            Category cat = new Category(1);
            Assert.IsFalse(cat.Equals((object)null), "Null object compares as true");
        }

        [TestCategory("Category"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void CategoryEquals_NonCategory_False()
        {
            Category cat = new Category(1);
            Assert.IsFalse(cat.Equals(new User(1)), "Invalid object compares as true");
        }

        [TestCategory("Category"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void CategoryEquals_DifferentCatAsObject_False()
        {
            Category cat = new Category(1);
            object cat2 = new Category(2);
            Assert.IsFalse(cat.Equals(cat2), "Different category object compares as true");
        }

        [TestCategory("Category"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void CategoryEquals_SameCategoryAsObject_True()
        {
            Category cat = new Category(1);
            object cat2 = new Category(1);
            Assert.IsTrue(cat.Equals(cat2), "Same category as object compares as false");
        }

        [TestCategory("Category"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void CategoryEquals_IdenticalObject_True()
        {
            Category cat = new Category(1);
            Assert.IsTrue(cat.Equals((object)cat), "Identical object compares as false");
        }

        [TestCategory("Category"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void CategoryEquals_NullCategory_False()
        {
            Category cat = new Category(1);
            Assert.IsFalse(cat.Equals((Category)null), "Null category compares as true");
        }

        [TestCategory("Category"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void CategoryEquals_DifferentCategory_False()
        {
            Category cat = new Category(1);
            Category target = new Category(2);
            Assert.IsFalse(cat.Equals(target), "Different category compares as true");
        }

        [TestCategory("Category"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void CategoryEquals_SameCategory_True()
        {
            Category cat = new Category(1);
            Category target = new Category(1);
            Assert.IsTrue(cat.Equals(target), "Same category compares as false");
        }

        [TestCategory("Category"), TestCategory("Method"), TestCategory("Equals"), TestCategory("Successful")]
        [TestMethod]
        public void CategoryEquals_IdenticalCategory_True()
        {
            Category cat = new Category(1);
            Assert.IsTrue(cat.Equals(cat), "Identical category compares as false");
        }



        [TestCategory("Category"), TestCategory("Operator"), TestCategory("Equals")]
        [TestMethod]
        public void CategoryEqualsOperator_LeftSideNull_False()
        {
            Category cat = new Category(1);
            Assert.IsFalse(null == cat, "Null on left side compares as true");
        }

        [TestCategory("Category"), TestCategory("Operator"), TestCategory("Equals")]
        [TestMethod]
        public void CategoryEqualsOperator_RightSideNull_False()
        {
            Category cat = new Category(1);
            Assert.IsFalse(cat == null, "Null on right side compares as true");
        }

        [TestCategory("Category"), TestCategory("Operator"), TestCategory("Equals")]
        [TestMethod]
        public void CategoryEqualsOperator_BothSideNull_True()
        {
            Assert.IsTrue((Category)null == (Category)null, "Null on both sides compares as false");
        }

        [TestCategory("Category"), TestCategory("Operator"), TestCategory("Equals")]
        [TestMethod]
        public void CategoryEqualsOperator_DifferentCategories_False()
        {
            Category cat1 = new Category(1);
            Category cat2 = new Category(2);
            Assert.IsFalse(cat1 == cat2, "Different Categories compare true");
            Assert.IsFalse(cat2 == cat1, "Different Categories compare true");
        }

        [TestCategory("Category"), TestCategory("Operator"), TestCategory("Equals")]
        [TestMethod]
        public void CategoryEqualsOperator_SameCategories_True()
        {

            Category cat1 = new Category(1);
            Category cat2 = new Category(1);
            Assert.IsTrue(cat1 == cat2, "Same Categories compare false");
            Assert.IsTrue(cat2 == cat1, "Same Categories compare false");
        }

        [TestCategory("Category"), TestCategory("Operator"), TestCategory("Equals")]
        [TestMethod]
        public void CategoryEqualsOperator_IdenticalCategory_True()
        {
            Category cat1 = new Category(1);
            Category cat2 = cat1;
            Assert.IsTrue(cat1 == cat2, "Same Categories compare false");
        }

        [ClassInitialize]
        public static void Initialize(TestContext ctx)
        {
            TestManager.Reset().Wait();
        }

        [ClassCleanup]
        public static void Cleanup(TestContext ctx)
        {
            TestManager.Reset().Wait();
        }
    }
}
