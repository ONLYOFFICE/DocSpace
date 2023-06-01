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

namespace ASC.Web.Studio.Core;

[Singletone]
public class SetupInfo
{
    private static string _webAutotestSecretEmail;
    private static string[] _webDisplayMobappsBanner;
    private static string[] _hideSettings;

    public string MetaImageURL { get; private set; }
    public string StatisticTrackURL { get; private set; }
    public string DemoOrder { get; private set; }
    public string RequestTraining { get; private set; }
    public string ZendeskKey { get; private set; }
    public string BookTrainingEmail { get; private set; }
    public string DocumentationEmail { get; private set; }
    public string UserVoiceURL { get; private set; }
    public string MainLogoURL { get; private set; }
    public string MainLogoMailTmplURL { get; private set; }
    public List<CultureInfo> EnabledCultures { get; private set; }
    private List<CultureInfo> EnabledCulturesPersonal { get; set; }
    public List<KeyValuePair<string, CultureInfo>> PersonalCultures { get; private set; }
    public long MaxImageUploadSize { get; private set; }

    /// <summary>
    /// Max possible file size for not chunked upload. Less or equal than 100 mb.
    /// </summary>
    public async Task<long> MaxUploadSize(TenantManager tenantManager, MaxTotalSizeStatistic maxTotalSizeStatistic)
    {
        return Math.Min(AvailableFileSize, await MaxChunkedUploadSize(tenantManager, maxTotalSizeStatistic));
    }

    public long AvailableFileSize
    {
        get;
        private set;
    }

    public string TeamlabSiteRedirect { get; private set; }
    public long ChunkUploadSize { get; set; }
    public long ProviderMaxUploadSize { get; private set; }
    public bool ThirdPartyAuthEnabled { get; private set; }
    public string LegalTerms { get; private set; }
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
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(_webAutotestSecretEmail))
        {
            return false;
        }

        var regex = new Regex(_webAutotestSecretEmail, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
        return regex.IsMatch(email);
    }

    public static bool DisplayMobappBanner(string product)
    {
        return _webDisplayMobappsBanner.Contains(product, StringComparer.InvariantCultureIgnoreCase);
    }

    public string ShareTwitterUrl { get; private set; }
    public string ShareFacebookUrl { get; private set; }
    public string ControlPanelUrl { get; private set; }
    public string FontOpenSansUrl { get; private set; }
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
    public string AmiMetaUrl { get; private set; }
    private readonly IConfiguration _configuration;

    public SetupInfo(IConfiguration configuration)
    {
        _configuration = configuration;
        MetaImageURL = GetAppSettings("web.meta-image-url", "https://download.onlyoffice.com/assets/fb/fb_icon_325x325.jpg");
        StatisticTrackURL = GetAppSettings("web.track-url", string.Empty);
        UserVoiceURL = GetAppSettings("web.uservoice", string.Empty);
        DemoOrder = GetAppSettings("web.demo-order", string.Empty);
        ZendeskKey = GetAppSettings("web:zendesk-key", string.Empty);
        BookTrainingEmail = GetAppSettings("web:book-training-email", string.Empty);
        DocumentationEmail = GetAppSettings("web:documentation-email", string.Empty);
        RequestTraining = GetAppSettings("web.request-training", string.Empty);
        MainLogoURL = GetAppSettings("web.logo.main", string.Empty);
        MainLogoMailTmplURL = GetAppSettings("web.logo.mail.tmpl", string.Empty);
        DownloadForDesktopUrl = GetAppSettings("web.download.for.desktop.url", "https://www.onlyoffice.com/desktop.aspx");
        DownloadForIosDocuments = GetAppSettings("web.download.for.ios.doc", "https://itunes.apple.com/app/onlyoffice-documents/id944896972");
        DownloadForIosProjects = GetAppSettings("web.download.for.ios.proj", "https://itunes.apple.com/app/onlyoffice-projects/id1353395928?mt=8");
        DownloadForAndroidDocuments = GetAppSettings("web.download.for.android.doc", "https://play.google.com/store/apps/details?id=com.onlyoffice.documents");

        EnabledCultures = GetAppSettings("web:cultures", "en-US")
            .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Distinct()
            .Select(l => CultureInfo.GetCultureInfo(l.Trim()))
            .OrderBy(l => l.DisplayName)
            .ToList();

        EnabledCulturesPersonal = GetAppSettings("web:cultures:personal", GetAppSettings("web:cultures", "en-US"))
            .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Distinct()
            .Select(l => CultureInfo.GetCultureInfo(l.Trim()))
            .ToList();

        PersonalCultures = GetPersonalCultures();
        MaxImageUploadSize = GetAppSettings<long>("web:max-upload-size", 1024 * 1024);
        AvailableFileSize = GetAppSettings("web:available-file-size", 100L * 1024L * 1024L);
        AvailableFileSize = GetAppSettings("web.available-file-size", 100L * 1024L * 1024L);

        TeamlabSiteRedirect = GetAppSettings("web:teamlab-site", string.Empty);
        ChunkUploadSize = GetAppSettings("files:uploader:chunk-size", 10 * 1024 * 1024);
        ProviderMaxUploadSize = GetAppSettings("files:provider:max-upload-size", 1024L * 1024L * 1024L);
        ThirdPartyAuthEnabled = string.Equals(GetAppSettings("web:thirdparty-auth", "true"), "true");
        NoTenantRedirectURL = GetAppSettings("web.notenant-url", "");
        LegalTerms = GetAppSettings("web:legalterms", "");

        NotifyAddress = GetAppSettings("web.promo-url", string.Empty);
        TipsAddress = GetAppSettings("web.promo-tips-url", string.Empty);
        UserForum = GetAppSettings("web.user-forum", string.Empty);
        SupportFeedback = GetAppSettings("web.support-feedback", string.Empty);

        ValidEmailKeyInterval = GetAppSettings("email.validinterval", TimeSpan.FromDays(7));
        ValidAuthKeyInterval = GetAppSettings("auth.validinterval", TimeSpan.FromHours(1));

        SalesEmail = GetAppSettings("web.payment.email", "sales@onlyoffice.com");
        _webAutotestSecretEmail = (configuration["web:autotest:secret-email"] ?? "").Trim();

        RecaptchaPublicKey = GetAppSettings("web.recaptcha.public-key", null);
        RecaptchaPrivateKey = GetAppSettings("web.recaptcha.private-key", "");
        RecaptchaVerifyUrl = GetAppSettings("web.recaptcha.verify-url", "https://www.recaptcha.net/recaptcha/api/siteverify");

        _webDisplayMobappsBanner = (configuration["web.display.mobapps.banner"] ?? "").Trim().Split(new char[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        ShareTwitterUrl = GetAppSettings("web.share.twitter", "https://twitter.com/intent/tweet?text={0}");
        ShareFacebookUrl = GetAppSettings("web.share.facebook", "");
        ControlPanelUrl = GetAppSettings("web:controlpanel:url", "");
        FontOpenSansUrl = GetAppSettings("web.font.opensans.url", "");
        StartProductList = GetAppSettings("web.start.product.list", "");
        SsoSamlLoginUrl = GetAppSettings("web.sso.saml.login.url", "");
        SsoSamlLogoutUrl = GetAppSettings("web.sso.saml.logout.url", "");

        _hideSettings = GetAppSettings("web.hide-settings", string.Empty).Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);

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
        return _hideSettings == null || !_hideSettings.Contains(settings, StringComparer.CurrentCultureIgnoreCase);
    }

    public async Task<long> MaxChunkedUploadSize(TenantManager tenantManager, MaxTotalSizeStatistic maxTotalSizeStatistic)
    {
        var diskQuota = await tenantManager.GetCurrentTenantQuotaAsync();
        if (diskQuota != null)
        {
            var usedSize = await maxTotalSizeStatistic.GetValueAsync();
            var freeSize = Math.Max(diskQuota.MaxTotalSize - usedSize, 0);
            return Math.Min(freeSize, diskQuota.MaxFileSize);
        }
        return ChunkUploadSize;
    }

    private string GetAppSettings(string key, string defaultValue)
    {
        var result = _configuration[key] ?? defaultValue;

        if (!string.IsNullOrEmpty(result))
        {
            result = result.Trim();
        }

        return result;

    }

    private T GetAppSettings<T>(string key, T defaultValue)
    {
        var configSetting = _configuration[key];
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
