namespace ASC.People.Models
{
    public class UploadPhotoModel
    {
        public List<IFormFile> Files { get; set; }
        public bool Autosave { get; set; }
    }

    public class FileUploadResult
    {
        public bool Success { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
    }
}
