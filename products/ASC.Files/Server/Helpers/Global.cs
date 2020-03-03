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

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Files.Core;
using ASC.Files.Core.Data;
using ASC.Files.Core.Security;
using ASC.Files.Resources;
using ASC.Web.Core;
using ASC.Web.Core.Users;
using ASC.Web.Core.WhiteLabel;
using ASC.Web.Files.Utils;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Constants = ASC.Core.Configuration.Constants;

namespace ASC.Web.Files.Classes
{
    public class GlobalNotify
    {
        public ICacheNotify<AscCacheItem> Notify { get; set; }
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

    public class Global
    {
        public IConfiguration Configuration { get; }
        public AuthContext AuthContext { get; }
        public UserManager UserManager { get; }
        public CoreSettings CoreSettings { get; }
        public DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        public CustomNamingPeople CustomNamingPeople { get; }
        public FileSecurityCommon FileSecurityCommon { get; }

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
        }

        #region Property

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

    public class GlobalStore
    {
        public StorageFactory StorageFactory { get; }
        public TenantManager TenantManager { get; }

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

    public class GlobalSpace
    {
        public FilesUserSpaceUsage FilesUserSpaceUsage { get; }
        public AuthContext AuthContext { get; }

        public GlobalSpace(FilesUserSpaceUsage filesUserSpaceUsage, AuthContext authContext)
        {
            FilesUserSpaceUsage = filesUserSpaceUsage;
            AuthContext = authContext;
        }

        public long GetUserUsedSpace()
        {
            return GetUserUsedSpace(AuthContext.CurrentAccount.ID);
        }

        public long GetUserUsedSpace(Guid userId)
        {
            return FilesUserSpaceUsage.GetUserSpaceUsage(userId);
        }
    }

    public class GlobalFolder
    {
        public CoreBaseSettings CoreBaseSettings { get; }
        public WebItemManager WebItemManager { get; }
        public WebItemSecurity WebItemSecurity { get; }
        public AuthContext AuthContext { get; }
        public TenantManager TenantManager { get; }
        public UserManager UserManager { get; }
        public SettingsManager SettingsManager { get; }
        public GlobalStore GlobalStore { get; }
        public IServiceProvider ServiceProvider { get; }
        public ILog Logger { get; }

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
            IServiceProvider serviceProvider
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
            Logger = options.Get("ASC.Files");
        }

        internal static readonly IDictionary<int, int> ProjectsRootFolderCache =
            new ConcurrentDictionary<int, int>(); /*Use SYNCHRONIZED for cross thread blocks*/

        public int GetFolderProjects(IDaoFactory daoFactory)
        {
            if (CoreBaseSettings.Personal) return default;

            if (WebItemManager[WebItemManager.ProjectsProductID].IsDisabled(WebItemSecurity, AuthContext)) return default;

            var folderDao = daoFactory.GetFolderDao<int>();
            if (!ProjectsRootFolderCache.TryGetValue(TenantManager.GetCurrentTenant().TenantId, out var result))
            {
                result = folderDao.GetFolderIDProjects(true);

                ProjectsRootFolderCache[TenantManager.GetCurrentTenant().TenantId] = result;
            }

            return result;
        }

        public T GetFolderProjects<T>(IDaoFactory daoFactory)
        {
            return (T)Convert.ChangeType(GetFolderProjects(daoFactory), typeof(T));
        }

        internal static readonly IDictionary<string, int> UserRootFolderCache =
            new ConcurrentDictionary<string, int>(); /*Use SYNCHRONIZED for cross thread blocks*/

        public int GetFolderMy(FileMarker fileMarker, IDaoFactory daoFactory)
        {
            if (!AuthContext.IsAuthenticated) return default;
            if (UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)) return default;

            var cacheKey = string.Format("my/{0}/{1}", TenantManager.GetCurrentTenant().TenantId, AuthContext.CurrentAccount.ID);

            if (!UserRootFolderCache.TryGetValue(cacheKey, out var myFolderId))
            {
                myFolderId = GetFolderIdAndProccessFirstVisit<int>(fileMarker, daoFactory, true);
                if (!Equals(myFolderId, 0))
                    UserRootFolderCache[cacheKey] = myFolderId;
            }
            return myFolderId;
        }

        protected internal void SetFolderMy(object value)
        {
            var cacheKey = string.Format("my/{0}/{1}", TenantManager.GetCurrentTenant().TenantId, value);
            UserRootFolderCache.Remove(cacheKey);
        }

        internal static readonly IDictionary<int, int> CommonFolderCache =
                new ConcurrentDictionary<int, int>(); /*Use SYNCHRONIZED for cross thread blocks*/

        public T GetFolderCommon<T>(FileMarker fileMarker, IDaoFactory daoFactory)
        {
            return (T)Convert.ChangeType(GetFolderCommon(fileMarker, daoFactory), typeof(T));
        }

        public int GetFolderCommon(FileMarker fileMarker, IDaoFactory daoFactory)
        {
            if (CoreBaseSettings.Personal) return default;

            if (!CommonFolderCache.TryGetValue(TenantManager.GetCurrentTenant().TenantId, out var commonFolderId))
            {
                commonFolderId = GetFolderIdAndProccessFirstVisit<int>(fileMarker, daoFactory, false);
                if (!Equals(commonFolderId, 0))
                    CommonFolderCache[TenantManager.GetCurrentTenant().TenantId] = commonFolderId;
            }
            return commonFolderId;
        }

        internal static readonly IDictionary<int, int> ShareFolderCache =
            new ConcurrentDictionary<int, int>(); /*Use SYNCHRONIZED for cross thread blocks*/

        public int GetFolderShare(IDaoFactory daoFactory)
        {
            if (CoreBaseSettings.Personal) return default;
            if (IsOutsider) return default;

            if (!ShareFolderCache.TryGetValue(TenantManager.GetCurrentTenant().TenantId, out var sharedFolderId))
            {
                sharedFolderId = daoFactory.GetFolderDao<int>().GetFolderIDShare(true);

                if (!sharedFolderId.Equals(default))
                    ShareFolderCache[TenantManager.GetCurrentTenant().TenantId] = sharedFolderId;
            }

            return sharedFolderId;
        }

        public T GetFolderShare<T>(IDaoFactory daoFactory)
        {
            return (T)Convert.ChangeType(GetFolderShare(daoFactory), typeof(T));
        }

        internal static readonly IDictionary<string, object> TrashFolderCache =
            new ConcurrentDictionary<string, object>(); /*Use SYNCHRONIZED for cross thread blocks*/

        public object GetFolderTrash(IFolderDao folderDao)
        {
            if (IsOutsider) return null;

            var cacheKey = string.Format("trash/{0}/{1}", TenantManager.GetCurrentTenant().TenantId, AuthContext.CurrentAccount.ID);

            if (!TrashFolderCache.TryGetValue(cacheKey, out var trashFolderId))
            {
                trashFolderId = AuthContext.IsAuthenticated ? folderDao.GetFolderIDTrash(true) : 0;
                TrashFolderCache[cacheKey] = trashFolderId;
            }
            return trashFolderId;
        }

        protected internal void SetFolderTrash(object value)
        {
            var cacheKey = string.Format("trash/{0}/{1}", TenantManager.GetCurrentTenant().TenantId, value);
            TrashFolderCache.Remove(cacheKey);
        }

        private T GetFolderIdAndProccessFirstVisit<T>(FileMarker fileMarker, IDaoFactory daoFactory, bool my)
        {
            var folderDao = daoFactory.GetFolderDao<T>();
            var fileDao = daoFactory.GetFileDao<T>();

            var id = my ? folderDao.GetFolderIDUser(false) : folderDao.GetFolderIDCommon(false);

            if (Equals(id, 0)) //TODO: think about 'null'
            {
                id = my ? folderDao.GetFolderIDUser(true) : folderDao.GetFolderIDCommon(true);

                //Copy start document
                if (AdditionalWhiteLabelSettings.Instance(SettingsManager).StartDocsEnabled)
                {
                    try
                    {
                        var storeTemplate = GlobalStore.GetStoreTemplate();

                        var culture = my ? UserManager.GetUsers(AuthContext.CurrentAccount.ID).GetCulture() : TenantManager.GetCurrentTenant().GetCulture();
                        var path = FileConstant.StartDocPath + culture + "/";

                        if (!storeTemplate.IsDirectory(path))
                            path = FileConstant.StartDocPath + "default/";
                        path += my ? "my/" : "corporate/";

                        SaveStartDocument(fileMarker, folderDao, fileDao, id, path, storeTemplate);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
            }

            return id;
        }

        private void SaveStartDocument<T>(FileMarker fileMarker, IFolderDao<T> folderDao, IFileDao<T> fileDao, T folderId, string path, IDataStore storeTemplate)
        {
            foreach (var file in storeTemplate.ListFilesRelative("", path, "*", false))
            {
                SaveFile(fileMarker, fileDao, folderId, path + file, storeTemplate);
            }

            foreach (var folderName in storeTemplate.ListDirectoriesRelative(path, false))
            {
                var folder = ServiceProvider.GetService<Folder<T>>();
                folder.Title = folderName;
                folder.ParentFolderID = folderId;

                var subFolderId = folderDao.SaveFolder(folder);

                SaveStartDocument(fileMarker, folderDao, fileDao, subFolderId, path + folderName + "/", storeTemplate);
            }
        }

        private void SaveFile<T>(FileMarker fileMarker, IFileDao<T> fileDao, T folder, string filePath, IDataStore storeTemp)
        {
            using var stream = storeTemp.GetReadStream("", filePath);
            var fileName = Path.GetFileName(filePath);
            var file = ServiceProvider.GetService<File<T>>();

            file.Title = fileName;
            file.ContentLength = stream.CanSeek ? stream.Length : storeTemp.GetFileSize("", filePath);
            file.FolderID = folder;
            file.Comment = FilesCommonResource.CommentCreate;

            stream.Position = 0;
            try
            {
                file = fileDao.SaveFile(file, stream);

                fileMarker.MarkAsNew<T>(file);
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

    public class GlobalFolderHelper
    {
        public FileMarker FileMarker { get; }
        public IDaoFactory DaoFactory { get; }
        public GlobalFolder GlobalFolder { get; }

        public GlobalFolderHelper(FileMarker fileMarker, IDaoFactory daoFactory, GlobalFolder globalFolder)
        {
            FileMarker = fileMarker;
            DaoFactory = daoFactory;
            GlobalFolder = globalFolder;
        }

        public int FolderProjects => GlobalFolder.GetFolderProjects(DaoFactory);
        public int FolderCommon => GlobalFolder.GetFolderCommon(FileMarker, DaoFactory);
        public int FolderMy => GlobalFolder.GetFolderMy(FileMarker, DaoFactory);

        public T GetFolderMy<T>() => (T)Convert.ChangeType(FolderMy, typeof(T));
        public T GetFolderCommon<T>() => (T)Convert.ChangeType(FolderCommon, typeof(T));
        public T GetFolderProjects<T>() => (T)Convert.ChangeType(FolderProjects, typeof(T));

        public void SetFolderMy<T>(T val)
        {
            GlobalFolder.SetFolderMy(val);
        }

        public T GetFolderShare<T>()
        {
            return (T)Convert.ChangeType(FolderShare, typeof(T));
        }

        public int FolderShare => GlobalFolder.GetFolderShare(DaoFactory);

        public object FolderTrash
        {
            get
            {
                return GlobalFolder.GetFolderTrash(DaoFactory.FolderDao);
            }
            set
            {
                GlobalFolder.SetFolderTrash(value);
            }
        }
    }

    public static class GlobalExtention
    {
        public static DIHelper AddGlobalNotifyService(this DIHelper services)
        {
            services.TryAddSingleton<GlobalNotify>();

            return services
                .AddKafkaService()
                .AddCoreBaseSettingsService();
        }

        public static DIHelper AddGlobalService(this DIHelper services)
        {
            services.TryAddScoped<Global>();

            return services
                .AddAuthContextService()
                .AddUserManagerService()
                .AddCoreSettingsService()
                .AddTenantManagerService()
                .AddDisplayUserSettingsService()
                .AddCustomNamingPeopleService()
                .AddFileSecurityCommonService();
        }

        public static DIHelper AddGlobalStoreService(this DIHelper services)
        {
            services.TryAddScoped<GlobalStore>();

            return services
                .AddStorageFactoryService()
                .AddTenantManagerService();
        }

        public static DIHelper AddGlobalSpaceService(this DIHelper services)
        {
            services.TryAddScoped<GlobalSpace>();

            return services
                .AddFilesUserSpaceUsageService()
                .AddAuthContextService();
        }
        public static DIHelper AddGlobalFolderService(this DIHelper services)
        {
            services.TryAddScoped<GlobalFolder>();

            return services
                .AddCoreBaseSettingsService()
                .AddWebItemManager()
                .AddWebItemSecurity()
                .AddAuthContextService()
                .AddTenantManagerService()
                .AddUserManagerService()
                .AddSettingsManagerService()
                .AddGlobalStoreService();
        }

        public static DIHelper AddGlobalFolderHelperService(this DIHelper services)
        {
            services.TryAddScoped<GlobalFolderHelper>();

            return services
                .AddGlobalFolderService()
                .AddDaoFactoryService()
                .AddFileMarkerService()
                ;
        }
    }
}