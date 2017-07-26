using System;

namespace GiftServer
{
    namespace Exceptions
    {
        public class UserNotFoundException : Exception
        {
            // A user wasn't found.
            public UserNotFoundException(string email) : base(email) { }
        }
        public class InvalidPasswordException : Exception
        {
            public InvalidPasswordException() : base() { }
        }
    }
}
