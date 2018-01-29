using System;
using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// A Blacked Out Date for an event
        /// </summary>
        /// <remarks>
        /// A Blackout is a day where an event _should_ occur, but won't.
        /// </remarks>
        public class Blackout : ISynchronizable, IFetchable
        {
            /// <summary>
            /// The ID of this Blackout
            /// </summary>
            public ulong BlackoutId
            {
                get;
                private set;
            } = 0;
            /// <summary>
            /// The event this blacks out
            /// </summary>
            public Event Event
            {
                get;
                private set;
            }
            /// <summary>
            /// The date to blackout an event
            /// </summary>
            public DateTime BlackoutDate
            {
                get;
                private set;
            }
            /// <summary>
            /// Create a new blackout date for a specified event
            /// </summary>
            /// <param name="Event">The event to blackout</param>
            /// <param name="blackoutDate">The date to blackout</param>
            public Blackout(Event Event, DateTime blackoutDate)
            {
                this.Event = Event;
                BlackoutDate = blackoutDate;
            }
            /// <summary>
            /// Fetch an existing Blackout from the database
            /// </summary>
            /// <param name="blackoutId">The blackout ID</param>
            public Blackout(ulong blackoutId)
            {

            }
            /// <summary>
            /// Create a record of this blackout date in the database
            /// </summary>
            /// <returns>A status flag</returns>
            /// <remarks>
            /// Note that, since Event already creates this, it is unlikely the end user will need this method
            /// </remarks>
            public bool Create()
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// Update the record of this blackout date in the database
            /// </summary>
            /// <returns>A status flag</returns>
            /// <remarks>
            /// Note that, since Event already updates this, it is unlikely the end user will need this method
            /// </remarks>
            public bool Update()
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// Delete the record of this blackout date
            /// </summary>
            /// <returns>A status flag</returns>
            /// <remarks>
            /// Note that, since Event already deletes this, it is unlikely the end user will need this method
            /// </remarks>
            public bool Delete()
            {
                throw new NotImplementedException();
            }
            /// <summary>
            /// Serialize this BlackoutDate
            /// </summary>
            /// <returns>A Serialized form of this ruleset</returns>
            public XmlDocument Fetch()
            {
                return new XmlDocument();
            }
        }
    }
}