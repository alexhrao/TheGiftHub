using System;

namespace GiftServer
{
    namespace Data
    {
        public class Group : ISynchronizable
        {
            public long GroupID = -1;
            public string Name = "Hello World!";
            public long AdminID;

            public Group(long groupID)
            {
                this.GroupID = groupID;
            }

            public bool Create()
            {
                return false;
            }
            public bool Update()
            {
                return false;
            }
            public bool Delete()
            {
                return false;
            }
        }
    }
}