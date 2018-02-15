using GiftServer.Exceptions;
using GiftServer.Properties;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// A single gift
        /// </summary>
        /// <remarks>
        /// A gift is owned by a specific user, and has a great deal of properties (color, quantity, etc.).
        /// </remarks>
        public class Gift : ISynchronizable, IShowable, IFetchable, IEquatable<Gift>
        {
            /// <summary>
            /// The GiftID
            /// </summary>
            /// <remarks>
            /// A GiftID means a deleted gift (or one never created).
            /// </remarks>
            public ulong ID
            {
                get;
                private set;
            } = 0;
            private User user;
            /// <summary>
            /// The owner of this gift
            /// </summary>
            public User Owner
            {
                get
                {
                    return user;
                }
                private set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException(nameof(value), "Owner cannot be null");
                    }
                    else
                    {
                        user = value;
                    }
                }
            }
            private string name = "";
            /// <summary>
            /// The name of this gift
            /// </summary>
            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException(nameof(value));
                    }
                    else if (String.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("Invalid name " + value, nameof(value));
                    }
                    else
                    {
                        name = value;
                    }
                }
            }
            private string description = "";
            /// <summary>
            /// This gift's description
            /// </summary>
            public string Description
            {
                get
                {
                    return description;
                }
                set
                {
                    if (String.IsNullOrEmpty(value))
                    {
                        value = "";
                    }
                    description = value;
                }
            }
            private string url = "";
            /// <summary>
            /// The URL associated with this gift
            /// </summary>
            public string Url
            {
                get
                {
                    return url;
                }
                set
                {
                    if (String.IsNullOrWhiteSpace(value))
                    {
                        url = "";
                    }
                    else
                    {
                        url = value;
                    }
                }
            }
            private double cost = 0.00;
            /// <summary>
            /// The Cost of this gift, as a double.
            /// </summary>
            /// <exception cref="ArgumentOutOfRangeException"></exception>
            public double Cost
            {
                get
                {
                    return cost;
                }
                set
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), "Cost must be positive or 0");
                    }
                    else
                    {
                        cost = value;
                    }
                }
            }
            private string stores = "";
            /// <summary>
            /// Stores this gift is sold at
            /// </summary>
            public string Stores
            {
                get
                {
                    return stores;
                }
                set
                {
                    if (String.IsNullOrWhiteSpace(value))
                    {
                        stores = "";
                    }
                    else
                    {
                        stores = value;
                    }
                }
            }
            private uint quantity = 1;
            /// <summary>
            /// The number of gifts desired
            /// </summary>
            /// <remarks>
            /// If no number (or a number less than 1) is given, 1 is assumed.
            /// </remarks>
            public uint Quantity
            {
                get
                {
                    return quantity;
                }
                set
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, "Quantity must be greater than 0");
                    }
                    else
                    {
                        quantity = value;
                    }
                }
            }
            private string color = "000000";
            /// <summary>
            /// The color, as HEX (without the #)
            /// </summary>
            public string Color
            {
                get
                {
                    return color;
                }
                set
                {
                    if (String.IsNullOrWhiteSpace(value))
                    {
                        color = "000000";
                        return;
                    }
                    value = value.Trim();
                    if (value.Substring(0, 1) == "#")
                    {
                        value = value.Substring(1);
                    }

                    if (value.Length == 0)
                    {
                        color = "000000";
                    }
                    else if (value.Length != 6)
                    {
                        throw new ArgumentException("Invalid value for Color!", nameof(value));
                    }
                    else
                    {
                        color = value.ToUpper();
                    }
                }
            }
            private string colorText = "";
            /// <summary>
            /// A description text for this color
            /// </summary>
            public string ColorText
            {
                get
                {
                    return colorText;
                }
                set
                {
                    if (String.IsNullOrWhiteSpace(value))
                    {
                        colorText = "";
                    }
                    else
                    {
                        colorText = value;
                    }
                }
            }
            private string size = "";
            /// <summary>
            /// The size of this gift
            /// </summary>
            public string Size
            {
                get
                {
                    return size;
                }
                set
                {
                    if (String.IsNullOrWhiteSpace(value))
                    {
                        size = "";
                    }
                    else
                    {
                        size = value;
                    }
                }
            }
            private Category category;
            /// <summary>
            /// The category this gift fits under
            /// </summary>
            public Category Category
            {
                get
                {
                    return category;
                }
                set
                {
                    if (value == null)
                    {
                        // Default category
                        category = new Category(1);
                    }
                    else
                    {
                        category = value;
                    }
                }
            }
            private double rating = 0.00;
            /// <summary>
            /// This gift's rating, between 0 and 5 only.
            /// </summary>
            public double Rating
            {
                get
                {
                    return rating;
                }
                set
                {
                    if (value > 5 || value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, "Rating must be between 0 and 5, inclusive");
                    }
                    else
                    {
                        rating = value;
                    }
                }
            }
            /// <summary>
            /// The time this gift was created
            /// </summary>
            public DateTime? TimeStamp
            {
                get;
                private set;
            } = null;
            /// <summary>
            /// The time this gift was received
            /// </summary>
            public DateTime? DateReceived = null;
            /// <summary>
            /// A List of reservations for this gift
            /// </summary>
            public List<Reservation> Reservations
            {
                get
                {
                    if (ID != 0)
                    {
                        List<Reservation> _reservations = new List<Reservation>();
                        using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                        {
                            con.Open();
                            using (MySqlCommand cmd = new MySqlCommand())
                            {
                                cmd.Connection = con;
                                cmd.CommandText = "SELECT ReservationID FROM reservations WHERE GiftID = @gid;";
                                cmd.Parameters.AddWithValue("@gid", ID);
                                cmd.Prepare();
                                using (MySqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        _reservations.Add(new Reservation(Convert.ToUInt64(reader["ReservationID"])));
                                    }
                                }
                            }
                        }
                        return _reservations;
                    }
                    else
                    {
                        throw new InvalidOperationException("Cannot get reservations for ID-less gift");
                    }
                }
            }
            /// <summary>
            /// All the groups this gift is viewable to
            /// </summary>
            public List<Group> Groups
            {
                get
                {
                    if (ID != 0)
                    {
                        List<Group> _groups = new List<Group>();
                        using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                        {
                            con.Open();
                            using (MySqlCommand cmd = new MySqlCommand())
                            {
                                cmd.Connection = con;
                                cmd.CommandText = "SELECT GroupID FROM groups_gifts WHERE GiftID = @gid;";
                                cmd.Parameters.AddWithValue("@gid", ID);
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
                    else
                    {
                        throw new InvalidOperationException("Cannot get groups for ID-less gift");
                    }
                }
            }
            /// <summary>
            /// Fetch an existing gift
            /// </summary>
            /// <param name="id">The existing gift</param>
            public Gift(ulong id)
            {
                if (id == 0)
                {
                    throw new ArgumentException("Invalid ID for gift", nameof(id));
                }
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
                                ID = id;
                                Owner = new User(Convert.ToUInt64(reader["UserID"]));
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
                                if (DBNull.Value == reader["GiftReceivedDate"])
                                {
                                    DateReceived = null;
                                }
                                else
                                {
                                    DateReceived = (DateTime)(reader["GiftReceivedDate"]);
                                }
                            }
                            else
                            {
                                throw new ArgumentException("Unknown ID " + id, nameof(id));
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Create a new gift with the given name
            /// </summary>
            /// <param name="Name">The new gifts name</param>
            /// <param name="owner">The owner of this gift</param>
            public Gift(string Name, User owner)
            {
                this.Name = Name;
                this.Owner = owner;
            }
            /// <summary>
            /// Create the gift within the database
            /// </summary>
            public void Create()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO gifts (UserID, GiftName, GiftDescription, GiftURL, GiftCost, GiftStores, GiftQuantity, GiftColor, GiftColorText, GiftSize, CategoryID, GiftRating, GiftReceivedDate) "
                                        + "VALUES (@uid, @name, @desc, @url, @cost, @stores, @quantity, @hex, @color, @size, @category, @rating, @rec);";
                        cmd.Parameters.AddWithValue("@uid", Owner.ID);
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
                        cmd.Parameters.AddWithValue("@rec", DateReceived.HasValue ? DateReceived.Value.ToString("yyyy-MM-dd") : null);
                        cmd.Prepare();
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            ID = Convert.ToUInt64(cmd.LastInsertedId);
                            using (MySqlCommand timer = new MySqlCommand())
                            {
                                timer.Connection = con;
                                timer.CommandText = "SELECT GiftAddStamp FROM gifts WHERE GiftID = @gid;";
                                timer.Parameters.AddWithValue("@gid", ID);
                                timer.Prepare();
                                using (MySqlDataReader reader = timer.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        TimeStamp = (DateTime)(reader["GiftAddStamp"]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Update this gift in the database
            /// </summary>
            public void Update()
            {
                if (ID == 0)
                {
                    Create();
                }
                else
                {
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
                            cmd.Parameters.AddWithValue("@gid", ID);
                            cmd.Parameters.AddWithValue("@rec", DateReceived.HasValue ? DateReceived.Value.ToString("yyyy-MM-dd") : null);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            /// <summary>
            /// Delete this gift from the database
            /// </summary>
            public void Delete()
            {
                if (ID != 0)
                {
                    // Delete from reservations, remove from groups, remove our image, delete from main table:
                    foreach (Reservation res in Reservations)
                    {
                        res.Delete();
                    }
                    foreach (Group group in Groups)
                    {
                        group.Remove(this);
                    }
                    RemoveImage();
                    using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                    {
                        con.Open();
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM gifts WHERE GiftID = @gid;";
                            cmd.Parameters.AddWithValue("@gid", ID);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                            ID = 0;
                        }
                    }
                }
            }
            /// <summary>
            /// Save the specified image as the image for this gift.
            /// </summary>
            /// <param name="contents">The image as a byte array</param>
            public void SaveImage(byte[] contents)
            {
                if (ID == 0)
                {
                    throw new InvalidOperationException("Cannot save image of ID-less gift");
                }
                else if (contents == null || contents.Length == 0)
                {
                    RemoveImage();
                }
                else
                {
                    ImageProcessor processor = new ImageProcessor(contents);
                    File.WriteAllBytes(Directory.GetCurrentDirectory() + "/resources/images/gifts/Giftr" + ID + Constants.ImageFormat, processor.Data);
                }
            }
            /// <summary>
            /// Remove the associated image
            /// </summary>
            public void RemoveImage()
            {
                if (ID == 0)
                {
                    throw new InvalidOperationException("Cannot remove image of ID-less gift");
                }
                else if (File.Exists(Directory.GetCurrentDirectory() + "/resources/images/gifts/Gift" + ID + Constants.ImageFormat))
                {
                    File.Delete(Directory.GetCurrentDirectory() + "/resources/images/gifts/Gift" + ID + Constants.ImageFormat);
                }
            }
            /// <summary>
            /// Get the image associated with this gift
            /// </summary>
            /// <returns>A qualified path for this gift's image</returns>
            /// <remarks>
            /// Note that qualified means with respect to the server's root, *not* necessarily '/' or 'C:\'
            /// </remarks>
            public string GetImage()
            {
                if (ID == 0)
                {
                    throw new InvalidOperationException("Cannot retrieve image of ID-less gift");
                }
                return GetImage(ID);
            }
            /// <summary>
            /// Get the image for a specified giftID
            /// </summary>
            /// <param name="id">The specified GiftID</param>
            /// <returns>The qualified path (See GetImage() for more information)</returns>
            public static string GetImage(ulong id)
            {
                if (id == 0)
                {
                    throw new ArgumentException("Cannot retrieve image of ID-less gift", nameof(id));
                }
                // Build path:
                string path = Directory.GetCurrentDirectory() + "/resources/images/gifts/Gift" + id + Constants.ImageFormat;
                // if file exists, return path. Otherwise, return default
                // Race condition, but I don't know how to solve (yet)
                if (File.Exists(path))
                {
                    return "resources/images/gifts/Gift" + id + Constants.ImageFormat;
                }
                else
                {
                    return "resources/images/gifts/default" + Constants.ImageFormat;
                }
            }
            /// <summary>
            /// Add this gift to a group
            /// </summary>
            /// <param name="group">The group that can now view this gift</param>
            public void Add(Group group)
            {
                if (group == null)
                {
                    throw new ArgumentNullException(nameof(group), "Group cannot be null");
                }
                else if (group.ID == 0)
                {
                    throw new ArgumentException("Group must have valid ID", nameof(group));
                }
                else if (ID == 0)
                {
                    throw new InvalidOperationException("Cannot add ID-less gift");
                }
                group.Add(this);
            }
            /// <summary>
            /// Remove this gift from a group
            /// </summary>
            /// <param name="group">The group that will no longer be able to view this gift</param>
            public void Remove(Group group)
            {
                if (group == null)
                {
                    throw new ArgumentNullException(nameof(group), "Group must not be null");
                }
                else if (group.ID == 0)
                {
                    throw new ArgumentException("Group must have valid ID", nameof(group));
                }
                else if (ID == 0)
                {
                    throw new InvalidOperationException("Cannot remove from ID-less gift");
                }
                group.Remove(this);
            }
            /// <summary>
            /// Reserve one of this gift for the reserver
            /// </summary>
            /// <param name="reserver">The reserver</param>
            public void Reserve(User reserver)
            {
                if (reserver == null)
                {
                    throw new ArgumentNullException(nameof(reserver), "Reserver must not be null");
                }
                else if (reserver.ID == 0)
                {
                    throw new ArgumentException("Reserver must have valid ID", nameof(reserver));
                }
                else if (ID == 0)
                {
                    throw new InvalidOperationException("ID must not be 0");
                }
                else if (reserver.Equals(Owner))
                {
                    throw new InvalidOperationException("User cannot reserve own gift");
                }
                else
                {
                    reserver.Reserve(this);
                }
            }
            /// <summary>
            /// Reserve a number of this gift
            /// </summary>
            /// <param name="reserver">The reserver</param>
            /// <param name="amount">The amount to reserve</param>
            /// <returns>The amount actually reserved</returns>
            public int Reserve(User reserver, int amount)
            {
                if (amount < 0)
                {
                    return Release(reserver, -amount);
                }
                else
                {
                    int counter = 0;
                    for (int i = 0; i < amount; i++)
                    {
                        try
                        {
                            Reserve(reserver);
                            counter++;
                        }
                        catch (ReservationOverflowException)
                        {
                            return counter;
                        }
                    }
                    return counter;
                }
            }
            /// <summary>
            /// Release a reservation owned by the releaser
            /// </summary>
            /// <param name="releaser">The owner of the reservation</param>
            public void Release(User releaser)
            {
                if (releaser == null)
                {
                    throw new ArgumentNullException(nameof(releaser), "Releaser must not be null");
                }
                else if (releaser.ID == 0)
                {
                    throw new ArgumentException("Releaser must have valid ID", nameof(releaser));
                }
                else if (ID == 0)
                {
                    throw new InvalidOperationException("ID must not be 0");
                }
                else if (releaser.Equals(Owner))
                {
                    throw new InvalidOperationException("User cannot Release own gift");
                }
                else
                {
                    releaser.Release(this);
                }
            }
            /// <summary>
            /// Release a given amount of the reservations for this user
            /// </summary>
            /// <param name="realeser">The releaser</param>
            /// <param name="amount">The number to release</param>
            /// <returns>How many were actually released</returns>
            public int Release(User realeser, int amount)
            {
                if (amount < 0)
                {
                    return Reserve(realeser, -amount);
                }
                else
                {
                    int counter = 0;
                    for (int i = 0; i < amount; i++)
                    {
                        Release(realeser);
                        counter++;
                    }
                    return counter;
                }
            }
            /// <summary>
            /// See if this object is this gift
            /// </summary>
            /// <param name="obj">The object to inspect</param>
            /// <returns>If the object is the same as this gift</returns>
            public override bool Equals(object obj)
            {
                if (obj != null && obj is Gift g)
                {
                    return Equals(g);
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// See if the two gifts are equal
            /// </summary>
            /// <param name="gift">The gift to compare</param>
            /// <returns>If the two gifts are equal</returns>
            public bool Equals(Gift gift)
            {
                return gift != null && gift.ID == ID;
            }
            /// <summary>
            /// Get the hash code for this gift
            /// </summary>
            /// <returns>This gift's hash code</returns>
            public override int GetHashCode()
            {
                return ID.GetHashCode();
            }
            /// <summary>
            /// Serialize this gift as an XML Document
            /// </summary>
            /// <remarks>
            /// This is a serialization for a gift. Note that the User element is _not_ expanded.
            /// This XML Document contains the following fields:
            ///     - giftId: This gift's ID
            ///     - user: The UserID associated with this gift
            ///     - name: This gift's name
            ///     - description: This gift's description
            ///     - url: This gift's URL
            ///     - cost: This gift's cost, encoded WITHOUT the currency. Note that currency isn't currently supported, but will be soon.
            ///     - stores: The stores where this gift is sold, as a string
            ///     - quantity: The quantity desired for this gift
            ///     - color: The hex code for this gift's color, with the leading # included
            ///     - colorText: The color description for this gift's color
            ///     - size: The size of this gift
            ///     - category: The name of the gift category
            ///     - rating: The rating for this gift
            ///     - image: The qualified path for this gift's image
            ///     - dateReceived: The day this was received; otherwise ""
            ///     - groups: The groups that can view this gift
            ///         - Note that each child element of _groups_ is a _group_ element
            ///     - reservations: The reservations currently held for this gift
            ///         - Note that each child element of _reservations_ is a _reservation_ element
            ///         
            /// All this is wrapped in a _gift_ container (I know, pun)
            /// </remarks>
            /// <returns>An XML document with all gift information</returns>
            public XmlDocument Fetch()
            {
                if (ID == 0)
                {
                    throw new InvalidOperationException("Cannot fetch ID-less gift");
                }
                XmlDocument info = new XmlDocument();
                XmlElement container = info.CreateElement("gift");
                info.AppendChild(container);
                XmlElement id = info.CreateElement("giftId");
                id.InnerText = ID.ToString();
                XmlElement user = info.CreateElement("user");
                user.InnerText = Owner.ID.ToString();
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
                XmlElement dateReceived = info.CreateElement("dateReceived");
                dateReceived.InnerText = DateReceived.HasValue ? DateReceived.Value.ToString("yyyy-MM-dd") : "";
                XmlElement groups = info.CreateElement("groups");
                foreach (Group group in Groups)
                {
                    XmlElement groupElem = info.CreateElement("group");
                    groupElem.InnerText = group.ID.ToString();
                    groups.AppendChild(groupElem);
                }
                XmlElement reservations = info.CreateElement("reservations");
                foreach (Reservation reservation in Reservations)
                {
                    reservations.AppendChild(info.ImportNode(reservation.Fetch().DocumentElement, true));
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
                container.AppendChild(dateReceived);
                container.AppendChild(groups);
                container.AppendChild(reservations);

                return info;
            }

            /// <summary>
            /// Fetch a gift, as visible by the viewer
            /// </summary>
            /// <param name="viewer">The viewer</param>
            /// <returns>All information about this gift the viewer can see</returns>
            public XmlDocument Fetch(User viewer)
            {
                if (viewer == null)
                {
                    throw new ArgumentNullException(nameof(viewer), "Viewer must not be null");
                }
                else if (viewer.ID == 0)
                {
                    throw new ArgumentException("Viewer must have valid ID", nameof(viewer));
                }
                else if (ID == 0)
                {
                    throw new InvalidOperationException("Cannot fetch ID-less gift");
                }
                if (viewer.ID == Owner.ID)
                {
                    return Fetch();
                }
                else if (Owner.GetGifts(viewer).Exists(g => g.ID == ID))
                {
                    XmlDocument info = new XmlDocument();
                    XmlElement container = info.CreateElement("gift");
                    info.AppendChild(container);
                    XmlElement id = info.CreateElement("giftId");
                    id.InnerText = ID.ToString();
                    XmlElement user = info.CreateElement("user");
                    user.InnerText = Owner.ID.ToString();
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
                    XmlElement dateReceived = info.CreateElement("dateReceived");
                    dateReceived.InnerText = DateReceived.HasValue ? DateReceived.Value.ToString("yyyy-MM-dd") : "";
                    XmlElement groups = info.CreateElement("groups");
                    // only attach group if viewer is also in that group
                    foreach (Group group in Groups.FindAll(group => group.Users.Exists(u => u.ID == viewer.ID)))
                    {
                        XmlElement groupElem = info.CreateElement("group");
                        groupElem.InnerText = group.ID.ToString();
                        groups.AppendChild(groupElem);
                    }
                    XmlElement reservations = info.CreateElement("reservations");
                    foreach (Reservation reservation in Reservations)
                    {
                        reservations.AppendChild(info.ImportNode(reservation.Fetch(viewer).DocumentElement, true));
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
                    container.AppendChild(dateReceived);
                    container.AppendChild(groups);
                    container.AppendChild(reservations);

                    return info;
                }
                else
                {
                    XmlDocument info = new XmlDocument();
                    XmlElement container = info.CreateElement("gift");
                    info.AppendChild(container);

                    return info;
                }
            }
        }
    }
}