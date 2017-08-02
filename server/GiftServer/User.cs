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
            public long id = -1;
            public string firstName;
            public string lastName;
            public string email;
            public string passwordHash;
            public int theme;
            public DateTime dob;
            public readonly DateTime dateJoined;
            public User(long id)
            {
                // User is already logged in; just fetch their information!
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT users.FirstName, users.LastName, users.UserEmail, passwords.PasswordHash, users.UserTheme, users.DateOfBirth, users.TimeCreated "
                                        + "FROM users "
                                        + "INNER JOIN passwords ON passwords.PasswordID = users.PasswordID "
                                        + "WHERE users.UserID = @id;";
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                this.id = id;
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
                        cmd.CommandText = "SELECT users.UserID, users.FirstName, users.LastName, passwords.PasswordHash, users.UserTheme, users.DateOfBirth, users.TimeCreated "
                                        + "FROM users "
                                        + "INNER JOIN passwords ON passwords.PasswordID = users.PasswordID "
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
                                id = Convert.ToInt64(reader["UserID"]);
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
                            }
                        }
                    }
                }
                
            }
            public User(string firstName, string lastName, string email, string password) : this(firstName, lastName, email, password, 1, DateTime.MinValue) { }
            public User(string firstName, string lastName, string email, string password, int theme, DateTime dob)
            {
                this.email = email;
                this.passwordHash = PasswordHash.Hash(password);
                this.firstName = firstName;
                this.lastName = lastName;
                this.theme = theme;
                this.dob = dob;
                this.dateJoined = DateTime.Now;
            }

            public bool Create()
            {
                // TODO: Check if email already exists
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    long pId;
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        // Create new password:
                        cmd.CommandText = "INSERT INTO passwords (PasswordHash) VALUES (@pwd);";
                        cmd.Parameters.AddWithValue("@pwd", this.passwordHash);
                        cmd.Parameters.AddWithValue("@stamp", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                        cmd.Prepare();
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            // Failed:
                            return false;
                        };
                        pId = cmd.LastInsertedId;
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO users (FirstName, LastName, UserEmail, PasswordID, UserTheme, DateOfBirth) "
                            + "VALUES (@fName, @lName, @email, @pid, @theme, @dob);";
                        cmd.Parameters.AddWithValue("@fName", this.firstName);
                        cmd.Parameters.AddWithValue("@lName", this.lastName);
                        cmd.Parameters.AddWithValue("@email", this.email);
                        cmd.Parameters.AddWithValue("@pid", pId);
                        cmd.Parameters.AddWithValue("@theme", this.theme);
                        cmd.Parameters.AddWithValue("@dob", this.dob);
                        cmd.Prepare();
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            return false;
                        }
                        this.id = cmd.LastInsertedId;
                    }
                }
                return true;
            }

            public bool Update()
            {
                // TODO: Check if email already exists
                if (this.id == -1)
                {
                    // User does not exist - create new one instead.
                    return Create();
                }
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    long pId;
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        // Update user information
                        // TODO: Deal with DOB
                        cmd.CommandText = "UPDATE users "
                            + "SET FirstName = @fName, "
                            + "LastName = @lName, "
                            + "UserEmail = @email, "
                            + "UserTheme = @theme "
                            + "WHERE UserID = @id;";
                        cmd.Parameters.AddWithValue("@fName", this.firstName);
                        cmd.Parameters.AddWithValue("@lName", this.lastName);
                        cmd.Parameters.AddWithValue("@email", this.email);
                        cmd.Parameters.AddWithValue("@theme", this.theme);
                        cmd.Parameters.AddWithValue("@id", this.id);
                        cmd.Parameters.AddWithValue("@dob", this.dob);
                        cmd.Prepare();
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            return false;
                        }
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        // Get password ID of user:
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT PasswordID FROM users WHERE UserID = @id;";
                        cmd.Parameters.AddWithValue("@id", this.id);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                pId = Convert.ToInt64(reader["PasswordID"]);
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        // Update password with PID:
                        cmd.Connection = con;
                        cmd.CommandText = "UPDATE passwords SET PasswordHash = @pwd WHERE PasswordID = @id;";
                        cmd.Parameters.AddWithValue("@pwd", this.passwordHash);
                        cmd.Parameters.AddWithValue("@id", pId);
                        cmd.Prepare();
                        return cmd.ExecuteNonQuery() != 0;
                    }
                }
            }

            public bool Delete()
            {
                if (this.id == -1)
                {
                    // User doesn't exist - don't delete
                    return false;
                }
                else
                {
                    using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                    {
                        con.Open();
                        long pId;
                        // Get password ID
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT PasswordID FROM users WHERE UserID = @id";
                            cmd.Parameters.AddWithValue("@id", this.id);
                            cmd.Prepare();
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    pId = Convert.ToInt64(reader["PasswordID"]);
                                }
                                else
                                {
                                    throw new InvalidPasswordException();
                                }
                            }
                        }
                        // Delete from passwords
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM passwords WHERE PasswordID = @id";
                            cmd.Parameters.AddWithValue("@id", pId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from users
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM users WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.id);
                            cmd.Prepare();
                            if (cmd.ExecuteNonQuery() == 0)
                            {
                                throw new UserNotFoundException(this.email);
                            }
                        }
                        // Delete from Events
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM events_users WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.id);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from receptions & reservations (based on GiftID):
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT GiftID FROM gifts WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.id);
                            cmd.Prepare();
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                // For each ID, delete from receptions:
                                while (reader.Read())
                                {
                                    using (MySqlCommand rm = new MySqlCommand())
                                    {
                                        rm.Connection = con;
                                        rm.CommandText = "DELETE FROM receptions WHERE GiftID = @id;";
                                        rm.Parameters.AddWithValue("@id", reader["GiftID"]);
                                        rm.Prepare();
                                        rm.ExecuteNonQuery();
                                    }
                                    using (MySqlCommand rm = new MySqlCommand())
                                    {
                                        rm.Connection = con;
                                        rm.CommandText = "DELETE FROM reservations WHERE GiftID = @id;";
                                        rm.Parameters.AddWithValue("@id", reader["GiftID"]);
                                        rm.Prepare();
                                        rm.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        // Delete from gifts
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM gifts WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.id);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from groups - what if s/he is admin??
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM groups_users WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.id);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from reservations
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM reservations WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.id);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                            return true;
                        }
                    }
                }
            }
            public void SaveImage(MultipartParser parser)
            {
                ImageProcessor processor = new ImageProcessor(parser);
                File.WriteAllBytes(Resources.BasePath + "/resources/images/users/User" + this.id + Resources.ImageFormat, processor.Data);
            }
            public void RemoveImage()
            {
                File.Delete(Resources.BasePath + "/resources/images/users/User" + this.id + Resources.ImageFormat);
            }
            public string GetImage()
            {
                return GetImage(this.id);
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