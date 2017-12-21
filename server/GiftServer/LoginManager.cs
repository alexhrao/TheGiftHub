using System;
using HtmlAgilityPack;
using GiftServer.Properties;
using System.Globalization;
using System.Resources;
using System.Threading;
using GiftServer.Server;
using GiftServer.Exceptions;

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
                Thread.CurrentThread.CurrentUICulture = controller.Culture;
            }
            public string Login()
            {
                return HtmlManager.GetString("header") + HtmlManager.GetString("login");
            }
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
        }
    }
}