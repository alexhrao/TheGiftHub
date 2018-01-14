using GiftServer.Exceptions;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Web;
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
        public class Category : IFetchable
        {
            /// <summary>
            /// The ID of this category as it appears in the database
            /// </summary>
            public readonly ulong CategoryId = 0;
            /// <summary>
            /// The short name of this category
            /// </summary>
            public readonly string Name;
            /// <summary>
            /// A longer description of this category
            /// </summary>
            public readonly string Description;

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
                                this.CategoryId = id;
                                this.Name = Convert.ToString(reader["CategoryName"]);
                                this.Description = Convert.ToString(reader["CategoryDescription"]);
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
                                this.CategoryId = Convert.ToUInt64(reader["CategoryID"]);
                                this.Name = Convert.ToString(reader["CategoryName"]);
                                this.Description = Convert.ToString(reader["CategoryDescription"]);
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
                id.InnerText = CategoryId.ToString();
                XmlElement name = info.CreateElement("name");
                name.InnerText = Name;
                XmlElement description = info.CreateElement("description");
                description.InnerText = Description;

                container.AppendChild(id);
                container.AppendChild(name);
                container.AppendChild(description);

                return info;
            }
        }
    }
}