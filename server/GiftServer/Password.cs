using GiftServer.Exceptions;
using System;
using System.Security.Cryptography;

namespace GiftServer
{
    namespace Security
    {
        public sealed class Password
        {
            /// <summary>
            /// Size of salt
            /// </summary>
            private const int SaltSize = 16;

            /// <summary>
            /// Size of hash
            /// </summary>
            private const int HashSize = 20;
            private readonly byte[] _hash;
            private readonly byte[] _salt;
            public string Hash
            {
                get
                {
                    return Convert.ToBase64String(_hash);
                }
            }
            public string Salt
            {
                get
                {
                    return Convert.ToBase64String(_salt);
                }
            }
            public readonly int Iterations;

            public Password(string hash, string salt, int iterations)
            {
                _hash = Convert.FromBase64String(hash);
                _salt = Convert.FromBase64String(salt);
                Iterations = iterations;
            }

            public Password(string password) : this(password, 10000) { }
            public Password(string password, int iterations)
            {
                this.Iterations = iterations;
                if (password == null || password.Length <= 3)
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

            public static bool Verify(string password, string hashed)
            {
                return false;
            }
        }
    }
}
