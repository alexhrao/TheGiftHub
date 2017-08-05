using System;
using System.Net;
using System.IO;
using System.Web;
using System.Collections.Specialized;
using GiftServer.Server;
using GiftServer.Data;
using GiftServer.Html;
using GiftServer.Security;
using GiftServer.Exceptions;
using GiftServer.Properties;

namespace GiftServer
{
    public class Program
    {
        /// <summary>
        /// Main method and entry point - starts the web server on the specified host/port.
        /// </summary>
        /// <param name="args">Reserved for future use</param>
        public static void Main(string[] args)
        {
            Server.WebServer server = new Server.WebServer(Resources.URL + "/", Dispatch);
            server.Run();
            Console.WriteLine("Server is Active...\nType quit or q to quit");
            string input = null;
            while ((input = Console.ReadLine()) != null)
            {
                if (string.Equals(input.ToLower(), "quit") || string.Equals(input.ToLower(), "q"))
                {
                    server.Stop();
                    return;
                }
            }
        }
        /// <summary>
        /// Dispatch will, given a request, return the webpage that will be shown to the user.
        /// </summary>
        /// <param name="request">The incoming HTML request, in it's entirety</param>
        /// <returns>The html to be sent back to the user. Additionally, it will also alter the response, if necessary</returns>
        public static string Dispatch(HttpListenerContext rtx)
        {
            // Check if user is logged in (via cookies?)
            HttpListenerRequest request = rtx.Request;
            HttpListenerResponse response = rtx.Response;
            Cookie reqLogger = request.Cookies["UserID"];
            // Get URI:
            string path = request.RawUrl;
            if (path.Contains("?") || path.Length < 2)
            {
                // there will be no img
                path = "";
            }
            User user = null;
            if (reqLogger != null)
            {
                user = new User(Convert.ToInt64(reqLogger.Value));
            }
            if (request.ContentType != null && request.ContentType.Contains("multipart/form-data"))
            {
                MultipartParser parser = new MultipartParser(request.InputStream, "image");
                if (parser.Success)
                {
                    // Image file will be saved in resources/images/users/User[UID].jpg
                    // Figure out which page user was on, engage.
                    if (request.QueryString["dest"] != null)
                    {
                        switch (request.QueryString["dest"])
                        {
                            case "dashboard":
                                return DashboardManager.Dashboard(user);
                            case "profile":
                                // Save user image:
                                user.SaveImage(parser);
                                return ProfileManager.ProfilePage(user);
                            default:
                                return DashboardManager.Dashboard(user);
                        }
                    }
                    else
                    {
                        // Just return dashboard
                        return DashboardManager.Dashboard(user);
                    }
                }
                // We have image; read! (how????)
                // We will return the same page, with new image!
            }
            if (request.HasEntityBody)
            {
                string input;
                // Read input, then dispatch accordingly
                using (StreamReader reader = new StreamReader(request.InputStream))
                {
                    input = reader.ReadToEnd();
                    NameValueCollection dict = HttpUtility.ParseQueryString(input);
                    if (dict["submit"] != null)
                    {
                        // Dispatch to correct logic:
                        switch (dict["submit"])
                        {
                            case "Logout":
                                if (request.Cookies["UserID"] != null)
                                {
                                    // Logout user by removing UserID token:
                                    Cookie cookie = new Cookie("UserID", "-1");
                                    cookie.Expires = DateTime.Now.AddDays(-1d);
                                    response.Cookies.Add(cookie);
                                }
                                return LoginManager.Login();
                            case "Signup":
                                user = new User(dict["firstName"], dict["lastName"], dict["email"], dict["password"]);
                                user.Create();
                                return LoginManager.SuccessSignup();
                            case "Login":
                                try
                                {
                                    user = new User(dict["email"], dict["password"]);
                                    Cookie logger = new Cookie("UserID", Convert.ToString(user.id));
                                    response.Cookies.Add(logger);
                                    response.AppendHeader("dest", "dashboard");
                                    return DashboardManager.Dashboard(user);
                                } catch (InvalidPasswordException)
                                {
                                    return LoginManager.FailLogin();
                                } catch (UserNotFoundException)
                                {
                                    return LoginManager.FailLogin();
                                }
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
                        // If string is not empty, perhaps it is a 
                        return LoginManager.Login();
                    }
                }
            }
            else if (user == null)
            {
                // Send login page EXCEPT if requesting password reset:
                if (request.QueryString["ResetToken"] != null)
                {
                    // Get token; search DB for hash. If it exists, show reset form
                    string token = request.QueryString["ResetToken"];
                    long id = PasswordReset.GetUser(token);
                    // Show reset form. Form will have a hidden input with UserID?
                    return ResetManager.CreateReset(id);
                }
                else
                {
                    return LoginManager.Login();
                }
            }
            else if (request.QueryString["dest"] != null)
            {
                switch (request.QueryString["dest"])
                {
                    case "dashboard":
                        return DashboardManager.Dashboard(user);
                    case "profile":
                        return ProfileManager.ProfilePage(user);
                    default:
                        return DashboardManager.Dashboard(user);
                }
            }
            else if (path.Length != 0)
            {
                // Check if user is allowed to access
                // Serve back whatever's at the path:
                
                byte[] buffer = File.ReadAllBytes(WebServer.GeneratePath(path));
                response.ContentLength64 = buffer.Length;
                using (Stream resp = response.OutputStream)
                {
                    resp.Write(buffer, 0, buffer.Length);
                }
                return "";
            }
            else
            {
                // If logged in (but no request), just send back home page:
                return DashboardManager.Dashboard(user);
            }
        }
    }
}
