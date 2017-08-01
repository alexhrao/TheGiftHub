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
            public int Year = -1;
            public bool isRecurring;
            public long UserID;
            public Event(long userID, string name, int day, int month, int year, bool recurs, string description)
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
                this.Year = year;
                this.isRecurring = recurs;
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
                        cmd.CommandText = "SELECT events_users.* FROM events_users WHERE events_users.UserID = @id AND events_users.EventName = @name;";
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
                                this.Year = Convert.ToInt32(reader["EventYear"]);
                                this.isRecurring = Convert.ToBoolean(reader["EventRecurs"]);
                            }
                            else
                            {
                                throw new EventNotFoundException(userID, name);
                            }
                        }
                    }
                }
            }
            public Event(long id)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT events_users.* FROM events_users WHERE events_users.EventUserID = @id;";
                        cmd.Parameters.AddWithValue("@id", id);
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
                                this.Year = Convert.ToInt32(reader["EventYear"]);
                                this.isRecurring = Convert.ToBoolean(reader["EventRecurs"]);
                            }
                            else
                            {
                                throw new EventNotFoundException(id);
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
                        cmd.CommandText = "INSERT INTO events_users (UserID, EventDay, EventMonth, EventYear, EventRecurs, EventName, EventDescription) VALUES ("
                                        + "@id, "
                                        + "@day, "
                                        + "@month, "
                                        + "@year, "
                                        + "@recurs, "
                                        + "@name, "
                                        + "@description);";
                        cmd.Parameters.AddWithValue("@id", this.UserID);
                        cmd.Parameters.AddWithValue("@day", this.Day);
                        cmd.Parameters.AddWithValue("@month", this.Month);
                        cmd.Parameters.AddWithValue("@year", this.Year);
                        cmd.Parameters.AddWithValue("@recurs", this.isRecurring);
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
                        cmd.CommandText = "UPDATE events_users "
                                        + "SET "
                                        + "UserID = @uid, "
                                        + "EventDay = @day, "
                                        + "EventMonth = @month, "
                                        + "EventYear = @year, "
                                        + "EventRecurs = @recurs, "
                                        + "EventName = @name, "
                                        + "EventDescription = @description "
                                        + "WHERE EventUserID = @id";
                        cmd.Parameters.AddWithValue("@id", this.id);
                        cmd.Parameters.AddWithValue("@uid", this.UserID);
                        cmd.Parameters.AddWithValue("@day", this.Day);
                        cmd.Parameters.AddWithValue("@month", this.Month);
                        cmd.Parameters.AddWithValue("@year", this.Year);
                        cmd.Parameters.AddWithValue("@recurs", this.isRecurring);
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
                        cmd.CommandText = "DELETE FROM events_users_groups WHERE events_users_groups.EventUserID = @id;";
                        cmd.Parameters.AddWithValue("@id", this.id);
                        cmd.Prepare();
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM events_users WHERE events_users.EventUserID = @id;";
                        cmd.Parameters.AddWithValue("@id", this.id);
                        cmd.Prepare();
                        return cmd.ExecuteNonQuery() == 1;
                    }
                }
            }
        }
    }
}
