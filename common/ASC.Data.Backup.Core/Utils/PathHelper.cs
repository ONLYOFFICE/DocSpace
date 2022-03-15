namespace ASC.Data.Backup.Utils;

internal static class PathHelper
{
    public static string ToRootedPath(string path)
    {
        return ToRootedPath(path, Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
    }

    public static string ToRootedPath(string path, string basePath)
    {
        if (!Path.IsPathRooted(path))
        {
            path = CrossPlatform.PathCombine(basePath, path);
        }

        return Path.GetFullPath(path);
    }

    public static string ToRootedConfigPath(string path)
    {
        if (!Path.HasExtension(path))
        {
            path = CrossPlatform.PathCombine(path, "Web.config");
        }

        return ToRootedPath(path);
    }

    public static string GetTempFileName(string tempDir)
    {
        string tempPath;
        do
        {
            tempPath = CrossPlatform.PathCombine(tempDir, Path.GetRandomFileName());
        } while (File.Exists(tempPath));

        return tempPath;
    }
}
