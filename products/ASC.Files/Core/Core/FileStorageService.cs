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
using System.Threading.Tasks;
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
using ASC.FederatedLogin.LoginProviders;
using ASC.Files.Core;
using ASC.Files.Core.Model;
using ASC.Files.Core.Resources;
using ASC.Files.Core.Security;
using ASC.Files.Core.Services.NotifyService;
using ASC.MessagingSystem;
using ASC.Web.Core.Files;
using ASC.Web.Core.PublicResources;
using ASC.Web.Core.Users;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Core.Compress;
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
        public CompressToArchive CompressToArchive { get; }
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
            EntryStatusManager entryStatusManager,
            CompressToArchive compressToArchive)
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
            CompressToArchive = compressToArchive;
        }

        public async Task<Folder<T>> GetFolderAsync(T folderId)
        {
            var folderDao = GetFolderDao();
            var folder = await folderDao.GetFolderAsync(folderId);

            ErrorIf(folder == null, FilesCommonResource.ErrorMassage_FolderNotFound);
            ErrorIf(!await FileSecurity.CanReadAsync(folder), FilesCommonResource.ErrorMassage_SecurityException_ReadFolder);

            return folder;
        }

        public async Task<List<FileEntry>> GetFoldersAsync(T parentId)
        {
            var folderDao = GetFolderDao();

            try
            {
                var entries = await EntryManager.GetEntriesAsync(await folderDao.GetFolderAsync(parentId), 0, 0, FilterType.FoldersOnly, false, Guid.Empty, string.Empty, false, false, new OrderBy(SortedByType.AZ, true));
                return new List<FileEntry>(entries.Entries);
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }
        }

        public async Task<List<object>> GetPathAsync(T folderId)
        {
            var folderDao = GetFolderDao();
            var folder = await folderDao.GetFolderAsync(folderId);

            ErrorIf(folder == null, FilesCommonResource.ErrorMassage_FolderNotFound);
            ErrorIf(!await FileSecurity.CanReadAsync(folder), FilesCommonResource.ErrorMassage_SecurityException_ViewFolder);

            var breadCrumbs = await EntryManager.GetBreadCrumbsAsync(folderId, folderDao);

            return new List<object>(breadCrumbs.Select(f =>
            {
                if (f is Folder<string> f1) return (object)f1.ID;
                if (f is Folder<int> f2) return f2.ID;
                return 0;
            }));
        }

        public async Task<DataWrapper<T>> GetFolderItemsAsync(T parentId, int from, int count, FilterType filter, bool subjectGroup, string ssubject, string searchText, bool searchInContent, bool withSubfolders, OrderBy orderBy)
        {
            var subjectId = string.IsNullOrEmpty(ssubject) ? Guid.Empty : new Guid(ssubject);

            var folderDao = GetFolderDao();

            Folder<T> parent = null;
            try
            {
                parent = await folderDao.GetFolderAsync(parentId);
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
            ErrorIf(!await FileSecurity.CanReadAsync(parent), FilesCommonResource.ErrorMassage_SecurityException_ViewFolder);
            ErrorIf(parent.RootFolderType == FolderType.TRASH && !Equals(parent.ID, GlobalFolderHelper.FolderTrash), FilesCommonResource.ErrorMassage_ViewTrashItem);

            if (orderBy != null)
            {
                FilesSettingsHelper.DefaultOrder = orderBy;
            }
            else
            {
                orderBy = FilesSettingsHelper.DefaultOrder;
            }

            if (Equals(parent.ID, await GlobalFolderHelper.FolderShareAsync) && orderBy.SortedBy == SortedByType.DateAndTime)
                orderBy.SortedBy = SortedByType.New;

            int total;
            IEnumerable<FileEntry> entries;
            try
            {
                (entries, total) = await EntryManager.GetEntriesAsync(parent, from, count, filter, subjectGroup, subjectId, searchText, searchInContent, withSubfolders, orderBy);
            }
            catch (Exception e)
            {
                if (parent.ProviderEntry)
                {
                    throw GenerateException(new Exception(FilesCommonResource.ErrorMassage_SharpBoxException, e));
                }
                throw GenerateException(e);
            }

            var breadCrumbs = await EntryManager.GetBreadCrumbsAsync(parentId, folderDao);

            var prevVisible = breadCrumbs.ElementAtOrDefault(breadCrumbs.Count - 2);
            if (prevVisible != null)
            {
                if (prevVisible is Folder<string> f1) parent.FolderID = (T)Convert.ChangeType(f1.ID, typeof(T));
                if (prevVisible is Folder<int> f2) parent.FolderID = (T)Convert.ChangeType(f2.ID, typeof(T));
            }

            parent.Shareable = await FileSharing.CanSetAccessAsync(parent)
                || parent.FolderType == FolderType.SHARE
                || parent.RootFolderType == FolderType.Privacy;

            entries = entries.Where(x => x.FileEntryType == FileEntryType.Folder ||
            x is File<string> f1 && !FileConverter.IsConverting(f1) ||
             x is File<int> f2 && !FileConverter.IsConverting(f2));

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
                New = await FileMarker.GetRootFoldersIdMarkedAsNewAsync(parentId)
            };

            return result;
        }

        public async Task<object> GetFolderItemsXmlAsync(T parentId, int from, int count, FilterType filter, bool subjectGroup, string subjectID, string search, bool searchInContent, bool withSubfolders, OrderBy orderBy)
        {
            var folderItems = await GetFolderItemsAsync(parentId, from, count, filter, subjectGroup, subjectID, search, searchInContent, withSubfolders, orderBy);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(serializer.ToXml(folderItems))
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
            return response;
        }

        public async Task<List<FileEntry>> GetItemsAsync<TId>(IEnumerable<TId> filesId, IEnumerable<TId> foldersId, FilterType filter, bool subjectGroup, string subjectID, string search)
        {
            var subjectId = string.IsNullOrEmpty(subjectID) ? Guid.Empty : new Guid(subjectID);

            var entries = Enumerable.Empty<FileEntry<TId>>();

            var folderDao = DaoFactory.GetFolderDao<TId>();
            var fileDao = DaoFactory.GetFileDao<TId>();
            var folders = await folderDao.GetFoldersAsync(foldersId).ToListAsync();
            var tmpFolders = await FileSecurity.FilterReadAsync(folders);
            folders = tmpFolders.ToList();
            entries = entries.Concat(folders);

            var files = await fileDao.GetFilesAsync(filesId).ToListAsync();
            files = (await FileSecurity.FilterReadAsync(files)).ToList();
            entries = entries.Concat(files);

            entries = EntryManager.FilterEntries(entries, filter, subjectGroup, subjectId, search, true);

            foreach (var fileEntry in entries)
            {
                if (fileEntry is File<TId> file)
                {
                    if (fileEntry.RootFolderType == FolderType.USER
                        && !Equals(fileEntry.RootFolderCreator, AuthContext.CurrentAccount.ID)
                        && !await FileSecurity.CanReadAsync(await folderDao.GetFolderAsync(file.FolderIdDisplay)))
                    {
                        file.FolderIdDisplay = await GlobalFolderHelper.GetFolderShareAsync<TId>();
                    }
                }
                else if (fileEntry is Folder<TId> folder)
                {
                    if (fileEntry.RootFolderType == FolderType.USER
                        && !Equals(fileEntry.RootFolderCreator, AuthContext.CurrentAccount.ID)
                        && !await FileSecurity.CanReadAsync(await folderDao.GetFolderAsync(folder.FolderIdDisplay)))
                    {
                        folder.FolderIdDisplay = await GlobalFolderHelper.GetFolderShareAsync<TId>();
                    }
                }
            }

            await EntryStatusManager.SetFileStatusAsync(entries);

            return new List<FileEntry>(entries);
        }

        public Task<Folder<T>> CreateNewFolderAsync(T parentId, string title)
        {
            if (string.IsNullOrEmpty(title) || parentId == null) throw new ArgumentException();

            return InternalCreateNewFolderAsync(parentId, title);
        }

        public async Task<Folder<T>> InternalCreateNewFolderAsync(T parentId, string title)
        {
            var folderDao = GetFolderDao();

            var parent = await folderDao.GetFolderAsync(parentId);
            ErrorIf(parent == null, FilesCommonResource.ErrorMassage_FolderNotFound);
            ErrorIf(!await FileSecurity.CanCreateAsync(parent), FilesCommonResource.ErrorMassage_SecurityException_Create);

            try
            {
                var newFolder = ServiceProvider.GetService<Folder<T>>();
                newFolder.Title = title;
                newFolder.FolderID = parent.ID;

                var folderId = await folderDao.SaveFolderAsync(newFolder);
                var folder = await folderDao.GetFolderAsync(folderId);
                FilesMessageService.Send(folder, GetHttpHeaders(), MessageAction.FolderCreated, folder.Title);

                return folder;
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }
        }

        public async Task<Folder<T>> FolderRenameAsync(T folderId, string title)
        {
            var tagDao = GetTagDao();
            var folderDao = GetFolderDao();
            var folder = await folderDao.GetFolderAsync(folderId);
            ErrorIf(folder == null, FilesCommonResource.ErrorMassage_FolderNotFound);
            ErrorIf(!await FileSecurity.CanEditAsync(folder), FilesCommonResource.ErrorMassage_SecurityException_RenameFolder);
            if (!await FileSecurity.CanDeleteAsync(folder) && UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_RenameFolder);
            ErrorIf(folder.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_ViewTrashItem);

            var folderAccess = folder.Access;

            if (!string.Equals(folder.Title, title, StringComparison.OrdinalIgnoreCase))
            {
                var newFolderID = await folderDao.RenameFolderAsync(folder, title);
                folder = await folderDao.GetFolderAsync(newFolderID);
                folder.Access = folderAccess;

                FilesMessageService.Send(folder, GetHttpHeaders(), MessageAction.FolderRenamed, folder.Title);

                //if (!folder.ProviderEntry)
                //{
                //    FoldersIndexer.IndexAsync(FoldersWrapper.GetFolderWrapper(ServiceProvider, folder));
                //}
            }

            var newTags = tagDao.GetNewTagsAsync(AuthContext.CurrentAccount.ID, folder);
            var tag = await newTags.FirstOrDefaultAsync();
            if (tag != null)
            {
                folder.NewForMe = tag.Count;
            }

            if (folder.RootFolderType == FolderType.USER
                && !Equals(folder.RootFolderCreator, AuthContext.CurrentAccount.ID)
                && !await FileSecurity.CanReadAsync(await folderDao.GetFolderAsync(folder.FolderID)))
            {
                folder.FolderIdDisplay = await GlobalFolderHelper.GetFolderShareAsync<T>();
            }

            return folder;
        }

        public async Task<File<T>> GetFileAsync(T fileId, int version)
        {
            var fileDao = GetFileDao();
            await fileDao.InvalidateCacheAsync(fileId);

            var file = version > 0
                           ? await fileDao.GetFileAsync(fileId, version)
                           : await fileDao.GetFileAsync(fileId);
            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
            ErrorIf(!await FileSecurity.CanReadAsync(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

            await EntryStatusManager.SetFileStatusAsync(file);

            if (file.RootFolderType == FolderType.USER
                && !Equals(file.RootFolderCreator, AuthContext.CurrentAccount.ID))
            {
                var folderDao = GetFolderDao();
                if (!await FileSecurity.CanReadAsync(await folderDao.GetFolderAsync(file.FolderID)))
                {
                    file.FolderIdDisplay = await GlobalFolderHelper.GetFolderShareAsync<T>();
                }
            }

            return file;
        }

        public async Task<List<File<T>>> GetSiblingsFileAsync(T fileId, T parentId, FilterType filter, bool subjectGroup, string subjectID, string search, bool searchInContent, bool withSubfolders, OrderBy orderBy)
        {
            var subjectId = string.IsNullOrEmpty(subjectID) ? Guid.Empty : new Guid(subjectID);

            var fileDao = GetFileDao();
            var folderDao = GetFolderDao();

            var file = await fileDao.GetFileAsync(fileId);
            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
            ErrorIf(!await FileSecurity.CanReadAsync(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

            var parent = await folderDao.GetFolderAsync(EqualityComparer<T>.Default.Equals(parentId, default(T)) ? file.FolderID : parentId);
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
            if (Equals(parent.ID, await GlobalFolderHelper.GetFolderShareAsync<T>()) && orderBy.SortedBy == SortedByType.DateAndTime)
            {
                orderBy.SortedBy = SortedByType.New;
            }

            var entries = Enumerable.Empty<FileEntry>();

            if (!await FileSecurity.CanReadAsync(parent))
            {
                file.FolderID = await GlobalFolderHelper.GetFolderShareAsync<T>();
                entries = entries.Concat(new[] { file });
            }
            else
            {
                try
                {
                    (entries, var total) = await EntryManager.GetEntriesAsync(parent, 0, 0, filter, subjectGroup, subjectId, search, searchInContent, withSubfolders, orderBy);
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

            var result = await FileSecurity.FilterReadAsync(entries.OfType<File<T>>());
            result = result.OfType<File<T>>().Where(f => previewedType.Contains(FileUtility.GetFileTypeByFileName(f.Title)));

            return new List<File<T>>(result);
        }

        public Task<File<T>> CreateNewFileAsync<TTemplate>(FileModel<T, TTemplate> fileWrapper, bool enableExternalExt = false)
        {
            if (string.IsNullOrEmpty(fileWrapper.Title) || fileWrapper.ParentId == null) throw new ArgumentException();

            return InternalCreateNewFileAsync(fileWrapper, enableExternalExt);
        }

        private async Task<File<T>> InternalCreateNewFileAsync<TTemplate>(FileModel<T, TTemplate> fileWrapper, bool enableExternalExt = false)
        {
            var fileDao = GetFileDao();
            var folderDao = GetFolderDao();

            Folder<T> folder = null;
            if (!EqualityComparer<T>.Default.Equals(fileWrapper.ParentId, default(T)))
            {
                folder = await folderDao.GetFolderAsync(fileWrapper.ParentId);
                var canCreate = await FileSecurity.CanCreateAsync(folder);
                if (!canCreate)
                {
                    folder = null;
                }
            }
            if (folder == null)
            {
                folder = await folderDao.GetFolderAsync(GlobalFolderHelper.GetFolderMy<T>());
            }


            var file = ServiceProvider.GetService<File<T>>();
            file.FolderID = folder.ID;
            file.Comment = FilesCommonResource.CommentCreate;

            if (string.IsNullOrEmpty(fileWrapper.Title))
            {
                fileWrapper.Title = UserControlsCommonResource.NewDocument + ".docx";
            }

            var externalExt = false;
            var title = fileWrapper.Title;
            var fileExt = FileUtility.GetFileExtension(title);
            if (fileExt != FileUtility.MasterFormExtension)
            {
                fileExt = FileUtility.GetInternalExtension(title);
                if (!FileUtility.InternalExtension.Values.Contains(fileExt))
                {
                    fileExt = FileUtility.InternalExtension[FileType.Document];
                    file.Title = title + fileExt;
                }
                else
                {
                    file.Title = FileUtility.ReplaceFileExtension(title, fileExt);
                }
            }
            else
            {
                file.Title = FileUtility.ReplaceFileExtension(title, fileExt);
            }

            if (EqualityComparer<TTemplate>.Default.Equals(fileWrapper.TemplateId, default(TTemplate)))
            {
                var culture = UserManager.GetUsers(AuthContext.CurrentAccount.ID).GetCulture();
                var storeTemplate = GetStoreTemplate();

                var path = FileConstant.NewDocPath + culture + "/";
                if (!await storeTemplate.IsDirectoryAsync(path))
                {
                    path = FileConstant.NewDocPath + "en-US/";
                }

                try
                {
                    if (!externalExt)
                    {
                        var pathNew = path + "new" + fileExt;
                        using (var stream = await storeTemplate.GetReadStreamAsync("", pathNew, 0))
                        {
                            file.ContentLength = stream.CanSeek ? stream.Length : await storeTemplate.GetFileSizeAsync(pathNew);
                            file = await fileDao.SaveFileAsync(file, stream);
                        }
                    }
                    else
                    {
                        file = await fileDao.SaveFileAsync(file, null);
                    }

                    var pathThumb = path + fileExt.Trim('.') + "." + Global.ThumbnailExtension;
                    if (await storeTemplate.IsFileAsync("", pathThumb))
                    {
                        using (var streamThumb = await storeTemplate.GetReadStreamAsync("", pathThumb, 0))
                        {
                            await fileDao.SaveThumbnailAsync(file, streamThumb);
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
                var fileTemlateDao = DaoFactory.GetFileDao<TTemplate>();
                var template = await fileTemlateDao.GetFileAsync(fileWrapper.TemplateId);
                ErrorIf(template == null, FilesCommonResource.ErrorMassage_FileNotFound);
                ErrorIf(!await FileSecurity.CanReadAsync(template), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

                try
                {
                    using (var stream = await fileTemlateDao.GetFileStreamAsync(template))
                    {
                        file.ContentLength = template.ContentLength;
                        file = await fileDao.SaveFileAsync(file, stream);
                    }

                    if (template.ThumbnailStatus == Thumbnail.Created)
                    {
                        using (var thumb = await fileTemlateDao.GetThumbnailAsync(template))
                        {
                            await fileDao.SaveThumbnailAsync(file, thumb);
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

            await FileMarker.MarkAsNewAsync(file);

            await SocketManager.CreateFileAsync(file);

            return file;
        }

        public async Task<KeyValuePair<bool, string>> TrackEditFileAsync(T fileId, Guid tabId, string docKeyForTrack, string doc = null, bool isFinish = false)
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
                    await SocketManager.StopEditAsync(id);
                }
                else
                {
                    await EntryManager.TrackEditingAsync(id, tabId, AuthContext.CurrentAccount.ID, doc);
                }

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }

        public async Task<Dictionary<string, string>> CheckEditingAsync(List<T> filesId)
        {
            ErrorIf(!AuthContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);
            var result = new Dictionary<string, string>();

            var fileDao = GetFileDao();
            var ids = filesId.Where(FileTracker.IsEditing).Select(id => id).ToList();

            await foreach (var file in fileDao.GetFilesAsync(ids))
            {
                if (file == null
                    || !await FileSecurity.CanEditAsync(file)
                    && !await FileSecurity.CanCustomFilterEditAsync(file)
                    && !await FileSecurity.CanReviewAsync(file)
                    && !await FileSecurity.CanFillFormsAsync(file)
                    && !await FileSecurity.CanCommentAsync(file)) continue;

                var usersId = FileTracker.GetEditingBy(file.ID);
                var value = string.Join(", ", usersId.Select(userId => Global.GetUserName(userId, true)).ToArray());
                result[file.ID.ToString()] = value;
            }

            return result;
        }

        public async Task<File<T>> SaveEditingAsync(T fileId, string fileExtension, string fileuri, Stream stream, string doc = null, bool forcesave = false)
        {
            try
            {
                if (!forcesave && FileTracker.IsEditingAlone(fileId))
                {
                    FileTracker.Remove(fileId);
                    await SocketManager.StopEditAsync(fileId);
                }

                var file = await EntryManager.SaveEditingAsync(fileId, fileExtension, fileuri, stream, doc, forcesave: forcesave ? ForcesaveType.User : ForcesaveType.None, keepLink: true);

                if (file != null)
                    FilesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileUpdated, file.Title);

                return file;
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }
        }

        public async Task<File<T>> UpdateFileStreamAsync(T fileId, Stream stream, string fileExtension, bool encrypted, bool forcesave)
        {
            try
            {
                if (!forcesave && FileTracker.IsEditing(fileId))
                {
                    FileTracker.Remove(fileId);
                    await SocketManager.StopEditAsync(fileId);
                }

                var file = await EntryManager.SaveEditingAsync(fileId,
                    fileExtension,
                    null,
                    stream,
                    null,
                    encrypted ? FilesCommonResource.CommentEncrypted : null,
                    encrypted: encrypted,
                    forcesave: forcesave ? ForcesaveType.User : ForcesaveType.None);

                if (file != null)
                    FilesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileUpdated, file.Title);

                return file;
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }
        }

        public async Task<string> StartEditAsync(T fileId, bool editingAlone = false, string doc = null)
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
                        await EntryManager.TrackEditingAsync(fileId, Guid.Empty, AuthContext.CurrentAccount.ID, doc, true);
                    }

                    //without StartTrack, track via old scheme
                    return DocumentServiceHelper.GetDocKey(fileId, -1, DateTime.MinValue);
                }

                (File<string> File, Configuration<string> Configuration) fileOptions;

                app = ThirdPartySelector.GetAppByFileId(fileId.ToString());
                if (app == null)
                {
                    fileOptions = await DocumentServiceHelper.GetParamsAsync(fileId.ToString(), -1, doc, true, true, false);
                }
                else
                {
                    var file = app.GetFile(fileId.ToString(), out var editable);
                    fileOptions = await DocumentServiceHelper.GetParamsAsync(file, true, editable ? FileShare.ReadWrite : FileShare.Read, false, editable, editable, editable, false);
                }

                var configuration = fileOptions.Configuration;

                ErrorIf(!configuration.EditorConfig.ModeWrite
                        || !(configuration.Document.Permissions.Edit
                             || configuration.Document.Permissions.ModifyFilter
                             || configuration.Document.Permissions.Review
                             || configuration.Document.Permissions.FillForms
                             || configuration.Document.Permissions.Comment),
                        !string.IsNullOrEmpty(configuration.ErrorMessage) ? configuration.ErrorMessage : FilesCommonResource.ErrorMassage_SecurityException_EditFile);
                var key = configuration.Document.Key;

                if (!await DocumentServiceTrackerHelper.StartTrackAsync(fileId.ToString(), key))
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

        public async Task<File<T>> FileRenameAsync(T fileId, string title)
        {
            try
            {
                var fileRename = await EntryManager.FileRenameAsync(fileId, title);
                var file = fileRename.File;

                if (fileRename.Renamed)
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
                    if (!await FileSecurity.CanReadAsync(await folderDao.GetFolderAsync(file.FolderID)))
                    {
                        file.FolderIdDisplay = await GlobalFolderHelper.GetFolderShareAsync<T>();
                    }
                }

                return file;
            }
            catch (Exception ex)
            {
                throw GenerateException(ex);
            }
        }

        public async Task<List<File<T>>> GetFileHistoryAsync(T fileId)
        {
            var fileDao = GetFileDao();
            var file = await fileDao.GetFileAsync(fileId);
            ErrorIf(!await FileSecurity.CanReadAsync(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

            return await fileDao.GetFileHistoryAsync(fileId).ToListAsync();
        }

        public async Task<KeyValuePair<File<T>, List<File<T>>>> UpdateToVersionAsync(T fileId, int version)
        {
            var file = await EntryManager.UpdateToVersionFileAsync(fileId, version);
            FilesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileRestoreVersion, file.Title, version.ToString(CultureInfo.InvariantCulture));

            if (file.RootFolderType == FolderType.USER
                && !Equals(file.RootFolderCreator, AuthContext.CurrentAccount.ID))
            {
                var folderDao = GetFolderDao();
                if (!await FileSecurity.CanReadAsync(await folderDao.GetFolderAsync(file.FolderID)))
                {
                    file.FolderIdDisplay = await GlobalFolderHelper.GetFolderShareAsync<T>();
                }
            }

            return new KeyValuePair<File<T>, List<File<T>>>(file, await GetFileHistoryAsync(fileId));
        }

        public async Task<string> UpdateCommentAsync(T fileId, int version, string comment)
        {
            var fileDao = GetFileDao();
            var file = await fileDao.GetFileAsync(fileId, version);
            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
            ErrorIf(!await FileSecurity.CanEditAsync(file) || UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager), FilesCommonResource.ErrorMassage_SecurityException_EditFile);
            ErrorIf(await EntryManager.FileLockedForMeAsync(file.ID), FilesCommonResource.ErrorMassage_LockedFile);
            ErrorIf(file.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_ViewTrashItem);

            comment = await fileDao.UpdateCommentAsync(fileId, version, comment);

            FilesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileUpdatedRevisionComment, file.Title, version.ToString(CultureInfo.InvariantCulture));

            return comment;
        }

        public async Task<KeyValuePair<File<T>, List<File<T>>>> CompleteVersionAsync(T fileId, int version, bool continueVersion)
        {
            var file = await EntryManager.CompleteVersionFileAsync(fileId, version, continueVersion);

            FilesMessageService.Send(file, GetHttpHeaders(),
                                     continueVersion ? MessageAction.FileDeletedVersion : MessageAction.FileCreatedVersion,
                                     file.Title, version == 0 ? (file.Version - 1).ToString(CultureInfo.InvariantCulture) : version.ToString(CultureInfo.InvariantCulture));

            if (file.RootFolderType == FolderType.USER
                && !Equals(file.RootFolderCreator, AuthContext.CurrentAccount.ID))
            {
                var folderDao = GetFolderDao();
                if (!await FileSecurity.CanReadAsync(await folderDao.GetFolderAsync(file.FolderID)))
                {
                    file.FolderIdDisplay = await GlobalFolderHelper.GetFolderShareAsync<T>();
                }
            }

            return new KeyValuePair<File<T>, List<File<T>>>(file, await GetFileHistoryAsync(fileId));
        }

        public async Task<File<T>> LockFileAsync(T fileId, bool lockfile)
        {
            var tagDao = GetTagDao();
            var fileDao = GetFileDao();
            var file = await fileDao.GetFileAsync(fileId);

            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
            ErrorIf(!await FileSecurity.CanEditAsync(file) || lockfile && UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager), FilesCommonResource.ErrorMassage_SecurityException_EditFile);
            ErrorIf(file.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_ViewTrashItem);

            var tags = tagDao.GetTagsAsync(file.ID, FileEntryType.File, TagType.Locked);
            var tagLocked = await tags.FirstOrDefaultAsync();

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
                if (usersDrop.Length > 0)
                {
                    var fileStable = file.Forcesave == ForcesaveType.None ? file : await fileDao.GetFileStableAsync(file.ID, file.Version);
                    var docKey = DocumentServiceHelper.GetDocKey(fileStable);
                    await DocumentServiceHelper.DropUserAsync(docKey, usersDrop, file.ID);
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
                    file = await EntryManager.CompleteVersionFileAsync(file.ID, 0, false);
                    await UpdateCommentAsync(file.ID, file.Version, FilesCommonResource.UnlockComment);
                }
            }

            await EntryStatusManager.SetFileStatusAsync(file);

            if (file.RootFolderType == FolderType.USER
                && !Equals(file.RootFolderCreator, AuthContext.CurrentAccount.ID))
            {
                var folderDao = GetFolderDao();
                if (!await FileSecurity.CanReadAsync(await folderDao.GetFolderAsync(file.FolderID)))
                {
                    file.FolderIdDisplay = await GlobalFolderHelper.GetFolderShareAsync<T>();
                }
            }

            return file;
        }

        public async Task<List<EditHistory>> GetEditHistoryAsync(T fileId, string doc = null)
        {
            var fileDao = GetFileDao();
            var (readLink, file) = await FileShareLink.CheckAsync(doc, true, fileDao);
            if (file == null)
                file = await fileDao.GetFileAsync(fileId);

            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
            ErrorIf(!readLink && !await FileSecurity.CanReadAsync(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
            ErrorIf(file.ProviderEntry, FilesCommonResource.ErrorMassage_BadRequest);

            return new List<EditHistory>(await fileDao.GetEditHistoryAsync(DocumentServiceHelper, file.ID));
        }

        public async Task<EditHistoryData> GetEditDiffUrlAsync(T fileId, int version = 0, string doc = null)
        {
            var fileDao = GetFileDao();
            var (readLink, file) = await FileShareLink.CheckAsync(doc, true, fileDao);

            if (file != null)
            {
                fileId = file.ID;
            }

            if (file == null
                || version > 0 && file.Version != version)
            {
                file = version > 0
                           ? await fileDao.GetFileAsync(fileId, version)
                           : await fileDao.GetFileAsync(fileId);
            }

            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
            ErrorIf(!readLink && !await FileSecurity.CanReadAsync(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
            ErrorIf(file.ProviderEntry, FilesCommonResource.ErrorMassage_BadRequest);

            var result = new EditHistoryData
            {
                Key = DocumentServiceHelper.GetDocKey(file),
                Url = DocumentServiceConnector.ReplaceCommunityAdress(PathProvider.GetFileStreamUrl(file, doc)),
                Version = version,
                FileType = GetFileExtensionWithoutDot(FileUtility.GetFileExtension(file.Title))
            };

            if (await fileDao.ContainChangesAsync(file.ID, file.Version))
            {
                string previouseKey;
                string sourceFileUrl;
                string previousFileExt;

                if (file.Version > 1)
                {
                    var previousFileStable = await fileDao.GetFileStableAsync(file.ID, file.Version - 1);
                    ErrorIf(previousFileStable == null, FilesCommonResource.ErrorMassage_FileNotFound);

                    sourceFileUrl = PathProvider.GetFileStreamUrl(previousFileStable, doc);

                    previouseKey = DocumentServiceHelper.GetDocKey(previousFileStable);
                    previousFileExt = FileUtility.GetFileExtension(previousFileStable.Title);
                }
                else
                {
                    var culture = UserManager.GetUsers(AuthContext.CurrentAccount.ID).GetCulture();
                    var storeTemplate = GetStoreTemplate();

                    var path = FileConstant.NewDocPath + culture + "/";
                    if (!await storeTemplate.IsDirectoryAsync(path))
                    {
                        path = FileConstant.NewDocPath + "en-US/";
                    }

                    var fileExt = FileUtility.GetFileExtension(file.Title);

                    path += "new" + fileExt;

                    var uri = await storeTemplate.GetUriAsync("", path);
                    sourceFileUrl = uri.ToString();
                    sourceFileUrl = BaseCommonLinkUtility.GetFullAbsolutePath(sourceFileUrl);

                    previouseKey = DocumentServiceConnector.GenerateRevisionId(Guid.NewGuid().ToString());
                    previousFileExt = fileExt;
                }

                result.Previous = new EditHistoryUrl
                {
                    Key = previouseKey,
                    Url = DocumentServiceConnector.ReplaceCommunityAdress(sourceFileUrl),
                    FileType = GetFileExtensionWithoutDot(previousFileExt)
                };
                result.ChangesUrl = PathProvider.GetFileChangesUrl(file, doc);
            }

            result.Token = DocumentServiceHelper.GetSignature(result);

            return result;

            string GetFileExtensionWithoutDot(string ext)
            {
                return ext.Substring(ext.IndexOf('.') + 1);
            }
        }

        public async Task<List<EditHistory>> RestoreVersionAsync(T fileId, int version, string url = null, string doc = null)
        {
            IFileDao<T> fileDao;
            File<T> file;
            if (string.IsNullOrEmpty(url))
            {
                file = await EntryManager.UpdateToVersionFileAsync(fileId, version, doc);
            }
            else
            {
                string modifiedOnString;
                fileDao = GetFileDao();
                var fromFile = await fileDao.GetFileAsync(fileId, version);
                modifiedOnString = fromFile.ModifiedOnString;
                file = await EntryManager.SaveEditingAsync(fileId, null, url, null, doc, string.Format(FilesCommonResource.CommentRevertChanges, modifiedOnString));
            }

            FilesMessageService.Send(file, HttpContextAccessor?.HttpContext?.Request?.Headers, MessageAction.FileRestoreVersion, file.Title, version.ToString(CultureInfo.InvariantCulture));

            fileDao = GetFileDao();
            return new List<EditHistory>(await fileDao.GetEditHistoryAsync(DocumentServiceHelper, file.ID));
        }

        public async Task<Web.Core.Files.DocumentService.FileLink> GetPresignedUriAsync(T fileId)
        {
            var file = await GetFileAsync(fileId, -1);
            var result = new Web.Core.Files.DocumentService.FileLink
            {
                FileType = FileUtility.GetFileExtension(file.Title),
                Url = DocumentServiceConnector.ReplaceCommunityAdress(PathProvider.GetFileStreamUrl(file))
            };

            result.Token = DocumentServiceHelper.GetSignature(result);

            return result;
        }

        public async Task<List<FileEntry>> GetNewItemsAsync(T folderId)
        {
            try
            {
                Folder<T> folder;
                var folderDao = GetFolderDao();
                folder = await folderDao.GetFolderAsync(folderId);

                var result = await FileMarker.MarkedItemsAsync(folder);

                result = new List<FileEntry>(EntryManager.SortEntries<T>(result, new OrderBy(SortedByType.DateAndTime, false)));

                if (result.Count == 0)
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

        public List<FileOperationResult> MarkAsRead(List<JsonElement> foldersId, List<JsonElement> filesId)
        {
            if (foldersId.Count == 0 && filesId.Count == 0) return GetTasksStatuses();
            return FileOperationsManager.MarkAsRead(AuthContext.CurrentAccount.ID, TenantManager.GetCurrentTenant(), foldersId, filesId);
        }

        public Task<List<ThirdPartyParams>> GetThirdPartyAsync()
        {
            var providerDao = GetProviderDao();
            if (providerDao == null) return Task.FromResult(new List<ThirdPartyParams>());

            return internalGetThirdPartyAsync(providerDao);
        }

        public async Task<List<ThirdPartyParams>> internalGetThirdPartyAsync(IProviderDao providerDao)
        {
            var providersInfo = await providerDao.GetProvidersInfoAsync().ToListAsync();

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

        public Task<List<FileEntry>> GetThirdPartyFolderAsync(int folderType = 0)
        {
            if (!FilesSettingsHelper.EnableThirdParty) return Task.FromResult(new List<FileEntry>());

            var providerDao = GetProviderDao();
            if (providerDao == null) return Task.FromResult(new List<FileEntry>());

            return InternalGetThirdPartyFolderAsync(folderType, providerDao);
        }

        private async Task<List<FileEntry>> InternalGetThirdPartyFolderAsync(int folderType, IProviderDao providerDao)
        {
            var providersInfo = await providerDao.GetProvidersInfoAsync((FolderType)folderType).ToListAsync();

            var folders = providersInfo.Select(providerInfo =>
            {
                var folder = EntryManager.GetFakeThirdpartyFolder(providerInfo);
                folder.NewForMe = folder.RootFolderType == FolderType.COMMON ? 1 : 0;
                return folder;
            });

            return new List<FileEntry>(folders);
        }

        public Task<Folder<T>> SaveThirdPartyAsync(ThirdPartyParams thirdPartyParams)
        {
            var providerDao = GetProviderDao();

            if (providerDao == null) return Task.FromResult<Folder<T>>(null);

            return InternalSaveThirdPartyAsync(thirdPartyParams, providerDao);
        }

        private async Task<Folder<T>> InternalSaveThirdPartyAsync(ThirdPartyParams thirdPartyParams, IProviderDao providerDao)
        {
            var folderDaoInt = DaoFactory.GetFolderDao<int>();
            var folderDao = GetFolderDao();

            ErrorIf(thirdPartyParams == null, FilesCommonResource.ErrorMassage_BadRequest);
            var parentFolder = await folderDaoInt.GetFolderAsync(thirdPartyParams.Corporate && !CoreBaseSettings.Personal ? await GlobalFolderHelper.FolderCommonAsync : GlobalFolderHelper.FolderMy);
            ErrorIf(!await FileSecurity.CanCreateAsync(parentFolder), FilesCommonResource.ErrorMassage_SecurityException_Create);
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
                    curProviderId = await providerDao.SaveProviderInfoAsync(thirdPartyParams.ProviderKey, thirdPartyParams.CustomerTitle, thirdPartyParams.AuthData, folderType);
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

                var lostProvider = await providerDao.GetProviderInfoAsync(curProviderId);
                ErrorIf(lostProvider.Owner != AuthContext.CurrentAccount.ID, FilesCommonResource.ErrorMassage_SecurityException);

                lostFolderType = lostProvider.RootFolderType;
                if (lostProvider.RootFolderType == FolderType.COMMON && !thirdPartyParams.Corporate)
                {
                    var lostFolder = await folderDao.GetFolderAsync((T)Convert.ChangeType(lostProvider.RootFolderId, typeof(T)));
                    await FileMarker.RemoveMarkAsNewForAllAsync(lostFolder);
                }

                curProviderId = await providerDao.UpdateProviderInfoAsync(curProviderId, thirdPartyParams.CustomerTitle, thirdPartyParams.AuthData, folderType);
                messageAction = MessageAction.ThirdPartyUpdated;
            }

            var provider = await providerDao.GetProviderInfoAsync(curProviderId);
            await provider.InvalidateStorageAsync();

            var folderDao1 = GetFolderDao();
            var folder = await folderDao1.GetFolderAsync((T)Convert.ChangeType(provider.RootFolderId, typeof(T)));
            ErrorIf(!await FileSecurity.CanReadAsync(folder), FilesCommonResource.ErrorMassage_SecurityException_ViewFolder);

            FilesMessageService.Send(parentFolder, GetHttpHeaders(), messageAction, folder.ID.ToString(), provider.ProviderKey);

            if (thirdPartyParams.Corporate && lostFolderType != FolderType.COMMON)
            {
                await FileMarker.MarkAsNewAsync(folder);
            }

            return folder;
        }

        public Task<object> DeleteThirdPartyAsync(string providerId)
        {
            var providerDao = GetProviderDao();
            if (providerDao == null) return Task.FromResult<object>(null);

            return InternalDeleteThirdPartyAsync(providerId, providerDao);
        }

        private async Task<object> InternalDeleteThirdPartyAsync(string providerId, IProviderDao providerDao)
        {
            var curProviderId = Convert.ToInt32(providerId);
            var providerInfo = await providerDao.GetProviderInfoAsync(curProviderId);

            var folder = EntryManager.GetFakeThirdpartyFolder(providerInfo);
            ErrorIf(!await FileSecurity.CanDeleteAsync(folder), FilesCommonResource.ErrorMassage_SecurityException_DeleteFolder);

            if (providerInfo.RootFolderType == FolderType.COMMON)
            {
                await FileMarker.RemoveMarkAsNewForAllAsync(folder);
            }

            await providerDao.RemoveProviderInfoAsync(folder.ProviderId);
            FilesMessageService.Send(folder, GetHttpHeaders(), MessageAction.ThirdPartyDeleted, folder.ID, providerInfo.ProviderKey);

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

        public Task<string> SendDocuSignAsync(T fileId, DocuSignData docuSignData)
        {
            try
            {
                ErrorIf(UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)
                        || !FilesSettingsHelper.EnableThirdParty
                        || !ThirdpartyConfiguration.SupportDocuSignInclusion, FilesCommonResource.ErrorMassage_SecurityException_Create);

                return DocuSignHelper.SendDocuSignAsync(fileId, docuSignData, GetHttpHeaders());
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
            ErrorIf(folders.Count == 0 && files.Count == 0, FilesCommonResource.ErrorMassage_BadRequest);

            return FileOperationsManager.Download(AuthContext.CurrentAccount.ID, TenantManager.GetCurrentTenant(), folders, files, GetHttpHeaders());
        }

        public async Task<(List<object>, List<object>)> MoveOrCopyFilesCheckAsync<T1>(List<JsonElement> filesId, List<JsonElement> foldersId, T1 destFolderId)
        {
            var (folderIntIds, folderStringIds) = FileOperationsManager.GetIds(foldersId);
            var (fileIntIds, fileStringIds) = FileOperationsManager.GetIds(filesId);

            var checkedFiles = new List<object>();
            var checkedFolders = new List<object>();

            var (filesInts, folderInts) = await MoveOrCopyFilesCheckAsync(fileIntIds, folderIntIds, destFolderId);

            foreach (var i in filesInts)
            {
                checkedFiles.Add(i);
            }

            foreach (var i in folderInts)
            {
                checkedFolders.Add(i);
            }

            var (filesStrings, folderStrings) = await MoveOrCopyFilesCheckAsync(fileStringIds, folderStringIds, destFolderId);

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

        private async Task<(List<TFrom>, List<TFrom>)> MoveOrCopyFilesCheckAsync<TFrom, TTo>(IEnumerable<TFrom> filesId, IEnumerable<TFrom> foldersId, TTo destFolderId)
        {
            var checkedFiles = new List<TFrom>();
            var checkedFolders = new List<TFrom>();
            var folderDao = DaoFactory.GetFolderDao<TFrom>();
            var fileDao = DaoFactory.GetFileDao<TFrom>();
            var destFolderDao = DaoFactory.GetFolderDao<TTo>();
            var destFileDao = DaoFactory.GetFileDao<TTo>();

            var toFolder = await destFolderDao.GetFolderAsync(destFolderId);
            ErrorIf(toFolder == null, FilesCommonResource.ErrorMassage_FolderNotFound);
            ErrorIf(!await FileSecurity.CanCreateAsync(toFolder), FilesCommonResource.ErrorMassage_SecurityException_Create);

            foreach (var id in filesId)
            {
                var file = await fileDao.GetFileAsync(id);
                if (file != null
                    && !file.Encrypted
                    && await destFileDao.IsExistAsync(file.Title, toFolder.ID))
                {
                    checkedFiles.Add(id);
                }
            }

            var folders = folderDao.GetFoldersAsync(foldersId);
            var foldersProject = folders.Where(folder => folder.FolderType == FolderType.BUNCH);
            var toSubfolders = destFolderDao.GetFoldersAsync(toFolder.ID);

            await foreach (var folderProject in foldersProject)
            {
                var toSub = await toSubfolders.FirstOrDefaultAsync(to => Equals(to.Title, folderProject.Title));
                if (toSub == null) continue;

                var filesPr = fileDao.GetFilesAsync(folderProject.ID);
                var foldersTmp = folderDao.GetFoldersAsync(folderProject.ID);
                var foldersPr = foldersTmp.Select(d => d.ID).ToListAsync();

                var (cFiles, cFolders) = await MoveOrCopyFilesCheckAsync(await filesPr, await foldersPr, toSub.ID);
                checkedFiles.AddRange(cFiles);
                checkedFolders.AddRange(cFolders);
            }

            try
            {
                foreach (var pair in await folderDao.CanMoveOrCopyAsync(foldersId.ToArray(), toFolder.ID))
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

        public List<FileOperationResult> MoveOrCopyItems(List<JsonElement> foldersId, List<JsonElement> filesId, JsonElement destFolderId, FileConflictResolveType resolve, bool ic, bool deleteAfter = false)
        {
            List<FileOperationResult> result;
            if (foldersId.Count > 0 || filesId.Count > 0)
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

        public async Task<List<FileOperationResult>> EmptyTrashAsync()
        {
            var folderDao = GetFolderDao();
            var fileDao = GetFileDao();
            var trashId = await folderDao.GetFolderIDTrashAsync(true);
            var foldersIdTask = await folderDao.GetFoldersAsync(trashId).Select(f => f.ID).ToListAsync();
            var filesIdTask = await fileDao.GetFilesAsync(trashId);

            return FileOperationsManager.Delete(AuthContext.CurrentAccount.ID, TenantManager.GetCurrentTenant(), foldersIdTask, filesIdTask, false, true, false, GetHttpHeaders());
        }

        public async IAsyncEnumerable<FileOperationResult> CheckConversionAsync(List<CheckConversionModel<T>> filesInfoJSON, bool sync = false)
        {
            if (filesInfoJSON == null || filesInfoJSON.Count == 0) yield break;

            var results = AsyncEnumerable.Empty<FileOperationResult>();
            var fileDao = GetFileDao();
            var files = new List<KeyValuePair<File<T>, bool>>();
            foreach (var fileInfo in filesInfoJSON)
            {
                var file = fileInfo.Version > 0
                                ? await fileDao.GetFileAsync(fileInfo.FileId, fileInfo.Version)
                                : await fileDao.GetFileAsync(fileInfo.FileId);

                if (file == null)
                {
                    var newFile = ServiceProvider.GetService<File<T>>();
                    newFile.ID = fileInfo.FileId;
                    newFile.Version = fileInfo.Version;

                    files.Add(new KeyValuePair<File<T>, bool>(newFile, true));
                    continue;
                }

                ErrorIf(!await FileSecurity.CanReadAsync(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

                if (fileInfo.StartConvert && FileConverter.MustConvert(file))
                {
                    try
                    {
                        if (sync)
                        {
                            results.Append(await FileConverter.ExecSynchronouslyAsync(file, fileInfo.Password));
                        }
                        else
                        {
                            await FileConverter.ExecAsynchronouslyAsync(file, false, fileInfo.Password);
                        }
                    }
                    catch (Exception e)
                    {
                        throw GenerateException(e);
                    }
                }

                files.Add(new KeyValuePair<File<T>, bool>(file, false));
            }

            if (!sync)
            {
                results = FileConverter.GetStatusAsync(files);
            }

            await foreach (var res in results)
            {
                yield return res;
            }
        }

        public async Task<string> CheckFillFormDraftAsync(T fileId, int version, string doc, bool editPossible, bool view)
        {
            var (file, _configuration) = await DocumentServiceHelper.GetParamsAsync(fileId, version, doc, editPossible, !view, true);
            var _valideShareLink = !string.IsNullOrEmpty(FileShareLink.Parse(doc));

            if (_valideShareLink)
            {
                _configuration.Document.SharedLinkKey += doc;
            }

            if (_configuration.EditorConfig.ModeWrite
                && FileUtility.CanWebRestrictedEditing(file.Title)
                && await FileSecurity.CanFillFormsAsync(file)
                && !await FileSecurity.CanEditAsync(file))
            {
                if (!file.IsFillFormDraft)
                {
                    await FileMarker.RemoveMarkAsNewAsync(file);

                    Folder<int> folderIfNew;
                    File<int> form;
                    try
                    {
                        (form, folderIfNew) = await EntryManager.GetFillFormDraftAsync(file);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("DocEditor", ex);
                        throw;
                    }

                    var comment = folderIfNew == null
                        ? string.Empty
                        : "#message/" + HttpUtility.UrlEncode(string.Format(FilesCommonResource.MessageFillFormDraftCreated, folderIfNew.Title));

                    return FilesLinkUtility.GetFileWebEditorUrl(form.ID) + comment;
                }
                else if (!await EntryManager.CheckFillFormDraftAsync(file))
                {
                    var comment = "#message/" + HttpUtility.UrlEncode(FilesCommonResource.MessageFillFormDraftDiscard);

                    return FilesLinkUtility.GetFileWebEditorUrl(file.ID) + comment;
                }
            }

            return FilesLinkUtility.GetFileWebEditorUrl(file.ID);
        }

        public async Task ReassignStorageAsync(Guid userFromId, Guid userToId)
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
                var providersInfo = await providerDao.GetProvidersInfoAsync(userFrom.ID).ToListAsync();
                var commonProvidersInfo = providersInfo.Where(provider => provider.RootFolderType == FolderType.COMMON);

                //move common thirdparty storage userFrom
                foreach (var commonProviderInfo in commonProvidersInfo)
                {
                    Logger.InfoFormat("Reassign provider {0} from {1} to {2}", commonProviderInfo.ID, userFrom.ID, userTo.ID);
                    await providerDao.UpdateProviderInfoAsync(commonProviderInfo.ID, null, null, FolderType.DEFAULT, userTo.ID);
                }
            }

            var folderDao = GetFolderDao();
            var fileDao = GetFileDao();

            if (!userFrom.IsVisitor(UserManager))
            {
                var folderIdFromMy = await folderDao.GetFolderIDUserAsync(false, userFrom.ID);

                if (!Equals(folderIdFromMy, 0))
                {
                    //create folder with name userFrom in folder userTo
                    var folderIdToMy = await folderDao.GetFolderIDUserAsync(true, userTo.ID);
                    var newFolder = ServiceProvider.GetService<Folder<T>>();
                    newFolder.Title = string.Format(CustomNamingPeople.Substitute<FilesCommonResource>("TitleDeletedUserFolder"), userFrom.DisplayUserName(false, DisplayUserSettingsHelper));
                    newFolder.FolderID = folderIdToMy;

                    var newFolderTo = await folderDao.SaveFolderAsync(newFolder);

                    //move items from userFrom to userTo
                    await EntryManager.MoveSharedItemsAsync(folderIdFromMy, newFolderTo, folderDao, fileDao);

                    await EntryManager.ReassignItemsAsync(newFolderTo, userFrom.ID, userTo.ID, folderDao, fileDao);
                }
            }

            await EntryManager.ReassignItemsAsync(await GlobalFolderHelper.GetFolderCommonAsync<T>(), userFrom.ID, userTo.ID, folderDao, fileDao);
        }

        public async Task DeleteStorageAsync(Guid userId)
        {
            //check current user have access
            ErrorIf(!Global.IsAdministrator, FilesCommonResource.ErrorMassage_SecurityException);

            //delete docuSign
            DocuSignToken.DeleteToken(userId);

            var providerDao = GetProviderDao();
            if (providerDao != null)
            {
                var providersInfo = await providerDao.GetProvidersInfoAsync(userId).ToListAsync();

                //delete thirdparty storage
                foreach (var myProviderInfo in providersInfo)
                {
                    Logger.InfoFormat("Delete provider {0} for {1}", myProviderInfo.ID, userId);
                    await providerDao.RemoveProviderInfoAsync(myProviderInfo.ID);
                }
            }

            var folderDao = GetFolderDao();
            var fileDao = GetFileDao();
            var linkDao = GetLinkDao();

            //delete all markAsNew
            var rootFoldersId = new List<T>
                {
                    await GlobalFolderHelper.GetFolderShareAsync<T>(),
                    await GlobalFolderHelper.GetFolderCommonAsync<T>(),
                    await GlobalFolderHelper.GetFolderProjectsAsync<T>(),
                };

            var folderIdFromMy = await folderDao.GetFolderIDUserAsync(false, userId);
            if (!Equals(folderIdFromMy, 0))
            {
                rootFoldersId.Add(folderIdFromMy);
            }

            var rootFolders = await folderDao.GetFoldersAsync(rootFoldersId).ToListAsync();
            foreach (var rootFolder in rootFolders)
            {
                await FileMarker.RemoveMarkAsNewAsync(rootFolder, userId);
            }

            //delete all from My
            if (!Equals(folderIdFromMy, 0))
            {
                await EntryManager.DeleteSubitemsAsync(folderIdFromMy, folderDao, fileDao, linkDao);

                //delete My userFrom folder
                await folderDao.DeleteFolderAsync(folderIdFromMy);
                GlobalFolderHelper.SetFolderMy(userId);
            }

            //delete all from Trash
            var folderIdFromTrash = await folderDao.GetFolderIDTrashAsync(false, userId);
            if (!Equals(folderIdFromTrash, 0))
            {
                await EntryManager.DeleteSubitemsAsync(folderIdFromTrash, folderDao, fileDao, linkDao);
                await folderDao.DeleteFolderAsync(folderIdFromTrash);
                GlobalFolderHelper.FolderTrash = userId;
            }

            await EntryManager.ReassignItemsAsync(await GlobalFolderHelper.GetFolderCommonAsync<T>(), userId, AuthContext.CurrentAccount.ID, folderDao, fileDao);
        }
        #region Favorites Manager

        public async Task<bool> ToggleFileFavoriteAsync(T fileId, bool favorite)
        {
            if (favorite)
            {
                await AddToFavoritesAsync(new List<T>(0), new List<T>(1) { fileId });
            }
            else
            {
                await DeleteFavoritesAsync(new List<T>(0), new List<T>(1) { fileId });
            }
            return favorite;
        }

        public Task<List<FileEntry<T>>> AddToFavoritesAsync(IEnumerable<T> foldersId, IEnumerable<T> filesId)
        {
            if (UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);

            return InternalAddToFavoritesAsync(foldersId, filesId);
        }

        private async Task<List<FileEntry<T>>> InternalAddToFavoritesAsync(IEnumerable<T> foldersId, IEnumerable<T> filesId)
        {
            var tagDao = GetTagDao();
            var fileDao = GetFileDao();
            var folderDao = GetFolderDao();
            var entries = Enumerable.Empty<FileEntry<T>>();

            var files = await fileDao.GetFilesAsync(filesId).Where(file => !file.Encrypted).ToListAsync();
            files = (await FileSecurity.FilterReadAsync(files)).ToList();
            entries = entries.Concat(files);

            var folders = await folderDao.GetFoldersAsync(foldersId).ToListAsync();
            folders = (await FileSecurity.FilterReadAsync(folders)).ToList();
            entries = entries.Concat(folders);

            var tags = entries.Select(entry => Tag.Favorite(AuthContext.CurrentAccount.ID, entry));

            tagDao.SaveTags(tags);

            return new List<FileEntry<T>>(entries);
        }

        public async Task<List<FileEntry<T>>> DeleteFavoritesAsync(IEnumerable<T> foldersId, IEnumerable<T> filesId)
        {
            var tagDao = GetTagDao();
            var fileDao = GetFileDao();
            var folderDao = GetFolderDao();
            var entries = Enumerable.Empty<FileEntry<T>>();

            var files = await fileDao.GetFilesAsync(filesId).ToListAsync();
            var filtredFiles = await FileSecurity.FilterReadAsync(files);
            entries = entries.Concat(filtredFiles);

            var folders = await folderDao.GetFoldersAsync(foldersId).ToListAsync();
            var filtredFolders = await FileSecurity.FilterReadAsync(folders);
            entries = entries.Concat(filtredFolders);

            var tags = entries.Select(entry => Tag.Favorite(AuthContext.CurrentAccount.ID, entry));

            tagDao.RemoveTags(tags);

            return new List<FileEntry<T>>(entries);
        }

        #endregion

        #region Templates Manager

        public Task<List<FileEntry<T>>> AddToTemplatesAsync(IEnumerable<T> filesId)
        {
            if (UserManager.GetUsers(AuthContext.CurrentAccount.ID).IsVisitor(UserManager)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);

            return InternalAddToTemplatesAsync(filesId);
        }

        private async Task<List<FileEntry<T>>> InternalAddToTemplatesAsync(IEnumerable<T> filesId)
        {
            var tagDao = GetTagDao();
            var fileDao = GetFileDao();
            var files = await fileDao.GetFilesAsync(filesId).ToListAsync();

            var filteredFiles = await FileSecurity.FilterReadAsync(files);
            files = filteredFiles
                .Where(file => FileUtility.ExtsWebTemplate.Contains(FileUtility.GetFileExtension(file.Title), StringComparer.CurrentCultureIgnoreCase))
                .ToList();

            var tags = files.Select(file => Tag.Template(AuthContext.CurrentAccount.ID, file));

            tagDao.SaveTags(tags);

            return new List<FileEntry<T>>(files);
        }

        public async Task<List<FileEntry<T>>> DeleteTemplatesAsync(IEnumerable<T> filesId)
        {
            var tagDao = GetTagDao();
            var fileDao = GetFileDao();
            var files = await fileDao.GetFilesAsync(filesId).ToListAsync();

            var filteredFiles = await FileSecurity.FilterReadAsync(files);
            files = filteredFiles.ToList();

            var tags = files.Select(file => Tag.Template(AuthContext.CurrentAccount.ID, file));

            tagDao.RemoveTags(tags);

            return new List<FileEntry<T>>(files);
        }

        public async Task<List<FileEntry<T>>> GetTemplatesAsync(FilterType filter, int from, int count, bool subjectGroup, string subjectID, string search, bool searchInContent)
        {
            try
            {
                IEnumerable<File<T>> result;

                var subjectId = string.IsNullOrEmpty(subjectID) ? Guid.Empty : new Guid(subjectID);
                var folderDao = GetFolderDao();
                var fileDao = GetFileDao();
                result = await EntryManager.GetTemplatesAsync(folderDao, fileDao, filter, subjectGroup, subjectId, search, searchInContent);

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

        public Task<List<AceWrapper>> GetSharedInfoAsync(IEnumerable<T> fileIds, IEnumerable<T> folderIds)
        {
            return FileSharing.GetSharedInfoAsync(fileIds, folderIds);
        }

        public Task<List<AceShortWrapper>> GetSharedInfoShortFileAsync(T fileId)
        {
            return FileSharing.GetSharedInfoShortFileAsync(fileId);
        }

        public Task<List<AceShortWrapper>> GetSharedInfoShortFolder(T folderId)
        {
            return FileSharing.GetSharedInfoShortFolderAsync(folderId);
        }

        public async Task<List<T>> SetAceObjectAsync(AceCollection<T> aceCollection, bool notify)
        {
            var fileDao = GetFileDao();
            var folderDao = GetFolderDao();
            var result = new List<T>();

            var entries = new List<FileEntry<T>>();

            foreach (var fileId in aceCollection.Files)
            {
                entries.Add(await fileDao.GetFileAsync(fileId));
            }

            foreach (var folderId in aceCollection.Folders)
            {
                entries.Add(await folderDao.GetFolderAsync(folderId));
            }

            foreach (var entry in entries)
            {
                try
                {
                    var changed = await FileSharingAceHelper.SetAceObjectAsync(aceCollection.Aces, entry, notify, aceCollection.Message);
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
                if (await securityDao.IsSharedAsync(entry.ID, entry.FileEntryType))
                {
                    result.Add(entry.ID);
                }
            }
            return result;
        }

        public Task RemoveAceAsync(List<T> filesId, List<T> foldersId)
        {
            ErrorIf(!AuthContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);
            var entries = AsyncEnumerable.Empty<FileEntry<T>>();

            var fileDao = GetFileDao();
            var folderDao = GetFolderDao();
            entries.Concat(filesId.ToAsyncEnumerable().SelectAwait(async fileId => await fileDao.GetFileAsync(fileId)));
            entries.Concat(foldersId.ToAsyncEnumerable().SelectAwait(async e => await folderDao.GetFolderAsync(e)));


            return FileSharingAceHelper.RemoveAceAsync(entries);
        }

        public async Task<string> GetShortenLinkAsync(T fileId)
        {
            File<T> file;
            var fileDao = GetFileDao();
            file = await fileDao.GetFileAsync(fileId);
            ErrorIf(!await FileSharing.CanSetAccessAsync(file), FilesCommonResource.ErrorMassage_SecurityException);
            var shareLink = FileShareLink.GetLink(file);

            try
            {
                return await UrlShortener.Instance.GetShortenLinkAsync(shareLink);
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }
        }

        public async Task<bool> SetAceLinkAsync(T fileId, FileShare share)
        {
            FileEntry<T> file;
            var fileDao = GetFileDao();
            file = await fileDao.GetFileAsync(fileId);
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

                var changed = await FileSharingAceHelper.SetAceObjectAsync(aces, file, false, null);
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
            return await securityDao.IsSharedAsync(file.ID, FileEntryType.File);
        }

        public Task<List<MentionWrapper>> SharedUsersAsync(T fileId)
        {
            if (!AuthContext.IsAuthenticated || CoreBaseSettings.Personal)
                return Task.FromResult<List<MentionWrapper>>(null);

            return InternalSharedUsersAsync(fileId);
        }

        public async Task<List<MentionWrapper>> InternalSharedUsersAsync(T fileId)
        {

            FileEntry<T> file;
            var fileDao = GetFileDao();
            file = await fileDao.GetFileAsync(fileId);

            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);

            var usersIdWithAccess = new List<Guid>();
            if (await FileSharing.CanSetAccessAsync(file))
            {
                var access = await FileSharing.GetSharedInfoAsync(file);
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

        public async Task<List<AceShortWrapper>> SendEditorNotifyAsync(T fileId, MentionMessageWrapper mentionMessage)
        {
            ErrorIf(!AuthContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);

            File<T> file;
            var fileDao = GetFileDao();
            file = await fileDao.GetFileAsync(fileId);

            ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);

            var fileSecurity = FileSecurity;
            var canRead = await fileSecurity.CanReadAsync(file);
            ErrorIf(!canRead, FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
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
                    canShare = await FileSharing.CanSetAccessAsync(file);
                }

                var recipient = UserManager.GetUserByEmail(email);
                if (recipient == null || recipient.ID == Constants.LostUser.ID)
                {
                    showSharingSettings = canShare.Value;
                    continue;
                }

                if (!await fileSecurity.CanReadAsync(file, recipient.ID))
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

                        showSharingSettings |= await FileSharingAceHelper.SetAceObjectAsync(aces, file, false, null);

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

            return showSharingSettings ? await GetSharedInfoShortFileAsync(fileId) : null;
        }

        public async Task<List<EncryptionKeyPair>> GetEncryptionAccessAsync(T fileId)
        {
            ErrorIf(!PrivacyRoomSettings.GetEnabled(SettingsManager), FilesCommonResource.ErrorMassage_SecurityException);

            var fileKeyPair = await EncryptionKeyPairHelper.GetKeyPairAsync(fileId, this);
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

        public async IAsyncEnumerable<FileEntry> ChangeOwnerAsync(IEnumerable<T> foldersId, IEnumerable<T> filesId, Guid userId)
        {
            var userInfo = UserManager.GetUsers(userId);
            ErrorIf(Equals(userInfo, Constants.LostUser) || userInfo.IsVisitor(UserManager), FilesCommonResource.ErrorMassage_ChangeOwner);

            var entries = AsyncEnumerable.Empty<FileEntry>();

            var folderDao = GetFolderDao();
            var folders = folderDao.GetFoldersAsync(foldersId);
            await foreach (var folder in folders)
            {
                ErrorIf(!await FileSecurity.CanEditAsync(folder), FilesCommonResource.ErrorMassage_SecurityException);
                ErrorIf(folder.RootFolderType != FolderType.COMMON, FilesCommonResource.ErrorMassage_SecurityException);
                if (folder.ProviderEntry) continue;

                var newFolder = folder;
                if (folder.CreateBy != userInfo.ID)
                {
                    var folderAccess = folder.Access;

                    newFolder.CreateBy = userInfo.ID;
                    var newFolderID = await folderDao.SaveFolderAsync(newFolder);

                    newFolder = await folderDao.GetFolderAsync(newFolderID);
                    newFolder.Access = folderAccess;

                    FilesMessageService.Send(newFolder, GetHttpHeaders(), MessageAction.FileChangeOwner, new[] { newFolder.Title, userInfo.DisplayUserName(false, DisplayUserSettingsHelper) });
                }
                entries.Append(newFolder);
            }

            var fileDao = GetFileDao();
            var files = fileDao.GetFilesAsync(filesId);

            await foreach (var file in files)
            {
                ErrorIf(!await FileSecurity.CanEditAsync(file), FilesCommonResource.ErrorMassage_SecurityException);
                ErrorIf(await EntryManager.FileLockedForMeAsync(file.ID), FilesCommonResource.ErrorMassage_LockedFile);
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

                    using (var stream = await fileDao.GetFileStreamAsync(file))
                    {
                        newFile.ContentLength = stream.CanSeek ? stream.Length : file.ContentLength;
                        newFile = await fileDao.SaveFileAsync(newFile, stream);
                    }

                    if (file.ThumbnailStatus == Thumbnail.Created)
                    {
                        using (var thumbnail = await fileDao.GetThumbnailAsync(file))
                        {
                            await fileDao.SaveThumbnailAsync(newFile, thumbnail);
                        }
                        newFile.ThumbnailStatus = Thumbnail.Created;
                    }

                    await FileMarker.MarkAsNewAsync(newFile);

                    await EntryStatusManager.SetFileStatusAsync(newFile);

                    FilesMessageService.Send(newFile, GetHttpHeaders(), MessageAction.FileChangeOwner, new[] { newFile.Title, userInfo.DisplayUserName(false, DisplayUserSettingsHelper) });
                }
                entries.Append(newFile);
            }

            await foreach (var entrie in entries)
            {
                yield return entrie;
            }
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

        public ICompress ChangeDownloadTarGz(bool set)
        {
            FilesSettingsHelper.DownloadTarGz = set;

            return CompressToArchive;
        }

        public bool ChangeDeleteConfrim(bool set)
        {
            FilesSettingsHelper.ConfirmDelete = set;

            return FilesSettingsHelper.ConfirmDelete;
        }

        public IEnumerable<JsonElement> CreateThumbnails(List<JsonElement> fileIds)
        {
            try
            {
                var req = new ThumbnailRequest()
                {
                    Tenant = TenantManager.GetCurrentTenant().TenantId,
                    BaseUrl = BaseCommonLinkUtility.GetFullAbsolutePath("")
                };

                var (fileIntIds, _) = FileOperationsManager.GetIds(fileIds);

                foreach (var f in fileIntIds)
                {
                    req.Files.Add(f);
                }

                ThumbnailNotify.Publish(req, CacheNotifyAction.Insert);
            }
            catch (Exception e)
            {
                Logger.Error("CreateThumbnails", e);
            }

            return fileIds;
        }

        public async Task<IEnumerable<JsonElement>> CreateThumbnailsAsync(List<JsonElement> fileIds)
        {
            try
            {
                var req = new ThumbnailRequest()
                {
                    Tenant = TenantManager.GetCurrentTenant().TenantId,
                    BaseUrl = BaseCommonLinkUtility.GetFullAbsolutePath("")
                };

                var (fileIntIds, _) = FileOperationsManager.GetIds(fileIds);

                foreach (var f in fileIntIds)
                {
                    req.Files.Add(f);
                }

                await ThumbnailNotify.PublishAsync(req, CacheNotifyAction.Insert);
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

        private ILinkDao GetLinkDao()
        {
            return DaoFactory.GetLinkDao();
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

    public class FileModel<T, TTempate>
    {
        public T ParentId { get; set; }
        public string Title { get; set; }
        public TTempate TemplateId { get; set; }
    }
}