namespace ASC.Files.Helpers;

public class OperationControllerHelper<T> : FilesHelperBase<T>
{
    private readonly FileOperationDtoHelper _fileOperationDtoHelper;

    public OperationControllerHelper(
        FilesSettingsHelper filesSettingsHelper,
        FileUploader fileUploader,
        SocketManager socketManager,
        FileDtoHelper fileDtoHelper,
        ApiContext apiContext,
        FileStorageService<T> fileStorageService,
        FolderContentDtoHelper folderContentDtoHelper,
        IHttpContextAccessor httpContextAccessor,
        FolderDtoHelper folderDtoHelper,
        FileOperationDtoHelper fileOperationDtoHelper) 
        : base(
            filesSettingsHelper,
            fileUploader,
            socketManager,
            fileDtoHelper,
            apiContext,
            fileStorageService,
            folderContentDtoHelper,
            httpContextAccessor,
            folderDtoHelper)
    {
        _fileOperationDtoHelper = fileOperationDtoHelper;
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

    public async Task<IEnumerable<FileOperationDto>> CopyBatchItemsAsync(BatchRequestDto batchRequestDto)
    {
        var result = new List<FileOperationDto>();

        foreach (var e in _fileStorageService.MoveOrCopyItems(batchRequestDto.FolderIds.ToList(), batchRequestDto.FileIds.ToList(), batchRequestDto.DestFolderId, batchRequestDto.ConflictResolveType, true, batchRequestDto.DeleteAfter))
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

    public async Task<IEnumerable<FileOperationDto>> MarkAsReadAsync(BaseBatchRequestDto batchRequestDto)
    {
        var result = new List<FileOperationDto>();

        foreach (var e in _fileStorageService.MarkAsRead(batchRequestDto.FolderIds.ToList(), batchRequestDto.FileIds.ToList()))
        {
            result.Add(await _fileOperationDtoHelper.GetAsync(e));
        }

        return result;
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
}