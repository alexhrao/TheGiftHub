using System;
using HtmlAgilityPack;
using GiftServer.Properties;
using GiftServer.Data;
using System.Web;
using System.Resources;
using GiftServer.Server;

namespace GiftServer
{
    namespace Html
    {
        public class ResetManager
        {
            private ResourceManager ResourceManager;
            private LoginManager LoginManager;
            public ResetManager(Controller controller)
            {
                ResourceManager = new ResourceManager("GiftServer.HtmlTemplates", typeof(ResetManager).Assembly);
                LoginManager = controller.LoginManager;
            }
            public string ResetPasswordSent()
            {
                HtmlDocument login = new HtmlDocument();
                login.LoadHtml(LoginManager.Login());
                HtmlNode alert = login.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                alert.AddClass("alert-success");
                alert.AddClass("in");
                alert.RemoveClass("hidden");
                HtmlNode message = HtmlNode.CreateNode("<p><strong>Recovery email sent</strong> - check your inbox</p>");
                HtmlNodeCollection children = new HtmlNodeCollection(alert)
                {
                    message
                };
                alert.AppendChildren(children);
                return login.DocumentNode.OuterHtml;
            }
            public string SuccessResetPassword()
            {
                HtmlDocument login = new HtmlDocument();
                login.LoadHtml(LoginManager.Login());
                HtmlNode alert = login.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                alert.AddClass("alert-success");
                alert.AddClass("in");
                alert.RemoveClass("hidden");
                HtmlNode message = HtmlNode.CreateNode("<p><strong>Password Reset</strong> Please login below with your new password</p>");
                HtmlNodeCollection children = new HtmlNodeCollection(alert)
                {
                    message
                };
                alert.AppendChildren(children);
                return login.DocumentNode.OuterHtml;
            }
            public string ResetFailed()
            {
                HtmlDocument login = new HtmlDocument();
                login.LoadHtml(HtmlTemplates.header + HtmlTemplates.login);
                HtmlNode alert = login.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                alert.AddClass("alert-danger in");
                alert.RemoveClass("hidden");
                HtmlNode message = HtmlNode.CreateNode("<p><strong>Uh-Oh...</strong> Looks like that code expired."
                                                        + " <a data-toggle=\"modal\" href=\"#resetPassword\">Reset your Password</a></p>");
                HtmlNodeCollection children = new HtmlNodeCollection(alert);
                children.Add(message);
                alert.AppendChildren(children);
                return login.DocumentNode.OuterHtml;
            }
            public string CreateReset(User user)
            {
                HtmlDocument pg = new HtmlDocument();
                pg.LoadHtml(HtmlTemplates.header + HtmlTemplates.passwordReset);
                HtmlNode hidden = pg.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@name), \" \"), \" userID \")]");
                hidden.Attributes["value"].Value = Convert.ToString(user.UserId);
                return pg.DocumentNode.OuterHtml;
            }
            public string GenerateEmail(string token)
            {
                HtmlDocument email = new HtmlDocument();
                email.LoadHtml(HtmlTemplates.header + HtmlTemplates.passwordResetEmail);
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
            public string GenerateEmail()
            {
                HtmlDocument email = new HtmlDocument();
                email.LoadHtml(HtmlTemplates.header + HtmlTemplates.passwordResetEmail);
                HtmlNode found = email.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userFound \")]");
                found.Remove();
                HtmlNode notfound = email.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userNotFound \")]");
                return email.DocumentNode.OuterHtml;
            }
            public string GenerateNotification(User user)
            {
                HtmlDocument email = new HtmlDocument();
                email.LoadHtml(HtmlTemplates.header + HtmlTemplates.passwordResetNotification);
                HtmlNode name = email.DocumentNode.SelectSingleNode(@"//*[contains(concat("" "", normalize-space(@id), "" ""), "" userName "")]");
                name.InnerHtml = HttpUtility.HtmlEncode(user.UserName);
                return email.DocumentNode.OuterHtml;
            }
        }
    }
}