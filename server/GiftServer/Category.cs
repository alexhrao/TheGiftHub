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
        public class Category : IFetchable
        {
            public readonly ulong CategoryId = 0;
            public readonly string Name;
            public readonly string Description;

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

            public XmlDocument Fetch()
            {
                XmlDocument info = new XmlDocument();
                XmlElement container = info.CreateElement("category");
                info.AppendChild(container);
                XmlElement id = info.CreateElement("categoryId");
                id.InnerText = HttpUtility.HtmlEncode(CategoryId);
                XmlElement name = info.CreateElement("name");
                name.InnerText = HttpUtility.HtmlEncode(Name);
                XmlElement description = info.CreateElement("description");
                description.InnerText = HttpUtility.HtmlEncode(Description);

                container.AppendChild(id);
                container.AppendChild(name);
                container.AppendChild(description);

                return info;
            }
        }
    }
}