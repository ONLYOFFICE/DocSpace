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
using ASC.Web.Core;
using ASC.Web.Core.Users;
using ASC.Web.Core.WhiteLabel;
using ASC.Web.Files.Resources;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Files.Utils;

using Autofac;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using Constants = ASC.Core.Configuration.Constants;
using File = ASC.Files.Core.File;

namespace ASC.Web.Files.Classes
{
    public class Global
    {
        public ICacheNotify<AscCacheItem> Notify { get; set; }
        public CoreBaseSettings CoreBaseSettings { get; }
        public IConfiguration Configuration { get; }
        public AuthContext AuthContext { get; }
        public UserManager UserManager { get; }
        public CoreSettings CoreSettings { get; }
        public WebItemManager WebItemManager { get; }
        public WebItemSecurity WebItemSecurity { get; }
        public TenantManager TenantManager { get; }
        public StorageFactory StorageFactory { get; }
        public DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        public CustomNamingPeople CustomNamingPeople { get; }
        public SettingsManager SettingsManager { get; }

        public Global(
            IContainer container,
            CoreBaseSettings coreBaseSettings,
            IOptionsMonitor<ILog> options,
            ICacheNotify<AscCacheItem> notify,
            IConfiguration configuration,
            AuthContext authContext,
            UserManager userManager,
            CoreSettings coreSettings,
            WebItemManager webItemManager,
            WebItemSecurity webItemSecurity,
            TenantManager tenantManager,
            StorageFactory storageFactory,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            CustomNamingPeople customNamingPeople,
            SettingsManager settingsManager)
        {
            try
            {
                Logger = options.CurrentValue;
                Notify = notify;
                if (!container.TryResolve(out IDaoFactory factory))
                {
                    factory = new DaoFactory();
                    Logger.Fatal("Could not resolve IDaoFactory instance. Using default DaoFactory instead.");
                }

                if (!container.TryResolve(out IFileStorageService storageService))
                {
                    storageService = new FileStorageServiceController();
                    Logger.Fatal("Could not resolve IFileStorageService instance. Using default FileStorageServiceController instead.");
                }

                DaoFactory = factory;
                FileStorageService = storageService;
                SocketManager = new SocketManager();
                if (coreBaseSettings.Standalone)
                {
                    ClearCache();
                }
            }
            catch (Exception error)
            {
                Logger.Fatal("Could not resolve IDaoFactory instance. Using default DaoFactory instead.", error);
                DaoFactory = new DaoFactory();
                FileStorageService = new FileStorageServiceController();
            }

            CoreBaseSettings = coreBaseSettings;
            Configuration = configuration;
            AuthContext = authContext;
            UserManager = userManager;
            CoreSettings = coreSettings;
            WebItemManager = webItemManager;
            WebItemSecurity = webItemSecurity;
            TenantManager = tenantManager;
            StorageFactory = storageFactory;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            CustomNamingPeople = customNamingPeople;
            SettingsManager = settingsManager;
        }

        private void ClearCache()
        {
            try
            {
                Notify.Subscribe((item) =>
                {
                    try
                    {
                        ProjectsRootFolderCache.Clear();
                        UserRootFolderCache.Clear();
                        CommonFolderCache.Clear();
                        ShareFolderCache.Clear();
                        TrashFolderCache.Clear();
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
            get { return FileSecurity.IsAdministrator(AuthContext.CurrentAccount.ID); }
        }

        public bool IsOutsider
        {
            get { return UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsOutsider(UserManager); }
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

        #region GlobalFolderID

        private static readonly IDictionary<int, object> ProjectsRootFolderCache =
            new ConcurrentDictionary<int, object>(); /*Use SYNCHRONIZED for cross thread blocks*/

        public object FolderProjects
        {
            get
            {
                if (CoreBaseSettings.Personal) return null;

                if (WebItemManager[WebItemManager.ProjectsProductID].IsDisabled(WebItemSecurity, AuthContext)) return null;

                using (var folderDao = DaoFactory.GetFolderDao())
                {
                    object result;
                    if (!ProjectsRootFolderCache.TryGetValue(TenantManager.GetCurrentTenant().TenantId, out result))
                    {
                        result = folderDao.GetFolderIDProjects(true);

                        ProjectsRootFolderCache[TenantManager.GetCurrentTenant().TenantId] = result;
                    }

                    return result;
                }
            }
        }

        private static readonly IDictionary<string, object> UserRootFolderCache =
            new ConcurrentDictionary<string, object>(); /*Use SYNCHRONIZED for cross thread blocks*/

        public object FolderMy
        {
            get
            {
                if (!AuthContext.IsAuthenticated) return 0;
                if (UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)) return 0;

                var cacheKey = string.Format("my/{0}/{1}", TenantManager.GetCurrentTenant().TenantId, AuthContext.CurrentAccount.ID);

                if (!UserRootFolderCache.TryGetValue(cacheKey, out var myFolderId))
                {
                    myFolderId = GetFolderIdAndProccessFirstVisit(true);
                    if (!Equals(myFolderId, 0))
                        UserRootFolderCache[cacheKey] = myFolderId;
                }
                return myFolderId;
            }
            protected internal set
            {
                var cacheKey = string.Format("my/{0}/{1}", TenantManager.GetCurrentTenant().TenantId, value);
                UserRootFolderCache.Remove(cacheKey);
            }
        }

        private static readonly IDictionary<int, object> CommonFolderCache =
            new ConcurrentDictionary<int, object>(); /*Use SYNCHRONIZED for cross thread blocks*/

        public object FolderCommon
        {
            get
            {
                if (CoreBaseSettings.Personal) return null;

                if (!CommonFolderCache.TryGetValue(TenantManager.GetCurrentTenant().TenantId, out var commonFolderId))
                {
                    commonFolderId = GetFolderIdAndProccessFirstVisit(false);
                    if (!Equals(commonFolderId, 0))
                        CommonFolderCache[TenantManager.GetCurrentTenant().TenantId] = commonFolderId;
                }
                return commonFolderId;
            }
        }

        private static readonly IDictionary<int, object> ShareFolderCache =
            new ConcurrentDictionary<int, object>(); /*Use SYNCHRONIZED for cross thread blocks*/

        public object FolderShare
        {
            get
            {
                if (CoreBaseSettings.Personal) return null;
                if (IsOutsider) return null;

                if (!ShareFolderCache.TryGetValue(TenantManager.GetCurrentTenant().TenantId, out var sharedFolderId))
                {
                    using (var folderDao = DaoFactory.GetFolderDao())
                    {
                        sharedFolderId = folderDao.GetFolderIDShare(true);
                    }

                    if (!sharedFolderId.Equals(0))
                        ShareFolderCache[TenantManager.GetCurrentTenant().TenantId] = sharedFolderId;
                }

                return sharedFolderId;
            }
        }

        private static readonly IDictionary<string, object> TrashFolderCache =
            new ConcurrentDictionary<string, object>(); /*Use SYNCHRONIZED for cross thread blocks*/

        public object FolderTrash
        {
            get
            {
                if (IsOutsider) return null;

                var cacheKey = string.Format("trash/{0}/{1}", TenantManager.GetCurrentTenant().TenantId, AuthContext.CurrentAccount.ID);

                if (!TrashFolderCache.TryGetValue(cacheKey, out var trashFolderId))
                {
                    using (var folderDao = DaoFactory.GetFolderDao())
                        trashFolderId = AuthContext.IsAuthenticated ? folderDao.GetFolderIDTrash(true) : 0;
                    TrashFolderCache[cacheKey] = trashFolderId;
                }
                return trashFolderId;
            }
            protected internal set
            {
                var cacheKey = string.Format("trash/{0}/{1}", TenantManager.GetCurrentTenant().TenantId, value);
                TrashFolderCache.Remove(cacheKey);
            }
        }

        #endregion

        #endregion

        public ILog Logger { get; set; }

        public static IDaoFactory DaoFactory { get; private set; }

        public EncryptedDataDao DaoEncryptedData
        {
            get { return new EncryptedDataDao(TenantManager.GetCurrentTenant().TenantId, FileConstant.DatabaseId); }
        }

        public static IFileStorageService FileStorageService { get; private set; }

        public static SocketManager SocketManager { get; private set; }

        public IDataStore GetStore(bool currentTenant = true)
        {
            return StorageFactory.GetStorage(currentTenant ? TenantManager.GetCurrentTenant().TenantId.ToString() : string.Empty, FileConstant.StorageModule);
        }

        public IDataStore GetStoreTemplate()
        {
            return StorageFactory.GetStorage(string.Empty, FileConstant.StorageTemplate);
        }

        public static FileSecurity GetFilesSecurity()
        {
            return new FileSecurity(DaoFactory);
        }

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

        #region Generate start documents

        private object GetFolderIdAndProccessFirstVisit(bool my)
        {
            using (var folderDao = DaoFactory.GetFolderDao())
            using (var fileDao = DaoFactory.GetFileDao())
            {
                var id = my ? folderDao.GetFolderIDUser(false) : folderDao.GetFolderIDCommon(false);

                if (Equals(id, 0)) //TODO: think about 'null'
                {
                    id = my ? folderDao.GetFolderIDUser(true) : folderDao.GetFolderIDCommon(true);

                    //Copy start document
                    if (AdditionalWhiteLabelSettings.Instance(SettingsManager).StartDocsEnabled)
                    {
                        try
                        {
                            var storeTemplate = GetStoreTemplate();

                            var culture = my ? UserManager.GetUsers(AuthContext.CurrentAccount.ID).GetCulture() : TenantManager.GetCurrentTenant().GetCulture();
                            var path = FileConstant.StartDocPath + culture + "/";

                            if (!storeTemplate.IsDirectory(path))
                                path = FileConstant.StartDocPath + "default/";
                            path += my ? "my/" : "corporate/";

                            SaveStartDocument(folderDao, fileDao, id, path, storeTemplate);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                    }
                }

                return id;
            }
        }

        private static void SaveStartDocument(IFolderDao folderDao, IFileDao fileDao, object folderId, string path, IDataStore storeTemplate)
        {
            foreach (var file in storeTemplate.ListFilesRelative("", path, "*", false))
            {
                SaveFile(fileDao, folderId, path + file, storeTemplate);
            }

            foreach (var folderName in storeTemplate.ListDirectoriesRelative(path, false))
            {
                var subFolderId = folderDao.SaveFolder(new Folder
                {
                    Title = folderName,
                    ParentFolderID = folderId
                });

                SaveStartDocument(folderDao, fileDao, subFolderId, path + folderName + "/", storeTemplate);
            }
        }

        private static void SaveFile(IFileDao fileDao, object folder, string filePath, IDataStore storeTemp)
        {
            using (var stream = storeTemp.GetReadStream("", filePath))
            {
                var fileName = Path.GetFileName(filePath);
                var file = new File
                {
                    Title = fileName,
                    ContentLength = stream.CanSeek ? stream.Length : storeTemp.GetFileSize("", filePath),
                    FolderID = folder,
                    Comment = FilesCommonResource.CommentCreate,
                };
                stream.Position = 0;
                try
                {
                    file = fileDao.SaveFile(file, stream);

                    FileMarker.MarkAsNew(file);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        #endregion


        public long GetUserUsedSpace()
        {
            return GetUserUsedSpace(AuthContext.CurrentAccount.ID);
        }

        public long GetUserUsedSpace(Guid userId)
        {
            var spaceUsageManager = new FilesSpaceUsageStatManager() as IUserSpaceUsage;

            return spaceUsageManager.GetUserSpaceUsage(userId);
        }
    }
}