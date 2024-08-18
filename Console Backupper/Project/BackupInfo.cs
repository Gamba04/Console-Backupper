using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleBackupper
{
    public class BackupInfo
    {
        public string source;
        public string destination;

        private const string separator = " > ";

        public BackupInfo(string source, string destination)
        {
            this.source = source;
            this.destination = destination;
        }

        public void Execute()
        {
            Dictionary<string, string> files = new Dictionary<string, string>();

            if (File.Exists(source))
            {
                files.Add(source, destination);
            }
            else if (Directory.Exists(source))
            {
                string[] sources = Directory.GetFiles(source);

                foreach (string source in sources)
                {
                    string relativePath = source.TrimStart(this.source.ToCharArray());
                    string destination = this.destination + relativePath;

                    files.Add(source, destination);
                }
            }
            else return;

            foreach (KeyValuePair<string, string> file in files)
            {
                File.Copy(file.Key, file.Value, true);
            }
        }

        public override string ToString() => source + separator + destination;

        public static BackupInfo Parse(string line)
        {
            int index = line.IndexOf(separator);

            if (index == -1) return null;

            line = line.Remove(index, separator.Length);

            string source = line.Remove(index);
            string destination = line.Remove(0, index);

            return new BackupInfo(source, destination);
        }
    }
}
