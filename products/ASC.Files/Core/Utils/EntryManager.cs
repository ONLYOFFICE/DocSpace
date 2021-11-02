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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Users;
using ASC.Files.Core;
using ASC.Files.Core.Resources;
using ASC.Files.Core.Security;
using ASC.Web.Core.Files;
using ASC.Web.Files.Api;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Core;
using ASC.Web.Files.Helpers;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.ThirdPartyApp;
using ASC.Web.Studio.Core;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;

using FileShare = ASC.Files.Core.Security.FileShare;

namespace ASC.Web.Files.Utils
{
    [Scope]
    public class LockerManager
    {
        private AuthContext AuthContext { get; }
        private IDaoFactory DaoFactory { get; }

        public LockerManager(AuthContext authContext, IDaoFactory daoFactory)
        {
            AuthContext = authContext;
            DaoFactory = daoFactory;
        }

        public bool FileLockedForMe<T>(T fileId, Guid userId = default)
        {
            var app = ThirdPartySelector.GetAppByFileId(fileId.ToString());
            if (app != null)
            {
                return false;
            }

            userId = userId == default ? AuthContext.CurrentAccount.ID : userId;
            var tagDao = DaoFactory.GetTagDao<T>();
            var lockedBy = FileLockedBy(fileId, tagDao);
            return lockedBy != Guid.Empty && lockedBy != userId;
        }

        public Guid FileLockedBy<T>(T fileId, ITagDao<T> tagDao)
        {
            var tagLock = tagDao.GetTags(fileId, FileEntryType.File, TagType.Locked).FirstOrDefault();
            return tagLock != null ? tagLock.Owner : Guid.Empty;
        }
    }

    [Scope]
    public class BreadCrumbsManager
    {
        private IDaoFactory DaoFactory { get; }
        private FileSecurity FileSecurity { get; }
        private GlobalFolderHelper GlobalFolderHelper { get; }
        private AuthContext AuthContext { get; }

        public BreadCrumbsManager(
            IDaoFactory daoFactory,
            FileSecurity fileSecurity,
            GlobalFolderHelper globalFolderHelper,
            AuthContext authContext)
        {
            DaoFactory = daoFactory;
            FileSecurity = fileSecurity;
            GlobalFolderHelper = globalFolderHelper;
            AuthContext = authContext;
        }

        public List<FileEntry> GetBreadCrumbs<T>(T folderId)
        {
            var folderDao = DaoFactory.GetFolderDao<T>();
            return GetBreadCrumbs(folderId, folderDao);
        }

        public List<FileEntry> GetBreadCrumbs<T>(T folderId, IFolderDao<T> folderDao)
        {
            if (folderId == null) return new List<FileEntry>();
            var breadCrumbs = FileSecurity.FilterRead(folderDao.GetParentFolders(folderId)).Cast<FileEntry>().ToList();

            var firstVisible = breadCrumbs.ElementAtOrDefault(0) as Folder<T>;

            var rootId = 0;
            if (firstVisible == null)
            {
                rootId = GlobalFolderHelper.FolderShare;
            }
            else
            {
                switch (firstVisible.FolderType)
                {
                    case FolderType.DEFAULT:
                        if (!firstVisible.ProviderEntry)
                        {
                            rootId = GlobalFolderHelper.FolderShare;
                        }
                        else
                        {
                            switch (firstVisible.RootFolderType)
                            {
                                case FolderType.USER:
                                    rootId = AuthContext.CurrentAccount.ID == firstVisible.RootFolderCreator
                                        ? GlobalFolderHelper.FolderMy
                                        : GlobalFolderHelper.FolderShare;
                                    break;
                                case FolderType.COMMON:
                                    rootId = GlobalFolderHelper.FolderCommon;
                                    break;
                            }
                        }
                        break;

                    case FolderType.BUNCH:
                        rootId = GlobalFolderHelper.FolderProjects;
                        break;
                }
            }

            var folderDaoInt = DaoFactory.GetFolderDao<int>();

            if (rootId != 0)
            {
                breadCrumbs.Insert(0, folderDaoInt.GetFolder(rootId));
            }

            return breadCrumbs;
        }
    }

    [Scope]
    public class EntryStatusManager
    {
        private IDaoFactory DaoFactory { get; }
        private AuthContext AuthContext { get; }
        private Global Global { get; }

        public EntryStatusManager(IDaoFactory daoFactory, AuthContext authContext, Global global)
        {
            DaoFactory = daoFactory;
            AuthContext = authContext;
            Global = global;
        }

        public void SetFileStatus<T>(File<T> file)
        {
            if (file == null || file.ID == null) return;

            SetFileStatus(new List<File<T>>(1) { file });
        }

        public void SetFileStatus(IEnumerable<FileEntry> files)
        {
            SetFileStatus(files.OfType<File<int>>().Where(r => r.ID != 0));
            SetFileStatus(files.OfType<File<string>>().Where(r => !string.IsNullOrEmpty(r.ID)));
        }

        public void SetFileStatus<T>(IEnumerable<File<T>> files)
        {
            var tagDao = DaoFactory.GetTagDao<T>();

            var tags = tagDao.GetTags(AuthContext.CurrentAccount.ID, new[] { TagType.Favorite, TagType.Template, TagType.Locked }, files);
            var tagsNew = tagDao.GetNewTags(AuthContext.CurrentAccount.ID, files);

            foreach (var file in files)
            {
                foreach (var t in tags)
                {
                    if (!t.Key.Equals(file.ID)) continue;

                    if (t.Value.Any(r => r.TagType == TagType.Favorite)) file.IsFavorite = true;
                    if (t.Value.Any(r => r.TagType == TagType.Template)) file.IsTemplate = true;

                    var lockedTag = t.Value.FirstOrDefault(r => r.TagType == TagType.Locked);
                    if (lockedTag != null)
                    {
                        var lockedBy = lockedTag.Owner;
                        file.Locked = lockedBy != Guid.Empty;
                        file.LockedBy = lockedBy != Guid.Empty && lockedBy != AuthContext.CurrentAccount.ID
                            ? Global.GetUserName(lockedBy)
                            : null;
                        continue;
                    }
                }

                if (tagsNew.Any(r => r.EntryId.Equals(file.ID)))
                {
                    file.IsNew = true;
                }
            }
        }
    }

    [Scope]
    public class EntryManager
    {
        private const string UPDATE_LIST = "filesUpdateList";

        private ICache Cache { get; set; }
        private FileTrackerHelper FileTracker { get; }
        private EntryStatusManager EntryStatusManager { get; }
        private IDaoFactory DaoFactory { get; }
        private FileSecurity FileSecurity { get; }
        private GlobalFolderHelper GlobalFolderHelper { get; }
        private PathProvider PathProvider { get; }
        private AuthContext AuthContext { get; }
        private FilesIntegration FilesIntegration { get; }
        private FileMarker FileMarker { get; }
        private FileUtility FileUtility { get; }
        private Global Global { get; }
        private GlobalStore GlobalStore { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private FilesSettingsHelper FilesSettingsHelper { get; }
        private UserManager UserManager { get; }
        private FileShareLink FileShareLink { get; }
        private DocumentServiceHelper DocumentServiceHelper { get; }
        private ThirdpartyConfiguration ThirdpartyConfiguration { get; }
        private DocumentServiceConnector DocumentServiceConnector { get; }
        private LockerManager LockerManager { get; }
        private BreadCrumbsManager BreadCrumbsManager { get; }
        private TenantManager TenantManager { get; }
        private SettingsManager SettingsManager { get; }
        private IServiceProvider ServiceProvider { get; }
        private ILog Logger { get; }

        public EntryManager(
            IDaoFactory daoFactory,
            FileSecurity fileSecurity,
            GlobalFolderHelper globalFolderHelper,
            PathProvider pathProvider,
            AuthContext authContext,
            FilesIntegration filesIntegration,
            FileMarker fileMarker,
            FileUtility fileUtility,
            Global global,
            GlobalStore globalStore,
            CoreBaseSettings coreBaseSettings,
            FilesSettingsHelper filesSettingsHelper,
            UserManager userManager,
            IOptionsMonitor<ILog> optionsMonitor,
            FileShareLink fileShareLink,
            DocumentServiceHelper documentServiceHelper,
            ThirdpartyConfiguration thirdpartyConfiguration,
            DocumentServiceConnector documentServiceConnector,
            LockerManager lockerManager,
            BreadCrumbsManager breadCrumbsManager,
            TenantManager tenantManager,
            SettingsManager settingsManager,
            IServiceProvider serviceProvider,
            ICache cache,
            FileTrackerHelper fileTracker,
            EntryStatusManager entryStatusManager)
        {
            DaoFactory = daoFactory;
            FileSecurity = fileSecurity;
            GlobalFolderHelper = globalFolderHelper;
            PathProvider = pathProvider;
            AuthContext = authContext;
            FilesIntegration = filesIntegration;
            FileMarker = fileMarker;
            FileUtility = fileUtility;
            Global = global;
            GlobalStore = globalStore;
            CoreBaseSettings = coreBaseSettings;
            FilesSettingsHelper = filesSettingsHelper;
            UserManager = userManager;
            FileShareLink = fileShareLink;
            DocumentServiceHelper = documentServiceHelper;
            ThirdpartyConfiguration = thirdpartyConfiguration;
            DocumentServiceConnector = documentServiceConnector;
            LockerManager = lockerManager;
            BreadCrumbsManager = breadCrumbsManager;
            TenantManager = tenantManager;
            SettingsManager = settingsManager;
            ServiceProvider = serviceProvider;
            Logger = optionsMonitor.CurrentValue;
            Cache = cache;
            FileTracker = fileTracker;
            EntryStatusManager = entryStatusManager;
        }

        public IEnumerable<FileEntry> GetEntries<T>(Folder<T> parent, int from, int count, FilterType filter, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent, bool withSubfolders, OrderBy orderBy, out int total)
        {
            total = 0;

            if (parent == null) throw new ArgumentNullException("parent", FilesCommonResource.ErrorMassage_FolderNotFound);
            if (parent.ProviderEntry && !FilesSettingsHelper.EnableThirdParty) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_ReadFolder);
            if (parent.RootFolderType == FolderType.Privacy && (!PrivacyRoomSettings.IsAvailable(TenantManager) || !PrivacyRoomSettings.GetEnabled(SettingsManager))) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_ReadFolder);

            var fileSecurity = FileSecurity;
            var entries = Enumerable.Empty<FileEntry>();

            searchInContent = searchInContent && filter != FilterType.ByExtension && !Equals(parent.ID, GlobalFolderHelper.FolderTrash);

            if (parent.FolderType == FolderType.Projects && parent.ID.Equals(GlobalFolderHelper.FolderProjects))
            {
                //TODO
                //var apiServer = new ASC.Api.ApiServer();
                //var apiUrl = string.Format("{0}project/maxlastmodified.json", SetupInfo.WebApiBaseUrl);

                string responseBody = null;// apiServer.GetApiResponse(apiUrl, "GET");
                if (responseBody != null)
                {
                    var responseApi = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(responseBody)));

                    var projectLastModified = responseApi["response"].Value<string>();
                    var projectListCacheKey = string.Format("documents/projectFolders/{0}", AuthContext.CurrentAccount.ID);
                    Dictionary<int, KeyValuePair<int, string>> folderIDProjectTitle = null;

                    if (folderIDProjectTitle == null)
                    {
                        //apiUrl = string.Format("{0}project/filter.json?sortBy=title&sortOrder=ascending&status=open&fields=id,title,security,projectFolder", SetupInfo.WebApiBaseUrl);

                        responseApi = JObject.Parse(""); //Encoding.UTF8.GetString(Convert.FromBase64String(apiServer.GetApiResponse(apiUrl, "GET"))));

                        var responseData = responseApi["response"];

                        if (!(responseData is JArray)) return entries.ToList();

                        folderIDProjectTitle = new Dictionary<int, KeyValuePair<int, string>>();
                        foreach (JObject projectInfo in responseData.Children())
                        {
                            var projectID = projectInfo["id"].Value<int>();
                            var projectTitle = Global.ReplaceInvalidCharsAndTruncate(projectInfo["title"].Value<string>());

                            if (projectInfo.TryGetValue("security", out var projectSecurityJToken))
                            {
                                var projectSecurity = projectInfo["security"].Value<JObject>();
                                if (projectSecurity.TryGetValue("canReadFiles", out var projectCanFileReadJToken))
                                {
                                    if (!projectSecurity["canReadFiles"].Value<bool>())
                                    {
                                        continue;
                                    }
                                }
                            }

                            int projectFolderID;
                            if (projectInfo.TryGetValue("projectFolder", out var projectFolderIDjToken))
                                projectFolderID = projectFolderIDjToken.Value<int>();
                            else
                                projectFolderID = FilesIntegration.RegisterBunch<int>("projects", "project", projectID.ToString());

                            if (!folderIDProjectTitle.ContainsKey(projectFolderID))
                                folderIDProjectTitle.Add(projectFolderID, new KeyValuePair<int, string>(projectID, projectTitle));

                            Cache.Remove("documents/folders/" + projectFolderID);
                            Cache.Insert("documents/folders/" + projectFolderID, projectTitle, TimeSpan.FromMinutes(30));
                        }
                    }

                    var rootKeys = folderIDProjectTitle.Keys.ToArray();
                    if (filter == FilterType.None || filter == FilterType.FoldersOnly)
                    {
                        var folders = DaoFactory.GetFolderDao<int>().GetFolders(rootKeys, filter, subjectGroup, subjectId, searchText, withSubfolders, false);

                        var emptyFilter = string.IsNullOrEmpty(searchText) && filter == FilterType.None && subjectId == Guid.Empty;
                        if (!emptyFilter)
                        {
                            var projectFolderIds =
                                folderIDProjectTitle
                                    .Where(projectFolder => string.IsNullOrEmpty(searchText)
                                                            || (projectFolder.Value.Value ?? "").ToLower().Trim().Contains(searchText.ToLower().Trim()))
                                    .Select(projectFolder => projectFolder.Key);

                            folders.RemoveAll(folder => rootKeys.Contains(folder.ID));

                            var projectFolders = DaoFactory.GetFolderDao<int>().GetFolders(projectFolderIds.ToList(), filter, subjectGroup, subjectId, null, false, false);
                            folders.AddRange(projectFolders);
                        }

                        folders.ForEach(x =>
                            {
                                x.Title = folderIDProjectTitle.ContainsKey(x.ID) ? folderIDProjectTitle[x.ID].Value : x.Title;
                                x.FolderUrl = folderIDProjectTitle.ContainsKey(x.ID) ? PathProvider.GetFolderUrl(x, folderIDProjectTitle[x.ID].Key) : string.Empty;
                            });

                        if (withSubfolders)
                        {
                            entries = entries.Concat(fileSecurity.FilterRead(folders));
                        }
                        else
                        {
                            entries = entries.Concat(folders);
                        }
                    }

                    if (filter != FilterType.FoldersOnly && withSubfolders)
                    {
                        var files = DaoFactory.GetFileDao<int>().GetFiles(rootKeys, filter, subjectGroup, subjectId, searchText, searchInContent);
                        entries = entries.Concat(fileSecurity.FilterRead(files));
                    }
                }

                CalculateTotal();
            }
            else if (parent.FolderType == FolderType.SHARE)
            {
                //share
                var shared = fileSecurity.GetSharesForMe(filter, subjectGroup, subjectId, searchText, searchInContent, withSubfolders);

                entries = entries.Concat(shared);

                CalculateTotal();
            }
            else if (parent.FolderType == FolderType.Recent)
            {
                var files = GetRecent(filter, subjectGroup, subjectId, searchText, searchInContent);
                entries = entries.Concat(files);

                CalculateTotal();
            }
            else if (parent.FolderType == FolderType.Favorites)
            {
                var fileDao = DaoFactory.GetFileDao<T>();
                var folderDao = DaoFactory.GetFolderDao<T>();

                var (files, folders) = GetFavorites(filter, subjectGroup, subjectId, searchText, searchInContent);

                entries = entries.Concat(folders);
                entries = entries.Concat(files);

                CalculateTotal();
            }
            else if (parent.FolderType == FolderType.Templates)
            {
                var folderDao = DaoFactory.GetFolderDao<T>();
                var fileDao = DaoFactory.GetFileDao<T>();
                var files = GetTemplates(folderDao, fileDao, filter, subjectGroup, subjectId, searchText, searchInContent);
                entries = entries.Concat(files);

                CalculateTotal();
            }
            else if (parent.FolderType == FolderType.Privacy)
            {
                var folderDao = DaoFactory.GetFolderDao<T>();
                var fileDao = DaoFactory.GetFileDao<T>();
                var folders = folderDao.GetFolders(parent.ID, orderBy, filter, subjectGroup, subjectId, searchText, withSubfolders);
                entries = entries.Concat(fileSecurity.FilterRead(folders));

                var files = fileDao.GetFiles(parent.ID, orderBy, filter, subjectGroup, subjectId, searchText, searchInContent, withSubfolders);
                entries = entries.Concat(fileSecurity.FilterRead(files));

                //share
                var shared = fileSecurity.GetPrivacyForMe(filter, subjectGroup, subjectId, searchText, searchInContent, withSubfolders);

                entries = entries.Concat(shared);

                CalculateTotal();
            }
            else
            {
                if (parent.FolderType == FolderType.TRASH)
                    withSubfolders = false;

                var folders = DaoFactory.GetFolderDao<T>().GetFolders(parent.ID, orderBy, filter, subjectGroup, subjectId, searchText, withSubfolders);
                entries = entries.Concat(fileSecurity.FilterRead(folders));

                var files = DaoFactory.GetFileDao<T>().GetFiles(parent.ID, orderBy, filter, subjectGroup, subjectId, searchText, searchInContent, withSubfolders);
                entries = entries.Concat(fileSecurity.FilterRead(files));

                if (filter == FilterType.None || filter == FilterType.FoldersOnly)
                {
                    var folderList = GetThirpartyFolders(parent, searchText);

                    var thirdPartyFolder = FilterEntries(folderList, filter, subjectGroup, subjectId, searchText, searchInContent);
                    entries = entries.Concat(thirdPartyFolder);
                }
            }

            if (orderBy.SortedBy != SortedByType.New)
            {
                if (parent.FolderType != FolderType.Recent)
                {
                    entries = SortEntries<T>(entries, orderBy);
                }

                total = entries.Count();
                if (0 < from) entries = entries.Skip(from);
                if (0 < count) entries = entries.Take(count);
            }

            entries = FileMarker.SetTagsNew(parent, entries);

            //sorting after marking
            if (orderBy.SortedBy == SortedByType.New)
            {
                entries = SortEntries<T>(entries, orderBy);

                total = entries.Count();
                if (0 < from) entries = entries.Skip(from);
                if (0 < count) entries = entries.Take(count);
            }

            EntryStatusManager.SetFileStatus(entries.Where(r => r != null && r.FileEntryType == FileEntryType.File).ToList());
            return entries;

            void CalculateTotal()
            {
                foreach (var f in entries)
                {
                    if (f is IFolder fold)
                    {
                        parent.TotalFiles += fold.TotalFiles;
                        parent.TotalSubFolders += fold.TotalSubFolders + 1;
                    }
                    else
                    {
                        parent.TotalFiles += 1;
                    }
                }
            }
        }

        public IEnumerable<File<T>> GetTemplates<T>(IFolderDao<T> folderDao, IFileDao<T> fileDao, FilterType filter, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent)
        {
            var tagDao = DaoFactory.GetTagDao<T>();
            var tags = tagDao.GetTags(AuthContext.CurrentAccount.ID, TagType.Template);

            var fileIds = tags.Where(tag => tag.EntryType == FileEntryType.File).Select(tag => (T)Convert.ChangeType(tag.EntryId, typeof(T))).ToArray();

            var files = fileDao.GetFilesFiltered(fileIds, filter, subjectGroup, subjectId, searchText, searchInContent);
            files = files.Where(file => file.RootFolderType != FolderType.TRASH).ToList();

            files = FileSecurity.FilterRead(files).ToList();

            CheckFolderId(folderDao, files);

            return files;
        }

        public IEnumerable<Folder<string>> GetThirpartyFolders<T>(Folder<T> parent, string searchText = null)
        {
            var folderList = new List<Folder<string>>();

            if ((parent.ID.Equals(GlobalFolderHelper.FolderMy) || parent.ID.Equals(GlobalFolderHelper.FolderCommon))
                && ThirdpartyConfiguration.SupportInclusion(DaoFactory)
                && (FilesSettingsHelper.EnableThirdParty
                    || CoreBaseSettings.Personal))
            {
                var providerDao = DaoFactory.ProviderDao;
                if (providerDao == null) return folderList;

                var fileSecurity = FileSecurity;

                var providers = providerDao.GetProvidersInfo(parent.RootFolderType, searchText);
                folderList = providers
                    .Select(providerInfo => GetFakeThirdpartyFolder<T>(providerInfo, parent.ID.ToString()))
                    .Where(r => fileSecurity.CanRead(r)).ToList();

                if (folderList.Any())
                {
                    var securityDao = DaoFactory.GetSecurityDao<string>();
                    securityDao.GetPureShareRecords(folderList)
                    //.Where(x => x.Owner == SecurityContext.CurrentAccount.ID)
                    .Select(x => x.EntryId).Distinct().ToList()
                    .ForEach(id =>
                    {
                        folderList.First(y => y.ID.Equals(id)).Shared = true;
                    });
                }
            }

            return folderList;
        }

        public IEnumerable<FileEntry> GetRecent(FilterType filter, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent)
        {
            var tagDao = DaoFactory.GetTagDao<int>();
            var tags = tagDao.GetTags(AuthContext.CurrentAccount.ID, TagType.Recent).ToList();

            var fileIds = tags.Where(tag => tag.EntryType == FileEntryType.File).ToList();

            List<FileEntry> files = GetRecentByIds(fileIds.Where(r => r.EntryId is int).Select(r => (int)r.EntryId), filter, subjectGroup, subjectId, searchText, searchInContent).ToList();
            files.AddRange(GetRecentByIds(fileIds.Where(r => r.EntryId is string).Select(r => (string)r.EntryId), filter, subjectGroup, subjectId, searchText, searchInContent));

            var listFileIds = fileIds.Select(tag => tag.EntryId).ToList();

            files = files.OrderBy(file =>
            {
                var fileId = "";
                if (file is File<int> fileInt)
                {
                    fileId = fileInt.ID.ToString();
                }
                else if (file is File<string> fileString)
                {
                    fileId = fileString.ID;
                }

                return listFileIds.IndexOf(fileId);
            }).ToList();

            return files;

            IEnumerable<FileEntry> GetRecentByIds<T>(IEnumerable<T> fileIds, FilterType filter, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent)
            {
                var folderDao = DaoFactory.GetFolderDao<T>();
                var fileDao = DaoFactory.GetFileDao<T>();
                var files = fileDao.GetFilesFiltered(fileIds, filter, subjectGroup, subjectId, searchText, searchInContent, true);
                files = files.Where(file => file.RootFolderType != FolderType.TRASH).ToList();

                files = FileSecurity.FilterRead(files).ToList();

                CheckFolderId(folderDao, files);

                return files;
            }
        }

        public (IEnumerable<FileEntry>, IEnumerable<FileEntry>) GetFavorites(FilterType filter, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent)
        {
            var fileSecurity = FileSecurity;
            var tagDao = DaoFactory.GetTagDao<int>();
            var tags = tagDao.GetTags(AuthContext.CurrentAccount.ID, TagType.Favorite);

            var fileIds = tags.Where(tag => tag.EntryType == FileEntryType.File).ToList();
            var folderIds = tags.Where(tag => tag.EntryType == FileEntryType.Folder).ToList();

            var (filesInt, foldersInt) = GetFavoritesById(fileIds.Where(r => r.EntryId is int).Select(r => (int)r.EntryId), folderIds.Where(r => r.EntryId is int).Select(r => (int)r.EntryId), filter, subjectGroup, subjectId, searchText, searchInContent);
            var (filesString, foldersString) = GetFavoritesById(fileIds.Where(r => r.EntryId is string).Select(r => (string)r.EntryId), folderIds.Where(r => r.EntryId is string).Select(r => (string)r.EntryId), filter, subjectGroup, subjectId, searchText, searchInContent);

            var files = new List<FileEntry>();
            files.AddRange(filesInt);
            files.AddRange(filesString);

            var folders = new List<FileEntry>();
            folders.AddRange(foldersInt);
            folders.AddRange(foldersString);

            return (files, folders);

            (IEnumerable<FileEntry>, IEnumerable<FileEntry>) GetFavoritesById<T>(IEnumerable<T> fileIds, IEnumerable<T> folderIds, FilterType filter, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent)
            {
                var folderDao = DaoFactory.GetFolderDao<T>();
                var fileDao = DaoFactory.GetFileDao<T>();
                var folders = new List<Folder<T>>();
                var files = new List<File<T>>();
                var fileSecurity = FileSecurity;

                if (filter == FilterType.None || filter == FilterType.FoldersOnly)
                {
                    folders = folderDao.GetFolders(folderIds, filter, subjectGroup, subjectId, searchText, false, false);
                    folders = folders.Where(folder => folder.RootFolderType != FolderType.TRASH).ToList();

                    folders = fileSecurity.FilterRead(folders).ToList();

                    CheckFolderId(folderDao, folders);
                }

                if (filter != FilterType.FoldersOnly)
                {
                    files = fileDao.GetFilesFiltered(fileIds, filter, subjectGroup, subjectId, searchText, searchInContent, true);
                    files = files.Where(file => file.RootFolderType != FolderType.TRASH).ToList();

                    files = fileSecurity.FilterRead(files).ToList();

                    CheckFolderId(folderDao, folders);
                }

                return (files, folders);
            }
        }

        public IEnumerable<FileEntry<T>> FilterEntries<T>(IEnumerable<FileEntry<T>> entries, FilterType filter, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent)
        {
            if (entries == null || !entries.Any()) return entries;

            if (subjectId != Guid.Empty)
            {
                entries = entries.Where(f =>
                                        subjectGroup
                                            ? UserManager.GetUsersByGroup(subjectId).Any(s => s.ID == f.CreateBy)
                                            : f.CreateBy == subjectId
                    )
                                 .ToList();
            }

            Func<FileEntry<T>, bool> where = null;

            switch (filter)
            {
                case FilterType.SpreadsheetsOnly:
                case FilterType.PresentationsOnly:
                case FilterType.ImagesOnly:
                case FilterType.DocumentsOnly:
                case FilterType.ArchiveOnly:
                case FilterType.FilesOnly:
                case FilterType.MediaOnly:
                    where = f => f.FileEntryType == FileEntryType.File && (((File<T>)f).FilterType == filter || filter == FilterType.FilesOnly);
                    break;
                case FilterType.FoldersOnly:
                    where = f => f.FileEntryType == FileEntryType.Folder;
                    break;
                case FilterType.ByExtension:
                    var filterExt = (searchText ?? string.Empty).ToLower().Trim();
                    where = f => !string.IsNullOrEmpty(filterExt) && f.FileEntryType == FileEntryType.File && FileUtility.GetFileExtension(f.Title).Contains(filterExt);
                    break;
            }

            if (where != null)
            {
                entries = entries.Where(where).ToList();
            }

            if ((!searchInContent || filter == FilterType.ByExtension) && !string.IsNullOrEmpty(searchText = (searchText ?? string.Empty).ToLower().Trim()))
            {
                entries = entries.Where(f => f.Title.ToLower().Contains(searchText)).ToList();
            }
            return entries;
        }

        public IEnumerable<FileEntry> SortEntries<T>(IEnumerable<FileEntry> entries, OrderBy orderBy)
        {
            if (entries == null || !entries.Any()) return entries;

            if (orderBy == null)
            {
                orderBy = FilesSettingsHelper.DefaultOrder;
            }

            var c = orderBy.IsAsc ? 1 : -1;
            Comparison<FileEntry> sorter = orderBy.SortedBy switch
            {
                SortedByType.Type => (x, y) =>
                {
                    var cmp = 0;
                    if (x.FileEntryType == FileEntryType.File && y.FileEntryType == FileEntryType.File)
                        cmp = c * (FileUtility.GetFileExtension((x.Title)).CompareTo(FileUtility.GetFileExtension(y.Title)));
                    return cmp == 0 ? x.Title.EnumerableComparer(y.Title) : cmp;
                }
                ,
                SortedByType.Author => (x, y) =>
                {
                    var cmp = c * string.Compare(x.CreateByString, y.CreateByString);
                    return cmp == 0 ? x.Title.EnumerableComparer(y.Title) : cmp;
                }
                ,
                SortedByType.Size => (x, y) =>
                {
                    var cmp = 0;
                    if (x.FileEntryType == FileEntryType.File && y.FileEntryType == FileEntryType.File)
                        cmp = c * ((File<T>)x).ContentLength.CompareTo(((File<T>)y).ContentLength);
                    return cmp == 0 ? x.Title.EnumerableComparer(y.Title) : cmp;
                }
                ,
                SortedByType.AZ => (x, y) => c * x.Title.EnumerableComparer(y.Title),
                SortedByType.DateAndTime => (x, y) =>
                {
                    var cmp = c * DateTime.Compare(x.ModifiedOn, y.ModifiedOn);
                    return cmp == 0 ? x.Title.EnumerableComparer(y.Title) : cmp;
                }
                ,
                SortedByType.DateAndTimeCreation => (x, y) =>
                {
                    var cmp = c * DateTime.Compare(x.CreateOn, y.CreateOn);
                    return cmp == 0 ? x.Title.EnumerableComparer(y.Title) : cmp;
                }
                ,
                SortedByType.New => (x, y) =>
                {
                    var isNewSortResult = x.IsNew.CompareTo(y.IsNew);
                    return c * (isNewSortResult == 0 ? DateTime.Compare(x.ModifiedOn, y.ModifiedOn) : isNewSortResult);
                }
                ,
                _ => (x, y) => c * x.Title.EnumerableComparer(y.Title),
            };
            if (orderBy.SortedBy != SortedByType.New)
            {
                // folders on top
                var folders = entries.Where(r => r.FileEntryType == FileEntryType.Folder).ToList();
                var files = entries.Where(r => r.FileEntryType == FileEntryType.File).ToList();
                folders.Sort(sorter);
                files.Sort(sorter);

                return folders.Concat(files);
            }

            var result = entries.ToList();

            result.Sort(sorter);

            return result;
        }

        public Folder<string> GetFakeThirdpartyFolder<T>(IProviderInfo providerInfo, string parentFolderId = null)
        {
            //Fake folder. Don't send request to third party
            var folder = ServiceProvider.GetService<Folder<string>>();

            folder.FolderID = parentFolderId;

            folder.ID = providerInfo.RootFolderId;
            folder.CreateBy = providerInfo.Owner;
            folder.CreateOn = providerInfo.CreateOn;
            folder.FolderType = FolderType.DEFAULT;
            folder.ModifiedBy = providerInfo.Owner;
            folder.ModifiedOn = providerInfo.CreateOn;
            folder.ProviderId = providerInfo.ID;
            folder.ProviderKey = providerInfo.ProviderKey;
            folder.RootFolderCreator = providerInfo.Owner;
            folder.RootFolderId = providerInfo.RootFolderId;
            folder.RootFolderType = providerInfo.RootFolderType;
            folder.Shareable = false;
            folder.Title = providerInfo.CustomerTitle;
            folder.TotalFiles = 0;
            folder.TotalSubFolders = 0;

            return folder;
        }


        public List<FileEntry> GetBreadCrumbs<T>(T folderId)
        {
            return BreadCrumbsManager.GetBreadCrumbs(folderId);
        }

        public List<FileEntry> GetBreadCrumbs<T>(T folderId, IFolderDao<T> folderDao)
        {
            return BreadCrumbsManager.GetBreadCrumbs(folderId, folderDao);
        }

        public void CheckFolderId<T>(IFolderDao<T> folderDao, IEnumerable<FileEntry<T>> entries)
        {
            foreach (var entry in entries)
            {
                if (entry.RootFolderType == FolderType.USER
                    && entry.RootFolderCreator != AuthContext.CurrentAccount.ID)
                {
                    var folderId = entry.FolderID;
                    var folder = folderDao.GetFolder(folderId);
                    if (!FileSecurity.CanRead(folder))
                    {
                        entry.FolderIdDisplay = GlobalFolderHelper.GetFolderShare<T>();
                    }
                }
            }
        }

        public bool FileLockedForMe<T>(T fileId, Guid userId = default)
        {
            return LockerManager.FileLockedForMe(fileId, userId);
        }

        public Guid FileLockedBy<T>(T fileId, ITagDao<T> tagDao)
        {
            return LockerManager.FileLockedBy(fileId, tagDao);
        }


        public File<T> SaveEditing<T>(T fileId, string fileExtension, string downloadUri, Stream stream, string doc, string comment = null, bool checkRight = true, bool encrypted = false, ForcesaveType? forcesave = null)
        {
            var newExtension = string.IsNullOrEmpty(fileExtension)
                              ? FileUtility.GetFileExtension(downloadUri)
                              : fileExtension;

            var app = ThirdPartySelector.GetAppByFileId(fileId.ToString());
            if (app != null)
            {
                app.SaveFile(fileId.ToString(), newExtension, downloadUri, stream);
                return null;
            }

            var fileDao = DaoFactory.GetFileDao<T>();
            var editLink = FileShareLink.Check(doc, false, fileDao, out var file);
            if (file == null)
            {
                file = fileDao.GetFile(fileId);
            }

            if (file == null) throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
            var fileSecurity = FileSecurity;
            if (checkRight && !editLink && (!fileSecurity.CanEdit(file) || UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager))) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_EditFile);
            if (checkRight && FileLockedForMe(file.ID)) throw new Exception(FilesCommonResource.ErrorMassage_LockedFile);
            if (checkRight && (!forcesave.HasValue || forcesave.Value == ForcesaveType.None) && FileTracker.IsEditing(file.ID)) throw new Exception(FilesCommonResource.ErrorMassage_SecurityException_UpdateEditingFile);
            if (file.RootFolderType == FolderType.TRASH) throw new Exception(FilesCommonResource.ErrorMassage_ViewTrashItem);

            var currentExt = file.ConvertedExtension;
            if (string.IsNullOrEmpty(newExtension)) newExtension = FileUtility.GetInternalExtension(file.Title);

            var replaceVersion = false;
            if (file.Forcesave != ForcesaveType.None)
            {
                if (file.Forcesave == ForcesaveType.User && FilesSettingsHelper.StoreForcesave || encrypted)
                {
                    file.Version++;
                }
                else
                {
                    replaceVersion = true;
                }
            }
            else
            {
                if (file.Version != 1)
                {
                    file.VersionGroup++;
                }
                else
                {
                    var storeTemplate = GlobalStore.GetStoreTemplate();

                    var path = FileConstant.NewDocPath + Thread.CurrentThread.CurrentCulture + "/";
                    if (!storeTemplate.IsDirectory(path))
                    {
                        path = FileConstant.NewDocPath + "en-US/";
                    }
                    path += "new" + FileUtility.GetInternalExtension(file.Title);

                    //todo: think about the criteria for saving after creation
                    if (!storeTemplate.IsFile(path) || file.ContentLength != storeTemplate.GetFileSize("", path))
                    {
                        file.VersionGroup++;
                    }
                }
                file.Version++;
            }
            file.Forcesave = forcesave ?? ForcesaveType.None;

            if (string.IsNullOrEmpty(comment))
                comment = FilesCommonResource.CommentEdit;

            file.Encrypted = encrypted;

            file.ConvertedType = FileUtility.GetFileExtension(file.Title) != newExtension ? newExtension : null;
            file.ThumbnailStatus = encrypted ? Thumbnail.NotRequired : Thumbnail.Waiting;

            if (file.ProviderEntry && !newExtension.Equals(currentExt))
            {
                if (FileUtility.ExtsConvertible.Keys.Contains(newExtension)
                    && FileUtility.ExtsConvertible[newExtension].Contains(currentExt))
                {
                    if (stream != null)
                    {
                        downloadUri = PathProvider.GetTempUrl(stream, newExtension);
                        downloadUri = DocumentServiceConnector.ReplaceCommunityAdress(downloadUri);
                    }

                    var key = DocumentServiceConnector.GenerateRevisionId(downloadUri);
                    DocumentServiceConnector.GetConvertedUri(downloadUri, newExtension, currentExt, key, null, null, null, false, out downloadUri);

                    stream = null;
                }
                else
                {
                    file.ID = default;
                    file.Title = FileUtility.ReplaceFileExtension(file.Title, newExtension);
                }

                file.ConvertedType = null;
            }

            using (var tmpStream = new MemoryStream())
            {
                if (stream != null)
                {
                    stream.CopyTo(tmpStream);
                }
                else
                {
                    // hack. http://ubuntuforums.org/showthread.php?t=1841740
                    if (WorkContext.IsMono)
                    {
                        ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                    }

                    var req = (HttpWebRequest)WebRequest.Create(downloadUri);
                    using var editedFileStream = new ResponseStream(req.GetResponse());
                    editedFileStream.CopyTo(tmpStream);
                }
                tmpStream.Position = 0;

                file.ContentLength = tmpStream.Length;
                file.Comment = string.IsNullOrEmpty(comment) ? null : comment;
                if (replaceVersion)
                {
                    file = fileDao.ReplaceFileVersion(file, tmpStream);
                }
                else
                {
                    file = fileDao.SaveFile(file, tmpStream);
                }
            }

            FileMarker.MarkAsNew(file);
            FileMarker.RemoveMarkAsNew(file);
            return file;
        }

        public void TrackEditing<T>(T fileId, Guid tabId, Guid userId, string doc, bool editingAlone = false)
        {
            bool checkRight;
            if (FileTracker.GetEditingBy(fileId).Contains(userId))
            {
                checkRight = FileTracker.ProlongEditing(fileId, tabId, userId, editingAlone);
                if (!checkRight) return;
            }

            bool editLink;
            var fileDao = DaoFactory.GetFileDao<T>();
            editLink = FileShareLink.Check(doc, false, fileDao, out var file);
            if (file == null)
                file = fileDao.GetFile(fileId);

            if (file == null) throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
            var fileSecurity = FileSecurity;
            if (!editLink
                && (!fileSecurity.CanEdit(file, userId)
                    && !fileSecurity.CanCustomFilterEdit(file, userId)
                    && !fileSecurity.CanReview(file, userId)
                    && !fileSecurity.CanFillForms(file, userId)
                    && !fileSecurity.CanComment(file, userId)
                    || UserManager.GetUsers(userId).IsVisitor(UserManager)))
            {
                throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_EditFile);
            }
            if (FileLockedForMe(file.ID, userId)) throw new Exception(FilesCommonResource.ErrorMassage_LockedFile);
            if (file.RootFolderType == FolderType.TRASH) throw new Exception(FilesCommonResource.ErrorMassage_ViewTrashItem);

            checkRight = FileTracker.ProlongEditing(fileId, tabId, userId, editingAlone);
            if (checkRight)
            {
                FileTracker.ChangeRight(fileId, userId, false);
            }
        }

        public File<T> UpdateToVersionFile<T>(T fileId, int version, string doc = null, bool checkRight = true)
        {
            var fileDao = DaoFactory.GetFileDao<T>();
            if (version < 1) throw new ArgumentNullException("version");

            var editLink = FileShareLink.Check(doc, false, fileDao, out var fromFile);

            if (fromFile == null)
                fromFile = fileDao.GetFile(fileId);

            if (fromFile == null) throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);

            if (fromFile.Version != version)
                fromFile = fileDao.GetFile(fromFile.ID, Math.Min(fromFile.Version, version));

            if (fromFile == null) throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
            if (checkRight && !editLink && (!FileSecurity.CanEdit(fromFile) || UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager))) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_EditFile);
            if (FileLockedForMe(fromFile.ID)) throw new Exception(FilesCommonResource.ErrorMassage_LockedFile);
            if (checkRight && FileTracker.IsEditing(fromFile.ID)) throw new Exception(FilesCommonResource.ErrorMassage_SecurityException_UpdateEditingFile);
            if (fromFile.RootFolderType == FolderType.TRASH) throw new Exception(FilesCommonResource.ErrorMassage_ViewTrashItem);
            if (fromFile.ProviderEntry) throw new Exception(FilesCommonResource.ErrorMassage_BadRequest);
            if (fromFile.Encrypted) throw new Exception(FilesCommonResource.ErrorMassage_NotSupportedFormat);

            var exists = Cache.Get<string>(UPDATE_LIST + fileId.ToString()) != null;
            if (exists)
            {
                throw new Exception(FilesCommonResource.ErrorMassage_UpdateEditingFile);
            }
            else
            {
                Cache.Insert(UPDATE_LIST + fileId.ToString(), fileId.ToString(), TimeSpan.FromMinutes(2));
            }

            try
            {
                var currFile = fileDao.GetFile(fileId);
                var newFile = ServiceProvider.GetService<File<T>>();

                newFile.ID = fromFile.ID;
                newFile.Version = currFile.Version + 1;
                newFile.VersionGroup = currFile.VersionGroup;
                newFile.Title = FileUtility.ReplaceFileExtension(currFile.Title, FileUtility.GetFileExtension(fromFile.Title));
                newFile.FileStatus = currFile.FileStatus;
                newFile.FolderID = currFile.FolderID;
                newFile.CreateBy = currFile.CreateBy;
                newFile.CreateOn = currFile.CreateOn;
                newFile.ModifiedBy = fromFile.ModifiedBy;
                newFile.ModifiedOn = fromFile.ModifiedOn;
                newFile.ConvertedType = fromFile.ConvertedType;
                newFile.Comment = string.Format(FilesCommonResource.CommentRevert, fromFile.ModifiedOnString);
                newFile.Encrypted = fromFile.Encrypted;

                using (var stream = fileDao.GetFileStream(fromFile))
                {
                    newFile.ContentLength = stream.CanSeek ? stream.Length : fromFile.ContentLength;
                    newFile = fileDao.SaveFile(newFile, stream);
                }

                if (fromFile.ThumbnailStatus == Thumbnail.Created)
                {
                    using (var thumb = fileDao.GetThumbnail(fromFile))
                    {
                        fileDao.SaveThumbnail(newFile, thumb);
                    }
                    newFile.ThumbnailStatus = Thumbnail.Created;
                }

                FileMarker.MarkAsNew(newFile);

                EntryStatusManager.SetFileStatus(newFile);

                newFile.Access = fromFile.Access;

                if (newFile.IsTemplate
                    && !FileUtility.ExtsWebTemplate.Contains(FileUtility.GetFileExtension(newFile.Title), StringComparer.CurrentCultureIgnoreCase))
                {
                    var tagTemplate = Tag.Template(AuthContext.CurrentAccount.ID, newFile);
                    var tagDao = DaoFactory.GetTagDao<T>();
                    tagDao.RemoveTags(tagTemplate);

                    newFile.IsTemplate = false;
                }

                return newFile;
            }
            catch (Exception e)
            {
                Logger.Error(string.Format("Error on update {0} to version {1}", fileId, version), e);
                throw new Exception(e.Message, e);
            }
            finally
            {
                Cache.Remove(UPDATE_LIST + fromFile.ID);
            }
        }

        public File<T> CompleteVersionFile<T>(T fileId, int version, bool continueVersion, bool checkRight = true)
        {
            var fileDao = DaoFactory.GetFileDao<T>();
            var fileVersion = version > 0
? fileDao.GetFile(fileId, version)
: fileDao.GetFile(fileId);
            if (fileVersion == null) throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
            if (checkRight && (!FileSecurity.CanEdit(fileVersion) || UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager))) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_EditFile);
            if (FileLockedForMe(fileVersion.ID)) throw new Exception(FilesCommonResource.ErrorMassage_LockedFile);
            if (fileVersion.RootFolderType == FolderType.TRASH) throw new Exception(FilesCommonResource.ErrorMassage_ViewTrashItem);
            if (fileVersion.ProviderEntry) throw new Exception(FilesCommonResource.ErrorMassage_BadRequest);

            var lastVersionFile = fileDao.GetFile(fileVersion.ID);

            if (continueVersion)
            {
                if (lastVersionFile.VersionGroup > 1)
                {
                    fileDao.ContinueVersion(fileVersion.ID, fileVersion.Version);
                    lastVersionFile.VersionGroup--;
                }
            }
            else
            {
                if (!FileTracker.IsEditing(lastVersionFile.ID))
                {
                    if (fileVersion.Version == lastVersionFile.Version)
                    {
                        lastVersionFile = UpdateToVersionFile(fileVersion.ID, fileVersion.Version, null, checkRight);
                    }

                    fileDao.CompleteVersion(fileVersion.ID, fileVersion.Version);
                    lastVersionFile.VersionGroup++;
                }
            }

            EntryStatusManager.SetFileStatus(lastVersionFile);

            return lastVersionFile;
        }

        public bool FileRename<T>(T fileId, string title, out File<T> file)
        {
            var fileDao = DaoFactory.GetFileDao<T>();
            file = fileDao.GetFile(fileId);
            if (file == null) throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
            if (!FileSecurity.CanEdit(file)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_RenameFile);
            if (!FileSecurity.CanDelete(file) && UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_RenameFile);
            if (FileLockedForMe(file.ID)) throw new Exception(FilesCommonResource.ErrorMassage_LockedFile);
            if (file.ProviderEntry && FileTracker.IsEditing(file.ID)) throw new Exception(FilesCommonResource.ErrorMassage_UpdateEditingFile);
            if (file.RootFolderType == FolderType.TRASH) throw new Exception(FilesCommonResource.ErrorMassage_ViewTrashItem);

            title = Global.ReplaceInvalidCharsAndTruncate(title);

            var ext = FileUtility.GetFileExtension(file.Title);
            if (string.Compare(ext, FileUtility.GetFileExtension(title), true) != 0)
            {
                title += ext;
            }

            var fileAccess = file.Access;

            var renamed = false;
            if (string.Compare(file.Title, title, false) != 0)
            {
                var newFileID = fileDao.FileRename(file, title);

                file = fileDao.GetFile(newFileID);
                file.Access = fileAccess;

                DocumentServiceHelper.RenameFile(file, fileDao);

                renamed = true;
            }

            EntryStatusManager.SetFileStatus(file);

            return renamed;
        }

        public void MarkAsRecent<T>(File<T> file)
        {
            if (file.Encrypted || file.ProviderEntry) throw new NotSupportedException();

            var tagDao = DaoFactory.GetTagDao<T>();
            var userID = AuthContext.CurrentAccount.ID;

            var tag = Tag.Recent(userID, file);
            tagDao.SaveTags(tag);
        }


        //Long operation
        public void DeleteSubitems<T>(T parentId, IFolderDao<T> folderDao, IFileDao<T> fileDao)
        {
            var folders = folderDao.GetFolders(parentId);
            foreach (var folder in folders)
            {
                DeleteSubitems(folder.ID, folderDao, fileDao);

                Logger.InfoFormat("Delete folder {0} in {1}", folder.ID, parentId);
                folderDao.DeleteFolder(folder.ID);
            }

            var files = fileDao.GetFiles(parentId, null, FilterType.None, false, Guid.Empty, string.Empty, true);
            foreach (var file in files)
            {
                Logger.InfoFormat("Delete file {0} in {1}", file.ID, parentId);
                fileDao.DeleteFile(file.ID);
            }
        }

        public void MoveSharedItems<T>(T parentId, T toId, IFolderDao<T> folderDao, IFileDao<T> fileDao)
        {
            var fileSecurity = FileSecurity;

            var folders = folderDao.GetFolders(parentId);
            foreach (var folder in folders)
            {
                var shared = folder.Shared
                             && fileSecurity.GetShares(folder).Any(record => record.Share != FileShare.Restrict);
                if (shared)
                {
                    Logger.InfoFormat("Move shared folder {0} from {1} to {2}", folder.ID, parentId, toId);
                    folderDao.MoveFolder(folder.ID, toId, null);
                }
                else
                {
                    MoveSharedItems(folder.ID, toId, folderDao, fileDao);
                }
            }

            var files = fileDao.GetFiles(parentId, null, FilterType.None, false, Guid.Empty, string.Empty, true)
                .Where(file => file.Shared &&
                fileSecurity.GetShares(file).Any(record => record.Subject != FileConstant.ShareLinkId && record.Share != FileShare.Restrict));

            foreach (var file in files)
            {
                Logger.InfoFormat("Move shared file {0} from {1} to {2}", file.ID, parentId, toId);
                fileDao.MoveFile(file.ID, toId);
            }
        }

        public static void ReassignItems<T>(T parentId, Guid fromUserId, Guid toUserId, IFolderDao<T> folderDao, IFileDao<T> fileDao)
        {
            var fileIds = fileDao.GetFiles(parentId, new OrderBy(SortedByType.AZ, true), FilterType.ByUser, false, fromUserId, null, true, true)
                                 .Where(file => file.CreateBy == fromUserId).Select(file => file.ID);

            fileDao.ReassignFiles(fileIds.ToArray(), toUserId);

            var folderIds = folderDao.GetFolders(parentId, new OrderBy(SortedByType.AZ, true), FilterType.ByUser, false, fromUserId, null, true)
                                     .Where(folder => folder.CreateBy == fromUserId).Select(folder => folder.ID);

            folderDao.ReassignFolders(folderIds.ToArray(), toUserId);
        }
    }
}