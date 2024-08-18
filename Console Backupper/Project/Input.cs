using System;
using System.Collections.Generic;

namespace ConsoleBackupper
{
    public static class Input
    {

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
                switch (name)
                {
                    case "add":
                        command = GetCommand<AddCommand>();
                        break;

                    case "remove":
                        command = GetCommand<RemoveCommand>();
                        break;

                    case "start":
                        command = GetCommand<StartCommand>();
                        break;

                    case "exit":
                        command = GetCommand<ExitCommand>();
                        break;

                    default:
                        Logger.LogError($"The command '{name}' is not a valid command.");
                        break;
                }
            }

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