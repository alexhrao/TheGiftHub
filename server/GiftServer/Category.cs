using GiftServer.Exceptions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// A possible gift category
        /// </summary>
        /// <remarks>
        /// Category represents a possible gift category (clothing, electronics, etc.). It is Read-only;
        /// New Categories are not allowed to be added! Instead, categories are static; this is the reason
        /// that Category does not implement the IFetchable Interface, and why all its properties are readonly
        /// </remarks>
        public class Category : IFetchable, IEquatable<Category>
        {
            /// <summary>
            /// The ID of this category as it appears in the database
            /// </summary>
            public readonly ulong ID = 0;
            /// <summary>
            /// The short name of this category
            /// </summary>
            public readonly string Name;
            private static List<Category> categories = null;
            /// <summary>
            /// A convenience method for getting all categories. Since categories are static in nature,
            /// This will always be correct
            /// </summary>
            public static List<Category> Categories
            {
                get
                {
                    if (categories == null)
                    {
                        categories = new List<Category>();
                        using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                        {
                            con.Open();
                            using (MySqlCommand cmd = new MySqlCommand())
                            {
                                cmd.Connection = con;
                                cmd.CommandText = "SELECT CategoryID FROM categories;";
                                cmd.Prepare();
                                using (MySqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        categories.Add(new Category(Convert.ToUInt64(reader["CategoryID"])));
                                    }
                                }
                            }
                        }
                    }
                    return categories;
                }
            }
            /// <summary>
            /// Given the Category ID, this will fetch all the necessary information
            /// </summary>
            /// <param name="id">The ID of the category; if 0, this method has undefined results</param>
            public Category(ulong id)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT * FROM categories WHERE CategoryID = @id;";
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ID = id;
                                Name = Convert.ToString(reader["CategoryName"]);
                            }
                            else
                            {
                                throw new CategoryNotFoundException(id);
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Like the constructor for ID, this will create a new instance of Category, fetching information for the specified category name
            /// </summary>
            /// <param name="name">The EXACT name of the category</param>
            public Category(string name)
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name), "Name cannot be null");
                }
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT * FROM categories WHERE CategoryName = @name;";
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ID = Convert.ToUInt64(reader["CategoryID"]);
                                Name = Convert.ToString(reader["CategoryName"]);
                            }
                            else
                            {
                                throw new CategoryNotFoundException(name);
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// See if the given object is actually this category
            /// </summary>
            /// <param name="obj">The object to compare</param>
            /// <returns>Whether or not the objects are equal</returns>
            public override bool Equals(object obj)
            {
                if (obj != null && obj is Category)
                {
                    return Equals((Category)(obj));
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// Check two categories.
            /// </summary>
            /// <remarks>
            /// This is identical to saying == since the value is immutable
            /// </remarks>
            /// <param name="category">The category to compare</param>
            /// <returns>Whether or not they are the same</returns>
            public bool Equals(Category category)
            {
                return category == this;
            }
            /// <summary>
            /// Get the hash for this category
            /// </summary>
            /// <returns>The hash code</returns>
            public override int GetHashCode()
            {
                return ID.GetHashCode();
            }
            /// <summary>
            /// See if two categories are equal
            /// </summary>
            /// <param name="a">The first category</param>
            /// <param name="b">The second category</param>
            /// <returns>Whether or not they are equal</returns>
            /// <remarks>
            /// Since the category class is immutable, this is safe
            /// </remarks>
            public static bool operator ==(Category a, Category b)
            {
                return ReferenceEquals(a, b) ? true : ((object)(a) != null) && ((object)(b) != null) && a.ID == b.ID;
            }
            /// <summary>
            /// See if two categories are unequal
            /// </summary>
            /// <param name="a">The first category</param>
            /// <param name="b">The second category</param>
            /// <returns>Whether or not they are equal</returns>
            /// <remarks>
            /// Since the category class is immutable, this is safe
            /// </remarks>
            public static bool operator !=(Category a, Category b)
            {
                return !(a == b);
            }
            /// <summary>
            /// Fetch implements the IFetchable interface.
            /// </summary>
            /// <remarks>
            /// This will "serialize" all information about this category, with the following fields:
            ///     - categoryId: The ID of this category
            ///     - name: The name of this category
            ///     - description: The description of this category
            /// 
            /// This is all wrapped inside a category container.
            /// </remarks>
            /// <returns>An XML document with all the necessary information encoded</returns>
            public XmlDocument Fetch()
            {
                XmlDocument info = new XmlDocument();
                XmlElement container = info.CreateElement("category");
                info.AppendChild(container);
                XmlElement id = info.CreateElement("categoryId");
                id.InnerText = ID.ToString();
                XmlElement name = info.CreateElement("name");
                name.InnerText = Name;

                container.AppendChild(id);
                container.AppendChild(name);

                return info;
            }
            /// <summary>
            /// Fetch the category visible to this user
            /// </summary>
            /// <param name="viewer">The viewer</param>
            /// <returns>The category serialization</returns>
            public XmlDocument Fetch(User viewer)
            {
                // Since a category is always viewable, just return Fetch()
                return Fetch();
            }
        }
    }
}