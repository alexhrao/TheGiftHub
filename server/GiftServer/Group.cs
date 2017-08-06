using System;

namespace GiftServer
{
    namespace Data
    {
        public class Group
        {
            public long GroupID = -1;
            public string Name = "Hello World!";
            public long AdminID;

            public Group(long groupID)
            {
                this.GroupID = groupID;
            }
        }
    }
}