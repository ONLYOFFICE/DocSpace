using System.Collections.Generic;

using ASC.Core.Tenants;

namespace ASC.Web.Api.Models
{
    public class MailDomainSettingsModel
    {
        public TenantTrustedDomainsType Type { get; set; }
        public List<string> Domains { get; set; }
        public bool InviteUsersAsVisitors { get; set; }
    }

    public class AdminMessageSettingsModel
    {
        public bool TurnOn { get; set; }
    }
}
