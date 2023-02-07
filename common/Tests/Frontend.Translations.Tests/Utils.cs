using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Frontend.Tests;

public static class Utils
{
    public static string ConvertPathToOS(string path)
    {
        return Path.DirectorySeparatorChar == '/' ? path.Replace("\\", "/") : path.Replace("/", "\\");
    }

    // Regex version
    public static IEnumerable<string> GetFiles(string path,
                        string searchPatternExpression = "",
                        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var reSearchPattern = new Regex(searchPatternExpression, RegexOptions.IgnoreCase);
        return Directory.EnumerateFiles(path, "*", searchOption)
                        .Where(file =>
                                 reSearchPattern.IsMatch(Path.GetExtension(file)));
    }

    // Takes same patterns, and executes in parallel
    public static IEnumerable<string> GetFiles(string path,
                        string[] searchPatterns,
                        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return searchPatterns.AsParallel()
               .SelectMany(searchPattern =>
                      Directory.EnumerateFiles(path, searchPattern, searchOption));
    }
}

