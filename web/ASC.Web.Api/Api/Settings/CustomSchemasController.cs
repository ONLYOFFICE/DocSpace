using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Api.Controllers.Settings;

public class CustomSchemasController : BaseSettingsController
{
    public CustomSchemasController(IOptionsMonitor<ILog> option,
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

    [Read("customschemas")]
    public List<SchemaDto> PeopleSchemas()
    {
        return _customNamingPeople
                .GetSchemas()
                .Select(r =>
                {
                    var names = _customNamingPeople.GetPeopleNames(r.Key);

                    return new SchemaDto
                    {
                        Id = names.Id,
                        Name = names.SchemaName,
                        UserCaption = names.UserCaption,
                        UsersCaption = names.UsersCaption,
                        GroupCaption = names.GroupCaption,
                        GroupsCaption = names.GroupsCaption,
                        UserPostCaption = names.UserPostCaption,
                        RegDateCaption = names.RegDateCaption,
                        GroupHeadCaption = names.GroupHeadCaption,
                        GuestCaption = names.GuestCaption,
                        GuestsCaption = names.GuestsCaption,
                    };
                })
                .ToList();
    }

    [Create("customschemas")]
    public SchemaDto SaveNamingSettings(SchemaDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        _customNamingPeople.SetPeopleNames(model.Id);

        _tenantManager.SaveTenant(_tenantManager.GetCurrentTenant());

        _messageService.Send(MessageAction.TeamTemplateChanged);

        return PeopleSchema(model.Id);
    }

    [Update("customschemas")]
    public SchemaDto SaveCustomNamingSettings(SchemaDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var usrCaption = (model.UserCaption ?? "").Trim();
        var usrsCaption = (model.UsersCaption ?? "").Trim();
        var grpCaption = (model.GroupCaption ?? "").Trim();
        var grpsCaption = (model.GroupsCaption ?? "").Trim();
        var usrStatusCaption = (model.UserPostCaption ?? "").Trim();
        var regDateCaption = (model.RegDateCaption ?? "").Trim();
        var grpHeadCaption = (model.GroupHeadCaption ?? "").Trim();
        var guestCaption = (model.GuestCaption ?? "").Trim();
        var guestsCaption = (model.GuestsCaption ?? "").Trim();

        if (string.IsNullOrEmpty(usrCaption)
            || string.IsNullOrEmpty(usrsCaption)
            || string.IsNullOrEmpty(grpCaption)
            || string.IsNullOrEmpty(grpsCaption)
            || string.IsNullOrEmpty(usrStatusCaption)
            || string.IsNullOrEmpty(regDateCaption)
            || string.IsNullOrEmpty(grpHeadCaption)
            || string.IsNullOrEmpty(guestCaption)
            || string.IsNullOrEmpty(guestsCaption))
        {
            throw new Exception(Resource.ErrorEmptyFields);
        }

        var names = new PeopleNamesItem
        {
            Id = PeopleNamesItem.CustomID,
            UserCaption = usrCaption.Substring(0, Math.Min(30, usrCaption.Length)),
            UsersCaption = usrsCaption.Substring(0, Math.Min(30, usrsCaption.Length)),
            GroupCaption = grpCaption.Substring(0, Math.Min(30, grpCaption.Length)),
            GroupsCaption = grpsCaption.Substring(0, Math.Min(30, grpsCaption.Length)),
            UserPostCaption = usrStatusCaption.Substring(0, Math.Min(30, usrStatusCaption.Length)),
            RegDateCaption = regDateCaption.Substring(0, Math.Min(30, regDateCaption.Length)),
            GroupHeadCaption = grpHeadCaption.Substring(0, Math.Min(30, grpHeadCaption.Length)),
            GuestCaption = guestCaption.Substring(0, Math.Min(30, guestCaption.Length)),
            GuestsCaption = guestsCaption.Substring(0, Math.Min(30, guestsCaption.Length)),
        };

        _customNamingPeople.SetPeopleNames(names);

        _tenantManager.SaveTenant(_tenantManager.GetCurrentTenant());

        _messageService.Send(MessageAction.TeamTemplateChanged);

        return PeopleSchema(PeopleNamesItem.CustomID);
    }

    [Read("customschemas/{id}")]
    public SchemaDto PeopleSchema(string id)
    {
        var names = _customNamingPeople.GetPeopleNames(id);
        var schemaItem = new SchemaDto
        {
            Id = names.Id,
            Name = names.SchemaName,
            UserCaption = names.UserCaption,
            UsersCaption = names.UsersCaption,
            GroupCaption = names.GroupCaption,
            GroupsCaption = names.GroupsCaption,
            UserPostCaption = names.UserPostCaption,
            RegDateCaption = names.RegDateCaption,
            GroupHeadCaption = names.GroupHeadCaption,
            GuestCaption = names.GuestCaption,
            GuestsCaption = names.GuestsCaption,
        };
        return schemaItem;
    }
}