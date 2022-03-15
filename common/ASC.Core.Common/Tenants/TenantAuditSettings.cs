namespace ASC.Core.Tenants;

[Serializable]
public class TenantAuditSettings : ISettings
{
    public const int MaxLifeTime = 180;

    public int LoginHistoryLifeTime { get; set; }
    public int AuditTrailLifeTime { get; set; }

    public static readonly Guid Guid = new Guid("{8337D0FB-AD67-4552-8297-802312E7F503}");
    public Guid ID => Guid;

    public ISettings GetDefault(IServiceProvider serviceProvider)
    {
        return new TenantAuditSettings
        {
            LoginHistoryLifeTime = MaxLifeTime,
            AuditTrailLifeTime = MaxLifeTime
        };
    }
}

public class TenantAuditSettingsWrapper
{
    public TenantAuditSettings settings { get; set; }
}
