using System;
using HtmlAgilityPack;
using GiftServer.Properties;
using GiftServer.Data;

namespace GiftServer
{
    namespace Html
    {
        public static class ResetManager
        {
            public static string ResetPasswordSent()
            {
                HtmlDocument login = new HtmlDocument();
                login.LoadHtml(LoginManager.Login());
                HtmlNode alert = login.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                alert.AddClass("alert-success");
                alert.AddClass("in");
                alert.RemoveClass("hidden");
                HtmlNode message = HtmlNode.CreateNode("<p><strong>Recovery email sent</strong> - check your inbox</p>");
                HtmlNodeCollection children = new HtmlNodeCollection(alert);
                children.Add(message);
                alert.AppendChildren(children);
                return login.DocumentNode.OuterHtml;
            }
            public static string SuccessResetPassword()
            {
                HtmlDocument login = new HtmlDocument();
                login.LoadHtml(LoginManager.Login());
                HtmlNode alert = login.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                alert.AddClass("alert-success");
                alert.AddClass("in");
                alert.RemoveClass("hidden");
                HtmlNode message = HtmlNode.CreateNode("<p><strong>Password Reset</strong> Please login below with your new password</p>");
                HtmlNodeCollection children = new HtmlNodeCollection(alert);
                children.Add(message);
                alert.AppendChildren(children);
                return login.DocumentNode.OuterHtml;
            }
            public static string CreateReset(User user)
            {
                HtmlDocument pg = new HtmlDocument();
                pg.LoadHtml(Resources.header + Resources.passwordReset);
                HtmlNode hidden = pg.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@name), \" \"), \" userID \")]");
                hidden.Attributes["value"].Value = Convert.ToString(user.UserID);
                return pg.DocumentNode.OuterHtml;
            }
            public static string GenerateEmail(string token)
            {
                HtmlDocument email = new HtmlDocument();
                email.LoadHtml(Resources.header + Resources.passwordResetEmail);
                HtmlNode resetLink = email.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" passwordReset \")]");
                resetLink.Attributes["href"].Value = Resources.URL + "?ResetToken=" + token;
                HtmlNode resetUrl = email.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" resetURL \")]");
                resetUrl.Attributes["href"].Value = Resources.URL + "?ResetToken=" + token;
                resetUrl.InnerHtml = Resources.URL + "?ResetToken=" + token;
                HtmlNode homePage = email.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" changePassword \")]");
                homePage.Attributes["href"].Value = Resources.URL;
                email.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userNotFound \")]").Remove();
                return email.DocumentNode.OuterHtml;
            }
            public static string GenerateEmail()
            {
                HtmlDocument email = new HtmlDocument();
                email.LoadHtml(Resources.header + Resources.passwordResetEmail);
                HtmlNode found = email.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userFound \")]");
                found.Remove();
                HtmlNode notfound = email.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userNotFound \")]");
                return email.DocumentNode.OuterHtml;
            }
        }
    }
}