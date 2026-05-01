namespace IntroSE.Kanban.BackendTests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting tests...");
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
