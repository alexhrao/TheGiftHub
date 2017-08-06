using System;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using System.Configuration;
using GiftServer.Exceptions;
using GiftServer.Data;
using GiftServer.Properties;
using System.Net.Mail;
using System.Net;
using GiftServer.Html;

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
            public static User GetUser(string token)
            {
                User ret;
                DateTime timestamp;
                // Hash and query DB for hash; if not found, throw error. Otherwise, get the user
                string hashed = PasswordReset.ComputeHash(token);
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
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
                                ret = new User(Convert.ToInt64(reader["UserID"]));
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
            public static void ResetPassword(User user, string password)
            {
                user.UpdatePassword(password);
            }
            public static void SendRecoveryEmail(string emailAddress)
            {
                long id = -1;
                string token = "";
                string body;
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["Development"].ConnectionString))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "SELECT users.UserID FROM users WHERE users.UserEmail = @email;";
                        cmd.Parameters.AddWithValue("@email", emailAddress);
                        cmd.Prepare();
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Get data:
                                id = Convert.ToInt64(reader["UserID"]);
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
                    if (id != -1)
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
                MailMessage email = new MailMessage(new MailAddress("The Gift Hub<support@TheGiftHub.org>"), new MailAddress(emailAddress));
                email.Body = body;
                email.Subject = "Password Reset";
                email.IsBodyHtml = true;
                using (SmtpClient sender = new SmtpClient("smtp.gmail.com", 587))
                {
                    sender.EnableSsl = true;
                    sender.DeliveryMethod = SmtpDeliveryMethod.Network;
                    sender.UseDefaultCredentials = false;
                    sender.Credentials = new NetworkCredential("support@thegifthub.org", Resources.emailPassword);
                    sender.Send(email);
                }
            }
        }
    }
}
