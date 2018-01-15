using GiftServer.Server;
using System;
using System.Resources;
using System.Threading;

namespace GiftServer
{
    namespace HtmlManager
    {
        /// <summary>
        /// The manager for all group-related HTML pages
        /// @DEPRECATED
        /// </summary>
        public class GroupManager
        {
            private ResourceManager ResourceManager;
            /// <summary>
            /// Create a new GroupManager with this controller.
            /// </summary>
            /// <param name="controller">The controller for this thread</param>
            public GroupManager(Controller controller)
            {
                Thread.CurrentThread.CurrentUICulture = controller.Culture;
                Thread.CurrentThread.CurrentCulture = controller.Culture;
                ResourceManager = new ResourceManager("GiftServer.HtmlTemplates", typeof(GroupManager).Assembly);
            }
        }
    }
}