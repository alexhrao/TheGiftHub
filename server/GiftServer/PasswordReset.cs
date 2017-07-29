using System;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using System.Configuration;
using GiftServer.Exceptions;
using GiftServer.Data;

namespace GiftServer
{
    namespace Security
    {
        public class PasswordReset
        {
            private const int TokenSize = 24;
            /// <summary>
            /// Generate a token suitable to be in the URL.
            /// </summary>
            /// <returns>A cryptographically strong one-time token.</returns>
            public static string GenerateToken()
            {
                byte[] token;
                using (RNGCryptoServiceProvider crypt = new RNGCryptoServiceProvider())
                {
                    crypt.GetBytes(token = new byte[TokenSize]);
                }
                return Convert.ToBase64String(token).Replace('+', '_');
            }
            public static string ComputeHash(string token)
            {
                byte[] hashed = new byte[64];
                using (SHA512Managed hasher = new SHA512Managed())
                {
                    hashed = hasher.ComputeHash(Convert.FromBase64String(token.Replace('_', '+')));
                }
                return Convert.ToBase64String(hashed);
            }
            private static bool VerifyToken(string token, string hashed)
            {
                return hashed.Equals(PasswordReset.ComputeHash(token));
            }
            public static long GetUser(string token)
            {
                long ret;
                DateTime timestamp;
                // Hash and query DB for hash; if not found, throw error. Otherwise, get the user
                string hashed = PasswordReset.ComputeHash(token);
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT passwordResets.UserID, passwordResets.TimeCreated FROM passwordResets WHERE passwordResets.ResetHash = @hash;";
                        cmd.Parameters.AddWithValue("@hash", hashed);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                // Throw UserNotFound
                                throw new UserNotFoundException(Convert.FromBase64String(token));
                            }
                            else
                            {
                                reader.Read();
                                ret = Convert.ToInt64(reader["UserID"]);
                                timestamp = (DateTime)(reader["TimeCreated"]);
                                if (timestamp.AddMinutes(30) < DateTime.Now)
                                {
                                    // More than 30 minutes have passed; throw error:
                                    throw new PasswordResetTimeoutException();
                                }
                            }
                        }
                    }
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM passwordResets WHERE passwordResets.ResetHash = @hash;";
                        cmd.Parameters.AddWithValue("@hash", hashed);
                        cmd.Prepare();
                        // Finish with reader, then do this
                        cmd.ExecuteNonQuery();
                    }
                    return ret;
                }
            }
            public static void ResetPassword(long userID, string password)
            {
                User user = new User(userID);
                user.passwordHash = PasswordHash.Hash(password);
                user.Update();
            }
        }
    }
}
