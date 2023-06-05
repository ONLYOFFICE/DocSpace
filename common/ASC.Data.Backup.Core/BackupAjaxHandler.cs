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

namespace ASC.Data.Backup;

[Scope]
public class BackupAjaxHandler
{
    private readonly TenantManager _tenantManager;
    private readonly MessageService _messageService;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CoreConfiguration _coreConfiguration;
    private readonly PermissionContext _permissionContext;
    private readonly SecurityContext _securityContext;
    private readonly UserManager _userManager;
    private readonly ConsumerFactory _consumerFactory;
    private readonly BackupService _backupService;
    private readonly StorageFactory _storageFactory;

    private const string BackupTempModule = "backup_temp";
    private const string BackupFileName = "backup.tmp";

    #region backup

    public BackupAjaxHandler(
        BackupService backupService,
        TenantManager tenantManager,
        MessageService messageService,
        CoreBaseSettings coreBaseSettings,
        CoreConfiguration coreConfiguration,
        PermissionContext permissionContext,
        SecurityContext securityContext,
        UserManager userManager,
        ConsumerFactory consumerFactory,
        StorageFactory storageFactory)
    {
        _tenantManager = tenantManager;
        _messageService = messageService;
        _coreBaseSettings = coreBaseSettings;
        _coreConfiguration = coreConfiguration;
        _permissionContext = permissionContext;
        _securityContext = securityContext;
        _userManager = userManager;
        _consumerFactory = consumerFactory;
        _backupService = backupService;
        _storageFactory = storageFactory;
    }

    public async Task StartBackupAsync(BackupStorageType storageType, Dictionary<string, string> storageParams)
    {
        await DemandPermissionsBackupAsync();

        var backupRequest = new StartBackupRequest
        {
            TenantId = await GetCurrentTenantIdAsync(),
            UserId = _securityContext.CurrentAccount.ID,
            StorageType = storageType,
            StorageParams = storageParams
        };

        switch (storageType)
        {
            case BackupStorageType.ThridpartyDocuments:
            case BackupStorageType.Documents:
                backupRequest.StorageBasePath = storageParams["folderId"];
                break;
            case BackupStorageType.Local:
                if (!_coreBaseSettings.Standalone)
                {
                    throw new Exception("Access denied");
                }

                backupRequest.StorageBasePath = storageParams["filePath"];
                break;
        }

        await _messageService.SendAsync(MessageAction.StartBackupSetting);

        _backupService.StartBackup(backupRequest);
    }

    public async Task<BackupProgress> GetBackupProgressAsync()
    {
        await DemandPermissionsBackupAsync();

        return _backupService.GetBackupProgress(await GetCurrentTenantIdAsync());
    }

    public async Task<BackupProgress> GetBackupProgressAsync(int tenantId)
    {
        await DemandPermissionsBackupAsync();

        return _backupService.GetBackupProgress(tenantId);
    }

    public async Task DeleteBackupAsync(Guid id)
    {
        await DemandPermissionsBackupAsync();

        await _backupService.DeleteBackupAsync(id);
    }

    public async Task DeleteAllBackupsAsync()
    {
        await DemandPermissionsBackupAsync();

        await _backupService.DeleteAllBackupsAsync(await GetCurrentTenantIdAsync());
    }

    public async Task<List<BackupHistoryRecord>> GetBackupHistory()
    {
        await DemandPermissionsBackupAsync();

        return await _backupService.GetBackupHistoryAsync(await GetCurrentTenantIdAsync());
    }

    public async Task CreateScheduleAsync(BackupStorageType storageType, Dictionary<string, string> storageParams, int backupsStored, CronParams cronParams)
    {
        await DemandPermissionsBackupAsync();

        if (!SetupInfo.IsVisibleSettings("AutoBackup"))
        {
            throw new InvalidOperationException(Resource.ErrorNotAllowedOption);
        }

        ValidateCronSettings(cronParams);

        var scheduleRequest = new CreateScheduleRequest
        {
            TenantId = await _tenantManager.GetCurrentTenantIdAsync(),
            Cron = cronParams.ToString(),
            NumberOfBackupsStored = backupsStored,
            StorageType = storageType,
            StorageParams = storageParams
        };

        switch (storageType)
        {
            case BackupStorageType.ThridpartyDocuments:
            case BackupStorageType.Documents:
                scheduleRequest.StorageBasePath = storageParams["folderId"];
                break;
            case BackupStorageType.Local:
                if (!_coreBaseSettings.Standalone)
                {
                    throw new Exception("Access denied");
                }

                scheduleRequest.StorageBasePath = storageParams["filePath"];
                break;
        }

        await _backupService.CreateScheduleAsync(scheduleRequest);
    }

    public async Task<Schedule> GetScheduleAsync()
    {
        await DemandPermissionsBackupAsync();

        ScheduleResponse response;

        response = await _backupService.GetScheduleAsync(await GetCurrentTenantIdAsync());
        if (response == null)
        {
            return null;
        }

        var schedule = new Schedule
        {
            StorageType = response.StorageType,
            StorageParams = response.StorageParams.ToDictionary(r => r.Key, r => r.Value) ?? new Dictionary<string, string>(),
            CronParams = new CronParams(response.Cron),
            BackupsStored = response.NumberOfBackupsStored.NullIfDefault(),
            LastBackupTime = response.LastBackupTime
        };

        if (response.StorageType == BackupStorageType.CustomCloud)
        {
            var amazonSettings = await _coreConfiguration.GetSectionAsync<AmazonS3Settings>();

            var consumer = _consumerFactory.GetByKey<DataStoreConsumer>("s3");
            if (!consumer.IsSet)
            {
                consumer["acesskey"] = amazonSettings.AccessKeyId;
                consumer["secretaccesskey"] = amazonSettings.SecretAccessKey;

                consumer["bucket"] = amazonSettings.Bucket;
                consumer["region"] = amazonSettings.Region;
            }

            schedule.StorageType = BackupStorageType.ThirdPartyConsumer;
            schedule.StorageParams = consumer.AdditionalKeys.ToDictionary(r => r, r => consumer[r]);
            schedule.StorageParams.Add("module", "S3");

            var Schedule = new CreateScheduleRequest
            {
                TenantId = await _tenantManager.GetCurrentTenantIdAsync(),
                Cron = schedule.CronParams.ToString(),
                NumberOfBackupsStored = schedule.BackupsStored == null ? 0 : (int)schedule.BackupsStored,
                StorageType = schedule.StorageType,
                StorageParams = schedule.StorageParams
            };

            await _backupService.CreateScheduleAsync(Schedule);

        }
        else if (response.StorageType != BackupStorageType.ThirdPartyConsumer)
        {
            schedule.StorageParams["folderId"] = response.StorageBasePath;
        }

        return schedule;
    }

    public async Task DeleteScheduleAsync()
    {
        await DemandPermissionsBackupAsync();

        await _backupService.DeleteScheduleAsync(await GetCurrentTenantIdAsync());
    }

    private async Task DemandPermissionsBackupAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        if (!_coreBaseSettings.Standalone && !SetupInfo.IsVisibleSettings(nameof(ManagementType.Backup)))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Backup");
        }
    }

    #endregion

    #region restore

    public async Task StartRestoreAsync(string backupId, BackupStorageType storageType, Dictionary<string, string> storageParams, bool notify)
    {
        await DemandPermissionsRestoreAsync();

        var restoreRequest = new StartRestoreRequest
        {
            TenantId = await GetCurrentTenantIdAsync(),
            NotifyAfterCompletion = notify,
            StorageParams = storageParams
        };

        if (Guid.TryParse(backupId, out var guidBackupId))
        {
            restoreRequest.BackupId = guidBackupId;
        }
        else
        {
            restoreRequest.StorageType = storageType;
            restoreRequest.FilePathOrId = storageParams["filePath"];

            if (restoreRequest.StorageType == BackupStorageType.Local && !_coreBaseSettings.Standalone)
            {
                restoreRequest.FilePathOrId = await GetTmpFilePathAsync();
            }
        }

        await _backupService.StartRestoreAsync(restoreRequest);
    }

    public async Task<BackupProgress> GetRestoreProgressAsync()
    {
        BackupProgress result;

        var tenant = await _tenantManager.GetCurrentTenantAsync();
        result = _backupService.GetRestoreProgress(tenant.Id);

        return result;
    }

    public async Task DemandPermissionsRestoreAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var quota = await _tenantManager.GetTenantQuotaAsync(await _tenantManager.GetCurrentTenantIdAsync());
        if (!SetupInfo.IsVisibleSettings("Restore") ||
            (!_coreBaseSettings.Standalone && !quota.AutoBackupRestore))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Restore");
        }


        if (!_coreBaseSettings.Standalone
            && (!SetupInfo.IsVisibleSettings("Restore")
                || !quota.AutoBackupRestore))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Restore");
        }
    }

    public async Task DemandPermissionsAutoBackupAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        if (!SetupInfo.IsVisibleSettings("AutoBackup") ||
            (!_coreBaseSettings.Standalone && !(await _tenantManager.GetTenantQuotaAsync(await _tenantManager.GetCurrentTenantIdAsync())).AutoBackupRestore))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "AutoBackup");
        }
    }

    #endregion

    #region transfer

    public async Task StartTransferAsync(string targetRegion, bool notifyUsers)
    {
        await DemandPermissionsTransferAsync();

        await _messageService.SendAsync(MessageAction.StartTransferSetting);
        _backupService.StartTransfer(
            new StartTransferRequest
            {
                TenantId = await GetCurrentTenantIdAsync(),
                TargetRegion = targetRegion,
                NotifyUsers = notifyUsers
            });

    }

    public async Task<BackupProgress> GetTransferProgressAsync()
    {
        return _backupService.GetTransferProgress(await GetCurrentTenantIdAsync());
    }

    private async Task DemandPermissionsTransferAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var currentUser = await _userManager.GetUsersAsync(_securityContext.CurrentAccount.ID);
        if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.Migration))
        || !currentUser.IsOwner(await _tenantManager.GetCurrentTenantAsync())
        || !SetupInfo.IsSecretEmail(currentUser.Email) && !(await _tenantManager.GetCurrentTenantQuotaAsync()).AutoBackupRestore)
        {
            throw new InvalidOperationException(Resource.ErrorNotAllowedOption);
        }
    }

    #endregion

    public string GetTmpFolder()
    {
        return _backupService.GetTmpFolder();
    }

    private static void ValidateCronSettings(CronParams cronParams)
    {
        new CronExpression(cronParams.ToString());
    }

    private async Task<int> GetCurrentTenantIdAsync()
    {
        return await _tenantManager.GetCurrentTenantIdAsync();
    }

    public async Task<string> GetTmpFilePathAsync()
    {
        var discStore = await _storageFactory.GetStorageAsync(await _tenantManager.GetCurrentTenantIdAsync(), BackupTempModule, (IQuotaController)null) as DiscDataStore;
        var folder = discStore.GetPhysicalPath("", "");

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        return Path.Combine(folder, BackupFileName);
    }

    public class Schedule
    {
        public BackupStorageType StorageType { get; set; }
        public Dictionary<string, string> StorageParams { get; set; }
        public CronParams CronParams { get; set; }
        public int? BackupsStored { get; set; }
        public DateTime LastBackupTime { get; set; }
    }

    public class CronParams
    {
        public BackupPeriod Period { get; set; }
        public int Hour { get; set; }
        public int Day { get; set; }

        public CronParams() { }

        public CronParams(string cronString)
        {
            var tokens = cronString.Split(' ');
            Hour = Convert.ToInt32(tokens[2]);
            if (tokens[3] != "?")
            {
                Period = BackupPeriod.EveryMonth;
                Day = Convert.ToInt32(tokens[3]);
            }
            else if (tokens[5] != "*")
            {
                Period = BackupPeriod.EveryWeek;
                Day = Convert.ToInt32(tokens[5]);
            }
            else
            {
                Period = BackupPeriod.EveryDay;
            }
        }

        public override string ToString()
        {
            return Period switch
            {
                BackupPeriod.EveryDay => string.Format("0 0 {0} ? * *", Hour),
                BackupPeriod.EveryMonth => string.Format("0 0 {0} {1} * ?", Hour, Day),
                BackupPeriod.EveryWeek => string.Format("0 0 {0} ? * {1}", Hour, Day),
                _ => base.ToString(),
            };
        }
    }

    public enum BackupPeriod
    {
        EveryDay = 0,
        EveryWeek = 1,
        EveryMonth = 2
    }
}
