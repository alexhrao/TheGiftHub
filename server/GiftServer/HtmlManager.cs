using System;
using HtmlAgilityPack;
using GiftServer.Properties;
namespace GiftServer
{
    namespace DataManipulation
    {
        public static class HtmlManager
        {
            public static string FailLogin()
            {
                HtmlDocument login = new HtmlDocument();
                login.LoadHtml(Resources.header + Resources.login);
                HtmlNode alert = login.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                alert.AddClass("alert-danger");
                HtmlNode message = HtmlNode.CreateNode("<p><strong>Uh-Oh...</strong> Looks like we didn't recognize that Username/Password pair. Try again or, <a data-toggle=\"modal\" href=\"#resetPassword\">Reset your Password</a></p>");
                HtmlNodeCollection children = new HtmlNodeCollection(alert);
                children.Add(message);
                alert.AppendChildren(children);
                return login.DocumentNode.OuterHtml;
            }
            public static string SuccessSignup()
            {
                HtmlDocument login = new HtmlDocument();
                login.LoadHtml(Resources.header + Resources.login);
                HtmlNode alert = login.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                alert.AddClass("alert-success");
                HtmlNode message = HtmlNode.CreateNode("<p><strong>Success!</strong> Please login below</p>");
                HtmlNodeCollection children = new HtmlNodeCollection(alert);
                children.Add(message);
                alert.AppendChildren(children);
                return login.DocumentNode.OuterHtml;
            }
            public static string ResetPasswordSent()
            {
                HtmlDocument login = new HtmlDocument();
                login.LoadHtml(Resources.header + Resources.login);
                HtmlNode alert = login.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                alert.AddClass("alert-success");
                HtmlNode message = HtmlNode.CreateNode("<p><strong>Recovery email sent</strong> - check your inbox</p>");
                HtmlNodeCollection children = new HtmlNodeCollection(alert);
                children.Add(message);
                alert.AppendChildren(children);
                return login.DocumentNode.OuterHtml;
            }
            public static string SuccessResetPassword()
            {
                HtmlDocument login = new HtmlDocument();
                login.LoadHtml(Resources.header + Resources.login);
                HtmlNode alert = login.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                alert.AddClass("alert-success");
                HtmlNode message = HtmlNode.CreateNode("<p><strong>Password Reset</strong> Please login below with your new password</p>");
                HtmlNodeCollection children = new HtmlNodeCollection(alert);
                children.Add(message);
                alert.AppendChildren(children);
                return login.DocumentNode.OuterHtml;
            }
            public static string CreateReset(long userID)
            {
                HtmlDocument pg = new HtmlDocument();
                pg.LoadHtml(Resources.header + Resources.passwordReset);
                HtmlNode hidden = pg.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@name), \" \"), \" userID \")]");
                hidden.Attributes["value"].Value = Convert.ToString(userID);
                return pg.DocumentNode.OuterHtml;
            }
        }
    }
}