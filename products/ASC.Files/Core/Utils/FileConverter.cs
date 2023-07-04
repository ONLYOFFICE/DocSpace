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

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace ASC.Web.Files.Utils;

[Singletone]
public class FileConverterQueue<T>
{
    private readonly object _locker = new object();
    private readonly IDistributedCache _distributedCache;
    private readonly string _cache_key_prefix = "asc_file_converter_queue_";

    public FileConverterQueue(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public void Add(File<T> file,
                        string password,
                        int tenantId,
                        IAccount account,
                        bool deleteAfter,
                        string url,
                        string serverRootPath)
    {
        lock (_locker)
        {
            var task = PeekTask(file);

            if (task != null)
            {
                if (task.Progress != 100)
                {
                    return;
                }

                Dequeue(task);
            }

            var queueResult = new FileConverterOperationResult
            {
                Source = JsonSerializer.Serialize(new { id = file.Id, version = file.Version }),
                OperationType = FileOperationType.Convert,
                Error = string.Empty,
                Progress = 0,
                Result = string.Empty,
                Processed = "",
                Id = string.Empty,
                TenantId = tenantId,
                Account = account.ID,
                Delete = deleteAfter,
                StartDateTime = DateTime.UtcNow,
                Url = url,
                Password = password,
                ServerRootPath = serverRootPath
            };

            Enqueue(queueResult);
        }
    }


    public void Enqueue(FileConverterOperationResult val)
    {
        var fromCache = LoadFromCache().ToList();

        fromCache.Add(val);

        SaveToCache(fromCache);
    }

    public void Dequeue(FileConverterOperationResult val)
    {
        var fromCache = LoadFromCache().ToList();

        fromCache.Remove(val);

        SaveToCache(fromCache);
    }

    public FileConverterOperationResult PeekTask(File<T> file)
    {
        var exist = LoadFromCache();

        return exist.LastOrDefault(x =>
                {
                    var fileId = JsonDocument.Parse(x.Source).RootElement.GetProperty("id").Deserialize<T>();
                    var fileVersion = JsonDocument.Parse(x.Source).RootElement.GetProperty("version").Deserialize<int>();

                    return String.Compare(file.Id.ToString(), fileId.ToString(), true) == 0;
                });
    }

    public bool IsConverting(File<T> file)
    {
        var result = PeekTask(file);

        return result != null && result.Progress != 100 && string.IsNullOrEmpty(result.Error);
    }


    public IEnumerable<FileConverterOperationResult> GetAllTask()
    {
        var queueTasks = LoadFromCache();

        queueTasks = DeleteOrphanCacheItem(queueTasks);

        return queueTasks;
    }

    public void SetAllTask(IEnumerable<FileConverterOperationResult> queueTasks)
    {
        SaveToCache(queueTasks);
    }


    public async Task<FileConverterOperationResult> GetStatusAsync(KeyValuePair<File<T>, bool> pair, FileSecurity fileSecurity)
    {
        var file = pair.Key;
        var operation = PeekTask(file);

        if (operation != null && (pair.Value || await fileSecurity.CanReadAsync(file)))
        {
            if (operation.Progress == 100)
            {
                Dequeue(operation);
            }

            return operation;
        }

        return null;
    }


    public async Task<string> FileJsonSerializerAsync(EntryStatusManager EntryManager, File<T> file, string folderTitle)
    {
        if (file == null)
        {
            return string.Empty;
        }

        await EntryManager.SetFileStatusAsync(file);

        var options = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IgnoreReadOnlyProperties = true,
            WriteIndented = false
        };

        return JsonSerializer.Serialize(
            new FileJsonSerializerData<T>()
            {
                Id = file.Id,
                Title = file.Title,
                Version = file.Version,
                FolderID = file.ParentId,
                FolderTitle = folderTitle ?? "",
                FileJson = JsonSerializer.Serialize(file, options)
            }, options);
    }

    private bool IsOrphanCacheItem(FileConverterOperationResult x)
    {
        return !string.IsNullOrEmpty(x.Processed)
                           && (x.Progress == 100 && DateTime.UtcNow - x.StopDateTime > TimeSpan.FromMinutes(1) ||
                               DateTime.UtcNow - x.StopDateTime > TimeSpan.FromMinutes(10));
    }

    private IEnumerable<FileConverterOperationResult> DeleteOrphanCacheItem(IEnumerable<FileConverterOperationResult> queueTasks)
    {
        var listTasks = queueTasks.ToList();

        if (listTasks.RemoveAll(IsOrphanCacheItem) > 0)
        {
            SaveToCache(listTasks);
        }

        return listTasks;
    }

    private void SaveToCache(IEnumerable<FileConverterOperationResult> queueTasks)
    {
        if (!queueTasks.Any())
        {
            _distributedCache.Remove(GetCacheKey());

            return;
        }

        using var ms = new MemoryStream();

        ProtoBuf.Serializer.Serialize(ms, queueTasks);

        _distributedCache.Set(GetCacheKey(), ms.ToArray(), new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(15)
        });
    }

    private string GetCacheKey()
    {
        return $"{_cache_key_prefix}_{typeof(T).Name}".ToLowerInvariant();
    }

    private IEnumerable<FileConverterOperationResult> LoadFromCache()
    {
        var serializedObject = _distributedCache.Get(GetCacheKey());

        if (serializedObject == null)
        {
            return new List<FileConverterOperationResult>();
        }

        using var ms = new MemoryStream(serializedObject);

        return ProtoBuf.Serializer.Deserialize<List<FileConverterOperationResult>>(ms);
    }
}

public class FileJsonSerializerData<T>
{
    public T Id { get; set; }
    public string Title { get; set; }
    public int Version { get; set; }
    public T FolderID { get; set; }
    public string FolderTitle { get; set; }
    public string FileJson { get; set; }
}

[Scope(Additional = typeof(FileConverterExtension))]
public class FileConverter
{
    private readonly FileUtility _fileUtility;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly IDaoFactory _daoFactory;
    private readonly SetupInfo _setupInfo;
    private readonly PathProvider _pathProvider;
    private readonly FileSecurity _fileSecurity;
    private readonly FileMarker _fileMarker;
    private readonly TenantManager _tenantManager;
    private readonly AuthContext _authContext;
    private readonly EntryManager _entryManager;
    private readonly FilesSettingsHelper _filesSettingsHelper;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly FilesMessageService _filesMessageService;
    private readonly FileShareLink _fileShareLink;
    private readonly DocumentServiceHelper _documentServiceHelper;
    private readonly DocumentServiceConnector _documentServiceConnector;
    private readonly FileTrackerHelper _fileTracker;
    private readonly BaseCommonLinkUtility _baseCommonLinkUtility;
    private readonly EntryStatusManager _entryStatusManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpContextAccessor _httpContextAccesor;
    private readonly IHttpClientFactory _clientFactory;
    private readonly SocketManager _socketManager;

    public FileConverter(
        FileUtility fileUtility,
        FilesLinkUtility filesLinkUtility,
        IDaoFactory daoFactory,
        SetupInfo setupInfo,
        PathProvider pathProvider,
        FileSecurity fileSecurity,
        FileMarker fileMarker,
        TenantManager tenantManager,
        AuthContext authContext,
        EntryManager entryManager,
        FilesSettingsHelper filesSettingsHelper,
        GlobalFolderHelper globalFolderHelper,
        FilesMessageService filesMessageService,
        FileShareLink fileShareLink,
        DocumentServiceHelper documentServiceHelper,
        DocumentServiceConnector documentServiceConnector,
        FileTrackerHelper fileTracker,
        BaseCommonLinkUtility baseCommonLinkUtility,
        EntryStatusManager entryStatusManager,
        IServiceProvider serviceProvider,
        IHttpClientFactory clientFactory,
        SocketManager socketManager)
    {
        _fileUtility = fileUtility;
        _filesLinkUtility = filesLinkUtility;
        _daoFactory = daoFactory;
        _setupInfo = setupInfo;
        _pathProvider = pathProvider;
        _fileSecurity = fileSecurity;
        _fileMarker = fileMarker;
        _tenantManager = tenantManager;
        _authContext = authContext;
        _entryManager = entryManager;
        _filesSettingsHelper = filesSettingsHelper;
        _globalFolderHelper = globalFolderHelper;
        _filesMessageService = filesMessageService;
        _fileShareLink = fileShareLink;
        _documentServiceHelper = documentServiceHelper;
        _documentServiceConnector = documentServiceConnector;
        _fileTracker = fileTracker;
        _baseCommonLinkUtility = baseCommonLinkUtility;
        _entryStatusManager = entryStatusManager;
        _serviceProvider = serviceProvider;
        _clientFactory = clientFactory;
        _socketManager = socketManager;
    }

    public FileConverter(
        FileUtility fileUtility,
        FilesLinkUtility filesLinkUtility,
        IDaoFactory daoFactory,
        SetupInfo setupInfo,
        PathProvider pathProvider,
        FileSecurity fileSecurity,
        FileMarker fileMarker,
        TenantManager tenantManager,
        AuthContext authContext,
        EntryManager entryManager,
        FilesSettingsHelper filesSettingsHelper,
        GlobalFolderHelper globalFolderHelper,
        FilesMessageService filesMessageService,
        FileShareLink fileShareLink,
        DocumentServiceHelper documentServiceHelper,
        DocumentServiceConnector documentServiceConnector,
        FileTrackerHelper fileTracker,
        BaseCommonLinkUtility baseCommonLinkUtility,
        EntryStatusManager entryStatusManager,
        IServiceProvider serviceProvider,
        IHttpContextAccessor httpContextAccesor,
        IHttpClientFactory clientFactory,
        SocketManager socketManager)
        : this(fileUtility, filesLinkUtility, daoFactory, setupInfo, pathProvider, fileSecurity,
              fileMarker, tenantManager, authContext, entryManager, filesSettingsHelper,
              globalFolderHelper, filesMessageService, fileShareLink, documentServiceHelper, documentServiceConnector, fileTracker,
              baseCommonLinkUtility, entryStatusManager, serviceProvider, clientFactory, socketManager)
    {
        _httpContextAccesor = httpContextAccesor;
    }

    public bool EnableAsUploaded => _fileUtility.ExtsMustConvert.Count > 0 && !string.IsNullOrEmpty(_filesLinkUtility.DocServiceConverterUrl);

    public bool MustConvert<T>(File<T> file)
    {
        if (file == null)
        {
            return false;
        }

        var ext = FileUtility.GetFileExtension(file.Title);

        return _fileUtility.ExtsMustConvert.Contains(ext);
    }

    public bool EnableConvert<T>(File<T> file, string toExtension)
    {
        if (file == null || string.IsNullOrEmpty(toExtension))
        {
            return false;
        }

        if (file.Encrypted)
        {
            return false;
        }

        var fileExtension = file.ConvertedExtension;
        if (fileExtension.Trim('.').Equals(toExtension.Trim('.'), StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        fileExtension = FileUtility.GetFileExtension(file.Title);
        if (_fileUtility.InternalExtension.ContainsValue(toExtension))
        {
            return true;
        }

        return _fileUtility.ExtsConvertible.ContainsKey(fileExtension) && _fileUtility.ExtsConvertible[fileExtension].Contains(toExtension);
    }

    public Task<Stream> ExecAsync<T>(File<T> file)
    {
        return ExecAsync(file, _fileUtility.GetInternalExtension(file.Title));
    }

    public async Task<Stream> ExecAsync<T>(File<T> file, string toExtension, string password = null)
    {
        if (!EnableConvert(file, toExtension))
        {
            var fileDao = _daoFactory.GetFileDao<T>();

            return await fileDao.GetFileStreamAsync(file);
        }

        if (file.ContentLength > _setupInfo.AvailableFileSize)
        {
            throw new Exception(string.Format(FilesCommonResource.ErrorMassage_FileSizeConvert, FileSizeComment.FilesSizeToString(_setupInfo.AvailableFileSize)));
        }

        var fileUri = _pathProvider.GetFileStreamUrl(file);
        var docKey = _documentServiceHelper.GetDocKey(file);
        fileUri = _documentServiceConnector.ReplaceCommunityAdress(fileUri);

        var uriTuple = await _documentServiceConnector.GetConvertedUriAsync(fileUri, file.ConvertedExtension, toExtension, docKey, password, CultureInfo.CurrentUICulture.Name, null, null, false);
        var convertUri = uriTuple.ConvertedDocumentUri;
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(convertUri)
        };

        var httpClient = _clientFactory.CreateClient();
        var response = await httpClient.SendAsync(request);

        return new ResponseStream(response);
    }

    public async Task<FileOperationResult> ExecSynchronouslyAsync<T>(File<T> file, string doc)
    {
        var fileDao = _daoFactory.GetFileDao<T>();

        if (!await _fileSecurity.CanReadAsync(file))
        {
            (var readLink, file, _) = await _fileShareLink.CheckAsync(doc, true, fileDao);
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file), FilesCommonResource.ErrorMassage_FileNotFound);
            }
            if (!readLink)
            {
                throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
            }
        }

        var fileUri = _pathProvider.GetFileStreamUrl(file);
        var fileExtension = file.ConvertedExtension;
        var toExtension = _fileUtility.GetInternalExtension(file.Title);
        var docKey = _documentServiceHelper.GetDocKey(file);

        fileUri = _documentServiceConnector.ReplaceCommunityAdress(fileUri);

        var uriTuple = await _documentServiceConnector.GetConvertedUriAsync(fileUri, fileExtension, toExtension, docKey, null, CultureInfo.CurrentUICulture.Name, null, null, false);
        var convertUri = uriTuple.ConvertedDocumentUri;

        var operationResult = new FileConverterOperationResult
        {
            Source = JsonSerializer.Serialize(new { id = file.Id, version = file.Version }),
            OperationType = FileOperationType.Convert,
            Error = string.Empty,
            Progress = 0,
            Result = string.Empty,
            Processed = "",
            Id = string.Empty,
            TenantId = _tenantManager.GetCurrentTenant().Id,
            Account = _authContext.CurrentAccount.ID,
            Delete = false,
            StartDateTime = DateTime.UtcNow,
            Url = _httpContextAccesor?.HttpContext != null ? _httpContextAccesor.HttpContext.Request.GetDisplayUrl() : null,
            Password = null,
            ServerRootPath = _baseCommonLinkUtility.ServerRootPath
        };

        var operationResultError = string.Empty;

        var newFile = await SaveConvertedFileAsync(file, convertUri);
        if (newFile != null)
        {
            await _socketManager.CreateFileAsync(file);
            var folderDao = _daoFactory.GetFolderDao<T>();
            var folder = await folderDao.GetFolderAsync(newFile.ParentId);
            var folderTitle = await _fileSecurity.CanReadAsync(folder) ? folder.Title : null;
            operationResult.Result = await GetFileConverter<T>().FileJsonSerializerAsync(_entryStatusManager, newFile, folderTitle);
        }

        operationResult.Progress = 100;
        operationResult.StopDateTime = DateTime.UtcNow;
        operationResult.Processed = "1";

        if (!string.IsNullOrEmpty(operationResultError))
        {
            operationResult.Error = operationResultError;
        }

        return operationResult;
    }

    public async Task ExecAsynchronouslyAsync<T>(File<T> file, bool deleteAfter, string password = null)
    {
        if (!MustConvert(file))
        {
            throw new ArgumentException(FilesCommonResource.ErrorMassage_NotSupportedFormat);
        }
        if (!string.IsNullOrEmpty(file.ConvertedType) || _fileUtility.InternalExtension.ContainsValue(FileUtility.GetFileExtension(file.Title)))
        {
            return;
        }

        await _fileMarker.RemoveMarkAsNewAsync(file);
        GetFileConverter<T>().Add(file, password, _tenantManager.GetCurrentTenant().Id, _authContext.CurrentAccount, deleteAfter, _httpContextAccesor?.HttpContext != null ? _httpContextAccesor.HttpContext.Request.GetDisplayUrl() : null, _baseCommonLinkUtility.ServerRootPath);
    }

    public bool IsConverting<T>(File<T> file)
    {
        if (!MustConvert(file) || !string.IsNullOrEmpty(file.ConvertedType))
        {
            return false;
        }

        return GetFileConverter<T>().IsConverting(file);
    }

    public async IAsyncEnumerable<FileOperationResult> GetStatusAsync<T>(IEnumerable<KeyValuePair<File<T>, bool>> filesPair)
    {
        var result = new List<FileOperationResult>();
        foreach (var pair in filesPair)
        {
            var r = await GetFileConverter<T>().GetStatusAsync(pair, _fileSecurity);

            if (r != null)
            {
                yield return r;
            }
        }
    }

    public async Task<File<T>> SaveConvertedFileAsync<T>(File<T> file, string convertedFileUrl)
    {
        var fileDao = _daoFactory.GetFileDao<T>();
        var folderDao = _daoFactory.GetFolderDao<T>();
        File<T> newFile = null;
        var markAsTemplate = false;
        var isNewFile = false;

        var newFileTitle = FileUtility.ReplaceFileExtension(file.Title, _fileUtility.GetInternalExtension(file.Title));

        if (!_filesSettingsHelper.StoreOriginalFiles && await _fileSecurity.CanEditAsync(file))
        {
            newFile = (File<T>)file.Clone();
            newFile.Version++;
            markAsTemplate = FileUtility.ExtsTemplate.Contains(FileUtility.GetFileExtension(file.Title), StringComparer.CurrentCultureIgnoreCase)
                          && _fileUtility.ExtsWebTemplate.Contains(FileUtility.GetFileExtension(newFileTitle), StringComparer.CurrentCultureIgnoreCase);
        }
        else
        {
            var folderId = _globalFolderHelper.GetFolderMy<T>();

            var parent = await folderDao.GetFolderAsync(file.ParentId);
            if (parent != null
                && await _fileSecurity.CanCreateAsync(parent))
            {
                folderId = parent.Id;
            }

            if (Equals(folderId, 0))
            {
                throw new SecurityException(FilesCommonResource.ErrorMassage_FolderNotFound);
            }

            if (_filesSettingsHelper.UpdateIfExist && (parent != null && !folderId.Equals(parent.Id) || !file.ProviderEntry))
            {
                newFile = await fileDao.GetFileAsync(folderId, newFileTitle);
                if (newFile != null && await _fileSecurity.CanEditAsync(newFile) && !await _entryManager.FileLockedForMeAsync(newFile.Id) && !_fileTracker.IsEditing(newFile.Id))
                {
                    newFile.Version++;
                    newFile.VersionGroup++;
                }
                else
                {
                    newFile = null;
                }
            }

            if (newFile == null)
            {
                newFile = _serviceProvider.GetService<File<T>>();
                newFile.ParentId = folderId;
                isNewFile = true;
            }
        }

        newFile.Title = newFileTitle;
        newFile.ConvertedType = null;
        newFile.Comment = string.Format(FilesCommonResource.CommentConvert, file.Title);
        newFile.ThumbnailStatus = Thumbnail.Waiting;

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(convertedFileUrl)
        };

        var httpClient = _clientFactory.CreateClient();

        try
        {
            using var response = await httpClient.SendAsync(request);
            using var convertedFileStream = new ResponseStream(response);
            newFile.ContentLength = convertedFileStream.Length;
            newFile = await fileDao.SaveFileAsync(newFile, convertedFileStream);

            if (!isNewFile)
            {
                await _socketManager.UpdateFileAsync(newFile);
            }
            else
            {
                await _socketManager.CreateFileAsync(newFile);
            }
        }
        catch (HttpRequestException e)
        {
            var errorString = $"HttpRequestException: {e.StatusCode}";

            if (e.StatusCode != HttpStatusCode.NotFound)
            {
                if (e.Message != null)
                {
                    errorString += $" Error {e.Message}";
                }
            }

            throw new Exception(errorString);
        }

        _ = _filesMessageService.Send(newFile, MessageInitiator.DocsService, MessageAction.FileConverted, newFile.Title);

        var linkDao = _daoFactory.GetLinkDao();
        await linkDao.DeleteAllLinkAsync(file.Id.ToString());

        await _fileMarker.MarkAsNewAsync(newFile);

        var tagDao = _daoFactory.GetTagDao<T>();
        var tags = await tagDao.GetTagsAsync(file.Id, FileEntryType.File, TagType.System).ToListAsync();
        if (tags.Count > 0)
        {
            tags.ForEach(r => r.EntryId = newFile.Id);
            await tagDao.SaveTags(tags);
        }

        if (markAsTemplate)
        {
            await tagDao.SaveTags(Tag.Template(_authContext.CurrentAccount.ID, newFile));
        }

        return newFile;
    }

    private FileConverterQueue<T> GetFileConverter<T>()
    {
        return _serviceProvider.GetService<FileConverterQueue<T>>();
    }
}

internal class FileComparer<T> : IEqualityComparer<File<T>>
{
    public bool Equals(File<T> x, File<T> y)
    {
        return x != null && y != null && Equals(x.Id, y.Id) && x.Version == y.Version;
    }

    public int GetHashCode(File<T> obj)
    {
        return obj.Id.GetHashCode() + obj.Version.GetHashCode();
    }
}


public static class FileConverterExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<FileConverterQueue<int>>();
        services.TryAdd<FileConverterQueue<string>>();
    }
}
