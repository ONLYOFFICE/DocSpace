namespace ASC.Web.Files.Services.WCFService.FileOperations;

public class FileOperationResult
{
    public string Id { get; set; }

    [JsonPropertyName("operation")]
    public FileOperationType OperationType { get; set; }
    public int Progress { get; set; }
    public string Source { get; set; }
    public string Result { get; set; }
    public string Error { get; set; }
    public string Processed { get; set; }
    public bool Finished { get; set; }
}
