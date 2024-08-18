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
            Dictionary<string, string> files = new Dictionary<string, string>();

            if (File.Exists(source))
            {
                string fileName = source.Substring(source.LastIndexOf('\\'));

                files.Add(source, destination + fileName);
            }
            else if (Directory.Exists(source))
            {
                string[] sources = Directory.GetFiles(source);

                foreach (string source in sources)
                {
                    string relativePath = source.Substring(this.source.Length);
                    string destination = this.destination + relativePath;

                    files.Add(source, destination);
                }
            }
            else return;

            List<string> log = new List<string>(files.Count);

            foreach (KeyValuePair<string, string> file in files)
            {
                File.Copy(file.Key, file.Value, true);

                log.Add($"Copied '{file.Key}' to '{file.Value}'");
            }

            Logger.Log(log);
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