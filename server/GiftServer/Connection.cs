using GiftServer.Security;
using System;
using System.Net;

namespace GiftServer
{
    namespace Server
    {
        public class Connection
        {
            public class UserInformation
            {
                public readonly ulong UserId;
                public readonly string Hash;
                public UserInformation(ulong id)
                {
                    UserId = id;
                    Hash = new Password(id.ToString("00000000")).Hash;
                }
            }
            public UserInformation Info;
            public IPEndPointCollection Ends;
            public Connection(ulong userId)
            {
                this.Info = new UserInformation(userId);
                this.Ends = new IPEndPointCollection();
            }
        }
    }
}