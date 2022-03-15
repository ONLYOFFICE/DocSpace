namespace ASC.Web.Files.ThirdPartyApp;

public interface IThirdPartyApp
{
    Task<bool> RequestAsync(HttpContext context);
    string GetRefreshUrl();
    File<string> GetFile(string fileId, out bool editable);
    string GetFileStreamUrl(File<string> file);
    Task SaveFileAsync(string fileId, string fileType, string downloadUrl, Stream stream);
}

public static class ThirdPartySelector
{
    public const string AppAttr = "app";
    public static readonly Regex AppRegex = new Regex("^" + AppAttr + @"-(\S+)\|(\S+)$", RegexOptions.Singleline | RegexOptions.Compiled);

    public static string BuildAppFileId(string app, object fileId)
    {
        return AppAttr + "-" + app + "|" + fileId;
    }

    public static string GetFileId(string appFileId)
    {
        return AppRegex.Match(appFileId).Groups[2].Value;
    }

    public static IThirdPartyApp GetAppByFileId(string fileId)
    {
        if (string.IsNullOrEmpty(fileId))
        {
            return null;
        }

        var match = AppRegex.Match(fileId);

        return match.Success ? GetApp(match.Groups[1].Value) : null;
    }

    public static IThirdPartyApp GetApp(string app)
    {
        return app switch
        {
            GoogleDriveApp.AppAttr => new GoogleDriveApp(),
            BoxApp.AppAttr => new BoxApp(),
            _ => new GoogleDriveApp(),
        };
    }
}
