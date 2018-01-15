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
        public class Group : ISynchronizable, IFetchable
        {
            public ulong GroupId
            {
                get;
                private set;
            } = 0;
            public string Name;
            public string Description;
            public User Admin
            {
                get
                {
                    return admin;
                }
            }
            private User admin;

            public List<User> Users
            {
                get
                {
                    return users;
                }
            }
            private List<User> users = new List<User>();

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
            public Group(User Admin, string Name)
            {
                this.admin = Admin;
                this.Name = Name;
            }
            // Eventually, need to add support for children!
            public Group(User Admin, string Name, List<User> Users)
            {
                this.admin = Admin;
                this.Name = Name;
                this.users = Users;
            }

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
            /// <param name="evnt">The event</param>
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
            /// <param name="evnt"></param>
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
            /// <param name="gift"></param>
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
            /// <param name="gift"></param>
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