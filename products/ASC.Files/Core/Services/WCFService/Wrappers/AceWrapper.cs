namespace ASC.Web.Files.Services.WCFService;

public class AceCollection<T>
{
    public IEnumerable<T> Files { get; set; }
    public IEnumerable<T> Folders { get; set; }
    public List<AceWrapper> Aces { get; set; }
    public string Message { get; set; }
}

public class AceWrapper
{
    public Guid SubjectId { get; set; }

    [JsonPropertyName("title")]
    public string SubjectName { get; set; }

    public string Link { get; set; }

    [JsonPropertyName("is_group")]
    public bool SubjectGroup { get; set; }

    public bool Owner { get; set; }

    [JsonPropertyName("ace_status")]
    public FileShare Share { get; set; }

    [JsonPropertyName("locked")]
    public bool LockedRights { get; set; }

    [JsonPropertyName("disable_remove")]
    public bool DisableRemove { get; set; }
}

public class AceShortWrapper
{
    public string User { get; set; }
    public string Permissions { get; set; }
    public bool? IsLink { get; set; }

    public AceShortWrapper(AceWrapper aceWrapper)
    {
        var permission = string.Empty;

        switch (aceWrapper.Share)
        {
            case FileShare.Read:
                permission = FilesCommonResource.AceStatusEnum_Read;
                break;
            case FileShare.ReadWrite:
                permission = FilesCommonResource.AceStatusEnum_ReadWrite;
                break;
            case FileShare.CustomFilter:
                permission = FilesCommonResource.AceStatusEnum_CustomFilter;
                break;
            case FileShare.Review:
                permission = FilesCommonResource.AceStatusEnum_Review;
                break;
            case FileShare.FillForms:
                permission = FilesCommonResource.AceStatusEnum_FillForms;
                break;
            case FileShare.Comment:
                permission = FilesCommonResource.AceStatusEnum_Comment;
                break;
            case FileShare.Restrict:
                permission = FilesCommonResource.AceStatusEnum_Restrict;
                break;
        }

        User = aceWrapper.SubjectName;
        if (aceWrapper.SubjectId.Equals(FileConstant.ShareLinkId))
        {
            IsLink = true;
            User = FilesCommonResource.AceShareLink;
        }

        Permissions = permission;
    }
}
