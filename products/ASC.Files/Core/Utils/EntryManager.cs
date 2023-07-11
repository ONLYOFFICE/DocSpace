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

namespace ASC.Web.Files.Utils;

[Scope]
public class LockerManager
{
    private readonly AuthContext _authContext;
    private readonly IDaoFactory _daoFactory;
    private readonly ThirdPartySelector _thirdPartySelector;
    public LockerManager(AuthContext authContext, IDaoFactory daoFactory, ThirdPartySelector thirdPartySelector)
    {
        _authContext = authContext;
        _daoFactory = daoFactory;
        _thirdPartySelector = thirdPartySelector;
    }

    public async Task<bool> FileLockedForMe<T>(T fileId, Guid userId = default)
    {
        var app = _thirdPartySelector.GetAppByFileId(fileId.ToString());
        if (app != null)
        {
            return false;
        }

        userId = userId == default ? _authContext.CurrentAccount.ID : userId;
        var tagDao = _daoFactory.GetTagDao<T>();
        var lockedBy = await FileLockedBy(fileId, tagDao);

        return lockedBy != Guid.Empty && lockedBy != userId;
    }

    public async Task<bool> FileLockedForMeAsync<T>(T fileId, Guid userId = default)
    {
        var app = _thirdPartySelector.GetAppByFileId(fileId.ToString());
        if (app != null)
        {
            return false;
        }

        userId = userId == default ? _authContext.CurrentAccount.ID : userId;
        var tagDao = _daoFactory.GetTagDao<T>();
        var lockedBy = await FileLockedByAsync(fileId, tagDao);

        return lockedBy != Guid.Empty && lockedBy != userId;
    }

    public async Task<Guid> FileLockedBy<T>(T fileId, ITagDao<T> tagDao)
    {
        var tagLock = await tagDao.GetTagsAsync(fileId, FileEntryType.File, TagType.Locked).FirstOrDefaultAsync();

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
    private readonly IDaoFactory _daoFactory;
    private readonly FileSecurity _fileSecurity;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly AuthContext _authContext;

    public BreadCrumbsManager(
        IDaoFactory daoFactory,
        FileSecurity fileSecurity,
        GlobalFolderHelper globalFolderHelper,
        AuthContext authContext)
    {
        _daoFactory = daoFactory;
        _fileSecurity = fileSecurity;
        _globalFolderHelper = globalFolderHelper;
        _authContext = authContext;
    }

    public Task<List<FileEntry>> GetBreadCrumbsAsync<T>(T folderId)
    {
        var folderDao = _daoFactory.GetFolderDao<T>();

        return GetBreadCrumbsAsync(folderId, folderDao);
    }

    public async Task<List<FileEntry>> GetBreadCrumbsAsync<T>(T folderId, IFolderDao<T> folderDao)
    {
        if (folderId == null)
        {
            return new List<FileEntry>();
        }

        var breadCrumbs = await _fileSecurity.FilterReadAsync(folderDao.GetParentFoldersAsync(folderId)).Cast<FileEntry>().ToListAsync();
        var firstVisible = breadCrumbs.ElementAtOrDefault(0) as Folder<T>;

        var rootId = 0;
        if (firstVisible == null)
        {
            rootId = await _globalFolderHelper.FolderShareAsync;
        }
        else
        {
            switch (firstVisible.FolderType)
            {
                case FolderType.DEFAULT:
                    if (!firstVisible.ProviderEntry)
                    {
                        rootId = await _globalFolderHelper.FolderShareAsync;
                    }
                    else
                    {
                        switch (firstVisible.RootFolderType)
                        {
                            case FolderType.USER:
                                rootId = _authContext.CurrentAccount.ID == firstVisible.RootCreateBy
                                    ? _globalFolderHelper.FolderMy
                                    : await _globalFolderHelper.FolderShareAsync;
                                break;
                            case FolderType.COMMON:
                                rootId = await _globalFolderHelper.FolderCommonAsync;
                                break;
                        }
                    }
                    break;

                case FolderType.BUNCH:
                    rootId = await _globalFolderHelper.FolderProjectsAsync;
                    break;
                case FolderType.VirtualRooms:
                    if (firstVisible.ProviderEntry)
                    {
                        rootId = await _globalFolderHelper.FolderVirtualRoomsAsync;
                        breadCrumbs = breadCrumbs.SkipWhile(f => f is Folder<T> folder && !DocSpaceHelper.IsRoom(folder.FolderType)).ToList();
                    }
                    break;
                case FolderType.Archive:
                    if (firstVisible.ProviderEntry)
                    {
                        rootId = await _globalFolderHelper.FolderArchiveAsync;
                        breadCrumbs = breadCrumbs.SkipWhile(f => f is Folder<T> folder && !DocSpaceHelper.IsRoom(folder.FolderType)).ToList();
                    }
                    break;
            }
        }

        var folderDaoInt = _daoFactory.GetFolderDao<int>();

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
    private readonly IDaoFactory _daoFactory;
    private readonly AuthContext _authContext;
    private readonly Global _global;

    public EntryStatusManager(IDaoFactory daoFactory, AuthContext authContext, Global global)
    {
        _daoFactory = daoFactory;
        _authContext = authContext;
        _global = global;
    }

    public async Task SetFileStatusAsync<T>(File<T> file)
    {
        if (file == null || file.Id == null)
        {
            return;
        }

        await SetFileStatusAsync(new List<File<T>>(1) { file });
    }

    public async Task SetFileStatusAsync<T>(IEnumerable<File<T>> files)
    {
        if (!files.Any())
        {
            return;
        }

        var tagDao = _daoFactory.GetTagDao<T>();

        var tagsTask = tagDao.GetTagsAsync(TagType.Locked, files).ToDictionaryAsync(k => k.EntryId, v => v);
        var tagsNewTask = tagDao.GetNewTagsAsync(_authContext.CurrentAccount.ID, files).ToListAsync();

        var tags = await tagsTask;
        var tagsNew = await tagsNewTask;

        foreach (var file in files)
        {
            if (tags.TryGetValue(file.Id, out var lockedTag))
            {
                var lockedBy = lockedTag.Owner;
                file.Locked = lockedBy != Guid.Empty;
                file.LockedBy = lockedBy != Guid.Empty && lockedBy != _authContext.CurrentAccount.ID
                    ? _global.GetUserName(lockedBy)
                    : null;

                continue;
            }

            if (tagsNew.Any(r => r.EntryId.Equals(file.Id)))
            {
                file.IsNew = true;
            }
        }
    }

    public async Task SetIsFavoriteFolderAsync<T>(Folder<T> folder)
    {
        if (folder == null || folder.Id == null)
        {
            return;
        }

        await SetIsFavoriteFoldersAsync(new List<Folder<T>>(1) { folder });
    }

    public async Task SetIsFavoriteFoldersAsync<T>(IEnumerable<Folder<T>> folders)
    {
        if (!folders.Any())
        {
            return;
        }

        var tagDao = _daoFactory.GetTagDao<T>();

        var tagsFavorite = await tagDao.GetTagsAsync(_authContext.CurrentAccount.ID, TagType.Favorite, folders).ToListAsync();

        foreach (var folder in folders)
        {
            if (tagsFavorite.Any(r => r.EntryId.Equals(folder.Id)))
            {
                folder.IsFavorite = true;
            }
        }
    }
}

[Scope]
public class EntryManager
{
    private const string _updateList = "filesUpdateList";
    private readonly ThirdPartySelector _thirdPartySelector;
    private readonly ThumbnailSettings _thumbnailSettings;

    private readonly ICache _cache;
    private readonly FileTrackerHelper _fileTracker;
    private readonly EntryStatusManager _entryStatusManager;
    private readonly IDaoFactory _daoFactory;
    private readonly FileSecurity _fileSecurity;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly PathProvider _pathProvider;
    private readonly AuthContext _authContext;
    private readonly FileMarker _fileMarker;
    private readonly FileUtility _fileUtility;
    private readonly GlobalStore _globalStore;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly FilesSettingsHelper _filesSettingsHelper;
    private readonly UserManager _userManager;
    private readonly FileShareLink _fileShareLink;
    private readonly DocumentServiceHelper _documentServiceHelper;
    private readonly ThirdpartyConfiguration _thirdpartyConfiguration;
    private readonly DocumentServiceConnector _documentServiceConnector;
    private readonly LockerManager _lockerManager;
    private readonly BreadCrumbsManager _breadCrumbsManager;
    private readonly SettingsManager _settingsManager;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EntryManager> _logger;
    private readonly IHttpClientFactory _clientFactory;
    private readonly FilesMessageService _filesMessageService;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly SocketManager _socketManager;

    public EntryManager(
        IDaoFactory daoFactory,
        FileSecurity fileSecurity,
        GlobalFolderHelper globalFolderHelper,
        PathProvider pathProvider,
        AuthContext authContext,
        FileMarker fileMarker,
        FileUtility fileUtility,
        GlobalStore globalStore,
        CoreBaseSettings coreBaseSettings,
        FilesSettingsHelper filesSettingsHelper,
        UserManager userManager,
        ILogger<EntryManager> logger,
        FileShareLink fileShareLink,
        DocumentServiceHelper documentServiceHelper,
        ThirdpartyConfiguration thirdpartyConfiguration,
        DocumentServiceConnector documentServiceConnector,
        LockerManager lockerManager,
        BreadCrumbsManager breadCrumbsManager,
        SettingsManager settingsManager,
        IServiceProvider serviceProvider,
        ICache cache,
        FileTrackerHelper fileTracker,
        EntryStatusManager entryStatusManager,
        ThirdPartySelector thirdPartySelector,
        IHttpClientFactory clientFactory,
        FilesMessageService filesMessageService,
        ThumbnailSettings thumbnailSettings,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        SocketManager socketManager)
    {
        _daoFactory = daoFactory;
        _fileSecurity = fileSecurity;
        _globalFolderHelper = globalFolderHelper;
        _pathProvider = pathProvider;
        _authContext = authContext;
        _fileMarker = fileMarker;
        _fileUtility = fileUtility;
        _globalStore = globalStore;
        _coreBaseSettings = coreBaseSettings;
        _filesSettingsHelper = filesSettingsHelper;
        _userManager = userManager;
        _fileShareLink = fileShareLink;
        _documentServiceHelper = documentServiceHelper;
        _thirdpartyConfiguration = thirdpartyConfiguration;
        _documentServiceConnector = documentServiceConnector;
        _lockerManager = lockerManager;
        _breadCrumbsManager = breadCrumbsManager;
        _settingsManager = settingsManager;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _cache = cache;
        _fileTracker = fileTracker;
        _entryStatusManager = entryStatusManager;
        _clientFactory = clientFactory;
        _filesMessageService = filesMessageService;
        _thirdPartySelector = thirdPartySelector;
        _thumbnailSettings = thumbnailSettings;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _socketManager = socketManager;
    }

    public async Task<(IEnumerable<FileEntry> Entries, int Total)> GetEntriesAsync<T>(Folder<T> parent, int from, int count, FilterType filterType, bool subjectGroup, Guid subjectId,
        string searchText, bool searchInContent, bool withSubfolders, OrderBy orderBy, T roomId = default, SearchArea searchArea = SearchArea.Active, bool withoutTags = false, IEnumerable<string> tagNames = null,
        bool excludeSubject = false, ProviderFilter provider = ProviderFilter.None, SubjectFilter subjectFilter = SubjectFilter.Owner)
    {
        var total = 0;

        if (parent == null)
        {
            throw new ArgumentNullException(nameof(parent), FilesCommonResource.ErrorMassage_FolderNotFound);
        }

        if (parent.ProviderEntry && !_filesSettingsHelper.EnableThirdParty)
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_ReadFolder);
        }

        if (parent.RootFolderType == FolderType.Privacy && (!PrivacyRoomSettings.IsAvailable() || !PrivacyRoomSettings.GetEnabled(_settingsManager)))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_ReadFolder);
        }

        var entries = new List<FileEntry>();

        searchInContent = searchInContent && filterType != FilterType.ByExtension && !Equals(parent.Id, _globalFolderHelper.FolderTrash);

        if (parent.FolderType == FolderType.TRASH)
        {
            withSubfolders = false;
        }

        if (parent.FolderType == FolderType.Projects && parent.Id.Equals(await _globalFolderHelper.FolderProjectsAsync))
        {

        }
        else if (parent.FolderType == FolderType.SHARE)
        {
            //share
            var shared = await _fileSecurity.GetSharesForMeAsync(filterType, subjectGroup, subjectId, searchText, searchInContent, withSubfolders).ToListAsync();

            entries.AddRange(shared);

            CalculateTotal();
        }
        else if (parent.FolderType == FolderType.Recent)
        {
            var files = await GetRecentAsync(filterType, subjectGroup, subjectId, searchText, searchInContent);
            entries.AddRange(files);

            CalculateTotal();
        }
        else if (parent.FolderType == FolderType.Favorites)
        {
            var (files, folders) = await GetFavoritesAsync(filterType, subjectGroup, subjectId, searchText, searchInContent);

            entries.AddRange(folders);
            entries.AddRange(files);

            CalculateTotal();
        }
        else if (parent.FolderType == FolderType.Templates)
        {
            var folderDao = _daoFactory.GetFolderDao<T>();
            var fileDao = _daoFactory.GetFileDao<T>();
            var files = await GetTemplatesAsync(folderDao, fileDao, filterType, subjectGroup, subjectId, searchText, searchInContent).ToListAsync();
            entries.AddRange(files);

            CalculateTotal();
        }
        else if (parent.FolderType == FolderType.Privacy)
        {
            var folderDao = _daoFactory.GetFolderDao<T>();
            var fileDao = _daoFactory.GetFileDao<T>();

            var folders = folderDao.GetFoldersAsync(parent.Id, orderBy, filterType, subjectGroup, subjectId, searchText, withSubfolders);
            var files = fileDao.GetFilesAsync(parent.Id, orderBy, filterType, subjectGroup, subjectId, searchText, searchInContent, withSubfolders);
            //share
            var shared = _fileSecurity.GetPrivacyForMeAsync(filterType, subjectGroup, subjectId, searchText, searchInContent, withSubfolders);

            var task1 = _fileSecurity.FilterReadAsync(folders).ToListAsync();
            var task2 = _fileSecurity.FilterReadAsync(files).ToListAsync();
            var task3 = shared.ToListAsync();

            entries.AddRange(await task1);
            entries.AddRange(await task2);
            entries.AddRange(await task3);

            CalculateTotal();
        }
        else if ((parent.FolderType == FolderType.VirtualRooms || parent.FolderType == FolderType.Archive) && !parent.ProviderEntry)
        {
            entries = await _fileSecurity.GetVirtualRoomsAsync(filterType, subjectId, searchText, searchInContent, withSubfolders, searchArea, withoutTags, tagNames, excludeSubject, provider, subjectFilter);

            CalculateTotal();
        }
        else if (!parent.ProviderEntry)
        {
            var folderDao = _daoFactory.GetFolderDao<T>();
            var fileDao = _daoFactory.GetFileDao<T>();

            var allFoldersCountTask = folderDao.GetFoldersCountAsync(parent.Id, filterType, subjectGroup, subjectId, searchText, withSubfolders, excludeSubject);
            var allFilesCountTask = fileDao.GetFilesCountAsync(parent.Id, filterType, subjectGroup, subjectId, searchText, withSubfolders, excludeSubject);

            var folders = await folderDao.GetFoldersAsync(parent.Id, orderBy, filterType, subjectGroup, subjectId, searchText, withSubfolders, excludeSubject, from, count)
                .ToListAsync();

            var filesCount = count - folders.Count;
            var filesOffset = folders.Count > 0 ? 0 : from - await allFoldersCountTask;

            var files = await fileDao.GetFilesAsync(parent.Id, orderBy, filterType, subjectGroup, subjectId, searchText, searchInContent, withSubfolders, excludeSubject, filesOffset, filesCount)
                .ToListAsync();

            entries = new List<FileEntry>(folders.Count + files.Count);
            entries.AddRange(folders);
            entries.AddRange(files);

            var fileStatusTask = _entryStatusManager.SetFileStatusAsync(files);
            var tagsNewTask = _fileMarker.SetTagsNewAsync(parent, entries);
            var originsTask = SetOriginsAsync(parent, entries);

            await Task.WhenAll(fileStatusTask, tagsNewTask, originsTask);

            total = await allFoldersCountTask + await allFilesCountTask;

            return (entries, total);
        }
        else
        {
            var folders = _daoFactory.GetFolderDao<T>().GetFoldersAsync(parent.Id, orderBy, filterType, subjectGroup, subjectId, searchText, withSubfolders, excludeSubject);
            var files = _daoFactory.GetFileDao<T>().GetFilesAsync(parent.Id, orderBy, filterType, subjectGroup, subjectId, searchText, searchInContent, withSubfolders, excludeSubject);

            var task1 = _fileSecurity.FilterReadAsync(folders).ToListAsync();
            var task2 = _fileSecurity.FilterReadAsync(files).ToListAsync();

            if (filterType == FilterType.None || filterType == FilterType.FoldersOnly)
            {
                var folderList = GetThirpartyFoldersAsync(parent, searchText);
                var thirdPartyFolder = FilterEntries(folderList, filterType, subjectGroup, subjectId, searchText, searchInContent);

                var task3 = thirdPartyFolder.ToListAsync();

                foreach (var items in await Task.WhenAll(task1.AsTask(), task2.AsTask()))
                {
                    entries.AddRange(items);
                }

                entries.AddRange(await task3);
            }
            else
            {
                foreach (var items in await Task.WhenAll(task1.AsTask(), task2.AsTask()))
                {
                    entries.AddRange(items);
                }
            }
        }

        total = entries.Count;

        IEnumerable<FileEntry> data = entries;

        if (orderBy.SortedBy != SortedByType.New)
        {
            if (parent.FolderType != FolderType.Recent)
            {
                data = SortEntries<T>(data, orderBy);
            }

            if (0 < from)
            {
                data = data.Skip(from);
            }

            if (0 < count)
            {
                data = data.Take(count);
            }

            data = data.ToList();
        }

        await _fileMarker.SetTagsNewAsync(parent, data);

        //sorting after marking
        if (orderBy.SortedBy == SortedByType.New)
        {
            data = SortEntries<T>(data, orderBy);

            if (0 < from)
            {
                data = data.Skip(from);
            }

            if (0 < count)
            {
                data = data.Take(count);
            }

            data = data.ToList();
        }

        var internalFiles = new List<File<int>>();
        var internalFolders = new List<Folder<int>>();
        var thirdPartyFiles = new List<File<string>>();
        var thirdPartyFolders = new List<Folder<string>>();

        foreach (var item in data.Where(r => r != null))
        {
            if (item.FileEntryType == FileEntryType.File)
            {
                if (item is File<int> internalFile)
                {
                    internalFiles.Add(internalFile);
                }
                else if (item is File<string> thirdPartyFile)
                {
                    thirdPartyFiles.Add(thirdPartyFile);
                }
            }
            else
            {
                if (item is Folder<int> internalFolder)
                {
                    internalFolders.Add(internalFolder);
                }
                else if (item is Folder<string> thirdPartyFolder)
                {
                    thirdPartyFolders.Add(thirdPartyFolder);
                }
            }
        }

        var t1 = _entryStatusManager.SetFileStatusAsync(internalFiles);
        var t2 = _entryStatusManager.SetIsFavoriteFoldersAsync(internalFolders);
        var t3 = _entryStatusManager.SetFileStatusAsync(thirdPartyFiles);
        var t4 = _entryStatusManager.SetIsFavoriteFoldersAsync(thirdPartyFolders);
        await Task.WhenAll(t1, t2, t3, t4);

        return (data, total);

        void CalculateTotal()
        {
            foreach (var f in entries)
            {
                if (f is IFolder fold)
                {
                    parent.FilesCount += fold.FilesCount;
                    parent.FoldersCount += fold.FoldersCount + 1;
                }
                else
                {
                    parent.FilesCount += 1;
                }
            }
        }
    }

    public async IAsyncEnumerable<FileEntry<T>> GetTemplatesAsync<T>(IFolderDao<T> folderDao, IFileDao<T> fileDao, FilterType filter, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent)
    {
        var tagDao = _daoFactory.GetTagDao<T>();
        var tags = tagDao.GetTagsAsync(_authContext.CurrentAccount.ID, TagType.Template);

        var fileIds = await tags.Where(tag => tag.EntryType == FileEntryType.File).Select(tag => (T)Convert.ChangeType(tag.EntryId, typeof(T))).ToArrayAsync();

        var filesAsync = fileDao.GetFilesFilteredAsync(fileIds, filter, subjectGroup, subjectId, searchText, searchInContent);
        var files = _fileSecurity.FilterReadAsync(filesAsync.Where(file => file.RootFolderType != FolderType.TRASH));

        await foreach (var file in files)
        {
            await CheckEntryAsync(folderDao, file);
            yield return file;
        }
    }

    public async IAsyncEnumerable<Folder<string>> GetThirpartyFoldersAsync<T>(Folder<T> parent, string searchText = null)
    {
        if ((parent.Id.Equals(_globalFolderHelper.FolderMy) || parent.Id.Equals(await _globalFolderHelper.FolderCommonAsync))
            && _thirdpartyConfiguration.SupportInclusion(_daoFactory)
            && (_filesSettingsHelper.EnableThirdParty
                || _coreBaseSettings.Personal))
        {
            var providerDao = _daoFactory.ProviderDao;
            if (providerDao == null)
            {
                yield break;
            }

            var providers = providerDao.GetProvidersInfoAsync(parent.RootFolderType, searchText);
            var securityDao = _daoFactory.GetSecurityDao<string>();

            await foreach (var e in providers)
            {
                var fake = GetFakeThirdpartyFolder(e, parent.Id.ToString());
                if (await _fileSecurity.CanReadAsync(fake))
                {
                    var pureShareRecords = securityDao.GetPureShareRecordsAsync(fake);
                    var isShared = await pureShareRecords
                    //.Where(x => x.Owner == SecurityContext.CurrentAccount.ID)
                    .Where(x => fake.Id.Equals(x.EntryId))
                    .AnyAsync();

                    if (isShared)
                    {
                        fake.Shared = true;
                    }

                    yield return fake;
                }
            }
        }
    }

    public async Task<IEnumerable<FileEntry>> GetRecentAsync(FilterType filter, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent)
    {
        var tagDao = _daoFactory.GetTagDao<int>();
        var tags = tagDao.GetTagsAsync(_authContext.CurrentAccount.ID, TagType.Recent).Where(tag => tag.EntryType == FileEntryType.File).Select(r => r.EntryId);

        var fileIdsInt = Enumerable.Empty<int>();
        var fileIdsString = Enumerable.Empty<string>();
        var listFileIds = new List<string>();

        await foreach (var fileId in tags)
        {
            if (fileId is int @int)
            {
                fileIdsInt = fileIdsInt.Append(@int);
            }
            if (fileId is string @string)
            {
                fileIdsString = fileIdsString.Append(@string);
            }

            listFileIds.Add(fileId.ToString());
        }

        var files = new List<FileEntry>();

        var firstTask = GetRecentByIdsAsync(fileIdsInt, filter, subjectGroup, subjectId, searchText, searchInContent).ToListAsync();
        var secondTask = GetRecentByIdsAsync(fileIdsString, filter, subjectGroup, subjectId, searchText, searchInContent).ToListAsync();

        foreach (var items in await Task.WhenAll(firstTask.AsTask(), secondTask.AsTask()))
        {
            files.AddRange(items);
        }

        var result = files.OrderBy(file =>
        {
            var fileId = "";
            if (file is File<int> fileInt)
            {
                fileId = fileInt.Id.ToString();
            }
            else if (file is File<string> fileString)
            {
                fileId = fileString.Id;
            }

            return listFileIds.IndexOf(fileId.ToString());
        });

        return result;

        async IAsyncEnumerable<FileEntry> GetRecentByIdsAsync<T>(IEnumerable<T> fileIds, FilterType filter, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent)
        {
            var folderDao = _daoFactory.GetFolderDao<T>();
            var fileDao = _daoFactory.GetFileDao<T>();

            var files = _fileSecurity.FilterReadAsync(fileDao.GetFilesFilteredAsync(fileIds, filter, subjectGroup, subjectId, searchText, searchInContent).Where(file => file.RootFolderType != FolderType.TRASH));

            await foreach (var file in files)
            {
                await CheckEntryAsync(folderDao, file);
                yield return file;
            }
        }
    }

    public async Task<(IEnumerable<FileEntry>, IEnumerable<FileEntry>)> GetFavoritesAsync(FilterType filter, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent)
    {
        var tagDao = _daoFactory.GetTagDao<int>();
        var tags = tagDao.GetTagsAsync(_authContext.CurrentAccount.ID, TagType.Favorite);

        var fileIdsInt = new List<int>();
        var fileIdsString = new List<string>();
        var folderIdsInt = new List<int>();
        var folderIdsString = new List<string>();

        await foreach (var tag in tags)
        {
            if (tag.EntryType == FileEntryType.File)
            {
                if (tag.EntryId is int)
                {
                    fileIdsInt.Add((int)tag.EntryId);
                }
                else if (tag.EntryId is string)
                {
                    fileIdsString.Add((string)tag.EntryId);
                }
            }
            else
            {
                if (tag.EntryId is int)
                {
                    folderIdsInt.Add((int)tag.EntryId);
                }
                else if (tag.EntryId is string)
                {
                    folderIdsString.Add((string)tag.EntryId);
                }
            }
        }

        var (filesInt, foldersInt) = await GetFavoritesByIdAsync(fileIdsInt, folderIdsInt, filter, subjectGroup, subjectId, searchText, searchInContent);
        var (filesString, foldersString) = await GetFavoritesByIdAsync(fileIdsString, folderIdsString, filter, subjectGroup, subjectId, searchText, searchInContent);

        var files = new List<FileEntry>(filesInt);
        files.AddRange(filesString);

        var folders = new List<FileEntry>(foldersInt);
        files.AddRange(foldersString);

        return (files, folders);

        async Task<(IEnumerable<FileEntry>, IEnumerable<FileEntry>)> GetFavoritesByIdAsync<T>(IEnumerable<T> fileIds, IEnumerable<T> folderIds, FilterType filter, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent)
        {
            var folderDao = _daoFactory.GetFolderDao<T>();
            var fileDao = _daoFactory.GetFileDao<T>();
            var asyncFolders = folderDao.GetFoldersAsync(folderIds, filter, subjectGroup, subjectId, searchText, false, false);
            var asyncFiles = fileDao.GetFilesFilteredAsync(fileIds, filter, subjectGroup, subjectId, searchText, searchInContent, true);

            List<FileEntry<T>> files = new();
            List<FileEntry<T>> folders = new();

            if (filter == FilterType.None || filter == FilterType.FoldersOnly)
            {
                var tmpFolders = asyncFolders.Where(folder => folder.RootFolderType != FolderType.TRASH);

                folders = await _fileSecurity.FilterReadAsync(tmpFolders).ToListAsync();

                await CheckFolderIdAsync(folderDao, folders);
            }

            if (filter != FilterType.FoldersOnly)
            {
                var tmpFiles = asyncFiles.Where(file => file.RootFolderType != FolderType.TRASH);

                files = await _fileSecurity.FilterReadAsync(tmpFiles).ToListAsync();

                await CheckFolderIdAsync(folderDao, folders);
            }

            return (files, folders);
        }
    }

    public IAsyncEnumerable<FileEntry<T>> FilterEntries<T>(IAsyncEnumerable<FileEntry<T>> entries, FilterType filter, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent)
    {
        if (entries == null)
        {
            return entries;
        }

        if (subjectId != Guid.Empty)
        {
            entries = entries.Where(f =>
                                    subjectGroup
                                        ? _userManager.GetUsersByGroup(subjectId).Any(s => s.Id == f.CreateBy)
                                        : f.CreateBy == subjectId
                );
        }

        Func<FileEntry<T>, bool> where = null;

        switch (filter)
        {
            case FilterType.SpreadsheetsOnly:
            case FilterType.PresentationsOnly:
            case FilterType.ImagesOnly:
            case FilterType.DocumentsOnly:
            case FilterType.OFormOnly:
            case FilterType.OFormTemplateOnly:
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
            entries = entries.Where(where);
        }

        searchText = (searchText ?? string.Empty).ToLower().Trim();

        if ((!searchInContent || filter == FilterType.ByExtension) && !string.IsNullOrEmpty(searchText))
        {
            entries = entries.Where(f => f.Title.Contains(searchText, StringComparison.InvariantCultureIgnoreCase));
        }

        return entries;
    }

    public IEnumerable<FileEntry> SortEntries<T>(IEnumerable<FileEntry> entries, OrderBy orderBy)
    {
        if (entries == null || !entries.Any())
        {
            return entries;
        }

        if (orderBy == null)
        {
            orderBy = _filesSettingsHelper.DefaultOrder;
        }

        var c = orderBy.IsAsc ? 1 : -1;
        Comparison<FileEntry> sorter = orderBy.SortedBy switch
        {
            SortedByType.Type => (x, y) =>
            {
                var cmp = 0;
                if (x.FileEntryType == FileEntryType.File && y.FileEntryType == FileEntryType.File)
                {
                    cmp = c * FileUtility.GetFileExtension(x.Title).CompareTo(FileUtility.GetFileExtension(y.Title));
                }

                return cmp == 0 ? x.Title.EnumerableComparer(y.Title) : cmp;
            }
            ,
            SortedByType.RoomType => (x, y) =>
            {
                var cmp = 0;

                if (x is IFolder x1 && DocSpaceHelper.IsRoom(x1.FolderType)
                    && y is IFolder x2 && DocSpaceHelper.IsRoom(x2.FolderType))
                {
                    cmp = c * Enum.GetName(x1.FolderType).EnumerableComparer(Enum.GetName(x2.FolderType));
                }

                return cmp == 0 ? x.Title.EnumerableComparer(y.Title) : cmp;
            }
            ,
            SortedByType.Tags => (x, y) =>
            {
                var cmp = 0;

                if (x is IFolder x1 && DocSpaceHelper.IsRoom(x1.FolderType)
                    && y is IFolder x2 && DocSpaceHelper.IsRoom(x2.FolderType))
                {
                    cmp = c * x1.Tags.Count().CompareTo(x2.Tags.Count());
                }

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
                {
                    cmp = c * ((File<T>)x).ContentLength.CompareTo(((File<T>)y).ContentLength);
                }

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
            SortedByType.Room => (x, y) =>
            {
                var x1 = x.OriginRoomTitle;
                var x2 = y.OriginRoomTitle;

                if (x1 == null && x2 == null)
                {
                    return 0;
                }

                if (x1 == null)
                {
                    return c * 1;
                }

                if (x2 == null)
                {
                    return c * -1;
                }

                return c * x1.EnumerableComparer(x2);
            }
            ,
            _ => (x, y) => c * x.Title.EnumerableComparer(y.Title),
        };

        var comparer = Comparer<FileEntry>.Create(sorter);

        if (orderBy.SortedBy != SortedByType.New)
        {
            var rooms = entries.Where(r => r.FileEntryType == FileEntryType.Folder && DocSpaceHelper.IsRoom(((IFolder)r).FolderType));
            var pinnedRooms = rooms.Where(r => ((IFolder)r).Pinned);
            rooms = rooms.Except(pinnedRooms);

            var folders = entries.Where(r => r.FileEntryType == FileEntryType.Folder).Except(pinnedRooms).Except(rooms);
            var files = entries.Where(r => r.FileEntryType == FileEntryType.File);
            pinnedRooms = pinnedRooms.OrderBy(r => r, comparer);
            rooms = rooms.OrderBy(r => r, comparer);
            folders = folders.OrderBy(r => r, comparer);
            files = files.OrderBy(r => r, comparer);

            return pinnedRooms.Concat(rooms).Concat(folders).Concat(files);
        }

        return entries.OrderBy(r => r, comparer);
    }

    public Folder<string> GetFakeThirdpartyFolder(IProviderInfo providerInfo, string parentFolderId = null)
    {
        //Fake folder. Don't send request to third party
        var folder = _serviceProvider.GetService<Folder<string>>();

        folder.ParentId = parentFolderId;

        folder.Id = providerInfo.RootFolderId;
        folder.CreateBy = providerInfo.Owner;
        folder.CreateOn = providerInfo.CreateOn;
        folder.FolderType = FolderType.DEFAULT;
        folder.ModifiedBy = providerInfo.Owner;
        folder.ModifiedOn = providerInfo.CreateOn;
        folder.ProviderId = providerInfo.ID;
        folder.ProviderKey = providerInfo.ProviderKey;
        folder.RootCreateBy = providerInfo.Owner;
        folder.RootId = providerInfo.RootFolderId;
        folder.RootFolderType = providerInfo.RootFolderType;
        folder.Shareable = false;
        folder.Title = providerInfo.CustomerTitle;
        folder.FilesCount = 0;
        folder.FoldersCount = 0;

        return folder;
    }

    public Task<List<FileEntry>> GetBreadCrumbsAsync<T>(T folderId)
    {
        return _breadCrumbsManager.GetBreadCrumbsAsync(folderId);
    }

    public Task<List<FileEntry>> GetBreadCrumbsAsync<T>(T folderId, IFolderDao<T> folderDao)
    {
        return _breadCrumbsManager.GetBreadCrumbsAsync(folderId, folderDao);
    }

    public async Task CheckFolderIdAsync<T>(IFolderDao<T> folderDao, IEnumerable<FileEntry<T>> entries)
    {
        foreach (var entry in entries)
        {
            await CheckEntryAsync(folderDao, entry);
        }
    }

    public async Task CheckEntryAsync<T>(IFolderDao<T> folderDao, FileEntry<T> entry)
    {
        if (entry.RootFolderType == FolderType.USER
            && entry.RootCreateBy != _authContext.CurrentAccount.ID)
        {
            var folderId = entry.ParentId;
            var folder = await folderDao.GetFolderAsync(folderId);
            if (!await _fileSecurity.CanReadAsync(folder))
            {
                entry.FolderIdDisplay = await _globalFolderHelper.GetFolderShareAsync<T>();
            }
        }
    }

    public Task<bool> FileLockedForMeAsync<T>(T fileId, Guid userId = default)
    {
        return _lockerManager.FileLockedForMeAsync(fileId, userId);
    }

    public Task<Guid> FileLockedByAsync<T>(T fileId, ITagDao<T> tagDao)
    {
        return _lockerManager.FileLockedByAsync(fileId, tagDao);
    }

    public async Task<(File<T> file, Folder<T> folderIfNew)> GetFillFormDraftAsync<T>(File<T> sourceFile)
    {
        Folder<T> folderIfNew = null;
        if (sourceFile == null)
        {
            return (null, folderIfNew);
        }

        File<T> linkedFile = null;
        var fileDao = _daoFactory.GetFileDao<T>();
        var sourceFileDao = _daoFactory.GetFileDao<T>();
        var linkDao = _daoFactory.GetLinkDao();

        var linkedId = await linkDao.GetLinkedAsync(sourceFile.Id.ToString());

        if (linkedId != null)
        {
            linkedFile = await fileDao.GetFileAsync((T)Convert.ChangeType(linkedId, typeof(T)));
            if (linkedFile == null
                || !await _fileSecurity.CanFillFormsAsync(linkedFile)
                || await FileLockedForMeAsync(linkedFile.Id)
                || linkedFile.RootFolderType == FolderType.TRASH)
            {
                await linkDao.DeleteLinkAsync(sourceFile.Id.ToString());
                linkedFile = null;
            }
        }

        if (linkedFile == null)
        {
            var folderId = sourceFile.ParentId;
            var folderDao = _daoFactory.GetFolderDao<T>();
            folderIfNew = await folderDao.GetFolderAsync(folderId);
            if (folderIfNew == null)
            {
                throw new Exception(FilesCommonResource.ErrorMassage_FolderNotFound);
            }

            if (!await _fileSecurity.CanFillFormsAsync(folderIfNew))
            {
                throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_Create);
            }

            var ext = FileUtility.GetFileExtension(sourceFile.Title);
            var sourceTitle = Path.GetFileNameWithoutExtension(sourceFile.Title);
            var title = $"{sourceTitle}-{DateTime.UtcNow:s}";

            if (sourceFile.ProviderEntry)
            {
                var user = _userManager.GetUsers(_authContext.CurrentAccount.ID);
                var displayedName = user.DisplayUserName(_displayUserSettingsHelper);

                title += $" ({displayedName})";
            }

            title += ext;

            linkedFile = _serviceProvider.GetService<File<T>>();
            linkedFile.Title = Global.ReplaceInvalidCharsAndTruncate(title);
            linkedFile.ParentId = folderIfNew.Id;
            linkedFile.FileStatus = sourceFile.FileStatus;
            linkedFile.ConvertedType = sourceFile.ConvertedType;
            linkedFile.Comment = FilesCommonResource.CommentCreateFillFormDraft;
            linkedFile.Encrypted = sourceFile.Encrypted;

            using (var stream = await sourceFileDao.GetFileStreamAsync(sourceFile))
            {
                linkedFile.ContentLength = stream.CanSeek ? stream.Length : sourceFile.ContentLength;
                linkedFile = await fileDao.SaveFileAsync(linkedFile, stream);
            }

            await _fileMarker.MarkAsNewAsync(linkedFile);

            await _socketManager.CreateFileAsync(linkedFile);

            await linkDao.AddLinkAsync(sourceFile.Id.ToString(), linkedFile.Id.ToString());
        }

        return (linkedFile, folderIfNew);
    }

    public async Task<bool> LinkedForMeAsync<T>(File<T> file)
    {
        if (file == null || !_fileUtility.CanWebRestrictedEditing(file.Title))
        {
            return false;
        }

        var linkDao = _daoFactory.GetLinkDao();
        var sourceId = await linkDao.GetSourceAsync(file.Id.ToString());

        return !string.IsNullOrEmpty(sourceId);
    }

    public async Task<bool> CheckFillFormDraftAsync<T>(File<T> linkedFile)
    {
        if (linkedFile == null)
        {
            return false;
        }

        var linkDao = _daoFactory.GetLinkDao();
        var sourceId = await linkDao.GetSourceAsync(linkedFile.Id.ToString());
        if (sourceId == null)
        {
            return false;
        }

        if (int.TryParse(sourceId, out var sId))
        {
            return await CheckAsync(sId);
        }

        return await CheckAsync(sourceId);

        async Task<bool> CheckAsync<T1>(T1 sourceId)
        {
            var fileDao = _daoFactory.GetFileDao<T1>();
            var sourceFile = await fileDao.GetFileAsync(sourceId);
            if (sourceFile == null
                || !await _fileSecurity.CanFillFormsAsync(sourceFile)
                || sourceFile.Access != FileShare.FillForms)
            {
                await linkDao.DeleteLinkAsync(sourceId.ToString());

                return false;
            }

            return true;
        }
    }

    public async Task<bool> SubmitFillForm<T>(File<T> draft)
    {
        if (draft == null)
        {
            return false;
        }

        try
        {
            var linkDao = _daoFactory.GetLinkDao();
            var sourceId = await linkDao.GetSourceAsync(draft.Id.ToString());
            if (sourceId == null)
            {
                throw new Exception("Link source is not found");
            }

            if (int.TryParse(sourceId, out var sId))
            {
                return await SubmitFillFormFromSource(draft, sId);
            }

            return await SubmitFillFormFromSource(draft, sourceId);
        }
        catch (Exception e)
        {
            _logger.WarningWithException(string.Format("Error on submit form {0}", draft.Id), e);
            return false;
        }
    }

    private async Task<bool> SubmitFillFormFromSource<TDraft, TSource>(File<TDraft> draft, TSource sourceId)
    {
        try
        {
            var linkDao = _daoFactory.GetLinkDao();
            var fileSourceDao = _daoFactory.GetFileDao<TSource>();
            var fileDraftDao = _daoFactory.GetFileDao<TDraft>();
            var folderSourceDao = _daoFactory.GetFolderDao<TSource>();
            var folderDraftDao = _daoFactory.GetFolderDao<TDraft>();

            if (sourceId == null)
            {
                throw new Exception("Link source is not found");
            }

            var sourceFile = await fileSourceDao.GetFileAsync(sourceId);
            if (sourceFile == null)
            {
                throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound, draft.Id.ToString());
            }

            if (!_fileUtility.CanWebRestrictedEditing(sourceFile.Title))
            {
                throw new Exception(FilesCommonResource.ErrorMassage_NotSupportedFormat);
            }

            var properties = await fileSourceDao.GetProperties(sourceFile.Id);
            if (properties == null
                || properties.FormFilling == null
                || !properties.FormFilling.CollectFillForm)
            {
                throw new Exception(FilesCommonResource.ErrorMassage_BadRequest);
            }

            var folderId = (TSource)Convert.ChangeType(properties.FormFilling.ToFolderId, typeof(TSource));
            if (!Equals(folderId, default(TSource)))
            {
                var folder = await folderSourceDao.GetFolderAsync(folderId);
                if (folder == null)
                {
                    folderId = sourceFile.ParentId;
                }
            }
            else
            {
                folderId = sourceFile.ParentId;
            }

            //todo: think about right to create in folder

            if (!string.IsNullOrEmpty(properties.FormFilling.CreateFolderTitle))
            {
                var newFolderTitle = Global.ReplaceInvalidCharsAndTruncate(properties.FormFilling.CreateFolderTitle);

                var folder = await folderSourceDao.GetFolderAsync(newFolderTitle, folderId);
                if (folder == null)
                {
                    folder = new Folder<TSource> { Title = newFolderTitle, ParentId = folderId };

                    folderId = await folderSourceDao.SaveFolderAsync(folder);
                    folder = await folderSourceDao.GetFolderAsync(folderId);

                    await _socketManager.CreateFolderAsync(folder);

                    _ = _filesMessageService.Send(folder, MessageInitiator.DocsService, MessageAction.FolderCreated, folder.Title);
                }

                folderId = folder.Id;
            }
            //todo: think about right to create in folder

            var title = properties.FormFilling.GetTitleByMask(sourceFile.Title);

            var submitFile = new File<TSource>
            {
                Title = title,
                ParentId = folderId,
                FileStatus = draft.FileStatus,
                ConvertedType = draft.ConvertedType,
                Comment = FilesCommonResource.CommentSubmitFillForm,
                Encrypted = draft.Encrypted,
            };

            using (var stream = await fileDraftDao.GetFileStreamAsync(draft))
            {
                submitFile.ContentLength = stream.CanSeek ? stream.Length : draft.ContentLength;
                submitFile = await fileSourceDao.SaveFileAsync(submitFile, stream);
            }

            _ = _filesMessageService.Send(submitFile, MessageInitiator.DocsService, MessageAction.FileCreated, submitFile.Title);

            await _fileMarker.MarkAsNewAsync(submitFile);

            return true;

        }
        catch (Exception e)
        {
            _logger.WarningWithException(string.Format("Error on submit form {0}", draft.Id), e);
            return false;
        }
    }

    public async Task<File<T>> SaveEditingAsync<T>(T fileId, string fileExtension, string downloadUri, Stream stream, string doc, string comment = null, bool checkRight = true, bool encrypted = false, ForcesaveType? forcesave = null, bool keepLink = false)
    {
        var newExtension = string.IsNullOrEmpty(fileExtension)
                          ? FileUtility.GetFileExtension(downloadUri)
                          : fileExtension;

        if (!string.IsNullOrEmpty(newExtension))
        {
            newExtension = "." + newExtension.Trim('.');
        }

        var app = _thirdPartySelector.GetAppByFileId(fileId.ToString());
        if (app != null)
        {
            await app.SaveFileAsync(fileId.ToString(), newExtension, downloadUri, stream);

            return null;
        }

        var fileDao = _daoFactory.GetFileDao<T>();
        var check = await _fileShareLink.CheckAsync(doc, false, fileDao);
        var editLink = check.EditLink;
        var file = check.File;
        if (file == null)
        {
            file = await fileDao.GetFileAsync(fileId);
        }

        if (file == null)
        {
            throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
        }

        if (checkRight && !editLink && (!await _fileSecurity.CanFillFormsAsync(file) || _userManager.IsUser(_authContext.CurrentAccount.ID)))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_EditFile);
        }

        if (checkRight && await FileLockedForMeAsync(file.Id))
        {
            throw new Exception(FilesCommonResource.ErrorMassage_LockedFile);
        }

        if (checkRight && (!forcesave.HasValue || forcesave.Value == ForcesaveType.None) && _fileTracker.IsEditing(file.Id))
        {
            throw new Exception(FilesCommonResource.ErrorMassage_SecurityException_UpdateEditingFile);
        }

        if (file.RootFolderType == FolderType.TRASH)
        {
            throw new Exception(FilesCommonResource.ErrorMassage_ViewTrashItem);
        }

        var currentExt = file.ConvertedExtension;
        if (string.IsNullOrEmpty(newExtension))
        {
            newExtension = FileUtility.GetFileExtension(file.Title);
        }

        var replaceVersion = false;
        if (file.Forcesave != ForcesaveType.None)
        {
            if (file.Forcesave == ForcesaveType.User && _filesSettingsHelper.StoreForcesave || encrypted)
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
            if (file.Version != 1 || string.IsNullOrEmpty(currentExt))
            {
                file.VersionGroup++;
            }
            else
            {
                var storeTemplate = _globalStore.GetStoreTemplate();

                var path = FileConstant.NewDocPath + Thread.CurrentThread.CurrentCulture + "/";
                if (!await storeTemplate.IsDirectoryAsync(path))
                {
                    path = FileConstant.NewDocPath + "en-US/";
                }

                var fileExt = currentExt != _fileUtility.MasterFormExtension
                    ? _fileUtility.GetInternalExtension(file.Title)
                    : currentExt;

                path += "new" + fileExt;

                //todo: think about the criteria for saving after creation
                if (!await storeTemplate.IsFileAsync(path) || file.ContentLength != await storeTemplate.GetFileSizeAsync("", path))
                {
                    file.VersionGroup++;
                }
            }
            file.Version++;

            if (file.VersionGroup == 1)
            {
                file.VersionGroup++;
            }
        }
        file.Forcesave = forcesave ?? ForcesaveType.None;

        if (string.IsNullOrEmpty(comment))
        {
            comment = FilesCommonResource.CommentEdit;
        }

        file.Encrypted = encrypted;

        file.ConvertedType = FileUtility.GetFileExtension(file.Title) != newExtension ? newExtension : null;
        file.ThumbnailStatus = encrypted ? Thumbnail.NotRequired : Thumbnail.Waiting;

        if (file.ProviderEntry && !newExtension.Equals(currentExt))
        {
            if (_fileUtility.ExtsConvertible.ContainsKey(newExtension) && _fileUtility.ExtsConvertible[newExtension].Contains(currentExt))
            {
                if (stream != null)
                {
                    downloadUri = await _pathProvider.GetTempUrlAsync(stream, newExtension);
                    downloadUri = _documentServiceConnector.ReplaceCommunityAdress(downloadUri);
                }

                var key = DocumentServiceConnector.GenerateRevisionId(downloadUri);

                var resultTuple = await _documentServiceConnector.GetConvertedUriAsync(downloadUri, newExtension, currentExt, key, null, CultureInfo.CurrentUICulture.Name, null, null, false);
                downloadUri = resultTuple.ConvertedDocumentUri;

                stream = null;
            }
            else
            {
                file.Id = default;
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

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(downloadUri)
                };

                var httpClient = _clientFactory.CreateClient();
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
               || (!file.ProviderEntry && file.CreateBy != _authContext.CurrentAccount.ID)
               || !await LinkedForMeAsync(file))
            {
                var linkDao = _daoFactory.GetLinkDao();
                await linkDao.DeleteAllLinkAsync(file.Id.ToString());
            }
        }

        await _fileMarker.MarkAsNewAsync(file);
        await _fileMarker.RemoveMarkAsNewAsync(file);

        return file;
    }

    public async Task TrackEditingAsync<T>(T fileId, Guid tabId, Guid userId, string doc, bool editingAlone = false)
    {
        bool checkRight;
        if (_fileTracker.GetEditingBy(fileId).Contains(userId))
        {
            checkRight = _fileTracker.ProlongEditing(fileId, tabId, userId, editingAlone);
            if (!checkRight)
            {
                return;
            }
        }

        var fileDao = _daoFactory.GetFileDao<T>();
        var check = await _fileShareLink.CheckAsync(doc, false, fileDao);
        var editLink = check.EditLink;
        var file = check.File;
        if (file == null)
        {
            file = await fileDao.GetFileAsync(fileId);
        }

        if (file == null)
        {
            throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
        }
        if (!editLink
            && (!await _fileSecurity.CanEditAsync(file, userId)
                && !await _fileSecurity.CanCustomFilterEditAsync(file, userId)
                && !await _fileSecurity.CanReviewAsync(file, userId)
                && !await _fileSecurity.CanFillFormsAsync(file, userId)
                && !await _fileSecurity.CanCommentAsync(file, userId)))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_EditFile);
        }
        if (await FileLockedForMeAsync(file.Id, userId))
        {
            throw new Exception(FilesCommonResource.ErrorMassage_LockedFile);
        }

        if (file.RootFolderType == FolderType.TRASH)
        {
            throw new Exception(FilesCommonResource.ErrorMassage_ViewTrashItem);
        }

        checkRight = _fileTracker.ProlongEditing(fileId, tabId, userId, editingAlone);
        if (checkRight)
        {
            _fileTracker.ChangeRight(fileId, userId, false);
        }
    }

    public async Task<File<T>> UpdateToVersionFileAsync<T>(T fileId, int version, string doc = null, bool checkRight = true, bool finalize = false)
    {
        var fileDao = _daoFactory.GetFileDao<T>();
        if (version < 1)
        {
            throw new ArgumentNullException(nameof(version));
        }

        var (editLink, fromFile, _) = await _fileShareLink.CheckAsync(doc, false, fileDao);

        if (fromFile == null)
        {
            fromFile = await fileDao.GetFileAsync(fileId);
        }

        if (fromFile == null)
        {
            throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
        }

        if (fromFile.Version != version)
        {
            fromFile = await fileDao.GetFileAsync(fromFile.Id, Math.Min(fromFile.Version, version));
        }
        else
        {
            if (!finalize)
            {
                throw new Exception(FilesCommonResource.ErrorMassage_FileUpdateToVersion);
            }
        }

        if (fromFile == null)
        {
            throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
        }

        if (checkRight && !editLink && (!await _fileSecurity.CanEditHistoryAsync(fromFile) || _userManager.IsUser(_authContext.CurrentAccount.ID)))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_EditFile);
        }

        if (await FileLockedForMeAsync(fromFile.Id))
        {
            throw new Exception(FilesCommonResource.ErrorMassage_LockedFile);
        }

        if (checkRight && _fileTracker.IsEditing(fromFile.Id))
        {
            throw new Exception(FilesCommonResource.ErrorMassage_SecurityException_UpdateEditingFile);
        }

        if (fromFile.RootFolderType == FolderType.TRASH)
        {
            throw new Exception(FilesCommonResource.ErrorMassage_ViewTrashItem);
        }

        if (fromFile.ProviderEntry)
        {
            throw new Exception(FilesCommonResource.ErrorMassage_BadRequest);
        }

        if (fromFile.Encrypted)
        {
            throw new Exception(FilesCommonResource.ErrorMassage_NotSupportedFormat);
        }

        var exists = _cache.Get<string>(_updateList + fileId.ToString()) != null;
        if (exists)
        {
            throw new Exception(FilesCommonResource.ErrorMassage_UpdateEditingFile);
        }
        else
        {
            _cache.Insert(_updateList + fileId.ToString(), fileId.ToString(), TimeSpan.FromMinutes(2));
        }

        try
        {
            var currFile = await fileDao.GetFileAsync(fileId);
            var newFile = _serviceProvider.GetService<File<T>>();

            newFile.Id = fromFile.Id;
            newFile.Version = currFile.Version + 1;
            newFile.VersionGroup = currFile.VersionGroup + 1;
            newFile.Title = FileUtility.ReplaceFileExtension(currFile.Title, FileUtility.GetFileExtension(fromFile.Title));
            newFile.FileStatus = currFile.FileStatus;
            newFile.ParentId = currFile.ParentId;
            newFile.CreateBy = currFile.CreateBy;
            newFile.CreateOn = currFile.CreateOn;
            newFile.ModifiedBy = fromFile.ModifiedBy;
            newFile.ModifiedOn = fromFile.ModifiedOn;
            newFile.ConvertedType = fromFile.ConvertedType;
            newFile.Comment = string.Format(FilesCommonResource.CommentRevert, fromFile.ModifiedOnString);
            newFile.Encrypted = fromFile.Encrypted;
            newFile.ThumbnailStatus = fromFile.ThumbnailStatus == Thumbnail.Created ? Thumbnail.Creating : Thumbnail.Waiting;

            using (var stream = await fileDao.GetFileStreamAsync(fromFile))
            {
                newFile.ContentLength = stream.CanSeek ? stream.Length : fromFile.ContentLength;
                newFile = await fileDao.SaveFileAsync(newFile, stream);
            }

            if (fromFile.ThumbnailStatus == Thumbnail.Created)
            {
                var CopyThumbnailsAsync = async () =>
                {
                    await using (var scope = _serviceProvider.CreateAsyncScope())
                    {
                        var _fileDao = scope.ServiceProvider.GetService<IDaoFactory>().GetFileDao<T>();
                        var _globalStoreLocal = scope.ServiceProvider.GetService<GlobalStore>();

                        foreach (var size in _thumbnailSettings.Sizes)
                        {
                            await _globalStoreLocal.GetStore().CopyAsync(String.Empty,
                                                                    _fileDao.GetUniqThumbnailPath(fromFile, size.Width, size.Height),
                                                                    String.Empty,
                                                                    _fileDao.GetUniqThumbnailPath(newFile, size.Width, size.Height));
                        }

                        await _fileDao.SetThumbnailStatusAsync(newFile, Thumbnail.Created);
                    }
                };

                _ = Task.Run(() => CopyThumbnailsAsync().GetAwaiter().GetResult());
            }


            var linkDao = _daoFactory.GetLinkDao();
            await linkDao.DeleteAllLinkAsync(newFile.Id.ToString());

            await _fileMarker.MarkAsNewAsync(newFile); ;

            await _entryStatusManager.SetFileStatusAsync(newFile);

            newFile.Access = fromFile.Access;

            if (newFile.IsTemplate
                && !_fileUtility.ExtsWebTemplate.Contains(FileUtility.GetFileExtension(newFile.Title), StringComparer.CurrentCultureIgnoreCase))
            {
                var tagTemplate = Tag.Template(_authContext.CurrentAccount.ID, newFile);
                var tagDao = _daoFactory.GetTagDao<T>();

                await tagDao.RemoveTags(tagTemplate);

                newFile.IsTemplate = false;
            }

            return newFile;
        }
        catch (Exception e)
        {
            _logger.ErrorUpdateFile(fileId.ToString(), version, e);

            throw new Exception(e.Message, e);
        }
        finally
        {
            _cache.Remove(_updateList + fromFile.Id);
        }
    }

    public async Task<File<T>> CompleteVersionFileAsync<T>(T fileId, int version, bool continueVersion, bool checkRight = true)
    {
        var fileDao = _daoFactory.GetFileDao<T>();
        var fileVersion = version > 0
            ? await fileDao.GetFileAsync(fileId, version)
            : await fileDao.GetFileAsync(fileId);
        if (fileVersion == null)
        {
            throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
        }

        if (checkRight && (!await _fileSecurity.CanEditHistoryAsync(fileVersion) || _userManager.IsUser(_authContext.CurrentAccount.ID)))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_EditFile);
        }

        if (await FileLockedForMeAsync(fileVersion.Id))
        {
            throw new Exception(FilesCommonResource.ErrorMassage_LockedFile);
        }

        if (fileVersion.RootFolderType == FolderType.TRASH)
        {
            throw new Exception(FilesCommonResource.ErrorMassage_ViewTrashItem);
        }

        if (fileVersion.ProviderEntry)
        {
            throw new Exception(FilesCommonResource.ErrorMassage_BadRequest);
        }

        var lastVersionFile = await fileDao.GetFileAsync(fileVersion.Id);

        if (continueVersion)
        {
            if (lastVersionFile.VersionGroup > 1)
            {
                await fileDao.ContinueVersionAsync(fileVersion.Id, fileVersion.Version);
                lastVersionFile.VersionGroup--;
            }
        }
        else
        {
            if (!_fileTracker.IsEditing(lastVersionFile.Id))
            {
                if (fileVersion.Version == lastVersionFile.Version)
                {
                    lastVersionFile = await UpdateToVersionFileAsync(fileVersion.Id, fileVersion.Version, null, checkRight, true);
                }

                await fileDao.CompleteVersionAsync(fileVersion.Id, fileVersion.Version);
                lastVersionFile.VersionGroup++;
            }
        }

        await _entryStatusManager.SetFileStatusAsync(lastVersionFile);

        return lastVersionFile;
    }

    public async Task<FileOptions<T>> FileRenameAsync<T>(T fileId, string title)
    {
        var fileDao = _daoFactory.GetFileDao<T>();
        var file = await fileDao.GetFileAsync(fileId);
        if (file == null)
        {
            throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
        }

        if (!await _fileSecurity.CanRenameAsync(file))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_RenameFile);
        }

        if (!await _fileSecurity.CanDeleteAsync(file) && _userManager.IsUser(_authContext.CurrentAccount.ID))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_RenameFile);
        }

        if (await FileLockedForMeAsync(file.Id))
        {
            throw new Exception(FilesCommonResource.ErrorMassage_LockedFile);
        }

        if (file.ProviderEntry && _fileTracker.IsEditing(file.Id))
        {
            throw new Exception(FilesCommonResource.ErrorMassage_UpdateEditingFile);
        }

        if (file.RootFolderType == FolderType.TRASH)
        {
            throw new Exception(FilesCommonResource.ErrorMassage_ViewTrashItem);
        }

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

            await _documentServiceHelper.RenameFileAsync(file, fileDao);

            renamed = true;
        }

        await _entryStatusManager.SetFileStatusAsync(file);

        return new FileOptions<T>
        {
            File = file,
            Renamed = renamed
        };
    }

    public async Task MarkAsRecent<T>(File<T> file)
    {
        if (file.Encrypted || file.ProviderEntry)
        {
            throw new NotSupportedException();
        }

        var tagDao = _daoFactory.GetTagDao<T>();
        var userID = _authContext.CurrentAccount.ID;

        var tag = Tag.Recent(userID, file);

        await tagDao.SaveTags(tag);
    }

    private async Task SetOriginsAsync(IFolder parent, IEnumerable<FileEntry> entries)
    {
        if (parent.FolderType != FolderType.TRASH || !entries.Any())
        {
            return;
        }

        var folderDao = _daoFactory.GetFolderDao<int>();

        var originsData = await folderDao.GetOriginsDataAsync(entries.Cast<FileEntry<int>>().Select(e => e.Id)).ToListAsync();

        foreach (var entry in entries)
        {
            var fileEntry = (FileEntry<int>)entry;
            var data = originsData.FirstOrDefault(data => data.Entries.Contains(new KeyValuePair<string, FileEntryType>(fileEntry.Id.ToString(), fileEntry.FileEntryType)));

            if (data?.OriginRoom != null && DocSpaceHelper.IsRoom(data.OriginRoom.FolderType))
            {
                fileEntry.OriginRoomId = data.OriginRoom.Id;
                fileEntry.OriginRoomTitle = data.OriginRoom.Title;
            }

            if (data?.OriginFolder == null)
            {
                continue;
            }

            fileEntry.OriginId = data.OriginFolder.Id;
            fileEntry.OriginTitle = data.OriginFolder.FolderType == FolderType.USER ? FilesUCResource.MyFiles : data.OriginFolder.Title;
        }
    }

    //Long operation
    public async Task DeleteSubitemsAsync<T>(T parentId, IFolderDao<T> folderDao, IFileDao<T> fileDao, ILinkDao linkDao)
    {
        var folders = folderDao.GetFoldersAsync(parentId);
        await foreach (var folder in folders)
        {
            await DeleteSubitemsAsync(folder.Id, folderDao, fileDao, linkDao);

            _logger.InformationDeleteFolder(folder.Id.ToString(), parentId.ToString());
            await folderDao.DeleteFolderAsync(folder.Id);
            await _socketManager.DeleteFolder(folder);
        }

        var files = fileDao.GetFilesAsync(parentId, null, FilterType.None, false, Guid.Empty, string.Empty, true);
        await foreach (var file in files)
        {
            _logger.InformationDeletefile(file.Id.ToString(), parentId.ToString());
            await fileDao.DeleteFileAsync(file.Id);
            await _socketManager.DeleteFile(file);

            await linkDao.DeleteAllLinkAsync(file.Id.ToString());
        }
    }

    public async Task MoveSharedItemsAsync<T>(T parentId, T toId, IFolderDao<T> folderDao, IFileDao<T> fileDao)
    {
        var folders = folderDao.GetFoldersAsync(parentId);
        await foreach (var folder in folders)
        {
            var shares = await _fileSecurity.GetSharesAsync(folder);
            var shared = folder.Shared
                         && shares.Any(record => record.Share != FileShare.Restrict);
            if (shared)
            {
                _logger.InformationMoveSharedFolder(folder.Id.ToString(), parentId.ToString(), toId.ToString());
                await folderDao.MoveFolderAsync(folder.Id, toId, null);
            }
            else
            {
                await MoveSharedItemsAsync(folder.Id, toId, folderDao, fileDao);
            }
        }

        var files = fileDao.GetFilesAsync(parentId, null, FilterType.None, false, Guid.Empty, string.Empty, true)
            .WhereAwait(async file => file.Shared &&
            (await _fileSecurity.GetSharesAsync(file)).Any(record => record.Subject != FileConstant.ShareLinkId && record.Share != FileShare.Restrict));

        await foreach (var file in files)
        {
            _logger.InformationMoveSharedFile(file.Id.ToString(), parentId.ToString(), toId.ToString());
            await fileDao.MoveFileAsync(file.Id, toId);
        }
    }

    public static async Task ReassignItemsAsync<T>(T parentId, Guid fromUserId, Guid toUserId, IFolderDao<T> folderDao, IFileDao<T> fileDao)
    {
        var files = await fileDao.GetFilesAsync(parentId, new OrderBy(SortedByType.AZ, true), FilterType.ByUser, false, fromUserId, null, true, default, true).ToListAsync();
        var fileIds = files.Where(file => file.CreateBy == fromUserId).Select(file => file.Id);

        await fileDao.ReassignFilesAsync(fileIds.ToArray(), toUserId);

        var folderIds = await folderDao.GetFoldersAsync(parentId, new OrderBy(SortedByType.AZ, true), FilterType.ByUser, false, fromUserId, null, default, true)
                                 .Where(folder => folder.CreateBy == fromUserId).Select(folder => folder.Id)
                                 .ToListAsync();

        await folderDao.ReassignFoldersAsync(folderIds.ToArray(), toUserId);
    }
}
