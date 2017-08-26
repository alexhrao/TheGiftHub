using GiftServer.Exceptions;
using MySql.Data.MySqlClient;
using System;
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
            public User Admin;

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
                                this.Admin = new User(Convert.ToUInt64(reader["AdminID"]));
                            }
                            else
                            {
                                throw new GroupNotFoundException(groupID);
                            }
                        }

                    }
                }
            }
            public Group(User Admin, string Name)
            {
                this.Admin = Admin;
                this.Name = Name;
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

                return info;
            }
        }
    }
}