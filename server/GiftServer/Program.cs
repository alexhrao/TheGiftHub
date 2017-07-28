using System;
using System.Net;
using System.IO;
using System.Web;
using System.Collections.Specialized;
using GiftServer.Data;
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
            Server.WebServer server = new Server.WebServer("http://localhost:60001/", Dispatch);
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
            User user = null;
            if (reqLogger != null)
            {
                user = new User(Convert.ToInt64(reqLogger.Value));
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
                                return Resources.header + Resources.login;
                            case "Signup":
                                user = new User(dict["firstName"], dict["lastName"], dict["email"], dict["password"]);
                                user.Create();
                                return Resources.header + Resources.onSignup;
                            case "Login":
                                try
                                {
                                    user = new User(dict["email"], dict["password"]);
                                    Cookie logger = new Cookie("UserID", Convert.ToString(user.id));
                                    response.Cookies.Add(logger);
                                    response.AppendHeader("dest", "dashboard");
                                    return Resources.header + Resources.navigationBar + Resources.dashboard;
                                } catch (InvalidPasswordException)
                                {
                                    return Resources.header + Resources.loginFailed;
                                } catch (UserNotFoundException)
                                {
                                    return Resources.header + Resources.loginFailed;
                                }
                            case "PasswordReset":
                                // Reset password and direct to login page
                                return Resources.header + /* Resources.changedPassword +  */ Resources.login;
                            default:
                                return Resources.header + Resources.login;
                        }
                    } else
                    {
                        return Resources.header + Resources.login;
                    }
                }
            } else if (user == null)
            {
                // Send login page EXCEPT if requesting password reset:
                if (request.QueryString["ResetToken"] != null)
                {
                    // Get token; search DB for hash. If it exists, show reset form
                    string token = request.QueryString["ResetToken"];
                    long id = PasswordReset.GetUser(token);
                    // Show reset form. Form will have a hidden input with UserID?
                    /* return Resources.header + Resources.changePassword; */
                    return Resources.header + Resources.login;
                }
                return Resources.header + Resources.login;
            } else if (request.QueryString["dest"] != null)
            {
                switch (request.QueryString["dest"])
                {
                    case "dashboard":
                        return Resources.header + Resources.navigationBar + Resources.dashboard;
                    default:
                        return Resources.header + Resources.navigationBar + Resources.dashboard;
                }
            }
            else
            {
                // If logged in (but no request), just send back home page:
                return Resources.header + Resources.navigationBar + Resources.dashboard;
            }
        }
    }
}
