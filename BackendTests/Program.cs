using BackendTests;

namespace IntroSE.Kanban.BackendTests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Tests...");

            TestRunner runner = new TestRunner();

            // UserService Tests
            UserServiceTests userTests = new UserServiceTests();

            runner.RunTest(userTests.Register_ValidUser_Success);
            runner.RunTest(userTests.Register_NullEmail_Failure);
            runner.RunTest(userTests.Register_NullPassword_Failure);
            runner.RunTest(userTests.Register_InvalidPassword_Failure);
            runner.RunTest(userTests.Register_InvalidEmail_Failure);
            runner.RunTest(userTests.Register_ExistingEmail_Failure);

            runner.RunTest(userTests.Login_ValidCredentials_Success);
            runner.RunTest(userTests.Login_NullEmail_Failure);
            runner.RunTest(userTests.Login_NonExistingEmail_Failure);
            runner.RunTest(userTests.Login_WrongPassword_Failure);

            runner.RunTest(userTests.Logout_ValidUser_Success);
            runner.RunTest(userTests.Logout_NullEmail_Failure);
            runner.RunTest(userTests.Logout_NotLoggedIn_Failure);

            // BoardService Tests
            BoardServiceTests boardTests = new BoardServiceTests();

            runner.RunTest(boardTests.CreateBoard_ValidNewBoard_Success);
            runner.RunTest(boardTests.CreateBoard_NullName_Failure);
            runner.RunTest(boardTests.CreateBoard_UserNotLoggedIn_Failure);
            runner.RunTest(boardTests.CreateBoard_BoardNameTaken_Failure);

            runner.RunTest(boardTests.DeleteBoard_ValidBoard_Success);
            runner.RunTest(boardTests.DeleteBoard_BoardDoesNotExist_Failure);
            runner.RunTest(boardTests.DeleteBoard_NullName_Failure);
            runner.RunTest(boardTests.DeleteBoard_UserNotLoggedIn_Failure);

            runner.RunTest(boardTests.LimitTasksInColumn_ValidLimit_Success);
            runner.RunTest(boardTests.LimitTasksInColumn_NullBoardName_Success);
            runner.RunTest(boardTests.LimitTasksInColumn_UserNotLoggedIn_Failure);
            runner.RunTest(boardTests.LimitTasksInColumn_BoardDoesNotExist_Failure);
            runner.RunTest(boardTests.LimitTasksInColumn_InvalidColumnIndex_Failure);
            runner.RunTest(boardTests.LimitTasksInColumn_NegativeLimit_Failure);
            runner.RunTest(boardTests.LimitTasksInColumn_LimitBelowTaskAmount_Failure);

            runner.RunTest(boardTests.GetInProgressTasks_ValidEmptyList_Success);
            runner.RunTest(boardTests.GetInProgressTasks_UserNotLoggedIn_Failure);

            runner.RunTest(boardTests.TransferOwnership_ValidTransfer_Success);
            runner.RunTest(boardTests.TransferOwnership_UserNotLoggedIn_Failure);
            runner.RunTest(boardTests.TransferOwnership_NewOwnerDoesNotExist_Failure);
            runner.RunTest(boardTests.TransferOwnership_BoardDoesNotExist_Failure);
            runner.RunTest(boardTests.TransferOwnership_UserIsNotOwner_Failure);

            runner.RunTest(boardTests.GetBoardName_ValidId_Success);
            runner.RunTest(boardTests.GetBoardName_BoardDoesNotExist_Failure);

            runner.RunTest(boardTests.GetBoard_ValidId_Success);
            runner.RunTest(boardTests.GetBoard_BoardDoesNotExist_Failure);

            runner.RunTest(boardTests.GetUserBoards_ValidUser_Success);
            runner.RunTest(boardTests.GetUserBoards_UserNotLoggedIn_Failure);

            runner.RunTest(boardTests.GetColumnName_ValidColumn_Success);
            runner.RunTest(boardTests.GetColumnName_InvalidColumnIndex_Failure);
            runner.RunTest(boardTests.GetColumnName_UserNotLoggedIn_Failure);

            runner.RunTest(boardTests.GetColumnLimit_ValidColumn_Success);
            runner.RunTest(boardTests.GetColumnLimit_InvalidColumnIndex_Failure);
            runner.RunTest(boardTests.GetColumnLimit_UserNotLoggedIn_Failure);

            runner.RunTest(boardTests.GetColumnTasks_ValidColumn_Success);
            runner.RunTest(boardTests.GetColumnTasks_InvalidColumnIndex_Failure);
            runner.RunTest(boardTests.GetColumnTasks_UserNotLoggedIn_Failure);

            runner.RunTest(boardTests.JoinBoard_ValidJoin_Success);
            runner.RunTest(boardTests.JoinBoard_UserNotLoggedIn_Failure);
            runner.RunTest(boardTests.JoinBoard_BoardDoesNotExist_Failure);

            runner.RunTest(boardTests.LeaveBoard_ValidLeave_Success);
            runner.RunTest(boardTests.LeaveBoard_UserNotLoggedIn_Failure);
            runner.RunTest(boardTests.LeaveBoard_BoardDoesNotExist_Failure);
            runner.RunTest(boardTests.LeaveBoard_UserIsOwner_Failure);

            // TaskService Tests
            TaskServiceTests taskTests = new TaskServiceTests();

            runner.RunTest(taskTests.AddTask_ValidTask_Success);
            runner.RunTest(taskTests.AddTask_UserNotLoggedIn_Failure);
            runner.RunTest(taskTests.AddTask_BoardDoesNotExist_Failure);
            runner.RunTest(taskTests.AddTask_InvalidTitle_Failure);
            runner.RunTest(taskTests.AddTask_DescriptionTooLong_Failure);
            runner.RunTest(taskTests.AddTask_BacklogIsFull_Failure);

            runner.RunTest(taskTests.EditTask_ValidEdit_Success);
            runner.RunTest(taskTests.EditTask_TaskIsDone_Failure);
            runner.RunTest(taskTests.EditTask_UserNotLoggedIn_Failure);
            runner.RunTest(taskTests.EditTask_TaskDoesNotExist_Failure);

            runner.RunTest(taskTests.AdvanceTask_ValidAdvance_Success);
            runner.RunTest(taskTests.AdvanceTask_TaskIsDone_Failure);
            runner.RunTest(taskTests.AdvanceTask_TaskDoesNotExist_Failure);
            runner.RunTest(taskTests.AdvanceTask_InvalidColumnIndex_Failure);
            runner.RunTest(taskTests.AdvanceTask_UserNotLoggedIn_Failure);
            runner.RunTest(taskTests.AdvanceTask_NextColumnIsFull_Failure);

            runner.RunTest(taskTests.AssignTask_ValidAssign_Success);
            runner.RunTest(taskTests.AssignTask_UserNotLoggedIn_Failure);
            runner.RunTest(taskTests.AssignTask_TaskDoesNotExist_Failure);
            runner.RunTest(taskTests.AssignTask_AssigneeDoesNotExist_Failure);

            // DataManagement Tests
            DataManagementTests dataTests = new DataManagementTests();
            runner.RunTest(dataTests.LoadData_ValidData_Success);
            runner.RunTest(dataTests.DeleteData_ClearsAllData_Success);

            runner.PrintSummary();
        }
    }


    /// <summary>
    /// Responsible for running the tests and keeping track of passed vs total tests
    /// </summary>
    internal class TestRunner
    {
        private int passedTests = 0;
        private int totalTests = 0;

        /// <summary>
        /// Executes a test and prints a formatted result.
        /// </summary>
        /// <param name="testMethod">The method to test</param>
        public void RunTest(Func<bool> testMethod)
        {
            totalTests++;
            Console.Write($"Running {testMethod.Method.Name}... ");
            bool passed = testMethod();

            if (passed)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("PASS");
                passedTests++;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("FAIL");
            }

            Console.ResetColor();
        }

        /// <summary>
        /// Prints the final count of passed vs total tests.
        /// </summary>
        public void PrintSummary()
        {
            Console.WriteLine("\n----------------------------------------");
            Console.Write("TESTING SUMMARY: ");

            if (passedTests == totalTests && totalTests > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            Console.WriteLine($"{passedTests}/{totalTests} Tests Passed.");
            Console.ResetColor();
            Console.WriteLine("----------------------------------------\n");
        }
    }
}
