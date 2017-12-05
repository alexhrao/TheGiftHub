using System;
using System.Net;
using System.Threading;
using System.Text;
using System.IO;
using GiftServer.Properties;
using System.Net.Sockets;
using System.Net.Security;

namespace GiftServer
{
    namespace Server
    {
        public class WebServer
        {
            private readonly HttpListener _listener = new HttpListener();
            private readonly Func<HttpListenerContext, string> _responseGenerator;

            public WebServer(string[] prefixes, Func<HttpListenerContext, string> method)
            {
                // Right now, this doesn't seem to work for anything but localhost - IDK why.
                // I think it has something to do with HttpListener requiring the local address... IDK.
                // Below is trying to use the TcpListener - I have no idea if this will work on an AWS server.
                // Really hope it does tho - IDK why this won't work on an AWS webserver.

                // Actually, I might have just fixed it. By allowing ALL prefixes with port 60001
                // This should mean that it's origin does not matter... We'll see when I get back to US
                // NOTE: To do this, I needed to run the following command in elevated mode:
                //  netsh http add urlacl url=http://+:60001/ user=<USERNAME>
                /*
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                TcpListener server = new TcpListener(localAddr, port);
                server.Start();
                while (true)
                {
                    TcpClient tcpClient = server.AcceptTcpClient();
                    NetworkStream stream = tcpClient.GetStream();
                    byte[] buffer = new byte[1024];
                    int length = 0;
                    while ((length = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        string data = System.Text.Encoding.ASCII.GetString(buffer, 0, length);
                        Console.WriteLine("Received: {0}", data);
                    }
                }
                */
                foreach (string prefix in prefixes)
                {
                    _listener.Prefixes.Add(prefix);
                }
                _responseGenerator = method;
                _listener.Start();
            }
            public void Run()
            {
                try
                {
                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                        while (_listener.IsListening)
                        {
                            try
                            {
                                ThreadPool.QueueUserWorkItem((r) =>
                                {
#if !DEBUG
                                    try
                                    {
#endif
                                        HttpListenerContext rtx = (HttpListenerContext)(r);
                                        string resp = _responseGenerator(rtx);
                                        if (resp != null)
                                        {
                                            using (Stream output = rtx.Response.OutputStream)
                                            {
                                                byte[] respBuffer = Encoding.UTF8.GetBytes(resp);
                                                rtx.Response.ContentLength64 = respBuffer.Length;
                                                rtx.Response.OutputStream.Write(respBuffer, 0, respBuffer.Length);
                                            }
                                        }
                                        else
                                    {
                                        if (rtx.Response.OutputStream.CanWrite)
                                        {
                                            rtx.Response.OutputStream.Close();
                                        }
                                    }
#if !DEBUG

                                    }
                                    catch (Exception)
                                    {
                                        // WebServer is shutting down, so safely ignore!
                                    }
#endif
                                }, _listener.GetContext());
                            }
                            catch (Exception)
                            {
                                // WebServer is shutting down, so safely ignore!
                            }
                        }
                    });
                } catch (Exception)
                {
                    // If any exception is thrown and NOT caught by Dispatch, then it's time to close down the server! So, don't do anything
                }
            }
            public void Stop()
            {
                _listener.Stop();
                _listener.Close();
            }
        }
    }
}