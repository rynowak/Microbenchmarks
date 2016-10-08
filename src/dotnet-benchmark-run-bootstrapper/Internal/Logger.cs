using System;

namespace dotnet_benchmark_run_bootstrapper.Internal
{
    public static class Logger
    {
        public static bool Verbose { get; set; }

        public static void PrintIfVerbose(string message)
        {
            if (Verbose)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }
    }
}
