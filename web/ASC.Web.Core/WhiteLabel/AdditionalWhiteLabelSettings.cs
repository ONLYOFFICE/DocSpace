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

public class AdditionalWhiteLabelSettingsWrapper
{
    public AdditionalWhiteLabelSettings Settings { get; set; }
}

[Serializable]
public class AdditionalWhiteLabelSettings : ISettings
{
    public bool StartDocsEnabled { get; set; }

    public bool HelpCenterEnabled { get; set; }

    public bool FeedbackAndSupportEnabled { get; set; }

    public string FeedbackAndSupportUrl { get; set; }

    public bool UserForumEnabled { get; set; }

    public string UserForumUrl { get; set; }

    public bool VideoGuidesEnabled { get; set; }

    public string VideoGuidesUrl { get; set; }

    public string SalesEmail { get; set; }

    public string BuyUrl { get; set; }

    public bool LicenseAgreementsEnabled { get; set; }

    public string LicenseAgreementsUrl { get; set; }

    public bool IsDefault(IConfiguration configuration)
    {
        if (!(GetDefault(configuration) is AdditionalWhiteLabelSettings defaultSettings))
        {
            return false;
        }

        return StartDocsEnabled == defaultSettings.StartDocsEnabled &&
                HelpCenterEnabled == defaultSettings.HelpCenterEnabled &&
                FeedbackAndSupportEnabled == defaultSettings.FeedbackAndSupportEnabled &&
                FeedbackAndSupportUrl == defaultSettings.FeedbackAndSupportUrl &&
                UserForumEnabled == defaultSettings.UserForumEnabled &&
                UserForumUrl == defaultSettings.UserForumUrl &&
                VideoGuidesEnabled == defaultSettings.VideoGuidesEnabled &&
                VideoGuidesUrl == defaultSettings.VideoGuidesUrl &&
                SalesEmail == defaultSettings.SalesEmail &&
                BuyUrl == defaultSettings.BuyUrl &&
                LicenseAgreementsEnabled == defaultSettings.LicenseAgreementsEnabled &&
                LicenseAgreementsUrl == defaultSettings.LicenseAgreementsUrl;
    }

    public Guid ID
    {
        get { return new Guid("{0108422F-C05D-488E-B271-30C4032494DA}"); }
    }

    public ISettings GetDefault()
    {
        return new AdditionalWhiteLabelSettings
        {
            StartDocsEnabled = true,
            LicenseAgreementsEnabled = true,
            LicenseAgreementsUrl = DefaultLicenseAgreements
        };
    }

    public ISettings GetDefault(IConfiguration configuration)
    {
        var additionalWhiteLabelSettingsHelper = new AdditionalWhiteLabelSettingsHelper(configuration);
        return new AdditionalWhiteLabelSettings
        {
            StartDocsEnabled = true,
            HelpCenterEnabled = additionalWhiteLabelSettingsHelper.DefaultHelpCenterUrl != null,
            FeedbackAndSupportEnabled = additionalWhiteLabelSettingsHelper.DefaultFeedbackAndSupportUrl != null,
            FeedbackAndSupportUrl = additionalWhiteLabelSettingsHelper.DefaultFeedbackAndSupportUrl,
            UserForumEnabled = additionalWhiteLabelSettingsHelper.DefaultUserForumUrl != null,
            UserForumUrl = additionalWhiteLabelSettingsHelper.DefaultUserForumUrl,
            VideoGuidesEnabled = additionalWhiteLabelSettingsHelper.DefaultVideoGuidesUrl != null,
            VideoGuidesUrl = additionalWhiteLabelSettingsHelper.DefaultVideoGuidesUrl,
            SalesEmail = additionalWhiteLabelSettingsHelper.DefaultMailSalesEmail,
            BuyUrl = additionalWhiteLabelSettingsHelper.DefaultBuyUrl,
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

    public ISettings GetDefault(IServiceProvider serviceProvider)
    {
        return GetDefault(serviceProvider.GetService<IConfiguration>());
    }
}

[Singletone]
public class AdditionalWhiteLabelSettingsHelper
{
    private IConfiguration Configuration { get; }

    public AdditionalWhiteLabelSettingsHelper(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public string DefaultHelpCenterUrl
    {
        get
        {
            var url = Configuration["web:help-center"];
            return string.IsNullOrEmpty(url) ? null : url;
        }
    }

    public string DefaultFeedbackAndSupportUrl
    {
        get
        {
            var url = Configuration["web:support-feedback"];
            return string.IsNullOrEmpty(url) ? null : url;
        }
    }

    public string DefaultUserForumUrl
    {
        get
        {
            var url = Configuration["web:user-forum"];
            return string.IsNullOrEmpty(url) ? null : url;
        }
    }

    public string DefaultVideoGuidesUrl
    {
        get
        {
            var url = DefaultHelpCenterUrl;
            return string.IsNullOrEmpty(url) ? null : url + "/video.aspx";
        }
    }

    public string DefaultMailSalesEmail
    {
        get
        {
            var email = Configuration["web:payment:email"];
            return !string.IsNullOrEmpty(email) ? email : "sales@onlyoffice.com";
        }
    }

    public string DefaultBuyUrl
    {
        get
        {
            var site = Configuration["web:teamlab-site"];
            return !string.IsNullOrEmpty(site) ? site + "/post.ashx?type=buyenterprise" : "";
        }
    }
}
