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
        /// <summary>
        /// A default (predefined) event, like New Year's, Christmas, etc.
        /// </summary>
        /// <remarks>
        /// This is a predefined event; as such, you can't create, update, or delete this.
        /// </remarks>
        public class DefaultEvent : IFetchable
        {
            /// <summary>
            /// This EventID
            /// </summary>
            public readonly ulong DefaultEventId;
            /// <summary>
            /// The name of this event
            /// </summary>
            public readonly string Name;
            /// <summary>
            /// The description for this event
            /// </summary>
            public readonly string Description;
            /// <summary>
            /// The day this event occurs
            /// </summary>
            public readonly int Day;
            /// <summary>
            /// The month this event occurs
            /// </summary>
            public readonly int Month;
            /// <summary>
            /// The year this event occurs
            /// </summary>
            public readonly int Year;
            /// <summary>
            /// If this event recurs every year.
            /// </summary>
            /// <remarks>
            /// If this is true, refer to the EventFutures for exact dates of occurence in each year
            /// </remarks>
            public readonly bool IsRecurring;
            /// <summary>
            /// If this event recurs, Futures store when the event occurs each year
            /// </summary>
            public List<EventFuture> Futures = new List<EventFuture>();
            /// <summary>
            /// Create a default Event from the given EventID
            /// </summary>
            /// <param name="EventID">The Default EventID</param>
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

            /// <summary>
            /// Fetch this DefaultEvent
            /// </summary>
            /// <remarks>
            /// This "serializes" the event, with the following fields:
            ///     - defaultEventId: The ID for this event
            ///     - name: The Name for this event
            ///     - description: The description for this event
            ///     - day: The day this event occurs
            ///     - month: The month this event occurs
            ///     - year: The year this event occurs
            ///     - isRecurring: Whether or not this event recurs, given as "true" or "false"
            ///     - eventFutures: A collection of nodes that represent the Futures for this event; refer to EventFutures for more information.
            /// All these fields are held in a container, defaultEvent
            /// </remarks>
            /// <returns>A complete XML serialization</returns>
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