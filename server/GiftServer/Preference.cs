using System;
using System.Configuration;
using System.Xml;
using GiftServer.Server;
using MySql.Data.MySqlClient;
namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// User Preference
        /// </summary>
        public class Preference : ISynchronizable, IFetchable, IEquatable<Preference>
        {
            /// <summary>
            /// The ID for this set of preferences
            /// </summary>
            public ulong ID
            {
                get;
                private set;
            } = 0;
            /// <summary>
            /// The user these preferences apply to
            /// </summary>
            public User User;
            private string culture = "en-US";
            /// <summary>
            /// The culture for this user
            /// </summary>
            /// <remarks>
            /// To set this, you must submit non-null, 5 letters, and formatted as &lt;lang&gt;-&lt;COUNTRY&gt;
            /// </remarks>
            public string Culture
            {
                get
                {
                    return culture;
                }
                set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException(nameof(value), "Value must not be null");
                    }
                    else
                    {
                        // Try our best to parse the culture
                        culture = Controller.ParseCulture(value);
                    }
                }
            }
            /// <summary>
            /// Fetch preferences tied to a specific user
            /// </summary>
            /// <param name="user">The User to fetch</param>
            public Preference(User user)
            {
                // Try and get preferences
                User = user;
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT * FROM preferences WHERE preferences.UserID = @uid;";
                        cmd.Parameters.AddWithValue("@uid", user.ID);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // We have data!
                                ID = Convert.ToUInt64(reader["PreferenceID"]);
                                culture = Convert.ToString(reader["UserCulture"]);
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Fetch preferences from the database
            /// </summary>
            /// <param name="preferenceId">The ID to fetch</param>
            /// <remarks>
            /// This method arguably should not be called for the following reasons:
            /// - A Preference object is already fetched when a User is instantiated
            /// - A new preferences is created by creating a new User
            /// - This method also fetches the user, which is redundant.
            /// 
            /// In addition, all this does is fetch the *User* associated with this preference set, then extract the preferences
            /// </remarks>
            [Obsolete("Preferences should not be created from an ID, and should be created from the User class", true)]
            public Preference(ulong preferenceId)
            {
                // Try and get preferences
                ID = preferenceId;
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT UserID FROM preferences WHERE preferences.PreferenceID = @pid;";
                        cmd.Parameters.AddWithValue("@pid", preferenceId);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // We have data!
                                User = new User(Convert.ToUInt64(reader["UserID"]));
                                ID = User.Preferences.ID;
                                culture = User.Preferences.Culture;
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Create a record for this set of preferences in the database
            /// </summary>
            public void Create()
            {
                if (User.ID == 0)
                {
                    throw new InvalidOperationException("User must not be ID-less!");
                }
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    // Check we don't already exist - if we do, just Update instead
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT PreferenceID FROM preferences WHERE UserID = @uid;";
                        cmd.Parameters.AddWithValue("@uid", User.ID);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                ID = Convert.ToUInt64(reader["PreferenceID"]);
                                Update();
                                return;
                            }
                        }
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO preferences (UserID, UserCulture) "
                                        + "VALUES (@uid, @clt);";
                        cmd.Parameters.AddWithValue("@uid", User.ID);
                        cmd.Parameters.AddWithValue("@clt", culture);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        ID = Convert.ToUInt64(cmd.LastInsertedId);
                    }
                }
            }
            /// <summary>
            /// Update existing preferences
            /// </summary>
            public void Update()
            {
                if (User.ID == 0)
                {
                    throw new InvalidOperationException("User must not be ID-less!");
                }
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    // Check we already exist - if we don't, just Create instead
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT PreferenceID FROM preferences WHERE UserID = @uid;";
                        cmd.Parameters.AddWithValue("@uid", User.ID);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                Create();
                                return;
                            }
                        }
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "UPDATE preferences "
                                        + "SET UserCulture = @clt "
                                        + "WHERE PreferenceID = @pid;";
                        cmd.Parameters.AddWithValue("@clt", culture);
                        cmd.Parameters.AddWithValue("@pid", ID);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            /// <summary>
            /// Delete these preferences
            /// </summary>
            public void Delete()
            {
                if (User.ID == 0)
                {
                    throw new InvalidOperationException("User must not be ID-less!");
                }
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM preferences WHERE PreferenceID = @pid;";
                        cmd.Parameters.AddWithValue("@pid", ID);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        ID = 0;
                    }
                }
            }
            /// <summary>
            /// See if the given object is actually this preferences set
            /// </summary>
            /// <param name="obj">The object to check</param>
            /// <returns>If the two are equivalent</returns>
            public override bool Equals(object obj)
            {
                if (obj != null && obj is Preference p)
                {
                    return Equals(p);
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// Check if a given preference is the same as this one
            /// </summary>
            /// <param name="prefs">The preference to compare</param>
            /// <returns>Whether or not they are equal</returns>
            public bool Equals(Preference prefs)
            {
                return prefs != null && prefs.ID == ID;
            }
            /// <summary>
            /// The hash code for these preferences
            /// </summary>
            /// <returns>The hash code</returns>
            public override int GetHashCode()
            {
                return ID.GetHashCode();
            }
            /// <summary>
            /// Serializes the Preference as an XML Document
            /// </summary>
            /// <remarks>
            /// This XML Document has the following fields:
            ///     - preferenceId: The PreferenceID for this set of User Preference
            ///     - culture: The preferred culture for this user
            ///     
            /// This is all wrapped in a preferences container
            /// </remarks>
            /// <returns></returns>
            public XmlDocument Fetch()
            {
                if (ID == 0 || User.ID == 0)
                {
                    throw new InvalidOperationException("Cannot fetch preferences without ID");
                }
                XmlDocument info = new XmlDocument();
                XmlElement container = info.CreateElement("preferences");
                info.AppendChild(container);
                XmlElement id = info.CreateElement("preferenceId");
                id.InnerText = ID.ToString();
                XmlElement userCulture = info.CreateElement("culture");
                userCulture.InnerText = culture;

                container.AppendChild(id);
                container.AppendChild(userCulture);

                return info;
            }
            /// <summary>
            /// Serialize the preferences
            /// </summary>
            /// <param name="viewer">The viewer - must be equal to this user</param>
            /// <returns>The serialization</returns>
            public XmlDocument Fetch(User viewer)
            {
                if (ID == 0 || User.ID == 0)
                {
                    throw new InvalidOperationException("Cannot fetch preferences without ID");
                }
                else if (viewer == null)
                {
                    throw new ArgumentNullException(nameof(viewer), "Viewer cannot be null");
                }
                else if (viewer.ID == 0)
                {
                    throw new ArgumentException("Viewer must be valid", nameof(viewer));
                }
                if (viewer.Equals(User))
                {
                    return Fetch();
                }
                else
                {
                    XmlDocument info = new XmlDocument();
                    XmlElement container = info.CreateElement("preferences");
                    info.AppendChild(container);
                    return info;
                }
            }
        }
    }
}
