using System;
using System.Collections.Generic;

namespace ConsoleBackupper
{
    public static class Input
    {
        private const string prompt = "> ";

        private static int startPos;

        public static readonly Dictionary<string, Type> commandsLibrary = new Dictionary<string, Type>()
        {
            ["query"]  = typeof(QueryCommand),
            ["add"]    = typeof(AddCommand),
            ["remove"] = typeof(RemoveCommand),
            ["reset"]  = typeof(ResetCommand),
            ["backup"] = typeof(BackupCommand),
            ["cls"]    = typeof(ClearCommand),
            ["help"]   = typeof(HelpCommand),
            ["exit"]   = typeof(ExitCommand),
        };

        #region Init

        public static void Init()
        {
            while (true)
            {
                ReadInput();
            }
        }

        private static void ReadInput()
        {
            startPos = Console.CursorTop;

            Console.Write(prompt);
            string input = Console.ReadLine();

            Logger.Space();

            if (TryGetCommand(input, out Command command))
            {
                command.Run();
            }
        }

        #endregion

        // ----------------------------------------------------------------

        #region Processing

        private static bool TryGetCommand(string input, out Command command)
        {
            command = null;

            if (TryParseCommand(input, out string name, out string[] args))
            {
                if (commandsLibrary.TryGetValue(name, out Type type))
                {
                    command = (Command)Activator.CreateInstance(type);

                    if (command.ValidateArgs(args))
                    {
                        command.Init(args);

                        return true;
                    }
                }
                else Logger.LogError($"The command '{name}' is not a valid command");
            }
            else Console.CursorTop = startPos;

            return false;
        }

        private static bool TryParseCommand(string input, out string name, out string[] args)
        {
            List<string> elements = GetCommandElements(input);

            return GetCommandOutput(elements, out name, out args);
        }

        private static List<string> GetCommandElements(string input)
        {
            List<string> elements = new List<string>();

            string buffer = "";
            bool quoted = false;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (c == '"')
                {
                    quoted = !quoted;
                }
                else if (quoted || c != ' ')
                {
                    buffer += c;

                    if (i < input.Length - 1) continue;
                }

                if (buffer.Length > 0)
                {
                    elements.Add(buffer);
                    buffer = "";
                }
            }

            return elements;
        }

        private static bool GetCommandOutput(List<string> elements, out string name, out string[] args)
        {
            if (elements.Count > 0)
            {
                name = elements[0].ToLower();

                elements.RemoveAt(0);
                args = elements.ToArray();

                return true;
            }

            name = null;
            args = null;

            return false;
        }

        #endregion

    }
}