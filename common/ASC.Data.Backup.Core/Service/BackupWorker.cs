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

namespace ASC.Data.Backup.Services;

[Singletone(Additional = typeof(BackupWorkerExtension))]
public class BackupWorker
{
    internal string TempFolder { get; set; }

    private DistributedTaskQueue _progressQueue;
    private string _currentRegion;
    private Dictionary<string, string> _configPaths;
    private int _limit;
    private string _upgradesPath;
    private readonly ILog _logger;
    private readonly FactoryProgressItem _factoryProgressItem;
    private readonly TempPath _tempPath;
    private readonly object _synchRoot = new object();

    public BackupWorker(
        IOptionsMonitor<ILog> options,
        DistributedTaskQueueOptionsManager progressQueue,
        FactoryProgressItem factoryProgressItem,
        TempPath tempPath)
    {
        _logger = options.CurrentValue;
        _progressQueue = progressQueue.Get<BaseBackupProgressItem>();
        _factoryProgressItem = factoryProgressItem;
        _tempPath = tempPath;
    }

    public void Start(BackupSettings settings)
    {
        TempFolder = _tempPath.GetTempPath();

        if (!Directory.Exists(TempFolder))
        {
            Directory.CreateDirectory(TempFolder);
        }

        _limit = settings.Limit;
        _upgradesPath = settings.UpgradesPath;
        _currentRegion = settings.WebConfigs.CurrentRegion;
        _configPaths = settings.WebConfigs.Elements.ToDictionary(el => el.Region, el => PathHelper.ToRootedConfigPath(el.Path));
        _configPaths[_currentRegion] = PathHelper.ToRootedConfigPath(settings.WebConfigs.CurrentPath);

        var invalidConfigPath = _configPaths.Values.FirstOrDefault(path => !File.Exists(path));
        
        if (invalidConfigPath != null)
        {
            _logger.WarnFormat("Configuration file {0} not found", invalidConfigPath);
        }
    }

    public void Stop()
    {
        if (_progressQueue != null)
        {
            var tasks = _progressQueue.GetTasks();

            foreach (var t in tasks)
            {
                _progressQueue.CancelTask(t.Id);
            }

            _progressQueue = null;
        }
    }

    public BackupProgress StartBackup(StartBackupRequest request)
    {
        lock (_synchRoot)
        {
            var item = _progressQueue.GetTasks<BackupProgressItem>().FirstOrDefault(t => t.TenantId == request.TenantId);
            if (item != null && item.IsCompleted)
            {
                _progressQueue.RemoveTask(item.Id);
                item = null;
            }
            if (item == null)
            {
                item = _factoryProgressItem.CreateBackupProgressItem(request, false, TempFolder, _limit, _currentRegion, _configPaths);
                _progressQueue.QueueTask(item);
            }

            item.PublishChanges();

            return ToBackupProgress(item);
        }
    }

    public void StartScheduledBackup(BackupSchedule schedule)
    {
        lock (_synchRoot)
        {
            var item = _progressQueue.GetTasks<BackupProgressItem>().FirstOrDefault(t => t.TenantId == schedule.TenantId);
            if (item != null && item.IsCompleted)
            {
                _progressQueue.RemoveTask(item.Id);
                item = null;
            }
            if (item == null)
            {
                item = _factoryProgressItem.CreateBackupProgressItem(schedule, false, TempFolder, _limit, _currentRegion, _configPaths);
                _progressQueue.QueueTask(item);
            }
        }
    }

    public BackupProgress GetBackupProgress(int tenantId)
    {
        lock (_synchRoot)
        {
            return ToBackupProgress(_progressQueue.GetTasks<BackupProgressItem>().FirstOrDefault(t => t.TenantId == tenantId));
        }
    }

    public BackupProgress GetTransferProgress(int tenantId)
    {
        lock (_synchRoot)
        {
            return ToBackupProgress(_progressQueue.GetTasks<TransferProgressItem>().FirstOrDefault(t => t.TenantId == tenantId));
        }
    }

    public BackupProgress GetRestoreProgress(int tenantId)
    {
        lock (_synchRoot)
        {
            return ToBackupProgress(_progressQueue.GetTasks<RestoreProgressItem>().FirstOrDefault(t => t.TenantId == tenantId));
        }
    }

    public void ResetBackupError(int tenantId)
    {
        lock (_synchRoot)
        {
            var progress = _progressQueue.GetTasks<BackupProgressItem>().FirstOrDefault(t => t.TenantId == tenantId);
            if (progress != null)
            {
                progress.Exception = null;
            }
        }
    }

    public void ResetRestoreError(int tenantId)
    {
        lock (_synchRoot)
        {
            var progress = _progressQueue.GetTasks<RestoreProgressItem>().FirstOrDefault(t => t.TenantId == tenantId);
            if (progress != null)
            {
                progress.Exception = null;
            }
        }
    }

    public BackupProgress StartRestore(StartRestoreRequest request)
    {
        lock (_synchRoot)
        {
            var item = _progressQueue.GetTasks<RestoreProgressItem>().FirstOrDefault(t => t.TenantId == request.TenantId);
            if (item != null && item.IsCompleted)
            {
                _progressQueue.RemoveTask(item.Id);
                item = null;
            }
            if (item == null)
            {
                item = _factoryProgressItem.CreateRestoreProgressItem(request, TempFolder, _upgradesPath, _currentRegion, _configPaths);
                _progressQueue.QueueTask(item);
            }
            return ToBackupProgress(item);
        }
    }

    public BackupProgress StartTransfer(int tenantId, string targetRegion, bool transferMail, bool notify)
    {
        lock (_synchRoot)
        {
            var item = _progressQueue.GetTasks<TransferProgressItem>().FirstOrDefault(t => t.TenantId == tenantId);
            if (item != null && item.IsCompleted)
            {
                _progressQueue.RemoveTask(item.Id);
                item = null;
            }

            if (item == null)
            {
                item = _factoryProgressItem.CreateTransferProgressItem(targetRegion, transferMail, tenantId, TempFolder, _limit, notify, _currentRegion, _configPaths);
                _progressQueue.QueueTask(item);
            }

            return ToBackupProgress(item);
        }
    }

    internal static string GetBackupHash(string path)
    {
        using (var sha256 = SHA256.Create())
        using (var fileStream = File.OpenRead(path))
        {
            fileStream.Position = 0;
            var hash = sha256.ComputeHash(fileStream);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
    }

    private BackupProgress ToBackupProgress(BaseBackupProgressItem progressItem)
    {
        if (progressItem == null)
        {
            return null;
        }
        var progress = new BackupProgress
        {
            IsCompleted = progressItem.IsCompleted,
            Progress = (int)progressItem.Percentage,
            Error = progressItem.Exception != null ? progressItem.Exception.Message : "",
            TenantId = progressItem.TenantId,
            BackupProgressEnum = progressItem.BackupProgressItemEnum.Convert()
        };

        if (progressItem is BackupProgressItem backupProgressItem && backupProgressItem.Link != null)
        {
            progress.Link = backupProgressItem.Link;
        }
        else
        {
            if (progressItem is TransferProgressItem transferProgressItem && transferProgressItem.Link != null)
            {
                progress.Link = transferProgressItem.Link;
            }
        }

        return progress;
    }

    public bool HaveBackupRestoreRequestWaitingTasks()
    {
        var countActiveTask = _progressQueue.GetTasks<RestoreProgressItem>().Where(x => !x.IsCompleted).Count();

        if (_progressQueue.MaxThreadsCount >= countActiveTask)
        {
            return false;
        }

        return true;

    }

    public bool HaveBackupTransferRequestWaitingTasks()
    {
        if (_progressQueue.MaxThreadsCount == -1) return false;

        var countActiveTask = _progressQueue.GetTasks<TransferProgressItem>().Where(x => !x.IsCompleted).Count();

        if (_progressQueue.MaxThreadsCount >= countActiveTask)
        {
            return false;
        }

        return true;

    }


    public bool HaveBackupRequestWaitingTasks()
    {
        if (_progressQueue.MaxThreadsCount == -1) return false;

        var countActiveTask = _progressQueue.GetTasks<BackupProgressItem>().Where(x => !x.IsCompleted).Count();

        if (_progressQueue.MaxThreadsCount >= countActiveTask)
        {
            return false;
        }

        return true;

    }
}

public static class BackupWorkerExtension
{
    public static void Register(DIHelper services)
    {
        services.AddDistributedTaskQueueService<BackupProgressItem>(5);
    }
}
