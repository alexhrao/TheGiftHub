using GiftServer.Security;
using System;
using System.Net;
using System.Security.Cryptography;

namespace GiftServer
{
    namespace Server
    {
        /// <summary>
        /// A Connection is a single cable between the server and a specific endpoint
        /// </summary>
        /// <remarks>
        /// A Connection is used to track where a user is logging in from, and what users are currently logged in.
        /// </remarks>
        public class Connection
        {
            /// <summary>
            /// UserInformation is a class that stores small tokens that represent the user.
            /// </summary>
            /// <remarks>
            /// Use this class to keep track of what users are logged in, and from where.
            /// </remarks>
            public class UserInformation
            {
                /// <summary>
                /// The UserID of this logged-in user
                /// </summary>
                public readonly ulong UserId;
                /// <summary>
                /// The Hash for this user - it's random every time this is created.
                /// 
                /// This hash is cryptographically strong.
                /// </summary>
                public readonly string Hash;
                /// <summary>
                /// Create a new UserInformation from the specified user.
                /// </summary>
                /// <param name="id">The User's ID</param>
                public UserInformation(ulong id)
                {
                    UserId = id;
                    using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                    {
                        byte[] randHash = new byte[8];
                        rng.GetNonZeroBytes(randHash);
                        Hash = new Password(Convert.ToBase64String(randHash)).Hash;
                    }
                }
            }
            /// <summary>
            /// This Connection's User Information
            /// </summary>
            public UserInformation Info;
            /// <summary>
            /// The Endpoints this connection has. Note that there can be multiple endpoints!
            /// </summary>
            public IPEndPointCollection Ends;
            /// <summary>
            /// Create a new Connection for the specified user
            /// </summary>
            /// <param name="userId">The User we're looking at</param>
            public Connection(ulong userId)
            {
                this.Info = new UserInformation(userId);
                this.Ends = new IPEndPointCollection();
            }
        }
    }
}