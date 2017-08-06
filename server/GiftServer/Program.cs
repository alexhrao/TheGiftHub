using System;
using GiftServer.Server;
using GiftServer.Properties;
using System.Net;
using System.Collections.Generic;

namespace GiftServer
{
    namespace Server
    {
        public class Program
        {
            public ulong NumContacts = 0;
            public List<IPEndPoint> addresses = new List<IPEndPoint>();
            /// <summary>
            /// Main method and entry point - starts the web server on the specified host/port.
            /// </summary>
            /// <param name="args">Reserved for future use</param>
            public static void Main(string[] args)
            {
                Program program = new Program();
                program.Start();
            }

            public void Start()
            {
                Server.WebServer server = new Server.WebServer(Resources.URL + "/", this.Route);
                server.Run();
                Console.WriteLine("Server is Active...\nType help for available commands");
                string input = null;
                while ((input = Console.ReadLine()) != null)
                {
                    switch (input.ToLower())
                    {
                        case "quit":
                            server.Stop();
                            return;
                        case "help":
                            Console.WriteLine("Available Commands:"
                                            + "\n\tquit - stops this instance of the server"
                                            + "\n\thelp - shows this information"
                                            + "\n\tconnections - shows information about connections to the server"
                                            + "\n\tstatistics - shows various statistics about this server session");
                            break;
                        case "connections":
                            if (NumContacts == 0)
                            {
                                Console.WriteLine("The server has not yet been contacted");
                            }
                            else if (NumContacts == 1)
                            {
                                Console.WriteLine("The server has been contacted 1 time by the following location:\n\t" + addresses[0].ToString());
                            }
                            else
                            {
                                Console.WriteLine("The Server has been contacted " + NumContacts + " times by the following locations:");
                                foreach (IPEndPoint end in addresses)
                                {
                                    Console.WriteLine("\t" + end.ToString());
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            public string Route(HttpListenerContext ctx)
            {
                this.NumContacts++;
                addresses.Add(ctx.Request.RemoteEndPoint);
                Controller control = new Controller(ctx);
                return control.Dispatch();
            }
        }
    }
}
