namespace ASC.Web.Api.ApiModel.ResponseDto;

public class SettingsDto
{
    public string Timezone { get; set; }
    public List<string> TrustedDomains { get; set; }
    public TenantTrustedDomainsType TrustedDomainsType { get; set; }
    public string Culture { get; set; }
    public TimeSpan UtcOffset { get; set; }
    public double UtcHoursOffset { get; set; }
    public string GreetingSettings { get; set; }
    public Guid OwnerId { get; set; }
    public string NameSchemaId { get; set; }
    public bool? EnabledJoin { get; set; }
    public bool? EnableAdmMess { get; set; }
    public bool? ThirdpartyEnable { get; set; }
    public bool Personal { get; set; }
    public string WizardToken { get; set; }
    public PasswordHasher PasswordHash { get; set; }
    public FirebaseDto Firebase { get; set; }
    public string Version { get; set; }
    public string RecaptchaPublicKey { get; set; }
    public bool DebugInfo { get; set; }

    public static SettingsDto GetSample()
    {
        return new SettingsDto
        {
            Culture = "en-US",
            Timezone = TimeZoneInfo.Utc.ToString(),
            TrustedDomains = new List<string> { "mydomain.com" },
            UtcHoursOffset = -8.5,
            UtcOffset = TimeSpan.FromHours(-8.5),
            GreetingSettings = "Web Office Applications",
            OwnerId = new Guid()
        };
    }
}