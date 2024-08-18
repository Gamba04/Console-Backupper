using System.Collections.Generic;
using System.IO;

namespace ConsoleBackupper
{
    public static class Configuration
    {
        private const string path = "config.txt";

        private delegate void Operation(ref string content);

        #region Operations

        public static void Add(Backup backup)
        {
            string line = backup.ToString();

            EditFile(Operation);
            Log();

            void Operation(ref string content)
            {
                if (content != "") content += "\n";

                content += line;
            }

            void Log() => Logger.Log($"Added '{line}' to configuration.");
        }

        public static void Remove(string source)
        {
            List<string> removed = new List<string>();

            EditFile(Operation);
            Log();

            void Operation(ref string content)
            {
                List<string> lines = GetLines(content);

                removed = lines.FindAll(MatchSource);
                removed.ForEach(RemoveLine);

                content = string.Join("\n", lines);

                bool MatchSource(string line)
                {
                    Backup backup = Backup.Parse(line);

                    return backup.source == source;
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

        public static List<Backup> GetBackups()
        {
            string content = ReadFile();

            List<string> lines = GetLines(content);

            return lines.ConvertAll(Backup.Parse);
        }

        #endregion

        // ----------------------------------------------------------------

        #region Processing

        private static void EditFile(Operation operation)
        {
            string content = ReadFile();

            operation?.Invoke(ref content);

            File.WriteAllText(path, content);
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