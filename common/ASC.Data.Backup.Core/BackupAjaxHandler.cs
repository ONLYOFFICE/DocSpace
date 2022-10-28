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
    private readonly TenantExtra _tenantExtra;
    private readonly ConsumerFactory _consumerFactory;
    private readonly BackupService _backupService;
    private readonly TempPath _tempPath;
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
        TenantExtra tenantExtra,
        ConsumerFactory consumerFactory,
        TempPath tempPath,
        StorageFactory storageFactory)
    {
        _tenantManager = tenantManager;
        _messageService = messageService;
        _coreBaseSettings = coreBaseSettings;
        _coreConfiguration = coreConfiguration;
        _permissionContext = permissionContext;
        _securityContext = securityContext;
        _userManager = userManager;
        _tenantExtra = tenantExtra;
        _consumerFactory = consumerFactory;
        _backupService = backupService;
        _tempPath = tempPath;
        _storageFactory = storageFactory;
    }

    public void StartBackup(BackupStorageType storageType, Dictionary<string, string> storageParams)
    {
        DemandPermissionsBackup();

        var backupRequest = new StartBackupRequest
        {
            TenantId = GetCurrentTenantId(),
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

        _messageService.Send(MessageAction.StartBackupSetting);

        _backupService.StartBackup(backupRequest);
    }

    public BackupProgress GetBackupProgress()
    {
        DemandPermissionsBackup();

        return _backupService.GetBackupProgress(GetCurrentTenantId());
    }

    public BackupProgress GetBackupProgress(int tenantId)
    {
        DemandPermissionsBackup();

        return _backupService.GetBackupProgress(tenantId);
    }

    public void DeleteBackup(Guid id)
    {
        DemandPermissionsBackup();

        _backupService.DeleteBackup(id);
    }

    public void DeleteAllBackups()
    {
        DemandPermissionsBackup();

        _backupService.DeleteAllBackups(GetCurrentTenantId());
    }

    public List<BackupHistoryRecord> GetBackupHistory()
    {
        DemandPermissionsBackup();

        return _backupService.GetBackupHistory(GetCurrentTenantId());
    }

    public void CreateSchedule(BackupStorageType storageType, Dictionary<string, string> storageParams, int backupsStored, CronParams cronParams)
    {
        DemandPermissionsBackup();

        if (!SetupInfo.IsVisibleSettings("AutoBackup"))
        {
            throw new InvalidOperationException(Resource.ErrorNotAllowedOption);
        }

        ValidateCronSettings(cronParams);

        var scheduleRequest = new CreateScheduleRequest
        {
            TenantId = _tenantManager.GetCurrentTenant().Id,
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

        _backupService.CreateSchedule(scheduleRequest);
    }

    public Schedule GetSchedule()
    {
        DemandPermissionsBackup();

        ScheduleResponse response;

        response = _backupService.GetSchedule(GetCurrentTenantId());
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
            var amazonSettings = _coreConfiguration.GetSection<AmazonS3Settings>();

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
                TenantId = _tenantManager.GetCurrentTenant().Id,
                Cron = schedule.CronParams.ToString(),
                NumberOfBackupsStored = schedule.BackupsStored == null ? 0 : (int)schedule.BackupsStored,
                StorageType = schedule.StorageType,
                StorageParams = schedule.StorageParams
            };

            _backupService.CreateSchedule(Schedule);

        }
        else if (response.StorageType != BackupStorageType.ThirdPartyConsumer)
        {
            schedule.StorageParams["folderId"] = response.StorageBasePath;
        }

        return schedule;
    }

    public void DeleteSchedule()
    {
        DemandPermissionsBackup();

        _backupService.DeleteSchedule(GetCurrentTenantId());

    }

    private void DemandPermissionsBackup()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (!_coreBaseSettings.Standalone && !SetupInfo.IsVisibleSettings(nameof(ManagementType.Backup)))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Backup");
        }
    }

    #endregion

    #region restore

    public void StartRestore(string backupId, BackupStorageType storageType, Dictionary<string, string> storageParams, bool notify)
    {
        DemandPermissionsRestore();

        var restoreRequest = new StartRestoreRequest
        {
            TenantId = GetCurrentTenantId(),
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
                restoreRequest.FilePathOrId = GetTmpFilePath();
            }
        }

        _backupService.StartRestore(restoreRequest);
    }

    public BackupProgress GetRestoreProgress()
    {
        BackupProgress result;

        var tenant = _tenantManager.GetCurrentTenant();
        result = _backupService.GetRestoreProgress(tenant.Id);

        return result;
    }

    public void DemandPermissionsRestore()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (!SetupInfo.IsVisibleSettings("Restore") ||
            (!_coreBaseSettings.Standalone && !_tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().Id).AutoBackupRestore))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Restore");
        }


        if (!_coreBaseSettings.Standalone
            && (!SetupInfo.IsVisibleSettings("Restore")
                || !_tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().Id).AutoBackupRestore))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "Restore");
        }
    }

    public void DemandPermissionsAutoBackup()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (!SetupInfo.IsVisibleSettings("AutoBackup") ||
            (!_coreBaseSettings.Standalone && !_tenantManager.GetTenantQuota(_tenantManager.GetCurrentTenant().Id).AutoBackupRestore))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "AutoBackup");
        }
    }

    #endregion

    #region transfer

    public void StartTransfer(string targetRegion, bool notifyUsers)
    {
        DemandPermissionsTransfer();

        _messageService.Send(MessageAction.StartTransferSetting);
        _backupService.StartTransfer(
            new StartTransferRequest
            {
                TenantId = GetCurrentTenantId(),
                TargetRegion = targetRegion,
                NotifyUsers = notifyUsers
            });

    }

    public BackupProgress GetTransferProgress()
    {
        return _backupService.GetTransferProgress(GetCurrentTenantId());
    }

    private void DemandPermissionsTransfer()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var currentUser = _userManager.GetUsers(_securityContext.CurrentAccount.ID);
        if (!SetupInfo.IsVisibleSettings(nameof(ManagementType.Migration))
        || !currentUser.IsOwner(_tenantManager.GetCurrentTenant())
        || !SetupInfo.IsSecretEmail(currentUser.Email) && !_tenantManager.GetCurrentTenantQuota().AutoBackupRestore)
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

    private int GetCurrentTenantId()
    {
        return _tenantManager.GetCurrentTenant().Id;
    }

    public string GetTmpFilePath()
    {
        var discStore = _storageFactory.GetStorage("", _tenantManager.GetCurrentTenant().Id.ToString(), BackupTempModule) as DiscDataStore;
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
