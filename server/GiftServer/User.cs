using System;
using GiftServer.Security;
using MySql.Data.MySqlClient;
using System.Configuration;
using GiftServer.Exceptions;
namespace GiftServer
{
    namespace Data
    {
        public class User : ISynchronizable
        {
            private int _id;
            public string firstName;
            public string lastName;
            public string email;
            public byte[] passwordHash;
            public int theme;
            public string imagePath;
            public User(string email, string password)
            {
                // If this is called, the user already exists in DB; fetch. If it can't find it, through UserNotFoundException. 
                // If found, but password mismatch, throw InvalidPasswordException.
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
                {
                    con.Open();
                    MySqlCommand command = new MySqlCommand();
                    command.Connection = con;
                    command.CommandText = "SELECT [users].[UserID], [users].[firstName], [users].[LastName], [passwords].[passwordHash], [users].[theme], [users].[imagePath] "
                        + "FROM [users] "
                        + "INNER JOIN [passwords] ON [users].[PasswordID] = [passwords].[PasswordID] "
                        + "WHERE [users].[email] = @email;";
                    command.Prepare();
                    command.Parameters.AddWithValue("@email", email);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            // User not found, throw correct exception
                            throw new UserNotFoundException(email);
                        }
                        while (reader.Read())
                        {
                            // Check password
                            PasswordHash correct = new PasswordHash((byte[])(reader["passwordHash"]));
                            if (!correct.Verify(password))
                            {
                                // Not correct, throw new exception!
                                throw new InvalidPasswordException();
                            }
                            _id = (int)(reader["UserId"]);
                            firstName = (string)(reader["FirstName"]);
                            lastName = (string)(reader["LastName"]);
                            // TODO: Add rest of data
                        }
                    }
                }
                
            }
            public User(string firstName, string lastName, string email, string password) : this(firstName, lastName, email, password, 1, "") { }
            public User(string firstName, string lastName, string email, string password, int theme, string imagePath)
            {
                // Fetch userid?
                this.email = email;
                PasswordHash hasher = new PasswordHash(password);
                passwordHash = hasher.ToArray();
                this.firstName = firstName;
                this.lastName = lastName;
                this.theme = theme;
                this.imagePath = imagePath;
            }

            public bool Create()
            {
                return false;
            }
            public bool Update()
            {
                return false;
            }
            public bool Delete()
            {
                return false;
            }

        }
    }
}