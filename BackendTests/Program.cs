using BackendTests;

namespace IntroSE.Kanban.BackendTests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Tests...");

            TestRunner runner = new TestRunner();
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
