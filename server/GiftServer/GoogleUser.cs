using GiftServer.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Mail;

namespace GiftServer
{
    namespace Data
    {
        public class GoogleUser
        {
            public readonly string Name;
            public readonly string Locale;
            public readonly MailAddress Email;
            public readonly string GoogleId;
            public readonly byte[] Picture;

            public GoogleUser(string token)
            {
                HttpClient sender = new HttpClient();
                HttpContent content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("id_token", token) });
                HttpResponseMessage resp = sender.PostAsync(Constants.GoogleOAuthUrl, content).Result;
                string result = resp.Content.ReadAsStringAsync().Result;
                // Parse
                JObject parsed = JObject.Parse(result);
                Name = parsed["name"].Value<string>();
                Locale = parsed["locale"].Value<string>();
                Email = new MailAddress(parsed["email"].Value<string>());
                GoogleId = parsed["sub"].Value<string>();
                string pictureUri = parsed["picture"].Value<string>();
            }
        }
    }
}

/*
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
