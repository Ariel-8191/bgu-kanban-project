using System;
using System.Linq;
using System.Text.RegularExpressions;
using IntroSE.Kanban.Backend.BusinessLayer.CrossCutting;
using IntroSE.Kanban.Backend.DataAccessLayer;

namespace IntroSE.Kanban.Backend.BusinessLayer.User
{
    /// <summary>
    /// Represents a user within the business layer of the Kanban application.
    /// </summary>
    internal class UserBL
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int minPasswordLength = 6;
        private const int maxPasswordLength = 20;


        private UserDAL userDTO;

        private string _email;
        public string Email {
            get => _email; 
            set
            {
                ValidateEmailStructure(value);
                _email = value;
            }
        }
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
            this.userDTO = new UserDAL(email, password);
            this.Email = email;
            this.Password = password;
            userDTO.Persist();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserBL"/> class using an existing <see cref="UserDAL"/> object.
        /// </summary>
        /// <param name="userDTO">The data access layer object representing the user.</param>
        public UserBL(UserDAL userDTO)
        {
            this.userDTO = userDTO;
            this.Email = userDTO.Email;
            this.Password = userDTO.Password;
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
        private void ValidatePasswordStructure(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                string message = "Password cannot be null or whitespace.";
                log.Warn(message);
                throw new KanbanValidationException(message);
            }

            if (password.Length < minPasswordLength || password.Length > maxPasswordLength)
            {
                string message = $"Password must be between {minPasswordLength} and {maxPasswordLength} characters. Attempted password length: {password.Length}";
                log.Warn(message);
                throw new KanbanValidationException(message);
            }

            if (!password.Any(char.IsUpper))
            {
                string message = "Password must contain at least one uppercase letter.";
                log.Warn(message);
                throw new KanbanValidationException(message);
            }

            if (!password.Any(char.IsLower))
            {
                string message = "Password must contain at least one lowercase letter.";
                log.Warn(message);
                throw new KanbanValidationException(message);
            }

            if (!password.Any(char.IsDigit))
            {
                string message = "Password must contain at least one number.";
                log.Warn(message);
                throw new KanbanValidationException(message);
            }
        }

        /// <summary>
        /// Validates that an email string conforms to a standard structural format.
        /// </summary>
        /// <param name="email">The raw email string to validate.</param>
        private void ValidateEmailStructure(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                string message = "Email cannot be null or whitespace.";
                log.Warn(message);
                throw new KanbanValidationException(message);
            }
            string emailPattern = @"^[a-zA-Z0-9_+&-]+(?:\.[a-zA-Z0-9_+&-]+)*@(?:[a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}$";

            if (!Regex.IsMatch(email, emailPattern))
            {
                string message = "Email must have a valid email structure.";
                log.Warn(message);
                throw new KanbanValidationException(message);
            }

            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);

                if (mailAddress.Address != email.Trim())
                {
                    throw new FormatException("Email format is invalid or contains a display name.");
                }
            }
            catch (FormatException)
            {
                string message = $"Email must have a valid RFC 5322 structure. Attempted email: {email}";
                log.Warn(message);
                throw new KanbanValidationException(message);
            }
        }

    }
}
