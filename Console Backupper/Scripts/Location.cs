using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleBackupper
{
    public class Location
    {
        public string name;
        public List<string> sources;
        public string destination;

        private const string nameSeparator = ": ";
        private const string sourcesSeparator = ", ";
        private const string destinationSeparator = " > ";

        public Location(string name, List<string> sources, string destination)
        {
            this.name = name;
            this.sources = sources;
            this.destination = destination;
        }

        #region Execution

        public void Backup()
        {
            Dictionary<string, string> instructions = new Dictionary<string, string>();

            List<string> log = new List<string>();
            List<string> error = new List<string>();

            sources.ForEach(Register);
            Execute(instructions);

            Logger.Log(log);
            Logger.LogError(error);

            void Register(string source)
            {
                string fullSource = Environment.ExpandEnvironmentVariables(source);
                string fullDestination = Environment.ExpandEnvironmentVariables(destination);

                bool success = this.Register(fullSource, fullDestination, instructions);

                if (success) log.Add($"Copied '{source}' into '{destination}'");
                else error.Add($"Could not perform backup because '{source}' doesn't exist");
            }
        }

        private bool Register(string source, string destination, Dictionary<string, string> instructions)
        {
            if (File.Exists(source))
            {
                string fileName = GetName(source);

                instructions.Add(source, destination + fileName);
            }
            else if (Directory.Exists(source))
            {
                string directoryName = GetName(source);

                List<string> directories = new List<string> { source };
                directories.AddRange(Directory.GetDirectories(source));

                foreach (string directory in directories)
                {
                    string[] files = Directory.GetFiles(directory);

                    foreach (string file in files)
                    {
                        string relativePath = GetRelativePath(file, source);
                        string targetPath = @$"{destination}\{directoryName}\{relativePath}";

                        instructions.Add(file, targetPath);
                    }
                }
            }
            else return false;

            return true;
        }

        private void Execute(Dictionary<string, string> instructions)
        {
            foreach (KeyValuePair<string, string> instruction in instructions)
            {
                string directory = GetDirectory(instruction.Value);
                Directory.CreateDirectory(directory);

                File.Copy(instruction.Key, instruction.Value, true);
            }
        }

        private static string GetName(string path) => path.Substring(path.LastIndexOf('\\'));

        private static string GetDirectory(string path) => path.Substring(0, path.LastIndexOf('\\'));

        private static string GetRelativePath(string fullPath, string root) => fullPath.Substring(root.Length);

        #endregion

        // ----------------------------------------------------------------

        #region Parsing

        public static implicit operator string(Location backup) => backup.ToString();

        public override string ToString() => name + nameSeparator + string.Join(sourcesSeparator, sources) + destinationSeparator + destination;

        public static Location Parse(string line)
        {
            string name = ReadNext(nameSeparator);
            List<string> sources = ReadNext(destinationSeparator).Split(sourcesSeparator);
            string destination = line;

            return new Location(name, sources, destination);

            string ReadNext(string target)
            {
                int index = line.IndexOf(target);

                if (index > -1)
                {
                    string value = line.Substring(0, index);

                    line = line.Remove(0, index + target.Length);

                    return value;
                }

                return line;
            }
        }

        #endregion

    }
}