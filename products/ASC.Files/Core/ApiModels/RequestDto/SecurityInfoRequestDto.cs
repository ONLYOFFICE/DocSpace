namespace ASC.Files.Core.ApiModels.RequestDto;

public class SecurityInfoRequestDto : BaseBatchRequestDto
{
    public IEnumerable<FileShareParams> Share { get; set; }
    public bool Notify { get; set; }
    public string SharingMessage { get; set; }
}
