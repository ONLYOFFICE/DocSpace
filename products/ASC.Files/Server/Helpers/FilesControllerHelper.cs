using FileShare = ASC.Files.Core.Security.FileShare;
using MimeMapping = ASC.Common.Web.MimeMapping;
using SortedByType = ASC.Files.Core.SortedByType;

namespace ASC.Files.Helpers;

[Scope]
public class FilesControllerHelper<T>
{
    private readonly ApiContext _apiContext;
    private readonly FileStorageService<T> _fileStorageService;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly FileUtility _fileUtility;
    private readonly FileDtoHelper _fileDtoHelper;
    private readonly FilesSettingsHelper _filesSettingsHelper;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly FileUploader _fileUploader;
    private readonly DocumentServiceHelper _documentServiceHelper;
    private readonly TenantManager _tenantManager;
    private readonly SecurityContext _securityContext;
    private readonly FolderDtoHelper _folderDtoHelper;
    private readonly FileOperationDtoHelper _fileOperationDtoHelper;
    private readonly FileShareDtoHelper _fileShareDtoHelper;
    private readonly FileShareParamsHelper _fileShareParamsHelper;
    private readonly EntryManager _entryManager;
    private readonly FolderContentDtoHelper _folderContentDtoHelper;
    private readonly ChunkedUploadSessionHelper _chunkedUploadSessionHelper;
    private readonly DocumentServiceTrackerHelper _documentServiceTracker;
    private readonly SettingsManager _settingsManager;
    private readonly EncryptionKeyPairDtoHelper _encryptionKeyPairHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly FileConverter _fileConverter;
    private readonly ApiDateTimeHelper _apiDateTimeHelper;
    private readonly UserManager _userManager;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    public readonly SocketManager _socketManager;
    public readonly IServiceProvider _serviceProvider;
    private readonly ILog _logger;
    private readonly IHttpClientFactory _clientFactory;

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="fileStorageService"></param>
    public FilesControllerHelper(
        ApiContext context,
        FileStorageService<T> fileStorageService,
        FileDtoHelper fileDtoHelper,
        FilesSettingsHelper filesSettingsHelper,
        FilesLinkUtility filesLinkUtility,
        FileUploader fileUploader,
        DocumentServiceHelper documentServiceHelper,
        TenantManager tenantManager,
        SecurityContext securityContext,
        FolderDtoHelper folderDtoHelper,
        FileOperationDtoHelper fileOperationDtoHelper,
        FileShareDtoHelper fileShareDtoHelper,
        FileShareParamsHelper fileShareParamsHelper,
        EntryManager entryManager,
        FolderContentDtoHelper folderContentDtoHelper,
        ChunkedUploadSessionHelper chunkedUploadSessionHelper,
        DocumentServiceTrackerHelper documentServiceTracker,
        IOptionsMonitor<ILog> optionMonitor,
        SettingsManager settingsManager,
        EncryptionKeyPairDtoHelper encryptionKeyPairHelper,
        IHttpContextAccessor httpContextAccessor,
        FileConverter fileConverter,
        ApiDateTimeHelper apiDateTimeHelper,
        UserManager userManager,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        IServiceProvider serviceProvider,
        SocketManager socketManager,
        IHttpClientFactory clientFactory,
        GlobalFolderHelper globalFolderHelper,
        CoreBaseSettings coreBaseSettings,
        FileUtility fileUtility)
    {
        _apiContext = context;
        _fileStorageService = fileStorageService;
        _fileDtoHelper = fileDtoHelper;
        _filesSettingsHelper = filesSettingsHelper;
        _filesLinkUtility = filesLinkUtility;
        _fileUploader = fileUploader;
        _documentServiceHelper = documentServiceHelper;
        _tenantManager = tenantManager;
        _securityContext = securityContext;
        _folderDtoHelper = folderDtoHelper;
        _fileOperationDtoHelper = fileOperationDtoHelper;
        _fileShareDtoHelper = fileShareDtoHelper;
        _fileShareParamsHelper = fileShareParamsHelper;
        _entryManager = entryManager;
        _folderContentDtoHelper = folderContentDtoHelper;
        _chunkedUploadSessionHelper = chunkedUploadSessionHelper;
        _documentServiceTracker = documentServiceTracker;
        _settingsManager = settingsManager;
        _encryptionKeyPairHelper = encryptionKeyPairHelper;
        _apiDateTimeHelper = apiDateTimeHelper;
        _userManager = userManager;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _serviceProvider = serviceProvider;
        _socketManager = socketManager;
        _httpContextAccessor = httpContextAccessor;
        _fileConverter = fileConverter;
        _logger = optionMonitor.Get("ASC.Files");
        _clientFactory = clientFactory;
        _globalFolderHelper = globalFolderHelper;
        _fileUtility = fileUtility;
        _coreBaseSettings = coreBaseSettings;
    }

    public async Task<SortedSet<int>> GetRootFoldersIdsAsync(bool withoutTrash, bool withoutAdditionalFolder)
    {
        var IsVisitor = _userManager.GetUsers(_securityContext.CurrentAccount.ID).IsVisitor(_userManager);
        var IsOutsider = _userManager.GetUsers(_securityContext.CurrentAccount.ID).IsOutsider(_userManager);
        var folders = new SortedSet<int>();

        if (IsOutsider)
        {
            withoutTrash = true;
            withoutAdditionalFolder = true;
        }

        if (!IsVisitor)
        {
            folders.Add(_globalFolderHelper.FolderMy);
        }

        if (!_coreBaseSettings.Personal && !_userManager.GetUsers(_securityContext.CurrentAccount.ID).IsOutsider(_userManager))
        {
            folders.Add(await _globalFolderHelper.FolderShareAsync);
        }

        if (!IsVisitor && !withoutAdditionalFolder)
        {
            if (_filesSettingsHelper.FavoritesSection)
            {
                folders.Add(await _globalFolderHelper.FolderFavoritesAsync);
            }
            if (_filesSettingsHelper.RecentSection)
            {
                folders.Add(await _globalFolderHelper.FolderRecentAsync);
            }

            if (!_coreBaseSettings.Personal && PrivacyRoomSettings.IsAvailable(_tenantManager))
            {
                folders.Add(await _globalFolderHelper.FolderPrivacyAsync);
            }
        }

        if (!_coreBaseSettings.Personal)
        {
            folders.Add(await _globalFolderHelper.FolderCommonAsync);
        }

        if (!IsVisitor
           && !withoutAdditionalFolder
           && _fileUtility.ExtsWebTemplate.Count > 0
           && _filesSettingsHelper.TemplatesSection)
        {
            folders.Add(await _globalFolderHelper.FolderTemplatesAsync);
        }

        if (!withoutTrash)
        {
            folders.Add((int)_globalFolderHelper.FolderTrash);
        }

        return folders;
    }

    public async Task<FolderContentDto<T>> GetFolderAsync(T folderId, Guid userIdOrGroupId, FilterType filterType, bool withSubFolders)
    {
        var folderContentWrapper = await ToFolderContentWrapperAsync(folderId, userIdOrGroupId, filterType, withSubFolders);

        return folderContentWrapper.NotFoundIfNull();
    }

    public async Task<object> UploadFileAsync(T folderId, UploadRequestDto uploadModel)
    {
        if (uploadModel.StoreOriginalFileFlag.HasValue)
        {
            _filesSettingsHelper.StoreOriginalFiles = uploadModel.StoreOriginalFileFlag.Value;
        }

        IEnumerable<IFormFile> files = _httpContextAccessor.HttpContext.Request.Form.Files;
        if (files == null || !files.Any())
        {
            files = uploadModel.Files;
        }

        if (files != null && files.Any())
        {
            if (files.Count() == 1)
            {
                //Only one file. return it
                var postedFile = files.First();

                return await InsertFileAsync(folderId, postedFile.OpenReadStream(), postedFile.FileName, uploadModel.CreateNewIfExist, uploadModel.KeepConvertStatus);
            }

            //For case with multiple files
            var result = new List<object>();

            foreach (var postedFile in uploadModel.Files)
            {
                result.Add(await InsertFileAsync(folderId, postedFile.OpenReadStream(), postedFile.FileName, uploadModel.CreateNewIfExist, uploadModel.KeepConvertStatus));
            }

            return result;
        }

        if (uploadModel.File != null)
        {
            var fileName = "file" + MimeMapping.GetExtention(uploadModel.ContentType.MediaType);
            if (uploadModel.ContentDisposition != null)
            {
                fileName = uploadModel.ContentDisposition.FileName;
            }

            return new List<FileDto<T>>
            {
                await InsertFileAsync(folderId, uploadModel.File.OpenReadStream(), fileName, uploadModel.CreateNewIfExist, uploadModel.KeepConvertStatus)
            };
        }

        throw new InvalidOperationException("No input files");
    }

    public async Task<FileDto<T>> InsertFileAsync(T folderId, Stream file, string title, bool? createNewIfExist, bool keepConvertStatus = false)
    {
        try
        {
            var resultFile = await _fileUploader.ExecAsync(folderId, title, file.Length, file, createNewIfExist ?? !_filesSettingsHelper.UpdateIfExist, !keepConvertStatus);

            await _socketManager.CreateFileAsync(resultFile);

            return await _fileDtoHelper.GetAsync(resultFile);
        }
        catch (FileNotFoundException e)
        {
            throw new ItemNotFoundException("File not found", e);
        }
        catch (DirectoryNotFoundException e)
        {
            throw new ItemNotFoundException("Folder not found", e);
        }
    }

    public async Task<FileDto<T>> UpdateFileStreamAsync(Stream file, T fileId, string fileExtension, bool encrypted = false, bool forcesave = false)
    {
        try
        {
            var resultFile = await _fileStorageService.UpdateFileStreamAsync(fileId, file, fileExtension, encrypted, forcesave);

            return await _fileDtoHelper.GetAsync(resultFile);
        }
        catch (FileNotFoundException e)
        {
            throw new ItemNotFoundException("File not found", e);
        }
    }

    public async Task<FileDto<T>> SaveEditingAsync(T fileId, string fileExtension, string downloadUri, Stream stream, string doc, bool forcesave)
    {
        return await _fileDtoHelper.GetAsync(await _fileStorageService.SaveEditingAsync(fileId, fileExtension, downloadUri, stream, doc, forcesave));
    }

    public Task<string> StartEditAsync(T fileId, bool editingAlone, string doc)
    {
        return _fileStorageService.StartEditAsync(fileId, editingAlone, doc);
    }

    public Task<KeyValuePair<bool, string>> TrackEditFileAsync(T fileId, Guid tabId, string docKeyForTrack, string doc, bool isFinish)
    {
        return _fileStorageService.TrackEditFileAsync(fileId, tabId, docKeyForTrack, doc, isFinish);
    }

    public async Task<Configuration<T>> OpenEditAsync(T fileId, int version, string doc, bool view)
    {
        var docParams = await _documentServiceHelper.GetParamsAsync(fileId, version, doc, true, !view, true);
        var configuration = docParams.Configuration;

        configuration.EditorType = EditorType.External;
        if (configuration.EditorConfig.ModeWrite)
        {
            configuration.EditorConfig.CallbackUrl = _documentServiceTracker.GetCallbackUrl(configuration.Document.Info.GetFile().Id.ToString());
        }

        if (configuration.Document.Info.GetFile().RootFolderType == FolderType.Privacy && PrivacyRoomSettings.GetEnabled(_settingsManager))
        {
            var keyPair = _encryptionKeyPairHelper.GetKeyPair();
            if (keyPair != null)
            {
                configuration.EditorConfig.EncryptionKeys = new EncryptionKeysConfig
                {
                    PrivateKeyEnc = keyPair.PrivateKeyEnc,
                    PublicKey = keyPair.PublicKey,
                };
            }
        }

        if (!configuration.Document.Info.GetFile().Encrypted && !configuration.Document.Info.GetFile().ProviderEntry) _entryManager.MarkAsRecent(configuration.Document.Info.GetFile());

        configuration.Token = _documentServiceHelper.GetSignature(configuration);

        return configuration;
    }

    public async Task<object> CreateUploadSessionAsync(T folderId, string fileName, long fileSize, string relativePath, ApiDateTime lastModified, bool encrypted)
    {
        var file = await _fileUploader.VerifyChunkedUploadAsync(folderId, fileName, fileSize, _filesSettingsHelper.UpdateIfExist, lastModified, relativePath);

        if (_filesLinkUtility.IsLocalFileUploader)
        {
            var session = await _fileUploader.InitiateUploadAsync(file.ParentId, file.Id ?? default, file.Title, file.ContentLength, encrypted);

            var responseObject = await _chunkedUploadSessionHelper.ToResponseObjectAsync(session, true);

            return new
            {
                success = true,
                data = responseObject
            };
        }

        var createSessionUrl = _filesLinkUtility.GetInitiateUploadSessionUrl(_tenantManager.GetCurrentTenant().Id, file.ParentId, file.Id, file.Title, file.ContentLength, encrypted, _securityContext);

        var httpClient = _clientFactory.CreateClient();

        var request = new HttpRequestMessage();
        request.RequestUri = new Uri(createSessionUrl);
        request.Method = HttpMethod.Post;

        // hack for uploader.onlyoffice.com in api requests
        var rewriterHeader = _apiContext.HttpContextAccessor.HttpContext.Request.Headers[HttpRequestExtensions.UrlRewriterHeader];
        if (!string.IsNullOrEmpty(rewriterHeader))
        {
            request.Headers.Add(HttpRequestExtensions.UrlRewriterHeader, rewriterHeader.ToString());
        }

        using var response = await httpClient.SendAsync(request);
        using var responseStream = await response.Content.ReadAsStreamAsync();
        using var streamReader = new StreamReader(responseStream);

        return JObject.Parse(await streamReader.ReadToEndAsync()); //result is json string
    }

    public Task<FileDto<T>> CreateTextFileAsync(T folderId, string title, string content)
    {
        ArgumentNullException.ThrowIfNull(title);

        //Try detect content
        var extension = ".txt";
        if (!string.IsNullOrEmpty(content))
        {
            if (Regex.IsMatch(content, @"<([^\s>]*)(\s[^<]*)>"))
            {
                extension = ".html";
            }
        }

        return CreateFileAsync(folderId, title, content, extension);
    }

    private async Task<FileDto<T>> CreateFileAsync(T folderId, string title, string content, string extension)
    {
        using var memStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var file = await _fileUploader.ExecAsync(folderId,
                          title.EndsWith(extension, StringComparison.OrdinalIgnoreCase) ? title : (title + extension),
                          memStream.Length, memStream);

        return await _fileDtoHelper.GetAsync(file);
    }

    public Task<FileDto<T>> CreateHtmlFileAsync(T folderId, string title, string content)
    {
        ArgumentNullException.ThrowIfNull(title);

        return CreateFileAsync(folderId, title, content, ".html");
    }

    public async Task<FolderDto<T>> CreateFolderAsync(T folderId, string title)
    {
        var folder = await _fileStorageService.CreateNewFolderAsync(folderId, title);

        return await _folderDtoHelper.GetAsync(folder);
    }

    public async Task<FileDto<T>> CreateFileAsync(T folderId, string title, JsonElement templateId, bool enableExternalExt = false)
    {
        File<T> file;

        if (templateId.ValueKind == JsonValueKind.Number)
        {
            file = await _fileStorageService.CreateNewFileAsync(new FileModel<T, int> { ParentId = folderId, Title = title, TemplateId = templateId.GetInt32() }, enableExternalExt);
        }
        else if (templateId.ValueKind == JsonValueKind.String)
        {
            file = await _fileStorageService.CreateNewFileAsync(new FileModel<T, string> { ParentId = folderId, Title = title, TemplateId = templateId.GetString() }, enableExternalExt);
        }
        else
        {
            file = await _fileStorageService.CreateNewFileAsync(new FileModel<T, int> { ParentId = folderId, Title = title, TemplateId = 0 }, enableExternalExt);
        }

        return await _fileDtoHelper.GetAsync(file);
    }

    public async Task<FolderDto<T>> RenameFolderAsync(T folderId, string title)
    {
        var folder = await _fileStorageService.FolderRenameAsync(folderId, title);

        return await _folderDtoHelper.GetAsync(folder);
    }

    public async Task<FolderDto<T>> GetFolderInfoAsync(T folderId)
    {
        var folder = await _fileStorageService.GetFolderAsync(folderId).NotFoundIfNull("Folder not found");

        return await _folderDtoHelper.GetAsync(folder);
    }

    public async IAsyncEnumerable<FileEntryDto> GetFolderPathAsync(T folderId)
    {
        var breadCrumbs = await _entryManager.GetBreadCrumbsAsync(folderId);

        foreach (var e in breadCrumbs)
        {
            yield return await GetFileEntryWrapperAsync(e);
        }
    }

    public async Task<FileDto<T>> GetFileInfoAsync(T fileId, int version = -1)
    {
        var file = await _fileStorageService.GetFileAsync(fileId, version);
        file = file.NotFoundIfNull("File not found");

        return await _fileDtoHelper.GetAsync(file);
    }

    public async Task<FileDto<TTemplate>> CopyFileAsAsync<TTemplate>(T fileId, TTemplate destFolderId, string destTitle, string password = null)
    {
        var service = _serviceProvider.GetService<FileStorageService<TTemplate>>();
        var controller = _serviceProvider.GetService<FilesControllerHelper<TTemplate>>();
        var file = await _fileStorageService.GetFileAsync(fileId, -1);
        var ext = FileUtility.GetFileExtension(file.Title);
        var destExt = FileUtility.GetFileExtension(destTitle);

        if (ext == destExt)
        {
            var newFile = await service.CreateNewFileAsync(new FileModel<TTemplate, T> { ParentId = destFolderId, Title = destTitle, TemplateId = fileId }, false);

            return await _fileDtoHelper.GetAsync(newFile);
        }

        using (var fileStream = await _fileConverter.ExecAsync(file, destExt, password))
        {
            return await controller.InsertFileAsync(destFolderId, fileStream, destTitle, true);
        }
    }

    public async Task<FileDto<T>> AddToRecentAsync(T fileId, int version = -1)
    {
        var file = await _fileStorageService.GetFileAsync(fileId, version).NotFoundIfNull("File not found");
        _entryManager.MarkAsRecent(file);

        return await _fileDtoHelper.GetAsync(file);
    }

    public async Task<List<FileEntryDto>> GetNewItemsAsync(T folderId)
    {
        var newItems = await _fileStorageService.GetNewItemsAsync(folderId);
        var result = new List<FileEntryDto>();

        foreach (var e in newItems)
        {
            result.Add(await GetFileEntryWrapperAsync(e));
        }

        return result;
    }

    public async Task<FileDto<T>> UpdateFileAsync(T fileId, string title, int lastVersion)
    {
        if (!string.IsNullOrEmpty(title))
        {
            await _fileStorageService.FileRenameAsync(fileId, title);
        }

        if (lastVersion > 0)
        {
            await _fileStorageService.UpdateToVersionAsync(fileId, lastVersion);
        }

        return await GetFileInfoAsync(fileId);
    }

    public async Task<IEnumerable<FileOperationDto>> DeleteFileAsync(T fileId, bool deleteAfter, bool immediately)
    {
        var result = new List<FileOperationDto>();

        foreach (var e in _fileStorageService.DeleteFile("delete", fileId, false, deleteAfter, immediately))
        {
            result.Add(await _fileOperationDtoHelper.GetAsync(e));
        }

        return result;
    }

    public IAsyncEnumerable<ConversationResultDto<T>> StartConversionAsync(CheckConversionRequestDto<T> cheqConversionRequestDto)
    {
        cheqConversionRequestDto.StartConvert = true;

        return CheckConversionAsync(cheqConversionRequestDto);
    }

    public async IAsyncEnumerable<ConversationResultDto<T>> CheckConversionAsync(CheckConversionRequestDto<T> cheqConversionRequestDto)
    {
        var checkConversaion = _fileStorageService.CheckConversionAsync(new List<CheckConversionRequestDto<T>>() { cheqConversionRequestDto }, cheqConversionRequestDto.Sync);

        await foreach (var r in checkConversaion)
        {
            var o = new ConversationResultDto<T>
            {
                Id = r.Id,
                Error = r.Error,
                OperationType = r.OperationType,
                Processed = r.Processed,
                Progress = r.Progress,
                Source = r.Source,
            };

            if (!string.IsNullOrEmpty(r.Result))
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        AllowTrailingCommas = true,
                        PropertyNameCaseInsensitive = true
                    };

                    var jResult = JsonSerializer.Deserialize<FileJsonSerializerData<T>>(r.Result, options);
                    o.File = await GetFileInfoAsync(jResult.Id, jResult.Version);
                }
                catch (Exception e)
                {
                    o.File = r.Result;
                    _logger.Error(e);
                }
            }

            yield return o;
        }
    }

    public Task<string> CheckFillFormDraftAsync(T fileId, int version, string doc, bool editPossible, bool view)
    {
        return _fileStorageService.CheckFillFormDraftAsync(fileId, version, doc, editPossible, view);
    }

    public async Task<IEnumerable<FileOperationDto>> DeleteFolder(T folderId, bool deleteAfter, bool immediately)
    {
        var result = new List<FileOperationDto>();

        foreach (var e in _fileStorageService.DeleteFolder("delete", folderId, false, deleteAfter, immediately))
        {
            result.Add(await _fileOperationDtoHelper.GetAsync(e));
        }

        return result;
    }

    public async IAsyncEnumerable<FileEntryDto> MoveOrCopyBatchCheckAsync(BatchRequestDto batchRequestDto)
    {
        List<object> checkedFiles;
        List<object> checkedFolders;

        if (batchRequestDto.DestFolderId.ValueKind == JsonValueKind.Number)
        {
            (checkedFiles, checkedFolders) = await _fileStorageService.MoveOrCopyFilesCheckAsync(batchRequestDto.FileIds.ToList(), batchRequestDto.FolderIds.ToList(), batchRequestDto.DestFolderId.GetInt32());
        }
        else
        {
            (checkedFiles, checkedFolders) = await _fileStorageService.MoveOrCopyFilesCheckAsync(batchRequestDto.FileIds.ToList(), batchRequestDto.FolderIds.ToList(), batchRequestDto.DestFolderId.GetString());
        }

        var entries = await _fileStorageService.GetItemsAsync(checkedFiles.OfType<int>().Select(Convert.ToInt32), checkedFiles.OfType<int>().Select(Convert.ToInt32), FilterType.FilesOnly, false, "", "");

        entries.AddRange(await _fileStorageService.GetItemsAsync(checkedFiles.OfType<string>(), checkedFiles.OfType<string>(), FilterType.FilesOnly, false, "", ""));

        foreach (var e in entries)
        {
            yield return await GetFileEntryWrapperAsync(e);
        }
    }

    public async Task<IEnumerable<FileOperationDto>> MoveBatchItemsAsync(BatchRequestDto batchRequestDto)
    {
        var result = new List<FileOperationDto>();

        foreach (var e in _fileStorageService.MoveOrCopyItems(batchRequestDto.FolderIds.ToList(), batchRequestDto.FileIds.ToList(), batchRequestDto.DestFolderId, batchRequestDto.ConflictResolveType, false, batchRequestDto.DeleteAfter))
        {
            result.Add(await _fileOperationDtoHelper.GetAsync(e));
        }

        return result;
    }

    public async Task<IEnumerable<FileOperationDto>> CopyBatchItemsAsync(BatchRequestDto batchRequestDto)
    {
        var result = new List<FileOperationDto>();

        foreach (var e in _fileStorageService.MoveOrCopyItems(batchRequestDto.FolderIds.ToList(), batchRequestDto.FileIds.ToList(), batchRequestDto.DestFolderId, batchRequestDto.ConflictResolveType, true, batchRequestDto.DeleteAfter))
        {
            result.Add(await _fileOperationDtoHelper.GetAsync(e));
        }

        return result;
    }

    public async Task<IEnumerable<FileOperationDto>> MarkAsReadAsync(BaseBatchRequestDto batchRequestDto)
    {
        var result = new List<FileOperationDto>();

        foreach (var e in _fileStorageService.MarkAsRead(batchRequestDto.FolderIds.ToList(), batchRequestDto.FileIds.ToList()))
        {
            result.Add(await _fileOperationDtoHelper.GetAsync(e));
        }

        return result;
    }

    public async Task<IEnumerable<FileOperationDto>> TerminateTasksAsync()
    {
        var result = new List<FileOperationDto>();

        foreach (var e in _fileStorageService.TerminateTasks())
        {
            result.Add(await _fileOperationDtoHelper.GetAsync(e));
        }

        return result;
    }

    public async Task<IEnumerable<FileOperationDto>> GetOperationStatusesAsync()
    {
        var result = new List<FileOperationDto>();

        foreach (var e in _fileStorageService.GetTasksStatuses())
        {
            result.Add(await _fileOperationDtoHelper.GetAsync(e));
        }

        return result;
    }

    public async Task<IEnumerable<FileOperationDto>> BulkDownloadAsync(DownloadRequestDto model)
    {
        var folders = new Dictionary<JsonElement, string>();
        var files = new Dictionary<JsonElement, string>();

        foreach (var fileId in model.FileConvertIds.Where(fileId => !files.ContainsKey(fileId.Key)))
        {
            files.Add(fileId.Key, fileId.Value);
        }

        foreach (var fileId in model.FileIds.Where(fileId => !files.ContainsKey(fileId)))
        {
            files.Add(fileId, string.Empty);
        }

        foreach (var folderId in model.FolderIds.Where(folderId => !folders.ContainsKey(folderId)))
        {
            folders.Add(folderId, string.Empty);
        }

        var result = new List<FileOperationDto>();

        foreach (var e in _fileStorageService.BulkDownload(folders, files))
        {
            result.Add(await _fileOperationDtoHelper.GetAsync(e));
        }

        return result;
    }

    public async Task<IEnumerable<FileOperationDto>> EmptyTrashAsync()
    {
        var emptyTrash = await _fileStorageService.EmptyTrashAsync();
        var result = new List<FileOperationDto>();

        foreach (var e in emptyTrash)
        {
            result.Add(await _fileOperationDtoHelper.GetAsync(e));
        }

        return result;
    }

    public async Task<IEnumerable<FileDto<T>>> GetFileVersionInfoAsync(T fileId)
    {
        var files = await _fileStorageService.GetFileHistoryAsync(fileId);
        var result = new List<FileDto<T>>();

        foreach (var e in files)
        {
            result.Add(await _fileDtoHelper.GetAsync(e));
        }

        return result;
    }

    public async Task<IEnumerable<FileDto<T>>> ChangeHistoryAsync(T fileId, int version, bool continueVersion)
    {
        var pair = await _fileStorageService.CompleteVersionAsync(fileId, version, continueVersion);
        var history = pair.Value;

        var result = new List<FileDto<T>>();

        foreach (var e in history)
        {
            result.Add(await _fileDtoHelper.GetAsync(e));
        }

        return result;
    }

    public async Task<FileDto<T>> LockFileAsync(T fileId, bool lockFile)
    {
        var result = await _fileStorageService.LockFileAsync(fileId, lockFile);
        return await _fileDtoHelper.GetAsync(result);
    }

    public Task<DocumentService.FileLink> GetPresignedUriAsync(T fileId)
    {
        return _fileStorageService.GetPresignedUriAsync(fileId);
    }

    public async Task<List<EditHistoryDto>> GetEditHistoryAsync(T fileId, string doc = null)
    {
        var result = await _fileStorageService.GetEditHistoryAsync(fileId, doc);

        return result.Select(r => new EditHistoryDto(r, _apiDateTimeHelper, _userManager, _displayUserSettingsHelper)).ToList();
    }

    public Task<EditHistoryDataDto> GetEditDiffUrlAsync(T fileId, int version = 0, string doc = null)
    {
        return _fileStorageService.GetEditDiffUrlAsync(fileId, version, doc);
    }

    public async Task<List<EditHistoryDto>> RestoreVersionAsync(T fileId, int version = 0, string url = null, string doc = null)
    {
        var result = await _fileStorageService.RestoreVersionAsync(fileId, version, url, doc);

        return result.Select(r => new EditHistoryDto(r, _apiDateTimeHelper, _userManager, _displayUserSettingsHelper)).ToList();
    }

    public Task<string> UpdateCommentAsync(T fileId, int version, string comment)
    {
        return _fileStorageService.UpdateCommentAsync(fileId, version, comment);
    }

    public Task<IEnumerable<FileShareDto>> GetFileSecurityInfoAsync(T fileId)
    {
        return GetSecurityInfoAsync(new List<T> { fileId }, new List<T> { });
    }

    public Task<IEnumerable<FileShareDto>> GetFolderSecurityInfoAsync(T folderId)
    {
        return GetSecurityInfoAsync(new List<T> { }, new List<T> { folderId });
    }

    public async IAsyncEnumerable<FileEntryDto> GetFoldersAsync(T folderId)
    {
        var folders = await _fileStorageService.GetFoldersAsync(folderId);
        foreach (var folder in folders)
        {
            yield return await GetFileEntryWrapperAsync(folder);
        }
    }

    public async Task<IEnumerable<FileShareDto>> GetSecurityInfoAsync(IEnumerable<T> fileIds, IEnumerable<T> folderIds)
    {
        var fileShares = await _fileStorageService.GetSharedInfoAsync(fileIds, folderIds);
        
        return fileShares.Select(_fileShareDtoHelper.Get).ToList();
    }

    public Task<IEnumerable<FileShareDto>> SetFileSecurityInfoAsync(T fileId, IEnumerable<FileShareParams> share, bool notify, string sharingMessage)
    {
        return SetSecurityInfoAsync(new List<T> { fileId }, new List<T>(), share, notify, sharingMessage);
    }

    public Task<IEnumerable<FileShareDto>> SetFolderSecurityInfoAsync(T folderId, IEnumerable<FileShareParams> share, bool notify, string sharingMessage)
    {
        return SetSecurityInfoAsync(new List<T>(), new List<T> { folderId }, share, notify, sharingMessage);
    }

    public async Task<IEnumerable<FileShareDto>> SetSecurityInfoAsync(IEnumerable<T> fileIds, IEnumerable<T> folderIds, IEnumerable<FileShareParams> share, bool notify, string sharingMessage)
    {
        if (share != null && share.Any())
        {
            var list = new List<AceWrapper>(share.Select(_fileShareParamsHelper.ToAceObject));

            var aceCollection = new AceCollection<T>
            {
                Files = fileIds,
                Folders = folderIds,
                Aces = list,
                Message = sharingMessage
            };

            await _fileStorageService.SetAceObjectAsync(aceCollection, notify);
        }

        return await GetSecurityInfoAsync(fileIds, folderIds);
    }

    public async Task<bool> RemoveSecurityInfoAsync(List<T> fileIds, List<T> folderIds)
    {
        await _fileStorageService.RemoveAceAsync(fileIds, folderIds);

        return true;
    }

    public async Task<string> GenerateSharedLinkAsync(T fileId, FileShare share)
    {
        var file = await GetFileInfoAsync(fileId);

        var tmpInfo = await _fileStorageService.GetSharedInfoAsync(new List<T> { fileId }, new List<T> { });
        var sharedInfo = tmpInfo.Find(r => r.SubjectId == FileConstant.ShareLinkId);

        if (sharedInfo == null || sharedInfo.Share != share)
        {
            var list = new List<AceWrapper>
            {
                new AceWrapper
                {
                    SubjectId = FileConstant.ShareLinkId,
                    SubjectGroup = true,
                    Share = share
                }
            };

            var aceCollection = new AceCollection<T>
            {
                Files = new List<T> { fileId },
                Folders = new List<T>(0),
                Aces = list
            };

            await _fileStorageService.SetAceObjectAsync(aceCollection, false);

            tmpInfo = await _fileStorageService.GetSharedInfoAsync(new List<T> { fileId }, new List<T> { });
            sharedInfo = tmpInfo.Find(r => r.SubjectId == FileConstant.ShareLinkId);
        }

        return sharedInfo.Link;
    }

    public Task<bool> SetAceLinkAsync(T fileId, FileShare share)
    {
        return _fileStorageService.SetAceLinkAsync(fileId, share);
    }

    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="query"></param>
    ///// <returns></returns>
    //[Read(@"@search/{query}")]
    //public IEnumerable<FileEntryWrapper> Search(string query)
    //{
    //    var searcher = new SearchHandler();
    //    var files = searcher.SearchFiles(query).Select(r => (FileEntryWrapper)FileWrapperHelper.Get(r));
    //    var folders = searcher.SearchFolders(query).Select(f => (FileEntryWrapper)FolderWrapperHelper.Get(f));

    //    return files.Concat(folders);
    //}

    private async Task<FolderContentDto<T>> ToFolderContentWrapperAsync(T folderId, Guid userIdOrGroupId, FilterType filterType, bool withSubFolders)
    {
        OrderBy orderBy = null;
        if (Enum.TryParse(_apiContext.SortBy, true, out SortedByType sortBy))
        {
            orderBy = new OrderBy(sortBy, !_apiContext.SortDescending);
        }

        var startIndex = Convert.ToInt32(_apiContext.StartIndex);
        var items = await _fileStorageService.GetFolderItemsAsync(folderId, startIndex, Convert.ToInt32(_apiContext.Count), filterType,
            filterType == FilterType.ByUser, userIdOrGroupId.ToString(), _apiContext.FilterValue, false, withSubFolders, orderBy);

        return await _folderContentDtoHelper.GetAsync(items, startIndex);
    }
    
    internal async Task<FileEntryDto> GetFileEntryWrapperAsync(FileEntry r)
    {
        FileEntryDto wrapper = null;
        
        if (r is Folder<int> fol1)
        {
            wrapper = await _folderDtoHelper.GetAsync(fol1);
        }
        else if (r is Folder<string> fol2)
        {
            wrapper = await _folderDtoHelper.GetAsync(fol2);
        }
        else if (r is File<int> file1)
        {
            wrapper = await _fileDtoHelper.GetAsync(file1);
        }
        else if (r is File<string> file2)
        {
            wrapper = await _fileDtoHelper.GetAsync(file2);
        }

        return wrapper;
    }

    internal IFormFile GetFileFromRequest(IModelWithFile model)
    {
        IEnumerable<IFormFile> files = _httpContextAccessor.HttpContext.Request.Form.Files;
        if (files != null && files.Any())
        {
            return files.First();
        }

        return model.File;
    }
}