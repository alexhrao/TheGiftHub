using GiftServer.Data;
using System;

namespace GiftServer
{
    namespace Exceptions
    {
        public class UserNotFoundException : Exception
        {
            // A user wasn't found.
            public UserNotFoundException(string email) : base(email) { }
            public UserNotFoundException(long id) : base(Convert.ToString(id)) { }
            public UserNotFoundException(byte[] pHash) : base(Convert.ToBase64String(pHash)) { }
        }
        public class DuplicateUserException : Exception
        {
            public DuplicateUserException(string email) : base(email) { }
            public DuplicateUserException(long id) : base(Convert.ToString(id)) { }
        }
        public class InvalidPasswordException : Exception
        {
            public InvalidPasswordException() : base() { }
        }
        public class PasswordResetTimeoutException : Exception
        {
            public PasswordResetTimeoutException() : base("Timeout Expired") { }
        }
        public class EventNotFoundException : Exception
        {
            public EventNotFoundException(long id, string name) : base("Event " + name + " for user " + id + " Does not exist") { }
            public EventNotFoundException(long id) : base("Event " + id + " does not exist") { }
        }
        public class InvalidEventException : Exception
        {
            public InvalidEventException(string type, int val) : base("Invalid " + type + " (" + val + ")") { }
        }
        public class DefaultEventException : Exception
        {
            public DefaultEventException(DefaultEvent e) : base(e.Name + " is a default event can cannot be modified") { }
        }
    }
}
