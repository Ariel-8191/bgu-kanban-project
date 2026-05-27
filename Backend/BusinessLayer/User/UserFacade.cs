using System;
using System.Collections.Generic;

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
                string message = "Cannot register a new user because the given email is null.";
                log.Warn(message);
                throw new KanbanValidationException(message);
            }
            if (users.ContainsKey(email))
            {
                string message = $"Cannot register a new user because the email '{email}' already exists in the system.";
                log.Warn(message);
                throw new KanbanConflictException(message);
            }

            UserBL user = new UserBL(email, password);
            users.Add(email, user);
            authenticationFacade.Login(email);

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
                string message = "Cannot log in a user because the given email is null.";
                log.Warn(message);
                throw new KanbanValidationException(message);
            }
            if (!users.ContainsKey(email))
            {
                string message = $"Cannot log in the user '{email}' because there is no user with that email.";
                log.Warn(message);
                throw new KanbanAuthenticationException(message);
            }

            UserBL user = users[email];
            if (user.CheckPassword(password))
            {
                authenticationFacade.Login(email);
                return user;
            }
            else
            {
                string message = $"Cannot log in the user '{email}' because the password is incorrect.";
                log.Warn(message);
                throw new KanbanAuthenticationException(message);
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
                string message = "Cannot log out a user because the given email is null.";
                log.Warn(message);
                throw new KanbanValidationException(message);
            }
            if (!users.ContainsKey(email))
            {
                string message = $"Cannot log out the user '{email}' because there is no user with that email.";
                log.Warn(message);
                throw new KanbanAuthenticationException(message);
            }

            UserBL user = users[email];
            authenticationFacade.Logout(email);
            return user;
        }
    }
}
