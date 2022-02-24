/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Files.Core;
using ASC.Files.Core.Data;
using ASC.Files.Core.Resources;
using ASC.Files.Core.Security;
using ASC.Web.Core;
using ASC.Web.Core.Files;
using ASC.Web.Core.Users;
using ASC.Web.Core.WhiteLabel;
using ASC.Web.Files.Utils;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Constants = ASC.Core.Configuration.Constants;

namespace ASC.Web.Files.Classes
{
    [Singletone]
    public class GlobalNotify
    {
        private ICacheNotify<AscCacheItem> Notify { get; set; }
        public ILog Logger { get; set; }

        public GlobalNotify(ICacheNotify<AscCacheItem> notify, IOptionsMonitor<ILog> options, CoreBaseSettings coreBaseSettings)
        {
            Notify = notify;
            Logger = options.Get("ASC.Files");
            if (coreBaseSettings.Standalone)
            {
                ClearCache();
            }
        }

        private void ClearCache()
        {
            try
            {
                Notify.Subscribe((item) =>
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
                        Logger.Fatal("ClearCache action", e);
                    }
                }, CacheNotifyAction.Any);
            }
            catch (Exception e)
            {
                Logger.Fatal("ClearCache subscribe", e);
            }
        }
    }

    [Scope]
    public class Global
    {
        private IConfiguration Configuration { get; }
        private AuthContext AuthContext { get; }
        private UserManager UserManager { get; }
        private CoreSettings CoreSettings { get; }
        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        private CustomNamingPeople CustomNamingPeople { get; }
        private FileSecurityCommon FileSecurityCommon { get; }

        public Global(
            IConfiguration configuration,
            AuthContext authContext,
            UserManager userManager,
            CoreSettings coreSettings,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            CustomNamingPeople customNamingPeople,
            FileSecurityCommon fileSecurityCommon)
        {
            Configuration = configuration;
            AuthContext = authContext;
            UserManager = userManager;
            CoreSettings = coreSettings;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            CustomNamingPeople = customNamingPeople;
            FileSecurityCommon = fileSecurityCommon;

            ThumbnailExtension = configuration["files:thumbnail:exts"] ?? "png";
        }

        #region Property

        public string ThumbnailExtension { get; set; }

        public const int MaxTitle = 170;

        public static readonly Regex InvalidTitleChars = new Regex("[\t*\\+:\"<>?|\\\\/\\p{Cs}]");

        public bool EnableUploadFilter
        {
            get { return bool.TrueString.Equals(Configuration["files:upload-filter"] ?? "false", StringComparison.InvariantCultureIgnoreCase); }
        }

        public TimeSpan StreamUrlExpire
        {
            get
            {
                int.TryParse(Configuration["files:stream-url-minute"], out var validateTimespan);
                if (validateTimespan <= 0) validateTimespan = 16;
                return TimeSpan.FromMinutes(validateTimespan);
            }
        }

        public bool IsAdministrator
        {
            get { return FileSecurityCommon.IsAdministrator(AuthContext.CurrentAccount.ID); }
        }

        public string GetDocDbKey()
        {
            const string dbKey = "UniqueDocument";
            var resultKey = CoreSettings.GetSetting(dbKey);

            if (!string.IsNullOrEmpty(resultKey)) return resultKey;

            resultKey = Guid.NewGuid().ToString();
            CoreSettings.SaveSetting(dbKey, resultKey);

            return resultKey;
        }

        #endregion

        public static string ReplaceInvalidCharsAndTruncate(string title)
        {
            if (string.IsNullOrEmpty(title)) return title;
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
            if (userId.Equals(AuthContext.CurrentAccount.ID)) return FilesCommonResource.Author_Me;
            if (userId.Equals(Constants.Guest.ID)) return FilesCommonResource.Guest;

            var userInfo = UserManager.GetUsers(userId);
            if (userInfo.Equals(ASC.Core.Users.Constants.LostUser)) return alive ? FilesCommonResource.Guest : CustomNamingPeople.Substitute<FilesCommonResource>("ProfileRemoved");

            return userInfo.DisplayUserName(false, DisplayUserSettingsHelper);
        }
    }

    [Scope]
    public class GlobalStore
    {
        private StorageFactory StorageFactory { get; }
        private TenantManager TenantManager { get; }

        public GlobalStore(StorageFactory storageFactory, TenantManager tenantManager)
        {
            StorageFactory = storageFactory;
            TenantManager = tenantManager;
        }

        public IDataStore GetStore(bool currentTenant = true)
        {
            return StorageFactory.GetStorage(currentTenant ? TenantManager.GetCurrentTenant().TenantId.ToString() : string.Empty, FileConstant.StorageModule);
        }

        public IDataStore GetStoreTemplate()
        {
            return StorageFactory.GetStorage(string.Empty, FileConstant.StorageTemplate);
        }
    }

    [Scope]
    public class GlobalSpace
    {
        private FilesUserSpaceUsage FilesUserSpaceUsage { get; }
        private AuthContext AuthContext { get; }

        public GlobalSpace(FilesUserSpaceUsage filesUserSpaceUsage, AuthContext authContext)
        {
            FilesUserSpaceUsage = filesUserSpaceUsage;
            AuthContext = authContext;
        }

        public Task<long> GetUserUsedSpaceAsync()
        {
            return GetUserUsedSpaceAsync(AuthContext.CurrentAccount.ID);
        }

        public Task<long> GetUserUsedSpaceAsync(Guid userId)
        {
            return FilesUserSpaceUsage.GetUserSpaceUsageAsync(userId);
        }
    }

    [Scope]
    public class GlobalFolder
    {
        private CoreBaseSettings CoreBaseSettings { get; }
        private WebItemManager WebItemManager { get; }
        private WebItemSecurity WebItemSecurity { get; }
        private AuthContext AuthContext { get; }
        private TenantManager TenantManager { get; }
        private UserManager UserManager { get; }
        private SettingsManager SettingsManager { get; }
        private GlobalStore GlobalStore { get; }
        private IServiceProvider ServiceProvider { get; }
        private Global Global { get; }
        private ILog Logger { get; }

        public GlobalFolder(
            CoreBaseSettings coreBaseSettings,
            WebItemManager webItemManager,
            WebItemSecurity webItemSecurity,
            AuthContext authContext,
            TenantManager tenantManager,
            UserManager userManager,
            SettingsManager settingsManager,
            GlobalStore globalStore,
            IOptionsMonitor<ILog> options,
            IServiceProvider serviceProvider,
            Global global
        )
        {
            CoreBaseSettings = coreBaseSettings;
            WebItemManager = webItemManager;
            WebItemSecurity = webItemSecurity;
            AuthContext = authContext;
            TenantManager = tenantManager;
            UserManager = userManager;
            SettingsManager = settingsManager;
            GlobalStore = globalStore;
            ServiceProvider = serviceProvider;
            Global = global;
            Logger = options.Get("ASC.Files");
        }

        internal static readonly IDictionary<int, int> ProjectsRootFolderCache =
            new ConcurrentDictionary<int, int>(); /*Use SYNCHRONIZED for cross thread blocks*/

        public async ValueTask<int> GetFolderProjectsAsync(IDaoFactory daoFactory)
        {
            if (CoreBaseSettings.Personal) return default;

            if (WebItemManager[WebItemManager.ProjectsProductID].IsDisabled(WebItemSecurity, AuthContext)) return default;

            var folderDao = daoFactory.GetFolderDao<int>();
            if (!ProjectsRootFolderCache.TryGetValue(TenantManager.GetCurrentTenant().TenantId, out var result))
            {
                result = await folderDao.GetFolderIDProjectsAsync(true);

                ProjectsRootFolderCache[TenantManager.GetCurrentTenant().TenantId] = result;
            }

            return result;
        }

        public async ValueTask<T> GetFolderProjectsAsync<T>(IDaoFactory daoFactory)
        {
            return (T)Convert.ChangeType(await GetFolderProjectsAsync(daoFactory), typeof(T));
        }

        internal static readonly ConcurrentDictionary<string, Lazy<int>> UserRootFolderCache =
            new ConcurrentDictionary<string, Lazy<int>>(); /*Use SYNCHRONIZED for cross thread blocks*/

        public T GetFolderMy<T>(FileMarker fileMarker, IDaoFactory daoFactory)
        {
            return (T)Convert.ChangeType(GetFolderMy(fileMarker, daoFactory), typeof(T));
        }

        public int GetFolderMy(FileMarker fileMarker, IDaoFactory daoFactory)
        {
            if (!AuthContext.IsAuthenticated) return default;
            if (UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)) return default;

            var cacheKey = string.Format("my/{0}/{1}", TenantManager.GetCurrentTenant().TenantId, AuthContext.CurrentAccount.ID);

            var myFolderId = UserRootFolderCache.GetOrAdd(cacheKey, (a) => new Lazy<int>(() => GetFolderIdAndProccessFirstVisitAsync(fileMarker, daoFactory, true).Result));
            return myFolderId.Value;
        }

        protected internal void SetFolderMy(object value)
        {
            var cacheKey = string.Format("my/{0}/{1}", TenantManager.GetCurrentTenant().TenantId, value);
            UserRootFolderCache.Remove(cacheKey, out _);
        }

        public async ValueTask<bool> IsFirstVisit(IDaoFactory daoFactory)
        {
            var cacheKey = string.Format("my/{0}/{1}", TenantManager.GetCurrentTenant().TenantId, AuthContext.CurrentAccount.ID);

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
            if (CoreBaseSettings.Personal) return default;

            if (!CommonFolderCache.TryGetValue(TenantManager.GetCurrentTenant().TenantId, out var commonFolderId))
            {
                commonFolderId = await GetFolderIdAndProccessFirstVisitAsync(fileMarker, daoFactory, false);
                if (!Equals(commonFolderId, 0))
                    CommonFolderCache[TenantManager.GetCurrentTenant().TenantId] = commonFolderId;
            }
            return commonFolderId;
        }

        internal static readonly IDictionary<int, int> ShareFolderCache =
            new ConcurrentDictionary<int, int>(); /*Use SYNCHRONIZED for cross thread blocks*/

        public async ValueTask<int> GetFolderShareAsync(IDaoFactory daoFactory)
        {
            if (CoreBaseSettings.Personal) return default;
            if (IsOutsider) return default;

            if (!ShareFolderCache.TryGetValue(TenantManager.GetCurrentTenant().TenantId, out var sharedFolderId))
            {
                sharedFolderId = await daoFactory.GetFolderDao<int>().GetFolderIDShareAsync(true);

                if (!sharedFolderId.Equals(default))
                    ShareFolderCache[TenantManager.GetCurrentTenant().TenantId] = sharedFolderId;
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
            if (!AuthContext.IsAuthenticated) return 0;
            if (UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)) return 0;

            if (!RecentFolderCache.TryGetValue(TenantManager.GetCurrentTenant().TenantId, out var recentFolderId))
            {
                var folderDao = daoFactory.GetFolderDao<int>();
                recentFolderId = await folderDao.GetFolderIDRecentAsync(true);

                if (!recentFolderId.Equals(0))
                    RecentFolderCache[TenantManager.GetCurrentTenant().TenantId] = recentFolderId;
            }

            return recentFolderId;
        }

        internal static readonly IDictionary<int, int> FavoritesFolderCache =
            new ConcurrentDictionary<int, int>(); /*Use SYNCHRONIZED for cross thread blocks*/

        public async ValueTask<int> GetFolderFavoritesAsync(IDaoFactory daoFactory)
        {
            if (!AuthContext.IsAuthenticated) return 0;
            if (UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)) return 0;

            if (!FavoritesFolderCache.TryGetValue(TenantManager.GetCurrentTenant().TenantId, out var favoriteFolderId))
            {
                var folderDao = daoFactory.GetFolderDao<int>();
                favoriteFolderId = await folderDao.GetFolderIDFavoritesAsync(true);

                if (!favoriteFolderId.Equals(0))
                    FavoritesFolderCache[TenantManager.GetCurrentTenant().TenantId] = favoriteFolderId;
            }

            return favoriteFolderId;
        }

        internal static readonly IDictionary<int, int> TemplatesFolderCache =
            new ConcurrentDictionary<int, int>(); /*Use SYNCHRONIZED for cross thread blocks*/

        public async ValueTask<int> GetFolderTemplatesAsync(IDaoFactory daoFactory)
        {
            if (!AuthContext.IsAuthenticated) return 0;
            if (UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)) return 0;

            if (!TemplatesFolderCache.TryGetValue(TenantManager.GetCurrentTenant().TenantId, out var templatesFolderId))
            {
                var folderDao = daoFactory.GetFolderDao<int>();
                templatesFolderId = await folderDao.GetFolderIDTemplatesAsync(true);

                if (!templatesFolderId.Equals(0))
                    TemplatesFolderCache[TenantManager.GetCurrentTenant().TenantId] = templatesFolderId;
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
            if (!AuthContext.IsAuthenticated) return 0;
            if (UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)) return 0;

            var cacheKey = string.Format("privacy/{0}/{1}", TenantManager.GetCurrentTenant().TenantId, AuthContext.CurrentAccount.ID);

            if (!PrivacyFolderCache.TryGetValue(cacheKey, out var privacyFolderId))
            {
                var folderDao = daoFactory.GetFolderDao<int>();
                privacyFolderId = await folderDao.GetFolderIDPrivacyAsync(true);

                if (!Equals(privacyFolderId, 0))
                    PrivacyFolderCache[cacheKey] = privacyFolderId;
            }
            return privacyFolderId;
        }


        internal static readonly IDictionary<string, object> TrashFolderCache =
            new ConcurrentDictionary<string, object>(); /*Use SYNCHRONIZED for cross thread blocks*/

        public async Task<T> GetFolderTrashAsync<T>(IDaoFactory daoFactory)
        {
            return (T)Convert.ChangeType(await GetFolderTrashAsync(daoFactory), typeof(T));
        }

        public async ValueTask<object> GetFolderTrashAsync(IDaoFactory daoFactory)
        {
            if (IsOutsider) return null;

            var cacheKey = string.Format("trash/{0}/{1}", TenantManager.GetCurrentTenant().TenantId, AuthContext.CurrentAccount.ID);

            if (!TrashFolderCache.TryGetValue(cacheKey, out var trashFolderId))
            {
                trashFolderId = AuthContext.IsAuthenticated ? await daoFactory.GetFolderDao<int>().GetFolderIDTrashAsync(true) : 0;
                TrashFolderCache[cacheKey] = trashFolderId;
            }
            return trashFolderId;
        }

        protected internal void SetFolderTrash(object value)
        {
            var cacheKey = string.Format("trash/{0}/{1}", TenantManager.GetCurrentTenant().TenantId, value);
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
                if (SettingsManager.LoadForDefaultTenant<AdditionalWhiteLabelSettings>().StartDocsEnabled)
                {
                    try
                    {
                        var storeTemplate = GlobalStore.GetStoreTemplate();

                        var culture = my ? UserManager.GetUsers(AuthContext.CurrentAccount.ID).GetCulture() : TenantManager.GetCurrentTenant().GetCulture();
                        var path = FileConstant.StartDocPath + culture + "/";

                        if (!await storeTemplate.IsDirectoryAsync(path))
                            path = FileConstant.StartDocPath + "en-US/";
                        path += my ? "my/" : "corporate/";

                        await SaveStartDocumentAsync(fileMarker, folderDao, fileDao, id, path, storeTemplate);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
            }

            return id;
        }

        private async Task SaveStartDocumentAsync(FileMarker fileMarker, FolderDao folderDao, FileDao fileDao, int folderId, string path, IDataStore storeTemplate)
        {
            await foreach (var file in storeTemplate.ListFilesRelativeAsync("", path, "*", false))
            {
                await SaveFileAsync(fileMarker, fileDao, folderId, path + file, storeTemplate);
            }

            await foreach (var folderName in storeTemplate.ListDirectoriesRelativeAsync(path, false))
            {
                var folder = ServiceProvider.GetService<Folder<int>>();
                folder.Title = folderName;
                folder.FolderID = folderId;

                var subFolderId = await folderDao.SaveFolderAsync(folder);

                await SaveStartDocumentAsync(fileMarker, folderDao, fileDao, subFolderId, path + folderName + "/", storeTemplate);
            }
        }

        private async Task SaveFileAsync(FileMarker fileMarker, FileDao fileDao, int folder, string filePath, IDataStore storeTemp)
        {
            try
            {
                if (FileUtility.GetFileExtension(filePath) == "." + Global.ThumbnailExtension
                    && await storeTemp.IsFileAsync("", Regex.Replace(filePath, "\\." + Global.ThumbnailExtension + "$", "")))
                    return;

                var fileName = Path.GetFileName(filePath);
                var file = ServiceProvider.GetService<File<int>>();

                file.Title = fileName;
                file.FolderID = folder;
                file.Comment = FilesCommonResource.CommentCreate;

                using (var stream = await storeTemp.GetReadStreamAsync("", filePath))
                {
                    file.ContentLength = stream.CanSeek ? stream.Length : await storeTemp.GetFileSizeAsync("", filePath);
                    file = await fileDao.SaveFileAsync(file, stream, false);
                }

                var pathThumb = filePath + "." + Global.ThumbnailExtension;
                if (await storeTemp.IsFileAsync("", pathThumb))
                {
                    using (var streamThumb = await storeTemp.GetReadStreamAsync("", pathThumb))
                    {
                        await fileDao.SaveThumbnailAsync(file, streamThumb);
                    }
                    file.ThumbnailStatus = Thumbnail.Created;
                }

                await fileMarker.MarkAsNewAsync(file);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public bool IsOutsider
        {
            get { return UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsOutsider(UserManager); }
        }
    }

    [Scope]
    public class GlobalFolderHelper
    {
        private FileMarker FileMarker { get; }
        private IDaoFactory DaoFactory { get; }
        private GlobalFolder GlobalFolder { get; }

        public GlobalFolderHelper(FileMarker fileMarker, IDaoFactory daoFactory, GlobalFolder globalFolder)
        {
            FileMarker = fileMarker;
            DaoFactory = daoFactory;
            GlobalFolder = globalFolder;
        }

        public ValueTask<int> FolderProjectsAsync => GlobalFolder.GetFolderProjectsAsync(DaoFactory);
        public ValueTask<int> FolderCommonAsync => GlobalFolder.GetFolderCommonAsync(FileMarker, DaoFactory);
        public int FolderMy => GlobalFolder.GetFolderMy(FileMarker, DaoFactory);
        public ValueTask<int> FolderPrivacyAsync => GlobalFolder.GetFolderPrivacyAsync(DaoFactory);
        public ValueTask<int> FolderRecentAsync => GlobalFolder.GetFolderRecentAsync(DaoFactory);
        public ValueTask<int> FolderFavoritesAsync => GlobalFolder.GetFolderFavoritesAsync(DaoFactory);
        public ValueTask<int> FolderTemplatesAsync => GlobalFolder.GetFolderTemplatesAsync(DaoFactory);
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

        public void SetFolderMy<T>(T val)
        {
            GlobalFolder.SetFolderMy(val);
        }

        public async ValueTask<T> GetFolderShareAsync<T>()
        {
            return (T)Convert.ChangeType(await FolderShareAsync, typeof(T));
        }
        public ValueTask<int> FolderShareAsync => GlobalFolder.GetFolderShareAsync(DaoFactory);

        public object FolderTrash
        {
            get
            {
                return GlobalFolder.GetFolderTrashAsync(DaoFactory).Result;
            }
            set
            {
                GlobalFolder.SetFolderTrash(value);
            }
        }
    }
}