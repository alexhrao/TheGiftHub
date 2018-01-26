using GiftServer.Properties;
using Newtonsoft.Json.Linq;
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
        public class GoogleUser : OAuthUser
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
        }
    }
}

/*
 * Old code - tried to do it without the URL
            public static UserCredential Verify(string token)
            {

                // testing basic API:
                PlusService plusService = new PlusService(
                    new BaseClientService.Initializer()
                    {
                        ApiKey = ""
                    });
                Person me = plusService.People.Get("me").Execute();
                Type myType = me.GetType();
                IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

                foreach (PropertyInfo info in props)
                {
                    Console.WriteLine("Property " + info.Name + " -> " + info.GetValue(me));
                }


                IAuthorizationCodeFlow flow = new AuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = Constants.GoogleClientID,
                        ClientSecret = Constants.GoogleClientSecret
                    },
                    Scopes = new[] { PlusService.Scope.UserinfoProfile, PlusService.Scope.UserinfoEmail },
                    DataStore = new FileDataStore("helloWorld")
                });
                flow.CreateAuthorizationCodeRequest(".");
                TokenResponse a = flow.LoadTokenAsync(token, CancellationToken.None).Result;
                UserCredential credential = new UserCredential(flow, token, a);
                return credential;
            }







            private static GoogleClientSecrets GetClientConfiguration()
            {
                using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
                {
                    return GoogleClientSecrets.Load(stream);
                }
            }
            private static PlusService GetPlusService(TokenResponse credentials)
            {
                IAuthorizationCodeFlow flow =
                new GoogleAuthorizationCodeFlow(
                    new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = GetClientConfiguration().Secrets,
                        Scopes = new string[] { PlusService.Scope.PlusLogin }
                    });

                UserCredential credential = new UserCredential(flow, "me", credentials);

                return new PlusService(
                    new Google.Apis.Services.BaseClientService.Initializer()
                    {
                        ApplicationName = "Haikunamatata",
                        HttpClientInitializer = credential
                    });
            }

*/
