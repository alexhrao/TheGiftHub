using System;
using System.Security.Cryptography;

namespace GiftServer
{
    namespace Security
    {
        /// <summary>
        /// PasswordHash
        /// Responsible for properly hashing, salting and verifying passwords.
        /// </summary>
        public class PasswordHash
        {
            public const int SALT_SIZE = 16;
            public const int HASH_SIZE = 20;
            public const int HASH_ITER = 10000;
            private readonly byte[] _salt = new byte[SALT_SIZE];
            private readonly byte[] _hash = new byte[HASH_SIZE];
            private readonly RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();

            /// <summary>
            /// Create a PasswordHash object from a password, which can then be used to retreive secure password hashes.
            /// </summary>
            /// <param name="password">The password you want hashed</param>
            public PasswordHash(string password)
            {
                rand.GetBytes(_salt);
                _hash = new Rfc2898DeriveBytes(password, _salt, HASH_ITER).GetBytes(HASH_SIZE);
            }
            /// <summary>
            /// Create a PasswordHash object from an existing hash - useful for comparison & verification
            /// </summary>
            /// <param name="hashBytes">The hashed password you want to box</param>
            public PasswordHash(byte[] hashBytes)
            {
                Array.Copy(hashBytes, 0, _salt = new byte[SALT_SIZE], 0, SALT_SIZE);
                Array.Copy(hashBytes, SALT_SIZE, _hash = new byte[HASH_SIZE], 0, HASH_SIZE);
            }
            /// <summary>
            /// Get the salted & hashed password from this PasswordHash
            /// </summary>
            /// <returns>The hashed password, as a byte array.</returns>
            public byte[] ToArray()
            {
                byte[] hashBytes = new byte[SALT_SIZE + HASH_SIZE];
                Array.Copy(_salt, 0, hashBytes, 0, SALT_SIZE);
                Array.Copy(_hash, 0, hashBytes, SALT_SIZE, HASH_SIZE);
                return hashBytes;
            }
            /// <summary>
            /// Verify that the input password is indeed the same as the stored password.
            /// </summary>
            /// <param name="password">The password you'd like to verify</param>
            /// <returns>True if the passwords match; false otherwise</returns>
            public bool Verify(string password)
            {
                byte[] test = new Rfc2898DeriveBytes(password, _salt, HASH_ITER).GetBytes(HASH_SIZE);
                for (int i = 0; i < HASH_SIZE; i++)
                {
                    if (test[i] != _hash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
