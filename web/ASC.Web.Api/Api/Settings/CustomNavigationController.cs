using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Api.Controllers.Settings;

public class CustomNavigationController: BaseSettingsController
{
    public CustomNavigationController(IOptionsMonitor<ILog> option,
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

    [Read("customnavigation/getall")]
    public List<CustomNavigationItem> GetCustomNavigationItems()
    {
        return _settingsManager.Load<CustomNavigationSettings>().Items;
    }

    [Read("customnavigation/getsample")]
    public CustomNavigationItem GetCustomNavigationItemSample()
    {
        return CustomNavigationItem.GetSample();
    }

    [Read("customnavigation/get/{id}")]
    public CustomNavigationItem GetCustomNavigationItem(Guid id)
    {
        return _settingsManager.Load<CustomNavigationSettings>().Items.FirstOrDefault(item => item.Id == id);
    }

    [Create("customnavigation/create")]
    public CustomNavigationItem CreateCustomNavigationItemFromBody([FromBody] CustomNavigationItem item)
    {
        return CreateCustomNavigationItem(item);
    }

    [Create("customnavigation/create")]
    [Consumes("application/x-www-form-urlencoded")]
    public CustomNavigationItem CreateCustomNavigationItemFromForm([FromForm] CustomNavigationItem item)
    {
        return CreateCustomNavigationItem(item);
    }

    private CustomNavigationItem CreateCustomNavigationItem(CustomNavigationItem item)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var settings = _settingsManager.Load<CustomNavigationSettings>();

        var exist = false;

        foreach (var existItem in settings.Items)
        {
            if (existItem.Id != item.Id) continue;

            existItem.Label = item.Label;
            existItem.Url = item.Url;
            existItem.ShowInMenu = item.ShowInMenu;
            existItem.ShowOnHomePage = item.ShowOnHomePage;

            if (existItem.SmallImg != item.SmallImg)
            {
                _storageHelper.DeleteLogo(existItem.SmallImg);
                existItem.SmallImg = _storageHelper.SaveTmpLogo(item.SmallImg);
            }

            if (existItem.BigImg != item.BigImg)
            {
                _storageHelper.DeleteLogo(existItem.BigImg);
                existItem.BigImg = _storageHelper.SaveTmpLogo(item.BigImg);
            }

            exist = true;
            break;
        }

        if (!exist)
        {
            item.Id = Guid.NewGuid();
            item.SmallImg = _storageHelper.SaveTmpLogo(item.SmallImg);
            item.BigImg = _storageHelper.SaveTmpLogo(item.BigImg);

            settings.Items.Add(item);
        }

        _settingsManager.Save(settings);

        _messageService.Send(MessageAction.CustomNavigationSettingsUpdated);

        return item;
    }

    [Delete("customnavigation/delete/{id}")]
    public void DeleteCustomNavigationItem(Guid id)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var settings = _settingsManager.Load<CustomNavigationSettings>();

        var terget = settings.Items.FirstOrDefault(item => item.Id == id);

        if (terget == null) return;

        _storageHelper.DeleteLogo(terget.SmallImg);
        _storageHelper.DeleteLogo(terget.BigImg);

        settings.Items.Remove(terget);
        _settingsManager.Save(settings);

        _messageService.Send(MessageAction.CustomNavigationSettingsUpdated);
    }
}
