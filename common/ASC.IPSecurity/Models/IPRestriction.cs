namespace ASC.IPSecurity;

[Serializable]
public class IPRestriction : IMapFrom<TenantIpRestrictions>
{
    public int Id { get; set; }
    public int Tenant { get; set; }
    public string Ip { get; set; }
}
