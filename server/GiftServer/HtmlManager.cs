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
                alert.RemoveClass("alert-success", false);
                alert.RemoveClass("alert-warning", false);
                alert.RemoveClass("alert-primary", false);
                alert.RemoveClass("alert-danger", false);
                alert.AddClass("alert-danger");
                HtmlNode message = HtmlNode.CreateNode("<p><strong>Uh-Oh...</strong> Looks like we didn't recognize that Username/Password pair. Try again or, <a href=\"#\">Reset your Password</a></p>");
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
                alert.RemoveClass("alert-success", false);
                alert.RemoveClass("alert-warning", false);
                alert.RemoveClass("alert-primary", false);
                alert.RemoveClass("alert-danger", false);
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
                HtmlNode message = HtmlNode.CreateNode("<p>Recovery email sent - check your inbox</p>");
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
        }
    }
}