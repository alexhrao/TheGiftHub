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
        public class GoogleUser : OAuthUser, IEquatable<GoogleUser>
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
            private string oAuthId;
            /// <summary>
            /// The unique GoogleID of this user
            /// </summary>
            public override string OAuthId
            {
                get
                {
                    return oAuthId;
                }
            }
            private string picture;
            /// <summary>
            /// The picture of this user, as a byte array
            /// </summary>
            public override byte[] Picture
            {
                get
                {
                    using (WebClient pictureClient = new WebClient())
                    {
                        return pictureClient.DownloadData(picture);
                    }
                }
            }

            /// <summary>
            /// Create a Google User from the given token
            /// </summary>
            /// <remarks>
            /// When the user signs in, the Token generated can be used to fetch this user's information.
            /// </remarks>
            /// <param name="token">The user's authentication token</param>
            public GoogleUser(string token)
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
                oAuthId = parsed["sub"].Value<string>();
                picture = parsed["picture"].Value<string>();
            }
            /// <summary>
            /// Find whether or not the object is this GoogleUser
            /// </summary>
            /// <param name="obj">Object to test</param>
            /// <returns>True if they are the same value</returns>
            public override bool Equals(object obj)
            {
                if (obj != null && obj is GoogleUser g)
                {
                    return Equals(g);
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// Find whether or not the OAuthUser is this GoogleUser
            /// </summary>
            /// <param name="user">The OAuthUser</param>
            /// <returns>True if they are the same value</returns>
            public override bool Equals(OAuthUser user)
            {
                if (user != null && user is GoogleUser g)
                {
                    return Equals(g);
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// Find whether or not the two GoogleUsers are identical
            /// </summary>
            /// <param name="user">The users to compare</param>
            /// <returns>True if they are the same user</returns>
            public bool Equals(GoogleUser user)
            {
                return user != null && user.OAuthId == OAuthId;
            }
            /// <summary>
            /// Get the hash code for this GoogleUser
            /// </summary>
            /// <returns>The hash code</returns>
            public override int GetHashCode()
            {
                return (OAuthId + "Google").GetHashCode();
            }
        }
    }
}