using System;
using System.Collections.Generic;
using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// A set of Rules that describes recurrence
        /// </summary>
        public abstract class RulesEngine : ISynchronizable, IFetchable
        {
            /// <summary>
            /// A List of Occurrences for this event
            /// </summary>
            /// <remarks>
            /// This iterator can be used in a foreach loop. It will return all the dates this event occurs.
            /// 
            /// Note that this means **This iterator can go infinitely**. It is up to the caller to limit 
            /// </remarks>
            public abstract IEnumerable<Occurrence> Occurrences
            {
                get;
            }
            /// <summary>
            /// The Event this is tied to
            /// </summary>
            public Event Event;
            /// <summary>
            /// Create a record of this event rule in the database
            /// </summary>
            /// <returns>A status flag</returns>
            public abstract bool Create();
            /// <summary>
            /// Update a record of this event rule in the database
            /// </summary>
            /// <returns>A status flag</returns>
            public abstract bool Update();
            /// <summary>
            /// Delete the record of this event rule in the database
            /// </summary>
            /// <returns>A status flag</returns>
            public abstract bool Delete();
            /// <summary>
            /// Serialize this Rule
            /// </summary>
            /// <remarks>
            /// Note that this method must be defined by derived classes - as such, no default implementation is given.
            /// </remarks>
            /// <returns>A Serialized XmlDocument</returns>
            public abstract XmlDocument Fetch();
        }
    }
}