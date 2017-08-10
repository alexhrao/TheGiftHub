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
                this._ctx = ctx;
                this._request = ctx.Request;
                this._response = ctx.Response;
                this._user = GetUser();
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
                                            // TODO: Save Gift
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
                                            _response.Cookies.Add(new Cookie
                                            {
                                                Name = "UserID",
                                                Value = "-1",
                                                Expires = DateTime.Now.AddDays(-1d)
                                            });
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
                        else if (path.Length != 0)
                        {
                            return ServeResource(path);
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
                        return ListManager.MyList(_user);
                    default:
                        return DashboardManager.Dashboard(_user);
                }
            }

            private string ServeResource(string path)
            {
                path = GeneratePath(path);
                // Check existence:
                if (File.Exists(path))
                {
                    // File exists: Check if filename even needs authentication:
                    if (Path.GetFileName(Path.GetDirectoryName(path)).Equals("users"))
                    {
                        if (_user != null && Path.GetFileNameWithoutExtension(path).Equals("User" + _user.UserId))
                        {
                            byte[] buffer = File.ReadAllBytes(path);
                            _response.ContentLength64 = buffer.Length;
                            using (Stream response = _response.OutputStream)
                            {
                                response.Write(buffer, 0, buffer.Length);
                            }
                        }
                    }
                    else if (Path.GetFileName(Path.GetDirectoryName(path)).Equals("gifts"))
                    {
                        // Get gift ID, see if any gifts attached to user has gift id
                        long giftID = Convert.ToInt64(Path.GetFileNameWithoutExtension(path).Substring(4));
                        using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                        {
                            con.Open();
                            using (MySqlCommand cmd = new MySqlCommand())
                            {
                                cmd.Connection = con;
                                cmd.CommandText = "SELECT GiftID FROM gifts WHERE UserID = @id;";
                                cmd.Parameters.AddWithValue("@id", _user.UserId);
                                cmd.Prepare();
                                using (MySqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        // OK; serve up image
                                        byte[] buffer = File.ReadAllBytes(path);
                                        _response.ContentLength64 = buffer.Length;
                                        using (Stream response = _response.OutputStream)
                                        {
                                            response.Write(buffer, 0, buffer.Length);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // Not accessing images or gifts, so OK to just send info:
                        byte[] buffer = File.ReadAllBytes(path);
                        _response.ContentLength64 = buffer.Length;
                        using (Stream response = _response.OutputStream)
                        {
                            response.Write(buffer, 0, buffer.Length);
                        }
#if DEBUG
                        Console.WriteLine("Resource at " + path + " Does not need authentication and was served.");
#endif
                    }
                }
                return null;
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
                        // TODO: Invalidate cookie
                        return LoginManager.Login();
                        // Return so that 
                        // will return HERE so as to not update a null user
                    default:
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
                        return "404";
                }
                group.Update();
                return "200";
            }


            private static string GeneratePath(string uri)
            {
                return Resources.BasePath + uri;
            }
        }
    }
}
