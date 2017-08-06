using GiftServer.Data;
using GiftServer.Exceptions;
using GiftServer.Html;
using GiftServer.Properties;
using GiftServer.Security;
using System;
using System.Collections.Specialized;
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
            /// <returns>The html to be sent back to the user. Additionally, it will also alter the response, if necessary</returns>
            public string Dispatch()
            {
                {
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
                            NameValueCollection dict = HttpUtility.ParseQueryString(input);
                            if (dict["submit"] != null)
                            {
                                // Dispatch to correct logic:
                                switch (dict["submit"])
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
                                        _user = new User(dict["firstName"], dict["lastName"], dict["email"], dict["password"]);
                                        _user.Create();
                                        return LoginManager.SuccessSignup();
                                    case "Login":
                                        return Login(dict["email"], dict["password"]);
                                    case "PasswordResetRequest":
                                        // POST data will have user email. Send recovery email.
                                        PasswordReset.SendRecoveryEmail(dict["email"]);
                                        return ResetManager.ResetPasswordSent();
                                    case "PasswordReset":
                                        // Reset password and direct to login page
                                        // POST data will have userID in userID input. Reset the password and let the user know.
                                        long id = Convert.ToInt64(dict["userID"]);
                                        string password = dict["password"];
                                        PasswordReset.ResetPassword(id, password);
                                        return ResetManager.SuccessResetPassword();
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
                        return ServeResource(GeneratePath(path));
                    }
                    else
                    {
                        // If logged in (but no request), just send back home page:
                        return DashboardManager.Dashboard(_user);
                    }
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
                    default:
                        return DashboardManager.Dashboard(_user);
                }
            }

            private string ServeResource(string path)
            {
                // TODO: Check if user is allowed to view resource
                byte[] buffer = File.ReadAllBytes(path);
                _response.ContentLength64 = buffer.Length;
                using (Stream response = _response.OutputStream)
                {
                    response.Write(buffer, 0, buffer.Length);
                }
                return "";
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
                    Cookie logger = new Cookie("UserID", Convert.ToString(_user.Id));
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

            private static string GeneratePath(string uri)
            {
                return Resources.BasePath + uri;
            }
        }
    }
}
