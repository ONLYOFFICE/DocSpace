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

/// <summary>
/// </summary>
public class AdditionalWhiteLabelSettingsWrapper
{
    /// <summary>Additional white label settings</summary>
    /// <type>ASC.Web.Core.WhiteLabel.AdditionalWhiteLabelSettings, ASC.Web.Core</type>
    public AdditionalWhiteLabelSettings Settings { get; set; }
}

/// <summary>
/// </summary>
[Serializable]
public class AdditionalWhiteLabelSettings : ISettings<AdditionalWhiteLabelSettings>
{
    /// <summary>Additional white label settings helper</summary>
    /// <type>ASC.Web.Core.WhiteLabel.AdditionalWhiteLabelSettingsHelperInit, ASC.Web.Core</type>
    public AdditionalWhiteLabelSettingsHelperInit AdditionalWhiteLabelSettingsHelper;

    /// <summary>Specifies if the start document is enabled or not</summary>
    /// <type>System.Boolean, System</type>
    public bool StartDocsEnabled { get; set; }

    /// <summary>Specifies if the help center is enabled or not</summary>
    /// <type>System.Boolean, System</type>
    public bool HelpCenterEnabled { get; set; }

    /// <summary>Specifies if feedback and support are available or not</summary>
    /// <type>System.Boolean, System</type>
    public bool FeedbackAndSupportEnabled { get; set; }

    /// <summary>Feedback and support URL</summary>
    /// <type>System.String, System</type>
    public string FeedbackAndSupportUrl { get; set; }

    /// <summary>Specifies if the user forum is enabled or not</summary>
    /// <type>System.Boolean, System</type>
    public bool UserForumEnabled { get; set; }

    /// <summary>User forum URL</summary>
    /// <type>System.String, System</type>
    public string UserForumUrl { get; set; }

    /// <summary>Specifies if the video guides are enabled or not</summary>
    /// <type>System.Boolean, System</type>
    public bool VideoGuidesEnabled { get; set; }

    /// <summary>Video guides URL</summary>
    /// <type>System.String, System</type>
    public string VideoGuidesUrl { get; set; }

    /// <summary>Sales email</summary>
    /// <type>System.String, System</type>
    public string SalesEmail { get; set; }

    /// <summary>URL to pay for the portal</summary>
    /// <type>System.String, System</type>
    public string BuyUrl { get; set; }

    /// <summary>Specifies if the license agreements are enabled or not</summary>
    /// <type>System.Boolean, System</type>
    public bool LicenseAgreementsEnabled { get; set; }

    /// <summary>License agreements URL</summary>
    /// <type>System.String, System</type>
    public string LicenseAgreementsUrl { get; set; }

    [JsonIgnore]
    public Guid ID
    {
        get { return new Guid("{0108422F-C05D-488E-B271-30C4032494DA}"); }
    }

    public AdditionalWhiteLabelSettings(AdditionalWhiteLabelSettingsHelperInit additionalWhiteLabelSettingsHelper)
    {
        this.AdditionalWhiteLabelSettingsHelper = additionalWhiteLabelSettingsHelper;
    }

    public AdditionalWhiteLabelSettings() { }

    public AdditionalWhiteLabelSettings GetDefault()
    {
        return new AdditionalWhiteLabelSettings(AdditionalWhiteLabelSettingsHelper)
        {
            StartDocsEnabled = true,
            HelpCenterEnabled = AdditionalWhiteLabelSettingsHelper?.DefaultHelpCenterUrl != null,
            FeedbackAndSupportEnabled = AdditionalWhiteLabelSettingsHelper?.DefaultFeedbackAndSupportUrl != null,
            FeedbackAndSupportUrl = AdditionalWhiteLabelSettingsHelper?.DefaultFeedbackAndSupportUrl,
            UserForumEnabled = AdditionalWhiteLabelSettingsHelper?.DefaultUserForumUrl != null,
            UserForumUrl = AdditionalWhiteLabelSettingsHelper?.DefaultUserForumUrl,
            VideoGuidesEnabled = AdditionalWhiteLabelSettingsHelper?.DefaultVideoGuidesUrl != null,
            VideoGuidesUrl = AdditionalWhiteLabelSettingsHelper?.DefaultVideoGuidesUrl,
            SalesEmail = AdditionalWhiteLabelSettingsHelper?.DefaultMailSalesEmail,
            BuyUrl = AdditionalWhiteLabelSettingsHelper?.DefaultBuyUrl,
            LicenseAgreementsEnabled = true,
            LicenseAgreementsUrl = DefaultLicenseAgreements
        };
    }

    public static string DefaultLicenseAgreements
    {
        get
        {
            return "https://help.onlyoffice.com/Products/Files/doceditor.aspx?fileid=6795868&doc=RG5GaVN6azdUQW5kLzZQNzBXbHZ4Rm9QWVZuNjZKUmgya0prWnpCd2dGcz0_IjY3OTU4Njgi0";
        }
    }
}

[Scope]
public class AdditionalWhiteLabelSettingsHelper
{
    private readonly AdditionalWhiteLabelSettingsHelperInit _additionalWhiteLabelSettingsHelperInit;

    public AdditionalWhiteLabelSettingsHelper(AdditionalWhiteLabelSettingsHelperInit additionalWhiteLabelSettingsHelperInit)
    {
        _additionalWhiteLabelSettingsHelperInit = additionalWhiteLabelSettingsHelperInit;
    }

    public bool IsDefault(AdditionalWhiteLabelSettings settings)
    {
        if (settings.AdditionalWhiteLabelSettingsHelper == null)
        {
            settings.AdditionalWhiteLabelSettingsHelper = _additionalWhiteLabelSettingsHelperInit;
        }
        var defaultSettings = settings.GetDefault();

        return settings.StartDocsEnabled == defaultSettings.StartDocsEnabled &&
                settings.HelpCenterEnabled == defaultSettings.HelpCenterEnabled &&
                settings.FeedbackAndSupportEnabled == defaultSettings.FeedbackAndSupportEnabled &&
                settings.FeedbackAndSupportUrl == defaultSettings.FeedbackAndSupportUrl &&
                settings.UserForumEnabled == defaultSettings.UserForumEnabled &&
                settings.UserForumUrl == defaultSettings.UserForumUrl &&
                settings.VideoGuidesEnabled == defaultSettings.VideoGuidesEnabled &&
                settings.VideoGuidesUrl == defaultSettings.VideoGuidesUrl &&
                settings.SalesEmail == defaultSettings.SalesEmail &&
                settings.BuyUrl == defaultSettings.BuyUrl &&
                settings.LicenseAgreementsEnabled == defaultSettings.LicenseAgreementsEnabled &&
                settings.LicenseAgreementsUrl == defaultSettings.LicenseAgreementsUrl;
    }
}

/// <summary>
/// </summary>
[Singletone]
public class AdditionalWhiteLabelSettingsHelperInit 
{
    private readonly IConfiguration _configuration;

    public AdditionalWhiteLabelSettingsHelperInit(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>Default help center URL</summary>
    /// <type>System.String, System</type>
    public string DefaultHelpCenterUrl
    {
        get
        {
            var url = _configuration["web:help-center"];
            return string.IsNullOrEmpty(url) ? null : url;
        }
    }

    /// <summary>Default feedback and support URL</summary>
    /// <type>System.String, System</type>
    public string DefaultFeedbackAndSupportUrl
    {
        get
        {
            var url = _configuration["web:support-feedback"];
            return string.IsNullOrEmpty(url) ? null : url;
        }
    }

    /// <summary>Default user forum URL</summary>
    /// <type>System.String, System</type>
    public string DefaultUserForumUrl
    {
        get
        {
            var url = _configuration["web:user-forum"];
            return string.IsNullOrEmpty(url) ? null : url;
        }
    }

    /// <summary>Default video guides URL</summary>
    /// <type>System.String, System</type>
    public string DefaultVideoGuidesUrl
    {
        get
        {
            var url = DefaultHelpCenterUrl;
            return string.IsNullOrEmpty(url) ? null : url + "/video.aspx";
        }
    }

    /// <summary>Mail sales URL</summary>
    /// <type>System.String, System</type>
    public string DefaultMailSalesEmail
    {
        get
        {
            var email = _configuration["core:payment:email"];
            return !string.IsNullOrEmpty(email) ? email : "sales@onlyoffice.com";
        }
    }

    /// <summary>Default URL to pay for the portal</summary>
    /// <type>System.String, System</type>
    public string DefaultBuyUrl
    {
        get
        {
            var site = _configuration["web:teamlab-site"];
            return !string.IsNullOrEmpty(site) ? site + "/post.ashx?type=buyenterprise" : "";
        }
    }
}
