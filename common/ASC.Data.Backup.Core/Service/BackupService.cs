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

namespace ASC.Data.Backup.Services;

[Scope]
public class BackupService : IBackupService
{
    private readonly ILogger<BackupService> _logger;
    private readonly BackupStorageFactory _backupStorageFactory;
    private readonly BackupWorker _backupWorker;
    private readonly BackupRepository _backupRepository;
    private readonly ConfigurationExtension _configuration;

    public BackupService(
        ILogger<BackupService> logger,
        BackupStorageFactory backupStorageFactory,
        BackupWorker backupWorker,
        BackupRepository backupRepository,
        ConfigurationExtension configuration)
    {
        _logger = logger;
        _backupStorageFactory = backupStorageFactory;
        _backupWorker = backupWorker;
        _backupRepository = backupRepository;
        _configuration = configuration;
    }

    public void StartBackup(StartBackupRequest request)
    {
        var progress = _backupWorker.StartBackup(request);
        if (!string.IsNullOrEmpty(progress.Error))
        {
            throw new FaultException();
        }
    }

    public async Task DeleteBackupAsync(Guid id)
    {
        var backupRecord = await _backupRepository.GetBackupRecordAsync(id);
        await _backupRepository.DeleteBackupRecordAsync(backupRecord.Id);

        var storage = await _backupStorageFactory.GetBackupStorageAsync(backupRecord);
        if (storage == null)
        {
            return;
        }

        await storage.DeleteAsync(backupRecord.StoragePath);
    }

    public async Task DeleteAllBackupsAsync(int tenantId)
    {
        foreach (var backupRecord in await _backupRepository.GetBackupRecordsByTenantIdAsync(tenantId))
        {
            try
            {
                await _backupRepository.DeleteBackupRecordAsync(backupRecord.Id);
                var storage = await _backupStorageFactory.GetBackupStorageAsync(backupRecord);
                if (storage == null)
                {
                    continue;
                }

                await storage.DeleteAsync(backupRecord.StoragePath);
            }
            catch (Exception error)
            {
                _logger.WarningErrorWhileBackupRecord(error);
            }
        }
    }

    public async Task<List<BackupHistoryRecord>> GetBackupHistoryAsync(int tenantId)
    {
        var backupHistory = new List<BackupHistoryRecord>();
        foreach (var record in await _backupRepository.GetBackupRecordsByTenantIdAsync(tenantId))
        {
            var storage = await _backupStorageFactory.GetBackupStorageAsync(record);
            if (storage == null)
            {
                continue;
            }

            if (await storage.IsExistsAsync(record.StoragePath))
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
                await _backupRepository.DeleteBackupRecordAsync(record.Id);
            }
        }
        return backupHistory;
    }

    public void StartTransfer(StartTransferRequest request)
    {
        var progress = _backupWorker.StartTransfer(request.TenantId, request.TargetRegion, request.NotifyUsers);
        if (!string.IsNullOrEmpty(progress.Error))
        {
            throw new FaultException();
        }
    }

    public async Task StartRestoreAsync(StartRestoreRequest request)
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
            var backupRecord = await _backupRepository.GetBackupRecordAsync(request.BackupId);
            if (backupRecord == null)
            {
                throw new FileNotFoundException();
            }

            request.FilePathOrId = backupRecord.StoragePath;
            request.StorageType = backupRecord.StorageType;
            request.StorageParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(backupRecord.StorageParams);
        }

        var progress = _backupWorker.StartRestore(request);
        if (!string.IsNullOrEmpty(progress.Error))
        {
            throw new FaultException();
        }
    }

    public BackupProgress GetBackupProgress(int tenantId)
    {
        return _backupWorker.GetBackupProgress(tenantId);
    }

    public BackupProgress GetTransferProgress(int tenantId)
    {
        return _backupWorker.GetTransferProgress(tenantId);
    }

    public BackupProgress GetRestoreProgress(int tenantId)
    {
        return _backupWorker.GetRestoreProgress(tenantId);
    }

    public string GetTmpFolder()
    {
        return _backupWorker.TempFolder;
    }

    public async Task CreateScheduleAsync(CreateScheduleRequest request)
    {
        await _backupRepository.SaveBackupScheduleAsync(
            new BackupSchedule()
            {
                TenantId = request.TenantId,
                Cron = request.Cron,
                BackupsStored = request.NumberOfBackupsStored,
                StorageType = request.StorageType,
                StorageBasePath = request.StorageBasePath,
                StorageParams = JsonConvert.SerializeObject(request.StorageParams)
            });
    }

    public async Task DeleteScheduleAsync(int tenantId)
    {
        await _backupRepository.DeleteBackupScheduleAsync(tenantId);
    }

    public async Task<ScheduleResponse> GetScheduleAsync(int tenantId)
    {
        var schedule = await _backupRepository.GetBackupScheduleAsync(tenantId);
        if (schedule != null)
        {
            var tmp = new ScheduleResponse
            {
                StorageType = schedule.StorageType,
                StorageBasePath = schedule.StorageBasePath,
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
