using System;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using System.Configuration;
using GiftServer.Exceptions;
using GiftServer.Data;
using GiftServer.Properties;
using System.Net.Mail;
using System.Net;
using GiftServer.HtmlManager;
using GiftServer.Server;

namespace GiftServer
{
    namespace Security
    {
        /// <summary>
        /// Manages the reset of a password, from a security standpoint
        /// </summary>
        public static class PasswordReset
        {
            /// <summary>
            /// How big a token should be
            /// </summary>
            public const int TokenSize = 24;
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
                return WebServer.SanitizeBase64(Convert.ToBase64String(token));
            }
            /// <summary>
            /// Computes the hash from a token
            /// </summary>
            /// <param name="token">The token to hash</param>
            /// <returns>A hashed token</returns>
            public static string ComputeHash(string token)
            {
                byte[] hashed = new byte[64];
                using (SHA512Managed hasher = new SHA512Managed())
                {
                    hashed = hasher.ComputeHash(Convert.FromBase64String(WebServer.DesanitizeBase64(token)));
                }
                return Convert.ToBase64String(hashed);
            }
            private static bool VerifyToken(string token, string hashed)
            {
                return hashed.Equals(ComputeHash(token));
            }
            /// <summary>
            /// Gets a user from a Reset Hash
            /// </summary>
            /// <remarks>
            /// This method will extract the user from a token. If the user isn't found, a UserNotFoundException is thrown; 
            /// if it has been more than 30 minutes since the token was generated, a PasswordResetTimeoutException will be thrown instead.
            /// </remarks>
            /// <param name="token">The User's Token</param>
            /// <returns>Associated User</returns>
            public static User GetUser(string token)
            {
                User ret;
                DateTime timestamp;
                DateTime curr;
                // Hash and query DB for hash; if not found, throw error. Otherwise, get the user
                string hashed = ComputeHash(token);
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT CURRENT_TIMESTAMP AS CurrTime, passwordResets.UserID, passwordResets.TimeCreated FROM passwordResets WHERE passwordResets.ResetHash = @hash;";
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
                                ret = new User(Convert.ToUInt64(reader["UserID"]));
                                timestamp = (DateTime)(reader["TimeCreated"]);
                                curr = (DateTime)(reader["CurrTime"]);
                            }
                        }
                    }
                }
                DeleteResetToken(token);
                if (timestamp.AddMinutes(30) < curr)
                {
                    // More than 30 minutes have passed; throw error:
                    throw new PasswordResetTimeoutException();
                }
                else
                {
                    return ret;
                }
            }
            /// <summary>
            /// Delete a reset token
            /// </summary>
            /// <param name="token">The issued reset token (*not* the hash)</param>
            public static void DeleteResetToken(string token)
            {
                string hashed = ComputeHash(token);
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM passwordResets WHERE passwordResets.ResetHash = @hash;";
                        cmd.Parameters.AddWithValue("@hash", hashed);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            /// <summary>
            /// Sends a reset email notification
            /// </summary>
            /// <param name="emailAddress">The MailAddress to send this to</param>
            /// <param name="ResetManager">The associated ResetManager for this email to send</param>
            public static void SendRecoveryEmail(MailAddress emailAddress, ResetManager ResetManager)
            {
                ulong id = 0;
                string token = "";
                string body = "";
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT users.UserID FROM users WHERE users.UserEmail = @email;";
                        cmd.Parameters.AddWithValue("@email", emailAddress.Address);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Get data:
                                id = Convert.ToUInt64(reader["UserID"]);
                                token = GenerateToken();
                                body = ResetManager.GenerateEmail(token);
                            }
                            else
                            {
                                // User doesn't exist. Send an email saying so!
                                body = ResetManager.GenerateEmail();
                            }
                        }
                    }
                    if (id != 0)
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = con;
                            cmd.CommandText = "INSERT INTO passwordResets (UserID, ResetHash) VALUES (@uid, @hash);";
                            cmd.Parameters.AddWithValue("@uid", id);
                            cmd.Parameters.AddWithValue("@hash", PasswordReset.ComputeHash(token));
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                try
                {
                    MailMessage email = new MailMessage(new MailAddress("The Gift Hub<support@TheGiftHub.org>"), emailAddress)
                    {
                        Body = body,
                        Subject = "Password Reset",
                        IsBodyHtml = true
                    };
                    using (SmtpClient sender = new SmtpClient("smtp.gmail.com", 587))
                    {
                        sender.EnableSsl = true;
                        sender.DeliveryMethod = SmtpDeliveryMethod.Network;
                        sender.UseDefaultCredentials = false;
                        sender.Credentials = new NetworkCredential("support@thegifthub.org", Constants.emailPassword);
                        sender.Send(email);
                    }
                }
                catch (SmtpException e)
                {
                    DeleteResetToken(token);
                    throw e;
                }
            }
        }
    }
}
