/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
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

namespace ASC.Data.Storage;

[Scope(Additional = typeof(StaticUploaderExtension))]
public class StaticUploader
{
    protected readonly DistributedTaskQueue Queue;
    private ICache _cache;
    private static readonly TaskScheduler _scheduler;
    private static readonly CancellationTokenSource _tokenSource;
    private static readonly object _locker;
    private readonly IServiceProvider _serviceProvider;
    private readonly TenantManager _tenantManager;
    private readonly SettingsManager _settingsManager;
    private readonly StorageSettingsHelper _storageSettingsHelper;

    static StaticUploader()
    {
        _scheduler = new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, 4).ConcurrentScheduler;
        _locker = new object();
        _tokenSource = new CancellationTokenSource();
    }

    public StaticUploader(
        IServiceProvider serviceProvider,
        TenantManager tenantManager,
        SettingsManager settingsManager,
        StorageSettingsHelper storageSettingsHelper,
        ICache cache,
        DistributedTaskQueueOptionsManager options)
    {
        _cache = cache;
        _serviceProvider = serviceProvider;
        _tenantManager = tenantManager;
        _settingsManager = settingsManager;
        _storageSettingsHelper = storageSettingsHelper;
        Queue = options.Get<UploadOperationProgress>();
    }

    public string UploadFile(string relativePath, string mappedPath, Action<string> onComplete = null)
    {
        if (_tokenSource.Token.IsCancellationRequested)
        {
            return null;
        }

        if (!CanUpload())
        {
            return null;
        }

        if (!File.Exists(mappedPath))
        {
            return null;
        }

        var tenantId = _tenantManager.GetCurrentTenant().Id;
        UploadOperation uploadOperation;
        var key = GetCacheKey(tenantId.ToString(), relativePath);

        lock (_locker)
        {
            uploadOperation = _cache.Get<UploadOperation>(key);
            if (uploadOperation != null)
            {
                return !string.IsNullOrEmpty(uploadOperation.Result) ? uploadOperation.Result : string.Empty;
            }

            uploadOperation = new UploadOperation(_serviceProvider, tenantId, relativePath, mappedPath);
            _cache.Insert(key, uploadOperation, DateTime.MaxValue);
        }

            uploadOperation.DoJobAsync().Wait();
        onComplete?.Invoke(uploadOperation.Result);

        return uploadOperation.Result;
    }

    public Task<string> UploadFileAsync(string relativePath, string mappedPath, Action<string> onComplete = null)
    {
        var tenantId = _tenantManager.GetCurrentTenant().Id;
        var task = new Task<string>(() =>
        {
            using var scope = _serviceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<StaticUploaderScope>();
            var (tenantManager, staticUploader, _, _, _) = scopeClass;
            tenantManager.SetCurrentTenant(tenantId);

            return staticUploader.UploadFile(relativePath, mappedPath, onComplete);
        }, TaskCreationOptions.LongRunning);

        task.ConfigureAwait(false);

        task.Start(_scheduler);

        return task;
    }

    public void UploadDir(string relativePath, string mappedPath)
    {
        if (!CanUpload())
        {
            return;
        }

        if (!Directory.Exists(mappedPath))
        {
            return;
        }

        var tenant = _tenantManager.GetCurrentTenant();
        var key = typeof(UploadOperationProgress).FullName + tenant.Id;
        UploadOperationProgress uploadOperation;

        lock (_locker)
        {
            uploadOperation = Queue.GetTask<UploadOperationProgress>(key);
            if (uploadOperation != null)
            {
                return;
            }

            uploadOperation = new UploadOperationProgress(_serviceProvider, key, tenant.Id, relativePath, mappedPath);
            Queue.QueueTask(uploadOperation);
        }
    }

    public bool CanUpload()
    {
        var current = _storageSettingsHelper.DataStoreConsumer(_settingsManager.Load<CdnStorageSettings>());
        if (current == null || !current.IsSet || (string.IsNullOrEmpty(current["cnamessl"]) && string.IsNullOrEmpty(current["cname"])))
        {
            return false;
        }

        return true;
    }

    public static void Stop()
    {
        _tokenSource.Cancel();
    }

    public UploadOperationProgress GetProgress(int tenantId)
    {
        lock (_locker)
        {
            var key = typeof(UploadOperationProgress).FullName + tenantId;

            return Queue.GetTask<UploadOperationProgress>(key);
        }
    }

    private static string GetCacheKey(string tenantId, string path)
    {
        return typeof(UploadOperation).FullName + tenantId + path;
    }
}

public class UploadOperation
{
    public string Result { get; private set; }

    private readonly ILog _logger;
    private readonly int _tenantId;
    private readonly string _path;
    private readonly string _mappedPath;

    private readonly IServiceProvider _serviceProvider;

    public UploadOperation(IServiceProvider serviceProvider, int tenantId, string path, string mappedPath)
    {
        _serviceProvider = serviceProvider;
        _logger = _serviceProvider.GetService<IOptionsMonitor<ILog>>().CurrentValue;
        _tenantId = tenantId;
        _path = path.TrimStart('/');
        _mappedPath = mappedPath;
        Result = string.Empty;
    }

        public async Task<string> DoJobAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<StaticUploaderScope>();
            var (tenantManager, _, securityContext, settingsManager, storageSettingsHelper) = scopeClass;
            var tenant = tenantManager.GetTenant(_tenantId);
            tenantManager.SetCurrentTenant(tenant);
            securityContext.AuthenticateMeWithoutCookie(tenant.OwnerId);

            var dataStore = storageSettingsHelper.DataStore(settingsManager.Load<CdnStorageSettings>());

            if (File.Exists(_mappedPath))
            {
                    if (!await dataStore.IsFileAsync(_path))
                {
                        using var stream = File.OpenRead(_mappedPath);
                        await dataStore.SaveAsync(_path, stream);
                }
                    var uri = await dataStore.GetInternalUriAsync("", _path, TimeSpan.Zero, null);
                    Result = uri.AbsoluteUri.ToLower();
                    _logger.DebugFormat("UploadFile {0}", Result);
                return Result;
            }
        }
        catch (Exception e)
        {
            _logger.Error(e);
        }

        return null;
    }
}

[Transient]
public class UploadOperationProgress : DistributedTaskProgress
{
    public int TenantId { get; }

    private readonly string _relativePath;
    private readonly string _mappedPath;
    private readonly IEnumerable<string> _directoryFiles;
    private readonly IServiceProvider _serviceProvider;

    public UploadOperationProgress(IServiceProvider serviceProvider, string key, int tenantId, string relativePath, string mappedPath)
    {
        _serviceProvider = serviceProvider;

        Id = key;
        Status = DistributedTaskStatus.Created;

        TenantId = tenantId;
        _relativePath = relativePath;
        _mappedPath = mappedPath;

            const string extensions = ".png|.jpeg|.jpg|.gif|.ico|.swf|.mp3|.ogg|.eot|.svg|.ttf|.woff|.woff2|.css|.less|.js";
        var extensionsArray = extensions.Split('|');

        _directoryFiles = Directory.GetFiles(mappedPath, "*", SearchOption.AllDirectories)
            .Where(r => extensionsArray.Contains(Path.GetExtension(r)))
            .ToList();

        StepCount = _directoryFiles.Count();
    }

    protected override void DoJob()
    {
        using var scope = _serviceProvider.CreateScope();
        var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
        var staticUploader = scope.ServiceProvider.GetService<StaticUploader>();
        var tenant = tenantManager.GetTenant(TenantId);
        tenantManager.SetCurrentTenant(tenant);

        tenant.SetStatus(TenantStatus.Migrating);
        tenantManager.SaveTenant(tenant);
        PublishChanges();

        foreach (var file in _directoryFiles)
        {
            var filePath = file.Substring(_mappedPath.TrimEnd('/').Length);
            staticUploader.UploadFile(CrossPlatform.PathCombine(_relativePath, filePath), file, (res) => StepDone());
        }

        tenant.SetStatus(TenantStatus.Active);
        tenantManager.SaveTenant(tenant);
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}

[Scope]
public class StaticUploaderScope
{
    private readonly TenantManager _tenantManager;
    private readonly StaticUploader _staticUploader;
    private readonly SecurityContext _securityContext;
    private readonly SettingsManager _settingsManager;
    private readonly StorageSettingsHelper _storageSettingsHelper;

    public StaticUploaderScope(TenantManager tenantManager,
        StaticUploader staticUploader,
        SecurityContext securityContext,
        SettingsManager settingsManager,
        StorageSettingsHelper storageSettingsHelper)
    {
        _tenantManager = tenantManager;
        _staticUploader = staticUploader;
        _securityContext = securityContext;
        _settingsManager = settingsManager;
        _storageSettingsHelper = storageSettingsHelper;
    }

    public void Deconstruct(
        out TenantManager tenantManager,
        out StaticUploader staticUploader,
        out SecurityContext securityContext,
        out SettingsManager settingsManager,
        out StorageSettingsHelper storageSettingsHelper)
    {
        tenantManager = _tenantManager;
        staticUploader = _staticUploader;
        securityContext = _securityContext;
        settingsManager = _settingsManager;
        storageSettingsHelper = _storageSettingsHelper;
    }
}

public static class StaticUploaderExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<StaticUploaderScope>();
        services.AddDistributedTaskQueueService<UploadOperationProgress>(1);
    }
}
