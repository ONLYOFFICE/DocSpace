namespace ASC.Web.Api.ApiModel.ResponseDto;

public class CapabilitiesDto
{
    public bool LdapEnabled { get; set; }
    public List<string> Providers { get; set; }
    public string SsoLabel { get; set; }

    /// <summary>
    /// if empty sso is disabled
    /// </summary>
    public string SsoUrl { get; set; }

    public static CapabilitiesDto GetSample()
    {
        return new CapabilitiesDto
        {
            LdapEnabled = false,
            // Providers = AccountLinkControl.AuthProviders,
            SsoLabel = string.Empty,
            SsoUrl = string.Empty,
        };
    }
}