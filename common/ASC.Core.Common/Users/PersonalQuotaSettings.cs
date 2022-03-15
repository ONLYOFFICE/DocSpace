namespace ASC.Core.Tenants;

[Serializable]
public class PersonalQuotaSettings : ISettings
{
    public long MaxSpace { get; set; }

    public Guid ID => new Guid("{C634A747-C39B-4517-8698-B3B39BF2BD8E}");

    public ISettings GetDefault(IServiceProvider serviceProvider)
    {
        return new PersonalQuotaSettings
        {
            MaxSpace = long.MaxValue
        };
    }
}
