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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

using ASC.Common;
using ASC.Common.Web;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;

using Microsoft.Extensions.Configuration;

namespace ASC.Web.Studio.Core
{
    [Singletone]
    public class SetupInfo
    {
        private static string web_autotest_secret_email;
        private static string[] web_display_mobapps_banner;
        private static string[] hideSettings;

        public string MetaImageURL { get; private set; }
        public string StatisticTrackURL { get; private set; }
        public string UserVoiceURL { get; private set; }
        public string MainLogoURL { get; private set; }
        public string MainLogoMailTmplURL { get; private set; }
        public List<CultureInfo> EnabledCultures { get; private set; }
        public List<CultureInfo> EnabledCulturesPersonal { get; set; }
        public List<KeyValuePair<string, CultureInfo>> PersonalCultures { get; private set; }
        public decimal ExchangeRateRuble { get; private set; }
        public long MaxImageUploadSize { get; private set; }

        /// <summary>
        /// Max possible file size for not chunked upload. Less or equal than 100 mb.
        /// </summary>
        public long MaxUploadSize(TenantExtra tenantExtra, TenantStatisticsProvider tenantStatisticsProvider)
        {
            return Math.Min(AvailableFileSize, MaxChunkedUploadSize(tenantExtra, tenantStatisticsProvider));
        }

        public long AvailableFileSize
        {
            get;
            private set;
        }

        public string TeamlabSiteRedirect { get; private set; }
        public long ChunkUploadSize { get; private set; }
        public bool ThirdPartyAuthEnabled { get; private set; }
        public bool ThirdPartyBannerEnabled { get; private set; }
        public string NoTenantRedirectURL { get; private set; }
        public string NotifyAddress { get; private set; }
        public string TipsAddress { get; private set; }
        public string UserForum { get; private set; }
        public string SupportFeedback { get; private set; }
        public string WebApiBaseUrl { get { return VirtualPathUtility.ToAbsolute(GetAppSettings("api.url", "~/api/2.0/")); } }
        public TimeSpan ValidEmailKeyInterval { get; private set; }
        public TimeSpan ValidAuthKeyInterval { get; private set; }
        public string SalesEmail { get; private set; }
        public static bool IsSecretEmail(string email)
        {
            email = Regex.Replace(email ?? "", "\\.*(?=\\S*(@gmail.com$))", "").ToLower();
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(web_autotest_secret_email))
                return false;

            var regex = new Regex(web_autotest_secret_email, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            return regex.IsMatch(email);
        }

        public static bool DisplayMobappBanner(string product)
        {
            return web_display_mobapps_banner.Contains(product, StringComparer.InvariantCultureIgnoreCase);
        }

        public string ShareTwitterUrl { get; private set; }
        public string ShareFacebookUrl { get; private set; }
        public string ControlPanelUrl { get; private set; }
        public string FontOpenSansUrl { get; private set; }
        public bool VoipEnabled { get; private set; }
        public string StartProductList { get; private set; }
        public string SsoSamlLoginUrl { get; private set; }
        public string DownloadForDesktopUrl { get; private set; }
        public string DownloadForIosDocuments { get; private set; }
        public string DownloadForIosProjects { get; private set; }
        public string DownloadForAndroidDocuments { get; private set; }
        public string SsoSamlLogoutUrl { get; private set; }
        public bool SmsTrial { get; private set; }
        public string TfaRegistration { get; private set; }
        public int TfaAppBackupCodeLength { get; private set; }
        public int TfaAppBackupCodeCount { get; private set; }
        public string TfaAppSender { get; private set; }
        public string RecaptchaPublicKey { get; private set; }
        public string RecaptchaPrivateKey { get; private set; }
        public string RecaptchaVerifyUrl { get; private set; }
        public int LoginThreshold { get; private set; }
        public string AmiMetaUrl { get; private set; }
        private IConfiguration Configuration { get; }

        public SetupInfo(IConfiguration configuration)
        {
            Configuration = configuration;
            MetaImageURL = GetAppSettings("web.meta-image-url", "https://download.onlyoffice.com/assets/fb/fb_icon_325x325.jpg");
            StatisticTrackURL = GetAppSettings("web.track-url", string.Empty);
            UserVoiceURL = GetAppSettings("web.uservoice", string.Empty);
            MainLogoURL = GetAppSettings("web.logo.main", string.Empty);
            MainLogoMailTmplURL = GetAppSettings("web.logo.mail.tmpl", string.Empty);
            DownloadForDesktopUrl = GetAppSettings("web.download.for.desktop.url", "https://www.onlyoffice.com/desktop.aspx");
            DownloadForIosDocuments = GetAppSettings("web.download.for.ios.doc", "https://itunes.apple.com/app/onlyoffice-documents/id944896972");
            DownloadForIosProjects = GetAppSettings("web.download.for.ios.proj", "https://itunes.apple.com/app/onlyoffice-projects/id1353395928?mt=8");
            DownloadForAndroidDocuments = GetAppSettings("web.download.for.android.doc", "https://play.google.com/store/apps/details?id=com.onlyoffice.documents");

            EnabledCultures = GetAppSettings("web:cultures", "en-US")
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => CultureInfo.GetCultureInfo(l.Trim()))
                .OrderBy(l => l.DisplayName)
                .ToList();

            EnabledCulturesPersonal = GetAppSettings("web:cultures:personal", GetAppSettings("web:cultures", "en-US"))
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => CultureInfo.GetCultureInfo(l.Trim()))
                .OrderBy(l => l.DisplayName)
                .ToList();

            PersonalCultures = GetPersonalCultures();

            ExchangeRateRuble = GetAppSettings("exchange-rate.ruble", 65);
            MaxImageUploadSize = GetAppSettings<long>("web:max-upload-size", 1024 * 1024);
            AvailableFileSize = GetAppSettings("web:available-file-size", 100L * 1024L * 1024L);
            AvailableFileSize = GetAppSettings("web.available-file-size", 100L * 1024L * 1024L);

            TeamlabSiteRedirect = GetAppSettings("web.teamlab-site", string.Empty);
            ChunkUploadSize = GetAppSettings("files:uploader:chunk-size", 10 * 1024 * 1024);
            ThirdPartyAuthEnabled = string.Equals(GetAppSettings("web.thirdparty-auth", "true"), "true");
            ThirdPartyBannerEnabled = string.Equals(GetAppSettings("web.thirdparty-banner", "false"), "true");
            NoTenantRedirectURL = GetAppSettings("web.notenant-url", "");

            NotifyAddress = GetAppSettings("web.promo-url", string.Empty);
            TipsAddress = GetAppSettings("web.promo-tips-url", string.Empty);
            UserForum = GetAppSettings("web.user-forum", string.Empty);
            SupportFeedback = GetAppSettings("web.support-feedback", string.Empty);

            ValidEmailKeyInterval = GetAppSettings("email.validinterval", TimeSpan.FromDays(7));
            ValidAuthKeyInterval = GetAppSettings("auth.validinterval", TimeSpan.FromHours(1));

            SalesEmail = GetAppSettings("web.payment.email", "sales@onlyoffice.com");
            web_autotest_secret_email = (configuration["web.autotest.secret-email"] ?? "").Trim();

            RecaptchaPublicKey = GetAppSettings("web.recaptcha.public-key", null);
            RecaptchaPrivateKey = GetAppSettings("web.recaptcha.private-key", "");
            RecaptchaVerifyUrl = GetAppSettings("web.recaptcha.verify-url", "https://www.recaptcha.net/recaptcha/api/siteverify");
            LoginThreshold = Convert.ToInt32(GetAppSettings("web.login.threshold", "0"));
            if (LoginThreshold < 1) LoginThreshold = 5;

            web_display_mobapps_banner = (configuration["web.display.mobapps.banner"] ?? "").Trim().Split(new char[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            ShareTwitterUrl = GetAppSettings("web.share.twitter", "https://twitter.com/intent/tweet?text={0}");
            ShareFacebookUrl = GetAppSettings("web.share.facebook", "");
            ControlPanelUrl = GetAppSettings("web:controlpanel:url", "");
            FontOpenSansUrl = GetAppSettings("web.font.opensans.url", "");
            VoipEnabled = GetAppSettings("voip.enabled", true);
            StartProductList = GetAppSettings("web.start.product.list", "");
            SsoSamlLoginUrl = GetAppSettings("web.sso.saml.login.url", "");
            SsoSamlLogoutUrl = GetAppSettings("web.sso.saml.logout.url", "");

            hideSettings = GetAppSettings("web.hide-settings", string.Empty).Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            SmsTrial = GetAppSettings("core.sms.trial", false);

            TfaRegistration = (GetAppSettings("core.tfa.registration", "") ?? "").Trim().ToLower();

            TfaAppBackupCodeLength = GetAppSettings("web.tfaapp.backup.length", 6);
            TfaAppBackupCodeCount = GetAppSettings("web.tfaapp.backup.count", 5);
            TfaAppSender = GetAppSettings("web.tfaapp.backup.title", "ONLYOFFICE");

            AmiMetaUrl = GetAppSettings("web:ami:meta", "");
        }


        //TODO
        public static bool IsVisibleSettings<TSettings>()
        {
            return IsVisibleSettings(typeof(TSettings).Name);
        }

        public static bool IsVisibleSettings(string settings)
        {
            return hideSettings == null || !hideSettings.Contains(settings, StringComparer.CurrentCultureIgnoreCase);
        }

        public long MaxChunkedUploadSize(TenantExtra tenantExtra, TenantStatisticsProvider tenantStatisticsProvider)
        {
            var diskQuota = tenantExtra.GetTenantQuota();
            if (diskQuota != null)
            {
                var usedSize = tenantStatisticsProvider.GetUsedSize();
                var freeSize = Math.Max(diskQuota.MaxTotalSize - usedSize, 0);
                return Math.Min(freeSize, diskQuota.MaxFileSize);
            }
            return ChunkUploadSize;
        }

        private string GetAppSettings(string key, string defaultValue)
        {
            var result = Configuration[key] ?? defaultValue;

            if (!string.IsNullOrEmpty(result))
                result = result.Trim();

            return result;

        }

        private T GetAppSettings<T>(string key, T defaultValue)
        {
            var configSetting = Configuration[key];
            if (!string.IsNullOrEmpty(configSetting))
            {
                configSetting = configSetting.Trim();
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null && converter.CanConvertFrom(typeof(string)))
                {
                    return (T)converter.ConvertFromString(configSetting);
                }
            }
            return defaultValue;
        }

        private List<KeyValuePair<string, CultureInfo>> GetPersonalCultures()
        {
            var result = new Dictionary<string, CultureInfo>();

            foreach (var culture in EnabledCulturesPersonal)
            {
                if (result.ContainsKey(culture.TwoLetterISOLanguageName))
                {
                    result.Add(culture.Name, culture);
                }
                else
                {
                    result.Add(culture.TwoLetterISOLanguageName, culture);
                }
            }

            return result.OrderBy(item => item.Value.DisplayName).ToList();
        }

        public KeyValuePair<string, CultureInfo> GetPersonalCulture(string lang)
        {
            foreach (var item in PersonalCultures)
            {
                if (string.Equals(item.Key, lang, StringComparison.InvariantCultureIgnoreCase))
                {
                    return item;
                }
            }

            var cultureInfo = EnabledCulturesPersonal.Find(c => string.Equals(c.Name, lang, StringComparison.InvariantCultureIgnoreCase));

            if (cultureInfo == null)
            {
                cultureInfo = EnabledCulturesPersonal.Find(c => string.Equals(c.TwoLetterISOLanguageName, lang, StringComparison.InvariantCultureIgnoreCase));
            }

            if (cultureInfo != null)
            {
                foreach (var item in PersonalCultures)
                {
                    if (item.Value == cultureInfo)
                    {
                        return item;
                    }
                }
            }

            return new KeyValuePair<string, CultureInfo>(lang, cultureInfo);
        }
    }
}