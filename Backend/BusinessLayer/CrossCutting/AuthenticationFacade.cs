using System;
using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.BusinessLayer.CrossCutting
{
    /// <summary>
    /// A facade class responsible for managing user authentication state within the Kanban system.
    /// It keeps track of which users are currently logged into the system.
    /// </summary>
    internal class AuthenticationFacade
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private HashSet<string> loggedInUsers;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationFacade"/> class.
        /// Sets up the internal data structures required to track user sessions.
        /// </summary>
        /// <exception cref="NotImplementedException">Thrown because the method is not yet implemented.</exception>
        public AuthenticationFacade()
        {
            this.loggedInUsers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks whether a specific user is currently logged into the system.
        /// </summary>
        /// <param name="email">The email address of the user to check.</param>
        /// <returns><c>true</c> if the user is currently logged in; otherwise, <c>false</c>.</returns>
        public bool IsLoggedIn(string email)
        {
            return loggedInUsers.Contains(email);
        }

        /// <summary>
        /// Authenticates a user and records their state as logged in.
        /// </summary>
        /// <param name="email">The email address of the user to log in.</param>
        public void Login(string email)
        {
            loggedInUsers.Add(email);
        }

        /// <summary>
        /// Terminates a user's active session and removes them from the logged-in state.
        /// </summary>
        /// <param name="email">The email address of the user to log in.</param>
        public void Logout(string email)
        {
            if (!IsLoggedIn(email))
            {
                log.WarnFormat("Failed logout attempt for email '{0}'. Reason: User is not currently logged in.", email);
                throw new InvalidOperationException("User is not currently logged in.");
            }
            loggedInUsers.Remove(email);
        }
    }
}