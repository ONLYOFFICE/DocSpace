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
using System.Text;

using ASC.Common;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.EF.Model;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Security.Cryptography;

using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ASC.Core.Data
{
    [Scope]
    public class ConfigureDbTenantService : IConfigureNamedOptions<DbTenantService>
    {
        private readonly TenantDomainValidator _tenantDomainValidator;
        private readonly DbContextManager<TenantDbContext> _dbContextManager;

        public ConfigureDbTenantService(
            TenantDomainValidator tenantDomainValidator,
            DbContextManager<TenantDbContext> dbContextManager)
        {
            _tenantDomainValidator = tenantDomainValidator;
            _dbContextManager = dbContextManager;
        }

        public void Configure(string name, DbTenantService options)
        {
            Configure(options);
            options.LazyTenantDbContext = new Lazy<TenantDbContext>(() => _dbContextManager.Get(name));
        }

        public void Configure(DbTenantService options)
        {
            options.TenantDomainValidator = _tenantDomainValidator;
            options.LazyTenantDbContext = new Lazy<TenantDbContext>(() => _dbContextManager.Value);
        }
    }

    [Scope]
    public class DbTenantService : ITenantService
    {
        internal TenantDomainValidator TenantDomainValidator { get; set; }
        internal TenantDbContext TenantDbContext => LazyTenantDbContext.Value;
        internal Lazy<TenantDbContext> LazyTenantDbContext { get; set; }
        internal UserDbContext UserDbContext => LazyUserDbContext.Value;
        internal Lazy<UserDbContext> LazyUserDbContext { get; set; }
        
        private readonly MachinePseudoKeys _machinePseudoKeys;
        private readonly IMapper _mapper;

        private List<string> _forbiddenDomains;
        private static Expression<Func<DbTenant, Tenant>> _fromDbTenantToTenant;
        private static Expression<Func<TenantUserSecurity, Tenant>> _fromTenantUserToTenant;

        static DbTenantService()
        {
            _fromDbTenantToTenant = r => new Tenant
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
                StatusChangeDate = r.StatusChangeDate,
                TenantAlias = r.TenantAlias,
                TenantId = r.TenantId,
                MappedDomain = r.MappedDomain,
                Version = r.Version,
                VersionChanged = r.VersionChanged,
                //TrustedDomainsRaw = r.TrustedDomains,
                TrustedDomainsType = r.TrustedDomainsType,
                //AffiliateId = r.Partner != null ? r.Partner.AffiliateId : null,
                //PartnerId = r.Partner != null ? r.Partner.PartnerId : null,
                TimeZone = r.TimeZone,
                //Campaign = r.Partner != null ? r.Partner.Campaign : null
            };

            var fromDbTenantToTenant = _fromDbTenantToTenant.Compile();
            _fromTenantUserToTenant = r => fromDbTenantToTenant(r.DbTenant);
        }

        public DbTenantService() { }

        public DbTenantService(
            DbContextManager<TenantDbContext> dbContextManager,
            DbContextManager<UserDbContext> DbContextManager,
            TenantDomainValidator tenantDomainValidator,
            MachinePseudoKeys machinePseudoKeys,
            IMapper mapper)
        {
            LazyTenantDbContext = new Lazy<TenantDbContext>(() => dbContextManager.Value);
            LazyUserDbContext = new Lazy<UserDbContext>(() => DbContextManager.Value);
            TenantDomainValidator = tenantDomainValidator;
            _machinePseudoKeys = machinePseudoKeys;
            _mapper = mapper;
        }

        public void ValidateDomain(string domain)
        {
            using var tr = TenantDbContext.Database.BeginTransaction();
            ValidateDomain(domain, Tenant.DefaultTenant, true);
        }

        public IEnumerable<Tenant> GetTenants(DateTime from, bool active = true)
        {
            var q = TenantsQuery();

            if (active) q = q.Where(r => r.Status == TenantStatus.Active);

            if (from != default) q = q.Where(r => r.LastModified >= from);

            return q.Select(_mapper.Map<DbTenant, Tenant>).ToList();
        }

        public IEnumerable<Tenant> GetTenants(List<int> ids)
        {
            var q = TenantsQuery();

            return q.Where(r => ids.Contains(r.TenantId) && r.Status == TenantStatus.Active).Select(_mapper.Map<DbTenant, Tenant>).ToList();
        }

        public IEnumerable<Tenant> GetTenants(string login, string passwordHash)
        {
            if (string.IsNullOrEmpty(login)) throw new ArgumentNullException(nameof(login));

            IQueryable<TenantUserSecurity> query() => TenantsQuery()
                    .Where(r => r.Status == TenantStatus.Active)
                    .Join(UserDbContext.Users, r => r.TenantId, r => r.Tenant, (tenant, user) => new
                    {
                        tenant,
                        user
                    })
                    .Join(UserDbContext.UserSecurity, r => r.user.Id, r => r.UserId, (tenantUser, security) => new TenantUserSecurity
                    {
                        DbTenant = tenantUser.tenant,
                        User = tenantUser.user,
                        UserSecurity = security

                    })
                    .Where(r => r.User.Status == EmployeeStatus.Active)
                    .Where(r => r.DbTenant.Status == TenantStatus.Active)
                    .Where(r => r.User.Removed == false);

            if (passwordHash == null)
            {
                var q = query()
                    .Where(r => login.Contains('@') ? r.User.Email == login : r.User.Id.ToString() == login);

                return q.Select(_fromTenantUserToTenant).ToList();
            }

            if (Guid.TryParse(login, out var userId))
            {
                var pwdHash = GetPasswordHash(userId, passwordHash);
                var oldHash = Hasher.Base64Hash(passwordHash, HashAlg.SHA256);
                var q = query()
                    .Where(r => r.User.Id == userId)
                    .Where(r => r.UserSecurity.PwdHash == pwdHash || r.UserSecurity.PwdHash == oldHash)  //todo: remove old scheme
                    ;

                return q.Select(_fromTenantUserToTenant).ToList();
            }
            else
            {
                var oldHash = Hasher.Base64Hash(passwordHash, HashAlg.SHA256);

                var q =
                    query()
                    .Where(r => r.UserSecurity.PwdHash == oldHash);

                if (login.Contains('@')) q = q.Where(r => r.User.Email == login);

                else if (Guid.TryParse(login, out var uId)) q = q.Where(r => r.User.Id == uId);

                //old password
                var result = q.Select(_fromTenantUserToTenant).ToList();

                var usersQuery = UserDbContext.Users
                    .Where(r => r.Email == login)
                    .Where(r => r.Status == EmployeeStatus.Active)
                    .Where(r => !r.Removed)
                    .Select(r => r.Id)
                    .ToList();

                var passwordHashs = usersQuery.Select(r => GetPasswordHash(r, passwordHash)).ToList();

                q = query()
                    .Where(r => passwordHashs.Any(p => r.UserSecurity.PwdHash == p) && r.DbTenant.Status == TenantStatus.Active);

                //new password
                result = result.Concat(q.Select(_fromTenantUserToTenant)).ToList();
                result.Distinct();

                return result;
            }
        }

        public Tenant GetTenant(int id)
        {
            return TenantsQuery()
                .Where(r => r.TenantId == id)
                .Select(_mapper.Map<DbTenant, Tenant>)
                .SingleOrDefault();
        }

        public Tenant GetTenant(string domain)
        {
            if (string.IsNullOrEmpty(domain)) throw new ArgumentNullException(nameof(domain));

            domain = domain.ToLowerInvariant();

            return TenantsQuery()
                .Where(r => r.TenantAlias == domain || r.MappedDomain == domain)
                .OrderBy(a => a.Status == TenantStatus.Restoring ? TenantStatus.Active : a.Status)
                .ThenByDescending(a => a.Status == TenantStatus.Restoring ? 0 : a.TenantId)
                .Select(_mapper.Map<DbTenant, Tenant>)
                .FirstOrDefault();
        }

        public Tenant GetTenantForStandaloneWithoutAlias(string ip)
        {
            return TenantsQuery()
                .OrderBy(a => a.Status)
                .ThenByDescending(a => a.TenantId)
                .Select(_mapper.Map<DbTenant, Tenant>)
                .FirstOrDefault();
        }

        public Tenant SaveTenant(CoreSettings coreSettings, Tenant t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));

            using var tx = TenantDbContext.Database.BeginTransaction();

            if (!string.IsNullOrEmpty(t.MappedDomain))
            {
                var baseUrl = coreSettings.GetBaseDomain(t.HostedRegion);

                if (baseUrl != null && t.MappedDomain.EndsWith("." + baseUrl, StringComparison.InvariantCultureIgnoreCase))
                    ValidateDomain(t.MappedDomain.Substring(0, t.MappedDomain.Length - baseUrl.Length - 1), t.TenantId, false);
                else
                    ValidateDomain(t.MappedDomain, t.TenantId, false);
            }

            if (t.TenantId == Tenant.DefaultTenant)
            {
                t.Version = TenantDbContext.TenantVersion
                    .Where(r => r.DefaultVersion == 1 || r.Id == 0)
                    .OrderByDescending(r => r.Id)
                    .Select(r => r.Id)
                    .FirstOrDefault();

                var tenant = new DbTenant
                {
                    TenantId = t.TenantId,
                    TenantAlias = t.TenantAlias.ToLowerInvariant(),
                    MappedDomain = !string.IsNullOrEmpty(t.MappedDomain) ? t.MappedDomain.ToLowerInvariant() : null,
                    Version = t.Version,
                    VersionChanged = t.VersionChanged,
                    Name = t.Name ?? t.TenantAlias,
                    Language = t.Language,
                    TimeZone = t.TimeZone,
                    OwnerId = t.OwnerId,
                    TrustedDomains = t.GetTrustedDomains(),
                    TrustedDomainsType = t.TrustedDomainsType,
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
                t.TenantId = tenant.TenantId;
            }
            else
            {
                var tenant = TenantDbContext.Tenants
                    .Where(r => r.TenantId == t.TenantId)
                    .FirstOrDefault();

                if (tenant != null)
                {
                    tenant.TenantAlias = t.TenantAlias.ToLowerInvariant();
                    tenant.MappedDomain = !string.IsNullOrEmpty(t.MappedDomain) ? t.MappedDomain.ToLowerInvariant() : null;
                    tenant.Version = t.Version;
                    tenant.VersionChanged = t.VersionChanged;
                    tenant.Name = t.Name ?? t.TenantAlias;
                    tenant.Language = t.Language;
                    tenant.TimeZone = t.TimeZone;
                    tenant.TrustedDomains = t.GetTrustedDomains();
                    tenant.TrustedDomainsType = t.TrustedDomainsType;
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

            if (string.IsNullOrEmpty(t.PartnerId) && string.IsNullOrEmpty(t.AffiliateId) && string.IsNullOrEmpty(t.Campaign))
            {
                var p = TenantDbContext.TenantPartner
                    .Where(r => r.TenantId == t.TenantId)
                    .FirstOrDefault();

                if (p != null) TenantDbContext.TenantPartner.Remove(p);
            }
            else
            {
                var tenantPartner = new DbTenantPartner
                {
                    TenantId = t.TenantId,
                    PartnerId = t.PartnerId,
                    AffiliateId = t.AffiliateId,
                    Campaign = t.Campaign
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
                .Where(r => r.TenantId == id)
                .Select(r => r.TenantAlias)
                .FirstOrDefault();

            var count = TenantDbContext.Tenants
                .Where(r => r.TenantAlias.StartsWith(alias + postfix))
                .Count();

            var tenant = TenantDbContext.Tenants.Where(r => r.TenantId == id).FirstOrDefault();

            if (tenant != null)
            {
                tenant.TenantAlias = alias + postfix + (count > 0 ? count.ToString() : "");
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

                if (settings != null)
                    TenantDbContext.CoreSettings.Remove(settings);
            }
            else
            {
                var settings = new DbCoreSettings
                {
                    Id = key,
                    Tenant = tenant,
                    Value = data,
                    LastModified = DateTime.UtcNow
                };

                TenantDbContext.AddOrUpdate(r => r.CoreSettings, settings);
            }

            TenantDbContext.SaveChanges();
            tx.Commit();
        }

        protected string GetPasswordHash(Guid userId, string password)
        {
            return Hasher.Base64Hash(password + userId + Encoding.UTF8.GetString(_machinePseudoKeys.GetMachineConstant()), HashAlg.SHA512);
        }

        private IQueryable<DbTenant> TenantsQuery() => TenantDbContext.Tenants;

        private void ValidateDomain(string domain, int tenantId, bool validateCharacters)
        {
            // size
            TenantDomainValidator.ValidateDomainLength(domain);

            // characters
            if (validateCharacters)
                TenantDomainValidator.ValidateDomainCharacters(domain);

            // forbidden or exists
            var exists = false;
            domain = domain.ToLowerInvariant();

            if (!exists)
            {
                if (_forbiddenDomains == null)
                    _forbiddenDomains = TenantDbContext.TenantForbiden.Select(r => r.Address).ToList();

                exists = tenantId != 0 && _forbiddenDomains.Contains(domain);
            }

            if (!exists) exists = 0 < TenantDbContext.Tenants.Where(r => r.TenantAlias == domain && r.TenantId != tenantId).Count();

            if (!exists)
                exists = 0 < TenantDbContext.Tenants
                    .Where(r => r.MappedDomain == domain && r.TenantId != tenantId && !(r.Status == TenantStatus.RemovePending || r.Status == TenantStatus.Restoring))
                    .Count();

            if (exists)
            {
                // cut number suffix
                while (true)
                {
                    if (6 < domain.Length && char.IsNumber(domain, domain.Length - 1)) 
                        domain = domain[0..^1];
                    else 
                        break;
                }

                var existsTenants = TenantDbContext.TenantForbiden.Where(r => r.Address.StartsWith(domain)).Select(r => r.Address)
                    .Union(TenantDbContext.Tenants.Where(r => r.TenantAlias.StartsWith(domain) && r.TenantId != tenantId).Select(r => r.TenantAlias))
                    .Union(TenantDbContext.Tenants.Where(r => r.MappedDomain.StartsWith(domain) && r.TenantId != tenantId && r.Status != TenantStatus.RemovePending).Select(r => r.MappedDomain));

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