using System;
using System.Net;
using System.Threading;
using System.Text;
using System.IO;
using GiftServer.Properties;

namespace GiftServer
{
    namespace Server
    {
        public class WebServer
        {
            private readonly HttpListener _listener = new HttpListener();
            private readonly Func<HttpListenerContext, string> _responseGenerator;

            public WebServer(string prefixes, Func<HttpListenerContext, string> method)
            {
                _listener.Prefixes.Add(prefixes);
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
                                        if (resp.Length != 0)
                                        {
                                            using (Stream output = rtx.Response.OutputStream)
                                            {
                                                byte[] respBuffer = Encoding.UTF8.GetBytes(resp);
                                                rtx.Response.ContentLength64 = respBuffer.Length;
                                                rtx.Response.OutputStream.Write(respBuffer, 0, respBuffer.Length);
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