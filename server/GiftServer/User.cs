using System;
using GiftServer.Security;
using MySql.Data.MySqlClient;
using System.Configuration;
using GiftServer.Exceptions;
namespace GiftServer
{
    namespace Data
    {
        public class User : ISynchronizable
        {
            private long _id = -1;
            public string firstName;
            public string lastName;
            public string email;
            public string passwordHash;
            public int theme;
            public string imagePath;
            public User(string email, string password)
            {
                // If this is called, the user already exists in DB; fetch. If it can't find it, throw UserNotFoundException. 
                // If found, but password mismatch, throw InvalidPasswordException.
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
                {
                    con.Open();
                    MySqlCommand command = new MySqlCommand();
                    command.Connection = con;
                    command.CommandText = "SELECT users.UserID, users.FirstName, users.LastName, passwords.PasswordHash, users.UserTheme, users.UserImage "
                        + "FROM users "
                        + "INNER JOIN passwords ON passwords.PasswordID = users.PasswordID "
                        + "WHERE users.UserEmail = @email;";
                    command.Parameters.AddWithValue("@email", email);
                    command.Prepare();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            // User not found, throw correct exception
                            throw new UserNotFoundException(email);
                        }
                        while (reader.Read())
                        {
                            // Check password
                            if (!PasswordHash.Verify(password, (string)(reader["PasswordHash"])))
                            {
                                // Not correct, throw new exception!
                                throw new InvalidPasswordException();
                            }
                            _id = (int)(reader["UserId"]);
                            this.firstName = (string)(reader["FirstName"]);
                            this.lastName = (string)(reader["LastName"]);
                            this.email = email;
                            this.passwordHash = PasswordHash.Hash(password);
                            this.theme = Convert.ToInt32(reader["UserTheme"]);
                            this.imagePath = (string)(reader["UserImage"]);
                        }
                    }
                }
                
            }
            public User(string firstName, string lastName, string email, string password) : this(firstName, lastName, email, password, 1, "") { }
            public User(string firstName, string lastName, string email, string password, int theme, string imagePath)
            {
                this.email = email;
                passwordHash = PasswordHash.Hash(password);
                this.firstName = firstName;
                this.lastName = lastName;
                this.theme = theme;
                this.imagePath = imagePath;
            }

            public bool Create()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
                {
                    con.Open();
                    MySqlCommand command = new MySqlCommand();
                    command.Connection = con;
                    // Create new password:
                    command.CommandText = "INSERT INTO passwords (PasswordHash) VALUES (@pwd);";
                    command.Parameters.AddWithValue("@pwd", this.passwordHash);
                    command.Parameters.AddWithValue("@stamp", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    command.Prepare();
                    command.ExecuteNonQuery();
                    long pId = command.LastInsertedId;
                    command.CommandText = "INSERT INTO users (FirstName, LastName, UserEmail, PasswordID, UserTheme, UserImage) "
                        + "VALUES (@fName, @lName, @email, @pid, @theme, @img);";
                    command.Parameters.AddWithValue("@fName", this.firstName);
                    command.Parameters.AddWithValue("@lName", this.lastName);
                    command.Parameters.AddWithValue("@email", this.email);
                    command.Parameters.AddWithValue("@pid", pId);
                    command.Parameters.AddWithValue("@theme", this.theme);
                    command.Parameters.AddWithValue("@img", this.imagePath);
                    command.Prepare();
                    command.ExecuteNonQuery();
                    this._id = command.LastInsertedId;
                }
                    return false;
            }
            public bool Update()
            {
                if (this._id == -1)
                {
                    // User does not exist - create new one instead.
                    return Create();
                }
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
                {
                    con.Open();
                    MySqlCommand command = new MySqlCommand();
                    command.Connection = con;
                    // Update user information
                    command.CommandText = "UPDATE users "
                        + "SET FirstName = @fName, "
                        + "LastName = @lName, "
                        + "UserEmail = @email, "
                        + "UserTheme = @theme, "
                        + "UserImage = @img "
                        + "WHERE UserID = @id;";
                    command.Parameters.AddWithValue("@fName", this.firstName);
                    command.Parameters.AddWithValue("@lName", this.lastName);
                    command.Parameters.AddWithValue("@email", this.email);
                    command.Parameters.AddWithValue("@theme", this.theme);
                    command.Parameters.AddWithValue("@img", this.imagePath);
                    command.Parameters.AddWithValue("@id", this._id);
                    command.Prepare();
                    if (command.ExecuteNonQuery() == 0)
                    {
                        return false;
                    } else
                    {
                        // Get password ID of user:
                        command.CommandText = "SELECT PasswordID FROM users WHERE UserID = @id;";
                        command.Parameters.AddWithValue("@id", this._id);
                        command.Prepare();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Update password with PID:
                                command.CommandText = "UPDATE passwords SET PasswordHash = @pwd WHERE PasswordID = @id;";
                                command.Parameters.AddWithValue("@pwd", this.passwordHash);
                                command.Parameters.AddWithValue("@id", (long)(reader["PasswordID"]));
                                command.Prepare();
                                return command.ExecuteNonQuery() != 0;
                            } else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            public bool Delete()
            {
                if (this._id == -1)
                {
                    // User doesn't exist - don't delete
                    return false;
                } else
                {
                    using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
                    {
                        con.Open();
                        MySqlCommand command = new MySqlCommand();
                        command.Connection = con;
                        command.CommandText = "SELECT PasswordID FROM users WHERE UserID = @id";
                        command.Parameters.AddWithValue("@id", this._id);
                        command.Prepare();
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            long pId = Convert.ToInt64(reader["PasswordID"]);
                            command.CommandText = "DELETE FROM passwords WHERE PasswordID = @id";
                            command.Parameters.AddWithValue("@id", pId);
                            command.Prepare();
                            command.ExecuteNonQuery();
                        }
                        command.CommandText = "DELETE FROM users WHERE UserID = @id;";
                        command.Parameters.AddWithValue("@id", this._id);
                        command.Prepare();
                        if (command.ExecuteNonQuery() != 0)
                        {
                            this._id = -1;
                            return true;
                        } else
                        {
                            throw new UserNotFoundException(this.email);
                        }
                    }
                }
            }

        }
    }
}