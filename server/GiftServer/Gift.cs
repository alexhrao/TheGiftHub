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
            public long GiftId = -1;
            public User User;
            public string Name;
            public string Description;
            public string Url;
            public double Cost;
            public string Stores;
            public int Quantity;
            public string Color;
            public string ColorText;
            public string Size;
            public Category Category;
            public double Rating;
            public DateTime TimeStamp;
            public DateTime ReceivedDate;

            public Gift(long id)
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
                                User = new User(Convert.ToInt64(reader["UserID"]));
                                Name = Convert.ToString(reader["GiftName"]);
                                Description = Convert.ToString(reader["GiftDescription"]);
                                Url = Convert.ToString(reader["GiftURL"]);
                                Cost = Convert.ToDouble(reader["GiftCost"]);
                                Stores = Convert.ToString(reader["GiftStores"]);
                                Quantity = Convert.ToInt32(reader["GiftQuantity"]);
                                Color = Convert.ToString(reader["GiftColor"]);
                                ColorText = Convert.ToString(reader["GiftColorText"]);
                                Size = Convert.ToString(reader["GiftSize"]);
                                Category = new Category(Convert.ToInt64(reader["CategoryID"]));
                                Rating = Convert.ToDouble(reader["GiftRating"]);
                                TimeStamp = (DateTime)(reader["GiftAddStamp"]);
                                try
                                {
                                    ReceivedDate = (DateTime)(reader["GiftReceivedDate"]);
                                }
                                catch (InvalidCastException)
                                {
                                    ReceivedDate = DateTime.MinValue;
                                }
                            }
                        }
                    }
                }
            }
            public Gift() { }

            public bool Create()
            {
                return false;
            }
            public bool Update()
            {
                return false;
            }
            public bool Delete()
            {
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
            public string GetImage(long id)
            {
                return Resources.BasePath + "/resources/images/gifts/Gift" + this.GiftId + Resources.ImageFormat;
            }
        }
    }
}