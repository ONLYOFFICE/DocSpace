/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Globalization;

using ASC.Common;
using ASC.Core.Common.Settings;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Web.Core.WhiteLabel
{
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
            if (!(GetDefault(configuration) is AdditionalWhiteLabelSettings defaultSettings)) return false;

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
                return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ru" ? "https://help.onlyoffice.com/products/files/doceditor.aspx?fileid=4522572&doc=VzZXY2FQb29EdjZmTnhlZkFYZS9XYzFPK3JTaC9zcC9mNHEvTTZXSXNLUT0_IjQ1MjI1NzIi0" : "https://help.onlyoffice.com/products/files/doceditor.aspx?fileid=4485697&doc=R29zSHZNRi9LYnRTb3JDditmVGpXQThVVXhMTWdja0xwemlYZXpiaDBYdz0_IjQ0ODU2OTci0";
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
}
