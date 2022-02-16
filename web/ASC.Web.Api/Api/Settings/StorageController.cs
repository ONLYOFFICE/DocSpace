using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Api.Controllers.Settings;

public class StorageController: BaseSettingsController
{
    public StorageController(IOptionsMonitor<ILog> option,
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

    [Read("storage")]
    public List<StorageWrapper> GetAllStorages()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        _tenantExtra.DemandControlPanelPermission();

        var current = _settingsManager.Load<StorageSettings>();
        var consumers = _consumerFactory.GetAll<DataStoreConsumer>();
        return consumers.Select(consumer => new StorageWrapper(consumer, current)).ToList();
    }

    [Read("storage/progress", false)]
    public double GetStorageProgress()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (!_coreBaseSettings.Standalone) return -1;

        return _serviceClient.GetProgress(Tenant.TenantId);
    }

    public readonly object Locker = new object();

    [Create("encryption/start")]
    public bool StartStorageEncryptionFromBody([FromBody] StorageEncryptionDto storageEncryption)
    {
        return StartStorageEncryption(storageEncryption);
    }

    [Create("encryption/start")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool StartStorageEncryptionFromForm([FromForm] StorageEncryptionDto storageEncryption)
    {
        return StartStorageEncryption(storageEncryption);
    }

    private bool StartStorageEncryption(StorageEncryptionDto storageEncryption)
    {
        if (_coreBaseSettings.CustomMode)
        {
            return false;
        }

        lock (Locker)
        {
            var activeTenants = _tenantManager.GetTenants();

            if (activeTenants.Count > 0)
            {
                StartEncryption(storageEncryption.NotifyUsers);
            }
        }
        return true;
    }

    private void StartEncryption(bool notifyUsers)
    {
        if (!SetupInfo.IsVisibleSettings<EncryptionSettings>())
        {
            throw new NotSupportedException();
        }

        if (!_coreBaseSettings.Standalone)
        {
            throw new NotSupportedException();
        }

        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        _tenantExtra.DemandControlPanelPermission();

        if (!_tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().TenantId).DiscEncryption)
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "DiscEncryption");
        }

        var storages = GetAllStorages();

        if (storages.Any(s => s.Current))
        {
            throw new NotSupportedException();
        }

        var cdnStorages = GetAllCdnStorages();

        if (cdnStorages.Any(s => s.Current))
        {
            throw new NotSupportedException();
        }

        var tenants = _tenantManager.GetTenants();

        foreach (var tenant in tenants)
        {
            var progress = _backupAjaxHandler.GetBackupProgress(tenant.TenantId);
            if (progress != null && !progress.IsCompleted)
            {
                throw new Exception();
            }
        }

        foreach (var tenant in tenants)
        {
            _cacheDeleteSchedule.Publish(new DeleteSchedule() { TenantId = tenant.TenantId }, Common.Caching.CacheNotifyAction.Insert);
        }

        var settings = _encryptionSettingsHelper.Load();

        settings.NotifyUsers = notifyUsers;

        if (settings.Status == EncryprtionStatus.Decrypted)
        {
            settings.Status = EncryprtionStatus.EncryptionStarted;
            settings.Password = _encryptionSettingsHelper.GeneratePassword(32, 16);
        }
        else if (settings.Status == EncryprtionStatus.Encrypted)
        {
            settings.Status = EncryprtionStatus.DecryptionStarted;
        }

        _messageService.Send(settings.Status == EncryprtionStatus.EncryptionStarted ? MessageAction.StartStorageEncryption : MessageAction.StartStorageDecryption);

        var serverRootPath = _commonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/');

        foreach (var tenant in tenants)
        {
            _tenantManager.SetCurrentTenant(tenant);

            if (notifyUsers)
            {
                if (settings.Status == EncryprtionStatus.EncryptionStarted)
                {
                    _studioNotifyService.SendStorageEncryptionStart(serverRootPath);
                }
                else
                {
                    _studioNotifyService.SendStorageDecryptionStart(serverRootPath);
                }
            }

            tenant.SetStatus(TenantStatus.Encryption);
            _tenantManager.SaveTenant(tenant);
        }

        _encryptionSettingsHelper.Save(settings);

        var encryptionSettingsProto = new EncryptionSettingsProto
        {
            NotifyUsers = settings.NotifyUsers,
            Password = settings.Password,
            Status = settings.Status,
            ServerRootPath = serverRootPath
        };
        _encryptionServiceClient.Start(encryptionSettingsProto);
    }

    /// <summary>
    /// Get storage encryption settings
    /// </summary>
    /// <returns>EncryptionSettings</returns>
    /// <visible>false</visible>
    [Read("encryption/settings")]
    public EncryptionSettings GetStorageEncryptionSettings()
    {
        try
        {
            if (_coreBaseSettings.CustomMode)
            {
                return null;
            }

            if (!SetupInfo.IsVisibleSettings<EncryptionSettings>())
            {
                throw new NotSupportedException();
            }

            if (!_coreBaseSettings.Standalone)
            {
                throw new NotSupportedException();
            }

            _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            _tenantExtra.DemandControlPanelPermission();

            if (!_tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().TenantId).DiscEncryption)
            {
                throw new BillingException(Resource.ErrorNotAllowedOption, "DiscEncryption");
            }

            var settings = _encryptionSettingsHelper.Load();

            settings.Password = string.Empty; // Don't show password

            return settings;
        }
        catch (Exception e)
        {
            _log.Error("GetStorageEncryptionSettings", e);
            return null;
        }
    }

    [Read("encryption/progress")]
    public double? GetStorageEncryptionProgress()
    {
        if (_coreBaseSettings.CustomMode)
        {
            return -1;
        }

        if (!SetupInfo.IsVisibleSettings<EncryptionSettings>())
        {
            throw new NotSupportedException();
        }

        if (!_coreBaseSettings.Standalone)
        {
            throw new NotSupportedException();
        }

        if (!_tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().TenantId).DiscEncryption)
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "DiscEncryption");
        }

        return _encryptionWorker.GetEncryptionProgress();
    }

    [Update("storage")]
    public StorageSettings UpdateStorageFromBody([FromBody] StorageDto model)
    {
        return UpdateStorage(model);
    }

    [Update("storage")]
    [Consumes("application/x-www-form-urlencoded")]
    public StorageSettings UpdateStorageFromForm([FromForm] StorageDto model)
    {
        return UpdateStorage(model);
    }

    private StorageSettings UpdateStorage(StorageDto model)
    {
        try
        {
            _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
            if (!_coreBaseSettings.Standalone) return null;

            _tenantExtra.DemandControlPanelPermission();

            var consumer = _consumerFactory.GetByKey(model.Module);
            if (!consumer.IsSet)
                throw new ArgumentException("module");

            var settings = _settingsManager.Load<StorageSettings>();
            if (settings.Module == model.Module) return settings;

            settings.Module = model.Module;
            settings.Props = model.Props.ToDictionary(r => r.Key, b => b.Value);

            StartMigrate(settings);
            return settings;
        }
        catch (Exception e)
        {
            _log.Error("UpdateStorage", e);
            throw;
        }
    }

    [Delete("storage")]
    public void ResetStorageToDefault()
    {
        try
        {
            _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
            if (!_coreBaseSettings.Standalone) return;

            _tenantExtra.DemandControlPanelPermission();

            var settings = _settingsManager.Load<StorageSettings>();

            settings.Module = null;
            settings.Props = null;


            StartMigrate(settings);
        }
        catch (Exception e)
        {
            _log.Error("ResetStorageToDefault", e);
            throw;
        }
    }

    [Read("storage/cdn")]
    public List<StorageWrapper> GetAllCdnStorages()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        if (!_coreBaseSettings.Standalone) return null;

        _tenantExtra.DemandControlPanelPermission();

        var current = _settingsManager.Load<CdnStorageSettings>();
        var consumers = _consumerFactory.GetAll<DataStoreConsumer>().Where(r => r.Cdn != null);
        return consumers.Select(consumer => new StorageWrapper(consumer, current)).ToList();
    }

    [Update("storage/cdn")]
    public CdnStorageSettings UpdateCdnFromBody([FromBody] StorageDto model)
    {
        return UpdateCdn(model);
    }

    [Update("storage/cdn")]
    [Consumes("application/x-www-form-urlencoded")]
    public CdnStorageSettings UpdateCdnFromForm([FromForm] StorageDto model)
    {
        return UpdateCdn(model);
    }

    private CdnStorageSettings UpdateCdn(StorageDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        if (!_coreBaseSettings.Standalone) return null;

        _tenantExtra.DemandControlPanelPermission();

        var consumer = _consumerFactory.GetByKey(model.Module);
        if (!consumer.IsSet)
            throw new ArgumentException("module");

        var settings = _settingsManager.Load<CdnStorageSettings>();
        if (settings.Module == model.Module) return settings;

        settings.Module = model.Module;
        settings.Props = model.Props.ToDictionary(r => r.Key, b => b.Value);

        try
        {
            _serviceClient.UploadCdn(Tenant.TenantId, "/", _webHostEnvironment.ContentRootPath, settings);
        }
        catch (Exception e)
        {
            _log.Error("UpdateCdn", e);
            throw;
        }

        return settings;
    }

    [Delete("storage/cdn")]
    public void ResetCdnToDefault()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        if (!_coreBaseSettings.Standalone) return;

        _tenantExtra.DemandControlPanelPermission();

        _storageSettingsHelper.Clear(_settingsManager.Load<CdnStorageSettings>());
    }

    [Read("storage/backup")]
    public List<StorageWrapper> GetAllBackupStorages()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        if (_coreBaseSettings.Standalone)
        {
            _tenantExtra.DemandControlPanelPermission();
        }
        var schedule = _backupAjaxHandler.GetSchedule();
        var current = new StorageSettings();

        if (schedule != null && schedule.StorageType == BackupStorageType.ThirdPartyConsumer)
        {
            current = new StorageSettings
            {
                Module = schedule.StorageParams["module"],
                Props = schedule.StorageParams.Where(r => r.Key != "module").ToDictionary(r => r.Key, r => r.Value)
            };
        }

        var consumers = _consumerFactory.GetAll<DataStoreConsumer>();
        return consumers.Select(consumer => new StorageWrapper(consumer, current)).ToList();
    }

    private void StartMigrate(StorageSettings settings)
    {
        _serviceClient.Migrate(Tenant.TenantId, settings);

        Tenant.SetStatus(TenantStatus.Migrating);
        _tenantManager.SaveTenant(Tenant);
    }
}
