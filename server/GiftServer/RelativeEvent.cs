using System;
using System.Collections.Generic;
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

            private char timeInterval = '\0';
            /// <summary>
            /// The Time Interval for this event
            /// </summary>
            /// <remarks>
            /// This method will accept the following values:
            /// 
            /// - d, day, or daily
            /// - w, week, or weekly
            /// - m, month, or monthly
            /// - y, year, or yearly
            /// 
            /// In any case, but will return a single character.
            /// </remarks>
            public string TimeInterval
            {
                get
                {
                    return timeInterval.ToString();
                }
                set
                {
                    switch (value.ToLower())
                    {
                        case "d":
                        case "day":
                        case "daily":
                            timeInterval = 'D';
                            break;
                        case "w":
                        case "week":
                        case "weekly":
                            timeInterval = 'W';
                            break;
                        case "m":
                        case "month":
                        case "monthly":
                            timeInterval = 'M';
                            break;
                        case "y":
                        case "year":
                        case "yearly":
                            timeInterval = 'Y';
                            break;
                        default:
                            throw new ArgumentException(value);
                    }
                }
            }

            private uint skipEvery = 0;
            /// <summary>
            /// Skip every x occurrences. Cannot be 0.
            /// </summary>
            public uint SkipEvery
            {
                get
                {
                    return skipEvery;
                }
                set
                {
                    if (value != 0)
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

            private uint posn = 0;
            /// <summary>
            /// The position within the month this event occurs.
            /// </summary>
            /// <remarks>
            /// Values 1-4 mean the first, second, third, and fourth instances of that day within the month, respectively.
            /// 
            /// A value of 5 means that it will *always* happen on the last instance of that day within the month.
            /// </remarks>
            public uint Posn
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
                    throw new NotImplementedException();
                    // yield return DateTime.MinValue;
                }
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }
            /// <summary>
            /// Serialize this ruleset
            /// </summary>
            /// <returns>A Serialized form of this ruleset</returns>
            public override XmlDocument Fetch()
            {
                return new XmlDocument();
            }
        }
    }
}