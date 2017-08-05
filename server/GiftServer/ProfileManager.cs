using System;
using HtmlAgilityPack;
using GiftServer.Properties;
using GiftServer.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Web;
namespace GiftServer
{
    namespace Html
    {
        public static class ProfileManager
        {
            public static string ProfilePage(User user)
            {
                // Add Side Navigation Bar (From Dashboard)
                HtmlDocument profile = new HtmlDocument();
                profile.LoadHtml(NavigationManager.NavigationBar(user) + Resources.profile);
                // Set src of image:
                HtmlNode img = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userImage \")]");
                img.Attributes["src"].Value = user.GetImage();
                HtmlNode name = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userName \")]");
                name.InnerHtml = HttpUtility.HtmlEncode(user.firstName + " " + user.lastName);
                HtmlNode timeMember = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" timeMember \")]");
                timeMember.InnerHtml = HttpUtility.HtmlEncode("Member since " + user.dateJoined.ToString("MMMM d, yyyy"));
                HtmlNode email = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" email \")]");
                email.InnerHtml = HttpUtility.HtmlEncode("Email: " + user.email);
                if (user.dob != DateTime.MinValue)
                {
                    HtmlNode birthday = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" birthday \")]");
                    birthday.InnerHtml = HttpUtility.HtmlEncode("Birthday: " + user.dob.ToString("MMMM d, yyyy"));
                }
                HtmlNode theme = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" theme \")]");
                switch (user.theme)
                {
                    case 1:
                        theme.Attributes["style"].Value = "background: red;";
                        break;
                    case 2:
                        theme.Attributes["style"].Value = "background: blue;";
                        break;
                    default:
                        theme.Attributes["style"].Value = "background: red;";
                        break;
                }
                return profile.DocumentNode.OuterHtml;
            }
        }
    }
}