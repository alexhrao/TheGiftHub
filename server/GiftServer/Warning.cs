using System;

namespace GiftServer
{
    namespace Exceptions
    {
        /// <summary>
        /// A Warning is not an exception (the server is still running) - derivatives must implement this class!
        /// </summary>
        public abstract class Warning
        {
            /// <summary>
            /// The message for this warning
            /// </summary>
            public readonly string Message;
            /// <summary>
            /// The name for this warning
            /// </summary>
            public readonly string Name;
            /// <summary>
            /// The time this warning was generated
            /// </summary>
            public readonly DateTime TimeStamp;
            /// <summary>
            /// Create a new warning with a given Name and Message
            /// </summary>
            /// <remarks>
            /// The base class for all other warnings
            /// </remarks>
            /// <param name="Name">The title</param>
            /// <param name="Message">The description</param>
            protected Warning(string Name, string Message)
            {
                this.TimeStamp = DateTime.Now;
                this.Name = Name;
                this.Message = Message;
            }
            /// <summary>
            /// Stringify this warning
            /// </summary>
            /// <returns>The String representation for this warning</returns>
            override public string ToString()
            {
                return TimeStamp.ToString("yyyy-dd-MM hh:mm:ss") + "\t" + Name + ": " + Message;
            }
        }
        /// <summary>
        /// A public resource is one that needs no authorization
        /// </summary>
        public class PublicResourceWarning : Warning
        {
            /// <summary>
            /// The constructor - only notes the path
            /// </summary>
            /// <param name="path">The path for the public resource</param>
            public PublicResourceWarning(string path) : base("No Authorization Needed", path) { }
        }
        /// <summary>
        /// Indicates that an invalid cookie was masquerading as a valid user.
        /// </summary>
        public class CookieNotInvalidWarning : Warning
        {
            /// <summary>
            /// The constructor: Notes the hash given
            /// </summary>
            /// <param name="hash">The hash supplied (invalid)</param>
            public CookieNotInvalidWarning(string hash) : base("Invalid User detected", "User with hash " + hash + " is not signed in, but given request assuming otherwise") { }
        }
        /// <summary>
        /// Indicates that there was an invalid culture given
        /// </summary>
        public class InvalidCultureWarning : Warning
        {
            /// <summary>
            /// The constructor: Notes the invalid culture
            /// </summary>
            /// <param name="culture">The culture as reported by the browser</param>
            public InvalidCultureWarning(string culture) : base("Invalid Culture", "User's Browser responded with culture " + culture) { }
        }
        /// <summary>
        /// An error has occured
        /// </summary>
        /// <remarks>
        /// Indicates a fatal error for the request from this client.
        /// This should never happen in production
        /// </remarks>
        public class ExecutionErrorWarning : Warning
        {
            /// <summary>
            /// The constructor, which notes the exception
            /// </summary>
            /// <param name="exception">The Exception thrown</param>
            public ExecutionErrorWarning(Exception exception) : base("Execution Error", exception.ToString()) { }
        }
    }
}
