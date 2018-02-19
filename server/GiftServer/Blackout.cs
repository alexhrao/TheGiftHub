using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// A Blacked Out Date for an event
        /// </summary>
        /// <remarks>
        /// A Blackout is a day where an event _should_ occur, but won't.
        /// </remarks>
        public class Blackout : ISynchronizable, IFetchable
        {
            /// <summary>
            /// The ID of this Blackout
            /// </summary>
            public ulong ID
            {
                get;
                private set;
            } = 0;
            /// <summary>
            /// The event this blacks out
            /// </summary>
            public Event Event
            {
                get;
                private set;
            }
            /// <summary>
            /// The date to blackout an event
            /// </summary>
            public DateTime BlackoutDate
            {
                get;
                private set;
            }
            /// <summary>
            /// Create a new blackout date for a specified event
            /// </summary>
            /// <param name="Event">The event to blackout</param>
            /// <param name="blackoutDate">The date to blackout</param>
            public Blackout(Event Event, DateTime blackoutDate)
            {
                this.Event = Event;
                BlackoutDate = blackoutDate;
            }
            /// <summary>
            /// Fetch an existing Blackout from the database
            /// </summary>
            /// <param name="blackoutId">The blackout ID</param>
            /// <param name="e">The Event tied to this blackout event</param>
            public Blackout(ulong blackoutId, Event e)
            {
                // Why can't we just create the event here? Confused...
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT EventBlackoutID, EventID, EventBlackoutDate FROM event_blackouts WHERE EventBlackoutID = @bid;";
                        cmd.Parameters.AddWithValue("@bid", blackoutId);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ID = blackoutId;
                                Event = e;
                                if (e.ID != Convert.ToUInt64(reader["EventID"]))
                                {
                                    throw new InvalidOperationException("EventIDs do not match!");
                                }
                                BlackoutDate = (DateTime)(reader["EventBlackoutDate"]);
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Create a record of this blackout date in the database
            /// </summary>
            /// <remarks>
            /// Note that, since Event already creates this, it is unlikely the end user will need this method
            /// </remarks>
            public void Create()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    // First check that no bo already exists with same date
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT EventBlackoutID FROM event_blackouts WHERE EventBlackoutDate = @dt;";
                        cmd.Parameters.AddWithValue("@dt", BlackoutDate);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                throw new InvalidOperationException("Event Blackout for date already exists");
                            }
                        }
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO event_blackouts (EventID, EventBlackoutDate) VALUES (@eid, @ebd);";
                        cmd.Parameters.AddWithValue("@eid", Event.ID);
                        cmd.Parameters.AddWithValue("@ebd", BlackoutDate);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        ID = Convert.ToUInt64(cmd.LastInsertedId);
                    }
                }
            }
            /// <summary>
            /// Update the record of this blackout date in the database
            /// </summary>
            /// <remarks>
            /// Note that, since Event already updates this, it is unlikely the end user will need this method
            /// </remarks>
            public void Update()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    // First check that no bo already exists with same date
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT EventBlackoutID FROM event_blackouts WHERE EventBlackoutDate = @dt;";
                        cmd.Parameters.AddWithValue("@dt", BlackoutDate);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read() && Convert.ToUInt64(reader["EventBlackoutID"]) != ID)
                            {
                                throw new InvalidOperationException("Event Blackout for date already exists");
                            }
                        }
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "UPDATE event_blackouts SET EventBlackoutDate = @ebd WHERE EventBlackoutID = @bid;";
                        cmd.Parameters.AddWithValue("@ebd", BlackoutDate);
                        cmd.Parameters.AddWithValue("@bid", ID);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            /// <summary>
            /// Delete the record of this blackout date
            /// </summary>
            /// <remarks>
            /// Note that, since Event already deletes this, it is unlikely the end user will need this method
            /// </remarks>
            public void Delete()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM event_blackouts WHERE EventBlackoutID = @bid;";
                        cmd.Parameters.AddWithValue("@bid", ID);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        ID = 0;
                    }
                }
            }
            /// <summary>
            /// Serialize this BlackoutDate
            /// </summary>
            /// <returns>A Serialized form of this ruleset</returns>
            /// <remarks>
            /// This returns an XML document with the following information:
            ///     - blackoutId: The ID of this reservation
            ///     - eventId: The ID of the corresponding event
            ///     - blackoutDate: The blackout date, represented as yyyy-MM-dd
            ///     
            /// This is all wrapped in a blackout container
            /// </remarks>
            public XmlDocument Fetch()
            {
                XmlDocument info = new XmlDocument();
                XmlElement container = info.CreateElement("blackout");
                info.AppendChild(container);

                XmlElement id = info.CreateElement("blackoutId");
                id.InnerText = ID.ToString();
                XmlElement eventId = info.CreateElement("eventId");
                eventId.InnerText = Event.ID.ToString();
                XmlElement blackoutDate = info.CreateElement("blackoutDate");
                blackoutDate.InnerText = BlackoutDate.ToString("yyyy-MM-dd");

                container.AppendChild(id);
                container.AppendChild(eventId);
                container.AppendChild(blackoutDate);
                return info;
            }
            /// <summary>
            /// Get the blackout date visible to this user
            /// </summary>
            /// <param name="viewer">The viewer</param>
            /// <returns>The same as Fetch()</returns>
            public XmlDocument Fetch(User viewer)
            {
                if (Event.User.GetEvents(viewer).Exists(e => e.ID == Event.ID))
                {
                    // return Fetch
                    return Fetch();
                }
                else
                {
                    XmlDocument info = new XmlDocument();
                    XmlElement container = info.CreateElement("blackout");
                    info.AppendChild(container);
                    return info;
                }
            }
        }
    }
}