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

[Singletone]
public class FileMarkerHelper
{
    public const string CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME = "file_marker";
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;
    public DistributedTaskQueue Tasks { get; set; }

    public FileMarkerHelper(
        IServiceProvider serviceProvider,
        ILogger<FileMarkerHelper> logger,
        IDistributedTaskQueueFactory queueFactory)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        Tasks = queueFactory.CreateQueue(CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME);
    }

    internal void Add<T>(AsyncTaskData<T> taskData)
    {
        Tasks.EnqueueTask(async (d, c) => await ExecMarkFileAsNewAsync(taskData), taskData);
    }

    private async Task ExecMarkFileAsNewAsync<T>(AsyncTaskData<T> obj)
    {
        try
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var fileMarker = scope.ServiceProvider.GetService<FileMarker>();
            var socketManager = scope.ServiceProvider.GetService<SocketManager>();
            await fileMarker.ExecMarkFileAsNewAsync(obj, socketManager);
        }
        catch (Exception e)
        {
            _logger.ErrorExecMarkFileAsNew(e);
        }
    }
}

[Scope(Additional = typeof(FileMarkerExtention))]
public class FileMarker
{
    private readonly ICache _cache;

    private const string CacheKeyFormat = "MarkedAsNew/{0}/folder_{1}";

    private readonly TenantManager _tenantManager;
    private readonly UserManager _userManager;
    private readonly IDaoFactory _daoFactory;
    private readonly GlobalFolder _globalFolder;
    private readonly FileSecurity _fileSecurity;
    private readonly AuthContext _authContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly FilesSettingsHelper _filesSettingsHelper;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
    private readonly RoomsNotificationSettingsHelper _roomsNotificationSettingsHelper;

    public FileMarker(
        TenantManager tenantManager,
        UserManager userManager,
        IDaoFactory daoFactory,
        GlobalFolder globalFolder,
        FileSecurity fileSecurity,
        AuthContext authContext,
        IServiceProvider serviceProvider,
        FilesSettingsHelper filesSettingsHelper,
        RoomsNotificationSettingsHelper roomsNotificationSettingsHelper,
        ICache cache)
    {
        _tenantManager = tenantManager;
        _userManager = userManager;
        _daoFactory = daoFactory;
        _globalFolder = globalFolder;
        _fileSecurity = fileSecurity;
        _authContext = authContext;
        _serviceProvider = serviceProvider;
        _filesSettingsHelper = filesSettingsHelper;
        _roomsNotificationSettingsHelper = roomsNotificationSettingsHelper;
        _cache = cache;
    }

    internal async Task ExecMarkFileAsNewAsync<T>(AsyncTaskData<T> obj, SocketManager socketManager)
    {
        await _tenantManager.SetCurrentTenantAsync(obj.TenantID);

        var folderDao = _daoFactory.GetFolderDao<T>();
        T parentFolderId;

        if (obj.FileEntry.FileEntryType == FileEntryType.File)
        {
            parentFolderId = ((File<T>)obj.FileEntry).ParentId;
        }
        else
        {
            parentFolderId = ((Folder<T>)obj.FileEntry).Id;
        }

        var parentFolders = await folderDao.GetParentFoldersAsync(parentFolderId).Reverse().ToListAsync();

        var userIDs = obj.UserIDs;

        var userEntriesData = new Dictionary<Guid, List<FileEntry>>();

        if (obj.FileEntry.RootFolderType == FolderType.BUNCH)
        {
            if (userIDs.Count == 0)
            {
                return;
            }

            var projectsFolder = await _globalFolder.GetFolderProjectsAsync<T>(_daoFactory);
            parentFolders.Add(await folderDao.GetFolderAsync(projectsFolder));

            var entries = new List<FileEntry> { obj.FileEntry };
            entries = entries.Concat(parentFolders).ToList();

            userIDs.ForEach(userID =>
            {
                if (userEntriesData.TryGetValue(userID, out var value))
                {
                    value.AddRange(entries);
                }
                else
                {
                    userEntriesData.Add(userID, entries);
                }

                RemoveFromCahce(projectsFolder, userID);
            });
        }
        else
        {
            var filesSecurity = _fileSecurity;

            if (userIDs.Count == 0)
            {
                var guids = await filesSecurity.WhoCanReadAsync(obj.FileEntry);
                userIDs = guids.Where(x => x != obj.CurrentAccountId).ToList();
            }
            if (obj.FileEntry.ProviderEntry)
            {
                userIDs = await userIDs.ToAsyncEnumerable().WhereAwait(async u => !await _userManager.IsUserAsync(u)).ToListAsync();

                if (obj.FileEntry.RootFolderType == FolderType.VirtualRooms)
                {
                    var parents = new List<Folder<T>>();

                    foreach (var folder in parentFolders)
                    {
                        if (DocSpaceHelper.IsRoom(folder.FolderType))
                        {
                            parents.Add(folder);
                            break;
                        }

                        parents.Add(folder);
                    }

                    parentFolders = parents;
                }
            }

            foreach (var parentFolder in parentFolders)
            {
                var whoCanRead = await filesSecurity.WhoCanReadAsync(parentFolder);
                var ids = whoCanRead
                    .Where(userID => userIDs.Contains(userID) && userID != obj.CurrentAccountId);
                foreach (var id in ids)
                {
                    if (userEntriesData.TryGetValue(id, out var value))
                    {
                        value.Add(parentFolder);
                    }
                    else
                    {
                        userEntriesData.Add(id, new List<FileEntry> { parentFolder });
                    }
                }
            }


            if (obj.FileEntry.RootFolderType == FolderType.USER)
            {
                var folderDaoInt = _daoFactory.GetFolderDao<int>();
                var folderShare = await folderDaoInt.GetFolderAsync(await _globalFolder.GetFolderShareAsync(_daoFactory));

                foreach (var userID in userIDs)
                {
                    var userFolderId = await folderDaoInt.GetFolderIDUserAsync(false, userID);
                    if (Equals(userFolderId, 0))
                    {
                        continue;
                    }

                    Folder<int> rootFolder = null;
                    if (obj.FileEntry.ProviderEntry)
                    {
                        rootFolder = obj.FileEntry.RootCreateBy == userID
                                         ? await folderDaoInt.GetFolderAsync(userFolderId)
                                         : folderShare;
                    }
                    else if (!Equals(obj.FileEntry.RootId, userFolderId))
                    {
                        rootFolder = folderShare;
                    }
                    else
                    {
                        RemoveFromCahce(userFolderId, userID);
                    }

                    if (rootFolder == null)
                    {
                        continue;
                    }

                    if (userEntriesData.TryGetValue(userID, out var value))
                    {
                        value.Add(rootFolder);
                    }
                    else
                    {
                        userEntriesData.Add(userID, new List<FileEntry> { rootFolder });
                    }

                    RemoveFromCahce(rootFolder.Id, userID);
                }
            }
            else if (obj.FileEntry.RootFolderType == FolderType.COMMON)
            {
                var commonFolderId = await _globalFolder.GetFolderCommonAsync(this, _daoFactory);
                userIDs.ForEach(userID => RemoveFromCahce(commonFolderId, userID));

                if (obj.FileEntry.ProviderEntry)
                {
                    var commonFolder = await folderDao.GetFolderAsync(await _globalFolder.GetFolderCommonAsync<T>(this, _daoFactory));
                    userIDs.ForEach(userID =>
                    {
                        if (userEntriesData.TryGetValue(userID, out var value))
                        {
                            value.Add(commonFolder);
                        }
                        else
                        {
                            userEntriesData.Add(userID, new List<FileEntry> { commonFolder });
                        }

                        RemoveFromCahce(commonFolderId, userID);
                    });
                }
            }
            else if (obj.FileEntry.RootFolderType == FolderType.VirtualRooms)
            {
                var virtualRoomsFolderId = await _globalFolder.GetFolderVirtualRoomsAsync(_daoFactory);
                userIDs.ForEach(userID => RemoveFromCahce(virtualRoomsFolderId, userID));

                var room = parentFolders.Where(f => DocSpaceHelper.IsRoom(f.FolderType)).FirstOrDefault();

                if (room.CreateBy != obj.CurrentAccountId)
                {
                    var roomOwnerEntries = parentFolders.Cast<FileEntry>().Concat(new[] { obj.FileEntry }).ToList();
                    userEntriesData.Add(room.CreateBy, roomOwnerEntries);

                    RemoveFromCahce(virtualRoomsFolderId, room.CreateBy);
                }

                if (obj.FileEntry.ProviderEntry)
                {
                    var virtualRoomsFolder = await _daoFactory.GetFolderDao<int>().GetFolderAsync(virtualRoomsFolderId);

                    userIDs.ForEach(userID =>
                    {
                        if (userEntriesData.TryGetValue(userID, out var value))
                        {
                            value.Add(virtualRoomsFolder);
                        }
                        else
                        {
                            userEntriesData.Add(userID, new List<FileEntry> { virtualRoomsFolder });
                        }

                        RemoveFromCahce(virtualRoomsFolderId, userID);
                    });
                }
            }
            else if (obj.FileEntry.RootFolderType == FolderType.Privacy)
            {
                foreach (var userID in userIDs)
                {
                    var privacyFolderId = await folderDao.GetFolderIDPrivacyAsync(false, userID);
                    if (Equals(privacyFolderId, 0))
                    {
                        continue;
                    }

                    var rootFolder = await folderDao.GetFolderAsync(privacyFolderId);
                    if (rootFolder == null)
                    {
                        continue;
                    }

                    if (userEntriesData.TryGetValue(userID, out var value))
                    {
                        value.Add(rootFolder);
                    }
                    else
                    {
                        userEntriesData.Add(userID, new List<FileEntry> { rootFolder });
                    }

                    RemoveFromCahce(rootFolder.Id, userID);
                }
            }

            userIDs.ForEach(userID =>
            {
                if (userEntriesData.TryGetValue(userID, out var value))
                {
                    value.Add(obj.FileEntry);
                }
                else
                {
                    userEntriesData.Add(userID, new List<FileEntry> { obj.FileEntry });
                }
            });
        }

        var tagDao = _daoFactory.GetTagDao<T>();
        var newTags = new List<Tag>();
        var updateTags = new List<Tag>();

        try
        {
            await _semaphore.WaitAsync();

        foreach (var userID in userEntriesData.Keys)
        {
            if (await tagDao.GetNewTagsAsync(userID, obj.FileEntry).AnyAsync())
            {
                continue;
            }

            var entries = userEntriesData[userID].Distinct().ToList();

            await GetNewTagsAsync(userID, entries.OfType<FileEntry<int>>().ToList());
            await GetNewTagsAsync(userID, entries.OfType<FileEntry<string>>().ToList());
        }

        if (updateTags.Count > 0)
        {
            await tagDao.UpdateNewTags(updateTags, obj.CurrentAccountId);
        }

        if (newTags.Count > 0)
        {
            await tagDao.SaveTags(newTags, obj.CurrentAccountId);
        }
        }
        catch
        {
            throw;
        }
        finally
        {
            _semaphore.Release();
        }

        await Task.WhenAll(ExecMarkAsNewRequest(updateTags.Concat(newTags), socketManager));

        async Task GetNewTagsAsync<T1>(Guid userID, List<FileEntry<T1>> entries)
        {
            var tagDao1 = _daoFactory.GetTagDao<T1>();
            var exist = await tagDao1.GetNewTagsAsync(userID, entries).ToListAsync();
            var update = exist.Where(t => t.EntryType == FileEntryType.Folder).ToList();
            update.ForEach(t => t.Count++);
            updateTags.AddRange(update);

            entries.ForEach(entry =>
            {
                if (entry != null && exist.All(tag => tag != null && !tag.EntryId.Equals(entry.Id)))
                {
                    newTags.Add(Tag.New(userID, entry));
                }
            });
        }
    }

    public async ValueTask MarkAsNewAsync<T>(FileEntry<T> fileEntry, List<Guid> userIDs = null)
    {
        if (fileEntry == null)
        {
            return;
        }

        userIDs ??= new List<Guid>();

        var taskData = _serviceProvider.GetService<AsyncTaskData<T>>();
        taskData.FileEntry = (FileEntry<T>)fileEntry.Clone();
        taskData.UserIDs = userIDs;

        if (fileEntry.RootFolderType == FolderType.BUNCH && userIDs.Count == 0)
        {
            var folderDao = _daoFactory.GetFolderDao<T>();
            var path = await folderDao.GetBunchObjectIDAsync(fileEntry.RootId);

            var projectID = path.Split('/').Last();
            if (string.IsNullOrEmpty(projectID))
            {
                return;
            }

            var whoCanRead = await _fileSecurity.WhoCanReadAsync(fileEntry);
            var projectTeam = whoCanRead.Where(x => x != _authContext.CurrentAccount.ID).ToList();

            if (projectTeam.Count == 0)
            {
                return;
            }

            taskData.UserIDs = projectTeam;
        }

        _serviceProvider.GetService<FileMarkerHelper>().Add(taskData);
    }

    public async ValueTask RemoveMarkAsNewAsync<T>(FileEntry<T> fileEntry, Guid userID = default)
    {
        if (fileEntry == null)
        {
            return;
        }

        userID = userID.Equals(default) ? _authContext.CurrentAccount.ID : userID;

        var tagDao = _daoFactory.GetTagDao<T>();
        var internalFolderDao = _daoFactory.GetFolderDao<int>();
        var folderDao = _daoFactory.GetFolderDao<T>();

        if (!await tagDao.GetNewTagsAsync(userID, fileEntry).AnyAsync())
        {
            return;
        }

        T folderID;
        int valueNew;
        var userFolderId = await internalFolderDao.GetFolderIDUserAsync(false, userID);
        var privacyFolderId = await internalFolderDao.GetFolderIDPrivacyAsync(false, userID);

        var removeTags = new List<Tag>();

        if (fileEntry.FileEntryType == FileEntryType.File)
        {
            folderID = ((File<T>)fileEntry).ParentId;

            removeTags.Add(Tag.New(userID, fileEntry));
            valueNew = 1;
        }
        else
        {
            folderID = fileEntry.Id;

            var listTags = await tagDao.GetNewTagsAsync(userID, (Folder<T>)fileEntry, true).ToListAsync();
            valueNew = listTags.FirstOrDefault(tag => tag.EntryId.Equals(fileEntry.Id)).Count;

            if (Equals(fileEntry.Id, userFolderId) || Equals(fileEntry.Id, await _globalFolder.GetFolderCommonAsync(this, _daoFactory)) || Equals(fileEntry.Id, await _globalFolder.GetFolderShareAsync(_daoFactory)))
            {
                var folderTags = listTags.Where(tag => tag.EntryType == FileEntryType.Folder);

                foreach (var tag in folderTags)
                {
                    var folderEntry = await folderDao.GetFolderAsync((T)tag.EntryId);
                    if (folderEntry != null && folderEntry.ProviderEntry)
                    {
                        listTags.Remove(tag);
                        listTags.AddRange(await tagDao.GetNewTagsAsync(userID, folderEntry, true).ToListAsync());
                    }
                }
            }

            removeTags.AddRange(listTags);
        }

        var parentFolders = await folderDao.GetParentFoldersAsync(folderID).Reverse().ToListAsync();

        var rootFolder = parentFolders.LastOrDefault();
        int rootFolderId = default;
        int cacheFolderId = default;
        if (rootFolder == null)
        {
        }
        else if (rootFolder.RootFolderType == FolderType.VirtualRooms)
        {
            cacheFolderId = rootFolderId = await _globalFolder.GetFolderVirtualRoomsAsync(_daoFactory);
        }
        else if (rootFolder.RootFolderType == FolderType.BUNCH)
        {
            cacheFolderId = rootFolderId = await _globalFolder.GetFolderProjectsAsync(_daoFactory);
        }
        else if (rootFolder.RootFolderType == FolderType.COMMON)
        {
            if (rootFolder.ProviderEntry)
            {
                cacheFolderId = rootFolderId = await _globalFolder.GetFolderCommonAsync(this, _daoFactory);
            }
            else
            {
                cacheFolderId = await _globalFolder.GetFolderCommonAsync(this, _daoFactory);
            }
        }
        else if (rootFolder.RootFolderType == FolderType.USER)
        {
            if (rootFolder.ProviderEntry && rootFolder.RootCreateBy == userID)
            {
                cacheFolderId = rootFolderId = userFolderId;
            }
            else if (!rootFolder.ProviderEntry && !Equals(rootFolder.RootId, userFolderId)
                     || rootFolder.ProviderEntry && rootFolder.RootCreateBy != userID)
            {
                cacheFolderId = rootFolderId = await _globalFolder.GetFolderShareAsync(_daoFactory);
            }
            else
            {
                cacheFolderId = userFolderId;
            }
        }
        else if (rootFolder.RootFolderType == FolderType.Privacy)
        {
            if (!Equals(privacyFolderId, 0))
            {
                cacheFolderId = rootFolderId = privacyFolderId;
            }
        }
        else if (rootFolder.RootFolderType == FolderType.SHARE)
        {
            cacheFolderId = await _globalFolder.GetFolderShareAsync(_daoFactory);
        }

        var updateTags = new List<Tag>();

        if (!rootFolderId.Equals(default))
        {
            await UpdateRemoveTags(await internalFolderDao.GetFolderAsync(rootFolderId));
        }

        if (!cacheFolderId.Equals(default))
        {
            RemoveFromCahce(cacheFolderId, userID);
        }

        foreach (var parentFolder in parentFolders)
        {
            await UpdateRemoveTags(parentFolder);
        }

        if (updateTags.Count > 0)
        {
            await tagDao.UpdateNewTags(updateTags);
        }

        if (removeTags.Count > 0)
        {
            await tagDao.RemoveTags(removeTags);
        }

        var socketManager = _serviceProvider.GetRequiredService<SocketManager>();

        var toRemove = removeTags.Select(r => new Tag(r.Name, r.Type, r.Owner, 0)
        {
            EntryId = r.EntryId,
            EntryType = r.EntryType
        });

        await Task.WhenAll(ExecMarkAsNewRequest(updateTags.Concat(toRemove), socketManager));

        async Task UpdateRemoveTags<TFolder>(Folder<TFolder> folder)
        {
            var tagDao = _daoFactory.GetTagDao<TFolder>();
            var newTags = tagDao.GetNewTagsAsync(userID, folder);
            var parentTag = await newTags.FirstOrDefaultAsync();

            if (parentTag != null)
            {
                parentTag.Count -= valueNew;

                if (parentTag.Count > 0)
                {
                    updateTags.Add(parentTag);
                }
                else
                {
                    removeTags.Add(parentTag);
                }
            }
        }
    }

    public async Task RemoveMarkAsNewForAllAsync<T>(FileEntry<T> fileEntry)
    {
        IAsyncEnumerable<Guid> userIDs;

        var tagDao = _daoFactory.GetTagDao<T>();
        var tags = tagDao.GetTagsAsync(fileEntry.Id, fileEntry.FileEntryType == FileEntryType.File ? FileEntryType.File : FileEntryType.Folder, TagType.New);
        userIDs = tags.Select(tag => tag.Owner).Distinct();

        await foreach (var userID in userIDs)
        {
            await RemoveMarkAsNewAsync(fileEntry, userID);
        }
    }


    public async Task<int> GetRootFoldersIdMarkedAsNewAsync<T>(T rootId)
    {
        var fromCache = GetCountFromCahce(rootId);
        if (fromCache == -1)
        {
            var tagDao = _daoFactory.GetTagDao<T>();
            var folderDao = _daoFactory.GetFolderDao<T>();
            var requestTags = tagDao.GetNewTagsAsync(_authContext.CurrentAccount.ID, await folderDao.GetFolderAsync(rootId));
            var requestTag = await requestTags.FirstOrDefaultAsync(tag => tag.EntryId.Equals(rootId));
            var count = requestTag == null ? 0 : requestTag.Count;
            InsertToCahce(rootId, count);

            return count;
        }
        else if (fromCache > 0)
        {
            return fromCache;
        }

        return 0;
    }

    public IAsyncEnumerable<FileEntry> MarkedItemsAsync<T>(Folder<T> folder)
    {
        if (folder == null)
        {
            throw new ArgumentNullException(nameof(folder), FilesCommonResource.ErrorMassage_FolderNotFound);
        }

        return InternalMarkedItemsAsync(folder);
    }

    private async IAsyncEnumerable<FileEntry> InternalMarkedItemsAsync<T>(Folder<T> folder)
    {
        if (!await _fileSecurity.CanReadAsync(folder))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_ViewFolder);
        }

        if (folder.RootFolderType == FolderType.TRASH && !Equals(folder.Id, await _globalFolder.GetFolderTrashAsync(_daoFactory)))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_ViewTrashItem);
        }

        var tagDao = _daoFactory.GetTagDao<T>();
        var providerFolderDao = _daoFactory.GetFolderDao<string>();
        var providerTagDao = _daoFactory.GetTagDao<string>();
        var tags = await (tagDao.GetNewTagsAsync(_authContext.CurrentAccount.ID, folder, true) ?? AsyncEnumerable.Empty<Tag>()).ToListAsync();

        if (!tags.Any())
        {
            yield break;
        }

        if (Equals(folder.Id, await _globalFolder.GetFolderMyAsync(this, _daoFactory)) ||
            Equals(folder.Id, await _globalFolder.GetFolderCommonAsync(this, _daoFactory)) ||
            Equals(folder.Id, await _globalFolder.GetFolderShareAsync(_daoFactory)) ||
            Equals(folder.Id, await _globalFolder.GetFolderVirtualRoomsAsync(_daoFactory)))
        {
            var folderTags = tags.Where(tag => tag.EntryType == FileEntryType.Folder && (tag.EntryId is string));

            var providerFolderTags = new List<KeyValuePair<Tag, Folder<string>>>();

            foreach (var tag in folderTags)
            {
                var pair = new KeyValuePair<Tag, Folder<string>>(tag, await providerFolderDao.GetFolderAsync(tag.EntryId.ToString()));
                if (pair.Value != null && pair.Value.ProviderEntry)
                {
                    providerFolderTags.Add(pair);
                }
            }

            providerFolderTags.Reverse();

            foreach (var providerFolderTag in providerFolderTags)
            {
                tags.AddRange(await providerTagDao.GetNewTagsAsync(_authContext.CurrentAccount.ID, providerFolderTag.Value, true).ToListAsync());
            }
        }

        tags = tags
            .Where(r => !Equals(r.EntryId, folder.Id))
                .Distinct()
                .ToList();

        //TODO: refactoring
        var entryTagsProvider = await GetEntryTagsAsync<string>(tags.Where(r => r.EntryId is string).ToAsyncEnumerable());
        var entryTagsInternal = await GetEntryTagsAsync<int>(tags.Where(r => r.EntryId is int).ToAsyncEnumerable());

        foreach (var entryTag in entryTagsInternal)
        {
            var parentEntry = entryTagsInternal.Keys
                .FirstOrDefault(entryCountTag => Equals(entryCountTag.Id, entryTag.Key.ParentId));

            if (parentEntry != null)
            {
                entryTagsInternal[parentEntry].Count -= entryTag.Value.Count;
            }
        }

        foreach (var entryTag in entryTagsProvider)
        {
            if (int.TryParse(entryTag.Key.ParentId, out var fId))
            {
                var parentEntryInt = entryTagsInternal.Keys
                        .FirstOrDefault(entryCountTag => Equals(entryCountTag.Id, fId));

                if (parentEntryInt != null)
                {
                    entryTagsInternal[parentEntryInt].Count -= entryTag.Value.Count;
                }

                continue;
            }

            var parentEntry = entryTagsProvider.Keys
                .FirstOrDefault(entryCountTag => Equals(entryCountTag.Id, entryTag.Key.ParentId));

            if (parentEntry != null)
            {
                entryTagsProvider[parentEntry].Count -= entryTag.Value.Count;
            }
        }

        await foreach (var r in GetResultAsync(entryTagsInternal))
        {
            yield return r;
        }

        await foreach (var r in GetResultAsync(entryTagsProvider))
        {
            yield return r;
        }

        async IAsyncEnumerable<FileEntry> GetResultAsync<TEntry>(Dictionary<FileEntry<TEntry>, Tag> entryTags)
        {
            foreach (var entryTag in entryTags)
            {
                if (!string.IsNullOrEmpty(entryTag.Key.Error))
                {
                    await RemoveMarkAsNewAsync(entryTag.Key);
                    continue;
                }

                if (entryTag.Value.Count > 0)
                {
                    yield return entryTag.Key;
                }
            }
        }
    }

    private async Task<Dictionary<FileEntry<T>, Tag>> GetEntryTagsAsync<T>(IAsyncEnumerable<Tag> tags)
    {
        var fileDao = _daoFactory.GetFileDao<T>();
        var folderDao = _daoFactory.GetFolderDao<T>();
        var entryTags = new Dictionary<FileEntry<T>, Tag>();

        await foreach (var tag in tags)
        {
            var entry = tag.EntryType == FileEntryType.File
                            ? await fileDao.GetFileAsync((T)tag.EntryId)
                            : (FileEntry<T>)await folderDao.GetFolderAsync((T)tag.EntryId);
            if (entry != null && (!entry.ProviderEntry || _filesSettingsHelper.EnableThirdParty))
            {
                entryTags.Add(entry, tag);
            }
            else
            {
                //todo: RemoveMarkAsNew(tag);
            }
        }

        return entryTags;
    }

    public async Task SetTagsNewAsync<T>(Folder<T> parent, IEnumerable<FileEntry> entries)
    {
        var tagDao = _daoFactory.GetTagDao<T>();
        var folderDao = _daoFactory.GetFolderDao<T>();
        var totalTags = await tagDao.GetNewTagsAsync(_authContext.CurrentAccount.ID, parent, false).ToListAsync();

        if (totalTags.Count <= 0)
        {
            return;
        }

        var shareFolder = await _globalFolder.GetFolderShareAsync<T>(_daoFactory);
        var parentFolderTag = Equals(shareFolder, parent.Id)
                                    ? await tagDao.GetNewTagsAsync(_authContext.CurrentAccount.ID, await folderDao.GetFolderAsync(shareFolder)).FirstOrDefaultAsync()
                                    : totalTags.FirstOrDefault(tag => tag.EntryType == FileEntryType.Folder && Equals(tag.EntryId, parent.Id));

        totalTags = totalTags.Where(e => e != parentFolderTag).ToList();

        foreach (var e in entries)
        {
            if (e is FileEntry<int> entry)
            {
                SetTagNewForEntry(entry);
            }
            else if (e is FileEntry<string> thirdPartyEntry)
            {
                SetTagNewForEntry(thirdPartyEntry);
            }
        }

        if (parent.FolderType == FolderType.VirtualRooms)
        {
            var disabledRooms = _roomsNotificationSettingsHelper.GetDisabledRoomsForCurrentUser();
            totalTags = totalTags.Where(e => !disabledRooms.Contains(e.EntryId.ToString())).ToList();
        }

        var countSubNew = 0;
        totalTags.ForEach(tag =>
        {
            countSubNew += tag.Count;
        });

        if (parentFolderTag == null)
        {
            parentFolderTag = Tag.New(_authContext.CurrentAccount.ID, parent, 0);
            parentFolderTag.Id = -1;
        }
        else
        {
            ((IFolder)parent).NewForMe = parentFolderTag.Count;
        }

        if (parent.FolderType != FolderType.VirtualRooms && parent.RootFolderType == FolderType.VirtualRooms && parent.ProviderEntry)
        {
            countSubNew = parentFolderTag.Count;
        }

        if (parentFolderTag.Count != countSubNew)
        {
            if(parent.FolderType == FolderType.VirtualRooms)
            {
                parentFolderTag.Count = countSubNew;
                if (parentFolderTag.Id == -1)
                {
                    await tagDao.SaveTagsAsync(parentFolderTag);
                }
                else
                {
                    await tagDao.UpdateNewTags(parentFolderTag);
                }

                var cacheFolderId = parent.Id;
                if (cacheFolderId != null)
                {
                    RemoveFromCahce(cacheFolderId);
                }
            }
            else if (countSubNew > 0)
            {
                var diff = parentFolderTag.Count - countSubNew;

                parentFolderTag.Count -= diff;
                if (parentFolderTag.Id == -1)
                {
                    await tagDao.SaveTagsAsync(parentFolderTag);
                }
                else
                {
                    await tagDao.UpdateNewTags(parentFolderTag);
                }

                var cacheFolderId = parent.Id;
                var parentsList = await _daoFactory.GetFolderDao<T>().GetParentFoldersAsync(parent.Id).Reverse().ToListAsync();
                parentsList.Remove(parent);

                if (parentsList.Count > 0)
                {
                    var rootFolder = parentsList.Last();
                    T rootFolderId = default;
                    cacheFolderId = rootFolder.Id;
                    if (rootFolder.RootFolderType == FolderType.BUNCH)
                    {
                        cacheFolderId = rootFolderId = await _globalFolder.GetFolderProjectsAsync<T>(_daoFactory);
                    }
                    else if (rootFolder.RootFolderType == FolderType.USER && !Equals(rootFolder.RootId, await _globalFolder.GetFolderMyAsync(this, _daoFactory)))
                    {
                        cacheFolderId = rootFolderId = shareFolder;
                    }
                    else if (rootFolder.RootFolderType == FolderType.VirtualRooms)
                    {
                        rootFolderId = IdConverter.Convert<T>(await _globalFolder.GetFolderVirtualRoomsAsync(_daoFactory));
                    }

                    if (rootFolderId != null)
                    {
                        parentsList.Add(await _daoFactory.GetFolderDao<T>().GetFolderAsync(rootFolderId));
                    }

                    foreach (var folderFromList in parentsList)
                    {
                        var parentTreeTag = await tagDao.GetNewTagsAsync(_authContext.CurrentAccount.ID, folderFromList).FirstOrDefaultAsync();

                        if (parentTreeTag == null)
                        {
                            if (await _fileSecurity.CanReadAsync(folderFromList))
                            {
                                await tagDao.SaveTagsAsync(Tag.New(_authContext.CurrentAccount.ID, folderFromList, -diff));
                            }
                        }
                        else
                        {
                            parentTreeTag.Count -= diff;
                            await tagDao.UpdateNewTags(parentTreeTag);
                        }
                    }
                }

                if (cacheFolderId != null)
                {
                    RemoveFromCahce(cacheFolderId);
                }
            }
            else
            {
                await RemoveMarkAsNewAsync(parent);
            }
        }

        void SetTagNewForEntry<TEntry>(FileEntry<TEntry> entry)
        {
            var curTag = totalTags.FirstOrDefault(tag => tag.EntryType == entry.FileEntryType && tag.EntryId.Equals(entry.Id));

            if (curTag != null)
            {
                if (entry.FileEntryType == FileEntryType.Folder)
                {
                    ((IFolder)entry).NewForMe = curTag.Count;
                }
                else
                {
                    entry.IsNew = true;
                }
            }
        }
    }

    private void InsertToCahce(object folderId, int count)
    {
        var key = string.Format(CacheKeyFormat, _authContext.CurrentAccount.ID, folderId);
        _cache.Insert(key, count.ToString(), TimeSpan.FromMinutes(10));
    }

    private int GetCountFromCahce(object folderId)
    {
        var key = string.Format(CacheKeyFormat, _authContext.CurrentAccount.ID, folderId);
        var count = _cache.Get<string>(key);

        return count == null ? -1 : int.Parse(count);
    }

    private void RemoveFromCahce(object folderId)
    {
        RemoveFromCahce(folderId, _authContext.CurrentAccount.ID);
    }

    private void RemoveFromCahce(object folderId, Guid userId)
    {
        var key = string.Format(CacheKeyFormat, userId, folderId);
        _cache.Remove(key);
    }

    private IEnumerable<Task> ExecMarkAsNewRequest(IEnumerable<Tag> tags, SocketManager socketManager)
    {
        foreach (var t in tags)
        {
            if (t.EntryType == FileEntryType.File)
            {
                yield return socketManager.ExecMarkAsNewFileAsync(t.EntryId, t.Count, t.Owner);
            }
            else if (t.EntryType == FileEntryType.Folder)
            {
                yield return socketManager.ExecMarkAsNewFolderAsync(t.EntryId, t.Count, t.Owner);
            }
        }
    }
}

[Transient]
public class AsyncTaskData<T> : DistributedTask
{
    public AsyncTaskData(TenantManager tenantManager, AuthContext authContext) : base()
    {
        TenantID = tenantManager.GetCurrentTenant().Id;
        CurrentAccountId = authContext.CurrentAccount.ID;
    }

    public int TenantID { get; private set; }
    public FileEntry<T> FileEntry { get; set; }
    public List<Guid> UserIDs { get; set; }
    public Guid CurrentAccountId { get; set; }
}

public static class FileMarkerExtention
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<AsyncTaskData<int>>();
        services.TryAdd<FileMarkerHelper>();

        services.TryAdd<AsyncTaskData<string>>();
    }
}
