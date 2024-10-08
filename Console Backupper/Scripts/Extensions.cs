using System;
using System.Collections.Generic;

namespace ConsoleBackupper
{
    public static class Extensions
    {
        public static List<string> Split(this string text, string separator)
        {
            string[] separators = { separator };
            string[] values = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            return new List<string>(values);
        }
    }
}
