using ASC.Files.Core.Security;

namespace ASC.Files.Model
{
    public class FileModelFull
    {
        public string FileId { get; set; }
        public string ParentId { get; set; }
        public string Title { get; set; }
        public int Version { get; set; } = -1;
        public FileShare Share { get; set; }
    }
}
