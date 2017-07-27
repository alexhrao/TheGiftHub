﻿using System;
using System.Net;
using System.IO;
using System.Web;
using System.Collections.Specialized;
using GiftServer.Data;
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
                                    return Resources.header + Resources.onLogin;
                                } catch (InvalidPasswordException)
                                {
                                    return Resources.header + Resources.loginFailed;
                                } catch (UserNotFoundException)
                                {
                                    return Resources.header + Resources.loginFailed;
                                }
                            default:
                                return Resources.header + Resources.login;
                        }
                    } else
                    {
                        return Resources.header + Resources.login;
                    }
                }
            } else if (request.QueryString["dest"] != null)
            {
                switch (request.QueryString["dest"])
                {
                    case "dashboard":
                        return Resources.header + Resources.navigationBar + Resources.dashboard;
                    default:
                        return Resources.header + Resources.navigationBar + Resources.dashboard;
                        break;
                }
                Console.WriteLine(request.QueryString["dest"]);
            }
            else if (user != null)
            {
                // If logged in (but no request), just send back home page:
                return "<html><body><form method=\"POST\"><input name=\"theMail\" type=\"email\"/><button type=\"submit\" value=\"submit\">Hello</button></form></body></html>";
            }
            else
            {
                // If not logged in, send the login page!
                return Resources.header + Resources.navigationBar + Resources.login;
            }
        }
    }
}
