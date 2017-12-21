using System;
using System.Configuration;
using System.Xml;
using MySql.Data.MySqlClient;
namespace GiftServer
{
    namespace Data
    {
        public class Preferences : ISynchronizable, IFetchable
        {
            public ulong PreferenceId
            {
                get;
                private set;
            } = 0;
            public User User;
            private int theme = 0;
            public int Theme
            {
                get
                {
                    return theme;
                }
                set
                {
                    if (value > 5)
                    {
                        throw new ArgumentException("Value must be less than 6");
                    }
                    else
                    {
                        theme = value;
                    }
                }
            }
            private string language = "en";
            public string Language
            {
                get
                {
                    return language;
                }
                set
                {
                    if (value == null || value.Length != 2)
                    {
                        throw new ArgumentException("Value must be non-null, 2 letters long");
                    }
                    else
                    {
                        language = value;
                    }
                }
            }
            private string location = "US";
            public string Location
            {
                get
                {
                    return location;
                }
                set
                {
                    if (value == null || value.Length != 2)
                    {
                        throw new ArgumentException("Value must be non-null, 2 letters long");
                    }
                    else
                    {
                        location = value;
                    }
                }
            }

            public Preferences(User user)
            {
                // Try and get preferences
                this.User = user;
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
                                this.PreferenceId = Convert.ToUInt64(Reader["PreferenceID"]);
                                this.language = Convert.ToString(Reader["UserLanguage"]);
                                this.location = Convert.ToString(Reader["UserLocation"]);
                                this.theme = Convert.ToInt32(Reader["UserTheme"]);
                            }
                        }
                    }
                }
            }
            public Preferences(ulong preferenceId)
            {
                // Try and get preferences
                this.PreferenceId = preferenceId;
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT * FROM preferences WHERE preferences.PreferenceID = @pid;";
                        cmd.Parameters.AddWithValue("@pid", preferenceId);
                        cmd.Prepare();
                        using (MySqlDataReader Reader = cmd.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                // We have data!
                                User = new User(Convert.ToUInt64(Reader["UserID"]));
                                this.language = Convert.ToString(Reader["UserLanguage"]);
                                this.location = Convert.ToString(Reader["UserLocation"]);
                                this.theme = Convert.ToInt32(Reader["UserTheme"]);
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
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO preferences (UserID, UserLanguage, UserLocation, UserTheme) "
                                        + "VALUES (@uid, @lng, @loc, @thm);";
                        cmd.Parameters.AddWithValue("@uid", User.UserId);
                        cmd.Parameters.AddWithValue("@lng", language);
                        cmd.Parameters.AddWithValue("@loc", location);
                        cmd.Parameters.AddWithValue("@thm", theme);
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
            public bool Update()
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "UPDATE preferences "
                                        + "SET UserLanguage = @lng, "
                                        + "UserLocation = @loc, "
                                        + "UserTheme = @thm "
                                        + "WHERE PreferenceID = @pid;";
                        cmd.Parameters.AddWithValue("@lng", language);
                        cmd.Parameters.AddWithValue("@loc", location);
                        cmd.Parameters.AddWithValue("@thm", theme);
                        cmd.Parameters.AddWithValue("@pid", PreferenceId);
                        cmd.Prepare();
                        return cmd.ExecuteNonQuery() == 1;
                    }
                }
            }
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
            public XmlDocument Fetch()
            {
                return new XmlDocument();
            }
        }
    }
}
