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
using System.Net.Http;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task<bool> FileLockedForMeAsync<T>(T fileId, Guid userId = default)
        {
            var app = ThirdPartySelector.GetAppByFileId(fileId.ToString());
            if (app != null)
            {
                return false;
            }

            userId = userId == default ? AuthContext.CurrentAccount.ID : userId;
            var tagDao = DaoFactory.GetTagDao<T>();
            var lockedBy = await FileLockedByAsync(fileId, tagDao);
            return lockedBy != Guid.Empty && lockedBy != userId;
        }

        public Guid FileLockedBy<T>(T fileId, ITagDao<T> tagDao)
        {
            var tagLock = tagDao.GetTagsAsync(fileId, FileEntryType.File, TagType.Locked).ToListAsync().Result.FirstOrDefault();
            return tagLock != null ? tagLock.Owner : Guid.Empty;
        }

        public async Task<Guid> FileLockedByAsync<T>(T fileId, ITagDao<T> tagDao)
        {
            var tags = tagDao.GetTagsAsync(fileId, FileEntryType.File, TagType.Locked);
            var tagLock = await tags.FirstOrDefaultAsync();
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

        public Task<List<FileEntry>> GetBreadCrumbsAsync<T>(T folderId)
        {
            var folderDao = DaoFactory.GetFolderDao<T>();
            return GetBreadCrumbsAsync(folderId, folderDao);
        }

        public async Task<List<FileEntry>> GetBreadCrumbsAsync<T>(T folderId, IFolderDao<T> folderDao)
        {
            if (folderId == null) return new List<FileEntry>();

            var tmpBreadCrumbs = await FileSecurity.FilterReadAsync(await folderDao.GetParentFoldersAsync(folderId));
            var breadCrumbs = tmpBreadCrumbs.Cast<FileEntry>().ToList();

            var firstVisible = breadCrumbs.ElementAtOrDefault(0) as Folder<T>;

            var rootId = 0;
            if (firstVisible == null)
            {
                rootId = await GlobalFolderHelper.FolderShareAsync;
            }
            else
            {
                switch (firstVisible.FolderType)
                {
                    case FolderType.DEFAULT:
                        if (!firstVisible.ProviderEntry)
                        {
                            rootId = await GlobalFolderHelper.FolderShareAsync;
                        }
                        else
                        {
                            switch (firstVisible.RootFolderType)
                            {
                                case FolderType.USER:
                                    rootId = AuthContext.CurrentAccount.ID == firstVisible.RootFolderCreator
                                        ? GlobalFolderHelper.FolderMy
                                        : await GlobalFolderHelper.FolderShareAsync;
                                    break;
                                case FolderType.COMMON:
                                    rootId = await GlobalFolderHelper.FolderCommonAsync;
                                    break;
                            }
                        }
                        break;

                    case FolderType.BUNCH:
                        rootId = await GlobalFolderHelper.FolderProjectsAsync;
                        break;
                }
            }

            var folderDaoInt = DaoFactory.GetFolderDao<int>();

            if (rootId != 0)
            {
                breadCrumbs.Insert(0, await folderDaoInt.GetFolderAsync(rootId));
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

        public async Task SetFileStatusAsync<T>(File<T> file)
        {
            if (file == null || file.ID == null) return;

            await SetFileStatusAsync(new List<File<T>>(1) { file });
        }

        public async Task SetFileStatusAsync(IEnumerable<FileEntry> files)
        {
            await SetFileStatusAsync(files.OfType<File<int>>().Where(r => r.ID != 0).ToList());
            await SetFileStatusAsync(files.OfType<File<string>>().Where(r => !string.IsNullOrEmpty(r.ID)).ToList());
        }

        public async Task SetFileStatusAsync<T>(IEnumerable<File<T>> files)
        {
            var tagDao = DaoFactory.GetTagDao<T>();

            var tags = await tagDao.GetTagsAsync(AuthContext.CurrentAccount.ID, new[] { TagType.Favorite, TagType.Template, TagType.Locked }, files);
            var tagsNew = await tagDao.GetNewTagsAsync(AuthContext.CurrentAccount.ID, files).ToListAsync();

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
        private IHttpClientFactory ClientFactory { get; }

        public EntryManager(
            IDaoFactory daoFactory,
            FileSecurity fileSecurity,
            GlobalFolderHelper globalFolderHelper,
            PathProvider pathProvider,
            AuthContext authContext,
            FilesIntegration filesIntegration,
            FileMarker fileMarker,
            FileUtility fileUtility,
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
            EntryStatusManager entryStatusManager,
            IHttpClientFactory clientFactory)
        {
            DaoFactory = daoFactory;
            FileSecurity = fileSecurity;
            GlobalFolderHelper = globalFolderHelper;
            PathProvider = pathProvider;
            AuthContext = authContext;
            FilesIntegration = filesIntegration;
            FileMarker = fileMarker;
            FileUtility = fileUtility;
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
            ClientFactory = clientFactory;
        }

        public async Task<(IEnumerable<FileEntry> Entries, int Total)> GetEntriesAsync<T>(Folder<T> parent, int from, int count, FilterType filter, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent, bool withSubfolders, OrderBy orderBy)
        {
            int total = 0;

            if (parent == null) throw new ArgumentNullException(nameof(parent), FilesCommonResource.ErrorMassage_FolderNotFound);
            if (parent.ProviderEntry && !FilesSettingsHelper.EnableThirdParty) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_ReadFolder);
            if (parent.RootFolderType == FolderType.Privacy && (!PrivacyRoomSettings.IsAvailable(TenantManager) || !PrivacyRoomSettings.GetEnabled(SettingsManager))) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_ReadFolder);

            var fileSecurity = FileSecurity;
            var entries = Enumerable.Empty<FileEntry>();

            searchInContent = searchInContent && filter != FilterType.ByExtension && !Equals(parent.ID, GlobalFolderHelper.FolderTrash);

            if (parent.FolderType == FolderType.Projects && parent.ID.Equals(await GlobalFolderHelper.FolderProjectsAsync))
            {
                //TODO
                //var apiServer = new ASC.Api.ApiServer();
                //var apiUrl = string.Format("{0}project/maxlastmodified.json", SetupInfo.WebApiBaseUrl);

                //const string responseBody = null;// apiServer.GetApiResponse(apiUrl, "GET");
                //if (responseBody != null)
                //{
                //    JObject responseApi;

                //    Dictionary<int, KeyValuePair<int, string>> folderIDProjectTitle = null;

                //    if (folderIDProjectTitle == null)
                //    {
                //        //apiUrl = string.Format("{0}project/filter.json?sortBy=title&sortOrder=ascending&status=open&fields=id,title,security,projectFolder", SetupInfo.WebApiBaseUrl);

                //        responseApi = JObject.Parse(""); //Encoding.UTF8.GetString(Convert.FromBase64String(apiServer.GetApiResponse(apiUrl, "GET"))));

                //        var responseData = responseApi["response"];

                //        if (!(responseData is JArray)) return (entries, total);

                //        folderIDProjectTitle = new Dictionary<int, KeyValuePair<int, string>>();
                //        foreach (JObject projectInfo in responseData.Children().OfType<JObject>())
                //        {
                //            var projectID = projectInfo["id"].Value<int>();
                //            var projectTitle = Global.ReplaceInvalidCharsAndTruncate(projectInfo["title"].Value<string>());

                //            if (projectInfo.TryGetValue("security", out var projectSecurityJToken))
                //            {
                //                var projectSecurity = projectInfo["security"].Value<JObject>();
                //                if (projectSecurity.TryGetValue("canReadFiles", out var projectCanFileReadJToken))
                //                {
                //                    if (!projectSecurity["canReadFiles"].Value<bool>())
                //                    {
                //                        continue;
                //                    }
                //                }
                //            }

                //            int projectFolderID;
                //            if (projectInfo.TryGetValue("projectFolder", out var projectFolderIDjToken))
                //                projectFolderID = projectFolderIDjToken.Value<int>();
                //            else
                //                projectFolderID = await FilesIntegration.RegisterBunchAsync<int>("projects", "project", projectID.ToString());

                //            if (!folderIDProjectTitle.ContainsKey(projectFolderID))
                //                folderIDProjectTitle.Add(projectFolderID, new KeyValuePair<int, string>(projectID, projectTitle));

                //            Cache.Remove("documents/folders/" + projectFolderID);
                //            Cache.Insert("documents/folders/" + projectFolderID, projectTitle, TimeSpan.FromMinutes(30));
                //        }
                //    }

                //    var rootKeys = folderIDProjectTitle.Keys.ToArray();
                //    if (filter == FilterType.None || filter == FilterType.FoldersOnly)
                //    {
                //        var folders = await DaoFactory.GetFolderDao<int>().GetFoldersAsync(rootKeys, filter, subjectGroup, subjectId, searchText, withSubfolders, false).ToListAsync();

                //        var emptyFilter = string.IsNullOrEmpty(searchText) && filter == FilterType.None && subjectId == Guid.Empty;
                //        if (!emptyFilter)
                //        {
                //            var projectFolderIds =
                //                folderIDProjectTitle
                //                    .Where(projectFolder => string.IsNullOrEmpty(searchText)
                //                                            || (projectFolder.Value.Value ?? "").ToLower().Trim().Contains(searchText.ToLower().Trim()))
                //                    .Select(projectFolder => projectFolder.Key);

                //            folders.RemoveAll(folder => rootKeys.Contains(folder.ID));

                //            var projectFolders = await DaoFactory.GetFolderDao<int>().GetFoldersAsync(projectFolderIds.ToList(), filter, subjectGroup, subjectId, null, false, false).ToListAsync();
                //            folders.AddRange(projectFolders);
                //        }

                //        folders.ForEach(x =>
                //            {
                //                x.Title = folderIDProjectTitle.ContainsKey(x.ID) ? folderIDProjectTitle[x.ID].Value : x.Title;
                //                x.FolderUrl = folderIDProjectTitle.ContainsKey(x.ID) ? PathProvider.GetFolderUrlAsync(x, folderIDProjectTitle[x.ID].Key).Result : string.Empty;
                //            });

                //        if (withSubfolders)
                //        {
                //            entries = entries.Concat(await fileSecurity.FilterReadAsync(folders));
                //        }
                //        else
                //        {
                //            entries = entries.Concat(folders);
                //        }
                //    }

                //    if (filter != FilterType.FoldersOnly && withSubfolders)
                //    {
                //        var files = await DaoFactory.GetFileDao<int>().GetFilesAsync(rootKeys, filter, subjectGroup, subjectId, searchText, searchInContent);
                //        entries = entries.Concat(await fileSecurity.FilterReadAsync(files))
                //    }
                //}

                //CalculateTotal();
            }
            else if (parent.FolderType == FolderType.SHARE)
            {
                //share
                var shared = await fileSecurity.GetSharesForMeAsync(filter, subjectGroup, subjectId, searchText, searchInContent, withSubfolders);

                entries = entries.Concat(shared);

                CalculateTotal();
            }
            else if (parent.FolderType == FolderType.Recent)
            {
                var files = await GetRecentAsync(filter, subjectGroup, subjectId, searchText, searchInContent);
                entries = entries.Concat(files);

                CalculateTotal();
            }
            else if (parent.FolderType == FolderType.Favorites)
            {
                var (files, folders) = await GetFavoritesAsync(filter, subjectGroup, subjectId, searchText, searchInContent);

                entries = entries.Concat(folders);
                entries = entries.Concat(files);

                CalculateTotal();
            }
            else if (parent.FolderType == FolderType.Templates)
            {
                var folderDao = DaoFactory.GetFolderDao<T>();
                var fileDao = DaoFactory.GetFileDao<T>();
                var files = await GetTemplatesAsync(folderDao, fileDao, filter, subjectGroup, subjectId, searchText, searchInContent);
                entries = entries.Concat(files);

                CalculateTotal();
            }
            else if (parent.FolderType == FolderType.Privacy)
            {
                var folderDao = DaoFactory.GetFolderDao<T>();
                var fileDao = DaoFactory.GetFileDao<T>();
                var folders = await folderDao.GetFoldersAsync(parent.ID, orderBy, filter, subjectGroup, subjectId, searchText, withSubfolders).ToListAsync();
                entries = entries.Concat(await fileSecurity.FilterReadAsync(folders));

                var files = await fileDao.GetFilesAsync(parent.ID, orderBy, filter, subjectGroup, subjectId, searchText, searchInContent, withSubfolders).ToListAsync();
                entries = entries.Concat(await fileSecurity.FilterReadAsync(files));

                //share
                var shared = await fileSecurity.GetPrivacyForMeAsync(filter, subjectGroup, subjectId, searchText, searchInContent, withSubfolders);

                entries = entries.Concat(shared);

                CalculateTotal();
            }
            else
            {
                if (parent.FolderType == FolderType.TRASH)
                    withSubfolders = false;

                var folders = await DaoFactory.GetFolderDao<T>().GetFoldersAsync(parent.ID, orderBy, filter, subjectGroup, subjectId, searchText, withSubfolders).ToListAsync();
                entries = entries.Concat(await fileSecurity.FilterReadAsync(folders));

                var files = await DaoFactory.GetFileDao<T>().GetFilesAsync(parent.ID, orderBy, filter, subjectGroup, subjectId, searchText, searchInContent, withSubfolders).ToListAsync();
                entries = entries.Concat(await fileSecurity.FilterReadAsync(files));

                if (filter == FilterType.None || filter == FilterType.FoldersOnly)
                {
                    var folderList = await GetThirpartyFoldersAsync(parent, searchText);

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

            entries = await FileMarker.SetTagsNewAsync(parent, entries);

            //sorting after marking
            if (orderBy.SortedBy == SortedByType.New)
            {
                entries = SortEntries<T>(entries, orderBy);

                total = entries.Count();
                if (0 < from) entries = entries.Skip(from);
                if (0 < count) entries = entries.Take(count);
            }

            await EntryStatusManager.SetFileStatusAsync(entries.Where(r => r != null && r.FileEntryType == FileEntryType.File).ToList());
            return (entries, total);

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

        public async Task<IEnumerable<File<T>>> GetTemplatesAsync<T>(IFolderDao<T> folderDao, IFileDao<T> fileDao, FilterType filter, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent)
        {
            var tagDao = DaoFactory.GetTagDao<T>();
            var tags = tagDao.GetTagsAsync(AuthContext.CurrentAccount.ID, TagType.Template);

            var fileIds = await tags.Where(tag => tag.EntryType == FileEntryType.File).Select(tag => (T)Convert.ChangeType(tag.EntryId, typeof(T))).ToArrayAsync();

            var filesAsync = fileDao.GetFilesFilteredAsync(fileIds, filter, subjectGroup, subjectId, searchText, searchInContent);
            var files = await filesAsync.Where(file => file.RootFolderType != FolderType.TRASH).ToListAsync();

            var tmpFiles = await FileSecurity.FilterReadAsync(files);
            files = tmpFiles.ToList();

            await CheckFolderIdAsync(folderDao, tmpFiles);

            return files;
        }

        public async Task<IEnumerable<Folder<string>>> GetThirpartyFoldersAsync<T>(Folder<T> parent, string searchText = null)
        {
            var folderList = new List<Folder<string>>();

            if ((parent.ID.Equals(GlobalFolderHelper.FolderMy) || parent.ID.Equals(await GlobalFolderHelper.FolderCommonAsync))
                && ThirdpartyConfiguration.SupportInclusion(DaoFactory)
                && (FilesSettingsHelper.EnableThirdParty
                    || CoreBaseSettings.Personal))
            {
                var providerDao = DaoFactory.ProviderDao;
                if (providerDao == null) return folderList;

                var fileSecurity = FileSecurity;

                var providers = await providerDao.GetProvidersInfoAsync(parent.RootFolderType, searchText).ToListAsync();

                foreach (var providerInfo in providers)
                {
                    var fake = GetFakeThirdpartyFolder(providerInfo, parent.ID.ToString());
                    if (await fileSecurity.CanReadAsync(fake))
                    {
                        folderList.Add(fake);
                    }
                }

                if (folderList.Count > 0)
                {
                    var securityDao = DaoFactory.GetSecurityDao<string>();
                    var pureShareRecords = await securityDao.GetPureShareRecordsAsync(folderList);
                    var ids = pureShareRecords
                    //.Where(x => x.Owner == SecurityContext.CurrentAccount.ID)
                    .Select(x => x.EntryId).Distinct();

                    foreach (var id in ids)
                    {
                        folderList.First(y => y.ID.Equals(id)).Shared = true;
                    }
                }
            }

            return folderList;
        }

        public async Task<IEnumerable<FileEntry>> GetRecentAsync(FilterType filter, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent)
        {
            var tagDao = DaoFactory.GetTagDao<int>();
            var tags = await tagDao.GetTagsAsync(AuthContext.CurrentAccount.ID, TagType.Recent).ToListAsync();

            var fileIds = tags.Where(tag => tag.EntryType == FileEntryType.File).ToList();

            var filesEnum = await GetRecentByIdsAsync(fileIds.Where(r => r.EntryId is int).Select(r => (int)r.EntryId), filter, subjectGroup, subjectId, searchText, searchInContent);
            List<FileEntry> files = filesEnum.ToList();
            files.AddRange(await GetRecentByIdsAsync(fileIds.Where(r => r.EntryId is string).Select(r => (string)r.EntryId), filter, subjectGroup, subjectId, searchText, searchInContent));

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

            async Task<IEnumerable<FileEntry>> GetRecentByIdsAsync<T>(IEnumerable<T> fileIds, FilterType filter, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent)
            {
                var folderDao = DaoFactory.GetFolderDao<T>();
                var fileDao = DaoFactory.GetFileDao<T>();
                var files = await fileDao.GetFilesFilteredAsync(fileIds, filter, subjectGroup, subjectId, searchText, searchInContent).Where(file => file.RootFolderType != FolderType.TRASH).ToListAsync();

                var tmpFiles = await FileSecurity.FilterReadAsync(files);
                files = tmpFiles.ToList();

                await CheckFolderIdAsync(folderDao, tmpFiles);

                return files;
            }
        }

        public async Task<(IEnumerable<FileEntry>, IEnumerable<FileEntry>)> GetFavoritesAsync(FilterType filter, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent)
        {
            var tagDao = DaoFactory.GetTagDao<int>();
            var tags = tagDao.GetTagsAsync(AuthContext.CurrentAccount.ID, TagType.Favorite);

            var fileIds = await tags.Where(tag => tag.EntryType == FileEntryType.File).ToListAsync();
            var folderIds = await tags.Where(tag => tag.EntryType == FileEntryType.Folder).ToListAsync();

            var (filesInt, foldersInt) = await GetFavoritesByIdAsync(fileIds.Where(r => r.EntryId is int).Select(r => (int)r.EntryId), folderIds.Where(r => r.EntryId is int).Select(r => (int)r.EntryId), filter, subjectGroup, subjectId, searchText, searchInContent);
            var (filesString, foldersString) = await GetFavoritesByIdAsync(fileIds.Where(r => r.EntryId is string).Select(r => (string)r.EntryId), folderIds.Where(r => r.EntryId is string).Select(r => (string)r.EntryId), filter, subjectGroup, subjectId, searchText, searchInContent);

            var files = new List<FileEntry>();
            files.AddRange(filesInt);
            files.AddRange(filesString);

            var folders = new List<FileEntry>();
            folders.AddRange(foldersInt);
            folders.AddRange(foldersString);

            return (files, folders);

            async Task<(IEnumerable<FileEntry>, IEnumerable<FileEntry>)> GetFavoritesByIdAsync<T>(IEnumerable<T> fileIds, IEnumerable<T> folderIds, FilterType filter, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent)
            {
                var folderDao = DaoFactory.GetFolderDao<T>();
                var fileDao = DaoFactory.GetFileDao<T>();
                var asyncFolders = folderDao.GetFoldersAsync(folderIds, filter, subjectGroup, subjectId, searchText, false, false);
                var files = Enumerable.Empty<FileEntry<T>>();
                var folders = Enumerable.Empty<FileEntry<T>>();
                var asyncFiles = fileDao.GetFilesFilteredAsync(fileIds, filter, subjectGroup, subjectId, searchText, searchInContent, true);
                var fileSecurity = FileSecurity;
                if (filter == FilterType.None || filter == FilterType.FoldersOnly)
                {
                    var tmpFolders = await asyncFolders.Where(folder => folder.RootFolderType != FolderType.TRASH).ToListAsync();

                    folders = await fileSecurity.FilterReadAsync(tmpFolders);

                    await CheckFolderIdAsync(folderDao, folders);
                }

                if (filter != FilterType.FoldersOnly)
                {
                    var tmpFiles = await asyncFiles.Where(file => file.RootFolderType != FolderType.TRASH).ToListAsync();

                    files = await fileSecurity.FilterReadAsync(tmpFiles);

                    await CheckFolderIdAsync(folderDao, folders);
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
                    where = f => !string.IsNullOrEmpty(filterExt) && f.FileEntryType == FileEntryType.File && FileUtility.GetFileExtension(f.Title).Equals(filterExt);
                    break;
            }

            if (where != null)
            {
                entries = entries.Where(where).ToList();
            }

            searchText = (searchText ?? string.Empty).ToLower().Trim();

            if ((!searchInContent || filter == FilterType.ByExtension) && !string.IsNullOrEmpty(searchText))
            {
                entries = entries.Where(f => f.Title.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList();
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
                        cmp = c * FileUtility.GetFileExtension(x.Title).CompareTo(FileUtility.GetFileExtension(y.Title));
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

        public Folder<string> GetFakeThirdpartyFolder(IProviderInfo providerInfo, string parentFolderId = null)
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

        public Task<List<FileEntry>> GetBreadCrumbsAsync<T>(T folderId)
        {
            return BreadCrumbsManager.GetBreadCrumbsAsync(folderId);
        }

        public Task<List<FileEntry>> GetBreadCrumbsAsync<T>(T folderId, IFolderDao<T> folderDao)
        {
            return BreadCrumbsManager.GetBreadCrumbsAsync(folderId, folderDao);
        }

        public async Task CheckFolderIdAsync<T>(IFolderDao<T> folderDao, IEnumerable<FileEntry<T>> entries)
        {
            foreach (var entry in entries)
            {
                if (entry.RootFolderType == FolderType.USER
                    && entry.RootFolderCreator != AuthContext.CurrentAccount.ID)
                {
                    var folderId = entry.FolderID;
                    var folder = await folderDao.GetFolderAsync(folderId);
                    if (!await FileSecurity.CanReadAsync(folder))
                    {
                        entry.FolderIdDisplay = await GlobalFolderHelper.GetFolderShareAsync<T>();
                    }
                }
            }
        }

        public Task<bool> FileLockedForMeAsync<T>(T fileId, Guid userId = default)
        {
            return LockerManager.FileLockedForMeAsync(fileId, userId);
        }

        public Task<Guid> FileLockedByAsync<T>(T fileId, ITagDao<T> tagDao)
        {
            return LockerManager.FileLockedByAsync(fileId, tagDao);
        }

        public async Task<(File<int> file, Folder<int> folderIfNew)> GetFillFormDraftAsync<T>(File<T> sourceFile)
        {
            Folder<int> folderIfNew = null;
            if (sourceFile == null) return (null, folderIfNew);

            File<int> linkedFile = null;
            var fileDao = DaoFactory.GetFileDao<int>();
            var sourceFileDao = DaoFactory.GetFileDao<T>();
            var linkDao = DaoFactory.GetLinkDao();

            var fileSecurity = FileSecurity;

            var linkedId = await linkDao.GetLinkedAsync(sourceFile.ID.ToString());
            if (linkedId != null)
            {
                linkedFile = await fileDao.GetFileAsync(int.Parse(linkedId));
                if (linkedFile == null
                    || !await fileSecurity.CanFillFormsAsync(linkedFile)
                    || await FileLockedForMeAsync(linkedFile.ID)
                    || linkedFile.RootFolderType == FolderType.TRASH)
                {
                    await linkDao.DeleteLinkAsync(sourceFile.ID.ToString());
                    linkedFile = null;
                }
            }

            if (linkedFile == null)
            {
                var folderId = GlobalFolderHelper.FolderMy;
                var folderDao = DaoFactory.GetFolderDao<int>();
                folderIfNew = await folderDao.GetFolderAsync(folderId);
                if (folderIfNew == null) throw new Exception(FilesCommonResource.ErrorMassage_FolderNotFound);
                if (!await fileSecurity.CanCreateAsync(folderIfNew)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);

                linkedFile = ServiceProvider.GetService<File<int>>();
                linkedFile.Title = sourceFile.Title;
                linkedFile.FolderID = folderIfNew.ID;
                linkedFile.FileStatus = sourceFile.FileStatus;
                linkedFile.ConvertedType = sourceFile.ConvertedType;
                linkedFile.Comment = FilesCommonResource.CommentCreateFillFormDraft;
                linkedFile.Encrypted = sourceFile.Encrypted;

                using (var stream = await sourceFileDao.GetFileStreamAsync(sourceFile))
                {
                    linkedFile.ContentLength = stream.CanSeek ? stream.Length : sourceFile.ContentLength;
                    linkedFile = await fileDao.SaveFileAsync(linkedFile, stream);
                }

                await FileMarker.MarkAsNewAsync(linkedFile);

                await linkDao.AddLinkAsync(sourceFile.ID.ToString(), linkedFile.ID.ToString());
            }

            return (linkedFile, folderIfNew);
        }

        public async Task<bool> CheckFillFormDraftAsync<T>(File<T> linkedFile)
        {
            if (linkedFile == null) return false;

            var linkDao = DaoFactory.GetLinkDao();
            var sourceId = await linkDao.GetSourceAsync(linkedFile.ID.ToString());
            var fileSecurity = FileSecurity;

            if (int.TryParse(sourceId, out var sId))
            {
                return await CheckAsync(sId);
            }

            return await CheckAsync(sourceId);

            async Task<bool> CheckAsync<T1>(T1 sourceId)
            {
                var fileDao = DaoFactory.GetFileDao<T1>();
                var sourceFile = await fileDao.GetFileAsync(sourceId);
                if (sourceFile == null
                    || !await fileSecurity.CanFillFormsAsync(sourceFile)
                    || sourceFile.Access != FileShare.FillForms)
                {
                    await linkDao.DeleteLinkAsync(sourceId.ToString());

                    return false;
                }

                return true;
            }
        }

        public async Task<File<T>> SaveEditingAsync<T>(T fileId, string fileExtension, string downloadUri, Stream stream, string doc, string comment = null, bool checkRight = true, bool encrypted = false, ForcesaveType? forcesave = null, bool keepLink = false)
        {
            var newExtension = string.IsNullOrEmpty(fileExtension)
                              ? FileUtility.GetFileExtension(downloadUri)
                              : fileExtension;

            var app = ThirdPartySelector.GetAppByFileId(fileId.ToString());
            if (app != null)
            {
                await app.SaveFileAsync(fileId.ToString(), newExtension, downloadUri, stream);
                return null;
            }

            var fileDao = DaoFactory.GetFileDao<T>();
            var check = await FileShareLink.CheckAsync(doc, false, fileDao);
            var editLink = check.EditLink;
            var file = check.File;
            if (file == null)
            {
                file = await fileDao.GetFileAsync(fileId);
            }

            if (file == null) throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
            var fileSecurity = FileSecurity;
            if (checkRight && !editLink && (!await fileSecurity.CanEditAsync(file) || UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager))) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_EditFile);
            if (checkRight && await FileLockedForMeAsync(file.ID)) throw new Exception(FilesCommonResource.ErrorMassage_LockedFile);
            if (checkRight && (!forcesave.HasValue || forcesave.Value == ForcesaveType.None) && FileTracker.IsEditing(file.ID)) throw new Exception(FilesCommonResource.ErrorMassage_SecurityException_UpdateEditingFile);
            if (file.RootFolderType == FolderType.TRASH) throw new Exception(FilesCommonResource.ErrorMassage_ViewTrashItem);

            var currentExt = file.ConvertedExtension;
            if (string.IsNullOrEmpty(newExtension)) newExtension = FileUtility.GetFileExtension(file.Title);

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
                    if (!await storeTemplate.IsDirectoryAsync(path))
                    {
                        path = FileConstant.NewDocPath + "en-US/";
                    }

                    var fileExt = currentExt != FileUtility.MasterFormExtension
                        ? FileUtility.GetInternalExtension(file.Title)
                        : currentExt;

                    path += "new" + fileExt;

                    //todo: think about the criteria for saving after creation
                    if (!await storeTemplate.IsFileAsync(path) || file.ContentLength != await storeTemplate.GetFileSizeAsync("", path))
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
                        downloadUri = await PathProvider.GetTempUrlAsync(stream, newExtension);
                        downloadUri = DocumentServiceConnector.ReplaceCommunityAdress(downloadUri);
                    }

                    var key = DocumentServiceConnector.GenerateRevisionId(downloadUri);

                    var resultTuple = await DocumentServiceConnector.GetConvertedUriAsync(downloadUri, newExtension, currentExt, key, null, null, null, false);
                    downloadUri = resultTuple.ConvertedDocumentUri;

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
                    await stream.CopyToAsync(tmpStream);
                }
                else
                {

                    var request = new HttpRequestMessage();
                    request.RequestUri = new Uri(downloadUri);

                    var httpClient = ClientFactory.CreateClient();
                    using var response = await httpClient.SendAsync(request);
                    using var editedFileStream = new ResponseStream(response);
                    await editedFileStream.CopyToAsync(tmpStream);
                }
                tmpStream.Position = 0;

                file.ContentLength = tmpStream.Length;
                file.Comment = string.IsNullOrEmpty(comment) ? null : comment;
                if (replaceVersion)
                {
                    file = await fileDao.ReplaceFileVersionAsync(file, tmpStream);
                }
                else
                {
                    file = await fileDao.SaveFileAsync(file, tmpStream);
                }
                if (!keepLink
                   || file.CreateBy != AuthContext.CurrentAccount.ID
                   || !file.IsFillFormDraft)
                {
                    var linkDao = DaoFactory.GetLinkDao();
                    await linkDao.DeleteAllLinkAsync(file.ID.ToString());
                }
            }

            await FileMarker.MarkAsNewAsync(file);
            await FileMarker.RemoveMarkAsNewAsync(file);
            return file;
        }

        public async Task TrackEditingAsync<T>(T fileId, Guid tabId, Guid userId, string doc, bool editingAlone = false)
        {
            bool checkRight;
            if (FileTracker.GetEditingBy(fileId).Contains(userId))
            {
                checkRight = FileTracker.ProlongEditing(fileId, tabId, userId, editingAlone);
                if (!checkRight) return;
            }

            var fileDao = DaoFactory.GetFileDao<T>();
            var check = await FileShareLink.CheckAsync(doc, false, fileDao);
            var editLink = check.EditLink;
            var file = check.File;
            if (file == null)
                file = await fileDao.GetFileAsync(fileId);

            if (file == null) throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
            var fileSecurity = FileSecurity;
            if (!editLink
                && (!await fileSecurity.CanEditAsync(file, userId)
                    && !await fileSecurity.CanCustomFilterEditAsync(file, userId)
                    && !await fileSecurity.CanReviewAsync(file, userId)
                    && !await fileSecurity.CanFillFormsAsync(file, userId)
                    && !await fileSecurity.CanCommentAsync(file, userId)
                    || UserManager.GetUsers(userId).IsVisitor(UserManager)))
            {
                throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_EditFile);
            }
            if (await FileLockedForMeAsync(file.ID, userId)) throw new Exception(FilesCommonResource.ErrorMassage_LockedFile);
            if (file.RootFolderType == FolderType.TRASH) throw new Exception(FilesCommonResource.ErrorMassage_ViewTrashItem);

            checkRight = FileTracker.ProlongEditing(fileId, tabId, userId, editingAlone);
            if (checkRight)
            {
                FileTracker.ChangeRight(fileId, userId, false);
            }
        }

        public async Task<File<T>> UpdateToVersionFileAsync<T>(T fileId, int version, string doc = null, bool checkRight = true)
        {
            var fileDao = DaoFactory.GetFileDao<T>();
            if (version < 1) throw new ArgumentNullException(nameof(version));

            var (editLink, fromFile) = await FileShareLink.CheckAsync(doc, false, fileDao);

            if (fromFile == null)
                fromFile = await fileDao.GetFileAsync(fileId);

            if (fromFile == null) throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);

            if (fromFile.Version != version)
                fromFile = await fileDao.GetFileAsync(fromFile.ID, Math.Min(fromFile.Version, version));

            if (fromFile == null) throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
            if (checkRight && !editLink && (!await FileSecurity.CanEditAsync(fromFile) || UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager))) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_EditFile);
            if (await FileLockedForMeAsync(fromFile.ID)) throw new Exception(FilesCommonResource.ErrorMassage_LockedFile);
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
                var currFile = await fileDao.GetFileAsync(fileId);
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

                using (var stream = await fileDao.GetFileStreamAsync(fromFile))
                {
                    newFile.ContentLength = stream.CanSeek ? stream.Length : fromFile.ContentLength;
                    newFile = await fileDao.SaveFileAsync(newFile, stream);
                }

                if (fromFile.ThumbnailStatus == Thumbnail.Created)
                {
                    using (var thumb = await fileDao.GetThumbnailAsync(fromFile))
                    {
                        await fileDao.SaveThumbnailAsync(newFile, thumb);
                    }
                    newFile.ThumbnailStatus = Thumbnail.Created;
                }

                var linkDao = DaoFactory.GetLinkDao();
                await linkDao.DeleteAllLinkAsync(newFile.ID.ToString());

                await FileMarker.MarkAsNewAsync(newFile); ;

                await EntryStatusManager.SetFileStatusAsync(newFile);

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

        public async Task<File<T>> CompleteVersionFileAsync<T>(T fileId, int version, bool continueVersion, bool checkRight = true)
        {
            var fileDao = DaoFactory.GetFileDao<T>();
            var fileVersion = version > 0
                ? await fileDao.GetFileAsync(fileId, version)
                : await fileDao.GetFileAsync(fileId);
            if (fileVersion == null) throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
            if (checkRight && (!await FileSecurity.CanEditAsync(fileVersion) || UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager))) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_EditFile);
            if (await FileLockedForMeAsync(fileVersion.ID)) throw new Exception(FilesCommonResource.ErrorMassage_LockedFile);
            if (fileVersion.RootFolderType == FolderType.TRASH) throw new Exception(FilesCommonResource.ErrorMassage_ViewTrashItem);
            if (fileVersion.ProviderEntry) throw new Exception(FilesCommonResource.ErrorMassage_BadRequest);

            var lastVersionFile = await fileDao.GetFileAsync(fileVersion.ID);

            if (continueVersion)
            {
                if (lastVersionFile.VersionGroup > 1)
                {
                    await fileDao.ContinueVersionAsync(fileVersion.ID, fileVersion.Version);
                    lastVersionFile.VersionGroup--;
                }
            }
            else
            {
                if (!FileTracker.IsEditing(lastVersionFile.ID))
                {
                    if (fileVersion.Version == lastVersionFile.Version)
                    {
                        lastVersionFile = await UpdateToVersionFileAsync(fileVersion.ID, fileVersion.Version, null, checkRight);
                    }

                    await fileDao.CompleteVersionAsync(fileVersion.ID, fileVersion.Version);
                    lastVersionFile.VersionGroup++;
                }
            }

            await EntryStatusManager.SetFileStatusAsync(lastVersionFile);

            return lastVersionFile;
        }

        public async Task<FileOptions<T>> FileRenameAsync<T>(T fileId, string title)
        {
            var fileDao = DaoFactory.GetFileDao<T>();
            var file = await fileDao.GetFileAsync(fileId);
            if (file == null) throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
            if (!await FileSecurity.CanEditAsync(file)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_RenameFile);
            if (!await FileSecurity.CanDeleteAsync(file) && UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_RenameFile);
            if (await FileLockedForMeAsync(file.ID)) throw new Exception(FilesCommonResource.ErrorMassage_LockedFile);
            if (file.ProviderEntry && FileTracker.IsEditing(file.ID)) throw new Exception(FilesCommonResource.ErrorMassage_UpdateEditingFile);
            if (file.RootFolderType == FolderType.TRASH) throw new Exception(FilesCommonResource.ErrorMassage_ViewTrashItem);

            title = Global.ReplaceInvalidCharsAndTruncate(title);

            var ext = FileUtility.GetFileExtension(file.Title);
            if (!string.Equals(ext, FileUtility.GetFileExtension(title), StringComparison.InvariantCultureIgnoreCase))
            {
                title += ext;
            }

            var fileAccess = file.Access;

            var renamed = false;
            if (!string.Equals(file.Title, title))
            {
                var newFileID = await fileDao.FileRenameAsync(file, title);

                file = await fileDao.GetFileAsync(newFileID);
                file.Access = fileAccess;

                await DocumentServiceHelper.RenameFileAsync(file, fileDao);

                renamed = true;
            }

            await EntryStatusManager.SetFileStatusAsync(file);

            return new FileOptions<T>
            {
                File = file,
                Renamed = renamed
            };
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
        public async Task DeleteSubitemsAsync<T>(T parentId, IFolderDao<T> folderDao, IFileDao<T> fileDao, ILinkDao linkDao)
        {
            var folders = folderDao.GetFoldersAsync(parentId);
            await foreach (var folder in folders)
            {
                await DeleteSubitemsAsync(folder.ID, folderDao, fileDao, linkDao);

                Logger.InfoFormat("Delete folder {0} in {1}", folder.ID, parentId);
                await folderDao.DeleteFolderAsync(folder.ID);
            }

            var files = fileDao.GetFilesAsync(parentId, null, FilterType.None, false, Guid.Empty, string.Empty, true);
            await foreach (var file in files)
            {
                Logger.InfoFormat("Delete file {0} in {1}", file.ID, parentId);
                await fileDao.DeleteFileAsync(file.ID);

                await linkDao.DeleteAllLinkAsync(file.ID.ToString());
            }
        }

        public async Task MoveSharedItemsAsync<T>(T parentId, T toId, IFolderDao<T> folderDao, IFileDao<T> fileDao)
        {
            var fileSecurity = FileSecurity;

            var folders = folderDao.GetFoldersAsync(parentId);
            await foreach (var folder in folders)
            {
                var shares = await fileSecurity.GetSharesAsync(folder);
                var shared = folder.Shared
                             && shares.Any(record => record.Share != FileShare.Restrict);
                if (shared)
                {
                    Logger.InfoFormat("Move shared folder {0} from {1} to {2}", folder.ID, parentId, toId);
                    await folderDao.MoveFolderAsync(folder.ID, toId, null);
                }
                else
                {
                    await MoveSharedItemsAsync(folder.ID, toId, folderDao, fileDao);
                }
            }

            var files = fileDao.GetFilesAsync(parentId, null, FilterType.None, false, Guid.Empty, string.Empty, true)
                .WhereAwait(async file => file.Shared &&
                (await fileSecurity.GetSharesAsync(file)).Any(record => record.Subject != FileConstant.ShareLinkId && record.Share != FileShare.Restrict));

            await foreach (var file in files)
            {
                Logger.InfoFormat("Move shared file {0} from {1} to {2}", file.ID, parentId, toId);
                await fileDao.MoveFileAsync(file.ID, toId);
            }
        }

        public static async Task ReassignItemsAsync<T>(T parentId, Guid fromUserId, Guid toUserId, IFolderDao<T> folderDao, IFileDao<T> fileDao)
        {
            var files = await fileDao.GetFilesAsync(parentId, new OrderBy(SortedByType.AZ, true), FilterType.ByUser, false, fromUserId, null, true, true).ToListAsync();
            var fileIds = files.Where(file => file.CreateBy == fromUserId).Select(file => file.ID);

            await fileDao.ReassignFilesAsync(fileIds.ToArray(), toUserId);

            var folderIds = await folderDao.GetFoldersAsync(parentId, new OrderBy(SortedByType.AZ, true), FilterType.ByUser, false, fromUserId, null, true)
                                     .Where(folder => folder.CreateBy == fromUserId).Select(folder => folder.ID)
                                     .ToListAsync();

            await folderDao.ReassignFoldersAsync(folderIds.ToArray(), toUserId);
        }
    }
}