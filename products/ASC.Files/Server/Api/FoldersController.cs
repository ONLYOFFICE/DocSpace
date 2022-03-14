namespace ASC.Files.Api;

[ConstraintRoute("int")]
public class FoldersControllerInternal : FoldersController<int>
{
    public FoldersControllerInternal(FoldersControllerHelper<int> foldersControllerHelper) : base(foldersControllerHelper)
    {
    }
}

public class FoldersControllerThirdparty : FoldersController<string>
{
    public FoldersControllerThirdparty(FoldersControllerHelper<string> foldersControllerHelper) : base(foldersControllerHelper)
    {
    }
}

public abstract class FoldersController<T> : ApiControllerBase
{
    private readonly FoldersControllerHelper<T> _foldersControllerHelper;

    public FoldersController(FoldersControllerHelper<T> foldersControllerHelper)
    {
        _foldersControllerHelper = foldersControllerHelper;
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
    public Task<FolderDto<T>> CreateFolderFromBodyAsync(T folderId, [FromBody] CreateFolderRequestDto inDto)
    {
        return _foldersControllerHelper.CreateFolderAsync(folderId, inDto.Title);
    }

    [Create("folder/{folderId}", order: int.MaxValue, DisableFormat = true)]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FolderDto<T>> CreateFolderFromFormAsync(T folderId, [FromForm] CreateFolderRequestDto inDto)
    {
        return _foldersControllerHelper.CreateFolderAsync(folderId, inDto.Title);
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
    public Task<IEnumerable<FileOperationDto>> DeleteFolder(T folderId, bool deleteAfter, bool immediately)
    {
        return _foldersControllerHelper.DeleteFolder(folderId, deleteAfter, immediately);
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
    public async Task<FolderContentDto<T>> GetFolderAsync(T folderId, Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        var folder = await _foldersControllerHelper.GetFolderAsync(folderId, userIdOrGroupId, filterType, withsubfolders);

        return folder.NotFoundIfNull();
    }

    /// <summary>
    /// Returns a detailed information about the folder with the ID specified in the request
    /// </summary>
    /// <short>Folder information</short>
    /// <category>Folders</category>
    /// <returns>Folder info</returns>
    [Read("folder/{folderId}", order: int.MaxValue, DisableFormat = true)]
    public Task<FolderDto<T>> GetFolderInfoAsync(T folderId)
    {
        return _foldersControllerHelper.GetFolderInfoAsync(folderId);
    }

    /// <summary>
    /// Returns parent folders
    /// </summary>
    /// <param name="folderId"></param>
    /// <category>Folders</category>
    /// <returns>Parent folders</returns>
    [Read("folder/{folderId}/path")]
    public IAsyncEnumerable<FileEntryDto> GetFolderPathAsync(T folderId)
    {
        return _foldersControllerHelper.GetFolderPathAsync(folderId);
    }

    [Read("{folderId}/subfolders")]
    public IAsyncEnumerable<FileEntryDto> GetFoldersAsync(T folderId)
    {
        return _foldersControllerHelper.GetFoldersAsync(folderId);
    }

    [Read("{folderId}/news")]
    public Task<List<FileEntryDto>> GetNewItemsAsync(T folderId)
    {
        return _foldersControllerHelper.GetNewItemsAsync(folderId);
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
    public Task<FolderDto<T>> RenameFolderFromBodyAsync(T folderId, [FromBody] CreateFolderRequestDto inDto)
    {
        return _foldersControllerHelper.RenameFolderAsync(folderId, inDto.Title);
    }

    [Update("folder/{folderId}", order: int.MaxValue, DisableFormat = true)]
    [Consumes("application/x-www-form-urlencoded")]
    public Task<FolderDto<T>> RenameFolderFromFormAsync(T folderId, [FromForm] CreateFolderRequestDto inDto)
    {
        return _foldersControllerHelper.RenameFolderAsync(folderId, inDto.Title);
    }
}

public class FoldersControllerCommon : ApiControllerBase
{
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly TenantManager _tenantManager;
    private readonly FoldersControllerHelper<int> _foldersControllerHelperInt;
    private readonly FoldersControllerHelper<string> _foldersControllerHelperString;

    public FoldersControllerCommon(
        GlobalFolderHelper globalFolderHelper,
        TenantManager tenantManager,
        FoldersControllerHelper<int> foldersControllerHelperInt,
        FoldersControllerHelper<string> foldersControllerHelperString)
    {
        _globalFolderHelper = globalFolderHelper;
        _tenantManager = tenantManager;
        _foldersControllerHelperInt = foldersControllerHelperInt;
        _foldersControllerHelperString = foldersControllerHelperString;
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
    public async Task<FolderContentDto<int>> GetCommonFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        return await _foldersControllerHelperInt.GetFolderAsync(await _globalFolderHelper.FolderCommonAsync, userIdOrGroupId, filterType, withsubfolders);
    }

    /// <summary>
    /// Returns the detailed list of favorites files
    /// </summary>
    /// <short>Section Favorite</short>
    /// <category>Folders</category>
    /// <returns>Favorites contents</returns>
    [Read("@favorites")]
    public async Task<FolderContentDto<int>> GetFavoritesFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        return await _foldersControllerHelperInt.GetFolderAsync(await _globalFolderHelper.FolderFavoritesAsync, userIdOrGroupId, filterType, withsubfolders);
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
    public Task<FolderContentDto<int>> GetMyFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        return _foldersControllerHelperInt.GetFolderAsync(_globalFolderHelper.FolderMy, userIdOrGroupId, filterType, withsubfolders);
    }

    [Read("@privacy")]
    public async Task<FolderContentDto<int>> GetPrivacyFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        if (PrivacyRoomSettings.IsAvailable(_tenantManager))
        {
            throw new System.Security.SecurityException();
        }

        return await _foldersControllerHelperInt.GetFolderAsync(await _globalFolderHelper.FolderPrivacyAsync, userIdOrGroupId, filterType, withsubfolders);
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
    public async Task<FolderContentDto<string>> GetProjectsFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        return await _foldersControllerHelperString.GetFolderAsync(await _globalFolderHelper.GetFolderProjectsAsync<string>(), userIdOrGroupId, filterType, withsubfolders);
    }

    /// <summary>
    /// Returns the detailed list of recent files
    /// </summary>
    /// <short>Section Recent</short>
    /// <category>Folders</category>
    /// <returns>Recent contents</returns>
    [Read("@recent")]
    public async Task<FolderContentDto<int>> GetRecentFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        return await _foldersControllerHelperInt.GetFolderAsync(await _globalFolderHelper.FolderRecentAsync, userIdOrGroupId, filterType, withsubfolders);
    }

    [Read("@root")]
    public async Task<IEnumerable<FolderContentDto<int>>> GetRootFoldersAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders, bool withoutTrash, bool withoutAdditionalFolder)
    {
        var foldersIds = await _foldersControllerHelperInt.GetRootFoldersIdsAsync(withoutTrash, withoutAdditionalFolder);

        var result = new List<FolderContentDto<int>>();

        foreach (var folder in foldersIds)
        {
            result.Add(await _foldersControllerHelperInt.GetFolderAsync(folder, userIdOrGroupId, filterType, withsubfolders));
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
    public async Task<FolderContentDto<int>> GetShareFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        return await _foldersControllerHelperInt.GetFolderAsync(await _globalFolderHelper.FolderShareAsync, userIdOrGroupId, filterType, withsubfolders);
    }

    /// <summary>
    /// Returns the detailed list of templates files
    /// </summary>
    /// <short>Section Template</short>
    /// <category>Folders</category>
    /// <returns>Templates contents</returns>
    [Read("@templates")]
    public async Task<FolderContentDto<int>> GetTemplatesFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        return await _foldersControllerHelperInt.GetFolderAsync(await _globalFolderHelper.FolderTemplatesAsync, userIdOrGroupId, filterType, withsubfolders);
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
    public Task<FolderContentDto<int>> GetTrashFolderAsync(Guid userIdOrGroupId, FilterType filterType, bool withsubfolders)
    {
        return _foldersControllerHelperInt.GetFolderAsync(Convert.ToInt32(_globalFolderHelper.FolderTrash), userIdOrGroupId, filterType, withsubfolders);
    }
}
