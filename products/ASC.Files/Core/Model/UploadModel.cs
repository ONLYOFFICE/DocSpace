﻿using System.Collections.Generic;
using System.Net.Mime;

using ASC.Files.Core.Model;

using Microsoft.AspNetCore.Http;

namespace ASC.Files.Model
{
    public class UploadModel : IModelWithFile
    {
        public IFormFile File { get; set; }
        public ContentType ContentType { get; set; }
        public ContentDisposition ContentDisposition { get; set; }
        public IEnumerable<IFormFile> Files { get; set; }
        public bool? CreateNewIfExist { get; set; }
        public bool? StoreOriginalFileFlag { get; set; }
        public bool KeepConvertStatus { get; set; }
    }
}
