using System.Collections.Generic;

namespace ASC.Web.Api.Models
{
    public class CapabilitiesData
    {
        public bool LdapEnabled { get; set; }

        public List<string> Providers { get; set; }

        public string SsoLabel { get; set; }

        /// <summary>
        /// if empty sso is disabled
        /// </summary>
        public string SsoUrl { get; set; }

        public static CapabilitiesData GetSample()
        {
            return new CapabilitiesData
            {
                LdapEnabled = false,
                // Providers = AccountLinkControl.AuthProviders,
                SsoLabel = string.Empty,
                SsoUrl = string.Empty,
            };
        }
    }
}
