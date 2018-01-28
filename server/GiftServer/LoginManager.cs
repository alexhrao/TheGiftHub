using System;
using HtmlAgilityPack;
using System.Resources;
using System.Threading;
using GiftServer.Server;
using GiftServer.Exceptions;

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
                return HtmlManager.GetString("header") + HtmlManager.GetString("login");
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