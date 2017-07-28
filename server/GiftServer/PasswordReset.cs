using System;
using System.Security.Cryptography;

namespace GiftServer
{
    namespace Security
    {
        public class PasswordReset
        {
            private const int TokenSize = 24;
            public static byte[] GenerateToken()
            {
                byte[] token;
                using (RNGCryptoServiceProvider crypt = new RNGCryptoServiceProvider())
                {
                    crypt.GetBytes(token = new byte[TokenSize]);
                }
                return token;
            }
            public static string ComputeHash(byte[] token)
            {
                byte[] hashed = new byte[64];
                using (SHA512Managed hasher = new SHA512Managed())
                {
                    hashed = hasher.ComputeHash(token);
                }
                return Convert.ToBase64String(hashed);
            }
            public static bool VerifyToken(string token, string hashed)
            {
                byte[] tok = Convert.FromBase64String(token); 
                // Now hash it!
                string test = PasswordReset.ComputeHash(tok);
                return hashed.Equals(test);
            }
        }
    }
}
