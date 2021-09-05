using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Api.Utils;
using ASC.Common;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Common.Configuration;
using ASC.Core.Users;
using ASC.Data.Backup.Contracts;
using ASC.Data.Backup.Service;
using ASC.MessagingSystem;
using ASC.Notify.Cron;
using ASC.Web.Core.PublicResources;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Backup;
using ASC.Web.Studio.Utility;

namespace ASC.Data.Backup
{
    [Scope]
    public class BackupAjaxHandler
    {
        private TenantManager TenantManager { get; }
        private MessageService MessageService { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private CoreConfiguration CoreConfiguration { get; }
        private PermissionContext PermissionContext { get; }
        private SecurityContext SecurityContext { get; }
        private UserManager UserManager { get; }
        private TenantExtra TenantExtra { get; }
        private ConsumerFactory ConsumerFactory { get; }
        private BackupFileUploadHandler BackupFileUploadHandler { get; }
        private BackupService BackupService { get; }

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
            BackupFileUploadHandler backupFileUploadHandler)
        {
            TenantManager = tenantManager;
            MessageService = messageService;
            CoreBaseSettings = coreBaseSettings;
            CoreConfiguration = coreConfiguration;
            PermissionContext = permissionContext;
            SecurityContext = securityContext;
            UserManager = userManager;
            TenantExtra = tenantExtra;
            ConsumerFactory = consumerFactory;
            BackupFileUploadHandler = backupFileUploadHandler;
            BackupService = backupService;
        }

        public void StartBackup(BackupStorageType storageType, Dictionary<string, string> storageParams, bool backupMail)
        {
            DemandPermissionsBackup();

            var backupRequest = new StartBackupRequest
            {
                TenantId = GetCurrentTenantId(),
                UserId = SecurityContext.CurrentAccount.ID,
                BackupMail = backupMail,
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
                    if (!CoreBaseSettings.Standalone) throw new Exception("Access denied");
                    backupRequest.StorageBasePath = storageParams["filePath"];
                    break;
            }

            MessageService.Send(MessageAction.StartBackupSetting);

            BackupService.StartBackup(backupRequest);
        }

        public BackupProgress GetBackupProgress()
        {
            DemandPermissionsBackup();

            return BackupService.GetBackupProgress(GetCurrentTenantId());
        }

        public BackupProgress GetBackupProgress(int tenantId)
        {
            DemandPermissionsBackup();

            return BackupService.GetBackupProgress(tenantId);
        }

        public void DeleteBackup(Guid id)
        {
            DemandPermissionsBackup();

            BackupService.DeleteBackup(id);
        }

        public void DeleteAllBackups()
        {
            DemandPermissionsBackup();

            BackupService.DeleteAllBackups(GetCurrentTenantId());
        }

        public List<BackupHistoryRecord> GetBackupHistory()
        {
            DemandPermissionsBackup();
            return BackupService.GetBackupHistory(GetCurrentTenantId());
        }

        public void CreateSchedule(BackupStorageType storageType, Dictionary<string, string> storageParams, int backupsStored, CronParams cronParams, bool backupMail)
        {
            DemandPermissionsBackup();

            if (!SetupInfo.IsVisibleSettings("AutoBackup"))
                throw new InvalidOperationException(Resource.ErrorNotAllowedOption);

            ValidateCronSettings(cronParams);

            var scheduleRequest = new CreateScheduleRequest
            {
                TenantId = TenantManager.GetCurrentTenant().TenantId,
                BackupMail = backupMail,
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
                    if (!CoreBaseSettings.Standalone) throw new Exception("Access denied");
                    scheduleRequest.StorageBasePath = storageParams["filePath"];
                    break;
            }

            BackupService.CreateSchedule(scheduleRequest);
        }

        public Schedule GetSchedule()
        {
            DemandPermissionsBackup();

            ScheduleResponse response;

            response = BackupService.GetSchedule(GetCurrentTenantId());
            if (response == null)
            {
                return null;
            }

            var schedule = new Schedule
            {
                StorageType = response.StorageType,
                StorageParams = response.StorageParams.ToDictionary(r => r.Key, r => r.Value) ?? new Dictionary<string, string>(),
                CronParams = new CronParams(response.Cron),
                BackupMail = response.BackupMail.NullIfDefault(),
                BackupsStored = response.NumberOfBackupsStored.NullIfDefault(),
                LastBackupTime = response.LastBackupTime
            };

            if (response.StorageType == BackupStorageType.CustomCloud)
            {
                var amazonSettings = CoreConfiguration.GetSection<AmazonS3Settings>();

                var consumer = ConsumerFactory.GetByKey<DataStoreConsumer>("S3");
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
                    TenantId = TenantManager.GetCurrentTenant().TenantId,
                    BackupMail = schedule.BackupMail != null && (bool)schedule.BackupMail,
                    Cron = schedule.CronParams.ToString(),
                    NumberOfBackupsStored = schedule.BackupsStored == null ? 0 : (int)schedule.BackupsStored,
                    StorageType = schedule.StorageType,
                    StorageParams = schedule.StorageParams
                };

                BackupService.CreateSchedule(Schedule);

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

            BackupService.DeleteSchedule(GetCurrentTenantId());

        }

        private void DemandPermissionsBackup()
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            if (!SetupInfo.IsVisibleSettings(ManagementType.Backup.ToString()))
                throw new BillingException(Resource.ErrorNotAllowedOption, "Backup");
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

                if (restoreRequest.StorageType == BackupStorageType.Local && !CoreBaseSettings.Standalone)
                {
                    restoreRequest.FilePathOrId = BackupFileUploadHandler.GetFilePath();
                }
            }

            BackupService.StartRestore(restoreRequest);
        }

        public BackupProgress GetRestoreProgress()
        {
            BackupProgress result;

            var tenant = TenantManager.GetCurrentTenant();
            result = BackupService.GetRestoreProgress(tenant.TenantId);

            return result;
        }

        private void DemandPermissionsRestore()
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            if (!SetupInfo.IsVisibleSettings("Restore") ||
                (!CoreBaseSettings.Standalone && !TenantManager.GetTenantQuota(TenantManager.GetCurrentTenant().TenantId).Restore))
                throw new BillingException(Resource.ErrorNotAllowedOption, "Restore");
        }

        #endregion

        #region transfer

        public void StartTransfer(string targetRegion, bool notifyUsers, bool transferMail)
        {
            DemandPermissionsTransfer();

            MessageService.Send(MessageAction.StartTransferSetting);
            BackupService.StartTransfer(
                new StartTransferRequest
                {
                    TenantId = GetCurrentTenantId(),
                    TargetRegion = targetRegion,
                    BackupMail = transferMail,
                    NotifyUsers = notifyUsers
                });

        }

        public BackupProgress GetTransferProgress()
        {
            return BackupService.GetTransferProgress(GetCurrentTenantId());
        }

        private void DemandPermissionsTransfer()
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var currentUser = UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            if (!SetupInfo.IsVisibleSettings(ManagementType.Migration.ToString())
                || !currentUser.IsOwner(TenantManager.GetCurrentTenant())
                || !SetupInfo.IsSecretEmail(currentUser.Email) && !TenantExtra.GetTenantQuota().HasMigration)
                throw new InvalidOperationException(Resource.ErrorNotAllowedOption);
        }

        #endregion

        public string GetTmpFolder()
        {
            return BackupService.GetTmpFolder();
        }

        private static void ValidateCronSettings(CronParams cronParams)
        {
            new CronExpression(cronParams.ToString());
        }

        private int GetCurrentTenantId()
        {
            return TenantManager.GetCurrentTenant().TenantId;
        }

        public class Schedule
        {
            public BackupStorageType StorageType { get; set; }
            public Dictionary<string, string> StorageParams { get; set; }
            public CronParams CronParams { get; set; }
            public bool? BackupMail { get; set; }
            public int? BackupsStored { get; set; }
            public DateTime LastBackupTime { get; set; }
        }

        public class CronParams
        {
            public BackupPeriod Period { get; set; }
            public int Hour { get; set; }
            public int Day { get; set; }

            public CronParams()
            {
            }

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
}
