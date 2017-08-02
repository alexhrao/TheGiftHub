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
            public static string ProfilePage(long userID)
            {
                // Create user from id
                User user = new User(userID);
                return ProfilePage(user);
            }

            public static string ProfilePage(User user)
            {
                HtmlDocument profile = new HtmlDocument();
                profile.LoadHtml(NavigationManager.NavigationBar(user) + Resources.profile);
                // Set src of image:
                HtmlNode img = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userImage \")]");
                img.Attributes["src"].Value = user.GetImage();
                HtmlNode name = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" userName \")]");
                name.InnerHtml = HttpUtility.HtmlEncode(user.firstName + " " + user.lastName);
                HtmlNode timeMember = profile.DocumentNode.SelectSingleNode("//*[contains(concat(\" \", normalize-space(@id), \" \"), \" timeMember \")]");
                timeMember.InnerHtml = HttpUtility.HtmlEncode("Member since " + user.dateJoined.ToString("MMMM d, yyyy"));
                return profile.DocumentNode.OuterHtml;
            }
        }
    }
}