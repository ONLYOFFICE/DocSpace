namespace ASC.Files.Core.Model;

public class FileStreamRequestDto : IModelWithFile
{
    public IFormFile File { get; set; }
    public bool Encrypted { get; set; }
    public bool Forcesave { get; set; }
    public string FileExtension { get; set; }
}
