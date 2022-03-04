using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ASC.Files.Core.Security;
using ASC.Web.Files.Services.DocumentService;

namespace ASC.Files.Core
{
    public class FileOptions<T>
    {
        public bool Renamed { get; set; }
        public File<T> File { get; set; }
        public FileShare FileShare { get; set; }
        public Configuration<T> Configuration { get; set;}
    }
}
