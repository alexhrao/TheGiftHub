using GiftServer.Exceptions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// A Group of users
        /// </summary>
        /// <remarks>
        /// Groups are the primary means by which users can interact with each other. Often, two users can _only_ interact if they have at least one common group.
        /// 
        /// The only person allowed to make changes to a groups structure (name, description, etc.) is the Admin, who is also the only one that can delete a group.
        /// </remarks>
        public class Group : ISynchronizable, IFetchable, IEquatable<Group>
        {
            /// <summary>
            /// This Group's ID
            /// </summary>
            /// <remarks>
            /// If this is 0, the group must be Created() before anything else makes sense.
            /// </remarks>
            public ulong ID
            {
                get;
                private set;
            } = 0;
            private string name = "";
            /// <summary>
            /// The name of this group
            /// </summary>
            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException(nameof(value), "Name must not be null");
                    }
                    else if (String.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("Name must not be empty or space", nameof(value));
                    }
                    else
                    {
                        name = value;
                    }
                }
            }
            /// <summary>
            /// The description for this group
            /// </summary>
            public string Description;
            private User admin;
            /// <summary>
            /// The Administrator for this group (An ordinary user in all other respects, however)
            /// </summary>
            public User Admin
            {
                get
                {
                    return admin;
                }
                private set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException(nameof(value), "Admin must not be null");
                    }
                    else if (value.ID == 0)
                    {
                        throw new ArgumentException("Admin must be a valid user", nameof(value));
                    }
                    else
                    {
                        admin = value;
                    }
                }
            }
            private List<Member> members = new List<Member>();
            /// <summary>
            /// A list of all members including the administrator
            /// </summary>
            public List<Member> Members
            {
                get
                {
                    return new List<Member>(members)
                    {
                        Admin
                    };
                }
            }
            /// <summary>
            /// A list of all events viewable to members of this group
            /// </summary>
            public List<Event> Events
            {
                get
                {
                    List<Event> events = new List<Event>();
                    // For each User in this group, get all their events.
                    // BUT: filter that by existing in that events groups!
                    foreach (User member in Members)
                    {
                        events.AddRange(member.GetEvents(this));
                    }
                    return events;
                }
            }
            /// <summary>
            /// Gifts viewable to all members of this group
            /// </summary>
            public List<Gift> Gifts
            {
                get
                {
                    List<Gift> gifts = new List<Gift>();
                    foreach (User member in Members)
                    {
                        gifts.AddRange(member.Gifts.FindAll(gift => gift.Groups.Exists(group => group.ID == ID)));
                    }
                    return gifts;
                }
            }

            /// <summary>
            /// Fetch a group from the database
            /// </summary>
            /// <param name="groupID">The group's ID</param>
            public Group(ulong groupID)
            {
                if (groupID == 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(groupID), "Group ID must be positive");
                }
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT * FROM groups WHERE GroupID = @id;";
                        cmd.Parameters.AddWithValue("@id", groupID);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ID = groupID;
                                Name = Convert.ToString(reader["GroupName"]);
                                Description = Convert.ToString(reader["GroupDescription"]);
                                admin = new User(Convert.ToUInt64(reader["AdminID"]));
                            }
                            else
                            {
                                throw new GroupNotFoundException(groupID);
                            }
                        }
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT groups_users.UserID, groups_users.IsChild FROM groups_users WHERE groups_users.GroupID = @gid AND NOT groups_users.UserID = @aid;";
                        cmd.Parameters.AddWithValue("@gid", ID);
                        cmd.Parameters.AddWithValue("@aid", Admin.ID);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Members.Add(new Member(
                                    new User(Convert.ToUInt64(reader["UserID"])),
                                    Convert.ToBoolean(reader["IsChild"])));
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Create a new group with no users
            /// </summary>
            /// <param name="admin">The Administrator for this group</param>
            /// <param name="name">The name of this group</param>
            public Group(User admin, string name)
            {
                Admin = admin;
                Name = name;
            }
            // Eventually, need to add support for children!
            /// <summary>
            /// Create a new group with specified members
            /// </summary>
            /// <param name="admin">The administrator for this group</param>
            /// <param name="name">The name of this group</param>
            /// <param name="members">The members of this group (excluding the admin)</param>
            public Group(User admin, string name, List<Member> members) : 
                this(admin, name)
            {
                // Error check users
                members = members ?? new List<Member>();
                members.RemoveAll(m => m == null || m.User == null || m.ID == admin.ID);
                members = members.Distinct().ToList();
                if (members.Exists(m => m.ID == 0))
                {
                    throw new ArgumentException("ID-less user found!", nameof(members));
                }
                else
                {
                    this.members = members;
                }
            }
            /// <summary>
            /// Create this group in the database
            /// </summary>
            public void Create()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO groups (GroupName, GroupDescription, AdminID) "
                                        + "VALUES (@name, @desc, @admin);";
                        cmd.Parameters.AddWithValue("@name", Name);
                        cmd.Parameters.AddWithValue("@desc", Description);
                        cmd.Parameters.AddWithValue("@admin", Admin.ID);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        ID = Convert.ToUInt64(cmd.LastInsertedId);
                    }
                    foreach (Member member in Members)
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "INSERT INTO groups_users (GroupID, UserID, IsChild) "
                                            + "VALUES (@gid, @uid, @isc);";
                            cmd.Parameters.AddWithValue("@gid", ID);
                            cmd.Parameters.AddWithValue("@uid", member.ID);
                            cmd.Parameters.AddWithValue("@isc", member.IsChild);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            /// <summary>
            /// Update this group in the database
            /// </summary>
            public void Update()
            {
                if (ID == 0)
                {
                    Create();
                }
                else
                {
                    using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                    {
                        con.Open();
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "UPDATE groups "
                                            + "SET GroupName = @name, "
                                            + "GroupDescription = @desc, "
                                            + "AdminID = @uid "
                                            + "WHERE GroupID = @id;";
                            cmd.Parameters.AddWithValue("@name", Name);
                            cmd.Parameters.AddWithValue("@desc", Description);
                            cmd.Parameters.AddWithValue("@uid", Admin.ID);
                            cmd.Parameters.AddWithValue("@id", ID);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            /// <summary>
            /// Delete this group from the database
            /// </summary>
            public void Delete()
            {
                // Remove gifts, users, events, Delete this
                foreach (Gift gift in Gifts)
                {
                    Remove(gift);
                }
                foreach (User member in members)
                {
                    // Use w/o ADMIN to remove
                    Remove(member);
                }
                foreach (Event e in Events)
                {
                    Remove(e);
                }
                // Delete myself

                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    // Delete from Groups
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM groups WHERE GroupID = @id;";
                        cmd.Parameters.AddWithValue("@id", ID);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            /// <summary>
            /// Add a user to the group
            /// </summary>
            /// <param name="user">The user to be added</param>
            public void Add(User user)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO groups_users (GroupID, UserID) VALUES (@gid, @uid);";
                        cmd.Parameters.AddWithValue("@gid", ID);
                        cmd.Parameters.AddWithValue("@uid", user.ID);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }

            }
            /// <summary>
            /// Remove a user from the group
            /// </summary>
            /// <param name="user">The user to be removed from the group</param>
            public void Remove(User user)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM groups_users WHERE GroupID = @gid AND UserID = @uid;";
                        cmd.Parameters.AddWithValue("@gid", ID);
                        cmd.Parameters.AddWithValue("@uid", user.ID);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            /// <summary>
            /// Allow a given event to be seen by all members of this group
            /// </summary>
            /// <param name="evnt">The event that will now be viewable</param>
            public void Add(Event evnt)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO groups_events (GroupID, EventID) VALUES (@gid, @eid);";
                        cmd.Parameters.AddWithValue("@eid", evnt.ID);
                        cmd.Parameters.AddWithValue("@gid", ID);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            /// <summary>
            /// Don't allow a given event to be seen by all members of this group
            /// </summary>
            /// <param name="evnt">The event no longer viewable</param>
            public void Remove(Event evnt)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM groups_events WHERE GroupID = @gid AND EventID = @eid;";
                        cmd.Parameters.AddWithValue("@gid", ID);
                        cmd.Parameters.AddWithValue("@eid", evnt.ID);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            /// <summary>
            /// Allow a given gift to be seen by all members of this group
            /// </summary>
            /// <param name="gift">The gift to become viewable</param>
            public void Add(Gift gift)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO groups_gifts (GroupID, GiftID) VALUES (@grid, @gid);";
                        cmd.Parameters.AddWithValue("@grid", ID);
                        cmd.Parameters.AddWithValue("@gid", gift.ID);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            /// <summary>
            /// Don't allow a given gift to be seen by all members of this group
            /// </summary>
            /// <param name="gift">The gift that will no longer be viewable</param>
            public void Remove(Gift gift)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM groups_gifts WHERE GroupID = @grid AND GiftID = @gid;";
                        cmd.Parameters.AddWithValue("@grid", ID);
                        cmd.Parameters.AddWithValue("@gid", gift.ID);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            /// <summary>
            /// Find if this group is equal to this object
            /// </summary>
            /// <param name="obj">The object to compare to</param>
            /// <returns>A boolean if the two objects are equal</returns>
            public override bool Equals(object obj)
            {
                if (obj != null && obj is Group g)
                {
                    return Equals(g);
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// Find out if this group is equal to another
            /// </summary>
            /// <param name="group">The group to compare</param>
            /// <returns>true if they are indeed the same group</returns>
            public bool Equals(Group group)
            {
                return group != null && group.ID == ID;
            }
            /// <summary>
            /// Get the hash code for this object
            /// </summary>
            /// <returns>The code</returns>
            public override int GetHashCode()
            {
                return ID.GetHashCode();
            }
            /// <summary>
            /// Serializes this group's information
            /// </summary>
            /// <remarks>
            /// As with all other Fetch() methods, this returns an XML document.
            /// This document has the following fields:
            ///     - groupId: The group's ID
            ///     - name: The group's name
            ///     - description: The group's description
            ///     - adminId: The group's Admin ID
            ///     - members: All members of the group
            ///         - Note that each element of _members_ is a _member_ which has the following fields:
            ///             - userId: This member's ID
            ///             - userName: This member's Name
            ///         - Note: The name is provided as a convenience for the caller.
            ///             
            /// This is all wrapped in a group container.
            /// </remarks>
            /// <returns>Serialization of this group as an XmlDocument</returns>
            public XmlDocument Fetch()
            {
                XmlDocument info = new XmlDocument();
                XmlElement container = info.CreateElement("group");
                info.AppendChild(container);

                XmlElement id = info.CreateElement("groupId");
                id.InnerText = ID.ToString();
                XmlElement name = info.CreateElement("name");
                name.InnerText = Name;
                XmlElement description = info.CreateElement("description");
                description.InnerText = Description;
                XmlElement admin = info.CreateElement("adminId");
                admin.InnerText = Admin.ID.ToString();
                XmlElement members = info.CreateElement("members");
                foreach (Member member in members)
                {
                    XmlElement mem = info.CreateElement("member");
                    XmlElement userId = info.CreateElement("userId");
                    XmlElement userName = info.CreateElement("userName");
                    XmlElement userChildStatus = info.CreateElement("isChild");
                    userId.InnerText = member.User.ID.ToString();
                    userName.InnerText = member.User.Name;
                    userChildStatus.InnerText = member.IsChild ? "1" : "0";
                    mem.AppendChild(userId);
                    mem.AppendChild(userName);
                    mem.AppendChild(userChildStatus);
                    members.AppendChild(mem);
                }

                container.AppendChild(id);
                container.AppendChild(name);
                container.AppendChild(description);
                container.AppendChild(admin);
                container.AppendChild(members);

                return info;
            }
            /// <summary>
            /// Fetch this group
            /// </summary>
            /// <param name="viewer">The viewer</param>
            /// <returns>Serialization of this group</returns>
            public XmlDocument Fetch(User viewer)
            {
                // First see if in groups
                // if viewer is in this group, good to go
                if (viewer.Groups.Exists(g => g.ID == ID))
                {
                    return Fetch();
                }
                else
                {
                    XmlDocument info = new XmlDocument();
                    XmlElement container = info.CreateElement("group");
                    info.AppendChild(container);

                    return info;
                }
            }
        }
    }
}