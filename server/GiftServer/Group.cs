using GiftServer.Exceptions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        public class Group : ISynchronizable, IFetchable
        {
            /// <summary>
            /// This Group's ID
            /// </summary>
            /// <remarks>
            /// If this is 0, the group must be Created() before anything else makes sense.
            /// </remarks>
            public ulong GroupId
            {
                get;
                private set;
            } = 0;
            /// <summary>
            /// The name of this group
            /// </summary>
            public string Name;
            /// <summary>
            /// The description for this group
            /// </summary>
            public string Description;
            /// <summary>
            /// The Administrator for this group (An ordinary user in all other respects, however)
            /// </summary>
            public User Admin
            {
                get
                {
                    return admin;
                }
            }
            private User admin;
            /// <summary>
            /// A list of all members
            /// </summary>
            public List<User> Users
            {
                get
                {
                    return users;
                }
            }
            private List<User> users = new List<User>();
            /// <summary>
            /// Fetch a group from the database
            /// </summary>
            /// <param name="groupID">The group's ID</param>
            public Group(ulong groupID)
            {
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
                                this.GroupId = groupID;
                                this.Name = Convert.ToString(reader["GroupName"]);
                                this.Description = Convert.ToString(reader["GroupDescription"]);
                                this.admin = new User(Convert.ToUInt64(reader["AdminID"]));
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
                        cmd.CommandText = "SELECT groups_users.UserID FROM groups_users WHERE groups_users.GroupID = @gid AND NOT groups_users.UserID = @aid;";
                        cmd.Parameters.AddWithValue("@gid", this.GroupId);
                        cmd.Parameters.AddWithValue("@aid", this.Admin.UserId);
                        cmd.Prepare();
                        using (MySqlDataReader Reader = cmd.ExecuteReader())
                        {
                            while (Reader.Read())
                            {
                                users.Add(new User(Convert.ToUInt64(Reader["UserID"])));
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Create a new group with no users
            /// </summary>
            /// <param name="Admin">The Administrator for this group</param>
            /// <param name="Name">The name of this group</param>
            public Group(User Admin, string Name)
            {
                this.admin = Admin;
                this.Name = Name;
            }
            // Eventually, need to add support for children!
            /// <summary>
            /// Create a new group with specified members
            /// </summary>
            /// <param name="Admin">The administrator for this group</param>
            /// <param name="Name">The name of this group</param>
            /// <param name="Users">The members of this group (excluding the admin)</param>
            public Group(User Admin, string Name, List<User> Users)
            {
                this.admin = Admin;
                this.Name = Name;
                this.users = Users;
            }
            /// <summary>
            /// Create this group in the database
            /// </summary>
            /// <returns>A status flag</returns>
            public bool Create()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO groups (GroupName, GroupDescription, AdminID) "
                                        + "VALUES (@name, @desc, @admin);";
                        cmd.Parameters.AddWithValue("@name", this.Name);
                        cmd.Parameters.AddWithValue("@desc", this.Description);
                        cmd.Parameters.AddWithValue("@admin", this.Admin.UserId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        this.GroupId = Convert.ToUInt64(cmd.LastInsertedId);
                    }
                    foreach (User user in Users)
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "INSERT INTO groups_users (GroupID, UserID, IsChild) "
                                            + "VALUES (@gid, @uid, FALSE);";
                            cmd.Parameters.AddWithValue("@gid", this.GroupId);
                            cmd.Parameters.AddWithValue("@uid", user.UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                return true;
            }
            /// <summary>
            /// Update this group in the database
            /// </summary>
            /// <returns>A status flag</returns>
            public bool Update()
            {
                if (GroupId == 0)
                {
                    return Create();
                }
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
                        cmd.Parameters.AddWithValue("@name", this.Name);
                        cmd.Parameters.AddWithValue("@desc", this.Description);
                        cmd.Parameters.AddWithValue("@uid", this.Admin.UserId);
                        cmd.Parameters.AddWithValue("@id", this.GroupId);
                        cmd.Prepare();
                        return cmd.ExecuteNonQuery() == 0;
                    }
                }
            }
            /// <summary>
            /// Delete this group from the database
            /// </summary>
            /// <returns>A status flag</returns>
            public bool Delete()
            {
                if (GroupId == 0)
                {
                    return false;
                }
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    // Delete from EventsUsersGroups
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM events_users_groups WHERE GroupID = @id;";
                        cmd.Parameters.AddWithValue("@id", this.GroupId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                    // Delete from GroupsGifts
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM groups_gifts WHERE GroupID = @id;";
                        cmd.Parameters.AddWithValue("@id", this.GroupId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                    // Delete from GroupsUsers
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM groups_users WHERE GroupID = @id;";
                        cmd.Parameters.AddWithValue("@id", this.GroupId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                    // Delete from Groups
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM groups WHERE GroupID = @id;";
                        cmd.Parameters.AddWithValue("@id", this.GroupId);
                        cmd.Prepare();
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            this.GroupId = 0;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
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
                        cmd.Parameters.AddWithValue("@gid", GroupId);
                        cmd.Parameters.AddWithValue("@uid", user.UserId);
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
                        cmd.Parameters.AddWithValue("@gid", GroupId);
                        cmd.Parameters.AddWithValue("@uid", user.UserId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            /// <summary>
            /// Allow a given event to be seen by all members of this group
            /// </summary>
            /// <param name="evnt">The event that will now be viewable</param>
            public void Add(EventUser evnt)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO events_users_groups (EventUserID, GroupID) VALUES (@eid, @gid);";
                        cmd.Parameters.AddWithValue("@eid", evnt.EventUserId);
                        cmd.Parameters.AddWithValue("@gid", GroupId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            /// <summary>
            /// Don't allow a given event to be seen by all members of this group
            /// </summary>
            /// <param name="evnt">The event no longer viewable</param>
            public void Remove(EventUser evnt)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM events_users_groups WHERE GroupID = @gid AND EventUserID = @eid;";
                        cmd.Parameters.AddWithValue("@gid", GroupId);
                        cmd.Parameters.AddWithValue("@eid", evnt.EventUserId);
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
                        cmd.Parameters.AddWithValue("@grid", GroupId);
                        cmd.Parameters.AddWithValue("@gid", gift.GiftId);
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
                        cmd.Parameters.AddWithValue("@grid", GroupId);
                        cmd.Parameters.AddWithValue("@gid", gift.GiftId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
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
                id.InnerText = GroupId.ToString();
                XmlElement name = info.CreateElement("name");
                name.InnerText = Name;
                XmlElement description = info.CreateElement("description");
                description.InnerText = Description;
                XmlElement admin = info.CreateElement("adminId");
                admin.InnerText = Admin.UserId.ToString();
                XmlElement members = info.CreateElement("members");
                foreach (User user in users)
                {
                    XmlElement member = info.CreateElement("member");
                    XmlElement userId = info.CreateElement("userId");
                    XmlElement userName = info.CreateElement("userName");
                    userId.InnerText = user.UserId.ToString();
                    userName.InnerText = user.UserName;
                    member.AppendChild(userId);
                    member.AppendChild(userName);
                    members.AppendChild(member);
                }

                container.AppendChild(id);
                container.AppendChild(name);
                container.AppendChild(description);
                container.AppendChild(admin);
                container.AppendChild(members);

                return info;
            }
        }
    }
}