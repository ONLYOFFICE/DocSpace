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

namespace ASC.Data.Storage;

[Scope]
public class StaticUploader
{
    protected readonly DistributedTaskQueue _queue;
    private readonly ICache _cache;
    public const string CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME = "static_upload"; private static readonly CancellationTokenSource _tokenSource;
    private static readonly object _locker;
    private readonly IServiceProvider _serviceProvider;
    private readonly TenantManager _tenantManager;
    private readonly SettingsManager _settingsManager;
    private readonly StorageSettingsHelper _storageSettingsHelper;
    private readonly UploadOperation _uploadOperation;

    static StaticUploader()
    {
        _locker = new object();
        _tokenSource = new CancellationTokenSource();
    }

    public StaticUploader(
        IServiceProvider serviceProvider,
        TenantManager tenantManager,
        SettingsManager settingsManager,
        StorageSettingsHelper storageSettingsHelper,
        UploadOperation uploadOperation,
        ICache cache,
        IDistributedTaskQueueFactory queueFactory)
    {
        _cache = cache;
        _serviceProvider = serviceProvider;
        _tenantManager = tenantManager;
        _settingsManager = settingsManager;
        _storageSettingsHelper = storageSettingsHelper;
        _queue = queueFactory.CreateQueue(CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME);
        _uploadOperation = uploadOperation;
    }

    public async Task<string> UploadFileAsync(string relativePath, string mappedPath, Action<string> onComplete = null)
    {
        if (_tokenSource.Token.IsCancellationRequested)
        {
            return null;
        }

        if (!await CanUploadAsync())
        {
            return null;
        }

        if (!File.Exists(mappedPath))
        {
            return null;
        }

        var tenantId = await _tenantManager.GetCurrentTenantIdAsync();
        var key = GetCacheKey(tenantId.ToString(), relativePath);

        lock (_locker)
        {
            var result = _cache.Get<string>(key);
            if (!string.IsNullOrEmpty(result))
            {
                return result;
            }
        }

        await _uploadOperation.DoJobAsync(tenantId, relativePath, mappedPath);
        onComplete?.Invoke(_uploadOperation.Result);

        lock (_locker)
        {
            _cache.Insert(key, _uploadOperation.Result, DateTime.MaxValue);
        }

        return _uploadOperation.Result;
    }

    public async Task UploadDirAsync(string relativePath, string mappedPath)
    {
        if (!await CanUploadAsync())
        {
            return;
        }

        if (!Directory.Exists(mappedPath))
        {
            return;
        }

        var tenant = await _tenantManager.GetCurrentTenantAsync();
        var key = typeof(UploadOperationProgress).FullName + tenant.Id;

        lock (_locker)
        {
            if (_queue.GetAllTasks().Any(x => x.Id != key))
            {
                return;
            }

            var uploadOperation = new UploadOperationProgress(_serviceProvider, key, tenant.Id, relativePath, mappedPath);

            _queue.EnqueueTask(uploadOperation);
        }
    }

    public async Task<bool> CanUploadAsync()
    {
        var current = _storageSettingsHelper.DataStoreConsumer(await _settingsManager.LoadAsync<CdnStorageSettings>());
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

            return _queue.PeekTask<UploadOperationProgress>(key);
        }
    }

    private static string GetCacheKey(string tenantId, string path)
    {
        return typeof(UploadOperation).FullName + tenantId + path;
    }
}

[Scope]
public class UploadOperation
{
    public string Result { get; private set; }

    private readonly ILogger<UploadOperation> _logger;
    private readonly TenantManager _tenantManager;
    private readonly SecurityContext _securityContext;
    private readonly SettingsManager _settingsManager;
    private readonly StorageSettingsHelper _storageSettingsHelper;

    public UploadOperation(
        ILogger<UploadOperation> logger,
        TenantManager tenantManager,
        SecurityContext securityContext,
        SettingsManager settingsManager,
        StorageSettingsHelper storageSettingsHelper)
    {
        _logger = logger;
        Result = string.Empty;
        _tenantManager = tenantManager;
        _securityContext = securityContext;
        _settingsManager = settingsManager;
        _storageSettingsHelper = storageSettingsHelper;
    }

    public async Task<string> DoJobAsync(int tenantId, string path, string mappedPath)
    {
        try
        {
            path = path.TrimStart('/');
            var tenant = await _tenantManager.GetTenantAsync(tenantId);
            _tenantManager.SetCurrentTenant(tenant);
            await _securityContext.AuthenticateMeWithoutCookieAsync(tenant.OwnerId);

            var dataStore = await _storageSettingsHelper.DataStoreAsync(await _settingsManager.LoadAsync<CdnStorageSettings>());

            if (File.Exists(mappedPath))
            {
                if (!await dataStore.IsFileAsync(path))
                {
                    await using var stream = File.OpenRead(mappedPath);
                    await dataStore.SaveAsync(path, stream);
                }
                var uri = await dataStore.GetInternalUriAsync("", path, TimeSpan.Zero, null);
                Result = uri.AbsoluteUri.ToLower();
                _logger.DebugUploadFile(Result);
                return Result;
            }
        }
        catch (Exception e)
        {
            _logger.ErrorUploadOperation(e);
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

    protected override async Task DoJob()
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
        var staticUploader = scope.ServiceProvider.GetService<StaticUploader>();
        var tenant = await tenantManager.GetTenantAsync(TenantId);
        tenantManager.SetCurrentTenant(tenant);

        tenant.SetStatus(TenantStatus.Migrating);
        await tenantManager.SaveTenantAsync(tenant);
        PublishChanges();

        foreach (var file in _directoryFiles)
        {
            var filePath = file.Substring(_mappedPath.TrimEnd('/').Length);
            await staticUploader.UploadFileAsync(CrossPlatform.PathCombine(_relativePath, filePath), file, (res) => StepDone());
        }

        tenant.SetStatus(TenantStatus.Active);
        await tenantManager.SaveTenantAsync(tenant);
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}