using System;
using GiftServer.Server;
using GiftServer.Properties;
using System.Net;
using System.Collections.Generic;
using GiftServer.Exceptions;
using System.Resources;

namespace GiftServer
{
    namespace Server
    {
        public class GiftServer
        {
            public List<IPEndPoint> Addresses = new List<IPEndPoint>();
            /// <summary>
            /// Main method and entry point - starts the web server on the specified host/port.
            /// </summary>
            /// <param name="args">Reserved for future use</param>
            public static void Main(string[] args)
            {
                /* for culture-specific resources
                ResourceManager rm = new ResourceManager("GiftServer.HtmlTemplates", typeof(Program).Assembly);
                Console.WriteLine(rm.GetString("profile"));
                */
                GiftServer program = new GiftServer();
                program.Start(args);
            }
            public void Start(string[] prefixes)
            {
                if (prefixes.Length == 0)
                {
                    prefixes = new string[2];
                    prefixes[0] = "http://*:80/";
                    prefixes[1] = "https://*:443/";
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
                                    foreach (IPEndPoint end in Addresses)
                                    {
                                        Console.WriteLine("\t" + end.ToString());
                                    }
                                }
                                break;
                            case "statistics":
                                break;
                            case "logged":
                                Console.WriteLine("Users logged in:");
                                foreach (Controller.Connection con in Controller.Connections)
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
                                foreach (Warning warn in Controller.Warnings)
                                {
                                    Console.WriteLine("\t" + warn.ToString());
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
            public string Route(HttpListenerContext ctx)
            {
                Addresses.Add(ctx.Request.RemoteEndPoint);
                Controller control = new Controller(ctx);
                return control.Dispatch();
            }
        }
    }
}
