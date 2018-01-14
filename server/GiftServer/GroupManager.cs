using GiftServer.Server;
using System;
using System.Resources;
using System.Threading;

namespace GiftServer
{
    namespace HtmlManager
    {
        public class GroupManager
        {
            private ResourceManager ResourceManager;

            public GroupManager(Controller controller)
            {
                Thread.CurrentThread.CurrentUICulture = controller.Culture;
                Thread.CurrentThread.CurrentCulture = controller.Culture;
                ResourceManager = new ResourceManager("GiftServer.HtmlTemplates", typeof(GroupManager).Assembly);
            }
        }
    }
}