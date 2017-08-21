using System;

namespace GiftServer
{
    namespace Exceptions
    {
        public abstract class Warning
        {
            public readonly string Message;
            public readonly string Name;
            public readonly DateTime TimeStamp;

            protected Warning(string Name, string Message)
            {
                this.TimeStamp = DateTime.Now;
                this.Name = Name;
                this.Message = Message;
            }
            override public string ToString()
            {
                return TimeStamp.ToString("yyyy-dd-MM hh:mm:ss") + "\t" + Name + ": " + Message;
            }
        }
        public class PublicResourceWarning : Warning
        {
            public PublicResourceWarning(string path) : base("No Authorization Needed", path) { }
        }
        public class CookieNotInvalidWarning : Warning
        {
            public CookieNotInvalidWarning(ulong id) : base("Invalid User detected", "User with ID " + id + " is not signed in, but given request assuming otherwise") { }
        }
    }
}
