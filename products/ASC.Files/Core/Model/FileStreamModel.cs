
using Microsoft.AspNetCore.Http;

namespace ASC.Files.Core.Model
{
    public class FileStreamModel
    {
        public IFormFile File { get; set; } 
        public bool Encrypted { get; set; }
        public bool Forcesave { get; set; }
    }
}
