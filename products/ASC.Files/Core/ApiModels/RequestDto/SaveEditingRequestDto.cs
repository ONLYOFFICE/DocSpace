namespace ASC.Files.Core.ApiModels.RequestDto;

public class SaveEditingRequestDto : IModelWithFile
{
    public string FileExtension { get; set; }
    public string DownloadUri { get; set; }
    public IFormFile File { get; set; }
    public string Doc { get; set; }
    public bool Forcesave { get; set; }
}
