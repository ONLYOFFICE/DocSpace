namespace ASC.Core.Tenants;

public class TenantRegistrationInfo
{
    public string Name { get; set; }
    public string Address { get; set; }
    public CultureInfo Culture { get; set; }
    public TimeZoneInfo TimeZoneInfo { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string MobilePhone { get; set; }
    public string PasswordHash { get; set; }
    public EmployeeActivationStatus ActivationStatus { get; set; }
    public string HostedRegion { get; set; }
    public string PartnerId { get; set; }
    public string AffiliateId { get; set; }
    public TenantIndustry Industry { get; set; }
    public bool Spam { get; set; }
    public bool Calls { get; set; }
    public string Campaign { get; set; }
    public bool LimitedControlPanel { get; set; }

    public TenantRegistrationInfo()
    {
        Culture = CultureInfo.CurrentCulture;
        TimeZoneInfo = TimeZoneInfo.Local;
    }
}
