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
            /// <summary>
            /// The date of this occurrence
            /// </summary>
            public readonly DateTime Date;
            /// <summary>
            /// The event this occurrence references
            /// </summary>
            public readonly Event Event;
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
            /// Create an occurrence from a given Event and DateTime
            /// </summary>
            /// <param name="e">The Event this occurrence should reference</param>
            /// <param name="date">The date this event occurs - time is ignored</param>
            public Occurrence(Event e, DateTime date)
            {
                Event = e;
                Date = date;
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