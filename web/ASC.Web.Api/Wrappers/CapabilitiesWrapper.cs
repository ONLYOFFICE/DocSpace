namespace ASC.Web.Api.Wrappers;

public class CapabilitiesWrapper
{
    public bool LdapEnabled { get; set; }

    public List<string> Providers { get; set; }

    public string SsoLabel { get; set; }

    /// <summary>
    /// if empty sso is disabled
    /// </summary>
    public string SsoUrl { get; set; }

    public static CapabilitiesWrapper GetSample()
    {
        return new CapabilitiesWrapper
        {
            LdapEnabled = false,
            // Providers = AccountLinkControl.AuthProviders,
            SsoLabel = string.Empty,
            SsoUrl = string.Empty,
        };
    }
}