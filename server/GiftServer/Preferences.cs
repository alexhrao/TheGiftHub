using System;
using System.Configuration;
using System.Xml;
using MySql.Data.MySqlClient;
namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// User Preferences
        /// </summary>
        public class Preferences : ISynchronizable, IFetchable, IEquatable<Preferences>
        {
            /// <summary>
            /// The ID for this set of preferences
            /// </summary>
            public ulong PreferenceId
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
                    if (value == null || value.Length != 5)
                    {
                        throw new ArgumentException("Value must be non-null, 5 letters long. Format: <lang>-<COUNTRY>");
                    }
                    else
                    {
                        culture = value;
                    }
                }
            }
            /// <summary>
            /// Fetch preferences tied to a specific user
            /// </summary>
            /// <param name="user">The User to fetch</param>
            public Preferences(User user)
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
                        cmd.Parameters.AddWithValue("@uid", user.UserId);
                        cmd.Prepare();
                        using (MySqlDataReader Reader = cmd.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                // We have data!
                                PreferenceId = Convert.ToUInt64(Reader["PreferenceID"]);
                                culture = Convert.ToString(Reader["UserCulture"]);
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
            /// - A Preferences object is already fetched when a User is instantiated
            /// - A new preferences is created by creating a new User
            /// - This method also fetches the user, which is redundant.
            /// 
            /// In addition, all this does is fetch the *User* associated with this preference set, then extract the preferences
            /// </remarks>
            public Preferences(ulong preferenceId)
            {
                // Try and get preferences
                PreferenceId = preferenceId;
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT UserID FROM preferences WHERE preferences.PreferenceID = @pid;";
                        cmd.Parameters.AddWithValue("@pid", preferenceId);
                        cmd.Prepare();
                        using (MySqlDataReader Reader = cmd.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                // We have data!
                                User = new User(Convert.ToUInt64(Reader["UserID"]));
                                PreferenceId = User.Preferences.PreferenceId;
                                culture = User.Preferences.Culture;
                            }
                        }
                    }
                }
            }
            /// <summary>
            /// Create a record for this set of preferences in the database
            /// </summary>
            /// <returns>A status flag</returns>
            public bool Create()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO preferences (UserID, UserCulture) "
                                        + "VALUES (@uid, @clt);";
                        cmd.Parameters.AddWithValue("@uid", User.UserId);
                        cmd.Parameters.AddWithValue("@clt", culture);
                        cmd.Prepare();
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            PreferenceId = Convert.ToUInt64(cmd.LastInsertedId);
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
            /// Update existing preferences
            /// </summary>
            /// <returns>A status flag</returns>
            public bool Update()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "UPDATE preferences "
                                        + "SET UserCulture = @clt "
                                        + "WHERE PreferenceID = @pid;";
                        cmd.Parameters.AddWithValue("@clt", culture);
                        cmd.Parameters.AddWithValue("@pid", PreferenceId);
                        cmd.Prepare();
                        return cmd.ExecuteNonQuery() == 1;
                    }
                }
            }
            /// <summary>
            /// Delete these preferences
            /// </summary>
            /// <returns>A status flag</returns>
            public bool Delete()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM preferences WHERE UserID = @uid;";
                        cmd.Parameters.AddWithValue("@uid", User.UserId);
                        cmd.Prepare();
                        if (cmd.ExecuteNonQuery() == 1)
                        {
                            PreferenceId = 0;
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
            /// See if the given object is actually this preferences set
            /// </summary>
            /// <param name="obj">The object to check</param>
            /// <returns>If the two are equivalent</returns>
            public override bool Equals(object obj)
            {
                if (obj != null && obj is Preferences p)
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
            public bool Equals(Preferences prefs)
            {
                return prefs != null && prefs.PreferenceId == PreferenceId;
            }
            /// <summary>
            /// The hash code for these preferences
            /// </summary>
            /// <returns>The hash code</returns>
            public override int GetHashCode()
            {
                return PreferenceId.GetHashCode();
            }
            /// <summary>
            /// Serializes the Preferences as an XML Document
            /// </summary>
            /// <remarks>
            /// This XML Document has the following fields:
            ///     - preferenceId: The PreferenceID for this set of User Preferences
            ///     - culture: The preferred culture for this user
            ///     
            /// This is all wrapped in a preferences container
            /// </remarks>
            /// <returns></returns>
            public XmlDocument Fetch()
            {
                XmlDocument info = new XmlDocument();
                XmlElement container = info.CreateElement("preferences");
                info.AppendChild(container);
                XmlElement id = info.CreateElement("preferenceId");
                id.InnerText = PreferenceId.ToString();
                XmlElement userCulture = info.CreateElement("culture");
                userCulture.InnerText = culture;

                container.AppendChild(id);
                container.AppendChild(userCulture);

                return info;
            }
        }
    }
}
