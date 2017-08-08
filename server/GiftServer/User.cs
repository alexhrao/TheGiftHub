using System;
using GiftServer.Security;
using MySql.Data.MySqlClient;
using System.Configuration;
using GiftServer.Exceptions;
using System.IO;
using GiftServer.Properties;
using GiftServer.Server;

namespace GiftServer
{
    namespace Data
    {
        public class User : ISynchronizable, IShowable
        {
            public long UserId = -1;
            public string FirstName;
            public string LastName;
            public string Email;
            public string PasswordHash;
            public int Theme;
            public int BirthMonth;
            public int BirthDay;
            public string Bio;
            public DateTime DateJoined;
            public User(long id)
            {
                // User is already logged in; just fetch their information!
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT users.FirstName, users.LastName, users.UserEmail, passwords.PasswordHash, users.UserTheme, users.UserBirthMonth, users.UserBirthDay, users.TimeCreated, users.UserBio "
                                        + "FROM users "
                                        + "INNER JOIN passwords ON passwords.UserID = users.UserID "
                                        + "WHERE users.UserID = @id;";
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                this.UserId = id;
                                this.FirstName = Convert.ToString(reader["FirstName"]);
                                this.LastName = Convert.ToString(reader["LastName"]);
                                this.Email = Convert.ToString(reader["UserEmail"]);
                                this.PasswordHash = Convert.ToString(reader["PasswordHash"]);
                                this.Theme = Convert.ToInt32(reader["UserTheme"]);
                                this.BirthDay = Convert.ToInt32(reader["UserBirthDay"]);
                                this.BirthMonth = Convert.ToInt32(reader["UserBirthMonth"]);
                                this.DateJoined = (DateTime)(reader["TimeCreated"]);
                                this.Bio = Convert.ToString(reader["UserBio"]);
                                
                            }
                            else
                            {
                                throw new UserNotFoundException(id);
                            }
                        }
                    }
                }
            }
            public User(string email, string password)
            {
                // If this is called, the user already exists in DB; fetch. If it can't find it, throw UserNotFoundException. 
                // If found, but password mismatch, throw InvalidPasswordException.
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT users.UserID, users.FirstName, users.LastName, passwords.PasswordHash, users.UserTheme, users.UserBirthMonth, users.UserBirthDay, users.TimeCreated, users.UserBio "
                                        + "FROM users "
                                        + "INNER JOIN passwords ON passwords.UserID = users.UserID "
                                        + "WHERE users.UserEmail = @email;";
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Prepare();

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                // User not found, throw correct exception
                                throw new UserNotFoundException(email);
                            }
                            else
                            {
                                // Check password
                                if (!Security.PasswordHash.Verify(password, Convert.ToString(reader["PasswordHash"])))
                                {
                                    // Not correct, throw new exception!
                                    throw new InvalidPasswordException();
                                }
                                UserId = Convert.ToInt64(reader["UserID"]);
                                this.FirstName = Convert.ToString(reader["FirstName"]);
                                this.LastName = Convert.ToString(reader["LastName"]);
                                this.Email = email;
                                this.PasswordHash = Security.PasswordHash.Hash(password);
                                this.Theme = Convert.ToInt32(reader["UserTheme"]);
                                this.BirthDay = Convert.ToInt32(reader["UserBirthMonth"]);
                                this.BirthMonth = Convert.ToInt32(reader["UserBirthDay"]);
                                this.DateJoined = (DateTime)(reader["TimeCreated"]);
                                this.Bio = Convert.ToString(reader["UserBio"]);
                            }
                        }
                    }
                }
                
            }
            public User() { }

            public bool UpdatePassword(string pass)
            {
                if (this.UserId == -1)
                {
                    return false;
                }
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "UPDATE passwords SET PasswordHash = @pwd WHERE UserID = @id;";
                        cmd.Parameters.AddWithValue("@pwd", Security.PasswordHash.Hash(pass));
                        cmd.Parameters.AddWithValue("@id", this.UserId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            public bool Create()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        // Check if email present:
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT UserID FROM users WHERE UserEmail = @email;";
                        cmd.Parameters.AddWithValue("@email", this.Email);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // User already exists; throw exception:
                                throw new DuplicateUserException(this.Email);
                            }
                        }
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO users (FirstName, LastName, UserEmail, UserTheme, UserBirthMonth, UserBirthDay, UserBio) "
                            + "VALUES (@fName, @lName, @email, @pid, @theme, @bmonth, @bday, @bio);";
                        cmd.Parameters.AddWithValue("@fName", this.FirstName);
                        cmd.Parameters.AddWithValue("@lName", this.LastName);
                        cmd.Parameters.AddWithValue("@email", this.Email);
                        cmd.Parameters.AddWithValue("@theme", this.Theme);
                        cmd.Parameters.AddWithValue("@bmonth", this.BirthMonth);
                        cmd.Parameters.AddWithValue("@bday", this.BirthDay);
                        cmd.Parameters.AddWithValue("@bio", this.Bio);
                        cmd.Prepare();
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            return false;
                        }
                        this.UserId = cmd.LastInsertedId;
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        // Create new password:
                        cmd.CommandText = "INSERT INTO passwords (PasswordHash, UserID) VALUES (@pwd, @id);";
                        cmd.Parameters.AddWithValue("@pwd", Security.PasswordHash.Hash(this.PasswordHash));
                        cmd.Parameters.AddWithValue("@id", this.UserId);
                        cmd.Prepare();
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            // Failed:
                            return false;
                        };
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT TimeCreated FROM users WHERE UserID = @id;";
                        cmd.Parameters.AddWithValue("@id", this.UserId);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            this.DateJoined = (DateTime)(reader["TimeCreated"]);
                        }
                    }
                }
                return true;
            }

            public bool Update()
            {
                if (this.UserId == -1)
                {
                    // User does not exist - create new one instead.
                    return Create();
                }
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        // Check if email present:
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT UserID FROM users WHERE UserEmail = @email AND UserID <> @id;";
                        cmd.Parameters.AddWithValue("@email", this.Email);
                        cmd.Parameters.AddWithValue("@id", this.UserId);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // User already exists; throw exception:
                                throw new DuplicateUserException(this.Email);
                            }
                        }
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        // Update user information
                        cmd.CommandText = "UPDATE users "
                            + "SET FirstName = @fName, "
                            + "LastName = @lName, "
                            + "UserEmail = @email, "
                            + "UserTheme = @theme, "
                            + "UserBio = @bio, "
                            + "UserBirthMonth = @bmonth, "
                            + "UserBirthDay = @bday "
                            + "WHERE UserID = @id;";
                        cmd.Parameters.AddWithValue("@fName", this.FirstName);
                        cmd.Parameters.AddWithValue("@lName", this.LastName);
                        cmd.Parameters.AddWithValue("@email", this.Email);
                        cmd.Parameters.AddWithValue("@theme", this.Theme);
                        cmd.Parameters.AddWithValue("@bio", this.Bio);
                        cmd.Parameters.AddWithValue("@bmonth", this.BirthMonth);
                        cmd.Parameters.AddWithValue("@bday", this.BirthDay);
                        cmd.Parameters.AddWithValue("@id", this.UserId);
                        cmd.Prepare();
                        return (cmd.ExecuteNonQuery() == 1);
                    }
                    // Only way to update password is through password reset, so no need here
                }
            }

            public bool Delete()
            {
                // TODO: Gauruntee not admin of any group
                if (this.UserId == -1)
                {
                    // User doesn't exist - don't delete
                    return false;
                }
                else
                {
                    using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                    {
                        con.Open();
                        // Delete from event futures, groups, and events:
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            // Get EventUserID:
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM events_users_futures WHERE EventUserID IN (SELECT EventUserID FROM events_users WHERE UserID = @id);";
                            cmd.Parameters.AddWithValue("@id", this.UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            // Get EventUserID:
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM events_users_groups WHERE EventUserID IN (SELECT EventUserID FROM events_users WHERE UserID = @id);";
                            cmd.Parameters.AddWithValue("@id", this.UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM events_users WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from purchases and reservations:
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM purchases WHERE ReservationID IN (SELECT ReservationID FROM reservations WHERE UserID = @id);";
                            cmd.Parameters.AddWithValue("@id", this.UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from reservations
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM reservations WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete gifts from receptions, groups, and gifts:
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM receptions WHERE GiftID IN (SELECT GiftID FROM gifts WHERE UserID = @id);";
                            cmd.Parameters.AddWithValue("@id", this.UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM groups_gifts WHERE GiftID IN (SELECT GiftID FROM gifts WHERE UserID = @id);";
                            cmd.Parameters.AddWithValue("@id", this.UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM gifts WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from groups_users
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM groups_users WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // No need to delete from groups; guaranteed to NOT be admin of any
                        // Delete from Passwordresets
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM passwordresets WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from preferences
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM users_preferences WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from passwords
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM passwords WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from users
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM users WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        this.UserId = -1;
                        return true;
                    }
                }
            }
            public void SaveImage(MultipartParser parser)
            {
                ImageProcessor processor = new ImageProcessor(parser);
                File.WriteAllBytes(Resources.BasePath + "/resources/images/users/User" + this.UserId + Resources.ImageFormat, processor.Data);
            }
            public void RemoveImage()
            {
                File.Delete(Resources.BasePath + "/resources/images/users/User" + this.UserId + Resources.ImageFormat);
            }
            public string GetImage()
            {
                return GetImage(this.UserId);
            }
            public static string GetImage(long userID)
            {
                // Build path:
                string path = Resources.BasePath + "/resources/images/users/User" + userID + Resources.ImageFormat;
                // if file exists, return path. Otherwise, return default
                // Race condition, but I don't know how to solve (yet)
                if (File.Exists(path))
                {
                    return "resources/images/users/User" + userID + Resources.ImageFormat;
                }
                else
                {
                    return "resources/images/users/default" + Resources.ImageFormat;
                }
            }
        }
    }
}