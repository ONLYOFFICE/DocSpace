
using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    [Keyless]
    public class Regions
    {
        public string Region { get; set; }
        public string Provider { get; set; }

        public string ConnectionString { get; set; }
    }
}
