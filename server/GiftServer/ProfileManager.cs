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
                // Populate page with information:
                return NavigationManager.NavigationBar(userID) + Resources.profile;
            }
        }
    }
}