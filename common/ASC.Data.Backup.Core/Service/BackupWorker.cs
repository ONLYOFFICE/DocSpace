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


using ConfigurationProvider = ASC.Data.Backup.Utils.ConfigurationProvider;

namespace ASC.Data.Backup.Services;

public enum BackupProgressItemEnum
{
    Backup,
    Restore,
    Transfer
}

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
}

public static class BackupProgressItemEnumConverter
{
    public static BackupProgressEnum Convert(this BackupProgressItemEnum backupProgressItemEnum)
    {
        return backupProgressItemEnum switch
        {
            BackupProgressItemEnum.Backup => BackupProgressEnum.Backup,
            BackupProgressItemEnum.Restore => BackupProgressEnum.Restore,
            BackupProgressItemEnum.Transfer => BackupProgressEnum.Transfer,
            _ => BackupProgressEnum.Backup
        };
    }
}

public abstract class BaseBackupProgressItem : DistributedTaskProgress
{
    public int TenantId
    {
        get => _tenantId ?? GetProperty<int>(nameof(_tenantId));
        set
        {
            _tenantId = value;
            SetProperty(nameof(_tenantId), value);
        }
    }
    public abstract BackupProgressItemEnum BackupProgressItemEnum { get; }
    protected ILog Logger { get; set; }
    protected IServiceScopeFactory ServiceScopeFactory { get; set; }

    private int? _tenantId;

    protected BaseBackupProgressItem(IOptionsMonitor<ILog> options, IServiceScopeFactory serviceScopeFactory)
    {
        Logger = options.CurrentValue;
        ServiceScopeFactory = serviceScopeFactory;
    }

    public abstract object Clone();
}

[Transient]
public class BackupProgressItem : BaseBackupProgressItem
{
    public bool BackupMail { get; set; }
    public Dictionary<string, string> StorageParams { get; set; }
    public string Link { get; private set; }
    public string TempFolder { get; set; }
    public override BackupProgressItemEnum BackupProgressItemEnum => BackupProgressItemEnum.Backup;

    private const string ArchiveFormat = "tar.gz";

    private bool _isScheduled;
    private Guid _userId;
    private BackupStorageType _storageType;
    private string _storageBasePath;
    private string _currentRegion;
    private Dictionary<string, string> _configPaths;
    private int _limit;

    public BackupProgressItem(
        IOptionsMonitor<ILog> options,
        IServiceScopeFactory serviceScopeFactory)
        : base(options, serviceScopeFactory)
    {
    }

    public void Init(BackupSchedule schedule, bool isScheduled, string tempFolder, int limit, string currentRegion, Dictionary<string, string> configPaths)
    {
        _userId = Guid.Empty;
        TenantId = schedule.TenantId;
        _storageType = schedule.StorageType;
        _storageBasePath = schedule.StorageBasePath;
        BackupMail = schedule.BackupMail;
        StorageParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(schedule.StorageParams);
        _isScheduled = isScheduled;
        TempFolder = tempFolder;
        _limit = limit;
        _currentRegion = currentRegion;
        _configPaths = configPaths;
    }

    public void Init(StartBackupRequest request, bool isScheduled, string tempFolder, int limit, string currentRegion, Dictionary<string, string> configPaths)
    {
        _userId = request.UserId;
        TenantId = request.TenantId;
        _storageType = request.StorageType;
        _storageBasePath = request.StorageBasePath;
        BackupMail = request.BackupMail;
        StorageParams = request.StorageParams.ToDictionary(r => r.Key, r => r.Value);
        _isScheduled = isScheduled;
        TempFolder = tempFolder;
        _limit = limit;
        _currentRegion = currentRegion;
        _configPaths = configPaths;
    }

    protected override void DoJob()
    {
        if (ThreadPriority.BelowNormal < Thread.CurrentThread.Priority)
        {
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
        }

        using var scope = ServiceScopeFactory.CreateScope();
        var scopeClass = scope.ServiceProvider.GetService<BackupWorkerScope>();
        var (tenantManager, backupStorageFactory, notifyHelper, backupRepository, backupWorker, backupPortalTask, _, _, coreBaseSettings) = scopeClass;

            var dateTime = coreBaseSettings.Standalone ? DateTime.Now : DateTime.UtcNow;
            var backupName = string.Format("{0}_{1:yyyy-MM-dd_HH-mm-ss}.{2}", tenantManager.GetTenant(TenantId).Alias, dateTime, ArchiveFormat);

        var tempFile = CrossPlatform.PathCombine(TempFolder, backupName);
        var storagePath = tempFile;
        try
        {
            var backupTask = backupPortalTask;

            backupTask.Init(TenantId, _configPaths[_currentRegion], tempFile, _limit);
            if (!BackupMail)
            {
                backupTask.IgnoreModule(ModuleName.Mail);
            }

            backupTask.ProgressChanged += (sender, args) =>
            {
                Percentage = 0.9 * args.Progress;
                PublishChanges();
            };

            backupTask.RunJob();

            var backupStorage = backupStorageFactory.GetBackupStorage(_storageType, TenantId, StorageParams);
            if (backupStorage != null)
            {
                storagePath = backupStorage.Upload(_storageBasePath, tempFile, _userId);
                Link = backupStorage.GetPublicLink(storagePath);
            }

            var repo = backupRepository;
            repo.SaveBackupRecord(
                new BackupRecord
                {
                    Id = Guid.Parse(Id),
                    TenantId = TenantId,
                    IsScheduled = _isScheduled,
                    Name = Path.GetFileName(tempFile),
                    StorageType = _storageType,
                    StorageBasePath = _storageBasePath,
                    StoragePath = storagePath,
                    CreatedOn = DateTime.UtcNow,
                    ExpiresOn = _storageType == BackupStorageType.DataStore ? DateTime.UtcNow.AddDays(1) : DateTime.MinValue,
                    StorageParams = JsonConvert.SerializeObject(StorageParams),
                    Hash = BackupWorker.GetBackupHash(tempFile)
                });

            Percentage = 100;

            if (_userId != Guid.Empty && !_isScheduled)
            {
                notifyHelper.SendAboutBackupCompleted(_userId);
            }

            IsCompleted = true;
            PublishChanges();
        }
        catch (Exception error)
        {
            Logger.ErrorFormat("RunJob - Params: {0}, Error = {1}", new { Id, Tenant = TenantId, File = tempFile, BasePath = _storageBasePath, }, error);
            Exception = error;
            IsCompleted = true;
        }
        finally
        {
            try
            {
                PublishChanges();
            }
            catch (Exception error)
            {
                Logger.Error("publish", error);
            }

            try
            {
                if (!(storagePath == tempFile && _storageType == BackupStorageType.Local))
                {
                    File.Delete(tempFile);
                }
            }
            catch (Exception error)
            {
                Logger.Error("can't delete file: {0}", error);
            }
        }
    }

    public override object Clone()
    {
        return MemberwiseClone();
    }
}

[Transient]
public class RestoreProgressItem : BaseBackupProgressItem
{
    public override BackupProgressItemEnum BackupProgressItemEnum { get => BackupProgressItemEnum.Restore; }
    public BackupStorageType StorageType { get; set; }
    public string StoragePath { get; set; }
    public bool Notify { get; set; }
    public Dictionary<string, string> StorageParams { get; set; }
    public string TempFolder { get; set; }

    private string _currentRegion;
    private string _upgradesPath;
    private Dictionary<string, string> _configPaths;

    public RestoreProgressItem(
        IOptionsMonitor<ILog> options,
        IServiceScopeFactory serviceScopeFactory)
        : base(options, serviceScopeFactory)
    {
    }

    public void Init(StartRestoreRequest request, string tempFolder, string upgradesPath, string currentRegion, Dictionary<string, string> configPaths)
    {
        TenantId = request.TenantId;
        Notify = request.NotifyAfterCompletion;
        StoragePath = request.FilePathOrId;
        StorageType = request.StorageType;
        TempFolder = tempFolder;
        _upgradesPath = upgradesPath;
        _currentRegion = currentRegion;
        _configPaths = configPaths;
    }

    protected override void DoJob()
    {
        using var scope = ServiceScopeFactory.CreateScope();
        var scopeClass = scope.ServiceProvider.GetService<BackupWorkerScope>();
        var (tenantManager, backupStorageFactory, notifyHelper, backupRepository, backupWorker, _, restorePortalTask, _, coreBaseSettings) = scopeClass;
        Tenant tenant = null;
        var tempFile = PathHelper.GetTempFileName(TempFolder);
        try
        {
            tenant = tenantManager.GetTenant(TenantId);
            tenantManager.SetCurrentTenant(tenant);
            notifyHelper.SendAboutRestoreStarted(tenant, Notify);
            var storage = backupStorageFactory.GetBackupStorage(StorageType, TenantId, StorageParams);
            storage.Download(StoragePath, tempFile);

            if (!coreBaseSettings.Standalone)
            {
                var backupHash = BackupWorker.GetBackupHash(tempFile);
                var record = backupRepository.GetBackupRecord(backupHash, TenantId);
                if (record == null)
                {
                    throw new Exception(BackupResource.BackupNotFound);
                }
            }

            Percentage = 10;

            tenant.SetStatus(TenantStatus.Restoring);
            tenantManager.SaveTenant(tenant);

                var columnMapper = new ColumnMapper();
                columnMapper.SetMapping("tenants_tenants", "alias", tenant.Alias, Guid.Parse(Id).ToString("N"));
                columnMapper.Commit();

            var restoreTask = restorePortalTask;
            restoreTask.Init(_configPaths[_currentRegion], tempFile, TenantId, columnMapper, _upgradesPath);
            restoreTask.ProgressChanged += (sender, args) =>
            {
                    Percentage = Percentage = 10d + 0.65 * args.Progress;
                PublishChanges();
            };
            restoreTask.RunJob();

            Tenant restoredTenant = null;

            if (restoreTask.Dump)
            {
                AscCacheNotify.OnClearCache();

                    if (Notify)
                    {
                        var tenants = tenantManager.GetTenants();
                        foreach (var t in tenants)
                        {
                            notifyHelper.SendAboutRestoreCompleted(t, Notify);
                        }
                    }
                }
                else
                {
                    tenantManager.RemoveTenant(tenant.Id);

                    restoredTenant = tenantManager.GetTenant(columnMapper.GetTenantMapping());
                    restoredTenant.SetStatus(TenantStatus.Active);
                    restoredTenant.Alias = tenant.Alias;
                    restoredTenant.PaymentId = string.Empty;
                    if (string.IsNullOrEmpty(restoredTenant.MappedDomain) && !string.IsNullOrEmpty(tenant.MappedDomain))
                    {
                        restoredTenant.MappedDomain = tenant.MappedDomain;
                    }
                    tenantManager.SaveTenant(restoredTenant);
                    tenantManager.SetCurrentTenant(restoredTenant);
                    // sleep until tenants cache expires
                    Thread.Sleep(TimeSpan.FromMinutes(2));

                notifyHelper.SendAboutRestoreCompleted(restoredTenant, Notify);
            }

            Percentage = 75;

            PublishChanges();

            File.Delete(tempFile);

            Percentage = 100;
            PublishChanges();
        }
        catch (Exception error)
        {
            Logger.Error(error);
            Exception = error;

            if (tenant != null)
            {
                tenant.SetStatus(TenantStatus.Active);
                tenantManager.SaveTenant(tenant);
            }
        }
        finally
        {
            try
            {
                PublishChanges();
            }
            catch (Exception error)
            {
                Logger.Error("publish", error);
            }

            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    public override object Clone()
    {
        return MemberwiseClone();
    }
}

[Transient]
public class TransferProgressItem : BaseBackupProgressItem
{
    public override BackupProgressItemEnum BackupProgressItemEnum { get => BackupProgressItemEnum.Transfer; }
    public string TargetRegion { get; set; }
    public bool TransferMail { get; set; }
    public bool Notify { get; set; }
    public string Link { get; set; }
    public string TempFolder { get; set; }
    public Dictionary<string, string> ConfigPaths { get; set; }
    public string CurrentRegion { get; set; }
    public int Limit { get; set; }

    public TransferProgressItem(
        IOptionsMonitor<ILog> options,
        IServiceScopeFactory serviceScopeFactory) : base(options, serviceScopeFactory)
    {
    }

    public void Init(
        string targetRegion,
        bool transferMail,
        int tenantId,
        string tempFolder,
        int limit,
        bool notify,
        string currentRegion,
        Dictionary<string, string> configPaths)
    {
        TenantId = tenantId;
        TargetRegion = targetRegion;
        TransferMail = transferMail;
        Notify = notify;
        TempFolder = tempFolder;
        ConfigPaths = configPaths;
        CurrentRegion = currentRegion;
        Limit = limit;

    }

        protected override void DoJob()
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<BackupWorkerScope>();
            var (tenantManager, _, notifyHelper, _, backupWorker, _, _, transferPortalTask, _) = scopeClass;
            var tempFile = PathHelper.GetTempFileName(TempFolder);
            var tenant = tenantManager.GetTenant(TenantId);
            var alias = tenant.Alias;

        try
        {
            notifyHelper.SendAboutTransferStart(tenant, TargetRegion, Notify);
            var transferProgressItem = transferPortalTask;
            transferProgressItem.Init(TenantId, ConfigPaths[CurrentRegion], ConfigPaths[TargetRegion], Limit, TempFolder);
            transferProgressItem.ProgressChanged += (sender, args) =>
            {
                Percentage = args.Progress;
                PublishChanges();
            };
            if (!TransferMail)
            {
                transferProgressItem.IgnoreModule(ModuleName.Mail);
            }
            transferProgressItem.RunJob();

            Link = GetLink(alias, false);
            notifyHelper.SendAboutTransferComplete(tenant, TargetRegion, Link, !Notify, transferProgressItem.ToTenantId);
            PublishChanges();
        }
        catch (Exception error)
        {
            Logger.Error(error);
            Exception = error;

            Link = GetLink(alias, true);
            notifyHelper.SendAboutTransferError(tenant, TargetRegion, Link, !Notify);
        }
        finally
        {
            try
            {
                PublishChanges();
            }
            catch (Exception error)
            {
                Logger.Error("publish", error);
            }

            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    private string GetLink(string alias, bool isErrorLink)
    {
        return "https://" + alias + "." + ConfigurationProvider.Open(ConfigPaths[isErrorLink ? CurrentRegion : TargetRegion]).AppSettings.Settings["core:base-domain"].Value;
    }

    public override object Clone()
    {
        return MemberwiseClone();
    }
}

[Singletone(Additional = typeof(FactoryProgressItemExtension))]
public class FactoryProgressItem
{
    public IServiceProvider ServiceProvider { get; }

    public FactoryProgressItem(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public BackupProgressItem CreateBackupProgressItem(
        StartBackupRequest request,
        bool isScheduled,
        string tempFolder,
        int limit,
        string currentRegion,
        Dictionary<string, string> configPaths)
    {
        var item = ServiceProvider.GetService<BackupProgressItem>();
        item.Init(request, isScheduled, tempFolder, limit, currentRegion, configPaths);

        return item;
    }

    public BackupProgressItem CreateBackupProgressItem(
        BackupSchedule schedule,
        bool isScheduled,
        string tempFolder,
        int limit,
        string currentRegion,
        Dictionary<string, string> configPaths
        )
    {
        var item = ServiceProvider.GetService<BackupProgressItem>();
        item.Init(schedule, isScheduled, tempFolder, limit, currentRegion, configPaths);

        return item;
    }

    public RestoreProgressItem CreateRestoreProgressItem(
        StartRestoreRequest request,
        string tempFolder,
        string upgradesPath,
        string currentRegion,
        Dictionary<string, string> configPaths
        )
    {
        var item = ServiceProvider.GetService<RestoreProgressItem>();
        item.Init(request, tempFolder, upgradesPath, currentRegion, configPaths);

        return item;
    }

    public TransferProgressItem CreateTransferProgressItem(
        string targetRegion,
        bool transferMail,
        int tenantId,
        string tempFolder,
        int limit,
        bool notify,
        string currentRegion,
        Dictionary<string, string> configPaths
        )
    {
        var item = ServiceProvider.GetService<TransferProgressItem>();
        item.Init(targetRegion, transferMail, tenantId, tempFolder, limit, notify, currentRegion, configPaths);

        return item;
    }
}

[Scope]
internal class BackupWorkerScope
{
    private readonly TenantManager _tenantManager;
    private readonly BackupStorageFactory _backupStorageFactory;
    private readonly NotifyHelper _notifyHelper;
    private readonly BackupRepository _backupRepository;
    private readonly BackupWorker _backupWorker;
    private readonly BackupPortalTask _backupPortalTask;
    private readonly RestorePortalTask _restorePortalTask;
    private readonly TransferPortalTask _transferPortalTask;
    private readonly CoreBaseSettings _coreBaseSettings;

    public BackupWorkerScope(TenantManager tenantManager,
        BackupStorageFactory backupStorageFactory,
        NotifyHelper notifyHelper,
        BackupRepository backupRepository,
        BackupWorker backupWorker,
        BackupPortalTask backupPortalTask,
        RestorePortalTask restorePortalTask,
        TransferPortalTask transferPortalTask,
        CoreBaseSettings coreBaseSettings)
    {
        _tenantManager = tenantManager;
        _backupStorageFactory = backupStorageFactory;
        _notifyHelper = notifyHelper;
        _backupRepository = backupRepository;
        _backupWorker = backupWorker;
        _backupPortalTask = backupPortalTask;
        _restorePortalTask = restorePortalTask;
        _transferPortalTask = transferPortalTask;
        _coreBaseSettings = coreBaseSettings;
    }

    public void Deconstruct(out TenantManager tenantManager,
        out BackupStorageFactory backupStorageFactory,
        out NotifyHelper notifyHelper,
        out BackupRepository backupRepository,
        out BackupWorker backupWorker,
        out BackupPortalTask backupPortalTask,
        out RestorePortalTask restorePortalTask,
        out TransferPortalTask transferPortalTask,
        out CoreBaseSettings coreBaseSettings)
    {
        tenantManager = _tenantManager;
        backupStorageFactory = _backupStorageFactory;
        notifyHelper = _notifyHelper;
        backupRepository = _backupRepository;
        backupWorker = _backupWorker;
        backupPortalTask = _backupPortalTask;
        restorePortalTask = _restorePortalTask;
        transferPortalTask = _transferPortalTask;
        coreBaseSettings = _coreBaseSettings;
    }
}

public static class BackupWorkerExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<BackupWorkerScope>();
        services.AddDistributedTaskQueueService<BackupProgressItem>(5);
    }
}

public static class FactoryProgressItemExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<BackupProgressItem>();
        services.TryAdd<RestoreProgressItem>();
        services.TryAdd<TransferProgressItem>();
    }
}
