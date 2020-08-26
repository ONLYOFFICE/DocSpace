using System.Collections.Generic;

using ASC.Web.Studio.UserControls.Management;

namespace ASC.Web.Api.Models
{
    public class AuthServiceModel
    {
        public string Name { get; set; }
        public List<AuthKey> Props { get; set; }
    }
}
