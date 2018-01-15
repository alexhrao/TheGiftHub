﻿using MySql.Data.MySqlClient;
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
        public class Reservation : IFetchable
        {
            /// <summary>
            /// The reservation ID
            /// </summary>
            public readonly ulong ReservationId;
            /// <summary>
            /// The user who reserved the gift
            /// </summary>
            public readonly User User;
            /// <summary>
            /// The gift itself
            /// </summary>
            public readonly Gift Gift;
            /// <summary>
            /// Whether or not this has been marked as purchased
            /// </summary>
            public readonly bool IsPurchased = false;
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
