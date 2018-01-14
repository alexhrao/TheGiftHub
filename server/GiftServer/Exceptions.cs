using GiftServer.Data;
using System;
using System.Net.Mail;

namespace GiftServer
{
    namespace Exceptions
    {
        /// <summary>
        /// A indefinite User Exception
        /// </summary>
        [Serializable]
        public abstract class UserException : Exception
        {
            /// <summary>
            /// The default constructor
            /// </summary>
            /// <param name="message">Message about this user</param>
            public UserException(string message) : base(message) { }
        }
        /// <summary>
        /// Use if the User wasn't found...
        /// </summary>
        public class UserNotFoundException : UserException
        {
            /// <summary>
            /// If the email is invalid / DNE
            /// </summary>
            /// <param name="email">the MailAddress for this user</param>
            // A user wasn't found.
            public UserNotFoundException(MailAddress email) : base(email.Address) { }
            /// <summary>
            /// If the ID was invalid / DNE
            /// </summary>
            /// <param name="id">The invalid UserID</param>
            public UserNotFoundException(ulong id) : base(Convert.ToString(id)) { }
            /// <summary>
            /// If the Hash was DNE
            /// </summary>
            /// <param name="pHash">The UserHash</param>
            public UserNotFoundException(byte[] pHash) : base(Convert.ToBase64String(pHash)) { }
        }
        /// <summary>
        /// If this email / ID already exists
        /// </summary>
        public class DuplicateUserException : UserException
        {
            /// <summary>
            /// If the email is already taken
            /// </summary>
            /// <param name="email">The MailAddress for this email</param>
            public DuplicateUserException(MailAddress email) : base(email.Address) { }
            /// <summary>
            /// The UserID for this duplicate
            /// </summary>
            /// <param name="id">The ID</param>
            public DuplicateUserException(ulong id) : base(Convert.ToString(id)) { }
        }
        /// <summary>
        /// Base class for password exceptions
        /// </summary>
        [Serializable]
        public abstract class PasswordException : Exception
        {
            /// <summary>
            /// Base constructor for password exceptions
            /// </summary>
            /// <param name="message">The message for this exception</param>
            public PasswordException(string message) : base(message) { }
        }
        /// <summary>
        /// An invalid (incorrect) password was supplied
        /// </summary>
        public class InvalidPasswordException : PasswordException
        {
            /// <summary>
            /// A default constructor
            /// </summary>
            public InvalidPasswordException() : base("") { }
        }
        /// <summary>
        /// The Reset Time has passed (defined as time since the reset token was generated)
        /// </summary>
        public class PasswordResetTimeoutException : PasswordException
        {
            /// <summary>
            /// A default constructor
            /// </summary>
            public PasswordResetTimeoutException() : base("Timeout Expired") { }
        }
        /// <summary>
        /// Base class for event exceptions
        /// </summary>
        [Serializable]
        public abstract class EventException : Exception
        {
            /// <summary>
            /// Base constructor
            /// </summary>
            /// <param name="message">Message to display</param>
            public EventException(string message) : base(message) { }
        }
        /// <summary>
        /// If the event was not found
        /// </summary>
        public class EventNotFoundException : EventException
        {
            /// <summary>
            /// If an Event with Name &amp; User doesn't exist (does NOT imply the existence of the event itself)
            /// </summary>
            /// <param name="id">The UserID</param>
            /// <param name="name">The name of the event</param>
            public EventNotFoundException(ulong id, string name) : base("Event " + name + " for user " + id + " Does not exist") { }
            /// <summary>
            /// No event with this ID exists
            /// </summary>
            /// <param name="id">The supposed EventID</param>
            public EventNotFoundException(ulong id) : base("Event " + id + " does not exist") { }
        }
        /// <summary>
        /// An invalid event was given
        /// </summary>
        public class InvalidEventException : EventException
        {
            /// <summary>
            /// Bad type was given
            /// </summary>
            /// <param name="type">The type</param>
            /// <param name="val">The underlying value</param>
            public InvalidEventException(string type, int val) : base("Invalid " + type + " (" + val + ")") { }
        }
        /// <summary>
        /// An attempt to modify a default event was made
        /// </summary>
        public class DefaultEventException : EventException
        {
            /// <summary>
            /// Creates a new exception with the name of the DefaultEvent
            /// </summary>
            /// <param name="e">the DefaultEvent</param>
            public DefaultEventException(DefaultEvent e) : base(e.Name + " is a default event and cannot be modified") { }
        }
        /// <summary>
        /// If the category was not found
        /// </summary>
        [Serializable]
        public class CategoryNotFoundException : Exception
        {
            /// <summary>
            /// If the category with ID was not found
            /// </summary>
            /// <param name="id">Category ID</param>
            public CategoryNotFoundException(ulong id) : base("Category with ID " + id + " Not Found") { }
            /// <summary>
            /// If the category with Name was not found
            /// </summary>
            /// <param name="name">The Category Name</param>
            public CategoryNotFoundException(string name) : base("Category with name " + name + " Not Found") { }
        }
        /// <summary>
        /// Base class for GroupExceptions
        /// </summary>
        [Serializable]
        public abstract class GroupException : Exception
        {
            /// <summary>
            /// Base Constructor
            /// </summary>
            /// <param name="message">Message for viewing</param>
            public GroupException(string message) : base(message) { }
        }
        /// <summary>
        /// If the group wasn't found
        /// </summary>
        public class GroupNotFoundException : GroupException
        {
            /// <summary>
            /// A group with ID was unable to be located
            /// </summary>
            /// <param name="id">The GroupID</param>
            public GroupNotFoundException(ulong id) : base("Group with ID " + id + " Not Found") { }
        }
    }
}
