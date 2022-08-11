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

    public bool IsAdministrator => _fileSecurityCommon.IsAdministrator(_authContext.CurrentAccount.ID);

    public string GetDocDbKey()
    {
        const string dbKey = "UniqueDocument";
        var resultKey = _coreSettings.GetSetting(dbKey);

        if (!string.IsNullOrEmpty(resultKey))
        {
            return resultKey;
        }

        resultKey = Guid.NewGuid().ToString();
        _coreSettings.SaveSetting(dbKey, resultKey);

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

    public IDataStore GetStore(bool currentTenant = true)
    {
        return _storageFactory.GetStorage(currentTenant ? _tenantManager.GetCurrentTenant().Id.ToString() : string.Empty, FileConstant.StorageModule);
    }

    public IDataStore GetStoreTemplate()
    {
        return _storageFactory.GetStorage(string.Empty, FileConstant.StorageTemplate);
    }
}

[Scope]
public class GlobalSpace
{
    private readonly FilesUserSpaceUsage _filesUserSpaceUsage;
    private readonly AuthContext _authContext;

    public GlobalSpace(FilesUserSpaceUsage filesUserSpaceUsage, AuthContext authContext)
    {
        _filesUserSpaceUsage = filesUserSpaceUsage;
        _authContext = authContext;
    }

    public Task<long> GetUserUsedSpaceAsync()
    {
        return GetUserUsedSpaceAsync(_authContext.CurrentAccount.ID);
    }

    public Task<long> GetUserUsedSpaceAsync(Guid userId)
    {
        return _filesUserSpaceUsage.GetUserSpaceUsageAsync(userId);
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
    private readonly Global _global;
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
        IServiceProvider serviceProvider,
            Global global,
            ThumbnailSettings thumbnailSettings
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
        _global = global;
        _logger = options.CreateLogger("ASC.Files");
        _thumbnailSettings = thumbnailSettings;
    }

    internal static readonly IDictionary<int, int> ProjectsRootFolderCache =
        new ConcurrentDictionary<int, int>(); /*Use SYNCHRONIZED for cross thread blocks*/

    public async ValueTask<int> GetFolderProjectsAsync(IDaoFactory daoFactory)
    {
        if (_coreBaseSettings.Personal)
        {
            return default;
        }

        if (_webItemManager[WebItemManager.ProjectsProductID].IsDisabled(_webItemSecurity, _authContext))
        {
            return default;
        }

        var folderDao = daoFactory.GetFolderDao<int>();
        if (!ProjectsRootFolderCache.TryGetValue(_tenantManager.GetCurrentTenant().Id, out var result))
        {
            result = await folderDao.GetFolderIDProjectsAsync(true);

            ProjectsRootFolderCache[_tenantManager.GetCurrentTenant().Id] = result;
        }

        return result;
    }

    public async ValueTask<T> GetFolderProjectsAsync<T>(IDaoFactory daoFactory)
    {
        return (T)Convert.ChangeType(await GetFolderProjectsAsync(daoFactory), typeof(T));
    }

    internal static readonly ConcurrentDictionary<string, int> DocSpaceFolderCache = 
        new ConcurrentDictionary<string, int>();

    public async ValueTask<int> GetFolderVirtualRoomsAsync(IDaoFactory daoFactory)
    {
        if (_coreBaseSettings.DisableDocSpace)
        {
            return default;
        }

        var key = $"vrooms/{_tenantManager.GetCurrentTenant().Id}";

        if (!DocSpaceFolderCache.TryGetValue(key, out var result))
        {
            result = await daoFactory.GetFolderDao<int>().GetFolderIDVirtualRooms(true);

            DocSpaceFolderCache[key] = result;
        }

        return result;
    }

    public async ValueTask<T> GetFolderVirtualRoomsAsync<T>(IDaoFactory daoFactory)
    {
        return (T)Convert.ChangeType(await GetFolderVirtualRoomsAsync(daoFactory), typeof(T));
    }

    public async ValueTask<int> GetFolderArchiveAsync(IDaoFactory daoFactory)
    {
        if (_coreBaseSettings.DisableDocSpace)
        {
            return default;
        }

        var key = $"archive/{_tenantManager.GetCurrentTenant().Id}";

        if (!DocSpaceFolderCache.TryGetValue(key, out var result))
        {
            result = await daoFactory.GetFolderDao<int>().GetFolderIDArchive(true);

            DocSpaceFolderCache[key] = result;
        }

        return result;
    }

    public async ValueTask<T> GetFolderArchive<T>(IDaoFactory daoFactory)
    {
        return (T)Convert.ChangeType(await GetFolderArchiveAsync(daoFactory), typeof(T));
    }

    internal static readonly ConcurrentDictionary<string, Lazy<int>> UserRootFolderCache =
        new ConcurrentDictionary<string, Lazy<int>>(); /*Use SYNCHRONIZED for cross thread blocks*/

    public T GetFolderMy<T>(FileMarker fileMarker, IDaoFactory daoFactory)
    {
        return (T)Convert.ChangeType(GetFolderMy(fileMarker, daoFactory), typeof(T));
    }

    public int GetFolderMy(FileMarker fileMarker, IDaoFactory daoFactory)
    {
        if (!_authContext.IsAuthenticated)
        {
            return default;
        }

        if (_userManager.GetUsers(_authContext.CurrentAccount.ID).IsVisitor(_userManager))
        {
            return default;
        }

        var cacheKey = string.Format("my/{0}/{1}", _tenantManager.GetCurrentTenant().Id, _authContext.CurrentAccount.ID);

        var myFolderId = UserRootFolderCache.GetOrAdd(cacheKey, (a) => new Lazy<int>(() => GetFolderIdAndProccessFirstVisitAsync(fileMarker, daoFactory, true).Result));

        return myFolderId.Value;
    }

    protected internal void SetFolderMy(object value)
    {
        var cacheKey = string.Format("my/{0}/{1}", _tenantManager.GetCurrentTenant().Id, value);
        UserRootFolderCache.Remove(cacheKey, out _);
    }

    public async ValueTask<bool> IsFirstVisit(IDaoFactory daoFactory)
    {
        var cacheKey = string.Format("my/{0}/{1}", _tenantManager.GetCurrentTenant().Id, _authContext.CurrentAccount.ID);

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
        return (T)Convert.ChangeType(await GetFolderCommonAsync(fileMarker, daoFactory), typeof(T));
    }

    public async ValueTask<int> GetFolderCommonAsync(FileMarker fileMarker, IDaoFactory daoFactory)
    {
        if (_coreBaseSettings.Personal)
        {
            return default;
        }

        if (!CommonFolderCache.TryGetValue(_tenantManager.GetCurrentTenant().Id, out var commonFolderId))
        {
            commonFolderId = await GetFolderIdAndProccessFirstVisitAsync(fileMarker, daoFactory, false);
            if (!Equals(commonFolderId, 0))
            {
                CommonFolderCache[_tenantManager.GetCurrentTenant().Id] = commonFolderId;
            }
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

        if (IsOutsider)
        {
            return default;
        }

        if (!ShareFolderCache.TryGetValue(_tenantManager.GetCurrentTenant().Id, out var sharedFolderId))
        {
            sharedFolderId = await daoFactory.GetFolderDao<int>().GetFolderIDShareAsync(true);

            if (!sharedFolderId.Equals(default))
            {
                ShareFolderCache[_tenantManager.GetCurrentTenant().Id] = sharedFolderId;
            }
        }

        return sharedFolderId;
    }

    public async ValueTask<T> GetFolderShareAsync<T>(IDaoFactory daoFactory)
    {
        return (T)Convert.ChangeType(await GetFolderShareAsync(daoFactory), typeof(T));
    }

    internal static readonly IDictionary<int, int> RecentFolderCache =
        new ConcurrentDictionary<int, int>(); /*Use SYNCHRONIZED for cross thread blocks*/

    public async ValueTask<int> GetFolderRecentAsync(IDaoFactory daoFactory)
    {
        if (!_authContext.IsAuthenticated)
        {
            return 0;
        }

        if (_userManager.GetUsers(_authContext.CurrentAccount.ID).IsVisitor(_userManager))
        {
            return 0;
        }

        if (!RecentFolderCache.TryGetValue(_tenantManager.GetCurrentTenant().Id, out var recentFolderId))
        {
            var folderDao = daoFactory.GetFolderDao<int>();
            recentFolderId = await folderDao.GetFolderIDRecentAsync(true);

            if (!recentFolderId.Equals(0))
            {
                RecentFolderCache[_tenantManager.GetCurrentTenant().Id] = recentFolderId;
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

        if (_userManager.GetUsers(_authContext.CurrentAccount.ID).IsVisitor(_userManager))
        {
            return 0;
        }

        if (!FavoritesFolderCache.TryGetValue(_tenantManager.GetCurrentTenant().Id, out var favoriteFolderId))
        {
            var folderDao = daoFactory.GetFolderDao<int>();
            favoriteFolderId = await folderDao.GetFolderIDFavoritesAsync(true);

            if (!favoriteFolderId.Equals(0))
            {
                FavoritesFolderCache[_tenantManager.GetCurrentTenant().Id] = favoriteFolderId;
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

        if (_userManager.GetUsers(_authContext.CurrentAccount.ID).IsVisitor(_userManager))
        {
            return 0;
        }

        if (!TemplatesFolderCache.TryGetValue(_tenantManager.GetCurrentTenant().Id, out var templatesFolderId))
        {
            var folderDao = daoFactory.GetFolderDao<int>();
            templatesFolderId = await folderDao.GetFolderIDTemplatesAsync(true);

            if (!templatesFolderId.Equals(0))
            {
                TemplatesFolderCache[_tenantManager.GetCurrentTenant().Id] = templatesFolderId;
            }
        }

        return templatesFolderId;
    }

    internal static readonly IDictionary<string, int> PrivacyFolderCache =
        new ConcurrentDictionary<string, int>(); /*Use SYNCHRONIZED for cross thread blocks*/

    public async ValueTask<T> GetFolderPrivacyAsync<T>(IDaoFactory daoFactory)
    {
        return (T)Convert.ChangeType(await GetFolderPrivacyAsync(daoFactory), typeof(T));
    }

    public async ValueTask<int> GetFolderPrivacyAsync(IDaoFactory daoFactory)
    {
        if (!_authContext.IsAuthenticated)
        {
            return 0;
        }

        if (_userManager.GetUsers(_authContext.CurrentAccount.ID).IsVisitor(_userManager))
        {
            return 0;
        }

        var cacheKey = string.Format("privacy/{0}/{1}", _tenantManager.GetCurrentTenant().Id, _authContext.CurrentAccount.ID);

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
    private readonly ThumbnailSettings _thumbnailSettings;

    public async Task<T> GetFolderTrashAsync<T>(IDaoFactory daoFactory)
    {
        return (T)Convert.ChangeType(await GetFolderTrashAsync(daoFactory), typeof(T));
    }

    public async ValueTask<object> GetFolderTrashAsync(IDaoFactory daoFactory)
    {
        if (IsOutsider)
        {
            return null;
        }

        var cacheKey = string.Format("trash/{0}/{1}", _tenantManager.GetCurrentTenant().Id, _authContext.CurrentAccount.ID);

        if (!TrashFolderCache.TryGetValue(cacheKey, out var trashFolderId))
        {
            trashFolderId = _authContext.IsAuthenticated ? await daoFactory.GetFolderDao<int>().GetFolderIDTrashAsync(true) : 0;
            TrashFolderCache[cacheKey] = trashFolderId;
        }

        return trashFolderId;
    }

    protected internal void SetFolderTrash(object value)
    {
        var cacheKey = string.Format("trash/{0}/{1}", _tenantManager.GetCurrentTenant().Id, value);
        TrashFolderCache.Remove(cacheKey);
    }

    private async Task<int> GetFolderIdAndProccessFirstVisitAsync(FileMarker fileMarker, IDaoFactory daoFactory, bool my)
    {
        var folderDao = (FolderDao)daoFactory.GetFolderDao<int>();
        var fileDao = (FileDao)daoFactory.GetFileDao<int>();

        var id = my ? await folderDao.GetFolderIDUserAsync(false) : await folderDao.GetFolderIDCommonAsync(false);

        if (Equals(id, 0)) //TODO: think about 'null'
        {
            id = my ? await folderDao.GetFolderIDUserAsync(true) : await folderDao.GetFolderIDCommonAsync(true);

            //Copy start document
            if (_settingsManager.LoadForDefaultTenant<AdditionalWhiteLabelSettings>().StartDocsEnabled)
            {
                try
                {
                    var storeTemplate = _globalStore.GetStoreTemplate();

                    var culture = my ? _userManager.GetUsers(_authContext.CurrentAccount.ID).GetCulture() : _tenantManager.GetCurrentTenant().GetCulture();
                    var path = FileConstant.StartDocPath + culture + "/";

                    if (!await storeTemplate.IsDirectoryAsync(path))
                    {
                        path = FileConstant.StartDocPath + "en-US/";
                    }

                    path += my ? "my/" : "corporate/";

                    await SaveStartDocumentAsync(fileMarker, folderDao, fileDao, id, path, storeTemplate);
                }
                catch (Exception ex)
                {
                    _logger.ErrorGetFolderIdAndProccessFirstVisit(ex);
                }
            }
        }

        return id;
    }

    private async Task SaveStartDocumentAsync(FileMarker fileMarker, FolderDao folderDao, FileDao fileDao, int folderId, string path, IDataStore storeTemplate)
    {
            var files = await storeTemplate.ListFilesRelativeAsync("", path, "*", false).ToListAsync();
            foreach (var file in files)
        {
                await SaveFileAsync(fileMarker, fileDao, folderId, path + file, storeTemplate, files);
        }

        await foreach (var folderName in storeTemplate.ListDirectoriesRelativeAsync(path, false))
        {
            var folder = _serviceProvider.GetService<Folder<int>>();
            folder.Title = folderName;
            folder.ParentId = folderId;

            var subFolderId = await folderDao.SaveFolderAsync(folder);

            await SaveStartDocumentAsync(fileMarker, folderDao, fileDao, subFolderId, path + folderName + "/", storeTemplate);
        }
    }

        private async Task SaveFileAsync(FileMarker fileMarker, FileDao fileDao, int folder, string filePath, IDataStore storeTemp, IEnumerable<string> files)
    {
        try
        {
                var fileName = Path.GetFileName(filePath);
                foreach (var ext in Enum.GetValues<ThumbnailExtension>())
                {
                    if (FileUtility.GetFileExtension(filePath) == "." + ext
                        && files.Contains(Regex.Replace(fileName, "\\." + ext + "$", "")))
                        return;
                }

            var file = _serviceProvider.GetService<File<int>>();

            file.Title = fileName;
            file.ParentId = folder;
            file.Comment = FilesCommonResource.CommentCreate;

            using (var stream = await storeTemp.GetReadStreamAsync("", filePath))
            {
                file.ContentLength = stream.CanSeek ? stream.Length : await storeTemp.GetFileSizeAsync("", filePath);
                file = await fileDao.SaveFileAsync(file, stream, false);
            }

            await fileMarker.MarkAsNewAsync(file);
        }
        catch (Exception ex)
        {
            _logger.ErrorSaveFile(ex);
        }
    }

    public bool IsOutsider => _userManager.GetUsers(_authContext.CurrentAccount.ID).IsOutsider(_userManager);
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
    public int FolderMy => _globalFolder.GetFolderMy(_fileMarker, _daoFactory);
    public ValueTask<int> FolderPrivacyAsync => _globalFolder.GetFolderPrivacyAsync(_daoFactory);
    public ValueTask<int> FolderRecentAsync => _globalFolder.GetFolderRecentAsync(_daoFactory);
    public ValueTask<int> FolderFavoritesAsync => _globalFolder.GetFolderFavoritesAsync(_daoFactory);
    public ValueTask<int> FolderTemplatesAsync => _globalFolder.GetFolderTemplatesAsync(_daoFactory);
    public ValueTask<int> FolderVirtualRoomsAsync => _globalFolder.GetFolderVirtualRoomsAsync(_daoFactory);
    public ValueTask<int> FolderArchiveAsync => _globalFolder.GetFolderArchiveAsync(_daoFactory);

    public T GetFolderMy<T>()
    {
        return (T)Convert.ChangeType(FolderMy, typeof(T));
    }

    public async ValueTask<T> GetFolderCommonAsync<T>()
    {
        return (T)Convert.ChangeType(await FolderCommonAsync, typeof(T));
    }

    public async ValueTask<T> GetFolderProjectsAsync<T>()
    {
        return (T)Convert.ChangeType(await FolderProjectsAsync, typeof(T));
    }

    public T GetFolderTrash<T>()
    {
        return (T)Convert.ChangeType(FolderTrash, typeof(T));
    }

    public async ValueTask<T> GetFolderPrivacyAsync<T>()
    {
        return (T)Convert.ChangeType(await FolderPrivacyAsync, typeof(T));
    }

    public async ValueTask<T> GetFolderVirtualRooms<T>()
    {
        return (T)Convert.ChangeType(await FolderVirtualRoomsAsync, typeof(T));
    }

    public async ValueTask<T> GetFolderArchive<T>()
    {
        return (T)Convert.ChangeType(await FolderArchiveAsync, typeof(T));
    }

    public void SetFolderMy<T>(T val)
    {
        _globalFolder.SetFolderMy(val);
    }

    public async ValueTask<T> GetFolderShareAsync<T>()
    {
        return (T)Convert.ChangeType(await FolderShareAsync, typeof(T));
    }
    public ValueTask<int> FolderShareAsync => _globalFolder.GetFolderShareAsync(_daoFactory);

    public object FolderTrash
    {
        get => _globalFolder.GetFolderTrashAsync(_daoFactory).Result;
        set => _globalFolder.SetFolderTrash(value);
    }
}
