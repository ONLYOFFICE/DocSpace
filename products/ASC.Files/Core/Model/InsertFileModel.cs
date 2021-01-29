
using Microsoft.AspNetCore.Http;

namespace ASC.Files.Core.Model
{
    public class InsertFileModel
    {
        public IFormFile File { get; set; }
        public string Title { get; set; }
        public bool? CreateNewIfExist { get; set; }
        public bool KeepConvertStatus { get; set; }
    }
}
