using System;
using IntroSE.Kanban.Backend.BusinessLayer.User;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// Provides service-layer functionality for user management within the application.
    /// </summary>
    public class UserService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private UserFacade userFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userFacade">The user facade instance that will handle the core logic.</param>
        internal UserService(UserFacade userFacade)
        {
            this.userFacade = userFacade;
        }

        /// <summary>
        /// This method registers a new user to the system.
        /// </summary>
        /// <param name="email">The user email address, used as the username for logging the system</param>
        /// <param name="password">The user password</param>
        /// <returns>A JSON representation of the new user</returns>
        public string Register(string email, string password)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        ///  This method logs in an existing user.
        /// </summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <returns>A JSON representation of the logged-in user</returns>
        public string Login(string email, string password)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// This method logs out a logged in user. 
        /// </summary>
        /// <param name="email">The email of the user to log out</param>
        /// <returns>A JSON representation of the logged-out user</returns>
        public string Logout(string email)
        {
            throw new NotImplementedException();
        }

    }
}
