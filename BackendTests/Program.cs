using BackendTests;

namespace IntroSE.Kanban.BackendTests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Tests...");

            TestRunner runner = new TestRunner();
            BoardServiceTests boardTests = new BoardServiceTests();

            runner.RunTest(boardTests.CreateBoard_ValidNewBoard_Success);
            runner.RunTest(boardTests.CreateBoard_NullName_Failure);
            runner.RunTest(boardTests.CreateBoard_UserNotLoggedIn_Failure);
            runner.RunTest(boardTests.CreateBoard_BoardNameTaken_Failure);

            runner.RunTest(boardTests.CreateBoard_ValidNewBoard_Success);
            runner.RunTest(boardTests.CreateBoard_NullName_Failure);
            runner.RunTest(boardTests.CreateBoard_UserNotLoggedIn_Failure);
            runner.RunTest(boardTests.CreateBoard_BoardNameTaken_Failure);

            runner.RunTest(boardTests.LimitTasksInColumn_ValidLimit_Success);
            runner.RunTest(boardTests.LimitTasksInColumn_NullBoardName_Success);
            runner.RunTest(boardTests.LimitTasksInColumn_UserNotLoggedIn_Failure);
            runner.RunTest(boardTests.LimitTasksInColumn_BoardDoesNotExist_Failure);
            runner.RunTest(boardTests.LimitTasksInColumn_InvalidColumnIndex_Failure);
            runner.RunTest(boardTests.LimitTasksInColumn_NegativeLimit_Failure);
            runner.RunTest(boardTests.LimitTasksInColumn_LimitBelowTaskAmount_Failure);

            runner.RunTest(boardTests.GetInProgressTasks_ValidEmptyList_Success);
            runner.RunTest(boardTests.GetInProgressTasks_UserNotLoggedIn_Failure);

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
