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

namespace ASC.Files.Thirdparty.ProviderDao;

[Scope]
internal class ProviderFolderDao : ProviderDaoBase, IFolderDao<string>
{
    private readonly SetupInfo _setupInfo;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly IProviderDao _providerDao;
    private readonly IDbContextFactory<FilesDbContext> _dbContextFactory;
    private readonly AuthContext _authContext;

    public ProviderFolderDao(
        SetupInfo setupInfo,
        IServiceProvider serviceProvider,
        TenantManager tenantManager,
        SecurityDao<string> securityDao,
        TagDao<string> tagDao,
        CrossDao crossDao,
        GlobalFolderHelper globalFolderHelper,
        IProviderDao providerDao,
        IDbContextFactory<FilesDbContext> dbContextFactory,
        AuthContext authContext)
        : base(serviceProvider, tenantManager, securityDao, tagDao, crossDao)
    {
        _setupInfo = setupInfo;
        _globalFolderHelper = globalFolderHelper;
        _providerDao = providerDao;
        _dbContextFactory = dbContextFactory;
        _authContext = authContext;
    }

    public Task<Folder<string>> GetFolderAsync(string folderId)
    {
        var selector = GetSelector(folderId);
        if (selector == null)
        {
            return null;
        }

        return InternalGetFolderAsync(folderId, selector);
    }

    private async Task<Folder<string>> InternalGetFolderAsync(string folderId, IDaoSelector selector)
    {
        var folderDao = selector.GetFolderDao(folderId);
        var result = await folderDao.GetFolderAsync(selector.ConvertId(folderId));

        return result;
    }

    public Task<Folder<string>> GetFolderAsync(string title, string parentId)
    {
        var selector = GetSelector(parentId);

        return selector.GetFolderDao(parentId).GetFolderAsync(title, selector.ConvertId(parentId));
    }

    public Task<Folder<string>> GetRootFolderAsync(string folderId)
    {
        var selector = GetSelector(folderId);
        var folderDao = selector.GetFolderDao(folderId);

        return folderDao.GetRootFolderAsync(selector.ConvertId(folderId));
    }

    public Task<Folder<string>> GetRootFolderByFileAsync(string fileId)
    {
        var selector = GetSelector(fileId);
        var folderDao = selector.GetFolderDao(fileId);

        return folderDao.GetRootFolderByFileAsync(selector.ConvertId(fileId));
    }

    public IAsyncEnumerable<Folder<string>> GetRoomsAsync(IEnumerable<string> parentsIds, IEnumerable<string> roomsIds, FilterType filterType, IEnumerable<string> tags, Guid subjectId, string searchText, bool withSubfolders, bool withoutTags, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter, IEnumerable<string> subjectEntriesIds)
    {
        var result = AsyncEnumerable.Empty<Folder<string>>();

        foreach (var selector in GetSelectors())
        {
            var selectorLocal = selector;
            var matchedIds = roomsIds.Where(selectorLocal.IsMatch).ToList();

            if (matchedIds.Count == 0)
            {
                continue;
            }

            result = result.Concat(matchedIds.GroupBy(selectorLocal.GetIdCode)
                .ToAsyncEnumerable()
                .SelectMany(matchedId =>
                {
                    var folderDao = selectorLocal.GetFolderDao(matchedId.FirstOrDefault());

                    return folderDao.GetRoomsAsync(parentsIds, matchedId.Select(selectorLocal.ConvertId).ToList(), filterType, tags, subjectId, searchText, withSubfolders, withoutTags, excludeSubject, provider, subjectFilter, subjectEntriesIds);
                })
                .Where(r => r != null));
        }

        result = FilterByProvider(result, provider);

        return result.Distinct();
    }

    public override async IAsyncEnumerable<Folder<string>> GetFakeRoomsAsync(IEnumerable<string> parentsIds, FilterType filterType, IEnumerable<string> tags, Guid subjectId,
        string searchText, bool withSubfolders, bool withoutTags, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter, IEnumerable<string> subjectEntriesIds)
    {
        var virtualRoomsFolderId = await _globalFolderHelper.GetFolderVirtualRooms<string>();
        var archiveFolderId = await _globalFolderHelper.GetFolderArchive<string>();

        var rooms = GetProvidersAsync(parentsIds, virtualRoomsFolderId, archiveFolderId).Where(p => !string.IsNullOrEmpty(p.FolderId))
            .Select(r => ToFakeRoom(r, virtualRoomsFolderId, archiveFolderId));

        var filesDbContext = _dbContextFactory.CreateDbContext();

        rooms = FilterRoomsAsync(rooms, provider, filterType, subjectId, excludeSubject, subjectFilter, subjectEntriesIds, searchText, withoutTags, tags, filesDbContext);

        await foreach (var room in rooms)
        {
            yield return room;
        }
    }

    public override async IAsyncEnumerable<Folder<string>> GetFakeRoomsAsync(IEnumerable<string> parentsIds, IEnumerable<string> roomsIds, FilterType filterType, IEnumerable<string> tags,
        Guid subjectId, string searchText, bool withSubfolders, bool withoutTags, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter,
        IEnumerable<string> subjectEntriesIds)
    {
        if (!roomsIds.Any())
        {
            yield break;
        }

        var virtualRoomsFolderId = await _globalFolderHelper.GetFolderVirtualRooms<string>();
        var archiveFolderId = await _globalFolderHelper.GetFolderArchive<string>();

        var rooms = GetProvidersAsync(parentsIds, virtualRoomsFolderId, archiveFolderId)
            .Where(p => !string.IsNullOrEmpty(p.FolderId) && (p.Owner == _authContext.CurrentAccount.ID || roomsIds.Contains(p.FolderId)))
            .Select(r => ToFakeRoom(r, virtualRoomsFolderId, archiveFolderId));

        var filesDbContext = _dbContextFactory.CreateDbContext();

        rooms = FilterRoomsAsync(rooms, provider, filterType, subjectId, excludeSubject, subjectFilter, subjectEntriesIds, searchText, withoutTags, tags, filesDbContext);

        await foreach (var room in rooms)
        {
            yield return room;
        }
    }

    public IAsyncEnumerable<Folder<string>> GetFoldersAsync(string parentId)
    {
        var selector = GetSelector(parentId);
        var folderDao = selector.GetFolderDao(parentId);
        var folders = folderDao.GetFoldersAsync(selector.ConvertId(parentId));

        return folders.Where(r => r != null);
    }

    public async IAsyncEnumerable<Folder<string>> GetFoldersAsync(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText,
        bool withSubfolders = false, bool excludeSubject = false, int offset = 0, int count = -1)
    {
        var selector = GetSelector(parentId);
        var folderDao = selector.GetFolderDao(parentId);
        var folders = folderDao.GetFoldersAsync(selector.ConvertId(parentId), orderBy, filterType, subjectGroup, subjectID, searchText, withSubfolders, excludeSubject);
        var result = await folders.Where(r => r != null).ToListAsync();

        foreach (var r in result)
        {
            yield return r;
        }
    }

    public IAsyncEnumerable<Folder<string>> GetFoldersAsync(IEnumerable<string> folderIds, FilterType filterType = FilterType.None, bool subjectGroup = false, Guid? subjectID = null, string searchText = "", bool searchSubfolders = false, bool checkShare = true, bool excludeSubject = false)
    {
        var result = AsyncEnumerable.Empty<Folder<string>>();

        foreach (var selector in GetSelectors())
        {
            var selectorLocal = selector;
            var matchedIds = folderIds.Where(selectorLocal.IsMatch).ToList();

            if (matchedIds.Count == 0)
            {
                continue;
            }

            result = result.Concat(matchedIds.GroupBy(selectorLocal.GetIdCode)
                .ToAsyncEnumerable()
                .SelectMany(matchedId =>
                {
                    var folderDao = selectorLocal.GetFolderDao(matchedId.FirstOrDefault());

                    return folderDao.GetFoldersAsync(matchedId.Select(selectorLocal.ConvertId).ToList(),
                        filterType, subjectGroup, subjectID, searchText, searchSubfolders, checkShare, excludeSubject);
                })
                .Where(r => r != null));
        }

        return result.Distinct();
    }

    public IAsyncEnumerable<Folder<string>> GetParentFoldersAsync(string folderId)
    {
        var selector = GetSelector(folderId);
        var folderDao = selector.GetFolderDao(folderId);

        return folderDao.GetParentFoldersAsync(selector.ConvertId(folderId));
    }


    public Task<string> SaveFolderAsync(Folder<string> folder)
    {
        ArgumentNullException.ThrowIfNull(folder);

        return InternalSaveFolderAsync(folder);
    }

    private async Task<string> InternalSaveFolderAsync(Folder<string> folder)
    {
        if (folder.Id != null)
        {
            var folderId = folder.Id;
            var selector = GetSelector(folderId);
            folder.Id = selector.ConvertId(folderId);
            var folderDao = selector.GetFolderDao(folderId);
            var newFolderId = await folderDao.SaveFolderAsync(folder);
            folder.Id = folderId;

            return newFolderId;
        }
        if (folder.ParentId != null)
        {
            var folderId = folder.ParentId;
            var selector = GetSelector(folderId);
            folder.ParentId = selector.ConvertId(folderId);
            var folderDao = selector.GetFolderDao(folderId);
            var newFolderId = await folderDao.SaveFolderAsync(folder);
            folder.ParentId = folderId;

            return newFolderId;

        }

        throw new ArgumentException("No folder id or parent folder id to determine provider");
    }

    public Task DeleteFolderAsync(string folderId)
    {
        var selector = GetSelector(folderId);
        var folderDao = selector.GetFolderDao(folderId);

        return folderDao.DeleteFolderAsync(selector.ConvertId(folderId));
    }

    public async Task<TTo> MoveFolderAsync<TTo>(string folderId, TTo toFolderId, CancellationToken? cancellationToken)
    {
        if (toFolderId is int tId)
        {
            return (TTo)Convert.ChangeType(await MoveFolderAsync(folderId, tId, cancellationToken), typeof(TTo));
        }

        if (toFolderId is string tsId)
        {
            return (TTo)Convert.ChangeType(await MoveFolderAsync(folderId, tsId, cancellationToken), typeof(TTo));
        }

        throw new NotImplementedException();
    }

    public async Task<string> MoveFolderAsync(string folderId, string toFolderId, CancellationToken? cancellationToken)
    {
        var selector = GetSelector(folderId);
        if (IsCrossDao(folderId, toFolderId))
        {
            var newFolder = await PerformCrossDaoFolderCopyAsync(folderId, toFolderId, true, cancellationToken);

            return newFolder?.Id;
        }
        var folderDao = selector.GetFolderDao(folderId);

        return await folderDao.MoveFolderAsync(selector.ConvertId(folderId), selector.ConvertId(toFolderId), null);
    }

    public async Task<int> MoveFolderAsync(string folderId, int toFolderId, CancellationToken? cancellationToken)
    {
        var newFolder = await PerformCrossDaoFolderCopyAsync(folderId, toFolderId, true, cancellationToken);

        return newFolder.Id;
    }

    public async Task<Folder<TTo>> CopyFolderAsync<TTo>(string folderId, TTo toFolderId, CancellationToken? cancellationToken)
    {
        if (toFolderId is int tId)
        {
            return await CopyFolderAsync(folderId, tId, cancellationToken) as Folder<TTo>;
        }

        if (toFolderId is string tsId)
        {
            return await CopyFolderAsync(folderId, tsId, cancellationToken) as Folder<TTo>;
        }

        throw new NotImplementedException();
    }

    public Task<Folder<int>> CopyFolderAsync(string folderId, int toFolderId, CancellationToken? cancellationToken)
    {
        return PerformCrossDaoFolderCopyAsync(folderId, toFolderId, false, cancellationToken);
    }

    public async Task<Folder<string>> CopyFolderAsync(string folderId, string toFolderId, CancellationToken? cancellationToken)
    {
        var selector = GetSelector(folderId);
        var folderDao = selector.GetFolderDao(folderId);

        return IsCrossDao(folderId, toFolderId)
                ? await PerformCrossDaoFolderCopyAsync(folderId, toFolderId, false, cancellationToken)
                : await folderDao.CopyFolderAsync(selector.ConvertId(folderId), selector.ConvertId(toFolderId), null);
    }

    public async Task<IDictionary<string, TTo>> CanMoveOrCopyAsync<TTo>(string[] folderIds, TTo to)
    {
        if (to is int tId)
        {
            return await CanMoveOrCopyAsync<TTo>(folderIds, tId);
        }

        if (to is string tsId)
        {
            return await CanMoveOrCopyAsync<TTo>(folderIds, tsId);
        }

        throw new NotImplementedException();
    }

    public Task<IDictionary<string, TTo>> CanMoveOrCopyAsync<TTo>(string[] folderIds, int to)
    {
        return Task.FromResult((IDictionary<string, TTo>)new Dictionary<string, TTo>());
    }

    public Task<IDictionary<string, TTo>> CanMoveOrCopyAsync<TTo>(string[] folderIds, string to)
    {
        if (folderIds.Length == 0)
        {
            return Task.FromResult<IDictionary<string, TTo>>(new Dictionary<string, TTo>());
        }

        var selector = GetSelector(to);
        var matchedIds = folderIds.Where(selector.IsMatch).ToArray();

        if (matchedIds.Length == 0)
        {
            return Task.FromResult<IDictionary<string, TTo>>(new Dictionary<string, TTo>());
        }

        return InternalCanMoveOrCopyAsync<TTo>(to, matchedIds, selector);
    }

    private Task<IDictionary<string, TTo>> InternalCanMoveOrCopyAsync<TTo>(string to, string[] matchedIds, IDaoSelector selector)
    {
        var folderDao = selector.GetFolderDao(matchedIds.FirstOrDefault());

        return folderDao.CanMoveOrCopyAsync<TTo>(matchedIds, to);
    }

    public Task<string> RenameFolderAsync(Folder<string> folder, string newTitle)
    {
        var folderId = folder.Id;
        var selector = GetSelector(folderId);
        folder.Id = selector.ConvertId(folderId);
        folder.ParentId = selector.ConvertId(folder.ParentId);
        var folderDao = selector.GetFolderDao(folderId);

        return folderDao.RenameFolderAsync(folder, newTitle);
    }

    public Task<int> GetItemsCountAsync(string folderId)
    {
        var selector = GetSelector(folderId);
        var folderDao = selector.GetFolderDao(folderId);

        return folderDao.GetItemsCountAsync(selector.ConvertId(folderId));
    }

    public Task<bool> IsEmptyAsync(string folderId)
    {
        var selector = GetSelector(folderId);
        var folderDao = selector.GetFolderDao(folderId);

        return folderDao.IsEmptyAsync(selector.ConvertId(folderId));
    }

    public bool UseTrashForRemoveAsync(Folder<string> folder)
    {
        var selector = GetSelector(folder.Id);
        var folderDao = selector.GetFolderDao(folder.Id);

        return folderDao.UseTrashForRemoveAsync(folder);
    }

    public bool UseRecursiveOperation<TTo>(string folderId, TTo toRootFolderId)
    {
        return false;
    }

    public bool UseRecursiveOperation(string folderId, int toRootFolderId)
    {
        return false;
    }

    public bool UseRecursiveOperation(string folderId, string toRootFolderId)
    {
        var selector = GetSelector(folderId);
        bool useRecursive;

        var folderDao = selector.GetFolderDao(folderId);
        useRecursive = folderDao.UseRecursiveOperation(folderId, null);

        if (toRootFolderId != null)
        {
            var toFolderSelector = GetSelector(toRootFolderId);

            var folderDao1 = toFolderSelector.GetFolderDao(toRootFolderId);
            useRecursive = useRecursive && folderDao1.UseRecursiveOperation(folderId, toFolderSelector.ConvertId(toRootFolderId));
        }

        return useRecursive;
    }

    public bool CanCalculateSubitems(string entryId)
    {
        var selector = GetSelector(entryId);
        var folderDao = selector.GetFolderDao(entryId);

        return folderDao.CanCalculateSubitems(entryId);
    }

    public async Task<long> GetMaxUploadSizeAsync(string folderId, bool chunkedUpload = false)
    {
        var selector = GetSelector(folderId);
        var folderDao = selector.GetFolderDao(folderId);
        var storageMaxUploadSize = await folderDao.GetMaxUploadSizeAsync(selector.ConvertId(folderId), chunkedUpload);

        if (storageMaxUploadSize == -1 || storageMaxUploadSize == long.MaxValue)
        {
            storageMaxUploadSize = _setupInfo.ProviderMaxUploadSize;
        }

        return storageMaxUploadSize;
    }

    public IDataWriteOperator CreateDataWriteOperator(
            string folderId,
            CommonChunkedUploadSession chunkedUploadSession,
            CommonChunkedUploadSessionHolder sessionHolder)
    {
        var selector = GetSelector(folderId);
        var folderDao = selector.GetFolderDao(folderId);
        return folderDao.CreateDataWriteOperator(folderId, chunkedUploadSession, sessionHolder);
    }

    private IAsyncEnumerable<Folder<string>> FilterByProvider(IAsyncEnumerable<Folder<string>> folders, ProviderFilter provider)
    {
        if (provider != ProviderFilter.kDrive && provider != ProviderFilter.WebDav && provider != ProviderFilter.Yandex)
        {
            return folders;
        }

        var providerKey = provider switch
        {
            ProviderFilter.Yandex => ProviderTypes.Yandex.ToStringFast(),
            ProviderFilter.WebDav => ProviderTypes.WebDav.ToStringFast(),
            ProviderFilter.kDrive => ProviderTypes.kDrive.ToStringFast(),
            _ => throw new NotImplementedException(),
        };

        return folders.Where(x => providerKey == x.ProviderKey);
    }

    private async IAsyncEnumerable<IProviderInfo> GetProvidersAsync(IEnumerable<string> parentsIds, string virtualRoomsFolderId, string archiveFolderId)
    {
        IAsyncEnumerable<IProviderInfo> providers;

        if (parentsIds.Count() > 1)
        {
            providers = _providerDao.GetProvidersInfoAsync(FolderType.VirtualRooms);
            providers = providers.Concat(_providerDao.GetProvidersInfoAsync(FolderType.Archive));
        }
        else if (parentsIds.FirstOrDefault() == virtualRoomsFolderId)
        {
            providers = _providerDao.GetProvidersInfoAsync(FolderType.VirtualRooms);
        }
        else
        {
            providers = _providerDao.GetProvidersInfoAsync(FolderType.Archive);
        }

        await foreach (var provider in providers)
        {
            yield return provider;
        }
    }

    private Folder<string> ToFakeRoom(IProviderInfo providerInfo, string roomsFolderId, string archiveFolderId)
    {
        var rootId = providerInfo.RootFolderType == FolderType.VirtualRooms ? roomsFolderId : archiveFolderId;

        var folder = _serviceProvider.GetRequiredService<Folder<string>>();
        folder.Id = providerInfo.FolderId;
        folder.ParentId = rootId;
        folder.RootCreateBy = providerInfo.Owner;
        folder.CreateBy = providerInfo.Owner;
        folder.ProviderKey = providerInfo.ProviderKey;
        folder.RootId = rootId;
        folder.Title = providerInfo.CustomerTitle;
        folder.CreateOn = providerInfo.CreateOn;
        folder.FileEntryType = FileEntryType.Folder;
        folder.FolderType = providerInfo.FolderType;
        folder.ProviderId = providerInfo.ID;
        folder.RootFolderType = providerInfo.RootFolderType;
        folder.HasLogo = providerInfo.HasLogo;
        folder.ModifiedBy = providerInfo.Owner;
        folder.ModifiedOn = providerInfo.CreateOn;

        return folder;
    }
}
