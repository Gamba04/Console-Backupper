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

        public static void Add(BackupInfo backup)
        {
            string line = backup.ToString();

            EditFile(Operation);
            Logger.LogInfo($"Added '{line}' to configuration.");

            void Operation(ref string content)
            {
                if (content != "") content += "\n";

                content += line;
            }
        }

        public static void Remove(string source)
        {
            List<string> removed = new List<string>();

            EditFile(Operation);
            Logger.LogInfo(GetLog());

            void Operation(ref string content)
            {
                List<string> lines = GetLines(content);

                removed = lines.FindAll(MatchSource);
                removed.ForEach(RemoveLine);

                content = string.Join("\n", lines);

                bool MatchSource(string line)
                {
                    BackupInfo backup = BackupInfo.Parse(line);

                    return backup.source == source;
                }

                void RemoveLine(string line)
                {
                    lines.Remove(line);
                }
            }

            string GetLog()
            {
                string log = "";

                removed.ForEach(line => log += $"Removed '{line}' from configuration.");

                return log;
            }
        }

        public static List<BackupInfo> GetBackups()
        {
            string content = ReadFile();

            List<string> lines = GetLines(content);

            return lines.ConvertAll(BackupInfo.Parse);
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