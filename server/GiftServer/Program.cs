using System;
using GiftServer.Server;
using GiftServer.Properties;
using System.Net;

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
            Server.WebServer server = new Server.WebServer(Resources.URL + "/", Route);
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
        public static string Route(HttpListenerContext ctx)
        {
            Controller control = new Controller(ctx);
            return control.Dispatch();
        }
    }
}
