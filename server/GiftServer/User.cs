using System;
using GiftServer.Security;
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

            public User(string email, string password) : this("Jane", "Doe", email, password, 1, "") { }
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

        }
    }
}