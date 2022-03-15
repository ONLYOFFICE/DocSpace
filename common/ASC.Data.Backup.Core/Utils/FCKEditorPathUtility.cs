namespace ASC.Data.Backup.Utils;

static class FCKEditorPathUtility
{
    private static readonly Regex _regex = new Regex("(?<start>\\/data\\/(?>htmleditorfiles|fckcomments))(?<tenant>\\/0\\/|\\/[\\d]+\\/\\d\\d\\/\\d\\d\\/)", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static string CorrectStoragePath(string content, int tenant)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return content;
        }

        var tenantPath = "/" + TenantPath.CreatePath(tenant.ToString()) + "/";

        return _regex.Replace(content, (m) => m.Success ? m.Groups["start"] + tenantPath : string.Empty);
    }
}
