using System.Collections.Generic;
using System.IO;
using System.Net.Mime;

using Microsoft.AspNetCore.Http;

namespace ASC.CRM.ApiModels
{
    public class UploadFileInCRMRequestDto
    {
        public string EntityType { get; set; }
        public int Entityid { get; set; }
        public Stream File { get; set; }
        public ContentType ContentType { get; set; }
        public ContentDisposition ContentDisposition { get; set; }
        public IEnumerable<IFormFile> Files { get; set; }
        public bool StoreOriginalFileFlag { get; set; }
    }
}
