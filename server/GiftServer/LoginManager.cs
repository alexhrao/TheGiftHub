using System;
using HtmlAgilityPack;
using System.Resources;
using System.Threading;
using GiftServer.Server;
using GiftServer.Exceptions;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Web;

namespace GiftServer
{
    namespace HtmlManager
    {
        /// <summary>
        /// Generates all HTML for loggin in
        /// </summary>
        /// <remarks>
        /// Arguably, this is the only class that doesn't necessarily require a user to operate
        /// </remarks>
        public class LoginManager
        {
            private ResourceManager HtmlManager;
            private ResourceManager StringManager;
            /// <summary>
            /// HTML Markup for a failed OAuth Login
            /// </summary>
            public string FailOAuth
            {
                get
                {
                    return StringManager.GetString("oAuthFailure");
                }
            }
            /// <summary>
            /// Create a new LoginManager
            /// </summary>
            /// <param name="controller">The controller for this thread</param>
            public LoginManager(Controller controller)
            {
                HtmlManager = new ResourceManager("GiftServer.HtmlTemplates", typeof(LoginManager).Assembly);
                StringManager = new ResourceManager("GiftServer.Strings", typeof(LoginManager).Assembly);
                Thread.CurrentThread.CurrentUICulture = controller.Culture;
            }
            /// <summary>
            /// Return the standard login page
            /// </summary>
            /// <returns>The HTML for standard login</returns>
            public string Login()
            {
                HtmlDocument login = new HtmlDocument();
                login.LoadHtml(HtmlManager.GetString("header") + HtmlManager.GetString("login"));
                return AddCulture(login).DocumentNode.OuterHtml;
            }
            /// <summary>
            /// Add culture to the login page
            /// </summary>
            /// <param name="doc">The document to manipulate</param>
            /// <returns>A culture-specific login page</returns>
            private HtmlDocument AddCulture(HtmlDocument doc)
            {
                HtmlNode cultures = doc.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" cultureSelector \")]");
                HtmlNode cultureIcon = doc.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" cultureIcon \")]");
                // Add our culture first, then all others in alphabetical order:
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT CultureLanguage, CultureLocation, CultureName, CultureDesc FROM cultures ORDER BY CultureName ASC;";
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Append unless ours, then prepend
                            while (reader.Read())
                            {
                                string culture = Convert.ToString(reader["CultureLanguage"]) + "-" + Convert.ToString(reader["CultureLocation"]);
                                HtmlNode option = HtmlNode.CreateNode("<option></option>");
                                option.AddClass("culture-selector");
                                option.Attributes.Add("value", culture);
                                option.InnerHtml = HttpUtility.HtmlEncode(reader["CultureName"]);
                                if (Thread.CurrentThread.CurrentUICulture.ToString() == culture)
                                {
                                    // This is us. Prepend
                                    cultures.PrependChild(option);
                                    // Also, set ICO:
                                    cultureIcon.Attributes.Add("src", culture.Substring(4) + ".ico");
                                }
                                else
                                {
                                    // Append
                                    cultures.AppendChild(option);
                                }
                            }
                        }
                    }
                }
                return AddMeta(doc);
            }
            /// <summary>
            /// Add any meta tags
            /// </summary>
            /// <param name="doc">The existing HTML document</param>
            /// <returns>The modified HTML document</returns>
            private HtmlDocument AddMeta(HtmlDocument doc)
            {
                // Add meta
                HtmlNode head = doc.DocumentNode.SelectSingleNode("/html/head");

                HtmlNode oauthFail = HtmlNode.CreateNode("<meta />");
                oauthFail.Attributes.Add("name", "oauth-fail-alert");
                oauthFail.Attributes.Add("content", HttpUtility.HtmlAttributeEncode(StringManager.GetString("oAuthFailure")));
                head.AppendChild(oauthFail);
                return doc;
            }
            /// <summary>
            /// The login page, with the specified failure reason
            /// </summary>
            /// <param name="e">The Exception thrown</param>
            /// <returns>HTML markup for a failed login attempt</returns>
            public string FailLogin(Exception e)
            {
                HtmlDocument login = new HtmlDocument();
                login.LoadHtml(HtmlManager.GetString("header") + HtmlManager.GetString("login"));
                login = AddCulture(login);
                HtmlNode alert = login.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                alert.AddClass("alert-danger in");
                alert.RemoveClass("hidden");
                HtmlNode message;
                if (e is DuplicateUserException)
                {
                    message = HtmlNode.CreateNode(StringManager.GetString("duplicateUser"));
                }
                else if (e is InvalidPasswordException || e is UserNotFoundException)
                {
                    message = HtmlNode.CreateNode(StringManager.GetString("invalidCredentials"));
                }
                else
                {
                    message = HtmlNode.CreateNode(StringManager.GetString("invalidCredentials"));
                }
                alert.AppendChild(message);
                return login.DocumentNode.OuterHtml;
            }
            /// <summary>
            /// The login page, with a successful creation notification
            /// </summary>
            /// <returns>HTML Markup for a successful user account creation</returns>
            public string SuccessSignup()
            {
                HtmlDocument login = new HtmlDocument();
                login.LoadHtml(HtmlManager.GetString("header") + HtmlManager.GetString("login"));
                login = AddCulture(login);
                HtmlNode alert = login.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                alert.AddClass("alert-success in");
                alert.RemoveClass("hidden");
                HtmlNode message = HtmlNode.CreateNode(StringManager.GetString("signupSuccess"));
                HtmlNodeCollection children = new HtmlNodeCollection(alert)
                {
                    message
                };
                alert.AppendChildren(children);
                return login.DocumentNode.OuterHtml;
            }
            /// <summary>
            /// The privacy policy page
            /// </summary>
            /// <returns>Complete HTML markup for the privacy policy</returns>
            public string PrivacyPolicy()
            {
                HtmlDocument policy = new HtmlDocument();
                policy.LoadHtml(HtmlManager.GetString("header") + HtmlManager.GetString("privacyPolicy"));
                return policy.DocumentNode.OuterHtml;
            }
            /// <summary>
            /// The Terms of Service page
            /// </summary>
            /// <returns>Complete HTML markup for the Terms of Service</returns>
            public string TermsOfService()
            {
                HtmlDocument terms = new HtmlDocument();
                terms.LoadHtml(HtmlManager.GetString("header") + HtmlManager.GetString("termsOfService"));
                return terms.DocumentNode.OuterHtml;
            }
        }
    }
}