using System;
using System.Collections.Generic;
using System.Security.Authentication;

using IntroSE.Kanban.Backend.BusinessLayer.CrossCutting;

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
            this.users = new Dictionary<string, UserBL>(StringComparer.OrdinalIgnoreCase);
            this.authenticationFacade = authenticationFacade;
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="email">The email address of the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <returns>A <see cref="UserBL"/> object representing the newly registered user.</returns>
        public UserBL Register(string email, string password)
        {
            if (email == null) 
            {
                log.Warn("Failed registration attempt. Reason: Email is null.");
                throw new ArgumentNullException(nameof(email), "Email cannot be null.");
            }
            if (users.ContainsKey(email))
            {
                log.WarnFormat("Failed registration attempt. Reason: The email '{0}' already exists.", email);
                throw new InvalidOperationException("Email already exists in the system.");
            }

            UserBL user = new UserBL(email, password);
            users.Add(email, user);
            authenticationFacade.Login(email);
            log.InfoFormat("User '{0}' successfully created", email);

            return user;
        }

        /// <summary>
        /// Authenticates a user and logs them into the system.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>A <see cref="UserBL"/> object representing the logged-in user.</returns>
        public UserBL Login(string email, string password)
        {
            if (email == null)
            {
                log.Warn("Failed login attempt. Reason: Email is null.");
                throw new ArgumentNullException(nameof(email), "Email cannot be null.");
            }
            if (!users.ContainsKey(email))
            {
                log.WarnFormat("Failed login attempt for email '{0}'. Reason: There is no user with that email.", email);
                throw new AuthenticationException("Email doesn't exist in the system.");
            }

            UserBL user = users[email];
            if (user.CheckPassword(password))
            {
                authenticationFacade.Login(email);
                log.InfoFormat("User '{0}' successfully logged in.", email);
                return user;
            }
            else
            {
                log.WarnFormat("Failed login attempt for email '{0}'. Reason: Incorrect password.", email);
                throw new AuthenticationException("Password is incorrect.");
            }
        }

        /// <summary>
        /// Logs out an authenticated user from the system.
        /// </summary>
        /// <param name="email">The email address of the user to log out.</param>
        /// <returns>A <see cref="UserBL"/> object representing the logged-out user.</returns>
        public UserBL Logout(string email)
        {
            if (email == null)
            {
                log.Warn("Failed logout attempt. Reason: Email is null.");
                throw new ArgumentNullException(nameof(email), "Email cannot be null.");
            }
            if (!users.ContainsKey(email))
            {
                log.WarnFormat("Failed logout attempt for email '{0}'. Reason: There is no user with that email.", email);
                throw new AuthenticationException("Email doesn't exist in the system.");
            }

            UserBL user = users[email];
            authenticationFacade.Logout(email);
            log.InfoFormat("User '{0}' successfully logged out.", email);
            return user;
        }
    }
}
