using System;
using MySql.Data.MySqlClient;
using System.Configuration;
using GiftServer.Exceptions;
using System.Collections.Generic;

namespace GiftServer
{
    namespace Data
    {
        public class EventUser : ISynchronizable
        {
            public long EventUserID;
            public User user;
            public string Name;
            public string Description;
            public int Day;
            public int Month;
            public int Year;
            public bool IsRecurring;
            public List<EventFuture> Futures = new List<EventFuture>();
            private readonly bool _isDefault;

            public EventUser() { }
            public EventUser(long EventUserID)
            {
                // Get information; if from Default Events, create a new default event and then copy over information
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT * FROM events_users WHERE EventUserID = @id;";
                        cmd.Parameters.AddWithValue("@id", EventUserID);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Get user:
                                this.user = new User(Convert.ToInt64(reader["UserID"]));
                                if (!Convert.IsDBNull(reader["EventID"]))
                                {
                                    DefaultEvent defEvent = new DefaultEvent(Convert.ToInt64(reader["EventID"]));
                                    // Copy over data
                                    this._isDefault = true;
                                    this.Name = defEvent.Name;
                                    this.Description = defEvent.Description;
                                    this.Day = defEvent.Day;
                                    this.Month = defEvent.Month;
                                    this.Year = defEvent.Year;
                                    this.IsRecurring = defEvent.IsRecurring;
                                    this.Futures = defEvent.Futures;
                                    return;
                                }
                                else
                                {
                                    this._isDefault = false;
                                    this.EventUserID = EventUserID;
                                    this.Name = Convert.ToString(reader["EventName"]);
                                    this.Description = Convert.ToString(reader["EventDescription"]);
                                    this.Day = Convert.ToInt32(reader["EventDay"]);
                                    this.Month = Convert.ToInt32(reader["EventMonth"]);
                                    this.Year = Convert.ToInt32(reader["EventYear"]);
                                    this.IsRecurring = Convert.ToBoolean(reader["EventRecurs"]);
                                }
                            }
                        }
                    }
                    if (IsRecurring)
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT * FROM events_users_futures WHERE EventUserID = @id;";
                            cmd.Parameters.AddWithValue("@id", EventUserID);
                            cmd.Prepare();
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    // Create new event future, add to list.
                                    Futures.Add(new EventFuture(Convert.ToInt32(reader["EventYear"]), Convert.ToInt32(reader["EventMonth"]), Convert.ToInt32(reader["EventDay"])));
                                }
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
                return !_isDefault;
            }
            public bool Delete()
            {
                return false;
            }
        }
    }
}
