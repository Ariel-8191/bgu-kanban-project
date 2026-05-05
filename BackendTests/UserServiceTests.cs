using IntroSE.Kanban.Backend.ServiceLayer;
using System.Text.Json;

namespace BackendTests
{
    internal class UserServiceTests
    {
        private ServiceFactory _serviceFactory;
        private UserService _userService;

        private readonly string _validEmail = "test@example.com";
        private readonly string _validPassword = "Password123";

        /// <summary>
        /// Resets the testing environment.
        /// Must be called at the beginning of every test method.
        /// </summary>
        private void SetUp()
        {
            _serviceFactory = new ServiceFactory();
            _userService = _serviceFactory.UserService;
        }

        // =========================================================================
        // Register Tests
        // =========================================================================

        /// <summary>
        /// Tests that a new user with valid credentials can successfully register.
        /// This function tests Requirement 6.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool Register_ValidUser_Success()
        {
            SetUp();
            string jsonResponse = _userService.Register(_validEmail, _validPassword);
            Response<UserSL> response = JsonSerializer.Deserialize<Response<UserSL>>(jsonResponse)!;
            return string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests that registering a user with a null email is handled as a malformed input error.
        /// This function tests Requirements 6 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool Register_NullEmail_Failure()
        {
            SetUp();
            string jsonResponse = _userService.Register(null, _validPassword);
            Response<UserSL> response = JsonSerializer.Deserialize<Response<UserSL>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }


        /// <summary>
        /// Tests that registering a user with a null password is handled as a malformed input error.
        /// This function tests Requirements 6 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool Register_NullPassword_Failure()
        {
            SetUp();
            string jsonResponse = _userService.Register(_validEmail, null);
            Response<UserSL> response = JsonSerializer.Deserialize<Response<UserSL>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests that registering a user with an invalid password is handled as a malformed input error.
        /// This function tests Requirements 2, 6 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool Register_InvalidPassword_Failure()
        {
            SetUp();
            string jsonResponse = _userService.Register(_validEmail, "ILoveEines");
            Response<UserSL> response = JsonSerializer.Deserialize<Response<UserSL>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests that registering a user with an invalid Email is handled as a malformed input error.
        /// This function tests Requirements 1, 6 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool Register_InvalidEmail_Failure()
        {
            SetUp();
            string jsonResponse = _userService.Register("OrMendellin", _validPassword);
            Response<UserSL> response = JsonSerializer.Deserialize<Response<UserSL>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to register with an email that is already taken.
        /// This function tests Requirements 3 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool Register_ExistingEmail_Failure()
        {
            SetUp();
            _userService.Register(_validEmail, _validPassword);
            string jsonResponse = _userService.Register(_validEmail, "DifferentPass123");
            Response<UserSL> response = JsonSerializer.Deserialize<Response<UserSL>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }


        // =========================================================================
        // Login Tests
        // =========================================================================

        /// <summary>
        /// Tests that an existing user can successfully log in with correct credentials.
        /// This function tests Requirement 7.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool Login_ValidCredentials_Success()
        {
            SetUp();
            _userService.Register(_validEmail, _validPassword);
            _userService.Logout(_validEmail);

            string jsonResponse = _userService.Login(_validEmail, _validPassword);
            Response<UserSL> response = JsonSerializer.Deserialize<Response<UserSL>>(jsonResponse)!;
            return string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests that logging in with a null email is handled as a malformed input error.
        /// This function tests Requirements 7 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool Login_NullEmail_Failure()
        {
            SetUp();
            string jsonResponse = _userService.Login(null, _validPassword);
            Response<UserSL> response = JsonSerializer.Deserialize<Response<UserSL>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to log in with an email that does not exist in the system.
        /// This function tests Requirements 7 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool Login_NonExistingEmail_Failure()
        {
            SetUp();
            string jsonResponse = _userService.Login("nobody@example.com", _validPassword);
            Response<UserSL> response = JsonSerializer.Deserialize<Response<UserSL>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to log in with the wrong password.
        /// This function tests Requirements 7 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool Login_WrongPassword_Failure()
        {
            SetUp();
            _userService.Register(_validEmail, _validPassword);
            _userService.Logout(_validEmail);

            string jsonResponse = _userService.Login(_validEmail, "WrongPassword123");
            Response<UserSL> response = JsonSerializer.Deserialize<Response<UserSL>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }


        // =========================================================================
        // Logout Tests
        // =========================================================================

        /// <summary>
        /// Tests that a logged-in user can successfully log out.
        /// This function tests Requirement 7.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool Logout_ValidUser_Success()
        {
            SetUp();
            _userService.Register(_validEmail, _validPassword);
            string jsonResponse = _userService.Logout(_validEmail);
            Response<UserSL> response = JsonSerializer.Deserialize<Response<UserSL>>(jsonResponse)!;
            return string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests that logging out with a null email is handled as a malformed input error.
        /// This function tests Requirements 7 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool Logout_NullEmail_Failure()
        {
            SetUp();
            string jsonResponse = _userService.Logout(null);
            Response<UserSL> response = JsonSerializer.Deserialize<Response<UserSL>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a logout request is made for an email that is not logged in.
        /// This function tests Requirements 7 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool Logout_NotLoggedIn_Failure()
        {
            SetUp();
            _userService.Register(_validEmail, _validPassword);
            _userService.Logout(_validEmail);

            string jsonResponse = _userService.Logout(_validEmail);
            Response<UserSL> response = JsonSerializer.Deserialize<Response<UserSL>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

    }
}