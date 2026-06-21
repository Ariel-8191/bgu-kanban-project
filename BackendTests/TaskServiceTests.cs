using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Text.Json;

namespace BackendTests
{
    internal class TaskServiceTests
    {
        private ServiceFactory _serviceFactory;
        private UserService _userService;
        private BoardService _boardService;
        private TaskService _taskService;

        private readonly string _testEmail = "test@example.com";
        private readonly string _testPassword = "Password123";
        private readonly string _testBoardName = "Test Board";

        /// <summary>
        /// Resets the testing environment, registers a user, and creates a default board.
        /// Must be called at the beginning of every test method.
        /// </summary>
        private void SetUp()
        {
            _serviceFactory = new ServiceFactory();
            _serviceFactory.DeleteData();
            _userService = _serviceFactory.UserService;
            _boardService = _serviceFactory.BoardService;
            _taskService = _serviceFactory.TaskService;

            _userService.Register(_testEmail, _testPassword);
            _boardService.CreateBoard(_testEmail, _testBoardName);
        }

        // =========================================================================
        // AddTask Tests
        // =========================================================================

        /// <summary>
        /// Tests that a logged-in user can successfully add a valid task to the backlog column.
        /// This function tests Requirements 5 and 13.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool AddTask_ValidTask_Success()
        {
            SetUp();
            string jsonResponse = _taskService.AddTask(_testEmail, _testBoardName, "Valid Title", DateTime.Now.AddDays(1), "Valid Description");
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(jsonResponse)!;
            return string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to add a valid task to the backlog column without being loggged in.
        /// This function tests Requirements 5, 13 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool AddTask_UserNotLoggedIn_Failure()
        {
            SetUp();
            _userService.Logout(_testEmail);
            string jsonResponse = _taskService.AddTask(_testEmail, _testBoardName, "Valid Title", DateTime.Now.AddDays(1), "Valid Description");
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to add a task to a board that does not exist.
        /// This function tests Requirements 13 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool AddTask_BoardDoesNotExist_Failure()
        {
            SetUp();
            string jsonResponse = _taskService.AddTask(_testEmail, "Fake Board", "Title", DateTime.Now.AddDays(1), "Description");
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests that adding a task with an invalid title is handled as a malformed input error.
        /// This function tests Requirements 5 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool AddTask_InvalidTitle_Failure()
        {
            SetUp();
            string jsonResponse = _taskService.AddTask(_testEmail, _testBoardName, "", DateTime.Now.AddDays(1), "Description");
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests that adding a task with a description exceeding 300 characters is rejected.
        /// This function tests Requirements 5 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool AddTask_DescriptionTooLong_Failure()
        {
            SetUp();
            string longDesc = new string('A', 301);
            string jsonResponse = _taskService.AddTask(_testEmail, _testBoardName, "Title", DateTime.Now.AddDays(1), longDesc);
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests that adding a task while the backlog is full results in an error.
        /// This function tests Requirements 11 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool AddTask_BacklogIsFull_Failure()
        {
            SetUp();
            _boardService.LimitTasksInColumn(_testEmail, _testBoardName, 0, 0);
            string jsonResponse = _taskService.AddTask(_testEmail, _testBoardName, "Title", DateTime.Now.AddDays(1), "Something");
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }


        // =========================================================================
        // EditTask Tests
        // =========================================================================

        /// <summary>
        /// Tests that a logged-in user can successfully edit an existing task's parameters.
        /// This function tests Requirements 15 and 16.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool EditTask_ValidEdit_Success()
        {
            SetUp();
            string taskJson = _taskService.AddTask(_testEmail, _testBoardName, "Initial Title", DateTime.Now.AddDays(1), "Initial Desc");
            TaskSL task = JsonSerializer.Deserialize<Response<TaskSL>>(taskJson)!.ReturnValue!;

            string editJson = _taskService.EditTask(_testEmail, _testBoardName, 0, task.TaskID, "New Title", DateTime.Now.AddDays(2), "New Desc");
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(editJson)!;
            return string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to edit a task that is in the 'done' column.
        /// This function tests Requirements 15 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool EditTask_TaskIsDone_Failure()
        {
            SetUp();
            string taskJson = _taskService.AddTask(_testEmail, _testBoardName, "Initial Title", DateTime.Now.AddDays(1), "Initial Desc");
            TaskSL task = JsonSerializer.Deserialize<Response<TaskSL>>(taskJson)!.ReturnValue!;

            _taskService.AdvanceTask(_testEmail, _testBoardName, 0, task.TaskID);
            _taskService.AdvanceTask(_testEmail, _testBoardName, 1, task.TaskID);

            string editJson = _taskService.EditTask(_testEmail, _testBoardName, 2, task.TaskID, "New Title", DateTime.Now.AddDays(2), "New Desc");
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(editJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }


        /// <summary>
        /// Tests the logic error where a user attempts to edit a valid task to the backlog column without being loggged in.
        /// This function tests Requirements 15, 16 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool EditTask_UserNotLoggedIn_Failure()
        {
            SetUp();
            string taskJson = _taskService.AddTask(_testEmail, _testBoardName, "Initial Title", DateTime.Now.AddDays(1), "Initial Description");
            TaskSL task = JsonSerializer.Deserialize<Response<TaskSL>>(taskJson)!.ReturnValue!;
            _userService.Logout(_testEmail);
            string jsonResponse = _taskService.EditTask(_testEmail, _testBoardName, 0, task.TaskID, "Other Title", DateTime.Now.AddDays(2), "Other Description");
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(jsonResponse)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to edit a task ID that does not exist.
        /// This function tests Requirements 15 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool EditTask_TaskDoesNotExist_Failure()
        {
            SetUp();
            string editJson = _taskService.EditTask(_testEmail, _testBoardName, 0, 9999, "New Title", DateTime.Now.AddDays(2), "New Desc");
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(editJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        // =========================================================================
        // AdvanceTask Tests
        // =========================================================================

        /// <summary>
        /// Tests that a task can be successfully advanced from the backlog to the in-progress column.
        /// This function tests Requirement 14.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool AdvanceTask_ValidAdvance_Success()
        {
            SetUp();
            string taskJson = _taskService.AddTask(_testEmail, _testBoardName, "Title", DateTime.Now.AddDays(1), "Desc");
            TaskSL task = JsonSerializer.Deserialize<Response<TaskSL>>(taskJson)!.ReturnValue!;

            string advanceJson = _taskService.AdvanceTask(_testEmail, _testBoardName, 0, task.TaskID);
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(advanceJson)!;
            return string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to advance a task from the 'done' column.
        /// This function tests Requirements 14 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool AdvanceTask_TaskIsDone_Failure()
        {
            SetUp();
            string taskJson = _taskService.AddTask(_testEmail, _testBoardName, "Title", DateTime.Now.AddDays(1), "Desc");
            TaskSL task = JsonSerializer.Deserialize<Response<TaskSL>>(taskJson)!.ReturnValue!;

            _taskService.AdvanceTask(_testEmail, _testBoardName, 0, task.TaskID);
            _taskService.AdvanceTask(_testEmail, _testBoardName, 1, task.TaskID);

            string advanceJson = _taskService.AdvanceTask(_testEmail, _testBoardName, 2, task.TaskID);
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(advanceJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to advance a task ID that does not exist.
        /// This function tests Requirements 14 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool AdvanceTask_TaskDoesNotExist_Failure()
        {
            SetUp();
            string editJson = _taskService.AdvanceTask(_testEmail, _testBoardName, 0, 9999);
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(editJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }


        /// <summary>
        /// Tests the logic error where a user attempts to advance a task providing an invalid column index.
        /// This function tests Requirements 14 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool AdvanceTask_InvalidColumnIndex_Failure()
        {
            SetUp();
            string taskJson = _taskService.AddTask(_testEmail, _testBoardName, "Title", DateTime.Now.AddDays(1), "Desc");
            TaskSL task = JsonSerializer.Deserialize<Response<TaskSL>>(taskJson)!.ReturnValue!;

            string advanceJson = _taskService.AdvanceTask(_testEmail, _testBoardName, 5, task.TaskID);
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(advanceJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to advance a task while they are not logged in.
        /// This function tests Requirements 14 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool AdvanceTask_UserNotLoggedIn_Failure()
        {
            SetUp();
            string taskJson = _taskService.AddTask(_testEmail, _testBoardName, "Title", DateTime.Now.AddDays(1), "Desc");
            TaskSL task = JsonSerializer.Deserialize<Response<TaskSL>>(taskJson)!.ReturnValue!;

            _userService.Logout(_testEmail);

            string advanceJson = _taskService.AdvanceTask(_testEmail, _testBoardName, 0, task.TaskID);
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(advanceJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to advance a task to a full column.
        /// This function tests Requirements 11,14 and 20.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool AdvanceTask_NextColumnIsFull_Failure()
        {
            SetUp();
            string taskJson = _taskService.AddTask(_testEmail, _testBoardName, "Title", DateTime.Now.AddDays(1), "Desc");
            TaskSL task = JsonSerializer.Deserialize<Response<TaskSL>>(taskJson)!.ReturnValue!;

            _boardService.LimitTasksInColumn(_testEmail, _testBoardName, 1, 0);
            string advanceJson = _taskService.AdvanceTask(_testEmail, _testBoardName, 0, task.TaskID);
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(advanceJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        // =========================================================================
        // AssignTask Tests
        // =========================================================================

        /// <summary>
        /// Tests that a logged-in user can successfully assign a task to a user.
        /// Requirement 23
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool AssignTask_ValidAssign_Success()
        {
            SetUp();
            string taskJson = _taskService.AddTask(_testEmail, _testBoardName, "Title", DateTime.Now.AddDays(1), "Desc");
            TaskSL task = JsonSerializer.Deserialize<Response<TaskSL>>(taskJson)!.ReturnValue!;

            // Note: AssignTask takes an int for taskID based on the signature in TaskService.cs
            string assignJson = _taskService.AssignTask(_testEmail, _testBoardName, 0, (int)task.TaskID, _testEmail);
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(assignJson)!;
            return string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to assign a task while they are not logged in.
        /// Requirement 26
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool AssignTask_UserNotLoggedIn_Failure()
        {
            SetUp();
            string taskJson = _taskService.AddTask(_testEmail, _testBoardName, "Title", DateTime.Now.AddDays(1), "Desc");
            TaskSL task = JsonSerializer.Deserialize<Response<TaskSL>>(taskJson)!.ReturnValue!;

            _userService.Logout(_testEmail);

            string assignJson = _taskService.AssignTask(_testEmail, _testBoardName, 0, (int)task.TaskID, _testEmail);
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(assignJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to assign a task ID that does not exist.
        /// Requirement 26
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool AssignTask_TaskDoesNotExist_Failure()
        {
            SetUp();
            string assignJson = _taskService.AssignTask(_testEmail, _testBoardName, 0, 9999, _testEmail);
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(assignJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to assign a task to a user email that doesn't exist.
        /// Requirement 26
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool AssignTask_AssigneeDoesNotExist_Failure()
        {
            SetUp();
            string taskJson = _taskService.AddTask(_testEmail, _testBoardName, "Title", DateTime.Now.AddDays(1), "Desc");
            TaskSL task = JsonSerializer.Deserialize<Response<TaskSL>>(taskJson)!.ReturnValue!;

            string fakeEmail = "fake.assignee@example.com";
            string assignJson = _taskService.AssignTask(_testEmail, _testBoardName, 0, (int)task.TaskID, fakeEmail);
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(assignJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to assign a task that is already in the 'done' column.
        /// Requirement 20 and 26
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool AssignTask_TaskIsDone_Failure()
        {
            SetUp();
            string taskJson = _taskService.AddTask(_testEmail, _testBoardName, "Title", DateTime.Now.AddDays(1), "Desc");
            TaskSL task = JsonSerializer.Deserialize<Response<TaskSL>>(taskJson)!.ReturnValue!;

            // Advance to "In Progress" then to "Done" (Assuming index 2 is Done)
            _taskService.AdvanceTask(_testEmail, _testBoardName, 0, task.TaskID);
            _taskService.AdvanceTask(_testEmail, _testBoardName, 1, task.TaskID);

            string assignJson = _taskService.AssignTask(_testEmail, _testBoardName, 2, (int)task.TaskID, _testEmail);
            Response<TaskSL> response = JsonSerializer.Deserialize<Response<TaskSL>>(assignJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        // =========================================================================
        // JoinBoard Tests
        // =========================================================================

        /// <summary>
        /// Tests that a logged-in user can successfully join an existing board.
        /// Requirement 12
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool JoinBoard_ValidJoin_Success()
        {
            SetUp();
            string boardJson = _boardService.CreateBoard(_testEmail, _testBoardName);
            BoardSL board = JsonSerializer.Deserialize<Response<BoardSL>>(boardJson)!.ReturnValue!;

            // Register a second user to join the board
            string otherEmail = "other@example.com";
            _userService.Register(otherEmail, "Password123");

            string joinJson = _boardService.JoinBoard(otherEmail, board.BoardID);
            Response<BoardSL> response = JsonSerializer.Deserialize<Response<BoardSL>>(joinJson)!;
            return string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to join a board while not logged in.
        /// Requirement 26
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool JoinBoard_UserNotLoggedIn_Failure()
        {
            SetUp();
            string boardJson = _boardService.CreateBoard(_testEmail, _testBoardName);
            BoardSL board = JsonSerializer.Deserialize<Response<BoardSL>>(boardJson)!.ReturnValue!;

            string otherEmail = "other@example.com";
            _userService.Register(otherEmail, "Password123");
            _userService.Logout(otherEmail);

            string joinJson = _boardService.JoinBoard(otherEmail, board.BoardID);
            Response<BoardSL> response = JsonSerializer.Deserialize<Response<BoardSL>>(joinJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to join a board ID that does not exist.
        /// Requirement 26
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool JoinBoard_BoardDoesNotExist_Failure()
        {
            SetUp();
            // _testEmail is already logged in from SetUp()
            string joinJson = _boardService.JoinBoard(_testEmail, 9999);
            Response<BoardSL> response = JsonSerializer.Deserialize<Response<BoardSL>>(joinJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }


        // =========================================================================
        // LeaveBoard Tests
        // =========================================================================

        /// <summary>
        /// Tests that a logged-in member can successfully leave a board they joined.
        /// Requirement 26
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool LeaveBoard_ValidLeave_Success()
        {
            SetUp();
            string boardJson = _boardService.CreateBoard(_testEmail, _testBoardName);
            BoardSL board = JsonSerializer.Deserialize<Response<BoardSL>>(boardJson)!.ReturnValue!;

            string otherEmail = "other@example.com";
            _userService.Register(otherEmail, "Password123");
            _boardService.JoinBoard(otherEmail, board.BoardID);

            // The other user leaves the board
            string leaveJson = _boardService.LeaveBoard(otherEmail, board.BoardID);
            Response<BoardSL> response = JsonSerializer.Deserialize<Response<BoardSL>>(leaveJson)!;
            return string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to leave a board while not logged in.
        /// Requirement 26
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool LeaveBoard_UserNotLoggedIn_Failure()
        {
            SetUp();
            string boardJson = _boardService.CreateBoard(_testEmail, _testBoardName);
            BoardSL board = JsonSerializer.Deserialize<Response<BoardSL>>(boardJson)!.ReturnValue!;

            string otherEmail = "other@example.com";
            _userService.Register(otherEmail, "Password123");
            _boardService.JoinBoard(otherEmail, board.BoardID);

            _userService.Logout(otherEmail);

            string leaveJson = _boardService.LeaveBoard(otherEmail, board.BoardID);
            Response<BoardSL> response = JsonSerializer.Deserialize<Response<BoardSL>>(leaveJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where a user attempts to leave a board ID that does not exist.
        /// Requirement 14
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool LeaveBoard_BoardDoesNotExist_Failure()
        {
            SetUp();
            string leaveJson = _boardService.LeaveBoard(_testEmail, 9999);
            Response<BoardSL> response = JsonSerializer.Deserialize<Response<BoardSL>>(leaveJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }

        /// <summary>
        /// Tests the logic error where the owner of a board attempts to leave it.
        /// The owner should not be allowed to leave without transferring ownership first.
        /// </summary>
        /// <returns>Returns true if the test passed, false otherwise.</returns>
        public bool LeaveBoard_UserIsOwner_Failure()
        {
            SetUp();
            string boardJson = _boardService.CreateBoard(_testEmail, _testBoardName);
            BoardSL board = JsonSerializer.Deserialize<Response<BoardSL>>(boardJson)!.ReturnValue!;

            // _testEmail is the owner who created the board
            string leaveJson = _boardService.LeaveBoard(_testEmail, board.BoardID);
            Response<BoardSL> response = JsonSerializer.Deserialize<Response<BoardSL>>(leaveJson)!;
            return !string.IsNullOrEmpty(response.ErrorMessage);
        }
    }
}