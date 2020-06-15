/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Common;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Mail.Core.Dao.Entities;
using ASC.Mail.Core.Dao.Interfaces;
using ASC.Mail.Core.Entities;

namespace ASC.Mail.Core.Dao
{
    public class ServerDomainDao : BaseDao, IServerDomainDao
    {
        public ServerDomainDao(
             TenantManager tenantManager,
             SecurityContext securityContext,
             DbContextManager<MailDbContext> dbContext)
            : base(tenantManager, securityContext, dbContext)
        {
        }

        public int Save(ServerDomain domain)
        {
            var mailServerDomain = new MailServerDomain { 
                Id = domain.Id,
                Name = domain.Name,
                Tenant = domain.Tenant,
                IsVerified = domain.IsVerified
            };

            if (domain.Id <= 0)
            {
                mailServerDomain.DateAdded = DateTime.UtcNow;
            }

            var entry = MailDb.MailServerDomain.Add(mailServerDomain).Entity;

            MailDb.SaveChanges();

            return entry.Id;
        }

        public int Delete(int id)
        {
            var mailServerDomain = new MailServerDomain
            {
                Id = id,
                Tenant = Tenant
            };

            MailDb.MailServerDomain.Remove(mailServerDomain);

            var result = MailDb.SaveChanges();

            var mailServerDns = new MailServerDns
            {
                IdDomain = id,
                Tenant = Tenant
            };

            MailDb.MailServerDns.Remove(mailServerDns);

            MailDb.SaveChanges();

            return result;
        }

        public List<ServerDomain> GetDomains()
        {
            var tenants = new List<int> { Tenant, Defines.SHARED_TENANT_ID };

            var list = MailDb.MailServerDomain
                .Where(d => tenants.Contains(d.Tenant))
                .Select(ToServerDomain)
                .ToList();

            return list;
        }

        public List<ServerDomain> GetAllDomains()
        {
            var query = Query();

            var list = Db.ExecuteList(query)
                .ConvertAll(ToServerDomain);

            return list;
        }

        public ServerDomain GetDomain(int id)
        {
            var tenants = new List<int> { Tenant, Defines.SHARED_TENANT_ID };

            var domain = MailDb.MailServerDomain
                .Where(d => tenants.Contains(d.Tenant))
                .Where(d => d.Id == id)
                .Select(ToServerDomain)
                .SingleOrDefault();

            return domain;
        }

        public bool IsDomainExists(string name)
        {
            var domain = MailDb.MailServerDomain
                .Where(d => d.Name == name)
                .Select(ToServerDomain)
                .SingleOrDefault();

            return domain != null;
        }

        public int SetVerified(int id, bool isVerified)
        {
            var domain = GetDomain(id);

            domain.IsVerified = isVerified;
            domain.DateChecked = DateTime.UtcNow;

            var result = MailDb.SaveChanges();

            return result;
        }

        protected ServerDomain ToServerDomain(MailServerDomain r)
        {
            var d = new ServerDomain
            {
                Id = r.Id,
                Tenant = r.Tenant,
                Name = r.Name,
                IsVerified = r.IsVerified,
                DateAdded = r.DateAdded,
                DateChecked = r.DateChecked
            };

            return d;
        }
    }

    public static class ServerDomainDaoExtension
    {
        public static DIHelper AddServerDomainDaoService(this DIHelper services)
        {
            services.TryAddScoped<ServerDomainDao>();

            return services;
        }
    }
}