using System;
using HtmlAgilityPack;
using GiftServer.Properties;
using GiftServer.Data;
using System.Web;
using System.Resources;
using GiftServer.Server;
using System.Threading;

namespace GiftServer
{
    namespace HtmlManager
    {
        /// <summary>
        /// Manages interaction with user (HTML and email) for resetting passwords
        /// </summary>
        public class ResetManager
        {
            private ResourceManager HtmlManager;
            private ResourceManager StringManager;
            private LoginManager LoginManager;
            /// <summary>
            /// Subject line for a PasswordReset Notification
            /// </summary>
            public string ResetNotificationSubject
            {
                get
                {
                    return StringManager.GetString("passwordResetNotification");
                }
            }
            /// <summary>
            /// Initiate a new ResetManager
            /// </summary>
            /// <param name="controller">The controller for this thread</param>
            public ResetManager(Controller controller)
            {
                Thread.CurrentThread.CurrentUICulture = controller.Culture;
                Thread.CurrentThread.CurrentCulture = controller.Culture;
                HtmlManager = new ResourceManager("GiftServer.HtmlTemplates", typeof(ResetManager).Assembly);
                StringManager = new ResourceManager("GiftServer.Strings", typeof(ResetManager).Assembly);
                LoginManager = controller.LoginManager;
            }
            /// <summary>
            /// Notify the user of a sent email
            /// </summary>
            /// <returns>HTML markup with alert for a reset email</returns>
            public string ResetPasswordSent()
            {
                HtmlDocument login = new HtmlDocument();
                login.LoadHtml(LoginManager.Login());
                HtmlNode alert = login.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                alert.AddClass("alert-success");
                alert.AddClass("in");
                alert.RemoveClass("hidden");
                HtmlNode message = HtmlNode.CreateNode(StringManager.GetString("recoveryEmailSent"));
                alert.AppendChild(message);
                return login.DocumentNode.OuterHtml;
            }
            /// <summary>
            /// Successfully changed password
            /// </summary>
            /// <returns>Complete HTML Markup for a successful reset</returns>
            public string ResetPasswordSuccess()
            {
                HtmlDocument login = new HtmlDocument();
                login.LoadHtml(LoginManager.Login());
                HtmlNode alert = login.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                alert.AddClass("alert-success");
                alert.AddClass("in");
                alert.RemoveClass("hidden");
                HtmlNode message = HtmlNode.CreateNode(StringManager.GetString("newLogin"));
                alert.AppendChild(message);
                return login.DocumentNode.OuterHtml;
            }
            /// <summary>
            /// Failed to reset password
            /// </summary>
            /// <returns>Complete HTML markup for an expired response</returns>
            public string ResetPasswordExpired()
            {
                HtmlDocument login = new HtmlDocument();
                login.LoadHtml(LoginManager.Login());
                HtmlNode alert = login.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                alert.AddClass("alert-danger in");
                alert.RemoveClass("hidden");
                HtmlNode message = HtmlNode.CreateNode(StringManager.GetString("codeExpired"));
                alert.AppendChild(message);
                return login.DocumentNode.OuterHtml;
            }
            /// <summary>
            /// Failed so send the reset email
            /// </summary>
            /// <returns>Complete HTML Markup for a failure to send email</returns>
            public string ResetPasswordFailure()
            {
                HtmlDocument login = new HtmlDocument();
                login.LoadHtml(LoginManager.Login());
                HtmlNode alert = login.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                alert.AddClass("alert-danger in");
                alert.RemoveClass("hidden");
                HtmlNode message = HtmlNode.CreateNode(StringManager.GetString("emailFailure"));
                alert.AppendChild(message);
                return login.DocumentNode.OuterHtml;
            }
            /// <summary>
            /// Create an email to reset the user's password
            /// </summary>
            /// <param name="user">The user to reset</param>
            /// <returns>HTML Markup for the reset email</returns>
            public string CreateReset(User user)
            {
                HtmlDocument pg = new HtmlDocument();
                pg.LoadHtml(HtmlManager.GetString("header") + HtmlManager.GetString("passwordReset"));
                HtmlNode hidden = pg.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@name), \" \"), \" userID \")]");
                hidden.Attributes["value"].Value = Convert.ToString(user.UserId);
                return pg.DocumentNode.OuterHtml;
            }
            /// <summary>
            /// Create an email with a reset token
            /// </summary>
            /// <param name="token">The reset token</param>
            /// <returns>HTML Markup allowing the receiver to reset a password</returns>
            public string GenerateEmail(string token)
            {
                HtmlDocument email = new HtmlDocument();
                email.LoadHtml(HtmlManager.GetString("header") + HtmlManager.GetString("passwordResetEmail"));
                HtmlNode resetLink = email.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" passwordReset \")]");
                resetLink.Attributes["href"].Value = Constants.URL + "?ResetToken=" + token;
                HtmlNode resetUrl = email.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" resetURL \")]");
                resetUrl.Attributes["href"].Value = Constants.URL + "?ResetToken=" + token;
                resetUrl.InnerHtml = Constants.URL + "?ResetToken=" + token;
                HtmlNode homePage = email.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" changePassword \")]");
                homePage.Attributes["href"].Value = Constants.URL;
                email.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userNotFound \")]").Remove();
                return email.DocumentNode.OuterHtml;
            }
            /// <summary>
            /// Generate an email for an unknown user
            /// </summary>
            /// <returns>HTML Markup telling the receiver to check their email</returns>
            public string GenerateEmail()
            {
                HtmlDocument email = new HtmlDocument();
                email.LoadHtml(HtmlManager.GetString("header") + HtmlManager.GetString("passwordResetEmail"));
                HtmlNode found = email.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userFound \")]");
                found.Remove();
                HtmlNode notfound = email.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userNotFound \")]");
                return email.DocumentNode.OuterHtml;
            }
            /// <summary>
            /// Notify the user of reset
            /// </summary>
            /// <param name="user">The user to notify</param>
            /// <returns>Complete HTML Markup for this notification</returns>
            public string GenerateNotification(User user)
            {
                HtmlDocument email = new HtmlDocument();
                email.LoadHtml(HtmlManager.GetString("header") + HtmlManager.GetString("passwordResetNotification"));
                HtmlNode name = email.DocumentNode.SelectSingleNode(@"//*[contains(concat("" "", normalize-space(@id), "" ""), "" userName "")]");
                name.InnerHtml = HttpUtility.HtmlEncode(user.UserName);
                return email.DocumentNode.OuterHtml;
            }
        }
    }
}