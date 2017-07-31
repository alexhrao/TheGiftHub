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
            public long id = -1;
            public string Name;
            public string Description;
            public int Day = -1;
            public int Month = -1;
            public long UserID;
            public Event(long userID, string name, int day, int month, string description)
            {
                // Check if user exists
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT UserID FROM users WHERE users.UserID = @id;";
                        cmd.Parameters.AddWithValue("@id", userID);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                this.UserID = userID;
                            }
                            else
                            {
                                throw new UserNotFoundException(userID);
                            }
                        }
                    }
                }
                this.Name = name;
                if (month < 1 || month > 12)
                {
                    throw new InvalidEventException("month", month);
                }
                this.Month = month;
                if (day < 1 || day > 31)
                {
                    throw new InvalidEventException("day", day);
                }
                this.Day = day;
                this.Description = description;
            }
            public Event(long userID, string name)
            {
                this.UserID = userID;
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT eventUsers.* FROM eventUsers WHERE eventUsers.UserID = @id AND eventUsers.EventName = @name;";
                        cmd.Parameters.AddWithValue("@id", userID);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                this.id = Convert.ToInt64(reader["EventUserID"]);
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
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO eventsUsers (UserID, EventDay, EventMonth, EventName, EventDescription) VALUES ("
                                        + "@id, "
                                        + "@day, "
                                        + "@month, "
                                        + "@name, "
                                        + "@description);";
                        cmd.Parameters.AddWithValue("@id", this.UserID);
                        cmd.Parameters.AddWithValue("@day", this.Day);
                        cmd.Parameters.AddWithValue("@month", this.Month);
                        cmd.Parameters.AddWithValue("@name", this.Name);
                        cmd.Parameters.AddWithValue("@description", this.Description);
                        cmd.Prepare();
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            this.id = cmd.LastInsertedId;
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
                if (this.id == -1)
                {
                    // User does not exist - create new one instead.
                    return Create();
                }
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "UPDATE eventsUsers "
                                        + "SET "
                                        + "UserID = @uid, "
                                        + "EventDay = @day, "
                                        + "EventMonth = @month, "
                                        + "EventName = @name, "
                                        + "EventDescription = @description "
                                        + "WHERE EventUserID = @id";
                        cmd.Parameters.AddWithValue("@id", this.id);
                        cmd.Parameters.AddWithValue("@uid", this.UserID);
                        cmd.Parameters.AddWithValue("@day", this.Day);
                        cmd.Parameters.AddWithValue("@month", this.Month);
                        cmd.Parameters.AddWithValue("@name", this.Name);
                        cmd.Parameters.AddWithValue("@description", this.Description);
                        cmd.Prepare();
                        return cmd.ExecuteNonQuery() == 1;
                    }
                }
            }

            public bool Delete()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM eventsUsers WHERE eventsUsers.EventUserID = @id;";
                        cmd.Parameters.AddWithValue("@id", this.id);
                        cmd.Prepare();
                        return cmd.ExecuteNonQuery() == 1;
                    }
                }
            }
        }
    }
}
