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

[Scope]
internal class TagDao<T> : AbstractDao, ITagDao<T>
{
    private static readonly object _syncRoot = new object();
    private readonly IMapper _mapper;

    public TagDao(
        UserManager userManager,
        DbContextManager<FilesDbContext> dbContextManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        SetupInfo setupInfo,
        TenantExtra tenantExtra,
        TenantStatisticsProvider tenantStatisticProvider,
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
              tenantExtra,
              tenantStatisticProvider,
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
            var idObj = await MappingIDAsync(f.Id).ConfigureAwait(false);
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

        var q = Query(FilesDbContext.Tag)
            .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
            .Where(r => r.Link.TenantId == r.Tag.TenantId)
            .Where(r => r.Tag.Type == tagType)
            .Where(r => r.Link.EntryType == FileEntryType.File && filesId.Contains(r.Link.EntryId)
            || r.Link.EntryType == FileEntryType.Folder && foldersId.Contains(r.Link.EntryId));

        if (subject != Guid.Empty)
        {
            q = q.Where(r => r.Link.CreateBy == subject);
        }

        await foreach (var e in FromQueryAsync(q).ConfigureAwait(false))
        {
            yield return e;
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
            var idObj = await MappingIDAsync(f.Id).ConfigureAwait(false);
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

        if (fileEntries.Any())
        {
            var fromQuery = await FromQueryAsync(_getTagsQuery(FilesDbContext, TenantID, subject, tagType, filesId, foldersId))
                .ToListAsync()
                .ConfigureAwait(false);

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
        var mappedId = (await MappingIDAsync(entryID)).ToString();
        var q = Query(FilesDbContext.Tag)
            .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
            .Where(r => r.Link.TenantId == r.Tag.TenantId)
            .Where(r => r.Link.EntryType == entryType)
            .Where(r => r.Link.EntryId == mappedId)
            .Where(r => r.Tag.Type == tagType);

        await foreach (var e in FromQueryAsync(q).ConfigureAwait(false))
        {
            yield return e;
        }
    }

    public IAsyncEnumerable<Tag> GetTagsAsync(string[] names, TagType tagType)
    {
        ArgumentNullException.ThrowIfNull(names);

        return InternalGetTagsAsync(names, tagType);
    }

    public async IAsyncEnumerable<Tag> InternalGetTagsAsync(string[] names, TagType tagType)
    {
        var q = Query(FilesDbContext.Tag)
            .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
            .Where(r => r.Link.TenantId == r.Tag.TenantId)
            .Where(r => r.Tag.Owner == Guid.Empty)
            .Where(r => names.Contains(r.Tag.Name))
            .Where(r => r.Tag.Type == tagType);

        await foreach (var e in FromQueryAsync(q).ConfigureAwait(false))
        {
            yield return e;
        }
    }

    public IAsyncEnumerable<Tag> GetTagsAsync(string name, TagType tagType)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(name);

        return GetTagsAsync(new[] { name }, tagType);
    }

    public async IAsyncEnumerable<Tag> GetTagsAsync(Guid owner, TagType tagType)
    {
        var q =
            Query(FilesDbContext.Tag)
            .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
            .Where(r => r.Link.TenantId == r.Tag.TenantId)
            .Where(r => r.Tag.Type == tagType);

        if (owner != Guid.Empty)
        {
            q = q.Where(r => r.Tag.Owner == owner);
        }

        q = q.OrderByDescending(r => r.Link.CreateOn);

        await foreach (var e in FromQueryAsync(q).ConfigureAwait(false))
        {
            yield return e;
        }
    }

    public IEnumerable<Tag> SaveTags(IEnumerable<Tag> tags)
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

        lock (_syncRoot)
        {
            var strategy = FilesDbContext.Database.CreateExecutionStrategy();

            strategy.Execute(() =>
            {
                using var tx = FilesDbContext.Database.BeginTransaction();
                DeleteTagsBeforeSave();

                var createOn = _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow());
                var cacheTagId = new Dictionary<string, int>();

                result.AddRange(tags.Select(t => SaveTagAsync(t, cacheTagId, createOn).Result));

                tx.Commit();
            });
        }

        return result;
    }

    public IEnumerable<Tag> SaveTags(Tag tag)
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

        lock (_syncRoot)
        {
            var strategy = FilesDbContext.Database.CreateExecutionStrategy();

            strategy.Execute(() =>
            {
                using var tx = FilesDbContext.Database.BeginTransaction();
                DeleteTagsBeforeSave();

                var createOn = _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow());
                var cacheTagId = new Dictionary<string, int>();

                result.Add(SaveTagAsync(tag, cacheTagId, createOn).Result);

                tx.Commit();
            });
        }

        return result;
    }

    private void DeleteTagsBeforeSave()
    {
        var mustBeDeleted =
            Query(FilesDbContext.Tag)
            .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
            .Where(r => r.Link.TenantId == r.Tag.TenantId)
            .Where(r => (r.Tag.Type == TagType.New || r.Tag.Type == TagType.Recent) && r.Link.CreateOn <= _tenantUtil.DateTimeNow().AddMonths(-1))
            .ToList();

        foreach (var row in mustBeDeleted)
        {
            var linksToRemove = Query(FilesDbContext.TagLink)
                .Where(r => r.TagId == row.Link.TagId)
                .Where(r => r.EntryId == row.Link.EntryId)
                .Where(r => r.EntryType == row.Link.EntryType)
                .ToList();
            FilesDbContext.TagLink.RemoveRange(linksToRemove);
        }

        FilesDbContext.SaveChanges();

        var tagsToRemove = from ft in FilesDbContext.Tag
                           join ftl in FilesDbContext.TagLink.DefaultIfEmpty() on new { TenantId = ft.TenantId, Id = ft.Id } equals new { TenantId = ftl.TenantId, Id = ftl.TagId }
                           where ftl == null
                           select ft;

        FilesDbContext.Tag.RemoveRange(tagsToRemove.ToList());
        FilesDbContext.SaveChanges();
    }

    private async Task<Tag> SaveTagAsync(Tag t, Dictionary<string, int> cacheTagId, DateTime createOn)
    {
        var cacheTagIdKey = string.Join("/", new[] { TenantID.ToString(), t.Owner.ToString(), t.Name, ((int)t.Type).ToString(CultureInfo.InvariantCulture) });

        if (!cacheTagId.TryGetValue(cacheTagIdKey, out var id))
        {
            id = await FilesDbContext.Tag
                .AsQueryable()
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

                toAdd = FilesDbContext.Tag.Add(toAdd).Entity;
                await FilesDbContext.SaveChangesAsync();
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
            CreateBy = _authContext.CurrentAccount.ID,
            CreateOn = createOn,
            Count = t.Count
        };

        await FilesDbContext.AddOrUpdateAsync(r => r.TagLink, linkToInsert);
        await FilesDbContext.SaveChangesAsync();

        return t;
    }

    public void UpdateNewTags(IEnumerable<Tag> tags)
    {
        if (tags == null || !tags.Any())
        {
            return;
        }

        lock (_syncRoot)
        {
            var strategy = FilesDbContext.Database.CreateExecutionStrategy();

            strategy.Execute(() =>
            {
                using var tx = FilesDbContext.Database.BeginTransaction();
                var createOn = _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow());

                foreach (var tag in tags)
                {
                    UpdateNewTagsInDbAsync(tag, createOn).Wait();
                }

                tx.Commit();
            });
        }
    }

    public void UpdateNewTags(Tag tag)
    {
        if (tag == null)
        {
            return;
        }

        lock (_syncRoot)
        {
            var createOn = _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow());

            UpdateNewTagsInDbAsync(tag, createOn).Wait();
        }
    }

    private Task UpdateNewTagsInDbAsync(Tag tag, DateTime createOn)
    {
        if (tag == null)
        {
            return Task.CompletedTask;
        }

        return InternalUpdateNewTagsInDbAsync(tag, createOn);
    }

    private async Task InternalUpdateNewTagsInDbAsync(Tag tag, DateTime createOn)
    {
        var mappedId = (await MappingIDAsync(tag.EntryId)).ToString();
        var forUpdate = Query(FilesDbContext.TagLink)
            .Where(r => r.TagId == tag.Id)
            .Where(r => r.EntryType == tag.EntryType)
            .Where(r => r.EntryId == mappedId);

        foreach (var f in forUpdate)
        {
            f.CreateBy = _authContext.CurrentAccount.ID;
            f.CreateOn = createOn;
            f.Count = tag.Count;
        }

        await FilesDbContext.SaveChangesAsync();
    }

    public void RemoveTags(IEnumerable<Tag> tags)
    {
        if (tags == null || !tags.Any())
        {
            return;
        }

        lock (_syncRoot)
        {
            var strategy = FilesDbContext.Database.CreateExecutionStrategy();

            strategy.Execute(() =>
            {
                using var tx = FilesDbContext.Database.BeginTransaction();

                foreach (var t in tags)
                {
                    RemoveTagInDbAsync(t).Wait();
                }

                tx.Commit();
            });
        }
    }

    public void RemoveTags(Tag tag)
    {
        if (tag == null)
        {
            return;
        }

        lock (_syncRoot)
        {
            var strategy = FilesDbContext.Database.CreateExecutionStrategy();

            strategy.Execute(() =>
            {
                using var tx = FilesDbContext.Database.BeginTransaction();
                RemoveTagInDbAsync(tag).Wait();

                tx.Commit();

            });
        }
    }

    private Task RemoveTagInDbAsync(Tag tag)
    {
        if (tag == null)
        {
            return Task.CompletedTask;
        }

        return InternalRemoveTagInDbAsync(tag);
    }

    private async Task InternalRemoveTagInDbAsync(Tag tag)
    {
        var id = await Query(FilesDbContext.Tag)
            .Where(r => r.Name == tag.Name &&
                        r.Owner == tag.Owner &&
                        r.Type == tag.Type)
            .Select(r => r.Id)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (id != 0)
        {
            var entryId = (await MappingIDAsync(tag.EntryId).ConfigureAwait(false)).ToString();
            var toDelete = await Query(FilesDbContext.TagLink)
                .Where(r => r.TagId == id &&
                            r.EntryId == entryId &&
                            r.EntryType == tag.EntryType)
                .ToListAsync();

            FilesDbContext.TagLink.RemoveRange(toDelete);
            await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);

            var any = await Query(FilesDbContext.TagLink).AnyAsync(r => r.TagId == id).ConfigureAwait(false);
            if (!any)
            {
                var tagToDelete = await Query(FilesDbContext.Tag).Where(r => r.Id == id).ToListAsync();
                FilesDbContext.Tag.RemoveRange(tagToDelete);
                await FilesDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }

    static readonly Func<FilesDbContext, int, Guid, List<string>, IAsyncEnumerable<TagLinkData>> _newTagsForFilesQuery =
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

    static readonly Func<FilesDbContext, int, Guid, List<string>, IAsyncEnumerable<TagLinkData>> _newTagsForFoldersQuery =
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

    static readonly Func<FilesDbContext, int, Guid, FolderType, IAsyncEnumerable<TagLinkData>> _tmpShareFileTagsQuery =
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

    static readonly Func<FilesDbContext, int, Guid, FolderType, IAsyncEnumerable<TagLinkData>> _tmpShareFolderTagsQuery =
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

    static readonly Func<FilesDbContext, int, Guid, IAsyncEnumerable<TagLinkData>> _tmpShareSboxTagsQuery =
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
                        (r.mapping.Id.StartsWith("sbox-" + r.account.Id) ||
                        r.mapping.Id.StartsWith("box-" + r.account.Id) ||
                        r.mapping.Id.StartsWith("dropbox-" + r.account.Id) ||
                        r.mapping.Id.StartsWith("spoint-" + r.account.Id) ||
                        r.mapping.Id.StartsWith("drive-" + r.account.Id) ||
                        r.mapping.Id.StartsWith("onedrive-" + r.account.Id))
                )
                .Select(r => r.tagLink)
                .Distinct()
            );

    static readonly Func<FilesDbContext, int, Guid, IAsyncEnumerable<TagLinkData>> _projectsQuery =
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

    static readonly Func<FilesDbContext, int, Guid, List<string>, IAsyncEnumerable<TagLinkData>> _newTagsForSBoxQuery =
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

    static readonly Func<FilesDbContext, List<int>, bool, IAsyncEnumerable<int>> _getFolderQuery = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((FilesDbContext ctx, List<int> monitorFolderIdsInt, bool deepSearch) =>
          ctx.Tree
              .AsNoTracking()
              .Where(r => monitorFolderIdsInt.Contains(r.ParentId))
              .Where(r => deepSearch || r.Level == 1)
              .Select(r => r.FolderId));

    static readonly Func<FilesDbContext, int, FolderType, Guid, IAsyncEnumerable<int>> _getThirdpartyAccountQuery = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((FilesDbContext ctx, int tenantId, FolderType folderType, Guid subject) =>
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
        var tags = new List<DbFilesTagLink>();
        var entryIds = new HashSet<string>();
        var entryTypes = new HashSet<int>();

        foreach (var r in fileEntries)
        {
            var idObj = await MappingIDAsync(r.Id).ConfigureAwait(false);
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
            var sqlQuery = Query(FilesDbContext.Tag)
                .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
                .Where(r => r.Link.TenantId == r.Tag.TenantId)
                .Where(r => r.Tag.Type == TagType.New)
                .Where(x => x.Link.EntryId != null)
                //.Where(r => tags.Any(t => t.TenantId == r.Link.TenantId && t.EntryId == r.Link.EntryId && t.EntryType == (int)r.Link.EntryType)); ;
                .Where(r => entryIds.Contains(r.Link.EntryId) && entryTypes.Contains((int)r.Link.EntryType));

            if (subject != Guid.Empty)
            {
                sqlQuery = sqlQuery.Where(r => r.Tag.Owner == subject);
            }

            await foreach (var e in FromQueryAsync(sqlQuery).ConfigureAwait(false))
            {
                yield return e;
            }
        }

        yield break;
    }

    public IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, Folder<T> parentFolder, bool deepSearch)
    {
        if (parentFolder == null || EqualityComparer<T>.Default.Equals(parentFolder.Id, default(T)))
        {
            throw new ArgumentException("folderId");
        }

        return InternalGetNewTagsAsync(subject, parentFolder, deepSearch);
    }

    private async IAsyncEnumerable<Tag> InternalGetNewTagsAsync(Guid subject, Folder<T> parentFolder, bool deepSearch)
    {
        var result = AsyncEnumerable.Empty<Tag>();

        var monitorFolderIds = new object[] { parentFolder.Id }.ToAsyncEnumerable();

        var tenantId = TenantID;

        var tempTags = AsyncEnumerable.Empty<Tag>();

        if (parentFolder.FolderType == FolderType.SHARE)
        {
            tempTags = tempTags.Concat(FromQueryAsync(_tmpShareFileTagsQuery(FilesDbContext, tenantId, subject, FolderType.USER)));
            tempTags = tempTags.Concat(FromQueryAsync(_tmpShareFolderTagsQuery(FilesDbContext, tenantId, subject, FolderType.USER)));
            tempTags = tempTags.Concat(FromQueryAsync(_tmpShareSboxTagsQuery(FilesDbContext, tenantId, subject)));
        }
        else if (parentFolder.FolderType == FolderType.Privacy)
        {
            tempTags = tempTags.Concat(FromQueryAsync(_tmpShareFileTagsQuery(FilesDbContext, tenantId, subject, FolderType.Privacy)));
            tempTags = tempTags.Concat(FromQueryAsync(_tmpShareFolderTagsQuery(FilesDbContext, tenantId, subject, FolderType.Privacy)));
        }
        else if (parentFolder.FolderType == FolderType.Projects)
        {
            tempTags = tempTags.Concat(FromQueryAsync(_projectsQuery(FilesDbContext, tenantId, subject)));
        }

        if (await tempTags.AnyAsync().ConfigureAwait(false))
        {
            if (!deepSearch)
            {
                await foreach (var e in tempTags)
                {
                    yield return e;
                }

                yield break;
            }

            monitorFolderIds = monitorFolderIds.Concat(tempTags.Where(x => x.EntryType == FileEntryType.Folder).Select(x => x.EntryId));
            result = result.Concat(tempTags);
        }

        var monitorFolderIdsInt = await monitorFolderIds.OfType<int>().ToListAsync();
        var subFoldersSqlQuery = _getFolderQuery(FilesDbContext, monitorFolderIdsInt, deepSearch);

        monitorFolderIds = monitorFolderIds.Concat(subFoldersSqlQuery.Select(r => (object)r));

        var monitorFolderIdsStrings = await monitorFolderIds.Select(r => r.ToString()).ToListAsync();

        result = result.Concat(FromQueryAsync(_newTagsForFoldersQuery(FilesDbContext, tenantId, subject, monitorFolderIdsStrings)));

        var where = (deepSearch ? await monitorFolderIds.ToArrayAsync().ConfigureAwait(false) : new object[] { parentFolder.Id })
            .Select(r => r.ToString())
            .ToList();

        result = result.Concat(FromQueryAsync(_newTagsForFilesQuery(FilesDbContext, tenantId, subject, where)));

        if (parentFolder.FolderType == FolderType.USER || parentFolder.FolderType == FolderType.COMMON)
        {
            var folderIds = await _getThirdpartyAccountQuery(FilesDbContext, tenantId, parentFolder.FolderType, subject).ToListAsync().ConfigureAwait(false);

            var thirdpartyFolderIds = folderIds.ConvertAll(r => "sbox-" + r)
                                                .Concat(folderIds.ConvertAll(r => $"box-{r}"))
                                                .Concat(folderIds.ConvertAll(r => $"dropbox-{r}"))
                                                .Concat(folderIds.ConvertAll(r => $"spoint-{r}"))
                                                .Concat(folderIds.ConvertAll(r => $"drive-{r}"))
                                                .Concat(folderIds.ConvertAll(r => $"onedrive-{r}"))
                                                .ToList();

            if (thirdpartyFolderIds.Count > 0)
            {
                result = result.Concat(FromQueryAsync(_newTagsForSBoxQuery(FilesDbContext, tenantId, subject, thirdpartyFolderIds)));
            }
        }

        await foreach (var e in result)
        {
            yield return e;
        }
    }

    protected async IAsyncEnumerable<Tag> FromQueryAsync(IQueryable<TagLinkData> dbFilesTags)
    {
        var files = await dbFilesTags
            .ToListAsync()
            .ConfigureAwait(false);

        foreach (var file in files)
        {
            yield return await ToTagAsync(file).ConfigureAwait(false);
        }
    }

    protected async IAsyncEnumerable<Tag> FromQueryAsync(IAsyncEnumerable<TagLinkData> dbFilesTags)
    {
        var files = await dbFilesTags
            .ToListAsync()
            .ConfigureAwait(false);

        foreach (var file in files)
        {
            yield return await ToTagAsync(file).ConfigureAwait(false);
        }
    }

    private async ValueTask<Tag> ToTagAsync(TagLinkData r)
    {
        var result = _mapper.Map<DbFilesTag, Tag>(r.Tag);
        _mapper.Map(r.Link, result);

        result.EntryId = await MappingIDAsync(r.Link.EntryId).ConfigureAwait(false);

        return result;
    }
}

public class TagLinkData
{
    public DbFilesTag Tag { get; set; }
    public DbFilesTagLink Link { get; set; }
}
