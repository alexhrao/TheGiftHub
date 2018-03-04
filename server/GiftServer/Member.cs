using System;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// Represents a member of a group
        /// </summary>
        public class Member : IEquatable<Member>
        {
            /// <summary>
            /// The ID of the enclosed user.
            /// </summary>
            public ulong ID
            {
                get
                {
                    return User.ID;
                }
            }
            /// <summary>
            /// The user this member represents
            /// </summary>
            public readonly User User;

            /// <summary>
            /// Whether or not this member is a child
            /// </summary>
            public bool IsChild;

            /// <summary>
            /// Create a new member
            /// </summary>
            /// <param name="user">The user this member is</param>
            /// <param name="isChild">Whether or not this user is a child</param>
            public Member(User user, bool isChild)
            {
                User = user;
                IsChild = isChild;
            }

            /// <summary>
            /// Create a new member, defaulting the isChild to false
            /// </summary>
            /// <param name="user">User this member represents</param>
            public Member(User user) : this(user, false) { }

            /// <summary>
            /// Convert a User to an adult member
            /// </summary>
            /// <param name="u">The user to convert</param>
            public static implicit operator Member(User u)
            {
                return new Member(u);
            }

            /// <summary>
            /// Convert a member back to a user.
            /// </summary>
            /// <param name="m">The member to convert</param>
            public static implicit operator User(Member m)
            {
                return m.User;
            }
            /// <summary>
            /// See if this object is equal to this member
            /// </summary>
            /// <param name="o">The object to compare</param>
            /// <returns>Whether or not the two objects are equal</returns>
            public override bool Equals(object o)
            {
                if (o != null && o is Member m)
                {
                    return Equals(m);
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="member"></param>
            /// <returns></returns>
            public bool Equals(Member member)
            {
                return member != null && member.User.Equals(User);
            }
            /// <summary>
            /// Gets the hashcode for this user
            /// </summary>
            /// <returns>A hash code for this member</returns>
            public override int GetHashCode()
            {
                return User.GetHashCode() + ID.GetHashCode();
            }
        }
    }
}
