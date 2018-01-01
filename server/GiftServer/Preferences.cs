using System;
using System.Configuration;
using System.Web;
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
            private string culture = "en-US";
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
                                this.culture = Convert.ToString(Reader["UserCulture"]);
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
                                this.culture = Convert.ToString(Reader["UserCulture"]);
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
                        cmd.CommandText = "INSERT INTO preferences (UserID, UserCulture, UserTheme) "
                                        + "VALUES (@uid, @clt, @thm);";
                        cmd.Parameters.AddWithValue("@uid", User.UserId);
                        cmd.Parameters.AddWithValue("@clt", this.culture);
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
                                        + "SET UserCulture = @clt, "
                                        + "UserTheme = @thm "
                                        + "WHERE PreferenceID = @pid;";
                        cmd.Parameters.AddWithValue("@clt", this.culture);
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
                XmlDocument info = new XmlDocument();
                XmlElement container = info.CreateElement("preferences");
                info.AppendChild(container);
                XmlElement id = info.CreateElement("preferenceId");
                id.InnerText = PreferenceId.ToString();
                XmlElement userTheme = info.CreateElement("theme");
                userTheme.InnerText = theme.ToString();
                XmlElement userCulture = info.CreateElement("culture");
                userCulture.InnerText = culture;

                container.AppendChild(id);
                container.AppendChild(userTheme);
                container.AppendChild(userCulture);

                return info;
            }
        }
    }
}
