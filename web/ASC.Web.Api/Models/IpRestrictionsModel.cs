using System.Collections.Generic;

namespace ASC.Web.Api.Models
{
    public class IpRestrictionsModel
    {
        public IEnumerable<string> Ips { get; set; }
        public bool Enable { get; set; }
    }
}
