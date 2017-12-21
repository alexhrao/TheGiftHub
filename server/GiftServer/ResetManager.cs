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
    namespace Html
    {
        public class ResetManager
        {
            private ResourceManager HtmlManager;
            private ResourceManager StringManager;
            private LoginManager LoginManager;
            public ResetManager(Controller controller)
            {
                Thread.CurrentThread.CurrentUICulture = controller.Culture;
                Thread.CurrentThread.CurrentCulture = controller.Culture;
                HtmlManager = new ResourceManager("GiftServer.HtmlTemplates", typeof(ResetManager).Assembly);
                StringManager = new ResourceManager("GiftServer.Strings", typeof(ResetManager).Assembly);
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
                HtmlNode message = HtmlNode.CreateNode(StringManager.GetString("recoveryEmailSent"));
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
                HtmlNode message = HtmlNode.CreateNode(StringManager.GetString("newLogin"));
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
                login.LoadHtml(LoginManager.Login());
                HtmlNode alert = login.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                alert.AddClass("alert-danger in");
                alert.RemoveClass("hidden");
                HtmlNode message = HtmlNode.CreateNode(StringManager.GetString("codeExpired"));
                HtmlNodeCollection children = new HtmlNodeCollection(alert);
                children.Add(message);
                alert.AppendChildren(children);
                return login.DocumentNode.OuterHtml;
            }
            public string CreateReset(User user)
            {
                HtmlDocument pg = new HtmlDocument();
                pg.LoadHtml(HtmlManager.GetString("header") + HtmlManager.GetString("passwordReset"));
                HtmlNode hidden = pg.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@name), \" \"), \" userID \")]");
                hidden.Attributes["value"].Value = Convert.ToString(user.UserId);
                return pg.DocumentNode.OuterHtml;
            }
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
            public string GenerateEmail()
            {
                HtmlDocument email = new HtmlDocument();
                email.LoadHtml(HtmlManager.GetString("header") + HtmlManager.GetString("passwordResetEmail"));
                HtmlNode found = email.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userFound \")]");
                found.Remove();
                HtmlNode notfound = email.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userNotFound \")]");
                return email.DocumentNode.OuterHtml;
            }
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