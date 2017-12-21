using System;
using HtmlAgilityPack;
using GiftServer.Properties;
using System.Globalization;
using System.Resources;
using System.Threading;
using GiftServer.Server;

namespace GiftServer
{
    namespace Html
    {
        public class LoginManager
        {
            ResourceManager ResourceManager;
            public LoginManager(Controller controller)
            {
                ResourceManager = new ResourceManager("GiftServer.HtmlTemplates", typeof(LoginManager).Assembly);
            }
            public string Login()
            {
                return ResourceManager.GetString("header") + ResourceManager.GetString("login");
            }
            public string FailLogin()
            {
                HtmlDocument login = new HtmlDocument();
                login.LoadHtml(HtmlTemplates.header + HtmlTemplates.login);
                HtmlNode alert = login.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                alert.AddClass("alert-danger in");
                alert.RemoveClass("hidden");
                HtmlNode message = HtmlNode.CreateNode("<p><strong>Uh-Oh...</strong> Looks like we didn't recognize that Username/Password pair."
                                                        + " Try again or <a data-toggle=\"modal\" href=\"#resetPassword\">Reset your Password</a></p>");
                HtmlNodeCollection children = new HtmlNodeCollection(alert);
                children.Add(message);
                alert.AppendChildren(children);
                return login.DocumentNode.OuterHtml;
            }
            public string SuccessSignup()
            {
                HtmlDocument login = new HtmlDocument();
                login.LoadHtml(HtmlTemplates.header + HtmlTemplates.login);
                HtmlNode alert = login.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                alert.AddClass("alert-success in");
                alert.RemoveClass("hidden");
                HtmlNode message = HtmlNode.CreateNode("<p><strong>Success!</strong> Please login below</p>");
                HtmlNodeCollection children = new HtmlNodeCollection(alert);
                children.Add(message);
                alert.AppendChildren(children);
                return login.DocumentNode.OuterHtml;
            }
        }
    }
}