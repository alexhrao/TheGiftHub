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
        }
    }
}
