using ASC.Mail.Models;
using ASC.Web.Api.Routing;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASC.Mail.Controllers
{
    public partial class MailController : ControllerBase
    {
        /// <summary>
        ///    Returns ServerData for mail server associated with tenant
        /// </summary>
        /// <returns>ServerData for current tenant.</returns>
        /// <short>Get mail server</short> 
        /// <category>Servers</category>
        [Read(@"server")]
        public ServerData GetMailServer()
        {
            return ServerEngine.GetMailServer();
        }

        /// <summary>
        ///    Returns ServerData for mail server associated with tenant
        /// </summary>
        /// <returns>ServerData for current tenant.</returns>
        /// <short>Get mail server</short> 
        /// <category>Servers</category>
        [Read(@"serverinfo/get")]
        public ServerFullData GetMailServerFullInfo()
        {
            var fullServerInfo = ServerEngine.GetMailServerFullInfo();

            if (!CoreBaseSettings.Standalone)
                return fullServerInfo;

            var commonDomain = fullServerInfo.Domains.FirstOrDefault(d => d.IsSharedDomain);
            if (commonDomain == null)
                return fullServerInfo;

            //Skip common domain
            fullServerInfo.Domains = fullServerInfo.Domains.Where(d => !d.IsSharedDomain).ToList();
            fullServerInfo.Mailboxes =
                fullServerInfo.Mailboxes.Where(m => m.Address.DomainId != commonDomain.Id).ToList();

            return fullServerInfo;
        }

        /// <summary>
        ///    Get or generate free to any domain DNS records
        /// </summary>
        /// <returns>DNS records for current tenant and user.</returns>
        /// <short>Get free DNS records</short>
        /// <category>DnsRecords</category>
        [Read(@"freedns/get")]
        public ServerDomainDnsData GetUnusedDnsRecords()
        {
            return ServerEngine.GetOrCreateUnusedDnsData();
        }
    }
}
