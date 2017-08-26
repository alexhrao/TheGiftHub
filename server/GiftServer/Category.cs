﻿using GiftServer.Exceptions;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
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

            public XmlDocument Fetch()
            {
                XmlDocument info = new XmlDocument();

                return info;
            }
        }
    }
}