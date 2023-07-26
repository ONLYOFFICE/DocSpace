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
    public bool DocSpace { get; set; }
    public bool Standalone { get; set; }
    public string BaseDomain { get; set; }
    public string WizardToken { get; set; }
    public PasswordHasher PasswordHash { get; set; }
    public FirebaseDto Firebase { get; set; }
    public string Version { get; set; }
    public string RecaptchaPublicKey { get; set; }
    public bool DebugInfo { get; set; }
    public string SocketUrl { get; set; }
    public TenantStatus TenantStatus { get; set; }
    public string TenantAlias { get; set; }
    public string HelpLink { get; set; }
    public string ApiDocsLink { get; set; }
    public TenantDomainValidator DomainValidator { get; set; }
    public string ZendeskKey { get; set; }
    public string BookTrainingEmail { get; set; }
    public string DocumentationEmail { get; set; }
    public string LegalTerms { get; set; }
    public bool CookieSettingsEnabled { get; set; }

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