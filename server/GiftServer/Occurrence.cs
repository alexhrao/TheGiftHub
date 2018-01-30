
using System;
using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// A single occurrence of an event
        /// </summary>
        public class Occurrence : IFetchable, IComparable<DateTime>, IComparable<Occurrence>
        {
            public readonly DateTime Date;
            /// <summary>
            /// The year this event occurs in
            /// </summary>
            public int Year
            {
                get
                {
                    return Date.Year;
                }
            }
            /// <summary>
            /// The month this event occurs in
            /// </summary>
            public int Month
            {
                get
                {
                    return Date.Month;
                }
            }
            /// <summary>
            /// The day this event occurs in
            /// </summary>
            public int Day
            {
                get
                {
                    return Date.Day;
                }
            }
            /// <summary>
            /// Compare this to another date
            /// </summary>
            /// <param name="d">The date to compare</param>
            /// <returns>-1 if this is before, 0 if on the same day, and 1 after</returns>
            public int CompareTo(DateTime d)
            {
                return Date.CompareTo(d);
            }
            /// <summary>
            /// Compare this to another Occurrence
            /// </summary>
            /// <param name="o">The occurrence to compare</param>
            /// <returns>-1 if this is before, 0 if on the same day, and 1 after</returns>
            public int CompareTo(Occurrence o)
            {
                return Date.CompareTo(o.Date);
            }
            /// <summary>
            /// Create an occurrence from a given DateTime
            /// </summary>
            /// <param name="date">The date this event occurs - time is ignored</param>
            public Occurrence(DateTime date)
            {
                Date = date;
            }
            /// <summary>
            /// Create an occurrence from a year, month, and day
            /// </summary>
            /// <param name="year">The year this event occurs in</param>
            /// <param name="month">The month this event occurs in</param>
            /// <param name="day">The day this event occurs on</param>
            public Occurrence(int year, int month, int day)
            {
                Date = new DateTime(year, month, day);
            }
            /// <summary>
            /// Serialize this Occurrence
            /// </summary>
            /// <returns>An XmlDocument with the year, month, and day of this occurrence</returns>
            public XmlDocument Fetch()
            {
                return new XmlDocument();
            }
        }
    }
}