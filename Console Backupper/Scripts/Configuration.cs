using System.Collections.Generic;
using System.IO;

namespace ConsoleBackupper
{
    public static class Configuration
    {
        private const string path = "config.txt";

        private delegate void Operation(List<Location> locations);

        #region Operations

        public static void Add(Location location)
        {
            EditFile(Operation);

            Logger.Log($"Added '{location}' to configuration.");

            void Operation(List<Location> locations)
            {
                locations.Add(location);
            }
        }

        public static void Remove(string name)
        {
            EditFile(Operation);

            void Operation(List<Location> locations)
            {
                int removed = locations.RemoveAll(location => location.name == name);

                Logger.Log(removed > 0 ? $"Removed location '{name}' from configuration" : $"No locations found with name '{name}'");
            }
        }

        public static void RemoveAll()
        {
            EditFile(Operation);

            static void Operation(List<Location> locations)
            {
                bool isEmpty = locations.Count == 0;

                if (!isEmpty) locations.Clear();

                Logger.Log(isEmpty ? "The backup configuration is already empty" : "All backup configuration was removed");
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

            List<string> lines = new List<string>(content.Split('\n'));

            return lines.ConvertAll(Location.Parse);
        }

        #endregion

        // ----------------------------------------------------------------

        #region Processing

        private static void EditFile(Operation operation)
        {
            List<Location> locations = GetLocations();

            operation?.Invoke(locations);

            string content = string.Join("\n", locations);

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