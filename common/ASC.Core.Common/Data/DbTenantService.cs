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

using AutoMapper.QueryableExtensions;

namespace ASC.Core.Data;

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
    private List<string> _forbiddenDomains;

    internal TenantDomainValidator TenantDomainValidator;
    private readonly MachinePseudoKeys _machinePseudoKeys;
    internal TenantDbContext TenantDbContext => LazyTenantDbContext.Value;
    internal Lazy<TenantDbContext> LazyTenantDbContext;
    internal UserDbContext UserDbContext => LazyUserDbContext.Value;
    internal Lazy<UserDbContext> LazyUserDbContext;
    private readonly IMapper _mapper;

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

        if (active)
        {
            q = q.Where(r => r.Status == TenantStatus.Active);
        }

        if (from != default)
        {
            q = q.Where(r => r.LastModified >= from);
        }

        return q.ProjectTo<Tenant>(_mapper.ConfigurationProvider).ToList();
    }

    public IEnumerable<Tenant> GetTenants(List<int> ids)
    {
        var q = TenantsQuery();

        return q.Where(r => ids.Contains(r.Id) && r.Status == TenantStatus.Active)
            .ProjectTo<Tenant>(_mapper.ConfigurationProvider).ToList();
    }

    public IEnumerable<Tenant> GetTenants(string login, string passwordHash)
    {
        if (string.IsNullOrEmpty(login))
        {
            throw new ArgumentNullException(nameof(login));
        }

        IQueryable<TenantUserSecurity> query() => TenantsQuery()
                .Where(r => r.Status == TenantStatus.Active)
                .Join(UserDbContext.Users, r => r.Id, r => r.Tenant, (tenant, user) => new
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
                .Where(r => !r.User.Removed);

        if (passwordHash == null)
        {
            var q = query()
                .Where(r => login.Contains('@') ? r.User.Email == login : r.User.Id.ToString() == login);

            return q.ProjectTo<Tenant>(_mapper.ConfigurationProvider).ToList();
        }

        if (Guid.TryParse(login, out var userId))
        {
            var pwdHash = GetPasswordHash(userId, passwordHash);
            var oldHash = Hasher.Base64Hash(passwordHash, HashAlg.SHA256);
            var q = query()
                .Where(r => r.User.Id == userId)
                .Where(r => r.UserSecurity.PwdHash == pwdHash || r.UserSecurity.PwdHash == oldHash)  //todo: remove old scheme
                ;

            return q.ProjectTo<Tenant>(_mapper.ConfigurationProvider).ToList();
        }
        else
        {
            var oldHash = Hasher.Base64Hash(passwordHash, HashAlg.SHA256);

            var q =
                query()
                .Where(r => r.UserSecurity.PwdHash == oldHash);

            if (login.Contains('@'))
            {
                q = q.Where(r => r.User.Email == login);
            }
            else if (Guid.TryParse(login, out var uId))
            {
                q = q.Where(r => r.User.Id == uId);
            }

            //old password
            var result = q.ProjectTo<Tenant>(_mapper.ConfigurationProvider).ToList();

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
            result = result.Concat(q.ProjectTo<Tenant>(_mapper.ConfigurationProvider)).ToList();

            return result.Distinct();
        }
    }

    public Tenant GetTenant(int id)
    {
        return TenantsQuery()
            .Where(r => r.Id == id)
            .ProjectTo<Tenant>(_mapper.ConfigurationProvider)
            .SingleOrDefault();
    }

    public Tenant GetTenant(string domain)
    {
        if (string.IsNullOrEmpty(domain))
        {
            throw new ArgumentNullException(nameof(domain));
        }

        domain = domain.ToLowerInvariant();

        return TenantsQuery()
            .Where(r => r.Alias == domain || r.MappedDomain == domain)
            .OrderBy(a => a.Status == TenantStatus.Restoring ? TenantStatus.Active : a.Status)
            .ThenByDescending(a => a.Status == TenantStatus.Restoring ? 0 : a.Id)
            .ProjectTo<Tenant>(_mapper.ConfigurationProvider)
            .FirstOrDefault();
    }

    public Tenant GetTenantForStandaloneWithoutAlias(string ip)
    {
        return TenantsQuery()
            .OrderBy(a => a.Status)
            .ThenByDescending(a => a.Id)
            .ProjectTo<Tenant>(_mapper.ConfigurationProvider)
            .FirstOrDefault();
    }

    public Tenant SaveTenant(CoreSettings coreSettings, Tenant t)
    {
        if (t == null)
        {
            throw new ArgumentNullException("tenant");
        }

        using var tx = TenantDbContext.Database.BeginTransaction();

        if (!string.IsNullOrEmpty(t.MappedDomain))
        {
            var baseUrl = coreSettings.GetBaseDomain(t.HostedRegion);

            if (baseUrl != null && t.MappedDomain.EndsWith("." + baseUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                ValidateDomain(t.MappedDomain.Substring(0, t.MappedDomain.Length - baseUrl.Length - 1), t.Id, false);
            }
            else
            {
                ValidateDomain(t.MappedDomain, t.Id, false);
            }
        }

        if (t.Id == Tenant.DefaultTenant)
        {
            t.Version = TenantDbContext.TenantVersion
                .Where(r => r.DefaultVersion == 1 || r.Id == 0)
                .OrderByDescending(r => r.Id)
                .Select(r => r.Id)
                .FirstOrDefault();

            t.LastModified = DateTime.UtcNow;

            var tenant = _mapper.Map<Tenant, DbTenant>(t);

            tenant = TenantDbContext.Tenants.Add(tenant).Entity;
            TenantDbContext.SaveChanges();
            t.Id = tenant.Id;
        }
        else
        {
            var tenant = TenantDbContext.Tenants
                .Where(r => r.Id == t.Id)
                .FirstOrDefault();

            if (tenant != null)
            {
                tenant.Alias = t.Alias.ToLowerInvariant();
                tenant.MappedDomain = !string.IsNullOrEmpty(t.MappedDomain) ? t.MappedDomain.ToLowerInvariant() : null;
                tenant.Version = t.Version;
                tenant.VersionChanged = t.VersionChanged;
                tenant.Name = t.Name ?? t.Alias;
                tenant.Language = t.Language;
                tenant.TimeZone = t.TimeZone;
                tenant.TrustedDomainsRaw = t.GetTrustedDomains();
                tenant.TrustedDomainsEnabled = t.TrustedDomainsType;
                tenant.CreationDateTime = t.CreationDateTime;
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
                .Where(r => r.TenantId == t.Id)
                .FirstOrDefault();

            if (p != null)
            {
                TenantDbContext.TenantPartner.Remove(p);
            }
        }
        else
        {
            var tenantPartner = new DbTenantPartner
            {
                TenantId = t.Id,
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
            .Where(r => r.Visible)
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
            {
                TenantDbContext.CoreSettings.Remove(settings);
            }
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

    private IQueryable<DbTenant> TenantsQuery()
    {
        return TenantDbContext.Tenants;
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
        if (_forbiddenDomains == null)
        {
            _forbiddenDomains = TenantDbContext.TenantForbiden.Select(r => r.Address).ToList();
        }

        exists = tenantId != 0 && _forbiddenDomains.Contains(domain);

        if (!exists)
        {
            exists = TenantDbContext.Tenants.Where(r => r.Alias == domain && r.Id != tenantId).Any();
        }
        if (!exists)
        {
            exists = TenantDbContext.Tenants
                .Where(r => r.MappedDomain == domain && r.Id != tenantId && !(r.Status == TenantStatus.RemovePending || r.Status == TenantStatus.Restoring))
                .Any();
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

    protected string GetPasswordHash(Guid userId, string password)
    {
        return Hasher.Base64Hash(password + userId + Encoding.UTF8.GetString(_machinePseudoKeys.GetMachineConstant()), HashAlg.SHA512);
    }
}

public class TenantUserSecurity
{
    public DbTenant DbTenant { get; set; }
    public User User { get; set; }
    public UserSecurity UserSecurity { get; set; }
}
