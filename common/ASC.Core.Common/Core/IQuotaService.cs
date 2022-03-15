namespace ASC.Core;

[Scope(typeof(ConfigureDbQuotaService), typeof(ConfigureCachedQuotaService))]
public interface IQuotaService
{
    IEnumerable<TenantQuota> GetTenantQuotas();
    IEnumerable<TenantQuotaRow> FindTenantQuotaRows(int tenantId);
    TenantQuota GetTenantQuota(int id);
    TenantQuota SaveTenantQuota(TenantQuota quota);
    void RemoveTenantQuota(int id);
    void SetTenantQuotaRow(TenantQuotaRow row, bool exchange);
}
