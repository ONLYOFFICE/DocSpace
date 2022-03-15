namespace ASC.Core.Tenants;

[Serializable]
[DataContract]
public class TenantControlPanelSettings : ISettings
{
    [DataMember(Name = "LimitedAccess")]
    public bool LimitedAccess { get; set; }

    public Guid ID => new Guid("{880585C4-52CD-4AE2-8DA4-3B8E2772753B}");

    public ISettings GetDefault(IServiceProvider serviceProvider)
    {
        return new TenantControlPanelSettings
        {
            LimitedAccess = false
        };
    }
}
