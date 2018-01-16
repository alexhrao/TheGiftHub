using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// A future date for an event.
        /// </summary>
        /// <remarks>
        /// This class is possibly unfinished
        /// </remarks>
        public class EventFuture : IFetchable
        {
            /// <summary>
            /// The ID for this EventFuture
            /// </summary>
            public ulong EventFutureId
            {
                get;
                private set;
            } = 0;
            /// <summary>
            /// The day this eventFuture occurs on
            /// </summary>
            public readonly int Day;
            /// <summary>
            /// The month this eventFuture occurs on
            /// </summary>
            public readonly int Month;
            /// <summary>
            /// The year this eventFuture occurs on
            /// </summary>
            public readonly int Year;
            /// <summary>
            /// Instantiate this with an ID - which is not actually required
            /// </summary>
            /// <param name="id">The ID of this</param>
            /// <param name="Year">The year</param>
            /// <param name="Month">The month</param>
            /// <param name="Day">The day</param>
            public EventFuture(ulong id, int Year, int Month, int Day)
            {
                this.EventFutureId = id;
                this.Year = Year;
                this.Month = Month;
                this.Day = Day;
            }
            /// <summary>
            /// Instantiate a new EventFuture
            /// </summary>
            /// <param name="Year">The year</param>
            /// <param name="Month">The month</param>
            /// <param name="Day">The day</param>
            public EventFuture(int Year, int Month, int Day)
            {
                this.Year = Year;
                this.Month = Month;
                this.Day = Day;
            }
            /// <summary>
            /// Serialize this eventFuture
            /// </summary>
            /// <remarks>
            /// This XML Document has the following fields:
            ///     - eventFutureId: The ID for this eventFuture
            ///     - year: The year this occurs on
            ///     - month: The month this occurs on
            ///     - day: The day this occurs on
            ///     
            /// This is all wrapped in an eventFuture container
            /// </remarks>
            /// <returns></returns>
            public XmlDocument Fetch()
            {
                XmlDocument info = new XmlDocument();
                XmlElement container = info.CreateElement("eventFuture");
                info.AppendChild(container);

                XmlElement eventFutureId = info.CreateElement("eventFutureId");
                eventFutureId.InnerText = EventFutureId.ToString();
                XmlElement year = info.CreateElement("year");
                year.InnerText = Year.ToString();
                XmlElement month = info.CreateElement("month");
                month.InnerText = Month.ToString();
                XmlElement day = info.CreateElement("day");
                day.InnerText = Day.ToString();

                container.AppendChild(eventFutureId);
                container.AppendChild(year);
                container.AppendChild(month);
                container.AppendChild(day);

                return info;
            }
        }
    }
}