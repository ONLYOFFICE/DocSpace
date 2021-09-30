using Microsoft.AspNetCore.Http;

namespace ASC.Files.Core.Model
{
    public class SaveEditingModel : IModelWithFile
    {
        public string FileExtension { get; set; }
        public string DownloadUri { get; set; }
        public IFormFile File { get; set; }
        public string Doc { get; set; }
        public bool Forcesave { get; set; }
    }
}
