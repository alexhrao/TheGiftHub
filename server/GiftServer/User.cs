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
            public long Id = -1;
            public string firstName;
            public string lastName;
            public string email;
            public string passwordHash;
            public int theme;
            public DateTime dob;
            public string bio;
            public DateTime dateJoined;
            public User(long id)
            {
                // User is already logged in; just fetch their information!
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT users.FirstName, users.LastName, users.UserEmail, passwords.PasswordHash, users.UserTheme, users.DateOfBirth, users.TimeCreated, users.UserBio "
                                        + "FROM users "
                                        + "INNER JOIN passwords ON passwords.UserID = users.UserID "
                                        + "WHERE users.UserID = @id;";
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                this.Id = id;
                                this.firstName = (string)(reader["FirstName"]);
                                this.lastName = (string)(reader["LastName"]);
                                this.email = (string)(reader["UserEmail"]);
                                this.passwordHash = (string)(reader["PasswordHash"]);
                                this.theme = Convert.ToInt32(reader["UserTheme"]);
                                try
                                {
                                    this.dob = (DateTime)(reader["DateOfBirth"]);
                                }
                                catch (InvalidCastException)
                                {
                                    this.dob = DateTime.MinValue;
                                }
                                this.dateJoined = (DateTime)(reader["TimeCreated"]);
                                this.bio = (string)(reader["UserBio"]);
                                
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
                        cmd.CommandText = "SELECT users.UserID, users.FirstName, users.LastName, passwords.PasswordHash, users.UserTheme, users.DateOfBirth, users.TimeCreated, users.UserBio "
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
                                if (!PasswordHash.Verify(password, (string)(reader["PasswordHash"])))
                                {
                                    // Not correct, throw new exception!
                                    throw new InvalidPasswordException();
                                }
                                Id = Convert.ToInt64(reader["UserID"]);
                                this.firstName = (string)(reader["FirstName"]);
                                this.lastName = (string)(reader["LastName"]);
                                this.email = email;
                                this.passwordHash = PasswordHash.Hash(password);
                                this.theme = Convert.ToInt32(reader["UserTheme"]);
                                try
                                {
                                    this.dob = (DateTime)(reader["DateOfBirth"]);
                                }
                                catch (InvalidCastException)
                                {
                                    this.dob = DateTime.MinValue;
                                }
                                this.dateJoined = (DateTime)(reader["TimeCreated"]);
                                this.bio = (string)(reader["UserBio"]);
                            }
                        }
                    }
                }
                
            }
            public User() { }

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
                        cmd.Parameters.AddWithValue("@email", this.email);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // User already exists; throw exception:
                                throw new DuplicateUserException(this.email);
                            }
                        }
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO users (FirstName, LastName, UserEmail, UserTheme, DateOfBirth, UserBio) "
                            + "VALUES (@fName, @lName, @email, @pid, @theme, @dob, @bio);";
                        cmd.Parameters.AddWithValue("@fName", this.firstName);
                        cmd.Parameters.AddWithValue("@lName", this.lastName);
                        cmd.Parameters.AddWithValue("@email", this.email);
                        cmd.Parameters.AddWithValue("@theme", this.theme);
                        cmd.Parameters.AddWithValue("@dob", this.dob);
                        cmd.Parameters.AddWithValue("@bio", this.bio);
                        cmd.Prepare();
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            return false;
                        }
                        this.Id = cmd.LastInsertedId;
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        // Create new password:
                        cmd.CommandText = "INSERT INTO passwords (PasswordHash, UserID) VALUES (@pwd, @id);";
                        cmd.Parameters.AddWithValue("@pwd", PasswordHash.Hash(this.passwordHash));
                        cmd.Parameters.AddWithValue("@id", this.Id);
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
                        cmd.Parameters.AddWithValue("@id", this.Id);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            this.dateJoined = (DateTime)(reader["TimeCreated"]);
                        }
                    }
                }
                return true;
            }

            public bool Update()
            {
                if (this.Id == -1)
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
                        cmd.Parameters.AddWithValue("@email", this.email);
                        cmd.Parameters.AddWithValue("@id", this.Id);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // User already exists; throw exception:
                                throw new DuplicateUserException(this.email);
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
                            + "DateOfBirth = @dob "
                            + "WHERE UserID = @id;";
                        cmd.Parameters.AddWithValue("@fName", this.firstName);
                        cmd.Parameters.AddWithValue("@lName", this.lastName);
                        cmd.Parameters.AddWithValue("@email", this.email);
                        cmd.Parameters.AddWithValue("@theme", this.theme);
                        cmd.Parameters.AddWithValue("@bio", this.bio);
                        cmd.Parameters.AddWithValue("@dob", this.dob.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@id", this.Id);
                        cmd.Prepare();
                        return (cmd.ExecuteNonQuery() == 1);
                    }
                    // Only way to update password is through password reset, so no need here
                }
            }

            public bool Delete()
            {
                // TODO: Gauruntee not admin of any group
                if (this.Id == -1)
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
                            cmd.Parameters.AddWithValue("@id", this.Id);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            // Get EventUserID:
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM events_users_groups WHERE EventUserID IN (SELECT EventUserID FROM events_users WHERE UserID = @id);";
                            cmd.Parameters.AddWithValue("@id", this.Id);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM events_users WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.Id);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from purchases and reservations:
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM purchases WHERE ReservationID IN (SELECT ReservationID FROM reservations WHERE UserID = @id);";
                            cmd.Parameters.AddWithValue("@id", this.Id);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from reservations
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM reservations WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.Id);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete gifts from receptions, groups, and gifts:
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM receptions WHERE GiftID IN (SELECT GiftID FROM gifts WHERE UserID = @id);";
                            cmd.Parameters.AddWithValue("@id", this.Id);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM groups_gifts WHERE GiftID IN (SELECT GiftID FROM gifts WHERE UserID = @id);";
                            cmd.Parameters.AddWithValue("@id", this.Id);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM gifts WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.Id);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from groups_users
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM groups_users WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.Id);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // No need to delete from groups; guaranteed to NOT be admin of any
                        // Delete from Passwordresets
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM passwordresets WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.Id);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from preferences
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM users_preferences WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.Id);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from passwords

                        // Delete from users
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM users WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.Id);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from passwords:
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = ""
                            cmd.CommandText = "SELECT PasswordID FROM users WHERE UserID = @id";
                            cmd.Parameters.AddWithValue("@id", this.Id);
                            cmd.Prepare();
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    using (MySqlCommand eDelete = new MySqlCommand())
                                    {
                                        eDelete.Connection = con;
                                        eDelete.CommandText = "DELETE FROM passwords WHERE PasswordID = @id;";
                                        eDelete.Parameters.AddWithValue("@id", Convert.ToInt64(reader["PasswordID"]));
                                        eDelete.Prepare();
                                        eDelete.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    throw new InvalidPasswordException();
                                }
                            }
                        }
                        this.Id = -1;
                        return true;
                    }
                }
            }
            public void SaveImage(MultipartParser parser)
            {
                ImageProcessor processor = new ImageProcessor(parser);
                File.WriteAllBytes(Resources.BasePath + "/resources/images/users/User" + this.Id + Resources.ImageFormat, processor.Data);
            }
            public void RemoveImage()
            {
                File.Delete(Resources.BasePath + "/resources/images/users/User" + this.Id + Resources.ImageFormat);
            }
            public string GetImage()
            {
                return GetImage(this.Id);
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