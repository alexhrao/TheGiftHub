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
            ResourceManager HtmlManager;
            ResourceManager StringManager;
            public LoginManager(Controller controller)
            {
                HtmlManager = new ResourceManager("GiftServer.HtmlTemplates", typeof(LoginManager).Assembly);
                StringManager = new ResourceManager("GiftServer.Strings", typeof(LoginManager).Assembly);
            }
            public string Login()
            {
                return HtmlManager.GetString("header") + HtmlManager.GetString("login");
            }
            public string FailLogin()
            {
                HtmlDocument login = new HtmlDocument();
                login.LoadHtml(HtmlManager.GetString("header") + HtmlManager.GetString("login"));
                HtmlNode alert = login.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                alert.AddClass("alert-danger in");
                alert.RemoveClass("hidden");
                HtmlNode message = HtmlNode.CreateNode(StringManager.GetString("loginFailed"));
                HtmlNodeCollection children = new HtmlNodeCollection(alert);
                children.Add(message);
                alert.AppendChildren(children);
                return login.DocumentNode.OuterHtml;
            }
            public string SuccessSignup()
            {
                HtmlDocument login = new HtmlDocument();
                login.LoadHtml(HtmlManager.GetString("header") + HtmlManager.GetString("login"));
                HtmlNode alert = login.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@class), \" \"), \" alert \")]");
                alert.AddClass("alert-success in");
                alert.RemoveClass("hidden");
                HtmlNode message = HtmlNode.CreateNode(StringManager.GetString("signupSuccess"));
                HtmlNodeCollection children = new HtmlNodeCollection(alert);
                children.Add(message);
                alert.AppendChildren(children);
                return login.DocumentNode.OuterHtml;
            }
        }
    }
}