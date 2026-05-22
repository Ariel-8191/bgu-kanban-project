using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace IntroSE.Kanban.Backend.BusinessLayer.User
{
    /// <summary>
    /// Represents a user within the business layer of the Kanban application.
    /// </summary>
    internal class UserBL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Email { get; }

        private string _password;
        private string Password
        {
            get => _password; 
            set
            {
                ValidatePasswordStructure(value);
                _password = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserBL"/> class.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="password">The password for the user.</param>
        public UserBL(string email, string password)
        {
            ValidateEmailStructure(email);
            this.Email = email;
            this.Password = password;
        }

        /// <summary>
        /// This method checks if the provided password matches the user's current password.
        /// </summary>
        /// <param name="password">The password string to validate.</param>
        /// <returns>True if the password matches; otherwise, false.</returns>
        public bool CheckPassword(string password)
        {
            return password == this.Password;
        }

        /// <summary>
        /// Validates that a password string meets the required complexity constraints.
        /// A valid password must be between 6 and 20 characters and contain at least one uppercase letter, one lowercase letter, and one number.
        /// </summary>
        /// <param name="password">The plaintext password string to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the password is null, whitespace, or fails any complexity constraint.</exception>
        private void ValidatePasswordStructure(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                log.Warn("Invalid password: Value is null or whitespace.");
                throw new ArgumentException("Password cannot be null or whitespace.", nameof(password));
            }

            if (password.Length < 6 || password.Length > 20)
            {
                log.Warn($"Invalid password: Invalid length ({password?.Length}).");
                throw new ArgumentException("Password length must be between 6 and 20 characters.", nameof(password));
            }

            if (!password.Any(char.IsUpper))
            {
                log.Warn("Invalid password: Missing uppercase letter.");
                throw new ArgumentException("Password must contain at least one uppercase letter.", nameof(password));
            }

            if (!password.Any(char.IsLower))
            {
                log.Warn("Invalid password: Missing lowercase letter.");
                throw new ArgumentException("Password must contain at least one lowercase letter.", nameof(password));
            }

            if (!password.Any(char.IsDigit))
            {
                log.Warn("Invalid password: Missing number.");
                throw new ArgumentException("Password must contain at least one number.", nameof(password));
            }
        }

        /// <summary>
        /// Validates that an email string conforms to a standard structural format.
        /// </summary>
        /// <param name="email">The raw email string to validate.</param>
        /// <exception cref="ArgumentException">Thrown when the email is null, whitespace, or fails regular expression pattern matching.</exception>
        private void ValidateEmailStructure(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                log.Warn("Invalid email: Email is null or whitespace.");
                throw new ArgumentException("Email cannot be null or whitespace.", nameof(email));
            }

            string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            if (!Regex.IsMatch(email, emailPattern))
            {
                log.Warn($"Invalid email: Invalid email structure.");
                throw new ArgumentException("Invalid email structure.", nameof(email));
            }
        }

    }
}
