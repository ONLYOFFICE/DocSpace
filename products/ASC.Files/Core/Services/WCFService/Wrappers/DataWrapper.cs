namespace ASC.Web.Files.Services.WCFService;

public class DataWrapper<T>
{
    public List<FileEntry> Entries { get; set; }
    public int Total { get; set; }

    [JsonPropertyName("path_parts")]
    public List<object> FolderPathParts { get; set; }

    [JsonPropertyName("folder_info")]
    public Folder<T> FolderInfo { get; set; }

    public int New { get; set; }
}
