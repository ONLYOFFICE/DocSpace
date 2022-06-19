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

    public SecurityDao(UserManager userManager,
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

    public async Task DeleteShareRecordsAsync(IEnumerable<FileShareRecord> records)
    {
        var strategy = FilesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var tx = await FilesDbContext.Database.BeginTransactionAsync();

            foreach (var record in records)
            {
                var query = await FilesDbContext.Security
                    .AsQueryable()
                    .Where(r => r.TenantId == record.TenantId)
                    .Where(r => r.EntryType == record.EntryType)
                    .Where(r => r.Subject == record.Subject)
                    .AsAsyncEnumerable()
                    .WhereAwait(async r => r.EntryId == (await MappingIDAsync(record.EntryId)).ToString())
                    .ToListAsync();

                FilesDbContext.RemoveRange(query);
            }

            await tx.CommitAsync();
        });
    }

    public async Task<bool> IsSharedAsync(T entryId, FileEntryType type)
    {
        var mappedId = (await MappingIDAsync(entryId)).ToString();

        return await Query(FilesDbContext.Security)
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

            var strategy = FilesDbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
             {
                 using var tx = await FilesDbContext.Database.BeginTransactionAsync();
                 var files = new List<string>();

                 if (r.EntryType == FileEntryType.Folder)
                 {
                     var folders = new List<string>();
                     if (int.TryParse(entryId, out var intEntryId))
                     {
                         var foldersInt = await FilesDbContext.Tree
                             .AsQueryable()
                             .Where(r => r.ParentId.ToString() == entryId)
                             .Select(r => r.FolderId)
                             .ToListAsync();

                         folders.AddRange(foldersInt.Select(folderInt => folderInt.ToString()));
                         files.AddRange(await Query(FilesDbContext.Files).Where(r => foldersInt.Contains(r.ParentId)).Select(r => r.Id.ToString()).ToListAsync());
                     }
                     else
                     {
                         folders.Add(entryId);
                     }

                     var toDelete = await FilesDbContext.Security
                         .AsQueryable()
                         .Where(a => a.TenantId == r.TenantId &&
                                     folders.Contains(a.EntryId) &&
                                     a.EntryType == FileEntryType.Folder &&
                                     a.Subject == r.Subject)
                         .ToListAsync();

                     FilesDbContext.Security.RemoveRange(toDelete);
                     await FilesDbContext.SaveChangesAsync();

                 }
                 else
                 {
                     files.Add(entryId);
                 }

                 if (0 < files.Count)
                 {
                     var toDelete = await FilesDbContext.Security
                         .AsQueryable()
                         .Where(a => a.TenantId == r.TenantId &&
                                     files.Contains(a.EntryId) &&
                                     a.EntryType == FileEntryType.File &&
                                     a.Subject == r.Subject)
                         .ToListAsync();

                     FilesDbContext.Security.RemoveRange(toDelete);
                     await FilesDbContext.SaveChangesAsync();
                 }

                 await tx.CommitAsync();

             });
        }
        else
        {
            var toInsert = _mapper.Map<FileShareRecord, DbFilesSecurity>(r);
            toInsert.EntryId = (await MappingIDAsync(r.EntryId, true)).ToString();

            await FilesDbContext.AddOrUpdateAsync(r => r.Security, toInsert);
            await FilesDbContext.SaveChangesAsync();
        }
    }

        public Task<List<FileShareRecord>> GetSharesAsync(IEnumerable<Guid> subjects)
    {
        var q = GetQuery(r => subjects.Contains(r.Subject));

        return FromQueryAsync(q);
    }

    public Task<IEnumerable<FileShareRecord>> GetPureShareRecordsAsync(IEnumerable<FileEntry<T>> entries)
    {
        if (entries == null)
        {
            return Task.FromResult<IEnumerable<FileShareRecord>>(new List<FileShareRecord>());
        }

        return InternalGetPureShareRecordsAsync(entries);
    }

    private async Task<IEnumerable<FileShareRecord>> InternalGetPureShareRecordsAsync(IEnumerable<FileEntry<T>> entries)
    {
        var files = new List<string>();
        var folders = new List<string>();

        foreach (var entry in entries)
        {
            await SelectFilesAndFoldersForShareAsync(entry, files, folders, null);
        }

        return await GetPureShareRecordsDbAsync(files, folders);
    }

    public Task<IEnumerable<FileShareRecord>> GetPureShareRecordsAsync(IAsyncEnumerable<FileEntry<T>> entries)
    {
        if (entries == null)
        {
            return Task.FromResult<IEnumerable<FileShareRecord>>(new List<FileShareRecord>());
        }

        return InternalGetPureShareRecordsAsync(entries);
    }

    private async Task<IEnumerable<FileShareRecord>> InternalGetPureShareRecordsAsync(IAsyncEnumerable<FileEntry<T>> entries)
    {
        var files = new List<string>();
        var folders = new List<string>();

        await foreach (var entry in entries)
        {
            await SelectFilesAndFoldersForShareAsync(entry, files, folders, null);
        }

        return await GetPureShareRecordsDbAsync(files, folders);
    }

    public Task<IEnumerable<FileShareRecord>> GetPureShareRecordsAsync(FileEntry<T> entry)
    {
        if (entry == null)
        {
            return Task.FromResult<IEnumerable<FileShareRecord>>(new List<FileShareRecord>());
        }

        return InternalGetPureShareRecordsAsync(entry);
    }

    private async Task<IEnumerable<FileShareRecord>> InternalGetPureShareRecordsAsync(FileEntry<T> entry)
    {
        var files = new List<string>();
        var folders = new List<string>();

        await SelectFilesAndFoldersForShareAsync(entry, files, folders, null);

        return await GetPureShareRecordsDbAsync(files, folders);
    }

    private async Task<IEnumerable<FileShareRecord>> GetPureShareRecordsDbAsync(List<string> files, List<string> folders)
    {
        var result = new List<FileShareRecord>();

        var q = GetQuery(r => folders.Contains(r.EntryId) && r.EntryType == FileEntryType.Folder);

        if (files.Count > 0)
        {
            q = q.Union(GetQuery(r => files.Contains(r.EntryId) && r.EntryType == FileEntryType.File));
        }

        result.AddRange(await FromQueryAsync(q));

        return result;
    }

    /// <summary>
    /// Get file share records with hierarchy.
    /// </summary>
    /// <param name="entries"></param>
    /// <returns></returns>
    public Task<IEnumerable<FileShareRecord>> GetSharesAsync(IEnumerable<FileEntry<T>> entries)
    {
        if (entries == null)
        {
            return Task.FromResult<IEnumerable<FileShareRecord>>(new List<FileShareRecord>());
        }

        return InternalGetSharesAsync(entries);
    }

    private async Task<IEnumerable<FileShareRecord>> InternalGetSharesAsync(IEnumerable<FileEntry<T>> entries)
    {
        var files = new List<string>();
        var foldersInt = new List<int>();

        foreach (var entry in entries)
        {
            await SelectFilesAndFoldersForShareAsync(entry, files, null, foldersInt);
        }

        return await SaveFilesAndFoldersForShareAsync(files, foldersInt);
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
            return Task.FromResult<IEnumerable<FileShareRecord>>(new List<FileShareRecord>());
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
        var q = await Query(FilesDbContext.Security)
            .Join(FilesDbContext.Tree, r => r.EntryId, a => a.ParentId.ToString(), (security, tree) => new SecurityTreeRecord { DbFilesSecurity = security, DbFolderTree = tree })
            .Where(r => folders.Contains(r.DbFolderTree.FolderId) &&
                        r.DbFilesSecurity.EntryType == FileEntryType.Folder)
            .ToListAsync();

        if (0 < files.Count)
        {
            var q1 = await GetQuery(r => files.Contains(r.EntryId) && r.EntryType == FileEntryType.File)
                .Select(r => new SecurityTreeRecord { DbFilesSecurity = r })
                .ToListAsync();
            q = q.Union(q1).ToList();
        }

        return await q
            .ToAsyncEnumerable()
            .SelectAwait(async e => await ToFileShareRecordAsync(e))
            .OrderBy(r => r.Level)
            .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer())
            .ToListAsync();
    }

    public async Task RemoveSubjectAsync(Guid subject)
    {
        var strategy = FilesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var tr = await FilesDbContext.Database.BeginTransactionAsync();

            var toDelete1 = await FilesDbContext.Security.AsQueryable().Where(r => r.Subject == subject).ToListAsync();
            var toDelete2 = await FilesDbContext.Security.AsQueryable().Where(r => r.Owner == subject).ToListAsync();

            FilesDbContext.RemoveRange(toDelete1);
            await FilesDbContext.SaveChangesAsync();

            FilesDbContext.RemoveRange(toDelete2);
            await FilesDbContext.SaveChangesAsync();

            await tr.CommitAsync();
        });
    }

    private IQueryable<DbFilesSecurity> GetQuery(Expression<Func<DbFilesSecurity, bool>> where = null)
    {
        var q = Query(FilesDbContext.Security);
        if (q != null)
        {

            q = q.Where(where);
        }
        return q;
    }

        protected async Task<List<FileShareRecord>> FromQueryAsync(IQueryable<DbFilesSecurity> filesSecurities)
    {
            var data = await filesSecurities.ToListAsync();
            var result = new List<FileShareRecord>();
            foreach (var file in data)
            {
                result.Add(await ToFileShareRecordAsync(file));
            }
            return result;
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
}

internal class SecurityTreeRecord
{
    public DbFilesSecurity DbFilesSecurity { get; set; }
    public DbFolderTree DbFolderTree { get; set; }
}
