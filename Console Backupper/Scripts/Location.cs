using System.Collections.Generic;
using System.IO;

namespace ConsoleBackupper
{
    public class Location
    {
        public string name;
        public string source;
        public string destination;

        private const string nameSeparator = ": ";
        private const string sourceSeparator = " > ";

        public Location(string name, string source, string destination)
        {
            this.name = name;
            this.source = source;
            this.destination = destination;
        }

        #region Execution

        public void Execute()
        {
            Dictionary<string, string> targets = new Dictionary<string, string>();

            if (File.Exists(source))
            {
                string fileName = GetFileName(source);

                targets.Add(source, destination + fileName);
            }
            else if (Directory.Exists(source))
            {
                List<string> directories = new List<string> { source };
                directories.AddRange(Directory.GetDirectories(source));

                foreach (string directory in directories)
                {
                    string[] files = Directory.GetFiles(directory);

                    foreach (string file in files)
                    {
                        string relativePath = GetRelativePath(file, source);
                        string targetPath = destination + relativePath;

                        targets.Add(file, targetPath);
                    }
                }
            }
            else
            {
                Logger.LogError($"Could not perform backup because '{source}' doesn't exist");

                return;
            }

            Logger.Log($"Copied '{source}' into '{destination}'");

            foreach (KeyValuePair<string, string> target in targets)
            {
                string directory = GetDirectory(target.Value);
                Directory.CreateDirectory(directory);

                File.Copy(target.Key, target.Value, true);
            }

            string GetFileName(string path) => path.Substring(path.LastIndexOf('\\'));

            string GetDirectory(string path) => path.Substring(0, path.LastIndexOf('\\'));

            string GetRelativePath(string fullPath, string root) => fullPath.Substring(root.Length);
        }

        #endregion

        // ----------------------------------------------------------------

        #region Parsing

        public static implicit operator string(Location backup) => backup.ToString();

        public override string ToString() => name + nameSeparator + source + sourceSeparator + destination;

        public static Location Parse(string line)
        {
            GetSeparatorIndex(nameSeparator, out int nameSeparatorIndex);
            GetSeparatorIndex(sourceSeparator, out int sourceSeparatorIndex);

            if (nameSeparatorIndex == -1 || sourceSeparatorIndex == -1) return null;

            string name = line.Remove(nameSeparatorIndex);
            string source = line.Substring(nameSeparatorIndex, sourceSeparatorIndex - nameSeparatorIndex);
            string destination = line.Substring(sourceSeparatorIndex);

            return new Location(name, source, destination);

            void GetSeparatorIndex(string separator, out int separatorIndex)
            {
                separatorIndex = line.IndexOf(separator);

                line = line.Remove(separatorIndex, separator.Length);
            }
        }

        #endregion

    }
}