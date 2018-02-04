using GiftServer.Exceptions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// An event that can recur
        /// </summary>
        public class Event : ISynchronizable, IFetchable, IEquatable<Event>, IComparable<Event>
        {
            /// <summary>
            /// The EventID for this Event
            /// </summary>
            public ulong ID
            {
                get;
                private set;
            } = 0;
            /// <summary>
            /// The Name for this event
            /// </summary>
            public string Name = "";
            /// <summary>
            /// The Starting Date of this event
            /// </summary>
            public DateTime StartDate;
            /// <summary>
            /// The Ending date of this event.
            /// </summary>
            /// <remarks>
            /// If this is null, then there is no end date
            /// </remarks>
            public DateTime? EndDate;
            /// <summary>
            /// The owner of this event
            /// </summary>
            public User User;
            /// <summary>
            /// The Rules Engine
            /// </summary>
            /// <remarks>
            /// The Rules Engine is a set of rules that describe when the next occurrence of an event will be.
            /// </remarks>
            public RulesEngine Rules;
            /// <summary>
            /// A list of Blackout Dates.
            /// </summary>
            public List<Blackout> Blackouts
            {
                get
                {
                    List<Blackout> blackouts = new List<Blackout>();
                    using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                    {
                        con.Open();
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT EventBlackoutID FROM event_blackouts WHERE EventID = @eid;";
                            cmd.Parameters.AddWithValue("@eid", ID);
                            cmd.Prepare();
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    blackouts.Add(new Blackout(Convert.ToUInt64(reader["EventBlackoutID"])));
                                }
                            }
                        }
                    }
                    return blackouts;
                }
            }
            /// <summary>
            /// A list of groups this event is visible to
            /// </summary>
            public List<Group> Groups
            {
                get
                {
                    List<Group> groups = new List<Group>();
                    using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                    {
                        con.Open();
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT GroupID FROM groups_events WHERE EventID = @eid;";
                            cmd.Parameters.AddWithValue("@eid", ID);
                            cmd.Prepare();
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    groups.Add(new Group(Convert.ToUInt64(reader["GroupID"])));
                                }
                                return groups;
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Fetch an existing Event from the database
            /// </summary>
            /// <param name="id">The EventID</param>
            public Event(ulong id)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT EventID, EventName, EventStartDate, EventEndDate, UserID FROM user_events WHERE EventID = @eid;";
                        cmd.Parameters.AddWithValue("@eid", id);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ID = id;
                                Name = Convert.ToString(reader["EventName"]);
                                StartDate = (DateTime)(reader["EventStartDate"]);
                                EndDate = (DateTime?)(reader["EventEndDate"] == DBNull.Value ? null : reader["EventEndDate"]);
                                User = new User(Convert.ToUInt64(reader["UserID"]));
                            }
                            else
                            {
                                // Throw new exception
                                throw new EventNotFoundException(id);
                            }
                        }
                    }
                    // Get rule engine
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT ExactEventID FROM exact_events WHERE EventID = @eid;";
                        cmd.Parameters.AddWithValue("@eid", id);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Rules = new ExactEvent(Convert.ToUInt64(reader["ExactEventID"]))
                                {
                                    Event = this
                                };
                            }
                        }
                    }
                    if (Rules == null)
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT RelativeEventID FROM relative_events WHERE EventID = @eid;";
                            cmd.Parameters.AddWithValue("@eid", id);
                            cmd.Prepare();
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    Rules = new RelativeEvent(Convert.ToUInt64(reader["RelativeEventID"]))
                                    {
                                        Event = this
                                    };
                                }
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Instantiate a newly created event
            /// </summary>
            /// <param name="name">The name of this event. Must not be empty</param>
            /// <param name="startDate">The start date for this event</param>
            /// <param name="owner">The creator of this event</param>
            /// <param name="engine">The engine this event runs on - can be null</param>
            public Event(string name, DateTime startDate, User owner, RulesEngine engine)
            {
                if (String.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("Name is blank");
                }
                Name = name;
                StartDate = startDate;
                User = owner;
                Rules = engine;
                if (Rules != null)
                {
                    Rules.Event = this;
                }
            }
            /// <summary>
            /// Get the closest occurrence that happens in the future
            /// </summary>
            /// <param name="near">The date to find an event near</param>
            /// <returns>The occurrence for this event - or null, if no occurrence after near is found</returns>
            public Occurrence GetNearestOccurrence(DateTime near)
            {
                if (Rules == null)
                {
                    return StartDate >= near ? new Occurrence(this, StartDate) : null;
                }
                else
                {
                    foreach (Occurrence occur in Rules.Occurrences)
                    {
                        if (occur.Date >= near)
                        {
                            return occur;
                        }
                    }
                    return null;
                }
            }
            /// <summary>
            /// Get the closest occurrence that happens in the future from today
            /// </summary>
            /// <returns>The same as GetNearestOccurrence(DateTime near), where near is Today</returns>
            public Occurrence GetNearestOccurrence()
            {
                return GetNearestOccurrence(DateTime.Today);
            }

            /// <summary>
            /// Give the events in order
            /// </summary>
            /// <remarks>
            /// Given a pool of events, this will continue to return the next event.
            /// 
            /// Note that this means, if event A occurs monthly, and event B occurs yearly, event A could show up many times before event B
            /// </remarks>
            /// <param name="events">The pool of events</param>
            /// <param name="start">The starting date</param>
            /// <param name="stop">The ending date</param>
            /// <returns></returns>
            public static IEnumerable<Occurrence> PoolOrder(List<Event> events, DateTime start, DateTime stop)
            {
                // Expand into a list of all occurrences
                List<Occurrence> occurrences = new List<Occurrence>();
                foreach (Event e in events)
                {
                    occurrences.AddRange(e.GetOccurrences(start, stop));
                }
                occurrences = occurrences.OrderBy(o => o.Date).ToList();
                foreach (Occurrence o in occurrences)
                {
                    yield return o;
                }
            }
            /// <summary>
            /// Create a record of this event in the database
            /// </summary>
            /// <remarks>
            /// This also creates the necessary rules engine.
            /// </remarks>
            public void Create()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO user_events (UserID, EventName, EventStartDate, EventEndDate)  "
                                        + "VALUES (@uid, @enm, @esd, @eed);";
                        cmd.Parameters.AddWithValue("@uid", User.ID);
                        cmd.Parameters.AddWithValue("@enm", Name);
                        cmd.Parameters.AddWithValue("@esd", StartDate);
                        cmd.Parameters.AddWithValue("@eed", EndDate.HasValue ? EndDate.Value.ToString("yyyy-MM-dd") : null);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        ID = Convert.ToUInt64(cmd.LastInsertedId);
                        Rules.Create();
                    }
                }
            }

            /// <summary>
            /// Update an existing event
            /// </summary>
            /// <remarks>
            /// This will also update any rule changes
            /// </remarks>
            public void Update()
            {
                // First update the event, then update the rules engine
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "UPDATE user_events SET EventName = @nam, EventStartDate = @std, EventEndDate = @edt "
                                        + "WHERE EventID = @eid;";
                        cmd.Parameters.AddWithValue("@nam", Name);
                        cmd.Parameters.AddWithValue("@std", StartDate);
                        cmd.Parameters.AddWithValue("@edt", EndDate.HasValue ? EndDate.Value.ToString("yyyy-MM-dd") : null);
                        cmd.Parameters.AddWithValue("@eid", ID);
                        cmd.ExecuteNonQuery();
                        if (Rules != null)
                        {
                            Rules.Update();
                        }
                    }
                }
            }
            /// <summary>
            /// Delete an event
            /// </summary>
            /// <remarks>
            /// This will also delete any rule sets
            /// </remarks>
            public void Delete()
            {
                // Delete rules, remove from groups, delete from db.
                if (Rules != null)
                {
                    Rules.Delete();
                    Rules = null;
                }
                foreach (Group group in Groups)
                {
                    group.Remove(this);
                }
                foreach (Blackout blackout in Blackouts)
                {
                    blackout.Delete();
                }
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM user_events WHERE EventID = @eid;";
                        cmd.Parameters.AddWithValue("@eid", ID);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            /// <summary>
            /// Get Occurrences starting from today, with a limiit
            /// </summary>
            /// <param name="limit">The maximum number of occurrences to fetch</param>
            /// <returns>A List of Occurrences</returns>
            public List<Occurrence> GetOccurrences(ulong limit)
            {
                return GetOccurrences(DateTime.Today, limit);
            }
            /// <summary>
            /// Get Occurrences starting from a date, with a limiit
            /// </summary>
            /// <param name="start">The starting date</param>
            /// <param name="limit">The maximum number of occurrences to fetch</param>
            /// <returns>A List of Occurrences</returns>
            public List<Occurrence> GetOccurrences(DateTime start, ulong limit)
            {
                List<Occurrence> occur = new List<Occurrence>();
                if (Rules == null)
                {
                    if (StartDate >= start)
                    {
                        occur.Add(new Occurrence(this, StartDate));
                    }
                    return occur;
                }
                uint count = 0;
                foreach (Occurrence o in Rules.Occurrences)
                {
                    // Wait until date is greater than or equal to our start:
                    if (o.Date >= start && count < limit)
                    {
                        occur.Add(o);
                        count++;
                    }
                }
                return occur;
            }
            /// <summary>
            /// Get Occurrences starting a date and ending on a date
            /// </summary>
            /// <param name="start">The start date, inclusive</param>
            /// <param name="stop">The end date, inclusive</param>
            /// <returns>A List of Occurrences</returns>
            public List<Occurrence> GetOccurrences(DateTime start, DateTime stop)
            {
                List<Occurrence> occur = new List<Occurrence>();
                if (Rules == null)
                {
                    if (StartDate >= start && StartDate <= stop)
                    {
                        occur.Add(new Occurrence(this, StartDate));
                    }
                    return occur;
                }
                foreach (Occurrence o in Rules.Occurrences)
                {
                    // Wait until date is greater than or equal to our start:
                    if (o.Date >= start && o.Date <= stop)
                    {
                        occur.Add(o);
                    }
                    if (o.Date > stop)
                    {
                        break;
                    }
                }
                return occur;
            }
            /// <summary>
            /// Find out if this object is equal to this event
            /// </summary>
            /// <param name="obj">The object to compare</param>
            /// <returns>A boolean of whether they are equal or not</returns>
            public override bool Equals(object obj)
            {
                if (obj != null && obj is Event e)
                {
                    return Equals(e);
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// Check if this Event is equal to another event
            /// </summary>
            /// <param name="evnt">The event to compare</param>
            /// <returns>False if null or not the same event</returns>
            public bool Equals(Event evnt)
            {
                return evnt != null && evnt.ID == ID;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return ID.GetHashCode();
            }
            /// <summary>
            /// Compares two events
            /// </summary>
            /// <param name="e">The event to compare</param>
            /// <returns>-1 if this event is before, 0 if this event is on the same day, and 1 if this event is after</returns>
            public int CompareTo(Event e)
            {
                return GetNearestOccurrence().CompareTo(e.GetNearestOccurrence());
            }
            /// <summary>
            /// Serialize this Event as an XML document.
            /// </summary>
            /// <returns></returns>
            public XmlDocument Fetch()
            {
                return new XmlDocument();
            }
        }
    }
}