using GiftServer.Exceptions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// A Relative event rule
        /// </summary>
        /// <remarks>
        /// Relative events are events whose time interval is relative to something else - there is no "exact" time span. 
        /// So, if a user wanted an event to occur every year on the third Thursday of January, this isn't possible to do with Exact Events. 
        /// Instead, we have Relative Events.
        /// 
        /// As an example, a Relative Event could be defined as starting on January 7th, 2017, and occurring every year on the second Monday of January, 
        /// and should stop by January 1st, 2020. In this case, the next event occurrences would be:
        /// - January 9th, 2017
        /// - January 8th, 2018
        /// - January 14th, 2019
        /// 
        /// Note that the start date doesn't necessarily indicate the first day the event occurs - it just specifies a lower bound. Furthermore, 
        /// the end date doesn't necessarily indicate the last occurrence - only an upper bound.
        /// </remarks>
        public class RelativeEvent : RulesEngine, IEquatable<RelativeEvent>
        {
            /// <summary>
            /// The ID for this Relative Event Rule
            /// </summary>
            public ulong RelativeEventId
            {
                get;
                private set;
            } = 0;

            private string timeInterval = "";
            /// <summary>
            /// The month this occurs in - blank if monthly
            /// </summary>
            /// <remarks>
            /// This method will always return the three letter month code in all capitals (i.e., JAN, FEB, etc.).
            /// 
            /// You can either give it a 3 letter code or the full name in any case.
            /// </remarks>
            public string TimeInterval
            {
                get
                {
                    return timeInterval;
                }
                set
                {
                    // Switch the month values
                    switch (value)
                    {
                        case "jan":
                        case "january":
                            timeInterval = "JAN";
                            break;
                        case "feb":
                        case "february":
                            timeInterval = "FEB";
                            break;
                        case "mar":
                        case "march":
                            timeInterval = "MAR";
                            break;
                        case "apr":
                        case "april":
                            timeInterval = "APR";
                            break;
                        case "may":
                            timeInterval = "MAY";
                            break;
                        case "jun":
                        case "june":
                            timeInterval = "JUN";
                            break;
                        case "jul":
                        case "july":
                            timeInterval = "JUL";
                            break;
                        case "aug":
                        case "august":
                            timeInterval = "AUG";
                            break;
                        case "sep":
                        case "september":
                            timeInterval = "SEP";
                            break;
                        case "oct":
                        case "october":
                            timeInterval = "OCT";
                            break;
                        case "nov":
                        case "november":
                            timeInterval = "NOV";
                            break;
                        case "dec":
                        case "december":
                            timeInterval = "DEC";
                            break;
                        default:
                            timeInterval = "";
                            break;
                    }
                }
            }
            private int skipEvery = 0;
            /// <summary>
            /// Skip every x occurrences. Cannot be 0.
            /// </summary>
            public int SkipEvery
            {
                get
                {
                    return skipEvery;
                }
                set
                {
                    if (value > 0)
                    {
                        skipEvery = value;
                    }
                }
            }

            private char dayOfWeek = '\0';
            /// <summary>
            /// The day of the week this occurs on
            /// </summary>
            /// <remarks>
            /// This method will accept any of the following values:
            /// 
            /// - n, sun, sunday
            /// - m, mon, monday
            /// - t, tue, tuesday
            /// - w, wed, wednesday
            /// - r, thu, thursday
            /// - f, fri, friday
            /// - s, sat, saturday
            /// 
            /// In any case. It will only return a single character, however.
            /// </remarks>
            public string DayOfWeek
            {
                get
                {
                    return dayOfWeek.ToString();
                }
                set
                {
                    switch (value.ToLower())
                    {
                        case "n":
                        case "sun":
                        case "sunday":
                            dayOfWeek = 'N';
                            break;
                        case "m":
                        case "mon":
                        case "monday":
                            dayOfWeek = 'M';
                            break;
                        case "t":
                        case "tue":
                        case "tuesday":
                            dayOfWeek = 'T';
                            break;
                        case "w":
                        case "wed":
                        case "wednesday":
                            dayOfWeek = 'W';
                            break;
                        case "r":
                        case "thu":
                        case "thursday":
                            dayOfWeek = 'R';
                            break;
                        case "f":
                        case "fri":
                        case "friday":
                            dayOfWeek = 'F';
                            break;
                        case "s":
                        case "sat":
                        case "saturday":
                            dayOfWeek = 'S';
                            break;
                        default:
                            throw new ArgumentException(value);
                    }
                }
            }

            private int posn = 0;
            /// <summary>
            /// The position within the month this event occurs.
            /// </summary>
            /// <remarks>
            /// Values 1-4 mean the first, second, third, and fourth instances of that day within the month, respectively.
            /// 
            /// A value of 5 means that it will *always* happen on the last instance of that day within the month.
            /// </remarks>
            public int Posn
            {
                get
                {
                    return posn;
                }
                set
                {
                    if (value > 0 && value < 6)
                    {
                        posn = value;
                    }
                    else
                    {
                        throw new ArgumentException(value.ToString());
                    }
                }
            }
            /// <summary>
            /// All occurrences of this event
            /// </summary>
            /// <remarks>
            /// This will return, in chronological order, event occurrences.
            /// 
            /// Please note - because of this, if there is no end date, _this iterator could iterate infinitely_. It is up to the caller to handle this.
            /// </remarks>
            public override IEnumerable<Occurrence> Occurrences
            {
                get
                {
                    DateTime currVal = Event.StartDate;
                    if (Event.EndDate.HasValue)
                    {
                        // We will have an end.
                        // TODO: Check if StartDate is occurrence
                        while (currVal <= Event.EndDate)
                        {
                            yield return new Occurrence(Event, currVal);
                            currVal = Increment(currVal);
                        }
                    }
                    else
                    {
                        // No end
                        while (true)
                        {
                            yield return new Occurrence(Event, currVal);
                            currVal = Increment(currVal);
                        }
                    }
                }
            }
            /// <summary>
            /// Fetch an existing RelativeEvent from the database
            /// </summary>
            /// <param name="id">The ID for this relative event</param>
            public RelativeEvent(ulong id)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT RelativeEventID, EventTimeInterval, EventSkipEvery, EventDayOfWeek, "
                                        + "EventPosn FROM relative_events WHERE RelativeEventID = @rid;";
                        cmd.Parameters.AddWithValue("@rid", id);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                RelativeEventId = id;
                                TimeInterval = Convert.ToString(reader["EventTimeInterval"]);
                                SkipEvery = Convert.ToInt32(reader["EventSkipEvery"]);
                                DayOfWeek = Convert.ToString(reader["EventDayOfWeek"]);
                                Posn = Convert.ToInt32(reader["EventPosn"]);
                            }
                            else
                            {
                                throw new EventNotFoundException(id);
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// Create a new RelativeEvent ruleset
            /// </summary>
            /// <param name="interval">The interval to use - A Month for a specific month or empty for occurring monthly</param>
            /// <param name="skip">The skip to use</param>
            /// <param name="day">The day of the week to use</param>
            /// <param name="posn">The position within the month to use</param>
            public RelativeEvent(string interval, int skip, string day, int posn)
            {
                TimeInterval = interval;
                SkipEvery = skip;
                DayOfWeek = day;
                Posn = posn;
            }

            private DateTime Increment(DateTime currVal)
            {
                DateTime incremented = currVal;
                // If a month, move a year into the first day of that month:
                switch (timeInterval)
                {
                    case "JAN":
                        incremented = new DateTime(currVal.Year + 1, 1, 1);
                        break;
                    case "FEB":
                        incremented = new DateTime(currVal.Year + 1, 2, 1);
                        break;
                    case "MAR":
                        incremented = new DateTime(currVal.Year + 1, 3, 1);
                        break;
                    case "APR":
                        incremented = new DateTime(currVal.Year + 1, 4, 1);
                        break;
                    case "MAY":
                        incremented = new DateTime(currVal.Year + 1, 5, 1);
                        break;
                    case "JUN":
                        incremented = new DateTime(currVal.Year + 1, 6, 1);
                        break;
                    case "JUL":
                        incremented = new DateTime(currVal.Year + 1, 7, 1);
                        break;
                    case "AUG":
                        incremented = new DateTime(currVal.Year + 1, 8, 1);
                        break;
                    case "SEP":
                        incremented = new DateTime(currVal.Year + 1, 9, 1);
                        break;
                    case "OCT":
                        incremented = new DateTime(currVal.Year + 1, 10, 1);
                        break;
                    case "NOV":
                        incremented = new DateTime(currVal.Year + 1, 11, 1);
                        break;
                    case "DEC":
                        incremented = new DateTime(currVal.Year + 1, 12, 1);
                        break;
                    default:
                        incremented = incremented.AddMonths(1);
                        incremented = new DateTime(incremented.Year, incremented.Month, 1);
                        break;
                        // Occurs monthly
                }
                incremented = new DateTime(incremented.Year, incremented.Month, 1);
                // loop until we reach the posn of that day. Unless it's 5
                if (posn == 5)
                {
                    // Loop from end of month until our day is it!
                    incremented = incremented.AddMonths(1);
                    bool found = false;
                    while (!found)
                    {
                        incremented = incremented.AddDays(-1);
                        // go back until our day  is correct!
                        switch (incremented.DayOfWeek)
                        {
                            case System.DayOfWeek.Sunday:
                                if (dayOfWeek == 'N')
                                {
                                    found = true;
                                }
                                break;
                            case System.DayOfWeek.Monday:
                                if (dayOfWeek == 'M')
                                {
                                    found = true;
                                }
                                break;
                            case System.DayOfWeek.Tuesday:
                                if (dayOfWeek == 'T')
                                {
                                    found = true;
                                }
                                break;
                            case System.DayOfWeek.Wednesday:
                                if (dayOfWeek == 'W')
                                {
                                    found = true;
                                }
                                break;
                            case System.DayOfWeek.Thursday:
                                if (dayOfWeek == 'R')
                                {
                                    found = true;
                                }
                                break;
                            case System.DayOfWeek.Friday:
                                if (dayOfWeek == 'F')
                                {
                                    found = true;
                                }
                                break;
                            case System.DayOfWeek.Saturday:
                                if (dayOfWeek == 'S')
                                {
                                    found = true;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    int numPassed = 0;
                    while (numPassed < posn)
                    {
                        incremented = incremented.AddDays(1);
                        switch (incremented.DayOfWeek)
                        {
                            case System.DayOfWeek.Sunday:
                                if (dayOfWeek == 'N')
                                {
                                    numPassed++;
                                }
                                break;
                            case System.DayOfWeek.Monday:
                                if (dayOfWeek == 'M')
                                {
                                    numPassed++;
                                }
                                break;
                            case System.DayOfWeek.Tuesday:
                                if (dayOfWeek == 'T')
                                {
                                    numPassed++;
                                }
                                break;
                            case System.DayOfWeek.Wednesday:
                                if (dayOfWeek == 'W')
                                {
                                    numPassed++;
                                }
                                break;
                            case System.DayOfWeek.Thursday:
                                if (dayOfWeek == 'R')
                                {
                                    numPassed++;
                                }
                                break;
                            case System.DayOfWeek.Friday:
                                if (dayOfWeek == 'F')
                                {
                                    numPassed++;
                                }
                                break;
                            case System.DayOfWeek.Saturday:
                                if (dayOfWeek == 'S')
                                {
                                    numPassed++;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                return IsBlackout(incremented) ? Increment(incremented) : incremented;
            }

            private bool IsBlackout(DateTime poss)
            {
                return Event.Blackouts.Exists(x =>
                                x.BlackoutDate.Year == poss.Year &&
                                x.BlackoutDate.Month == poss.Month &&
                                x.BlackoutDate.Day == poss.Day);

            }
            /// <summary>
            /// Create a record of this ruleset in the database
            /// </summary>
            /// <returns>A status flag</returns>
            /// <remarks>
            /// Note that, since Event already creates this, it is unlikely the end user will need this method
            /// </remarks>
            public override bool Create()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO relative_events (EventID, EventTimeInterval, EventSkipEvery, EventDayOfWeek, EventPosn) "
                                        + "VALUES (@eid, @tin, @ski, @dow, @pos);";
                        cmd.Parameters.AddWithValue("@eid", Event.EventId);
                        cmd.Parameters.AddWithValue("@tin", timeInterval);
                        cmd.Parameters.AddWithValue("@ski", skipEvery);
                        cmd.Parameters.AddWithValue("@dow", dayOfWeek);
                        cmd.Parameters.AddWithValue("@pos", posn);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        RelativeEventId = Convert.ToUInt64(cmd.LastInsertedId);
                        return true;
                    }
                }
            }
            /// <summary>
            /// Update the record of this ruleset in the database
            /// </summary>
            /// <returns>A status flag</returns>
            /// <remarks>
            /// Note that, since Event already updates this, it is unlikely the end user will need this method
            /// </remarks>
            public override bool Update()
            {
                if (RelativeEventId == 0)
                {
                    return Create();
                }
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "UPDATE exact_events SET "
                                        + "EventTimeInterval = @tin, "
                                        + "EventSkipEvery = @ski, "
                                        + "EventDayOfWeek = @dow, "
                                        + "EventPosn = @pos "
                                        + "WHERE ExactEventID = @rid;";
                        cmd.Parameters.AddWithValue("@tin", timeInterval);
                        cmd.Parameters.AddWithValue("@ski", skipEvery);
                        cmd.Parameters.AddWithValue("@dow", dayOfWeek);
                        cmd.Parameters.AddWithValue("@pos", posn);
                        cmd.Parameters.AddWithValue("@rid", RelativeEventId);
                        cmd.Prepare();
                        return cmd.ExecuteNonQuery() == 1;
                    }
                }
            }
            /// <summary>
            /// Deletethe record of this ruleset in the database
            /// </summary>
            /// <returns>A status flag</returns>
            /// <remarks>
            /// Note that, since Event already deletes this, it is unlikely the end user will need this method
            /// </remarks>
            public override bool Delete()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM relative_events WHERE ExactEventID = @eid;";
                        cmd.Parameters.AddWithValue("@eid", RelativeEventId);
                        cmd.Prepare();
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            RelativeEventId = 0;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            /// <summary>
            /// See if the given object is the same as this one
            /// </summary>
            /// <param name="obj">The object to compare</param>
            /// <returns>Whether or not they are equal</returns>
            public override bool Equals(object obj)
            {
                if (obj != null && obj is RelativeEvent r)
                {
                    return Equals(r);
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// See if the given rules engine is the same as this one
            /// </summary>
            /// <param name="engine">The engine to compare</param>
            /// <returns>Whether or not they are equal</returns>
            public override bool Equals(RulesEngine engine)
            {
                if (engine != null && engine is RelativeEvent r)
                {
                    return Equals(r);
                }
                else
                {
                    return false;
                }

            }
            /// <summary>
            /// Check if the given RelativeEvent Engine is equal to this one
            /// </summary>
            /// <param name="engine">The engine to compare</param>
            /// <returns>If the two are the same engine</returns>
            public bool Equals(RelativeEvent engine)
            {
                return engine != null && engine.RelativeEventId == RelativeEventId;
            }
            /// <summary>
            /// A hash for this engine
            /// </summary>
            /// <returns>The hash for this engine</returns>
            public override int GetHashCode()
            {
                return RelativeEventId.GetHashCode();
            }
            /// <summary>
            /// Serialize this ruleset
            /// </summary>
            /// <returns>A Serialized form of this ruleset</returns>
            /// <remarks>
            /// This XML Document has the following fields:
            /// - relativeEventId: The ID for this rule set
            /// - timeInterval: A single character:
            ///     - 'M' -> Monthly
            ///     - 'Y' -> Yearly
            /// - skipEvery: A number that represents how many iterations to skip (i.e., every = 1, every other = 2, ...)
            /// - dayOfWeek: A character that represents the day of the week this event occurs on:
            ///     - 'N' -> Sunday
            ///     - 'M' -> Monday
            ///     - 'T' -> Tuesday
            ///     - 'W' -> Wednesday
            ///     - 'R' -> Thursday
            ///     - 'F' -> Friday
            ///     - 'S' -> Saturday
            /// - posn: A number that represents the position within the month, where 5 is the last x of the month
            /// 
            /// This is all wrapped in a relativeEvent container
            /// </remarks>
            public override XmlDocument Fetch()
            {
                XmlDocument info = new XmlDocument();
                XmlElement container = info.CreateElement("relativeEvent");
                info.AppendChild(container);

                XmlElement id = info.CreateElement("relativeEventId");
                id.InnerText = RelativeEventId.ToString();
                XmlElement _timeInterval = info.CreateElement("timeInterval");
                _timeInterval.InnerText = TimeInterval;
                XmlElement _skipEvery = info.CreateElement("skipEvery");
                _skipEvery.InnerText = skipEvery.ToString();
                XmlElement _dayOfWeek = info.CreateElement("dayOfWeek");
                _dayOfWeek.InnerText = DayOfWeek;
                XmlElement _posn = info.CreateElement("posn");
                _posn.InnerText = posn.ToString();

                container.AppendChild(id);
                container.AppendChild(_timeInterval);
                container.AppendChild(_skipEvery);
                container.AppendChild(_dayOfWeek);
                container.AppendChild(_posn);

                return info;
            }
        }
    }
}