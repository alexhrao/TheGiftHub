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
            public UserNotFoundException(long id) : base(Convert.ToString(id)) { }
            public UserNotFoundException(byte[] pHash) : base(Convert.ToBase64String(pHash)) { }
        }
        public class DuplicateUserException : UserException
        {
            public DuplicateUserException(string email) : base(email) { }
            public DuplicateUserException(long id) : base(Convert.ToString(id)) { }
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
            public EventNotFoundException(long id, string name) : base("Event " + name + " for user " + id + " Does not exist") { }
            public EventNotFoundException(long id) : base("Event " + id + " does not exist") { }
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
            public CategoryNotFoundException(long id) : base("Category with ID " + id + " Not Found") { }
        }

        public abstract class GroupException : Exception
        {
            public GroupException(string message) : base(message) { }
        }
        public class GroupNotFoundException : GroupException
        {
            public GroupNotFoundException(long id) : base("Group with ID " + id + " Not Found") { }
        }
    }
}
