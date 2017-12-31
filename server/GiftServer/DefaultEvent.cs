using GiftServer.Exceptions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        public class DefaultEvent : IFetchable
        {
            public readonly ulong DefaultEventId;
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
                                this.DefaultEventId = EventID;
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
                                        Convert.ToUInt64(reader["EventFutureID"]),
                                        Convert.ToInt32(reader["EventYear"]), 
                                        Convert.ToInt32(reader["EventMonth"]), 
                                        Convert.ToInt32(reader["EventDay"])));
                                }
                            }
                        }
                    }
                }
            }

            public XmlDocument Fetch()
            {
                XmlDocument info = new XmlDocument();
                XmlElement container = info.CreateElement("defaultEvent");
                info.AppendChild(container);
                XmlElement id = info.CreateElement("defaultEventId");
                id.InnerText = DefaultEventId.ToString();
                XmlElement name = info.CreateElement("name");
                name.InnerText = Name;
                XmlElement description = info.CreateElement("description");
                description.InnerText = Description;
                XmlElement day = info.CreateElement("day");
                day.InnerText = Day.ToString();
                XmlElement month = info.CreateElement("month");
                month.InnerText = Month.ToString();
                XmlElement year = info.CreateElement("year");
                year.InnerText = Year.ToString();
                XmlElement isRecurring = info.CreateElement("isRecurring");
                isRecurring.InnerText = IsRecurring.ToString();

                container.AppendChild(id);
                container.AppendChild(name);
                container.AppendChild(description);
                container.AppendChild(day);
                container.AppendChild(month);
                container.AppendChild(year);
                container.AppendChild(isRecurring);

                XmlElement futures = info.CreateElement("eventFutures");
                foreach (EventFuture future in Futures)
                {
                    futures.AppendChild(future.Fetch().DocumentElement);
                }

                container.AppendChild(futures);
                return info;
            }
        }
    }
}