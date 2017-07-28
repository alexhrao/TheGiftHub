using System;
using System.Security.Cryptography;

namespace GiftServer
{
    namespace Security
    {
        public class PasswordReset
        {
            private const int TokenSize = 24;
            public static string GenerateToken()
            {
                byte[] token;
                using (RNGCryptoServiceProvider crypt = new RNGCryptoServiceProvider())
                {
                    crypt.GetBytes(token = new byte[TokenSize]);
                }
                return Convert.ToBase64String(token);
            }
            public static string ComputeHash(string token)
            {
                byte[] hashed = new byte[64];
                using (SHA512Managed hasher = new SHA512Managed())
                {
                    hashed = hasher.ComputeHash(Convert.FromBase64String(token));
                }
                return Convert.ToBase64String(hashed);
            }
            public static bool VerifyToken(string token, string hashed)
            {
                return hashed.Equals(PasswordReset.ComputeHash(token));
            }
        }
    }
}
