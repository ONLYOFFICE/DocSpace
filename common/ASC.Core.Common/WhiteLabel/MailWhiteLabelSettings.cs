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

namespace ASC.Web.Core.WhiteLabel;

[Serializable]
public class MailWhiteLabelSettings : ISettings<MailWhiteLabelSettings>
{
    private readonly MailWhiteLabelSettingsHelper _mailWhiteLabelSettingsHelper;
    private readonly IConfiguration _configuration;

    public bool FooterEnabled { get; set; }
    public bool FooterSocialEnabled { get; set; }
    public string SupportUrl { get; set; }
    public string SupportEmail { get; set; }
    public string SalesEmail { get; set; }
    public string DemoUrl { get; set; }
    public string SiteUrl { get; set; }

    [JsonIgnore]
    public Guid ID => new Guid("{C3602052-5BA2-452A-BD2A-ADD0FAF8EB88}");

    public MailWhiteLabelSettings(IConfiguration configuration)
    {
        _mailWhiteLabelSettingsHelper = new MailWhiteLabelSettingsHelper(configuration);
        _configuration = configuration;
    }

    public MailWhiteLabelSettings()
    {

    }

    public MailWhiteLabelSettings GetDefault()
    {
        return new MailWhiteLabelSettings(_configuration)
        {
            FooterEnabled = true,
            FooterSocialEnabled = true,
            SupportUrl = _mailWhiteLabelSettingsHelper?.DefaultMailSupportUrl,
            SupportEmail = _mailWhiteLabelSettingsHelper?.DefaultMailSupportEmail,
            SalesEmail = _mailWhiteLabelSettingsHelper?.DefaultMailSalesEmail,
            DemoUrl = _mailWhiteLabelSettingsHelper?.DefaultMailDemoUrl,
            SiteUrl = _mailWhiteLabelSettingsHelper?.DefaultMailSiteUrl
        };
    }

    public bool IsDefault()
    {
        var defaultSettings = GetDefault();
        return FooterEnabled == defaultSettings.FooterEnabled &&
                FooterSocialEnabled == defaultSettings.FooterSocialEnabled &&
                SupportUrl == defaultSettings.SupportUrl &&
                SupportEmail == defaultSettings.SupportEmail &&
                SalesEmail == defaultSettings.SalesEmail &&
                DemoUrl == defaultSettings.DemoUrl &&
                SiteUrl == defaultSettings.SiteUrl;
    }

    public static async Task<MailWhiteLabelSettings> InstanceAsync(SettingsManager settingsManager)
    {
        return await settingsManager.LoadForDefaultTenantAsync<MailWhiteLabelSettings>();
    }

    public static async Task<bool> IsDefaultAsync(SettingsManager settingsManager)
    {
        return (await InstanceAsync(settingsManager)).IsDefault();
    }
}

[Singletone]
public class MailWhiteLabelSettingsHelper
{
    private readonly IConfiguration _configuration;

    public MailWhiteLabelSettingsHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string DefaultMailSupportUrl
    {
        get
        {
            var url = BaseCommonLinkUtility.GetRegionalUrl(_configuration["web:support-feedback"] ?? string.Empty, null);

            return !string.IsNullOrEmpty(url) ? url : "http://helpdesk.onlyoffice.com";
        }
    }

    public string DefaultMailSupportEmail
    {
        get
        {
            var email = _configuration["web:support:email"];

            return !string.IsNullOrEmpty(email) ? email : "support@onlyoffice.com";
        }
    }

    public string DefaultMailSalesEmail
    {
        get
        {
            var email = _configuration["web:payment:email"];

            return !string.IsNullOrEmpty(email) ? email : "sales@onlyoffice.com";
        }
    }

    public string DefaultMailDemoUrl
    {
        get
        {
            var url = BaseCommonLinkUtility.GetRegionalUrl(_configuration["web:demo-order"] ?? string.Empty, null);

            return !string.IsNullOrEmpty(url) ? url : "http://www.onlyoffice.com/demo-order.aspx";
        }
    }

    public string DefaultMailSiteUrl
    {
        get
        {
            var url = _configuration["web:teamlab-site"];

            return !string.IsNullOrEmpty(url) ? url : "http://www.onlyoffice.com";
        }
    }
}
