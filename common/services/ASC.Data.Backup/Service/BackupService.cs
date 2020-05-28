/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Contracts;
using ASC.Core.Tenants;
using ASC.Data.Backup.EF.Model;
using ASC.Data.Backup.Storage;
using ASC.Data.Backup.Utils;
using Microsoft.Extensions.Options;

namespace ASC.Data.Backup.Service
{
    public class BackupService : IBackupService
    {
        private readonly ILog log;
        private readonly BackupStorageFactory backupStorageFactory;
        private readonly BackupWorker backupWorker;
        private readonly TenantManager tenantManager;
        private readonly TenantUtil tenantUtil;
        private readonly IOptionsMonitor<ILog> options;
        private readonly BackupHelper backupHelper;
        public BackupService(IOptionsMonitor<ILog> options, BackupStorageFactory backupStorageFactory, BackupWorker backupWorker, TenantManager tenantManager, TenantUtil tenantUtil, BackupHelper backupHelper)
        {
            log = options.CurrentValue;
            this.backupStorageFactory = backupStorageFactory;
            this.backupWorker = backupWorker;
            this.tenantManager = tenantManager;
            this.tenantUtil = tenantUtil;
            this.options = options;
            this.backupHelper = backupHelper;
        }

        public BackupProgress StartBackup(StartBackupRequest request)
        {
            var progress = backupWorker.StartBackup(request);
            if (!string.IsNullOrEmpty(progress.Error))
            {
                throw new FaultException();
            }
            return progress;
        }

        public BackupProgress GetBackupProgress(int tenantId)
        {
            var progress = backupWorker.GetBackupProgress(tenantId);
            if (progress != null && !string.IsNullOrEmpty(progress.Error))
            {
                backupWorker.ResetBackupError(tenantId);
                throw new FaultException();
                
            }
            return progress;
        }

        public void DeleteBackup(Guid id)
        {
            var backupRepository = backupStorageFactory.GetBackupRepository();
            var backupRecord = backupRepository.GetBackupRecord(id);
            backupRepository.DeleteBackupRecord(backupRecord.Id);

            var storage = backupStorageFactory.GetBackupStorage(backupRecord);
            if (storage == null) return;
            storage.Delete(backupRecord.StoragePath);
        }

        public void DeleteAllBackups(int tenantId)
        {
            var backupRepository = backupStorageFactory.GetBackupRepository();
            foreach (var backupRecord in backupRepository.GetBackupRecordsByTenantId(tenantId))
            {
                try
                {
                    backupRepository.DeleteBackupRecord(backupRecord.Id);
                    var storage = backupStorageFactory.GetBackupStorage(backupRecord);
                    if (storage == null) continue;
                    storage.Delete(backupRecord.StoragePath);
                }
                catch (Exception error)
                {
                    log.Warn("error while removing backup record: {0}", error);
                }
            }
        }

        public List<BackupHistoryRecord> GetBackupHistory(int tenantId)
        {
            var backupHistory = new List<BackupHistoryRecord>();
            var backupRepository = backupStorageFactory.GetBackupRepository();
            foreach (var record in backupRepository.GetBackupRecordsByTenantId(tenantId))
            {
                var storage = backupStorageFactory.GetBackupStorage(record);
                if (storage == null) continue;
                if (storage.IsExists(record.StoragePath))
                {
                    backupHistory.Add(new BackupHistoryRecord
                        {
                            Id = record.Id,
                            FileName = record.Name,
                            StorageType = record.StorageType,
                            CreatedOn = record.CreatedOn,
                            ExpiresOn = record.ExpiresOn
                        });
                }
                else
                {
                    backupRepository.DeleteBackupRecord(record.Id);
                }
            }
            return backupHistory;
        }

        public BackupProgress StartTransfer(StartTransferRequest request)
        {
            var progress = backupWorker.StartTransfer(request.TenantId, request.TargetRegion, request.BackupMail, request.NotifyUsers);
            if (!string.IsNullOrEmpty(progress.Error))
            {
                throw new FaultException();
            }
            return progress;
        }

        public BackupProgress GetTransferProgress(int tenantID)
        {
            var progress = backupWorker.GetTransferProgress(tenantID);
            if (!string.IsNullOrEmpty(progress.Error))
            {
                throw new FaultException();
            }
            return progress;
        }

        public BackupProgress StartRestore(StartRestoreRequest request)
        {
            if (request.StorageType == BackupStorageType.Local)
            {
                if (string.IsNullOrEmpty(request.FilePathOrId) || !File.Exists(request.FilePathOrId))
                {
                    throw new FileNotFoundException();
                }
            }

            if (!request.BackupId.Equals(Guid.Empty))
            {
                var backupRepository = backupStorageFactory.GetBackupRepository();
                var backupRecord = backupRepository.GetBackupRecord(request.BackupId);
                if (backupRecord == null)
                {
                    throw new FileNotFoundException();
                }

                request.FilePathOrId = backupRecord.StoragePath;
                request.StorageType = backupRecord.StorageType;
                request.StorageParams = backupRecord.StorageParams;
            }

            var progress = backupWorker.StartRestore(request);
            if (!string.IsNullOrEmpty(progress.Error))
            {
                throw new FaultException();
            }
            return progress;
        }

        public BackupProgress GetRestoreProgress(int tenantId)
        {
            var progress = backupWorker.GetRestoreProgress(tenantId);
            if (progress != null && !string.IsNullOrEmpty(progress.Error))
            {
                backupWorker.ResetRestoreError(tenantId);
                throw new FaultException();
            }
            return progress;
        }

        public string GetTmpFolder()
        {
            return backupWorker.TempFolder;
        }

        public List<TransferRegion> GetTransferRegions()
        {
            var webConfigs = BackupConfigurationSection.GetSection().WebConfigs;
            return webConfigs
                .Cast<WebConfigElement>()
                .Select(configElement =>
                    {
                        var config = ConfigurationProvider.Open(PathHelper.ToRootedConfigPath(configElement.Path));
                        var baseDomain = config.AppSettings.Settings["core.base-domain"].Value;
                        return new TransferRegion
                            {
                                Name = configElement.Region,
                                BaseDomain = baseDomain,
                                IsCurrentRegion = configElement.Region.Equals(webConfigs.CurrentRegion, StringComparison.InvariantCultureIgnoreCase)
                            };
                    })
                .ToList();
        }

        public void CreateSchedule(CreateScheduleRequest request)
        {
            backupStorageFactory.GetBackupRepository().SaveBackupSchedule(
                new Schedule(options, request.TenantId, tenantManager, tenantUtil, backupHelper)
                    {
                        Cron = request.Cron,
                        BackupMail = request.BackupMail,
                        BackupsStored = request.NumberOfBackupsStored,
                        StorageType = request.StorageType,
                        StorageBasePath = request.StorageBasePath,
                        StorageParams = request.StorageParams
                    });
        }

        public void DeleteSchedule(int tenantId)
        {
            backupStorageFactory.GetBackupRepository().DeleteBackupSchedule(tenantId);
        }

        public ScheduleResponse GetSchedule(int tenantId)
        {
            var schedule = backupStorageFactory.GetBackupRepository().GetBackupSchedule(tenantId);
            return schedule != null
                       ? new ScheduleResponse
                           {
                               StorageType = schedule.StorageType,
                               StorageBasePath = schedule.StorageBasePath,
                               BackupMail = schedule.BackupMail,
                               NumberOfBackupsStored = schedule.BackupsStored,
                               Cron = schedule.Cron,
                               LastBackupTime = schedule.LastBackupTime,
                               StorageParams = schedule.StorageParams
                           }
                       : null;
        }
    }
    public static class BackupServiceExtension
    {
        public static DIHelper AddBackupService(this DIHelper services)
        {
            services.TryAddScoped<BackupService>();
            return services;
        }
    }
}
