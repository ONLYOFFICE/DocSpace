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

namespace ASC.Files.Thirdparty;

internal abstract class ThirdPartyProviderDao
{
    #region FileDao

    public Task ReassignFilesAsync(string[] fileIds, Guid newOwnerId)
    {
        return Task.CompletedTask;
    }

    public IAsyncEnumerable<File<string>> GetFilesAsync(IEnumerable<string> parentIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent)
    {
        return AsyncEnumerable.Empty<File<string>>();
    }

    public IAsyncEnumerable<File<string>> SearchAsync(string text, bool bunch)
    {
        return null;
    }

    public Task<bool> IsExistOnStorageAsync(File<string> file)
    {
        return Task.FromResult(true);
    }

    public Task SaveEditHistoryAsync(File<string> file, string changes, Stream differenceStream)
    {
        //Do nothing
        return Task.CompletedTask;
    }

    public IAsyncEnumerable<EditHistory> GetEditHistoryAsync(DocumentServiceHelper documentServiceHelper, string fileId, int fileVersion)
    {
        return null;
    }

    public Task<Stream> GetDifferenceStreamAsync(File<string> file)
    {
        return null;
    }

    public Task<bool> ContainChangesAsync(string fileId, int fileVersion)
    {
        return Task.FromResult(false);
    }

    public string GetUniqThumbnailPath(File<string> file, int width, int height)
    {
        //Do nothing
        return null;
    }

    public Task SetThumbnailStatusAsync(File<string> file, Thumbnail status)
    {
        return Task.CompletedTask;
    }

    public virtual Task<Stream> GetThumbnailAsync(File<string> file, int width, int height)
    {
        return GetThumbnailAsync(file.Id, width, height);
    }

    public virtual Task<Stream> GetThumbnailAsync(string file, int width, int height)
    {
        return Task.FromResult<Stream>(null);
    }

    public Task<EntryProperties> GetProperties(string fileId)
    {
        return Task.FromResult<EntryProperties>(null);
    }

    public Task SaveProperties(string fileId, EntryProperties entryProperties)
    {
        return null;
    }

    public virtual Task<Stream> GetFileStreamAsync(File<string> file)
    {
        return null;
    }

    public string GetUniqFilePath(File<string> file, string fileTitle)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<FileWithShare> GetFeedsAsync(int tenant, DateTime from, DateTime to)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<int> GetTenantsWithFeedsAsync(DateTime fromTime, bool includeSecurity)
    {
        throw new NotImplementedException();
    }

    #endregion
    #region FolderDao

    public Task ReassignFoldersAsync(string[] folderIds, Guid newOwnerId)
    {
        return Task.CompletedTask;
    }

    public IAsyncEnumerable<Folder<string>> SearchFoldersAsync(string text, bool bunch)
    {
        return null;
    }


    public Task<string> GetFolderIDAsync(string module, string bunch, string data, bool createIfNotExists)
    {
        return null;
    }

    public IAsyncEnumerable<string> GetFolderIDsAsync(string module, string bunch, IEnumerable<string> data, bool createIfNotExists)
    {
        return AsyncEnumerable.Empty<string>();
    }

    public Task<string> GetFolderIDCommonAsync(bool createIfNotExists)
    {
        return null;
    }


    public Task<string> GetFolderIDUserAsync(bool createIfNotExists, Guid? userId)
    {
        return null;
    }

    public Task<string> GetFolderIDShareAsync(bool createIfNotExists)
    {
        return null;
    }


    public Task<string> GetFolderIDRecentAsync(bool createIfNotExists)
    {
        return null;
    }

    public Task<string> GetFolderIDFavoritesAsync(bool createIfNotExists)
    {
        return null;
    }

    public Task<string> GetFolderIDTemplatesAsync(bool createIfNotExists)
    {
        return null;
    }

    public Task<string> GetFolderIDPrivacyAsync(bool createIfNotExists, Guid? userId)
    {
        return null;
    }

    public Task<string> GetFolderIDTrashAsync(bool createIfNotExists, Guid? userId)
    {
        return null;
    }

    public string GetFolderIDPhotos(bool createIfNotExists)
    {
        return null;
    }


    public Task<string> GetFolderIDProjectsAsync(bool createIfNotExists)
    {
        return null;
    }

    public Task<string> GetFolderIDVirtualRooms(bool createIfNotExists)
    {
        return null;
    }

    public Task<string> GetFolderIDArchive(bool createIfNotExists)
    {
        return null;
    }

    public Task<string> GetBunchObjectIDAsync(string folderID)
    {
        return null;
    }

    public Task<Dictionary<string, string>> GetBunchObjectIDsAsync(List<string> folderIDs)
    {
        return null;
    }

    public IAsyncEnumerable<FolderWithShare> GetFeedsForRoomsAsync(int tenant, DateTime from, DateTime to)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<FolderWithShare> GetFeedsForFoldersAsync(int tenant, DateTime from, DateTime to)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<ParentRoomPair> GetParentRoomsAsync(IEnumerable<int> foldersIds)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<int> GetTenantsWithFoldersFeedsAsync(DateTime fromTime)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<int> GetTenantsWithRoomsFeedsAsync(DateTime fromTime)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<OriginData> GetOriginsDataAsync(IEnumerable<string> entriesId)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Folder<string>> GetRoomsAsync(IEnumerable<string> parentsIds, FilterType filterType, IEnumerable<string> tags, Guid subjectId, string searchText,
        bool withSubfolders, bool withoutTags, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter, IEnumerable<string> subjectEntriesIds)
    {
        return AsyncEnumerable.Empty<Folder<string>>();
    }

    public virtual IAsyncEnumerable<Folder<string>> GetFakeRoomsAsync(IEnumerable<string> parentsIds, FilterType filterType, IEnumerable<string> tags, Guid subjectId, string searchText,
        bool withSubfolders, bool withoutTags, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter, IEnumerable<string> subjectEntriesIds)
    {
        return AsyncEnumerable.Empty<Folder<string>>();
    }

    public virtual IAsyncEnumerable<Folder<string>> GetFakeRoomsAsync(IEnumerable<string> parentsIds, IEnumerable<string> roomsIds, FilterType filterType, IEnumerable<string> tags,
        Guid subjectId, string searchText, bool withSubfolders, bool withoutTags, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter,
        IEnumerable<string> subjectEntriesIds)
    {
        return AsyncEnumerable.Empty<Folder<string>>();
    }

    protected static IAsyncEnumerable<Folder<string>> FilterRoomsAsync(IAsyncEnumerable<Folder<string>> rooms, ProviderFilter provider, FilterType filterType, Guid subjectId,
        bool excludeSubject, SubjectFilter subjectFilter, IEnumerable<string> subjectEntriesIds, string searchText, bool withoutTags, IEnumerable<string> tags, 
        FilesDbContext filesDbContext)
    {
        rooms = FilterByProvidersAsync(rooms, provider);
        rooms = FilterByRoomType(rooms, filterType);
        rooms = FilterBySubject(rooms, subjectId, excludeSubject, subjectFilter, subjectEntriesIds);
        rooms = FilterByTitle(rooms, searchText);
        rooms = FilterByTags(rooms, withoutTags, tags, filesDbContext);

        return rooms;
    }

    protected static IAsyncEnumerable<Folder<string>> FilterByTags(IAsyncEnumerable<Folder<string>> rooms, bool withoutTags, IEnumerable<string> tags, FilesDbContext filesDbContext)
    {
        if (withoutTags)
        {
            return rooms.Join(filesDbContext.ThirdpartyIdMapping.ToAsyncEnumerable(), f => f.Id, m => m.Id, (folder, map) => new { folder, map.HashId })
                .WhereAwait(async r => !await filesDbContext.TagLink.Join(filesDbContext.Tag, l => l.TagId, t => t.Id, (link, tag) => new { link.EntryId, tag })
                    .Where(r => r.tag.Type == TagType.Custom).ToAsyncEnumerable().AnyAsync(t => t.EntryId == r.HashId))
                .Select(r => r.folder);
        }

        if (tags == null || !tags.Any())
        {
            return rooms;
        }

        var filtered = rooms.Join(filesDbContext.ThirdpartyIdMapping.ToAsyncEnumerable(), f => f.Id, m => m.Id, (folder, map) => new { folder, map.HashId })
            .Join(filesDbContext.TagLink.ToAsyncEnumerable(), r => r.HashId, t => t.EntryId, (result, tag) => new { result.folder, tag.TagId })
            .Join(filesDbContext.Tag.ToAsyncEnumerable(), r => r.TagId, t => t.Id, (result, tagInfo) => new { result.folder, tagInfo.Name })
            .Where(r => tags.Contains(r.Name))
            .Select(r => r.folder);

        return filtered;
    }

    protected static IAsyncEnumerable<Folder<string>> FilterByProvidersAsync(IAsyncEnumerable<Folder<string>> rooms, ProviderFilter providerFilter)
    {
        if (providerFilter == ProviderFilter.None)
        {
            return rooms;
        }

        var filter = providerFilter switch
        {
            ProviderFilter.WebDav => new[] { ProviderTypes.WebDav.ToStringFast() },
            ProviderFilter.GoogleDrive => new[] { ProviderTypes.GoogleDrive.ToStringFast() },
            ProviderFilter.OneDrive => new[] { ProviderTypes.OneDrive.ToStringFast() },
            ProviderFilter.DropBox => new[] { ProviderTypes.DropBox.ToStringFast(), ProviderTypes.DropboxV2.ToStringFast() },
            ProviderFilter.kDrive => new[] { ProviderTypes.kDrive.ToStringFast() },
            ProviderFilter.Yandex => new[] { ProviderTypes.Yandex.ToStringFast() },
            ProviderFilter.SharePoint => new[] { ProviderTypes.SharePoint.ToStringFast() },
            ProviderFilter.Box => new[] { ProviderTypes.Box.ToStringFast() },
            _ => throw new NotImplementedException()
        };

        return rooms.Where(f => filter.Contains(f.ProviderKey));
    }

    protected static IAsyncEnumerable<Folder<string>> FilterByRoomType(IAsyncEnumerable<Folder<string>> rooms, FilterType filterType)
    {
        if (filterType is FilterType.None or FilterType.FoldersOnly)
        {
            return rooms;
        }

        var typeFilter = filterType switch
        {
            FilterType.FillingFormsRooms => FolderType.FillingFormsRoom,
            FilterType.EditingRooms => FolderType.EditingRoom,
            FilterType.ReviewRooms => FolderType.ReviewRoom,
            FilterType.ReadOnlyRooms => FolderType.ReadOnlyRoom,
            FilterType.CustomRooms => FolderType.CustomRoom,
            _ => FolderType.DEFAULT,
        };

        return rooms.Where(f => f.FolderType == typeFilter);
    }

    protected static IAsyncEnumerable<Folder<string>> FilterBySubject(IAsyncEnumerable<Folder<string>> rooms, Guid subjectId, bool excludeSubject, SubjectFilter subjectFilter,
        IEnumerable<string> subjectEntriesIds)
    {
        if (subjectId == Guid.Empty)
        {
            return rooms;
        }

        if (subjectFilter == SubjectFilter.Owner)
        {
            return excludeSubject ? rooms.Where(f => f != null && f.CreateBy != subjectId) : rooms.Where(f => f != null && f.CreateBy == subjectId);
        }
        if (subjectFilter == SubjectFilter.Member)
        {
            return excludeSubject ? rooms.Where(f => f != null && f.CreateBy != subjectId && !subjectEntriesIds.Contains(f.Id))
                : rooms.Where(f => f != null && (f.CreateBy == subjectId || subjectEntriesIds.Contains(f.Id)));
        }

        return rooms;
    }

    protected static IAsyncEnumerable<Folder<string>> FilterByTitle(IAsyncEnumerable<Folder<string>> rooms, string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return rooms;
        }
        
        return rooms.Where(x => x.Title.IndexOf(text, StringComparison.OrdinalIgnoreCase) != -1);
    }

    #endregion
}

internal abstract class ThirdPartyProviderDao<T> : ThirdPartyProviderDao, IDisposable where T : class, IProviderInfo
{
    public int TenantID { get; private set; }
    protected readonly IServiceProvider _serviceProvider;
    protected readonly UserManager _userManager;
    protected readonly TenantUtil _tenantUtil;
    protected readonly IDbContextFactory<FilesDbContext> _dbContextFactory;
    protected readonly SetupInfo _setupInfo;
    protected readonly ILogger _logger;
    protected readonly FileUtility _fileUtility;
    protected readonly TempPath _tempPath;
    protected readonly AuthContext _authContext;
    protected RegexDaoSelectorBase<T> DaoSelector { get; set; }
    protected T ProviderInfo { get; set; }
    protected string PathPrefix { get; private set; }

    protected abstract string Id { get; }

    protected ThirdPartyProviderDao(
        IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        IDbContextFactory<FilesDbContext> dbContextFactory,
        SetupInfo setupInfo,
        ILogger logger,
        FileUtility fileUtility,
        TempPath tempPath,
        AuthContext authContext)
    {
        _serviceProvider = serviceProvider;
        _userManager = userManager;
        _tenantUtil = tenantUtil;
        _dbContextFactory = dbContextFactory;
        _setupInfo = setupInfo;
        _logger = logger;
        _fileUtility = fileUtility;
        _tempPath = tempPath;
        TenantID = tenantManager.GetCurrentTenant().Id;
        _authContext = authContext;
    }

    public void Init(BaseProviderInfo<T> providerInfo, RegexDaoSelectorBase<T> selectorBase)
    {
        ProviderInfo = providerInfo.ProviderInfo;
        PathPrefix = providerInfo.PathPrefix;
        DaoSelector = selectorBase;
    }

    protected IQueryable<TSet> Query<TSet>(DbSet<TSet> set) where TSet : class, IDbFile
    {
        return set.Where(r => r.TenantId == TenantID);
    }

    protected Task<string> MappingIDAsync(string id, bool saveIfNotExist = false)
    {
        if (id == null)
        {
            return null;
        }

        return InternalMappingIDAsync(id, saveIfNotExist);
    }

    private async Task<string> InternalMappingIDAsync(string id, bool saveIfNotExist = false)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        string result;
        if (id.StartsWith(Id))
        {
            result = Regex.Replace(BitConverter.ToString(Hasher.Hash(id, HashAlg.MD5)), "-", "").ToLower();
        }
        else
        {
            result = await filesDbContext.ThirdpartyIdMapping
                    .Where(r => r.HashId == id)
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();
        }
        if (saveIfNotExist)
        {
            var newMapping = new DbFilesThirdpartyIdMapping
            {
                Id = id,
                HashId = result,
                TenantId = TenantID
            };

            await filesDbContext.ThirdpartyIdMapping.AddAsync(newMapping);
            await filesDbContext.SaveChangesAsync();
        }

        return result;
    }

    protected Folder<string> GetFolder()
    {
        var folder = _serviceProvider.GetService<Folder<string>>();

        InitFileEntry(folder);

        folder.FolderType = FolderType.DEFAULT;
        folder.Shareable = false;
        folder.FilesCount = 0;
        folder.FoldersCount = 0;

        return folder;
    }

    protected Folder<string> GetErrorFolder(ErrorEntry entry)
    {
        var folder = GetFolder();

        InitFileEntryError(folder, entry);

        folder.ParentId = null;

        return folder;
    }

    protected File<string> GetFile()
    {
        var file = _serviceProvider.GetService<File<string>>();

        InitFileEntry(file);

        file.Access = FileShare.None;
        file.Shared = false;
        file.Version = 1;

        return file;
    }

    protected File<string> GetErrorFile(ErrorEntry entry)
    {
        var file = GetFile();
        InitFileEntryError(file, entry);

        return file;
    }

    protected void InitFileEntry(FileEntry<string> fileEntry)
    {
        fileEntry.CreateBy = ProviderInfo.Owner;
        fileEntry.ModifiedBy = ProviderInfo.Owner;
        fileEntry.ProviderId = ProviderInfo.ID;
        fileEntry.ProviderKey = ProviderInfo.ProviderKey;
        fileEntry.RootCreateBy = ProviderInfo.Owner;
        fileEntry.RootFolderType = ProviderInfo.RootFolderType;
        fileEntry.RootId = MakeId();
    }

    protected void InitFileEntryError(FileEntry<string> fileEntry, ErrorEntry entry)
    {
        fileEntry.Id = MakeId(entry.ErrorId);
        fileEntry.CreateOn = _tenantUtil.DateTimeNow();
        fileEntry.ModifiedOn = _tenantUtil.DateTimeNow();
        fileEntry.Error = entry.Error;
    }

    protected void SetFolderType(Folder<string> folder, bool isRoot)
    {
        if (isRoot && (ProviderInfo.RootFolderType == FolderType.VirtualRooms ||
            ProviderInfo.RootFolderType == FolderType.Archive))
        {
            folder.FolderType = ProviderInfo.RootFolderType;
        }
        else if (ProviderInfo.FolderId == folder.Id)
        {
            folder.FolderType = ProviderInfo.FolderType;
        }
    }

    protected bool CheckInvalidFilter(FilterType filterType)
    {
        return filterType is
            FilterType.FilesOnly or
            FilterType.ByExtension or
            FilterType.DocumentsOnly or
            FilterType.OFormOnly or
            FilterType.OFormTemplateOnly or
            FilterType.ImagesOnly or
            FilterType.PresentationsOnly or
            FilterType.SpreadsheetsOnly or
            FilterType.ArchiveOnly or
            FilterType.MediaOnly;
    }

    protected abstract string MakeId(string path = null);


    #region SecurityDao
    public Task SetShareAsync(FileShareRecord r)
    {
        return Task.CompletedTask;
    }

    public IAsyncEnumerable<FileShareRecord> GetSharesAsync(IEnumerable<Guid> subjects)
    {
        return AsyncEnumerable.Empty<FileShareRecord>();
    }

    public Task<IEnumerable<FileShareRecord>> GetSharesAsync(FileEntry<string> entry)
    {
        return null;
    }

    public Task RemoveSubjectAsync(Guid subject)
    {
        return Task.CompletedTask;
    }

    public IAsyncEnumerable<FileShareRecord> GetPureShareRecordsAsync(IEnumerable<FileEntry<string>> entries)
    {
        return null;
    }

    public IAsyncEnumerable<FileShareRecord> GetPureShareRecordsAsync(FileEntry<string> entry)
    {
        return null;
    }

    public Task DeleteShareRecordsAsync(IEnumerable<FileShareRecord> records)
    {
        return Task.CompletedTask;
    }

    public Task<bool> IsSharedAsync(string entryId, FileEntryType type)
    {
        return null;
    }

    #endregion

    #region TagDao

    public IAsyncEnumerable<Tag> GetTagsAsync(Guid subject, TagType tagType, IEnumerable<FileEntry<string>> fileEntries)
    {
        return AsyncEnumerable.Empty<Tag>();
    }

    public IAsyncEnumerable<Tag> GetTagsAsync(TagType tagType, IEnumerable<FileEntry<string>> fileEntries)
    {
        return AsyncEnumerable.Empty<Tag>();
    }

    public IAsyncEnumerable<Tag> GetTagsAsync(Guid owner, TagType tagType)
    {
        return AsyncEnumerable.Empty<Tag>();
    }

    public IAsyncEnumerable<Tag> GetTagsAsync(string name, TagType tagType)
    {
        return AsyncEnumerable.Empty<Tag>();
    }

    public IAsyncEnumerable<Tag> GetTagsAsync(string[] names, TagType tagType)
    {
        return AsyncEnumerable.Empty<Tag>();
    }

    public Task<IDictionary<object, IEnumerable<Tag>>> GetTagsAsync(Guid subject, IEnumerable<TagType> tagType, IEnumerable<FileEntry<string>> fileEntries)
    {
        return Task.FromResult((IDictionary<object, IEnumerable<Tag>>)new Dictionary<object, IEnumerable<Tag>>());
    }

    public Task<IDictionary<object, IEnumerable<Tag>>> GetTagsAsync(Guid subject, IEnumerable<TagType> tagType, IAsyncEnumerable<FileEntry<string>> fileEntries)
    {
        return Task.FromResult((IDictionary<object, IEnumerable<Tag>>)new Dictionary<object, IEnumerable<Tag>>());
    }

    public IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, IEnumerable<FileEntry<string>> fileEntries)
    {
        return AsyncEnumerable.Empty<Tag>();
    }

    public IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, IAsyncEnumerable<FileEntry<string>> fileEntries)
    {
        return AsyncEnumerable.Empty<Tag>();
    }

    public IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, FileEntry<string> fileEntry)
    {
        return AsyncEnumerable.Empty<Tag>();
    }

    public IAsyncEnumerable<TagInfo> GetTagsInfoAsync(string searchText, TagType tagType, bool byName, int from = 0, int count = 0)
    {
        return AsyncEnumerable.Empty<TagInfo>();
    }

    public IAsyncEnumerable<TagInfo> GetTagsInfoAsync(IEnumerable<string> names)
    {
        return AsyncEnumerable.Empty<TagInfo>();
    }

    public Task<TagInfo> SaveTagInfoAsync(TagInfo tagInfo)
    {
        return Task.FromResult(tagInfo);
    }

    public Task<IEnumerable<Tag>> SaveTags(IEnumerable<Tag> tag, Guid createdBy = default)
    {
        return Task.FromResult(Enumerable.Empty<Tag>());
    }

    public Task<IEnumerable<Tag>> SaveTags(Tag tag)
    {
        return Task.FromResult(Enumerable.Empty<Tag>());
    }

    public Task UpdateNewTags(IEnumerable<Tag> tag, Guid createdBy = default)
    {
        return Task.CompletedTask;
    }

    public Task UpdateNewTags(Tag tag)
    {
        return Task.CompletedTask;
    }

    public Task RemoveTagsAsync(FileEntry<string> entry, IEnumerable<int> tagsIds)
    {
        return Task.CompletedTask;
    }

    public Task RemoveTagsAsync(IEnumerable<int> tagsIds)
    {
        return Task.CompletedTask;
    }

    public Task RemoveTags(IEnumerable<Tag> tag)
    {
        return Task.CompletedTask;
    }

    public Task RemoveTags(Tag tag)
    {
        return Task.CompletedTask;
    }

    public Task<int> RemoveTagLinksAsync(string entryId, FileEntryType entryType, TagType tagType)
    {
        return Task.FromResult(default(int));
    }

    public IAsyncEnumerable<Tag> GetTagsAsync(string entryID, FileEntryType entryType, TagType tagType)
    {
        return AsyncEnumerable.Empty<Tag>();
    }

    public void MarkAsNew(Guid subject, FileEntry<string> fileEntry)
    {
    }

    public async IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, Folder<string> parentFolder, bool deepSearch)
    {
        var folderId = DaoSelector.ConvertId(parentFolder.Id);

        var filesDbContext = _dbContextFactory.CreateDbContext();
        var entryIDs = await filesDbContext.ThirdpartyIdMapping
                   .Where(r => r.Id.StartsWith(PathPrefix))
                   .Select(r => r.HashId)
                   .ToListAsync();

        if (!entryIDs.Any())
        {
            yield break;
        }

        var q = from r in filesDbContext.Tag
                from l in filesDbContext.TagLink.Where(a => a.TenantId == r.TenantId && a.TagId == r.Id).DefaultIfEmpty()
                where r.TenantId == TenantID && l.TenantId == TenantID && r.Type == TagType.New && entryIDs.Contains(l.EntryId)
                select new { tag = r, tagLink = l };

        if (subject != Guid.Empty)
        {
            q = q.Where(r => r.tag.Owner == subject);
        }

        var qList = await q
            .Distinct()
            .AsAsyncEnumerable()
            .ToListAsync();

        var tags = new List<Tag>();

        foreach (var r in qList)
        {
            tags.Add(new Tag
            {
                Name = r.tag.Name,
                Type = r.tag.Type,
                Owner = r.tag.Owner,
                EntryId = await MappingIDAsync(r.tagLink.EntryId),
                EntryType = r.tagLink.EntryType,
                Count = r.tagLink.Count,
                Id = r.tag.Id
            });
        }


        if (deepSearch)
        {
            foreach (var e in tags)
            {
                yield return e;
            }
            yield break;
        }

        var folderFileIds = new[] { parentFolder.Id }
            .Concat(await GetChildrenAsync(folderId));

        foreach (var e in tags.Where(tag => folderFileIds.Contains(tag.EntryId.ToString())))
        {
            yield return e;
        }
    }

    protected abstract Task<IEnumerable<string>> GetChildrenAsync(string folderId);

    #endregion

    public void Dispose()
    {
        if (ProviderInfo != null)
        {
            ProviderInfo.Dispose();
            ProviderInfo = null;
        }
    }
}

internal class ErrorEntry
{
    public string Error { get; set; }
    public string ErrorId { get; set; }

    public ErrorEntry(string error, string errorId)
    {
        Error = error;
        ErrorId = errorId;
    }
}

public class TagLink
{
    public int TenantId { get; set; }
    public int Id { get; set; }
}

public class TagLinkComparer : IEqualityComparer<TagLink>
{
    public bool Equals([AllowNull] TagLink x, [AllowNull] TagLink y)
    {
        return x.Id == y.Id && x.TenantId == y.TenantId;
    }

    public int GetHashCode([DisallowNull] TagLink obj)
    {
        return obj.Id.GetHashCode() + obj.TenantId.GetHashCode();
    }
}
