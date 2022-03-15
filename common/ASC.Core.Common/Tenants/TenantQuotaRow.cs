namespace ASC.Core.Tenants;

[Serializable]
public class TenantQuotaRow : IMapFrom<DbQuotaRow>
{
    public int Tenant { get; set; }
    public string Path { get; set; }
    public long Counter { get; set; }
    public string Tag { get; set; }
}
