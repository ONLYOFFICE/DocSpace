namespace ASC.Files.Api;

public class TagsController : ApiControllerBase
{
    private readonly FileStorageService<int> _fileStorageServiceInt;
    private readonly FileStorageService<string> _fileStorageServiceString;

    public TagsController(
        FilesControllerHelper<int> filesControllerHelperInt,
        FilesControllerHelper<string> filesControllerHelperString,
        FileStorageService<int> fileStorageServiceInt,
        FileStorageService<string> fileStorageServiceString) 
        : base(filesControllerHelperInt, filesControllerHelperString)
    {
        _fileStorageServiceString = fileStorageServiceString;
        _fileStorageServiceInt = fileStorageServiceInt;
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
    public Task<bool> AddFavoritesFromBodyAsync([FromBody] BaseBatchModel model)
    {
        return AddFavoritesAsync(model);
    }

    [Create("favorites")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<bool> AddFavoritesFromFormAsync([FromForm][ModelBinder(BinderType = typeof(BaseBatchModelBinder))] BaseBatchModel model)
    {
        return await AddFavoritesAsync(model);
    }

    /// <summary>
    /// Adding files to template list
    /// </summary>
    /// <short>Template add</short>
    /// <category>Files</category>
    /// <param name="fileIds">File IDs</param>
    /// <returns></returns>
    [Create("templates")]
    public async Task<bool> AddTemplatesFromBodyAsync([FromBody] TemplatesModel model)
    {
        await _fileStorageServiceInt.AddToTemplatesAsync(model.FileIds);

        return true;
    }

    [Create("templates")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<bool> AddTemplatesFromFormAsync([FromForm] TemplatesModel model)
    {
        await _fileStorageServiceInt.AddToTemplatesAsync(model.FileIds);

        return true;
    }

    [Create("file/{fileId}/recent", order: int.MaxValue)]
    public Task<FileWrapper<string>> AddToRecentAsync(string fileId)
    {
        return _filesControllerHelperString.AddToRecentAsync(fileId);
    }

    [Create("file/{fileId:int}/recent", order: int.MaxValue - 1)]
    public Task<FileWrapper<int>> AddToRecentAsync(int fileId)
    {
        return _filesControllerHelperInt.AddToRecentAsync(fileId);
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
    public Task<bool> DeleteFavoritesFromBodyAsync([FromBody] BaseBatchModel model)
    {
        return DeleteFavoritesAsync(model);
    }

    [Delete("favorites")]
    public async Task<bool> DeleteFavoritesFromQueryAsync([FromQuery][ModelBinder(BinderType = typeof(BaseBatchModelBinder))] BaseBatchModel model)
    {
        return await DeleteFavoritesAsync(model);
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

    private async Task<bool> AddFavoritesAsync(BaseBatchModel model)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(model.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(model.FileIds);

        await _fileStorageServiceInt.AddToFavoritesAsync(folderIntIds, fileIntIds);
        await _fileStorageServiceString.AddToFavoritesAsync(folderStringIds, fileStringIds);

        return true;
    }

    private async Task<bool> DeleteFavoritesAsync(BaseBatchModel model)
    {
        var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(model.FolderIds);
        var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(model.FileIds);

        await _fileStorageServiceInt.DeleteFavoritesAsync(folderIntIds, fileIntIds);
        await _fileStorageServiceString.DeleteFavoritesAsync(folderStringIds, fileStringIds);

        return true;
    }
}