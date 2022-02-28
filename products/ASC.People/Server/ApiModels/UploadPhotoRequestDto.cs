namespace ASC.People.Models
{
    public class UploadPhotoModel
    {
        public List<IFormFile> Files { get; set; }
        public bool Autosave { get; set; }
    }
}
