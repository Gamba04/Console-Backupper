using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleBackupper
{
    public class Backup
    {
        public string source;
        public string destination;

        private const string separator = " > ";

        public Backup(string source, string destination)
        {
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

        public override string ToString() => source + separator + destination;

        public static Backup Parse(string line)
        {
            int index = line.IndexOf(separator);

            if (index == -1) return null;

            line = line.Remove(index, separator.Length);

            string source = line.Remove(index);
            string destination = line.Remove(0, index);

            return new Backup(source, destination);
        }

        #endregion

    }
}