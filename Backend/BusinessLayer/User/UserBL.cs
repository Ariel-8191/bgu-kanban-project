using System;

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
            get { return _password; }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserBL"/> class.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="password">The password for the user.</param>
        public UserBL(string email, string password)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method checks if the provided password matches the user's current password.
        /// </summary>
        /// <param name="password">The password string to validate.</param>
        /// <returns>True if the password matches; otherwise, false.</returns>
        public bool CheckPassword(string password)
        {
            throw new NotImplementedException();
        }


    }
}
