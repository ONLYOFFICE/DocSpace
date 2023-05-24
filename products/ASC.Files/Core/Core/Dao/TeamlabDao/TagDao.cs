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

namespace ASC.Files.Core.Data;

internal abstract class BaseTagDao<T> : AbstractDao
{
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);
    private readonly IMapper _mapper;

    public BaseTagDao(
        UserManager userManager,
        IDbContextFactory<FilesDbContext> dbContextManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        SetupInfo setupInfo,
        MaxTotalSizeStatistic maxTotalSizeStatistic,
        CoreBaseSettings coreBaseSettings,
        CoreConfiguration coreConfiguration,
        SettingsManager settingsManager,
        AuthContext authContext,
        IServiceProvider serviceProvider,
        ICache cache,
        IMapper mapper)
        : base(dbContextManager,
              userManager,
              tenantManager,
              tenantUtil,
              setupInfo,
              maxTotalSizeStatistic,
              coreBaseSettings,
              coreConfiguration,
              settingsManager,
              authContext,
              serviceProvider,
              cache)
    {
        _mapper = mapper;
    }

    public async IAsyncEnumerable<Tag> GetTagsAsync(Guid subject, TagType tagType, IEnumerable<FileEntry<T>> fileEntries)
    {
        var filesId = new HashSet<string>();
        var foldersId = new HashSet<string>();

        foreach (var f in fileEntries)
        {
            var idObj = f.Id is int fid ? MappingIDAsync(fid) : await MappingIDAsync(f.Id);
            var id = idObj.ToString();
            if (f.FileEntryType == FileEntryType.File)
            {
                filesId.Add(id);
            }
            else if (f.FileEntryType == FileEntryType.Folder)
            {
                foldersId.Add(id);
            }
        }

        var filesDbContext = _dbContextFactory.CreateDbContext();
        var q = Query(filesDbContext.Tag)
            .Join(filesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
            .Where(r => r.Link.TenantId == r.Tag.TenantId)
            .Where(r => r.Tag.Type == tagType)
            .Where(r => r.Link.EntryType == FileEntryType.File && filesId.Contains(r.Link.EntryId)
            || r.Link.EntryType == FileEntryType.Folder && foldersId.Contains(r.Link.EntryId));

        if (subject != Guid.Empty)
        {
            q = q.Where(r => r.Link.CreateBy == subject);
        }

        await foreach (var e in q.AsAsyncEnumerable())
        {
            yield return await ToTagAsync(e);
        }
    }

    static readonly Func<FilesDbContext, int, Guid, IEnumerable<TagType>, HashSet<string>, HashSet<string>, IAsyncEnumerable<TagLinkData>> _getTagsQuery =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((FilesDbContext ctx, int tenantId, Guid subject, IEnumerable<TagType> tagType, HashSet<string> filesId, HashSet<string> foldersId) =>
            ctx.Tag.AsNoTracking()
            .Where(r => r.TenantId == tenantId)
            .Where(r => tagType.Contains(r.Type))
            .Join(ctx.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
            .Where(r => r.Link.TenantId == r.Tag.TenantId)
            .Where(r => r.Link.EntryType == FileEntryType.File && filesId.Contains(r.Link.EntryId) || r.Link.EntryType == FileEntryType.Folder && foldersId.Contains(r.Link.EntryId))
            .Where(r => subject == Guid.Empty || r.Link.CreateBy == subject));

    public async Task<IDictionary<object, IEnumerable<Tag>>> GetTagsAsync(Guid subject, IEnumerable<TagType> tagType, IEnumerable<FileEntry<T>> fileEntries)
    {
        var filesId = new HashSet<string>();
        var foldersId = new HashSet<string>();

        foreach (var f in fileEntries)
        {
            var idObj = f.Id is int fid ? MappingIDAsync(fid) : await MappingIDAsync(f.Id);
            var id = idObj.ToString();
            if (f.FileEntryType == FileEntryType.File)
            {
                filesId.Add(id);
            }
            else if (f.FileEntryType == FileEntryType.Folder)
            {
                foldersId.Add(id);
            }
        }

        if (filesId.Any() || foldersId.Any())
        {

            using var filesDbContext = _dbContextFactory.CreateDbContext();
            var fromQuery = await FromQueryAsync(_getTagsQuery(filesDbContext, TenantID, subject, tagType, filesId, foldersId)).ToListAsync();

            return fromQuery
                .GroupBy(r => r.EntryId)
                .ToDictionary(r => r.Key, r => r.AsEnumerable());
        }

        return new Dictionary<object, IEnumerable<Tag>>();
    }

    public async Task<IDictionary<object, IEnumerable<Tag>>> GetTagsAsync(Guid subject, IEnumerable<TagType> tagType, IAsyncEnumerable<FileEntry<T>> fileEntries)
    {
        var filesId = new HashSet<string>();
        var foldersId = new HashSet<string>();

        await foreach (var f in fileEntries)
        {
            var idObj = f.Id is int fid ? MappingIDAsync(fid) : await MappingIDAsync(f.Id);
            var id = idObj.ToString();
            if (f.FileEntryType == FileEntryType.File)
            {
                filesId.Add(id);
            }
            else if (f.FileEntryType == FileEntryType.Folder)
            {
                foldersId.Add(id);
            }
        }

        if (filesId.Any() || foldersId.Any())
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            var fromQuery = await FromQueryAsync(_getTagsQuery(filesDbContext, TenantID, subject, tagType, filesId, foldersId)).ToListAsync();

            return fromQuery
                .GroupBy(r => r.EntryId)
                .ToDictionary(r => r.Key, r => r.AsEnumerable());
        }

        return new Dictionary<object, IEnumerable<Tag>>();
    }

    public IAsyncEnumerable<Tag> GetTagsAsync(TagType tagType, IEnumerable<FileEntry<T>> fileEntries)
    {
        return GetTagsAsync(Guid.Empty, tagType, fileEntries);
    }

    public async IAsyncEnumerable<Tag> GetTagsAsync(T entryID, FileEntryType entryType, TagType tagType)
    {
        var mappedId = (entryID is int fid ? MappingIDAsync(fid) : await MappingIDAsync(entryID)).ToString();

        var filesDbContext = _dbContextFactory.CreateDbContext();
        var q = Query(filesDbContext.Tag)
            .Join(filesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
            .Where(r => r.Link.TenantId == r.Tag.TenantId)
            .Where(r => r.Link.EntryType == entryType)
            .Where(r => r.Link.EntryId == mappedId)
            .Where(r => r.Tag.Type == tagType);

        await foreach (var e in q.AsAsyncEnumerable())
        {
            yield return await ToTagAsync(e);
        }
    }

    public IAsyncEnumerable<Tag> GetTagsAsync(string[] names, TagType tagType)
    {
        ArgumentNullException.ThrowIfNull(names);

        return InternalGetTagsAsync(names, tagType);
    }

    private async IAsyncEnumerable<Tag> InternalGetTagsAsync(string[] names, TagType tagType)
    {
        var filesDbContext = _dbContextFactory.CreateDbContext();
        var q = Query(filesDbContext.Tag)
            .Join(filesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
            .Where(r => r.Link.TenantId == r.Tag.TenantId)
            .Where(r => r.Tag.Owner == Guid.Empty)
            .Where(r => names.Contains(r.Tag.Name))
            .Where(r => r.Tag.Type == tagType);

        await foreach (var e in q.AsAsyncEnumerable())
        {
            yield return await ToTagAsync(e);
        }
    }

    public IAsyncEnumerable<Tag> GetTagsAsync(string name, TagType tagType)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(name);

        return GetTagsAsync(new[] { name }, tagType);
    }

    public async IAsyncEnumerable<Tag> GetTagsAsync(Guid owner, TagType tagType)
    {
        var filesDbContext = _dbContextFactory.CreateDbContext();
        var q =
            Query(filesDbContext.Tag)
            .Join(filesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
            .Where(r => r.Link.TenantId == r.Tag.TenantId)
            .Where(r => r.Tag.Type == tagType);

        if (owner != Guid.Empty)
        {
            q = q.Where(r => r.Tag.Owner == owner);
        }

        q = q.OrderByDescending(r => r.Link.CreateOn);

        await foreach (var e in q.AsAsyncEnumerable())
        {
            yield return await ToTagAsync(e);
        }
    }

    public async IAsyncEnumerable<TagInfo> GetTagsInfoAsync(string searchText, TagType tagType, bool byName, int from = 0, int count = 0)
    {
        var filesDbContext = _dbContextFactory.CreateDbContext();
        var q = Query(filesDbContext.Tag).AsNoTracking().Where(r => r.Type == tagType);

        if (byName)
        {
            q = q.Where(r => r.Name == searchText);
        }
        else if (!string.IsNullOrEmpty(searchText))
        {
            var lowerText = searchText.ToLower().Trim().Replace("%", "\\%").Replace("_", "\\_");
            q = q.Where(r => r.Name.ToLower().Contains(lowerText));
        }

        if (count != 0)
        {
            q = q.Take(count);
        }

        q = q.Skip(from);

        await foreach (var tag in q.AsAsyncEnumerable())
        {
            yield return _mapper.Map<DbFilesTag, TagInfo>(tag);
        }
    }

    public async IAsyncEnumerable<TagInfo> GetTagsInfoAsync(IEnumerable<string> names)
    {
        var filesDbContext = _dbContextFactory.CreateDbContext();
        var q = Query(filesDbContext.Tag).AsNoTracking().Where(r => names.Contains(r.Name));

        await foreach (var tag in q.AsAsyncEnumerable())
        {
            yield return _mapper.Map<DbFilesTag, TagInfo>(tag);
        }
    }

    public async Task<TagInfo> SaveTagInfoAsync(TagInfo tagInfo)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var tagDb = _mapper.Map<TagInfo, DbFilesTag>(tagInfo);
        tagDb.TenantId = TenantID;

        var tag = await filesDbContext.Tag.AddAsync(tagDb);
        await filesDbContext.SaveChangesAsync();

        return _mapper.Map<DbFilesTag, TagInfo>(tag.Entity);
    }

    public async Task<IEnumerable<Tag>> SaveTags(IEnumerable<Tag> tags, Guid createdBy = default)
    {
        var result = new List<Tag>();

        if (tags == null)
        {
            return result;
        }

        tags = tags.Where(x => x != null && !x.EntryId.Equals(null) && !x.EntryId.Equals(0)).ToArray();

        if (!tags.Any())
        {
            return result;
        }


        await _semaphore.WaitAsync();

        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            using var tx = await filesDbContext.Database.BeginTransactionAsync();
            await DeleteTagsBeforeSave();

            var createOn = _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow());
            var cacheTagId = new Dictionary<string, int>();

            foreach (var t in tags)
            {
                result.Add(await SaveTagAsync(t, cacheTagId, createOn, createdBy));
            }

            await tx.CommitAsync();
        });

        _semaphore.Release();

        return result;
    }

    public async Task<IEnumerable<Tag>> SaveTagsAsync(Tag tag)
    {
        var result = new List<Tag>();

        if (tag == null)
        {
            return result;
        }

        if (tag.EntryId.Equals(null) || tag.EntryId.Equals(0))
        {
            return result;
        }

        try
        {
            await _semaphore.WaitAsync();

            using var filesDbContext = _dbContextFactory.CreateDbContext();
            var strategy = filesDbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var filesDbContext = _dbContextFactory.CreateDbContext();
                using var tx = await filesDbContext.Database.BeginTransactionAsync();

                await DeleteTagsBeforeSave();

                var createOn = _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow());
                var cacheTagId = new Dictionary<string, int>();

                result.Add(await SaveTagAsync(tag, cacheTagId, createOn));

                await tx.CommitAsync();
            });
        }
        catch
        {
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
        return result;
    }

    private async Task DeleteTagsBeforeSave()
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var mustBeDeleted = await
            Query(filesDbContext.Tag)
            .Join(filesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
            .Where(r => r.Link.TenantId == r.Tag.TenantId)
            .Where(r => (r.Tag.Type == TagType.New || r.Tag.Type == TagType.Recent) && r.Link.CreateOn <= _tenantUtil.DateTimeNow().AddMonths(-1))
            .ToListAsync();

        foreach (var row in mustBeDeleted)
        {
            await Query(filesDbContext.TagLink)
                .Where(r => r.TagId == row.Link.TagId)
                .Where(r => r.EntryId == row.Link.EntryId)
                .Where(r => r.EntryType == row.Link.EntryType)
                .ExecuteDeleteAsync();
        }


        var tagsToRemove = from ft in filesDbContext.Tag
                           join ftl in filesDbContext.TagLink.DefaultIfEmpty() on new { ft.TenantId, ft.Id } equals new { ftl.TenantId, Id = ftl.TagId }
                           where ftl == null
                           select ft;

        await tagsToRemove.ExecuteDeleteAsync();
    }

    private async Task<Tag> SaveTagAsync(Tag t, Dictionary<string, int> cacheTagId, DateTime createOn, Guid createdBy = default)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var cacheTagIdKey = string.Join("/", new[] { TenantID.ToString(), t.Owner.ToString(), t.Name, ((int)t.Type).ToString(CultureInfo.InvariantCulture) });

        if (!cacheTagId.TryGetValue(cacheTagIdKey, out var id))
        {
            id = await filesDbContext.Tag
                .Where(r => r.Owner == t.Owner)
                .Where(r => r.Name == t.Name)
                .Where(r => r.Type == t.Type)
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            if (id == 0)
            {
                var toAdd = new DbFilesTag
                {
                    Id = 0,
                    Name = t.Name,
                    Owner = t.Owner,
                    Type = t.Type,
                    TenantId = TenantID
                };

                toAdd = filesDbContext.Tag.Add(toAdd).Entity;
                await filesDbContext.SaveChangesAsync();
                id = toAdd.Id;
            }

            cacheTagId.Add(cacheTagIdKey, id);
        }

        t.Id = id;

        var linkToInsert = new DbFilesTagLink
        {
            TenantId = TenantID,
            TagId = id,
            EntryId = (await MappingIDAsync(t.EntryId, true)).ToString(),
            EntryType = t.EntryType,
            CreateBy = createdBy != default ? createdBy : _authContext.CurrentAccount.ID,
            CreateOn = createOn,
            Count = t.Count
        };

        await filesDbContext.AddOrUpdateAsync(r => r.TagLink, linkToInsert);
        await filesDbContext.SaveChangesAsync();

        return t;
    }

    public async Task UpdateNewTags(IEnumerable<Tag> tags, Guid createdBy = default)
    {
        if (tags == null || !tags.Any())
        {
            return;
        }

        await _semaphore.WaitAsync();

        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            using var tx = await filesDbContext.Database.BeginTransactionAsync();

            var createOn = _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow());

            foreach (var tag in tags)
            {
                await UpdateNewTagsInDbAsync(tag, createOn, createdBy);
            }

            await tx.CommitAsync();
        });

        _semaphore.Release();
    }

    public async Task UpdateNewTags(Tag tag)
    {
        if (tag == null)
        {
            return;
        }

        await _semaphore.WaitAsync();

        var createOn = _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow());

        await UpdateNewTagsInDbAsync(tag, createOn);

        _semaphore.Release();
    }

    private async ValueTask UpdateNewTagsInDbAsync(Tag tag, DateTime createOn, Guid createdBy = default)
    {
        if (tag == null)
        {
            return;
        }

        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var mappedId = (await MappingIDAsync(tag.EntryId)).ToString();
        var tagId = tag.Id;
        var tagEntryType = tag.EntryType;

        await Query(filesDbContext.TagLink)
            .Where(r => r.TagId == tagId)
            .Where(r => r.EntryType == tagEntryType)
            .Where(r => r.EntryId == mappedId)
            .ExecuteUpdateAsync(f => f
            .SetProperty(p => p.CreateBy, createdBy != default ? createdBy : _authContext.CurrentAccount.ID)
            .SetProperty(p => p.CreateOn, createOn)
            .SetProperty(p => p.Count, tag.Count)
            );
        }

    public async Task RemoveTags(IEnumerable<Tag> tags)
    {
        if (tags == null || !tags.Any())
        {
            return;
        }

        await _semaphore.WaitAsync();

        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            using var tx = await filesDbContext.Database.BeginTransactionAsync();

            foreach (var t in tags)
            {
                await RemoveTagInDbAsync(t);
            }

            await tx.CommitAsync();
        });

        _semaphore.Release();
    }

    public async Task RemoveTags(Tag tag)
    {
        if (tag == null)
        {
            return;
        }

        try
        {
            await _semaphore.WaitAsync();

            using var filesDbContext = _dbContextFactory.CreateDbContext();
            var strategy = filesDbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var filesDbContext = _dbContextFactory.CreateDbContext();
                using var tx = await filesDbContext.Database.BeginTransactionAsync();
                await RemoveTagInDbAsync(tag);

                await tx.CommitAsync();
            });
        }
        catch
        {
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task RemoveTagsAsync(FileEntry<T> entry, IEnumerable<int> tagsIds)
    {
        var entryId = (entry.Id is int fid ? MappingIDAsync(fid) : await MappingIDAsync(entry.Id)).ToString();
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        await Query(filesDbContext.TagLink)
            .Where(r => tagsIds.Contains(r.TagId) && r.EntryId == entryId && r.EntryType == entry.FileEntryType)
            .ExecuteDeleteAsync();

        var any = await Query(filesDbContext.TagLink).AnyAsync(r => tagsIds.Contains(r.TagId));
        if (!any)
        {
            await Query(filesDbContext.Tag)
                .Where(r => tagsIds.Contains(r.Id))
                .ExecuteDeleteAsync();
        }
    }

    public async Task RemoveTagsAsync(IEnumerable<int> tagsIds)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var toDeleteTags = Query(filesDbContext.Tag)
                .Where(r => tagsIds.Contains(r.Id));

        filesDbContext.TagLink.RemoveRange(Query(filesDbContext.TagLink)
                .Where(r => toDeleteTags.Select(t => t.Id).Contains(r.TagId)));

        filesDbContext.Tag.RemoveRange(toDeleteTags);

        await filesDbContext.SaveChangesAsync();
    }

    public async Task<int> RemoveTagLinksAsync(T entryId, FileEntryType entryType, TagType tagType)
    {
        await using var filesDbContext = _dbContextFactory.CreateDbContext();
        var mappedId = (await MappingIDAsync(entryId)).ToString();

        return await Query(filesDbContext.TagLink)
                .Where(l => l.EntryId == mappedId && l.EntryType == entryType)
                .Join(filesDbContext.Tag, l => l.TagId, t => t.Id, (l, t) => new { l, t.Type })
                .Where(r => r.Type == tagType)
                .Select(r => r.l)
                .ExecuteDeleteAsync();

    }

    private async ValueTask RemoveTagInDbAsync(Tag tag)
    {
        if (tag == null)
        {
            return;
        }

        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var id = await Query(filesDbContext.Tag)
            .Where(r => r.Name == tag.Name &&
                        r.Owner == tag.Owner &&
                        r.Type == tag.Type)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();

        if (id != 0)
        {
            var entryId = (tag.EntryId is int fid ? MappingIDAsync(fid) : await MappingIDAsync(tag.EntryId)).ToString();

            await Query(filesDbContext.TagLink)
                .Where(r => r.TagId == id &&
                            r.EntryId == entryId &&
                            r.EntryType == tag.EntryType)
                .ExecuteDeleteAsync();

            var any = await Query(filesDbContext.TagLink).AnyAsync(r => r.TagId == id);
            if (!any)
            {
                await Query(filesDbContext.Tag)
                   .Where(r => r.Id == id)
                   .ExecuteDeleteAsync();
            }
        }
    }

    protected static readonly Func<FilesDbContext, int, Guid, List<string>, IAsyncEnumerable<TagLinkData>> _newTagsForFilesQuery =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((FilesDbContext ctx, int tenantId, Guid subject, List<string> where) =>
        ctx.Tag
        .AsNoTracking()
        .Where(r => r.TenantId == tenantId)
        .Where(r => subject == Guid.Empty || r.Owner == subject)
        .Where(r => r.Type == TagType.New)
        .Join(ctx.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData
        {
            Tag = tag,
            Link = link
        })
        .Where(r => r.Link.TenantId == r.Tag.TenantId)
        .Join(ctx.Files, r => Regex.IsMatch(r.Link.EntryId, "^[0-9]+$") ? Convert.ToInt32(r.Link.EntryId) : -1, r => r.Id, (tagLink, file) => new { tagLink, file })
        .Where(r => r.file.TenantId == r.tagLink.Link.TenantId)
        .Where(r => where.Contains(r.file.ParentId.ToString()))
        .Where(r => r.tagLink.Link.EntryType == FileEntryType.File)
        .Select(r => r.tagLink)
        .Distinct()
        );

    protected static readonly Func<FilesDbContext, int, Guid, List<string>, IAsyncEnumerable<TagLinkData>> _newTagsForFoldersQuery =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((FilesDbContext ctx, int tenantId, Guid subject, List<string> monitorFolderIdsStrings) =>
        ctx.Tag
        .AsNoTracking()
        .Where(r => r.TenantId == tenantId)
        .Where(r => subject == Guid.Empty || r.Owner == subject)
        .Where(r => r.Type == TagType.New)
        .Join(ctx.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData
        {
            Tag = tag,
            Link = link
        })
        .Where(r => r.Link.TenantId == r.Tag.TenantId)
        .Where(r => monitorFolderIdsStrings.Contains(r.Link.EntryId))
        .Where(r => r.Link.EntryType == FileEntryType.Folder)
        );

    protected static readonly Func<FilesDbContext, int, Guid, FolderType, IAsyncEnumerable<TagLinkData>> _tmpShareFileTagsQuery =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((FilesDbContext ctx, int tenantId, Guid subject, FolderType folderType) =>
        ctx.Tag
        .AsNoTracking()
        .Where(r => r.TenantId == tenantId)
        .Where(r => subject == Guid.Empty || r.Owner == subject)
        .Where(r => r.Type == TagType.New)
        .Join(ctx.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData
        {
            Tag = tag,
            Link = link
        })
        .Where(r => r.Link.TenantId == r.Tag.TenantId)
        .Where(r => ctx.Security.Any(a => a.TenantId == tenantId && a.EntryId == r.Link.EntryId && a.EntryType == r.Link.EntryType))
        .Join(ctx.Files, r => Regex.IsMatch(r.Link.EntryId, "^[0-9]+$") ? Convert.ToInt32(r.Link.EntryId) : -1, f => f.Id, (tagLink, file) => new { tagLink, file })
        .Where(r => r.file.TenantId == tenantId && r.file.CreateBy != subject && r.tagLink.Link.EntryType == FileEntryType.File)
        .Select(r => new
        {
            r.tagLink,
            root = ctx.Folders
                .Join(ctx.Tree, a => a.Id, b => b.ParentId, (folder, tree) => new { folder, tree })
                .Where(x => x.folder.TenantId == tenantId && x.tree.FolderId == r.file.ParentId)
                .OrderByDescending(r => r.tree.Level)
                .Select(r => r.folder)
                .Take(1)
                .FirstOrDefault()
        })
        .Where(r => r.root.FolderType == folderType)
        .Select(r => r.tagLink)
        .Distinct()
        );

    protected static readonly Func<FilesDbContext, int, Guid, FolderType, IAsyncEnumerable<TagLinkData>> _tmpShareFolderTagsQuery =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((FilesDbContext ctx, int tenantId, Guid subject, FolderType folderType) =>
            ctx.Tag
            .AsNoTracking()
            .Where(r => r.TenantId == tenantId)
            .Where(r => subject == Guid.Empty || r.Owner == subject)
            .Where(r => r.Type == TagType.New)
            .Join(ctx.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData
            {
                Tag = tag,
                Link = link
            })
            .Where(r => r.Link.TenantId == r.Tag.TenantId)
            .Where(r => ctx.Security.Any(a => a.TenantId == tenantId && a.EntryId == r.Link.EntryId && a.EntryType == r.Link.EntryType))
            .Join(ctx.Folders, r => Regex.IsMatch(r.Link.EntryId, "^[0-9]+$") ? Convert.ToInt32(r.Link.EntryId) : -1, f => f.Id, (tagLink, folder) => new { tagLink, folder })
                .Where(r => r.folder.TenantId == tenantId && r.folder.CreateBy != subject && r.tagLink.Link.EntryType == FileEntryType.Folder)
                .Select(r => new
                {
                    r.tagLink,
                    root = ctx.Folders
                        .Join(ctx.Tree, a => a.Id, b => b.ParentId, (folder, tree) => new { folder, tree })
                        .Where(x => x.folder.TenantId == tenantId)
                        .Where(x => x.tree.FolderId == r.folder.ParentId)
                        .OrderByDescending(r => r.tree.Level)
                        .Select(r => r.folder)
                        .Take(1)
                        .FirstOrDefault()
                })
                .Where(r => r.root.FolderType == folderType)
                .Select(r => r.tagLink)
                .Distinct()
            );

    protected static readonly Func<FilesDbContext, int, Guid, IAsyncEnumerable<TagLinkData>> _tmpShareSboxTagsQuery =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((FilesDbContext ctx, int tenantId, Guid subject) =>
            ctx.Tag
            .AsNoTracking()
            .Where(r => r.TenantId == tenantId)
            .Where(r => subject == Guid.Empty || r.Owner == subject)
            .Where(r => r.Type == TagType.New)
            .Join(ctx.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData
            {
                Tag = tag,
                Link = link
            })
            .Where(r => r.Link.TenantId == r.Tag.TenantId)
            .Where(r => ctx.Security.Any(a => a.TenantId == tenantId && a.EntryId == r.Link.EntryId && a.EntryType == r.Link.EntryType))
            .Join(ctx.ThirdpartyIdMapping, r => r.Link.EntryId, r => r.HashId, (tagLink, mapping) => new { tagLink, mapping })
            .Where(r => r.mapping.TenantId == r.tagLink.Link.TenantId)
            .Join(ctx.ThirdpartyAccount, r => r.mapping.TenantId, r => r.TenantId, (tagLinkMapping, account) => new { tagLinkMapping.tagLink, tagLinkMapping.mapping, account })
            .Where(r => r.account.UserId != subject &&
                        r.account.FolderType == FolderType.USER &&
                        Selectors.All.Any(s => r.mapping.Id.StartsWith($"{s.Id}-" + r.account.Id))
                )
                .Select(r => r.tagLink)
                .Distinct()
            );

    protected static readonly Func<FilesDbContext, int, Guid, IAsyncEnumerable<TagLinkData>> _projectsQuery =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((FilesDbContext ctx, int tenantId, Guid subject) =>
            ctx.Tag
            .AsNoTracking()
            .Where(r => r.TenantId == tenantId)
            .Where(r => subject == Guid.Empty || r.Owner == subject)
            .Where(r => r.Type == TagType.New)
            .Join(ctx.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData
            {
                Tag = tag,
                Link = link
            })
            .Where(r => r.Link.TenantId == r.Tag.TenantId)
            .Join(ctx.BunchObjects, r => r.Link.TenantId, r => r.TenantId, (tagLink, bunch) => new { tagLink, bunch })
                .Where(r => r.bunch.LeftNode == r.tagLink.Link.EntryId &&
                            r.tagLink.Link.EntryType == FileEntryType.Folder &&
                            r.bunch.RightNode.StartsWith("projects/project/"))
                .Select(r => r.tagLink)
                .Distinct()
            );

    protected static readonly Func<FilesDbContext, int, Guid, List<string>, IAsyncEnumerable<TagLinkData>> _newTagsForSBoxQuery =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((FilesDbContext ctx, int tenantId, Guid subject, List<string> thirdpartyFolderIds) =>
            ctx.Tag
            .AsNoTracking()
            .Where(r => r.TenantId == tenantId)
            .Where(r => subject == Guid.Empty || r.Owner == subject)
            .Where(r => r.Type == TagType.New)
            .Join(ctx.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData
            {
                Tag = tag,
                Link = link
            })
            .Where(r => r.Link.TenantId == r.Tag.TenantId)
            .Join(ctx.ThirdpartyIdMapping, r => r.Link.EntryId, r => r.HashId, (tagLink, mapping) => new { tagLink, mapping })
                    .Where(r => r.mapping.TenantId == tenantId &&
                                thirdpartyFolderIds.Contains(r.mapping.Id) &&
                                r.tagLink.Tag.Owner == subject &&
                                r.tagLink.Link.EntryType == FileEntryType.Folder)
                    .Select(r => r.tagLink)
                    .Distinct()
            );

    protected static readonly Func<FilesDbContext, int, Guid, IAsyncEnumerable<TagLinkData>> _newTagsThirdpartyRoomsQuery =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((FilesDbContext ctx, int tenantId, Guid subject) =>
            ctx.Tag
            .AsNoTracking()
            .Where(r => r.TenantId == tenantId)
            .Where(r => subject == Guid.Empty || r.Owner == subject)
            .Where(r => r.Type == TagType.New)
            .Join(ctx.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData
            {
                Tag = tag,
                Link = link
            })
            .Where(r => r.Link.TenantId == r.Tag.TenantId)
            .Join(ctx.ThirdpartyIdMapping, r => r.Link.EntryId, r => r.HashId, (tagLink, mapping) => new { tagLink, mapping })
            .Where(r => r.mapping.TenantId == tenantId && r.tagLink.Tag.Owner == subject && r.tagLink.Link.EntryType == FileEntryType.Folder)
            .Join(ctx.ThirdpartyAccount, r => r.mapping.Id, r => r.FolderId, (tagLinkData, account) => new { tagLinkData, account })
            .Where(r => r.tagLinkData.mapping.Id == r.account.FolderId && r.account.FolderType == FolderType.VirtualRooms)
            .Select(r => r.tagLinkData.tagLink).Distinct()
            );

    protected static readonly Func<FilesDbContext, List<int>, bool, IAsyncEnumerable<int>> _getFolderQuery = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((FilesDbContext ctx, List<int> monitorFolderIdsInt, bool deepSearch) =>
          ctx.Tree
              .AsNoTracking()
              .Where(r => monitorFolderIdsInt.Contains(r.ParentId))
              .Where(r => deepSearch || r.Level == 1)
              .Select(r => r.FolderId));

    protected static readonly Func<FilesDbContext, int, FolderType, Guid, IAsyncEnumerable<int>> _getThirdpartyAccountQuery = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((FilesDbContext ctx, int tenantId, FolderType folderType, Guid subject) =>
          ctx.ThirdpartyAccount
                .Where(r => r.TenantId == tenantId)
                .Where(r => r.FolderType == folderType)
                .Where(r => folderType != FolderType.USER || r.UserId == subject)
                .Select(r => r.Id));

    public IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, FileEntry<T> fileEntry)
    {
        return GetNewTagsAsync(subject, new List<FileEntry<T>>(1) { fileEntry });
    }

    public async IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, IEnumerable<FileEntry<T>> fileEntries)
    {
        var filesDbContext = _dbContextFactory.CreateDbContext();

        var tags = new List<DbFilesTagLink>();
        var entryIds = new HashSet<string>();
        var entryTypes = new HashSet<int>();

        foreach (var r in fileEntries)
        {
            var idObj = r.Id is int fid ? MappingIDAsync(fid) : await MappingIDAsync(r.Id);
            var id = idObj.ToString();
            var entryType = (r.FileEntryType == FileEntryType.File) ? FileEntryType.File : FileEntryType.Folder;

            tags.Add(new DbFilesTagLink
            {
                TenantId = TenantID,
                EntryId = id,
                EntryType = entryType
            });

            entryIds.Add(id);
            entryTypes.Add((int)entryType);
        }

        if (entryIds.Count > 0)
        {
            var sqlQuery = Query(filesDbContext.Tag)
                .Join(filesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
                .Where(r => r.Link.TenantId == r.Tag.TenantId)
                .Where(r => r.Tag.Type == TagType.New)
                .Where(x => x.Link.EntryId != null)
                //.Where(r => tags.Any(t => t.TenantId == r.Link.TenantId && t.EntryId == r.Link.EntryId && t.EntryType == (int)r.Link.EntryType)); ;
                .Where(r => entryIds.Contains(r.Link.EntryId) && entryTypes.Contains((int)r.Link.EntryType));

            if (subject != Guid.Empty)
            {
                sqlQuery = sqlQuery.Where(r => r.Tag.Owner == subject);
            }

            await foreach (var e in sqlQuery.AsAsyncEnumerable())
            {
                yield return await ToTagAsync(e);
            }
        }

        yield break;
    }

    protected async IAsyncEnumerable<Tag> FromQueryAsync(IAsyncEnumerable<TagLinkData> dbFilesTags)
    {
        await foreach (var file in dbFilesTags)
        {
            yield return await ToTagAsync(file);
        }
    }

    protected async ValueTask<Tag> ToTagAsync(TagLinkData r)
    {
        var result = _mapper.Map<DbFilesTag, Tag>(r.Tag);
        _mapper.Map(r.Link, result);

        result.EntryId = await MappingIDAsync(r.Link.EntryId);

        return result;
    }
}


[Scope]
internal class TagDao : BaseTagDao<int>, ITagDao<int>
{
    public TagDao(
        UserManager userManager,
        IDbContextFactory<FilesDbContext> dbContextManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        SetupInfo setupInfo,
        MaxTotalSizeStatistic maxTotalSizeStatistic,
        CoreBaseSettings coreBaseSettings,
        CoreConfiguration coreConfiguration,
        SettingsManager settingsManager,
        AuthContext authContext,
        IServiceProvider serviceProvider,
        ICache cache,
        IMapper mapper)
        : base(userManager,
              dbContextManager,
              tenantManager,
              tenantUtil,
              setupInfo,
              maxTotalSizeStatistic,
              coreBaseSettings,
              coreConfiguration,
              settingsManager,
              authContext,
              serviceProvider,
              cache,
              mapper)
    {
    }

    public IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, Folder<int> parentFolder, bool deepSearch)
    {
        if (parentFolder == null || EqualityComparer<int>.Default.Equals(parentFolder.Id, 0))
        {
            throw new ArgumentException("folderId");
        }

        return InternalGetNewTagsAsync(subject, parentFolder, deepSearch);
    }

    private async IAsyncEnumerable<Tag> InternalGetNewTagsAsync(Guid subject, Folder<int> parentFolder, bool deepSearch)
    {
        var filesDbContext = _dbContextFactory.CreateDbContext();

        var monitorFolderIds = new List<object> { parentFolder.Id };

        var tenantId = TenantID;

        var tempTags = AsyncEnumerable.Empty<TagLinkData>();

        if (parentFolder.FolderType == FolderType.SHARE)
        {
            tempTags = tempTags.Concat(_tmpShareFileTagsQuery(filesDbContext, tenantId, subject, FolderType.USER));
            tempTags = tempTags.Concat(_tmpShareFolderTagsQuery(filesDbContext, tenantId, subject, FolderType.USER));
            tempTags = tempTags.Concat(_tmpShareSboxTagsQuery(filesDbContext, tenantId, subject));
        }
        else if (parentFolder.FolderType == FolderType.Privacy)
        {
            tempTags = tempTags.Concat(_tmpShareFileTagsQuery(filesDbContext, tenantId, subject, FolderType.Privacy));
            tempTags = tempTags.Concat(_tmpShareFolderTagsQuery(filesDbContext, tenantId, subject, FolderType.Privacy));
        }
        else if (parentFolder.FolderType == FolderType.Projects)
        {
            tempTags = tempTags.Concat(_projectsQuery(filesDbContext, tenantId, subject));
        }

        if (!deepSearch && parentFolder.RootFolderType != FolderType.VirtualRooms)
        {
            await foreach (var e in tempTags)
            {
                yield return await ToTagAsync(e);
            }

            yield break;
        }

        await foreach (var e in tempTags)
        {
            var tag = await ToTagAsync(e);
            yield return tag;

            if (tag.EntryType == FileEntryType.Folder)
            {
                monitorFolderIds.Add(tag.EntryId);
            }
        }


        var monitorFolderIdsInt = monitorFolderIds.OfType<int>().ToList();
        var subFoldersSqlQuery = _getFolderQuery(filesDbContext, monitorFolderIdsInt, deepSearch);

        monitorFolderIds.AddRange(await subFoldersSqlQuery.Select(r => (object)r).ToListAsync());

        var monitorFolderIdsStrings = monitorFolderIds.Select(r => r.ToString()).ToList();

        var result = AsyncEnumerable.Empty<TagLinkData>();
        result = result.Concat(_newTagsForFoldersQuery(filesDbContext, tenantId, subject, monitorFolderIdsStrings));

        var where = (deepSearch ? monitorFolderIds : new List<object> { parentFolder.Id })
            .Select(r => r.ToString())
            .ToList();

        result = result.Concat(_newTagsForFilesQuery(filesDbContext, tenantId, subject, where));

        if (parentFolder.FolderType == FolderType.USER || parentFolder.FolderType == FolderType.COMMON)
        {
            var folderIds = await _getThirdpartyAccountQuery(filesDbContext, tenantId, parentFolder.FolderType, subject).ToListAsync();

            var thirdpartyFolderIds = folderIds.ConvertAll(r => $"{Selectors.SharpBox.Id}-" + r)
                                                .Concat(folderIds.ConvertAll(r => $"{Selectors.Box.Id}-{r}"))
                                                .Concat(folderIds.ConvertAll(r => $"{Selectors.Dropbox.Id}-{r}"))
                                                .Concat(folderIds.ConvertAll(r => $"{Selectors.SharePoint.Id}-{r}"))
                                                .Concat(folderIds.ConvertAll(r => $"{Selectors.GoogleDrive.Id}-{r}"))
                                                .Concat(folderIds.ConvertAll(r => $"{Selectors.OneDrive.Id}-{r}"))
                                                .ToList();

            if (thirdpartyFolderIds.Count > 0)
            {
                result = result.Concat(_newTagsForSBoxQuery(filesDbContext, tenantId, subject, thirdpartyFolderIds));
            }
        }

        if (parentFolder.FolderType == FolderType.VirtualRooms)
        {
            result = result.Concat(_newTagsThirdpartyRoomsQuery(filesDbContext, tenantId, subject));
        }

        await foreach (var e in result)
        {
            yield return await ToTagAsync(e);
        }
    }
}

[Scope]
internal class ThirdPartyTagDao : BaseTagDao<string>, ITagDao<string>
{
    private readonly IThirdPartyTagDao _thirdPartyTagDao;
    public ThirdPartyTagDao(
        UserManager userManager,
        IDbContextFactory<FilesDbContext> dbContextManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        SetupInfo setupInfo,
        MaxTotalSizeStatistic maxTotalSizeStatistic,
        CoreBaseSettings coreBaseSettings,
        CoreConfiguration coreConfiguration,
        SettingsManager settingsManager,
        AuthContext authContext,
        IServiceProvider serviceProvider,
        ICache cache,
        IMapper mapper,
        IThirdPartyTagDao thirdPartyTagDao)
        : base(userManager,
              dbContextManager,
              tenantManager,
              tenantUtil,
              setupInfo,
              maxTotalSizeStatistic,
              coreBaseSettings,
              coreConfiguration,
              settingsManager,
              authContext,
              serviceProvider,
              cache,
              mapper)
    {
        _thirdPartyTagDao = thirdPartyTagDao;
    }

    public IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, Folder<string> parentFolder, bool deepSearch)
    {
        return _thirdPartyTagDao.GetNewTagsAsync(subject, parentFolder, deepSearch);
    }
}

public class TagLinkData
{
    public DbFilesTag Tag { get; set; }
    public DbFilesTagLink Link { get; set; }
}
