using System;
using System.Net;
using System.Collections.Generic;
using GiftServer.Exceptions;

namespace GiftServer
{
    namespace Server
    {
        /// <summary>
        /// The main class for the server, and the entry point
        /// </summary>
        public class GiftServer
        {
            /// <summary>
            /// A list of all addresses contacted
            /// </summary>
            public List<Contact> Addresses = new List<Contact>();
            /// <summary>
            /// A single contact to the server
            /// </summary>
            public class Contact
            {
                /// <summary>
                /// The IP Endpoint of the contact
                /// </summary>
                public readonly IPEndPoint Address;
                /// <summary>
                /// The Time this connection was made
                /// </summary>
                public readonly DateTime TimeStamp;
                /// <summary>
                /// The only constructor
                /// </summary>
                /// <param name="address"></param>
                public Contact(IPEndPoint address)
                {
                    Address = address;
                    TimeStamp = DateTime.Now;
                }
                /// <summary>
                /// Overrides base ToString method
                /// </summary>
                /// <returns>A string with the endpoint and timestamp</returns>
                public override string ToString()
                {
                    return TimeStamp.ToString("yyyy-MM-dd HH:mm:ss") + " -> " + Address.ToString();
                }

            }
            private Object key = new Object();
            /// <summary>
            /// Main method and entry point - starts the web server on the specified host/port.
            /// </summary>
            /// <param name="args">Optional prefixes</param>
            public static void Main(string[] args)
            {
                GiftServer program = new GiftServer();
                program.Start(args);
            }
            /// <summary>
            /// Starts up the webserver with the specified prefixes
            /// </summary>
            /// <param name="prefixes">Prefixes for the server to listen on</param>
            public void Start(string[] prefixes)
            {
                if (prefixes.Length == 0)
                {
                    prefixes = new string[3];
                    prefixes[0] = "http://*:80/";
                    prefixes[1] = "https://*:443/";
                    prefixes[2] = "https://*:44300/";
                }
                using (WebServer server = new WebServer(prefixes, this.Route))
                {
                    server.Run();
                    Console.WriteLine("Server is Active...\nType help for available commands");
                    string input = null;
                    while ((input = Console.ReadLine()) != null)
                    {
                        switch (input.ToLower())
                        {
                            case "quit":
                                return;
                            case "help":
                                Console.WriteLine("Available Commands:"
                                                + "\n\tquit - stops this instance of the server"
                                                + "\n\thelp - shows this information"
                                                + "\n\tconnections - shows information about connections to the server"
                                                + "\n\tstatistics - shows various statistics about this server session"
                                                + "\n\tlogged - shows UserIDs currently logged in"
                                                + "\n\twarnings - shows any warnings that have been issued");
                                break;
                            case "connections":
                                if (Addresses.Count == 0)
                                {
                                    Console.WriteLine("The server has not yet been contacted");
                                }
                                else if (Addresses.Count == 1)
                                {
                                    Console.WriteLine("The server has been contacted 1 time by the following location:\n\t" + Addresses[0].ToString());
                                }
                                else
                                {
                                    Console.WriteLine("The Server has been contacted " + Addresses.Count + " times by the following locations:");
                                    foreach (Contact end in Addresses)
                                    {
                                        Console.WriteLine("\t" + end.ToString());
                                    }
                                }
                                break;
                            case "statistics":
                                if (Addresses.Count == 0)
                                {
                                    Console.WriteLine("The server has not yet been contacted");
                                }
                                else if (Addresses.Count == 1)
                                {
                                    Console.WriteLine("The server has been contacted 1 time");
                                }
                                else
                                {
                                    Console.WriteLine("The Server has been contacted " + Addresses.Count + " times.");
                                }
                                Console.WriteLine(Controller.Connections.Count + " user(s) are currently logged in");
                                Console.WriteLine(Controller.Warnings.Count + " warning(s) have been issued");
                                Console.WriteLine(Controller.Warnings.FindAll(x => x is ExecutionErrorWarning).Count + " execution error(s) have been raised");
                                break;
                            case "logged":
                                Console.WriteLine("Users logged in:");
                                foreach (Connection con in Controller.Connections)
                                {
                                    Console.WriteLine("\tUser " + con.Info.UserId + " (Hash: " + con.Info.Hash + ") Connected from following locations:");
                                    foreach (IPEndPoint end in con.Ends)
                                    {
                                        Console.WriteLine("\t\t" + end.ToString());
                                    }
                                }
                                break;
                            case "warnings":
                                Console.WriteLine("Warnings Issued:");
                                Console.WriteLine("\tExecution Errors:");
                                foreach (ExecutionErrorWarning warn in Controller.Warnings.FindAll(x => x is ExecutionErrorWarning))
                                {
                                    Console.WriteLine("\t\t" + warn.ToString());
                                }
                                Console.WriteLine("\tPublic Resources:");
                                foreach (PublicResourceWarning warn in Controller.Warnings.FindAll(x => x is PublicResourceWarning))
                                {
                                    Console.WriteLine("\t\t" + warn.ToString());
                                }
                                break;
                            default:
                                Console.WriteLine("Unknown command \"" + input + "\"");
                                Console.WriteLine("Type \"help\" for available commands");
                                break;
                        }
                    }
                }
            }
            /// <summary>
            /// Routes a specific contact and records the contact
            /// </summary>
            /// <remarks>
            /// This is basically just a wrapper for controller.Dispatch();
            /// </remarks>
            /// <param name="ctx">This HttpListenerContext</param>
            /// <returns>Complete HTML markup to be returned</returns>
            public string Route(HttpListenerContext ctx)
            {
                lock(key)
                {
                    Addresses.Add(new Contact(ctx.Request.RemoteEndPoint));
                }
                Controller control = new Controller(ctx);
                return control.Dispatch();
            }
        }
    }
}
