namespace ASC.IPSecurity;

[Serializable]
public class IPRestrictionsSettings : ISettings
{
    public bool Enable { get; set; }

    public Guid ID => new Guid("{2EDDDF64-F792-4498-A638-2E3E6EBB13C9}");

    public ISettings GetDefault(IServiceProvider serviceProvider)
    {
        return new IPRestrictionsSettings { Enable = false };
    }
}
