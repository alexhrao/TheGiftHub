using GiftServer.Properties;
using Newtonsoft.Json.Linq;
using System;
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
        public class FacebookUser : OAuthUser, IEquatable<FacebookUser>
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
            /// This will be a 5 character string that represents the **language**-**Location** - en, fr, etc.
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
            /// The unique FacebookID of this user
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
            /// Create a Facebook User from the given token
            /// </summary>
            /// <remarks>
            /// When the user signs in, the Token generated can be used to fetch this user's information.
            /// </remarks>
            /// <param name="token">The user's authentication token</param>
            public FacebookUser(string token)
            {
                HttpClient sender = new HttpClient();
                string fields = "/me?fields=" + Uri.EscapeDataString("id,name,email,picture.width(1024).height(1024),locale") + "&access_token=" + Uri.EscapeDataString(token);
                HttpResponseMessage resp = sender.GetAsync(Constants.FacebookOAuthUrl + fields).Result;
                string result = resp.Content.ReadAsStringAsync().Result;
                // Parse
                JObject parsed = JObject.Parse(result);
                try
                {
                    name = parsed["name"].Value<string>();
                    locale = parsed["locale"].Value<string>();
                    email = new MailAddress(parsed["email"].Value<string>());
                    oAuthId = parsed["id"].Value<string>();
                    picture = parsed["picture"]["data"]["url"].Value<string>();
                }
                catch (ArgumentNullException)
                {
                    throw new ArgumentException("Invalid Facebook Token");
                }
            }
            /// <summary>
            /// Find whether or not the object is this FacebookUser
            /// </summary>
            /// <param name="obj">Object to test</param>
            /// <returns>True if they are the same value</returns>
            public override bool Equals(object obj)
            {
                if (obj != null && obj is FacebookUser f)
                {
                    return Equals(f);
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// Find whether or not the OAuthUser is this FacebookUser
            /// </summary>
            /// <param name="user">The OAuthUser</param>
            /// <returns>True if they are the same value</returns>
            public override bool Equals(OAuthUser user)
            {
                if (user != null && user is FacebookUser f)
                {
                    return Equals(f);
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// Find whether or not the two FacebookUsers are identical
            /// </summary>
            /// <param name="user">The users to compare</param>
            /// <returns>True if they are the same user</returns>
            public bool Equals(FacebookUser user)
            {
                return user != null && user.OAuthId == OAuthId;
            }
            /// <summary>
            /// Get the hash code for this FacebookUser
            /// </summary>
            /// <returns>The hash code</returns>
            public override int GetHashCode()
            {
                return (OAuthId + "Facebook").GetHashCode();
            }
        }
    }
}
