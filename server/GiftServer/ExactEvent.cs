using System;
using System.Collections.Generic;
using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// A rule engine for Exact Events
        /// </summary>
        /// <remarks>
        /// Exact Events are events that occur on an "exact" time scale. This does not mean that the time itself never varies; 
        /// indeed, with Daylight Savings Time and Leap Year, it undoubtedly will. Instead, Exact means that the event begins on it's Start Date and happens every Time Interval.
        /// 
        /// As an example, an Exact Event could be defined as starting on January 10th, 2017 and happening every month.In this case, the next event occurrences would be:
        /// - February 10th, 2017
        /// - March 10th, 2017
        /// - April 10th, 2017
        /// 
        /// This doesn't depend on the "week" it's in, or on a specific day of the week.
        /// </remarks>
        public class ExactEvent : RulesEngine, IEquatable<ExactEvent>
        {
            /// <summary>
            /// The ID for this Exact Event
            /// </summary>
            public ulong ExactEventId
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
            /// See if these two objects are equal
            /// </summary>
            /// <param name="obj">The object to compare</param>
            /// <returns>Whether or not they are equal</returns>
            public override bool Equals(object obj)
            {
                if (obj != null && obj is ExactEvent e)
                {
                    return Equals(e);
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// See if these two RulesEngines are equivalent
            /// </summary>
            /// <param name="engine">The engine to compare</param>
            /// <returns>If they are the same</returns>
            public override bool Equals(RulesEngine engine)
            {
                if (engine != null && engine is ExactEvent e)
                {
                    return Equals(e);
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// See if these two ExactEvent Engines are the same
            /// </summary>
            /// <param name="engine">the ExactEvent engine to compare</param>
            /// <returns>If the two are equal</returns>
            public bool Equals(ExactEvent engine)
            {
                return engine != null && engine.ExactEventId == ExactEventId;
            }
            /// <summary>
            /// Get the hash code for this instnace
            /// </summary>
            /// <returns>The hash code</returns>
            public override int GetHashCode()
            {
                return ExactEventId.GetHashCode();
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
