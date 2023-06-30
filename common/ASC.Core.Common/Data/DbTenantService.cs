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

    public async Task ValidateDomainAsync(string domain)
    {
        // TODO: Why does open transaction?
        //        using var tr = TenantDbContext.Database.BeginTransaction();
        await ValidateDomainAsync(domain, Tenant.DefaultTenant, true);
    }

    public async Task<IEnumerable<Tenant>> GetTenantsAsync(DateTime from, bool active = true)
    {
        await using var tenantDbContext = _dbContextFactory.CreateDbContext();
        var q = tenantDbContext.Tenants.AsQueryable();

        if (active)
        {
            q = q.Where(r => r.Status == TenantStatus.Active);
        }

        if (from != default)
        {
            q = q.Where(r => r.LastModified >= from);
        }

        return await q.ProjectTo<Tenant>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<IEnumerable<Tenant>> GetTenantsAsync(List<int> ids)
    {
        await using var tenantDbContext = _dbContextFactory.CreateDbContext();

        return await tenantDbContext.Tenants
            .Where(r => ids.Contains(r.Id) && r.Status == TenantStatus.Active)
            .ProjectTo<Tenant>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tenant>> GetTenantsAsync(string login, string passwordHash)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(login);

        await using var tenantDbContext = _dbContextFactory.CreateDbContext();
        await using var userDbContext = _userDbContextFactory.CreateDbContext();//TODO: remove
        IQueryable<TenantUserSecurity> query() => tenantDbContext.Tenants
                .Where(r => r.Status == TenantStatus.Active)
                .Join(userDbContext.Users, r => r.Id, r => r.TenantId, (tenant, user) => new
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

            return await q.ProjectTo<Tenant>(_mapper.ConfigurationProvider).ToListAsync();
        }

        if (Guid.TryParse(login, out var userId))
        {
            var pwdHash = GetPasswordHash(userId, passwordHash);
            var q = query()
                .Where(r => r.User.Id == userId)
                .Where(r => r.UserSecurity.PwdHash == pwdHash);

            return await q.ProjectTo<Tenant>(_mapper.ConfigurationProvider).ToListAsync();
        }
        else
        {
            var usersQuery = await userDbContext.Users
                .Where(r => r.Email == login)
                .Where(r => r.Status == EmployeeStatus.Active)
                .Where(r => !r.Removed)
                .Select(r => r.Id)
                .ToListAsync();

            var passwordHashs = usersQuery.Select(r => GetPasswordHash(r, passwordHash)).ToList();

            var q = query()
                .Where(r => passwordHashs.Any(p => r.UserSecurity.PwdHash == p) && r.DbTenant.Status == TenantStatus.Active);

            return await q.ProjectTo<Tenant>(_mapper.ConfigurationProvider).ToListAsync();
        }
    }

    public async Task<Tenant> GetTenantAsync(int id)
    {
        await using var tenantDbContext = _dbContextFactory.CreateDbContext();
        return await tenantDbContext.Tenants
            .Where(r => r.Id == id)
            .ProjectTo<Tenant>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public Tenant GetTenant(int id)
    {
        using var tenantDbContext = _dbContextFactory.CreateDbContext();
        return tenantDbContext.Tenants
            .Where(r => r.Id == id)
            .ProjectTo<Tenant>(_mapper.ConfigurationProvider)
            .SingleOrDefault();
    }

    public async Task<Tenant> GetTenantAsync(string domain)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(domain);

        domain = domain.ToLowerInvariant();

        await using var tenantDbContext = _dbContextFactory.CreateDbContext();

        return await tenantDbContext.Tenants
            .Where(r => r.Alias == domain || r.MappedDomain == domain)
            .OrderBy(a => a.Status == TenantStatus.Restoring ? TenantStatus.Active : a.Status)
            .ThenByDescending(a => a.Status == TenantStatus.Restoring ? 0 : a.Id)
            .ProjectTo<Tenant>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
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

    public async Task<Tenant> GetTenantForStandaloneWithoutAliasAsync(string ip)
    {
        await using var tenantDbContext = _dbContextFactory.CreateDbContext();

        return await tenantDbContext.Tenants
            .OrderBy(a => a.Status)
            .ThenByDescending(a => a.Id)
            .ProjectTo<Tenant>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<Tenant> SaveTenantAsync(CoreSettings coreSettings, Tenant tenant)
    {
        ArgumentNullException.ThrowIfNull(tenant);

        await using var tenantDbContext = _dbContextFactory.CreateDbContext();

        if (!string.IsNullOrEmpty(tenant.MappedDomain))
        {
            var baseUrl = coreSettings.GetBaseDomain(tenant.HostedRegion);

            if (baseUrl != null && tenant.MappedDomain.EndsWith("." + baseUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                await ValidateDomainAsync(tenant.MappedDomain.Substring(0, tenant.MappedDomain.Length - baseUrl.Length - 1), tenant.Id, false);
            }
            else
            {
                await ValidateDomainAsync(tenant.MappedDomain, tenant.Id, false);
            }
        }

        if (tenant.Id == Tenant.DefaultTenant)
        {
            tenant.Version = await Queries.VersionIdAsync(tenantDbContext);

            tenant.LastModified = DateTime.UtcNow;

            var dbTenant = _mapper.Map<Tenant, DbTenant>(tenant);
            dbTenant.Id = 0;
            dbTenant = (await tenantDbContext.Tenants.AddAsync(dbTenant)).Entity;
            await tenantDbContext.SaveChangesAsync();
            tenant.Id = dbTenant.Id;
        }
        else
        {
            var dbTenant = await Queries.TenantAsync(tenantDbContext, tenant.Id);

            if (dbTenant != null)
            {
                dbTenant.Alias = tenant.Alias.ToLowerInvariant();
                dbTenant.MappedDomain = !string.IsNullOrEmpty(tenant.MappedDomain) ? tenant.MappedDomain.ToLowerInvariant() : null;
                dbTenant.Version = tenant.Version;
                dbTenant.VersionChanged = tenant.VersionChanged;
                dbTenant.Name = tenant.Name ?? "";
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
                dbTenant.OwnerId = tenant.OwnerId;

                await tenantDbContext.SaveChangesAsync();
            }
        }

        //CalculateTenantDomain(t);
        return tenant;
    }

    public async Task RemoveTenantAsync(int id, bool auto = false)
    {
        var postfix = auto ? "_auto_deleted" : "_deleted";

        await using var tenantDbContext = _dbContextFactory.CreateDbContext();

        var alias = await Queries.GetAliasAsync(tenantDbContext, id);

        var count = await Queries.TenantsCountAsync(tenantDbContext, alias + postfix);

        var tenant = await Queries.TenantAsync(tenantDbContext, id);

        if (tenant != null)
        {
            tenant.Alias = alias + postfix + (count > 0 ? count.ToString() : "");
            tenant.Status = TenantStatus.RemovePending;
            tenant.StatusChanged = DateTime.UtcNow;
            tenant.LastModified = DateTime.UtcNow;

            await tenantDbContext.SaveChangesAsync();
        }
    }

    public async Task PermanentlyRemoveTenantAsync(int id)
    {
        using var tenantDbContext = _dbContextFactory.CreateDbContext();
        var tenant = await tenantDbContext.Tenants.SingleOrDefaultAsync(r => r.Id == id);
        tenantDbContext.Tenants.Remove(tenant);
        await tenantDbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<TenantVersion>> GetTenantVersionsAsync()
    {
        await using var tenantDbContext = _dbContextFactory.CreateDbContext();
        return await Queries.TenantVersionsAsync(tenantDbContext).ToListAsync();
    }


    public async Task<byte[]> GetTenantSettingsAsync(int tenant, string key)
    {
        await using var tenantDbContext = _dbContextFactory.CreateDbContext();
        return await Queries.SettingValueAsync(tenantDbContext, tenant, key);
    }

    public byte[] GetTenantSettings(int tenant, string key)
    {
        using var tenantDbContext = _dbContextFactory.CreateDbContext();
        return Queries.SettingValue(tenantDbContext, tenant, key);
    }


    public async Task SetTenantSettingsAsync(int tenant, string key, byte[] data)
    {
        await using var tenantDbContext = _dbContextFactory.CreateDbContext();
        if (data == null || data.Length == 0)
        {
            var settings = await Queries.CoreSettingsAsync(tenantDbContext, tenant, key);

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
                TenantId = tenant,
                Value = data,
                LastModified = DateTime.UtcNow
            };

            await tenantDbContext.AddOrUpdateAsync(q => q.CoreSettings, settings);
        }

        await tenantDbContext.SaveChangesAsync();
    }

    public void SetTenantSettings(int tenant, string key, byte[] data)
    {
        using var tenantDbContext = _dbContextFactory.CreateDbContext();
        if (data == null || data.Length == 0)
        {
            var settings = Queries.CoreSettings(tenantDbContext, tenant, key);

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
                TenantId = tenant,
                Value = data,
                LastModified = DateTime.UtcNow
            };

            tenantDbContext.AddOrUpdate(tenantDbContext.CoreSettings, settings);
        }

        tenantDbContext.SaveChanges();
    }

    private async Task ValidateDomainAsync(string domain, int tenantId, bool validateCharacters)
    {
        // size
        _tenantDomainValidator.ValidateDomainLength(domain);

        // characters
        if (validateCharacters)
        {
            _tenantDomainValidator.ValidateDomainCharacters(domain);
        }

        await using var tenantDbContext = _dbContextFactory.CreateDbContext();
        // forbidden or exists
        var exists = false;

        domain = domain.ToLowerInvariant();
        if (_forbiddenDomains == null)
        {
            _forbiddenDomains = await Queries.AddressAsync(tenantDbContext).ToListAsync();
        }

        exists = tenantId != 0 && _forbiddenDomains.Contains(domain);

        if (!exists)
        {
            exists = await Queries.AnyTenantsAsync(tenantDbContext, tenantId, domain);
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

            var existsTenants = await tenantDbContext.TenantForbiden.Where(r => r.Address.StartsWith(domain)).Select(r => r.Address).ToListAsync();
            existsTenants.AddRange(await tenantDbContext.Tenants.Where(r => r.Alias.StartsWith(domain) && r.Id != tenantId).Select(r => r.Alias).ToListAsync());
            existsTenants.AddRange(await tenantDbContext.Tenants.Where(r => r.MappedDomain.StartsWith(domain) && r.Id != tenantId && r.Status != TenantStatus.RemovePending).Select(r => r.MappedDomain).ToListAsync());

            throw new TenantAlreadyExistsException("Address busy.", existsTenants.Distinct());
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

static file class Queries
{
    public static readonly Func<TenantDbContext, Task<int>> VersionIdAsync =
        EF.CompileAsyncQuery(
            (TenantDbContext ctx) =>
                ctx.TenantVersion
                    .Where(r => r.DefaultVersion == 1 || r.Id == 0)
                    .OrderByDescending(r => r.Id)
                    .Select(r => r.Id)
                    .FirstOrDefault());

    public static readonly Func<TenantDbContext, int, Task<DbTenant>> TenantAsync =
        EF.CompileAsyncQuery(
            (TenantDbContext ctx, int tenantId) =>
                ctx.Tenants
                    .Where(r => r.Id == tenantId)
                    .FirstOrDefault());

    public static readonly Func<TenantDbContext, int, Task<string>> GetAliasAsync =
        EF.CompileAsyncQuery(
            (TenantDbContext ctx, int tenantId) =>
                ctx.Tenants
                    .Where(r => r.Id == tenantId)
                    .Select(r => r.Alias)
                    .FirstOrDefault());

    public static readonly Func<TenantDbContext, string, Task<int>> TenantsCountAsync = EF.CompileAsyncQuery(
    (TenantDbContext ctx, string startAlias) =>
        ctx.Tenants
            .Count(r => r.Alias.StartsWith(startAlias)));

    public static readonly Func<TenantDbContext, IAsyncEnumerable<TenantVersion>> TenantVersionsAsync =
        EF.CompileAsyncQuery(
            (TenantDbContext ctx) =>
                ctx.TenantVersion
                    .Where(r => r.Visible)
                    .Select(r => new TenantVersion(r.Id, r.Version)));

    public static readonly Func<TenantDbContext, int, string, Task<byte[]>> SettingValueAsync =
        EF.CompileAsyncQuery(
            (TenantDbContext ctx, int tenantId, string id) =>
                ctx.CoreSettings
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.Id == id)
                    .Select(r => r.Value)
                    .FirstOrDefault());

    public static readonly Func<TenantDbContext, int, string, byte[]> SettingValue =
        EF.CompileQuery(
            (TenantDbContext ctx, int tenantId, string id) =>
                ctx.CoreSettings
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.Id == id)
                    .Select(r => r.Value)
                    .FirstOrDefault());

    public static readonly Func<TenantDbContext, int, string, Task<DbCoreSettings>> CoreSettingsAsync =
        EF.CompileAsyncQuery(
            (TenantDbContext ctx, int tenantId, string id) =>
                ctx.CoreSettings
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.Id == id)
                    .FirstOrDefault());

    public static readonly Func<TenantDbContext, int, string, DbCoreSettings> CoreSettings =
        EF.CompileQuery(
            (TenantDbContext ctx, int tenantId, string id) =>
                ctx.CoreSettings
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.Id == id)
                    .FirstOrDefault());

    public static readonly Func<TenantDbContext, IAsyncEnumerable<string>> AddressAsync =
        EF.CompileAsyncQuery(
            (TenantDbContext ctx) =>
                ctx.TenantForbiden.Select(r => r.Address));

    public static readonly Func<TenantDbContext, int, string, Task<bool>> AnyTenantsAsync =
        EF.CompileAsyncQuery(
            (TenantDbContext ctx, int tenantId, string domain) =>
                ctx.Tenants
                    .Any(r => (r.Alias == domain ||
                              r.MappedDomain == domain && !(r.Status == TenantStatus.RemovePending ||
                                                           r.Status == TenantStatus.Restoring))
                                                       && r.Id != tenantId));
}