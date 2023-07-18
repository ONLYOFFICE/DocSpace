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

namespace ASC.Web.Api.Controllers.Settings;

public class SettingsController : BaseSettingsController
{
    private static readonly object locked = new object();
    private Tenant Tenant { get { return ApiContext.Tenant; } }

    private readonly MessageService _messageService;
    private readonly ConsumerFactory _consumerFactory;
    private readonly TimeZoneConverter _timeZoneConverter;
    private readonly CustomNamingPeople _customNamingPeople;
    private readonly ProviderManager _providerManager;
    private readonly FirstTimeTenantSettings _firstTimeTenantSettings;
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly TenantExtra _tenantExtra;
    private readonly AuthContext _authContext;
    private readonly PermissionContext _permissionContext;
    private readonly SettingsManager _settingsManager;
    private readonly WebItemManagerSecurity _webItemManagerSecurity;
    private readonly TenantInfoSettingsHelper _tenantInfoSettingsHelper;
    private readonly TenantUtil _tenantUtil;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly IConfiguration _configuration;
    private readonly SetupInfo _setupInfo;
    private readonly StatisticManager _statisticManager;
    private readonly UrlShortener _urlShortener;
    private readonly PasswordHasher _passwordHasher;
    private readonly ILogger _log;
    private readonly TelegramHelper _telegramHelper;
    private readonly DnsSettings _dnsSettings;
    private readonly AdditionalWhiteLabelSettingsHelperInit _additionalWhiteLabelSettingsHelper;
    private readonly CustomColorThemesSettingsHelper _customColorThemesSettingsHelper;
    private readonly QuotaUsageManager _quotaUsageManager;
    private readonly TenantDomainValidator _tenantDomainValidator;
    private readonly QuotaSyncOperation _quotaSyncOperation;

    public SettingsController(
        ILoggerProvider option,
        MessageService messageService,
        ApiContext apiContext,
        UserManager userManager,
        TenantManager tenantManager,
        TenantExtra tenantExtra,
        AuthContext authContext,
        PermissionContext permissionContext,
        SettingsManager settingsManager,
        WebItemManager webItemManager,
        WebItemManagerSecurity webItemManagerSecurity,
        TenantInfoSettingsHelper tenantInfoSettingsHelper,
        TenantUtil tenantUtil,
        CoreBaseSettings coreBaseSettings,
        CommonLinkUtility commonLinkUtility,
        IConfiguration configuration,
        SetupInfo setupInfo,
        StatisticManager statisticManager,
        ConsumerFactory consumerFactory,
        TimeZoneConverter timeZoneConverter,
        CustomNamingPeople customNamingPeople,
        IMemoryCache memoryCache,
        ProviderManager providerManager,
        FirstTimeTenantSettings firstTimeTenantSettings,
        TelegramHelper telegramHelper,
        UrlShortener urlShortener,
        PasswordHasher passwordHasher,
        IHttpContextAccessor httpContextAccessor,
        DnsSettings dnsSettings,
        AdditionalWhiteLabelSettingsHelperInit additionalWhiteLabelSettingsHelper,
        CustomColorThemesSettingsHelper customColorThemesSettingsHelper,
        QuotaSyncOperation quotaSyncOperation,
        QuotaUsageManager quotaUsageManager,
        TenantDomainValidator tenantDomainValidator
        ) : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _log = option.CreateLogger("ASC.Api");
        _consumerFactory = consumerFactory;
        _timeZoneConverter = timeZoneConverter;
        _customNamingPeople = customNamingPeople;
        _providerManager = providerManager;
        _firstTimeTenantSettings = firstTimeTenantSettings;
        _messageService = messageService;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _tenantExtra = tenantExtra;
        _authContext = authContext;
        _permissionContext = permissionContext;
        _settingsManager = settingsManager;
        _webItemManagerSecurity = webItemManagerSecurity;
        _tenantInfoSettingsHelper = tenantInfoSettingsHelper;
        _tenantUtil = tenantUtil;
        _coreBaseSettings = coreBaseSettings;
        _commonLinkUtility = commonLinkUtility;
        _configuration = configuration;
        _setupInfo = setupInfo;
        _statisticManager = statisticManager;
        _passwordHasher = passwordHasher;
        _urlShortener = urlShortener;
        _telegramHelper = telegramHelper;
        _dnsSettings = dnsSettings;
        _additionalWhiteLabelSettingsHelper = additionalWhiteLabelSettingsHelper;
        _quotaSyncOperation = quotaSyncOperation;
        _customColorThemesSettingsHelper = customColorThemesSettingsHelper;
        _quotaUsageManager = quotaUsageManager;
        _tenantDomainValidator = tenantDomainValidator;
    }

    [HttpGet("")]
    [AllowNotPayment, AllowSuspended, AllowAnonymous]
    public SettingsDto GetSettings(bool? withpassword)
    {
        var studioAdminMessageSettings = _settingsManager.Load<StudioAdminMessageSettings>();
        var tenantCookieSettings = _settingsManager.Load<TenantCookieSettings>();

        var settings = new SettingsDto
        {
            Culture = Tenant.GetCulture().ToString(),
            GreetingSettings = Tenant.Name == "" ? Resource.PortalName : Tenant.Name,
            Personal = _coreBaseSettings.Personal,
            DocSpace = !_coreBaseSettings.DisableDocSpace,
            Standalone = _coreBaseSettings.Standalone,
            BaseDomain = _coreBaseSettings.Basedomain,
            Version = _configuration["version:number"] ?? "",
            TenantStatus = _tenantManager.GetCurrentTenant().Status,
            TenantAlias = Tenant.Alias,
            EnableAdmMess = studioAdminMessageSettings.Enable || _tenantExtra.IsNotPaid(),
            LegalTerms = _setupInfo.LegalTerms,
            CookieSettingsEnabled = tenantCookieSettings.Enabled
        };

        if (_authContext.IsAuthenticated)
        {
            settings.TrustedDomains = Tenant.TrustedDomains;
            settings.TrustedDomainsType = Tenant.TrustedDomainsType;
            var timeZone = Tenant.TimeZone;
            settings.Timezone = timeZone;
            settings.UtcOffset = _timeZoneConverter.GetTimeZone(timeZone).GetUtcOffset(DateTime.UtcNow);
            settings.UtcHoursOffset = settings.UtcOffset.TotalHours;
            settings.OwnerId = Tenant.OwnerId;
            settings.NameSchemaId = _customNamingPeople.Current.Id;
            settings.SocketUrl = _configuration["web:hub:url"] ?? "";
            settings.DomainValidator = _tenantDomainValidator;
            settings.ZendeskKey = _setupInfo.ZendeskKey;
            settings.BookTrainingEmail = _setupInfo.BookTrainingEmail;
            settings.DocumentationEmail = _setupInfo.DocumentationEmail;

            settings.Firebase = new FirebaseDto
            {
                ApiKey = _configuration["firebase:apiKey"] ?? "",
                AuthDomain = _configuration["firebase:authDomain"] ?? "",
                ProjectId = _configuration["firebase:projectId"] ?? "",
                StorageBucket = _configuration["firebase:storageBucket"] ?? "",
                MessagingSenderId = _configuration["firebase:messagingSenderId"] ?? "",
                AppId = _configuration["firebase:appId"] ?? "",
                MeasurementId = _configuration["firebase:measurementId"] ?? "",
                DatabaseURL = _configuration["firebase:databaseURL"] ?? ""
            };

            settings.HelpLink = _commonLinkUtility.GetHelpLink(_settingsManager, _additionalWhiteLabelSettingsHelper, true);
            settings.ApiDocsLink = _configuration["web:api-docs"];

            bool debugInfo;
            if (bool.TryParse(_configuration["debug-info:enabled"], out debugInfo))
            {
                settings.DebugInfo = debugInfo;
            }

            settings.Plugins = new PluginsDto();

            bool pluginsEnabled;
            if (bool.TryParse(_configuration["plugins:enabled"], out pluginsEnabled))
            {
                settings.Plugins.Enabled = pluginsEnabled;
            }

            settings.Plugins.Allow = _configuration.GetSection("plugins:allow").Get<List<string>>() ?? new List<string>();
        }
        else
        {
            if (!_settingsManager.Load<WizardSettings>().Completed)
            {
                settings.WizardToken = _commonLinkUtility.GetToken(Tenant.Id, "", ConfirmType.Wizard, userId: Tenant.OwnerId);
            }

            settings.EnabledJoin =
                (Tenant.TrustedDomainsType == TenantTrustedDomainsType.Custom &&
                Tenant.TrustedDomains.Count > 0) ||
                Tenant.TrustedDomainsType == TenantTrustedDomainsType.All;

            if (settings.EnabledJoin.GetValueOrDefault(false))
            {
                settings.TrustedDomainsType = Tenant.TrustedDomainsType;
                settings.TrustedDomains = Tenant.TrustedDomains;
            }

            settings.ThirdpartyEnable = _setupInfo.ThirdPartyAuthEnabled && _providerManager.IsNotEmpty;

            settings.RecaptchaPublicKey = _setupInfo.RecaptchaPublicKey;
        }

        if (!_authContext.IsAuthenticated || (withpassword.HasValue && withpassword.Value))
        {
            settings.PasswordHash = _passwordHasher;
        }

        return settings;
    }

    [HttpPost("maildomainsettings")]
    public object SaveMailDomainSettings(MailDomainSettingsRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (inDto.Type == TenantTrustedDomainsType.Custom)
        {
            Tenant.TrustedDomainsRaw = "";
            Tenant.TrustedDomains.Clear();
            foreach (var d in inDto.Domains.Select(domain => (domain ?? "").Trim().ToLower()))
            {
                if (!(!string.IsNullOrEmpty(d) && new Regex("^[a-z0-9]([a-z0-9-.]){1,98}[a-z0-9]$").IsMatch(d)))
                {
                    return Resource.ErrorNotCorrectTrustedDomain;
                }

                Tenant.TrustedDomains.Add(d);
            }

            if (Tenant.TrustedDomains.Count == 0)
            {
                inDto.Type = TenantTrustedDomainsType.None;
            }
        }

        Tenant.TrustedDomainsType = inDto.Type;

        _settingsManager.Save(new StudioTrustedDomainSettings { InviteAsUsers = inDto.InviteAsUsers });

        _tenantManager.SaveTenant(Tenant);

        _messageService.Send(MessageAction.TrustedMailDomainSettingsUpdated);

        return Resource.SuccessfullySaveSettingsMessage;
    }

    [HttpGet("quota")]
    public async Task<QuotaUsageDto> GetQuotaUsed()
    {
        return await _quotaUsageManager.Get();
    }

    [HttpPost("userquotasettings")]
    public object SaveUserQuotaSettings(UserQuotaSettingsRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        _settingsManager.Save(new TenantUserQuotaSettings { EnableUserQuota = inDto.EnableUserQuota, DefaultUserQuota = inDto.DefaultUserQuota });

        return Resource.SuccessfullySaveSettingsMessage;
    }

    [AllowAnonymous]
    [AllowNotPayment]
    [HttpGet("cultures")]
    public IEnumerable<object> GetSupportedCultures()
    {
        return _setupInfo.EnabledCultures.Select(r => r.Name).OrderBy(s => s).ToArray();
    }

    [Authorize(AuthenticationSchemes = "confirm", Roles = "Wizard,Administrators")]
    [HttpGet("timezones")]
    [AllowNotPayment]
    public List<TimezonesRequestsDto> GetTimeZones()
    {
        ApiContext.AuthByClaim();
        var timeZones = TimeZoneInfo.GetSystemTimeZones().ToList();

        if (timeZones.All(tz => tz.Id != "UTC"))
        {
            timeZones.Add(TimeZoneInfo.Utc);
        }

        var listOfTimezones = new List<TimezonesRequestsDto>();

        foreach (var tz in timeZones.OrderBy(z => z.BaseUtcOffset))
        {
            listOfTimezones.Add(new TimezonesRequestsDto
            {
                Id = tz.Id,
                DisplayName = _timeZoneConverter.GetTimeZoneDisplayName(tz)
            });
        }

        return listOfTimezones;
    }

    [Authorize(AuthenticationSchemes = "confirm", Roles = "Wizard")]
    [HttpGet("machine")]
    [AllowNotPayment]
    public object GetMachineName()
    {
        return Dns.GetHostName().ToLowerInvariant();
    }

    [HttpPut("dns")]
    public object SaveDnsSettings(DnsSettingsRequestsDto model)
    {
        return _dnsSettings.SaveDnsSettings(model.DnsName, model.Enable);
    }

    [HttpGet("recalculatequota")]
    public void RecalculateQuota()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        _quotaSyncOperation.RecalculateQuota(_tenantManager.GetCurrentTenant());
    }

    [HttpGet("checkrecalculatequota")]
    public bool CheckRecalculateQuota()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        return _quotaSyncOperation.CheckRecalculateQuota(_tenantManager.GetCurrentTenant());
    }

    [HttpGet("logo")]
    public object GetLogo()
    {
        return _tenantInfoSettingsHelper.GetAbsoluteCompanyLogoPath(_settingsManager.Load<TenantInfoSettings>());
    }

    [AllowNotPayment]
    [HttpPut("wizard/complete")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "Wizard")]
    public async Task<WizardSettings> CompleteWizard(WizardRequestsDto inDto)
    {
        ApiContext.AuthByClaim();

        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        return await _firstTimeTenantSettings.SaveData(inDto);
    }

    ///<visible>false</visible>
    [HttpPut("welcome/close")]
    public void CloseWelcomePopup()
    {
        var currentUser = _userManager.GetUsers(_authContext.CurrentAccount.ID);

        var collaboratorPopupSettings = _settingsManager.LoadForCurrentUser<CollaboratorSettings>();

        if (!(_userManager.IsUser(currentUser) && collaboratorPopupSettings.FirstVisit && !_userManager.IsOutsider(currentUser)))
        {
            throw new NotSupportedException("Not available.");
        }

        collaboratorPopupSettings.FirstVisit = false;
        _settingsManager.SaveForCurrentUser(collaboratorPopupSettings);
    }

    [AllowAnonymous, AllowNotPayment, AllowSuspended]
    [HttpGet("colortheme")]
    public CustomColorThemesSettingsDto GetColorTheme()
    {
        return new CustomColorThemesSettingsDto(_settingsManager.Load<CustomColorThemesSettings>(), _customColorThemesSettingsHelper.Limit);
    }

    [HttpPut("colortheme")]
    public CustomColorThemesSettingsDto SaveColorTheme(CustomColorThemesSettingsRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        var settings = _settingsManager.Load<CustomColorThemesSettings>();

        if (inDto.Theme != null)
        {
            lock (locked)
            {
                var theme = inDto.Theme;

                if (CustomColorThemesSettingsItem.Default.Any(r => r.Id == theme.Id))
                {
                    theme.Id = 0;
                }

                var settingItem = settings.Themes.SingleOrDefault(r => r.Id == theme.Id);
                if (settingItem != null)
                {
                    if (theme.Main != null)
                    {
                        settingItem.Main = new CustomColorThemesSettingsColorItem
                        {
                            Accent = theme.Main.Accent,
                            Buttons = theme.Main.Buttons
                        };
                    }
                    if (theme.Text != null)
                    {
                        settingItem.Text = new CustomColorThemesSettingsColorItem
                        {
                            Accent = theme.Text.Accent,
                            Buttons = theme.Text.Buttons
                        };
                    }
                }
                else
                {
                    if (_customColorThemesSettingsHelper.Limit == 0 || settings.Themes.Count < _customColorThemesSettingsHelper.Limit)
                    {
                        if (theme.Id == 0)
                        {
                            theme.Id = settings.Themes.Max(r => r.Id) + 1;
                        }

                        theme.Name = "";
                        settings.Themes = settings.Themes.Append(theme).ToList();
                    }
                }


                _settingsManager.Save(settings);
            }
        }

        if (inDto.Selected.HasValue && settings.Themes.Any(r => r.Id == inDto.Selected.Value))
        {
            settings.Selected = inDto.Selected.Value;
            _settingsManager.Save(settings);
            _messageService.Send(MessageAction.ColorThemeChanged);
        }

        return new CustomColorThemesSettingsDto(settings, _customColorThemesSettingsHelper.Limit);
    }

    [HttpDelete("colortheme")]
    public CustomColorThemesSettingsDto DeleteColorTheme(int id)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var settings = _settingsManager.Load<CustomColorThemesSettings>();

        if (CustomColorThemesSettingsItem.Default.Any(r => r.Id == id))
        {
            return new CustomColorThemesSettingsDto(settings, _customColorThemesSettingsHelper.Limit);
        }

        settings.Themes = settings.Themes.Where(r => r.Id != id).ToList();

        if (settings.Selected == id)
        {
            settings.Selected = settings.Themes.Min(r => r.Id);
            _messageService.Send(MessageAction.ColorThemeChanged);
        }

        _settingsManager.Save(settings);

        return new CustomColorThemesSettingsDto(settings, _customColorThemesSettingsHelper.Limit);
    }

    [HttpPut("closeadminhelper")]
    public void CloseAdminHelper()
    {
        if (!_userManager.IsDocSpaceAdmin(_authContext.CurrentAccount.ID) || _coreBaseSettings.CustomMode || !_coreBaseSettings.Standalone)
        {
            throw new NotSupportedException("Not available.");
        }

        var adminHelperSettings = _settingsManager.LoadForCurrentUser<AdminHelperSettings>();
        adminHelperSettings.Viewed = true;
        _settingsManager.SaveForCurrentUser(adminHelperSettings);
    }

    ///<visible>false</visible>
    [HttpPut("timeandlanguage")]
    public object TimaAndLanguage(SettingsRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var culture = CultureInfo.GetCultureInfo(inDto.Lng);

        var changelng = false;
        if (_setupInfo.EnabledCultures.Find(c => string.Equals(c.Name, culture.Name, StringComparison.InvariantCultureIgnoreCase)) != null)
        {
            if (!string.Equals(Tenant.Language, culture.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                Tenant.Language = culture.Name;
                changelng = true;
            }
        }

        var oldTimeZone = Tenant.TimeZone;
        var timeZones = TimeZoneInfo.GetSystemTimeZones().ToList();
        if (timeZones.All(tz => tz.Id != "UTC"))
        {
            timeZones.Add(TimeZoneInfo.Utc);
        }
        Tenant.TimeZone = timeZones.FirstOrDefault(tz => tz.Id == inDto.TimeZoneID)?.Id ?? TimeZoneInfo.Utc.Id;

        _tenantManager.SaveTenant(Tenant);

        if (!Tenant.TimeZone.Equals(oldTimeZone) || changelng)
        {
            if (!Tenant.TimeZone.Equals(oldTimeZone))
            {
                _messageService.Send(MessageAction.TimeZoneSettingsUpdated);
            }
            if (changelng)
            {
                _messageService.Send(MessageAction.LanguageSettingsUpdated);
            }
        }

        return Resource.SuccessfullySaveSettingsMessage;
    }

    ///<visible>false</visible>
    [HttpPut("defaultpage")]
    public object SaveDefaultPageSetting(SettingsRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        _settingsManager.Save(new StudioDefaultPageSettings { DefaultProductID = inDto.DefaultProductID });

        _messageService.Send(MessageAction.DefaultStartPageSettingsUpdated);

        return Resource.SuccessfullySaveSettingsMessage;
    }

    [HttpPut("emailactivation")]
    public EmailActivationSettings UpdateEmailActivationSettings(EmailActivationSettings settings)
    {
        _settingsManager.SaveForCurrentUser(settings);
        return settings;
    }

    [HttpGet("statistics/spaceusage/{id}")]
    public Task<List<UsageSpaceStatItemDto>> GetSpaceUsageStatistics(Guid id)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var webitem = _webItemManagerSecurity.GetItems(WebZoneType.All, ItemAvailableState.All)
                                   .FirstOrDefault(item =>
                                                   item != null &&
                                                   item.ID == id &&
                                                   item.Context != null &&
                                                   item.Context.SpaceUsageStatManager != null);

        if (webitem == null)
        {
            return Task.FromResult(new List<UsageSpaceStatItemDto>());
        }

        return InternalGetSpaceUsageStatistics(webitem);
    }

    private async Task<List<UsageSpaceStatItemDto>> InternalGetSpaceUsageStatistics(IWebItem webitem)
    {
        var statData = await webitem.Context.SpaceUsageStatManager.GetStatDataAsync();

        return statData.ConvertAll(it => new UsageSpaceStatItemDto
        {
            Name = it.Name.HtmlEncode(),
            Icon = it.ImgUrl,
            Disabled = it.Disabled,
            Size = FileSizeComment.FilesSizeToString(it.SpaceUsage),
            Url = it.Url
        });
    }

    [HttpGet("statistics/visit")]
    public List<ChartPointDto> GetVisitStatistics(ApiDateTime fromDate, ApiDateTime toDate)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var from = _tenantUtil.DateTimeFromUtc(fromDate);
        var to = _tenantUtil.DateTimeFromUtc(toDate);

        var points = new List<ChartPointDto>();

        if (from.CompareTo(to) >= 0)
        {
            return points;
        }

        for (var d = new DateTime(from.Ticks); d.Date.CompareTo(to.Date) <= 0; d = d.AddDays(1))
        {
            points.Add(new ChartPointDto
            {
                DisplayDate = d.Date.ToShortDateString(),
                Date = d.Date,
                Hosts = 0,
                Hits = 0
            });
        }

        var hits = _statisticManager.GetHitsByPeriod(Tenant.Id, from, to);
        var hosts = _statisticManager.GetHostsByPeriod(Tenant.Id, from, to);

        if (hits.Count == 0 || hosts.Count == 0)
        {
            return points;
        }

        hits.Sort((x, y) => x.VisitDate.CompareTo(y.VisitDate));
        hosts.Sort((x, y) => x.VisitDate.CompareTo(y.VisitDate));

        for (int i = 0, n = points.Count, hitsNum = 0, hostsNum = 0; i < n; i++)
        {
            while (hitsNum < hits.Count && points[i].Date.CompareTo(hits[hitsNum].VisitDate.Date) == 0)
            {
                points[i].Hits += hits[hitsNum].VisitCount;
                hitsNum++;
            }
            while (hostsNum < hosts.Count && points[i].Date.CompareTo(hosts[hostsNum].VisitDate.Date) == 0)
            {
                points[i].Hosts++;
                hostsNum++;
            }
        }

        return points;
    }

    [HttpGet("socket")]
    public object GetSocketSettings()
    {
        var hubUrl = _configuration["web:hub"] ?? string.Empty;
        if (hubUrl.Length != 0)
        {
            if (!hubUrl.EndsWith('/'))
            {
                hubUrl += "/";
            }
        }

        return new { Url = hubUrl };
    }

    /*///<visible>false</visible>
    [HttpGet("controlpanel")]
    public TenantControlPanelSettings GetTenantControlPanelSettings()
    {
        return _settingsManager.Load<TenantControlPanelSettings>();
    }*/

    [HttpGet("authservice")]
    public IEnumerable<AuthServiceRequestsDto> GetAuthServices()
    {
        return _consumerFactory.GetAll<Consumer>()
            .Where(consumer => consumer.ManagedKeys.Any())
            .OrderBy(services => services.Order)
            .Select(r => new AuthServiceRequestsDto(r))
            .ToList();
    }

    [HttpPost("authservice")]
    public bool SaveAuthKeys(AuthServiceRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var saveAvailable = _coreBaseSettings.Standalone || _tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().Id).ThirdParty;
        if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.ThirdPartyAuthorization))
            || !saveAvailable)
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "ThirdPartyAuthorization");
        }

        var changed = false;
        var consumer = _consumerFactory.GetByKey<Consumer>(inDto.Name);

        var validateKeyProvider = consumer as IValidateKeysProvider;

        if (validateKeyProvider != null)
        {
            try
            {
                if (validateKeyProvider is BitlyLoginProvider bitly)
                {
                    _urlShortener.Instance = null;
                }
            }
            catch (Exception e)
            {
                _log.ErrorSaveAuthKeys(e);
            }
        }

        if (inDto.Props.All(r => string.IsNullOrEmpty(r.Value)))
        {
            consumer.Clear();
            changed = true;
        }
        else
        {
            foreach (var authKey in inDto.Props.Where(authKey => consumer[authKey.Name] != authKey.Value))
            {
                consumer[authKey.Name] = authKey.Value;
                changed = true;
            }
        }

        //TODO: Consumer implementation required (Bug 50606)
        var allPropsIsEmpty = consumer.GetType() == typeof(SmscProvider)
            ? consumer.ManagedKeys.All(key => string.IsNullOrEmpty(consumer[key]))
            : consumer.All(r => string.IsNullOrEmpty(r.Value));

        if (validateKeyProvider != null && !validateKeyProvider.ValidateKeys() && !allPropsIsEmpty)
        {
            consumer.Clear();
            throw new ArgumentException(Resource.ErrorBadKeys);
        }

        if (changed)
        {
            _messageService.Send(MessageAction.AuthorizationKeysSetting);
        }

        return changed;
    }

    [AllowNotPayment]
    [HttpGet("payment")]
    public object PaymentSettings()
    {
        var settings = _settingsManager.LoadForDefaultTenant<AdditionalWhiteLabelSettings>();
        var currentQuota = _tenantManager.GetCurrentTenantQuota();
        var currentTariff = _tenantExtra.GetCurrentTariff();

        if (!int.TryParse(_configuration["core:payment:max-quantity"], out var maxQuotaQuantity))
        {
            maxQuotaQuantity = 999;
        }

        return
            new
            {
                settings.SalesEmail,
                settings.FeedbackAndSupportUrl,
                settings.BuyUrl,
                _coreBaseSettings.Standalone,
                currentLicense = new
                {
                    currentQuota.Trial,
                    currentTariff.DueDate.Date
                },
                max = maxQuotaQuantity
            };
    }

    /// <visible>false</visible>
    /// <summary>
    /// Gets a link that will connect TelegramBot to your account
    /// </summary>
    /// <returns>url</returns>
    /// 
    [HttpGet("telegramlink")]
    public object TelegramLink()
    {
        var currentLink = _telegramHelper.CurrentRegistrationLink(_authContext.CurrentAccount.ID, Tenant.Id);

        if (string.IsNullOrEmpty(currentLink))
        {
            var url = _telegramHelper.RegisterUser(_authContext.CurrentAccount.ID, Tenant.Id);
            return url;
        }
        else
        {
            return currentLink;
        }
    }

    /// <summary>
    /// Checks if user has connected TelegramBot
    /// </summary>
    /// <returns>0 - not connected, 1 - connected, 2 - awaiting confirmation</returns>
    [HttpGet("telegramisconnected")]
    public object TelegramIsConnected()
    {
        return (int)_telegramHelper.UserIsConnected(_authContext.CurrentAccount.ID, Tenant.Id);
    }

    /// <summary>
    /// Unlinks TelegramBot from your account
    /// </summary>
    [HttpDelete("telegramdisconnect")]
    public void TelegramDisconnect()
    {
        _telegramHelper.Disconnect(_authContext.CurrentAccount.ID, Tenant.Id);
    }
}