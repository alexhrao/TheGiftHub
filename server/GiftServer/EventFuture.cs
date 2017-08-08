using System;

namespace GiftServer
{
    namespace Data
    {
        public class EventFuture
        {
            public readonly long EventFutureId;
            public readonly int Day;
            public readonly int Month;
            public readonly int Year;
            public EventFuture(long id, int Year, int Month, int Day)
            {
                this.Year = Year;
                this.Month = Month;
                this.Day = Day;
            }
        }
    }
}