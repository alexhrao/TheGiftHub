using GiftServer.Data;
using GiftServer.Exceptions;
using System;
using System.Security.Cryptography;
using System.Web;
using System.Xml;

namespace GiftServer
{
    namespace Security
    {
        /// <summary>
        /// A Password
        /// </summary>
        /// <remarks>
        /// Note that we never store the actual text of a password, only its hash! Also note that this can NOT be fetched from the database via this class.
        /// </remarks>
        public sealed class Password : IFetchable
        {
            /// <summary>
            /// Size of salt
            /// </summary>
            public const int SaltSize = 16;

            /// <summary>
            /// Size of hash
            /// </summary>
            public const int HashSize = 20;
            private readonly byte[] _hash;
            private readonly byte[] _salt;
            /// <summary>
            /// Get the hash for this password as a Base64 String
            /// </summary>
            public string Hash
            {
                get
                {
                    return Convert.ToBase64String(_hash);
                }
            }
            /// <summary>
            /// Get the salt for this hash as a Base64 String
            /// </summary>
            public string Salt
            {
                get
                {
                    return Convert.ToBase64String(_salt);
                }
            }
            /// <summary>
            /// Get the number of iterations
            /// </summary>
            public readonly int Iterations;
            /// <summary>
            /// Create a password from existing Hash, Salt, and iteratiosn
            /// </summary>
            /// <param name="hash">The existing hash</param>
            /// <param name="salt">The existing salt</param>
            /// <param name="iterations">The number of iterations used to generate the password</param>
            public Password(string hash, string salt, int iterations)
            {
                _hash = Convert.FromBase64String(hash);
                _salt = Convert.FromBase64String(salt);
                Iterations = iterations;
            }
            /// <summary>
            /// A convenience method used to generate a password with 10,000 iterations
            /// </summary>
            /// <param name="password">The password to hash</param>
            public Password(string password) : this(password, 10000) { }
            /// <summary>
            /// Create a password using the given number of iterations
            /// </summary>
            /// <param name="password">The password to hash</param>
            /// <param name="iterations">The number of iterations to use</param>
            public Password(string password, int iterations)
            {
                this.Iterations = iterations;
                if (password == null)
                {
                    throw new InvalidPasswordException();
                }
                _salt = new byte[SaltSize];
                using (RNGCryptoServiceProvider crypt = new RNGCryptoServiceProvider())
                {
                    crypt.GetBytes(_salt);
                }
                _hash = new Rfc2898DeriveBytes(password, _salt, iterations).GetBytes(HashSize);
            }
            /// <summary>
            /// Check if the two passwords are indeed the same.
            /// </summary>
            /// <param name="password">The password to check</param>
            /// <returns>True if the password matches</returns>
            public bool Verify(string password)
            {
                byte[] hash = new Rfc2898DeriveBytes(password, _salt, Iterations).GetBytes(HashSize);
                for (int i = 0; i < HashSize; i++)
                {
                    if (hash[i] != _hash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            /// <summary>
            /// Fetch the contents of this Password. _Do not use this - it will return an empty document._
            /// </summary>
            /// <remarks>
            /// This is deprecated - the user should never have a need to request any user's password.
            /// </remarks>
            /// <returns>An XmlDocument with no information.</returns>
            public XmlDocument Fetch()
            {
                XmlDocument info = new XmlDocument();
                return info;
                /*
                XmlElement container = info.CreateElement("password");
                info.AppendChild(container);

                XmlElement hash = info.CreateElement("hash");
                hash.InnerText = Hash;
                XmlElement salt = info.CreateElement("salt");
                salt.InnerText = Salt;
                XmlElement iterations = info.CreateElement("iterations");
                iterations.InnerText = Iterations.ToString() ;

                container.AppendChild(hash);
                container.AppendChild(salt);
                container.AppendChild(iterations);

                return info;
                */
            }
        }
    }
}
