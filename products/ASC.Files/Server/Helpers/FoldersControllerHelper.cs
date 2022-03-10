namespace ASC.Files.Helpers;

public class FoldersControllerHelper<T> : FilesHelperBase<T>
{
    private readonly FileOperationDtoHelper _fileOperationDtoHelper;
    private readonly EntryManager _entryManager;
    private readonly UserManager _userManager;
    private readonly SecurityContext _securityContext;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly FileUtility _fileUtility;
    private readonly TenantManager _tenantManager;

    public FoldersControllerHelper(
        FilesSettingsHelper filesSettingsHelper,
        FileUploader fileUploader,
        SocketManager socketManager,
        FileDtoHelper fileDtoHelper,
        ApiContext apiContext,
        FileStorageService<T> fileStorageService,
        FolderContentDtoHelper folderContentDtoHelper,
        IHttpContextAccessor httpContextAccessor,
        FolderDtoHelper folderDtoHelper,
        FileOperationDtoHelper fileOperationDtoHelper,
        EntryManager entryManager,
        UserManager userManager,
        SecurityContext securityContext,
        GlobalFolderHelper globalFolderHelper,
        CoreBaseSettings coreBaseSettings,
        FileUtility fileUtility,
        TenantManager tenantManager) 
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
        _globalFolderHelper = globalFolderHelper;
        _coreBaseSettings = coreBaseSettings;
        _fileUtility = fileUtility;
        _tenantManager = tenantManager;
        _securityContext = securityContext;
        _entryManager = entryManager;
        _userManager = userManager;
    }

    public async Task<FolderDto<T>> CreateFolderAsync(T folderId, string title)
    {
        var folder = await _fileStorageService.CreateNewFolderAsync(folderId, title);

        return await _folderDtoHelper.GetAsync(folder);
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

    public async Task<FolderContentDto<T>> GetFolderAsync(T folderId, Guid userIdOrGroupId, FilterType filterType, bool withSubFolders)
    {
        var folderContentWrapper = await ToFolderContentWrapperAsync(folderId, userIdOrGroupId, filterType, withSubFolders);

        return folderContentWrapper.NotFoundIfNull();
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

    public async IAsyncEnumerable<FileEntryDto> GetFoldersAsync(T folderId)
    {
        var folders = await _fileStorageService.GetFoldersAsync(folderId);
        foreach (var folder in folders)
        {
            yield return await GetFileEntryWrapperAsync(folder);
        }
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

    public async Task<FolderDto<T>> RenameFolderAsync(T folderId, string title)
    {
        var folder = await _fileStorageService.FolderRenameAsync(folderId, title);

        return await _folderDtoHelper.GetAsync(folder);
    }

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
}