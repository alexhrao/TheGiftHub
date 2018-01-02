using GiftServer.Properties;
using GiftServer.Server;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        public class Gift : ISynchronizable, IShowable, IFetchable
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
            public List<Group> Groups
            {
                get
                {
                    List<Group> _groups = new List<Group>();
                    using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                    {
                        con.Open();
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT GroupID FROM groups_gifts WHERE GiftID = @gid;";
                            cmd.Parameters.AddWithValue("@gid", GiftId);
                            cmd.Prepare();
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    _groups.Add(new Group(Convert.ToUInt64(reader["GroupID"])));
                                }
                                return _groups;
                            }
                        }
                    }
                }
            }

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
                                catch (InvalidCastException)
                                {
                                    DateReceived = DateTime.MinValue;
                                }
                            }
                        }
                    }
                }
            }
            public Gift(string Name)
            {
                this.Name = Name;
            }

            public bool Create()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO gifts (UserID, GiftName, GiftDescription, GiftURL, GiftCost, GiftStores, GiftQuantity, GiftColor, GiftColorText, GiftSize, CategoryID, GiftRating, GiftReceivedDate) "
                                        + "VALUES (@uid, @name, @desc, @url, @cost, @stores, @quantity, @hex, @color, @size, @category, @rating, @rec);";
                        cmd.Parameters.AddWithValue("@uid", User.UserId);
                        cmd.Parameters.AddWithValue("@name", Name);
                        cmd.Parameters.AddWithValue("@desc", Description);
                        cmd.Parameters.AddWithValue("@url", Url);
                        cmd.Parameters.AddWithValue("@cost", Cost);
                        cmd.Parameters.AddWithValue("@stores", Stores);
                        cmd.Parameters.AddWithValue("@quantity", Quantity);
                        cmd.Parameters.AddWithValue("@hex", Color);
                        cmd.Parameters.AddWithValue("@color", ColorText);
                        cmd.Parameters.AddWithValue("@size", Size);
                        cmd.Parameters.AddWithValue("@category", Category.CategoryId);
                        cmd.Parameters.AddWithValue("@rating", Rating);
                        cmd.Parameters.AddWithValue("@rec", DateReceived.ToString("yyyy-MM-dd"));
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
                        cmd.CommandText = "UPDATE gifts " +
                                            "SET GiftName = @name, " +
                                            "GiftDescription = @description, " +
                                            "GiftURL = @url, " +
                                            "GiftCost = @cost, " +
                                            "GiftStores = @stores, " +
                                            "GiftQuantity = @quant, " +
                                            "GiftColor = @color, " +
                                            "GiftColorText = @colorText, " +
                                            "GiftSize = @size, " +
                                            "CategoryID = @cid, " +
                                            "GiftRating = @rating, " +
                                            "GiftReceivedDate = @rec " +
                                            "WHERE GiftID = @gid;";
                        cmd.Parameters.AddWithValue("@name", Name);
                        cmd.Parameters.AddWithValue("@description", Description);
                        cmd.Parameters.AddWithValue("@url", Url);
                        cmd.Parameters.AddWithValue("@cost", Cost);
                        cmd.Parameters.AddWithValue("@stores", Stores);
                        cmd.Parameters.AddWithValue("@quant", Quantity);
                        cmd.Parameters.AddWithValue("@color", Color);
                        cmd.Parameters.AddWithValue("@colorText", ColorText);
                        cmd.Parameters.AddWithValue("@size", Size);
                        cmd.Parameters.AddWithValue("@cid", Category.CategoryId);
                        cmd.Parameters.AddWithValue("@rating", Rating);
                        cmd.Parameters.AddWithValue("@gid", GiftId);
                        if (DateReceived == DateTime.MinValue)
                        {
                            cmd.Parameters.AddWithValue("@rec", null);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@rec", DateReceived.ToString("yyyy-MM-dd"));
                        }
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            public bool Delete()
            {
                if (GiftId == 0)
                {
                    return false;
                }
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE p.* FROM purchases p "
                                        + "INNER JOIN reservations r ON p.ReservationID = r.ReservationID "
                                        + "WHERE r.GiftID = @gid;";
                        cmd.Parameters.AddWithValue("@gid", GiftId);
                        cmd.Prepare();
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM reservations WHERE GiftID = @gid;";
                        cmd.Parameters.AddWithValue("@gid", GiftId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM receptions WHERE GiftID = @gid;";
                        cmd.Parameters.AddWithValue("@gid", GiftId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM groups_gifts WHERE GiftID = @gid;";
                        cmd.Parameters.AddWithValue("@gid", GiftId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM gifts WHERE GiftID = @gid;";
                        cmd.Parameters.AddWithValue("@gid", GiftId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        GiftId = 0;
                        return true;
                    }
                }
            }

            public void SaveImage(MultipartParser parser)
            {
                ImageProcessor processor = new ImageProcessor(parser);
                File.WriteAllBytes(System.IO.Directory.GetCurrentDirectory() + "/resources/images/gifts/Gift" + this.GiftId + Constants.ImageFormat, processor.Data);
            }
            public void RemoveImage()
            {
                File.Delete(System.IO.Directory.GetCurrentDirectory() + "/resources/images/gifts/Gift" + this.GiftId + Constants.ImageFormat);
            }
            public string GetImage()
            {
                return GetImage(this.GiftId);
            }
            public static string GetImage(ulong id)
            {
                string path = System.IO.Directory.GetCurrentDirectory() + "/resources/images/gifts/Gift" + id + Constants.ImageFormat;
                // if file exists, return path. Otherwise, return default
                // Race condition, but I don't know how to solve (yet)
                if (File.Exists(path))
                {
                    return "/resources/images/gifts/Gift" + id + Constants.ImageFormat;
                }
                else
                {
                    return "resources/images/gifts/default" + Constants.ImageFormat;
                }
            }

            public void Add(Group group)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO groups_gifts (GroupID, GiftID) VALUES (@groupId, @giftId);";
                        cmd.Parameters.AddWithValue("@groupId", group.GroupId);
                        cmd.Parameters.AddWithValue("@giftId", GiftId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            public void Remove(Group group)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM groups_gifts WHERE GiftID = @giftId AND GroupID = @groupId;";
                        cmd.Parameters.AddWithValue("@giftId", GiftId);
                        cmd.Parameters.AddWithValue("@groupId", group.GroupId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            public XmlDocument Fetch()
            {
                XmlDocument info = new XmlDocument();
                XmlElement container = info.CreateElement("gift");
                info.AppendChild(container);
                XmlElement id = info.CreateElement("giftId");
                id.InnerText = GiftId.ToString();
                XmlElement user = info.CreateElement("user");
                user.InnerText = User.UserId.ToString();
                XmlElement name = info.CreateElement("name");
                name.InnerText = Name;
                XmlElement description = info.CreateElement("description");
                description.InnerText = Description;
                XmlElement url = info.CreateElement("url");
                url.InnerText = Url;
                XmlElement cost = info.CreateElement("cost");
                cost.InnerText = Cost.ToString("#.##");
                XmlElement stores = info.CreateElement("stores");
                stores.InnerText = Stores;
                XmlElement quantity = info.CreateElement("quantity");
                quantity.InnerText = Quantity.ToString();
                XmlElement color = info.CreateElement("color");
                color.InnerText = "#" + Color;
                XmlElement colorText = info.CreateElement("colorText");
                colorText.InnerText = ColorText;
                XmlElement size = info.CreateElement("size");
                size.InnerText = Size;
                XmlElement category = info.CreateElement("category");
                category.InnerText = Category.Name;
                XmlElement rating = info.CreateElement("rating");
                rating.InnerText = Rating.ToString();
                XmlElement image = info.CreateElement("image");
                image.InnerText = GetImage();
                XmlElement groups = info.CreateElement("groups");
                foreach (Group group in Groups)
                {
                    XmlElement groupElem = info.CreateElement("group");
                    groupElem.InnerText = group.GroupId.ToString();
                    groups.AppendChild(groupElem);
                }

                container.AppendChild(id);
                container.AppendChild(user);
                container.AppendChild(name);
                container.AppendChild(description);
                container.AppendChild(url);
                container.AppendChild(cost);
                container.AppendChild(stores);
                container.AppendChild(quantity);
                container.AppendChild(color);
                container.AppendChild(colorText);
                container.AppendChild(size);
                container.AppendChild(category);
                container.AppendChild(rating);
                container.AppendChild(image);
                container.AppendChild(groups);

                return info;
            }
        }
    }
}