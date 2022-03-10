namespace ASC.Files.Api;

public class TagsController : ApiControllerBase
{
    private readonly FileStorageService<int> _fileStorageServiceInt;
    private readonly FileStorageService<string> _fileStorageServiceString;
    private readonly EntryManager _entryManager;
    private readonly FileDtoHelper _fileDtoHelper;

    public TagsController(
        FileStorageService<int> fileStorageServiceInt,
        FileStorageService<string> fileStorageServiceString,
        EntryManager entryManager,
        FileDtoHelper fileDtoHelper)
    {
        _fileStorageServiceInt = fileStorageServiceInt;
        _fileStorageServiceString = fileStorageServiceString;
        _entryManager = entryManager;
        _fileDtoHelper = fileDtoHelper;
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
        await _fileStorageServiceInt.AddToTemplatesAsync(inDto.FileIds);

        return true;
    }

    [Create("templates")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<bool> AddTemplatesFromFormAsync([FromForm] TemplatesRequestDto inDto)
    {
        await _fileStorageServiceInt.AddToTemplatesAsync(inDto.FileIds);

        return true;
    }

    [Create("file/{fileId}/recent", order: int.MaxValue)]
    public Task<FileDto<string>> AddToRecentAsync(string fileId)
    {
        return AddToRecentStringAsync(fileId);
    }

    [Create("file/{fileId:int}/recent", order: int.MaxValue - 1)]
    public Task<FileDto<int>> AddToRecentAsync(int fileId)
    {
        return AddToRecentIntAsync(fileId);
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
        await _fileStorageServiceInt.DeleteTemplatesAsync(fileIds);

        return true;
    }

    [Read("favorites/{fileId:int}")]
    public Task<bool> ToggleFavoriteFromFormAsync(int fileId, bool favorite)
    {
        return _fileStorageServiceInt.ToggleFileFavoriteAsync(fileId, favorite);
    }

    [Read("favorites/{fileId}")]
    public Task<bool> ToggleFileFavoriteAsync(string fileId, bool favorite)
    {
        return _fileStorageServiceString.ToggleFileFavoriteAsync(fileId, favorite);
    }

    private async Task<bool> AddFavoritesAsync(BaseBatchRequestDto inDto)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(inDto.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(inDto.FileIds);

        await _fileStorageServiceInt.AddToFavoritesAsync(folderIntIds, fileIntIds);
        await _fileStorageServiceString.AddToFavoritesAsync(folderStringIds, fileStringIds);

        return true;
    }

    private async Task<bool> DeleteFavoritesAsync(BaseBatchRequestDto inDto)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(inDto.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(inDto.FileIds);

        await _fileStorageServiceInt.DeleteFavoritesAsync(folderIntIds, fileIntIds);
        await _fileStorageServiceString.DeleteFavoritesAsync(folderStringIds, fileStringIds);

        return true;
    }

    private async Task<FileDto<string>> AddToRecentStringAsync(string fileId)
    {
        var file = await _fileStorageServiceString.GetFileAsync(fileId, -1).NotFoundIfNull("File not found");
        _entryManager.MarkAsRecent(file);

        return await _fileDtoHelper.GetAsync(file);
    }

    private async Task<FileDto<int>> AddToRecentIntAsync(int fileId)
    {
        var file = await _fileStorageServiceInt.GetFileAsync(fileId, -1).NotFoundIfNull("File not found");
        _entryManager.MarkAsRecent(file);

        return await _fileDtoHelper.GetAsync(file);
    }
}