using GiftServer.Server;
using System;
using System.Resources;

namespace GiftServer
{
    namespace Html
    {
        public class GroupManager
        {
            private ResourceManager ResourceManager;

            public GroupManager(Controller controller)
            {
                ResourceManager = new ResourceManager("GiftServer.HtmlTemplates", typeof(GroupManager).Assembly);
            }
        }
    }
}