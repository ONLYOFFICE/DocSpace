namespace ASC.Files.Api;

[ConstraintRoute("int")]
public class TagsControllerInternal : TagsController<int>
{
    public TagsControllerInternal(FileStorageService<int> fileStorageServiceString, EntryManager entryManager, FileDtoHelper fileDtoHelper) : base(fileStorageServiceString, entryManager, fileDtoHelper)
    {
    }
}

public class TagsControllerThirdparty : TagsController<string>
{
    public TagsControllerThirdparty(FileStorageService<string> fileStorageServiceString, EntryManager entryManager, FileDtoHelper fileDtoHelper) : base(fileStorageServiceString, entryManager, fileDtoHelper)
    {
    }
}

public abstract class TagsController<T> : ApiControllerBase
{
    private readonly FileStorageService<T> _fileStorageServiceString;
    private readonly EntryManager _entryManager;
    private readonly FileDtoHelper _fileDtoHelper;

    public TagsController(
        FileStorageService<T> fileStorageServiceString,
        EntryManager entryManager,
        FileDtoHelper fileDtoHelper)
    {
        _fileStorageServiceString = fileStorageServiceString;
        _entryManager = entryManager;
        _fileDtoHelper = fileDtoHelper;
    }

    [Create("file/{fileId}/recent", order: int.MaxValue)]
    public async Task<FileDto<T>> AddToRecentAsync(T fileId)
    {
        var file = await _fileStorageServiceString.GetFileAsync(fileId, -1).NotFoundIfNull("File not found");
        _entryManager.MarkAsRecent(file);

        return await _fileDtoHelper.GetAsync(file);
    }

    [Read("favorites/{fileId}")]
    public Task<bool> ToggleFileFavoriteAsync(T fileId, bool favorite)
    {
        return _fileStorageServiceString.ToggleFileFavoriteAsync(fileId, favorite);
    }
}

public class TagsControllerCommon : ApiControllerBase
{
    private readonly FileStorageService<int> _fileStorageService;
    private readonly FileStorageService<string> _fileStorageServiceThirdparty;

    public TagsControllerCommon(
        FileStorageService<int> fileStorageService,
        FileStorageService<string> fileStorageServiceThirdparty)
    {
        _fileStorageService = fileStorageService;
        _fileStorageServiceThirdparty = fileStorageServiceThirdparty;
    }

    /// <summary>
    /// Adding files to favorite list
    /// </summary>
    /// <short>Favorite add</short>
    /// <category>Files</category>
    /// <param name="folderIds" visible="false"></param>
    /// <param name="fileIds">File IDs</param>
    /// <returns></returns>
    [Create("favorites")]
    public Task<bool> AddFavoritesFromBodyAsync([FromBody] BaseBatchRequestDto inDto)
    {
        return AddFavoritesAsync(inDto);
    }

    [Create("favorites")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<bool> AddFavoritesFromFormAsync([FromForm][ModelBinder(BinderType = typeof(BaseBatchModelBinder))] BaseBatchRequestDto inDto)
    {
        return await AddFavoritesAsync(inDto);
    }

    /// <summary>
    /// Adding files to template list
    /// </summary>
    /// <short>Template add</short>
    /// <category>Files</category>
    /// <param name="fileIds">File IDs</param>
    /// <returns></returns>
    [Create("templates")]
    public async Task<bool> AddTemplatesFromBodyAsync([FromBody] TemplatesRequestDto inDto)
    {
        await _fileStorageService.AddToTemplatesAsync(inDto.FileIds);

        return true;
    }

    [Create("templates")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<bool> AddTemplatesFromFormAsync([FromForm] TemplatesRequestDto inDto)
    {
        await _fileStorageService.AddToTemplatesAsync(inDto.FileIds);

        return true;
    }

    /// <summary>
    /// Removing files from favorite list
    /// </summary>
    /// <short>Favorite delete</short>
    /// <category>Files</category>
    /// <param name="folderIds" visible="false"></param>
    /// <param name="fileIds">File IDs</param>
    /// <returns></returns>
    [Delete("favorites")]
    [Consumes("application/json")]
    public Task<bool> DeleteFavoritesFromBodyAsync([FromBody] BaseBatchRequestDto inDto)
    {
        return DeleteFavoritesAsync(inDto);
    }

    [Delete("favorites")]
    public async Task<bool> DeleteFavoritesFromQueryAsync([FromQuery][ModelBinder(BinderType = typeof(BaseBatchModelBinder))] BaseBatchRequestDto inDto)
    {
        return await DeleteFavoritesAsync(inDto);
    }

    /// <summary>
    /// Removing files from template list
    /// </summary>
    /// <short>Template delete</short>
    /// <category>Files</category>
    /// <param name="fileIds">File IDs</param>
    /// <returns></returns>
    [Delete("templates")]
    public async Task<bool> DeleteTemplatesAsync(IEnumerable<int> fileIds)
    {
        await _fileStorageService.DeleteTemplatesAsync(fileIds);

        return true;
    }

    private async Task<bool> AddFavoritesAsync(BaseBatchRequestDto inDto)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(inDto.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(inDto.FileIds);

        await _fileStorageService.AddToFavoritesAsync(folderIntIds, fileIntIds);
        await _fileStorageServiceThirdparty.AddToFavoritesAsync(folderStringIds, fileStringIds);

        return true;
    }

    private async Task<bool> DeleteFavoritesAsync(BaseBatchRequestDto inDto)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(inDto.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(inDto.FileIds);

        await _fileStorageService.DeleteFavoritesAsync(folderIntIds, fileIntIds);
        await _fileStorageServiceThirdparty.DeleteFavoritesAsync(folderStringIds, fileStringIds);

        return true;
    }
}