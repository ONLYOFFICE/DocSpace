namespace ASC.People.Models
{
    public class UploadPhotoRequestDto
    {
        public List<IFormFile> Files { get; set; }
        public bool Autosave { get; set; }
    }
}
