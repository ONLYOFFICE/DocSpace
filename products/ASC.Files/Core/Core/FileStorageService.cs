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

using UrlShortener = ASC.Web.Core.Utility.UrlShortener;

namespace ASC.Web.Files.Services.WCFService;

[Scope]
public class FileStorageService //: IFileStorageService
{
    private static readonly FileEntrySerializer _serializer = new FileEntrySerializer();
    private readonly CompressToArchive _compressToArchive;
    private readonly OFormRequestManager _oFormRequestManager;
    private readonly ThirdPartySelector _thirdPartySelector;
    private readonly ThumbnailSettings _thumbnailSettings;
    private readonly MessageService _messageService;
    private readonly IServiceScopeFactory _serviceScopeFactory;
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
    private readonly FileSharingAceHelper _fileSharingAceHelper;
    private readonly ConsumerFactory _consumerFactory;
    private readonly EncryptionKeyPairDtoHelper _encryptionKeyPairHelper;
    private readonly SettingsManager _settingsManager;
    private readonly FileOperationsManager _fileOperationsManager;
    private readonly TenantManager _tenantManager;
    private readonly FileTrackerHelper _fileTracker;
    private readonly IEventBus _eventBus;
    private readonly EntryStatusManager _entryStatusManager;
    private readonly ILogger _logger;
    private readonly FileShareParamsHelper _fileShareParamsHelper;
    private readonly EncryptionLoginProvider _encryptionLoginProvider;
    private readonly CountRoomChecker _countRoomChecker;
    private readonly InvitationLinkService _invitationLinkService;
    private readonly InvitationLinkHelper _invitationLinkHelper;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly TenantQuotaFeatureStatHelper _tenantQuotaFeatureStatHelper;
    private readonly QuotaSocketManager _quotaSocketManager;
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
        ILoggerProvider optionMonitor,
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
        FileSharingAceHelper fileSharingAceHelper,
        ConsumerFactory consumerFactory,
        EncryptionKeyPairDtoHelper encryptionKeyPairHelper,
        SettingsManager settingsManager,
        FileOperationsManager fileOperationsManager,
        TenantManager tenantManager,
        FileTrackerHelper fileTracker,
        IEventBus eventBus,
        EntryStatusManager entryStatusManager,
        CompressToArchive compressToArchive,
        OFormRequestManager oFormRequestManager,
        MessageService messageService,
        IServiceScopeFactory serviceScopeFactory,
        ThirdPartySelector thirdPartySelector,
        ThumbnailSettings thumbnailSettings,
        FileShareParamsHelper fileShareParamsHelper,
        EncryptionLoginProvider encryptionLoginProvider,
        CountRoomChecker countRoomChecker,
        InvitationLinkService invitationLinkService,
        InvitationLinkHelper invitationLinkHelper,
        StudioNotifyService studioNotifyService,
        TenantQuotaFeatureStatHelper tenantQuotaFeatureStatHelper,
        QuotaSocketManager quotaSocketManager)
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
        _logger = optionMonitor.CreateLogger("ASC.Files");
        _fileOperationsManager = fileOperationsManager;
        _tenantManager = tenantManager;
        _fileTracker = fileTracker;
        _eventBus = eventBus;
        _entryStatusManager = entryStatusManager;
        _compressToArchive = compressToArchive;
        _oFormRequestManager = oFormRequestManager;
        _messageService = messageService;
        _serviceScopeFactory = serviceScopeFactory;
        _thirdPartySelector = thirdPartySelector;
        _thumbnailSettings = thumbnailSettings;
        _fileShareParamsHelper = fileShareParamsHelper;
        _encryptionLoginProvider = encryptionLoginProvider;
        _countRoomChecker = countRoomChecker;
        _invitationLinkService = invitationLinkService;
        _invitationLinkHelper = invitationLinkHelper;
        _studioNotifyService = studioNotifyService;
        _tenantQuotaFeatureStatHelper = tenantQuotaFeatureStatHelper;
        _quotaSocketManager = quotaSocketManager;
    }

    public async Task<Folder<T>> GetFolderAsync<T>(T folderId)
    {
        var folderDao = GetFolderDao<T>();
        var tagDao = GetTagDao<T>();
        var folder = await folderDao.GetFolderAsync(folderId);

        ErrorIf(folder == null, FilesCommonResource.ErrorMassage_FolderNotFound);
        ErrorIf(!await _fileSecurity.CanReadAsync(folder), FilesCommonResource.ErrorMassage_SecurityException_ReadFolder);

        await _entryStatusManager.SetIsFavoriteFolderAsync(folder);

        var tag = await tagDao.GetNewTagsAsync(_authContext.CurrentAccount.ID, folder).FirstOrDefaultAsync();
        if (tag != null)
        {
            folder.NewForMe = tag.Count;
        }

        return folder;
    }

    public async Task<IEnumerable<FileEntry>> GetFoldersAsync<T>(T parentId)
    {
        var folderDao = GetFolderDao<T>();
        IEnumerable<FileEntry> entries;

        try
        {
            (entries, _) = await _entryManager.GetEntriesAsync(
                await folderDao.GetFolderAsync(parentId), 0, 0, FilterType.FoldersOnly,
                false, Guid.Empty, string.Empty, false, false, new OrderBy(SortedByType.AZ, true));
        }
        catch (Exception e)
        {
            throw GenerateException(e);
        }

        return entries;
    }

    public async Task<List<object>> GetPathAsync<T>(T folderId)
    {
        var folderDao = GetFolderDao<T>();
        var folder = await folderDao.GetFolderAsync(folderId);

        ErrorIf(folder == null, FilesCommonResource.ErrorMassage_FolderNotFound);
        ErrorIf(!await _fileSecurity.CanReadAsync(folder), FilesCommonResource.ErrorMassage_SecurityException_ViewFolder);

        var breadCrumbs = await _entryManager.GetBreadCrumbsAsync(folderId, folderDao);

        return new List<object>(breadCrumbs.Select(f =>
        {
            if (f is Folder<string> f1)
            {
                return (object)f1.Id;
            }

            if (f is Folder<int> f2)
            {
                return f2.Id;
            }

            return 0;
        }));
    }

    public async Task<DataWrapper<T>> GetFolderItemsAsync<T>(
        T parentId,
        int from,
        int count,
        FilterType filterType,
        bool subjectGroup,
        string subject,
        string searchText,
        bool searchInContent,
        bool withSubfolders,
        OrderBy orderBy,
        SearchArea searchArea = SearchArea.Active,
        T roomId = default,
        bool withoutTags = false,
        IEnumerable<string> tagNames = null,
        bool excludeSubject = false,
        ProviderFilter provider = ProviderFilter.None,
        SubjectFilter subjectFilter = SubjectFilter.Owner)
    {
        var subjectId = string.IsNullOrEmpty(subject) ? Guid.Empty : new Guid(subject);

        var folderDao = GetFolderDao<T>();

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
        ErrorIf(parent.RootFolderType == FolderType.TRASH && !Equals(parent.Id, await _globalFolderHelper.FolderTrashAsync), FilesCommonResource.ErrorMassage_ViewTrashItem);

        if (orderBy != null)
        {
            _filesSettingsHelper.DefaultOrder = orderBy;
        }
        else
        {
            orderBy = _filesSettingsHelper.DefaultOrder;
        }

        if (Equals(parent.Id, await _globalFolderHelper.FolderShareAsync) && orderBy.SortedBy == SortedByType.DateAndTime)
        {
            orderBy.SortedBy = SortedByType.New;
        }

        searchArea = parent.FolderType == FolderType.Archive ? SearchArea.Archive : searchArea;

        int total;
        IEnumerable<FileEntry> entries;
        try
        {
            (entries, total) = await _entryManager.GetEntriesAsync(parent, from, count, filterType, subjectGroup, subjectId, searchText, searchInContent, withSubfolders, orderBy, roomId, searchArea,
                withoutTags, tagNames, excludeSubject, provider, subjectFilter);
        }
        catch (Exception e)
        {
            if (parent.ProviderEntry)
            {
                throw GenerateException(new Exception(FilesCommonResource.ErrorMassage_SharpBoxException, e));
            }

            throw GenerateException(e);
        }

        var breadCrumbsTask = _entryManager.GetBreadCrumbsAsync(parentId, folderDao);
        var shareableTask = _fileSharing.CanSetAccessAsync(parent);
        var newTask = _fileMarker.GetRootFoldersIdMarkedAsNewAsync(parentId);

        var breadCrumbs = await breadCrumbsTask;

        var prevVisible = breadCrumbs.ElementAtOrDefault(breadCrumbs.Count - 2);
        if (prevVisible != null && !DocSpaceHelper.IsRoom(parent.FolderType))
        {
            if (prevVisible.FileEntryType == FileEntryType.Folder)
            {
                if (prevVisible is Folder<string> f1)
                {
                    parent.ParentId = (T)Convert.ChangeType(f1.Id, typeof(T));
                }
                else if (prevVisible is Folder<int> f2)
                {
                    parent.ParentId = (T)Convert.ChangeType(f2.Id, typeof(T));
                }
            }
        }

        parent.Shareable = await shareableTask
            || parent.FolderType == FolderType.SHARE
            || parent.RootFolderType == FolderType.Privacy;

        entries = entries.Where(x =>
        {
            if (x.FileEntryType == FileEntryType.Folder)
            {
                return true;
            }

            if (x is File<string> f1)
            {
                return !_fileConverter.IsConverting(f1);
            }

            return x is File<int> f2 && !_fileConverter.IsConverting(f2);
        });

        var result = new DataWrapper<T>
        {
            Total = total,
            Entries = entries.ToList(),
            FolderPathParts = new List<object>(breadCrumbs.Select(f =>
            {
                if (f.FileEntryType == FileEntryType.Folder)
                {
                    if (f is Folder<string> f1)
                    {
                        return (object)f1.Id;
                    }

                    if (f is Folder<int> f2)
                    {
                        return f2.Id;
                    }
                }

                return 0;
            })),
            FolderInfo = parent,
            New = await newTask
        };

        return result;
    }

    public async Task<List<FileEntry>> GetItemsAsync<TId>(IEnumerable<TId> filesId, IEnumerable<TId> foldersId, FilterType filter, bool subjectGroup, string subjectID, string search)
    {
        var subjectId = string.IsNullOrEmpty(subjectID) ? Guid.Empty : new Guid(subjectID);

        var entries = AsyncEnumerable.Empty<FileEntry<TId>>();

        var folderDao = _daoFactory.GetFolderDao<TId>();
        var fileDao = _daoFactory.GetFileDao<TId>();

        entries = entries.Concat(_fileSecurity.FilterReadAsync(folderDao.GetFoldersAsync(foldersId)));
        entries = entries.Concat(_fileSecurity.FilterReadAsync(fileDao.GetFilesAsync(filesId)));
        entries = _entryManager.FilterEntries(entries, filter, subjectGroup, subjectId, search, true);

        var result = new List<FileEntry>();
        var files = new List<File<TId>>();
        var folders = new List<Folder<TId>>();

        await foreach (var fileEntry in entries)
        {
            if (fileEntry is File<TId> file)
            {
                if (fileEntry.RootFolderType == FolderType.USER
                    && !Equals(fileEntry.RootCreateBy, _authContext.CurrentAccount.ID)
                    && !await _fileSecurity.CanReadAsync(await folderDao.GetFolderAsync(file.FolderIdDisplay)))
                {
                    file.FolderIdDisplay = await _globalFolderHelper.GetFolderShareAsync<TId>();
                }
                if (!Equals(file.Id, default(TId)))
                {
                    files.Add(file);
                }
            }
            else if (fileEntry is Folder<TId> folder)
            {
                if (fileEntry.RootFolderType == FolderType.USER
                    && !Equals(fileEntry.RootCreateBy, _authContext.CurrentAccount.ID)
                    && !await _fileSecurity.CanReadAsync(await folderDao.GetFolderAsync(folder.FolderIdDisplay)))
                {
                    folder.FolderIdDisplay = await _globalFolderHelper.GetFolderShareAsync<TId>();
                }

                if (!Equals(folder.Id, default(TId)))
                {
                    folders.Add(folder);
                }
            }

            result.Add(fileEntry);
        }

        var setFilesStatus = _entryStatusManager.SetFileStatusAsync(files);
        var setFavorites = _entryStatusManager.SetIsFavoriteFoldersAsync(folders);

        await Task.WhenAll(setFilesStatus, setFavorites);

        return result;
    }

    public async Task<Folder<T>> CreateNewFolderAsync<T>(T parentId, string title)
    {
        if (string.IsNullOrEmpty(title) || parentId == null)
        {
            throw new ArgumentException();
        }

        return await InternalCreateNewFolderAsync(parentId, title);
    }

    public async Task<Folder<int>> CreateRoomAsync(string title, RoomType roomType, bool @private, IEnumerable<FileShareParams> share, bool notify, string sharingMessage)
    {
        ArgumentNullException.ThrowIfNull(title, nameof(title));

        await _countRoomChecker.CheckAppend();

        if (@private && (share == null || !share.Any()))
        {
            throw new ArgumentNullException(nameof(share));
        }

        List<AceWrapper> aces = null;

        if (@private)
        {
            aces = await GetFullAceWrappersAsync(share);
            await CheckEncryptionKeysAsync(aces);
        }

        var parentId = await _globalFolderHelper.GetFolderVirtualRooms();

        var room = roomType switch
        {
            RoomType.CustomRoom => await CreateCustomRoomAsync(title, parentId, @private),
            RoomType.EditingRoom => await CreateEditingRoom(title, parentId, @private),
            _ => await CreateCustomRoomAsync(title, parentId, @private),
        };

        if (@private)
        {
            await SetAcesForPrivateRoomAsync(room, aces, notify, sharingMessage);
        }

        return room;
    }

    public async Task<Folder<T>> CreateThirdPartyRoomAsync<T>(string title, RoomType roomType, T parentId, bool @private, IEnumerable<FileShareParams> share, bool notify, string sharingMessage)
    {
        ArgumentNullException.ThrowIfNull(title, nameof(title));
        ArgumentNullException.ThrowIfNull(parentId, nameof(parentId));

        if (@private && (share == null || !share.Any()))
        {
            throw new ArgumentNullException(nameof(share));
        }

        var folderDao = GetFolderDao<T>();
        var providerDao = GetProviderDao<T>();

        var parent = await folderDao.GetFolderAsync(parentId);
        var providerInfo = await providerDao.GetProviderInfoAsync(parent.ProviderId);

        ErrorIf(providerInfo.RootFolderType != FolderType.VirtualRooms, FilesCommonResource.ErrorMessage_InvalidProvider);
        ErrorIf(providerInfo.FolderId != null, FilesCommonResource.ErrorMessage_ProviderAlreadyConnect);

        List<AceWrapper> aces = null;

        if (@private)
        {
            aces = await GetFullAceWrappersAsync(share);
            await CheckEncryptionKeysAsync(aces);
        }

        var result = roomType switch
        {
            RoomType.CustomRoom => (await CreateCustomRoomAsync(title, parentId, @private), FolderType.CustomRoom),
            RoomType.EditingRoom => (await CreateEditingRoom(title, parentId, @private), FolderType.EditingRoom),
            _ => (await CreateCustomRoomAsync(title, parentId, @private), FolderType.CustomRoom),
        };

        ErrorIf(result.Item1.Id.Equals(result.Item1.RootId), FilesCommonResource.ErrorMessage_InvalidThirdPartyFolder);

        if (@private)
        {
            await SetAcesForPrivateRoomAsync(result.Item1, aces, notify, sharingMessage);
        }

        await providerDao.UpdateProviderInfoAsync(providerInfo.ProviderId, title, result.Item1.Id.ToString(), result.Item2, @private);

        return result.Item1;
    }

    private async Task<Folder<T>> CreateCustomRoomAsync<T>(string title, T parentId, bool privacy)
    {
        return await InternalCreateNewFolderAsync(parentId, title, FolderType.CustomRoom, privacy);
    }

    private async Task<Folder<T>> CreateFillingFormsRoom<T>(string title, T parentId, bool privacy)
    {
        return await InternalCreateNewFolderAsync(parentId, title, FolderType.FillingFormsRoom, privacy);
    }

    private async Task<Folder<T>> CreateReviewRoom<T>(string title, T parentId, bool privacy)
    {
        return await InternalCreateNewFolderAsync(parentId, title, FolderType.ReviewRoom, privacy);
    }

    private async Task<Folder<T>> CreateReadOnlyRoom<T>(string title, T parentId, bool privacy)
    {
        return await InternalCreateNewFolderAsync(parentId, title, FolderType.ReadOnlyRoom, privacy);
    }

    private async Task<Folder<T>> CreateEditingRoom<T>(string title, T parentId, bool privacy)
    {
        return await InternalCreateNewFolderAsync(parentId, title, FolderType.EditingRoom, privacy);
    }

    private async ValueTask<Folder<T>> InternalCreateNewFolderAsync<T>(T parentId, string title, FolderType folderType = FolderType.DEFAULT, bool privacy = false)
    {
        var folderDao = GetFolderDao<T>();

        var parent = await folderDao.GetFolderAsync(parentId);
        var isRoom = DocSpaceHelper.IsRoom(folderType);

        ErrorIf(parent == null, FilesCommonResource.ErrorMassage_FolderNotFound);
        ErrorIf(!await _fileSecurity.CanCreateAsync(parent), FilesCommonResource.ErrorMassage_SecurityException_Create);
        ErrorIf(parent.RootFolderType == FolderType.Archive, FilesCommonResource.ErrorMessage_UpdateArchivedRoom);
        ErrorIf(parent.FolderType == FolderType.Archive, FilesCommonResource.ErrorMassage_SecurityException);
        ErrorIf(!isRoom && parent.FolderType == FolderType.VirtualRooms, FilesCommonResource.ErrorMassage_SecurityException_Create);

        try
        {
            var newFolder = _serviceProvider.GetService<Folder<T>>();
            newFolder.Title = title;
            newFolder.ParentId = parent.Id;
            newFolder.FolderType = folderType;
            newFolder.Private = parent.Private ? parent.Private : privacy;

            var folderId = await folderDao.SaveFolderAsync(newFolder);
            var folder = await folderDao.GetFolderAsync(folderId);

            await _socketManager.CreateFolderAsync(folder);

            if (isRoom)
            {
                var (name, value) = await _tenantQuotaFeatureStatHelper.GetStatAsync<CountRoomFeature, int>();
                _ = _quotaSocketManager.ChangeQuotaUsedValueAsync(name, value);
            }

            _ = _filesMessageService.SendAsync(folder, GetHttpHeaders(), isRoom ? MessageAction.RoomCreated : MessageAction.FolderCreated, folder.Title);

            return folder;
        }
        catch (Exception e)
        {
            throw GenerateException(e);
        }
    }

    public async Task<Folder<T>> FolderRenameAsync<T>(T folderId, string title)
    {
        var tagDao = GetTagDao<T>();
        var folderDao = GetFolderDao<T>();
        var folder = await folderDao.GetFolderAsync(folderId);
        ErrorIf(folder == null, FilesCommonResource.ErrorMassage_FolderNotFound);

        var canEdit = DocSpaceHelper.IsRoom(folder.FolderType) ? folder.RootFolderType != FolderType.Archive && await _fileSecurity.CanEditRoomAsync(folder)
            : await _fileSecurity.CanRenameAsync(folder);

        ErrorIf(!canEdit, FilesCommonResource.ErrorMassage_SecurityException_RenameFolder);
        if (!canEdit && await _userManager.IsUserAsync(_authContext.CurrentAccount.ID))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_RenameFolder);
        }

        ErrorIf(folder.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_ViewTrashItem);
        ErrorIf(folder.RootFolderType == FolderType.Archive, FilesCommonResource.ErrorMessage_UpdateArchivedRoom);

        var folderAccess = folder.Access;

        if (!string.Equals(folder.Title, title, StringComparison.OrdinalIgnoreCase))
        {
            var oldTitle = folder.Title;
            var newFolderID = await folderDao.RenameFolderAsync(folder, title);
            folder = await folderDao.GetFolderAsync(newFolderID);
            folder.Access = folderAccess;

            if (DocSpaceHelper.IsRoom(folder.FolderType))
            {
                _ = _filesMessageService.SendAsync(folder, GetHttpHeaders(), oldTitle, MessageAction.RoomRenamed, folder.Title);
            }
            else
            {
                _ = _filesMessageService.SendAsync(folder, GetHttpHeaders(), MessageAction.FolderRenamed, folder.Title);
            }

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
            && !Equals(folder.RootCreateBy, _authContext.CurrentAccount.ID)
            && !await _fileSecurity.CanReadAsync(await folderDao.GetFolderAsync(folder.ParentId)))
        {
            folder.FolderIdDisplay = await _globalFolderHelper.GetFolderShareAsync<T>();
        }

        await _entryStatusManager.SetIsFavoriteFolderAsync(folder);

        await _socketManager.UpdateFolderAsync(folder);

        return folder;
    }

    public async Task<File<T>> GetFileAsync<T>(T fileId, int version)
    {
        var fileDao = GetFileDao<T>();
        await fileDao.InvalidateCacheAsync(fileId);

        var file = version > 0
                       ? await fileDao.GetFileAsync(fileId, version)
                       : await fileDao.GetFileAsync(fileId);
        ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
        ErrorIf(!await _fileSecurity.CanReadAsync(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

        await _entryStatusManager.SetFileStatusAsync(file);

        if (file.RootFolderType == FolderType.USER
            && !Equals(file.RootCreateBy, _authContext.CurrentAccount.ID))
        {
            var folderDao = GetFolderDao<T>();
            if (!await _fileSecurity.CanReadAsync(await folderDao.GetFolderAsync(file.ParentId)))
            {
                file.FolderIdDisplay = await _globalFolderHelper.GetFolderShareAsync<T>();
            }
        }

        return file;
    }

    public async IAsyncEnumerable<FileEntry<T>> GetSiblingsFileAsync<T>(T fileId, T parentId, FilterType filter, bool subjectGroup, string subjectID, string search, bool searchInContent, bool withSubfolders, OrderBy orderBy)
    {
        var subjectId = string.IsNullOrEmpty(subjectID) ? Guid.Empty : new Guid(subjectID);

        var fileDao = GetFileDao<T>();
        var folderDao = GetFolderDao<T>();

        var file = await fileDao.GetFileAsync(fileId);
        ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
        ErrorIf(!await _fileSecurity.CanReadAsync(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

        var parent = await folderDao.GetFolderAsync(EqualityComparer<T>.Default.Equals(parentId, default(T)) ? file.ParentId : parentId);
        ErrorIf(parent == null, FilesCommonResource.ErrorMassage_FolderNotFound);
        ErrorIf(parent.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_ViewTrashItem);

        if (filter == FilterType.FoldersOnly)
        {
            yield break;
        }
        if (filter == FilterType.None)
        {
            filter = FilterType.FilesOnly;
        }

        if (orderBy == null)
        {
            orderBy = _filesSettingsHelper.DefaultOrder;
        }
        if (Equals(parent.Id, await _globalFolderHelper.GetFolderShareAsync<T>()) && orderBy.SortedBy == SortedByType.DateAndTime)
        {
            orderBy.SortedBy = SortedByType.New;
        }

        var entries = Enumerable.Empty<FileEntry>();

        if (!await _fileSecurity.CanReadAsync(parent))
        {
            file.ParentId = await _globalFolderHelper.GetFolderShareAsync<T>();
            entries = entries.Append(file);
        }
        else
        {
            try
            {
                (entries, _) = await _entryManager.GetEntriesAsync(parent, 0, 0, filter, subjectGroup, subjectId, search, searchInContent, withSubfolders, orderBy);
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


        await foreach (var f in _fileSecurity.FilterReadAsync(entries.OfType<File<T>>().ToAsyncEnumerable()))
        {
            if (f is File<T> && previewedType.Contains(FileUtility.GetFileTypeByFileName(f.Title)))
            {
                yield return f;
            }
        }
    }

    public async ValueTask<File<T>> CreateNewFileAsync<T, TTemplate>(FileModel<T, TTemplate> fileWrapper, bool enableExternalExt = false)
    {
        if (string.IsNullOrEmpty(fileWrapper.Title) || fileWrapper.ParentId == null)
        {
            throw new ArgumentException();
        }

        var fileDao = GetFileDao<T>();
        var folderDao = GetFolderDao<T>();

        Folder<T> folder = null;
        if (!EqualityComparer<T>.Default.Equals(fileWrapper.ParentId, default(T)))
        {
            folder = await folderDao.GetFolderAsync(fileWrapper.ParentId);
            var canCreate = await _fileSecurity.CanCreateAsync(folder) && folder.FolderType != FolderType.VirtualRooms
                && folder.FolderType != FolderType.Archive;

            if (!canCreate)
            {
                folder = null;
            }
        }
        if (folder == null)
        {
            folder = await folderDao.GetFolderAsync(await _globalFolderHelper.GetFolderMyAsync<T>());
        }


        var file = _serviceProvider.GetService<File<T>>();
        file.ParentId = folder.Id;
        file.Comment = FilesCommonResource.CommentCreate;

        if (string.IsNullOrEmpty(fileWrapper.Title))
        {
            fileWrapper.Title = UserControlsCommonResource.NewDocument + ".docx";
        }

        var title = fileWrapper.Title;
        var fileExt = FileUtility.GetFileExtension(title);
        if (!enableExternalExt && fileExt != _fileUtility.MasterFormExtension)
        {
            fileExt = _fileUtility.GetInternalExtension(title);
            if (!_fileUtility.InternalExtension.ContainsValue(fileExt))
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

        if (fileWrapper.FormId != 0)
        {
            using (var stream = await _oFormRequestManager.Get(fileWrapper.FormId))
            {
                file.ContentLength = stream.Length;
                file = await fileDao.SaveFileAsync(file, stream);
            }
        }
        else if (EqualityComparer<TTemplate>.Default.Equals(fileWrapper.TemplateId, default(TTemplate)))
        {
            var culture = (await _userManager.GetUsersAsync(_authContext.CurrentAccount.ID)).GetCulture();
            var storeTemplate = await GetStoreTemplateAsync();

            var path = FileConstant.NewDocPath + culture + "/";
            if (!await storeTemplate.IsDirectoryAsync(path))
            {
                path = FileConstant.NewDocPath + "en-US/";
            }

            try
            {

                file.ThumbnailStatus = Thumbnail.Creating;

                if (!enableExternalExt)
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

                var counter = 0;

                foreach (var size in _thumbnailSettings.Sizes)
                {
                    var pathThumb = $"{path}{fileExt.Trim('.')}.{size.Width}x{size.Height}.{_global.ThumbnailExtension}";

                    if (!await storeTemplate.IsFileAsync("", pathThumb))
                    {
                        break;
                    }

                    using (var streamThumb = await storeTemplate.GetReadStreamAsync("", pathThumb, 0))
                    {
                        await (await _globalStore.GetStoreAsync()).SaveAsync(fileDao.GetUniqThumbnailPath(file, size.Width, size.Height), streamThumb);
                    }

                    counter++;
                }

                if (_thumbnailSettings.Sizes.Count() == counter)
                {
                    await fileDao.SetThumbnailStatusAsync(file, Thumbnail.Created);

                    file.ThumbnailStatus = Thumbnail.Created;

                }
                else
                {
                    await fileDao.SetThumbnailStatusAsync(file, Thumbnail.NotRequired);

                    file.ThumbnailStatus = Thumbnail.NotRequired;
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

            file.ThumbnailStatus = template.ThumbnailStatus == Thumbnail.Created ? Thumbnail.Creating : Thumbnail.Waiting;

            try
            {
                using (var stream = await fileTemlateDao.GetFileStreamAsync(template))
                {
                    file.ContentLength = template.ContentLength;
                    file = await fileDao.SaveFileAsync(file, stream);
                }

                if (template.ThumbnailStatus == Thumbnail.Created)
                {
                    foreach (var size in _thumbnailSettings.Sizes)
                    {
                        await (await _globalStore.GetStoreAsync()).CopyAsync(String.Empty,
                                        fileTemlateDao.GetUniqThumbnailPath(template, size.Width, size.Height),
                                        String.Empty,
                                        fileDao.GetUniqThumbnailPath(file, size.Width, size.Height));
                    }

                    await fileDao.SetThumbnailStatusAsync(file, Thumbnail.Created);

                    file.ThumbnailStatus = Thumbnail.Created;
                }
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }
        }

        _ = _filesMessageService.SendAsync(file, GetHttpHeaders(), MessageAction.FileCreated, file.Title);

        await _fileMarker.MarkAsNewAsync(file);

        await _socketManager.CreateFileAsync(file);

        return file;
    }

    public async Task<KeyValuePair<bool, string>> TrackEditFileAsync<T>(T fileId, Guid tabId, string docKeyForTrack, string doc = null, bool isFinish = false)
    {
        try
        {
            var id = await _fileShareLink.ParseAsync<T>(doc);
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

            if (docKeyForTrack != await _documentServiceHelper.GetDocKeyAsync(id, -1, DateTime.MinValue))
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

    public async Task<Dictionary<string, string>> CheckEditingAsync<T>(List<T> filesId)
    {
        ErrorIf(!_authContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);
        var result = new Dictionary<string, string>();

        var fileDao = GetFileDao<T>();
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

            var usersId = _fileTracker.GetEditingBy(file.Id).ToAsyncEnumerable();
            var value = string.Join(", ", await usersId.SelectAwait(async userId => await _global.GetUserNameAsync(userId, true)).ToArrayAsync());
            result[file.Id.ToString()] = value;
        }

        return result;
    }

    public async Task<File<T>> SaveEditingAsync<T>(T fileId, string fileExtension, string fileuri, Stream stream, string doc = null, bool forcesave = false)
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
                _ = _filesMessageService.SendAsync(file, GetHttpHeaders(), MessageAction.FileUpdated, file.Title);
            }

            return file;
        }
        catch (Exception e)
        {
            throw GenerateException(e);
        }
    }

    public async Task<File<T>> UpdateFileStreamAsync<T>(T fileId, Stream stream, string fileExtension, bool encrypted, bool forcesave)
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
                _ = _filesMessageService.SendAsync(file, GetHttpHeaders(), MessageAction.FileUpdated, file.Title);
            }

            return file;
        }
        catch (Exception e)
        {
            throw GenerateException(e);
        }
    }

    public async Task<string> StartEditAsync<T>(T fileId, bool editingAlone = false, string doc = null)
    {
        try
        {
            IThirdPartyApp app;
            if (editingAlone)
            {
                ErrorIf(_fileTracker.IsEditing(fileId), FilesCommonResource.ErrorMassage_SecurityException_EditFileTwice);

                app = _thirdPartySelector.GetAppByFileId(fileId.ToString());
                if (app == null)
                {
                    await _entryManager.TrackEditingAsync(fileId, Guid.Empty, _authContext.CurrentAccount.ID, doc, true);
                }

                //without StartTrack, track via old scheme
                return await _documentServiceHelper.GetDocKeyAsync(fileId, -1, DateTime.MinValue);
            }

            (File<string> File, Configuration<string> Configuration, bool LocatedInPrivateRoom) fileOptions;

            app = _thirdPartySelector.GetAppByFileId(fileId.ToString());
            if (app == null)
            {
                fileOptions = await _documentServiceHelper.GetParamsAsync(fileId.ToString(), -1, doc, true, true, false);
            }
            else
            {
                (var file, var editable) = await app.GetFileAsync(fileId.ToString());
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

    public async Task<File<T>> FileRenameAsync<T>(T fileId, string title)
    {
        try
        {
            var fileRename = await _entryManager.FileRenameAsync(fileId, title);
            var file = fileRename.File;

            if (fileRename.Renamed)
            {
                _ = _filesMessageService.SendAsync(file, GetHttpHeaders(), MessageAction.FileRenamed, file.Title);

                //if (!file.ProviderEntry)
                //{
                //    FilesIndexer.UpdateAsync(FilesWrapper.GetFilesWrapper(ServiceProvider, file), true, r => r.Title);
                //}
            }

            if (file.RootFolderType == FolderType.USER
                && !Equals(file.RootCreateBy, _authContext.CurrentAccount.ID))
            {
                var folderDao = GetFolderDao<T>();
                if (!await _fileSecurity.CanReadAsync(await folderDao.GetFolderAsync(file.ParentId)))
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

    public async IAsyncEnumerable<File<T>> GetFileHistoryAsync<T>(T fileId)
    {
        var fileDao = GetFileDao<T>();
        var file = await fileDao.GetFileAsync(fileId);
        ErrorIf(!await _fileSecurity.CanReadHistoryAsync(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

        await foreach (var r in fileDao.GetFileHistoryAsync(fileId))
        {
            await _entryStatusManager.SetFileStatusAsync(r);
            yield return r;
        }
    }

    public async Task<KeyValuePair<File<T>, IAsyncEnumerable<File<T>>>> UpdateToVersionAsync<T>(T fileId, int version)
    {
        var file = await _entryManager.UpdateToVersionFileAsync(fileId, version);
        _ = _filesMessageService.SendAsync(file, GetHttpHeaders(), MessageAction.FileRestoreVersion, file.Title, version.ToString(CultureInfo.InvariantCulture));

        if (file.RootFolderType == FolderType.USER
            && !Equals(file.RootCreateBy, _authContext.CurrentAccount.ID))
        {
            var folderDao = GetFolderDao<T>();
            if (!await _fileSecurity.CanReadAsync(await folderDao.GetFolderAsync(file.ParentId)))
            {
                file.FolderIdDisplay = await _globalFolderHelper.GetFolderShareAsync<T>();
            }
        }

        return new KeyValuePair<File<T>, IAsyncEnumerable<File<T>>>(file, GetFileHistoryAsync(fileId));
    }

    public async Task<string> UpdateCommentAsync<T>(T fileId, int version, string comment)
    {
        var fileDao = GetFileDao<T>();
        var file = await fileDao.GetFileAsync(fileId, version);
        ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
        ErrorIf(!await _fileSecurity.CanEditHistoryAsync(file) || await _userManager.IsUserAsync(_authContext.CurrentAccount.ID), FilesCommonResource.ErrorMassage_SecurityException_EditFile);
        ErrorIf(await _entryManager.FileLockedForMeAsync(file.Id), FilesCommonResource.ErrorMassage_LockedFile);
        ErrorIf(file.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_ViewTrashItem);

        comment = await fileDao.UpdateCommentAsync(fileId, version, comment);

        _ = _filesMessageService.SendAsync(file, GetHttpHeaders(), MessageAction.FileUpdatedRevisionComment, file.Title, version.ToString(CultureInfo.InvariantCulture));

        return comment;
    }

    public async Task<KeyValuePair<File<T>, IAsyncEnumerable<File<T>>>> CompleteVersionAsync<T>(T fileId, int version, bool continueVersion)
    {
        var file = await _entryManager.CompleteVersionFileAsync(fileId, version, continueVersion);

        _ = _filesMessageService.SendAsync(file, GetHttpHeaders(),
                                 continueVersion ? MessageAction.FileDeletedVersion : MessageAction.FileCreatedVersion,
                                 file.Title, version == 0 ? (file.Version - 1).ToString(CultureInfo.InvariantCulture) : version.ToString(CultureInfo.InvariantCulture));

        if (file.RootFolderType == FolderType.USER
            && !Equals(file.RootCreateBy, _authContext.CurrentAccount.ID))
        {
            var folderDao = GetFolderDao<T>();
            if (!await _fileSecurity.CanReadAsync(await folderDao.GetFolderAsync(file.ParentId)))
            {
                file.FolderIdDisplay = await _globalFolderHelper.GetFolderShareAsync<T>();
            }
        }

        await _socketManager.UpdateFileAsync(file);

        return new KeyValuePair<File<T>, IAsyncEnumerable<File<T>>>(file, GetFileHistoryAsync(fileId));
    }

    public async Task<File<T>> LockFileAsync<T>(T fileId, bool lockfile)
    {
        var tagDao = GetTagDao<T>();
        var fileDao = GetFileDao<T>();
        var file = await fileDao.GetFileAsync(fileId);

        ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
        ErrorIf(!await _fileSecurity.CanLockAsync(file) || lockfile && await _userManager.IsUserAsync(_authContext.CurrentAccount.ID), FilesCommonResource.ErrorMassage_SecurityException_EditFile);
        ErrorIf(file.RootFolderType == FolderType.TRASH, FilesCommonResource.ErrorMassage_ViewTrashItem);

        var tags = tagDao.GetTagsAsync(file.Id, FileEntryType.File, TagType.Locked);
        var tagLocked = await tags.FirstOrDefaultAsync();

        if (lockfile)
        {
            if (tagLocked == null)
            {
                tagLocked = new Tag("locked", TagType.Locked, _authContext.CurrentAccount.ID, 0).AddEntry(file);

                await tagDao.SaveTagsAsync(tagLocked);
            }

            var usersDrop = _fileTracker.GetEditingBy(file.Id).Where(uid => uid != _authContext.CurrentAccount.ID).Select(u => u.ToString()).ToArray();
            if (usersDrop.Length > 0)
            {
                var fileStable = file.Forcesave == ForcesaveType.None ? file : await fileDao.GetFileStableAsync(file.Id, file.Version);
                var docKey = await _documentServiceHelper.GetDocKeyAsync(fileStable);
                await _documentServiceHelper.DropUserAsync(docKey, usersDrop, file.Id);
            }

            _ = _filesMessageService.SendAsync(file, GetHttpHeaders(), MessageAction.FileLocked, file.Title);
        }
        else
        {
            if (tagLocked != null)
            {
                await tagDao.RemoveTags(tagLocked);

                _ = _filesMessageService.SendAsync(file, GetHttpHeaders(), MessageAction.FileUnlocked, file.Title);
            }

            if (!file.ProviderEntry)
            {
                file = await _entryManager.CompleteVersionFileAsync(file.Id, 0, false);
                await UpdateCommentAsync(file.Id, file.Version, FilesCommonResource.UnlockComment);
            }
        }

        await _entryStatusManager.SetFileStatusAsync(file);

        if (file.RootFolderType == FolderType.USER
            && !Equals(file.RootCreateBy, _authContext.CurrentAccount.ID))
        {
            var folderDao = GetFolderDao<T>();
            if (!await _fileSecurity.CanReadAsync(await folderDao.GetFolderAsync(file.ParentId)))
            {
                file.FolderIdDisplay = await _globalFolderHelper.GetFolderShareAsync<T>();
            }
        }

        await _socketManager.UpdateFileAsync(file);

        return file;
    }

    public async IAsyncEnumerable<EditHistory> GetEditHistoryAsync<T>(T fileId, string doc = null)
    {
        var fileDao = GetFileDao<T>();
        var (readLink, file, _) = await _fileShareLink.CheckAsync(doc, true, fileDao);
        if (file == null)
        {
            file = await fileDao.GetFileAsync(fileId);
        }

        ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
        ErrorIf(!readLink && !await _fileSecurity.CanReadHistoryAsync(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
        ErrorIf(file.ProviderEntry, FilesCommonResource.ErrorMassage_BadRequest);

        await foreach (var f in fileDao.GetEditHistoryAsync(_documentServiceHelper, file.Id))
        {
            yield return f;
        }
    }

    public async Task<EditHistoryDataDto> GetEditDiffUrlAsync<T>(T fileId, int version = 0, string doc = null)
    {
        var fileDao = GetFileDao<T>();
        var (readLink, file, _) = await _fileShareLink.CheckAsync(doc, true, fileDao);

        if (file != null)
        {
            fileId = file.Id;
        }

        if (file == null
            || version > 0 && file.Version != version)
        {
            file = version > 0
                       ? await fileDao.GetFileAsync(fileId, version)
                       : await fileDao.GetFileAsync(fileId);
        }

        ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
        ErrorIf(!readLink && !await _fileSecurity.CanReadHistoryAsync(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);
        ErrorIf(file.ProviderEntry, FilesCommonResource.ErrorMassage_BadRequest);

        var result = new EditHistoryDataDto
        {
            FileType = file.ConvertedExtension.Trim('.'),
            Key = await _documentServiceHelper.GetDocKeyAsync(file),
            Url = await _documentServiceConnector.ReplaceCommunityAdressAsync(await _pathProvider.GetFileStreamUrlAsync(file, doc)),
            Version = version
        };

        if (await fileDao.ContainChangesAsync(file.Id, file.Version))
        {
            string previouseKey;
            string sourceFileUrl;
            string previousFileExt;
            string sourceExt;

            if (file.Version > 1)
            {
                var previousFileStable = await fileDao.GetFileStableAsync(file.Id, file.Version - 1);
                ErrorIf(previousFileStable == null, FilesCommonResource.ErrorMassage_FileNotFound);

                sourceFileUrl = await _pathProvider.GetFileStreamUrlAsync(previousFileStable, doc);
                sourceExt = previousFileStable.ConvertedExtension;

                previouseKey = await _documentServiceHelper.GetDocKeyAsync(previousFileStable);
                previousFileExt = FileUtility.GetFileExtension(previousFileStable.Title);
            }
            else
            {
                var culture = (await _userManager.GetUsersAsync(_authContext.CurrentAccount.ID)).GetCulture();
                var storeTemplate = await GetStoreTemplateAsync();

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
                sourceExt = fileExt.Trim('.');

                previouseKey = DocumentServiceConnector.GenerateRevisionId(Guid.NewGuid().ToString());
                previousFileExt = fileExt;
            }

            result.Previous = new EditHistoryUrl
            {
                Key = previouseKey,
                Url = await _documentServiceConnector.ReplaceCommunityAdressAsync(sourceFileUrl),
                FileType = sourceExt.Trim('.')
            };

            result.ChangesUrl = await _documentServiceConnector.ReplaceCommunityAdressAsync(await _pathProvider.GetFileChangesUrlAsync(file, doc));
        }

        result.Token = _documentServiceHelper.GetSignature(result);

        return result;
    }

    public async IAsyncEnumerable<EditHistory> RestoreVersionAsync<T>(T fileId, int version, string url = null, string doc = null)
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
            fileDao = GetFileDao<T>();
            var fromFile = await fileDao.GetFileAsync(fileId, version);
            modifiedOnString = fromFile.ModifiedOnString;
            file = await _entryManager.SaveEditingAsync(fileId, null, url, null, doc, string.Format(FilesCommonResource.CommentRevertChanges, modifiedOnString));
        }

        _ = _filesMessageService.SendAsync(file, GetHttpHeaders(), MessageAction.FileRestoreVersion, file.Title, version.ToString(CultureInfo.InvariantCulture));

        await foreach (var f in GetFileDao<T>().GetEditHistoryAsync(_documentServiceHelper, file.Id))
        {
            yield return f;
        }
    }

    public async Task<FileLink> GetPresignedUriAsync<T>(T fileId)
    {
        var file = await GetFileAsync(fileId, -1);
        var result = new FileLink
        {
            FileType = FileUtility.GetFileExtension(file.Title),
            Url = await _documentServiceConnector.ReplaceCommunityAdressAsync(await _pathProvider.GetFileStreamUrlAsync(file))
        };

        result.Token = _documentServiceHelper.GetSignature(result);

        return result;
    }

    public async Task<EntryProperties> GetFileProperties<T>(T fileId)
    {
        var fileDao = GetFileDao<T>();

        await fileDao.InvalidateCacheAsync(fileId);

        var file = await fileDao.GetFileAsync(fileId);
        ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);
        ErrorIf(!await _fileSecurity.CanReadAsync(file), FilesCommonResource.ErrorMassage_SecurityException_ReadFile);

        var properties = await fileDao.GetProperties(fileId) ?? new EntryProperties();

        if (properties.FormFilling != null)
        {
            if (!await _fileSharing.CanSetAccessAsync(file) || !_fileUtility.CanWebRestrictedEditing(file.Title))
            {
                properties.FormFilling = null;
            }
            else
            {
                var folderId = properties.FormFilling.ToFolderId;
                if (int.TryParse(folderId, out var fId))
                {
                    await SetFormFillingFolderProps(fId);
                }
                else
                {
                    await SetFormFillingFolderProps(folderId);
                }
            }
        }

        return properties;

        async Task SetFormFillingFolderProps<TProp>(TProp toFolderId)
        {
            var folderDao = _daoFactory.GetFolderDao<TProp>();
            var folder = await folderDao.GetFolderAsync(toFolderId);

            if (folder == null)
            {
                properties.FormFilling.ToFolderId = null;
            }
            else if (await _fileSecurity.CanCreateAsync(folder))
            {
                properties.FormFilling.ToFolderPath = null;
                var breadCrumbs = await _entryManager.GetBreadCrumbsAsync(folder.Id, folderDao);
                properties.FormFilling.ToFolderPath = string.Join("/", breadCrumbs.Select(f => f.Title));
            }
        }
    }

    public async Task<EntryProperties> SetFileProperties<T>(T fileId, EntryProperties fileProperties)
    {
        var fileDao = GetFileDao<T>();

        var file = await fileDao.GetFileAsync(fileId);
        if (file == null)
        {
            throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
        }

        if (!await _fileSecurity.CanEditAsync(file))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_RenameFile);
        }

        if (await _entryManager.FileLockedForMeAsync(file.Id))
        {
            throw new Exception(FilesCommonResource.ErrorMassage_LockedFile);
        }

        if (file.ProviderEntry)
        {
            throw new Exception(FilesCommonResource.ErrorMassage_BadRequest);
        }

        if (file.RootFolderType == FolderType.TRASH)
        {
            throw new Exception(FilesCommonResource.ErrorMassage_ViewTrashItem);
        }

        var currentProperies = await fileDao.GetProperties(fileId) ?? new EntryProperties();
        if (fileProperties != null)
        {
            if (fileProperties.FormFilling != null)
            {
                if (!await _fileSharing.CanSetAccessAsync(file))
                {
                    throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
                }

                if (!_fileUtility.CanWebRestrictedEditing(file.Title))
                {
                    throw new Exception(FilesCommonResource.ErrorMassage_NotSupportedFormat);
                }

                if (currentProperies.FormFilling == null)
                {
                    await using var scope = _serviceScopeFactory.CreateAsyncScope();
                    currentProperies.FormFilling = scope.ServiceProvider.GetService<FormFillingProperties>();
                }

                currentProperies.FormFilling.CollectFillForm = fileProperties.FormFilling.CollectFillForm;

                if (!string.IsNullOrEmpty(fileProperties.FormFilling.ToFolderId))
                {
                    if (int.TryParse(fileProperties.FormFilling.ToFolderId, out var fId))
                    {
                        currentProperies.FormFilling.ToFolderId = await GetFormFillingFolder(fId);
                    }
                    else
                    {
                        currentProperies.FormFilling.ToFolderId = await GetFormFillingFolder(fId);
                    }
                }

                currentProperies.FormFilling.CreateFolderTitle = Global.ReplaceInvalidCharsAndTruncate(fileProperties.FormFilling.CreateFolderTitle);

                currentProperies.FormFilling.CreateFileMask = fileProperties.FormFilling.CreateFileMask;
                currentProperies.FormFilling.FixFileMask();
            }

            await fileDao.SaveProperties(file.Id, currentProperies);
        }

        return currentProperies;

        async Task<string> GetFormFillingFolder<TProp>(TProp toFolderId)
        {
            var folderDao = _daoFactory.GetFolderDao<TProp>();

            var folder = await folderDao.GetFolderAsync(toFolderId);

            ErrorIf(folder == null, FilesCommonResource.ErrorMassage_FolderNotFound);
            ErrorIf(!await _fileSecurity.CanCreateAsync(folder), FilesCommonResource.ErrorMassage_SecurityException_Create);

            return folder.Id.ToString();
        }
    }

    public async Task<List<FileEntry>> GetNewItemsAsync<T>(T folderId)
    {
        try
        {
            Folder<T> folder;
            var folderDao = GetFolderDao<T>();
            folder = await folderDao.GetFolderAsync(folderId);

            var result = await _fileMarker.MarkedItemsAsync(folder).Where(e => e.FileEntryType == FileEntryType.File).ToListAsync();

            result = new List<FileEntry>(_entryManager.SortEntries<T>(result, new OrderBy(SortedByType.DateAndTime, false)));

            if (result.Count == 0)
            {
                await MarkAsReadAsync(new List<JsonElement>() { JsonDocument.Parse(JsonSerializer.Serialize(folderId)).RootElement }, new List<JsonElement>() { }); //TODO
            }

            return result;
        }
        catch (Exception e)
        {
            throw GenerateException(e);
        }
    }

    public async Task<List<FileOperationResult>> MarkAsReadAsync(List<JsonElement> foldersId, List<JsonElement> filesId)
    {
        if (foldersId.Count == 0 && filesId.Count == 0)
        {
            return GetTasksStatuses();
        }

        return _fileOperationsManager.MarkAsRead(_authContext.CurrentAccount.ID, await _tenantManager.GetCurrentTenantAsync(), foldersId, filesId, GetHttpHeaders());
    }

    public IAsyncEnumerable<ThirdPartyParams> GetThirdPartyAsync()
    {
        var providerDao = GetProviderDao<string>();
        if (providerDao == null)
        {
            return AsyncEnumerable.Empty<ThirdPartyParams>();
        }

        return InternalGetThirdPartyAsync(providerDao);
    }

    private async IAsyncEnumerable<ThirdPartyParams> InternalGetThirdPartyAsync(IProviderDao providerDao)
    {
        await foreach (var r in providerDao.GetProvidersInfoAsync())
        {
            yield return new ThirdPartyParams
            {
                CustomerTitle = r.CustomerTitle,
                Corporate = r.RootFolderType == FolderType.COMMON,
                ProviderId = r.ProviderId.ToString(),
                ProviderKey = r.ProviderKey
            };
        }
    }

    public async ValueTask<Folder<string>> GetBackupThirdPartyAsync()
    {
        var providerDao = GetProviderDao<string>();
        if (providerDao == null)
        {
            return null;
        }

        var providerInfo = await providerDao.GetProvidersInfoAsync(FolderType.ThirdpartyBackup).SingleOrDefaultAsync();

        if (providerInfo != null)
        {
            var folderDao = GetFolderDao<string>();
            var folder = await folderDao.GetFolderAsync(providerInfo.RootFolderId);
            ErrorIf(!await _fileSecurity.CanReadAsync(folder), FilesCommonResource.ErrorMassage_SecurityException_ViewFolder);

            return folder;
        }
        else
        {
            return null;
        }
    }

    public IAsyncEnumerable<FileEntry> GetThirdPartyFolderAsync(int folderType = 0)
    {
        if (!_filesSettingsHelper.EnableThirdParty)
        {
            return AsyncEnumerable.Empty<FileEntry>();
        }

        var providerDao = GetProviderDao<string>();
        if (providerDao == null)
        {
            return AsyncEnumerable.Empty<FileEntry>();
        }

        return InternalGetThirdPartyFolderAsync(folderType, providerDao);
    }

    private async IAsyncEnumerable<FileEntry> InternalGetThirdPartyFolderAsync(int folderType, IProviderDao providerDao)
    {
        await foreach (var providerInfo in providerDao.GetProvidersInfoAsync((FolderType)folderType))
        {
            var folder = _entryManager.GetFakeThirdpartyFolder(providerInfo);
            folder.NewForMe = folder.RootFolderType == FolderType.COMMON ? 1 : 0;

            yield return folder;
        }
    }

    public async ValueTask<Folder<string>> SaveThirdPartyAsync(ThirdPartyParams thirdPartyParams)
    {
        var providerDao = GetProviderDao<string>();

        if (providerDao == null)
        {
            return null;
        }

        var folderDaoInt = _daoFactory.GetFolderDao<int>();
        var folderDao = GetFolderDao<string>();

        ErrorIf(thirdPartyParams == null, FilesCommonResource.ErrorMassage_BadRequest);

        var folderId = thirdPartyParams.Corporate && !_coreBaseSettings.Personal ? await _globalFolderHelper.FolderCommonAsync
            : thirdPartyParams.RoomsStorage && !_coreBaseSettings.DisableDocSpace ? await _globalFolderHelper.FolderVirtualRoomsAsync : await _globalFolderHelper.FolderMyAsync;

        var parentFolder = await folderDaoInt.GetFolderAsync(folderId);

        ErrorIf(!await _fileSecurity.CanCreateAsync(parentFolder), FilesCommonResource.ErrorMassage_SecurityException_Create);
        ErrorIf(!_filesSettingsHelper.EnableThirdParty, FilesCommonResource.ErrorMassage_SecurityException_Create);

        var lostFolderType = FolderType.USER;
        var folderType = thirdPartyParams.Corporate ? FolderType.COMMON : thirdPartyParams.RoomsStorage ? FolderType.VirtualRooms : FolderType.USER;

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
                var lostFolder = await folderDao.GetFolderAsync(lostProvider.RootFolderId);
                await _fileMarker.RemoveMarkAsNewForAllAsync(lostFolder);
            }

            curProviderId = await providerDao.UpdateProviderInfoAsync(curProviderId, thirdPartyParams.CustomerTitle, thirdPartyParams.AuthData, folderType);
            messageAction = MessageAction.ThirdPartyUpdated;
        }

        var provider = await providerDao.GetProviderInfoAsync(curProviderId);
        await provider.InvalidateStorageAsync();

        var folderDao1 = GetFolderDao<string>();
        var folder = await folderDao1.GetFolderAsync(provider.RootFolderId);
        ErrorIf(!await _fileSecurity.CanReadAsync(folder), FilesCommonResource.ErrorMassage_SecurityException_ViewFolder);

        _ = _filesMessageService.SendAsync(parentFolder, GetHttpHeaders(), messageAction, folder.Id, provider.ProviderKey);

        if (thirdPartyParams.Corporate && lostFolderType != FolderType.COMMON)
        {
            await _fileMarker.MarkAsNewAsync(folder);
        }

        return folder;
    }

    public async ValueTask<Folder<string>> SaveThirdPartyBackupAsync(ThirdPartyParams thirdPartyParams)
    {
        var providerDao = GetProviderDao<string>();

        if (providerDao == null)
        {
            return null;
        }

        ErrorIf(thirdPartyParams == null, FilesCommonResource.ErrorMassage_BadRequest);
        ErrorIf(!_filesSettingsHelper.EnableThirdParty, FilesCommonResource.ErrorMassage_SecurityException_Create);

        var folderType = FolderType.ThirdpartyBackup;

        int curProviderId;

        MessageAction messageAction;

        var thirdparty = await GetBackupThirdPartyAsync();
        if (thirdparty == null)
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
            curProviderId = await providerDao.UpdateBackupProviderInfoAsync(thirdPartyParams.ProviderKey, thirdPartyParams.CustomerTitle, thirdPartyParams.AuthData);
            messageAction = MessageAction.ThirdPartyUpdated;
        }

        var provider = await providerDao.GetProviderInfoAsync(curProviderId);
        await provider.InvalidateStorageAsync();

        var folderDao1 = GetFolderDao<string>();
        var folder = await folderDao1.GetFolderAsync(provider.RootFolderId);

        await _filesMessageService.SendAsync(GetHttpHeaders(), messageAction, folder.Id, provider.ProviderKey);

        return folder;
    }

    public async ValueTask<object> DeleteThirdPartyAsync(string providerId)
    {
        var providerDao = GetProviderDao<string>();
        if (providerDao == null)
        {
            return null;
        }

        var curProviderId = Convert.ToInt32(providerId);
        var providerInfo = await providerDao.GetProviderInfoAsync(curProviderId);

        var folder = _entryManager.GetFakeThirdpartyFolder(providerInfo);
        ErrorIf(!await _fileSecurity.CanDeleteAsync(folder), FilesCommonResource.ErrorMassage_SecurityException_DeleteFolder);

        if (providerInfo.RootFolderType == FolderType.COMMON)
        {
            await _fileMarker.RemoveMarkAsNewForAllAsync(folder);
        }

        await providerDao.RemoveProviderInfoAsync(folder.ProviderId);
        _ = _filesMessageService.SendAsync(folder, GetHttpHeaders(), MessageAction.ThirdPartyDeleted, folder.Id, providerInfo.ProviderKey);

        return folder.Id;
    }

    public async Task<bool> ChangeAccessToThirdpartyAsync(bool enable)
    {
        ErrorIf(!await _global.IsDocSpaceAdministratorAsync, FilesCommonResource.ErrorMassage_SecurityException);

        _filesSettingsHelper.EnableThirdParty = enable;
        await _messageService.SendAsync(GetHttpHeaders(), MessageAction.DocumentsThirdPartySettingsUpdated);

        return _filesSettingsHelper.EnableThirdParty;
    }

    public async Task<bool> SaveDocuSignAsync(string code)
    {
        ErrorIf(!_authContext.IsAuthenticated
                || await _userManager.IsUserAsync(_authContext.CurrentAccount.ID)
                || !_filesSettingsHelper.EnableThirdParty
                || !_thirdpartyConfiguration.SupportDocuSignInclusion, FilesCommonResource.ErrorMassage_SecurityException_Create);

        var token = _consumerFactory.Get<DocuSignLoginProvider>().GetAccessToken(code);
        await _docuSignHelper.ValidateTokenAsync(token);
        await _docuSignToken.SaveTokenAsync(token);

        return true;
    }

    public async Task DeleteDocuSignAsync()
    {
        await _docuSignToken.DeleteTokenAsync();
    }

    public async Task<string> SendDocuSignAsync<T>(T fileId, DocuSignData docuSignData)
    {
        try
        {
            ErrorIf(await _userManager.IsUserAsync(_authContext.CurrentAccount.ID)
                    || !_filesSettingsHelper.EnableThirdParty
                    || !_thirdpartyConfiguration.SupportDocuSignInclusion, FilesCommonResource.ErrorMassage_SecurityException_Create);

            return await _docuSignHelper.SendDocuSignAsync(fileId, docuSignData, GetHttpHeaders());
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

    public async Task<List<FileOperationResult>> BulkDownloadAsync(Dictionary<JsonElement, string> folders, Dictionary<JsonElement, string> files)
    {
        ErrorIf(folders.Count == 0 && files.Count == 0, FilesCommonResource.ErrorMassage_BadRequest);

        return _fileOperationsManager.Download(_authContext.CurrentAccount.ID, await _tenantManager.GetCurrentTenantAsync(), folders, files, GetHttpHeaders());
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
                && await destFileDao.IsExistAsync(file.Title, toFolder.Id))
            {
                checkedFiles.Add(id);
            }
        }

        var folders = folderDao.GetFoldersAsync(foldersId);
        var foldersProject = folders.Where(folder => folder.FolderType == FolderType.BUNCH);
        var toSubfolders = destFolderDao.GetFoldersAsync(toFolder.Id);

        await foreach (var folderProject in foldersProject)
        {
            var toSub = await toSubfolders.FirstOrDefaultAsync(to => Equals(to.Title, folderProject.Title));
            if (toSub == null)
            {
                continue;
            }

            var filesPr = fileDao.GetFilesAsync(folderProject.Id).ToListAsync();
            var foldersTmp = folderDao.GetFoldersAsync(folderProject.Id);
            var foldersPr = foldersTmp.Select(d => d.Id).ToListAsync();

            var (cFiles, cFolders) = await MoveOrCopyFilesCheckAsync(await filesPr, await foldersPr, toSub.Id);
            checkedFiles.AddRange(cFiles);
            checkedFolders.AddRange(cFolders);
        }

        try
        {
            foreach (var pair in await folderDao.CanMoveOrCopyAsync(foldersId.ToArray(), toFolder.Id))
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

    public async Task<List<FileOperationResult>> MoveOrCopyItemsAsync(List<JsonElement> foldersId, List<JsonElement> filesId, JsonElement destFolderId, FileConflictResolveType resolve, bool ic, bool deleteAfter = false)
    {
        ErrorIf(resolve == FileConflictResolveType.Overwrite && await _userManager.IsUserAsync(_authContext.CurrentAccount.ID), FilesCommonResource.ErrorMassage_SecurityException);

        List<FileOperationResult> result;
        if (foldersId.Count > 0 || filesId.Count > 0)
        {
            result = _fileOperationsManager.MoveOrCopy(_authContext.CurrentAccount.ID, await _tenantManager.GetCurrentTenantAsync(), foldersId, filesId, destFolderId, ic, resolve, !deleteAfter, GetHttpHeaders());
        }
        else
        {
            result = _fileOperationsManager.GetOperationResults(_authContext.CurrentAccount.ID);
        }

        return result;
    }

    public async Task<List<FileOperationResult>> DeleteFileAsync<T>(string action, T fileId, bool ignoreException = false, bool deleteAfter = false, bool immediately = false)
    {
        return _fileOperationsManager.Delete(_authContext.CurrentAccount.ID, await _tenantManager.GetCurrentTenantAsync(), new List<T>(), new List<T>() { fileId }, ignoreException, !deleteAfter, immediately, GetHttpHeaders());
    }
    public async Task<List<FileOperationResult>> DeleteFolderAsync<T>(string action, T folderId, bool ignoreException = false, bool deleteAfter = false, bool immediately = false)
    {
        return _fileOperationsManager.Delete(_authContext.CurrentAccount.ID, await _tenantManager.GetCurrentTenantAsync(), new List<T>() { folderId }, new List<T>(), ignoreException, !deleteAfter, immediately, GetHttpHeaders());
    }

    public async Task<List<FileOperationResult>> DeleteItemsAsync(string action, List<JsonElement> files, List<JsonElement> folders, bool ignoreException = false, bool deleteAfter = false, bool immediately = false)
    {
        return _fileOperationsManager.Delete(_authContext.CurrentAccount.ID, await _tenantManager.GetCurrentTenantAsync(), folders, files, ignoreException, !deleteAfter, immediately, GetHttpHeaders());
    }

    public async Task<List<FileOperationResult>> DeleteItemsAsync<T>(string action, List<T> files, List<T> folders, bool ignoreException = false, bool deleteAfter = false, bool immediately = false)
    {
        return _fileOperationsManager.Delete(_authContext.CurrentAccount.ID, await _tenantManager.GetCurrentTenantAsync(), folders, files, ignoreException, !deleteAfter, immediately, GetHttpHeaders());
    }

    public async Task<List<FileOperationResult>> EmptyTrashAsync()
    {
        var folderDao = GetFolderDao<int>();
        var fileDao = GetFileDao<int>();
        var trashId = await folderDao.GetFolderIDTrashAsync(true);
        var foldersIdTask = await folderDao.GetFoldersAsync(trashId).Select(f => f.Id).ToListAsync();
        var filesIdTask = await fileDao.GetFilesAsync(trashId).ToListAsync();

        return _fileOperationsManager.Delete(_authContext.CurrentAccount.ID, await _tenantManager.GetCurrentTenantAsync(), foldersIdTask, filesIdTask, false, true, false, GetHttpHeaders(), true);
    }

    public async IAsyncEnumerable<FileOperationResult> CheckConversionAsync<T>(List<CheckConversionRequestDto<T>> filesInfoJSON, bool sync = false)
    {
        if (filesInfoJSON == null || filesInfoJSON.Count == 0)
        {
            yield break;
        }

        var results = AsyncEnumerable.Empty<FileOperationResult>();
        var fileDao = GetFileDao<T>();
        var files = new List<KeyValuePair<File<T>, bool>>();
        foreach (var fileInfo in filesInfoJSON)
        {
            var file = fileInfo.Version > 0
                            ? await fileDao.GetFileAsync(fileInfo.FileId, fileInfo.Version)
                            : await fileDao.GetFileAsync(fileInfo.FileId);

            if (file == null)
            {
                var newFile = _serviceProvider.GetService<File<T>>();
                newFile.Id = fileInfo.FileId;
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
                        results = results.Append(await _fileConverter.ExecSynchronouslyAsync(file, fileInfo.Password));
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

    public async Task<string> CheckFillFormDraftAsync<T>(T fileId, int version, string doc, bool editPossible, bool view)
    {
        var (file, _configuration, _) = await _documentServiceHelper.GetParamsAsync(fileId, version, doc, editPossible, !view, true);
        var _valideShareLink = !string.IsNullOrEmpty(await _fileShareLink.ParseAsync(doc));

        if (_valideShareLink)
        {
            _configuration.Document.SharedLinkKey += doc;
        }

        if (_configuration.EditorConfig.ModeWrite
            && _fileUtility.CanWebRestrictedEditing(file.Title)
            && await _fileSecurity.CanFillFormsAsync(file)
            && !await _fileSecurity.CanEditAsync(file))
        {
            if (!await _entryManager.LinkedForMeAsync(file))
            {
                await _fileMarker.RemoveMarkAsNewAsync(file);

                Folder<T> folderIfNew;
                File<T> form;
                try
                {
                    (form, folderIfNew) = await _entryManager.GetFillFormDraftAsync(file);
                }
                catch (Exception ex)
                {
                    _logger.ErrorDocEditor(ex);
                    throw;
                }

                var comment = folderIfNew == null
                    ? string.Empty
                    : "#message/" + HttpUtility.UrlEncode(string.Format(FilesCommonResource.MessageFillFormDraftCreated, folderIfNew.Title));

                return _filesLinkUtility.GetFileWebEditorUrl(form.Id) + comment;
            }
            else if (!await _entryManager.CheckFillFormDraftAsync(file))
            {
                var comment = "#message/" + HttpUtility.UrlEncode(FilesCommonResource.MessageFillFormDraftDiscard);

                return _filesLinkUtility.GetFileWebEditorUrl(file.Id) + comment;
            }
        }

        return _filesLinkUtility.GetFileWebEditorUrl(file.Id);
    }

    public async Task ReassignStorageAsync<T>(Guid userFromId, Guid userToId)
    {
        //check current user have access
        ErrorIf(!await _global.IsDocSpaceAdministratorAsync, FilesCommonResource.ErrorMassage_SecurityException);

        //check exist userFrom
        var userFrom = await _userManager.GetUsersAsync(userFromId);
        ErrorIf(Equals(userFrom, Constants.LostUser), FilesCommonResource.ErrorMassage_UserNotFound);

        //check exist userTo
        var userTo = await _userManager.GetUsersAsync(userToId);
        ErrorIf(Equals(userTo, Constants.LostUser), FilesCommonResource.ErrorMassage_UserNotFound);
        ErrorIf(await _userManager.IsUserAsync(userTo), FilesCommonResource.ErrorMassage_SecurityException);

        var providerDao = GetProviderDao<T>();
        if (providerDao != null)
        {
            //move common thirdparty storage userFrom
            await foreach (var commonProviderInfo in providerDao.GetProvidersInfoAsync(userFrom.Id).Where(provider => provider.RootFolderType == FolderType.COMMON))
            {
                _logger.InformationReassignProvider(commonProviderInfo.ProviderId, userFrom.Id, userTo.Id);
                await providerDao.UpdateProviderInfoAsync(commonProviderInfo.ProviderId, null, null, FolderType.DEFAULT, userTo.Id);
            }
        }

        var folderDao = GetFolderDao<T>();
        var fileDao = GetFileDao<T>();

        if (!await _userManager.IsUserAsync(userFrom))
        {
            var folderIdFromMy = await folderDao.GetFolderIDUserAsync(false, userFrom.Id);

            if (!Equals(folderIdFromMy, 0))
            {
                //create folder with name userFrom in folder userTo
                var folderIdToMy = await folderDao.GetFolderIDUserAsync(true, userTo.Id);
                var newFolder = _serviceProvider.GetService<Folder<T>>();
                newFolder.Title = string.Format(_customNamingPeople.Substitute<FilesCommonResource>("TitleDeletedUserFolder"), userFrom.DisplayUserName(false, _displayUserSettingsHelper));
                newFolder.ParentId = folderIdToMy;

                var newFolderTo = await folderDao.SaveFolderAsync(newFolder);
                await _socketManager.CreateFolderAsync(newFolder);

                //move items from userFrom to userTo
                await _entryManager.MoveSharedItemsAsync(folderIdFromMy, newFolderTo, folderDao, fileDao);

                await EntryManager.ReassignItemsAsync(newFolderTo, userFrom.Id, userTo.Id, folderDao, fileDao);
            }
        }

        await EntryManager.ReassignItemsAsync(await _globalFolderHelper.GetFolderCommonAsync<T>(), userFrom.Id, userTo.Id, folderDao, fileDao);
    }

    #region Favorites Manager

    public async Task<bool> ToggleFileFavoriteAsync<T>(T fileId, bool favorite)
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

    public async ValueTask<List<FileEntry<T>>> AddToFavoritesAsync<T>(IEnumerable<T> foldersId, IEnumerable<T> filesId)
    {
        if (await _userManager.IsUserAsync(_authContext.CurrentAccount.ID))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
        }

        var tagDao = GetTagDao<T>();
        var fileDao = GetFileDao<T>();
        var folderDao = GetFolderDao<T>();

        var files = _fileSecurity.FilterReadAsync(fileDao.GetFilesAsync(filesId).Where(file => !file.Encrypted)).ToListAsync();
        var folders = _fileSecurity.FilterReadAsync(folderDao.GetFoldersAsync(foldersId)).ToListAsync();

        List<FileEntry<T>> entries = new();

        foreach (var items in await Task.WhenAll(files.AsTask(), folders.AsTask()))
        {
            entries.AddRange(items);
        }

        var tags = entries.Select(entry => Tag.Favorite(_authContext.CurrentAccount.ID, entry));

        await tagDao.SaveTags(tags);

        foreach (var entry in entries)
        {
            _ = _filesMessageService.SendAsync(entry, MessageAction.FileMarkedAsFavorite, entry.Title);
        }

        return entries;
    }

    public async Task<List<FileEntry<T>>> DeleteFavoritesAsync<T>(IEnumerable<T> foldersId, IEnumerable<T> filesId)
    {
        var tagDao = GetTagDao<T>();
        var fileDao = GetFileDao<T>();
        var folderDao = GetFolderDao<T>();

        var files = _fileSecurity.FilterReadAsync(fileDao.GetFilesAsync(filesId)).ToListAsync();
        var folders = _fileSecurity.FilterReadAsync(folderDao.GetFoldersAsync(foldersId)).ToListAsync();

        List<FileEntry<T>> entries = new();

        foreach (var items in await Task.WhenAll(files.AsTask(), folders.AsTask()))
        {
            entries.AddRange(items);
        }

        var tags = entries.Select(entry => Tag.Favorite(_authContext.CurrentAccount.ID, entry));

        await tagDao.RemoveTags(tags);

        foreach (var entry in entries)
        {
            _ = _filesMessageService.SendAsync(entry, MessageAction.FileRemovedFromFavorite, entry.Title);
        }

        return entries;
    }

    #endregion

    #region Templates Manager

    public async ValueTask<List<FileEntry<T>>> AddToTemplatesAsync<T>(IEnumerable<T> filesId)
    {
        if (await _userManager.IsUserAsync(_authContext.CurrentAccount.ID))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
        }

        var tagDao = GetTagDao<T>();
        var fileDao = GetFileDao<T>();

        var files = await _fileSecurity.FilterReadAsync(fileDao.GetFilesAsync(filesId))
            .Where(file => _fileUtility.ExtsWebTemplate.Contains(FileUtility.GetFileExtension(file.Title), StringComparer.CurrentCultureIgnoreCase))
            .ToListAsync();

        var tags = files.Select(file => Tag.Template(_authContext.CurrentAccount.ID, file));

        await tagDao.SaveTags(tags);

        return files;
    }

    public async Task<List<FileEntry<T>>> DeleteTemplatesAsync<T>(IEnumerable<T> filesId)
    {
        var tagDao = GetTagDao<T>();
        var fileDao = GetFileDao<T>();

        var files = await _fileSecurity.FilterReadAsync(fileDao.GetFilesAsync(filesId)).ToListAsync();

        var tags = files.Select(file => Tag.Template(_authContext.CurrentAccount.ID, file));

        await tagDao.RemoveTags(tags);

        return files;
    }

    public async IAsyncEnumerable<FileEntry<T>> GetTemplatesAsync<T>(FilterType filter, int from, int count, bool subjectGroup, string subjectID, string search, bool searchInContent)
    {
        var subjectId = string.IsNullOrEmpty(subjectID) ? Guid.Empty : new Guid(subjectID);
        var folderDao = GetFolderDao<T>();
        var fileDao = GetFileDao<T>();

        var result = _entryManager.GetTemplatesAsync(folderDao, fileDao, filter, subjectGroup, subjectId, search, searchInContent);

        await foreach (var r in result.Skip(from).Take(count))
        {
            yield return r;
        }
    }

    #endregion

    public async Task<List<AceWrapper>> GetSharedInfoAsync<T>(IEnumerable<T> fileIds, IEnumerable<T> folderIds)
    {
        return await _fileSharing.GetSharedInfoAsync(fileIds, folderIds);
    }

    public async Task<List<AceShortWrapper>> GetSharedInfoShortFileAsync<T>(T fileId)
    {
        return await _fileSharing.GetSharedInfoShortFileAsync(fileId);
    }

    public async Task<List<AceShortWrapper>> GetSharedInfoShortFolder<T>(T folderId)
    {
        return await _fileSharing.GetSharedInfoShortFolderAsync(folderId);
    }

    public async Task<string> SetAceObjectAsync<T>(AceCollection<T> aceCollection, bool notify)
    {
        var fileDao = GetFileDao<T>();
        var folderDao = GetFolderDao<T>();
        var securityDao = GetSecurityDao<T>();

        var entries = new List<FileEntry<T>>();
        string warning = null;

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

                var eventTypes = new List<(UserInfo User, EventType EventType, FileShare Access, string Email)>();
                foreach (var ace in aceCollection.Aces)
                {
                    var user = _userManager.GetUsers(ace.Id);

                    if (user == Constants.LostUser)
                    {
                        eventTypes.Add((null, EventType.Create, ace.Access, ace.Email));
                        continue;
                    }

                    var userId = user.Id;

                    var userSubjects = await _fileSecurity.GetUserSubjectsAsync(user.Id);
                    var usersRecords = await securityDao.GetSharesAsync(userSubjects).ToListAsync();
                    var recordEntrys = usersRecords.Select(r => r.EntryId.ToString());

                    EventType eventType;

                    if (usersRecords.Any() && ace.Access != FileShare.None && recordEntrys.Contains(entry.Id.ToString()))
                    {
                        eventType = EventType.Update;
                    }
                    else if (!usersRecords.Any() || !recordEntrys.Contains(entry.Id.ToString()))
                    {
                        eventType = EventType.Create;
                    }
                    else
                    {
                        eventType = EventType.Remove;
                    }

                    eventTypes.Add((user, eventType, ace.Access, null));
                }

                var (changed, warningMessage) = await _fileSharingAceHelper.SetAceObjectAsync(aceCollection.Aces, entry, notify, aceCollection.Message, aceCollection.AdvancedSettings);
                warning ??= warningMessage;

                if (changed)
                {
                    foreach (var e in eventTypes)
                    {
                        var user = e.User ?? await _userManager.GetUserByEmailAsync(e.Email);
                        var name = user.DisplayUserName(false, _displayUserSettingsHelper);

                        var access = e.Access;

                        if (entry.FileEntryType == FileEntryType.Folder && DocSpaceHelper.IsRoom(((Folder<T>)entry).FolderType))
                        {
                            switch (e.EventType)
                            {
                                case EventType.Create:
                                    _ = _filesMessageService.SendAsync(entry, GetHttpHeaders(), MessageAction.RoomCreateUser, user.Id, name, GetAccessString(access));
                                    break;
                                case EventType.Remove:
                                    _ = _filesMessageService.SendAsync(entry, GetHttpHeaders(), MessageAction.RoomRemoveUser, user.Id, name, GetAccessString(access));
                                    break;
                                case EventType.Update:
                                    _ = _filesMessageService.SendAsync(entry, GetHttpHeaders(), MessageAction.RoomUpdateAccessForUser, access, user.Id, name);
                                    break;
                            }
                        }
                        else
                        {

                            _ = _filesMessageService.SendAsync(entry, GetHttpHeaders(),
                                                 entry.FileEntryType == FileEntryType.Folder ? MessageAction.FolderUpdatedAccessFor : MessageAction.FileUpdatedAccessFor,
                                                 entry.Title, name, GetAccessString(access));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw GenerateException(e);
            }
        }

        return warning;
    }

    public async Task RemoveAceAsync<T>(List<T> filesId, List<T> foldersId)
    {
        ErrorIf(!_authContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);

        var fileDao = GetFileDao<T>();
        var folderDao = GetFolderDao<T>();

        var headers = GetHttpHeaders();

        foreach (var fileId in filesId)
        {
            var entry = await fileDao.GetFileAsync(fileId);
            await _fileSharingAceHelper.RemoveAceAsync(entry);
            _ = _filesMessageService.SendAsync(entry, headers, MessageAction.FileRemovedFromList, entry.Title);

        }

        foreach (var folderId in foldersId)
        {
            var entry = await folderDao.GetFolderAsync(folderId);
            await _fileSharingAceHelper.RemoveAceAsync(entry);
            _ = _filesMessageService.SendAsync(entry, headers, MessageAction.FolderRemovedFromList, entry.Title);
        }
    }

    public async Task<string> GetShortenLinkAsync<T>(T fileId)
    {
        File<T> file;
        var fileDao = GetFileDao<T>();
        file = await fileDao.GetFileAsync(fileId);
        ErrorIf(!await _fileSharing.CanSetAccessAsync(file), FilesCommonResource.ErrorMassage_SecurityException);
        var shareLink = await _fileShareLink.GetLinkAsync(file);

        try
        {
            return await _urlShortener.Instance.GetShortenLinkAsync(shareLink);
        }
        catch (Exception e)
        {
            throw GenerateException(e);
        }
    }

    public async Task<List<AceWrapper>> SetInvitationLink<T>(T roomId, Guid linkId, string title, FileShare share)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(title);

        var folderDao = GetFolderDao<T>();
        var room = (await folderDao.GetFolderAsync(roomId)).NotFoundIfNull();
        var messageAction = MessageAction.RoomLinkCreated;

        if (share == FileShare.None)
        {
            messageAction = MessageAction.RoomLinkDeleted;
        }
        else
        {
            var linkExist = (await _fileSecurity.GetSharesAsync(room))
                .Any(r => r.Subject == linkId && r.SubjectType == SubjectType.InvitationLink);

            if (linkExist)
            {
                messageAction = MessageAction.RoomLinkUpdated;
            }
        }

        var aces = new List<AceWrapper>
        {
            new()
            {
                Access = share,
                Id = linkId,
                SubjectType = SubjectType.InvitationLink,
                FileShareOptions = new FileShareOptions
                {
                    Title = title,
                    ExpirationDate = DateTime.UtcNow.Add(_invitationLinkHelper.IndividualLinkExpirationInterval)
                }
            }
        };

        try
        {
            var (changed, _) = await _fileSharingAceHelper.SetAceObjectAsync(aces, room, false, null, null);
            if (changed)
            {
                _ = _filesMessageService.SendAsync(room, GetHttpHeaders(), messageAction, room.Title, GetAccessString(share));
            }
        }
        catch (Exception e)
        {
            throw GenerateException(e);
        }

        return await GetSharedInfoAsync(Array.Empty<T>(), new[] { roomId });
    }

    public async Task<bool> SetAceLinkAsync<T>(T fileId, FileShare share)
    {
        FileEntry<T> file;
        var fileDao = GetFileDao<T>();
        file = await fileDao.GetFileAsync(fileId);
        var aces = new List<AceWrapper>
            {
                new AceWrapper
                {
                    Access = share,
                    Id = FileConstant.ShareLinkId,
                    SubjectGroup = true,
                }
            };

        try
        {
            var (changed, _) = await _fileSharingAceHelper.SetAceObjectAsync(aces, file, false, null, null);
            if (changed)
            {
                _ = _filesMessageService.SendAsync(file, GetHttpHeaders(), MessageAction.FileExternalLinkAccessUpdated, file.Title, GetAccessString(share));
            }
        }
        catch (Exception e)
        {
            throw GenerateException(e);
        }

        var securityDao = GetSecurityDao<T>();

        return await securityDao.IsSharedAsync(file.Id, FileEntryType.File);
    }

    public async Task<List<MentionWrapper>> SharedUsersAsync<T>(T fileId)
    {
        if (!_authContext.IsAuthenticated || _coreBaseSettings.Personal)
        {
            return null;
        }

        return await InternalSharedUsersAsync(fileId);
    }

    public async Task<FileReference<T>> GetReferenceDataAsync<T>(T fileId, string portalName, T sourceFileId, string path)
    {
        File<T> file = null;
        var fileDao = _daoFactory.GetFileDao<T>();
        if (portalName == (await _tenantManager.GetCurrentTenantIdAsync()).ToString())
        {
            file = await fileDao.GetFileAsync(fileId);
        }

        if (file == null)
        {
            var source = await fileDao.GetFileAsync(sourceFileId);

            if (source == null)
            {
                return new FileReference<T>
                {
                    Error = FilesCommonResource.ErrorMassage_FileNotFound
                };
            }

            if (!await _fileSecurity.CanReadAsync(source))
            {
                return new FileReference<T>
                {
                    Error = FilesCommonResource.ErrorMassage_SecurityException_ReadFile
                };
            }

            var folderDao = _daoFactory.GetFolderDao<T>();
            var folder = await folderDao.GetFolderAsync(source.ParentId);
            if (!await _fileSecurity.CanReadAsync(folder))
            {
                return new FileReference<T>
                {
                    Error = FilesCommonResource.ErrorMassage_SecurityException_ReadFolder
                };
            }

            var list = fileDao.GetFilesAsync(folder.Id, new OrderBy(SortedByType.AZ, true), FilterType.FilesOnly, false, Guid.Empty, path, false, false);
            file = await list.FirstOrDefaultAsync(fileItem => fileItem.Title == path);
        }

        if (!await _fileSecurity.CanReadAsync(file))
        {
            return new FileReference<T>
            {
                Error = FilesCommonResource.ErrorMassage_SecurityException_ReadFile
            };
        }

        var fileReference = new FileReference<T>
        {
            Path = file.Title,
            ReferenceData = new FileReferenceData<T>
            {
                FileKey = file.Id,
                InstanceId = (await _tenantManager.GetCurrentTenantIdAsync()).ToString()
            },
            Url = await _documentServiceConnector.ReplaceCommunityAdressAsync(await _pathProvider.GetFileStreamUrlAsync(file, lastVersion: true)),
            FileType = file.ConvertedExtension.Trim('.')
        };
        fileReference.Token = _documentServiceHelper.GetSignature(fileReference);
        return fileReference;
    }

    public async Task<List<MentionWrapper>> InternalSharedUsersAsync<T>(T fileId)
    {
        FileEntry<T> file;
        var fileDao = GetFileDao<T>();

        file = await fileDao.GetFileAsync(fileId);

        ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);

        var folderDao = GetFolderDao<T>();

        var (roomId, _) = await folderDao.GetParentRoomInfoFromFileEntryAsync(file);

        var access = await _fileSharing.GetSharedInfoAsync(Enumerable.Empty<int>(), new[] { roomId });
        var usersIdWithAccess = access.Where(aceWrapper => !aceWrapper.SubjectGroup
                                        && aceWrapper.Access != FileShare.Restrict)
                                      .Select(aceWrapper => aceWrapper.Id);

        var users = usersIdWithAccess
            .Where(id => !id.Equals(_authContext.CurrentAccount.ID))
            .Select(_userManager.GetUsers);

        var result = users
            .Where(u => u.Status != EmployeeStatus.Terminated)
            .Select(u => new MentionWrapper(u, _displayUserSettingsHelper))
            .OrderBy(u => u.User, UserInfoComparer.Default)
            .ToList();

        return result;
    }

    public async Task<Folder<T>> SetPinnedStatusAsync<T>(T folderId, bool pin)
    {
        var folderDao = GetFolderDao<T>();

        var room = await folderDao.GetFolderAsync(folderId);

        ErrorIf(room == null, FilesCommonResource.ErrorMassage_FolderNotFound);
        ErrorIf(!await _fileSecurity.CanPinAsync(room), FilesCommonResource.ErrorrMessage_PinRoom);

        var tagDao = GetTagDao<T>();
        var tag = Tag.Pin(_authContext.CurrentAccount.ID, room);

        if (pin)
        {
            await tagDao.SaveTagsAsync(tag);
        }
        else
        {
            await tagDao.RemoveTags(tag);
        }

        room.Pinned = pin;

        return room;
    }

    public async Task<List<AceShortWrapper>> SendEditorNotifyAsync<T>(T fileId, MentionMessageWrapper mentionMessage)
    {
        ErrorIf(!_authContext.IsAuthenticated, FilesCommonResource.ErrorMassage_SecurityException);

        var fileDao = GetFileDao<T>();
        var file = await fileDao.GetFileAsync(fileId);

        ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);

        var canRead = await _fileSecurity.CanReadAsync(file);
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

            var recipient = await _userManager.GetUserByEmailAsync(email);
            if (recipient == null || recipient.Id == Constants.LostUser.Id)
            {
                showSharingSettings = canShare.Value;
                continue;
            }

            recipients.Add(recipient.Id);
        }

        var fileLink = _filesLinkUtility.GetFileWebEditorUrl(file.Id);
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

        try
        {
            await _notifyClient.SendEditorMentions(file, fileLink, recipients, message);
        }
        catch (Exception ex)
        {
            _logger.ErrorWithException(ex);
        }

        return showSharingSettings ? await GetSharedInfoShortFileAsync(fileId) : null;
    }

    public async Task<List<EncryptionKeyPairDto>> GetEncryptionAccessAsync<T>(T fileId)
    {
        ErrorIf(!await PrivacyRoomSettings.GetEnabledAsync(_settingsManager), FilesCommonResource.ErrorMassage_SecurityException);

        var fileKeyPair = await _encryptionKeyPairHelper.GetKeyPairAsync(fileId, this);

        return new List<EncryptionKeyPairDto>(fileKeyPair);
    }

    public async Task<bool> ChangeExternalShareSettingsAsync(bool enable)
    {
        ErrorIf(!await _global.IsDocSpaceAdministratorAsync, FilesCommonResource.ErrorMassage_SecurityException);

        _filesSettingsHelper.ExternalShare = enable;

        if (!enable)
        {
            _filesSettingsHelper.ExternalShareSocialMedia = false;
        }

        await _messageService.SendAsync(GetHttpHeaders(), MessageAction.DocumentsExternalShareSettingsUpdated);

        return _filesSettingsHelper.ExternalShare;
    }

    public async Task<bool> ChangeExternalShareSocialMediaSettingsAsync(bool enable)
    {
        ErrorIf(!await _global.IsDocSpaceAdministratorAsync, FilesCommonResource.ErrorMassage_SecurityException);

        _filesSettingsHelper.ExternalShareSocialMedia = _filesSettingsHelper.ExternalShare && enable;

        await _messageService.SendAsync(GetHttpHeaders(), MessageAction.DocumentsExternalShareSettingsUpdated);

        return _filesSettingsHelper.ExternalShareSocialMedia;
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

    public async IAsyncEnumerable<FileEntry> ChangeOwnerAsync<T>(IEnumerable<T> foldersId, IEnumerable<T> filesId, Guid userId)
    {
        var userInfo = await _userManager.GetUsersAsync(userId);
        ErrorIf(Equals(userInfo, Constants.LostUser) || await _userManager.IsUserAsync(userInfo), FilesCommonResource.ErrorMassage_ChangeOwner);

        var folderDao = GetFolderDao<T>();
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

                await _socketManager.CreateFolderAsync(newFolder);
                await _entryStatusManager.SetIsFavoriteFolderAsync(folder);

                _ = _filesMessageService.SendAsync(newFolder, GetHttpHeaders(), MessageAction.FileChangeOwner, new[] { newFolder.Title, userInfo.DisplayUserName(false, _displayUserSettingsHelper) });
            }

            yield return newFolder;
        }

        var fileDao = GetFileDao<T>();
        var files = fileDao.GetFilesAsync(filesId);

        await foreach (var file in files)
        {
            ErrorIf(!await _fileSecurity.CanEditAsync(file), FilesCommonResource.ErrorMassage_SecurityException);
            ErrorIf(await _entryManager.FileLockedForMeAsync(file.Id), FilesCommonResource.ErrorMassage_LockedFile);
            ErrorIf(_fileTracker.IsEditing(file.Id), FilesCommonResource.ErrorMassage_UpdateEditingFile);
            ErrorIf(file.RootFolderType != FolderType.COMMON, FilesCommonResource.ErrorMassage_SecurityException);
            if (file.ProviderEntry)
            {
                continue;
            }

            var newFile = file;
            if (file.CreateBy != userInfo.Id)
            {
                newFile = _serviceProvider.GetService<File<T>>();
                newFile.Id = file.Id;
                newFile.Version = file.Version + 1;
                newFile.VersionGroup = file.VersionGroup + 1;
                newFile.Title = file.Title;
                newFile.FileStatus = file.FileStatus;
                newFile.ParentId = file.ParentId;
                newFile.CreateBy = userInfo.Id;
                newFile.CreateOn = file.CreateOn;
                newFile.ConvertedType = file.ConvertedType;
                newFile.Comment = FilesCommonResource.CommentChangeOwner;
                newFile.Encrypted = file.Encrypted;
                newFile.ThumbnailStatus = file.ThumbnailStatus == Thumbnail.Created ? Thumbnail.Creating : Thumbnail.Waiting;

                using (var stream = await fileDao.GetFileStreamAsync(file))
                {
                    newFile.ContentLength = stream.CanSeek ? stream.Length : file.ContentLength;
                    newFile = await fileDao.SaveFileAsync(newFile, stream);
                }

                if (file.ThumbnailStatus == Thumbnail.Created)
                {
                    foreach (var size in _thumbnailSettings.Sizes)
                    {
                        await (await _globalStore.GetStoreAsync()).CopyAsync(String.Empty,
                                                                fileDao.GetUniqThumbnailPath(file, size.Width, size.Height),
                                                                String.Empty,
                                                                fileDao.GetUniqThumbnailPath(newFile, size.Width, size.Height));
                    }

                    await fileDao.SetThumbnailStatusAsync(newFile, Thumbnail.Created);

                    newFile.ThumbnailStatus = Thumbnail.Created;
                }

                await _fileMarker.MarkAsNewAsync(newFile);

                await _entryStatusManager.SetFileStatusAsync(newFile);

                _ = _filesMessageService.SendAsync(newFile, GetHttpHeaders(), MessageAction.FileChangeOwner, new[] { newFile.Title, userInfo.DisplayUserName(false, _displayUserSettingsHelper) });
            }
            yield return newFile;
        }
    }

    public async Task<bool> StoreOriginalAsync(bool set)
    {
        _filesSettingsHelper.StoreOriginalFiles = set;
        await _messageService.SendAsync(GetHttpHeaders(), MessageAction.DocumentsUploadingFormatsSettingsUpdated);

        return _filesSettingsHelper.StoreOriginalFiles;
    }

    public async Task<bool> KeepNewFileNameAsync(bool set)
    {
        _filesSettingsHelper.KeepNewFileName = set;
        await _messageService.SendAsync(GetHttpHeaders(), MessageAction.DocumentsKeepNewFileNameSettingsUpdated);

        return _filesSettingsHelper.KeepNewFileName;
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

    public async Task<bool> UpdateIfExistAsync(bool set)
    {
        ErrorIf(await _userManager.IsUserAsync(_authContext.CurrentAccount.ID), FilesCommonResource.ErrorMassage_SecurityException);

        _filesSettingsHelper.UpdateIfExist = set;
        await _messageService.SendAsync(GetHttpHeaders(), MessageAction.DocumentsOverwritingSettingsUpdated);

        return _filesSettingsHelper.UpdateIfExist;
    }

    public async Task<bool> ForcesaveAsync(bool set)
    {
        _filesSettingsHelper.Forcesave = set;
        await _messageService.SendAsync(GetHttpHeaders(), MessageAction.DocumentsForcesave);

        return _filesSettingsHelper.Forcesave;
    }

    public async Task<bool> StoreForcesaveAsync(bool set)
    {
        ErrorIf(!await _global.IsDocSpaceAdministratorAsync, FilesCommonResource.ErrorMassage_SecurityException);

        _filesSettingsHelper.StoreForcesave = set;
        await _messageService.SendAsync(GetHttpHeaders(), MessageAction.DocumentsStoreForcesave);

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

        return _compressToArchive;
    }

    public bool ChangeDeleteConfrim(bool set)
    {
        _filesSettingsHelper.ConfirmDelete = set;

        return _filesSettingsHelper.ConfirmDelete;
    }

    public AutoCleanUpData ChangeAutomaticallyCleanUp(bool set, DateToAutoCleanUp gap)
    {
        _filesSettingsHelper.AutomaticallyCleanUp = new AutoCleanUpData() { IsAutoCleanUp = set, Gap = gap };

        return _filesSettingsHelper.AutomaticallyCleanUp;
    }

    public AutoCleanUpData GetSettingsAutomaticallyCleanUp()
    {
        return _filesSettingsHelper.AutomaticallyCleanUp;
    }

    public List<FileShare> ChangeDafaultAccessRights(List<FileShare> value)
    {
        _filesSettingsHelper.DefaultSharingAccessRights = value;

        return _filesSettingsHelper.DefaultSharingAccessRights;
    }

    public async Task<IEnumerable<JsonElement>> CreateThumbnailsAsync(List<JsonElement> fileIds)
    {
        try
        {
            var (fileIntIds, _) = FileOperationsManager.GetIds(fileIds);

            _eventBus.Publish(new ThumbnailRequestedIntegrationEvent(_authContext.CurrentAccount.ID, await _tenantManager.GetCurrentTenantIdAsync())
            {
                BaseUrl = _baseCommonLinkUtility.GetFullAbsolutePath(""),
                FileIds = fileIntIds
            });

        }
        catch (Exception e)
        {
            _logger.ErrorCreateThumbnails(e);
        }

        return fileIds;
    }

    public async Task ResendEmailInvitationsAsync<T>(T id, IEnumerable<Guid> usersIds)
    {
        ArgumentNullException.ThrowIfNull(usersIds);

        var folderDao = _daoFactory.GetFolderDao<T>();
        var room = await folderDao.GetFolderAsync(id);

        ErrorIf(room == null, FilesCommonResource.ErrorMassage_FolderNotFound);
        ErrorIf(!await _fileSecurity.CanEditRoomAsync(room), FilesCommonResource.ErrorMassage_SecurityException);

        var shares = (await _fileSharing.GetSharedInfoAsync(room)).ToDictionary(k => k.Id, v => v);

        foreach (var userId in usersIds)
        {
            if (!shares.TryGetValue(userId, out var share))
            {
                continue;
            }

            var user = await _userManager.GetUserAsync(share.Id, null);

            if (user.ActivationStatus != EmployeeActivationStatus.Pending)
            {
                continue;
            }

            var link = await _invitationLinkService.GetInvitationLinkAsync(user.Email, share.Access, _authContext.CurrentAccount.ID);
            await _studioNotifyService.SendEmailRoomInviteAsync(user.Email, room.Title, link);
        }
    }

    public async Task<List<MentionWrapper>> ProtectUsersAsync<T>(T fileId)
    {
        if (!_authContext.IsAuthenticated || _coreBaseSettings.Personal)
        {
            return null;
        }

        var fileDao = GetFileDao<T>();
        var file = await fileDao.GetFileAsync(fileId);

        ErrorIf(file == null, FilesCommonResource.ErrorMassage_FileNotFound);

        var users = new List<MentionWrapper>();
        if (file.RootFolderType == FolderType.BUNCH)
        {
            //todo: request project team
            return new List<MentionWrapper>(users);
        }

        var acesForObject = await _fileSharing.GetSharedInfoAsync(file);

        var usersInfo = new List<UserInfo>();
        foreach (var ace in acesForObject)
        {
            if (ace.Access == FileShare.Restrict || ace.Id.Equals(FileConstant.ShareLinkId))
            {
                continue;
            }

            if (ace.SubjectGroup)
            {
                usersInfo.AddRange(await _userManager.GetUsersByGroupAsync(ace.Id));
            }
            else
            {
                usersInfo.Add(_userManager.GetUsers(ace.Id));
            }
        }

        users = usersInfo.Distinct()
                         .Where(user => !user.Id.Equals(_authContext.CurrentAccount.ID)
                                        && !user.Id.Equals(Constants.LostUser.Id))
                         .Select(user => new MentionWrapper(user, _displayUserSettingsHelper))
                         .ToList();

        users = users
            .OrderBy(user => user.User, UserInfoComparer.Default)
            .ToList();

        return new List<MentionWrapper>(users);
    }

    public string GetHelpCenter()
    {
        return string.Empty; //TODO: Studio.UserControls.Common.HelpCenter.HelpCenter.RenderControlToString();
    }

    private IFolderDao<T> GetFolderDao<T>()
    {
        return _daoFactory.GetFolderDao<T>();
    }

    private IFileDao<T> GetFileDao<T>()
    {
        return _daoFactory.GetFileDao<T>();
    }

    private ITagDao<T> GetTagDao<T>()
    {
        return _daoFactory.GetTagDao<T>();
    }

    private async Task<IDataStore> GetStoreTemplateAsync()
    {
        return await _globalStore.GetStoreTemplateAsync();
    }

    private IProviderDao GetProviderDao<T>()
    {
        return _daoFactory.ProviderDao;
    }

    private ISecurityDao<T> GetSecurityDao<T>()
    {
        return _daoFactory.GetSecurityDao<T>();
    }

    private ILinkDao GetLinkDao<T>()
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
            _logger.Information(error.ToString());
        }
        else
        {
            _logger.ErrorFileStorageService(error);
        }

        return new InvalidOperationException(error.Message, error);
    }

    private IDictionary<string, StringValues> GetHttpHeaders()
    {
        return _httpContextAccessor?.HttpContext?.Request?.Headers?.ToDictionary(k => k.Key, v => v.Value);
    }

    public static string GetAccessString(FileShare fileShare)
    {
        switch (fileShare)
        {
            case FileShare.Read:
            case FileShare.ReadWrite:
            case FileShare.CustomFilter:
            case FileShare.Review:
            case FileShare.FillForms:
            case FileShare.Comment:
            case FileShare.Restrict:
            case FileShare.RoomAdmin:
            case FileShare.Editing:
            case FileShare.Collaborator:
            case FileShare.None:
                return FilesCommonResource.ResourceManager.GetString("AceStatusEnum_" + fileShare.ToStringFast());
            default:
                return string.Empty;
        }
    }

    private async Task<List<AceWrapper>> GetFullAceWrappersAsync(IEnumerable<FileShareParams> share)
    {
        var dict = await share.ToAsyncEnumerable().SelectAwait(async s => await _fileShareParamsHelper.ToAceObjectAsync(s)).ToDictionaryAsync(k => k.Id, v => v);

        var admins = await _userManager.GetUsersByGroupAsync(Constants.GroupAdmin.ID);
        var onlyFilesAdmins = await _userManager.GetUsersByGroupAsync(WebItemManager.DocumentsProductID);

        var userInfos = admins.Union(onlyFilesAdmins).ToList();

        foreach (var userInfo in userInfos)
        {
            dict[userInfo.Id] = new AceWrapper
            {
                Access = FileShare.ReadWrite,
                Id = userInfo.Id
            };
        }

        return dict.Values.ToList();
    }

    private async Task CheckEncryptionKeysAsync(IEnumerable<AceWrapper> aceWrappers)
    {
        var users = aceWrappers.Select(s => s.Id).ToList();
        var keys = await _encryptionLoginProvider.GetKeysAsync(users);

        foreach (var user in users)
        {
            if (!keys.ContainsKey(user))
            {
                var userInfo = await _userManager.GetUsersAsync(user);
                throw new InvalidOperationException($"The user {userInfo.DisplayUserName(_displayUserSettingsHelper)} does not have an encryption key");
            }
        }
    }

    private async Task SetAcesForPrivateRoomAsync<T>(Folder<T> room, List<AceWrapper> aces, bool notify, string sharingMessage)
    {
        var advancedSettings = new AceAdvancedSettingsWrapper
        {
            AllowSharingPrivateRoom = true
        };

        var aceCollection = new AceCollection<T>
        {
            Folders = new[] { room.Id },
            Files = Array.Empty<T>(),
            Aces = aces,
            Message = sharingMessage,
            AdvancedSettings = advancedSettings
        };

        await SetAceObjectAsync(aceCollection, notify);
    }

    private enum EventType
    {
        Update,
        Create,
        Remove
    }
}

public class FileModel<T, TTempate>
{
    public T ParentId { get; set; }
    public string Title { get; set; }
    public TTempate TemplateId { get; set; }
    public int FormId { get; set; }
}
