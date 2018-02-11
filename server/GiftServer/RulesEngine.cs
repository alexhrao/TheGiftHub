using System;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// A set of Rules that describes recurrence
        /// </summary>
        public abstract class RulesEngine : ISynchronizable, IFetchable, IEquatable<RulesEngine>
        {
            /// <summary>
            /// The ID of this rules engine
            /// </summary>
            public abstract ulong ID
            {
                get;
                private protected set;
            }
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
            public abstract void Create();
            /// <summary>
            /// Update a record of this event rule in the database
            /// </summary>
            /// <returns>A status flag</returns>
            public abstract void Update();
            /// <summary>
            /// Delete the record of this event rule in the database
            /// </summary>
            /// <returns>A status flag</returns>
            public abstract void Delete();
            /// <summary>
            /// See if two rule sets are actually the same one
            /// </summary>
            /// <param name="engine">The rules engine</param>
            /// <returns>True if they are the same type and have the same ID</returns>
            public abstract bool Equals(RulesEngine engine);
            /// <summary>
            /// Serialize this Rule
            /// </summary>
            /// <remarks>
            /// Note that this method must be defined by derived classes - as such, no default implementation is given.
            /// </remarks>
            /// <returns>A Serialized XmlDocument</returns>
            public abstract XmlDocument Fetch();
            /// <summary>
            /// Serialize this RuleEngine
            /// </summary>
            /// <param name="viewer">The viewer</param>
            /// <returns>A serialization viewable to the viewer</returns>
            public XmlDocument Fetch(User viewer)
            {
                if (Event.User.GetEvents(viewer).Exists(e => e.ID == ID))
                {
                    return Fetch();
                }
                else
                {
                    XmlDocument info = new XmlDocument();
                    TextInfo converter = new CultureInfo("en-US", false).TextInfo;
                    string converted = converter.ToTitleCase(GetType().ToString());
                    converted = converted.Substring(0, 1).ToLower() + converted.Substring(1);
                    XmlElement container = info.CreateElement(converted);
                    info.AppendChild(container);
                    return info;
                }
            }
        }
    }
}