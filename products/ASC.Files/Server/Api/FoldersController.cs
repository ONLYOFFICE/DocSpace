namespace ASC.Files.Api;

public class FoldersController : ApiControllerBase
{
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly TenantManager _tenantManager;

    public FoldersController(
        FilesControllerHelper<int> filesControllerHelperInt,
        FilesControllerHelper<string> filesControllerHelperString,
        GlobalFolderHelper globalFolderHelper,
        TenantManager tenantManage) 
        : base(filesControllerHelperInt, filesControllerHelperString)
    {
        _globalFolderHelper = globalFolderHelper;
        _tenantManager = tenantManage;
    }

    /// <summary>
    /// Creates a new folder with the title sent in the request. The ID of a parent folder can be also specified.
    /// </summary>
    /// <short>
    /// New folder
    /// </short>
    /// <category>Folders</category>
    /// <param name="folderId">Parent folder ID</param>
    /// <param name="title">Title of new folder</param>
    /// <returns>New folder contents</returns>
    [Create("folder/{folderId}", order: int.MaxValue, DisableFormat = true)]
    public Task<FolderWrapper<string>> CreateFolderFromBodyAsync(string folderId, [FromBody] CreateFolderModel folderModel)
    {
        return _filesControllerHelperString.CreateFolderAsync(folderId, folderModel.Title);
    }

    [Create("folder/{folderId:int}", order: int.MaxValue - 1, DisableFormat = true)]
    public Task<FolderWrapper<int>> CreateFolderFromBodyAsync(int folderId, [FromBody] CreateFolderModel folderModel)
    {
        return _filesControllerHelperInt.CreateFolderAsync(folderId, folderModel.Title);
    }

    [Create("folder/{folderId}", order: int.MaxValue, DisableFormat = true)]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FolderWrapper<string>> CreateFolderFromFormAsync(string folderId, [FromForm] CreateFolderModel folderModel)
    {
        return _filesControllerHelperString.CreateFolderAsync(folderId, folderModel.Title);
    }

    [Create("folder/{folderId:int}", order: int.MaxValue - 1, DisableFormat = true)]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FolderWrapper<int>> CreateFolderFromFormAsync(int folderId, [FromForm] CreateFolderModel folderModel)
    {
        return _filesControllerHelperInt.CreateFolderAsync(folderId, folderModel.Title);
    }

    /// <summary>
    /// Deletes the folder with the ID specified in the request
    /// </summary>
    /// <short>Delete folder</short>
    /// <category>Folders</category>
    /// <param name="folderId">Folder ID</param>
    /// <param name="deleteAfter">Delete after finished</param>
    /// <param name="immediately">Don't move to the Recycle Bin</param>
    /// <returns>Operation result</returns>
    [Delete("folder/{folderId}", order: int.MaxValue - 1, DisableFormat = true)]
    public Task<IEnumerable<FileOperationWraper>> DeleteFolder(string folderId, bool deleteAfter, bool immediately)
    {
        return _filesControllerHelperString.DeleteFolder(folderId, deleteAfter, immediately);
    }

    [Delete("folder/{folderId:int}")]
    public Task<IEnumerable<FileOperationWraper>> DeleteFolder(int folderId, bool deleteAfter, bool immediately)
    {
        return _filesControllerHelperInt.DeleteFolder(folderId, deleteAfter, immediately);
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the 'Common Documents' section
    /// </summary>
    /// <short>
    /// Common folder
    /// </short>
    /// <category>Folders</category>
    /// <returns>Common folder contents</returns>
    [Read("@common")]
    public async Task<FolderContentWrapper<int>> GetCommonFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        return await _filesControllerHelperInt.GetFolderAsync(await _globalFolderHelper.FolderCommonAsync, userIdOrGroupId, filterType, withsubfolders);
    }

    /// <summary>
    /// Returns the detailed list of favorites files
    /// </summary>
    /// <short>Section Favorite</short>
    /// <category>Folders</category>
    /// <returns>Favorites contents</returns>
    [Read("@favorites")]
    public async Task<FolderContentWrapper<int>> GetFavoritesFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        return await _filesControllerHelperInt.GetFolderAsync(await _globalFolderHelper.FolderFavoritesAsync, userIdOrGroupId, filterType, withsubfolders);
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the folder with the ID specified in the request
    /// </summary>
    /// <short>
    /// Folder by ID
    /// </short>
    /// <category>Folders</category>
    /// <param name="folderId">Folder ID</param>
    /// <param name="userIdOrGroupId" optional="true">User or group ID</param>
    /// <param name="filterType" optional="true" remark="Allowed values: None (0), FilesOnly (1), FoldersOnly (2), DocumentsOnly (3), PresentationsOnly (4), SpreadsheetsOnly (5) or ImagesOnly (7)">Filter type</param>
    /// <returns>Folder contents</returns>
    [Read("{folderId}", order: int.MaxValue, DisableFormat = true)]
    public async Task<FolderContentWrapper<string>> GetFolderAsync(string folderId, Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        var folder = await _filesControllerHelperString.GetFolderAsync(folderId, userIdOrGroupId, filterType, withsubfolders);
        return folder.NotFoundIfNull();
    }

    [Read("{folderId:int}", order: int.MaxValue - 1, DisableFormat = true)]
    public Task<FolderContentWrapper<int>> GetFolderAsync(int folderId, Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        return _filesControllerHelperInt.GetFolderAsync(folderId, userIdOrGroupId, filterType, withsubfolders);
    }

    /// <summary>
    /// Returns a detailed information about the folder with the ID specified in the request
    /// </summary>
    /// <short>Folder information</short>
    /// <category>Folders</category>
    /// <returns>Folder info</returns>
    [Read("folder/{folderId}", order: int.MaxValue, DisableFormat = true)]
    public Task<FolderWrapper<string>> GetFolderInfoAsync(string folderId)
    {
        return _filesControllerHelperString.GetFolderInfoAsync(folderId);
    }

    [Read("folder/{folderId:int}", order: int.MaxValue - 1, DisableFormat = true)]
    public Task<FolderWrapper<int>> GetFolderInfoAsync(int folderId)
    {
        return _filesControllerHelperInt.GetFolderInfoAsync(folderId);
    }

    /// <summary>
    /// Returns parent folders
    /// </summary>
    /// <param name="folderId"></param>
    /// <category>Folders</category>
    /// <returns>Parent folders</returns>
    [Read("folder/{folderId}/path")]
    public IAsyncEnumerable<FileEntryWrapper> GetFolderPathAsync(string folderId)
    {
        return _filesControllerHelperString.GetFolderPathAsync(folderId);
    }

    [Read("folder/{folderId:int}/path")]
    public IAsyncEnumerable<FileEntryWrapper> GetFolderPathAsync(int folderId)
    {
        return _filesControllerHelperInt.GetFolderPathAsync(folderId);
    }

    [Read("{folderId}/subfolders")]
    public IAsyncEnumerable<FileEntryWrapper> GetFoldersAsync(string folderId)
    {
        return _filesControllerHelperString.GetFoldersAsync(folderId);
    }

    [Read("{folderId:int}/subfolders")]
    public IAsyncEnumerable<FileEntryWrapper> GetFoldersAsync(int folderId)
    {
        return _filesControllerHelperInt.GetFoldersAsync(folderId);
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the current user 'My Documents' section
    /// </summary>
    /// <short>
    /// My folder
    /// </short>
    /// <category>Folders</category>
    /// <returns>My folder contents</returns>
    [Read("@my")]
    public Task<FolderContentWrapper<int>> GetMyFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        return _filesControllerHelperInt.GetFolderAsync(_globalFolderHelper.FolderMy, userIdOrGroupId, filterType, withsubfolders);
    }

    [Read("{folderId}/news")]
    public Task<List<FileEntryWrapper>> GetNewItemsAsync(string folderId)
    {
        return _filesControllerHelperString.GetNewItemsAsync(folderId);
    }

    [Read("{folderId:int}/news")]
    public Task<List<FileEntryWrapper>> GetNewItemsAsync(int folderId)
    {
        return _filesControllerHelperInt.GetNewItemsAsync(folderId);
    }

    [Read("@privacy")]
    public Task<FolderContentWrapper<int>> GetPrivacyFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        if (PrivacyRoomSettings.IsAvailable(_tenantManager))
        {
            throw new System.Security.SecurityException();
        }

        return InternalGetPrivacyFolderAsync(userIdOrGroupId, filterType, withsubfolders);
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the current user 'Projects Documents' section
    /// </summary>
    /// <short>
    /// Projects folder
    /// </short>
    /// <category>Folders</category>
    /// <returns>Projects folder contents</returns>
    [Read("@projects")]
    public async Task<FolderContentWrapper<string>> GetProjectsFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        return await _filesControllerHelperString.GetFolderAsync(await _globalFolderHelper.GetFolderProjectsAsync<string>(), userIdOrGroupId, filterType, withsubfolders);
    }

    /// <summary>
    /// Returns the detailed list of recent files
    /// </summary>
    /// <short>Section Recent</short>
    /// <category>Folders</category>
    /// <returns>Recent contents</returns>
    [Read("@recent")]
    public async Task<FolderContentWrapper<int>> GetRecentFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        return await _filesControllerHelperInt.GetFolderAsync(await _globalFolderHelper.FolderRecentAsync, userIdOrGroupId, filterType, withsubfolders);
    }

    [Read("@root")]
    public async Task<IEnumerable<FolderContentWrapper<int>>> GetRootFoldersAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders, bool withoutTrash, bool withoutAdditionalFolder)
    {
        var foldersIds = await _filesControllerHelperInt.GetRootFoldersIdsAsync(withoutTrash, withoutAdditionalFolder);

        var result = new List<FolderContentWrapper<int>>();

        foreach (var folder in foldersIds)
        {
            result.Add(await _filesControllerHelperInt.GetFolderAsync(folder, userIdOrGroupId, filterType, withsubfolders));
        }

        return result;
    }
    /// <summary>
    /// Returns the detailed list of files and folders located in the 'Shared with Me' section
    /// </summary>
    /// <short>
    /// Shared folder
    /// </short>
    /// <category>Folders</category>
    /// <returns>Shared folder contents</returns>
    [Read("@share")]
    public async Task<FolderContentWrapper<int>> GetShareFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        return await _filesControllerHelperInt.GetFolderAsync(await _globalFolderHelper.FolderShareAsync, userIdOrGroupId, filterType, withsubfolders);
    }

    /// <summary>
    /// Returns the detailed list of templates files
    /// </summary>
    /// <short>Section Template</short>
    /// <category>Folders</category>
    /// <returns>Templates contents</returns>
    [Read("@templates")]
    public async Task<FolderContentWrapper<int>> GetTemplatesFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        return await _filesControllerHelperInt.GetFolderAsync(await _globalFolderHelper.FolderTemplatesAsync, userIdOrGroupId, filterType, withsubfolders);
    }

    /// <summary>
    /// Returns the detailed list of files and folders located in the 'Recycle Bin' section
    /// </summary>
    /// <short>
    /// Trash folder
    /// </short>
    /// <category>Folders</category>
    /// <returns>Trash folder contents</returns>
    [Read("@trash")]
    public Task<FolderContentWrapper<int>> GetTrashFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        return _filesControllerHelperInt.GetFolderAsync(Convert.ToInt32(_globalFolderHelper.FolderTrash), userIdOrGroupId, filterType, withsubfolders);
    }

    /// <summary>
    /// Renames the selected folder to the new title specified in the request
    /// </summary>
    /// <short>
    /// Rename folder
    /// </short>
    /// <category>Folders</category>
    /// <param name="folderId">Folder ID</param>
    /// <param name="title">New title</param>
    /// <returns>Folder contents</returns>
    [Update("folder/{folderId}", order: int.MaxValue, DisableFormat = true)]
    public Task<FolderWrapper<string>> RenameFolderFromBodyAsync(string folderId, [FromBody] CreateFolderModel folderModel)
    {
        return _filesControllerHelperString.RenameFolderAsync(folderId, folderModel.Title);
    }

    [Update("folder/{folderId:int}", order: int.MaxValue - 1, DisableFormat = true)]
    public Task<FolderWrapper<int>> RenameFolderFromBodyAsync(int folderId, [FromBody] CreateFolderModel folderModel)
    {
        return _filesControllerHelperInt.RenameFolderAsync(folderId, folderModel.Title);
    }

    [Update("folder/{folderId}", order: int.MaxValue, DisableFormat = true)]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FolderWrapper<string>> RenameFolderFromFormAsync(string folderId, [FromForm] CreateFolderModel folderModel)
    {
        return _filesControllerHelperString.RenameFolderAsync(folderId, folderModel.Title);
    }

    [Update("folder/{folderId:int}", order: int.MaxValue - 1, DisableFormat = true)]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FolderWrapper<int>> RenameFolderFromFormAsync(int folderId, [FromForm] CreateFolderModel folderModel)
    {
        return _filesControllerHelperInt.RenameFolderAsync(folderId, folderModel.Title);
    }

    private async Task<FolderContentWrapper<int>> InternalGetPrivacyFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        return await _filesControllerHelperInt.GetFolderAsync(await _globalFolderHelper.FolderPrivacyAsync, userIdOrGroupId, filterType, withsubfolders);
    }
}
