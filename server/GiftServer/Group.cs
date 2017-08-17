using GiftServer.Exceptions;
using MySql.Data.MySqlClient;
using System;
using System.Configuration;

namespace GiftServer
{
    namespace Data
    {
        public class Group : ISynchronizable
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
        }
    }
}