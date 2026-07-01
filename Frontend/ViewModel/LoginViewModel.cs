using Frontend.Controllers;
using Frontend.Model;
using System;

namespace Frontend.ViewModel
{
    /// <summary>
    /// The view model backing the login/registration window.
    /// </summary>
    public class LoginViewModel : Notifiable
    {
        private readonly UserController userController;

        private string email;
        /// <summary>
        /// The email typed by the user.
        /// </summary>
        public string Email
        {
            get => email;
            set { email = value; RaisePropertyChanged(); }
        }

        private string errorMessage;
        /// <summary>
        /// The error message shown to the user, empty when there is no error.
        /// </summary>
        public string ErrorMessage
        {
            get => errorMessage;
            set { errorMessage = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
        /// </summary>
        /// <param name="userController">The user controller used to log in and register.</param>
        public LoginViewModel(UserController userController)
        {
            this.userController = userController;
            this.email = string.Empty;
            this.errorMessage = string.Empty;
        }

        /// <summary>
        /// Attempts to log the user in with the current email and the given password.
        /// </summary>
        /// <param name="password">The password entered by the user.</param>
        /// <returns>The logged-in user on success, or <c>null</c> if the login failed.</returns>
        public UserModel? Login(string password)
        {
            ErrorMessage = string.Empty;
            try
            {
                return userController.Login(Email, password);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Attempts to register a new user with the current email and the given password.
        /// On success the user is also logged in.
        /// </summary>
        /// <param name="password">The password entered by the user.</param>
        /// <returns>The registered user on success, or <c>null</c> if the registration failed.</returns>
        public UserModel? Register(string password)
        {
            ErrorMessage = string.Empty;
            try
            {
                return userController.Register(Email, password);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return null;
            }
        }
    }
}
