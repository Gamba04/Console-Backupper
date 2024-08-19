using System;
using System.Collections.Generic;

namespace ConsoleBackupper
{
    public static class Logger
    {
        private const ConsoleColor infoColor = ConsoleColor.Yellow;
        private const ConsoleColor errorColor = ConsoleColor.Red;

        #region Public Methods

        public static void Log(List<string> messages) => Log(messages, infoColor);

        public static void Log(string message) => Log(message, infoColor);

        public static void LogError(List<string> messages) => Log(messages, errorColor);

        public static void LogError(Exception exception) => Log(exception.Message, errorColor);

        public static void LogError(string message) => Log(message, errorColor);

        #endregion

        // ----------------------------------------------------------------

        #region Internal

        private static void Log(List<string> messages, ConsoleColor color)
        {
            string log = "";

            for (int i = 0; i < messages.Count; i++)
            {
                if (i > 0) log += "\n";

                log += messages[i];
            }

            Log(log, color);
        }

        private static void Log(string message, ConsoleColor color)
        {
            if (message == "")
            {
                Console.WriteLine();

                return;
            }

            Console.ForegroundColor = color;
            Console.WriteLine($"\n{message}\n");
            Console.ResetColor();
        }

        #endregion

    }
}