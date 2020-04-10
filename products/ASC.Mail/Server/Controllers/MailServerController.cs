using ASC.Mail.Core.Engine.Operations.Base;
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

        /// <summary>
        ///    Returns list of the web domains associated with tenant
        /// </summary>
        /// <returns>List of WebDomainData for current tenant</returns>
        /// <short>Get tenant web domain list</short> 
        /// <category>Domains</category>
        [Read(@"domains/get")]
        public List<ServerDomainData> GetDomains()
        {
            var listDomainData = ServerDomainEngine.GetDomains();

            if (CoreBaseSettings.Standalone)
            {
                //Skip common domain
                listDomainData = listDomainData.Where(d => !d.IsSharedDomain).ToList();
            }

            return listDomainData;
        }

        /// <summary>
        ///    Returns the common web domain
        /// </summary>
        /// <returns>WebDomainData for common web domain</returns>
        /// <short>Get common web domain</short> 
        /// <category>Domains</category>
        [Read(@"domains/common")]
        public ServerDomainData GetCommonDomain()
        {
            var commonDomain = ServerDomainEngine.GetCommonDomain();
            return commonDomain;
        }

        /// <summary>
        ///    Associate a web domain with tenant
        /// </summary>
        /// <param name="name">web domain name</param>
        /// <param name="id_dns"></param>
        /// <returns>WebDomainData associated with tenant</returns>
        /// <short>Add domain to mail server</short> 
        /// <category>Domains</category>
        [Create(@"domains/add")]
        public ServerDomainData AddDomain(string name, int id_dns)
        {
            var domain = ServerDomainEngine.AddDomain(name, id_dns);
            return domain;
        }

        /// <summary>
        ///    Deletes the selected web domain
        /// </summary>
        /// <param name="id">id of web domain</param>
        /// <returns>MailOperationResult object</returns>
        /// <short>Remove domain from mail server</short> 
        /// <category>Domains</category>
        [Delete(@"domains/remove/{id}")]
        public MailOperationStatus RemoveDomain(int id)
        {
            var status = ServerDomainEngine.RemoveDomain(id);
            return status;
        }

        /// <summary>
        ///    Returns dns records associated with domain
        /// </summary>
        /// <param name="id">id of domain</param>
        /// <returns>Dns records associated with domain</returns>
        /// <short>Returns dns records</short>
        /// <category>DnsRecords</category>
        [Read(@"domains/dns/get")]
        public ServerDomainDnsData GetDnsRecords(int id)
        {
            var dns = ServerDomainEngine.GetDnsData(id);
            return dns;
        }

        /// <summary>
        ///    Check web domain name existance
        /// </summary>
        /// <param name="name">web domain name</param>
        /// <returns>True if domain name already exists.</returns>
        /// <short>Is domain name exists.</short> 
        /// <category>Domains</category>
        [Read(@"domains/exists")]
        public bool IsDomainExists(string name)
        {
            var isExists = ServerDomainEngine.IsDomainExists(name);
            return isExists;
        }

        /// <summary>
        ///    Check web domain name ownership over txt record in dns
        /// </summary>
        /// <param name="name">web domain name</param>
        /// <returns>True if user is owner of this domain.</returns>
        /// <short>Check domain ownership.</short> 
        /// <category>Domains</category>
        [Read(@"domains/ownership/check")]
        public bool CheckDomainOwnership(string name)
        {
            var isOwnershipProven = ServerEngine.CheckDomainOwnership(name);
            return isOwnershipProven;
        }
    }
}
