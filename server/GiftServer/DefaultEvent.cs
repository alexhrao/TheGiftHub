using GiftServer.Exceptions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace GiftServer
{
    namespace Data
    {
        public class DefaultEvent
        {
            public readonly ulong EventID;
            public readonly string Name;
            public readonly string Description;
            public readonly int Day;
            public readonly int Month;
            public readonly int Year;
            public readonly bool IsRecurring;

            public List<EventFuture> Futures = new List<EventFuture>();

            public DefaultEvent(ulong EventID)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT * FROM default_events WHERE EventID = @id;";
                        cmd.Parameters.AddWithValue("@id", EventID);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Get data:
                                this.EventID = EventID;
                                this.Name = Convert.ToString(reader["EventName"]);
                                this.Description = Convert.ToString(reader["EventDescription"]);
                                this.Day = Convert.ToInt32(reader["EventDay"]);
                                this.Month = Convert.ToInt32(reader["EventMonth"]);
                                this.Year = Convert.ToInt32(reader["EventYear"]);
                                this.IsRecurring = Convert.ToBoolean(reader["EventRecurs"]);
                            }
                            else
                            {
                                throw new EventNotFoundException(EventID);
                            }
                        }
                    }
                    if (!IsRecurring)
                    {
                        // Get EventFutures:
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT * FROM default_events_futures WHERE EventID = @id;";
                            cmd.Parameters.AddWithValue("@id", EventID);
                            cmd.Prepare();
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {   
                                while (reader.Read())
                                {
                                    // Create new event future, add to list.
                                    Futures.Add(new EventFuture(
                                        Convert.ToUInt64(reader["EventUserFutureID"]),
                                        Convert.ToInt32(reader["EventYear"]), 
                                        Convert.ToInt32(reader["EventMonth"]), 
                                        Convert.ToInt32(reader["EventDay"])));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}