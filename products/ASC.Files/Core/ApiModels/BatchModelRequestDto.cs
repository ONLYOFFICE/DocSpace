namespace ASC.Files.Model;

public class BaseBatchRequestDto
{
    public IEnumerable<JsonElement> FolderIds { get; set; }
    public IEnumerable<JsonElement> FileIds { get; set; }

    public BaseBatchRequestDto()
    {
        FolderIds = new List<JsonElement>();
        FileIds = new List<JsonElement>();
    }
}

public class DownloadRequestDto : BaseBatchRequestDto
{
    public IEnumerable<ItemKeyValuePair<JsonElement, string>> FileConvertIds { get; set; }

    public DownloadRequestDto() : base()
    {
        FileConvertIds = new List<ItemKeyValuePair<JsonElement, string>>();
    }
}

public class DeleteBatchRequestDto : BaseBatchRequestDto
{
    public bool DeleteAfter { get; set; }
    public bool Immediately { get; set; }
}

public class DeleteRequestDto
{
    public bool DeleteAfter { get; set; }
    public bool Immediately { get; set; }
}

public class BatchRequestDto : BaseBatchRequestDto
{
    public JsonElement DestFolderId { get; set; }
    public FileConflictResolveType ConflictResolveType { get; set; }
    public bool DeleteAfter { get; set; }
}
