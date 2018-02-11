using System;
using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// A single occurrence of an event
        /// </summary>
        public class Occurrence : IFetchable, IComparable<DateTime>, IComparable<Occurrence>, IEquatable<Occurrence>
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
                if (o == null)
                {
                    return -1;
                }
                else
                {
                    return Date.CompareTo(o.Date);
                }
            }
            /// <summary>
            /// See if the given object is actually this occurrence
            /// </summary>
            /// <param name="obj">The object to inspect</param>
            /// <returns>True if they represent the same occurrence value</returns>
            public override bool Equals(object obj)
            {
                if (obj != null && obj is Occurrence o)
                {
                    return Equals(o);
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// See if these two Occurrences are equal
            /// </summary>
            /// <param name="o">The occurrence</param>
            /// <returns>True if they are the same day and event</returns>
            public bool Equals(Occurrence o)
            {
                return o != null && o.Event.Equals(Event) && o.Date.Equals(Date);
            }
            /// <summary>
            /// Get the hash code for this occurrence
            /// </summary>
            /// <returns>The hash code</returns>
            public override int GetHashCode()
            {
                return Event.GetHashCode() + Date.GetHashCode();
            }
            /// <summary>
            /// Serialize this Occurrence
            /// </summary>
            /// <returns>An XmlDocument with the year, month, and day of this occurrence</returns>
            /// <remarks>
            /// This serialization has the following fields:
            /// - eventId: The ID for the associated event
            /// - year: The year this event occurs
            /// - month: The month this event occurs
            /// - day: The day this event occurs
            /// 
            /// This is all wrapped in an occurrence container
            /// </remarks>
            public XmlDocument Fetch()
            {

                XmlDocument info = new XmlDocument();
                XmlElement container = info.CreateElement("occurrence");
                info.AppendChild(container);

                XmlElement id = info.CreateElement("eventId");
                id.InnerText = Event.ID.ToString();
                XmlElement year = info.CreateElement("year");
                year.InnerText = Date.Year.ToString();
                XmlElement month = info.CreateElement("month");
                month.InnerText = Date.Month.ToString();
                XmlElement day = info.CreateElement("day");
                day.InnerText = Date.Day.ToString();

                container.AppendChild(id);
                container.AppendChild(year);
                container.AppendChild(month);
                container.AppendChild(day);

                return info;
            }
            /// <summary>
            /// Get an occurrence viewable by this user
            /// </summary>
            /// <param name="viewer">The viewer of this event</param>
            /// <returns>A serialized version of this occurrence</returns>
            public XmlDocument Fetch(User viewer)
            {
                XmlDocument info = new XmlDocument();
                XmlElement container = info.CreateElement("occurrence");
                info.AppendChild(container);
                // Check that events and viewer are in sync
                if (Event.User.GetEvents(viewer).Exists(e => e.ID == Event.ID))
                {
                    // good to go, just return Fetch();
                    return Fetch();
                }
                else
                {
                    return info;
                }
            }
        }
    }
}