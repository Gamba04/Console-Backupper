using System;

namespace ConsoleBackupper
{
    public static class Logger
    {
        private const ConsoleColor infoColor = ConsoleColor.Yellow;
        private const ConsoleColor errorColor = ConsoleColor.Red;

        public static void LogInfo(object message) => Log(message, infoColor);

        public static void LogError(object message) => Log(message, errorColor);

        private static void Log(object message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"\n{message}\n");
            Console.ResetColor();
        }
    }
}