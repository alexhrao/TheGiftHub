using System;

namespace GiftServer
{
    namespace Data
    {
        public class DefaultEvent
        {
            public long DefaultEventID;
            public string Name;
            public string Description;
            public int Day;
            public int Month;
            public int Year;
            public bool IsRecurring;
            public EventFuture[] futures;

            public DefaultEvent(long DefaultEventID)
            {

            }
        }

        public class EventFuture
        {
            public readonly int Day;
            public readonly int Month;
            public readonly int Year;
            public EventFuture(int Year, int Month, int Day)
            {
                this.Year = Year;
                this.Month = Month;
                this.Day = Day;
            }
        }
    }
}