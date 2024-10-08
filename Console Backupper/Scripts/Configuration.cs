using System.Collections.Generic;
using System.IO;

namespace ConsoleBackupper
{
    public static class Configuration
    {
        private const string path = "config.txt";
        private const string lineSeparator = "\n";

        private delegate void Operation(List<Location> locations, out string log);

        #region Operations

        public static void Add(Location location)
        {
            EditFile(Operation);

            void Operation(List<Location> locations, out string log)
            {
                locations.Add(location);

                log = $"Added '{location}' to configuration.";
            }
        }

        public static void Remove(string name)
        {
            EditFile(Operation);

            void Operation(List<Location> locations, out string log)
            {
                int removed = locations.RemoveAll(location => location.name == name);

                log = removed > 0 ? $"Removed location '{name}' from configuration" : $"No locations found with name '{name}'";
            }
        }

        public static void RemoveAll()
        {
            EditFile(Operation);

            static void Operation(List<Location> locations, out string log)
            {
                bool isEmpty = locations.Count == 0;

                if (!isEmpty) locations.Clear();

                log = isEmpty ? "The backup configuration is already empty" : "All backup configuration was removed";
            }
        }

        #endregion

        // ----------------------------------------------------------------

        #region Query

        public static Location GetLocation(string name)
        {
            List<Location> locations = GetLocations();

            return locations.Find(location => location.name == name);
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

            operation(locations, out string log);

            string content = string.Join(lineSeparator, locations);

            Logger.Log(log);

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