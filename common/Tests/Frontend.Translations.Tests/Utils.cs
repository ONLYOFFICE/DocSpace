using System;
using System.IO;

namespace Frontend.Translations.Tests
{
    public static class Utils
    {
        public static string ConvertPathToOS(string path)
        {
            return Path.DirectorySeparatorChar == '/' ? path.Replace("\\", "/") : path.Replace("/", "\\");
        }
    }
}

