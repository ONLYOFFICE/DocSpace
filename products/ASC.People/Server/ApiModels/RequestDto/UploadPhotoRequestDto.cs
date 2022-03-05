namespace ASC.People.ApiModels.RequestDto;

public class UploadPhotoRequestDto
{
    public List<IFormFile> Files { get; set; }
    public bool Autosave { get; set; }
}
