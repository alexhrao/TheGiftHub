using GiftServer.Properties;
using GiftServer.Server;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.IO;

namespace GiftServer
{
    namespace Data
    {
        public class Gift : ISynchronizable, IShowable
        {
            public ulong GiftId
            {
                get;
                private set;
            } = 0;
            public User User;
            public string Name;
            public string Description = "";
            public string Url = "";
            public double Cost = 0.00;
            public string Stores = "";
            public uint Quantity = 1;
            public string Color = "000000";
            public string ColorText = "";
            public string Size = "";
            public Category Category;
            public double Rating = 0.00;
            public DateTime TimeStamp;
            public DateTime DateReceived = DateTime.MinValue;

            public Gift(ulong id)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT * FROM gifts WHERE GiftID = @id;";
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                GiftId = id;
                                User = new User(Convert.ToUInt64(reader["UserID"]));
                                Name = Convert.ToString(reader["GiftName"]);
                                Description = Convert.ToString(reader["GiftDescription"]);
                                Url = Convert.ToString(reader["GiftURL"]);
                                Cost = Convert.ToDouble(reader["GiftCost"]);
                                Stores = Convert.ToString(reader["GiftStores"]);
                                Quantity = Convert.ToUInt32(reader["GiftQuantity"]);
                                Color = Convert.ToString(reader["GiftColor"]);
                                ColorText = Convert.ToString(reader["GiftColorText"]);
                                Size = Convert.ToString(reader["GiftSize"]);
                                Category = new Category(Convert.ToUInt64(reader["CategoryID"]));
                                Rating = Convert.ToDouble(reader["GiftRating"]);
                                TimeStamp = (DateTime)(reader["GiftAddStamp"]);
                                try
                                {
                                    DateReceived = (DateTime)(reader["GiftReceivedDate"]);
                                }
                                catch (InvalidCastException) { }
                            }
                        }
                    }
                }
            }
            public Gift() { }

            // TODO: Add Update and Delete
            public bool Create()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        // TODO: Add received date?
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO gifts (UserID, GiftName, GiftDescription, GiftURL, GiftCost, GiftStores, GiftQuantity, GiftColor, GiftColorText, GiftSize, CategoryID, GiftRating, GiftReceivedDate) "
                                        + "VALUES (@uid, @name, @desc, @url, @cost, @stores, @quantity, @hex, @color, @size, @category, @rating, @rec);";
                        cmd.Parameters.AddWithValue("@uid", User.UserId);
                        cmd.Parameters.AddWithValue("@name", Name);
                        cmd.Parameters.AddWithValue("@desc", Description);
                        cmd.Parameters.AddWithValue("@url", Url);
                        cmd.Parameters.AddWithValue("@cost", Cost);
                        cmd.Parameters.AddWithValue("@hex", Color);
                        cmd.Parameters.AddWithValue("@color", ColorText);
                        cmd.Parameters.AddWithValue("@size", Size);
                        cmd.Parameters.AddWithValue("@category", Category.CategoryId);
                        cmd.Parameters.AddWithValue("@rating", Rating);
                        if (DateReceived == DateTime.MinValue)
                        {
                            cmd.Parameters.AddWithValue("@rec", "");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@rec", DateReceived.ToString("yyyy-MM-dd"));
                        }
                        cmd.Prepare();
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            GiftId = Convert.ToUInt64(cmd.LastInsertedId);
                            using (MySqlCommand timer = new MySqlCommand())
                            {
                                timer.Connection = con;
                                timer.CommandText = "SELECT GiftAddStamp FROM gifts WHERE GiftID = @gid;";
                                timer.Parameters.AddWithValue("@gid", GiftId);
                                timer.Prepare();
                                using (MySqlDataReader reader = timer.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        TimeStamp = (DateTime)(reader["GiftAddStamp"]);
                                        return true;
                                    }
                                }
                            }
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            public bool Update()
            {
                if (GiftId == 0)
                {
                    return Create();
                }
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "UPDATE gifts ";
                    }
                }
                return false;
            }
            public bool Delete()
            {
                if (GiftId == 0)
                {
                    return false;
                }
                return false;
            }

            public void SaveImage(MultipartParser parser)
            {
                ImageProcessor processor = new ImageProcessor(parser);
                File.WriteAllBytes(Resources.BasePath + "/resources/images/gifts/Gift" + this.GiftId + Resources.ImageFormat, processor.Data);
            }
            public void RemoveImage()
            {
                File.Delete(Resources.BasePath + "/resources/images/gifts/Gift" + this.GiftId + Resources.ImageFormat);
            }
            public string GetImage()
            {
                return GetImage(this.GiftId);
            }
            public string GetImage(ulong id)
            {
                string path = Resources.BasePath + "/resources/images/gifts/Gift" + id + Resources.ImageFormat;
                // if file exists, return path. Otherwise, return default
                // Race condition, but I don't know how to solve (yet)
                if (File.Exists(path))
                {
                    return "/resources/images/gifts/Gift" + id + Resources.ImageFormat;
                }
                else
                {
                    return "resources/images/gifts/default" + Resources.ImageFormat;
                }
            }
        }
    }
}