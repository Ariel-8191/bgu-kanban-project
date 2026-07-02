using Frontend.Model;
using IntroSE.Kanban.Backend.ServiceLayer;
using System.Text.Json;

namespace Frontend.Controllers
{
    /// <summary>
    /// Frontend controller that wraps the backend <see cref="UserService"/>.
    /// It translates the JSON responses of the service layer into frontend models
    /// and throws an exception whenever the service layer reports an error.
    /// </summary>
    public class UserController
    {
        private readonly UserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userService">The backend user service to delegate to.</param>
        public UserController(UserService userService)
        {
            this.userService = userService;
        }

        /// <summary>
        /// Registers a new user and logs them in.
        /// </summary>
        /// <param name="email">The email of the user to register.</param>
        /// <param name="password">The password of the user to register.</param>
        /// <returns>A <see cref="UserModel"/> representing the newly registered user.</returns>
        /// <exception cref="Exception">Thrown if registration fails.</exception>
        public UserModel Register(string email, string password)
        {
            string json = userService.Register(email, password);
            UserSL user = ParseUser(json);
            return new UserModel(user.Email);
        }

        /// <summary>
        /// Logs in an existing user.
        /// </summary>
        /// <param name="email">The email of the user to log in.</param>
        /// <param name="password">The password of the user to log in.</param>
        /// <returns>A <see cref="UserModel"/> representing the logged-in user.</returns>
        /// <exception cref="Exception">Thrown if the login fails.</exception>
        public UserModel Login(string email, string password)
        {
            string json = userService.Login(email, password);
            UserSL user = ParseUser(json);
            return new UserModel(user.Email);
        }

        /// <summary>
        /// Logs out a logged-in user.
        /// </summary>
        /// <param name="email">The email of the user to log out.</param>
        /// <exception cref="Exception">Thrown if the logout fails.</exception>
        public void Logout(string email)
        {
            string json = userService.Logout(email);
            Response<UserSL> response = JsonSerializer.Deserialize<Response<UserSL>>(json)!;
            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                throw new Exception(response.ErrorMessage);
            }
        }

        /// <summary>
        /// Deserializes a service-layer response holding a user, throwing on error.
        /// </summary>
        /// <param name="json">The JSON response returned from the service layer.</param>
        /// <returns>The user contained in the response.</returns>
        /// <exception cref="Exception">Thrown if the response contains an error message.</exception>
        private UserSL ParseUser(string json)
        {
            Response<UserSL> response = JsonSerializer.Deserialize<Response<UserSL>>(json)!;
            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                throw new Exception(response.ErrorMessage);
            }
            return response.ReturnValue!;
        }
    }
}
