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
internal class SecurityDao<T> : AbstractDao, ISecurityDao<T>
{
    private readonly IMapper _mapper;

    public SecurityDao(
        UserManager userManager,
        IDbContextFactory<FilesDbContext> dbContextFactory,
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
        : base(dbContextFactory,
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

    public async Task DeleteShareRecordsAsync(IEnumerable<FileShareRecord> records)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            using var tx = await filesDbContext.Database.BeginTransactionAsync();

            foreach (var record in records)
            {
                var query = await filesDbContext.Security
                .Where(r => r.TenantId == record.TenantId)
                .Where(r => r.EntryType == record.EntryType)
                .Where(r => r.Subject == record.Subject)
                .AsAsyncEnumerable()
                .WhereAwait(async r => r.EntryId == (await MappingIDAsync(record.EntryId)).ToString())
                .ToListAsync();

                filesDbContext.RemoveRange(query);
                await filesDbContext.SaveChangesAsync();
            }

            await tx.CommitAsync();
        });
    }

    public async Task<bool> IsSharedAsync(T entryId, FileEntryType type)
    {
        var mappedId = (await MappingIDAsync(entryId)).ToString();
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        return await Query(filesDbContext.Security)
            .AnyAsync(r => r.EntryId == mappedId && r.EntryType == type && !(new[] { FileConstant.DenyDownloadId, FileConstant.DenySharingId }).Contains(r.Subject));
    }

    public async Task SetShareAsync(FileShareRecord r)
    {
        if (r.Share == FileShare.None)
        {
            var entryId = (await MappingIDAsync(r.EntryId) ?? "").ToString();
            if (string.IsNullOrEmpty(entryId))
            {
                return;
            }
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            var strategy = filesDbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
             {
                 using var filesDbContext = _dbContextFactory.CreateDbContext();
                 using var tx = await filesDbContext.Database.BeginTransactionAsync();
                 var files = new List<string>();

                 if (r.EntryType == FileEntryType.Folder)
                 {
                     var folders = new List<string>();
                     if (int.TryParse(entryId, out var intEntryId))
                     {
                         var foldersInt = await filesDbContext.Tree
                             .Where(r => r.ParentId.ToString() == entryId)
                             .Select(r => r.FolderId)
                             .ToListAsync();

                         folders.AddRange(foldersInt.Select(folderInt => folderInt.ToString()));
                         files.AddRange(await Query(filesDbContext.Files).Where(r => foldersInt.Contains(r.ParentId)).Select(r => r.Id.ToString()).ToListAsync());
                     }
                     else
                     {
                         folders.Add(entryId);
                     }

                     await filesDbContext.Security
                         .Where(a => a.TenantId == r.TenantId &&
                                     folders.Contains(a.EntryId) &&
                                     a.EntryType == FileEntryType.Folder &&
                                     a.Subject == r.Subject)
                         .ExecuteDeleteAsync();
                 }
                 else
                 {
                     files.Add(entryId);
                 }

                 if (files.Count > 0)
                 {
                     await filesDbContext.Security
                         .Where(a => a.TenantId == r.TenantId &&
                                     files.Contains(a.EntryId) &&
                                     a.EntryType == FileEntryType.File &&
                                     a.Subject == r.Subject)
                         .ExecuteDeleteAsync();
                 }

                 await tx.CommitAsync();

             });
        }
        else
        {
            var toInsert = _mapper.Map<FileShareRecord, DbFilesSecurity>(r);
            toInsert.EntryId = (await MappingIDAsync(r.EntryId, true)).ToString();

            using var filesDbContext = _dbContextFactory.CreateDbContext();
            await filesDbContext.AddOrUpdateAsync(r => r.Security, toInsert);
            await filesDbContext.SaveChangesAsync();
        }
    }

    public async IAsyncEnumerable<FileShareRecord> GetShareForEntryIdsAsync(Guid subject, IEnumerable<string> roomIds)
    {
        var filesDbContext = _dbContextFactory.CreateDbContext();
        var q = GetQuery(filesDbContext,
            r => (r.Subject == subject || r.Owner == subject)
            && roomIds.Contains(r.EntryId));

        await foreach (var e in q.AsAsyncEnumerable())
        {
            yield return await ToFileShareRecordAsync(e);
        }
    }

    public async IAsyncEnumerable<FileShareRecord> GetSharesAsync(IEnumerable<Guid> subjects)
    {
        var filesDbContext = _dbContextFactory.CreateDbContext();
        var q = GetQuery(filesDbContext, r => subjects.Contains(r.Subject));

        await foreach (var e in q.AsAsyncEnumerable())
        {
            yield return await ToFileShareRecordAsync(e);
        }
    }

    public IAsyncEnumerable<FileShareRecord> GetPureShareRecordsAsync(IEnumerable<FileEntry<T>> entries)
    {
        if (entries == null)
        {
            return AsyncEnumerable.Empty<FileShareRecord>();
        }

        return InternalGetPureShareRecordsAsync(entries);
    }

    private async IAsyncEnumerable<FileShareRecord> InternalGetPureShareRecordsAsync(IEnumerable<FileEntry<T>> entries)
    {
        var files = new List<string>();
        var folders = new List<string>();

        foreach (var entry in entries)
        {
            await SelectFilesAndFoldersForShareAsync(entry, files, folders, null);
        }

        await foreach (var e in GetPureShareRecordsDbAsync(files, folders))
        {
            yield return e;
        }
    }

    public IAsyncEnumerable<FileShareRecord> GetPureShareRecordsAsync(FileEntry<T> entry)
    {
        if (entry == null)
        {
            return AsyncEnumerable.Empty<FileShareRecord>();
        }

        return InternalGetPureShareRecordsAsync(entry);
    }

    private async IAsyncEnumerable<FileShareRecord> InternalGetPureShareRecordsAsync(FileEntry<T> entry)
    {
        var files = new List<string>();
        var folders = new List<string>();

        await SelectFilesAndFoldersForShareAsync(entry, files, folders, null);

        await foreach (var r in GetPureShareRecordsDbAsync(files, folders))
        {
            yield return r;
        }
    }

    private async IAsyncEnumerable<FileShareRecord> GetPureShareRecordsDbAsync(List<string> files, List<string> folders)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var q = GetQuery(filesDbContext, r => folders.Contains(r.EntryId) && r.EntryType == FileEntryType.Folder);

        if (files.Count > 0)
        {
            q = q.Union(GetQuery(filesDbContext, r => files.Contains(r.EntryId) && r.EntryType == FileEntryType.File));
        }

        await foreach (var e in q.AsAsyncEnumerable())
        {
            yield return await ToFileShareRecordAsync(e);
        }
    }

    /// <summary>
    /// Get file share records with hierarchy.
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public Task<IEnumerable<FileShareRecord>> GetSharesAsync(FileEntry<T> entry)
    {
        if (entry == null)
        {
            return Task.FromResult(Enumerable.Empty<FileShareRecord>());
        }

        return InternalGetSharesAsync(entry);
    }

    public async Task<IEnumerable<FileShareRecord>> InternalGetSharesAsync(FileEntry<T> entry)
    {
        var files = new List<string>();
        var foldersInt = new List<int>();

        await SelectFilesAndFoldersForShareAsync(entry, files, null, foldersInt);

        return await SaveFilesAndFoldersForShareAsync(files, foldersInt);
    }

    private async Task SelectFilesAndFoldersForShareAsync(FileEntry<T> entry, ICollection<string> files, ICollection<string> folders, ICollection<int> foldersInt)
    {
        T folderId;
        if (entry.FileEntryType == FileEntryType.File)
        {
            var fileId = await MappingIDAsync(entry.Id);
            folderId = ((File<T>)entry).ParentId;
            if (!files.Contains(fileId.ToString()))
            {
                files.Add(fileId.ToString());
            }
        }
        else
        {
            folderId = entry.Id;
        }

        if (foldersInt != null && int.TryParse(folderId.ToString(), out var folderIdInt) && !foldersInt.Contains(folderIdInt))
        {
            foldersInt.Add(folderIdInt);
        }

        var mappedId = await MappingIDAsync(folderId);
        if (folders != null)
        {
            folders.Add(mappedId.ToString());
        }
    }

    private async Task<IEnumerable<FileShareRecord>> SaveFilesAndFoldersForShareAsync(List<string> files, List<int> folders)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var q = await Query(filesDbContext.Security)
            .Join(filesDbContext.Tree, r => r.EntryId, a => a.ParentId.ToString(), (security, tree) => new SecurityTreeRecord { DbFilesSecurity = security, DbFolderTree = tree })
            .Where(r => folders.Contains(r.DbFolderTree.FolderId) &&
                        r.DbFilesSecurity.EntryType == FileEntryType.Folder)
            .ToListAsync();

        if (0 < files.Count)
        {
            var q1 = await GetQuery(filesDbContext, r => files.Contains(r.EntryId) && r.EntryType == FileEntryType.File)
                .Select(r => new SecurityTreeRecord { DbFilesSecurity = r })
                .ToListAsync();
            q = q.Union(q1).ToList();
        }

        var records = await q
            .ToAsyncEnumerable()
            .SelectAwait(async e => await ToFileShareRecordAsync(e))
            .OrderBy(r => r.Level)
            .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer())
            .ToListAsync();

        await DeleteExpiredAsync(records, filesDbContext);

        return records;
    }

    public async Task RemoveSubjectAsync(Guid subject)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            using var tr = await filesDbContext.Database.BeginTransactionAsync();

            await filesDbContext.Security.Where(r => r.Subject == subject).ExecuteDeleteAsync();
            await filesDbContext.Security.Where(r => r.Owner == subject).ExecuteDeleteAsync();

            await tr.CommitAsync();
        });
    }

    private IQueryable<DbFilesSecurity> GetQuery(FilesDbContext filesDbContext, Expression<Func<DbFilesSecurity, bool>> where = null)
    {
        var q = Query(filesDbContext.Security);
        if (q != null)
        {

            q = q.Where(where);
        }
        return q;
    }

    protected async IAsyncEnumerable<FileShareRecord> FromQueryAsync(IQueryable<DbFilesSecurity> filesSecurities)
    {
        await foreach (var e in filesSecurities.AsAsyncEnumerable())
        {
            yield return await ToFileShareRecordAsync(e);
        }
    }

    private async Task<FileShareRecord> ToFileShareRecordAsync(DbFilesSecurity r)
    {
        var result = _mapper.Map<DbFilesSecurity, FileShareRecord>(r);
        result.EntryId = await MappingIDAsync(r.EntryId);

        return result;
    }

    private async Task<FileShareRecord> ToFileShareRecordAsync(SecurityTreeRecord r)
    {
        var result = await ToFileShareRecordAsync(r.DbFilesSecurity);
        if (r.DbFolderTree != null)
        {
            result.EntryId = r.DbFolderTree.FolderId;
        }

        result.Level = r.DbFolderTree?.Level ?? -1;

        return result;
    }

    private async Task DeleteExpiredAsync(List<FileShareRecord> records, FilesDbContext filesDbContext)
    {
        var expired = new List<Guid>();

        for (var i = 0; i < records.Count; i++)
        {
            var r = records[i];
            if (r.SubjectType != SubjectType.InvitationLink || r.FileShareOptions is not { IsExpired: true })
            {
                continue;
            }

            expired.Add(r.Subject);
            records.RemoveAt(i);
        }

        if (expired.Count > 0)
        {
            var tenantId = TenantID;

            await filesDbContext.Security
                .Where(s => s.TenantId == tenantId && s.SubjectType == SubjectType.InvitationLink && expired.Contains(s.Subject))
                .ExecuteDeleteAsync();
        }
    }
}

internal class SecurityTreeRecord
{
    public DbFilesSecurity DbFilesSecurity { get; set; }
    public DbFolderTree DbFolderTree { get; set; }
}
