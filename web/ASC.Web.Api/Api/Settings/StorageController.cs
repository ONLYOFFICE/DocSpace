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

public class StorageController : BaseSettingsController, IDisposable
{
    private Tenant Tenant { get { return ApiContext.Tenant; } }

    private readonly MessageService _messageService;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ConsumerFactory _consumerFactory;
    private readonly TenantManager _tenantManager;
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
    private readonly SecurityContext _securityContext;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

    public StorageController(
        ILoggerProvider option,
        ServiceClient serviceClient,
        MessageService messageService,
        SecurityContext securityContext,
        StudioNotifyService studioNotifyService,
        ApiContext apiContext,
        TenantManager tenantManager,
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

    /// <summary>
    /// Returns a list of all the portal storages.
    /// </summary>
    /// <category>Storage</category>
    /// <short>Get storages</short>
    /// <returns type="ASC.Web.Api.ApiModel.ResponseDto.StorageDto, ASC.Web.Api">List of storages with the following parameters</returns>
    /// <path>api/2.0/settings/storage</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("storage")]
    public async Task<List<StorageDto>> GetAllStoragesAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        if (!_coreBaseSettings.Standalone)
        {
            throw new SecurityException(Resource.ErrorAccessDenied);
        }

        var current = await _settingsManager.LoadAsync<StorageSettings>();
        var consumers = _consumerFactory.GetAll<DataStoreConsumer>();
        return consumers.Select(consumer => new StorageDto(consumer, current)).ToList();
    }

    /// <summary>
    /// Returns the storage progress.
    /// </summary>
    /// <category>Storage</category>
    /// <short>Get the storage progress</short>
    /// <returns type="System.Double, System">Storage progress</returns>
    /// <path>api/2.0/settings/storage/progress</path>
    /// <httpMethod>GET</httpMethod>
    [AllowNotPayment]
    [HttpGet("storage/progress")]
    public async Task<double> GetStorageProgressAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        if (!_coreBaseSettings.Standalone)
        {
            return -1;
        }

        return _serviceClient.GetProgress(Tenant.Id);
    }

    /// <summary>
    /// Starts the storage encryption process.
    /// </summary>
    /// <short>Start the storage encryption process</short>
    /// <category>Encryption</category>
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.StorageEncryptionRequestsDto, ASC.Web.Api" name="inDto">Storage encryption request parameters</param>
    /// <returns type="System.Boolean, System">Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/settings/encryption/start</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("encryption/start")]
    public async Task<bool> StartStorageEncryptionAsync(StorageEncryptionRequestsDto inDto)
    {
        if (_coreBaseSettings.CustomMode)
        {
            return false;
        }

        try
        {
            await _semaphore.WaitAsync();

            var activeTenants = await _tenantManager.GetTenantsAsync();

            if (activeTenants.Count > 0)
            {
                await StartEncryptionAsync(inDto.NotifyUsers);
            }
        }
        catch
        {
            throw;
        }
        finally
        {
            _semaphore.Release();
        }

        return true;
    }

    private async Task StartEncryptionAsync(bool notifyUsers)
    {
        if (!SetupInfo.IsVisibleSettings<EncryptionSettings>())
        {
            throw new NotSupportedException();
        }

        if (!_coreBaseSettings.Standalone)
        {
            throw new SecurityException(Resource.ErrorAccessDenied);
        }

        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var storages = await GetAllStoragesAsync();

        if (storages.Any(s => s.Current))
        {
            throw new NotSupportedException();
        }

        var cdnStorages = await GetAllCdnStoragesAsync();

        if (cdnStorages.Any(s => s.Current))
        {
            throw new NotSupportedException();
        }

        var tenants = await _tenantManager.GetTenantsAsync();

        foreach (var tenant in tenants)
        {
            var progress = await _backupAjaxHandler.GetBackupProgressAsync(tenant.Id);
            if (progress != null && !progress.IsCompleted)
            {
                throw new Exception();
            }
        }

        foreach (var tenant in tenants)
        {
            _cacheDeleteSchedule.Publish(new DeleteSchedule() { TenantId = tenant.Id }, CacheNotifyAction.Insert);
        }

        var settings = await _encryptionSettingsHelper.LoadAsync();

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

        await _messageService.SendAsync(settings.Status == EncryprtionStatus.EncryptionStarted ? MessageAction.StartStorageEncryption : MessageAction.StartStorageDecryption);

        var serverRootPath = _commonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/');

        foreach (var tenant in tenants)
        {
            _tenantManager.SetCurrentTenant(tenant);

            if (notifyUsers)
            {
                if (settings.Status == EncryprtionStatus.EncryptionStarted)
                {
                    await _studioNotifyService.SendStorageEncryptionStartAsync(serverRootPath);
                }
                else
                {
                    await _studioNotifyService.SendStorageDecryptionStartAsync(serverRootPath);
                }
            }

            tenant.SetStatus(TenantStatus.Encryption);
            await _tenantManager.SaveTenantAsync(tenant);
        }

        await _encryptionSettingsHelper.SaveAsync(settings);

        _eventBus.Publish(new EncryptionDataStorageRequestedIntegrationEvent
        (
              encryptionSettings: new EncryptionSettings
              {
                  NotifyUsers = settings.NotifyUsers,
                  Password = settings.Password,
                  Status = settings.Status
              },
              serverRootPath: serverRootPath,
              createBy: _securityContext.CurrentAccount.ID,
              tenantId: await _tenantManager.GetCurrentTenantIdAsync()

        ));
    }

    /// <summary>
    /// Returns the storage encryption settings.
    /// </summary>
    /// <short>Get the storage encryption settings</short>
    /// <category>Encryption</category>
    /// <returns type="ASC.Core.Encryption.EncryptionSettings, ASC.Core.Encryption">Storage encryption settings</returns>
    /// <path>api/2.0/settings/encryption/settings</path>
    /// <httpMethod>GET</httpMethod>
    /// <visible>false</visible>
    [HttpGet("encryption/settings")]
    public async Task<EncryptionSettings> GetStorageEncryptionSettingsAsync()
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

            await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

            var settings = await _encryptionSettingsHelper.LoadAsync();

            settings.Password = string.Empty; // Don't show password

            return settings;
        }
        catch (Exception e)
        {
            _log.ErrorGetStorageEncryptionSettings(e);
            return null;
        }
    }

    /// <summary>
    /// Returns the storage encryption progress.
    /// </summary>
    /// <short>Get the storage encryption progress</short>
    /// <category>Encryption</category>
    /// <returns type="System.Nullable{System.Double}, System">Storage encryption progress</returns>
    /// <path>api/2.0/settings/encryption/progress</path>
    /// <httpMethod>GET</httpMethod>
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

    /// <summary>
    /// Updates a storage with the parameters specified in the request.
    /// </summary>
    /// <category>Storage</category>
    /// <short>Update a storage</short>
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.StorageRequestsDto, ASC.Web.Api" name="inDto">Storage settings request parameters</param>
    /// <returns type="ASC.Data.Storage.Configuration.StorageSettings, ASC.Data.Storage">Updated storage settings</returns>
    /// <path>api/2.0/settings/storage</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("storage")]
    public async Task<StorageSettings> UpdateStorageAsync(StorageRequestsDto inDto)
    {
        try
        {
            await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

            if (!_coreBaseSettings.Standalone)
            {
                throw new SecurityException(Resource.ErrorAccessDenied);
            }

            var consumer = _consumerFactory.GetByKey(inDto.Module);
            if (!consumer.IsSet)
            {
                throw new ArgumentException("module");
            }

            var settings = await _settingsManager.LoadAsync<StorageSettings>();
            if (settings.Module == inDto.Module)
            {
                return settings;
            }

            settings.Module = inDto.Module;
            settings.Props = inDto.Props.ToDictionary(r => r.Key, b => b.Value);

            await StartMigrateAsync(settings);
            return settings;
        }
        catch (Exception e)
        {
            _log.ErrorUpdateStorage(e);
            throw;
        }
    }

    /// <summary>
    /// Resets the storage settings to the default parameters.
    /// </summary>
    /// <category>Storage</category>
    /// <short>Reset the storage settings</short>
    /// <path>api/2.0/settings/storage</path>
    /// <httpMethod>DELETE</httpMethod>
    /// <returns></returns>
    [HttpDelete("storage")]
    public async Task ResetStorageToDefaultAsync()
    {
        try
        {
            await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

            if (!_coreBaseSettings.Standalone)
            {
                throw new SecurityException(Resource.ErrorAccessDenied);
            }

            var settings = await _settingsManager.LoadAsync<StorageSettings>();

            settings.Module = null;
            settings.Props = null;


            await StartMigrateAsync(settings);
        }
        catch (Exception e)
        {
            _log.ErrorResetStorageToDefault(e);
            throw;
        }
    }

    /// <summary>
    /// Returns a list of all the CDN storages.
    /// </summary>
    /// <category>Storage</category>
    /// <short>Get the CDN storages</short>
    /// <returns type="ASC.Web.Api.ApiModel.ResponseDto.StorageDto, ASC.Web.Api">List of the CDN storages with the following parameters</returns>
    /// <path>api/2.0/settings/storage/cdn</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("storage/cdn")]
    public async Task<List<StorageDto>> GetAllCdnStoragesAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        if (!_coreBaseSettings.Standalone)
        {
            throw new SecurityException(Resource.ErrorAccessDenied);
        }

        var current = await _settingsManager.LoadAsync<CdnStorageSettings>();
        var consumers = _consumerFactory.GetAll<DataStoreConsumer>().Where(r => r.Cdn != null);
        return consumers.Select(consumer => new StorageDto(consumer, current)).ToList();
    }

    /// <summary>
    /// Updates the CDN storage with the parameters specified in the request.
    /// </summary>
    /// <category>Storage</category>
    /// <short>Update the CDN storage</short>
    /// <returns type="ASC.Data.Storage.Configuration.CdnStorageSettings, ASC.Data.Storage">Updated CDN storage</returns>
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.StorageRequestsDto, ASC.Web.Api" name="inDto">CDN storage settings request parameters</param>
    /// <path>api/2.0/settings/storage/cdn</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("storage/cdn")]
    public async Task<CdnStorageSettings> UpdateCdnAsync(StorageRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        if (!_coreBaseSettings.Standalone)
        {
            throw new SecurityException(Resource.ErrorAccessDenied);
        }

        var consumer = _consumerFactory.GetByKey(inDto.Module);
        if (!consumer.IsSet)
        {
            throw new ArgumentException("module");
        }

        var settings = await _settingsManager.LoadAsync<CdnStorageSettings>();
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

    /// <summary>
    /// Resets the CDN storage settings to the default parameters.
    /// </summary>
    /// <category>Storage</category>
    /// <short>Reset the CDN storage settings</short>
    /// <path>api/2.0/settings/storage/cdn</path>
    /// <httpMethod>DELETE</httpMethod>
    /// <returns></returns>
    [HttpDelete("storage/cdn")]
    public async Task ResetCdnToDefaultAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        if (!_coreBaseSettings.Standalone)
        {
            throw new SecurityException(Resource.ErrorAccessDenied);
        }

        await _storageSettingsHelper.ClearAsync(await _settingsManager.LoadAsync<CdnStorageSettings>());
    }

    /// <summary>
    /// Returns a list of all the backup storages.
    /// </summary>
    /// <category>Storage</category>
    /// <short>Get the backup storages</short>
    /// <returns type="ASC.Web.Api.ApiModel.ResponseDto.StorageDto, ASC.Web.Api">List of the backup storages with the following parameters</returns>
    /// <path>api/2.0/settings/storage/backup</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("storage/backup")]
    public async Task<List<StorageDto>> GetAllBackupStorages()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var schedule = await _backupAjaxHandler.GetScheduleAsync();
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

    private async Task StartMigrateAsync(StorageSettings settings)
    {
        _serviceClient.Migrate(Tenant.Id, settings);

        Tenant.SetStatus(TenantStatus.Migrating);
        await _tenantManager.SaveTenantAsync(Tenant);
    }

    /// <summary>
    /// Returns a list of all Amazon regions.
    /// </summary>
    /// <category>Storage</category>
    /// <short>Get Amazon regions</short>
    /// <returns type="System.Object, System">List of the Amazon regions</returns>
    /// <path>api/2.0/settings/storage/s3/regions</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("storage/s3/regions")]
    public object GetAmazonS3Regions()
    {
        return Amazon.RegionEndpoint.EnumerableAllRegions;
    }

    public void Dispose()
    {
        _semaphore.Dispose();
    }
}
