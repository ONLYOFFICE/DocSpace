// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Web.Api.ApiModel.ResponseDto;

/// <summary>
/// </summary>
public class SettingsDto
{
    /// <summary>Time zone</summary>
    /// <type>System.String, System</type>
    public string Timezone { get; set; }

    /// <summary>List of trusted domains</summary>
    /// <type>System.Collections.Generic.List{System.String}, System.Collections.Generic</type>
    public List<string> TrustedDomains { get; set; }

    /// <summary>Trusted domains type</summary>
    /// <type>ASC.Core.Tenants.TenantTrustedDomainsType, ASC.Core.Common</type>
    public TenantTrustedDomainsType TrustedDomainsType { get; set; }

    /// <summary>Language</summary>
    /// <type>System.String, System</type>
    public string Culture { get; set; }

    /// <summary>UTC offset</summary>
    /// <type>System.TimeSpan, System</type>
    public TimeSpan UtcOffset { get; set; }

    /// <summary>UTC hours offset</summary>
    /// <type>System.Double, System</type>
    public double UtcHoursOffset { get; set; }

    /// <summary>Greeting settings</summary>
    /// <type>System.String, System</type>
    public string GreetingSettings { get; set; }

    /// <summary>Owner ID</summary>
    /// <type>System.Guid, System</type>
    public Guid OwnerId { get; set; }

    /// <summary>Team template ID</summary>
    /// <type>System.String, System</type>
    public string NameSchemaId { get; set; }

    /// <summary>Specifies if a user can join to the portal or not</summary>
    /// <type>System.Nullable{System.Boolean}, System</type>
    public bool? EnabledJoin { get; set; }

    /// <summary>Specifies if a user can send a message to the administrator or not</summary>
    /// <type>System.Nullable{System.Boolean}, System</type>
    public bool? EnableAdmMess { get; set; }

    /// <summary>Specifies if a user can connect third-party providers or not</summary>
    /// <type>System.Nullable{System.Boolean}, System</type>
    public bool? ThirdpartyEnable { get; set; }

    /// <summary>Specifies if this is a Personal portal or not</summary>
    /// <type>System.Boolean, System</type>
    public bool Personal { get; set; }

    /// <summary>Specifies if this is a DocSpace portal or not</summary>
    /// <type>System.Boolean, System</type>
    public bool DocSpace { get; set; }

    /// <summary>Specifies if this is a standalone portal or not</summary>
    /// <type>System.Boolean, System</type>
    public bool Standalone { get; set; }

    /// <summary>Base domain</summary>
    /// <type>System.String, System</type>
    public string BaseDomain { get; set; }

    /// <summary>Wizard token</summary>
    /// <type>System.String, System</type>
    public string WizardToken { get; set; }

    /// <summary>Password hash</summary>
    /// <type>ASC.Security.Cryptography.PasswordHasher, ASC.Common</type>
    public PasswordHasher PasswordHash { get; set; }

    /// <summary>Firebase parameters</summary>
    /// <type>ASC.Web.Api.ApiModel.ResponseDto.FirebaseDto, ASC.Web.Api</type>
    public FirebaseDto Firebase { get; set; }

    /// <summary>Version</summary>
    /// <type>System.String, System</type>
    public string Version { get; set; }

    /// <summary>ReCAPTCHA public key</summary>
    /// <type>System.String, System</type>
    public string RecaptchaPublicKey { get; set; }

    /// <summary>Specifies if the debug information will be sent or not</summary>
    /// <type>System.Boolean, System</type>
    public bool DebugInfo { get; set; }

    /// <summary>Socket URL</summary>
    /// <type>System.String, System</type>
    public string SocketUrl { get; set; }

    /// <summary>Tenant status</summary>
    /// <type>ASC.Core.Tenants.TenantStatus, ASC.Core.Common</type>
    public TenantStatus TenantStatus { get; set; }

    /// <summary>Tenant alias</summary>
    /// <type>System.String, System</type>
    public string TenantAlias { get; set; }

    /// <summary>Link to the help</summary>
    /// <type>System.String, System</type>
    public string HelpLink { get; set; }

    /// <summary>API documentation link</summary>
    /// <type>System.String, System</type>
    public string ApiDocsLink { get; set; }

    /// <summary>Domain validator</summary>
    /// <type>ASC.Core.Tenants.TenantDomainValidator, ASC.Core.Common</type>
    public TenantDomainValidator DomainValidator { get; set; }

    /// <summary>Zendesk key</summary>
    /// <type>System.String, System</type>
    public string ZendeskKey { get; set; }

    /// <summary>Email for training booking</summary>
    /// <type>System.String, System</type>
    public string BookTrainingEmail { get; set; }

    /// <summary>Documentation email</summary>
    /// <type>System.String, System</type>
    public string DocumentationEmail { get; set; }

    /// <summary>Legal terms</summary>
    /// <type>System.String, System</type>
    public string LegalTerms { get; set; }

    /// <summary>Specifies whether the cookie settings are enabled</summary>
    /// <type>System.Boolean, System</type>
    public bool CookieSettingsEnabled { get; set; }

    /// <summary>Plugins</summary>
    /// <type>ASC.Web.Api.ApiModel.ResponseDto.PluginsDto, ASC.Web.Api</type>
    public PluginsDto Plugins { get; set; }

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