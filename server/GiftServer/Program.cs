using System;
using System.Net;
using System.Threading;
using System.Text;
using System.IO;

namespace GiftServer
{
    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responseGenerator;
        
        public WebServer(string prefixes, Func<HttpListenerRequest, string> method)
        {
            _listener.Prefixes.Add(prefixes);
            _responseGenerator = method;
            _listener.Start();
        }
        public void Run()
        {
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                while (_listener.IsListening)
                {
                    ThreadPool.QueueUserWorkItem((r) =>
                    {
                        HttpListenerContext rtx = (HttpListenerContext)(r);
                        string resp = _responseGenerator(rtx.Request);
                        byte[] respBuffer = Encoding.UTF8.GetBytes(resp);
                        rtx.Response.ContentLength64 = respBuffer.Length;
                        rtx.Response.OutputStream.Write(respBuffer, 0, respBuffer.Length);
                        rtx.Response.OutputStream.Close();
                    }, _listener.GetContext());
                }
            });
        }
        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            WebServer server = new WebServer("http://localhost:60001/", Dispatch);
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
        public static string Dispatch(HttpListenerRequest request)
        {
            Stream input = request.InputStream;
            System.Collections.Specialized.NameValueCollection headers = request.Headers;
            foreach (string key in headers.AllKeys)
            {
                string[] values = headers.GetValues(key);
                if (values.Length > 0)
                {
                    Console.WriteLine("The values of the {0} header are: ", key);
                    foreach (string value in values)
                    {
                        Console.WriteLine("   {0}", value);
                    }
                }
                else
                    Console.WriteLine("There is no value associated with the header.");
            }
            if (request.HasEntityBody)
            {
                Console.WriteLine("Has Body");
            } else
            {
                Console.WriteLine("No body");
            }
            return string.Format("<HTML><BODY>My web page.<br>{0}</BODY></HTML>", DateTime.Now);
        }
    }
}
