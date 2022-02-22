using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Api.Controllers.Settings;
public class SettingsController: BaseSettingsController
{
    private Tenant Tenant { get { return _apiContext.Tenant; } }

    private readonly MessageService _messageService;
    private readonly ConsumerFactory _consumerFactory;
    private readonly TimeZoneConverter _timeZoneConverter;
    private readonly CustomNamingPeople _customNamingPeople;
    private readonly ProviderManager _providerManager;
    private readonly FirstTimeTenantSettings _firstTimeTenantSettings;
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly TenantExtra _tenantExtra;
    private readonly TenantStatisticsProvider _tenantStatisticsProvider;
    private readonly AuthContext _authContext;
    private readonly PermissionContext _permissionContext;
    private readonly SettingsManager _settingsManager;
    private readonly WebItemManagerSecurity _webItemManagerSecurity;
    private readonly TenantInfoSettingsHelper _tenantInfoSettingsHelper;
    private readonly TenantUtil _tenantUtil;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly ColorThemesSettingsHelper _colorThemesSettingsHelper;
    private readonly IConfiguration _configuration;
    private readonly SetupInfo _setupInfo;
    private readonly StatisticManager _statisticManager;
    private readonly CoreConfiguration _coreConfiguration;
    private readonly UrlShortener _urlShortener;
    private readonly PasswordHasher _passwordHasher;
    private readonly ILog _log;
    private readonly TelegramHelper _telegramHelper;
    private readonly Constants _constants;

    public SettingsController(
        IOptionsMonitor<ILog> option,
        MessageService messageService,
        ApiContext apiContext,
        UserManager userManager,
        TenantManager tenantManager,
        TenantExtra tenantExtra,
        TenantStatisticsProvider tenantStatisticsProvider,
        AuthContext authContext,
        PermissionContext permissionContext,
        SettingsManager settingsManager,
        WebItemManager webItemManager,
        WebItemManagerSecurity webItemManagerSecurity,
        TenantInfoSettingsHelper tenantInfoSettingsHelper,
        TenantUtil tenantUtil,
        CoreBaseSettings coreBaseSettings,
        CommonLinkUtility commonLinkUtility,
        ColorThemesSettingsHelper colorThemesSettingsHelper,
        IConfiguration configuration,
        SetupInfo setupInfo,
        StatisticManager statisticManager,
        CoreConfiguration coreConfiguration,
        ConsumerFactory consumerFactory,
        TimeZoneConverter timeZoneConverter,
        CustomNamingPeople customNamingPeople,
        IMemoryCache memoryCache,
        ProviderManager providerManager,
        FirstTimeTenantSettings firstTimeTenantSettings,
        TelegramHelper telegramHelper,
        UrlShortener urlShortener,
        PasswordHasher passwordHasher,
        Constants constants) : base(apiContext, memoryCache, webItemManager)
    {
        _log = option.Get("ASC.Api");
        _consumerFactory = consumerFactory;
        _timeZoneConverter = timeZoneConverter;
        _customNamingPeople = customNamingPeople;
        _providerManager = providerManager;
        _firstTimeTenantSettings = firstTimeTenantSettings;
        _messageService = messageService;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _tenantExtra = tenantExtra;
        _tenantStatisticsProvider = tenantStatisticsProvider;
        _authContext = authContext;
        _permissionContext = permissionContext;
        _settingsManager = settingsManager;
        _webItemManagerSecurity = webItemManagerSecurity;
        _tenantInfoSettingsHelper = tenantInfoSettingsHelper;
        _tenantUtil = tenantUtil;
        _coreBaseSettings = coreBaseSettings;
        _commonLinkUtility = commonLinkUtility;
        _colorThemesSettingsHelper = colorThemesSettingsHelper;
        _configuration = configuration;
        _setupInfo = setupInfo;
        _statisticManager = statisticManager;
        _coreConfiguration = coreConfiguration;
        _passwordHasher = passwordHasher;
        _urlShortener = urlShortener;
        _telegramHelper = telegramHelper;
        _constants = constants;
    }

    [Read("", Check = false)]
    [AllowAnonymous]
    public SettingsResponseDto GetSettings(bool? withpassword)
    {
        var settings = new SettingsResponseDto
        {
            Culture = Tenant.GetCulture().ToString(),
            GreetingSettings = Tenant.Name,
            Personal = _coreBaseSettings.Personal,
            Version = _configuration["version:number"] ?? ""
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

            settings.Firebase = new FirebaseResponseDto
            {
                ApiKey = _configuration["firebase:apiKey"] ?? "",
                AuthDomain = _configuration["firebase:authDomain"] ?? "",
                ProjectId = _configuration["firebase:projectId"] ?? "",
                StorageBucket = _configuration["firebase:storageBucket"] ?? "",
                MessagingSenderId = _configuration["firebase:messagingSenderId"] ?? "",
                AppId = _configuration["firebase:appId"] ?? "",
                MeasurementId = _configuration["firebase:measurementId"] ?? ""
            };

            bool debugInfo;
            if (bool.TryParse(_configuration["debug-info:enabled"], out debugInfo))
            {
                settings.DebugInfo = debugInfo;
            }
        }
        else
        {
            if (!_settingsManager.Load<WizardSettings>().Completed)
            {
                settings.WizardToken = _commonLinkUtility.GetToken(Tenant.TenantId, "", ConfirmType.Wizard, userId: Tenant.OwnerId);
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

            var studioAdminMessageSettings = _settingsManager.Load<StudioAdminMessageSettings>();

            settings.EnableAdmMess = studioAdminMessageSettings.Enable || _tenantExtra.IsNotPaid();

            settings.ThirdpartyEnable = _setupInfo.ThirdPartyAuthEnabled && _providerManager.IsNotEmpty;

            settings.RecaptchaPublicKey = _setupInfo.RecaptchaPublicKey;
        }

        if (!_authContext.IsAuthenticated || (withpassword.HasValue && withpassword.Value))
        {
            settings.PasswordHash = _passwordHasher;
        }

        return settings;
    }

    [Create("maildomainsettings")]
    public object SaveMailDomainSettingsFromBody([FromBody] MailDomainSettingsDto model)
    {
        return SaveMailDomainSettings(model);
    }

    [Create("maildomainsettings")]
    [Consumes("application/x-www-form-urlencoded")]
    public object SaveMailDomainSettingsFromForm([FromForm] MailDomainSettingsDto model)
    {
        return SaveMailDomainSettings(model);
    }

    private object SaveMailDomainSettings(MailDomainSettingsDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (model.Type == TenantTrustedDomainsType.Custom)
        {
            Tenant.TrustedDomains.Clear();
            foreach (var d in model.Domains.Select(domain => (domain ?? "").Trim().ToLower()))
            {
                if (!(!string.IsNullOrEmpty(d) && new Regex("^[a-z0-9]([a-z0-9-.]){1,98}[a-z0-9]$").IsMatch(d)))
                    return Resource.ErrorNotCorrectTrustedDomain;

                Tenant.TrustedDomains.Add(d);
            }

            if (Tenant.TrustedDomains.Count == 0)
                model.Type = TenantTrustedDomainsType.None;
        }

        Tenant.TrustedDomainsType = model.Type;

        _settingsManager.Save(new StudioTrustedDomainSettings { InviteUsersAsVisitors = model.InviteUsersAsVisitors });

        _tenantManager.SaveTenant(Tenant);

        _messageService.Send(MessageAction.TrustedMailDomainSettingsUpdated);

        return Resource.SuccessfullySaveSettingsMessage;
    }

    [Read("quota")]
    public QuotaResponseDto GetQuotaUsed()
    {
        return new QuotaResponseDto(Tenant, _coreBaseSettings, _coreConfiguration, _tenantExtra, _tenantStatisticsProvider, _authContext, _settingsManager, _webItemManager, _constants);
    }

    [AllowAnonymous]
    [Read("cultures", Check = false)]
    public IEnumerable<object> GetSupportedCultures()
    {
        return _setupInfo.EnabledCultures.Select(r => r.Name).ToArray();
    }

    [Authorize(AuthenticationSchemes = "confirm", Roles = "Wizard,Administrators")]
    [Read("timezones", Check = false)]
    public List<TimezonesDto> GetTimeZones()
    {
        _apiContext.AuthByClaim();
        var timeZones = TimeZoneInfo.GetSystemTimeZones().ToList();

        if (timeZones.All(tz => tz.Id != "UTC"))
        {
            timeZones.Add(TimeZoneInfo.Utc);
        }

        var listOfTimezones = new List<TimezonesDto>();

        foreach (var tz in timeZones.OrderBy(z => z.BaseUtcOffset))
        {
            listOfTimezones.Add(new TimezonesDto
            {
                Id = tz.Id,
                DisplayName = _timeZoneConverter.GetTimeZoneDisplayName(tz)
            });
        }

        return listOfTimezones;
    }

    [Authorize(AuthenticationSchemes = "confirm", Roles = "Wizard")]
    [Read("machine", Check = false)]
    public object GetMachineName()
    {
        return Dns.GetHostName().ToLowerInvariant();
    }

    //[Read("recalculatequota")]
    //public void RecalculateQuota()
    //{
    //    SecurityContext.DemandPermissions(Tenant, SecutiryConstants.EditPortalSettings);

    //    var operations = quotaTasks.GetTasks()
    //        .Where(t => t.GetProperty<int>(QuotaSync.TenantIdKey) == Tenant.TenantId);

    //    if (operations.Any(o => o.Status <= DistributedTaskStatus.Running))
    //    {
    //        throw new InvalidOperationException(Resource.LdapSettingsTooManyOperations);
    //    }

    //    var op = new QuotaSync(Tenant.TenantId, ServiceProvider);

    //    quotaTasks.QueueTask(op.RunJob, op.GetDistributedTask());
    //}

    //[Read("checkrecalculatequota")]
    //public bool CheckRecalculateQuota()
    //{
    //    PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

    //    var task = quotaTasks.GetTasks().FirstOrDefault(t => t.GetProperty<int>(QuotaSync.TenantIdKey) == Tenant.TenantId);

    //    if (task != null && task.Status == DistributedTaskStatus.Completed)
    //    {
    //        quotaTasks.RemoveTask(task.Id);
    //        return false;
    //    }

    //    return task != null;
    //}

    [Read("logo")]
    public object GetLogo()
    {
        return _tenantInfoSettingsHelper.GetAbsoluteCompanyLogoPath(_settingsManager.Load<TenantInfoSettings>());
    }

    [Update("wizard/complete", Check = false)]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "Wizard")]
    public WizardSettings CompleteWizardFromBody([FromBody] WizardDto wizardModel)
    {
        return CompleteWizard(wizardModel);
    }

    [Update("wizard/complete", Check = false)]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "Wizard")]
    [Consumes("application/x-www-form-urlencoded")]
    public WizardSettings CompleteWizardFromForm([FromForm] WizardDto wizardModel)
    {
        return CompleteWizard(wizardModel);
    }

    private WizardSettings CompleteWizard(WizardDto wizardModel)
    {
        _apiContext.AuthByClaim();

        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        return _firstTimeTenantSettings.SaveData(wizardModel);
    }

    ///<visible>false</visible>
    [Update("welcome/close")]
    public void CloseWelcomePopup()
    {
        var currentUser = _userManager.GetUsers(_authContext.CurrentAccount.ID);

        var collaboratorPopupSettings = _settingsManager.LoadForCurrentUser<CollaboratorSettings>();

        if (!(currentUser.IsVisitor(_userManager) && collaboratorPopupSettings.FirstVisit && !currentUser.IsOutsider(_userManager)))
            throw new NotSupportedException("Not available.");

        collaboratorPopupSettings.FirstVisit = false;
        _settingsManager.SaveForCurrentUser(collaboratorPopupSettings);
    }

    ///<visible>false</visible>
    [Update("colortheme")]
    public void SaveColorThemeFromBody([FromBody] SettingsDto model)
    {
        SaveColorTheme(model);
    }

    [Update("colortheme")]
    [Consumes("application/x-www-form-urlencoded")]
    public void SaveColorThemeFromForm([FromForm] SettingsDto model)
    {
        SaveColorTheme(model);
    }

    private void SaveColorTheme(SettingsDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        _colorThemesSettingsHelper.SaveColorTheme(model.Theme);
        _messageService.Send(MessageAction.ColorThemeChanged);
    }
    ///<visible>false</visible>
    [Update("timeandlanguage")]
    public object TimaAndLanguageFromBody([FromBody] SettingsDto model)
    {
        return TimaAndLanguage(model);
    }

    [Update("timeandlanguage")]
    [Consumes("application/x-www-form-urlencoded")]
    public object TimaAndLanguageFromForm([FromForm] SettingsDto model)
    {
        return TimaAndLanguage(model);
    }

    private object TimaAndLanguage(SettingsDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var culture = CultureInfo.GetCultureInfo(model.Lng);

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
        Tenant.TimeZone = timeZones.FirstOrDefault(tz => tz.Id == model.TimeZoneID)?.Id ?? TimeZoneInfo.Utc.Id;

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
    [Update("defaultpage")]
    public object SaveDefaultPageSettingsFromBody([FromBody] SettingsDto model)
    {
        return SaveDefaultPageSettings(model);
    }

    [Update("defaultpage")]
    [Consumes("application/x-www-form-urlencoded")]
    public object SaveDefaultPageSettingsFromForm([FromForm] SettingsDto model)
    {
        return SaveDefaultPageSettings(model);
    }

    private object SaveDefaultPageSettings(SettingsDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        _settingsManager.Save(new StudioDefaultPageSettings { DefaultProductID = model.DefaultProductID });

        _messageService.Send(MessageAction.DefaultStartPageSettingsUpdated);

        return Resource.SuccessfullySaveSettingsMessage;
    }

    [Update("emailactivation")]
    public EmailActivationSettings UpdateEmailActivationSettingsFromBody([FromBody] EmailActivationSettings settings)
    {
        _settingsManager.SaveForCurrentUser(settings);
        return settings;
    }

    [Update("emailactivation")]
    [Consumes("application/x-www-form-urlencoded")]
    public EmailActivationSettings UpdateEmailActivationSettingsFromForm([FromForm] EmailActivationSettings settings)
    {
        _settingsManager.SaveForCurrentUser(settings);
        return settings;
    }

    [Read("statistics/spaceusage/{id}")]
    public List<UsageSpaceStatItemResponseDto> GetSpaceUsageStatistics(Guid id)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var webtem = _webItemManagerSecurity.GetItems(WebZoneType.All, ItemAvailableState.All)
                                    .FirstOrDefault(item =>
                                                    item != null &&
                                                    item.ID == id &&
                                                    item.Context != null &&
                                                    item.Context.SpaceUsageStatManager != null);

        if (webtem == null) return new List<UsageSpaceStatItemResponseDto>();

        return webtem.Context.SpaceUsageStatManager.GetStatData()
                        .ConvertAll(it => new UsageSpaceStatItemResponseDto
                        {
                            Name = it.Name.HtmlEncode(),
                            Icon = it.ImgUrl,
                            Disabled = it.Disabled,
                            Size = FileSizeComment.FilesSizeToString(it.SpaceUsage),
                            Url = it.Url
                        });
    }

    [Read("statistics/visit")]
    public List<ChartPointResponseDto> GetVisitStatistics(ApiDateTime fromDate, ApiDateTime toDate)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var from = _tenantUtil.DateTimeFromUtc(fromDate);
        var to = _tenantUtil.DateTimeFromUtc(toDate);

        var points = new List<ChartPointResponseDto>();

        if (from.CompareTo(to) >= 0) return points;

        for (var d = new DateTime(from.Ticks); d.Date.CompareTo(to.Date) <= 0; d = d.AddDays(1))
        {
            points.Add(new ChartPointResponseDto
            {
                DisplayDate = d.Date.ToShortDateString(),
                Date = d.Date,
                Hosts = 0,
                Hits = 0
            });
        }

        var hits = _statisticManager.GetHitsByPeriod(Tenant.TenantId, from, to);
        var hosts = _statisticManager.GetHostsByPeriod(Tenant.TenantId, from, to);

        if (hits.Count == 0 || hosts.Count == 0) return points;

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

    [Read("socket")]
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

    ///<visible>false</visible>
    [Read("controlpanel")]
    public TenantControlPanelSettings GetTenantControlPanelSettings()
    {
        return _settingsManager.Load<TenantControlPanelSettings>();
    }

    [Read("authservice")]
    public IEnumerable<AuthServiceDto> GetAuthServices()
    {
        return _consumerFactory.GetAll<Consumer>()
            .Where(consumer => consumer.ManagedKeys.Any())
            .OrderBy(services => services.Order)
            .Select(r => new AuthServiceDto(r))
            .ToList();
    }

    [Create("authservice")]
    public bool SaveAuthKeysFromBody([FromBody] AuthServiceDto model)
    {
        return SaveAuthKeys(model);
    }

    [Create("authservice")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool SaveAuthKeysFromForm([FromForm] AuthServiceDto model)
    {
        return SaveAuthKeys(model);
    }

    private bool SaveAuthKeys(AuthServiceDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var saveAvailable = _coreBaseSettings.Standalone || _tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().TenantId).ThirdParty;
        if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.ThirdPartyAuthorization))
            || !saveAvailable)
            throw new BillingException(Resource.ErrorNotAllowedOption, "ThirdPartyAuthorization");

        var changed = false;
        var consumer = _consumerFactory.GetByKey<Consumer>(model.Name);

        var validateKeyProvider = consumer as IValidateKeysProvider;

        if (validateKeyProvider != null)
        {
            try
            {
                if (validateKeyProvider is TwilioProvider twilioLoginProvider)
                {
                    twilioLoginProvider.ClearOldNumbers();
                }
                if (validateKeyProvider is BitlyLoginProvider bitly)
                {
                    _urlShortener.Instance = null;
                }
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
        }

        if (model.Props.All(r => string.IsNullOrEmpty(r.Value)))
        {
            consumer.Clear();
            changed = true;
        }
        else
        {
            foreach (var authKey in model.Props.Where(authKey => consumer[authKey.Name] != authKey.Value))
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
            _messageService.Send(MessageAction.AuthorizationKeysSetting);

        return changed;
    }

    [Read("payment", Check = false)]
    public object PaymentSettings()
    {
        var settings = _settingsManager.LoadForDefaultTenant<AdditionalWhiteLabelSettings>();
        var currentQuota = _tenantExtra.GetTenantQuota();
        var currentTariff = _tenantExtra.GetCurrentTariff();

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
                }
            };
    }

    /// <visible>false</visible>
    /// <summary>
    /// Gets a link that will connect TelegramBot to your account
    /// </summary>
    /// <returns>url</returns>
    /// 
    [Read("telegramlink")]
    public object TelegramLink()
    {
        var currentLink = _telegramHelper.CurrentRegistrationLink(_authContext.CurrentAccount.ID, Tenant.TenantId);

        if (string.IsNullOrEmpty(currentLink))
        {
            var url = _telegramHelper.RegisterUser(_authContext.CurrentAccount.ID, Tenant.TenantId);
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
    [Read("telegramisconnected")]
    public object TelegramIsConnected()
    {
        return (int)_telegramHelper.UserIsConnected(_authContext.CurrentAccount.ID, Tenant.TenantId);
    }

    /// <summary>
    /// Unlinks TelegramBot from your account
    /// </summary>
    [Delete("telegramdisconnect")]
    public void TelegramDisconnect()
    {
        _telegramHelper.Disconnect(_authContext.CurrentAccount.ID, Tenant.TenantId);
    }

}