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
            WebServer server = new WebServer("http://localhost:60001/", SendResponse);
            server.Run();
            Console.WriteLine("Hello World~!");
            Console.ReadKey();
            server.Stop();
        }
        public static string SendResponse(HttpListenerRequest request)
        {
            Stream input = request.InputStream;
            return string.Format("<HTML><BODY>My web page.<br>{0}</BODY></HTML>", DateTime.Now);
        }
    }
}
