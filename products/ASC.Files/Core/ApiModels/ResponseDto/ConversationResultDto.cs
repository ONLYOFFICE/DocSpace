namespace ASC.Files.Core.ApiModels.ResponseDto;

/// <summary>
/// Result of file conversation operation.
/// </summary>
public class ConversationResultDto<T>
{
    /// <summary>
    /// Operation Id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Operation type.
    /// </summary>
    [JsonPropertyName("Operation")]
    public FileOperationType OperationType { get; set; }

    /// <summary>
    /// Operation progress.
    /// </summary>
    public int Progress { get; set; }

    /// <summary>
    /// Source files for operation.
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Result file of operation.
    /// </summary>
    [JsonPropertyName("result")]
    public object File { get; set; }

    /// <summary>
    /// Error during conversation.
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// Is operation processed.
    /// </summary>
    public string Processed { get; set; }
}
