using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

namespace ASC.Web.Api.Models
{
    public class UploadLicenseModel
    {
        public IEnumerable<IFormFile> Files { get; set; }
    }
}
