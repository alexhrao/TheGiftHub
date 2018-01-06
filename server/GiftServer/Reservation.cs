using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        public class Reservation : IFetchable
        {
            public ulong ReservationId = 0;
            public User User;
            public Gift Gift;
            public bool IsPurchased = false;

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
                purchased.InnerText = IsPurchased ? "1" : "0";

                container.AppendChild(resId);
                container.AppendChild(user);
                container.AppendChild(gift);
                container.AppendChild(purchased);
                return info;
            }
        }
    }
}
