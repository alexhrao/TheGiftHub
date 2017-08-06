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
            public long EventUserID = -1;
            public User user;
            private string _name;
            private string _description;
            private int _day;
            private int _month;
            private int _year;
            private bool _isRecurring;
            private List<EventFuture> _futures = new List<EventFuture>();
            public string Name
            {
                get
                {
                    if (_isDefault)
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
                    this._isDefault = false;
                    this._name = value;
                }
            }
            public string Description
            {
                get
                {
                    if (_isDefault)
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
                    this._isDefault = false;
                    this._description = value;
                }
            }
            public int Day
            {
                get
                {
                    if (_isDefault)
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
                    this._isDefault = false;
                    this._day = value;
                }
            }
            public int Month
            {
                get
                {
                    if (_isDefault)
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
                    this._isDefault = false;
                    this._month = value;
                }
            }
            public int Year
            {
                get
                {
                    if (_isDefault)
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
                    this._isDefault = false;
                    this._year = value;
                }
            }
            public bool IsRecurring
            {
                get
                {
                    if (_isDefault)
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
                    this._isDefault = false;
                    this._isRecurring = value;
                }
            }
            public List<EventFuture> Futures
            {
                get
                {
                    if (_isDefault)
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
                    this._isDefault = false;
                    this._futures = value;
                }
            }
            private bool _isDefault;
            private DefaultEvent _defaultEvent;

            public EventUser(DefaultEvent defaultEvent)
            {
                this._defaultEvent = defaultEvent;
                this.Name = _defaultEvent.Name;
                this.Description = _defaultEvent.Description;
                this.Day = _defaultEvent.Day;
                this.Month = _defaultEvent.Month;
                this.Year = _defaultEvent.Year;
                this.IsRecurring = _defaultEvent.IsRecurring;
                this.Futures = _defaultEvent.Futures;
                this._isDefault = true;
            }
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
                                    this._defaultEvent = new DefaultEvent(Convert.ToInt64(reader["EventID"]));
                                    this._isDefault = true;
                                    this.EventUserID = EventUserID;
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
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO events_users (UserID, EventID, EventName, EventDescription, EventDay, EventMonth, EventYear, EventRecurs) "
                                        + " VALUES (@uid, @eid, @eName, @eDesc, @eDay, @eMonth, @eYear, @eRecurs);";
                        cmd.Parameters.AddWithValue("@uid", this.user.Id);
                        if (_defaultEvent != null)
                        {
                            cmd.Parameters.AddWithValue("@eid", this._defaultEvent.EventID);
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
                    }
                }
                return false;
            }
            public bool Update()
            {
                // If isdefault, remove this and make it it's own event?
                return !_isDefault;
            }
            public bool Delete()
            {
                return false;
            }
        }
    }
}
