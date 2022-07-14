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
public class FileMarkerHelper<T>
{
    public const string CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME = "file_marker";
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;
    public DistributedTaskQueue Tasks { get; set; }

    public FileMarkerHelper(
        IServiceProvider serviceProvider,
        ILogger<FileMarkerHelper<T>> logger,
        IDistributedTaskQueueFactory queueFactory)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        Tasks = queueFactory.CreateQueue(CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME);
    }

    internal void Add(AsyncTaskData<T> taskData)
    {
        Tasks.EnqueueTask(async (d, c) => await ExecMarkFileAsNewAsync(taskData), taskData);
    }

    private async Task ExecMarkFileAsNewAsync(AsyncTaskData<T> obj)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var fileMarker = scope.ServiceProvider.GetService<FileMarker>();
            await fileMarker.ExecMarkFileAsNewAsync(obj);
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

    private const string _cacheKeyFormat = "MarkedAsNew/{0}/folder_{1}";

    private readonly TenantManager _tenantManager;
    private readonly UserManager _userManager;
    private readonly IDaoFactory _daoFactory;
    private readonly GlobalFolder _globalFolder;
    private readonly FileSecurity _fileSecurity;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly AuthContext _authContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly FilesSettingsHelper _filesSettingsHelper;

    public FileMarker(
        TenantManager tenantManager,
        UserManager userManager,
        IDaoFactory daoFactory,
        GlobalFolder globalFolder,
        FileSecurity fileSecurity,
        CoreBaseSettings coreBaseSettings,
        AuthContext authContext,
        IServiceProvider serviceProvider,
        FilesSettingsHelper filesSettingsHelper,
        ICache cache)
    {
        _tenantManager = tenantManager;
        _userManager = userManager;
        _daoFactory = daoFactory;
        _globalFolder = globalFolder;
        _fileSecurity = fileSecurity;
        _coreBaseSettings = coreBaseSettings;
        _authContext = authContext;
        _serviceProvider = serviceProvider;
        _filesSettingsHelper = filesSettingsHelper;
        this._cache = cache;
    }

    internal async Task ExecMarkFileAsNewAsync<T>(AsyncTaskData<T> obj)
    {
        _tenantManager.SetCurrentTenant(obj.TenantID);

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

        var parentFolders = await folderDao.GetParentFoldersAsync(parentFolderId);
        parentFolders.Reverse();

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
                userIDs = userIDs.Where(u => !_userManager.GetUsers(u).IsVisitor(_userManager)).ToList();
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
            tagDao.UpdateNewTags(updateTags);
        }

        if (newTags.Count > 0)
        {
            tagDao.SaveTags(newTags);
        }

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

    public Task MarkAsNewAsync<T>(FileEntry<T> fileEntry, List<Guid> userIDs = null)
    {
        if (fileEntry == null)
        {
            return Task.CompletedTask;
        }

        return InternalMarkAsNewAsync(fileEntry, userIDs);
    }

    private async Task InternalMarkAsNewAsync<T>(FileEntry<T> fileEntry, List<Guid> userIDs = null)
    {
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

        _serviceProvider.GetService<FileMarkerHelper<T>>().Add(taskData);
    }

    public Task RemoveMarkAsNewAsync<T>(FileEntry<T> fileEntry, Guid userID = default)
    {
        if (fileEntry == null)
        {
            return Task.CompletedTask;
        }

        return InternalRemoveMarkAsNewAsync(fileEntry, userID);
    }

    public async Task InternalRemoveMarkAsNewAsync<T>(FileEntry<T> fileEntry, Guid userID = default)
    {
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

        var parentFolders = await folderDao.GetParentFoldersAsync(folderID);
        parentFolders.Reverse();

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
            tagDao.UpdateNewTags(updateTags);
        }

        if (removeTags.Count > 0)
        {
            tagDao.RemoveTags(removeTags);
        }

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

    public Task<List<FileEntry>> MarkedItemsAsync<T>(Folder<T> folder)
    {
        if (folder == null)
        {
            throw new ArgumentNullException(nameof(folder), FilesCommonResource.ErrorMassage_FolderNotFound);
        }

        return InternalMarkedItemsAsync(folder);
    }

    private async Task<List<FileEntry>> InternalMarkedItemsAsync<T>(Folder<T> folder)
    {
        if (!await _fileSecurity.CanReadAsync(folder))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_ViewFolder);
        }

        if (folder.RootFolderType == FolderType.TRASH && !Equals(folder.Id, await _globalFolder.GetFolderTrashAsync<T>(_daoFactory)))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_ViewTrashItem);
        }

        var tagDao = _daoFactory.GetTagDao<T>();
        var providerFolderDao = _daoFactory.GetFolderDao<string>();
        var providerTagDao = _daoFactory.GetTagDao<string>();
        var tags = await (tagDao.GetNewTagsAsync(_authContext.CurrentAccount.ID, folder, true) ?? AsyncEnumerable.Empty<Tag>()).ToListAsync();

        if (!tags.Any())
        {
            return new List<FileEntry>();
        }

        if (Equals(folder.Id, _globalFolder.GetFolderMy(this, _daoFactory)) ||
            Equals(folder.Id, await _globalFolder.GetFolderCommonAsync(this, _daoFactory)) ||
            Equals(folder.Id, await _globalFolder.GetFolderShareAsync(_daoFactory)))
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

        var result = new List<FileEntry>();

        await GetResultAsync(entryTagsInternal);
        await GetResultAsync(entryTagsProvider);

        return result;

        async Task GetResultAsync<TEntry>(Dictionary<FileEntry<TEntry>, Tag> entryTags)
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
                    result.Add(entryTag.Key);
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

    public async Task<IEnumerable<FileEntry>> SetTagsNewAsync<T>(Folder<T> parent, IEnumerable<FileEntry> entries)
    {
        var tagDao = _daoFactory.GetTagDao<T>();
        var folderDao = _daoFactory.GetFolderDao<T>();
        var totalTags = await tagDao.GetNewTagsAsync(_authContext.CurrentAccount.ID, parent, false).ToListAsync();

        if (totalTags.Count > 0)
        {
            var parentFolderTag = Equals(await _globalFolder.GetFolderShareAsync<T>(_daoFactory), parent.Id)
                                        ? await tagDao.GetNewTagsAsync(_authContext.CurrentAccount.ID, await folderDao.GetFolderAsync(await _globalFolder.GetFolderShareAsync<T>(_daoFactory))).FirstOrDefaultAsync()
                                        : totalTags.FirstOrDefault(tag => tag.EntryType == FileEntryType.Folder && Equals(tag.EntryId, parent.Id));

            totalTags.Remove(parentFolderTag);
            var countSubNew = 0;
            totalTags.ForEach(tag => countSubNew += tag.Count);

            if (parentFolderTag == null)
            {
                parentFolderTag = Tag.New(_authContext.CurrentAccount.ID, parent, 0);
                parentFolderTag.Id = -1;
            }

            if (parentFolderTag.Count != countSubNew)
            {
                if (countSubNew > 0)
                {
                    var diff = parentFolderTag.Count - countSubNew;

                    parentFolderTag.Count -= diff;
                    if (parentFolderTag.Id == -1)
                    {
                        tagDao.SaveTags(parentFolderTag);
                    }
                    else
                    {
                        tagDao.UpdateNewTags(parentFolderTag);
                    }

                    var cacheFolderId = parent.Id;
                    var parentsList = await _daoFactory.GetFolderDao<T>().GetParentFoldersAsync(parent.Id);
                    parentsList.Reverse();
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
                        else if (rootFolder.RootFolderType == FolderType.USER && !Equals(rootFolder.RootId, _globalFolder.GetFolderMy(this, _daoFactory)))
                        {
                            cacheFolderId = rootFolderId = await _globalFolder.GetFolderShareAsync<T>(_daoFactory);
                        }

                        if (rootFolderId != null)
                        {
                            parentsList.Add(await _daoFactory.GetFolderDao<T>().GetFolderAsync(rootFolderId));
                        }

                        var fileSecurity = _fileSecurity;

                        foreach (var folderFromList in parentsList)
                        {
                            var parentTreeTag = await tagDao.GetNewTagsAsync(_authContext.CurrentAccount.ID, folderFromList).FirstOrDefaultAsync();

                            if (parentTreeTag == null)
                            {
                                if (await fileSecurity.CanReadAsync(folderFromList))
                                {
                                    tagDao.SaveTags(Tag.New(_authContext.CurrentAccount.ID, folderFromList, -diff));
                                }
                            }
                            else
                            {
                                parentTreeTag.Count -= diff;
                                tagDao.UpdateNewTags(parentTreeTag);
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

            SetTagsNew(totalTags, entries.OfType<FileEntry<int>>().ToList());
            SetTagsNew(totalTags, entries.OfType<FileEntry<string>>().ToList());
        }

        void SetTagsNew<T1>(List<Tag> tags, List<FileEntry<T1>> fileEntries)
        {
            fileEntries
                .ForEach(
                entry =>
                {
                    var curTag = tags.FirstOrDefault(tag => tag.EntryType == entry.FileEntryType && tag.EntryId.Equals(entry.Id));

                    if (entry.FileEntryType == FileEntryType.Folder)
                    {
                        ((IFolder)entry).NewForMe = curTag != null ? curTag.Count : 0;
                    }
                    else if (curTag != null)
                    {
                        entry.IsNew = true;
                    }
                });
        }

        return entries;
    }

    private void InsertToCahce(object folderId, int count)
    {
        var key = string.Format(_cacheKeyFormat, _authContext.CurrentAccount.ID, folderId);
        _cache.Insert(key, count.ToString(), TimeSpan.FromMinutes(10));
    }

    private int GetCountFromCahce(object folderId)
    {
        var key = string.Format(_cacheKeyFormat, _authContext.CurrentAccount.ID, folderId);
        var count = _cache.Get<string>(key);

        return count == null ? -1 : int.Parse(count);
    }

    private void RemoveFromCahce(object folderId)
    {
        RemoveFromCahce(folderId, _authContext.CurrentAccount.ID);
    }

    private void RemoveFromCahce(object folderId, Guid userId)
    {
        var key = string.Format(_cacheKeyFormat, userId, folderId);
        _cache.Remove(key);
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
        services.TryAdd<FileMarkerHelper<int>>();

        services.TryAdd<AsyncTaskData<string>>();
        services.TryAdd<FileMarkerHelper<string>>();
    }
}
