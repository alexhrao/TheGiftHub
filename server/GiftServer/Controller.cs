using GiftServer.Data;
using GiftServer.Exceptions;
using GiftServer.Html;
using GiftServer.Properties;
using GiftServer.Security;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;

namespace GiftServer
{
    namespace Server
    {
        public class Controller
        {
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
                _user = GetUser();
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
                                            // Save user image:
                                            _user.SaveImage(parser);
                                            break;
                                        case "gift":
                                            Gift gift = new Gift(Convert.ToInt64(parser.Parameters["giftID"]));
                                            gift.SaveImage(parser);
                                            break;
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
                                            InvalidateCookie();
                                            return LoginManager.Login();
                                        case "Signup":
                                            _user = new User
                                            {
                                                FirstName = _dict["firstName"],
                                                LastName = _dict["lastName"],
                                                Email = _dict["email"],
                                                PasswordHash = _dict["password"]
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
                                            _user = new User(Convert.ToInt64(_dict["userID"]));
                                            string password = _dict["password"];
                                            PasswordReset.ResetPassword(_user, password);
                                            return ResetManager.SuccessResetPassword();
                                        case "UserChange":
                                            return UpdateUser();
                                        case "EventChange":
                                            return UpdateEvent();
                                        case "GroupChange":
                                            return UpdateGroup();
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
                                return ResetManager.CreateReset(PasswordReset.GetUser(_request.QueryString["ResetToken"]));
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
#if DEBUG
                            Console.WriteLine("Resource at " + path + " Does not need authentication and was served.");
#endif
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
#if DEBUG
                            Console.WriteLine("Resource at " + path + " Does not need authentication and was served.");
#endif
                        }
                        else if (_user != null)
                        {
                            // If GiftID and UserID match, we will be able to read; otherwise, no
                            // Get GID:
                            long gid = Convert.ToInt64(Path.GetFileNameWithoutExtension(path).Substring(4));
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
#if DEBUG
                        Console.WriteLine("Resource at " + path + " Does not need authentication and was served.");
#endif
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
                byte[] buffer = File.ReadAllBytes(path);
                _response.ContentLength64 = buffer.Length;
                using (Stream response = _response.OutputStream)
                {
                    response.Write(buffer, 0, buffer.Length);
                }
            }
            private User GetUser()
            {
                // Check if user is logged in (via cookies?)
                Cookie reqLogger = _request.Cookies["UserID"];
                if (reqLogger != null)
                {
                    return new User(Convert.ToInt64(reqLogger.Value));
                }
                else
                {
                    return null;
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
                    Cookie logger = new Cookie("UserID", Convert.ToString(_user.UserId));
                    _response.Cookies.Add(logger);
                    _response.AppendHeader("dest", "dashboard");
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

            private string UpdateUser()
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
                        InvalidateCookie();
                        return LoginManager.Login();
                        // Return so that 
                        // will return HERE so as to not update a null user
                    default:
                        _response.StatusCode = 404;
                        return "404";
                }
                _user.Update();
                return "200";
            }

            private string UpdateEvent()
            {
                EventUser evnt = new EventUser(Convert.ToInt64(_dict["eventID"]));
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

            private string UpdateGroup()
            {
                Group group = new Group(Convert.ToInt64(_dict["groupID"]));
                switch (_dict["item"])
                {
                    case "name":
                        break;
                    case "delete":
                        group.Delete();
                        return "200";
                    default:
                        _response.StatusCode = 404;
                        return "404";
                }
                group.Update();
                return "200";
            }

            private void InvalidateCookie()
            {
                _response.Cookies.Add(new Cookie
                {
                    Name = "UserID",
                    Value = "-1",
                    Expires = DateTime.Now.AddDays(-1d)
                });
            }


            private static string GeneratePath(string uri)
            {
                return Resources.BasePath + uri;
            }
        }
    }
}
