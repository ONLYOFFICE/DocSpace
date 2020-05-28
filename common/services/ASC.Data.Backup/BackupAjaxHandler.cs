using System;
using System.Collections.Generic;
using System.Linq;
using AjaxPro;
using ASC.Common;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Common.Configuration;
using ASC.Core.Common.Contracts;
using ASC.Core.Users;
using ASC.MessagingSystem;
using ASC.Notify.Cron;
using ASC.Web.Core.PublicResources;
using ASC.Web.Core.Security;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;

namespace ASC.Data.Backup
{
    [AjaxNamespace("AjaxPro.Backup")]
    public class BackupAjaxHandler
    {

        private TenantManager tenantManager;
        private MessageService messageService;
        private CoreBaseSettings coreBaseSettings;
        private CoreConfiguration coreConfiguration;
        private PermissionContext  permissionContext;
        private SecurityContext securityContext;
        private UserManager userManager;
        private TenantExtra tenantExtra;
        private BackupHelper backupHelper;
        private ConsumerFactory consumerFactory;
        #region backup

        public BackupAjaxHandler(TenantManager tenantManager, MessageService messageService, CoreBaseSettings coreBaseSettings, CoreConfiguration coreConfiguration, PermissionContext permissionContext, SecurityContext securityContext, UserManager userManager, TenantExtra tenantExtra, BackupHelper backupHelper, ConsumerFactory consumerFactory)
        {
            this.tenantManager = tenantManager;
            this.messageService = messageService;
            this.coreBaseSettings = coreBaseSettings;
            this.coreConfiguration = coreConfiguration;
            this.permissionContext = permissionContext;
            this.securityContext = securityContext;
            this.userManager = userManager;
            this.tenantExtra = tenantExtra;
            this.backupHelper = backupHelper;
            this.consumerFactory = consumerFactory;
        }

        [AjaxMethod]
        public BackupProgress StartBackup(BackupStorageType storageType, Dictionary<string, string> storageParams, bool backupMail)
        {

            DemandPermissionsBackup();
            DemandSize();

            var backupRequest = new StartBackupRequest
            {
                TenantId = GetCurrentTenantId(),
                UserId = securityContext.CurrentAccount.ID,
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
                    if (!coreBaseSettings.Standalone) throw new Exception("Access denied");
                    backupRequest.StorageBasePath = storageParams["filePath"];
                    break;
            }

            messageService.Send(MessageAction.StartBackupSetting);

            using (var service = new BackupServiceClient())
            {
                return service.StartBackup(backupRequest);
            }
        }

        [AjaxMethod]
        [SecurityPassthrough]
        public BackupProgress GetBackupProgress()
        {
            DemandPermissionsBackup();

            using (var service = new BackupServiceClient())
            {
                return service.GetBackupProgress(GetCurrentTenantId());
            }
        }

        [AjaxMethod]
        public void DeleteBackup(Guid id)
        {
            DemandPermissionsBackup();

            using (var service = new BackupServiceClient())
            {
                service.DeleteBackup(id);
            }
        }

        [AjaxMethod]
        public void DeleteAllBackups()
        {
            DemandPermissionsBackup();

            using (var service = new BackupServiceClient())
            {
                service.DeleteAllBackups(GetCurrentTenantId());
            }
        }

        [AjaxMethod]
        public List<BackupHistoryRecord> GetBackupHistory()
        {
            DemandPermissionsBackup();

            using (var service = new BackupServiceClient())
            {
                return service.GetBackupHistory(GetCurrentTenantId());
            }
        }


        [AjaxMethod]
        public void CreateSchedule(BackupStorageType storageType, Dictionary<string, string> storageParams, int backupsStored, CronParams cronParams, bool backupMail)
        {
            DemandPermissionsBackup();
            DemandSize();

            if (!SetupInfo.IsVisibleSettings("AutoBackup"))
                throw new InvalidOperationException(Resource.ErrorNotAllowedOption);

            ValidateCronSettings(cronParams);

            var scheduleRequest = new CreateScheduleRequest
            {
                TenantId = tenantManager.GetCurrentTenant().TenantId,
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
                    if (!coreBaseSettings.Standalone) throw new Exception("Access denied");
                    scheduleRequest.StorageBasePath = storageParams["filePath"];
                    break;
            }

            using (var service = new BackupServiceClient())
            {
                service.CreateSchedule(scheduleRequest);
            }
        }

        [AjaxMethod]
        public Schedule GetSchedule()
        {
            DemandPermissionsBackup();

            ScheduleResponse response;
            using (var service = new BackupServiceClient())
            {
                response = service.GetSchedule(GetCurrentTenantId());
                if (response == null)
                {
                    return null;
                }
            }

            var schedule = new Schedule
            {
                StorageType = response.StorageType,
                StorageParams = response.StorageParams ?? new Dictionary<string, string>(),
                CronParams = new CronParams(response.Cron),
                BackupMail = response.BackupMail,
                BackupsStored = response.NumberOfBackupsStored,
                LastBackupTime = response.LastBackupTime
            };

            if (response.StorageType == BackupStorageType.CustomCloud)
            {
                var amazonSettings = coreConfiguration.GetSection<AmazonS3Settings>();

                var consumer = consumerFactory.GetByKey<DataStoreConsumer>("S3");//here
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

                using (var service = new BackupServiceClient())
                {
                    service.CreateSchedule(new CreateScheduleRequest
                    {
                        TenantId = tenantManager.GetCurrentTenant().TenantId,
                        BackupMail = schedule.BackupMail,
                        Cron = schedule.CronParams.ToString(),
                        NumberOfBackupsStored = schedule.BackupsStored,
                        StorageType = schedule.StorageType,
                        StorageParams = schedule.StorageParams
                    });
                }

            }
            else if (response.StorageType != BackupStorageType.ThirdPartyConsumer)
            {
                schedule.StorageParams["folderId"] = response.StorageBasePath;
            }

            return schedule;
        }

        [AjaxMethod]
        public void DeleteSchedule()
        {
            DemandPermissionsBackup();

            using (var service = new BackupServiceClient())
            {
                service.DeleteSchedule(GetCurrentTenantId());
            }
        }

        private void DemandPermissionsBackup()
        {
            permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            if (!SetupInfo.IsVisibleSettings(ManagementType.Backup.ToString()))
                throw new BillingException(Resource.ErrorNotAllowedOption, "Backup");
        }

        #endregion

        #region restore

        [AjaxMethod]
        public BackupProgress StartRestore(string backupId, BackupStorageType storageType, Dictionary<string, string> storageParams, bool notify)
        {
            DemandPermissionsRestore();

            var restoreRequest = new StartRestoreRequest
            {
                TenantId = GetCurrentTenantId(),
                NotifyAfterCompletion = notify,
                StorageParams = storageParams
            };

            Guid guidBackupId;
            if (Guid.TryParse(backupId, out guidBackupId))
            {
                restoreRequest.BackupId = guidBackupId;
            }
            else
            {
                restoreRequest.StorageType = storageType;
                restoreRequest.FilePathOrId = storageParams["filePath"];
            }

            using (var service = new BackupServiceClient())
            {
                return service.StartRestore(restoreRequest);
            }
        }

        [AjaxMethod]
        [SecurityPassthrough]
        public BackupProgress GetRestoreProgress()
        {
            BackupProgress result;

            var tenant = tenantManager.GetCurrentTenant();
            using (var service = new BackupServiceClient())
            {
                result = service.GetRestoreProgress(tenant.TenantId);
            }

            return result;
        }

        private void DemandPermissionsRestore()
        {
            permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            if (!SetupInfo.IsVisibleSettings("Restore"))
                throw new BillingException(Resource.ErrorNotAllowedOption, "Restore");
        }

        #endregion

        #region transfer

        [AjaxMethod]
        public BackupProgress StartTransfer(string targetRegion, bool notifyUsers, bool transferMail)
        {
            DemandPermissionsTransfer();
            DemandSize();

            messageService.Send(MessageAction.StartTransferSetting);

            using (var service = new BackupServiceClient())
            {
                return service.StartTransfer(
                    new StartTransferRequest
                    {
                        TenantId = GetCurrentTenantId(),
                        TargetRegion = targetRegion,
                        BackupMail = transferMail,
                        NotifyUsers = notifyUsers
                    });
            }
        }

        [AjaxMethod]
        [SecurityPassthrough]
        public BackupProgress GetTransferProgress()
        {
            using (var service = new BackupServiceClient())
            {
                return service.GetTransferProgress(GetCurrentTenantId());
            }
        }

        private void DemandPermissionsTransfer()
        {
            permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var currentUser = userManager.GetUsers(securityContext.CurrentAccount.ID);
            if (!SetupInfo.IsVisibleSettings(ManagementType.Migration.ToString())
                || !currentUser.IsOwner(tenantManager.CurrentTenant)
                || !SetupInfo.IsSecretEmail(currentUser.Email) && !tenantExtra.GetTenantQuota().HasMigration)
                throw new InvalidOperationException(Resource.ErrorNotAllowedOption);
        }

        #endregion

        public string GetTmpFolder()
        {
            using (var service = new BackupServiceClient())
            {
                return service.GetTmpFolder();
            }
        }

        private void DemandSize()
        {
            if (backupHelper.ExceedsMaxAvailableSize(tenantManager.CurrentTenant.TenantId))
                throw new InvalidOperationException(string.Format(UserControlsCommonResource.BackupSpaceExceed,
                    FileSizeComment.FilesSizeToString(BackupHelper.AvailableZipSize),
                    "",
                    ""));
        }

        private static void ValidateCronSettings(CronParams cronParams)
        {
            new CronExpression(cronParams.ToString());
        }

        private int GetCurrentTenantId()
        {
            return tenantManager.GetCurrentTenant().TenantId;
        }

        public class Schedule
        {
            public BackupStorageType StorageType { get; set; }
            public Dictionary<string, string> StorageParams { get; set; }
            public CronParams CronParams { get; set; }
            public bool BackupMail { get; set; }
            public int BackupsStored { get; set; }
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
                switch (Period)
                {
                    case BackupPeriod.EveryDay:
                        return string.Format("0 0 {0} ? * *", Hour);
                    case BackupPeriod.EveryMonth:
                        return string.Format("0 0 {0} {1} * ?", Hour, Day);
                    case BackupPeriod.EveryWeek:
                        return string.Format("0 0 {0} ? * {1}", Hour, Day);
                    default:
                        return base.ToString();
                }
            }
        }

        public enum BackupPeriod
        {
            EveryDay = 0,
            EveryWeek = 1,
            EveryMonth = 2
        }
    }
    public static class BackupAjaxHandlerExtension
    {
        public static DIHelper AddBackupAjaxHandler(this DIHelper services)
        {
            services.TryAddScoped<BackupAjaxHandler>();
            return services
                .AddTenantManagerService()
                .AddCoreBaseSettingsService()
                .AddMessageServiceService()
                .AddCoreSettingsService()
                .AddPermissionContextService()
                .AddSecurityContextService()
                .AddUserManagerService()
                .AddTenantExtraService()
                .AddConsumerFactoryService();
        }
    }
}
