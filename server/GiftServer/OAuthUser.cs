using System;
using System.Net.Mail;

namespace GiftServer
{
    namespace Data
    {
        /// <summary>
        /// A User who has logged in from an OAuth source
        /// </summary>
        public abstract class OAuthUser
        {
            /// <summary>
            /// The name of this user
            /// </summary>
            /// <remarks>
            /// This does not differentiate between first, last, etc.
            /// </remarks>
            public abstract string Name
            {
                get;
            }
            /// <summary>
            /// The locale of this user
            /// </summary>
            /// <remarks>
            /// This will be a two character string that represents the **language** - en, fr, etc.
            /// </remarks>
            public abstract string Locale
            {
                get;
            }
            /// <summary>
            /// The Email of this user
            /// </summary>
            public abstract MailAddress Email
            {
                get;
            }
            /// <summary>
            /// The picture of this user, as a byte array
            /// </summary>
            public abstract byte[] Picture
            {
                get;
            }
        }
    }
}