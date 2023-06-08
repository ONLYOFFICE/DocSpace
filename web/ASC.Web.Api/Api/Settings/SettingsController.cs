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
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
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
    public async Task<SettingsDto> GetSettingsAsync(bool? withpassword)
    {
        var studioAdminMessageSettings = await _settingsManager.LoadAsync<StudioAdminMessageSettings>();

        var settings = new SettingsDto
        {
            Culture = Tenant.GetCulture().ToString(),
            GreetingSettings = Tenant.Name == "" ? Resource.PortalName : Tenant.Name,
            Personal = _coreBaseSettings.Personal,
            DocSpace = !_coreBaseSettings.DisableDocSpace,
            Standalone = _coreBaseSettings.Standalone,
            BaseDomain = _coreBaseSettings.Basedomain,
            Version = _configuration["version:number"] ?? "",
            TenantStatus = (await _tenantManager.GetCurrentTenantAsync()).Status,
            TenantAlias = Tenant.Alias,
            EnableAdmMess = studioAdminMessageSettings.Enable || await _tenantExtra.IsNotPaidAsync(),
            LegalTerms = _setupInfo.LegalTerms
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
                MeasurementId = _configuration["firebase:measurementId"] ?? ""
            };

            settings.HelpLink = await _commonLinkUtility.GetHelpLinkAsync(_settingsManager, _additionalWhiteLabelSettingsHelper, true);

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
            if (!(await _settingsManager.LoadAsync<WizardSettings>()).Completed)
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
    public async Task<object> SaveMailDomainSettingsAsync(MailDomainSettingsRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

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

        await _settingsManager.SaveAsync(new StudioTrustedDomainSettings { InviteAsUsers = inDto.InviteAsUsers });

        await _tenantManager.SaveTenantAsync(Tenant);

        await _messageService.SendAsync(MessageAction.TrustedMailDomainSettingsUpdated);

        return Resource.SuccessfullySaveSettingsMessage;
    }

    [HttpGet("quota")]
    public async Task<QuotaUsageDto> GetQuotaUsed()
    {
        return await _quotaUsageManager.Get();
    }

    [HttpPost("userquotasettings")]
    public async Task<object> SaveUserQuotaSettingsAsync(UserQuotaSettingsRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        await _settingsManager.SaveAsync(new TenantUserQuotaSettings { EnableUserQuota = inDto.EnableUserQuota, DefaultUserQuota = inDto.DefaultUserQuota });

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
    public async Task<List<TimezonesRequestsDto>> GetTimeZonesAsyncAsync()
    {
        await ApiContext.AuthByClaimAsync();
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
    public async Task<object> SaveDnsSettingsAsync(DnsSettingsRequestsDto model)
    {
        return await _dnsSettings.SaveDnsSettingsAsync(model.DnsName, model.Enable);
    }

    [HttpGet("recalculatequota")]
    public async Task RecalculateQuotaAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);
        _quotaSyncOperation.RecalculateQuota(await _tenantManager.GetCurrentTenantAsync());
    }

    [HttpGet("checkrecalculatequota")]
    public async Task<bool> CheckRecalculateQuotaAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);
        return _quotaSyncOperation.CheckRecalculateQuota(await _tenantManager.GetCurrentTenantAsync());
    }

    [HttpGet("logo")]
    public async Task<object> GetLogoAsync()
    {
        return await _tenantInfoSettingsHelper.GetAbsoluteCompanyLogoPathAsync(await _settingsManager.LoadAsync<TenantInfoSettings>());
    }

    [AllowNotPayment]
    [HttpPut("wizard/complete")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "Wizard")]
    public async Task<WizardSettings> CompleteWizardAsync(WizardRequestsDto inDto)
    {
        await ApiContext.AuthByClaimAsync();

        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        return await _firstTimeTenantSettings.SaveDataAsync(inDto);
    }

    ///<visible>false</visible>
    [HttpPut("welcome/close")]
    public async Task CloseWelcomePopupAsync()
    {
        var currentUser = await _userManager.GetUsersAsync(_authContext.CurrentAccount.ID);

        var collaboratorPopupSettings = await _settingsManager.LoadForCurrentUserAsync<CollaboratorSettings>();

        if (!(await _userManager.IsUserAsync(currentUser) && collaboratorPopupSettings.FirstVisit && !await _userManager.IsOutsiderAsync(currentUser)))
        {
            throw new NotSupportedException("Not available.");
        }

        collaboratorPopupSettings.FirstVisit = false;
        await _settingsManager.SaveForCurrentUserAsync(collaboratorPopupSettings);
    }

    [AllowAnonymous, AllowNotPayment, AllowSuspended]
    [HttpGet("colortheme")]
    public async Task<CustomColorThemesSettingsDto> GetColorThemeAsync()
    {
        return new CustomColorThemesSettingsDto(await _settingsManager.LoadAsync<CustomColorThemesSettings>(), _customColorThemesSettingsHelper.Limit);
    }

    [HttpPut("colortheme")]
    public async Task<CustomColorThemesSettingsDto> SaveColorThemeAsync(CustomColorThemesSettingsRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);
        var settings = await _settingsManager.LoadAsync<CustomColorThemesSettings>();

        if (inDto.Theme != null)
        {
            try
            {
                await _semaphore.WaitAsync();
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


                await _settingsManager.SaveAsync(settings);
            }
            catch
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        if (inDto.Selected.HasValue && settings.Themes.Any(r => r.Id == inDto.Selected.Value))
        {
            settings.Selected = inDto.Selected.Value;
            await _settingsManager.SaveAsync(settings);
            await _messageService.SendAsync(MessageAction.ColorThemeChanged);
        }

        return new CustomColorThemesSettingsDto(settings, _customColorThemesSettingsHelper.Limit);
    }

    [HttpDelete("colortheme")]
    public async Task<CustomColorThemesSettingsDto> DeleteColorThemeAsync(int id)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var settings = await _settingsManager.LoadAsync<CustomColorThemesSettings>();

        if (CustomColorThemesSettingsItem.Default.Any(r => r.Id == id))
        {
            return new CustomColorThemesSettingsDto(settings, _customColorThemesSettingsHelper.Limit);
        }

        settings.Themes = settings.Themes.Where(r => r.Id != id).ToList();

        if (settings.Selected == id)
        {
            settings.Selected = settings.Themes.Min(r => r.Id);
           await _messageService.SendAsync(MessageAction.ColorThemeChanged);
        }

        await _settingsManager.SaveAsync(settings);

        return new CustomColorThemesSettingsDto(settings, _customColorThemesSettingsHelper.Limit);
    }

    [HttpPut("closeadminhelper")]
    public async Task CloseAdminHelperAsync()
    {
        if (!await _userManager.IsDocSpaceAdminAsync(_authContext.CurrentAccount.ID) || _coreBaseSettings.CustomMode || !_coreBaseSettings.Standalone)
        {
            throw new NotSupportedException("Not available.");
        }

        var adminHelperSettings = await _settingsManager.LoadForCurrentUserAsync<AdminHelperSettings>();
        adminHelperSettings.Viewed = true;
        await _settingsManager.SaveForCurrentUserAsync(adminHelperSettings);
    }

    ///<visible>false</visible>
    [HttpPut("timeandlanguage")]
    public async Task<object> TimaAndLanguageAsync(SettingsRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

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

        await _tenantManager.SaveTenantAsync(Tenant);

        if (!Tenant.TimeZone.Equals(oldTimeZone) || changelng)
        {
            if (!Tenant.TimeZone.Equals(oldTimeZone))
            {
                await _messageService.SendAsync(MessageAction.TimeZoneSettingsUpdated);
            }
            if (changelng)
            {
                await _messageService.SendAsync(MessageAction.LanguageSettingsUpdated);
            }
        }

        return Resource.SuccessfullySaveSettingsMessage;
    }

    ///<visible>false</visible>
    [HttpPut("defaultpage")]
    public async Task<object> SaveDefaultPageSettingAsync(SettingsRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        await _settingsManager.SaveAsync(new StudioDefaultPageSettings { DefaultProductID = inDto.DefaultProductID });

        await _messageService.SendAsync(MessageAction.DefaultStartPageSettingsUpdated);

        return Resource.SuccessfullySaveSettingsMessage;
    }

    [HttpPut("emailactivation")]
    public async Task<EmailActivationSettings> UpdateEmailActivationSettingsAsync(EmailActivationSettings settings)
    {
        await _settingsManager.SaveForCurrentUserAsync(settings);
        return settings;
    }

    [HttpGet("statistics/spaceusage/{id}")]
    public async Task<List<UsageSpaceStatItemDto>> GetSpaceUsageStatistics(Guid id)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var webitem = _webItemManagerSecurity.GetItems(WebZoneType.All, ItemAvailableState.All)
                                   .FirstOrDefault(item =>
                                                   item != null &&
                                                   item.ID == id &&
                                                   item.Context != null &&
                                                   item.Context.SpaceUsageStatManager != null);

        if (webitem == null)
        {
            return new List<UsageSpaceStatItemDto>();
        }

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
    public async Task<List<ChartPointDto>> GetVisitStatisticsAsync(ApiDateTime fromDate, ApiDateTime toDate)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

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

        var hits = await _statisticManager.GetHitsByPeriodAsync(Tenant.Id, from, to);
        var hosts = await _statisticManager.GetHostsByPeriodAsync(Tenant.Id, from, to);

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
    public async Task<bool> SaveAuthKeys(AuthServiceRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var saveAvailable = _coreBaseSettings.Standalone || (await _tenantManager.GetTenantQuotaAsync(await _tenantManager.GetCurrentTenantIdAsync())).ThirdParty;
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

        if (validateKeyProvider != null && !await validateKeyProvider.ValidateKeysAsync() && !allPropsIsEmpty)
        {
            consumer.Clear();
            throw new ArgumentException(Resource.ErrorBadKeys);
        }

        if (changed)
        {
            await _messageService.SendAsync(MessageAction.AuthorizationKeysSetting);
        }

        return changed;
    }

    [AllowNotPayment]
    [HttpGet("payment")]
    public async Task<object> PaymentSettingsAsync()
    {
        var settings = await _settingsManager.LoadForDefaultTenantAsync<AdditionalWhiteLabelSettings>();
        var currentQuota = await _tenantManager.GetCurrentTenantQuotaAsync();
        var currentTariff = await _tenantExtra.GetCurrentTariffAsync();

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
    public async Task<object> TelegramIsConnectedAsync()
    {
        return (int) await _telegramHelper.UserIsConnectedAsync(_authContext.CurrentAccount.ID, Tenant.Id);
    }

    /// <summary>
    /// Unlinks TelegramBot from your account
    /// </summary>
    [HttpDelete("telegramdisconnect")]
    public async Task TelegramDisconnectAsync()
    {
        await _telegramHelper.DisconnectAsync(_authContext.CurrentAccount.ID, Tenant.Id);
    }
}