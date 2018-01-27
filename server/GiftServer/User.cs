using System;
using System.IO;
using System.Xml;
using System.Net.Mail;
using System.Net;
using System.Collections.Generic;
using System.Configuration;
using GiftServer.Properties;
using GiftServer.Security;
using GiftServer.Exceptions;
using GiftServer.HtmlManager;
using MySql.Data.MySqlClient;
using GiftServer.Server;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// A single user of The GiftHub
        /// </summary>
        /// <remarks>
        /// A User is the _center_ of all operations within The GiftHub. Except for very limited circumstances, a User is required to do anything.
        /// As it's the center of any operation, the User class is quite complex, though not a "God Object" - it does not keep track of state.
        /// Note that certain methods are "lazy" - they won't populate until requested. This is done to save on unnecessary execution, and to ensure
        /// that the most recent changes are reflected.
        /// 
        /// The User class implements the ISynchronizable, IShowable, and IFetchable interfaces, which means:
        ///     - It can be changed, created, or deleted from the database
        ///     - It has an associated image
        ///     - It can be "serialized" as an XML Document
        /// </remarks>
        public class User : ISynchronizable, IShowable, IFetchable
        {
            /// <summary>
            /// The UserID for this user
            /// </summary>
            /// <remarks>
            /// If this is 0, this is a "dead" user - an invalid user. No operations should be made on such a user,
            /// though an operation will NOT throw an exception if the ID is 0.
            /// </remarks>
            public ulong UserId
            {
                get;
                private set;
            } = 0;
            /// <summary>
            /// The User's Name
            /// </summary>
            /// <remarks>
            /// Can contain any character, including emojis. There is no "First, Last" fields, to aid localization
            /// </remarks>
            public string UserName = "";
            /// <summary>
            /// The Email Address for this user
            /// </summary>
            /// <remarks>
            /// This address MUST be unique, since it's used as the Unique identifier (aside from the UserID, of course)
            /// </remarks>
            public MailAddress Email;
            /// <summary>
            /// The Password for this user.
            /// </summary>
            /// <remarks>
            /// This does NOT expose the user's actual password - this password is unavailable,
            /// since salting and hashing is done to prevent possible password leaks. However, one 
            /// CAN access the Salt, Hash, and number of iterations used via the Password, and can 
            /// validate a given password against this property.
            /// </remarks>
            public Password Password;
            /// <summary>
            /// The birth month - we don't store their year of birth.
            /// </summary>
            public int BirthMonth = 0;
            /// <summary>
            /// The birth day - we don't store their year of birth.
            /// </summary>
            public int BirthDay = 0;
            /// <summary>
            /// The user's "bio"
            /// </summary>
            /// <remarks>
            /// This field accepts any non-null value.
            /// </remarks>
            public string Bio = "";
            /// <summary>
            /// The user's preferences
            /// </summary>
            public Preferences Preferences;
            /// <summary>
            /// The UserURL
            /// </summary>
            /// <remarks>
            /// The URL is a Unique Identifier that, without access to the database, cannot be "reinterpreted" into anything about the user.
            /// In fact, this URL is generated at User Creation time without using _any_ user information. Indeed, not only is this locator 
            /// randomly generated, it is also cryptographically strong, though at this time that is certainly not necessary.
            /// 
            /// By definition, any character within this URL does not need to be escaped in any known HTML protocol. Furthermore, it 
            /// is gauranteed to be unique among all users.
            /// </remarks>
            public string UserUrl = "";
            /// <summary>
            /// The date this user created his or her account.
            /// </summary>
            public DateTime DateJoined
            {
                get;
                private set;
            }
            private string googleId = null;
            /// <summary>
            /// The Unique GoogleID for this user.
            /// </summary>
            /// <remarks>
            /// If the user has never signed in with Google, this will be null.
            /// 
            /// See GoogleUser for more information
            /// </remarks>
            public string GoogleId
            {
                get
                {
                    return googleId;
                }
                set
                {
                    googleId = String.IsNullOrEmpty(value) ? null : value;
                }
            }
            private string facebookId = null;
            /// <summary>
            /// The Unique FacebookID for this user.
            /// </summary>
            /// <remarks>
            /// If the user has never signed in with Facebook, this will be null.
            /// 
            /// See GoogleUser for more information
            /// </remarks>
            public string FacebookId
            {
                get
                {
                    return facebookId;
                }
                set
                {
                    facebookId = String.IsNullOrEmpty(value) ? null : value;
                }
            }
            /// <summary>
            /// The user's gifts
            /// </summary>
            /// <remarks>
            /// This is a lazy operator; it will only populate when called upon. This ensures the latest data is fetched.
            /// </remarks>
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
            /// <summary>
            /// The user's groups
            /// </summary>
            /// <remarks>
            /// This is a lazy operator; it will only populate when called upon. This ensures the latest data is fetched.
            /// </remarks>
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
            /// <summary>
            /// The user's events
            /// </summary>
            /// <remarks>
            /// This is a lazy operator; it will only populate when called upon. This ensures the latest data is fetched.
            /// </remarks>
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
            /// <summary>
            /// Initializes a new User
            /// </summary>
            /// <param name="id">The UserID</param>
            /// <remarks>
            /// This assumes that the ID exists; if it doesn't, a UserNotFoundException is thrown.
            /// </remarks>
            public User(ulong id)
            {
                FetchInformation(id);
            }
            /// <summary>
            /// Initializes a new User
            /// </summary>
            /// <param name="email">The User's Email</param>
            /// <remarks>
            /// This assumes a user with this email already exists; failure to find one results in a UserNotFoundException
            /// </remarks>
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
                            else
                            {
                                throw new UserNotFoundException(email);
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Initializes a new User
            /// </summary>
            /// <param name="hash">The UserURL</param>
            /// <remarks>
            /// The URL for this user, if this URL isn't found, a UserNotFoundException is thrown
            /// </remarks>
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
                            else
                            {
                                throw new UserNotFoundException(hash);
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Initialize a user from an OAuth Sign In
            /// </summary>
            /// <param name="user">The OAuthUser (created from the authentication token)</param>
            /// <param name="sender">A function that will send the reset email to a specified MailAddress</param>
            public User(OAuthUser user, Action<MailAddress> sender)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        switch (user)
                        {
                            case GoogleUser g:
                                cmd.CommandText = "SELECT users.UserID FROM users WHERE users.UserGoogleID = @oid;";
                                break;
                            case FacebookUser f:
                                cmd.CommandText = "SELECT users.UserID FROM users WHERE users.UserFacebookID = @oid;";
                                break;
                            default:
                                throw new ArgumentException("Unkown OAuth type: " + nameof(user));
                        }
                        cmd.Parameters.AddWithValue("@oid", user.OAuthId);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // User exists; fetch normally and move on
                                FetchInformation(Convert.ToUInt64(reader["UserID"]));
                                Update(user);
                            }
                            else
                            {
                                // We have a new user - check and see if email is already here. If so, just update that user's info!
                                if (!Search(user))
                                {
                                    // We have a new user; copy over information and CREATE()
                                    Create(user);
                                    // Send password reset email
                                    sender(Email);
                                }
                            }
                        }
                    }
                }
            }
            private bool Search(OAuthUser user)
            {
                // Search for user via email - if found, add OAuthID and return true; otherwise, false
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT users.UserID FROM users WHERE users.UserEmail = @eml;";
                        cmd.Parameters.AddWithValue("@eml", user.Email);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // If found, get id:
                            if (reader.Read())
                            {
                                ulong uid = Convert.ToUInt64(reader["UserID"]);
                                FetchInformation(uid);
                                switch (user)
                                {
                                    case GoogleUser g:
                                        GoogleId = g.OAuthId;
                                        break;
                                    case FacebookUser f:
                                        FacebookId = f.OAuthId;
                                        break;
                                    default:
                                        throw new ArgumentException("Unkown OAuth type: " + nameof(user));
                                }
                                return Update();
                            }
                            else
                            {
                                // Not found - false!
                                return false;
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Fetches a user with the specified email and password
            /// </summary>
            /// <param name="email">The user's email</param>
            /// <param name="password">The plaintext form of the user's password</param>
            /// <remarks>
            /// Even if the email is found, if the veracity of the password cannot be found,
            /// this will throw an InvalidPasswordException
            /// </remarks>
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
                                throw new UserNotFoundException(email);
                            }
                            else
                            {
                                Password = new Password(Convert.ToString(Reader["PasswordHash"]),
                                                             Convert.ToString(Reader["PasswordSalt"]),
                                                             Convert.ToInt32(Reader["PasswordIter"]));
                                // Check password
                                if (!Password.Verify(password))
                                {
                                    throw new InvalidPasswordException();
                                }
                                UserId = Convert.ToUInt64(Reader["UserID"]);
                                UserName = Convert.ToString(Reader["UserName"]);
                                Email = email;
                                BirthDay = Convert.ToInt32(Reader["UserBirthDay"]);
                                BirthMonth = Convert.ToInt32(Reader["UserBirthMonth"]);
                                DateJoined = (DateTime)(Reader["TimeCreated"]);
                                Bio = Convert.ToString(Reader["UserBio"]);
                                UserUrl = Convert.ToString(Reader["UserURL"]);
                                Preferences = new Preferences(this);
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Creates a *new* user with email and password
            /// </summary>
            /// <param name="email">The user's email</param>
            /// <param name="password">The user's password, *_hashed_*</param>
            public User(MailAddress email, Password password)
            {
                Email = email;
                Password = password;
            }
            /// <summary>
            /// Resets their password
            /// </summary>
            /// <param name="password">The new password</param>
            /// <param name="ResetManager">A ResetManager that will generate the notification for this user.</param>
            /// <returns>A status of whether or not it successfully reset the user's password</returns>
            public bool UpdatePassword(string password, ResetManager ResetManager)
            {
                if (UserId == 0)
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
                        cmd.CommandText = "UPDATE passwords SET PasswordHash = @hsh, PasswordSalt = @slt, PasswordIter = @itr WHERE UserID = @uid;";
                        cmd.Parameters.AddWithValue("@hsh", Password.Hash);
                        cmd.Parameters.AddWithValue("@slt", Password.Salt);
                        cmd.Parameters.AddWithValue("@itr", Password.Iterations);
                        cmd.Parameters.AddWithValue("@uid", UserId);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
                // Send email
                try
                {
                    MailMessage email = new MailMessage(new MailAddress("The Gift Hub<support@TheGiftHub.org>"), Email)
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
                        sender.Credentials = new NetworkCredential("support@thegifthub.org", Constants.EmailPassword);
                        sender.Send(email);
                    }
                }
                catch (SmtpException)
                {
                    // Silenced - nothing we can do here and, frankly, nothing to tell the user...
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
                                UserId = id;
                                UserName = Convert.ToString(Reader["UserName"]);
                                Email = new MailAddress(Convert.ToString(Reader["UserEmail"]));
                                Password = new Password(Convert.ToString(Reader["PasswordHash"]),
                                                        Convert.ToString(Reader["PasswordSalt"]),
                                                        Convert.ToInt32(Reader["PasswordIter"]));
                                BirthDay = Convert.ToInt32(Reader["UserBirthDay"]);
                                BirthMonth = Convert.ToInt32(Reader["UserBirthMonth"]);
                                DateJoined = (DateTime)(Reader["TimeCreated"]);
                                Bio = Convert.ToString(Reader["UserBio"]);
                                UserUrl = Convert.ToString(Reader["UserURL"]);
                                GoogleId = Convert.ToString(Reader["UserGoogleID"]);
                                FacebookId = Convert.ToString(Reader["UserFacebookID"]);

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
            /// <summary>
            /// Creates the user in the database.
            /// </summary>
            /// <remarks>
            /// If the email is already taken, this will throw a DuplicatUserException
            /// </remarks>
            /// <returns>A status of success or failure</returns>
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
                        cmd.Parameters.AddWithValue("@email", Email.Address);
                        cmd.Prepare();
                        using (MySqlDataReader Reader = cmd.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                // User already exists; throw exception:
                                throw new DuplicateUserException(Email);
                            }
                        }
                    }
                    bool isUnique = false;
                    while (!isUnique)
                    {
                        Password url = new Password(Email.Address);
                        UserUrl = url.Hash.Replace("+", "0").Replace("/", "A").Replace("=", "R");
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "SELECT users.UserID FROM users WHERE users.UserURL = @url;";
                            cmd.Parameters.AddWithValue("@url", UserUrl);
                            cmd.Prepare();
                            using (MySqlDataReader Reader = cmd.ExecuteReader())
                            {
                                isUnique = !Reader.HasRows;
                            }
                        }
                    }
                    // Look and see
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO users (UserName, UserEmail, UserBirthMonth, UserBirthDay, UserBio, UserURL, UserGoogleID, userFacebookID) "
                            + "VALUES (@name, @email, @bmonth, @bday, @bio, @url, @gid, @fid);";
                        cmd.Parameters.AddWithValue("@name", UserName);
                        cmd.Parameters.AddWithValue("@email", Email);
                        cmd.Parameters.AddWithValue("@bmonth", BirthMonth);
                        cmd.Parameters.AddWithValue("@bday", BirthDay);
                        cmd.Parameters.AddWithValue("@bio", Bio);
                        cmd.Parameters.AddWithValue("@url", UserUrl);
                        cmd.Parameters.AddWithValue("@gid", GoogleId);
                        cmd.Parameters.AddWithValue("@fid", FacebookId);
                        cmd.Prepare();
                        if (cmd.ExecuteNonQuery() == 0)
                        {
                            return false;
                        }
                        UserId = Convert.ToUInt64(cmd.LastInsertedId);
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        // Create new password:
                        cmd.CommandText = "INSERT INTO passwords (UserID, PasswordHash, PasswordSalt, PasswordIter) VALUES (@uid, @hsh, @slt, @itr);";
                        cmd.Parameters.AddWithValue("@uid", UserId);
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
                        cmd.Parameters.AddWithValue("@id", UserId);
                        cmd.Prepare();
                        using (MySqlDataReader Reader = cmd.ExecuteReader())
                        {
                            while (Reader.Read())
                            {
                                DateJoined = (DateTime)(Reader["TimeCreated"]);
                            }
                        }
                    }
                    Preferences = new Preferences(this);
                    Preferences.Create();
                }
                return true;
            }
            private void Create(OAuthUser info)
            {
                Email = info.Email;
                UserName = info.Name;

                switch (info)
                {
                    case GoogleUser g:
                        GoogleId = g.OAuthId;
                        break;
                    case FacebookUser f:
                        FacebookId = f.OAuthId;
                        break;
                    default:
                        throw new ArgumentException("Unkown OAuth type: " + nameof(info));
                }
                Password = new Password(info.OAuthId);
                Create();
                // Change locale and update:
                // Find nearest locale, update with that:
                Preferences.Culture = Controller.ParseCulture(info.Locale);
                // Save image:
                SaveImage(info.Picture);
            }
            /// <summary>
            /// Updates the user
            /// </summary>
            /// <remarks>
            /// If the ID is 0, this will instead _create_ a new user
            /// </remarks>
            /// <returns>A status of the update process</returns>
            public bool Update()
            {
                if (UserId == 0)
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
                        cmd.Parameters.AddWithValue("@email", Email);
                        cmd.Parameters.AddWithValue("@id", UserId);
                        cmd.Prepare();
                        using (MySqlDataReader Reader = cmd.ExecuteReader())
                        {
                            if (Reader.Read())
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
                            + "SET UserName = @name, "
                            + "UserEmail = @email, "
                            + "UserBio = @bio, "
                            + "UserBirthMonth = @bmonth, "
                            + "UserBirthDay = @bday, "
                            + "UserGoogleID = @gid, "
                            + "UserFacebookID = @fid "
                            + "WHERE UserID = @uid;";
                        cmd.Parameters.AddWithValue("@name", UserName);
                        cmd.Parameters.AddWithValue("@email", Email);
                        cmd.Parameters.AddWithValue("@bio", Bio);
                        cmd.Parameters.AddWithValue("@bmonth", BirthMonth);
                        cmd.Parameters.AddWithValue("@bday", BirthDay);
                        cmd.Parameters.AddWithValue("@gid", GoogleId);
                        cmd.Parameters.AddWithValue("@fid", FacebookId);
                        cmd.Parameters.AddWithValue("@uid", UserId);
                        cmd.Prepare();
                        return (cmd.ExecuteNonQuery() == 1);
                    }
                    // Only way to update password is through password reset, so no need here
                }
            }
            private void Update(OAuthUser user)
            {
                bool isChanged = false;
                if (user.Name != UserName)
                {
                    UserName = user.Name;
                    isChanged = true;
                }
                if (Controller.ParseCulture(user.Locale) != Preferences.Culture)
                {
                    Preferences.Culture = Controller.ParseCulture(user.Locale);
                    isChanged = true;
                }
                if (isChanged)
                {
                    Update();
                }
            }
            /// <summary>
            /// Deletes a user from the database
            /// </summary>
            /// <remarks>
            /// This is a permanent change, and will delete all memberships, gifts, and preferences.
            /// </remarks>
            /// <returns>A status flag</returns>
            public bool Delete()
            {
                // TODO: Gauruntee not admin of any group
                if (UserId == 0)
                {
                    // User doesn't exist - don't delete
                    return false;
                }
                else
                {
                    RemoveImage();
                    using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                    {
                        con.Open();
                        // Remove image:
                        RemoveImage();
                        // Remove preferences
                        Preferences.Delete();
                        // Delete from event futures, groups, and events:
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            // Get EventUserID:
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM events_users_futures WHERE EventUserID IN (SELECT EventUserID FROM events_users WHERE UserID = @id);";
                            cmd.Parameters.AddWithValue("@id", UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            // Get EventUserID:
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM events_users_groups WHERE EventUserID IN (SELECT EventUserID FROM events_users WHERE UserID = @id);";
                            cmd.Parameters.AddWithValue("@id", UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM events_users WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from purchases and reservations:
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM purchases WHERE ReservationID IN (SELECT ReservationID FROM reservations WHERE UserID = @id);";
                            cmd.Parameters.AddWithValue("@id", UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from reservations
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM reservations WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete gifts from receptions, groups, and gifts:
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM receptions WHERE GiftID IN (SELECT GiftID FROM gifts WHERE UserID = @id);";
                            cmd.Parameters.AddWithValue("@id", UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM groups_gifts WHERE GiftID IN (SELECT GiftID FROM gifts WHERE UserID = @id);";
                            cmd.Parameters.AddWithValue("@id", UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM gifts WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from groups_users
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM groups_users WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // No need to delete from groups; guaranteed to NOT be admin of any
                        // Delete from Passwordresets
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM passwordresets WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from preferences
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM users_preferences WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from passwords
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM passwords WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        // Delete from users
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "DELETE FROM users WHERE UserID = @id;";
                            cmd.Parameters.AddWithValue("@id", UserId);
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                        this.UserId = 0;
                        return true;
                    }
                }
            }
            /// <summary>
            /// Saves an image for this user.
            /// </summary>
            /// <param name="contents">The new image, regardless of original format</param>
            public void SaveImage(byte[] contents)
            {
                ImageProcessor processor = new ImageProcessor(contents);
                File.WriteAllBytes(Directory.GetCurrentDirectory() + "/resources/images/users/User" + UserId + Constants.ImageFormat, processor.Data);
            }
            /// <summary>
            /// Removes an associated image
            /// </summary>
            /// <remarks>
            /// The user's image will be replaced with default.png.
            /// </remarks>
            public void RemoveImage()
            {
                File.Delete(Directory.GetCurrentDirectory() + "/resources/images/users/User" + UserId + Constants.ImageFormat);
            }
            /// <summary>
            /// Returns the path for this user's image
            /// </summary>
            /// <returns>A qualified path for this user's image</returns>
            /// <remarks>
            /// The qualified path is with respect to the _server's_ root directory, which is *not necessarily 'C:\' or '/'*
            /// </remarks>
            public string GetImage()
            {
                return GetImage(UserId);
            }
            /// <summary>
            /// Gets the image for any user
            /// </summary>
            /// <param name="userID">The user ID for this given user</param>
            /// <returns>A qualified path for this user's image. See GetImage() for more information</returns>
            public static string GetImage(ulong userID)
            {
                // Build path:
                string path = Directory.GetCurrentDirectory() + "/resources/images/users/User" + userID + Constants.ImageFormat;
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
            /// <param name="gift">The gift to reserve</param>
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
                            left = reader.Read() && Convert.ToUInt32(reader["NumRes"]) < gift.Quantity;
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
                        throw new ReservationOverflowException(gift);
                    }
                }
            }

            /// <summary>
            /// Reserve |amount| of gift.
            /// </summary>
            /// <param name="gift">The gift to reserve</param>
            /// <param name="amount">The number of reservations to make</param>
            /// <returns>Number of successfully reserved gifts</returns>
            public int Reserve(Gift gift, int amount)
            {
                for (int c = 0; c < amount; c++)
                {
                    try
                    {
                        Reserve(gift);
                    }
                    catch (ReservationOverflowException)
                    {
                        return c;
                    }
                }
                return amount;
            }

            /// <summary>
            /// Release ONE of the gifts (if multiple are reserved)
            /// Does NOT release purchased gifts.
            /// </summary>
            /// <param name="gift">The gift to release</param>
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
            /// Release a specified amount of gifts.
            /// </summary>
            /// <param name="gift">The gift to release</param>
            /// <param name="amount">The number of releases to perform</param>
            /// <returns>The actual number of releases successfully performed</returns>
            public int Release(Gift gift, int amount)
            {
                for (int c = 0; c < amount; c++)
                {
                    Release(gift);
                }
                return amount;
            }

            /// <summary>
            /// Mark a gift as purchased
            /// </summary>
            /// <param name="gift">The gift to purchase</param>
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
            /// <param name="gift">The gift to be returned</param>
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
            /// <summary>
            /// Get all gifts viewable by this user
            /// </summary>
            /// <param name="target">The owner</param>
            /// <returns>The list of viewable gifts</returns>
            /// <remarks>
            /// Like other Get* operations, _this_ is considered the viewer, and _target_ is the owner.
            /// 
            /// In other words, GetGifts will get all gifts owned by the target &amp; viewable by this user. The target
            /// will have at least one group in common with this user, *and* have marked one gift viewable by said group
            /// for this to happen.
            /// </remarks>
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
            /// <summary>
            /// Get all groups viewable by this user
            /// </summary>
            /// <param name="target">The owner</param>
            /// <returns>The list of viewable groups</returns>
            /// <remarks>
            /// Like other Get* operations, _this_ is considered the viewer, and _target_ is the owner.
            /// 
            /// In other words, GetGroups will get all groups that target and this have in common.
            /// </remarks>
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
                        cmd.Parameters.AddWithValue("@id1", UserId);
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
            /// <summary>
            /// Get all events viewable by this user
            /// </summary>
            /// <param name="target">The owner</param>
            /// <returns>The list of viewable events</returns>
            /// <remarks>
            /// Like other Get* operations, _this_ is considered the viewer, and _target_ is the owner.
            /// 
            /// In other words, GetEvents will get all Events owned by the target &amp; viewable by this user. The target
            /// will have at least one group in common with this user, *and* have marked one Event viewable by said group
            /// for this to happen.
            /// </remarks>
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
            /// <summary>
            /// Add an OAuth token for this user
            /// </summary>
            /// <remarks>
            /// This will allow the user to login with said service
            /// </remarks>
            /// <param name="token">The OAuth token to add</param>
            public void AddOAuth(OAuthUser token)
            {
                // TODO: Ensure that login isn't already taken
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        switch (token)
                        {
                            case GoogleUser g:
                                cmd.CommandText = "SELECT users.UserID FROM users WHERE UserGoogleID = @oid;";
                                break;
                            case FacebookUser f:
                                cmd.CommandText = "SELECT users.UserID FROM users WHERE UserFacebookID = @oid;";
                                break;
                        }
                        cmd.Parameters.AddWithValue("@oid", token.OAuthId);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                // We are good to go! Nobody has this ID
                                switch (token)
                                {
                                    case GoogleUser g:
                                        GoogleId = g.OAuthId;
                                        break;
                                    case FacebookUser f:
                                        FacebookId = f.OAuthId;
                                        break;
                                }
                                Update();
                            }
                            else
                            {
                                throw new DuplicateUserException(token);
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Remove an OAuth token for this user
            /// </summary>
            /// <remarks>
            /// This will bar the user from loggin in with said service
            /// </remarks>
            /// <param name="token">The OAuth token to remove</param>
            public void RemoveOAuth(OAuthUser token)
            {
                switch (token)
                {
                    case GoogleUser g:
                        GoogleId = null;
                        break;
                    case FacebookUser f:
                        FacebookId = null;
                        break;
                }
                Update();
            }
            /// <summary>
            /// "Serializes" the User as an XML Document
            /// </summary>
            /// <returns>An XmlDocument with all information about this user enclosed</returns>
            /// <remarks>
            /// This is the "root" of all fetch operations; the User class is the only one allowed to 
            /// traverse &amp; _expand all of its elements_. While this sounds like a lot of data, since it's just textual information\
            /// (no images), the data sent is always quite minimal.
            /// 
            /// This method returns an XML Document with the following fields:
            ///     - userId: The user's ID
            ///     - userName: The user's name
            ///     - email: The User's email
            ///     - birthMonth: The User's birth month
            ///     - birthYear: The User's birth year
            ///     - bio: The User's biography
            ///     - dateJoined: The date this user joined, encoded as "yyyy-MM-dd"
            ///     - image: The qualified path for this user's image
            ///     - groups: An expanded list of all this user's groups.
            ///         - Note that each child of _groups_ is a _group_ element.
            ///     - gifts: An expanded list of all this user's gifts.
            ///         - Note that each child of _gifts_ is a _gift_ element.
            ///     - events: An expanded list of all this user's events.
            ///         - Note that each child of _events_ is an _event_ element.
            ///     - preferences: This user's preferences
            ///     
            /// This is all contained within a _user_ container.
            /// </remarks>
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
                XmlElement image = info.CreateElement("image");
                image.InnerText = GetImage();
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

                container.AppendChild(id);
                container.AppendChild(userName);
                container.AppendChild(email);
                container.AppendChild(birthMonth);
                container.AppendChild(birthDay);
                container.AppendChild(bio);
                container.AppendChild(dateJoined);
                container.AppendChild(image);
                container.AppendChild(groups);
                container.AppendChild(gifts);
                container.AppendChild(events);
                container.AppendChild(info.ImportNode(Preferences.Fetch().DocumentElement, true));

                return info;
            }
        }
    }
}