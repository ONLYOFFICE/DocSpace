// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Core.Data;

[Scope]
public class DbTenantService : ITenantService
{
    private readonly TenantDomainValidator _tenantDomainValidator;
    private readonly IDbContextFactory<TenantDbContext> _dbContextFactory;
    private readonly IDbContextFactory<UserDbContext> _userDbContextFactory;
    private readonly IMapper _mapper;
    private readonly MachinePseudoKeys _machinePseudoKeys;
    private List<string> _forbiddenDomains;

    public DbTenantService(
        IDbContextFactory<TenantDbContext> dbContextFactory,
        IDbContextFactory<UserDbContext> userDbContextFactory,
        TenantDomainValidator tenantDomainValidator,
        MachinePseudoKeys machinePseudoKeys,
        IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _userDbContextFactory = userDbContextFactory;
        _tenantDomainValidator = tenantDomainValidator;
        _machinePseudoKeys = machinePseudoKeys;
        _mapper = mapper;
    }

    public void ValidateDomain(string domain)
    {
        // TODO: Why does open transaction?
        //        using var tr = TenantDbContext.Database.BeginTransaction();
        ValidateDomain(domain, Tenant.DefaultTenant, true);
    }

    public IEnumerable<Tenant> GetTenants(DateTime from, bool active = true)
    {
        using var tenantDbContext = _dbContextFactory.CreateDbContext();
        var q = tenantDbContext.Tenants.AsQueryable();

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
        using var tenantDbContext = _dbContextFactory.CreateDbContext();

        return tenantDbContext.Tenants
            .Where(r => ids.Contains(r.Id) && r.Status == TenantStatus.Active)
            .ProjectTo<Tenant>(_mapper.ConfigurationProvider)
            .ToList();
    }

    public IEnumerable<Tenant> GetTenants(string login, string passwordHash)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(login);

        using var tenantDbContext = _dbContextFactory.CreateDbContext();
        using var userDbContext = _userDbContextFactory.CreateDbContext();//TODO: remove
        IQueryable<TenantUserSecurity> query() => tenantDbContext.Tenants
                .Where(r => r.Status == TenantStatus.Active)
                .Join(userDbContext.Users, r => r.Id, r => r.Tenant, (tenant, user) => new
                {
                    tenant,
                    user
                })
                .Join(userDbContext.UserSecurity, r => r.user.Id, r => r.UserId, (tenantUser, security) => new TenantUserSecurity
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
            var q = query()
                .Where(r => r.User.Id == userId)
                .Where(r => r.UserSecurity.PwdHash == pwdHash)
                ;

            return q.ProjectTo<Tenant>(_mapper.ConfigurationProvider).ToList();
        }
        else
        {
            var usersQuery = userDbContext.Users
                .Where(r => r.Email == login)
                .Where(r => r.Status == EmployeeStatus.Active)
                .Where(r => !r.Removed)
                .Select(r => r.Id)
                .ToList();

            var passwordHashs = usersQuery.Select(r => GetPasswordHash(r, passwordHash)).ToList();

            var q = query()
                .Where(r => passwordHashs.Any(p => r.UserSecurity.PwdHash == p) && r.DbTenant.Status == TenantStatus.Active);

            return q.ProjectTo<Tenant>(_mapper.ConfigurationProvider).ToList();
        }
    }

    public Tenant GetTenant(int id)
    {
        using var tenantDbContext = _dbContextFactory.CreateDbContext();
        return tenantDbContext.Tenants
            .Where(r => r.Id == id)
            .ProjectTo<Tenant>(_mapper.ConfigurationProvider)
            .SingleOrDefault();
    }

    public Tenant GetTenant(string domain)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(domain);

        domain = domain.ToLowerInvariant();

        using var tenantDbContext = _dbContextFactory.CreateDbContext();

        return tenantDbContext.Tenants
            .Where(r => r.Alias == domain || r.MappedDomain == domain)
            .OrderBy(a => a.Status == TenantStatus.Restoring ? TenantStatus.Active : a.Status)
            .ThenByDescending(a => a.Status == TenantStatus.Restoring ? 0 : a.Id)
            .ProjectTo<Tenant>(_mapper.ConfigurationProvider)
            .FirstOrDefault();
    }

    public Tenant GetTenantForStandaloneWithoutAlias(string ip)
    {
        using var tenantDbContext = _dbContextFactory.CreateDbContext();

        return tenantDbContext.Tenants
            .OrderBy(a => a.Status)
            .ThenByDescending(a => a.Id)
            .ProjectTo<Tenant>(_mapper.ConfigurationProvider)
            .FirstOrDefault();
    }

    public Tenant SaveTenant(CoreSettings coreSettings, Tenant tenant)
    {
        ArgumentNullException.ThrowIfNull(tenant);

        using var tenantDbContext = _dbContextFactory.CreateDbContext();
        var strategy = tenantDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var tenantDbContext = _dbContextFactory.CreateDbContext();
            using var tx = tenantDbContext.Database.BeginTransaction();

            if (!string.IsNullOrEmpty(tenant.MappedDomain))
            {
                var baseUrl = coreSettings.GetBaseDomain(tenant.HostedRegion);

                if (baseUrl != null && tenant.MappedDomain.EndsWith("." + baseUrl, StringComparison.InvariantCultureIgnoreCase))
                {
                    ValidateDomain(tenant.MappedDomain.Substring(0, tenant.MappedDomain.Length - baseUrl.Length - 1), tenant.Id, false);
                }
                else
                {
                    ValidateDomain(tenant.MappedDomain, tenant.Id, false);
                }
            }

            if (tenant.Id == Tenant.DefaultTenant)
            {
                tenant.Version = tenantDbContext.TenantVersion
                    .Where(r => r.DefaultVersion == 1 || r.Id == 0)
                    .OrderByDescending(r => r.Id)
                    .Select(r => r.Id)
                    .FirstOrDefault();

                tenant.LastModified = DateTime.UtcNow;

                var dbTenant = _mapper.Map<Tenant, DbTenant>(tenant);

                dbTenant = tenantDbContext.Tenants.Add(dbTenant).Entity;
                tenantDbContext.SaveChanges();
                tenant.Id = dbTenant.Id;
            }
            else
            {
                var dbTenant = tenantDbContext.Tenants
                    .Where(r => r.Id == tenant.Id)
                    .FirstOrDefault();

                if (dbTenant != null)
                {
                    dbTenant.Alias = tenant.Alias.ToLowerInvariant();
                    dbTenant.MappedDomain = !string.IsNullOrEmpty(tenant.MappedDomain) ? tenant.MappedDomain.ToLowerInvariant() : null;
                    dbTenant.Version = tenant.Version;
                    dbTenant.VersionChanged = tenant.VersionChanged;
                    dbTenant.Name = tenant.Name ?? tenant.Alias;
                    dbTenant.Language = tenant.Language;
                    dbTenant.TimeZone = tenant.TimeZone;
                    dbTenant.TrustedDomainsRaw = tenant.GetTrustedDomains();
                    dbTenant.TrustedDomainsEnabled = tenant.TrustedDomainsType;
                    dbTenant.CreationDateTime = tenant.CreationDateTime;
                    dbTenant.Status = tenant.Status;
                    dbTenant.StatusChanged = tenant.StatusChangeDate;
                    dbTenant.PaymentId = tenant.PaymentId;
                    dbTenant.LastModified = tenant.LastModified = DateTime.UtcNow;
                    dbTenant.Industry = tenant.Industry;
                    dbTenant.Spam = tenant.Spam;
                    dbTenant.Calls = tenant.Calls;
                }

                tenantDbContext.SaveChanges();
            }

            tx.Commit();
        });

        //CalculateTenantDomain(t);
        return tenant;
    }

    public void RemoveTenant(int id, bool auto = false)
    {
        var postfix = auto ? "_auto_deleted" : "_deleted";

        using var tenantDbContext = _dbContextFactory.CreateDbContext();
        var strategy = tenantDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var tenantDbContext = _dbContextFactory.CreateDbContext();
            using var tx = tenantDbContext.Database.BeginTransaction();

            var alias = tenantDbContext.Tenants
                .Where(r => r.Id == id)
                .Select(r => r.Alias)
                .FirstOrDefault();

            var count = tenantDbContext.Tenants
                .Where(r => r.Alias.StartsWith(alias + postfix))
                .Count();

            var tenant = tenantDbContext.Tenants.Where(r => r.Id == id).FirstOrDefault();

            if (tenant != null)
            {
                tenant.Alias = alias + postfix + (count > 0 ? count.ToString() : "");
                tenant.Status = TenantStatus.RemovePending;
                tenant.StatusChanged = DateTime.UtcNow;
                tenant.LastModified = DateTime.UtcNow;
            }

            tenantDbContext.SaveChanges();

            tx.Commit();
        });
    }

    public IEnumerable<TenantVersion> GetTenantVersions()
    {
        using var tenantDbContext = _dbContextFactory.CreateDbContext();
        return tenantDbContext.TenantVersion
            .Where(r => r.Visible)
            .Select(r => new TenantVersion(r.Id, r.Version))
            .ToList();
    }


    public byte[] GetTenantSettings(int tenant, string key)
    {
        using var tenantDbContext = _dbContextFactory.CreateDbContext();
        return tenantDbContext.CoreSettings
            .Where(r => r.Tenant == tenant)
            .Where(r => r.Id == key)
            .Select(r => r.Value)
            .FirstOrDefault();
    }

    public void SetTenantSettings(int tenant, string key, byte[] data)
    {
        using var tenantDbContext = _dbContextFactory.CreateDbContext();
        var strategy = tenantDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var tenantDbContext = _dbContextFactory.CreateDbContext();
            using var tx = tenantDbContext.Database.BeginTransaction();

            if (data == null || data.Length == 0)
            {
                var settings = tenantDbContext.CoreSettings
                    .Where(r => r.Tenant == tenant)
                    .Where(r => r.Id == key)
                    .FirstOrDefault();

                if (settings != null)
                {
                    tenantDbContext.CoreSettings.Remove(settings);
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

                tenantDbContext.AddOrUpdate(r => r.CoreSettings, settings);
            }

            tenantDbContext.SaveChanges();
            tx.Commit();
        });
    }

    private void ValidateDomain(string domain, int tenantId, bool validateCharacters)
    {
        // size
        _tenantDomainValidator.ValidateDomainLength(domain);

        // characters
        if (validateCharacters)
        {
            TenantDomainValidator.ValidateDomainCharacters(domain);
        }

        using var tenantDbContext = _dbContextFactory.CreateDbContext();
        // forbidden or exists
        var exists = false;

        domain = domain.ToLowerInvariant();
        if (_forbiddenDomains == null)
        {
            _forbiddenDomains = tenantDbContext.TenantForbiden.Select(r => r.Address).ToList();
        }

        exists = tenantId != 0 && _forbiddenDomains.Contains(domain);

        if (!exists)
        {
            exists = tenantDbContext.Tenants.Where(r => r.Alias == domain && r.Id != tenantId).Any();
        }
        if (!exists)
        {
            exists = tenantDbContext.Tenants
                .Where(r => r.MappedDomain == domain && r.Id != tenantId && !(r.Status == TenantStatus.RemovePending || r.Status == TenantStatus.Restoring))
                .Any();
        }
        if (exists)
        {
            // cut number suffix
            while (true)
            {
                if (_tenantDomainValidator.MinLength < domain.Length && char.IsNumber(domain, domain.Length - 1))
                {
                    domain = domain[0..^1];
                }
                else
                {
                    break;
                }
            }

            var existsTenants = tenantDbContext.TenantForbiden.Where(r => r.Address.StartsWith(domain)).Select(r => r.Address)
                .Union(tenantDbContext.Tenants.Where(r => r.Alias.StartsWith(domain) && r.Id != tenantId).Select(r => r.Alias))
                .Union(tenantDbContext.Tenants.Where(r => r.MappedDomain.StartsWith(domain) && r.Id != tenantId && r.Status != TenantStatus.RemovePending).Select(r => r.MappedDomain));

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
