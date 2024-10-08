using System.Collections.Generic;
using System.IO;

namespace ConsoleBackupper
{
    public static class Configuration
    {
        private const string path = "config.txt";
        private const string lineSeparator = "\n";

        private delegate void Operation(List<Location> locations);

        #region Operations

        public static void Add(Location location)
        {
            EditFile(Operation);

            void Operation(List<Location> locations)
            {
                if (locations.Exists(l => l.name == location.name))
                {
                    Logger.LogError($"'{location.name}' already exists");

                    return;
                }

                locations.Add(location);

                Logger.Log($"Added '{location.name}' to configuration");
            }
        }

        public static void Remove(string name)
        {
            EditFile(Operation);

            void Operation(List<Location> locations)
            {
                bool removed = locations.Remove(locations.Find(location => location.name == name));

                if (removed) Logger.Log($"Removed location '{name}' from configuration");
                else Logger.LogError($"The location '{name}' does not exist");
            }
        }

        public static void RemoveAll()
        {
            EditFile(Operation);

            static void Operation(List<Location> locations)
            {
                bool isEmpty = locations.Count == 0;

                if (!isEmpty) locations.Clear();

                Logger.Log(isEmpty ? "The backup configuration is already empty" : "All backup configuration has been removed");
            }
        }

        #endregion

        // ----------------------------------------------------------------

        #region Query

        public static Location GetLocation(string name)
        {
            List<Location> locations = GetLocations();

            Location location = locations.Find(location => location.name == name);

            if (location == null) Logger.LogError($"The location '{name}' does not exist");

            return location;
        }

        public static List<Location> GetLocations()
        {
            string content = ReadFile();

            List<string> lines = content.Split(lineSeparator);
            lines.RemoveAll(line => line == "");

            return lines.ConvertAll(Location.Parse);
        }

        #endregion

        // ----------------------------------------------------------------

        #region Processing

        private static void EditFile(Operation operation)
        {
            List<Location> locations = GetLocations();

            operation(locations);

            string content = string.Join(lineSeparator, locations);

            WriteFile(content);
        }

        private static string ReadFile()
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            return File.ReadAllText(path);
        }

        private static void WriteFile(string content)
        {
            File.WriteAllText(path, content);
        }

        #endregion

    }
}