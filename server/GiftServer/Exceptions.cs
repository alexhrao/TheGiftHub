using GiftServer.Data;
using System;

namespace GiftServer
{
    namespace Exceptions
    {
        public abstract class UserException : Exception
        {
            public UserException(string message) : base(message) { }
        }
        public class UserNotFoundException : UserException
        {
            // A user wasn't found.
            public UserNotFoundException(string email) : base(email) { }
            public UserNotFoundException(ulong id) : base(Convert.ToString(id)) { }
            public UserNotFoundException(byte[] pHash) : base(Convert.ToBase64String(pHash)) { }
        }
        public class DuplicateUserException : UserException
        {
            public DuplicateUserException(string email) : base(email) { }
            public DuplicateUserException(ulong id) : base(Convert.ToString(id)) { }
        }

        public abstract class PasswordException : Exception
        {
            public PasswordException(string message) : base(message) { }
        }
        public class InvalidPasswordException : PasswordException
        {
            public InvalidPasswordException() : base("") { }
        }
        public class PasswordResetTimeoutException : PasswordException
        {
            public PasswordResetTimeoutException() : base("Timeout Expired") { }
        }

        public abstract class EventException : Exception
        {
            public EventException(string message) : base(message) { }
        }
        public class EventNotFoundException : EventException
        {
            public EventNotFoundException(ulong id, string name) : base("Event " + name + " for user " + id + " Does not exist") { }
            public EventNotFoundException(ulong id) : base("Event " + id + " does not exist") { }
        }
        public class InvalidEventException : EventException
        {
            public InvalidEventException(string type, int val) : base("Invalid " + type + " (" + val + ")") { }
        }
        public class DefaultEventException : EventException
        {
            public DefaultEventException(DefaultEvent e) : base(e.Name + " is a default event and cannot be modified") { }
        }

        public class CategoryNotFoundException : Exception
        {
            public CategoryNotFoundException(ulong id) : base("Category with ID " + id + " Not Found") { }
        }

        public abstract class GroupException : Exception
        {
            public GroupException(string message) : base(message) { }
        }
        public class GroupNotFoundException : GroupException
        {
            public GroupNotFoundException(ulong id) : base("Group with ID " + id + " Not Found") { }
        }
    }
}
