using System;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// Provides service layer functionality for user management, including registration, login, and logout operations.
    /// </summary>
    public class UserService
    {
        private UserFacade userFacade;

        /// <summary>
        /// Intializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userFacade"></param>
        public UserService(UserFacade userFacade)
        {
            userFacade = userFacade;
        }

        /// <summary>
        /// This method registers a new user to the system.
        /// </summary>
        /// <param name="email">The user email address, used as the username for logging the system.</param>
        /// <param name="password">The user password.</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string Register(string email, string password)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        ///  This method logs in an existing user.
        /// </summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <returns>A response with the user's email, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string Login(string email, string password)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// This method logs out a logged in user. 
        /// </summary>
        /// <param name="email">The email of the user to log out</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string Logout(string email)
        {
            throw new NotImplementedException();
        }

    }
}
