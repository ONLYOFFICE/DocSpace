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


using UrlShortener = ASC.Web.Core.Utility.UrlShortener;

namespace ASC.Web.Files.Services.WCFService;

[Scope]
public class FileStorageService<T> //: IFileStorageService
{
    public CompressToArchive CompressToArchive { get; }

    private static readonly FileEntrySerializer _serializer = new FileEntrySerializer();
    private readonly Global _global;
    private readonly GlobalStore _globalStore;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly FilesSettingsHelper _filesSettingsHelper;
    private readonly AuthContext _authContext;
    private readonly UserManager _userManager;
    private readonly FileUtility _fileUtility;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly BaseCommonLinkUtility _baseCommonLinkUtility;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CustomNamingPeople _customNamingPeople;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly PathProvider _pathProvider;
    private readonly FileSecurity _fileSecurity;
    private readonly SocketManager _socketManager;
    private readonly IDaoFactory _daoFactory;
    private readonly FileMarker _fileMarker;
    private readonly EntryManager _entryManager;
    private readonly FilesMessageService _filesMessageService;
    private readonly DocumentServiceTrackerHelper _documentServiceTrackerHelper;
    private readonly DocuSignToken _docuSignToken;
    private readonly DocuSignHelper _docuSignHelper;
    private readonly FileShareLink _fileShareLink;
    private readonly FileConverter _fileConverter;
    private readonly DocumentServiceHelper _documentServiceHelper;
    private readonly ThirdpartyConfiguration _thirdpartyConfiguration;
    private readonly DocumentServiceConnector _documentServiceConnector;
    private readonly FileSharing _fileSharing;
    private readonly NotifyClient _notifyClient;
    private readonly UrlShortener _urlShortener;
    private readonly IServiceProvider _serviceProvider;
    private readonly FileSharingAceHelper<T> _fileSharingAceHelper;
    private readonly ConsumerFactory _consumerFactory;
    private readonly EncryptionKeyPairHelper _encryptionKeyPairHelper;
    private readonly SettingsManager _settingsManager;
    private readonly FileOperationsManager _fileOperationsManager;
    private readonly TenantManager _tenantManager;
    private readonly FileTrackerHelper _fileTracker;
    private readonly ICacheNotify<ThumbnailRequest> _thumbnailNotify;
    private readonly EntryStatusManager _entryStatusManager;
    private ILog _logger;

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
        _global = global;
        _globalStore = globalStore;
        _globalFolderHelper = globalFolderHelper;
        _filesSettingsHelper = filesSettingsHelper;
        _authContext = authContext;
        _userManager = userManager;
        _fileUtility = fileUtility;
        _filesLinkUtility = filesLinkUtility;
        _baseCommonLinkUtility = baseCommonLinkUtility;
        _coreBaseSettings = coreBaseSettings;
        _customNamingPeople = customNamingPeople;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _httpContextAccessor = httpContextAccessor;
        _pathProvider = pathProvider;
        _fileSecurity = fileSecurity;
        _socketManager = socketManager;
        _daoFactory = daoFactory;
        _fileMarker = fileMarker;
        _entryManager = entryManager;
        _filesMessageService = filesMessageService;
        _documentServiceTrackerHelper = documentServiceTrackerHelper;
        _docuSignToken = docuSignToken;
        _docuSignHelper = docuSignHelper;
        _fileShareLink = fileShareLink;
        _fileConverter = fileConverter;
        _documentServiceHelper = documentServiceHelper;
        _thirdpartyConfiguration = thirdpartyConfiguration;
        _documentServiceConnector = documentServiceConnector;
        _fileSharing = fileSharing;
        _notifyClient = notifyClient;
        _urlShortener = urlShortener;
        _serviceProvider = serviceProvider;
        _fileSharingAceHelper = fileSharingAceHelper;
        _consumerFactory = consumerFactory;
        _encryptionKeyPairHelper = encryptionKeyPairHelper;
        _settingsManager = settingsManager;
        _logger = optionMonitor.Get("ASC.Files");
        _fileOperationsManager = fileOperationsManager;
        _tenantManager = tenantManager;
        _fileTracker = fileTracker;
        _thumbnailNotify = thumbnailNotify;
        _entryStatusManager = entryStatusManager;
        CompressToArchive = compressToArchive;
    }

    public async Task<Folder<T>> GetFolderAsync(T folderId)
    {
        var folderDao = GetFolderDao();
        var folder = await folderDao.GetFolderAsync(folderId);

        ErrorIf(folder == null, FilesCommonResource.ErrorMassage_FolderNotFound);
        ErrorIf(!await _fileSecurity.CanReadAsync(folder), FilesCommonResource.ErrorMassage_SecurityException_ReadFolder);

        return folder;
    }

    public async Task<List<FileEntry>> GetFoldersAsync(T parentId)
    {
        var folderDao = GetFolderDao();

        try
        {
            var entries = await _entryManager.GetEntriesAsync(
                await folderDao.GetFolderAsync(parentId), 0, 0, FilterType.FoldersOnly,
                false, Guid.Empty, string.Empty, false, false, new OrderBy(SortedByType.AZ, true));

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
        ErrorIf(!await _fileSecurity.CanReadAsync(folder), FilesCommonResource.ErrorMassage_SecurityException_ViewFolder);

        var breadCrumbs = await _entryManager.GetBreadCrumbsAsync(folderId, folderDao);

        return new List<object>(breadCrumbs.Select(f =>
        {
            if (f is Folder<string> f1)
            {
                return (object)f1.ID;
            }

            if (f is Folder<int> f2)
            {
                return f2.ID;
            }

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
            if (parent != null && !string.IsNullOrEmpty(parent.Error))
            {
                throw new Exception(parent.Error);
            }
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
        ErrorIf(!await _fileSecurity.CanReadAsync(parent), FilesCommonResource.ErrorMassage_SecurityException_ViewFolder);
        ErrorIf(parent.RootFolderType == FolderType.TRASH && !Equals(parent.ID, _globalFolderHelper.FolderTrash), FilesCommonResource.ErrorMassage_ViewTrashItem);

        if (orderBy != null)
        {
            _filesSettingsHelper.DefaultOrder = orderBy;
        }
        else
        {
            orderBy = _filesSettingsHelper.DefaultOrder;
        }

        if (Equals(parent.ID, await _globalFolderHelper.FolderShareAsync) && orderBy.SortedBy == SortedByType.DateAndTime)
        {
            orderBy.SortedBy = SortedByType.New;
        }

        int total;
        IEnumerable<FileEntry> entries;
        try
        {
            (entries, total) = await _entryManager.GetEntriesAsync(parent, from, count, filter, subjectGroup, subjectId, searchText, searchInContent, withSubfolders, orderBy);
        }
        catch (Exception e)
        {
            if (parent.ProviderEntry)
            {
                throw GenerateException(new Exception(FilesCommonResource.ErrorMassage_SharpBoxException, e));
            }

            throw GenerateException(e);
        }

        var breadCrumbs = await _entryManager.GetBreadCrumbsAsync(parentId, folderDao);

        var prevVisible = breadCrumbs.ElementAtOrDefault(breadCrumbs.Count - 2);
        if (prevVisible != null)
        {
            if (prevVisible is Folder<string> f1)
            {
                parent.FolderID = (T)Convert.ChangeType(f1.ID, typeof(T));
            }

            if (prevVisible is Folder<int> f2)
            {
                parent.FolderID = (T)Convert.ChangeType(f2.ID, typeof(T));
            }
        }

        parent.Shareable = await _fileSharing.CanSetAccessAsync(parent)
            || parent.FolderType == FolderType.SHARE
            || parent.RootFolderType == FolderType.Privacy;

        entries = entries.Where(x => x.FileEntryType == FileEntryType.Folder ||
        x is File<string> f1 && !_fileConverter.IsConverting(f1) ||
         x is File<int> f2 && !_fileConverter.IsConverting(f2));

        var result = new DataWrapper<T>
        {
            Total = total,
            Entries = new List<FileEntry>(entries.ToList()),
            FolderPathParts = new List<object>(breadCrumbs.Select(f =>
            {
                if (f is Folder<string> f1)
                {
                    return (object)f1.ID;
                }

                if (f is Folder<int> f2)
                {
                    return f2.ID;
                }

                return 0;
            })),
            FolderInfo = parent,
            New = await _fileMarker.GetRootFoldersIdMarkedAsNewAsync(parentId)
        };

        return result;
    }

    public async Task<object> GetFolderItemsXmlAsync(T parentId, int from, int count, FilterType filter, bool subjectGroup, string subjectID, string search, bool searchInContent, bool withSubfolders, OrderBy orderBy)
    {
        var folderItems = await GetFolderItemsAsync(parentId, from, count, filter, subjectGroup, subjectID, search, searchInContent, withSubfolders, orderBy);
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StreamContent(_serializer.ToXml(folderItems))
        };
        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
        return response;
    }

    public async Task<List<FileEntry>> GetItemsAsync<TId>(IEnumerable<TId> filesId, IEnumerable<TId> foldersId, FilterType filter, bool subjectGroup, string subjectID, string search)
    {
        var subjectId = string.IsNullOrEmpty(subjectID) ? Guid.Empty : new Guid(subjectID);

        var entries = Enumerable.Empty<FileEntry<TId>>();

        var folderDao = _daoFactory.GetFolderDao<TId>();
        var fileDao = _daoFactory.GetFileDao<TId>();
        var folders = await folderDao.GetFoldersAsync(foldersId).ToListAsync();
        var tmpFolders = await _fileSecurity.FilterReadAsync(folders);
        folders = tmpFolders.ToList();
        entries = entries.Concat(folders);

        var files = await fileDao.GetFilesAsync(filesId).ToListAsync();
        files = (await _fileSecurity.FilterReadAsync(files)).ToList();
        entries = entries.Concat(files);

        entries = _entryManager.FilterEntries(entries, filter, subjectGroup, subjectId, search, true);

        foreach (var fileEntry in entries)
        {
            if (fileEntry is File<TId> file)
            {
                if (fileEntry.RootFolderType == FolderType.USER
                    && !Equals(fileEntry.RootFolderCreator, _authContext.CurrentAccount.ID)
                    && !await _fileSecurity.CanReadAsync(await folderDao.GetFolderAsync(file.FolderIdDisplay)))
                {
                    file.FolderIdDisplay = await _globalFolderHelper.GetFolderShareAsync<TId>();
                }
            }
            else if (fileEntry is Folder<TId> folder)
            {
                if (fileEntry.RootFolderType == FolderType.USER
                    && !Equals(fileEntry.RootFolderCreator, _authContext.CurrentAccount.ID)
                    && !await _fileSecurity.CanReadAsync(await folderDao.GetFolderAsync(folder.FolderIdDisplay)))
                {
                    folder.FolderIdDisplay = await _globalFolderHelper.GetFolderShareAsync<TId>();
                }
            }
        }

        await _entryStatusManager.SetFileStatusAsync(entries);

        return new List<FileEntry>(entries);
    }

    public Task<Folder<T>> CreateNewFolderAsync(T parentId, string title)
    {
        if (string.IsNullOrEmpty(title) || parentId == null)
        {
            throw new ArgumentException();
        }

        return InternalCreateNewFolderAsync(parentId, title);
    }

    public async Task<Folder<T>> InternalCreateNewFolderAsync(T parentId, string title)
    {
        var folderDao = GetFolderDao();

        var parent = await folderDao.GetFolderAsync(parentId);
        ErrorIf(parent == null, FilesCommonResource.ErrorMassage_FolderNotFound);
        ErrorIf(!await _fileSecurity.CanCreateAsync(parent), FilesCommonResource.ErrorMassage_SecurityException_Create);

        try
        {
            var newFolder = _serviceProvider.GetService<Folder<T>>();
            newFolder.Title = title;
            newFolder.FolderID = parent.ID;

            var folderId = await folderDao.SaveFolderAsync(newFolder);
            var folder = await folderDao.GetFolderAsync(folderId);
            _filesMessageService.Send(folder, GetHttpHeaders(), MessageAction.FolderCreated, folder.Title);

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
        ErrorIf(!await _fileSecurity.CanEditAsync(folder), FilesCommonResource.ErrorMassage_SecurityException_RenameFolder);
        if (!await _fileSecurity.CanDeleteAsync(folder) && _userManager.GetUsers(_authContext.CurrentAccount.ID).IsVisitor(_userManager))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_RenameFolder);
        }

        ErrorIf(folder.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_ViewTrashItem);

        var folderAccess = folder.Access;

        if (!string.Equals(folder.Title, title, StringComparison.OrdinalIgnoreCase))
        {
            var newFolderID = await folderDao.RenameFolderAsync(folder, title);
            folder = await folderDao.GetFolderAsync(newFolderID);
            folder.Access = folderAccess;

            _filesMessageService.Send(folder, GetHttpHeaders(), MessageAction.FolderRenamed, folder.Title);

            //if (!folder.ProviderEntry)
            //{
            //    FoldersIndexer.IndexAsync(FoldersWrapper.GetFolderWrapper(ServiceProvider, folder));
            //}
        }

        var newTags = tagDao.GetNewTagsAsync(_authContext.CurrentAccount.ID, folder);
        var tag = await newTags.FirstOrDefaultAsync();
        if (tag != null)
        {
            folder.NewForMe = tag.Count;
        }

        if (folder.RootFolderType == FolderType.USER
            && !Equals(folder.RootFolderCreator, _authContext.CurrentAccount.ID)
            && !await _fileSecurity.CanReadAsync(await folderDao.GetFolderAsync(folder.FolderID)))
        {
            folder.FolderIdDisplay = await _globalFolderHelper.GetFolderShareAsync<T>();
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
        ErrorIf(!await _fileSecurity.CanReadAsync(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

        await _entryStatusManager.SetFileStatusAsync(file);

        if (file.RootFolderType == FolderType.USER
            && !Equals(file.RootFolderCreator, _authContext.CurrentAccount.ID))
        {
            var folderDao = GetFolderDao();
            if (!await _fileSecurity.CanReadAsync(await folderDao.GetFolderAsync(file.FolderID)))
            {
                file.FolderIdDisplay = await _globalFolderHelper.GetFolderShareAsync<T>();
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
        ErrorIf(!await _fileSecurity.CanReadAsync(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

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
            orderBy = _filesSettingsHelper.DefaultOrder;
        }
        if (Equals(parent.ID, await _globalFolderHelper.GetFolderShareAsync<T>()) && orderBy.SortedBy == SortedByType.DateAndTime)
        {
            orderBy.SortedBy = SortedByType.New;
        }

        var entries = Enumerable.Empty<FileEntry>();

        if (!await _fileSecurity.CanReadAsync(parent))
        {
            file.FolderID = await _globalFolderHelper.GetFolderShareAsync<T>();
            entries = entries.Concat(new[] { file });
        }
        else
        {
            try
            {
                (entries, var total) = await _entryManager.GetEntriesAsync(parent, 0, 0, filter, subjectGroup, subjectId, search, searchInContent, withSubfolders, orderBy);
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

        var result = await _fileSecurity.FilterReadAsync(entries.OfType<File<T>>());
        result = result.OfType<File<T>>().Where(f => previewedType.Contains(FileUtility.GetFileTypeByFileName(f.Title)));

        return new List<File<T>>(result);
    }

    public Task<File<T>> CreateNewFileAsync<TTemplate>(FileModel<T, TTemplate> fileWrapper, bool enableExternalExt = false)
    {
        if (string.IsNullOrEmpty(fileWrapper.Title) || fileWrapper.ParentId == null)
        {
            throw new ArgumentException();
        }

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
            var canCreate = await _fileSecurity.CanCreateAsync(folder);
            if (!canCreate)
            {
                folder = null;
            }
        }
        if (folder == null)
        {
            folder = await folderDao.GetFolderAsync(_globalFolderHelper.GetFolderMy<T>());
        }


        var file = _serviceProvider.GetService<File<T>>();
        file.FolderID = folder.ID;
        file.Comment = FilesCommonResource.CommentCreate;

        if (string.IsNullOrEmpty(fileWrapper.Title))
        {
            fileWrapper.Title = UserControlsCommonResource.NewDocument + ".docx";
        }

        var externalExt = false;
        var title = fileWrapper.Title;
        var fileExt = FileUtility.GetFileExtension(title);
        if (fileExt != _fileUtility.MasterFormExtension)
        {
            fileExt = _fileUtility.GetInternalExtension(title);
            if (!_fileUtility.InternalExtension.Values.Contains(fileExt))
            {
                fileExt = _fileUtility.InternalExtension[FileType.Document];
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
            var culture = _userManager.GetUsers(_authContext.CurrentAccount.ID).GetCulture();
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

                var pathThumb = path + fileExt.Trim('.') + "." + _global.ThumbnailExtension;
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
            var fileTemlateDao = _daoFactory.GetFileDao<TTemplate>();
            var template = await fileTemlateDao.GetFileAsync(fileWrapper.TemplateId);
            ErrorIf(template == null, FilesCommonResource.ErrorMassage_FileNotFound);
            ErrorIf(!await _fileSecurity.CanReadAsync(template), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

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

        _filesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileCreated, file.Title);

        await _fileMarker.MarkAsNewAsync(file);

        await _socketManager.CreateFileAsync(file);

        return file;
    }

    public async Task<KeyValuePair<bool, string>> TrackEditFileAsync(T fileId, Guid tabId, string docKeyForTrack, string doc = null, bool isFinish = false)
    {
        try
        {
            var id = _fileShareLink.Parse<T>(doc);
            if (id == null)
            {
                if (!_authContext.IsAuthenticated)
                {
                    throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
                }

                if (!string.IsNullOrEmpty(doc))
                {
                    throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
                }

                id = fileId;
            }

            if (docKeyForTrack != _documentServiceHelper.GetDocKey(id, -1, DateTime.MinValue))
            {
                throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
            }

            if (isFinish)
            {
                _fileTracker.Remove(id, tabId);
                await _socketManager.StopEditAsync(id);
            }
            else
            {
                await _entryManager.TrackEditingAsync(id, tabId, _authContext.CurrentAccount.ID, doc);
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
        ErrorIf(!_authContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);
        var result = new Dictionary<string, string>();

        var fileDao = GetFileDao();
        var ids = filesId.Where(_fileTracker.IsEditing).Select(id => id).ToList();

        await foreach (var file in fileDao.GetFilesAsync(ids))
        {
            if (file == null
                || !await _fileSecurity.CanEditAsync(file)
                && !await _fileSecurity.CanCustomFilterEditAsync(file)
                && !await _fileSecurity.CanReviewAsync(file)
                && !await _fileSecurity.CanFillFormsAsync(file)
                && !await _fileSecurity.CanCommentAsync(file))
            {
                continue;
            }

            var usersId = _fileTracker.GetEditingBy(file.ID);
            var value = string.Join(", ", usersId.Select(userId => _global.GetUserName(userId, true)).ToArray());
            result[file.ID.ToString()] = value;
        }

        return result;
    }

    public async Task<File<T>> SaveEditingAsync(T fileId, string fileExtension, string fileuri, Stream stream, string doc = null, bool forcesave = false)
    {
        try
        {
            if (!forcesave && _fileTracker.IsEditingAlone(fileId))
            {
                _fileTracker.Remove(fileId);
                await _socketManager.StopEditAsync(fileId);
            }

            var file = await _entryManager.SaveEditingAsync(fileId, fileExtension, fileuri, stream, doc, forcesave: forcesave ? ForcesaveType.User : ForcesaveType.None, keepLink: true);

            if (file != null)
            {
                _filesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileUpdated, file.Title);
            }

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
            if (!forcesave && _fileTracker.IsEditing(fileId))
            {
                _fileTracker.Remove(fileId);
                await _socketManager.StopEditAsync(fileId);
            }

            var file = await _entryManager.SaveEditingAsync(fileId,
                fileExtension,
                null,
                stream,
                null,
                encrypted ? FilesCommonResource.CommentEncrypted : null,
                encrypted: encrypted,
                forcesave: forcesave ? ForcesaveType.User : ForcesaveType.None);

            if (file != null)
            {
                _filesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileUpdated, file.Title);
            }

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
                ErrorIf(_fileTracker.IsEditing(fileId), FilesCommonResource.ErrorMassage_SecurityException_EditFileTwice);

                app = ThirdPartySelector.GetAppByFileId(fileId.ToString());
                if (app == null)
                {
                    await _entryManager.TrackEditingAsync(fileId, Guid.Empty, _authContext.CurrentAccount.ID, doc, true);
                }

                //without StartTrack, track via old scheme
                return _documentServiceHelper.GetDocKey(fileId, -1, DateTime.MinValue);
            }

            (File<string> File, Configuration<string> Configuration) fileOptions;

            app = ThirdPartySelector.GetAppByFileId(fileId.ToString());
            if (app == null)
            {
                fileOptions = await _documentServiceHelper.GetParamsAsync(fileId.ToString(), -1, doc, true, true, false);
            }
            else
            {
                var file = app.GetFile(fileId.ToString(), out var editable);
                fileOptions = await _documentServiceHelper.GetParamsAsync(file, true, editable ? FileShare.ReadWrite : FileShare.Read, false, editable, editable, editable, false);
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

            if (!await _documentServiceTrackerHelper.StartTrackAsync(fileId.ToString(), key))
            {
                throw new Exception(FilesCommonResource.ErrorMassage_StartEditing);
            }

            return key;
        }
        catch (Exception e)
        {
            _fileTracker.Remove(fileId);

            throw GenerateException(e);
        }
    }

    public async Task<File<T>> FileRenameAsync(T fileId, string title)
    {
        try
        {
            var fileRename = await _entryManager.FileRenameAsync(fileId, title);
            var file = fileRename.File;

            if (fileRename.Renamed)
            {
                _filesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileRenamed, file.Title);

                //if (!file.ProviderEntry)
                //{
                //    FilesIndexer.UpdateAsync(FilesWrapper.GetFilesWrapper(ServiceProvider, file), true, r => r.Title);
                //}
            }

            if (file.RootFolderType == FolderType.USER
                && !Equals(file.RootFolderCreator, _authContext.CurrentAccount.ID))
            {
                var folderDao = GetFolderDao();
                if (!await _fileSecurity.CanReadAsync(await folderDao.GetFolderAsync(file.FolderID)))
                {
                    file.FolderIdDisplay = await _globalFolderHelper.GetFolderShareAsync<T>();
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
        ErrorIf(!await _fileSecurity.CanReadAsync(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

        return await fileDao.GetFileHistoryAsync(fileId).ToListAsync();
    }

    public async Task<KeyValuePair<File<T>, List<File<T>>>> UpdateToVersionAsync(T fileId, int version)
    {
        var file = await _entryManager.UpdateToVersionFileAsync(fileId, version);
        _filesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileRestoreVersion, file.Title, version.ToString(CultureInfo.InvariantCulture));

        if (file.RootFolderType == FolderType.USER
            && !Equals(file.RootFolderCreator, _authContext.CurrentAccount.ID))
        {
            var folderDao = GetFolderDao();
            if (!await _fileSecurity.CanReadAsync(await folderDao.GetFolderAsync(file.FolderID)))
            {
                file.FolderIdDisplay = await _globalFolderHelper.GetFolderShareAsync<T>();
            }
        }

        return new KeyValuePair<File<T>, List<File<T>>>(file, await GetFileHistoryAsync(fileId));
    }

    public async Task<string> UpdateCommentAsync(T fileId, int version, string comment)
    {
        var fileDao = GetFileDao();
        var file = await fileDao.GetFileAsync(fileId, version);
        ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
        ErrorIf(!await _fileSecurity.CanEditAsync(file) || _userManager.GetUsers(_authContext.CurrentAccount.ID).IsVisitor(_userManager), FilesCommonResource.ErrorMassage_SecurityException_EditFile);
        ErrorIf(await _entryManager.FileLockedForMeAsync(file.ID), FilesCommonResource.ErrorMassage_LockedFile);
        ErrorIf(file.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_ViewTrashItem);

        comment = await fileDao.UpdateCommentAsync(fileId, version, comment);

        _filesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileUpdatedRevisionComment, file.Title, version.ToString(CultureInfo.InvariantCulture));

        return comment;
    }

    public async Task<KeyValuePair<File<T>, List<File<T>>>> CompleteVersionAsync(T fileId, int version, bool continueVersion)
    {
        var file = await _entryManager.CompleteVersionFileAsync(fileId, version, continueVersion);

        _filesMessageService.Send(file, GetHttpHeaders(),
                                 continueVersion ? MessageAction.FileDeletedVersion : MessageAction.FileCreatedVersion,
                                 file.Title, version == 0 ? (file.Version - 1).ToString(CultureInfo.InvariantCulture) : version.ToString(CultureInfo.InvariantCulture));

        if (file.RootFolderType == FolderType.USER
            && !Equals(file.RootFolderCreator, _authContext.CurrentAccount.ID))
        {
            var folderDao = GetFolderDao();
            if (!await _fileSecurity.CanReadAsync(await folderDao.GetFolderAsync(file.FolderID)))
            {
                file.FolderIdDisplay = await _globalFolderHelper.GetFolderShareAsync<T>();
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
        ErrorIf(!await _fileSecurity.CanEditAsync(file) || lockfile && _userManager.GetUsers(_authContext.CurrentAccount.ID).IsVisitor(_userManager), FilesCommonResource.ErrorMassage_SecurityException_EditFile);
        ErrorIf(file.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_ViewTrashItem);

        var tags = tagDao.GetTagsAsync(file.ID, FileEntryType.File, TagType.Locked);
        var tagLocked = await tags.FirstOrDefaultAsync();

        ErrorIf(tagLocked != null
                && tagLocked.Owner != _authContext.CurrentAccount.ID
                && !_global.IsAdministrator
                && (file.RootFolderType != FolderType.USER || file.RootFolderCreator != _authContext.CurrentAccount.ID), FilesCommonResource.ErrorMassage_LockedFile);

        if (lockfile)
        {
            if (tagLocked == null)
            {
                tagLocked = new Tag("locked", TagType.Locked, _authContext.CurrentAccount.ID, 0).AddEntry(file);

                tagDao.SaveTags(tagLocked);
            }

            var usersDrop = _fileTracker.GetEditingBy(file.ID).Where(uid => uid != _authContext.CurrentAccount.ID).Select(u => u.ToString()).ToArray();
            if (usersDrop.Length > 0)
            {
                var fileStable = file.Forcesave == ForcesaveType.None ? file : await fileDao.GetFileStableAsync(file.ID, file.Version);
                var docKey = _documentServiceHelper.GetDocKey(fileStable);
                await _documentServiceHelper.DropUserAsync(docKey, usersDrop, file.ID);
            }

            _filesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileLocked, file.Title);
        }
        else
        {
            if (tagLocked != null)
            {
                tagDao.RemoveTags(tagLocked);

                _filesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileUnlocked, file.Title);
            }

            if (!file.ProviderEntry)
            {
                file = await _entryManager.CompleteVersionFileAsync(file.ID, 0, false);
                await UpdateCommentAsync(file.ID, file.Version, FilesCommonResource.UnlockComment);
            }
        }

        await _entryStatusManager.SetFileStatusAsync(file);

        if (file.RootFolderType == FolderType.USER
            && !Equals(file.RootFolderCreator, _authContext.CurrentAccount.ID))
        {
            var folderDao = GetFolderDao();
            if (!await _fileSecurity.CanReadAsync(await folderDao.GetFolderAsync(file.FolderID)))
            {
                file.FolderIdDisplay = await _globalFolderHelper.GetFolderShareAsync<T>();
            }
        }

        return file;
    }

    public async Task<List<EditHistory>> GetEditHistoryAsync(T fileId, string doc = null)
    {
        var fileDao = GetFileDao();
        var (readLink, file) = await _fileShareLink.CheckAsync(doc, true, fileDao);
        if (file == null)
        {
            file = await fileDao.GetFileAsync(fileId);
        }

        ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
        ErrorIf(!readLink && !await _fileSecurity.CanReadAsync(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
        ErrorIf(file.ProviderEntry, FilesCommonResource.ErrorMassage_BadRequest);

        return new List<EditHistory>(await fileDao.GetEditHistoryAsync(_documentServiceHelper, file.ID));
    }

    public async Task<EditHistoryData> GetEditDiffUrlAsync(T fileId, int version = 0, string doc = null)
    {
        var fileDao = GetFileDao();
        var (readLink, file) = await _fileShareLink.CheckAsync(doc, true, fileDao);

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
        ErrorIf(!readLink && !await _fileSecurity.CanReadAsync(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
        ErrorIf(file.ProviderEntry, FilesCommonResource.ErrorMassage_BadRequest);

        var result = new EditHistoryData
        {
            Key = _documentServiceHelper.GetDocKey(file),
            Url = _documentServiceConnector.ReplaceCommunityAdress(_pathProvider.GetFileStreamUrl(file, doc)),
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

                sourceFileUrl = _pathProvider.GetFileStreamUrl(previousFileStable, doc);

                previouseKey = _documentServiceHelper.GetDocKey(previousFileStable);
                previousFileExt = FileUtility.GetFileExtension(previousFileStable.Title);
            }
            else
            {
                var culture = _userManager.GetUsers(_authContext.CurrentAccount.ID).GetCulture();
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
                sourceFileUrl = _baseCommonLinkUtility.GetFullAbsolutePath(sourceFileUrl);

                previouseKey = DocumentServiceConnector.GenerateRevisionId(Guid.NewGuid().ToString());
                previousFileExt = fileExt;
            }

            result.Previous = new EditHistoryUrl
            {
                Key = previouseKey,
                Url = _documentServiceConnector.ReplaceCommunityAdress(sourceFileUrl),
                FileType = GetFileExtensionWithoutDot(previousFileExt)
            };

            result.ChangesUrl = _pathProvider.GetFileChangesUrl(file, doc);
        }

        result.Token = _documentServiceHelper.GetSignature(result);

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
            file = await _entryManager.UpdateToVersionFileAsync(fileId, version, doc);
        }
        else
        {
            string modifiedOnString;
            fileDao = GetFileDao();
            var fromFile = await fileDao.GetFileAsync(fileId, version);
            modifiedOnString = fromFile.ModifiedOnString;
            file = await _entryManager.SaveEditingAsync(fileId, null, url, null, doc, string.Format(FilesCommonResource.CommentRevertChanges, modifiedOnString));
        }

        _filesMessageService.Send(file, _httpContextAccessor?.HttpContext?.Request?.Headers, MessageAction.FileRestoreVersion, file.Title, version.ToString(CultureInfo.InvariantCulture));

        fileDao = GetFileDao();

        return new List<EditHistory>(await fileDao.GetEditHistoryAsync(_documentServiceHelper, file.ID));
    }

    public async Task<Web.Core.Files.DocumentService.FileLink> GetPresignedUriAsync(T fileId)
    {
        var file = await GetFileAsync(fileId, -1);
        var result = new Web.Core.Files.DocumentService.FileLink
        {
            FileType = FileUtility.GetFileExtension(file.Title),
            Url = _documentServiceConnector.ReplaceCommunityAdress(_pathProvider.GetFileStreamUrl(file))
        };

        result.Token = _documentServiceHelper.GetSignature(result);

        return result;
    }

    public async Task<List<FileEntry>> GetNewItemsAsync(T folderId)
    {
        try
        {
            Folder<T> folder;
            var folderDao = GetFolderDao();
            folder = await folderDao.GetFolderAsync(folderId);

            var result = await _fileMarker.MarkedItemsAsync(folder);

            result = new List<FileEntry>(_entryManager.SortEntries<T>(result, new OrderBy(SortedByType.DateAndTime, false)));

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
        if (foldersId.Count == 0 && filesId.Count == 0)
        {
            return GetTasksStatuses();
        }

        return _fileOperationsManager.MarkAsRead(_authContext.CurrentAccount.ID, _tenantManager.GetCurrentTenant(), foldersId, filesId);
    }

    public Task<List<ThirdPartyParams>> GetThirdPartyAsync()
    {
        var providerDao = GetProviderDao();
        if (providerDao == null)
        {
            return Task.FromResult(new List<ThirdPartyParams>());
        }

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
                    });

        return new List<ThirdPartyParams>(resultList.ToList());
    }

    public Task<List<FileEntry>> GetThirdPartyFolderAsync(int folderType = 0)
    {
        if (!_filesSettingsHelper.EnableThirdParty)
        {
            return Task.FromResult(new List<FileEntry>());
        }

        var providerDao = GetProviderDao();
        if (providerDao == null)
        {
            return Task.FromResult(new List<FileEntry>());
        }

        return InternalGetThirdPartyFolderAsync(folderType, providerDao);
    }

    private async Task<List<FileEntry>> InternalGetThirdPartyFolderAsync(int folderType, IProviderDao providerDao)
    {
        var providersInfo = await providerDao.GetProvidersInfoAsync((FolderType)folderType).ToListAsync();

        var folders = providersInfo.Select(providerInfo =>
        {
            var folder = _entryManager.GetFakeThirdpartyFolder(providerInfo);
            folder.NewForMe = folder.RootFolderType == FolderType.COMMON ? 1 : 0;

            return folder;
        });

        return new List<FileEntry>(folders);
    }

    public Task<Folder<T>> SaveThirdPartyAsync(ThirdPartyParams thirdPartyParams)
    {
        var providerDao = GetProviderDao();

        if (providerDao == null)
        {
            return Task.FromResult<Folder<T>>(null);
        }

        return InternalSaveThirdPartyAsync(thirdPartyParams, providerDao);
    }

    private async Task<Folder<T>> InternalSaveThirdPartyAsync(ThirdPartyParams thirdPartyParams, IProviderDao providerDao)
    {
        var folderDaoInt = _daoFactory.GetFolderDao<int>();
        var folderDao = GetFolderDao();

        ErrorIf(thirdPartyParams == null, FilesCommonResource.ErrorMassage_BadRequest);
        var parentFolder = await folderDaoInt.GetFolderAsync(thirdPartyParams.Corporate && !_coreBaseSettings.Personal ? await _globalFolderHelper.FolderCommonAsync : _globalFolderHelper.FolderMy);
        ErrorIf(!await _fileSecurity.CanCreateAsync(parentFolder), FilesCommonResource.ErrorMassage_SecurityException_Create);
        ErrorIf(!_filesSettingsHelper.EnableThirdParty, FilesCommonResource.ErrorMassage_SecurityException_Create);

        var lostFolderType = FolderType.USER;
        var folderType = thirdPartyParams.Corporate ? FolderType.COMMON : FolderType.USER;

        int curProviderId;

        MessageAction messageAction;
        if (string.IsNullOrEmpty(thirdPartyParams.ProviderId))
        {
            ErrorIf(!_thirdpartyConfiguration.SupportInclusion(_daoFactory)
                    ||
                    (!_filesSettingsHelper.EnableThirdParty
                     && !_coreBaseSettings.Personal)
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
            ErrorIf(lostProvider.Owner != _authContext.CurrentAccount.ID, FilesCommonResource.ErrorMassage_SecurityException);

            lostFolderType = lostProvider.RootFolderType;
            if (lostProvider.RootFolderType == FolderType.COMMON && !thirdPartyParams.Corporate)
            {
                var lostFolder = await folderDao.GetFolderAsync((T)Convert.ChangeType(lostProvider.RootFolderId, typeof(T)));
                await _fileMarker.RemoveMarkAsNewForAllAsync(lostFolder);
            }

            curProviderId = await providerDao.UpdateProviderInfoAsync(curProviderId, thirdPartyParams.CustomerTitle, thirdPartyParams.AuthData, folderType);
            messageAction = MessageAction.ThirdPartyUpdated;
        }

        var provider = await providerDao.GetProviderInfoAsync(curProviderId);
        await provider.InvalidateStorageAsync();

        var folderDao1 = GetFolderDao();
        var folder = await folderDao1.GetFolderAsync((T)Convert.ChangeType(provider.RootFolderId, typeof(T)));
        ErrorIf(!await _fileSecurity.CanReadAsync(folder), FilesCommonResource.ErrorMassage_SecurityException_ViewFolder);

        _filesMessageService.Send(parentFolder, GetHttpHeaders(), messageAction, folder.ID.ToString(), provider.ProviderKey);

        if (thirdPartyParams.Corporate && lostFolderType != FolderType.COMMON)
        {
            await _fileMarker.MarkAsNewAsync(folder);
        }

        return folder;
    }

    public Task<object> DeleteThirdPartyAsync(string providerId)
    {
        var providerDao = GetProviderDao();
        if (providerDao == null)
        {
            return Task.FromResult<object>(null);
        }

        return InternalDeleteThirdPartyAsync(providerId, providerDao);
    }

    private async Task<object> InternalDeleteThirdPartyAsync(string providerId, IProviderDao providerDao)
    {
        var curProviderId = Convert.ToInt32(providerId);
        var providerInfo = await providerDao.GetProviderInfoAsync(curProviderId);

        var folder = _entryManager.GetFakeThirdpartyFolder(providerInfo);
        ErrorIf(!await _fileSecurity.CanDeleteAsync(folder), FilesCommonResource.ErrorMassage_SecurityException_DeleteFolder);

        if (providerInfo.RootFolderType == FolderType.COMMON)
        {
            await _fileMarker.RemoveMarkAsNewForAllAsync(folder);
        }

        await providerDao.RemoveProviderInfoAsync(folder.ProviderId);
        _filesMessageService.Send(folder, GetHttpHeaders(), MessageAction.ThirdPartyDeleted, folder.ID, providerInfo.ProviderKey);

        return folder.ID;
    }

    public bool ChangeAccessToThirdparty(bool enable)
    {
        ErrorIf(!_global.IsAdministrator, FilesCommonResource.ErrorMassage_SecurityException);

        _filesSettingsHelper.EnableThirdParty = enable;
        _filesMessageService.Send(GetHttpHeaders(), MessageAction.DocumentsThirdPartySettingsUpdated);

        return _filesSettingsHelper.EnableThirdParty;
    }

    public bool SaveDocuSign(string code)
    {
        ErrorIf(!_authContext.IsAuthenticated
                || _userManager.GetUsers(_authContext.CurrentAccount.ID).IsVisitor(_userManager)
                || !_filesSettingsHelper.EnableThirdParty
                || !_thirdpartyConfiguration.SupportDocuSignInclusion, FilesCommonResource.ErrorMassage_SecurityException_Create);

        var token = _consumerFactory.Get<DocuSignLoginProvider>().GetAccessToken(code);
        _docuSignHelper.ValidateToken(token);
        _docuSignToken.SaveToken(token);

        return true;
    }

    public object DeleteDocuSign()
    {
        _docuSignToken.DeleteToken();

        return null;
    }

    public Task<string> SendDocuSignAsync(T fileId, DocuSignData docuSignData)
    {
        try
        {
            ErrorIf(_userManager.GetUsers(_authContext.CurrentAccount.ID).IsVisitor(_userManager)
                    || !_filesSettingsHelper.EnableThirdParty
                    || !_thirdpartyConfiguration.SupportDocuSignInclusion, FilesCommonResource.ErrorMassage_SecurityException_Create);

            return _docuSignHelper.SendDocuSignAsync(fileId, docuSignData, GetHttpHeaders());
        }
        catch (Exception e)
        {
            throw GenerateException(e);
        }
    }

    public List<FileOperationResult> GetTasksStatuses()
    {
        ErrorIf(!_authContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);

        return _fileOperationsManager.GetOperationResults(_authContext.CurrentAccount.ID);
    }

    public List<FileOperationResult> TerminateTasks()
    {
        ErrorIf(!_authContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);

        return _fileOperationsManager.CancelOperations(_authContext.CurrentAccount.ID);
    }

    public List<FileOperationResult> BulkDownload(Dictionary<JsonElement, string> folders, Dictionary<JsonElement, string> files)
    {
        ErrorIf(folders.Count == 0 && files.Count == 0, FilesCommonResource.ErrorMassage_BadRequest);

        return _fileOperationsManager.Download(_authContext.CurrentAccount.ID, _tenantManager.GetCurrentTenant(), folders, files, GetHttpHeaders());
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
        var folderDao = _daoFactory.GetFolderDao<TFrom>();
        var fileDao = _daoFactory.GetFileDao<TFrom>();
        var destFolderDao = _daoFactory.GetFolderDao<TTo>();
        var destFileDao = _daoFactory.GetFileDao<TTo>();

        var toFolder = await destFolderDao.GetFolderAsync(destFolderId);
        ErrorIf(toFolder == null, FilesCommonResource.ErrorMassage_FolderNotFound);
        ErrorIf(!await _fileSecurity.CanCreateAsync(toFolder), FilesCommonResource.ErrorMassage_SecurityException_Create);

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
            if (toSub == null)
            {
                continue;
            }

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
            result = _fileOperationsManager.MoveOrCopy(_authContext.CurrentAccount.ID, _tenantManager.GetCurrentTenant(), foldersId, filesId, destFolderId, ic, resolve, !deleteAfter, GetHttpHeaders());
        }
        else
        {
            result = _fileOperationsManager.GetOperationResults(_authContext.CurrentAccount.ID);
        }

        return result;
    }

    public List<FileOperationResult> DeleteFile(string action, T fileId, bool ignoreException = false, bool deleteAfter = false, bool immediately = false)
    {
        return _fileOperationsManager.Delete(_authContext.CurrentAccount.ID, _tenantManager.GetCurrentTenant(), new List<T>(), new List<T>() { fileId }, ignoreException, !deleteAfter, immediately, GetHttpHeaders());
    }
    public List<FileOperationResult> DeleteFolder(string action, T folderId, bool ignoreException = false, bool deleteAfter = false, bool immediately = false)
    {
        return _fileOperationsManager.Delete(_authContext.CurrentAccount.ID, _tenantManager.GetCurrentTenant(), new List<T>() { folderId }, new List<T>(), ignoreException, !deleteAfter, immediately, GetHttpHeaders());
    }

    public List<FileOperationResult> DeleteItems(string action, List<JsonElement> files, List<JsonElement> folders, bool ignoreException = false, bool deleteAfter = false, bool immediately = false)
    {
        return _fileOperationsManager.Delete(_authContext.CurrentAccount.ID, _tenantManager.GetCurrentTenant(), folders, files, ignoreException, !deleteAfter, immediately, GetHttpHeaders());
    }

    public async Task<List<FileOperationResult>> EmptyTrashAsync()
    {
        var folderDao = GetFolderDao();
        var fileDao = GetFileDao();
        var trashId = await folderDao.GetFolderIDTrashAsync(true);
        var foldersIdTask = await folderDao.GetFoldersAsync(trashId).Select(f => f.ID).ToListAsync();
        var filesIdTask = await fileDao.GetFilesAsync(trashId);

        return _fileOperationsManager.Delete(_authContext.CurrentAccount.ID, _tenantManager.GetCurrentTenant(), foldersIdTask, filesIdTask, false, true, false, GetHttpHeaders());
    }

    public async IAsyncEnumerable<FileOperationResult> CheckConversionAsync(List<CheckConversionRequestDto<T>> filesInfoJSON, bool sync = false)
    {
        if (filesInfoJSON == null || filesInfoJSON.Count == 0)
        {
            yield break;
        }

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
                var newFile = _serviceProvider.GetService<File<T>>();
                newFile.ID = fileInfo.FileId;
                newFile.Version = fileInfo.Version;

                files.Add(new KeyValuePair<File<T>, bool>(newFile, true));

                continue;
            }

            ErrorIf(!await _fileSecurity.CanReadAsync(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

            if (fileInfo.StartConvert && _fileConverter.MustConvert(file))
            {
                try
                {
                    if (sync)
                    {
                        results.Append(await _fileConverter.ExecSynchronouslyAsync(file, fileInfo.Password));
                    }
                    else
                    {
                        await _fileConverter.ExecAsynchronouslyAsync(file, false, fileInfo.Password);
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
            results = _fileConverter.GetStatusAsync(files);
        }

        await foreach (var res in results)
        {
            yield return res;
        }
    }

    public async Task<string> CheckFillFormDraftAsync(T fileId, int version, string doc, bool editPossible, bool view)
    {
        var (file, _configuration) = await _documentServiceHelper.GetParamsAsync(fileId, version, doc, editPossible, !view, true);
        var _valideShareLink = !string.IsNullOrEmpty(_fileShareLink.Parse(doc));

        if (_valideShareLink)
        {
            _configuration.Document.SharedLinkKey += doc;
        }

        if (_configuration.EditorConfig.ModeWrite
            && _fileUtility.CanWebRestrictedEditing(file.Title)
            && await _fileSecurity.CanFillFormsAsync(file)
            && !await _fileSecurity.CanEditAsync(file))
        {
            if (!file.IsFillFormDraft)
            {
                await _fileMarker.RemoveMarkAsNewAsync(file);

                Folder<int> folderIfNew;
                File<int> form;
                try
                {
                    (form, folderIfNew) = await _entryManager.GetFillFormDraftAsync(file);
                }
                catch (Exception ex)
                {
                    _logger.Error("DocEditor", ex);
                    throw;
                }

                var comment = folderIfNew == null
                    ? string.Empty
                    : "#message/" + HttpUtility.UrlEncode(string.Format(FilesCommonResource.MessageFillFormDraftCreated, folderIfNew.Title));

                return _filesLinkUtility.GetFileWebEditorUrl(form.ID) + comment;
            }
            else if (!await _entryManager.CheckFillFormDraftAsync(file))
            {
                var comment = "#message/" + HttpUtility.UrlEncode(FilesCommonResource.MessageFillFormDraftDiscard);

                return _filesLinkUtility.GetFileWebEditorUrl(file.ID) + comment;
            }
        }

        return _filesLinkUtility.GetFileWebEditorUrl(file.ID);
    }

    public async Task ReassignStorageAsync(Guid userFromId, Guid userToId)
    {
        //check current user have access
        ErrorIf(!_global.IsAdministrator, FilesCommonResource.ErrorMassage_SecurityException);

        //check exist userFrom
        var userFrom = _userManager.GetUsers(userFromId);
        ErrorIf(Equals(userFrom, Constants.LostUser), FilesCommonResource.ErrorMassage_UserNotFound);

        //check exist userTo
        var userTo = _userManager.GetUsers(userToId);
        ErrorIf(Equals(userTo, Constants.LostUser), FilesCommonResource.ErrorMassage_UserNotFound);
        ErrorIf(userTo.IsVisitor(_userManager), FilesCommonResource.ErrorMassage_SecurityException);

        var providerDao = GetProviderDao();
        if (providerDao != null)
        {
            var providersInfo = await providerDao.GetProvidersInfoAsync(userFrom.Id).ToListAsync();
            var commonProvidersInfo = providersInfo.Where(provider => provider.RootFolderType == FolderType.COMMON);

            //move common thirdparty storage userFrom
            foreach (var commonProviderInfo in commonProvidersInfo)
            {
                _logger.InfoFormat("Reassign provider {0} from {1} to {2}", commonProviderInfo.ID, userFrom.Id, userTo.Id);
                await providerDao.UpdateProviderInfoAsync(commonProviderInfo.ID, null, null, FolderType.DEFAULT, userTo.Id);
            }
        }

        var folderDao = GetFolderDao();
        var fileDao = GetFileDao();

        if (!userFrom.IsVisitor(_userManager))
        {
            var folderIdFromMy = await folderDao.GetFolderIDUserAsync(false, userFrom.Id);

            if (!Equals(folderIdFromMy, 0))
            {
                //create folder with name userFrom in folder userTo
                var folderIdToMy = await folderDao.GetFolderIDUserAsync(true, userTo.Id);
                var newFolder = _serviceProvider.GetService<Folder<T>>();
                newFolder.Title = string.Format(_customNamingPeople.Substitute<FilesCommonResource>("TitleDeletedUserFolder"), userFrom.DisplayUserName(false, _displayUserSettingsHelper));
                newFolder.FolderID = folderIdToMy;

                var newFolderTo = await folderDao.SaveFolderAsync(newFolder);

                //move items from userFrom to userTo
                await _entryManager.MoveSharedItemsAsync(folderIdFromMy, newFolderTo, folderDao, fileDao);

                await EntryManager.ReassignItemsAsync(newFolderTo, userFrom.Id, userTo.Id, folderDao, fileDao);
            }
        }

        await EntryManager.ReassignItemsAsync(await _globalFolderHelper.GetFolderCommonAsync<T>(), userFrom.Id, userTo.Id, folderDao, fileDao);
    }

    public async Task DeleteStorageAsync(Guid userId)
    {
        //check current user have access
        ErrorIf(!_global.IsAdministrator, FilesCommonResource.ErrorMassage_SecurityException);

        //delete docuSign
        _docuSignToken.DeleteToken(userId);

        var providerDao = GetProviderDao();
        if (providerDao != null)
        {
            var providersInfo = await providerDao.GetProvidersInfoAsync(userId).ToListAsync();

            //delete thirdparty storage
            foreach (var myProviderInfo in providersInfo)
            {
                _logger.InfoFormat("Delete provider {0} for {1}", myProviderInfo.ID, userId);
                await providerDao.RemoveProviderInfoAsync(myProviderInfo.ID);
            }
        }

        var folderDao = GetFolderDao();
        var fileDao = GetFileDao();
        var linkDao = GetLinkDao();

        //delete all markAsNew
        var rootFoldersId = new List<T>
                {
                    await _globalFolderHelper.GetFolderShareAsync<T>(),
                    await _globalFolderHelper.GetFolderCommonAsync<T>(),
                    await _globalFolderHelper.GetFolderProjectsAsync<T>(),
                };

        var folderIdFromMy = await folderDao.GetFolderIDUserAsync(false, userId);
        if (!Equals(folderIdFromMy, 0))
        {
            rootFoldersId.Add(folderIdFromMy);
        }

        var rootFolders = await folderDao.GetFoldersAsync(rootFoldersId).ToListAsync();
        foreach (var rootFolder in rootFolders)
        {
            await _fileMarker.RemoveMarkAsNewAsync(rootFolder, userId);
        }

        //delete all from My
        if (!Equals(folderIdFromMy, 0))
        {
            await _entryManager.DeleteSubitemsAsync(folderIdFromMy, folderDao, fileDao, linkDao);

            //delete My userFrom folder
            await folderDao.DeleteFolderAsync(folderIdFromMy);
            _globalFolderHelper.SetFolderMy(userId);
        }

        //delete all from Trash
        var folderIdFromTrash = await folderDao.GetFolderIDTrashAsync(false, userId);
        if (!Equals(folderIdFromTrash, 0))
        {
            await _entryManager.DeleteSubitemsAsync(folderIdFromTrash, folderDao, fileDao, linkDao);
            await folderDao.DeleteFolderAsync(folderIdFromTrash);
            _globalFolderHelper.FolderTrash = userId;
        }

        await EntryManager.ReassignItemsAsync(await _globalFolderHelper.GetFolderCommonAsync<T>(), userId, _authContext.CurrentAccount.ID, folderDao, fileDao);
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
        if (_userManager.GetUsers(_authContext.CurrentAccount.ID).IsVisitor(_userManager)) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);

        return InternalAddToFavoritesAsync(foldersId, filesId);
    }

    private async Task<List<FileEntry<T>>> InternalAddToFavoritesAsync(IEnumerable<T> foldersId, IEnumerable<T> filesId)
    {
        var tagDao = GetTagDao();
        var fileDao = GetFileDao();
        var folderDao = GetFolderDao();
        var entries = Enumerable.Empty<FileEntry<T>>();

        var files = await fileDao.GetFilesAsync(filesId).Where(file => !file.Encrypted).ToListAsync();
        files = (await _fileSecurity.FilterReadAsync(files)).ToList();
        entries = entries.Concat(files);

        var folders = await folderDao.GetFoldersAsync(foldersId).ToListAsync();
        folders = (await _fileSecurity.FilterReadAsync(folders)).ToList();
        entries = entries.Concat(folders);

        var tags = entries.Select(entry => Tag.Favorite(_authContext.CurrentAccount.ID, entry));

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
        var filtredFiles = await _fileSecurity.FilterReadAsync(files);
        entries = entries.Concat(filtredFiles);

        var folders = await folderDao.GetFoldersAsync(foldersId).ToListAsync();
        var filtredFolders = await _fileSecurity.FilterReadAsync(folders);
        entries = entries.Concat(filtredFolders);

        var tags = entries.Select(entry => Tag.Favorite(_authContext.CurrentAccount.ID, entry));

        tagDao.RemoveTags(tags);

        return new List<FileEntry<T>>(entries);
    }

    #endregion

    #region Templates Manager

    public Task<List<FileEntry<T>>> AddToTemplatesAsync(IEnumerable<T> filesId)
    {
        if (_userManager.GetUsers(_authContext.CurrentAccount.ID).IsVisitor(_userManager))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
        }

        return InternalAddToTemplatesAsync(filesId);
    }

    private async Task<List<FileEntry<T>>> InternalAddToTemplatesAsync(IEnumerable<T> filesId)
    {
        var tagDao = GetTagDao();
        var fileDao = GetFileDao();
        var files = await fileDao.GetFilesAsync(filesId).ToListAsync();

        var filteredFiles = await _fileSecurity.FilterReadAsync(files);
        files = filteredFiles
            .Where(file => _fileUtility.ExtsWebTemplate.Contains(FileUtility.GetFileExtension(file.Title), StringComparer.CurrentCultureIgnoreCase))
            .ToList();

        var tags = files.Select(file => Tag.Template(_authContext.CurrentAccount.ID, file));

        tagDao.SaveTags(tags);

        return new List<FileEntry<T>>(files);
    }

    public async Task<List<FileEntry<T>>> DeleteTemplatesAsync(IEnumerable<T> filesId)
    {
        var tagDao = GetTagDao();
        var fileDao = GetFileDao();
        var files = await fileDao.GetFilesAsync(filesId).ToListAsync();

        var filteredFiles = await _fileSecurity.FilterReadAsync(files);
        files = filteredFiles.ToList();

        var tags = files.Select(file => Tag.Template(_authContext.CurrentAccount.ID, file));

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
            result = await _entryManager.GetTemplatesAsync(folderDao, fileDao, filter, subjectGroup, subjectId, search, searchInContent);

            if (result.Count() <= from)
            {
                return null;
            }

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
        return _fileSharing.GetSharedInfoAsync(fileIds, folderIds);
    }

    public Task<List<AceShortWrapper>> GetSharedInfoShortFileAsync(T fileId)
    {
        return _fileSharing.GetSharedInfoShortFileAsync(fileId);
    }

    public Task<List<AceShortWrapper>> GetSharedInfoShortFolder(T folderId)
    {
        return _fileSharing.GetSharedInfoShortFolderAsync(folderId);
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
                var changed = await _fileSharingAceHelper.SetAceObjectAsync(aceCollection.Aces, entry, notify, aceCollection.Message);
                if (changed)
                {
                    _filesMessageService.Send(entry, GetHttpHeaders(),
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
        ErrorIf(!_authContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);
        var entries = AsyncEnumerable.Empty<FileEntry<T>>();

        var fileDao = GetFileDao();
        var folderDao = GetFolderDao();
        entries.Concat(filesId.ToAsyncEnumerable().SelectAwait(async fileId => await fileDao.GetFileAsync(fileId)));
        entries.Concat(foldersId.ToAsyncEnumerable().SelectAwait(async e => await folderDao.GetFolderAsync(e)));

        return _fileSharingAceHelper.RemoveAceAsync(entries);
    }

    public async Task<string> GetShortenLinkAsync(T fileId)
    {
        File<T> file;
        var fileDao = GetFileDao();
        file = await fileDao.GetFileAsync(fileId);
        ErrorIf(!await _fileSharing.CanSetAccessAsync(file), FilesCommonResource.ErrorMassage_SecurityException);
        var shareLink = _fileShareLink.GetLink(file);

        try
        {
            return await _urlShortener.Instance.GetShortenLinkAsync(shareLink);
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

            var changed = await _fileSharingAceHelper.SetAceObjectAsync(aces, file, false, null);
            if (changed)
            {
                _filesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileUpdatedAccess, file.Title);
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
        if (!_authContext.IsAuthenticated || _coreBaseSettings.Personal)
        {
            return Task.FromResult<List<MentionWrapper>>(null);
        }

        return InternalSharedUsersAsync(fileId);
    }

    public async Task<List<MentionWrapper>> InternalSharedUsersAsync(T fileId)
    {

        FileEntry<T> file;
        var fileDao = GetFileDao();
        file = await fileDao.GetFileAsync(fileId);

        ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);

        var usersIdWithAccess = new List<Guid>();
        if (await _fileSharing.CanSetAccessAsync(file))
        {
            var access = await _fileSharing.GetSharedInfoAsync(file);
            usersIdWithAccess = access.Where(aceWrapper => !aceWrapper.SubjectGroup && aceWrapper.Share != FileShare.Restrict)
                                      .Select(aceWrapper => aceWrapper.SubjectId)
                                      .ToList();
        }
        else
        {
            usersIdWithAccess.Add(file.CreateBy);
        }

        var users = _userManager.GetUsersByGroup(Constants.GroupEveryone.ID)
                               .Where(user => !user.Id.Equals(_authContext.CurrentAccount.ID)
                                              && !user.Id.Equals(Constants.LostUser.Id))
                               .Select(user => new MentionWrapper(user, _displayUserSettingsHelper) { HasAccess = usersIdWithAccess.Contains(user.Id) })
                               .ToList();

        users = users
            .OrderBy(user => !user.HasAccess)
            .ThenBy(user => user.User, UserInfoComparer.Default)
            .ToList();

        return new List<MentionWrapper>(users);
    }

    public async Task<List<AceShortWrapper>> SendEditorNotifyAsync(T fileId, MentionMessageWrapper mentionMessage)
    {
        ErrorIf(!_authContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);

        File<T> file;
        var fileDao = GetFileDao();
        file = await fileDao.GetFileAsync(fileId);

        ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);

        var fileSecurity = _fileSecurity;
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
                canShare = await _fileSharing.CanSetAccessAsync(file);
            }

            var recipient = _userManager.GetUserByEmail(email);
            if (recipient == null || recipient.Id == Constants.LostUser.Id)
            {
                showSharingSettings = canShare.Value;
                continue;
            }

            if (!await fileSecurity.CanReadAsync(file, recipient.Id))
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
                                SubjectId = recipient.Id,
                                SubjectGroup = false,
                            }
                        };

                    showSharingSettings |= await _fileSharingAceHelper.SetAceObjectAsync(aces, file, false, null);

                    recipients.Add(recipient.Id);
                }
                catch (Exception e)
                {
                    throw GenerateException(e);
                }
            }
            else
            {
                recipients.Add(recipient.Id);
            }
        }

        if (showSharingSettings)
        {
            _filesMessageService.Send(file, GetHttpHeaders(), MessageAction.FileUpdatedAccess, file.Title);
        }

        var fileLink = _filesLinkUtility.GetFileWebEditorUrl(file.ID);
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

        _notifyClient.SendEditorMentions(file, fileLink, recipients, message);

        return showSharingSettings ? await GetSharedInfoShortFileAsync(fileId) : null;
    }

    public async Task<List<EncryptionKeyPair>> GetEncryptionAccessAsync(T fileId)
    {
        ErrorIf(!PrivacyRoomSettings.GetEnabled(_settingsManager), FilesCommonResource.ErrorMassage_SecurityException);

        var fileKeyPair = await _encryptionKeyPairHelper.GetKeyPairAsync(fileId, this);

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
        var userInfo = _userManager.GetUsers(userId);
        ErrorIf(Equals(userInfo, Constants.LostUser) || userInfo.IsVisitor(_userManager), FilesCommonResource.ErrorMassage_ChangeOwner);

        var entries = AsyncEnumerable.Empty<FileEntry>();

        var folderDao = GetFolderDao();
        var folders = folderDao.GetFoldersAsync(foldersId);
        await foreach (var folder in folders)
        {
            ErrorIf(!await _fileSecurity.CanEditAsync(folder), FilesCommonResource.ErrorMassage_SecurityException);
            ErrorIf(folder.RootFolderType != FolderType.COMMON, FilesCommonResource.ErrorMassage_SecurityException);
            if (folder.ProviderEntry)
            {
                continue;
            }

            var newFolder = folder;
            if (folder.CreateBy != userInfo.Id)
            {
                var folderAccess = folder.Access;

                newFolder.CreateBy = userInfo.Id;
                var newFolderID = await folderDao.SaveFolderAsync(newFolder);

                newFolder = await folderDao.GetFolderAsync(newFolderID);
                newFolder.Access = folderAccess;

                _filesMessageService.Send(newFolder, GetHttpHeaders(), MessageAction.FileChangeOwner, new[] { newFolder.Title, userInfo.DisplayUserName(false, _displayUserSettingsHelper) });
            }
            entries.Append(newFolder);
        }

        var fileDao = GetFileDao();
        var files = fileDao.GetFilesAsync(filesId);

        await foreach (var file in files)
        {
            ErrorIf(!await _fileSecurity.CanEditAsync(file), FilesCommonResource.ErrorMassage_SecurityException);
            ErrorIf(await _entryManager.FileLockedForMeAsync(file.ID), FilesCommonResource.ErrorMassage_LockedFile);
            ErrorIf(_fileTracker.IsEditing(file.ID), FilesCommonResource.ErrorMassage_UpdateEditingFile);
            ErrorIf(file.RootFolderType != FolderType.COMMON, FilesCommonResource.ErrorMassage_SecurityException);
            if (file.ProviderEntry)
            {
                continue;
            }

            var newFile = file;
            if (file.CreateBy != userInfo.Id)
            {
                newFile = _serviceProvider.GetService<File<T>>();
                newFile.ID = file.ID;
                newFile.Version = file.Version + 1;
                newFile.VersionGroup = file.VersionGroup + 1;
                newFile.Title = file.Title;
                newFile.FileStatus = file.FileStatus;
                newFile.FolderID = file.FolderID;
                newFile.CreateBy = userInfo.Id;
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

                await _fileMarker.MarkAsNewAsync(newFile);

                await _entryStatusManager.SetFileStatusAsync(newFile);

                _filesMessageService.Send(newFile, GetHttpHeaders(), MessageAction.FileChangeOwner, new[] { newFile.Title, userInfo.DisplayUserName(false, _displayUserSettingsHelper) });
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
        _filesSettingsHelper.StoreOriginalFiles = set;
        _filesMessageService.Send(GetHttpHeaders(), MessageAction.DocumentsUploadingFormatsSettingsUpdated);

        return _filesSettingsHelper.StoreOriginalFiles;
    }

    public bool HideConfirmConvert(bool isForSave)
    {
        if (isForSave)
        {
            _filesSettingsHelper.HideConfirmConvertSave = true;
        }
        else
        {
            _filesSettingsHelper.HideConfirmConvertOpen = true;
        }

        return true;
    }

    public bool UpdateIfExist(bool set)
    {
        _filesSettingsHelper.UpdateIfExist = set;
        _filesMessageService.Send(GetHttpHeaders(), MessageAction.DocumentsOverwritingSettingsUpdated);

        return _filesSettingsHelper.UpdateIfExist;
    }

    public bool Forcesave(bool set)
    {
        _filesSettingsHelper.Forcesave = set;
        _filesMessageService.Send(GetHttpHeaders(), MessageAction.DocumentsForcesave);

        return _filesSettingsHelper.Forcesave;
    }

    public bool StoreForcesave(bool set)
    {
        ErrorIf(!_global.IsAdministrator, FilesCommonResource.ErrorMassage_SecurityException);

        _filesSettingsHelper.StoreForcesave = set;
        _filesMessageService.Send(GetHttpHeaders(), MessageAction.DocumentsStoreForcesave);

        return _filesSettingsHelper.StoreForcesave;
    }

    public bool DisplayRecent(bool set)
    {
        if (!_authContext.IsAuthenticated)
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
        }

        _filesSettingsHelper.RecentSection = set;

        return _filesSettingsHelper.RecentSection;
    }

    public bool DisplayFavorite(bool set)
    {
        if (!_authContext.IsAuthenticated)
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
        }

        _filesSettingsHelper.FavoritesSection = set;

        return _filesSettingsHelper.FavoritesSection;
    }

    public bool DisplayTemplates(bool set)
    {
        if (!_authContext.IsAuthenticated)
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
        }

        _filesSettingsHelper.TemplatesSection = set;

        return _filesSettingsHelper.TemplatesSection;
    }

    public ICompress ChangeDownloadTarGz(bool set)
    {
        _filesSettingsHelper.DownloadTarGz = set;

        return CompressToArchive;
    }

    public bool ChangeDeleteConfrim(bool set)
    {
        _filesSettingsHelper.ConfirmDelete = set;

        return _filesSettingsHelper.ConfirmDelete;
    }

    public IEnumerable<JsonElement> CreateThumbnails(List<JsonElement> fileIds)
    {
        try
        {
            var req = new ThumbnailRequest()
            {
                Tenant = _tenantManager.GetCurrentTenant().Id,
                BaseUrl = _baseCommonLinkUtility.GetFullAbsolutePath("")
            };

            var (fileIntIds, _) = FileOperationsManager.GetIds(fileIds);

            foreach (var f in fileIntIds)
            {
                req.Files.Add(f);
            }

            _thumbnailNotify.Publish(req, Common.Caching.CacheNotifyAction.Insert);
        }
        catch (Exception e)
        {
            _logger.Error("CreateThumbnails", e);
        }

        return fileIds;
    }

    public async Task<IEnumerable<JsonElement>> CreateThumbnailsAsync(List<JsonElement> fileIds)
    {
        try
        {
            var req = new ThumbnailRequest()
            {
                Tenant = _tenantManager.GetCurrentTenant().Id,
                BaseUrl = _baseCommonLinkUtility.GetFullAbsolutePath("")
            };

            var (fileIntIds, _) = FileOperationsManager.GetIds(fileIds);

            foreach (var f in fileIntIds)
            {
                req.Files.Add(f);
            }

            await _thumbnailNotify.PublishAsync(req, CacheNotifyAction.Insert);
        }
        catch (Exception e)
        {
            _logger.Error("CreateThumbnails", e);
        }

        return fileIds;
    }

    public string GetHelpCenter()
    {
        return string.Empty; //TODO: Studio.UserControls.Common.HelpCenter.HelpCenter.RenderControlToString();
    }

    private IFolderDao<T> GetFolderDao()
    {
        return _daoFactory.GetFolderDao<T>();
    }

    private IFileDao<T> GetFileDao()
    {
        return _daoFactory.GetFileDao<T>();
    }

    private ITagDao<T> GetTagDao()
    {
        return _daoFactory.GetTagDao<T>();
    }

    private IDataStore GetStoreTemplate()
    {
        return _globalStore.GetStoreTemplate();
    }

    private IProviderDao GetProviderDao()
    {
        return _daoFactory.ProviderDao;
    }

    private ISecurityDao<T> GetSecurityDao()
    {
        return _daoFactory.GetSecurityDao<T>();
    }

    private ILinkDao GetLinkDao()
    {
        return _daoFactory.GetLinkDao();
    }

    private static void ErrorIf(bool condition, string errorMessage)
    {
        if (condition)
        {
            throw new InvalidOperationException(errorMessage);
        }
    }

    private Exception GenerateException(Exception error, bool warning = false)
    {
        if (warning)
        {
            _logger.Info(error);
        }
        else
        {
            _logger.Error(error);
        }

        return new InvalidOperationException(error.Message, error);
    }

    private IDictionary<string, StringValues> GetHttpHeaders()
    {
        return _httpContextAccessor?.HttpContext?.Request?.Headers;
    }
}

public class FileModel<T, TTempate>
{
    public T ParentId { get; set; }
    public string Title { get; set; }
    public TTempate TemplateId { get; set; }
}
