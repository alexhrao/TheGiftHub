using System;
using System.Web;
using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        public class EventFuture : IFetchable
        {
            public ulong EventFutureId
            {
                get;
                private set;
            } = 0;
            public readonly int Day;
            public readonly int Month;
            public readonly int Year;
            public EventFuture(ulong id, int Year, int Month, int Day)
            {
                this.EventFutureId = id;
                this.Year = Year;
                this.Month = Month;
                this.Day = Day;
            }
            
            public EventFuture(int Year, int Month, int Day)
            {
                this.Year = Year;
                this.Month = Month;
                this.Day = Day;
            }

            public XmlDocument Fetch()
            {
                XmlDocument info = new XmlDocument();
                XmlElement container = info.CreateElement("eventFuture");
                info.AppendChild(container);

                XmlElement year = info.CreateElement("year");
                year.InnerText = HttpUtility.HtmlEncode(Year);
                XmlElement month = info.CreateElement("month");
                month.InnerText = HttpUtility.HtmlEncode(Month);
                XmlElement day = info.CreateElement("day");
                day.InnerText = HttpUtility.HtmlEncode(Day);

                container.AppendChild(year);
                container.AppendChild(month);
                container.AppendChild(day);

                return info;
            }
        }
    }
}