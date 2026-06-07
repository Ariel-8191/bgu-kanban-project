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
    }
}