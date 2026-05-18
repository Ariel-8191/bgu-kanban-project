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
        private HashSet<string> loggedInUsers;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationFacade"/> class.
        /// Sets up the internal data structures required to track user sessions.
        /// </summary>
        /// <exception cref="NotImplementedException">Thrown because the method is not yet implemented.</exception>
        public AuthenticationFacade()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether a specific user is currently logged into the system.
        /// </summary>
        /// <param name="email">The email address of the user to check.</param>
        /// <returns><c>true</c> if the user is currently logged in; otherwise, <c>false</c>.</returns>
        /// <exception cref="NotImplementedException">Thrown because the method is not yet implemented.</exception>
        public bool IsLoggedIn(string email)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Authenticates a user and records their state as logged in.
        /// (Note: In a complete implementation, this method typically accepts parameters such as email and password).
        /// </summary>
        /// <exception cref="NotImplementedException">Thrown because the method is not yet implemented.</exception>
        public void Login()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Terminates a user's active session and removes them from the logged-in state.
        /// (Note: In a complete implementation, this method typically accepts a parameter such as the user's email to identify who is logging out).
        /// </summary>
        /// <exception cref="NotImplementedException">Thrown because the method is not yet implemented.</exception>
        public void Logout()
        {
            throw new NotImplementedException();
        }
    }
}