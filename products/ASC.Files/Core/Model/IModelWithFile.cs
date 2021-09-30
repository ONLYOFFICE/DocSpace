
using Microsoft.AspNetCore.Http;

namespace ASC.Files.Core.Model
{
    public interface IModelWithFile
    {
        IFormFile File { get; set; }
    }
}
