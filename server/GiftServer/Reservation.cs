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
            public ulong ReservationId
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
            /// <summary>
            /// Whether or not this has been marked as purchased
            /// </summary>
            public bool IsPurchased = false;
            /// <summary>
            /// The date this reservation was marked as purchased
            /// </summary>
            public readonly DateTime PurchaseDate;
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
                        cmd.CommandText = "SELECT UserID, GiftID FROM reservations WHERE ReservationID = @rid;";
                        cmd.Parameters.AddWithValue("@rid", reservationId);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                User = new User(Convert.ToUInt64(reader["UserID"]));
                                Gift = new Gift(Convert.ToUInt64(reader["GiftID"]));
                                // IsPurchased = Convert.ToBoolean(reader["IsPurchased"]);
                                ReservationId = reservationId;
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
            /// <returns>A Status Flag</returns>
            public bool Create()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    bool left = false;
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT COUNT(*) AS NumRes FROM reservations WHERE GiftID = @gid;";
                        cmd.Parameters.AddWithValue("@gid", Gift.GiftId);
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
                            cmd.CommandText = "INSERT INTO reservations (GiftID, UserID) VALUES (@gid, @uid);";
                            cmd.Parameters.AddWithValue("@gid", Gift.GiftId);
                            cmd.Parameters.AddWithValue("@uid", User.UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        throw new ReservationOverflowException(Gift);
                    }
                    // If purchased, insert into purchased table? (or just have date that reps purchase date)
                }
                return true;
            }

            /// <summary>
            /// Updates a record of this in the database
            /// </summary>
            /// <returns>A Status flag</returns>
            public bool Update()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "UPDATE reservations SET UserID = @uid, GiftID = @gid WHERE ReservationID = @rid;";
                        cmd.Parameters.AddWithValue("@uid", User.UserId);
                        cmd.Parameters.AddWithValue("@gid", Gift.GiftId);
                        cmd.Parameters.AddWithValue("@rid", ReservationId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            /// <summary>
            /// Deletes the record of this from the database
            /// </summary>
            /// <returns>A status flag</returns>
            public bool Delete()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM reservations WHERE ReservationID = @rid;";
                        cmd.Parameters.AddWithValue("@rid", ReservationId);
                        ReservationId = 0;
                    }
                }
                return true;
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
                return reservation != null && reservation.ReservationId == ReservationId;
            }
            /// <summary>
            /// Get the hash code for this reservation
            /// </summary>
            /// <returns>The hash code for this reservation</returns>
            public override int GetHashCode()
            {
                return ReservationId.GetHashCode();
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
                resId.InnerText = ReservationId.ToString();
                XmlElement user = info.CreateElement("userId");
                user.InnerText = User.UserId.ToString();
                XmlElement gift = info.CreateElement("giftId");
                gift.InnerText = Gift.GiftId.ToString();
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
