using IntroSE.Kanban.Backend.ServiceLayer;
using System.Text.Json;

namespace BackendTests
{
    internal class DataManagementTests
    {
        private ServiceFactory _serviceFactory;
        private UserService _userService;
        private BoardService _boardService;

        private readonly string _testEmail = "persisted@example.com";
        private readonly string _testPassword = "Password123";
        private readonly string _testBoardName = "Persisted Board";

        /// <summary>
        /// Resets the testing environment. 
        /// Because these tests test persistence, we wipe the database at the start of each test.
        /// </summary>
        private void SetUp()
        {
            _serviceFactory = new ServiceFactory();
            _serviceFactory.DeleteData();
            _userService = _serviceFactory.UserService;
            _boardService = _serviceFactory.BoardService;
        }

        // =========================================================================
        // LoadData Tests
        // =========================================================================

        /// <summary>
        /// Tests that data is successfully loaded from the persistent storage after a system restart.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool LoadData_ValidData_Success()
        {
            SetUp();

            _userService.Register(_testEmail, _testPassword);
            _boardService.CreateBoard(_testEmail, _testBoardName);
            _userService.Logout(_testEmail);

            _serviceFactory = new ServiceFactory();
            _userService = _serviceFactory.UserService;
            _boardService = _serviceFactory.BoardService;

            try
            {
                _serviceFactory.LoadData();
            }
            catch
            {
                return false;
            }

            string loginJson = _userService.Login(_testEmail, _testPassword);
            Response<UserSL> response = JsonSerializer.Deserialize<Response<UserSL>>(loginJson)!;

            return string.IsNullOrEmpty(response.ErrorMessage);
        }

        // =========================================================================
        // DeleteData Tests
        // =========================================================================

        /// <summary>
        /// Tests that deleting data completely wipes the database and resets the system state.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool DeleteData_ClearsAllData_Success()
        {
            SetUp();

            _userService.Register(_testEmail, _testPassword);
            _userService.Logout(_testEmail);

            try
            {
                _serviceFactory.DeleteData();
            }
            catch
            {
                return false;
            }

            string loginJson = _userService.Login(_testEmail, _testPassword);
            Response<UserSL> response = JsonSerializer.Deserialize<Response<UserSL>>(loginJson)!;

            return !string.IsNullOrEmpty(response.ErrorMessage);
        }
    }
}