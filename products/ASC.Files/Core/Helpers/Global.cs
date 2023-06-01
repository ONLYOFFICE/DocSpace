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

namespace ASC.Web.Files.Classes;

[Singletone]
public class GlobalNotify
{
    public ILogger Logger { get; set; }
    private readonly ICacheNotify<AscCacheItem> _notify;

    public GlobalNotify(ICacheNotify<AscCacheItem> notify, ILoggerProvider options, CoreBaseSettings coreBaseSettings)
    {
        _notify = notify;
        Logger = options.CreateLogger("ASC.Files");
        if (coreBaseSettings.Standalone)
        {
            ClearCache();
        }
    }

    private void ClearCache()
    {
        try
        {
            _notify.Subscribe((item) =>
            {
                try
                {
                    GlobalFolder.ProjectsRootFolderCache.Clear();
                    GlobalFolder.UserRootFolderCache.Clear();
                    GlobalFolder.CommonFolderCache.Clear();
                    GlobalFolder.ShareFolderCache.Clear();
                    GlobalFolder.RecentFolderCache.Clear();
                    GlobalFolder.FavoritesFolderCache.Clear();
                    GlobalFolder.TemplatesFolderCache.Clear();
                    GlobalFolder.PrivacyFolderCache.Clear();
                    GlobalFolder.TrashFolderCache.Clear();
                }
                catch (Exception e)
                {
                    Logger.CriticalClearCacheAction(e);
                }
            }, CacheNotifyAction.Any);
        }
        catch (Exception e)
        {
            Logger.CriticalClearCacheSubscribe(e);
        }
    }
}

[EnumExtensions]
public enum ThumbnailExtension
{
    bmp,
    gif,
    jpg,
    png,
    pbm,
    tiff,
    tga,
    webp
}

[EnumExtensions]
public enum DocThumbnailExtension
{
    bmp,
    gif,
    jpg,
    png
}

[Scope]
public class Global
{
    private readonly IConfiguration _configuration;
    private readonly AuthContext _authContext;
    private readonly UserManager _userManager;
    private readonly CoreSettings _coreSettings;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly CustomNamingPeople _customNamingPeople;
    private readonly FileSecurityCommon _fileSecurityCommon;

    public Global(
        IConfiguration configuration,
        AuthContext authContext,
        UserManager userManager,
        CoreSettings coreSettings,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        CustomNamingPeople customNamingPeople,
        FileSecurityCommon fileSecurityCommon)
    {
        _configuration = configuration;
        _authContext = authContext;
        _userManager = userManager;
        _coreSettings = coreSettings;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _customNamingPeople = customNamingPeople;
        _fileSecurityCommon = fileSecurityCommon;

        if (!DocThumbnailExtensionExtensions.TryParse(configuration["files:thumbnail:docs-exts"] ?? "jpg", true, out DocThumbnailExtension))
        {
            DocThumbnailExtension = DocThumbnailExtension.jpg;
        }
        if (!ThumbnailExtensionExtensions.TryParse(configuration["files:thumbnail:exts"] ?? "webp", true, out ThumbnailExtension))
        {
            ThumbnailExtension = ThumbnailExtension.jpg;
        }
    }

    #region Property

    public DocThumbnailExtension DocThumbnailExtension;
    public ThumbnailExtension ThumbnailExtension;

    public const int MaxTitle = 170;

    public static readonly Regex InvalidTitleChars = new Regex("[\t*\\+:\"<>?|\\\\/\\p{Cs}]");

    public bool EnableUploadFilter => bool.TrueString.Equals(_configuration["files:upload-filter"] ?? "false", StringComparison.InvariantCultureIgnoreCase);

    public TimeSpan StreamUrlExpire
    {
        get
        {
            int.TryParse(_configuration["files:stream-url-minute"], out var validateTimespan);
            if (validateTimespan <= 0)
            {
                validateTimespan = 16;
            }

            return TimeSpan.FromMinutes(validateTimespan);
        }
    }

    public Task<bool> IsDocSpaceAdministratorAsync => _fileSecurityCommon.IsDocSpaceAdministratorAsync(_authContext.CurrentAccount.ID);

    public async Task<string> GetDocDbKeyAsync()
    {
        const string dbKey = "UniqueDocument";
        var resultKey = await _coreSettings.GetSettingAsync(dbKey);

        if (!string.IsNullOrEmpty(resultKey))
        {
            return resultKey;
        }

        resultKey = Guid.NewGuid().ToString();
        await _coreSettings.SaveSettingAsync(dbKey, resultKey);

        return resultKey;
    }

    #endregion

    public static string ReplaceInvalidCharsAndTruncate(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            return title;
        }

        title = title.Trim();
        if (MaxTitle < title.Length)
        {
            var pos = title.LastIndexOf('.');
            if (MaxTitle - 20 < pos)
            {
                title = title.Substring(0, MaxTitle - (title.Length - pos)) + title.Substring(pos);
            }
            else
            {
                title = title.Substring(0, MaxTitle);
            }
        }

        return InvalidTitleChars.Replace(title, "_");
    }

    public async Task<string> GetUserNameAsync(Guid userId, bool alive = false)
    {
        if (userId.Equals(_authContext.CurrentAccount.ID))
        {
            return FilesCommonResource.Author_Me;
        }

        if (userId.Equals(ASC.Core.Configuration.Constants.Guest.ID))
        {
            return FilesCommonResource.Guest;
        }

        var userInfo = await _userManager.GetUsersAsync(userId);
        if (userInfo.Equals(Constants.LostUser))
        {
            return alive ? FilesCommonResource.Guest : _customNamingPeople.Substitute<FilesCommonResource>("ProfileRemoved");
        }

        return userInfo.DisplayUserName(false, _displayUserSettingsHelper);
    }

    public string GetUserName(Guid userId, bool alive = false)
    {
        if (userId.Equals(_authContext.CurrentAccount.ID))
        {
            return FilesCommonResource.Author_Me;
        }

        if (userId.Equals(ASC.Core.Configuration.Constants.Guest.ID))
        {
            return FilesCommonResource.Guest;
        }

        var userInfo = _userManager.GetUsers(userId);
        if (userInfo.Equals(Constants.LostUser))
        {
            return alive ? FilesCommonResource.Guest : _customNamingPeople.Substitute<FilesCommonResource>("ProfileRemoved");
        }

        return userInfo.DisplayUserName(false, _displayUserSettingsHelper);
    }
}

[Scope]
public class GlobalStore
{
    private readonly StorageFactory _storageFactory;
    private readonly TenantManager _tenantManager;

    public GlobalStore(StorageFactory storageFactory, TenantManager tenantManager)
    {
        _storageFactory = storageFactory;
        _tenantManager = tenantManager;
    }

    public async Task<IDataStore> GetStoreAsync(bool currentTenant = true)
    {
        return await _storageFactory.GetStorageAsync(currentTenant ? await _tenantManager.GetCurrentTenantIdAsync() : null, FileConstant.StorageModule);
    }


    public async Task<IDataStore> GetStoreTemplateAsync()
    {
        return await _storageFactory.GetStorageAsync(null, FileConstant.StorageTemplate);
    }
}

[Scope]
public class GlobalSpace
{
    private readonly FilesSpaceUsageStatManager _filesSpaceUsageStatManager;
    private readonly AuthContext _authContext;

    public GlobalSpace(FilesSpaceUsageStatManager filesSpaceUsageStatManager, AuthContext authContext)
    {
        _filesSpaceUsageStatManager = filesSpaceUsageStatManager;
        _authContext = authContext;
    }

    public async Task<long> GetUserUsedSpaceAsync()
    {
        return await GetUserUsedSpaceAsync(_authContext.CurrentAccount.ID);
    }

    public async Task<long> GetUserUsedSpaceAsync(Guid userId)
    {
        return await _filesSpaceUsageStatManager.GetUserSpaceUsageAsync(userId);
    }
}

[Scope]
public class GlobalFolder
{
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly WebItemManager _webItemManager;
    private readonly WebItemSecurity _webItemSecurity;
    private readonly AuthContext _authContext;
    private readonly TenantManager _tenantManager;
    private readonly UserManager _userManager;
    private readonly SettingsManager _settingsManager;
    private readonly GlobalStore _globalStore;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;

    public GlobalFolder(
        CoreBaseSettings coreBaseSettings,
        WebItemManager webItemManager,
        WebItemSecurity webItemSecurity,
        AuthContext authContext,
        TenantManager tenantManager,
        UserManager userManager,
        SettingsManager settingsManager,
        GlobalStore globalStore,
        ILoggerProvider options,
        IServiceProvider serviceProvider
    )
    {
        _coreBaseSettings = coreBaseSettings;
        _webItemManager = webItemManager;
        _webItemSecurity = webItemSecurity;
        _authContext = authContext;
        _tenantManager = tenantManager;
        _userManager = userManager;
        _settingsManager = settingsManager;
        _globalStore = globalStore;
        _serviceProvider = serviceProvider;
        _logger = options.CreateLogger("ASC.Files");
    }

    internal static readonly IDictionary<int, int> ProjectsRootFolderCache =
        new ConcurrentDictionary<int, int>(); /*Use SYNCHRONIZED for cross thread blocks*/

    public async ValueTask<int> GetFolderProjectsAsync(IDaoFactory daoFactory)
    {
        if (_coreBaseSettings.Personal)
        {
            return default;
        }

        if (await _webItemManager[WebItemManager.ProjectsProductID].IsDisabledAsync(_webItemSecurity, _authContext))
        {
            return default;
        }

        var tenant = await _tenantManager.GetCurrentTenantAsync();
        var folderDao = daoFactory.GetFolderDao<int>();
        if (!ProjectsRootFolderCache.TryGetValue(tenant.Id, out var result))
        {
            result = await folderDao.GetFolderIDProjectsAsync(true);

            ProjectsRootFolderCache[tenant.Id] = result;
        }

        return result;
    }

    public async ValueTask<T> GetFolderProjectsAsync<T>(IDaoFactory daoFactory)
    {
        return IdConverter.Convert<T>(await GetFolderProjectsAsync(daoFactory));
    }

    internal static readonly ConcurrentDictionary<string, int> DocSpaceFolderCache =
        new ConcurrentDictionary<string, int>();

    public async ValueTask<int> GetFolderVirtualRoomsAsync(IDaoFactory daoFactory, bool createIfNotExist = true)
    {
        if (_coreBaseSettings.DisableDocSpace)
        {
            return default;
        }

        var key = $"vrooms/{await _tenantManager.GetCurrentTenantIdAsync()}";

        if (DocSpaceFolderCache.TryGetValue(key, out var result))
        {
            return result;
        }

        result = await daoFactory.GetFolderDao<int>().GetFolderIDVirtualRooms(createIfNotExist);

        if (result != default)
        {
            DocSpaceFolderCache[key] = result;
        }

        return result;
    }

    public async ValueTask<int> GetFolderArchiveAsync(IDaoFactory daoFactory)
    {
        if (_coreBaseSettings.DisableDocSpace)
        {
            return default;
        }

        var key = $"archive/{await _tenantManager.GetCurrentTenantIdAsync()}";

        if (!DocSpaceFolderCache.TryGetValue(key, out var result))
        {
            result = await daoFactory.GetFolderDao<int>().GetFolderIDArchive(true);

            DocSpaceFolderCache[key] = result;
        }

        return result;
    }

    public async ValueTask<int> GetFolderArchiveAsync<T>(IDaoFactory daoFactory)
    {
        return await GetFolderArchiveAsync(daoFactory);
    }

    internal static readonly ConcurrentDictionary<string, Lazy<int>> UserRootFolderCache =
        new ConcurrentDictionary<string, Lazy<int>>(); /*Use SYNCHRONIZED for cross thread blocks*/

    public async ValueTask<int> GetFolderMyAsync(FileMarker fileMarker, IDaoFactory daoFactory)
    {
        if (!_authContext.IsAuthenticated)
        {
            return default;
        }

        if (await _userManager.IsUserAsync(_authContext.CurrentAccount.ID))
        {
            return default;
        }

        var cacheKey = $"my/{await _tenantManager.GetCurrentTenantIdAsync()}/{_authContext.CurrentAccount.ID}";

        var myFolderId = UserRootFolderCache.GetOrAdd(cacheKey, (a) => new Lazy<int>(() => GetFolderIdAndProcessFirstVisitAsync(daoFactory, true).Result));

        return myFolderId.Value;
    }

    protected internal async Task SetFolderMyAsync(object value)
    {
        var cacheKey = string.Format("my/{0}/{1}", await _tenantManager.GetCurrentTenantIdAsync(), value);
        UserRootFolderCache.Remove(cacheKey, out _);
    }

    public async Task<bool> IsFirstVisit(IDaoFactory daoFactory)
    {
        var cacheKey = string.Format("my/{0}/{1}", await _tenantManager.GetCurrentTenantIdAsync(), _authContext.CurrentAccount.ID);

        if (!UserRootFolderCache.TryGetValue(cacheKey, out var _))
        {
            var folderDao = daoFactory.GetFolderDao<int>();
            var myFolderId = await folderDao.GetFolderIDUserAsync(false);

            if (Equals(myFolderId, 0))
            {
                return true;
            }
        }

        return false;
    }

    internal static readonly IDictionary<int, int> CommonFolderCache =
            new ConcurrentDictionary<int, int>(); /*Use SYNCHRONIZED for cross thread blocks*/

    public async ValueTask<T> GetFolderCommonAsync<T>(FileMarker fileMarker, IDaoFactory daoFactory)
    {
        return IdConverter.Convert<T>(await GetFolderCommonAsync(fileMarker, daoFactory));
    }

    public async ValueTask<int> GetFolderCommonAsync(FileMarker fileMarker, IDaoFactory daoFactory)
    {
        if (_coreBaseSettings.Personal)
        {
            return default;
        }

        var tenant = await _tenantManager.GetCurrentTenantAsync();
        if (CommonFolderCache.TryGetValue(tenant.Id, out var commonFolderId))
        {
            return commonFolderId;
        }

        commonFolderId = await GetFolderIdAndProcessFirstVisitAsync(daoFactory, false);
        
            if (!Equals(commonFolderId, 0))
            {
                CommonFolderCache[tenant.Id] = commonFolderId;
            }

        return commonFolderId;
    }

    internal static readonly IDictionary<int, int> ShareFolderCache =
        new ConcurrentDictionary<int, int>(); /*Use SYNCHRONIZED for cross thread blocks*/

    public async ValueTask<int> GetFolderShareAsync(IDaoFactory daoFactory)
    {
        if (_coreBaseSettings.Personal)
        {
            return default;
        }

        if (await IsOutsiderAsync)
        {
            return default;
        }

        var tenant = await _tenantManager.GetCurrentTenantAsync();
        if (!ShareFolderCache.TryGetValue(tenant.Id, out var sharedFolderId))
        {
            sharedFolderId = await daoFactory.GetFolderDao<int>().GetFolderIDShareAsync(true);

            if (!sharedFolderId.Equals(default))
            {
                ShareFolderCache[tenant.Id] = sharedFolderId;
            }
        }

        return sharedFolderId;
    }

    public async ValueTask<T> GetFolderShareAsync<T>(IDaoFactory daoFactory)
    {
        return IdConverter.Convert<T>(await GetFolderShareAsync(daoFactory));
    }

    internal static readonly IDictionary<int, int> RecentFolderCache =
        new ConcurrentDictionary<int, int>(); /*Use SYNCHRONIZED for cross thread blocks*/

    public async ValueTask<int> GetFolderRecentAsync(IDaoFactory daoFactory)
    {
        if (!_authContext.IsAuthenticated)
        {
            return 0;
        }

        var tenant = await _tenantManager.GetCurrentTenantAsync();
        if (!RecentFolderCache.TryGetValue(tenant.Id, out var recentFolderId))
        {
            var folderDao = daoFactory.GetFolderDao<int>();
            recentFolderId = await folderDao.GetFolderIDRecentAsync(true);

            if (!recentFolderId.Equals(0))
            {
                RecentFolderCache[tenant.Id] = recentFolderId;
            }
        }

        return recentFolderId;
    }

    internal static readonly IDictionary<int, int> FavoritesFolderCache =
        new ConcurrentDictionary<int, int>(); /*Use SYNCHRONIZED for cross thread blocks*/

    public async ValueTask<int> GetFolderFavoritesAsync(IDaoFactory daoFactory)
    {
        if (!_authContext.IsAuthenticated)
        {
            return 0;
        }

        var tenant = await _tenantManager.GetCurrentTenantAsync();
        if (!FavoritesFolderCache.TryGetValue(tenant.Id, out var favoriteFolderId))
        {
            var folderDao = daoFactory.GetFolderDao<int>();
            favoriteFolderId = await folderDao.GetFolderIDFavoritesAsync(true);

            if (!favoriteFolderId.Equals(0))
            {
                FavoritesFolderCache[tenant.Id] = favoriteFolderId;
            }
        }

        return favoriteFolderId;
    }

    internal static readonly IDictionary<int, int> TemplatesFolderCache =
        new ConcurrentDictionary<int, int>(); /*Use SYNCHRONIZED for cross thread blocks*/

    public async ValueTask<int> GetFolderTemplatesAsync(IDaoFactory daoFactory)
    {
        if (!_authContext.IsAuthenticated)
        {
            return 0;
        }

        if (await _userManager.IsUserAsync(_authContext.CurrentAccount.ID))
        {
            return 0;
        }
        var tenant = await _tenantManager.GetCurrentTenantAsync();
        if (!TemplatesFolderCache.TryGetValue(tenant.Id, out var templatesFolderId))
        {
            var folderDao = daoFactory.GetFolderDao<int>();
            templatesFolderId = await folderDao.GetFolderIDTemplatesAsync(true);

            if (!templatesFolderId.Equals(0))
            {
                TemplatesFolderCache[tenant.Id] = templatesFolderId;
            }
        }

        return templatesFolderId;
    }

    internal static readonly IDictionary<string, int> PrivacyFolderCache =
        new ConcurrentDictionary<string, int>(); /*Use SYNCHRONIZED for cross thread blocks*/

    public async ValueTask<T> GetFolderPrivacyAsync<T>(IDaoFactory daoFactory)
    {
        return IdConverter.Convert<T>(await GetFolderPrivacyAsync(daoFactory));
    }

    public async ValueTask<int> GetFolderPrivacyAsync(IDaoFactory daoFactory)
    {
        if (!_authContext.IsAuthenticated)
        {
            return 0;
        }

        if (await _userManager.IsUserAsync(_authContext.CurrentAccount.ID))
        {
            return 0;
        }

        var cacheKey = string.Format("privacy/{0}/{1}", await _tenantManager.GetCurrentTenantIdAsync(), _authContext.CurrentAccount.ID);

        if (!PrivacyFolderCache.TryGetValue(cacheKey, out var privacyFolderId))
        {
            var folderDao = daoFactory.GetFolderDao<int>();
            privacyFolderId = await folderDao.GetFolderIDPrivacyAsync(true);

            if (!Equals(privacyFolderId, 0))
            {
                PrivacyFolderCache[cacheKey] = privacyFolderId;
            }
        }

        return privacyFolderId;
    }


    internal static readonly IDictionary<string, object> TrashFolderCache =
        new ConcurrentDictionary<string, object>(); /*Use SYNCHRONIZED for cross thread blocks*/

    public async ValueTask<int> GetFolderTrashAsync(IDaoFactory daoFactory)
    {
        var id = 0;
        if (await IsOutsiderAsync)
        {
            return id;
        }

        var cacheKey = string.Format("trash/{0}/{1}", _tenantManager.GetCurrentTenant().Id, _authContext.CurrentAccount.ID);
        if (!TrashFolderCache.TryGetValue(cacheKey, out var trashFolderId))
        {
            id = _authContext.IsAuthenticated ? await daoFactory.GetFolderDao<int>().GetFolderIDTrashAsync(true) : 0;
            TrashFolderCache[cacheKey] = id;
        }
        else
        {
            id = (int)trashFolderId;
        }

        return id;
    }

    public async Task SetFolderTrashAsync(object value)
    {
        var cacheKey = string.Format("trash/{0}/{1}", await _tenantManager.GetCurrentTenantIdAsync(), value);
        TrashFolderCache.Remove(cacheKey);
    }

    private async Task<int> GetFolderIdAndProcessFirstVisitAsync(IDaoFactory daoFactory, bool my)
    {
        var folderDao = (FolderDao)daoFactory.GetFolderDao<int>();

        var id = my ? await folderDao.GetFolderIDUserAsync(false) : await folderDao.GetFolderIDCommonAsync(false);

        if (!Equals(id, 0))
        {
            return id;
        }

        id = my ? await folderDao.GetFolderIDUserAsync(true) : await folderDao.GetFolderIDCommonAsync(true);
        
        if (!_settingsManager.LoadForDefaultTenant<AdditionalWhiteLabelSettings>().StartDocsEnabled)
        {
            return id;
        }

        var tenantId = await _tenantManager.GetCurrentTenantIdAsync();
        var userId = _authContext.CurrentAccount.ID;

        var task = new Task(async () => await CreateSampleDocumentsAsync(_serviceProvider, tenantId, userId, id, my), 
            TaskCreationOptions.LongRunning);

        _ = task.ConfigureAwait(false);
        
        task.Start();

        return id;
    }
    
    private async Task CreateSampleDocumentsAsync(IServiceProvider serviceProvider, int tenantId, Guid userId, int folderId, bool my)
    {
        try
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            
            var tenantManager = scope.ServiceProvider.GetRequiredService<TenantManager>();
            var securityContext = scope.ServiceProvider.GetRequiredService<SecurityContext>();

            await tenantManager.SetCurrentTenantAsync(tenantId);
            await securityContext.AuthenticateMeWithoutCookieAsync(userId);

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager>();
            var culture = my ? userManager.GetUsers(userId).GetCulture() : (await tenantManager.GetCurrentTenantAsync()).GetCulture();

            var globalStore = scope.ServiceProvider.GetRequiredService<GlobalStore>();
            var storeTemplate = await globalStore.GetStoreTemplateAsync();
            
            var path = FileConstant.StartDocPath + culture + "/";
            
            if (!await storeTemplate.IsDirectoryAsync(path))
            {
                path = FileConstant.StartDocPath + "en-US/";
            }
        
            path += my ? "my/" : "corporate/";

            var fileMarker = scope.ServiceProvider.GetRequiredService<FileMarker>();
            var fileDao = (FileDao)scope.ServiceProvider.GetRequiredService<IFileDao<int>>();
            var folderDao = (FolderDao)scope.ServiceProvider.GetRequiredService<IFolderDao<int>>();
            var socketManager = scope.ServiceProvider.GetRequiredService<SocketManager>();

            await SaveSampleDocumentsAsync(scope.ServiceProvider, fileMarker, folderDao, fileDao, socketManager, folderId, path, storeTemplate);
        }
        catch (Exception e)
        {
            _logger.ErrorCreateSampleDocuments(e);
        }
    }
    
    private async Task SaveSampleDocumentsAsync(IServiceProvider serviceProvider, FileMarker fileMarker, FolderDao folderDao, FileDao fileDao, SocketManager socketManager, 
        int folderId, string path, IDataStore storeTemplate)
    { 
        var files = await storeTemplate.ListFilesRelativeAsync("", path, "*", false)
            .Where(f => FileUtility.GetFileTypeByFileName(f) is not (FileType.Audio or FileType.Video))
            .ToListAsync();
        
        _logger.Debug($"Found {files.Count} sample documents. Path: {path}");
        
        foreach (var file in files)
        {
            await SaveFileAsync(serviceProvider, storeTemplate, fileMarker, fileDao, socketManager, path + file, folderId, files);
        }

        await foreach (var folderName in storeTemplate.ListDirectoriesRelativeAsync(path, false))
        {
            try
            {
                var folder = serviceProvider.GetRequiredService<Folder<int>>();
                folder.Title = folderName;
                folder.ParentId = folderId;

                var subFolderId = await folderDao.SaveFolderAsync(folder);
                    
                var subFolder = await folderDao.GetFolderAsync(subFolderId);
                await socketManager.CreateFolderAsync(subFolder);
                    
                await SaveSampleDocumentsAsync(serviceProvider, fileMarker, folderDao, fileDao, socketManager, folderId, path + folderName + "/", storeTemplate);
            }
            catch (Exception e)
            {
                _logger.ErrorSaveSampleFolder(e);
            }   
        }
    }

    private async Task SaveFileAsync(IServiceProvider serviceProvider, IDataStore storeTemplate, FileMarker fileMarker, FileDao fileDao, SocketManager socketManager,
        string filePath, int folderId, IEnumerable<string> files)
    {
        try
        {
            var fileName = Path.GetFileName(filePath);
            
            foreach (var ext in Enum.GetValues<ThumbnailExtension>()) 
            { 
                if (FileUtility.GetFileExtension(filePath) == "." + ext
                    && files.Contains(Regex.Replace(fileName, "\\." + ext + "$", "")))
                {
                    return;
                }
            }

            var newFile = serviceProvider.GetRequiredService<File<int>>();

            newFile.Title = fileName;
            newFile.ParentId = folderId;
            newFile.Comment = FilesCommonResource.CommentCreate;

            await using (var stream = await storeTemplate.GetReadStreamAsync("", filePath))
            {
                newFile.ContentLength = stream.CanSeek ? stream.Length : await storeTemplate.GetFileSizeAsync("", filePath);
                newFile = await fileDao.SaveFileAsync(newFile, stream, false);
            }

            await fileMarker.MarkAsNewAsync(newFile);
            await socketManager.CreateFileAsync(newFile);
        }
        catch (Exception e)
        {
            _logger.ErrorSaveSampleFile(e);
        }
    }

    private Task<bool> IsOutsiderAsync => _userManager.IsOutsiderAsync(_authContext.CurrentAccount.ID);
}

[Scope]
public class GlobalFolderHelper
{
    private readonly FileMarker _fileMarker;
    private readonly IDaoFactory _daoFactory;
    private readonly GlobalFolder _globalFolder;

    public GlobalFolderHelper(FileMarker fileMarker, IDaoFactory daoFactory, GlobalFolder globalFolder)
    {
        _fileMarker = fileMarker;
        _daoFactory = daoFactory;
        _globalFolder = globalFolder;
    }

    public ValueTask<int> FolderProjectsAsync => _globalFolder.GetFolderProjectsAsync(_daoFactory);
    public ValueTask<int> FolderCommonAsync => _globalFolder.GetFolderCommonAsync(_fileMarker, _daoFactory);
    public ValueTask<int> FolderMyAsync => _globalFolder.GetFolderMyAsync(_fileMarker, _daoFactory);
    public ValueTask<int> FolderPrivacyAsync => _globalFolder.GetFolderPrivacyAsync(_daoFactory);
    public ValueTask<int> FolderRecentAsync => _globalFolder.GetFolderRecentAsync(_daoFactory);
    public ValueTask<int> FolderFavoritesAsync => _globalFolder.GetFolderFavoritesAsync(_daoFactory);
    public ValueTask<int> FolderTemplatesAsync => _globalFolder.GetFolderTemplatesAsync(_daoFactory);
    public ValueTask<int> FolderVirtualRoomsAsync => _globalFolder.GetFolderVirtualRoomsAsync(_daoFactory);
    public ValueTask<int> FolderArchiveAsync => _globalFolder.GetFolderArchiveAsync(_daoFactory);

    public async Task<T> GetFolderMyAsync<T>()
    {
        return IdConverter.Convert<T>(await FolderMyAsync);
    }

    public async ValueTask<T> GetFolderCommonAsync<T>()
    {
        return IdConverter.Convert<T>(await FolderCommonAsync);
    }

    public async ValueTask<T> GetFolderProjectsAsync<T>()
    {
        return IdConverter.Convert<T>(await FolderProjectsAsync);
    }

    public async ValueTask<T> GetFolderPrivacyAsync<T>()
    {
        return IdConverter.Convert<T>(await FolderPrivacyAsync);
    }

    public async ValueTask<int> GetFolderVirtualRooms()
    {
        return await FolderVirtualRoomsAsync;
    }

    public async ValueTask<int> GetFolderArchive()
    {
        return await FolderArchiveAsync;
    }

    public async ValueTask<T> GetFolderShareAsync<T>()
    {
        return IdConverter.Convert<T>(await FolderShareAsync);
    }
    public ValueTask<int> FolderShareAsync => _globalFolder.GetFolderShareAsync(_daoFactory);

    public async Task SetFolderTrashAsync(object value)
    {
        await _globalFolder.SetFolderTrashAsync(value);
    }
    public ValueTask<int> FolderTrashAsync
    {
        get => _globalFolder.GetFolderTrashAsync(_daoFactory);
    }

}
