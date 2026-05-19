using IntroSE.Kanban.Backend.BusinessLayer.CrossCutting;
using System;
using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.BusinessLayer.User
{
    /// <summary>
    /// Provides a simplified interface for managing user authentication and operations.
    /// </summary>
    internal class UserFacade
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<string, UserBL> users;
        private AuthenticationFacade authenticationFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserFacade"/> class.
        /// </summary>
        /// <param name="authenticationFacade">The authentication facade used to verify users.</param>
        public UserFacade(AuthenticationFacade authenticationFacade)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="email">The email address of the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <returns>A <see cref="UserBL"/> object representing the newly registered user.</returns>
        public UserBL Register(string email, string password)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Authenticates a user and logs them into the system.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>A <see cref="UserBL"/> object representing the logged-in user.</returns>
        public UserBL Login(string email, string password)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Logs out an authenticated user from the system.
        /// </summary>
        /// <param name="email">The email address of the user to log out.</param>
        public void Logout(string email)
        {
            throw new NotImplementedException();
        }
    }
}
