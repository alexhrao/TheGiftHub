using System;
using MySql.Data.MySqlClient;
using System.Configuration;
using GiftServer.Exceptions;

namespace GiftServer
{
    namespace Data
    {
        public class Event : ISynchronizable
        {
            public string Name;
            public string Description;
            public int Day = -1;
            public int Month = -1;
            public long UserID;
            public Event(long userID, string name)
            {
                this.UserID = userID;
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT giftEventUsers.* FROM giftEventUsers WHERE giftEventUsers.UserID = @id AND giftEventUsers.EventName = @name;";
                        cmd.Parameters.AddWithValue("@id", userID);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                this.Name = (string)(reader["EventName"]);
                                this.Description = (string)(reader["EventDescription"]);
                                this.Day = Convert.ToInt32(reader["EventDay"]);
                                this.Month = Convert.ToInt32(reader["EventMonth"]);
                            }
                            else
                            {
                                throw new EventNotFoundException(userID, name);
                            }
                        }
                    }
                }
            }

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
        }
    }
}
