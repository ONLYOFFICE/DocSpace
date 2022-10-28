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

using ASC.Data.Storage.Encryption.IntegrationEvents.Events;
using ASC.EventBus.Abstractions;

namespace ASC.Web.Api.Controllers.Settings;

public class StorageController : BaseSettingsController
{
    private Tenant Tenant { get { return ApiContext.Tenant; } }

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
    private readonly EncryptionSettingsHelper _encryptionSettingsHelper;
    private readonly BackupAjaxHandler _backupAjaxHandler;
    private readonly ICacheNotify<DeleteSchedule> _cacheDeleteSchedule;
    private readonly EncryptionWorker _encryptionWorker;
    private readonly ILogger _log;
    private readonly IEventBus _eventBus;
    private readonly ASC.Core.SecurityContext _securityContext;

    public StorageController(
        ILoggerProvider option,
        ServiceClient serviceClient,
        MessageService messageService,
        ASC.Core.SecurityContext securityContext,
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
        IEventBus eventBus,
        EncryptionSettingsHelper encryptionSettingsHelper,
        BackupAjaxHandler backupAjaxHandler,
        ICacheNotify<DeleteSchedule> cacheDeleteSchedule,
        EncryptionWorker encryptionWorker,
        IHttpContextAccessor httpContextAccessor) : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _log = option.CreateLogger("ASC.Api");
        _eventBus = eventBus;
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
        _encryptionSettingsHelper = encryptionSettingsHelper;
        _backupAjaxHandler = backupAjaxHandler;
        _cacheDeleteSchedule = cacheDeleteSchedule;
        _encryptionWorker = encryptionWorker;
        _securityContext = securityContext;
    }

    [HttpGet("storage")]
    public List<StorageDto> GetAllStorages()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (!_coreBaseSettings.Standalone)
        {
            throw new SecurityException(Resource.ErrorAccessDenied);
        }

        var current = _settingsManager.Load<StorageSettings>();
        var consumers = _consumerFactory.GetAll<DataStoreConsumer>();
        return consumers.Select(consumer => new StorageDto(consumer, current)).ToList();
    }

    [AllowNotPayment]
    [HttpGet("storage/progress")]
    public double GetStorageProgress()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (!_coreBaseSettings.Standalone)
        {
            return -1;
        }

        return _serviceClient.GetProgress(Tenant.Id);
    }

    public readonly object Locker = new object();

    [HttpPost("encryption/start")]
    public bool StartStorageEncryption(StorageEncryptionRequestsDto inDto)
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
                StartEncryption(inDto.NotifyUsers);
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
            throw new SecurityException(Resource.ErrorAccessDenied);
        }

        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

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
            _cacheDeleteSchedule.Publish(new DeleteSchedule() { TenantId = tenant.Id }, CacheNotifyAction.Insert);
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

        _eventBus.Publish(new EncryptionDataStorageRequestedIntegration
        (
              encryptionSettings: new EncryptionSettings
              {
                  NotifyUsers = settings.NotifyUsers,
                  Password = settings.Password,
                  Status = settings.Status
              },
              serverRootPath: serverRootPath,
              createBy: _securityContext.CurrentAccount.ID,
              tenantId: _tenantManager.GetCurrentTenant().Id

        ));
    }

    /// <summary>
    /// Get storage encryption settings
    /// </summary>
    /// <returns>EncryptionSettings</returns>
    /// <visible>false</visible>
    [HttpGet("encryption/settings")]
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
                throw new SecurityException(Resource.ErrorAccessDenied);
            }

            _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var settings = _encryptionSettingsHelper.Load();

            settings.Password = string.Empty; // Don't show password

            return settings;
        }
        catch (Exception e)
        {
            _log.ErrorGetStorageEncryptionSettings(e);
            return null;
        }
    }

    [HttpGet("encryption/progress")]
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

        return _encryptionWorker.GetEncryptionProgress();
    }

    [HttpPut("storage")]
    public StorageSettings UpdateStorage(StorageRequestsDto inDto)
    {
        try
        {
            _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            if (!_coreBaseSettings.Standalone)
            {
                throw new SecurityException(Resource.ErrorAccessDenied);
            }

            var consumer = _consumerFactory.GetByKey(inDto.Module);
            if (!consumer.IsSet)
            {
                throw new ArgumentException("module");
            }

            var settings = _settingsManager.Load<StorageSettings>();
            if (settings.Module == inDto.Module)
            {
                return settings;
            }

            settings.Module = inDto.Module;
            settings.Props = inDto.Props.ToDictionary(r => r.Key, b => b.Value);

            StartMigrate(settings);
            return settings;
        }
        catch (Exception e)
        {
            _log.ErrorUpdateStorage(e);
            throw;
        }
    }

    [HttpDelete("storage")]
    public void ResetStorageToDefault()
    {
        try
        {
            _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            if (!_coreBaseSettings.Standalone)
            {
                throw new SecurityException(Resource.ErrorAccessDenied);
            }

            var settings = _settingsManager.Load<StorageSettings>();

            settings.Module = null;
            settings.Props = null;


            StartMigrate(settings);
        }
        catch (Exception e)
        {
            _log.ErrorResetStorageToDefault(e);
            throw;
        }
    }

    [HttpGet("storage/cdn")]
    public List<StorageDto> GetAllCdnStorages()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (!_coreBaseSettings.Standalone)
        {
            throw new SecurityException(Resource.ErrorAccessDenied);
        }

        var current = _settingsManager.Load<CdnStorageSettings>();
        var consumers = _consumerFactory.GetAll<DataStoreConsumer>().Where(r => r.Cdn != null);
        return consumers.Select(consumer => new StorageDto(consumer, current)).ToList();
    }

    [HttpPut("storage/cdn")]
    public CdnStorageSettings UpdateCdn(StorageRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (!_coreBaseSettings.Standalone)
        {
            throw new SecurityException(Resource.ErrorAccessDenied);
        }

        var consumer = _consumerFactory.GetByKey(inDto.Module);
        if (!consumer.IsSet)
        {
            throw new ArgumentException("module");
        }

        var settings = _settingsManager.Load<CdnStorageSettings>();
        if (settings.Module == inDto.Module)
        {
            return settings;
        }

        settings.Module = inDto.Module;
        settings.Props = inDto.Props.ToDictionary(r => r.Key, b => b.Value);

        try
        {
            _serviceClient.UploadCdn(Tenant.Id, "/", _webHostEnvironment.ContentRootPath, settings);
        }
        catch (Exception e)
        {
            _log.ErrorUpdateCdn(e);
            throw;
        }

        return settings;
    }

    [HttpDelete("storage/cdn")]
    public void ResetCdnToDefault()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (!_coreBaseSettings.Standalone)
        {
            throw new SecurityException(Resource.ErrorAccessDenied);
        }

        _storageSettingsHelper.Clear(_settingsManager.Load<CdnStorageSettings>());
    }

    [HttpGet("storage/backup")]
    public List<StorageDto> GetAllBackupStorages()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

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
        return consumers.Select(consumer => new StorageDto(consumer, current)).ToList();
    }

    private void StartMigrate(StorageSettings settings)
    {
        _serviceClient.Migrate(Tenant.Id, settings);

        Tenant.SetStatus(TenantStatus.Migrating);
        _tenantManager.SaveTenant(Tenant);
    }

    /// <summary>
    /// Returns a list of all Amazon regions.
    /// </summary>
    /// <category>Storage</category>
    /// <short>Get all Amazon regions</short>
    /// <returns>List of the Amazon regions</returns>
    [HttpGet("storage/s3/regions")]
    public object GetAmazonS3Regions()
    {
        return Amazon.RegionEndpoint.EnumerableAllRegions;
    }
}
