using System.IO;

namespace ASC.Files.Core.Model
{
    public class FileStreamModel
    {
        public Stream File { get; set; } 
        public bool Encrypted { get; set; }
        public bool Forcesave { get; set; }
    }
}
