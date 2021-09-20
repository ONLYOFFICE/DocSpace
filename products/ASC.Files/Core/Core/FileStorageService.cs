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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text.Json;
using System.Web;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common;
using ASC.Core.Common.Configuration;
using ASC.Core.Common.Settings;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.ElasticSearch;
using ASC.FederatedLogin.LoginProviders;
using ASC.Files.Core;
using ASC.Files.Core.Resources;
using ASC.Files.Core.Security;
using ASC.Files.Core.Services.NotifyService;
using ASC.MessagingSystem;
using ASC.Web.Core.Files;
using ASC.Web.Core.PublicResources;
using ASC.Web.Core.Users;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Core.Entries;
using ASC.Web.Files.Helpers;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.Services.WCFService.FileOperations;
using ASC.Web.Files.ThirdPartyApp;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

using FileShare = ASC.Files.Core.Security.FileShare;
using UrlShortener = ASC.Web.Core.Utility.UrlShortener;

namespace ASC.Web.Files.Services.WCFService
{
    [Scope]
    public class FileStorageService<T> //: IFileStorageService
    {
        private static readonly FileEntrySerializer serializer = new FileEntrySerializer();
        private Global Global { get; }
        private GlobalStore GlobalStore { get; }
        private GlobalFolderHelper GlobalFolderHelper { get; }
        private FilesSettingsHelper FilesSettingsHelper { get; }
        private AuthContext AuthContext { get; }
        private UserManager UserManager { get; }
        private FileUtility FileUtility { get; }
        private FilesLinkUtility FilesLinkUtility { get; }
        private BaseCommonLinkUtility BaseCommonLinkUtility { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private CustomNamingPeople CustomNamingPeople { get; }
        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }
        private PathProvider PathProvider { get; }
        private FileSecurity FileSecurity { get; }
        private SocketManager SocketManager { get; }
        private IDaoFactory DaoFactory { get; }
        private FileMarker FileMarker { get; }
        private EntryManager EntryManager { get; }
        private FilesMessageService FilesMessageService { get; }
        private DocumentServiceTrackerHelper DocumentServiceTrackerHelper { get; }
        private DocuSignToken DocuSignToken { get; }
        private DocuSignHelper DocuSignHelper { get; }
        private FileShareLink FileShareLink { get; }
        private FileConverter FileConverter { get; }
        private DocumentServiceHelper DocumentServiceHelper { get; }
        private ThirdpartyConfiguration ThirdpartyConfiguration { get; }
        private DocumentServiceConnector DocumentServiceConnector { get; }
        private FileSharing FileSharing { get; }
        private NotifyClient NotifyClient { get; }
        private UrlShortener UrlShortener { get; }
        private IServiceProvider ServiceProvider { get; }
        private FileSharingAceHelper<T> FileSharingAceHelper { get; }
        private ConsumerFactory ConsumerFactory { get; }
        private EncryptionKeyPairHelper EncryptionKeyPairHelper { get; }
        private SettingsManager SettingsManager { get; }
        private FileOperationsManager FileOperationsManager { get; }
        private TenantManager TenantManager { get; }
        private FileTrackerHelper FileTracker { get; }
        private ICacheNotify<ThumbnailRequest> ThumbnailNotify { get; }
        private EntryStatusManager EntryStatusManager { get; }
        private ILog Logger { get; set; }

        public FileStorageService(
            Global global,
            GlobalStore globalStore,
            GlobalFolderHelper globalFolderHelper,
            FilesSettingsHelper filesSettingsHelper,
            AuthContext authContext,
            UserManager userManager,
            FileUtility fileUtility,
            FilesLinkUtility filesLinkUtility,
            BaseCommonLinkUtility baseCommonLinkUtility,
            CoreBaseSettings coreBaseSettings,
            CustomNamingPeople customNamingPeople,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            IHttpContextAccessor httpContextAccessor,
            IOptionsMonitor<ILog> optionMonitor,
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
            UrlShortener urlShortener,
            IServiceProvider serviceProvider,
            FileSharingAceHelper<T> fileSharingAceHelper,
            ConsumerFactory consumerFactory,
            EncryptionKeyPairHelper encryptionKeyPairHelper,
            SettingsManager settingsManager,
            FileOperationsManager fileOperationsManager,
            TenantManager tenantManager,
            FileTrackerHelper fileTracker,
            ICacheNotify<ThumbnailRequest> thumbnailNotify,
            EntryStatusManager entryStatusManager)
        {
            Global = global;
            GlobalStore = globalStore;
            GlobalFolderHelper = globalFolderHelper;
            FilesSettingsHelper = filesSettingsHelper;
            AuthContext = authContext;
            UserManager = userManager;
            FileUtility = fileUtility;
            FilesLinkUtility = filesLinkUtility;
            BaseCommonLinkUtility = baseCommonLinkUtility;
            CoreBaseSettings = coreBaseSettings;
            CustomNamingPeople = customNamingPeople;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            HttpContextAccessor = httpContextAccessor;
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
            UrlShortener = urlShortener;
            ServiceProvider = serviceProvider;
            FileSharingAceHelper = fileSharingAceHelper;
            ConsumerFactory = consumerFactory;
            EncryptionKeyPairHelper = encryptionKeyPairHelper;
            SettingsManager = settingsManager;
            Logger = optionMonitor.Get("ASC.Files");
            FileOperationsManager = fileOperationsManager;
            TenantManager = tenantManager;
            FileTracker = fileTracker;
            ThumbnailNotify = thumbnailNotify;
            EntryStatusManager = entryStatusManager;
        }

        public Folder<T> GetFolder(T folderId)
        {
            var folderDao = GetFolderDao();
            var folder = folderDao.GetFolder(folderId);

            ErrorIf(folder == null, FilesCommonResource.ErrorMassage_FolderNotFound);
            ErrorIf(!FileSecurity.CanRead(folder), FilesCommonResource.ErrorMassage_SecurityException_ReadFolder);

            return folder;
        }

        public List<FileEntry> GetFolders(T parentId)
        {
            var folderDao = GetFolderDao();

            try
            {
                var folders = EntryManager.GetEntries(folderDao.GetFolder(parentId), 0, 0, FilterType.FoldersOnly, false, Guid.Empty, string.Empty, false, false, new OrderBy(SortedByType.AZ, true), out var total);
                return new List<FileEntry>(folders);
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }
        }

        public List<object> GetPath(T folderId)
        {
            var folderDao = GetFolderDao();
            var folder = folderDao.GetFolder(folderId);

            ErrorIf(folder == null, FilesCommonResource.ErrorMassage_FolderNotFound);
            ErrorIf(!FileSecurity.CanRead(folder), FilesCommonResource.ErrorMassage_SecurityException_ViewFolder);

            return new List<object>(EntryManager.GetBreadCrumbs(folderId, folderDao).Select(f =>
            {
                if (f is Folder<string> f1) return (object)f1.ID;
                if (f is Folder<int> f2) return f2.ID;
                return 0;
            }));
        }

        public DataWrapper<T> GetFolderItems(T parentId, int from, int count, FilterType filter, bool subjectGroup, string ssubject, string searchText, bool searchInContent, bool withSubfolders, OrderBy orderBy)
        {
            var subjectId = string.IsNullOrEmpty(ssubject) ? Guid.Empty : new Guid(ssubject);

            var folderDao = GetFolderDao();
            var fileDao = GetFileDao();

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
            ErrorIf(!FileSecurity.CanRead(parent), FilesCommonResource.ErrorMassage_SecurityException_ViewFolder);
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
                if (prevVisible is Folder<string> f1) parent.FolderID = (T)Convert.ChangeType(f1.ID, typeof(T));
                if (prevVisible is Folder<int> f2) parent.FolderID = (T)Convert.ChangeType(f2.ID, typeof(T));
            }

            parent.Shareable = FileSharing.CanSetAccess(parent)
                || parent.FolderType == FolderType.SHARE
                || parent.RootFolderType == FolderType.Privacy;

            entries = entries.Where(x => x.FileEntryType == FileEntryType.Folder ||
            (x is File<string> f1 && !FileConverter.IsConverting(f1) ||
             x is File<int> f2 && !FileConverter.IsConverting(f2)));

            var result = new DataWrapper<T>
            {
                Total = total,
                Entries = new List<FileEntry>(entries.ToList()),
                FolderPathParts = new List<object>(breadCrumbs.Select(f =>
                {
                    if (f is Folder<string> f1) return (object)f1.ID;
                    if (f is Folder<int> f2) return f2.ID;
                    return 0;
                })),
                FolderInfo = parent,
                New = FileMarker.GetRootFoldersIdMarkedAsNew(parentId)
            };

            return result;
        }

        public object GetFolderItemsXml(T parentId, int from, int count, FilterType filter, bool subjectGroup, string subjectID, string search, bool searchInContent, bool withSubfolders, OrderBy orderBy)
        {
            var folderItems = GetFolderItems(parentId, from, count, filter, subjectGroup, subjectID, search, searchInContent, withSubfolders, orderBy);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(serializer.ToXml(folderItems))
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
            return response;
        }

        public List<FileEntry> GetItems<TId>(IEnumerable<TId> filesId, IEnumerable<TId> foldersId, FilterType filter, bool subjectGroup, string subjectID, string search)
        {
            var subjectId = string.IsNullOrEmpty(subjectID) ? Guid.Empty : new Guid(subjectID);

            var entries = Enumerable.Empty<FileEntry<TId>>();

            var folderDao = DaoFactory.GetFolderDao<TId>();
            var fileDao = DaoFactory.GetFileDao<TId>();
            var folders = folderDao.GetFolders(foldersId);
            folders = FileSecurity.FilterRead(folders).ToList();
            entries = entries.Concat(folders);

            var files = fileDao.GetFiles(filesId);
            files = FileSecurity.FilterRead(files).ToList();
            entries = entries.Concat(files);

            entries = EntryManager.FilterEntries(entries, filter, subjectGroup, subjectId, search, true);

            foreach (var fileEntry in entries)
            {
                if (fileEntry is File<TId> file)
                {
                    if (fileEntry.RootFolderType == FolderType.USER
                        && !Equals(fileEntry.RootFolderCreator, AuthContext.CurrentAccount.ID)
                        && !FileSecurity.CanRead(folderDao.GetFolder(file.FolderIdDisplay)))
                    {
                        file.FolderIdDisplay = GlobalFolderHelper.GetFolderShare<TId>();
                    }
                }
                else if (fileEntry is Folder<TId> folder)
                {
                    if (fileEntry.RootFolderType == FolderType.USER
                        && !Equals(fileEntry.RootFolderCreator, AuthContext.CurrentAccount.ID)
                        && !FileSecurity.CanRead(folderDao.GetFolder(folder.FolderIdDisplay)))
                    {
                        folder.FolderIdDisplay = GlobalFolderHelper.GetFolderShare<TId>();
                    }
                }
            }

            EntryStatusManager.SetFileStatus(entries);

            return new List<FileEntry>(entries);
        }

        public Folder<T> CreateNewFolder(T parentId, string title)
        {
            if (string.IsNullOrEmpty(title) || parentId == null) throw new ArgumentException();

            var folderDao = GetFolderDao();
            var parent = folderDao.GetFolder(parentId);
            ErrorIf(parent == null, FilesCommonResource.ErrorMassage_FolderNotFound);
            ErrorIf(!FileSecurity.CanCreate(parent), FilesCommonResource.ErrorMassage_SecurityException_Create);

            try
            {
                var newFolder = ServiceProvider.GetService<Folder<T>>();
                newFolder.Title = title;
                newFolder.FolderID = parent.ID;

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

        public Folder<T> FolderRename(T folderId, string title)
        {
            var tagDao = GetTagDao();
            var folderDao = GetFolderDao();
            var folder = folderDao.GetFolder(folderId);
            ErrorIf(folder == null, FilesCommonResource.ErrorMassage_FolderNotFound);
            ErrorIf(!FileSecurity.CanEdit(folder), FilesCommonResource.ErrorMassage_SecurityException_RenameFolder);
            if (!FileSecurity.CanDelete(folder) && UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_RenameFolder);
            ErrorIf(folder.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_ViewTrashItem);

            var folderAccess = folder.Access;

            if (string.Compare(folder.Title, title, false) != 0)
            {
                var newFolderID = folderDao.RenameFolder(folder, title);
                folder = folderDao.GetFolder(newFolderID);
                folder.Access = folderAccess;

                FilesMessageService.Send(folder, GetHttpHeaders(), MessageAction.FolderRenamed, folder.Title);

                //if (!folder.ProviderEntry)
                //{
                //    FoldersIndexer.IndexAsync(FoldersWrapper.GetFolderWrapper(ServiceProvider, folder));
                //}
            }

            var tag = tagDao.GetNewTags(AuthContext.CurrentAccount.ID, folder).FirstOrDefault();
            if (tag != null)
            {
                folder.NewForMe = tag.Count;
            }

            if (folder.RootFolderType == FolderType.USER
                && !Equals(folder.RootFolderCreator, AuthContext.CurrentAccount.ID)
                && !FileSecurity.CanRead(folderDao.GetFolder(folder.FolderID)))
            {
                folder.FolderIdDisplay = GlobalFolderHelper.GetFolderShare<T>();
            }

            return folder;
        }

        public File<T> GetFile(T fileId, int version)
        {
            var fileDao = GetFileDao();
            fileDao.InvalidateCache(fileId);

            var file = version > 0
                           ? fileDao.GetFile(fileId, version)
                           : fileDao.GetFile(fileId);
            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
            ErrorIf(!FileSecurity.CanRead(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

            EntryStatusManager.SetFileStatus(file);

            if (file.RootFolderType == FolderType.USER
                && !Equals(file.RootFolderCreator, AuthContext.CurrentAccount.ID))
            {
                var folderDao = GetFolderDao();
                if (!FileSecurity.CanRead(folderDao.GetFolder(file.FolderID)))
                {
                    file.FolderIdDisplay = GlobalFolderHelper.GetFolderShare<T>();
                }
            }

            return file;
        }

        public List<File<T>> GetSiblingsFile(T fileId, T parentId, FilterType filter, bool subjectGroup, string subjectID, string search, bool searchInContent, bool withSubfolders, OrderBy orderBy)
        {
            var subjectId = string.IsNullOrEmpty(subjectID) ? Guid.Empty : new Guid(subjectID);

            var fileDao = GetFileDao();
            var folderDao = GetFolderDao();

            var file = fileDao.GetFile(fileId);
            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
            ErrorIf(!FileSecurity.CanRead(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

            var parent = folderDao.GetFolder(EqualityComparer<T>.Default.Equals(parentId, default(T)) ? file.FolderID : parentId);
            ErrorIf(parent == null, FilesCommonResource.ErrorMassage_FolderNotFound);
            ErrorIf(parent.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_ViewTrashItem);

            if (filter == FilterType.FoldersOnly)
            {
                return new List<File<T>>();
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

            if (!FileSecurity.CanRead(parent))
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

            return new List<File<T>>(result);
        }

        public File<T> CreateNewFile(FileModel<T> fileWrapper, bool enableExternalExt = false)
        {
            if (string.IsNullOrEmpty(fileWrapper.Title) || fileWrapper.ParentId == null) throw new ArgumentException();

            var fileDao = GetFileDao();
            var folderDao = GetFolderDao();

            Folder<T> folder = null;
            if (!EqualityComparer<T>.Default.Equals(fileWrapper.ParentId, default(T)))
            {
                folder = folderDao.GetFolder(fileWrapper.ParentId);

                if (!FileSecurity.CanCreate(folder))
                {
                    folder = null;
                }
            }
            if (folder == null)
            {
                folder = folderDao.GetFolder(GlobalFolderHelper.GetFolderMy<T>());
            }


            var file = ServiceProvider.GetService<File<T>>();
            file.FolderID = folder.ID;
            file.Comment = FilesCommonResource.CommentCreate;

            if (string.IsNullOrEmpty(fileWrapper.Title))
            {
                fileWrapper.Title = UserControlsCommonResource.NewDocument + ".docx";
            }

            var externalExt = false;

            var fileExt = !enableExternalExt ? FileUtility.GetInternalExtension(fileWrapper.Title) : FileUtility.GetFileExtension(fileWrapper.Title);
            if (!FileUtility.InternalExtension.Values.Contains(fileExt))
            {
                if (!enableExternalExt)
                {
                    fileExt = FileUtility.InternalExtension[FileType.Document];
                    file.Title = fileWrapper.Title + fileExt;
                }
                else
                {
                    externalExt = true;
                    file.Title = fileWrapper.Title;
                }
            }
            else
            {
                file.Title = FileUtility.ReplaceFileExtension(fileWrapper.Title, fileExt);
            }

            if (EqualityComparer<T>.Default.Equals(fileWrapper.TemplateId, default(T)))
            {
                var culture = UserManager.GetUsers(AuthContext.CurrentAccount.ID).GetCulture();
                var storeTemplate = GetStoreTemplate();

                var path = FileConstant.NewDocPath + culture + "/";
                if (!storeTemplate.IsDirectory(path))
                {
                    path = FileConstant.NewDocPath + "en-US/";
                }

                try
                {
                    if (!externalExt)
                    {
                        var pathNew = path + "new" + fileExt;
                        using (var stream = storeTemplate.GetReadStream("", pathNew))
                        {
                            file.ContentLength = stream.CanSeek ? stream.Length : storeTemplate.GetFileSize(pathNew);
                            file = fileDao.SaveFile(file, stream);
                        }
                    }
                    else
                    {
                        file = fileDao.SaveFile(file, null);
                    }

                    var pathThumb = path + fileExt.Trim('.') + "." + Global.ThumbnailExtension;
                    if (storeTemplate.IsFile("", pathThumb))
                    {
                        using (var streamThumb = storeTemplate.GetReadStream("", pathThumb))
                        {
                            fileDao.SaveThumbnail(file, streamThumb);
                        }
                        file.ThumbnailStatus = Thumbnail.Created;
                    }
                }
                catch (Exception e)
                {
                    throw GenerateException(e);
                }
            }
            else
            {
                var template = fileDao.GetFile(fileWrapper.TemplateId);
                ErrorIf(template == null, FilesCommonResource.ErrorMassage_FileNotFound);
                ErrorIf(!FileSecurity.CanRead(template), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

                try
                {
                    using (var stream = fileDao.GetFileStream(template))
                    {
                        file.ContentLength = template.ContentLength;
                        file = fileDao.SaveFile(file, stream);
                    }

                    if (template.ThumbnailStatus == Thumbnail.Created)
                    {
                        using (var thumb = fileDao.GetThumbnail(template))
                        {
                            fileDao.SaveThumbnail(file, thumb);
                        }
                        file.ThumbnailStatus = Thumbnail.Created;
                    }
                }
                catch (Exception e)
                {
                    throw GenerateException(e);
                }
            }

            FilesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileCreated, file.Title);

            FileMarker.MarkAsNew(file);

            return file;
        }

        public KeyValuePair<bool, string> TrackEditFile(T fileId, Guid tabId, string docKeyForTrack, string doc = null, bool isFinish = false)
        {
            try
            {
                var id = FileShareLink.Parse<T>(doc);
                if (id == null)
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

        public Dictionary<string, string> CheckEditing(List<T> filesId)
        {
            ErrorIf(!AuthContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);
            var result = new Dictionary<string, string>();

            var fileDao = GetFileDao();
            var ids = filesId.Where(FileTracker.IsEditing).Select(id => id).ToList();

            foreach (var file in fileDao.GetFiles(ids))
            {
                if (file == null
                    || !FileSecurity.CanEdit(file)
                    && !FileSecurity.CanCustomFilterEdit(file)
                    && !FileSecurity.CanReview(file)
                    && !FileSecurity.CanFillForms(file)
                    && !FileSecurity.CanComment(file)) continue;

                var usersId = FileTracker.GetEditingBy(file.ID);
                var value = string.Join(", ", usersId.Select(userId => Global.GetUserName(userId, true)).ToArray());
                result[file.ID.ToString()] = value;
            }

            return result;
        }

        public File<T> SaveEditing(T fileId, string fileExtension, string fileuri, Stream stream, string doc = null, bool forcesave = false)
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

        public File<T> UpdateFileStream(T fileId, Stream stream, bool encrypted, bool forcesave)
        {
            try
            {
                if (!forcesave && FileTracker.IsEditing(fileId))
                {
                    FileTracker.Remove(fileId);
                }

                var file = EntryManager.SaveEditing(fileId,
                    null,
                    null,
                    stream,
                    null,
                    encrypted ? FilesCommonResource.CommentEncrypted : null,
                    encrypted: encrypted,
                    forcesave: forcesave ? ForcesaveType.User : ForcesaveType.None);

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

        public string StartEdit(T fileId, bool editingAlone = false, string doc = null)
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
                    DocumentServiceHelper.GetParams(file, true, editable ? FileShare.ReadWrite : FileShare.Read, false, editable, editable, editable, false, out configuration);
                }

                ErrorIf(!configuration.EditorConfig.ModeWrite
                        || !(configuration.Document.Permissions.Edit
                             || configuration.Document.Permissions.ModifyFilter
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

        public File<T> FileRename(T fileId, string title)
        {
            try
            {
                var renamed = EntryManager.FileRename(fileId, title, out var file);
                if (renamed)
                {
                    FilesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileRenamed, file.Title);

                    //if (!file.ProviderEntry)
                    //{
                    //    FilesIndexer.UpdateAsync(FilesWrapper.GetFilesWrapper(ServiceProvider, file), true, r => r.Title);
                    //}
                }

                if (file.RootFolderType == FolderType.USER
                    && !Equals(file.RootFolderCreator, AuthContext.CurrentAccount.ID))
                {
                    var folderDao = GetFolderDao();
                    if (!FileSecurity.CanRead(folderDao.GetFolder(file.FolderID)))
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

        public List<File<T>> GetFileHistory(T fileId)
        {
            var fileDao = GetFileDao();
            var file = fileDao.GetFile(fileId);
            ErrorIf(!FileSecurity.CanRead(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

            return new List<File<T>>(fileDao.GetFileHistory(fileId));
        }

        public KeyValuePair<File<T>, List<File<T>>> UpdateToVersion(T fileId, int version)
        {
            var file = EntryManager.UpdateToVersionFile(fileId, version);
            FilesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileRestoreVersion, file.Title, version.ToString(CultureInfo.InvariantCulture));

            if (file.RootFolderType == FolderType.USER
                && !Equals(file.RootFolderCreator, AuthContext.CurrentAccount.ID))
            {
                var folderDao = GetFolderDao();
                if (!FileSecurity.CanRead(folderDao.GetFolder(file.FolderID)))
                {
                    file.FolderIdDisplay = GlobalFolderHelper.GetFolderShare<T>();
                }
            }

            return new KeyValuePair<File<T>, List<File<T>>>(file, GetFileHistory(fileId));
        }

        public string UpdateComment(T fileId, int version, string comment)
        {
            var fileDao = GetFileDao();
            var file = fileDao.GetFile(fileId, version);
            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
            ErrorIf(!FileSecurity.CanEdit(file) || UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager), FilesCommonResource.ErrorMassage_SecurityException_EditFile);
            ErrorIf(EntryManager.FileLockedForMe(file.ID), FilesCommonResource.ErrorMassage_LockedFile);
            ErrorIf(file.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_ViewTrashItem);

            comment = fileDao.UpdateComment(fileId, version, comment);

            FilesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileUpdatedRevisionComment, file.Title, version.ToString(CultureInfo.InvariantCulture));

            return comment;
        }

        public KeyValuePair<File<T>, List<File<T>>> CompleteVersion(T fileId, int version, bool continueVersion)
        {
            var file = EntryManager.CompleteVersionFile(fileId, version, continueVersion);

            FilesMessageService.Send(file, GetHttpHeaders(),
                                     continueVersion ? MessageAction.FileDeletedVersion : MessageAction.FileCreatedVersion,
                                     file.Title, version == 0 ? (file.Version - 1).ToString(CultureInfo.InvariantCulture) : version.ToString(CultureInfo.InvariantCulture));

            if (file.RootFolderType == FolderType.USER
                && !Equals(file.RootFolderCreator, AuthContext.CurrentAccount.ID))
            {
                var folderDao = GetFolderDao();
                if (!FileSecurity.CanRead(folderDao.GetFolder(file.FolderID)))
                {
                    file.FolderIdDisplay = GlobalFolderHelper.GetFolderShare<T>();
                }
            }

            return new KeyValuePair<File<T>, List<File<T>>>(file, GetFileHistory(fileId));
        }

        public File<T> LockFile(T fileId, bool lockfile)
        {
            var tagDao = GetTagDao();
            var fileDao = GetFileDao();
            var file = fileDao.GetFile(fileId);

            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
            ErrorIf(!FileSecurity.CanEdit(file) || lockfile && UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager), FilesCommonResource.ErrorMassage_SecurityException_EditFile);
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
                    tagLocked = new Tag("locked", TagType.Locked, AuthContext.CurrentAccount.ID, 0).AddEntry(file);

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
                    UpdateComment(file.ID, file.Version, FilesCommonResource.UnlockComment);
                }
            }

            EntryStatusManager.SetFileStatus(file);

            if (file.RootFolderType == FolderType.USER
                && !Equals(file.RootFolderCreator, AuthContext.CurrentAccount.ID))
            {
                var folderDao = GetFolderDao();
                if (!FileSecurity.CanRead(folderDao.GetFolder(file.FolderID)))
                {
                    file.FolderIdDisplay = GlobalFolderHelper.GetFolderShare<T>();
                }
            }

            return file;
        }

        public List<EditHistory> GetEditHistory(T fileId, string doc = null)
        {
            var fileDao = GetFileDao();
            var readLink = FileShareLink.Check(doc, true, fileDao, out var file);
            if (file == null)
                file = fileDao.GetFile(fileId);

            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
            ErrorIf(!readLink && !FileSecurity.CanRead(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
            ErrorIf(file.ProviderEntry, FilesCommonResource.ErrorMassage_BadRequest);

            return new List<EditHistory>(fileDao.GetEditHistory(DocumentServiceHelper, file.ID));
        }

        public EditHistoryData GetEditDiffUrl(T fileId, int version = 0, string doc = null)
        {
            var fileDao = GetFileDao();
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
            ErrorIf(!readLink && !FileSecurity.CanRead(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
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
                        path = FileConstant.NewDocPath + "en-US/";
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

        public List<EditHistory> RestoreVersion(T fileId, int version, string url = null, string doc = null)
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
                fileDao = GetFileDao();
                var fromFile = fileDao.GetFile(fileId, version);
                modifiedOnString = fromFile.ModifiedOnString;
                file = EntryManager.SaveEditing(fileId, null, url, null, doc, string.Format(FilesCommonResource.CommentRevertChanges, modifiedOnString));
            }

            FilesMessageService.Send(file, HttpContextAccessor?.HttpContext?.Request?.Headers, MessageAction.FileRestoreVersion, file.Title, version.ToString(CultureInfo.InvariantCulture));

            fileDao = GetFileDao();
            return new List<EditHistory>(fileDao.GetEditHistory(DocumentServiceHelper, file.ID));
        }

        public Web.Core.Files.DocumentService.FileLink GetPresignedUri(T fileId)
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

        public List<FileEntry> GetNewItems(T folderId)
        {
            try
            {
                Folder<T> folder;
                var folderDao = GetFolderDao();
                folder = folderDao.GetFolder(folderId);

                var result = FileMarker.MarkedItems(folder);

                result = new List<FileEntry>(EntryManager.SortEntries<T>(result, new OrderBy(SortedByType.DateAndTime, false)));

                if (!result.Any())
                {
                    MarkAsRead(new List<JsonElement>() { JsonDocument.Parse(JsonSerializer.Serialize(folderId)).RootElement }, new List<JsonElement>() { }); //TODO
                }


                return result;
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }
        }

        public List<FileOperationResult> MarkAsRead(IEnumerable<JsonElement> foldersId, IEnumerable<JsonElement> filesId)
        {
            if (!foldersId.Any() && !filesId.Any()) return GetTasksStatuses();
            return FileOperationsManager.MarkAsRead(AuthContext.CurrentAccount.ID, TenantManager.GetCurrentTenant(), foldersId, filesId);
        }

        public List<ThirdPartyParams> GetThirdParty()
        {
            var providerDao = GetProviderDao();
            if (providerDao == null) return new List<ThirdPartyParams>();

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
            return new List<ThirdPartyParams>(resultList.ToList());
        }

        public List<FileEntry> GetThirdPartyFolder(int folderType = 0)
        {
            if (!FilesSettingsHelper.EnableThirdParty) return new List<FileEntry>();

            var providerDao = GetProviderDao();
            if (providerDao == null) return new List<FileEntry>();

            var providersInfo = providerDao.GetProvidersInfo((FolderType)folderType);

            var folders = providersInfo.Select(providerInfo =>
                {
                    var folder = EntryManager.GetFakeThirdpartyFolder<T>(providerInfo);
                    folder.NewForMe = folder.RootFolderType == FolderType.COMMON ? 1 : 0;
                    return folder;
                });

            return new List<FileEntry>(folders);
        }

        public Folder<T> SaveThirdParty(ThirdPartyParams thirdPartyParams)
        {
            var folderDaoInt = DaoFactory.GetFolderDao<int>();
            var folderDao = GetFolderDao();
            var providerDao = GetProviderDao();

            if (providerDao == null) return null;

            ErrorIf(thirdPartyParams == null, FilesCommonResource.ErrorMassage_BadRequest);
            var parentFolder = folderDaoInt.GetFolder(thirdPartyParams.Corporate && !CoreBaseSettings.Personal ? GlobalFolderHelper.FolderCommon : GlobalFolderHelper.FolderMy);
            ErrorIf(!FileSecurity.CanCreate(parentFolder), FilesCommonResource.ErrorMassage_SecurityException_Create);
            ErrorIf(!FilesSettingsHelper.EnableThirdParty, FilesCommonResource.ErrorMassage_SecurityException_Create);

            var lostFolderType = FolderType.USER;
            var folderType = thirdPartyParams.Corporate ? FolderType.COMMON : FolderType.USER;

            int curProviderId;

            MessageAction messageAction;
            if (string.IsNullOrEmpty(thirdPartyParams.ProviderId))
            {
                ErrorIf(!ThirdpartyConfiguration.SupportInclusion(DaoFactory)
                        ||
                        (!FilesSettingsHelper.EnableThirdParty
                         && !CoreBaseSettings.Personal)
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
                    throw GenerateException(e.InnerException ?? e);
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
                    var lostFolder = folderDao.GetFolder((T)Convert.ChangeType(lostProvider.RootFolderId, typeof(T)));
                    FileMarker.RemoveMarkAsNewForAll(lostFolder);
                }

                curProviderId = providerDao.UpdateProviderInfo(curProviderId, thirdPartyParams.CustomerTitle, thirdPartyParams.AuthData, folderType);
                messageAction = MessageAction.ThirdPartyUpdated;
            }

            var provider = providerDao.GetProviderInfo(curProviderId);
            provider.InvalidateStorage();

            var folderDao1 = GetFolderDao();
            var folder = folderDao1.GetFolder((T)Convert.ChangeType(provider.RootFolderId, typeof(T)));
            ErrorIf(!FileSecurity.CanRead(folder), FilesCommonResource.ErrorMassage_SecurityException_ViewFolder);

            FilesMessageService.Send(parentFolder, GetHttpHeaders(), messageAction, folder.ID.ToString(), provider.ProviderKey);

            if (thirdPartyParams.Corporate && lostFolderType != FolderType.COMMON)
            {
                FileMarker.MarkAsNew(folder);
            }

            return folder;
        }

        public object DeleteThirdParty(string providerId)
        {
            var providerDao = GetProviderDao();
            if (providerDao == null) return null;

            var curProviderId = Convert.ToInt32(providerId);
            var providerInfo = providerDao.GetProviderInfo(curProviderId);

            var folder = EntryManager.GetFakeThirdpartyFolder<T>(providerInfo);
            ErrorIf(!FileSecurity.CanDelete(folder), FilesCommonResource.ErrorMassage_SecurityException_DeleteFolder);

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
                    || !FilesSettingsHelper.EnableThirdParty
                    || !ThirdpartyConfiguration.SupportDocuSignInclusion, FilesCommonResource.ErrorMassage_SecurityException_Create);

            var token = ConsumerFactory.Get<DocuSignLoginProvider>().GetAccessToken(code);
            DocuSignHelper.ValidateToken(token);
            DocuSignToken.SaveToken(token);
            return true;
        }

        public object DeleteDocuSign()
        {
            DocuSignToken.DeleteToken();
            return null;
        }

        public string SendDocuSign(T fileId, DocuSignData docuSignData)
        {
            try
            {
                ErrorIf(UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)
                        || !FilesSettingsHelper.EnableThirdParty
                        || !ThirdpartyConfiguration.SupportDocuSignInclusion, FilesCommonResource.ErrorMassage_SecurityException_Create);

                return DocuSignHelper.SendDocuSign(fileId, docuSignData, GetHttpHeaders());
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }
        }

        public List<FileOperationResult> GetTasksStatuses()
        {
            ErrorIf(!AuthContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);

            return FileOperationsManager.GetOperationResults(AuthContext.CurrentAccount.ID);
        }

        public List<FileOperationResult> TerminateTasks()
        {
            ErrorIf(!AuthContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);

            return FileOperationsManager.CancelOperations(AuthContext.CurrentAccount.ID);
        }

        public List<FileOperationResult> BulkDownload(Dictionary<JsonElement, string> folders, Dictionary<JsonElement, string> files)
        {
            ErrorIf(!folders.Any() && !files.Any(), FilesCommonResource.ErrorMassage_BadRequest);

            return FileOperationsManager.Download(AuthContext.CurrentAccount.ID, TenantManager.GetCurrentTenant(), folders, files, GetHttpHeaders());
        }


        public (List<object>, List<object>) MoveOrCopyFilesCheck<T1>(IEnumerable<JsonElement> filesId, IEnumerable<JsonElement> foldersId, T1 destFolderId)
        {
            var checkedFiles = new List<object>();
            var checkedFolders = new List<object>();

            var (filesInts, folderInts) = MoveOrCopyFilesCheck(
                filesId.Where(r => r.ValueKind == JsonValueKind.Number).Select(r => r.GetInt32()),
                foldersId.Where(r => r.ValueKind == JsonValueKind.Number).Select(r => r.GetInt32()),
                destFolderId);

            foreach (var i in filesInts)
            {
                checkedFiles.Add(i);
            }

            foreach (var i in folderInts)
            {
                checkedFolders.Add(i);
            }

            var (filesStrings, folderStrings) = MoveOrCopyFilesCheck(
                filesId.Where(r => r.ValueKind == JsonValueKind.String).Select(r => r.GetString()),
                foldersId.Where(r => r.ValueKind == JsonValueKind.String).Select(r => r.GetString()),
                destFolderId);

            foreach (var i in filesStrings)
            {
                checkedFiles.Add(i);
            }

            foreach (var i in folderStrings)
            {
                checkedFolders.Add(i);
            }

            return (checkedFiles, checkedFolders);
        }

        private (List<TFrom>, List<TFrom>) MoveOrCopyFilesCheck<TFrom, TTo>(IEnumerable<TFrom> filesId, IEnumerable<TFrom> foldersId, TTo destFolderId)
        {
            var checkedFiles = new List<TFrom>();
            var checkedFolders = new List<TFrom>();
            var folderDao = DaoFactory.GetFolderDao<TFrom>();
            var fileDao = DaoFactory.GetFileDao<TFrom>();
            var destFolderDao = DaoFactory.GetFolderDao<TTo>();
            var destFileDao = DaoFactory.GetFileDao<TTo>();

            var toFolder = destFolderDao.GetFolder(destFolderId);
            ErrorIf(toFolder == null, FilesCommonResource.ErrorMassage_FolderNotFound);
            ErrorIf(!FileSecurity.CanCreate(toFolder), FilesCommonResource.ErrorMassage_SecurityException_Create);

            foreach (var id in filesId)
            {
                var file = fileDao.GetFile(id);
                if (file != null
                    && !file.Encrypted
                    && destFileDao.IsExist(file.Title, toFolder.ID))
                {
                    checkedFiles.Add(id);
                }
            }

            var folders = folderDao.GetFolders(foldersId);
            var foldersProject = folders.Where(folder => folder.FolderType == FolderType.BUNCH).ToList();
            if (foldersProject.Any())
            {
                var toSubfolders = destFolderDao.GetFolders(toFolder.ID);

                foreach (var folderProject in foldersProject)
                {
                    var toSub = toSubfolders.FirstOrDefault(to => Equals(to.Title, folderProject.Title));
                    if (toSub == null) continue;

                    var filesPr = fileDao.GetFiles(folderProject.ID);
                    var foldersPr = folderDao.GetFolders(folderProject.ID).Select(d => d.ID);

                    var (cFiles, cFolders) = MoveOrCopyFilesCheck(filesPr, foldersPr, toSub.ID);
                    checkedFiles.AddRange(cFiles);
                    checkedFolders.AddRange(cFolders);
                }
            }
            try
            {
                foreach (var pair in folderDao.CanMoveOrCopy(foldersId.ToArray(), toFolder.ID))
                {
                    checkedFolders.Add(pair.Key);
                }
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }

            return (checkedFiles, checkedFolders);
        }

        public List<FileOperationResult> MoveOrCopyItems(IEnumerable<JsonElement> foldersId, IEnumerable<JsonElement> filesId, JsonElement destFolderId, FileConflictResolveType resolve, bool ic, bool deleteAfter = false)
        {
            List<FileOperationResult> result;
            if (foldersId.Any() || filesId.Any())
            {
                result = FileOperationsManager.MoveOrCopy(AuthContext.CurrentAccount.ID, TenantManager.GetCurrentTenant(), foldersId, filesId, destFolderId, ic, resolve, !deleteAfter, GetHttpHeaders());
            }
            else
            {
                result = FileOperationsManager.GetOperationResults(AuthContext.CurrentAccount.ID);
            }
            return result;
        }


        public List<FileOperationResult> DeleteFile(string action, T fileId, bool ignoreException = false, bool deleteAfter = false, bool immediately = false)
        {
            return FileOperationsManager.Delete(AuthContext.CurrentAccount.ID, TenantManager.GetCurrentTenant(), new List<T>(), new List<T>() { fileId }, ignoreException, !deleteAfter, immediately, GetHttpHeaders());
        }
        public List<FileOperationResult> DeleteFolder(string action, T folderId, bool ignoreException = false, bool deleteAfter = false, bool immediately = false)
        {
            return FileOperationsManager.Delete(AuthContext.CurrentAccount.ID, TenantManager.GetCurrentTenant(), new List<T>() { folderId }, new List<T>(), ignoreException, !deleteAfter, immediately, GetHttpHeaders());
        }

        public List<FileOperationResult> DeleteItems(string action, List<JsonElement> files, List<JsonElement> folders, bool ignoreException = false, bool deleteAfter = false, bool immediately = false)
        {
            return FileOperationsManager.Delete(AuthContext.CurrentAccount.ID, TenantManager.GetCurrentTenant(), folders, files, ignoreException, !deleteAfter, immediately, GetHttpHeaders());
        }

        public List<FileOperationResult> EmptyTrash()
        {
            var folderDao = GetFolderDao();
            var fileDao = GetFileDao();
            var trashId = folderDao.GetFolderIDTrash(true);
            var foldersId = folderDao.GetFolders(trashId).Select(f => f.ID).ToList();
            var filesId = fileDao.GetFiles(trashId).ToList();

            return FileOperationsManager.Delete(AuthContext.CurrentAccount.ID, TenantManager.GetCurrentTenant(), foldersId, filesId, false, true, false, GetHttpHeaders());
        }

        public List<FileOperationResult> CheckConversion(List<List<string>> filesInfoJSON, bool sync = false)
        {
            if (filesInfoJSON == null || filesInfoJSON.Count == 0) return new List<FileOperationResult>();

            var results = new List<FileOperationResult>();
            var fileDao = GetFileDao();
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

                ErrorIf(!FileSecurity.CanRead(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

                var startConvert = Convert.ToBoolean(fileInfo[2]);
                if (startConvert && FileConverter.MustConvert(file))
                {
                    try
                    {
                        if (sync)
                        {
                            results.Add(FileConverter.ExecSync(file, fileInfo.Count > 3 ? fileInfo[3] : null));
                        }
                        else
                        {
                            FileConverter.ExecAsync(file, false, fileInfo.Count > 3 ? fileInfo[3] : null);
                        }
                    }
                    catch (Exception e)
                    {
                        throw GenerateException(e);
                    }
                }

                files.Add(new KeyValuePair<File<T>, bool>(file, false));
            }

            if (sync)
            {
                return results;
            }
            else
            {
                return FileConverter.GetStatus(files).ToList();
            }
        }

        public void ReassignStorage(Guid userFromId, Guid userToId)
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

            var folderDao = GetFolderDao();
            var fileDao = GetFileDao();

            if (!userFrom.IsVisitor(UserManager))
            {
                var folderIdFromMy = folderDao.GetFolderIDUser(false, userFrom.ID);

                if (!Equals(folderIdFromMy, 0))
                {
                    //create folder with name userFrom in folder userTo
                    var folderIdToMy = folderDao.GetFolderIDUser(true, userTo.ID);
                    var newFolder = ServiceProvider.GetService<Folder<T>>();
                    newFolder.Title = string.Format(CustomNamingPeople.Substitute<FilesCommonResource>("TitleDeletedUserFolder"), userFrom.DisplayUserName(false, DisplayUserSettingsHelper));
                    newFolder.FolderID = folderIdToMy;

                    var newFolderTo = folderDao.SaveFolder(newFolder);

                    //move items from userFrom to userTo
                    EntryManager.MoveSharedItems(folderIdFromMy, newFolderTo, folderDao, fileDao);

                    EntryManager.ReassignItems(newFolderTo, userFrom.ID, userTo.ID, folderDao, fileDao);
                }
            }

            EntryManager.ReassignItems(GlobalFolderHelper.GetFolderCommon<T>(), userFrom.ID, userTo.ID, folderDao, fileDao);
        }

        public void DeleteStorage(Guid userId)
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

            var folderDao = GetFolderDao();
            var fileDao = GetFileDao();

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

            var rootFolders = folderDao.GetFolders(rootFoldersId);
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
        #region Favorites Manager

        public bool ToggleFileFavorite(T fileId, bool favorite)
        {
            if (favorite)
            {
                AddToFavorites(new List<T>(0), new List<T>(1) { fileId });
            }
            else
            {
                DeleteFavorites(new List<T>(0), new List<T>(1) { fileId });
            }
            return favorite;
        }

        public List<FileEntry<T>> AddToFavorites(IEnumerable<T> foldersId, IEnumerable<T> filesId)
        {
            if (UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);

            var tagDao = GetTagDao();
            var fileDao = GetFileDao();
            var folderDao = GetFolderDao();
            var entries = Enumerable.Empty<FileEntry<T>>();

            var files = fileDao.GetFiles(filesId).Where(file => !file.Encrypted);
            files = FileSecurity.FilterRead(files).ToList();
            entries = entries.Concat(files);

            var folders = folderDao.GetFolders(foldersId);
            folders = FileSecurity.FilterRead(folders).ToList();
            entries = entries.Concat(folders);

            var tags = entries.Select(entry => Tag.Favorite(AuthContext.CurrentAccount.ID, entry));

            tagDao.SaveTags(tags);

            return new List<FileEntry<T>>(entries);
        }

        public List<FileEntry<T>> DeleteFavorites(IEnumerable<T> foldersId, IEnumerable<T> filesId)
        {
            var tagDao = GetTagDao();
            var fileDao = GetFileDao();
            var folderDao = GetFolderDao();
            var entries = Enumerable.Empty<FileEntry<T>>();

            var files = fileDao.GetFiles(filesId);
            files = FileSecurity.FilterRead(files).ToList();
            entries = entries.Concat(files);

            var folders = folderDao.GetFolders(foldersId);
            folders = FileSecurity.FilterRead(folders).ToList();
            entries = entries.Concat(folders);

            var tags = entries.Select(entry => Tag.Favorite(AuthContext.CurrentAccount.ID, entry));

            tagDao.RemoveTags(tags);

            return new List<FileEntry<T>>(entries);
        }

        #endregion

        #region Templates Manager

        public List<FileEntry<T>> AddToTemplates(IEnumerable<T> filesId)
        {
            if (UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);

            var tagDao = GetTagDao();
            var fileDao = GetFileDao();
            var files = fileDao.GetFiles(filesId);

            files = FileSecurity.FilterRead(files)
                .Where(file => FileUtility.ExtsWebTemplate.Contains(FileUtility.GetFileExtension(file.Title), StringComparer.CurrentCultureIgnoreCase))
                .ToList();

            var tags = files.Select(file => Tag.Template(AuthContext.CurrentAccount.ID, file));

            tagDao.SaveTags(tags);

            return new List<FileEntry<T>>(files);
        }

        public List<FileEntry<T>> DeleteTemplates(IEnumerable<T> filesId)
        {
            var tagDao = GetTagDao();
            var fileDao = GetFileDao();
            var files = fileDao.GetFiles(filesId);

            files = FileSecurity.FilterRead(files).ToList();

            var tags = files.Select(file => Tag.Template(AuthContext.CurrentAccount.ID, file));

            tagDao.RemoveTags(tags);

            return new List<FileEntry<T>>(files);
        }

        public List<FileEntry<T>> GetTemplates(FilterType filter, int from, int count, bool subjectGroup, string subjectID, string search, bool searchInContent)
        {
            try
            {
                IEnumerable<File<T>> result;

                var subjectId = string.IsNullOrEmpty(subjectID) ? Guid.Empty : new Guid(subjectID);
                var folderDao = GetFolderDao();
                var fileDao = GetFileDao();
                result = EntryManager.GetTemplates(folderDao, fileDao, filter, subjectGroup, subjectId, search, searchInContent);

                if (result.Count() <= from)
                    return null;

                result = result.Skip(from).Take(count);
                return new List<FileEntry<T>>(result);
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }
        }

        #endregion

        public List<AceWrapper> GetSharedInfo(IEnumerable<T> fileIds, IEnumerable<T> folderIds)
        {
            return FileSharing.GetSharedInfo(fileIds, folderIds);
        }

        public List<AceShortWrapper> GetSharedInfoShortFile(T fileId)
        {
            return FileSharing.GetSharedInfoShortFile(fileId);
        }

        public List<AceShortWrapper> GetSharedInfoShortFolder(T folderId)
        {
            return FileSharing.GetSharedInfoShortFolder(folderId);
        }

        public List<T> SetAceObject(AceCollection<T> aceCollection, bool notify)
        {
            var fileDao = GetFileDao();
            var folderDao = GetFolderDao();
            var result = new List<T>();

            var entries = new List<FileEntry<T>>();
            entries.AddRange(aceCollection.Files.Select(fileId => fileDao.GetFile(fileId)));
            entries.AddRange(aceCollection.Folders.Select(folderDao.GetFolder));

            foreach (var entry in entries)
            {
                try
                {
                    var changed = FileSharingAceHelper.SetAceObject(aceCollection.Aces, entry, notify, aceCollection.Message);
                    if (changed)
                    {
                        FilesMessageService.Send(entry, GetHttpHeaders(),
                                                    entry.FileEntryType == FileEntryType.Folder ? MessageAction.FolderUpdatedAccess : MessageAction.FileUpdatedAccess,
                                                    entry.Title);
                    }
                }
                catch (Exception e)
                {
                    throw GenerateException(e);
                }

                var securityDao = GetSecurityDao();
                if (securityDao.IsShared(entry.ID, entry.FileEntryType))
                {
                    result.Add(entry.ID);
                }
            }
            return result;
        }

        public void RemoveAce(List<T> filesId, List<T> foldersId)
        {
            ErrorIf(!AuthContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);
            var entries = new List<FileEntry<T>>();

            var fileDao = GetFileDao();
            var folderDao = GetFolderDao();
            entries.AddRange(filesId.Select(fileId => fileDao.GetFile(fileId)));
            entries.AddRange(foldersId.Select(folderDao.GetFolder));

            FileSharingAceHelper.RemoveAce(entries);
        }

        public string GetShortenLink(T fileId)
        {
            File<T> file;
            var fileDao = GetFileDao();
            file = fileDao.GetFile(fileId);
            ErrorIf(!FileSharing.CanSetAccess(file), FilesCommonResource.ErrorMassage_SecurityException);
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

        public bool SetAceLink(T fileId, FileShare share)
        {
            FileEntry<T> file;
            var fileDao = GetFileDao();
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

                var changed = FileSharingAceHelper.SetAceObject(aces, file, false, null);
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

        public List<MentionWrapper> SharedUsers(T fileId)
        {
            if (!AuthContext.IsAuthenticated || CoreBaseSettings.Personal)
                return null;

            FileEntry<T> file;
            var fileDao = GetFileDao();
            file = fileDao.GetFile(fileId);

            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);

            var usersIdWithAccess = new List<Guid>();
            if (FileSharing.CanSetAccess(file))
            {
                var access = FileSharing.GetSharedInfo(file);
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

            return new List<MentionWrapper>(users);
        }

        public List<AceShortWrapper> SendEditorNotify(T fileId, MentionMessageWrapper mentionMessage)
        {
            ErrorIf(!AuthContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);

            File<T> file;
            var fileDao = GetFileDao();
            file = fileDao.GetFile(fileId);

            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);

            var fileSecurity = FileSecurity;
            ErrorIf(!fileSecurity.CanRead(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
            ErrorIf(mentionMessage == null || mentionMessage.Emails == null, FilesCommonResource.ErrorMassage_BadRequest);

            var showSharingSettings = false;
            bool? canShare = null;
            if (file.Encrypted)
            {
                canShare = false;
                showSharingSettings = true;
            }


            var recipients = new List<Guid>();
            foreach (var email in mentionMessage.Emails)
            {
                if (!canShare.HasValue)
                {
                    canShare = FileSharing.CanSetAccess(file);
                }

                var recipient = UserManager.GetUserByEmail(email);
                if (recipient == null || recipient.ID == Constants.LostUser.ID)
                {
                    showSharingSettings = canShare.Value;
                    continue;
                }

                if (!fileSecurity.CanRead(file, recipient.ID))
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

                        showSharingSettings |= FileSharingAceHelper.SetAceObject(aces, file, false, null);

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

            if (showSharingSettings)
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

            return showSharingSettings ? GetSharedInfoShortFile(fileId) : null;
        }

        public List<EncryptionKeyPair> GetEncryptionAccess(T fileId)
        {
            ErrorIf(!PrivacyRoomSettings.GetEnabled(SettingsManager), FilesCommonResource.ErrorMassage_SecurityException);

            var fileKeyPair = EncryptionKeyPairHelper.GetKeyPair(fileId, this);
            return new List<EncryptionKeyPair>(fileKeyPair);
        }

        public List<string> GetMailAccounts()
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

            //return new List<string>(accounts);
        }

        public IEnumerable<FileEntry> ChangeOwner(IEnumerable<T> foldersId, IEnumerable<T> filesId, Guid userId)
        {
            var userInfo = UserManager.GetUsers(userId);
            ErrorIf(Equals(userInfo, Constants.LostUser) || userInfo.IsVisitor(UserManager), FilesCommonResource.ErrorMassage_ChangeOwner);

            var entries = new List<FileEntry>();

            var folderDao = GetFolderDao();
            var folders = folderDao.GetFolders(foldersId);

            foreach (var folder in folders)
            {
                ErrorIf(!FileSecurity.CanEdit(folder), FilesCommonResource.ErrorMassage_SecurityException);
                ErrorIf(folder.RootFolderType != FolderType.COMMON, FilesCommonResource.ErrorMassage_SecurityException);
                if (folder.ProviderEntry) continue;

                var newFolder = folder;
                if (folder.CreateBy != userInfo.ID)
                {
                    var folderAccess = folder.Access;

                    newFolder.CreateBy = userInfo.ID;
                    var newFolderID = folderDao.SaveFolder(newFolder);

                    newFolder = folderDao.GetFolder(newFolderID);
                    newFolder.Access = folderAccess;

                    FilesMessageService.Send(newFolder, GetHttpHeaders(), MessageAction.FileChangeOwner, new[] { newFolder.Title, userInfo.DisplayUserName(false, DisplayUserSettingsHelper) });
                }
                entries.Add(newFolder);
            }

            var fileDao = GetFileDao();
            var files = fileDao.GetFiles(filesId);

            foreach (var file in files)
            {
                ErrorIf(!FileSecurity.CanEdit(file), FilesCommonResource.ErrorMassage_SecurityException);
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

                    if (file.ThumbnailStatus == Thumbnail.Created)
                    {
                        using (var thumbnail = fileDao.GetThumbnail(file))
                        {
                            fileDao.SaveThumbnail(newFile, thumbnail);
                        }
                        newFile.ThumbnailStatus = Thumbnail.Created;
                    }

                    FileMarker.MarkAsNew(newFile);

                    EntryStatusManager.SetFileStatus(newFile);

                    FilesMessageService.Send(newFile, GetHttpHeaders(), MessageAction.FileChangeOwner, new[] { newFile.Title, userInfo.DisplayUserName(false, DisplayUserSettingsHelper) });
                }
                entries.Add(newFile);
            }

            return entries;
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

        public bool DisplayRecent(bool set)
        {
            if (!AuthContext.IsAuthenticated) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);

            FilesSettingsHelper.RecentSection = set;

            return FilesSettingsHelper.RecentSection;
        }

        public bool DisplayFavorite(bool set)
        {
            if (!AuthContext.IsAuthenticated) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);

            FilesSettingsHelper.FavoritesSection = set;

            return FilesSettingsHelper.FavoritesSection;
        }

        public bool DisplayTemplates(bool set)
        {
            if (!AuthContext.IsAuthenticated) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);

            FilesSettingsHelper.TemplatesSection = set;

            return FilesSettingsHelper.TemplatesSection;
        }

        public bool ChangeDownloadTarGz(bool set)
        {
            FilesSettingsHelper.DownloadTarGz = set;

            return FilesSettingsHelper.DownloadTarGz;
        }

        public bool ChangeDeleteConfrim(bool set)
        {
            FilesSettingsHelper.ConfirmDelete = set;

            return FilesSettingsHelper.ConfirmDelete;
        }

        public IEnumerable<JsonElement> CreateThumbnails(IEnumerable<JsonElement> fileIds)
        {
            try
            {
                var req = new ThumbnailRequest()
                {
                    Tenant = TenantManager.GetCurrentTenant().TenantId,
                    BaseUrl = BaseCommonLinkUtility.GetFullAbsolutePath("")
                };

                foreach (var f in fileIds.Where(r => r.ValueKind == JsonValueKind.Number))
                {
                    req.Files.Add(f.GetInt32());
                }

                ThumbnailNotify.Publish(req, CacheNotifyAction.Insert);
            }
            catch (Exception e)
            {
                Logger.Error("CreateThumbnails", e);
            }

            return fileIds;
        }

        public string GetHelpCenter()
        {
            return ""; //TODO: Studio.UserControls.Common.HelpCenter.HelpCenter.RenderControlToString();
        }

        private IFolderDao<T> GetFolderDao()
        {
            return DaoFactory.GetFolderDao<T>();
        }

        private IFileDao<T> GetFileDao()
        {
            return DaoFactory.GetFileDao<T>();
        }

        private ITagDao<T> GetTagDao()
        {
            return DaoFactory.GetTagDao<T>();
        }

        private IDataStore GetStoreTemplate()
        {
            return GlobalStore.GetStoreTemplate();
        }

        private IProviderDao GetProviderDao()
        {
            return DaoFactory.ProviderDao;
        }

        private ISecurityDao<T> GetSecurityDao()
        {
            return DaoFactory.GetSecurityDao<T>();
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

        private IDictionary<string, StringValues> GetHttpHeaders()
        {
            return HttpContextAccessor?.HttpContext?.Request?.Headers;
        }
    }

    public class FileModel<T>
    {
        public T ParentId { get; set; }
        public string Title { get; set; }
        public T TemplateId { get; set; }
    }
}