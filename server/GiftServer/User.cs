using System;
using GiftServer.Security;
using MySql.Data.MySqlClient;
using System.Configuration;
using GiftServer.Exceptions;
using GiftServer.Properties;
using System.Net.Mail;
using System.Net;

namespace GiftServer
{
    namespace Data
    {
        public class User : ISynchronizable
        {
            public long id = -1;
            public string firstName;
            public string lastName;
            public string email;
            public string passwordHash;
            public int theme;
            public string imagePath;
            public User(long id)
            {
                // User is already logged in; just fetch their information!
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT users.FirstName, users.LastName, users.UserEmail, passwords.PasswordHash, users.UserTheme, users.UserImage "
                                        + "FROM users "
                                        + "INNER JOIN passwords ON passwords.PasswordID = users.PasswordID "
                                        + "WHERE users.UserID = @id;";
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                this.id = id;
                                this.firstName = (string)(reader["FirstName"]);
                                this.lastName = (string)(reader["LastName"]);
                                this.email = (string)(reader["UserEmail"]);
                                this.passwordHash = (string)(reader["PasswordHash"]);
                                this.theme = Convert.ToInt32(reader["UserTheme"]);
                                this.imagePath = (string)(reader["UserImage"]);
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
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT users.UserID, users.FirstName, users.LastName, passwords.PasswordHash, users.UserTheme, users.UserImage "
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
                                this.imagePath = (string)(reader["UserImage"]);
                            }
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
            
            public static bool SendRecoveryEmail(string emailAddress)
            {
                long id;
                string token;
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT users.UserID FROM users WHERE users.UserEmail = @email;";
                        cmd.Parameters.AddWithValue("@email", emailAddress);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Get data:
                                id = Convert.ToInt64(reader["UserID"]);
                                token = PasswordReset.GenerateToken();
                                string URL = Resources.URL + "?ResetToken=" + token;
                                string body = URL;
                                /* actually, body will contain ALL html in email, but haven't written it yet. */
                                MailMessage email = new MailMessage(new MailAddress("GiftRegistry<no-reply@GiftRegistry.com>"), new MailAddress(emailAddress));
                                email.Body = body;
                                email.Subject = "Password Reset";
                                using (SmtpClient sender = new SmtpClient("smtp.gmail.com", 587))
                                {
                                    sender.EnableSsl = true;
                                    sender.DeliveryMethod = SmtpDeliveryMethod.Network;
                                    sender.UseDefaultCredentials = false;
                                    sender.Credentials = new NetworkCredential("NoReplyGiftRegistry@gmail.com", Resources.emailPassword);
                                    sender.Send(email);
                                }
                            }
                            else
                            {
                                throw new UserNotFoundException(emailAddress);
                            }
                        }
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO passwordResets (UserID, ResetHash) VALUES (@uid, @hash);";
                        cmd.Parameters.AddWithValue("@uid", id);
                        cmd.Parameters.AddWithValue("@hash", PasswordReset.ComputeHash(token));
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            public bool Create()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
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
                        cmd.CommandText = "INSERT INTO users (FirstName, LastName, UserEmail, PasswordID, UserTheme, UserImage) "
                            + "VALUES (@fName, @lName, @email, @pid, @theme, @img);";
                        cmd.Parameters.AddWithValue("@fName", this.firstName);
                        cmd.Parameters.AddWithValue("@lName", this.lastName);
                        cmd.Parameters.AddWithValue("@email", this.email);
                        cmd.Parameters.AddWithValue("@pid", pId);
                        cmd.Parameters.AddWithValue("@theme", this.theme);
                        cmd.Parameters.AddWithValue("@img", this.imagePath);
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
                if (this.id == -1)
                {
                    // User does not exist - create new one instead.
                    return Create();
                }
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
                {
                    long pId;
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        // Update user information
                        cmd.CommandText = "UPDATE users "
                            + "SET FirstName = @fName, "
                            + "LastName = @lName, "
                            + "UserEmail = @email, "
                            + "UserTheme = @theme, "
                            + "UserImage = @img "
                            + "WHERE UserID = @id;";
                        cmd.Parameters.AddWithValue("@fName", this.firstName);
                        cmd.Parameters.AddWithValue("@lName", this.lastName);
                        cmd.Parameters.AddWithValue("@email", this.email);
                        cmd.Parameters.AddWithValue("@theme", this.theme);
                        cmd.Parameters.AddWithValue("@img", this.imagePath);
                        cmd.Parameters.AddWithValue("@id", this.id);
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
                    using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
                    {
                        con.Open();
                        long pId;
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            MySqlCommand command = new MySqlCommand();
                            command.Connection = con;
                            command.CommandText = "SELECT PasswordID FROM users WHERE UserID = @id";
                            command.Parameters.AddWithValue("@id", this.id);
                            command.Prepare();
                            using (MySqlDataReader reader = command.ExecuteReader())
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
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.CommandText = "DELETE FROM passwords WHERE PasswordID = @id";
                            cmd.Parameters.AddWithValue("@id", pId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.CommandText = "DELETE FROM users WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", this.id);
                            cmd.Prepare();
                            if (cmd.ExecuteNonQuery() != 0)
                            {
                                this.id = -1;
                                return true;
                            }
                            else
                            {
                                this.id = -1;
                                throw new UserNotFoundException(this.email);
                            }
                        }
                    }
                }
            }

        }
    }
}