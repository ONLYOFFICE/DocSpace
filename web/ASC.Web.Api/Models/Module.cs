using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Api.Models
{
    public class Module
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public bool IsPrimary { get; set; }
    }
}
