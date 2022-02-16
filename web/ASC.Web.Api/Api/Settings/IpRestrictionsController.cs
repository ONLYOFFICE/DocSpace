namespace ASC.Web.Api.Controllers.Settings;

public class IpRestrictionsController: BaseSettingsController
{
    public IpRestrictionsController(IOptionsMonitor<ILog> option,
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
    ASC.Core.Users.Constants constants,
    InstanceCrypto instanceCrypto,
    Signature signature,
    DbWorker dbWorker,
    IHttpClientFactory clientFactory) : base(option, messageService, studioNotifyService, apiContext, userManager, tenantManager, tenantExtra, tenantStatisticsProvider, authContext, cookiesManager, webItemSecurity, studioNotifyHelper, licenseReader, permissionContext, settingsManager, tfaManager, webItemManager, webItemManagerSecurity, tenantInfoSettingsHelper, tenantWhiteLabelSettingsHelper, storageHelper, tenantLogoManager, tenantUtil, coreBaseSettings, commonLinkUtility, colorThemesSettingsHelper, configuration, setupInfo, buildVersion, displayUserSettingsHelper, statisticManager, iPRestrictionsService, coreConfiguration, messageTarget, studioSmsNotificationSettingsHelper, coreSettings, storageSettingsHelper, webHostEnvironment, serviceProvider, employeeWraperHelper, consumerFactory, smsProviderManager, timeZoneConverter, customNamingPeople, ipSecurity, memoryCache, providerManager, firstTimeTenantSettings, serviceClient, telegramHelper, storageFactory, urlShortener, encryptionServiceClient, encryptionSettingsHelper, backupAjaxHandler, cacheDeleteSchedule, encryptionWorker, passwordHasher, paymentManager, constants, instanceCrypto, signature, dbWorker, clientFactory)
    {
    }

    [Read("iprestrictions")]
    public IEnumerable<IPRestriction> GetIpRestrictions()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        return _iPRestrictionsService.Get(Tenant.TenantId);
    }

    [Update("iprestrictions")]
    public IEnumerable<string> SaveIpRestrictionsFromBody([FromBody] IpRestrictionsDto model)
    {
        return SaveIpRestrictions(model);
    }

    [Update("iprestrictions")]
    [Consumes("application/x-www-form-urlencoded")]
    public IEnumerable<string> SaveIpRestrictionsFromForm([FromForm] IpRestrictionsDto model)
    {
        return SaveIpRestrictions(model);
    }

    private IEnumerable<string> SaveIpRestrictions(IpRestrictionsDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        return _iPRestrictionsService.Save(model.Ips, Tenant.TenantId);
    }

    [Update("iprestrictions/settings")]
    public IPRestrictionsSettings UpdateIpRestrictionsSettingsFromBody([FromBody] IpRestrictionsDto model)
    {
        return UpdateIpRestrictionsSettings(model);
    }

    [Update("iprestrictions/settings")]
    [Consumes("application/x-www-form-urlencoded")]
    public IPRestrictionsSettings UpdateIpRestrictionsSettingsFromForm([FromForm] IpRestrictionsDto model)
    {
        return UpdateIpRestrictionsSettings(model);
    }

    private IPRestrictionsSettings UpdateIpRestrictionsSettings(IpRestrictionsDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var settings = new IPRestrictionsSettings { Enable = model.Enable };
        _settingsManager.Save(settings);

        return settings;
    }
}