using System;
using MySql.Data.MySqlClient;
using System.Configuration;
using GiftServer.Exceptions;
using System.IO;
using GiftServer.Properties;
using GiftServer.Server;
using System.Collections.Generic;
using GiftServer.Security;
using System.Xml;
using System.Net.Mail;
using GiftServer.Html;
using System.Net;

namespace GiftServer
{
    namespace Data
    {
        public class User : ISynchronizable, IShowable, IFetchable
        {
            public ulong UserId
            {
                get;
                private set;
            } = 0;
            public string UserName = "";
            public MailAddress Email;
            public Password Password;
            public int BirthMonth = 0;
            public int BirthDay = 0;
            public string Bio = "";
            public Preferences Preferences;
            public string UserUrl = "";
            public DateTime DateJoined
            {
                get;
                private set;
            }
            public List<Gift> Gifts
            {
                get
                {
                    List<Gift> _gifts = new List<Gift>();
                    if (UserId != 0)
                    {
                        using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                        {
                            con.Open();
                            using (MySqlCommand cmd = new MySqlCommand())
                            {
                                cmd.Connection = con;
                                cmd.CommandText = "SELECT GiftID FROM gifts WHERE UserID = @id ORDER BY GiftRating DESC;";
                                cmd.Parameters.AddWithValue("@id", UserId);
                                cmd.Prepare();
                                using (MySqlDataReader Reader = cmd.ExecuteReader())
                                {
                                    // add a new gift for each id:
                                    while (Reader.Read())
                                    {
                                        _gifts.Add(new Gift(Convert.ToUInt64(Reader["GiftID"])));
                                    }
                                }
                            }
                        }
                    }
                    return _gifts;
                }
            }
            public List<Group> Groups
            {
                get
                {
                    List<Group> _groups = new List<Group>();
                    if (UserId != 0)
                    {
                        using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                        {
                            con.Open();
                            using (MySqlCommand cmd = new MySqlCommand())
                            {
                                cmd.Connection = con;
                                cmd.CommandText = "SELECT GroupID FROM groups_users WHERE UserID = @id UNION SELECT GroupID FROM groups WHERE AdminID = @id;";
                                cmd.Parameters.AddWithValue("@id", UserId);
                                cmd.Prepare();
                                using (MySqlDataReader Reader = cmd.ExecuteReader())
                                {
                                    while (Reader.Read())
                                    {
                                        _groups.Add(new Group(Convert.ToUInt64(Reader["GroupID"])));
                                    }
                                }
                            }
                        }
                    }
                    _groups.Sort((a, b) => a.Name.CompareTo(b.Name));
                    return _groups;
                }
            }
            public List<EventUser> Events
            {
                get
                {
                    List<EventUser> _events = new List<EventUser>();
                    if (UserId != 0)
                    {
                        using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                        {
                            con.Open();
                            using (MySqlCommand cmd = new MySqlCommand())
                            {
                                cmd.Connection = con;
                                cmd.CommandText = "SELECT EventUserID FROM events_users WHERE UserID = @id;";
                                cmd.Parameters.AddWithValue("@id", UserId);
                                cmd.Prepare();
                                using (MySqlDataReader Reader = cmd.ExecuteReader())
                                {
                                    while (Reader.Read())
                                    {
                                        _events.Add(new EventUser(Convert.ToUInt64(Reader["EventUserID"])));
                                    }
                                }
                            }
                        }
                    }
                    return _events;
                }
            }
            public User(ulong id)
            {
                // User is already logged in; just fetch their information!
                FetchInformation(id);
            }
            public User(MailAddress email)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT users.UserID FROM users WHERE UserEmail = @eml;";
                        cmd.Parameters.AddWithValue("@eml", email.Address);
                        cmd.Prepare();
                        using (MySqlDataReader Reader = cmd.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                FetchInformation(Convert.ToUInt64(Reader["UserID"]));
                            }
                        }
                    }
                }
            }
            public User(string hash)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT users.UserID FROM users WHERE UserURL = @url;";
                        cmd.Parameters.AddWithValue("@url", hash);
                        cmd.Prepare();
                        using (MySqlDataReader Reader = cmd.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                FetchInformation(Convert.ToUInt64(Reader["UserID"]));
                            }
                        }
                    }
                }
            }
            public User(MailAddress email, string password)
            {
                // If this is called, the user already exists in DB; fetch. If it can't find it, throw UserNotFoundException. 
                // If found, but password mismatch, throw InvalidPasswordException.
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT users.*, passwords.PasswordHash, passwords.PasswordSalt, passwords.PasswordIter "
                                        + "FROM users "
                                        + "INNER JOIN passwords ON passwords.UserID = users.UserID "
                                        + "WHERE users.UserEmail = @email;";
                        cmd.Parameters.AddWithValue("@email", email.Address);
                        cmd.Prepare();

                        using (MySqlDataReader Reader = cmd.ExecuteReader())
                        {
                            if (!Reader.Read())
                            {
                                // User not found, throw correct exception
                                throw new UserNotFoundException(email.Address);
                            }
                            else
                            {
                                this.Password = new Password(Convert.ToString(Reader["PasswordHash"]),
                                                             Convert.ToString(Reader["PasswordSalt"]),
                                                             Convert.ToInt32(Reader["PasswordIter"]));
                                // Check password
                                if (!Password.Verify(password))
                                {
                                    throw new InvalidPasswordException();
                                }
                                UserId = Convert.ToUInt64(Reader["UserID"]);
                                this.UserName = Convert.ToString(Reader["UserName"]);
                                this.Email = email;
                                this.BirthDay = Convert.ToInt32(Reader["UserBirthDay"]);
                                this.BirthMonth = Convert.ToInt32(Reader["UserBirthMonth"]);
                                this.DateJoined = (DateTime)(Reader["TimeCreated"]);
                                this.Bio = Convert.ToString(Reader["UserBio"]);
                                this.UserUrl = Convert.ToString(Reader["UserURL"]);
                                this.Preferences = new Preferences(this);
                            }
                        }
                    }
                }
            }

            public User(MailAddress Email, Password Password)
            {
                this.Email = Email;
                this.Password = Password;
            }

            public bool UpdatePassword(string password, ResetManager ResetManager)
            {
                if (this.UserId == 0)
                {
                    return false;
                }
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        Password = new Password(password);
                        cmd.Connection = con;
                        cmd.CommandText = "UPDATE passwords SET PasswordHash = @hsh, PasswordSalt = @slt, PasswordIter = @itr WHERE UserID = @id;";
                        cmd.Parameters.AddWithValue("@hsh", Password.Hash);
                        cmd.Parameters.AddWithValue("@slt", Password.Salt);
                        cmd.Parameters.AddWithValue("@itr", Password.Iterations);
                        cmd.Parameters.AddWithValue("@id", this.UserId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
                // Send email
                
                MailMessage email = new MailMessage(new MailAddress("The Gift Hub<support@TheGiftHub.org>"), this.Email)
                {
                    Body = ResetManager.GenerateNotification(this),
                    Subject = "Password Reset Notification",
                    IsBodyHtml = true
                };
                using (SmtpClient sender = new SmtpClient("smtp.gmail.com", 587))
                {
                    sender.EnableSsl = true;
                    sender.DeliveryMethod = SmtpDeliveryMethod.Network;
                    sender.UseDefaultCredentials = false;
                    sender.Credentials = new NetworkCredential("support@thegifthub.org", Constants.emailPassword);
                    sender.Send(email);
                }
                return true;
            }
            private void FetchInformation(ulong id)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT users.*, passwords.PasswordHash, passwords.PasswordSalt, passwords.PasswordIter "
                                        + "FROM users "
                                        + "INNER JOIN passwords ON passwords.UserID = users.UserID "
                                        + "WHERE users.UserID = @id;";
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Prepare();
                        using (MySqlDataReader Reader = cmd.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                this.UserId = id;
                                this.UserName = Convert.ToString(Reader["UserName"]);
                                this.Email = new MailAddress(Convert.ToString(Reader["UserEmail"]));
                                this.Password = new Password(Convert.ToString(Reader["PasswordHash"]),
                                                             Convert.ToString(Reader["PasswordSalt"]),
                                                             Convert.ToInt32(Reader["PasswordIter"]));
                                this.BirthDay = Convert.ToInt32(Reader["UserBirthDay"]);
                                this.BirthMonth = Convert.ToInt32(Reader["UserBirthMonth"]);
                                this.DateJoined = (DateTime)(Reader["TimeCreated"]);
                                this.Bio = Convert.ToString(Reader["UserBio"]);
                                this.UserUrl = Convert.ToString(Reader["UserURL"]);
                                Preferences = new Preferences(this);
                            }
                            else
                            {
                                throw new UserNotFoundException(id);
                            }
                        }
                    }
                }
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
                        cmd.Parameters.AddWithValue("@email", this.Email.Address);
                        cmd.Prepare();
                        using (MySqlDataReader Reader = cmd.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                // User already exists; throw exception:
                                throw new DuplicateUserException(this.Email.Address);
                            }
                        }
                    }
                    bool isUnique = false;
                    while (!isUnique)
                    {
                        Password url = new Password(Email.Address);
                        UserUrl = url.Hash.Replace("+", "0").Replace("/", "0");
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT users.UserID FROM users WHERE users.UserURL = @url;";
                            cmd.Parameters.AddWithValue("@url", UserUrl);
                            cmd.Prepare();
                            using (MySqlDataReader Reader = cmd.ExecuteReader())
                            {
                                if (!Reader.Read())
                                {
                                    // Unique!
                                    isUnique = true;
                                }
                            }
                        }
                    }
                    // Look and see
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO users (UserName, UserEmail, UserBirthMonth, UserBirthDay, UserBio, UserURL) "
                            + "VALUES (@name, @email, @bmonth, @bday, @bio, @url);";
                        cmd.Parameters.AddWithValue("@name", this.UserName);
                        cmd.Parameters.AddWithValue("@email", this.Email);
                        cmd.Parameters.AddWithValue("@bmonth", this.BirthMonth);
                        cmd.Parameters.AddWithValue("@bday", this.BirthDay);
                        cmd.Parameters.AddWithValue("@bio", this.Bio);
                        cmd.Parameters.AddWithValue("@url", this.UserUrl);
                        cmd.Prepare();
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            return false;
                        }
                        this.UserId = Convert.ToUInt64(cmd.LastInsertedId);
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        // Create new password:
                        cmd.CommandText = "INSERT INTO passwords (UserID, PasswordHash, PasswordSalt, PasswordIter) VALUES (@uid, @hsh, @slt, @itr);";
                        cmd.Parameters.AddWithValue("@uid", this.UserId);
                        cmd.Parameters.AddWithValue("@hsh", Password.Hash);
                        cmd.Parameters.AddWithValue("@slt", Password.Salt);
                        cmd.Parameters.AddWithValue("@itr", Password.Iterations);
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
                        using (MySqlDataReader Reader = cmd.ExecuteReader())
                        {
                            while (Reader.Read())
                            {
                                this.DateJoined = (DateTime)(Reader["TimeCreated"]);
                            }
                        }
                    }
                    Preferences = new Preferences(this);
                    Preferences.Create();
                }
                return true;
            }

            public bool Update()
            {
                if (this.UserId == 0)
                {
                    // User does not exist - create new one instead.
                    return Create();
                }
                Preferences.Update();
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
                        using (MySqlDataReader Reader = cmd.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                // User already exists; throw exception:
                                throw new DuplicateUserException(this.Email.Address);
                            }
                        }
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        // Update user information
                        cmd.CommandText = "UPDATE users "
                            + "SET UserName = @name, "
                            + "UserEmail = @email, "
                            + "UserBio = @bio, "
                            + "UserBirthMonth = @bmonth, "
                            + "UserBirthDay = @bday "
                            + "WHERE UserID = @id;";
                        cmd.Parameters.AddWithValue("@name", this.UserName);
                        cmd.Parameters.AddWithValue("@email", this.Email);
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
                if (this.UserId == 0)
                {
                    // User doesn't exist - don't delete
                    return false;
                }
                else
                {
                    using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                    {
                        con.Open();
                        // Remove image:
                        this.RemoveImage();
                        // Remove preferences
                        Preferences.Delete();
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
                        this.UserId = 0;
                        return true;
                    }
                }
            }
            public void SaveImage(MultipartParser parser)
            {
                ImageProcessor processor = new ImageProcessor(parser);
                File.WriteAllBytes(System.IO.Directory.GetCurrentDirectory() + "/resources/images/users/User" + this.UserId + Constants.ImageFormat, processor.Data);
            }
            public void RemoveImage()
            {
                File.Delete(System.IO.Directory.GetCurrentDirectory() + "/resources/images/users/User" + this.UserId + Constants.ImageFormat);
            }
            public string GetImage()
            {
                return GetImage(this.UserId);
            }
            public static string GetImage(ulong userID)
            {
                // Build path:
                string path = System.IO.Directory.GetCurrentDirectory() + "/resources/images/users/User" + userID + Constants.ImageFormat;
                // if file exists, return path. Otherwise, return default
                // Race condition, but I don't know how to solve (yet)
                if (File.Exists(path))
                {
                    return "resources/images/users/User" + userID + Constants.ImageFormat;
                }
                else
                {
                    return "resources/images/users/default" + Constants.ImageFormat;
                }
            }

            /// <summary>
            /// Reserve ONE of a given gift;
            /// </summary>
            /// <param name="gift"></param>
            public void Reserve(Gift gift)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    // Check if any left
                    bool left = false;
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT COUNT(*) AS NumRes FROM reservations WHERE GiftID = @gid;";
                        cmd.Parameters.AddWithValue("@gid", gift.GiftId);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read() && Convert.ToUInt32(reader["NumRes"]) < gift.Quantity)
                            {
                                left = true;
                            }
                            else
                            {
                                left = false;
                            }
                        }
                    }
                    if (left)
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            // Add to reserved:
                            cmd.CommandText = "INSERT INTO reservations (GiftID, UserID) VALUES (@gid, @uid);";
                            cmd.Parameters.AddWithValue("@gid", gift.GiftId);
                            cmd.Parameters.AddWithValue("@uid", UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Throw new exception?
                    }
                }
            }

            /// <summary>
            /// Release ONE of the gifts (if multiple are reserved)
            /// Does NOT release purchased gifts.
            /// </summary>
            /// <param name="gift"></param>
            public void Release(Gift gift)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM reservations "
                                        + "WHERE ReserveStamp IN "
                                        + "( "
                                            + "SELECT MIN(ReserveStamp) FROM reservations WHERE GiftID = @gid AND UserID = @uid "
                                        + ")"
                                        + "AND ReservationID NOT IN purchases.ReservationID;";
                        cmd.Parameters.AddWithValue("@gid", gift.GiftId);
                        cmd.Parameters.AddWithValue("@uid", UserId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            /// <summary>
            /// Mark a gift as purchased
            /// </summary>
            /// <param name="gift"></param>
            public void Purchase(Gift gift)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    uint resID = 0;
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        // Get ID of reservation NOT in purchases!
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT reservations.ReservationID "
                                        + "FROM reservations "
                                        + "WHERE reservations.ReservationID NOT IN purchases.ReservationID AND reservations.UserID = @uid AND reservations.GiftID = @gid;";
                        cmd.Parameters.AddWithValue("@uid", UserId);
                        cmd.Parameters.AddWithValue("@gid", gift.GiftId);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                resID = Convert.ToUInt32(reader["ReservationID"]);
                            }
                        }
                    }
                    if (resID != 0)
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "INSERT INTO purchases (ReservationID) VALUES (@rid);";
                            cmd.Parameters.AddWithValue("@rid", resID);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            /// <summary>
            /// Unmark as purchased, but it is still reserved!
            /// </summary>
            /// <param name="gift"></param>
            public void Return(Gift gift)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM purchases "
                                        + "WHERE PurchaseStamp IN "
                                        + "("
                                            + "SELECT MIN(PurchaseStamp) "
                                            + "FROM purchases "
                                            + "INNER JOIN reservations ON reservations.ReservationID = purchases.ReservationID "
                                            + "WHERE reservations.GiftID = @gid AND reservations.UserID = @uid "
                                        + ");";
                        cmd.Parameters.AddWithValue("@gid", gift.GiftId);
                        cmd.Parameters.AddWithValue("@uid", UserId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            // NOTE: In all Get*, this is viewer!
            // In other words, Get* will get all * owned by target and viewable by this
            public List<Gift> GetGifts(User target)
            {
                List<Gift> gifts = new List<Gift>();
                // get all gifts owned by target and that have a record in groups_gifts
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    foreach (Group group in GetGroups(target))
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT groups_gifts.GiftID "
                                            + "FROM groups_gifts "
                                            + "INNER JOIN gifts ON gifts.GiftID = groups_gifts.GiftID "
                                            + "WHERE GroupID = @gid "
                                            + "AND gifts.UserID = @uid;";
                            cmd.Parameters.AddWithValue("@gid", group.GroupId);
                            cmd.Parameters.AddWithValue("@uid", target.UserId);
                            cmd.Prepare();
                            using (MySqlDataReader Reader = cmd.ExecuteReader())
                            {
                                while (Reader.Read())
                                {
                                    gifts.Add(new Gift(Convert.ToUInt64(Reader["GiftID"])));
                                }
                            }
                        }
                    }
                }
                return gifts;
            }
            public List<Group> GetGroups(User target)
            {
                List<Group> groups = new List<Group>();
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT DISTINCT GroupID "
                                        + "FROM groups_users "
                                        + "WHERE UserID = @id2 "
                                        + "AND GroupID IN ( "
                                            + "SELECT DISTINCT GroupID "
                                            + "FROM groups_users "
                                            + "WHERE UserID = @id1 "
                                        + ");";
                        cmd.Parameters.AddWithValue("@id1", this.UserId);
                        cmd.Parameters.AddWithValue("@id2", target.UserId);
                        cmd.Prepare();
                        using (MySqlDataReader Reader = cmd.ExecuteReader())
                        {
                            while (Reader.Read())
                            {
                                groups.Add(new Group(Convert.ToUInt64(Reader["GroupID"])));
                            }
                        }
                    }
                }
                return groups;
            }
            public List<EventUser> GetEvents(User target)
            {
                List<EventUser> events = new List<EventUser>();
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    foreach (Group group in GetGroups(target))
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT events_users_groups.EventUserID "
                                            + "FROM events_users_groups "
                                            + "INNER JOIN events_users ON events_users.EventUserID = events_users_groups.EventUserID "
                                            + "WHERE GroupID = @gid "
                                            + "AND events_users.UserID = @uid;";
                            cmd.Parameters.AddWithValue("@gid", group.GroupId);
                            cmd.Parameters.AddWithValue("@uid", target.UserId);
                            cmd.Prepare();
                            using (MySqlDataReader Reader = cmd.ExecuteReader())
                            {
                                while (Reader.Read())
                                {
                                    events.Add(new EventUser(Convert.ToUInt64(Reader["EventUserID"])));
                                }
                            }
                        }
                    }
                }
                return events;
            }
            public XmlDocument Fetch()
            {
                XmlDocument info = new XmlDocument();
                XmlElement container = info.CreateElement("user");
                info.AppendChild(container);

                XmlElement id = info.CreateElement("userId");
                id.InnerText = UserId.ToString();
                XmlElement userName = info.CreateElement("userName");
                userName.InnerText = (UserName);
                XmlElement email = info.CreateElement("email");
                email.InnerText = Email.Address;
                // XmlElement password = Password.Fetch().DocumentElement;
                XmlElement birthMonth = info.CreateElement("birthMonth");
                birthMonth.InnerText = BirthMonth.ToString();
                XmlElement birthDay = info.CreateElement("birthDay");
                birthDay.InnerText = BirthDay.ToString();
                XmlElement bio = info.CreateElement("bio");
                bio.InnerText = Bio;
                XmlElement dateJoined = info.CreateElement("dateJoined");
                dateJoined.InnerText = (DateJoined.ToString("yyyy-MM-dd"));
                XmlElement groups = info.CreateElement("groups");
                foreach (Group group in Groups)
                {
                    groups.AppendChild(info.ImportNode(group.Fetch().DocumentElement, true));
                }
                XmlElement events = info.CreateElement("events");
                foreach (EventUser evnt in Events)
                {
                    events.AppendChild(info.ImportNode(evnt.Fetch().DocumentElement, true));
                }
                XmlElement gifts = info.CreateElement("gifts");
                foreach (Gift gift in Gifts)
                {
                    gifts.AppendChild(info.ImportNode(gift.Fetch().DocumentElement, true));
                }
                XmlElement preferences = info.CreateElement("preferences");
                preferences.AppendChild(info.ImportNode(Preferences.Fetch().DocumentElement, true));

                container.AppendChild(id);
                container.AppendChild(userName);
                container.AppendChild(email);
                container.AppendChild(birthMonth);
                container.AppendChild(birthDay);
                container.AppendChild(bio);
                container.AppendChild(dateJoined);
                container.AppendChild(groups);
                container.AppendChild(gifts);
                container.AppendChild(events);
                container.AppendChild(preferences);

                return info;
            }
        }
    }
}