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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Web;

using ASC.Api.Core;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.ElasticSearch;
using ASC.FederatedLogin.LoginProviders;
using ASC.Files.Core;
using ASC.Files.Core.Data;
using ASC.Files.Core.Security;
using ASC.Files.Resources;
using ASC.MessagingSystem;
using ASC.Web.Core.Files;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Core.Search;
using ASC.Web.Files.Helpers;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.Services.NotifyService;
using ASC.Web.Files.Services.WCFService.FileOperations;
using ASC.Web.Files.ThirdPartyApp;
using ASC.Web.Files.Utils;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using File = ASC.Files.Core.File;
using FileShare = ASC.Files.Core.Security.FileShare;
using UrlShortener = ASC.Web.Core.Utility.UrlShortener;

namespace ASC.Web.Files.Services.WCFService
{
    public class FileStorageService //: IFileStorageService
    {
        private static readonly FileEntrySerializer serializer = new FileEntrySerializer();
        public Global Global { get; }
        public GlobalStore GlobalStore { get; }
        public GlobalFolderHelper GlobalFolderHelper { get; }
        public FilesSettingsHelper FilesSettingsHelper { get; }
        public AuthContext AuthContext { get; }
        public UserManager UserManager { get; }
        public FactoryIndexer<FoldersWrapper> FoldersIndexer { get; }
        public FactoryIndexer<FilesWrapper> FilesIndexer { get; }
        public FileUtility FileUtility { get; }
        public FilesLinkUtility FilesLinkUtility { get; }
        public BaseCommonLinkUtility BaseCommonLinkUtility { get; }
        public CoreBaseSettings CoreBaseSettings { get; }
        public CustomNamingPeople CustomNamingPeople { get; }
        public DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }
        public DocuSignLoginProvider DocuSignLoginProvider { get; }
        public PathProvider PathProvider { get; }
        public FileSecurity FileSecurity { get; }
        public SocketManager SocketManager { get; }
        public IDaoFactory DaoFactory { get; }
        public FileMarker FileMarker { get; }
        public EntryManager EntryManager { get; }
        public FilesMessageService FilesMessageService { get; }
        public DocumentServiceTrackerHelper DocumentServiceTrackerHelper { get; }
        public DocuSignToken DocuSignToken { get; }
        public DocuSignHelper DocuSignHelper { get; }
        public FileShareLink FileShareLink { get; }
        public FileConverter FileConverter { get; }
        public DocumentServiceHelper DocumentServiceHelper { get; }
        public ThirdpartyConfiguration ThirdpartyConfiguration { get; }
        public DocumentServiceConnector DocumentServiceConnector { get; }
        public FileSharing FileSharing { get; }
        public NotifyClient NotifyClient { get; }
        public FileOperationsManagerHelper FileOperationsManagerHelper { get; }
        public UrlShortener UrlShortener { get; }
        public IServiceProvider ServiceProvider { get; }
        public FileSharingAceHelper FileSharingAceHelper { get; }
        public ApiContext ApiContext { get; }
        public ILog Logger { get; set; }

        public FileStorageService(
            Global global,
            GlobalStore globalStore,
            GlobalFolderHelper globalFolderHelper,
            FilesSettingsHelper filesSettingsHelper,
            AuthContext authContext,
            UserManager userManager,
            FactoryIndexer<FoldersWrapper> foldersIndexer,
            FactoryIndexer<FilesWrapper> filesIndexer,
            FileUtility fileUtility,
            FilesLinkUtility filesLinkUtility,
            BaseCommonLinkUtility baseCommonLinkUtility,
            CoreBaseSettings coreBaseSettings,
            CustomNamingPeople customNamingPeople,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            IHttpContextAccessor httpContextAccessor,
            IOptionsMonitor<ILog> optionMonitor,
            DocuSignLoginProvider docuSignLoginProvider,
            PathProvider pathProvider,
            FileSecurity fileSecurity,
            SocketManager socketManager,
            IDaoFactory daoFactory,
            FileMarker fileMarker,
            EntryManager entryManager,
            FilesMessageService filesMessageService,
            DocumentServiceTrackerHelper documentServiceTrackerHelper,
            DocuSignToken docuSignToken,
            DocuSignHelper docuSignHelper,
            FileShareLink fileShareLink,
            FileConverter fileConverter,
            DocumentServiceHelper documentServiceHelper,
            ThirdpartyConfiguration thirdpartyConfiguration,
            DocumentServiceConnector documentServiceConnector,
            FileSharing fileSharing,
            NotifyClient notifyClient,
            FileOperationsManagerHelper fileOperationsManagerHelper,
            UrlShortener urlShortener,
            IServiceProvider serviceProvider,
            FileSharingAceHelper fileSharingAceHelper,
            ApiContext apiContext)
        {
            Global = global;
            GlobalStore = globalStore;
            GlobalFolderHelper = globalFolderHelper;
            FilesSettingsHelper = filesSettingsHelper;
            AuthContext = authContext;
            UserManager = userManager;
            FoldersIndexer = foldersIndexer;
            FilesIndexer = filesIndexer;
            FileUtility = fileUtility;
            FilesLinkUtility = filesLinkUtility;
            BaseCommonLinkUtility = baseCommonLinkUtility;
            CoreBaseSettings = coreBaseSettings;
            CustomNamingPeople = customNamingPeople;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            HttpContextAccessor = httpContextAccessor;
            DocuSignLoginProvider = docuSignLoginProvider;
            PathProvider = pathProvider;
            FileSecurity = fileSecurity;
            SocketManager = socketManager;
            DaoFactory = daoFactory;
            FileMarker = fileMarker;
            EntryManager = entryManager;
            FilesMessageService = filesMessageService;
            DocumentServiceTrackerHelper = documentServiceTrackerHelper;
            DocuSignToken = docuSignToken;
            DocuSignHelper = docuSignHelper;
            FileShareLink = fileShareLink;
            FileConverter = fileConverter;
            DocumentServiceHelper = documentServiceHelper;
            ThirdpartyConfiguration = thirdpartyConfiguration;
            DocumentServiceConnector = documentServiceConnector;
            FileSharing = fileSharing;
            NotifyClient = notifyClient;
            FileOperationsManagerHelper = fileOperationsManagerHelper;
            UrlShortener = urlShortener;
            ServiceProvider = serviceProvider;
            FileSharingAceHelper = fileSharingAceHelper;
            ApiContext = apiContext;
            Logger = optionMonitor.Get("ASC.Files");
        }

        public Folder<T> GetFolder<T>(T folderId)
        {
            var folderDao = GetFolderDao<T>();
            var folder = folderDao.GetFolder(folderId);

            ErrorIf(folder == null, FilesCommonResource.ErrorMassage_FolderNotFound);
            ErrorIf(!FileSecurity.CanRead<T>(folder), FilesCommonResource.ErrorMassage_SecurityException_ReadFolder);

            return folder;
        }

        public ItemList<Folder<T>> GetFolders<T>(T parentId)
        {
            var folderDao = GetFolderDao<T>();

            try
            {
                var folders = EntryManager.GetEntries(folderDao.GetFolder(parentId), 0, 0, FilterType.FoldersOnly, false, Guid.Empty, string.Empty, false, false, new OrderBy(SortedByType.AZ, true), out var total);
                return new ItemList<Folder<T>>(folders.OfType<Folder<T>>());
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }
        }

        public ItemList<T> GetPath<T>(T folderId)
        {
            var folderDao = GetFolderDao<T>();
            var folder = folderDao.GetFolder(folderId);

            ErrorIf(folder == null, FilesCommonResource.ErrorMassage_FolderNotFound);
            ErrorIf(!FileSecurity.CanRead<T>(folder), FilesCommonResource.ErrorMassage_SecurityException_ViewFolder);

            return new ItemList<T>(EntryManager.GetBreadCrumbs(folderId, folderDao).Select(f => f.ID));
        }

        public DataWrapper<T> GetFolderItems<T>(T parentId, int from, int count, FilterType filter, bool subjectGroup, string ssubject, string searchText, bool searchInContent, bool withSubfolders, OrderBy orderBy)
        {
            var subjectId = string.IsNullOrEmpty(ssubject) ? Guid.Empty : new Guid(ssubject);

            var folderDao = GetFolderDao<T>();
            var fileDao = GetFileDao<T>();

            Folder<T> parent = null;
            try
            {
                parent = folderDao.GetFolder(parentId);
                if (parent != null && !string.IsNullOrEmpty(parent.Error)) throw new Exception(parent.Error);
            }
            catch (Exception e)
            {
                if (parent != null && parent.ProviderEntry)
                {
                    throw GenerateException(new Exception(FilesCommonResource.ErrorMassage_SharpBoxException, e));
                }
                throw GenerateException(e);
            }

            ErrorIf(parent == null, FilesCommonResource.ErrorMassage_FolderNotFound);
            ErrorIf(!FileSecurity.CanRead<T>(parent), FilesCommonResource.ErrorMassage_SecurityException_ViewFolder);
            ErrorIf(parent.RootFolderType == FolderType.TRASH && !Equals(parent.ID, GlobalFolderHelper.FolderTrash), FilesCommonResource.ErrorMassage_ViewTrashItem);

            if (orderBy != null)
            {
                FilesSettingsHelper.DefaultOrder = orderBy;
            }
            else
            {
                orderBy = FilesSettingsHelper.DefaultOrder;
            }

            if (Equals(parent.ID, GlobalFolderHelper.FolderShare) && orderBy.SortedBy == SortedByType.DateAndTime)
                orderBy.SortedBy = SortedByType.New;

            int total;
            IEnumerable<FileEntry> entries;
            try
            {
                entries = EntryManager.GetEntries(parent, from, count, filter, subjectGroup, subjectId, searchText, searchInContent, withSubfolders, orderBy, out total);
            }
            catch (Exception e)
            {
                if (parent.ProviderEntry)
                {
                    throw GenerateException(new Exception(FilesCommonResource.ErrorMassage_SharpBoxException, e));
                }
                throw GenerateException(e);
            }

            var breadCrumbs = EntryManager.GetBreadCrumbs(parentId, folderDao);

            var prevVisible = breadCrumbs.ElementAtOrDefault(breadCrumbs.Count() - 2);
            if (prevVisible != null)
            {
                parent.ParentFolderID = prevVisible.ID;
            }

            parent.Shareable = FileSharing.CanSetAccess<T>(parent) || parent.FolderType == FolderType.SHARE;

            entries = entries.Where(x => x.FileEntryType == FileEntryType.Folder || !FileConverter.IsConverting((File)x));

            var result = new DataWrapper<T>
            {
                Total = total,
                Entries = new ItemList<FileEntry>(entries.ToList()),
                FolderPathParts = new ItemList<T>(breadCrumbs.Select(f => f.ID)),
                FolderInfo = parent,
                RootFoldersIdMarkedAsNew = FileMarker.GetRootFoldersIdMarkedAsNew<T>()
            };

            return result;
        }

        public object GetFolderItemsXml<T>(T parentId, int from, int count, FilterType filter, bool subjectGroup, string subjectID, string search, bool searchInContent, bool withSubfolders, OrderBy orderBy)
        {
            var folderItems = GetFolderItems(parentId, from, count, filter, subjectGroup, subjectID, search, searchInContent, withSubfolders, orderBy);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(serializer.ToXml(folderItems))
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
            return response;
        }

        public ItemList<FileEntry> GetItems<T>(ItemList<string> items, FilterType filter, bool subjectGroup, string subjectID, string search)
        {
            ParseArrayItems<T>(items, out var foldersId, out var filesId);

            var subjectId = string.IsNullOrEmpty(subjectID) ? Guid.Empty : new Guid(subjectID);

            var entries = Enumerable.Empty<FileEntry>();

            var folderDao = GetFolderDao<T>();
            var fileDao = GetFileDao<T>();
            var folders = folderDao.GetFolders(foldersId.ToArray());
            folders = FileSecurity.FilterRead(folders).ToList();
            entries = entries.Concat(folders);

            var files = fileDao.GetFiles(filesId.ToArray());
            files = FileSecurity.FilterRead(files).ToList();
            entries = entries.Concat(files);

            entries = EntryManager.FilterEntries(entries, filter, subjectGroup, subjectId, search, true);

            foreach (var fileEntry in entries)
            {
                if (fileEntry is File<T> file)
                {
                    if (fileEntry.RootFolderType == FolderType.USER
                        && !Equals(fileEntry.RootFolderCreator, AuthContext.CurrentAccount.ID)
                        && !FileSecurity.CanRead<T>(folderDao.GetFolder(file.FolderIdDisplay)))
                    {
                        file.FolderIdDisplay = GlobalFolderHelper.GetFolderShare<T>();
                    }
                }
                else if (fileEntry is Folder<T> folder)
                {
                    if (fileEntry.RootFolderType == FolderType.USER
                        && !Equals(fileEntry.RootFolderCreator, AuthContext.CurrentAccount.ID)
                        && !FileSecurity.CanRead<T>(folderDao.GetFolder(folder.FolderIdDisplay)))
                    {
                        folder.FolderIdDisplay = GlobalFolderHelper.GetFolderShare<T>();
                    }
                }
            }

            EntryManager.SetFileStatus(entries.OfType<File>().Where(r => r.ID != null).ToList());

            return new ItemList<FileEntry>(entries);
        }

        public Folder<T> CreateNewFolder<T>(T parentId, string title)
        {
            if (string.IsNullOrEmpty(title) || parentId == null) throw new ArgumentException();

            var folderDao = GetFolderDao<T>();
            var parent = folderDao.GetFolder(parentId);
            ErrorIf(parent == null, FilesCommonResource.ErrorMassage_FolderNotFound);
            ErrorIf(!FileSecurity.CanCreate<T>(parent), FilesCommonResource.ErrorMassage_SecurityException_Create);

            try
            {
                var newFolder = ServiceProvider.GetService<Folder<T>>();
                newFolder.Title = title;
                newFolder.ParentFolderID = parent.ID;

                var folderId = folderDao.SaveFolder(newFolder);
                var folder = folderDao.GetFolder(folderId);
                FilesMessageService.Send(folder, GetHttpHeaders(), MessageAction.FolderCreated, folder.Title);

                return folder;
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }
        }

        public Folder<T> FolderRename<T>(T folderId, string title)
        {
            var tagDao = GetTagDao();
            var folderDao = GetFolderDao<T>();
            var folder = folderDao.GetFolder(folderId);
            ErrorIf(folder == null, FilesCommonResource.ErrorMassage_FolderNotFound);
            ErrorIf(!FileSecurity.CanEdit<T>(folder), FilesCommonResource.ErrorMassage_SecurityException_RenameFolder);
            if (!FileSecurity.CanDelete<T>(folder) && UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_RenameFolder);
            ErrorIf(folder.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_ViewTrashItem);

            var folderAccess = folder.Access;

            if (string.Compare(folder.Title, title, false) != 0)
            {
                var newFolderID = folderDao.RenameFolder(folder, title);
                folder = folderDao.GetFolder(newFolderID);
                folder.Access = folderAccess;

                FilesMessageService.Send(folder, GetHttpHeaders(), MessageAction.FolderRenamed, folder.Title);

                if (!folder.ProviderEntry)
                {
                    FoldersIndexer.IndexAsync(FoldersWrapper.GetFolderWrapper(ServiceProvider, folder));
                }
            }

            var tag = tagDao.GetNewTags(AuthContext.CurrentAccount.ID, folder).FirstOrDefault();
            if (tag != null)
            {
                folder.NewForMe = tag.Count;
            }

            if (folder.RootFolderType == FolderType.USER
                && !Equals(folder.RootFolderCreator, AuthContext.CurrentAccount.ID)
                && !FileSecurity.CanRead<T>(folderDao.GetFolder(folder.ParentFolderID)))
            {
                folder.FolderIdDisplay = GlobalFolderHelper.GetFolderShare<T>();
            }

            return folder;
        }

        public File<T> GetFile<T>(T fileId, int version)
        {
            var fileDao = GetFileDao<T>();
            fileDao.InvalidateCache(fileId);

            var file = version > 0
                           ? fileDao.GetFile(fileId, version)
                           : fileDao.GetFile(fileId);
            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
            ErrorIf(!FileSecurity.CanRead<T>(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

            EntryManager.SetFileStatus(file);

            if (file.RootFolderType == FolderType.USER
                && !Equals(file.RootFolderCreator, AuthContext.CurrentAccount.ID))
            {
                var folderDao = GetFolderDao<T>();
                if (!FileSecurity.CanRead<T>(folderDao.GetFolder(file.FolderID)))
                {
                    file.FolderIdDisplay = GlobalFolderHelper.GetFolderShare<T>();
                }
            }

            return file;
        }

        public ItemList<File<T>> GetSiblingsFile<T>(T fileId, T parentId, FilterType filter, bool subjectGroup, string subjectID, string search, bool searchInContent, bool withSubfolders, OrderBy orderBy)
        {
            var subjectId = string.IsNullOrEmpty(subjectID) ? Guid.Empty : new Guid(subjectID);

            var fileDao = GetFileDao<T>();
            var folderDao = GetFolderDao<T>();

            var file = fileDao.GetFile(fileId);
            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
            ErrorIf(!FileSecurity.CanRead<T>(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

            var parent = folderDao.GetFolder(parentId == null || parentId.Equals(default) ? file.FolderID : parentId);
            ErrorIf(parent == null, FilesCommonResource.ErrorMassage_FolderNotFound);
            ErrorIf(parent.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_ViewTrashItem);

            if (filter == FilterType.FoldersOnly)
            {
                return new ItemList<File<T>>();
            }
            if (filter == FilterType.None)
            {
                filter = FilterType.FilesOnly;
            }

            if (orderBy == null)
            {
                orderBy = FilesSettingsHelper.DefaultOrder;
            }
            if (Equals(parent.ID, GlobalFolderHelper.GetFolderShare<T>()) && orderBy.SortedBy == SortedByType.DateAndTime)
            {
                orderBy.SortedBy = SortedByType.New;
            }

            var entries = Enumerable.Empty<FileEntry>();

            if (!FileSecurity.CanRead<T>(parent))
            {
                file.FolderID = GlobalFolderHelper.GetFolderShare<T>();
                entries = entries.Concat(new[] { file });
            }
            else
            {
                try
                {
                    entries = EntryManager.GetEntries(parent, 0, 0, filter, subjectGroup, subjectId, search, searchInContent, withSubfolders, orderBy, out var total);
                }
                catch (Exception e)
                {
                    if (parent.ProviderEntry)
                    {
                        throw GenerateException(new Exception(FilesCommonResource.ErrorMassage_SharpBoxException, e));
                    }
                    throw GenerateException(e);
                }
            }

            var previewedType = new[] { FileType.Image, FileType.Audio, FileType.Video };

            var result =
                FileSecurity.FilterRead(entries.OfType<File<T>>())
                            .OfType<File<T>>()
                            .Where(f => previewedType.Contains(FileUtility.GetFileTypeByFileName(f.Title)));

            return new ItemList<File<T>>(result);
        }

        public File<T> CreateNewFile<T>(FileModel<T> fileWrapper)
        {
            if (string.IsNullOrEmpty(fileWrapper.Title) || fileWrapper.ParentId == null) throw new ArgumentException();

            var fileDao = GetFileDao<T>();
            var folderDao = GetFolderDao<T>();

            var folder = folderDao.GetFolder(fileWrapper.ParentId);
            ErrorIf(folder == null, FilesCommonResource.ErrorMassage_FolderNotFound);
            ErrorIf(folder.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_CreateNewFolderInTrash);
            ErrorIf(!FileSecurity.CanCreate<T>(folder), FilesCommonResource.ErrorMassage_SecurityException_Create);

            var file = ServiceProvider.GetService<File<T>>();
            file.FolderID = folder.ID;
            file.Comment = FilesCommonResource.CommentCreate;

            var fileExt = FileUtility.GetInternalExtension(fileWrapper.Title);
            if (!FileUtility.InternalExtension.Values.Contains(fileExt))
            {
                fileExt = FileUtility.InternalExtension[FileType.Document];
                file.Title = fileWrapper.Title + fileExt;
            }
            else
            {
                file.Title = FileUtility.ReplaceFileExtension(fileWrapper.Title, fileExt);
            }

            var culture = UserManager.GetUsers(AuthContext.CurrentAccount.ID).GetCulture();
            var storeTemplate = GetStoreTemplate();

            var path = FileConstant.NewDocPath + culture + "/";
            if (!storeTemplate.IsDirectory(path))
            {
                path = FileConstant.NewDocPath + "default/";
            }

            path += "new" + fileExt;

            try
            {
                using var stream = storeTemplate.GetReadStream("", path);
                file.ContentLength = stream.CanSeek ? stream.Length : storeTemplate.GetFileSize(path);
                file = fileDao.SaveFile(file, stream);
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }
            FilesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileCreated, file.Title);

            FileMarker.MarkAsNew<T>(file);

            return file;
        }

        public KeyValuePair<bool, string> TrackEditFile(string fileId, Guid tabId, string docKeyForTrack, string doc = null, bool isFinish = false)
        {
            try
            {
                var id = FileShareLink.Parse(doc);
                if (string.IsNullOrEmpty(id))
                {
                    if (!AuthContext.IsAuthenticated) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
                    if (!string.IsNullOrEmpty(doc)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
                    id = fileId;
                }

                if (docKeyForTrack != DocumentServiceHelper.GetDocKey(id, -1, DateTime.MinValue)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);

                if (isFinish)
                {
                    FileTracker.Remove(id, tabId);
                    SocketManager.FilesChangeEditors(id, true);
                }
                else
                {
                    EntryManager.TrackEditing(id, tabId, AuthContext.CurrentAccount.ID, doc);
                }

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }

        public ItemDictionary<string, string> CheckEditing<T>(ItemList<T> filesId)
        {
            ErrorIf(!AuthContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);
            var result = new ItemDictionary<string, string>();

            var fileDao = GetFileDao<T>();
            var ids = filesId.Where(FileTracker.IsEditing).Select(id => id).ToArray();

            foreach (var file in fileDao.GetFiles(ids))
            {
                if (file == null || !FileSecurity.CanEdit<T>(file) && !FileSecurity.CanReview<T>(file)) continue;

                var usersId = FileTracker.GetEditingBy(file.ID);
                var value = string.Join(", ", usersId.Select(userId => Global.GetUserName(userId, true)).ToArray());
                result[file.ID.ToString()] = value;
            }

            return result;
        }

        public File<T> SaveEditing<T>(T fileId, string fileExtension, string fileuri, Stream stream, string doc = null, bool forcesave = false)
        {
            try
            {
                if (!forcesave && FileTracker.IsEditingAlone(fileId))
                {
                    FileTracker.Remove(fileId);
                }

                var file = EntryManager.SaveEditing(fileId, fileExtension, fileuri, stream, doc, forcesave: forcesave ? ForcesaveType.User : ForcesaveType.None);

                if (file != null)
                    FilesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileUpdated, file.Title);

                SocketManager.FilesChangeEditors(fileId, !forcesave);
                return file;
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }
        }

        public File<T> UpdateFileStream<T>(T fileId, Stream stream, bool encrypted)
        {
            try
            {
                if (FileTracker.IsEditing(fileId))
                {
                    FileTracker.Remove(fileId);
                }

                var file = EntryManager.SaveEditing(fileId, null, null, stream, null, encrypted ? FilesCommonResource.CommentEncrypted : null, encrypted: encrypted);

                if (file != null)
                    FilesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileUpdated, file.Title);

                SocketManager.FilesChangeEditors(fileId, true);
                return file;
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }
        }

        public string StartEdit<T>(T fileId, bool editingAlone = false, string doc = null)
        {
            try
            {
                IThirdPartyApp app;
                if (editingAlone)
                {
                    ErrorIf(FileTracker.IsEditing(fileId), FilesCommonResource.ErrorMassage_SecurityException_EditFileTwice);

                    app = ThirdPartySelector.GetAppByFileId(fileId.ToString());
                    if (app == null)
                    {
                        EntryManager.TrackEditing(fileId, Guid.Empty, AuthContext.CurrentAccount.ID, doc, true);
                    }

                    //without StartTrack, track via old scheme
                    return DocumentServiceHelper.GetDocKey(fileId, -1, DateTime.MinValue);
                }

                Configuration<string> configuration;

                app = ThirdPartySelector.GetAppByFileId(fileId.ToString());
                if (app == null)
                {
                    DocumentServiceHelper.GetParams(fileId.ToString(), -1, doc, true, true, false, out configuration);
                }
                else
                {
                    var file = app.GetFile(fileId.ToString(), out var editable);
                    DocumentServiceHelper.GetParams<string>(file, true, editable ? FileShare.ReadWrite : FileShare.Read, false, editable, editable, editable, false, out configuration);
                }

                ErrorIf(!configuration.EditorConfig.ModeWrite
                        || !(configuration.Document.Permissions.Edit
                             || configuration.Document.Permissions.Review
                             || configuration.Document.Permissions.FillForms
                             || configuration.Document.Permissions.Comment),
                        !string.IsNullOrEmpty(configuration.ErrorMessage) ? configuration.ErrorMessage : FilesCommonResource.ErrorMassage_SecurityException_EditFile);
                var key = configuration.Document.Key;

                if (!DocumentServiceTrackerHelper.StartTrack(fileId.ToString(), key))
                {
                    throw new Exception(FilesCommonResource.ErrorMassage_StartEditing);
                }

                return key;
            }
            catch (Exception e)
            {
                FileTracker.Remove(fileId);
                throw GenerateException(e);
            }
        }

        public File<T> FileRename<T>(T fileId, string title)
        {
            try
            {
                var renamed = EntryManager.FileRename(fileId, title, out var file);
                if (renamed)
                {
                    FilesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileRenamed, file.Title);

                    if (!file.ProviderEntry)
                    {
                        FilesIndexer.UpdateAsync(FilesWrapper.GetFilesWrapper(ServiceProvider, file), true, r => r.Title);
                    }
                }

                if (file.RootFolderType == FolderType.USER
                    && !Equals(file.RootFolderCreator, AuthContext.CurrentAccount.ID))
                {
                    var folderDao = GetFolderDao<T>();
                    if (!FileSecurity.CanRead<T>(folderDao.GetFolder(file.FolderID)))
                    {
                        file.FolderIdDisplay = GlobalFolderHelper.GetFolderShare<T>();
                    }
                }

                return file;
            }
            catch (Exception ex)
            {
                throw GenerateException(ex);
            }
        }

        public ItemList<File<T>> GetFileHistory<T>(T fileId)
        {
            var fileDao = GetFileDao<T>();
            var file = fileDao.GetFile(fileId);
            ErrorIf(!FileSecurity.CanRead<T>(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

            return new ItemList<File<T>>(fileDao.GetFileHistory(fileId));
        }

        public KeyValuePair<File, ItemList<File<T>>> UpdateToVersion<T>(T fileId, int version)
        {
            var file = EntryManager.UpdateToVersionFile(fileId, version);
            FilesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileRestoreVersion, file.Title, version.ToString(CultureInfo.InvariantCulture));

            if (file.RootFolderType == FolderType.USER
                && !Equals(file.RootFolderCreator, AuthContext.CurrentAccount.ID))
            {
                var folderDao = GetFolderDao<T>();
                if (!FileSecurity.CanRead<T>(folderDao.GetFolder(file.FolderID)))
                {
                    file.FolderIdDisplay = GlobalFolderHelper.GetFolderShare<T>();
                }
            }

            return new KeyValuePair<File, ItemList<File<T>>>(file, GetFileHistory(fileId));
        }

        public string UpdateComment<T>(T fileId, int version, string comment)
        {
            var fileDao = GetFileDao<T>();
            var file = fileDao.GetFile(fileId, version);
            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
            ErrorIf(!FileSecurity.CanEdit<T>(file) || UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager), FilesCommonResource.ErrorMassage_SecurityException_EditFile);
            ErrorIf(EntryManager.FileLockedForMe(file.ID), FilesCommonResource.ErrorMassage_LockedFile);
            ErrorIf(file.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_ViewTrashItem);

            comment = fileDao.UpdateComment(fileId, version, comment);

            FilesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileUpdatedRevisionComment, file.Title, version.ToString(CultureInfo.InvariantCulture));

            return comment;
        }

        public KeyValuePair<File, ItemList<File<T>>> CompleteVersion<T>(T fileId, int version, bool continueVersion)
        {
            var file = EntryManager.CompleteVersionFile(fileId, version, continueVersion);

            FilesMessageService.Send(file, GetHttpHeaders(),
                                     continueVersion ? MessageAction.FileDeletedVersion : MessageAction.FileCreatedVersion,
                                     file.Title, version == 0 ? (file.Version - 1).ToString(CultureInfo.InvariantCulture) : version.ToString(CultureInfo.InvariantCulture));

            if (file.RootFolderType == FolderType.USER
                && !Equals(file.RootFolderCreator, AuthContext.CurrentAccount.ID))
            {
                var folderDao = GetFolderDao<T>();
                if (!FileSecurity.CanRead<T>(folderDao.GetFolder(file.FolderID)))
                {
                    file.FolderIdDisplay = GlobalFolderHelper.GetFolderShare<T>();
                }
            }

            return new KeyValuePair<File, ItemList<File<T>>>(file, GetFileHistory(fileId));
        }

        public File<T> LockFile<T>(T fileId, bool lockfile)
        {
            var tagDao = GetTagDao();
            var fileDao = GetFileDao<T>();
            var file = fileDao.GetFile(fileId);

            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
            ErrorIf(!FileSecurity.CanEdit<T>(file) || lockfile && UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager), FilesCommonResource.ErrorMassage_SecurityException_EditFile);
            ErrorIf(file.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_ViewTrashItem);

            var tagLocked = tagDao.GetTags(file.ID, FileEntryType.File, TagType.Locked).FirstOrDefault();

            ErrorIf(tagLocked != null
                    && tagLocked.Owner != AuthContext.CurrentAccount.ID
                    && !Global.IsAdministrator
                    && (file.RootFolderType != FolderType.USER || file.RootFolderCreator != AuthContext.CurrentAccount.ID), FilesCommonResource.ErrorMassage_LockedFile);

            if (lockfile)
            {
                if (tagLocked == null)
                {
                    tagLocked = new Tag("locked", TagType.Locked, AuthContext.CurrentAccount.ID, file, 0);

                    tagDao.SaveTags(tagLocked);
                }

                var usersDrop = FileTracker.GetEditingBy(file.ID).Where(uid => uid != AuthContext.CurrentAccount.ID).Select(u => u.ToString()).ToArray();
                if (usersDrop.Any())
                {
                    var fileStable = file.Forcesave == ForcesaveType.None ? file : fileDao.GetFileStable(file.ID, file.Version);
                    var docKey = DocumentServiceHelper.GetDocKey(fileStable);
                    DocumentServiceHelper.DropUser(docKey, usersDrop, file.ID);
                }

                FilesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileLocked, file.Title);
            }
            else
            {
                if (tagLocked != null)
                {
                    tagDao.RemoveTags(tagLocked);

                    FilesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileUnlocked, file.Title);
                }

                if (!file.ProviderEntry)
                {
                    file = EntryManager.CompleteVersionFile(file.ID, 0, false);
                    UpdateComment(file.ID.ToString(), file.Version, FilesCommonResource.UnlockComment);
                }
            }

            EntryManager.SetFileStatus(file);

            if (file.RootFolderType == FolderType.USER
                && !Equals(file.RootFolderCreator, AuthContext.CurrentAccount.ID))
            {
                var folderDao = GetFolderDao<T>();
                if (!FileSecurity.CanRead<T>(folderDao.GetFolder(file.FolderID)))
                {
                    file.FolderIdDisplay = GlobalFolderHelper.GetFolderShare<T>();
                }
            }

            return file;
        }

        public ItemList<EditHistory> GetEditHistory<T>(T fileId, string doc = null)
        {
            var fileDao = GetFileDao<T>();
            var readLink = FileShareLink.Check(doc, true, fileDao, out var file);
            if (file == null)
                file = fileDao.GetFile(fileId);

            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
            ErrorIf(!readLink && !FileSecurity.CanRead<T>(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
            ErrorIf(file.ProviderEntry, FilesCommonResource.ErrorMassage_BadRequest);

            return new ItemList<EditHistory>(fileDao.GetEditHistory(DocumentServiceHelper, file.ID));
        }

        public EditHistoryData GetEditDiffUrl<T>(T fileId, int version = 0, string doc = null)
        {
            var fileDao = GetFileDao<T>();
            var readLink = FileShareLink.Check(doc, true, fileDao, out var file);

            if (file != null)
            {
                fileId = file.ID;
            }

            if (file == null
                || version > 0 && file.Version != version)
            {
                file = version > 0
                           ? fileDao.GetFile(fileId, version)
                           : fileDao.GetFile(fileId);
            }

            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
            ErrorIf(!readLink && !FileSecurity.CanRead<T>(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
            ErrorIf(file.ProviderEntry, FilesCommonResource.ErrorMassage_BadRequest);

            var result = new EditHistoryData
            {
                Key = DocumentServiceHelper.GetDocKey(file),
                Url = DocumentServiceConnector.ReplaceCommunityAdress(PathProvider.GetFileStreamUrl(file, doc)),
                Version = version,
            };

            if (fileDao.ContainChanges(file.ID, file.Version))
            {
                string previouseKey;
                string sourceFileUrl;
                if (file.Version > 1)
                {
                    var previousFileStable = fileDao.GetFileStable(file.ID, file.Version - 1);
                    ErrorIf(previousFileStable == null, FilesCommonResource.ErrorMassage_FileNotFound);

                    sourceFileUrl = PathProvider.GetFileStreamUrl(previousFileStable, doc);

                    previouseKey = DocumentServiceHelper.GetDocKey(previousFileStable);
                }
                else
                {
                    var culture = UserManager.GetUsers(AuthContext.CurrentAccount.ID).GetCulture();
                    var storeTemplate = GetStoreTemplate();

                    var path = FileConstant.NewDocPath + culture + "/";
                    if (!storeTemplate.IsDirectory(path))
                    {
                        path = FileConstant.NewDocPath + "default/";
                    }

                    var fileExt = FileUtility.GetFileExtension(file.Title);

                    path += "new" + fileExt;

                    sourceFileUrl = storeTemplate.GetUri("", path).ToString();
                    sourceFileUrl = BaseCommonLinkUtility.GetFullAbsolutePath(sourceFileUrl);

                    previouseKey = DocumentServiceConnector.GenerateRevisionId(Guid.NewGuid().ToString());
                }

                result.Previous = new EditHistoryUrl
                {
                    Key = previouseKey,
                    Url = DocumentServiceConnector.ReplaceCommunityAdress(sourceFileUrl),
                };
                result.ChangesUrl = PathProvider.GetFileChangesUrl(file, doc);
            }

            result.Token = DocumentServiceHelper.GetSignature(result);

            return result;
        }

        public ItemList<EditHistory> RestoreVersion<T>(T fileId, int version, string url = null, string doc = null)
        {
            IFileDao<T> fileDao;
            File<T> file;
            if (string.IsNullOrEmpty(url))
            {
                file = EntryManager.UpdateToVersionFile(fileId, version, doc);
            }
            else
            {
                string modifiedOnString;
                fileDao = GetFileDao<T>();
                var fromFile = fileDao.GetFile(fileId, version);
                modifiedOnString = fromFile.ModifiedOnString;
                file = EntryManager.SaveEditing(fileId, null, url, null, doc, string.Format(FilesCommonResource.CommentRevertChanges, modifiedOnString));
            }

            FilesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileRestoreVersion, file.Title, version.ToString(CultureInfo.InvariantCulture));

            fileDao = GetFileDao<T>();
            return new ItemList<EditHistory>(fileDao.GetEditHistory(DocumentServiceHelper, file.ID));
        }

        public Web.Core.Files.DocumentService.FileLink GetPresignedUri(string fileId)
        {
            var file = GetFile(fileId, -1);
            var result = new Web.Core.Files.DocumentService.FileLink
            {
                FileType = FileUtility.GetFileExtension(file.Title),
                Url = DocumentServiceConnector.ReplaceCommunityAdress(PathProvider.GetFileStreamUrl(file))
            };

            result.Token = DocumentServiceHelper.GetSignature(result);

            return result;
        }

        public object GetNewItems<T>(T folderId)
        {
            try
            {
                Folder<T> folder;
                var folderDao = GetFolderDao<T>();
                folder = folderDao.GetFolder(folderId);

                var result = FileMarker.MarkedItems(folder);

                result = new List<FileEntry>(EntryManager.SortEntries(result, new OrderBy(SortedByType.DateAndTime, false)));

                if (!result.ToList().Any())
                {
                    MarkAsRead<T>(new ItemList<string> { "folder_" + folderId });
                }

                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(serializer.ToXml(new ItemList<FileEntry>(result)))
                };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                return response;
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }
        }

        public ItemList<FileOperationResult> MarkAsRead<T>(ItemList<string> items)
        {
            if (items.Count == 0) return GetTasksStatuses();

            ParseArrayItems<T>(items, out var foldersId, out var filesId);

            return FileOperationsManagerHelper.MarkAsRead(foldersId, filesId);
        }

        public ItemList<ThirdPartyParams> GetThirdParty()
        {
            var providerDao = GetProviderDao();
            if (providerDao == null) return new ItemList<ThirdPartyParams>();

            var providersInfo = providerDao.GetProvidersInfo();

            var resultList = providersInfo
                .Select(r =>
                        new ThirdPartyParams
                        {
                            CustomerTitle = r.CustomerTitle,
                            Corporate = r.RootFolderType == FolderType.COMMON,
                            ProviderId = r.ID.ToString(),
                            ProviderKey = r.ProviderKey
                        }
                );
            return new ItemList<ThirdPartyParams>(resultList.ToList());
        }

        public ItemList<Folder<T>> GetThirdPartyFolder<T>(int folderType = 0)
        {
            var providerDao = GetProviderDao();
            if (providerDao == null) return new ItemList<Folder<T>>();

            var providersInfo = providerDao.GetProvidersInfo((FolderType)folderType);

            var folders = providersInfo.Select(providerInfo =>
                {
                    var folder = EntryManager.GetFakeThirdpartyFolder<T>(providerInfo);
                    folder.NewForMe = folder.RootFolderType == FolderType.COMMON ? 1 : 0;
                    return folder;
                });

            return new ItemList<Folder<T>>(folders);
        }

        public Folder<T> SaveThirdParty<T>(ThirdPartyParams thirdPartyParams)
        {
            var folderDao = GetFolderDao<int>();
            var providerDao = GetProviderDao();

            if (providerDao == null) return null;

            ErrorIf(thirdPartyParams == null, FilesCommonResource.ErrorMassage_BadRequest);
            var parentFolder = folderDao.GetFolder(thirdPartyParams.Corporate && !CoreBaseSettings.Personal ? GlobalFolderHelper.FolderCommon : GlobalFolderHelper.FolderMy);
            ErrorIf(!FileSecurity.CanCreate<T>(parentFolder), FilesCommonResource.ErrorMassage_SecurityException_Create);
            ErrorIf(!Global.IsAdministrator && !FilesSettingsHelper.EnableThirdParty, FilesCommonResource.ErrorMassage_SecurityException_Create);

            var lostFolderType = FolderType.USER;
            var folderType = thirdPartyParams.Corporate ? FolderType.COMMON : FolderType.USER;

            int curProviderId;

            MessageAction messageAction;
            if (string.IsNullOrEmpty(thirdPartyParams.ProviderId))
            {
                ErrorIf(!ThirdpartyConfiguration.SupportInclusion
                        ||
                        (!Global.IsAdministrator
                         && !CoreBaseSettings.Personal
                         && !FilesSettingsHelper.EnableThirdParty)
                        , FilesCommonResource.ErrorMassage_SecurityException_Create);

                thirdPartyParams.CustomerTitle = Global.ReplaceInvalidCharsAndTruncate(thirdPartyParams.CustomerTitle);
                ErrorIf(string.IsNullOrEmpty(thirdPartyParams.CustomerTitle), FilesCommonResource.ErrorMassage_InvalidTitle);

                try
                {
                    curProviderId = providerDao.SaveProviderInfo(thirdPartyParams.ProviderKey, thirdPartyParams.CustomerTitle, thirdPartyParams.AuthData, folderType);
                    messageAction = MessageAction.ThirdPartyCreated;
                }
                catch (UnauthorizedAccessException e)
                {
                    throw GenerateException(e, true);
                }
                catch (Exception e)
                {
                    throw GenerateException(e);
                }
            }
            else
            {
                curProviderId = Convert.ToInt32(thirdPartyParams.ProviderId);

                var lostProvider = providerDao.GetProviderInfo(curProviderId);
                ErrorIf(lostProvider.Owner != AuthContext.CurrentAccount.ID, FilesCommonResource.ErrorMassage_SecurityException);

                lostFolderType = lostProvider.RootFolderType;
                if (lostProvider.RootFolderType == FolderType.COMMON && !thirdPartyParams.Corporate)
                {
                    var lostFolder = folderDao.GetFolder((int)lostProvider.RootFolderId);
                    FileMarker.RemoveMarkAsNewForAll(lostFolder);
                }

                curProviderId = providerDao.UpdateProviderInfo(curProviderId, thirdPartyParams.CustomerTitle, thirdPartyParams.AuthData, folderType);
                messageAction = MessageAction.ThirdPartyUpdated;
            }

            var provider = providerDao.GetProviderInfo(curProviderId);
            provider.InvalidateStorage();

            var folderDao1 = GetFolderDao<T>();
            var folder = folderDao1.GetFolder((T)Convert.ChangeType(provider.RootFolderId, typeof(T)));
            ErrorIf(!FileSecurity.CanRead<T>(folder), FilesCommonResource.ErrorMassage_SecurityException_ViewFolder);

            FilesMessageService.Send(parentFolder, GetHttpHeaders(), messageAction, folder.ID.ToString(), provider.ProviderKey);

            if (thirdPartyParams.Corporate && lostFolderType != FolderType.COMMON)
            {
                FileMarker.MarkAsNew<T>(folder);
            }

            return folder;
        }

        public object DeleteThirdParty<T>(string providerId)
        {
            var providerDao = GetProviderDao();
            if (providerDao == null) return null;

            var curProviderId = Convert.ToInt32(providerId);
            var providerInfo = providerDao.GetProviderInfo(curProviderId);

            var folder = EntryManager.GetFakeThirdpartyFolder<T>(providerInfo);
            ErrorIf(!FileSecurity.CanDelete<T>(folder), FilesCommonResource.ErrorMassage_SecurityException_DeleteFolder);

            if (providerInfo.RootFolderType == FolderType.COMMON)
            {
                FileMarker.RemoveMarkAsNewForAll(folder);
            }

            providerDao.RemoveProviderInfo(folder.ProviderId);
            FilesMessageService.Send(folder, GetHttpHeaders(), MessageAction.ThirdPartyDeleted, folder.ID.ToString(), providerInfo.ProviderKey);

            return folder.ID;
        }

        public bool ChangeAccessToThirdparty(bool enable)
        {
            ErrorIf(!Global.IsAdministrator, FilesCommonResource.ErrorMassage_SecurityException);

            FilesSettingsHelper.EnableThirdParty = enable;
            FilesMessageService.Send(GetHttpHeaders(), MessageAction.DocumentsThirdPartySettingsUpdated);

            return FilesSettingsHelper.EnableThirdParty;
        }

        public bool SaveDocuSign(string code)
        {
            ErrorIf(!AuthContext.IsAuthenticated
                    || UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)
                    || !Global.IsAdministrator && !FilesSettingsHelper.EnableThirdParty
                    || !ThirdpartyConfiguration.SupportDocuSignInclusion, FilesCommonResource.ErrorMassage_SecurityException_Create);

            var token = DocuSignLoginProvider.Instance.GetAccessToken(code);
            DocuSignHelper.ValidateToken(token);
            DocuSignToken.SaveToken(token);
            return true;
        }

        public object DeleteDocuSign()
        {
            DocuSignToken.DeleteToken();
            return null;
        }

        public string SendDocuSign<T>(T fileId, DocuSignData docuSignData)
        {
            try
            {
                ErrorIf(UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)
                    || !FilesSettingsHelper.EnableThirdParty || !ThirdpartyConfiguration.SupportDocuSignInclusion, FilesCommonResource.ErrorMassage_SecurityException_Create);

                return DocuSignHelper.SendDocuSign(fileId, docuSignData, GetHttpHeaders());
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }
        }

        public ItemList<FileOperationResult> GetTasksStatuses()
        {
            ErrorIf(!AuthContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);

            return FileOperationsManagerHelper.GetOperationResults();
        }

        public ItemList<FileOperationResult> TerminateTasks()
        {
            ErrorIf(!AuthContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);

            return FileOperationsManagerHelper.CancelOperations();
        }

        public ItemList<FileOperationResult> BulkDownload(Dictionary<string, string> items)
        {
            ParseArrayItems(items, out var folders, out var files);
            ErrorIf(folders.Count == 0 && files.Count == 0, FilesCommonResource.ErrorMassage_BadRequest);

            return FileOperationsManagerHelper.Download(folders, files, GetHttpHeaders());
        }

        public ItemDictionary<string, string> MoveOrCopyFilesCheck<T>(ItemList<string> items, T destFolderId)
        {
            if (items.Count == 0) return new ItemDictionary<string, string>();

            ParseArrayItems<T>(items, out var foldersId, out var filesId);

            return new ItemDictionary<string, string>(MoveOrCopyFilesCheck(filesId, foldersId, destFolderId));
        }

        private Dictionary<string, string> MoveOrCopyFilesCheck<T>(IEnumerable<T> filesId, IEnumerable<T> foldersId, T destFolderId)
        {
            var result = new Dictionary<string, string>();
            var folderDao = GetFolderDao<T>();
            var fileDao = GetFileDao<T>();

            var toFolder = folderDao.GetFolder(destFolderId);
            ErrorIf(toFolder == null, FilesCommonResource.ErrorMassage_FolderNotFound);
            ErrorIf(!FileSecurity.CanCreate<T>(toFolder), FilesCommonResource.ErrorMassage_SecurityException_Create);

            foreach (var id in filesId)
            {
                var file = fileDao.GetFile(id);
                if (file != null && fileDao.IsExist(file.Title, toFolder.ID))
                {
                    result.Add(id.ToString(), file.Title);
                }
            }

            var folders = folderDao.GetFolders(foldersId.ToArray());
            var foldersProject = folders.Where(folder => folder.FolderType == FolderType.BUNCH).ToList();
            if (foldersProject.Any())
            {
                var toSubfolders = folderDao.GetFolders(toFolder.ID);

                foreach (var folderProject in foldersProject)
                {
                    var toSub = toSubfolders.FirstOrDefault(to => Equals(to.Title, folderProject.Title));
                    if (toSub == null) continue;

                    var filesPr = fileDao.GetFiles(folderProject.ID);
                    var foldersPr = folderDao.GetFolders(folderProject.ID).Select(d => d.ID);

                    var recurseItems = MoveOrCopyFilesCheck(filesPr, foldersPr, toSub.ID);
                    foreach (var recurseItem in recurseItems)
                    {
                        result.Add(recurseItem.Key, recurseItem.Value);
                    }
                }
            }
            try
            {
                foreach (var pair in folderDao.CanMoveOrCopy(foldersId.ToArray(), toFolder.ID))
                {
                    result.Add(pair.Key.ToString(), pair.Value);
                }
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }
            return result;
        }

        public ItemList<FileOperationResult> MoveOrCopyItems<T>(ItemList<string> items, T destFolderId, FileConflictResolveType resolve, bool ic, bool deleteAfter = false)
        {
            ItemList<FileOperationResult> result;
            if (items.Count != 0)
            {
                ParseArrayItems<T>(items, out var foldersId, out var filesId);

                result = FileOperationsManagerHelper.MoveOrCopy(foldersId, filesId, destFolderId, ic, resolve, !deleteAfter, GetHttpHeaders());
            }
            else
            {
                result = FileOperationsManagerHelper.GetOperationResults();
            }
            return result;
        }

        public ItemList<FileOperationResult> DeleteItems<T>(string action, ItemList<string> items, bool ignoreException = false, bool deleteAfter = false, bool immediately = false)
        {
            ParseArrayItems<T>(items, out var foldersId, out var filesId);

            return FileOperationsManagerHelper.Delete(foldersId, filesId, ignoreException, !deleteAfter, immediately, GetHttpHeaders());
        }

        public ItemList<FileOperationResult> EmptyTrash<T>()
        {
            var folderDao = GetFolderDao<T>();
            var fileDao = GetFileDao<T>();
            var trashId = folderDao.GetFolderIDTrash(true);
            var foldersId = folderDao.GetFolders(trashId).Select(f => f.ID).ToList();
            var filesId = fileDao.GetFiles(trashId).ToList();

            return FileOperationsManagerHelper.Delete(foldersId, filesId, false, true, false, GetHttpHeaders());
        }

        public ItemList<FileOperationResult> CheckConversion<T>(ItemList<ItemList<string>> filesInfoJSON)
        {
            if (filesInfoJSON == null || filesInfoJSON.Count == 0) return new ItemList<FileOperationResult>();

            var fileDao = GetFileDao<T>();
            var files = new List<KeyValuePair<File<T>, bool>>();
            foreach (var fileInfo in filesInfoJSON)
            {
                var fileId = (T)Convert.ChangeType(fileInfo[0], typeof(T));

                var file = int.TryParse(fileInfo[1], out var version) && version > 0
                                ? fileDao.GetFile(fileId, version)
                                : fileDao.GetFile(fileId);

                if (file == null)
                {
                    var newFile = ServiceProvider.GetService<File<T>>();
                    newFile.ID = fileId;
                    newFile.Version = version;

                    files.Add(new KeyValuePair<File<T>, bool>(newFile, true));
                    continue;
                }

                ErrorIf(!FileSecurity.CanRead<T>(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

                var startConvert = Convert.ToBoolean(fileInfo[2]);
                if (startConvert && FileConverter.MustConvert(file))
                {
                    try
                    {
                        FileConverter.ExecAsync(file, false, fileInfo.Count > 3 ? fileInfo[3] : null);
                    }
                    catch (Exception e)
                    {
                        throw GenerateException(e);
                    }
                }

                files.Add(new KeyValuePair<File<T>, bool>(file, false));
            }

            var results = FileConverter.GetStatus(files).ToList();

            return new ItemList<FileOperationResult>(results);
        }

        public void ReassignStorage<T>(Guid userFromId, Guid userToId)
        {
            //check current user have access
            ErrorIf(!Global.IsAdministrator, FilesCommonResource.ErrorMassage_SecurityException);

            //check exist userFrom
            var userFrom = UserManager.GetUsers(userFromId);
            ErrorIf(Equals(userFrom, Constants.LostUser), FilesCommonResource.ErrorMassage_UserNotFound);

            //check exist userTo
            var userTo = UserManager.GetUsers(userToId);
            ErrorIf(Equals(userTo, Constants.LostUser), FilesCommonResource.ErrorMassage_UserNotFound);
            ErrorIf(userTo.IsVisitor(UserManager), FilesCommonResource.ErrorMassage_SecurityException);

            var providerDao = GetProviderDao();
            if (providerDao != null)
            {
                var providersInfo = providerDao.GetProvidersInfo(userFrom.ID);
                var commonProvidersInfo = providersInfo.Where(provider => provider.RootFolderType == FolderType.COMMON).ToList();

                //move common thirdparty storage userFrom
                foreach (var commonProviderInfo in commonProvidersInfo)
                {
                    Logger.InfoFormat("Reassign provider {0} from {1} to {2}", commonProviderInfo.ID, userFrom.ID, userTo.ID);
                    providerDao.UpdateProviderInfo(commonProviderInfo.ID, null, null, FolderType.DEFAULT, userTo.ID);
                }
            }

            var folderDao = GetFolderDao<T>();
            var fileDao = GetFileDao<T>();

            if (!userFrom.IsVisitor(UserManager))
            {
                var folderIdFromMy = folderDao.GetFolderIDUser(false, userFrom.ID);

                if (!Equals(folderIdFromMy, 0))
                {
                    //create folder with name userFrom in folder userTo
                    var folderIdToMy = folderDao.GetFolderIDUser(true, userTo.ID);
                    var newFolder = ServiceProvider.GetService<Folder<T>>();
                    newFolder.Title = string.Format(CustomNamingPeople.Substitute<FilesCommonResource>("TitleDeletedUserFolder"), userFrom.DisplayUserName(false, DisplayUserSettingsHelper));
                    newFolder.ParentFolderID = folderIdToMy;

                    var newFolderTo = folderDao.SaveFolder(newFolder);

                    //move items from userFrom to userTo
                    EntryManager.MoveSharedItems(folderIdFromMy, newFolderTo, folderDao, fileDao);

                    EntryManager.ReassignItems(newFolderTo, userFrom.ID, userTo.ID, folderDao, fileDao);
                }
            }

            EntryManager.ReassignItems(GlobalFolderHelper.GetFolderCommon<T>(), userFrom.ID, userTo.ID, folderDao, fileDao);
        }

        public void DeleteStorage<T>(Guid userId)
        {
            //check current user have access
            ErrorIf(!Global.IsAdministrator, FilesCommonResource.ErrorMassage_SecurityException);

            //delete docuSign
            DocuSignToken.DeleteToken(userId);

            var providerDao = GetProviderDao();
            if (providerDao != null)
            {
                var providersInfo = providerDao.GetProvidersInfo(userId);

                //delete thirdparty storage
                foreach (var myProviderInfo in providersInfo)
                {
                    Logger.InfoFormat("Delete provider {0} for {1}", myProviderInfo.ID, userId);
                    providerDao.RemoveProviderInfo(myProviderInfo.ID);
                }
            }

            var folderDao = GetFolderDao<T>();
            var fileDao = GetFileDao<T>();

            //delete all markAsNew
            var rootFoldersId = new List<T>
                {
                    GlobalFolderHelper.GetFolderShare<T>(),
                    GlobalFolderHelper.GetFolderCommon<T>(),
                    GlobalFolderHelper.GetFolderProjects<T>(),
                };

            var folderIdFromMy = folderDao.GetFolderIDUser(false, userId);
            if (!Equals(folderIdFromMy, 0))
            {
                rootFoldersId.Add(folderIdFromMy);
            }

            var rootFolders = folderDao.GetFolders(rootFoldersId.ToArray());
            foreach (var rootFolder in rootFolders)
            {
                FileMarker.RemoveMarkAsNew(rootFolder, userId);
            }

            //delete all from My
            if (!Equals(folderIdFromMy, 0))
            {
                EntryManager.DeleteSubitems(folderIdFromMy, folderDao, fileDao);

                //delete My userFrom folder
                folderDao.DeleteFolder(folderIdFromMy);
                GlobalFolderHelper.SetFolderMy(userId);
            }

            //delete all from Trash
            var folderIdFromTrash = folderDao.GetFolderIDTrash(false, userId);
            if (!Equals(folderIdFromTrash, 0))
            {
                EntryManager.DeleteSubitems(folderIdFromTrash, folderDao, fileDao);
                folderDao.DeleteFolder(folderIdFromTrash);
                GlobalFolderHelper.FolderTrash = userId;
            }

            EntryManager.ReassignItems(GlobalFolderHelper.GetFolderCommon<T>(), userId, AuthContext.CurrentAccount.ID, folderDao, fileDao);
        }

        public ItemList<AceWrapper> GetSharedInfo<T>(ItemList<string> objectIds)
        {
            return FileSharing.GetSharedInfo<T>(objectIds);
        }

        public ItemList<AceShortWrapper> GetSharedInfoShort<T>(string objectId)
        {
            return FileSharing.GetSharedInfoShort<T>(objectId);
        }

        public ItemList<string> SetAceObject<T>(AceCollection aceCollection, bool notify)
        {
            var fileDao = GetFileDao<T>();
            var folderDao = GetFolderDao<T>();
            var result = new ItemList<string>();
            foreach (var objectId in aceCollection.Entries)
            {
                Debug.Assert(objectId != null, "objectId != null");
                var entryType = objectId.StartsWith("file_") ? FileEntryType.File : FileEntryType.Folder;
                var entryId = (T)Convert.ChangeType(objectId.Substring((entryType == FileEntryType.File ? "file_" : "folder_").Length), typeof(T));
                var entry = entryType == FileEntryType.File
                                ? (FileEntry)fileDao.GetFile(entryId)
                                : (FileEntry)folderDao.GetFolder(entryId);

                try
                {
                    var changed = FileSharingAceHelper.SetAceObject<T>(aceCollection.Aces, entry, notify, aceCollection.Message);
                    if (changed)
                    {
                        FilesMessageService.Send(entry, GetHttpHeaders(),
                                                    entryType == FileEntryType.Folder ? MessageAction.FolderUpdatedAccess : MessageAction.FileUpdatedAccess,
                                                    entry.Title);
                    }
                }
                catch (Exception e)
                {
                    throw GenerateException(e);
                }

                var securityDao = GetSecurityDao();
                if (securityDao.IsShared(entry.ID, entryType))
                {
                    result.Add(objectId);
                }
            }
            return result;
        }

        public void RemoveAce<T>(ItemList<string> items)
        {
            ErrorIf(!AuthContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);
            ParseArrayItems<T>(items, out var foldersId, out var filesId);

            var entries = new List<FileEntry>();

            var fileDao = GetFileDao<T>();
            var folderDao = GetFolderDao<T>();
            entries.AddRange(filesId.Select(fileId => fileDao.GetFile(fileId)));
            entries.AddRange(foldersId.Select(folderDao.GetFolder));

            FileSharingAceHelper.RemoveAce<T>(entries);
        }

        public string GetShortenLink<T>(T fileId)
        {
            File<T> file;
            var fileDao = GetFileDao<T>();
            file = fileDao.GetFile(fileId);
            ErrorIf(!FileSharing.CanSetAccess<T>(file), FilesCommonResource.ErrorMassage_SecurityException);
            var shareLink = FileShareLink.GetLink(file);

            try
            {
                return UrlShortener.Instance.GetShortenLink(shareLink);
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }
        }

        public bool SetAceLink<T>(T fileId, FileShare share)
        {
            FileEntry file;
            var fileDao = GetFileDao<T>();
            file = fileDao.GetFile(fileId);
            var aces = new List<AceWrapper>
                {
                    new AceWrapper
                        {
                            Share = share,
                            SubjectId = FileConstant.ShareLinkId,
                            SubjectGroup = true,
                        }
                };

            try
            {

                var changed = FileSharingAceHelper.SetAceObject<T>(aces, file, false, null);
                if (changed)
                {
                    FilesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileUpdatedAccess, file.Title);
                }
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }

            var securityDao = GetSecurityDao();
            return securityDao.IsShared(file.ID, FileEntryType.File);
        }

        public ItemList<MentionWrapper> SharedUsers<T>(T fileId)
        {
            if (!AuthContext.IsAuthenticated || CoreBaseSettings.Personal)
                return null;

            FileEntry file;
            var fileDao = GetFileDao<T>();
            file = fileDao.GetFile(fileId);

            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);

            var usersIdWithAccess = new List<Guid>();
            if (FileSharing.CanSetAccess<T>(file))
            {
                var access = FileSharing.GetSharedInfo<T>(file);
                usersIdWithAccess = access.Where(aceWrapper => !aceWrapper.SubjectGroup && aceWrapper.Share != FileShare.Restrict)
                                          .Select(aceWrapper => aceWrapper.SubjectId)
                                          .ToList();
            }
            else
            {
                usersIdWithAccess.Add(file.CreateBy);
            }

            var users = UserManager.GetUsersByGroup(Constants.GroupEveryone.ID)
                                   .Where(user => !user.ID.Equals(AuthContext.CurrentAccount.ID)
                                                  && !user.ID.Equals(Constants.LostUser.ID))
                                   .Select(user => new MentionWrapper(user, DisplayUserSettingsHelper) { HasAccess = usersIdWithAccess.Contains(user.ID) })
                                   .ToList();

            users = users
                .OrderBy(user => !user.HasAccess)
                .ThenBy(user => user.User, UserInfoComparer.Default)
                .ToList();

            return new ItemList<MentionWrapper>(users);
        }

        public ItemList<AceShortWrapper> SendEditorNotify<T>(T fileId, MentionMessageWrapper mentionMessage)
        {
            ErrorIf(!AuthContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);

            File<T> file;
            var fileDao = GetFileDao<T>();
            file = fileDao.GetFile(fileId);

            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);

            var fileSecurity = FileSecurity;
            ErrorIf(!fileSecurity.CanRead<T>(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
            ErrorIf(mentionMessage == null || mentionMessage.Emails == null, FilesCommonResource.ErrorMassage_BadRequest);

            var changed = false;
            bool? canShare = null;
            if (file.Encrypted) canShare = false;

            var recipients = new List<Guid>();
            foreach (var email in mentionMessage.Emails)
            {
                if (!canShare.HasValue)
                {
                    canShare = FileSharing.CanSetAccess<T>(file);
                }

                var recipient = UserManager.GetUserByEmail(email);
                if (recipient == null || recipient.ID == Constants.LostUser.ID)
                {
                    changed = canShare.Value;
                    continue;
                }

                if (!fileSecurity.CanRead<T>(file, recipient.ID))
                {
                    if (!canShare.Value)
                    {
                        continue;
                    }

                    try
                    {
                        var aces = new List<AceWrapper>
                            {
                                new AceWrapper
                                    {
                                        Share = FileShare.Read,
                                        SubjectId = recipient.ID,
                                        SubjectGroup = false,
                                    }
                            };

                        changed |= FileSharingAceHelper.SetAceObject<T>(aces, file, false, null);

                        recipients.Add(recipient.ID);
                    }
                    catch (Exception e)
                    {
                        throw GenerateException(e);
                    }
                }
                else
                {
                    recipients.Add(recipient.ID);
                }
            }

            if (changed)
            {
                FilesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileUpdatedAccess, file.Title);
            }

            var fileLink = FilesLinkUtility.GetFileWebEditorUrl(file.ID);
            if (mentionMessage.ActionLink != null)
            {
                fileLink += "&" + FilesLinkUtility.Anchor + "=" + HttpUtility.UrlEncode(ActionLinkConfig.Serialize(mentionMessage.ActionLink));
            }

            var message = (mentionMessage.Message ?? "").Trim();
            const int maxMessageLength = 200;
            if (message.Length > maxMessageLength)
            {
                message = message.Substring(0, maxMessageLength) + "...";
            }

            NotifyClient.SendEditorMentions(file, fileLink, recipients, message);

            return changed ? GetSharedInfoShort<T>("file_" + fileId) : null;
        }

        public ItemList<string> GetMailAccounts()
        {
            return null;
            //var apiServer = new ASC.Api.ApiServer();
            //var apiUrl = string.Format("{0}mail/accounts.json", SetupInfo.WebApiBaseUrl);

            //var accounts = new List<string>();

            //var responseBody = apiServer.GetApiResponse(apiUrl, "GET");
            //if (responseBody != null)
            //{
            //    var responseApi = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(responseBody)));

            //    var responseData = responseApi["response"];
            //    if (responseData is JArray)
            //    {
            //        accounts.AddRange(
            //            from account in responseData.Children()
            //            orderby account["isDefault"].Value<bool>() descending
            //            where account["enabled"].Value<bool>() && !account["isGroup"].Value<bool>()
            //            select account["email"].Value<string>()
            //            );
            //    }
            //}
            //ErrorIf(!accounts.Any(), FilesCommonResource.ErrorMassage_MailAccountNotFound);

            //return new ItemList<string>(accounts);
        }

        public ItemList<FileEntry> ChangeOwner<T>(ItemList<string> items, Guid userId)
        {
            var userInfo = UserManager.GetUsers(userId);
            ErrorIf(Equals(userInfo, Constants.LostUser) || userInfo.IsVisitor(UserManager), FilesCommonResource.ErrorMassage_ChangeOwner);

            ParseArrayItems<T>(items, out var foldersId, out var filesId);

            var entries = new List<FileEntry>();

            var folderDao = GetFolderDao<T>();
            var folders = folderDao.GetFolders(foldersId.ToArray());

            foreach (var folder in folders)
            {
                ErrorIf(!FileSecurity.CanEdit<T>(folder), FilesCommonResource.ErrorMassage_SecurityException);
                ErrorIf(folder.RootFolderType != FolderType.COMMON, FilesCommonResource.ErrorMassage_SecurityException);
                if (folder.ProviderEntry) continue;

                var newFolder = folder;
                if (folder.CreateBy != userInfo.ID)
                {
                    var folderAccess = folder.Access;

                    newFolder.CreateBy = userInfo.ID;
                    var newFolderID = folderDao.SaveFolder((Folder<T>)newFolder);

                    newFolder = folderDao.GetFolder(newFolderID);
                    newFolder.Access = folderAccess;

                    FilesMessageService.Send(newFolder, GetHttpHeaders(), MessageAction.FileChangeOwner, new[] { newFolder.Title, userInfo.DisplayUserName(false, DisplayUserSettingsHelper) });
                }
                entries.Add(newFolder);
            }

            var fileDao = GetFileDao<T>();
            var files = fileDao.GetFiles(filesId.ToArray());

            foreach (var file in files)
            {
                ErrorIf(!FileSecurity.CanEdit<T>(file), FilesCommonResource.ErrorMassage_SecurityException);
                ErrorIf(EntryManager.FileLockedForMe(file.ID), FilesCommonResource.ErrorMassage_LockedFile);
                ErrorIf(FileTracker.IsEditing(file.ID), FilesCommonResource.ErrorMassage_UpdateEditingFile);
                ErrorIf(file.RootFolderType != FolderType.COMMON, FilesCommonResource.ErrorMassage_SecurityException);
                if (file.ProviderEntry) continue;

                var newFile = file;
                if (file.CreateBy != userInfo.ID)
                {
                    newFile = ServiceProvider.GetService<File<T>>();
                    newFile.ID = file.ID;
                    newFile.Version = file.Version + 1;
                    newFile.VersionGroup = file.VersionGroup + 1;
                    newFile.Title = file.Title;
                    newFile.FileStatus = file.FileStatus;
                    newFile.FolderID = file.FolderID;
                    newFile.CreateBy = userInfo.ID;
                    newFile.CreateOn = file.CreateOn;
                    newFile.ConvertedType = file.ConvertedType;
                    newFile.Comment = FilesCommonResource.CommentChangeOwner;
                    newFile.Encrypted = file.Encrypted;

                    using (var stream = fileDao.GetFileStream(file))
                    {
                        newFile.ContentLength = stream.CanSeek ? stream.Length : file.ContentLength;
                        newFile = fileDao.SaveFile(newFile, stream);
                    }

                    FileMarker.MarkAsNew<T>(newFile);

                    EntryManager.SetFileStatus(newFile);

                    FilesMessageService.Send(newFile, GetHttpHeaders(), MessageAction.FileChangeOwner, new[] { newFile.Title, userInfo.DisplayUserName(false, DisplayUserSettingsHelper) });
                }
                entries.Add(newFile);
            }

            return new ItemList<FileEntry>(entries);
        }

        public bool StoreOriginal(bool set)
        {
            FilesSettingsHelper.StoreOriginalFiles = set;
            FilesMessageService.Send(GetHttpHeaders(), MessageAction.DocumentsUploadingFormatsSettingsUpdated);

            return FilesSettingsHelper.StoreOriginalFiles;
        }

        public bool HideConfirmConvert(bool isForSave)
        {
            if (isForSave)
            {
                FilesSettingsHelper.HideConfirmConvertSave = true;
            }
            else
            {
                FilesSettingsHelper.HideConfirmConvertOpen = true;
            }

            return true;
        }

        public bool UpdateIfExist(bool set)
        {
            FilesSettingsHelper.UpdateIfExist = set;
            FilesMessageService.Send(GetHttpHeaders(), MessageAction.DocumentsOverwritingSettingsUpdated);

            return FilesSettingsHelper.UpdateIfExist;
        }

        public bool Forcesave(bool set)
        {
            FilesSettingsHelper.Forcesave = set;
            FilesMessageService.Send(GetHttpHeaders(), MessageAction.DocumentsForcesave);

            return FilesSettingsHelper.Forcesave;
        }

        public bool StoreForcesave(bool set)
        {
            ErrorIf(!Global.IsAdministrator, FilesCommonResource.ErrorMassage_SecurityException);

            FilesSettingsHelper.StoreForcesave = set;
            FilesMessageService.Send(GetHttpHeaders(), MessageAction.DocumentsStoreForcesave);

            return FilesSettingsHelper.StoreForcesave;
        }

        public bool ChangeDeleteConfrim(bool set)
        {
            FilesSettingsHelper.ConfirmDelete = set;

            return FilesSettingsHelper.ConfirmDelete;
        }

        public string GetHelpCenter()
        {
            return ""; //TODO: Studio.UserControls.Common.HelpCenter.HelpCenter.RenderControlToString();
        }

        private IFolderDao<T> GetFolderDao<T>()
        {
            return DaoFactory.GetFolderDao<T>();
        }

        private IFileDao<T> GetFileDao<T>()
        {
            return DaoFactory.GetFileDao<T>();
        }

        private ITagDao GetTagDao()
        {
            return DaoFactory.TagDao;
        }

        private IDataStore GetStoreTemplate()
        {
            return GlobalStore.GetStoreTemplate();
        }

        private IProviderDao GetProviderDao()
        {
            return DaoFactory.ProviderDao;
        }

        private ISecurityDao GetSecurityDao()
        {
            return DaoFactory.SecurityDao;
        }

        private static void ParseArrayItems<T>(IEnumerable<string> data, out List<T> foldersId, out List<T> filesId)
        {
            //TODO:!!!!Fix
            foldersId = new List<T>();
            filesId = new List<T>();
            foreach (var id in data)
            {
                if (id.StartsWith("file_")) filesId.Add((T)Convert.ChangeType(id.Substring("file_".Length), typeof(T)));
                if (id.StartsWith("folder_")) foldersId.Add((T)Convert.ChangeType(id.Substring("folder_".Length), typeof(T)));
            }
        }

        private static void ParseArrayItems(Dictionary<string, string> items, out Dictionary<object, string> folders, out Dictionary<object, string> files)
        {
            //TODO:!!!!Fix
            folders = new Dictionary<object, string>();
            files = new Dictionary<object, string>();
            foreach (var item in (items ?? new Dictionary<string, string>()))
            {
                if (item.Key.StartsWith("file_")) files.Add(item.Key.Substring("file_".Length), item.Value);
                if (item.Key.StartsWith("folder_")) folders.Add(item.Key.Substring("folder_".Length), item.Value);
            }
        }

        private static void ErrorIf(bool condition, string errorMessage)
        {
            if (condition) throw new InvalidOperationException(errorMessage);
        }

        private Exception GenerateException(Exception error, bool warning = false)
        {
            if (warning)
            {
                Logger.Info(error);
            }
            else
            {
                Logger.Error(error);
            }
            return new InvalidOperationException(error.Message, error);
        }

        private Dictionary<string, string> GetHttpHeaders()
        {
            if (HttpContextAccessor?.HttpContext != null && HttpContextAccessor?.HttpContext.Request != null && HttpContextAccessor?.HttpContext.Request.Headers != null)
            {
                var headers = new Dictionary<string, string>();
                foreach (var k in HttpContextAccessor?.HttpContext.Request.Headers)
                {
                    headers[k.Key] = string.Join(", ", k.Value);
                }
                return headers;
            }
            return null;
        }
    }

    public static class FileStorageServiceExtention
    {
        public static DIHelper AddFileStorageService(this DIHelper services)
        {
            services.TryAddScoped<FileStorageService>();
            //services.TryAddScoped<IFileStorageService, FileStorageService>();
            return services
                .AddGlobalService()
                .AddGlobalStoreService()
                .AddGlobalFolderHelperService()
                .AddAuthContextService()
                .AddUserManagerService()
                .AddFoldersWrapperService()
                .AddFilesWrapperService()
                .AddFilesLinkUtilityService()
                .AddBaseCommonLinkUtilityService()
                .AddCoreBaseSettingsService()
                .AddCustomNamingPeopleService()
                .AddDisplayUserSettingsService()
                .AddPathProviderService()
                .AddDaoFactoryService()
                .AddFileMarkerService()
                .AddFilesSettingsHelperService()
                .AddFileUtilityService()
                .AddFileSecurityService()
                .AddFilesMessageService()
                .AddFileShareLinkService()
                .AddDocumentServiceConnectorService()
                .AddDocuSignLoginProviderService()
                .AddEntryManagerService()
                .AddDocumentServiceHelperService()
                .AddThirdpartyConfigurationService()
                .AddUrlShortener()
                .AddDocuSignHelperService()
                .AddDocuSignTokenService()
                .AddFileConverterService()
                .AddNotifyClientService()
                .AddFileSharingService()
                .AddDocumentServiceTrackerHelperService()
                .AddSocketManagerService()
                .AddFileOperationsManagerHelperService()
                .AddFileSharingAceHelperService();
            ;
        }
    }

    public class FileModel<T>
    {
        public T ParentId { get; set; }
        public string Title { get; set; }
    }
}