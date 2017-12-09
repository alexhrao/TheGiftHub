using GiftServer.Data;
using GiftServer.Exceptions;
using GiftServer.Html;
using GiftServer.Properties;
using GiftServer.Security;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;

namespace GiftServer
{
    namespace Server
    {
        public class Controller
        {
            public class Connection
            {
                public class UserInformation
                {
                    public readonly ulong UserId;
                    public readonly string Hash;
                    public UserInformation(ulong id)
                    {
                        UserId = id;
                        Hash = new Password(id.ToString("00000000")).Hash;
                    }
                }
                public UserInformation Info;
                public IPEndPointCollection Ends;
                public Connection(ulong userId)
                {
                    this.Info = new UserInformation(userId);
                    this.Ends = new IPEndPointCollection();
                }
            }
            public static readonly List<Connection> Connections = new List<Connection>();
            public static readonly List<Warning> Warnings = new List<Warning>();
            private User _user;
            private HttpListenerContext _ctx;
            private HttpListenerRequest _request;
            private HttpListenerResponse _response;
            private NameValueCollection _dict;
            public Controller(HttpListenerContext ctx)
            {
                _ctx = ctx;
                _request = ctx.Request;
                _response = ctx.Response;
                GetUser();
            }
            /// <summary>
            /// Dispatch will, given a request, return the webpage that will be shown to the user.
            /// </summary>
            /// <param name="request">The incoming HTML request, in it's entirety</param>
            /// <remarks>Dispatch is used to communicate with the server</remarks>
            /// <returns>The html to be sent back to the user. Additionally, it will also alter the response, if necessary</returns>
            public string Dispatch()
            {
                {
#if !DEBUG
                    try
                    {
#endif
                        string path = ParsePath();
                        if (_request.ContentType != null && _request.ContentType.Contains("multipart/form-data"))
                        {
                            MultipartParser parser = new MultipartParser(_request.InputStream, "image");
                            if (parser.Success)
                            {
                                // Image file will be saved in resources/images/users/User[UID].jpg
                                // Figure out which page user was on, engage.
                                if (_request.QueryString["dest"] != null)
                                {
                                    switch (_request.QueryString["dest"])
                                    {
                                        case "profile":
                                            {
                                                // Save user image:
                                                _user.SaveImage(parser);
                                                break;
                                            }
                                        case "myList":
                                            {
                                                Gift gift = new Gift(Convert.ToUInt64(parser.Parameters["itemid"]));
                                                gift.SaveImage(parser);
                                                break;
                                            }
                                    }
                                    return ParseQuery();
                                }
                                else
                                {
                                    // Just return dashboard
                                    return DashboardManager.Dashboard(_user);
                                }
                            }
                        }
                        if (_request.HasEntityBody)
                        {
                            string input;
                            // Read input, then dispatch accordingly
                            using (StreamReader reader = new StreamReader(_request.InputStream))
                            {
                                input = reader.ReadToEnd();
                                _dict = HttpUtility.ParseQueryString(input);
                                if (_dict["submit"] != null)
                                {
                                    // Dispatch to correct logic:
                                    switch (_dict["submit"])
                                    {
                                        case "Logout":
                                            Logout();
                                            return LoginManager.Login();
                                        case "Signup":
                                            _user = new User(_dict["email"], new Password(_dict["password"]))
                                            {
                                                FirstName = _dict["firstName"],
                                                LastName = _dict["lastName"],
                                            };
                                            _user.Create();
                                            return LoginManager.SuccessSignup();
                                        case "Login":
                                            return Login(_dict["email"], _dict["password"]);
                                        case "PasswordResetRequest":
                                            // POST data will have user email. Send recovery email.
                                            PasswordReset.SendRecoveryEmail(_dict["email"]);
                                            return ResetManager.ResetPasswordSent();
                                        case "PasswordReset":
                                            // Reset password and direct to login page
                                            // POST data will have userID in userID input. Reset the password and let the user know.
                                            _user = new User(Convert.ToUInt64(_dict["userID"]));
                                            string password = _dict["password"];
                                            _user.UpdatePassword(password);
                                            return ResetManager.SuccessResetPassword();
                                        case "Change":
                                            ulong changeId = Convert.ToUInt64(_dict["itemId"]);
                                            switch (_dict["type"])
                                            {
                                                case "User":
                                                    return Update();
                                                case "Event":
                                                    return Update(new EventUser(changeId));
                                                case "Group":
                                                    return Update(new Group(changeId));
                                                case "Gift":
                                                    return Update(new Gift(changeId));
                                                default:
                                                    return "";
                                            }
                                        case "Fetch":
                                            ulong fetchId = Convert.ToUInt64(_dict["itemId"]);
                                            IFetchable item = null;
                                            switch (_dict["type"])
                                            {
                                                case "Gift":
                                                    item = new Gift(fetchId);
                                                    break;
                                                case "User":
                                                    item = new User(fetchId);
                                                    break;
                                                case "Event":
                                                    item = new EventUser(fetchId);
                                                    break;
                                                case "Group":
                                                    item = new Group(fetchId);
                                                    break;
                                                default:
                                                    _response.StatusCode = 404;
                                                    return "Specified information not found";
                                            }
                                            return item.Fetch().OuterXml;
                                        default:
                                            return LoginManager.Login();
                                    }
                                }
                                else
                                {
                                    return LoginManager.Login();
                                }
                            }
                        }
                        else if (path.Length != 0)
                        {
                            return ServeResource(path);
                        }
                        else if (_user == null)
                        {
                            // Send login page EXCEPT if requesting password reset:
                            if (_request.QueryString["ResetToken"] != null)
                            {
                                try
                                {
                                    return ResetManager.CreateReset(PasswordReset.GetUser(_request.QueryString["ResetToken"]));
                                } catch (PasswordResetTimeoutException)
                                {
                                    return ResetManager.ResetFailed();
                                }

                            }
                            else
                            {
                                return LoginManager.Login();
                            }
                        }
                        else if (_request.QueryString["dest"] != null)
                        {
                            return ParseQuery();
                        }
                        else
                        {
                            // If logged in (but no request), just send back home page:
                            return DashboardManager.Dashboard(_user);
                        }
#if !DEBUG
                        // catch exceptions and return something meaningful
                    } catch (Exception)
                    {
                        return LoginManager.Login();
                    }
#endif
                }
            }

            private string ParseQuery()
            {
                string query = _request.QueryString["dest"];
                switch (query)
                {
                    case "dashboard":
                        return DashboardManager.Dashboard(_user);
                    case "profile":
                        return ProfileManager.ProfilePage(_user);
                    case "myList":
                        return ListManager.GiftList(_user);
                    default:
                        return DashboardManager.Dashboard(_user);
                }
            }

            private string ServeResource(string serverPath)
            {
                string message = null;
                string path = GeneratePath(serverPath);
                // Check existence:
                if (File.Exists(path))
                {
                    // File exists: Check if filename even needs authentication:
                    if (Path.GetFileName(Path.GetDirectoryName(path)).Equals("users"))
                    {
                        if (Path.GetFileNameWithoutExtension(path).Equals("default"))
                        {
                            // Serve up immediately
                            Write(path);
                            Warnings.Add(new PublicResourceWarning(path));
                        }
                        else if (_user != null)
                        {
                            if (Path.GetFileNameWithoutExtension(path).Equals("User" + _user.UserId))
                            {
                                // This user - write path
                                Write(path);
                            }
                            else
                            {
                                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                                {
                                    con.Open();
                                    using (MySqlCommand cmd = new MySqlCommand())
                                    {
                                        // See if our user (_user) and requested user (from string) have common groups
                                        // Subquery selects Groups tied to _user; 
                                        cmd.Connection = con;
                                        cmd.CommandText = "SELECT users.UserID "
                                                        + "FROM users "
                                                        + "INNER JOIN groups_users ON groups_users.UserID = users.UserID "
                                                        + "WHERE groups_users.UserID = @otherID "
                                                        + "AND groups_users.GroupID IN "
                                                        + "( "
                                                            + "SELECT GroupID FROM groups_users WHERE groups_users.UserID = @meID "
                                                        + ");";
                                        cmd.Parameters.AddWithValue("@meID", _user.UserId);
                                        cmd.Parameters.AddWithValue("@otherID", Path.GetFileNameWithoutExtension(path).Substring(4));
                                        cmd.Prepare();
                                        using (MySqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            if (reader.Read())
                                            {
                                                // connected
                                                Write(path);
                                            }
                                            else
                                            {
                                                _response.StatusCode = 403;
                                                message = "Forbidden - You are not in any common groups with this user.";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (Path.GetFileName(Path.GetDirectoryName(path)).Equals("gifts"))
                    {
                        // If default image is desired, serve up immediately:
                        if (Path.GetFileNameWithoutExtension(path).Equals("default"))
                        {
                            Write(path);
                            Warnings.Add(new PublicResourceWarning(path));
                        }
                        else if (_user != null)
                        {
                            // If GiftID and UserID match, we will be able to read; otherwise, no
                            // Get GID:
                            ulong gid = Convert.ToUInt64(Path.GetFileNameWithoutExtension(path).Substring(4));
                            if (_user.Gifts.Exists(new Predicate<Gift>((Gift g) => g.GiftId == gid)))
                            {
                                // Found in our own gifts; write
                                Write(path);
                            }
                            else
                            {
                                // See if tied to gift through groups_gifts
                                // Subquery selects groups tied to user:
                                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                                {
                                    con.Open();
                                    using (MySqlCommand cmd = new MySqlCommand())
                                    {
                                        cmd.Connection = con;
                                        cmd.CommandText = "SELECT GiftID "
                                                        + "FROM groups_gifts "
                                                        + "WHERE groups_gifts.GiftID = @gid "
                                                        + "AND groups_gifts.GroupID IN "
                                                        + "( "
                                                            + "SELECT GroupID FROM groups_users WHERE groups_users.UserID = @uid "
                                                        + ");";
                                        cmd.Parameters.AddWithValue("@gid", gid);
                                        cmd.Parameters.AddWithValue("@uid", _user.UserId);
                                        cmd.Prepare();
                                        using (MySqlDataReader reader = cmd.ExecuteReader())
                                        {
                                            if (reader.Read())
                                            {
                                                // Tied; write
                                                Write(path);
                                            }
                                            else
                                            {
                                                _response.StatusCode = 403;
                                                message = "Forbidden - this gift is not currently shared with you.";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // Not accessing images or gifts, so OK to just send info:
                        Write(path);
                        Warnings.Add(new PublicResourceWarning(path));
                    }
                }
                else if (Path.GetFileNameWithoutExtension(path).Equals("favicon"))
                {
                    Write(GeneratePath("/resources/images/branding/favicon.ico"));
                }
                else
                {
                    _response.StatusCode = 404;
                    message = "File Not Found: Unknown resource " + serverPath + ".";
                }
                return message;
            }

            private void Write(string path)
            {
                switch (Path.GetExtension(path))
                {
                    case "bm":
                    case "bmp":
                        _response.ContentType = "image/bmp";
                        break;
                    case "css":
                        _response.ContentType = "text/css";
                        break;
                    case "gif":
                        _response.ContentType = "image/gif";
                        break;
                    case "jpe":
                    case "jpeg":
                    case "jpg":
                        _response.ContentType = "image/jpeg";
                        break;
                    case "js":
                        _response.ContentType = "text/javascript";
                        break;
                    case "png":
                        _response.ContentType = "image/png";
                        break;

                    default:
                        break;
                }
                byte[] buffer = File.ReadAllBytes(path);
                _response.ContentLength64 = buffer.Length;
                using (Stream response = _response.OutputStream)
                {
                    response.Write(buffer, 0, buffer.Length);
                }
            }

            private void GetUser()
            {
                // Check if user is logged in (via cookies?)
                Cookie reqLogger = _request.Cookies["UserHash"];
                if (reqLogger != null)
                {
                    string hash = Convert.ToString(reqLogger.Value);
                    ulong id;
                    if ((id = GetLogged(hash)) != 0)
                    {
                        _user = new User(id);
                    }
                    else
                    {
                        _user = null;
                        Warnings.Add(new CookieNotInvalidWarning(hash));
                    }
                }
                else 
                {
                    _user = null;
                }
            }

            private string ParsePath()
            {
                string path = _request.RawUrl;
                if (path.Contains("?") || path.Length < 2)
                {
                    // there will be no img
                    path = "";
                }
                return path;
            }

            private string Login(string email, string password)
            {
                try
                {
                    _user = new User(email, password);
                    // Get hash
                    string hash = AddConnection(_user.UserId, _request.RemoteEndPoint);
                    Cookie logger = new Cookie("UserHash", hash);
                    _response.Cookies.Add(logger);
                    _response.AppendHeader("dest", "dashboard");
                    // If already logged in, just add remote end point:
                    return ParseQuery();
                }
                catch (InvalidPasswordException)
                {
                    return LoginManager.FailLogin();
                }
                catch (UserNotFoundException)
                {
                    return LoginManager.FailLogin();
                }
            }
            private string Update()
            {
                switch (_dict["item"])
                {
                    case "name":
                        // Update this user's name, then respond back with success:
                        _user.FirstName = _dict["firstName"];
                        _user.LastName = _dict["lastName"];
                        break;
                    case "email":
                        _user.Email = _dict["email"];
                        break;
                    case "birthday":
                        _user.BirthMonth = Convert.ToInt32(_dict["month"]);
                        _user.BirthDay = Convert.ToInt32(_dict["day"]);
                        break;
                    case "bio":
                        _user.Bio = _dict["bio"];
                        break;
                    case "delete":
                        _user.Delete();
                        Logout();
                        return LoginManager.Login();
                    // will return HERE so as to not update a null user
                    default:
                        _response.StatusCode = 404;
                        return "404";
                }
                _user.Update();
                return "200";
            }
            private string Update(EventUser evnt)
            {
                                switch (_dict["item"])
                {
                    case "name":
                        break;
                    case "delete":
                        evnt.Delete();
                        return "200";
                    default:
                        _response.StatusCode = 404;
                        return "404";
                }
                evnt.Update();
                return "200";
            }
            private string Update(Group group)
            {
                switch (_dict["item"])
                {
                    case "addUser":
                        {
                            // Get ID of user to add to this group
                            User added = new User(Convert.ToUInt64(_dict["userID"]));
                            group.Add(added);
                            break;
                        }
                    case "removeUser":
                        {
                            User removed = new User(Convert.ToUInt64(_dict["userID"]));
                            group.Remove(removed);
                            break;
                        }
                    case "removeMe":
                        {
                            group.Remove(_user);
                            break;
                        }
                    case "addEvent":
                        {
                            EventUser added = new EventUser(Convert.ToUInt64(_dict["eventID"]));
                            group.Add(added);
                            break;
                        }
                    case "removeEvent":
                        {
                            EventUser removed = new EventUser(Convert.ToUInt64(_dict["eventID"]));
                            group.Remove(removed);
                            break;
                        }
                    case "name":
                        {
                            break;
                        }
                    case "delete":
                        {
                            group.Delete();
                            return "200";
                        }
                    default:
                        {
                            _response.StatusCode = 404;
                            return "404";
                        }
                }
                group.Update();
                return "200";
            }
            private string Update(Gift gift)
            {
                if (_user != null)
                {
                    gift.Name = _dict["giftName"];
                    gift.Description = _dict["giftDescription"];
                    gift.Url = _dict["giftUrl"];
                    gift.Cost = Convert.ToDouble(_dict["giftCost"] == null || _dict["giftCost"].Length == 0 ? "0.00" : _dict["giftCost"]);
                    gift.Quantity = Convert.ToUInt32(_dict["giftQuantity"] == null || _dict["giftQuantity"].Length == 0 ? "1" : _dict["giftQuantity"]);
                    gift.Rating = Convert.ToDouble(_dict["giftRating"] == null || _dict["giftRating"].Length == 0 ? "0.0" : _dict["giftRating"]);
                    gift.ColorText = _dict["giftColorText"];
                    gift.Update();
                    return ListManager.GiftList(_user);
                }
                else
                {
                    return LoginManager.Login();
                }
            }

            private void Logout()
            {
                _response.Cookies.Add(new Cookie
                {
                    Name = "UserHash",
                    Value = "",
                    Expires = DateTime.Now.AddDays(-1d)
                });
                // If currently logged in, request will have cookie. See if cookie exists, and remove if so
                if (_request.Cookies["UserHash"] != null)
                {
                    RemoveConnection(_request.Cookies["UserHash"].Value);
                }
            }

            private bool IsLogged(string hash)
            {
                return Connections.Exists(new Predicate<Connection>((Connection con) =>
                {
                    return con.Info != null && hash.Equals(con.Info.Hash);
                }));
            }
            private ulong GetLogged(string hash)
            {
                ulong id = 0;
                Connections.Exists(new Predicate<Connection>((Connection con) =>
                {
                    if (con.Info != null && hash.Equals(con.Info.Hash))
                    {
                        id = con.Info.UserId;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }));
                return id;
            }

            private string AddConnection(ulong userId, IPEndPoint iPEndPoint)
            {
                string hash = "";
                if (!Connections.Exists(new Predicate<Connection>((Connection con) =>
                {
                    if (con.Info != null && userId.Equals(con.Info.UserId))
                    {
                        hash = con.Info.Hash;
                        con.Ends.Add(iPEndPoint);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                })))
                {
                    Connection con = new Connection(userId);
                    con.Ends.Add(iPEndPoint);
                    Connections.Add(con);
                    hash = con.Info.Hash;
                }
                return hash;
            }
            private void RemoveConnection(ulong userId)
            {
                Connections.RemoveAll(new Predicate<Connection>((Connection con) =>
                {
                    return con.Info.UserId == userId;
                }));
            }
            private void RemoveConnection(string hash)
            {
                Connections.RemoveAll(new Predicate<Connection>((Connection con) =>
                {
                    return hash.Equals(con.Info.Hash);
                }));
            }
            private static string GeneratePath(string uri)
            {
                return Directory.GetCurrentDirectory() + uri;
            }
        }
    }
}
