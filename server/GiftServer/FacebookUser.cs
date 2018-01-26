using GiftServer.Properties;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mail;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// A user who has authenticated via Google
        /// </summary>
        /// <remarks>
        /// This is for users who authenticate via the "Sign in with Google" method.
        /// </remarks>
        public class FacebookUser : OAuthUser
        {
            private string name;
            /// <summary>
            /// The name of this user
            /// </summary>
            /// <remarks>
            /// This does not differentiate between first, last, etc.
            /// </remarks>
            public override string Name
            {
                get
                {
                    return name;
                }
            }
            private string locale;
            /// <summary>
            /// The locale of this user
            /// </summary>
            /// <remarks>
            /// This will be a two character string that represents the **language** - en, fr, etc.
            /// </remarks>
            public override string Locale
            {
                get
                {
                    return locale;
                }
            }
            private MailAddress email;
            /// <summary>
            /// The Email of this user
            /// </summary>
            public override MailAddress Email
            {
                get
                {
                    return email;
                }
            }
            /// <summary>
            /// The unique FacebookID of this user
            /// </summary>
            public readonly string FacebookId;
            private byte[] picture;
            /// <summary>
            /// The picture of this user, as a byte array
            /// </summary>
            public override byte[] Picture
            {
                get
                {
                    return picture;
                }
            }

            /// <summary>
            /// Create a Google User from the given token
            /// </summary>
            /// <remarks>
            /// When the user signs in, the Token generated can be used to fetch this user's information.
            /// </remarks>
            /// <param name="token">The user's authentication token</param>
            public FacebookUser(string token)
            {
                HttpClient sender = new HttpClient();
                HttpContent content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("id_token", token) });
                HttpResponseMessage resp = sender.PostAsync(Constants.GoogleOAuthUrl, content).Result;
                string result = resp.Content.ReadAsStringAsync().Result;
                // Parse
                JObject parsed = JObject.Parse(result);
                name = parsed["name"].Value<string>();
                locale = parsed["locale"].Value<string>();
                email = new MailAddress(parsed["email"].Value<string>());
                FacebookId = parsed["sub"].Value<string>();
                using (WebClient pictureClient = new WebClient())
                {
                    picture = pictureClient.DownloadData(parsed["picture"].Value<string>());
                }

            }
        }
    }
}
