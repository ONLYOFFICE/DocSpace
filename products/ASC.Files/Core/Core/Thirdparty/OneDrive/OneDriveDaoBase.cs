namespace ASC.Files.Thirdparty.OneDrive;

internal abstract class OneDriveDaoBase : ThirdPartyProviderDao<OneDriveProviderInfo>
{
    protected override string Id => "onedrive";

    protected OneDriveDaoBase(
        IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        DbContextManager<FilesDbContext> dbContextManager,
        SetupInfo setupInfo,
        IOptionsMonitor<ILog> monitor,
        FileUtility fileUtility,
        TempPath tempPath)
        : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath)
    {
    }

    protected static string MakeOneDriveId(string entryId)
    {
        var id = entryId;

        return string.IsNullOrEmpty(id)
                   ? string.Empty
                   : id.TrimStart('/');
    }

    protected static string GetParentFolderId(Item onedriveItem)
    {
        return onedriveItem == null || IsRoot(onedriveItem)
                   ? null
                   : (onedriveItem.ParentReference.Path.Equals(OneDriveStorage.RootPath, StringComparison.InvariantCultureIgnoreCase)
                          ? string.Empty
                          : onedriveItem.ParentReference.Id);
    }

    protected string MakeId(Item onedriveItem)
    {
        var id = string.Empty;
        if (onedriveItem != null)
        {
            id = onedriveItem.Id;
        }

        return MakeId(id);
    }

    protected override string MakeId(string id = null)
    {
        var i = string.IsNullOrEmpty(id) ? "" : ("-|" + id.TrimStart('/'));

        return $"{PathPrefix}{i}";
    }

    public string MakeOneDrivePath(Item onedriveItem)
    {
        return onedriveItem == null || IsRoot(onedriveItem)
                   ? string.Empty
                   : OneDriveStorage.MakeOneDrivePath(
                       new Regex("^" + OneDriveStorage.RootPath).Replace(onedriveItem.ParentReference.Path, ""),
                       onedriveItem.Name);
    }

    protected string MakeItemTitle(Item onedriveItem)
    {
        if (onedriveItem == null || IsRoot(onedriveItem))
        {
            return ProviderInfo.CustomerTitle;
        }

        return Global.ReplaceInvalidCharsAndTruncate(onedriveItem.Name);
    }

    protected Folder<string> ToFolder(Item onedriveFolder)
    {
        if (onedriveFolder == null)
        {
            return null;
        }

        if (onedriveFolder is ErrorItem)
        {
            //Return error entry
            return ToErrorFolder(onedriveFolder as ErrorItem);
        }

        if (onedriveFolder.Folder == null)
        {
            return null;
        }

        var isRoot = IsRoot(onedriveFolder);

        var folder = GetFolder();

        folder.ID = MakeId(isRoot ? string.Empty : onedriveFolder.Id);
        folder.FolderID = isRoot ? null : MakeId(GetParentFolderId(onedriveFolder));
        folder.CreateOn = isRoot ? ProviderInfo.CreateOn : (onedriveFolder.CreatedDateTime.HasValue ? TenantUtil.DateTimeFromUtc(onedriveFolder.CreatedDateTime.Value.DateTime) : default);
        folder.ModifiedOn = isRoot ? ProviderInfo.CreateOn : (onedriveFolder.LastModifiedDateTime.HasValue ? TenantUtil.DateTimeFromUtc(onedriveFolder.LastModifiedDateTime.Value.DateTime) : default);

        folder.Title = MakeItemTitle(onedriveFolder);

        return folder;
    }

    protected static bool IsRoot(Item onedriveFolder)
    {
        return onedriveFolder.ParentReference == null || onedriveFolder.ParentReference.Id == null;
    }

    private File<string> ToErrorFile(ErrorItem onedriveFile)
    {
        if (onedriveFile == null)
        {
            return null;
        }

        var file = GetErrorFile(new ErrorEntry(onedriveFile.Error, onedriveFile.ErrorId));

        file.Title = MakeItemTitle(onedriveFile);

        return file;
    }

    private Folder<string> ToErrorFolder(ErrorItem onedriveFolder)
    {
        if (onedriveFolder == null)
        {
            return null;
        }

        var folder = GetErrorFolder(new ErrorEntry(onedriveFolder.Error, onedriveFolder.ErrorId));

        folder.Title = MakeItemTitle(onedriveFolder);

        return folder;
    }

    public File<string> ToFile(Item onedriveFile)
    {
        if (onedriveFile == null)
        {
            return null;
        }

        if (onedriveFile is ErrorItem)
        {
            //Return error entry
            return ToErrorFile(onedriveFile as ErrorItem);
        }

        if (onedriveFile.File == null)
        {
            return null;
        }

        var file = GetFile();

        file.ID = MakeId(onedriveFile.Id);
        file.ContentLength = onedriveFile.Size.HasValue ? (long)onedriveFile.Size : 0;
        file.CreateOn = onedriveFile.CreatedDateTime.HasValue ? TenantUtil.DateTimeFromUtc(onedriveFile.CreatedDateTime.Value.DateTime) : default;
        file.FolderID = MakeId(GetParentFolderId(onedriveFile));
        file.ModifiedOn = onedriveFile.LastModifiedDateTime.HasValue ? TenantUtil.DateTimeFromUtc(onedriveFile.LastModifiedDateTime.Value.DateTime) : default;
        file.NativeAccessor = onedriveFile;
        file.Title = MakeItemTitle(onedriveFile);

        return file;
    }

    public async Task<Folder<string>> GetRootFolderAsync(string folderId)
    {
        return ToFolder(await GetOneDriveItemAsync(""));
    }

    protected Item GetOneDriveItem(string itemId)
    {
        var onedriveId = MakeOneDriveId(itemId);
        try
        {
            return ProviderInfo.GetOneDriveItemAsync(onedriveId).Result;
        }
        catch (Exception ex)
        {
            return new ErrorItem(ex, onedriveId);
        }
    }

    protected async Task<Item> GetOneDriveItemAsync(string itemId)
    {
        var onedriveId = MakeOneDriveId(itemId);
        try
        {
            return await ProviderInfo.GetOneDriveItemAsync(onedriveId);
        }
        catch (Exception ex)
        {
            return new ErrorItem(ex, onedriveId);
        }
    }

    protected override async Task<IEnumerable<string>> GetChildrenAsync(string folderId)
    {
        var items = await GetOneDriveItemsAsync(folderId);

        return items.Select(entry => MakeId(entry.Id));
    }

    protected List<Item> GetOneDriveItems(string parentId, bool? folder = null)
    {
        var onedriveFolderId = MakeOneDriveId(parentId);
        var items = ProviderInfo.GetOneDriveItemsAsync(onedriveFolderId).Result;

        if (folder.HasValue)
        {
            if (folder.Value)
            {
                return items.Where(i => i.Folder != null).ToList();
            }

            return items.Where(i => i.File != null).ToList();
        }

        return items;
    }

    protected async Task<List<Item>> GetOneDriveItemsAsync(string parentId, bool? folder = null)
    {
        var onedriveFolderId = MakeOneDriveId(parentId);
        var items = await ProviderInfo.GetOneDriveItemsAsync(onedriveFolderId);

        if (folder.HasValue)
        {
            if (folder.Value)
            {
                return items.Where(i => i.Folder != null).ToList();
            }

            return items.Where(i => i.File != null).ToList();
        }

        return items;
    }

    protected sealed class ErrorItem : Item
    {
        public string Error { get; set; }
        public string ErrorId { get; private set; }

        public ErrorItem(Exception e, object id)
        {
            ErrorId = id.ToString();
            if (e != null)
            {
                Error = e.Message;
            }
        }
    }

    protected string GetAvailableTitle(string requestTitle, string parentFolderId, Func<string, string, bool> isExist)
    {
        requestTitle = new Regex("\\.$").Replace(requestTitle, "_");
        if (!isExist(requestTitle, parentFolderId))
        {
            return requestTitle;
        }

        var re = new Regex(@"( \(((?<index>[0-9])+)\)(\.[^\.]*)?)$");
        var match = re.Match(requestTitle);

        if (!match.Success)
        {
            var insertIndex = requestTitle.Length;
            if (requestTitle.LastIndexOf('.') != -1)
            {
                insertIndex = requestTitle.LastIndexOf('.');
            }

            requestTitle = requestTitle.Insert(insertIndex, " (1)");
        }

        while (isExist(requestTitle, parentFolderId))
        {
            requestTitle = re.Replace(requestTitle, MatchEvaluator);
        }

        return requestTitle;
    }

    protected async Task<string> GetAvailableTitleAsync(string requestTitle, string parentFolderId, Func<string, string, Task<bool>> isExist)
    {
        requestTitle = new Regex("\\.$").Replace(requestTitle, "_");
        if (!await isExist(requestTitle, parentFolderId))
        {
            return requestTitle;
        }

        var re = new Regex(@"( \(((?<index>[0-9])+)\)(\.[^\.]*)?)$");
        var match = re.Match(requestTitle);

        if (!match.Success)
        {
            var insertIndex = requestTitle.Length;
            if (requestTitle.LastIndexOf(".", StringComparison.InvariantCulture) != -1)
            {
                insertIndex = requestTitle.LastIndexOf(".", StringComparison.InvariantCulture);
            }

            requestTitle = requestTitle.Insert(insertIndex, " (1)");
        }

        while (await isExist(requestTitle, parentFolderId))
        {
            requestTitle = re.Replace(requestTitle, MatchEvaluator);
        }

        return requestTitle;
    }

    private string MatchEvaluator(Match match)
    {
        var index = Convert.ToInt32(match.Groups[2].Value);
        var staticText = match.Value.Substring(string.Format(" ({0})", index).Length);

        return string.Format(" ({0}){1}", index + 1, staticText);
    }
}
