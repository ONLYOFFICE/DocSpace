using System.Collections.Generic;
using System.IO;
using System.Net.Mime;

using ASC.Projects.Core.Domain;

using Microsoft.AspNetCore.Http;

namespace ASC.Projects.Model.Projects
{
    public class ModelUploadFiles
    {
        public EntityType EntityType { get; set; }
        public int Folderid { get; set; }
        public Stream File { get; set; } 
        public ContentType ContentType { get; set; }
        public ContentDisposition ContentDisposition { get; set; }
        public IEnumerable<IFormFile> Files { get; set; }
        public  bool CreateNewIfExist { get; set; }
        public  bool StoreOriginalFileFlag { get; set; }
    }
}
