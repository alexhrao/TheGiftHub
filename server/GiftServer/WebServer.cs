using System;
using System.Net;
using System.Threading;
using System.Text;
using System.IO;

namespace GiftServer
{
    namespace Server
    {
        /// <summary>
        /// The "Engine" of the website: The Server
        /// </summary>
        /// <remarks>
        /// This implements IDisposable; whenever the Server is finished, it'll properly close all sockets before exiting.
        /// </remarks>
        public sealed class WebServer : IDisposable
        {
            private readonly HttpListener _listener = new HttpListener();
            private readonly Func<HttpListenerContext, string> _responseGenerator;
            /// <summary>
            /// Create a new WebServer with the given prefixes and runner
            /// </summary>
            /// <remarks>
            /// The runner method must accept an HttpListenerContext, and return an HTML string.
            /// </remarks>
            /// <param name="prefixes">Prefixes for the server to listen upon</param>
            /// <param name="method">The runner method</param>
            public WebServer(string[] prefixes, Func<HttpListenerContext, string> method)
            {
                // NOTE: To do this, I needed to run the following command in elevated mode:
                //  netsh http add urlacl url=http://+:80/ user=<USERNAME>
                foreach (string prefix in prefixes)
                {
                    try
                    {
                        _listener.Prefixes.Add(prefix);
                    }
                    catch (Exception)
                    {
                        // OK - only exception is access is denied - we know this will be ok
                    }
                }
                _responseGenerator = method;
                _listener.Start();
            }
            /// <summary>
            /// Run the multithreaded Server
            /// </summary>
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
                                    try
                                    {
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
                                    }
                                    catch (Exception)
                                    {
                                        // WebServer is shutting down, so safely ignore!
                                    }
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
            /// <summary>
            /// Properly dispose of the _listener
            /// </summary>
            public void Dispose()
            {
                _listener.Stop();
                _listener.Close();
            }
        }
    }
}