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
using ASC.Common.Utils;
using ASC.Data.Backup.Contracts;
using ASC.Data.Backup.EF.Model;
using ASC.Data.Backup.Storage;
using ASC.Data.Backup.Utils;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace ASC.Data.Backup.Service
{
    [Scope]
    public class BackupService : IBackupService
    {
        private ILog Log { get; set; }
        private BackupStorageFactory BackupStorageFactory { get; set; }
        private BackupWorker BackupWorker { get; set; }
        private BackupRepository BackupRepository { get; }
        private ConfigurationExtension Configuration { get; }

        public BackupService(
            IOptionsMonitor<ILog> options,
            BackupStorageFactory backupStorageFactory,
            BackupWorker backupWorker,
            BackupRepository backupRepository,
            ConfigurationExtension configuration)
        {
            Log = options.CurrentValue;
            BackupStorageFactory = backupStorageFactory;
            BackupWorker = backupWorker;
            BackupRepository = backupRepository;
            Configuration = configuration;
        }

        public void StartBackup(StartBackupRequest request)
        {
            var progress = BackupWorker.StartBackup(request);
            if (!string.IsNullOrEmpty(progress.Error))
            {
                throw new FaultException();
            }
        }

        public void DeleteBackup(Guid id)
        {
            var backupRecord = BackupRepository.GetBackupRecord(id);
            BackupRepository.DeleteBackupRecord(backupRecord.Id);

            var storage = BackupStorageFactory.GetBackupStorage(backupRecord);
            if (storage == null) return;
            storage.Delete(backupRecord.StoragePath);
        }

        public void DeleteAllBackups(int tenantId)
        {
            foreach (var backupRecord in BackupRepository.GetBackupRecordsByTenantId(tenantId))
            {
                try
                {
                    BackupRepository.DeleteBackupRecord(backupRecord.Id);
                    var storage = BackupStorageFactory.GetBackupStorage(backupRecord);
                    if (storage == null) continue;
                    storage.Delete(backupRecord.StoragePath);
                }
                catch (Exception error)
                {
                    Log.Warn("error while removing backup record: {0}", error);
                }
            }
        }

        public List<BackupHistoryRecord> GetBackupHistory(int tenantId)
        {
            var backupHistory = new List<BackupHistoryRecord>();
            foreach (var record in BackupRepository.GetBackupRecordsByTenantId(tenantId))
            {
                var storage = BackupStorageFactory.GetBackupStorage(record);
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
                    BackupRepository.DeleteBackupRecord(record.Id);
                }
            }
            return backupHistory;
        }

        public void StartTransfer(StartTransferRequest request)
        {
            var progress = BackupWorker.StartTransfer(request.TenantId, request.TargetRegion, request.BackupMail, request.NotifyUsers);
            if (!string.IsNullOrEmpty(progress.Error))
            {
                throw new FaultException();
            }
        }

        public void StartRestore(StartRestoreRequest request)
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
                var backupRecord = BackupRepository.GetBackupRecord(request.BackupId);
                if (backupRecord == null)
                {
                    throw new FileNotFoundException();
                }

                request.FilePathOrId = backupRecord.StoragePath;
                request.StorageType = backupRecord.StorageType;
                request.StorageParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(backupRecord.StorageParams);
            }

            var progress = BackupWorker.StartRestore(request);
            if (!string.IsNullOrEmpty(progress.Error))
            {
                throw new FaultException();
            }
        }

        public BackupProgress GetBackupProgress(int tenantId)
        {
            return BackupWorker.GetBackupProgress(tenantId);
        }

        public BackupProgress GetTransferProgress(int tenantId)
        {
            return BackupWorker.GetTransferProgress(tenantId);
        }

        public BackupProgress GetRestoreProgress(int tenantId)
        {
            return BackupWorker.GetRestoreProgress(tenantId);
        }

        public string GetTmpFolder()
        {
            return BackupWorker.TempFolder;
        }

        public List<TransferRegion> GetTransferRegions()
        {
            var settings = Configuration.GetSetting<BackupSettings>("backup");
            return settings.WebConfigs.Elements.Select(configElement =>
            {
                var config = Utils.ConfigurationProvider.Open(PathHelper.ToRootedConfigPath(configElement.Path));
                var baseDomain = config.AppSettings.Settings["core:base-domain"].Value;
                return new TransferRegion
                {
                    Name = configElement.Region,
                    BaseDomain = baseDomain,
                    IsCurrentRegion = configElement.Region.Equals(settings.WebConfigs.CurrentRegion, StringComparison.InvariantCultureIgnoreCase)
                };
            })
            .ToList();
        }

        public void CreateSchedule(CreateScheduleRequest request)
        {
            BackupRepository.SaveBackupSchedule(
                new BackupSchedule()
                {
                    TenantId = request.TenantId,
                    Cron = request.Cron,
                    BackupMail = request.BackupMail,
                    BackupsStored = request.NumberOfBackupsStored,
                    StorageType = request.StorageType,
                    StorageBasePath = request.StorageBasePath,
                    StorageParams = JsonConvert.SerializeObject(request.StorageParams)
                });
        }

        public void DeleteSchedule(int tenantId)
        {
            BackupRepository.DeleteBackupSchedule(tenantId);
        }

        public ScheduleResponse GetSchedule(int tenantId)
        {
            var schedule = BackupRepository.GetBackupSchedule(tenantId);
            if (schedule != null)
            {
                var tmp = new ScheduleResponse
                {
                    StorageType = schedule.StorageType,
                    StorageBasePath = schedule.StorageBasePath,
                    BackupMail = schedule.BackupMail,
                    NumberOfBackupsStored = schedule.BackupsStored,
                    Cron = schedule.Cron,
                    LastBackupTime = schedule.LastBackupTime,
                    StorageParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(schedule.StorageParams)
                };
                return tmp;
            }
            else
            {
                return null;
            }
        }
    }
}
