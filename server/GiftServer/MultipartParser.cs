using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace GiftServer
{
    namespace Server
    {
        /// <summary>
        /// MultipartParser parses a post *not* encoded by the browser
        /// </summary>
        /// <remarks>
        /// Only really use this when dealing with forms that have images...
        /// </remarks>
        public class MultipartParser
        {
            public IDictionary<string, string> Parameters = new Dictionary<string, string>();

            public string FilePartName
            {
                get;
                private set;
            }

            public bool Success
            {
                get;
                private set;
            }

            public string Title
            {
                get;
                private set;
            }

            public int UserId
            {
                get;
                private set;
            }

            public string ContentType
            {
                get;
                private set;
            }

            public string Filename
            {
                get;
                private set;
            }

            public byte[] FileContents
            {
                get;
                private set;
            }

            public MultipartParser(Stream stream, string filePartName)
            {
                FilePartName = filePartName;
                this.Parse(stream, Encoding.UTF8);
            }

            public MultipartParser(Stream stream, Encoding encoding, string filePartName)
            {
                FilePartName = filePartName;
                this.Parse(stream, encoding);
            }

            private void Parse(Stream stream, Encoding encoding)
            {
                this.Success = false;

                // Read the stream into a byte array
                byte[] data = Misc.ToByteArray(stream);

                // Copy to a string for header parsing
                string content = encoding.GetString(data);

                // The first line should contain the delimiter
                int delimiterEndIndex = content.IndexOf("\r\n");

                if (delimiterEndIndex > -1)
                {
                    string delimiter = content.Substring(0, content.IndexOf("\r\n"));

                    string[] sections = content.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string s in sections)
                    {
                        if (s.Contains("Content-Disposition"))
                        {
                            // If we find "Content-Disposition", this is a valid multi-part section
                            // Now, look for the "name" parameter
                            Match nameMatch = new Regex(@"(?<=name\=\"")(.*?)(?=\"")").Match(s);
                            string name = nameMatch.Value.Trim().ToLower();

                            if (name == FilePartName)
                            {
                                // Look for Content-Type
                                Regex re = new Regex(@"(?<=Content\-Type:)(.*?)(?=\r\n\r\n)");
                                Match contentTypeMatch = re.Match(content);

                                // Look for filename
                                re = new Regex(@"(?<=filename\=\"")(.*?)(?=\"")");
                                Match filenameMatch = re.Match(content);

                                // Did we find the required values?
                                if (contentTypeMatch.Success && filenameMatch.Success)
                                {
                                    // Set properties
                                    this.ContentType = contentTypeMatch.Value.Trim();
                                    this.Filename = filenameMatch.Value.Trim();

                                    // Get the start & end indexes of the file contents
                                    int startIndex = contentTypeMatch.Index + contentTypeMatch.Length + "\r\n\r\n".Length;

                                    byte[] delimiterBytes = encoding.GetBytes("\r\n" + delimiter);
                                    int endIndex = Misc.IndexOf(data, delimiterBytes, startIndex);

                                    int contentLength = endIndex - startIndex;

                                    // Extract the file contents from the byte array
                                    byte[] fileData = new byte[contentLength];

                                    Buffer.BlockCopy(data, startIndex, fileData, 0, contentLength);

                                    this.FileContents = fileData;
                                }
                            }
                            else if (!string.IsNullOrWhiteSpace(name))
                            {
                                // Get the start & end indexes of the file contents
                                int startIndex = nameMatch.Index + nameMatch.Length + "\r\n\r\n".Length;
                                Parameters.Add(name, s.Substring(startIndex).TrimEnd(new char[] { '\r', '\n' }).Trim());
                            }
                        }
                    }

                    // If some data has been successfully received, set success to true
                    if (FileContents != null || Parameters.Count != 0)
                        this.Success = true;
                }
            }
        }
        static class Misc
        {
            public static int IndexOf(byte[] searchWithin, byte[] serachFor, int startIndex)
            {
                int index = 0;
                int startPos = Array.IndexOf(searchWithin, serachFor[0], startIndex);

                if (startPos != -1)
                {
                    while ((startPos + index) < searchWithin.Length)
                    {
                        if (searchWithin[startPos + index] == serachFor[index])
                        {
                            index++;
                            if (index == serachFor.Length)
                            {
                                return startPos;
                            }
                        }
                        else
                        {
                            startPos = Array.IndexOf<byte>(searchWithin, serachFor[0], startPos + index);
                            if (startPos == -1)
                            {
                                return -1;
                            }
                            index = 0;
                        }
                    }
                }

                return -1;
            }

            public static byte[] ToByteArray(Stream stream)
            {
                byte[] buffer = new byte[32768];
                using (MemoryStream ms = new MemoryStream())
                {
                    while (true)
                    {
                        int read = stream.Read(buffer, 0, buffer.Length);
                        if (read <= 0)
                            return ms.ToArray();
                        ms.Write(buffer, 0, read);
                    }
                }
            }
        }
    }
}