using System;
using System.Collections.Generic;

namespace ConsoleBackupper
{
    public static class Input
    {
        private const string prompt = "> ";

        private static int startPos;

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
                try
                {
                    command = name switch
                    {
                        "query" => GetCommand<QueryCommand>(),
                        "add" => GetCommand<AddCommand>(),
                        "remove" => GetCommand<RemoveCommand>(),
                        "removeall" => GetCommand<RemoveAllCommand>(),
                        "start" => GetCommand<StartCommand>(),
                        "cls" => GetCommand<ClearCommand>(),
                        "exit" => GetCommand<ExitCommand>(),
                        _ => throw new FormatException($"The command '{name}' is not a valid command.")
                    };
                }
                catch (FormatException e)
                {
                    Logger.LogError(e);
                }
            }
            else Console.CursorTop = startPos;

            return command != null;

            Command GetCommand<C>()
                where C : Command, new()
            {
                return Command.GetCommand<C>(args);
            }
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