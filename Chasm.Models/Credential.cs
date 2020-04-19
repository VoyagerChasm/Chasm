using System;

namespace Chasm.Models
{

    /// <summary>
    /// Map username and password for generic use
    /// </summary>
    public class Credential
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        public Credential(string username, string password)
        {
            if (username == null)
                throw new ArgumentNullException(nameof(username), $"{nameof(username)} must to be not null");

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException($"{nameof(username)} is not a valid username");

            if (password == null)
                throw new ArgumentNullException(nameof(password), $"{nameof(password)} must to be not null");

            Username = username;
            Password = password;
        }

        public string Username { get; private set; }
        public string Password { get; private set; }

        public override bool Equals(object obj)
        {
            return obj is Credential authentication &&
                   Username == authentication.Username &&
                   Password == authentication.Password;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Username, Password);
        }

        /// <summary>
        /// Username:Password string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Username}:{Password}";
        }
    }
}
