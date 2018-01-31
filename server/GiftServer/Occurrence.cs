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
                id.InnerText = Event.EventId.ToString();
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
        }
    }
}