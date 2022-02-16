using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Api.Controllers.Settings;

public class TipsController: BaseSettingsController
{
    public TipsController(IOptionsMonitor<ILog> option,
    MessageService messageService,
    StudioNotifyService studioNotifyService,
    ApiContext apiContext,
    UserManager userManager,
    TenantManager tenantManager,
    TenantExtra tenantExtra,
    TenantStatisticsProvider tenantStatisticsProvider,
    AuthContext authContext,
    CookiesManager cookiesManager,
    WebItemSecurity webItemSecurity,
    StudioNotifyHelper studioNotifyHelper,
    LicenseReader licenseReader,
    PermissionContext permissionContext,
    SettingsManager settingsManager,
    TfaManager tfaManager,
    WebItemManager webItemManager,
    WebItemManagerSecurity webItemManagerSecurity,
    TenantInfoSettingsHelper tenantInfoSettingsHelper,
    TenantWhiteLabelSettingsHelper tenantWhiteLabelSettingsHelper,
    StorageHelper storageHelper,
    TenantLogoManager tenantLogoManager,
    TenantUtil tenantUtil,
    CoreBaseSettings coreBaseSettings,
    CommonLinkUtility commonLinkUtility,
    ColorThemesSettingsHelper colorThemesSettingsHelper,
    IConfiguration configuration,
    SetupInfo setupInfo,
    BuildVersion buildVersion,
    DisplayUserSettingsHelper displayUserSettingsHelper,
    StatisticManager statisticManager,
    IPRestrictionsService iPRestrictionsService,
    CoreConfiguration coreConfiguration,
    MessageTarget messageTarget,
    StudioSmsNotificationSettingsHelper studioSmsNotificationSettingsHelper,
    CoreSettings coreSettings,
    StorageSettingsHelper storageSettingsHelper,
    IWebHostEnvironment webHostEnvironment,
    IServiceProvider serviceProvider,
    EmployeeWraperHelper employeeWraperHelper,
    ConsumerFactory consumerFactory,
    SmsProviderManager smsProviderManager,
    TimeZoneConverter timeZoneConverter,
    CustomNamingPeople customNamingPeople,
    IPSecurity.IPSecurity ipSecurity,
    IMemoryCache memoryCache,
    ProviderManager providerManager,
    FirstTimeTenantSettings firstTimeTenantSettings,
    ServiceClient serviceClient,
    TelegramHelper telegramHelper,
    StorageFactory storageFactory,
    UrlShortener urlShortener,
    EncryptionServiceClient encryptionServiceClient,
    EncryptionSettingsHelper encryptionSettingsHelper,
    BackupAjaxHandler backupAjaxHandler,
    ICacheNotify<DeleteSchedule> cacheDeleteSchedule,
    EncryptionWorker encryptionWorker,
    PasswordHasher passwordHasher,
    PaymentManager paymentManager,
    Constants constants,
    InstanceCrypto instanceCrypto,
    Signature signature,
    DbWorker dbWorker,
    IHttpClientFactory clientFactory) : base(option, messageService, studioNotifyService, apiContext, userManager, tenantManager, tenantExtra, tenantStatisticsProvider, authContext, cookiesManager, webItemSecurity, studioNotifyHelper, licenseReader, permissionContext, settingsManager, tfaManager, webItemManager, webItemManagerSecurity, tenantInfoSettingsHelper, tenantWhiteLabelSettingsHelper, storageHelper, tenantLogoManager, tenantUtil, coreBaseSettings, commonLinkUtility, colorThemesSettingsHelper, configuration, setupInfo, buildVersion, displayUserSettingsHelper, statisticManager, iPRestrictionsService, coreConfiguration, messageTarget, studioSmsNotificationSettingsHelper, coreSettings, storageSettingsHelper, webHostEnvironment, serviceProvider, employeeWraperHelper, consumerFactory, smsProviderManager, timeZoneConverter, customNamingPeople, ipSecurity, memoryCache, providerManager, firstTimeTenantSettings, serviceClient, telegramHelper, storageFactory, urlShortener, encryptionServiceClient, encryptionSettingsHelper, backupAjaxHandler, cacheDeleteSchedule, encryptionWorker, passwordHasher, paymentManager, constants, instanceCrypto, signature, dbWorker, clientFactory)
    {
    }

    [Update("tips")]
    public TipsSettings UpdateTipsSettingsFromBody([FromBody] SettingsDto model)
    {
        return UpdateTipsSettings(model);
    }

    [Update("tips")]
    [Consumes("application/x-www-form-urlencoded")]
    public TipsSettings UpdateTipsSettingsFromForm([FromForm] SettingsDto model)
    {
        return UpdateTipsSettings(model);
    }

    private TipsSettings UpdateTipsSettings(SettingsDto model)
    {
        var settings = new TipsSettings { Show = model.Show };
        _settingsManager.SaveForCurrentUser(settings);

        if (!model.Show && !string.IsNullOrEmpty(_setupInfo.TipsAddress))
        {
            try
            {
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri($"{_setupInfo.TipsAddress}/tips/deletereaded");

                var data = new NameValueCollection
                {
                    ["userId"] = _authContext.CurrentAccount.ID.ToString(),
                    ["tenantId"] = Tenant.TenantId.ToString(CultureInfo.InvariantCulture)
                };
                var body = JsonSerializer.Serialize(data);//todo check
                request.Content = new StringContent(body);

                var httpClient = _clientFactory.CreateClient();
                using var response = httpClient.Send(request);

            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        return settings;
    }

    [Update("tips/change/subscription")]
    public bool UpdateTipsSubscription()
    {
        return StudioPeriodicNotify.ChangeSubscription(_authContext.CurrentAccount.ID, _studioNotifyHelper);
    }
}
