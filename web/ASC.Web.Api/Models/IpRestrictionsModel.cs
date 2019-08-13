using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Api.Models
{
    public class IpRestrictionsModel
    {
        public IEnumerable<string> Ips { get; set; }
        public bool Enable { get; set; }
    }
}
