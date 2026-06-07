using IntroSE.Kanban.Backend.ServiceLayer;
using System.Text.Json;

namespace BackendTests
{
    internal class BoardServiceTests
    {
        private ServiceFactory _serviceFactory;
        private UserService _userService;
        private BoardService _boardService;

        private readonly string _testEmail = "test@example.com";
        private readonly string _testPassword = "Password123";
        private readonly string _testBoardName = "Test Board";

        /// <summary>
        /// Resets the testing environment and add a registered user.
        /// Must be called at the beginning of every test method.
        /// </summary>
        private void SetUp()
        {
            _serviceFactory = new ServiceFactory();
            _userService = _serviceFactory.UserService;
            _boardService = _serviceFactory.BoardService;

            _userService.Register(_testEmail, _testPassword);
        }

        // =========================================================================
        // CreateBoard Tests
        // =========================================================================

        /// <summary>
        /// Tests that a new board can be successfully created by a logged-in user.
        /// This function tests Requirement 8.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool CreateBoard_ValidNewBoard_Success()
        {
            SetUp();
            string jsonResponse = _boardService.CreateBoard(_testEmail, _testBoardName);
            Response<BoardSL> response = JsonSerializer.Deserialize<Response<BoardSL>>(jsonResponse)!;
            return string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests that creating a board with a null name is handled as a malformed input error.
        /// This function tests Requirements 8 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool CreateBoard_NullName_Failure()
        {
            SetUp();
            // Passing null for the board name should fail
            string jsonResponse = _boardService.CreateBoard(_testEmail, null);
            Response<object> response = JsonSerializer.Deserialize<Response<object>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to create a board without being logged in.
        /// This function tests Requirements 8 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool CreateBoard_UserNotLoggedIn_Failure()
        {
            SetUp();
            _userService.Logout(_testEmail);
            string jsonResponse = _boardService.CreateBoard(_testEmail, _testBoardName);
            Response<object> response = JsonSerializer.Deserialize<Response<object>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests that a user cannot have two boards with the same name.
        /// This function tests Requirements 10 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool CreateBoard_BoardNameTaken_Failure()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);
            string jsonResponse = _boardService.CreateBoard(_testEmail, _testBoardName);
            Response<object> response = JsonSerializer.Deserialize<Response<object>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        // =========================================================================
        // DeleteBoard Tests
        // =========================================================================

        /// <summary>
        /// Tests that a logged-in user can successfully delete an existing board.
        /// This function tests Requirement 8.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool DeleteBoard_ValidBoard_Success()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);
            string jsonResponse = _boardService.DeleteBoard(_testEmail, _testBoardName);
            Response<BoardSL> response = JsonSerializer.Deserialize<Response<BoardSL>>(jsonResponse)!;
            return string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests that deleting a board with a null name is handled as a malformed input error.
        /// This function tests Requirements 8 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool DeleteBoard_NullName_Failure()
        {
            SetUp();
            string jsonResponse = _boardService.DeleteBoard(_testEmail, null);
            Response<object> response = JsonSerializer.Deserialize<Response<object>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to delete a board without being logged in.
        /// This function tests Requirements 8 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool DeleteBoard_UserNotLoggedIn_Failure()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);
            _userService.Logout(_testEmail);
            string jsonResponse = _boardService.DeleteBoard(_testEmail, _testBoardName);
            Response<object> response = JsonSerializer.Deserialize<Response<object>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to delete a board that does not exist.
        /// This function tests Requirements 8 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool DeleteBoard_BoardDoesNotExist_Failure()
        {
            SetUp();
            string jsonResponse = _boardService.DeleteBoard(_testEmail, "NonExistentBoard");
            Response<object> response = JsonSerializer.Deserialize<Response<object>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        // =========================================================================
        // LimitTasksInColumn Tests
        // =========================================================================

        /// <summary>
        /// Tests that a column's task limit can be successfully updated.
        /// This function tests Requirement 11.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool LimitTasksInColumn_ValidLimit_Success()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);
            string jsonResponse = _boardService.LimitTasksInColumn(_testEmail, _testBoardName, 0, 5);
            Response<object> response = JsonSerializer.Deserialize<Response<object>>(jsonResponse)!;
            return string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests that limiting a task in a board with a null name is handled as a malformed input error.
        /// This function tests Requirement 11 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool LimitTasksInColumn_NullBoardName_Success()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);
            string jsonResponse = _boardService.LimitTasksInColumn(_testEmail, null, 0, 5);
            Response<object> response = JsonSerializer.Deserialize<Response<object>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }


        /// <summary>
        /// Tests the logic error where a user attempts to limit a column without being logged in.
        /// This function tests Requirements 11 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool LimitTasksInColumn_UserNotLoggedIn_Failure()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);
            _userService.Logout(_testEmail);
            string jsonResponse = _boardService.LimitTasksInColumn(_testEmail, _testBoardName, 0, 5);
            Response<object> response = JsonSerializer.Deserialize<Response<object>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to limit a column on a non-existent board.
        /// This function tests Requirements 11 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool LimitTasksInColumn_BoardDoesNotExist_Failure()
        {
            SetUp();
            string jsonResponse = _boardService.LimitTasksInColumn(_testEmail, "Fake Board", 0, 5);
            Response<object> response = JsonSerializer.Deserialize<Response<object>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a limit is applied to a column index that does not exist.
        /// This function tests Requirements 11 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool LimitTasksInColumn_InvalidColumnIndex_Failure()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);
            string jsonResponse = _boardService.LimitTasksInColumn(_testEmail, _testBoardName, 3, 5);
            Response<object> response = JsonSerializer.Deserialize<Response<object>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a negative limit is provided.
        /// This function tests Requirements 11 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool LimitTasksInColumn_NegativeLimit_Failure()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);
            string jsonResponse = _boardService.LimitTasksInColumn(_testEmail, _testBoardName, 0, -5);
            Response<object> response = JsonSerializer.Deserialize<Response<object>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a limit is lower than the current amount of tasks in the column.
        /// This function tests Requirements 11 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool LimitTasksInColumn_LimitBelowTaskAmount_Failure()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);
            _serviceFactory.TaskService.AddTask(_testEmail, _testBoardName, "test task", DateTime.Now.AddDays(1), "test description");
            string jsonResponse = _boardService.LimitTasksInColumn(_testEmail, _testBoardName, 0, 0);
            Response<object> response = JsonSerializer.Deserialize<Response<object>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        // =========================================================================
        // GetInProgressTasks Tests
        // =========================================================================

        /// <summary>
        /// Tests that a logged-in user can retrieve their in-progress tasks (expecting an empty list initially).
        /// This function tests Requirement 17.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool GetInProgressTasks_ValidEmptyList_Success()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);
            string jsonResponse = _boardService.GetInProgressTasks(_testEmail);
            Response<List<TaskSL>> response = JsonSerializer.Deserialize<Response<List<TaskSL>>>(jsonResponse)!;
            return string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to fetch in-progress tasks without being logged in.
        /// This function tests Requirements 17 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool GetInProgressTasks_UserNotLoggedIn_Failure()
        {
            SetUp();
            _userService.Logout(_testEmail);
            string jsonResponse = _boardService.GetInProgressTasks(_testEmail);
            Response<object> response = JsonSerializer.Deserialize<Response<object>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        // =========================================================================
        // TransferOwnership Tests
        // =========================================================================

        /// <summary>
        /// Tests that the owner of a board can successfully transfer ownership to another registered user.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool TransferOwnership_ValidTransfer_Success()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);

            // Register the new owner
            string newOwnerEmail = "newowner@example.com";
            _userService.Register(newOwnerEmail, "Password123");

            string transferJson = _boardService.TransferOwnership(_testEmail, newOwnerEmail, _testBoardName);
            Response<BoardSL> response = JsonSerializer.Deserialize<Response<BoardSL>>(transferJson)!;
            return string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to transfer ownership of a board while not logged in.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool TransferOwnership_UserNotLoggedIn_Failure()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);

            string newOwnerEmail = "newowner@example.com";
            _userService.Register(newOwnerEmail, "Password123");

            _userService.Logout(_testEmail);

            string transferJson = _boardService.TransferOwnership(_testEmail, newOwnerEmail, _testBoardName);
            Response<BoardSL> response = JsonSerializer.Deserialize<Response<BoardSL>>(transferJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to transfer ownership to a user that does not exist.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool TransferOwnership_NewOwnerDoesNotExist_Failure()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);

            string fakeEmail = "fakeuser@example.com";

            string transferJson = _boardService.TransferOwnership(_testEmail, fakeEmail, _testBoardName);
            Response<BoardSL> response = JsonSerializer.Deserialize<Response<BoardSL>>(transferJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to transfer ownership of a board that does not exist.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool TransferOwnership_BoardDoesNotExist_Failure()
        {
            SetUp();
            string newOwnerEmail = "newowner@example.com";
            _userService.Register(newOwnerEmail, "Password123");

            string transferJson = _boardService.TransferOwnership(_testEmail, newOwnerEmail, "NonExistentBoard");
            Response<BoardSL> response = JsonSerializer.Deserialize<Response<BoardSL>>(transferJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user who is not the current owner attempts to transfer ownership.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool TransferOwnership_UserIsNotOwner_Failure()
        {
            SetUp();
            string boardJson = _boardService.CreateBoard(_testEmail, _testBoardName);
            BoardSL board = JsonSerializer.Deserialize<Response<BoardSL>>(boardJson)!.ReturnValue!;

            string otherUser = "otheruser@example.com";
            _userService.Register(otherUser, "Password123");
            _boardService.JoinBoard(otherUser, board.BoardID);

            // otherUser tries to transfer the board they do not own to a third user
            string thirdUser = "thirduser@example.com";
            _userService.Register(thirdUser, "Password123");

            string transferJson = _boardService.TransferOwnership(otherUser, thirdUser, _testBoardName);
            Response<BoardSL> response = JsonSerializer.Deserialize<Response<BoardSL>>(transferJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        // =========================================================================
        // GetBoardName Tests
        // =========================================================================

        /// <summary>
        /// Tests that the name of a board can be successfully retrieved using a valid board ID.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool GetBoardName_ValidId_Success()
        {
            SetUp();
            string boardJson = _boardService.CreateBoard(_testEmail, _testBoardName);
            BoardSL board = JsonSerializer.Deserialize<Response<BoardSL>>(boardJson)!.ReturnValue!;

            string nameJson = _boardService.GetBoardName(board.BoardID);
            Response<string> response = JsonSerializer.Deserialize<Response<string>>(nameJson)!;

            // Verifies there are no errors and that the retrieved name matches the created board name
            return string.IsNullOrEmpty(response.ErrorMessage) && response.ReturnValue == _testBoardName;
        }

        /// <summary>
        /// Tests the logic error where a user attempts to get the name of a board ID that does not exist.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool GetBoardName_BoardDoesNotExist_Failure()
        {
            SetUp();
            string nameJson = _boardService.GetBoardName(9999);
            Response<string> response = JsonSerializer.Deserialize<Response<string>>(nameJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        // =========================================================================
        // GetUserBoards Tests
        // =========================================================================

        /// <summary>
        /// Tests that a logged-in user can successfully retrieve a list of their boards.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool GetUserBoards_ValidUser_Success()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);
            string jsonResponse = _boardService.GetUserBoards(_testEmail);

            // Assuming board IDs are integers
            Response<List<int>> response = JsonSerializer.Deserialize<Response<List<int>>>(jsonResponse)!;
            return string.IsNullOrEmpty(response.ErrorMessage) && response.ReturnValue != null;
        }

        /// <summary>
        /// Tests the logic error where a user attempts to retrieve their boards while not logged in.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool GetUserBoards_UserNotLoggedIn_Failure()
        {
            SetUp();
            _userService.Logout(_testEmail);
            string jsonResponse = _boardService.GetUserBoards(_testEmail);
            Response<List<int>> response = JsonSerializer.Deserialize<Response<List<int>>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        // =========================================================================
        // GetColumnName Tests
        // =========================================================================

        /// <summary>
        /// Tests that a logged-in user can successfully retrieve the name of a valid column.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool GetColumnName_ValidColumn_Success()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);
            string jsonResponse = _boardService.GetColumnName(_testEmail, _testBoardName, 0);
            Response<string> response = JsonSerializer.Deserialize<Response<string>>(jsonResponse)!;
            return string.IsNullOrEmpty(response.ErrorMessage) && !string.IsNullOrEmpty(response.ReturnValue);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to retrieve the name of a column index that does not exist.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool GetColumnName_InvalidColumnIndex_Failure()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);
            string jsonResponse = _boardService.GetColumnName(_testEmail, _testBoardName, 9999);
            Response<string> response = JsonSerializer.Deserialize<Response<string>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to retrieve a column name while not logged in.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool GetColumnName_UserNotLoggedIn_Failure()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);
            _userService.Logout(_testEmail);
            string jsonResponse = _boardService.GetColumnName(_testEmail, _testBoardName, 0);
            Response<string> response = JsonSerializer.Deserialize<Response<string>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }


        // =========================================================================
        // GetColumnLimit Tests
        // =========================================================================

        /// <summary>
        /// Tests that a logged-in user can successfully retrieve the limit of a valid column.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool GetColumnLimit_ValidColumn_Success()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);
            _boardService.LimitTasksInColumn(_testEmail, _testBoardName, 0, 5);

            string jsonResponse = _boardService.GetColumnLimit(_testEmail, _testBoardName, 0);
            Response<int?> response = JsonSerializer.Deserialize<Response<int?>>(jsonResponse)!;
            return string.IsNullOrEmpty(response.ErrorMessage) && response.ReturnValue == 5;
        }

        /// <summary>
        /// Tests the logic error where a user attempts to retrieve the limit of a column index that does not exist.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool GetColumnLimit_InvalidColumnIndex_Failure()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);
            string jsonResponse = _boardService.GetColumnLimit(_testEmail, _testBoardName, 9999);
            Response<int?> response = JsonSerializer.Deserialize<Response<int?>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to retrieve a column limit while not logged in.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool GetColumnLimit_UserNotLoggedIn_Failure()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);
            _userService.Logout(_testEmail);
            string jsonResponse = _boardService.GetColumnLimit(_testEmail, _testBoardName, 0);
            Response<int?> response = JsonSerializer.Deserialize<Response<int?>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        // =========================================================================
        // GetColumnTasks Tests
        // =========================================================================

        /// <summary>
        /// Tests that a logged-in user can successfully retrieve all tasks in a valid column.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool GetColumnTasks_ValidColumn_Success()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);
            _serviceFactory.TaskService.AddTask(_testEmail, _testBoardName, "Task 1", DateTime.Now.AddDays(1), "Desc");

            string jsonResponse = _boardService.GetColumnTasks(_testEmail, _testBoardName, 0);
            Response<List<TaskSL>> response = JsonSerializer.Deserialize<Response<List<TaskSL>>>(jsonResponse)!;
            return string.IsNullOrEmpty(response.ErrorMessage) && response.ReturnValue != null && response.ReturnValue.Count == 1;
        }

        /// <summary>
        /// Tests the logic error where a user attempts to retrieve tasks from a column index that does not exist.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool GetColumnTasks_InvalidColumnIndex_Failure()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);
            string jsonResponse = _boardService.GetColumnTasks(_testEmail, _testBoardName, 9999);
            Response<List<TaskSL>> response = JsonSerializer.Deserialize<Response<List<TaskSL>>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to retrieve column tasks while not logged in.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool GetColumnTasks_UserNotLoggedIn_Failure()
        {
            SetUp();
            _boardService.CreateBoard(_testEmail, _testBoardName);
            _userService.Logout(_testEmail);
            string jsonResponse = _boardService.GetColumnTasks(_testEmail, _testBoardName, 0);
            Response<List<TaskSL>> response = JsonSerializer.Deserialize<Response<List<TaskSL>>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }
    }
}
