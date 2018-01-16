using System;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// An event, as defined by the user
        /// </summary>
        public class EventUser : ISynchronizable, IFetchable
        {
            /// <summary>
            /// The ID for this EventUser
            /// </summary>
            public ulong EventUserId
            {
                get;
                private set;
            } = 0;
            /// <summary>
            /// The user that owns this event
            /// </summary>
            public User User;
            private string _name;
            private string _description;
            private int _day;
            private int _month;
            private int _year;
            private bool _isRecurring;
            private List<EventFuture> _futures = new List<EventFuture>();
            /// <summary>
            /// The name of this event
            /// </summary>
            public string Name
            {
                get
                {
                    if (IsDefault)
                    {
                        return _defaultEvent.Name;
                    }
                    else
                    {
                        return this._name;
                    }
                }
                set
                {
                    this.IsDefault = false;
                    this._name = value;
                }
            }
            /// <summary>
            /// The description for this event
            /// </summary>
            public string Description
            {
                get
                {
                    if (IsDefault)
                    {
                        return _defaultEvent.Description;
                    }
                    else
                    {
                        return this._description;
                    }
                }
                set
                {
                    this.IsDefault = false;
                    this._description = value;
                }
            }
            /// <summary>
            /// The day this event occurs on
            /// </summary>
            public int Day
            {
                get
                {
                    if (IsDefault)
                    {
                        return _defaultEvent.Day;
                    }
                    else
                    {
                        return this._day;
                    }
                }
                set
                {
                    this.IsDefault = false;
                    this._day = value;
                }
            }
            /// <summary>
            /// The month this event occurs on
            /// </summary>
            public int Month
            {
                get
                {
                    if (IsDefault)
                    {
                        return _defaultEvent.Month;
                    }
                    else
                    {
                        return this._month;
                    }
                }
                set
                {
                    this.IsDefault = false;
                    this._month = value;
                }
            }
            /// <summary>
            /// The year this event occurs on
            /// </summary>
            public int Year
            {
                get
                {
                    if (IsDefault)
                    {
                        return _defaultEvent.Year;
                    }
                    else
                    {
                        return this._year;
                    }
                }
                set
                {
                    this.IsDefault = false;
                    this._year = value;
                }
            }
            /// <summary>
            /// Whether or not this event recurs
            /// </summary>
            public bool IsRecurring
            {
                get
                {
                    if (IsDefault)
                    {
                        return _defaultEvent.IsRecurring;
                    }
                    else
                    {
                        return this._isRecurring;
                    }
                }
                set
                {
                    this.IsDefault = false;
                    this._isRecurring = value;
                }
            }
            /// <summary>
            /// Future event occurences
            /// </summary>
            public List<EventFuture> Futures
            {
                get
                {
                    if (IsDefault)
                    {
                        return _defaultEvent.Futures;
                    }
                    else
                    {
                        return this._futures;
                    }
                }
                set
                {
                    this.IsDefault = false;
                    this._futures = value;
                }
            }
            /// <summary>
            /// Whether or not this event is a default event
            /// </summary>
            public bool IsDefault
            {
                get;
                private set;
            }
            private DefaultEvent _defaultEvent;
            /// <summary>
            /// Create a new Event from a default event
            /// </summary>
            /// <param name="defaultEvent">The default event to emulate</param>
            public EventUser(DefaultEvent defaultEvent)
            {
                this._defaultEvent = defaultEvent;
                this.IsDefault = true;
            }
            /// <summary>
            /// Fetch an event from the database
            /// </summary>
            /// <param name="EventUserID">The ID to fetch</param>
            public EventUser(ulong EventUserID)
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
                                this.User = new User(Convert.ToUInt64(reader["UserID"]));
                                if (!Convert.IsDBNull(reader["EventID"]))
                                {
                                    this._defaultEvent = new DefaultEvent(Convert.ToUInt64(reader["EventID"]));
                                    this.IsDefault = true;
                                    this.EventUserId = EventUserID;
                                    return;
                                }
                                else
                                {
                                    this.IsDefault = false;
                                    this.EventUserId = EventUserID;
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
                    if (!IsRecurring)
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
            /// Instantiate a new, empty Event
            /// </summary>
            public EventUser()
            {

            }
            /// <summary>
            /// Create this event in the database
            /// </summary>
            /// <returns>A Status flag</returns>
            public bool Create()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO events_users (UserID, EventID, EventName, EventDescription, EventDay, EventMonth, EventYear, EventRecurs) "
                                        + " VALUES (@uid, @eid, @eName, @eDesc, @eDay, @eMonth, @eYear, @eRecurs);";
                        cmd.Parameters.AddWithValue("@uid", this.User.UserId);
                        if (_defaultEvent != null)
                        {
                            cmd.Parameters.AddWithValue("@eid", this._defaultEvent.DefaultEventId);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@eid", null);
                        }
                        cmd.Parameters.AddWithValue("@eName", this.Name);
                        cmd.Parameters.AddWithValue("@eDesc", this.Description);
                        cmd.Parameters.AddWithValue("@eDay", this.Day);
                        cmd.Parameters.AddWithValue("@eMonth", this.Month);
                        cmd.Parameters.AddWithValue("@eYear", this.Year);
                        cmd.Parameters.AddWithValue("@eRecurs", this.IsRecurring);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        EventUserId = Convert.ToUInt64(cmd.LastInsertedId);
                    }
                }
                return false;
            }
            /// <summary>
            /// Update this event in the database
            /// </summary>
            /// <returns>A status flag</returns>
            public bool Update()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    if (!IsDefault)
                    {
                        // If we're not default, then just update existing event.
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM events_users_futures WHERE EventUserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.EventUserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "UPDATE events_users "
                                            + "SET UserID = @uid, "
                                            + "EventID = NULL, "
                                            + "EventDay = @day, "
                                            + "EventMonth = @month, "
                                            + "EventYear = @year, "
                                            + "EventRecurs = TRUE, "
                                            + "EventName = @name, "
                                            + "EventDescription = @descrip "
                                            + "WHERE EventUserID = @id;";
                            cmd.Parameters.AddWithValue("@uid", this.User.UserId);
                            cmd.Parameters.AddWithValue("@day", this.Day);
                            cmd.Parameters.AddWithValue("@month", this.Month);
                            cmd.Parameters.AddWithValue("@year", this.Year);
                            cmd.Parameters.AddWithValue("@name", this.Name);
                            cmd.Parameters.AddWithValue("@description", this.Description);
                            cmd.Parameters.AddWithValue("@id", this.EventUserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        if (!IsRecurring)
                        {
                            // Recreate EventFutures
                            // For each event future, insert it:
                            foreach (EventFuture future in this.Futures)
                            {
                                using (MySqlCommand cmd = new MySqlCommand())
                                {
                                    cmd.Connection = con;
                                    cmd.CommandText = "INSERT INTO events_users_futures (EventUserID, EventYear, EventMonth, EventDay) "
                                                    + "VALUES (@euid, @year, @month, @day);";
                                    cmd.Parameters.AddWithValue("@euid", this.EventUserId);
                                    cmd.Parameters.AddWithValue("@year", future.Year);
                                    cmd.Parameters.AddWithValue("@month", future.Month);
                                    cmd.Parameters.AddWithValue("@day", future.Day);
                                    cmd.Prepare();
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    else
                    {
                        // We'll need to copy event - We wouldn't be updating if we were using a default event
                        // Copy over all data, then call ourself AFTER isDefault is false!
                        this.Name = _defaultEvent.Name;
                        this.Description = _defaultEvent.Description;
                        this.Day = _defaultEvent.Day;
                        this.Month = _defaultEvent.Month;
                        this.Year = _defaultEvent.Year;
                        this.IsRecurring = _defaultEvent.IsRecurring;
                        this.Futures = _defaultEvent.Futures;
                        this.IsDefault = false;
                        _defaultEvent = null;
                        return Update();
                    }
                }
                // If isdefault, remove this and make it it's own event?
                return !IsDefault;
            }
            /// <summary>
            /// Delete the gift from the database
            /// </summary>
            /// <returns>A status flag</returns>
            public bool Delete()
            {
                // if ID == -1, don't do anything
                if (EventUserId == 0)
                {
                    return false;
                }
                // Delete from EventUsersGroups:
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    // Delete from EventsUsersGroups:
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM events_users_groups WHERE EventUserID = @id;";
                        cmd.Parameters.AddWithValue("@id", this.EventUserId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                    // Delete from futures:
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM events_users_futures WHERE EventUserID = @id;";
                        cmd.Parameters.AddWithValue("@id", this.EventUserId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM events_users WHERE EventUserID = @id;";
                        cmd.Parameters.AddWithValue("@id", this.EventUserId);
                        cmd.Prepare();
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            return false;
                        }
                        else
                        {
                            this.EventUserId = 0;
                            return true;
                        }
                    }
                }
            }

            /// <summary>
            /// Serialize this event as an XML Document
            /// </summary>
            /// <remarks>
            /// As with all fetch methods, this is serialized as an XML Document.
            /// 
            /// It has the following fields:
            ///     - eventId: The Event ID
            ///     - name: The name of this event
            ///     - description: The description of this event
            ///     - day: The day this event occurs on
            ///     - month: The month this event occurs on
            ///     - year: The year this event occurs on
            ///     - isRecurring: Whether or not this event recurs
            ///     - eventFutures: A collection of futures
            ///         - Note that each element of _eventFutures_ is an _eventFuture_ element
            ///         
            /// This is wrapped in an event container
            /// </remarks>
            /// <returns>Serialized XML Representation</returns>
            public XmlDocument Fetch()
            {
                XmlDocument info = new XmlDocument();
                XmlElement container = info.CreateElement("event");
                info.AppendChild(container);
                XmlElement id = info.CreateElement("eventId");
                id.InnerText = EventUserId.ToString();
                XmlElement name = info.CreateElement("name");
                name.InnerText = Name.ToString();
                XmlElement description = info.CreateElement("description");
                description.InnerText = Description;
                XmlElement day = info.CreateElement("day");
                day.InnerText = Day.ToString();
                XmlElement month = info.CreateElement("month");
                month.InnerText = Month.ToString();
                XmlElement year = info.CreateElement("year");
                year.InnerText = Year.ToString();
                XmlElement isRecurring = info.CreateElement("isRecurring");
                isRecurring.InnerText = IsRecurring.ToString().ToLower();

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
