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
using System.Linq.Expressions;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.EF.Model;
using ASC.Core.Tenants;
using ASC.Core.Users;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Data
{
    public class DbTenantService : ITenantService
    {
        private List<string> forbiddenDomains;

        private TenantDomainValidator TenantDomainValidator { get; }

        private TenantDbContext TenantDbContext { get; }

        public Expression<Func<DbTenant, Tenant>> FromDbTenantToTenant { get; set; }
        public Expression<Func<TenantUserSecurity, Tenant>> FromTenantUserToTenant { get; set; }

        public DbTenantService(TenantDomainValidator tenantDomainValidator)
        {
            TenantDomainValidator = tenantDomainValidator;
            FromDbTenantToTenant = r => new Tenant
            {
                Calls = r.Calls,
                CreatedDateTime = r.CreationDateTime,
                Industry = r.Industry != null ? r.Industry.Value : TenantIndustry.Other,
                Language = r.Language,
                LastModified = r.LastModified,
                Name = r.Name,
                OwnerId = r.OwnerId,
                PaymentId = r.PaymentId,
                Spam = r.Spam,
                Status = r.Status,
                StatusChangeDate = r.StatusChanged,
                TenantAlias = r.Alias,
                TenantId = r.Id,
                MappedDomain = r.MappedDomain,
                Version = r.Version,
                VersionChanged = r.VersionChanged,
                TrustedDomainsRaw = r.TrustedDomains,
                TrustedDomainsType = r.TrustedDomainsEnabled,
                AffiliateId = r.Partner != null ? r.Partner.AffiliateId : null,
                PartnerId = r.Partner != null ? r.Partner.PartnerId : null,
                TimeZone = r.TimeZone
            };

            var fromDbTenantToTenant = FromDbTenantToTenant.Compile();
            FromTenantUserToTenant = r => fromDbTenantToTenant(r.DbTenant);
        }

        public DbTenantService(DbContextManager<TenantDbContext> dbContextManager, TenantDomainValidator tenantDomainValidator) : this(tenantDomainValidator)
        {
            TenantDbContext = dbContextManager.Value;
        }

        public DbTenantService(TenantDbContext dbContextManager, TenantDomainValidator tenantDomainValidator) : this(tenantDomainValidator)
        {
            TenantDbContext = dbContextManager;
        }


        public void ValidateDomain(string domain)
        {
            using var tr = TenantDbContext.Database.BeginTransaction();
            ValidateDomain(domain, Tenant.DEFAULT_TENANT, true);
        }

        public IEnumerable<Tenant> GetTenants(DateTime from, bool active = true)
        {
            var q = TenantsQuery();

            if (active)
            {
                q = q.Where(r => r.Status == TenantStatus.Active);
            }

            if (from != default)
            {
                q = q.Where(r => r.LastModified >= from);
            }

            return q.Select(FromDbTenantToTenant).ToList();
        }

        public IEnumerable<Tenant> GetTenants(string login, string passwordHash)
        {
            if (string.IsNullOrEmpty(login)) throw new ArgumentNullException("login");

            var q = TenantsQuery()
                .Where(r => r.Status == TenantStatus.Active)
                .Join(TenantDbContext.Users, r => r.Id, r => r.Tenant, (tenant, user) => new
                {
                    tenant,
                    user
                })
                .Join(TenantDbContext.UserSecurity, r => r.user.Id, r => r.UserId, (tenantUser, security) => new TenantUserSecurity
                {
                    DbTenant = tenantUser.tenant,
                    User = tenantUser.user,
                    UserSecurity = security

                })
                .Where(r => r.User.Status == EmployeeStatus.Active)
                .Where(r => r.User.Removed == false)
                .Where(r => login.Contains('@') ? r.User.Email == login : r.User.Id.ToString() == login);


            if (passwordHash != null)
            {
                q.Where(r => r.UserSecurity.PwdHash == passwordHash);
            }

            return q.Select(FromTenantUserToTenant).ToList();
        }

        public Tenant GetTenant(int id)
        {
            return TenantsQuery()
                .Where(r => r.Id == id)
                .Select(FromDbTenantToTenant)
                .SingleOrDefault();
        }

        public Tenant GetTenant(string domain)
        {
            if (string.IsNullOrEmpty(domain)) throw new ArgumentNullException("domain");

            return TenantsQuery()
                .Where(r => r.Alias == domain.ToLowerInvariant() || r.MappedDomain == domain.ToLowerInvariant())
                .OrderBy(a => a.Status == TenantStatus.Restoring ? TenantStatus.Active : a.Status)
                .ThenByDescending(a => a.Status == TenantStatus.Restoring ? 0 : a.Id)
                .Select(FromDbTenantToTenant)
                .FirstOrDefault();
        }

        public Tenant GetTenantForStandaloneWithoutAlias(string ip)
        {
            return TenantsQuery()
                .OrderBy(a => a.Status)
                .ThenByDescending(a => a.Id)
                .Select(FromDbTenantToTenant)
                .FirstOrDefault();
        }

        public Tenant SaveTenant(CoreSettings coreSettings, Tenant t)
        {
            if (t == null) throw new ArgumentNullException("tenant");

            using var tx = TenantDbContext.Database.BeginTransaction();

            if (!string.IsNullOrEmpty(t.MappedDomain))
            {
                var baseUrl = coreSettings.GetBaseDomain(t.HostedRegion);

                if (baseUrl != null && t.MappedDomain.EndsWith("." + baseUrl, StringComparison.InvariantCultureIgnoreCase))
                {
                    ValidateDomain(t.MappedDomain.Substring(0, t.MappedDomain.Length - baseUrl.Length - 1), t.TenantId, false);
                }
                else
                {
                    ValidateDomain(t.MappedDomain, t.TenantId, false);
                }
            }

            if (t.TenantId == Tenant.DEFAULT_TENANT)
            {
                t.Version = TenantDbContext.TenantVersion
                    .Where(r => r.DefaultVersion == 1 || r.Id == 0)
                    .OrderByDescending(r => r.Id)
                    .Select(r => r.Id)
                    .FirstOrDefault();

                var tenant = new DbTenant
                {
                    Id = t.TenantId,
                    Alias = t.TenantAlias.ToLowerInvariant(),
                    MappedDomain = !string.IsNullOrEmpty(t.MappedDomain) ? t.MappedDomain.ToLowerInvariant() : null,
                    Version = t.Version,
                    VersionChanged = t.VersionChanged,
                    Name = t.Name ?? t.TenantAlias,
                    Language = t.Language,
                    TimeZone = t.TimeZone,
                    OwnerId = t.OwnerId,
                    TrustedDomains = t.GetTrustedDomains(),
                    TrustedDomainsEnabled = t.TrustedDomainsType,
                    CreationDateTime = t.CreatedDateTime,
                    Status = t.Status,
                    StatusChanged = t.StatusChangeDate,
                    PaymentId = t.PaymentId,
                    LastModified = t.LastModified = DateTime.UtcNow,
                    Industry = t.Industry,
                    Spam = t.Spam,
                    Calls = t.Calls
                };

                tenant = TenantDbContext.Tenants.Add(tenant).Entity;
                TenantDbContext.SaveChanges();
                t.TenantId = tenant.Id;
            }
            else
            {
                var tenant = TenantDbContext.Tenants
                    .Where(r => r.Id == t.TenantId)
                    .FirstOrDefault();

                if (tenant != null)
                {
                    tenant.Alias = t.TenantAlias.ToLowerInvariant();
                    tenant.MappedDomain = !string.IsNullOrEmpty(t.MappedDomain) ? t.MappedDomain.ToLowerInvariant() : null;
                    tenant.Version = t.Version;
                    tenant.VersionChanged = t.VersionChanged;
                    tenant.Name = t.Name ?? t.TenantAlias;
                    tenant.Language = t.Language;
                    tenant.TimeZone = t.TimeZone;
                    tenant.TrustedDomains = t.GetTrustedDomains();
                    tenant.TrustedDomainsEnabled = t.TrustedDomainsType;
                    tenant.CreationDateTime = t.CreatedDateTime;
                    tenant.Status = t.Status;
                    tenant.StatusChanged = t.StatusChangeDate;
                    tenant.PaymentId = t.PaymentId;
                    tenant.LastModified = t.LastModified = DateTime.UtcNow;
                    tenant.Industry = t.Industry;
                    tenant.Spam = t.Spam;
                    tenant.Calls = t.Calls;
                }
                TenantDbContext.SaveChanges();
            }

            if (string.IsNullOrEmpty(t.PartnerId) && string.IsNullOrEmpty(t.AffiliateId))
            {
                var p = TenantDbContext.TenantPartner
                    .Where(r => r.TenantId == t.TenantId)
                    .FirstOrDefault();

                TenantDbContext.TenantPartner.Remove(p);
            }
            else
            {
                var tenantPartner = new DbTenantPartner
                {
                    TenantId = t.TenantId,
                    PartnerId = t.PartnerId,
                    AffiliateId = t.AffiliateId
                };

                TenantDbContext.TenantPartner.Add(tenantPartner);
            }

            tx.Commit();

            //CalculateTenantDomain(t);
            return t;
        }

        public void RemoveTenant(int id, bool auto = false)
        {
            var postfix = auto ? "_auto_deleted" : "_deleted";

            using var tx = TenantDbContext.Database.BeginTransaction();

            var alias = TenantDbContext.Tenants
                .Where(r => r.Id == id)
                .Select(r => r.Alias)
                .FirstOrDefault();

            var count = TenantDbContext.Tenants
                .Where(r => r.Alias.StartsWith(alias + postfix))
                .Count();

            var tenant = TenantDbContext.Tenants.Where(r => r.Id == id).FirstOrDefault();

            if (tenant != null)
            {
                tenant.Alias = alias + postfix + (count > 0 ? count.ToString() : "");
                tenant.Status = TenantStatus.RemovePending;
                tenant.StatusChanged = DateTime.UtcNow;
                tenant.LastModified = DateTime.UtcNow;
            }

            TenantDbContext.SaveChanges();

            tx.Commit();
        }

        public IEnumerable<TenantVersion> GetTenantVersions()
        {
            return TenantDbContext.TenantVersion
                .Where(r => r.Visible == true)
                .Select(r => new TenantVersion(r.Id, r.Version))
                .ToList();
        }


        public byte[] GetTenantSettings(int tenant, string key)
        {
            return TenantDbContext.CoreSettings
                .Where(r => r.Tenant == tenant)
                .Where(r => r.Id == key)
                .Select(r => r.Value)
                .FirstOrDefault();
        }

        public void SetTenantSettings(int tenant, string key, byte[] data)
        {
            using var tx = TenantDbContext.Database.BeginTransaction();

            if (data == null || data.Length == 0)
            {
                var settings = TenantDbContext.CoreSettings
                    .Where(r => r.Tenant == tenant)
                    .Where(r => r.Id == key)
                    .FirstOrDefault();
                TenantDbContext.CoreSettings.Remove(settings);
            }
            else
            {
                var settings = new DbCoreSettings
                {
                    Id = key,
                    Tenant = tenant,
                    Value = data
                };
                TenantDbContext.CoreSettings.Add(settings);
            }
            TenantDbContext.SaveChanges();
            tx.Commit();
        }

        private IQueryable<DbTenant> TenantsQuery()
        {
            return TenantDbContext.Tenants
                .Include(r => r.Partner);
        }

        private void ValidateDomain(string domain, int tenantId, bool validateCharacters)
        {
            // size
            TenantDomainValidator.ValidateDomainLength(domain);

            // characters
            if (validateCharacters)
            {
                TenantDomainValidator.ValidateDomainCharacters(domain);
            }

            // forbidden or exists
            var exists = false;
            domain = domain.ToLowerInvariant();
            if (!exists)
            {
                if (forbiddenDomains == null)
                {
                    forbiddenDomains = TenantDbContext.TenantForbiden.Select(r => r.Address).ToList();
                }
                exists = tenantId != 0 && forbiddenDomains.Contains(domain);
            }
            if (!exists)
            {
                exists = 0 < TenantDbContext.Tenants.Where(r => r.Alias == domain && r.Id != tenantId).Count();
            }
            if (!exists)
            {
                exists = 0 < TenantDbContext.Tenants
                    .Where(r => r.MappedDomain == domain && r.Id != tenantId && !(r.Status == TenantStatus.RemovePending || r.Status == TenantStatus.Restoring))
                    .Count();
            }
            if (exists)
            {
                // cut number suffix
                while (true)
                {
                    if (6 < domain.Length && char.IsNumber(domain, domain.Length - 1))
                    {
                        domain = domain[0..^1];
                    }
                    else
                    {
                        break;
                    }
                }

                var existsTenants = TenantDbContext.TenantForbiden.Where(r => r.Address.StartsWith(domain)).Select(r => r.Address)
                    .Union(TenantDbContext.Tenants.Where(r => r.Alias.StartsWith(domain) && r.Id != tenantId).Select(r => r.Alias))
                    .Union(TenantDbContext.Tenants.Where(r => r.MappedDomain.StartsWith(domain) && r.Id != tenantId && r.Status != TenantStatus.RemovePending).Select(r => r.MappedDomain));

                throw new TenantAlreadyExistsException("Address busy.", existsTenants);
            }
        }
    }

    public class TenantUserSecurity
    {
        public DbTenant DbTenant { get; set; }
        public User User { get; set; }
        public UserSecurity UserSecurity { get; set; }
    }
}
