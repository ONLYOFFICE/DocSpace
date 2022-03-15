namespace ASC.Files.Core.ApiModels.RequestDto;

public class SessionRequestDto
{
    public string FileName { get; set; }
    public long FileSize { get; set; }
    public string RelativePath { get; set; }
    public ApiDateTime LastModified { get; set; }
    public bool Encrypted { get; set; }
}
