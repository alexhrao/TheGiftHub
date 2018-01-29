
using System;
using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// A single occurrence of an event
        /// </summary>
        public class Occurrence : IFetchable
        {
            private DateTime _date;
            /// <summary>
            /// The year this event occurs in
            /// </summary>
            public int Year
            {
                get
                {
                    return _date.Year;
                }
            }
            /// <summary>
            /// The month this event occurs in
            /// </summary>
            public int Month
            {
                get
                {
                    return _date.Month;
                }
            }
            /// <summary>
            /// The day this event occurs in
            /// </summary>
            public int Day
            {
                get
                {
                    return _date.Day;
                }
            }
            /// <summary>
            /// Create an occurrence from a given DateTime
            /// </summary>
            /// <param name="date">The date this event occurs - time is ignored</param>
            public Occurrence(DateTime date)
            {
                _date = date;
            }
            /// <summary>
            /// Create an occurrence from a year, month, and day
            /// </summary>
            /// <param name="year">The year this event occurs in</param>
            /// <param name="month">The month this event occurs in</param>
            /// <param name="day">The day this event occurs on</param>
            public Occurrence(int year, int month, int day)
            {
                _date = new DateTime(year, month, day);
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