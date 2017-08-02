using GiftServer.Properties;
using System;
using GiftServer.Data;
namespace GiftServer
{
    namespace Html
    {
        public class NavigationManager
        {
            public static string NavigationBar(User user)
            {
                return Resources.header + Resources.navigationBar;
            }
        }
    }
}
