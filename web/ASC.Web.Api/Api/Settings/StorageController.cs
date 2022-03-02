namespace ASC.Web.Api.Controllers.Settings;

public class StorageController : BaseSettingsController
{
    private Tenant Tenant { get { return _apiContext.Tenant; } }

    private readonly MessageService _messageService;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ConsumerFactory _consumerFactory;
    private readonly TenantManager _tenantManager;
    private readonly TenantExtra _tenantExtra;
    private readonly PermissionContext _permissionContext;
    private readonly SettingsManager _settingsManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly StorageSettingsHelper _storageSettingsHelper;
    private readonly ServiceClient _serviceClient;
    private readonly EncryptionServiceClient _encryptionServiceClient;
    private readonly EncryptionSettingsHelper _encryptionSettingsHelper;
    private readonly BackupAjaxHandler _backupAjaxHandler;
    private readonly ICacheNotify<DeleteSchedule> _cacheDeleteSchedule;
    private readonly EncryptionWorker _encryptionWorker;
    private readonly ILog _log;

    public StorageController(
        IOptionsMonitor<ILog> option,
        ServiceClient serviceClient,
        MessageService messageService,
        StudioNotifyService studioNotifyService,
        ApiContext apiContext,
        TenantManager tenantManager,
        TenantExtra tenantExtra,
        PermissionContext permissionContext,
        SettingsManager settingsManager,
        WebItemManager webItemManager,
        CoreBaseSettings coreBaseSettings,
        CommonLinkUtility commonLinkUtility,
        StorageSettingsHelper storageSettingsHelper,
        IWebHostEnvironment webHostEnvironment,
        ConsumerFactory consumerFactory,
        IMemoryCache memoryCache,
        EncryptionServiceClient encryptionServiceClient,
        EncryptionSettingsHelper encryptionSettingsHelper,
        BackupAjaxHandler backupAjaxHandler,
        ICacheNotify<DeleteSchedule> cacheDeleteSchedule,
        EncryptionWorker encryptionWorker) : base(apiContext, memoryCache, webItemManager)
    {
        _log = option.Get("ASC.Api");
        _serviceClient = serviceClient;
        _webHostEnvironment = webHostEnvironment;
        _consumerFactory = consumerFactory;
        _messageService = messageService;
        _studioNotifyService = studioNotifyService;
        _tenantManager = tenantManager;
        _tenantExtra = tenantExtra;
        _permissionContext = permissionContext;
        _settingsManager = settingsManager;
        _coreBaseSettings = coreBaseSettings;
        _commonLinkUtility = commonLinkUtility;
        _storageSettingsHelper = storageSettingsHelper;
        _encryptionServiceClient = encryptionServiceClient;
        _encryptionSettingsHelper = encryptionSettingsHelper;
        _backupAjaxHandler = backupAjaxHandler;
        _cacheDeleteSchedule = cacheDeleteSchedule;
        _encryptionWorker = encryptionWorker;
    }

    [Read("storage")]
    public List<StorageResponseDto> GetAllStorages()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        _tenantExtra.DemandControlPanelPermission();

        var current = _settingsManager.Load<StorageSettings>();
        var consumers = _consumerFactory.GetAll<DataStoreConsumer>();
        return consumers.Select(consumer => new StorageResponseDto(consumer, current)).ToList();
    }

    [Read("storage/progress", false)]
    public double GetStorageProgress()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (!_coreBaseSettings.Standalone) return -1;

        return _serviceClient.GetProgress(Tenant.Id);
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

        if (!_tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().Id).DiscEncryption)
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
            var progress = _backupAjaxHandler.GetBackupProgress(tenant.Id);
            if (progress != null && !progress.IsCompleted)
            {
                throw new Exception();
            }
        }

        foreach (var tenant in tenants)
        {
            _cacheDeleteSchedule.Publish(new DeleteSchedule() { TenantId = tenant.Id }, Common.Caching.CacheNotifyAction.Insert);
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

            if (!_tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().Id).DiscEncryption)
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

        if (!_tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().Id).DiscEncryption)
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
    public List<StorageResponseDto> GetAllCdnStorages()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        if (!_coreBaseSettings.Standalone) return null;

        _tenantExtra.DemandControlPanelPermission();

        var current = _settingsManager.Load<CdnStorageSettings>();
        var consumers = _consumerFactory.GetAll<DataStoreConsumer>().Where(r => r.Cdn != null);
        return consumers.Select(consumer => new StorageResponseDto(consumer, current)).ToList();
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
            _serviceClient.UploadCdn(Tenant.Id, "/", _webHostEnvironment.ContentRootPath, settings);
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
    public List<StorageResponseDto> GetAllBackupStorages()
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
        return consumers.Select(consumer => new StorageResponseDto(consumer, current)).ToList();
    }

    private void StartMigrate(StorageSettings settings)
    {
        _serviceClient.Migrate(Tenant.Id, settings);

        Tenant.SetStatus(TenantStatus.Migrating);
        _tenantManager.SaveTenant(Tenant);
    }
}
