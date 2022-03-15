namespace ASC.Core;

[Scope(typeof(ConfigureDbTenantService), typeof(ConfigureCachedTenantService))]
public interface ITenantService
{
    byte[] GetTenantSettings(int tenant, string key);
    IEnumerable<Tenant> GetTenants(DateTime from, bool active = true);
    IEnumerable<Tenant> GetTenants(List<int> ids);
    IEnumerable<Tenant> GetTenants(string login, string passwordHash);
    IEnumerable<TenantVersion> GetTenantVersions();
    Tenant GetTenant(int id);
    Tenant GetTenant(string domain);
    Tenant GetTenantForStandaloneWithoutAlias(string ip);
    Tenant SaveTenant(CoreSettings coreSettings, Tenant tenant);
    void RemoveTenant(int id, bool auto = false);
    void SetTenantSettings(int tenant, string key, byte[] data);
    void ValidateDomain(string domain);
}
