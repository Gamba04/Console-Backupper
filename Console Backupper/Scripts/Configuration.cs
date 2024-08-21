using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleBackupper
{
    public static class Configuration
    {
        private const string path = "config.txt";

        private delegate void Operation(ref string content);

        #region Operations

        public static void Add(Location location)
        {
            EditFile(Operation, Log);

            void Operation(ref string content)
            {
                if (content != "") content += "\n";

                content += location;
            }

            void Log() => Logger.Log($"Added '{location}' to configuration.");
        }

        public static void Remove(string source)
        {
            List<string> removed = new List<string>();

            EditFile(Operation, Log);

            void Operation(ref string content)
            {
                List<string> lines = GetLines(content);

                removed = lines.FindAll(MatchSource);
                removed.ForEach(RemoveLine);

                content = string.Join("\n", lines);

                bool MatchSource(string line)
                {
                    Location location = Location.Parse(line);

                    return location.source == source;
                }

                void RemoveLine(string line)
                {
                    lines.Remove(line);
                }
            }

            void Log()
            {
                List<string> log = new List<string>(removed.Count);

                log.AddRange(removed.ConvertAll(line => $"Removed '{line}' from configuration."));

                if (log.Count == 0)
                {
                    log.Add($"No entries found with source '{source}'");
                }

                Logger.Log(log);
            }
        }

        public static void RemoveAll()
        {
            bool isEmpty = false;

            EditFile(Operation, Log);

            void Operation(ref string content)
            {
                isEmpty = content == "";
                content = "";
            }

            void Log() => Logger.Log(isEmpty ? "The backup configuration is already empty" : "All backup configuration was removed");
        }

        public static List<Location> GetLocations()
        {
            string content = ReadFile();

            List<string> lines = GetLines(content);

            return lines.ConvertAll(Location.Parse);
        }

        #endregion

        // ----------------------------------------------------------------

        #region Processing

        private static void EditFile(Operation operation, Action log = null)
        {
            string content = ReadFile();

            operation?.Invoke(ref content);

            File.WriteAllText(path, content);

            log?.Invoke();
        }

        private static string ReadFile()
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            return File.ReadAllText(path);
        }

        private static List<string> GetLines(string content)
        {
            if (content == "") return new List<string>();

            return new List<string>(content.Split('\n'));
        }

        #endregion

    }
}