using GiftServer.Properties;
using System;

namespace GiftServer
{
    namespace Html
    {
        public class NavigationManager
        {
            public static string NavigationBar(long userID)
            {
                return Resources.header + Resources.navigationBar;
            }
        }
    }
}
