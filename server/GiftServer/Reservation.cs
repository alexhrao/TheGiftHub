using GiftServer.Exceptions;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// A single reservation for a gift
        /// </summary>
        public class Reservation : ISynchronizable, IFetchable, IEquatable<Reservation>
        {
            /// <summary>
            /// The reservation ID
            /// </summary>
            public ulong ID
            {
                get;
                private set;
            }
            /// <summary>
            /// The user who reserved the gift
            /// </summary>
            public User User;
            /// <summary>
            /// The gift itself
            /// </summary>
            public Gift Gift;
            private bool isPurchased = false;
            /// <summary>
            /// Whether or not this has been marked as purchased
            /// </summary>
            public bool IsPurchased
            {
                get
                {
                    return isPurchased;
                }
                set
                {
                    if (value)
                    {
                        PurchaseDate = DateTime.Now;
                        isPurchased = true;
                    }
                    else
                    {
                        PurchaseDate = null;
                        isPurchased = false;
                    }
                }
            }
            /// <summary>
            /// The date this reservation was marked as purchased
            /// </summary>
            public DateTime? PurchaseDate
            {
                get;
                private set;
            }
            /// <summary>
            /// The date this reservation was made
            /// </summary>
            public DateTime? ReserveDate
            {
                get;
                private set;
            }
            /// <summary>
            /// Fetch an existing reservation record from the database
            /// </summary>
            /// <param name="reservationId">The ReservationID</param>
            public Reservation(ulong reservationId)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT UserID, GiftID, PurchaseStamp, ReserveStamp FROM reservations WHERE ReservationID = @rid;";
                        cmd.Parameters.AddWithValue("@rid", reservationId);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                User = new User(Convert.ToUInt64(reader["UserID"]));
                                Gift = new Gift(Convert.ToUInt64(reader["GiftID"]));
                                ReserveDate = (DateTime)(reader["ReserveStamp"]);
                                if (reader["PurchaseStamp"] == DBNull.Value)
                                {
                                    PurchaseDate = null;
                                }
                                else
                                {
                                    PurchaseDate = (DateTime)reader["PurchaseStamp"];
                                }
                                isPurchased = PurchaseDate.HasValue;
                                ID = reservationId;
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Initialize a new reservation from the given user and gift
            /// </summary>
            /// <param name="user">The User who reserved this gift</param>
            /// <param name="gift">The gift being reserved</param>
            public Reservation(User user, Gift gift)
            {
                User = user;
                Gift = gift;
            }

            /// <summary>
            /// Creates a record of this in the Database
            /// </summary>
            public void Create()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    bool left = false;
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT COUNT(*) AS NumRes FROM reservations WHERE GiftID = @gid;";
                        cmd.Parameters.AddWithValue("@gid", Gift.ID);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            left = reader.Read() && Convert.ToUInt32(reader["NumRes"]) < Gift.Quantity;
                        }
                    }
                    if (left)
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            // Add to reserved:
                            cmd.CommandText = "INSERT INTO reservations (GiftID, UserID, PurchaseStamp) VALUES (@gid, @uid, @pst);";
                            cmd.Parameters.AddWithValue("@gid", Gift.ID);
                            cmd.Parameters.AddWithValue("@uid", User.ID);
                            cmd.Parameters.AddWithValue("@pst", PurchaseDate);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                            ID = Convert.ToUInt64(cmd.LastInsertedId);
                        }
                    }
                    else
                    {
                        throw new ReservationOverflowException(Gift);
                    }
                    // If purchased, insert into purchased table? (or just have date that reps purchase date)
                }
            }

            /// <summary>
            /// Updates a record of this in the database
            /// </summary>
            public void Update()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "UPDATE reservations SET UserID = @uid, GiftID = @gid WHERE ReservationID = @rid;";
                        cmd.Parameters.AddWithValue("@uid", User.ID);
                        cmd.Parameters.AddWithValue("@gid", Gift.ID);
                        cmd.Parameters.AddWithValue("@rid", ID);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            /// <summary>
            /// Deletes the record of this from the database
            /// </summary>
            public void Delete()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM reservations WHERE ReservationID = @rid;";
                        cmd.Parameters.AddWithValue("@rid", ID);
                        ID = 0;
                    }
                }
            }
            /// <summary>
            /// See if the given object is the same as this reservation
            /// </summary>
            /// <param name="obj">The object to compare</param>
            /// <returns>Whether or not the two objects are actually the same</returns>
            public override bool Equals(object obj)
            {
                if (obj != null && obj is Reservation r)
                {
                    return Equals(r);
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// See if this reservation is the same as the given one
            /// </summary>
            /// <param name="reservation">The reservation to check</param>
            /// <returns>Whether or not the two reservations are equal</returns>
            public bool Equals(Reservation reservation)
            {
                return reservation != null && reservation.ID == ID;
            }
            /// <summary>
            /// Get the hash code for this reservation
            /// </summary>
            /// <returns>The hash code for this reservation</returns>
            public override int GetHashCode()
            {
                return ID.GetHashCode();
            }
            /// <summary>
            /// Serializes this reservation
            /// </summary>
            /// <remarks>
            /// As with other Fetch() methods, this method will serialize the reservation as an XML Document
            /// This document has the following fields:
            ///     - reservationId: The ID for this reservation
            ///     - userId: The reserver's ID
            ///     - giftId: The ID of the reserved gift
            ///     - isPurchased: A boolean encoded as either "true" or "false"
            ///     
            /// This is all wrapped in a _reservation_ container
            /// </remarks>
            /// <returns></returns>
            public XmlDocument Fetch()
            {
                XmlDocument info = new XmlDocument();
                XmlElement container = info.CreateElement("reservation");
                info.AppendChild(container);

                XmlElement resId = info.CreateElement("reservationId");
                resId.InnerText = ID.ToString();
                XmlElement user = info.CreateElement("userId");
                user.InnerText = User.ID.ToString();
                XmlElement gift = info.CreateElement("giftId");
                gift.InnerText = Gift.ID.ToString();
                XmlElement purchased = info.CreateElement("isPurchased");
                purchased.InnerText = IsPurchased.ToString().ToLower();

                container.AppendChild(resId);
                container.AppendChild(user);
                container.AppendChild(gift);
                container.AppendChild(purchased);
                return info;
            }
        }
    }
}
