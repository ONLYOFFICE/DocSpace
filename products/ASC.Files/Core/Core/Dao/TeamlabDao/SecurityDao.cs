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

using User = ASC.Core.Common.EF.User;

namespace ASC.Files.Core.Data;

[Scope]
internal abstract class SecurityBaseDao<T> : AbstractDao
{
    private readonly IMapper _mapper;

    public SecurityBaseDao(
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
        await using var filesDbContext = _dbContextFactory.CreateDbContext();

        foreach (var record in records)
        {
            var query = Queries.ForDeleteShareRecordsAsync(filesDbContext, record)
            .WhereAwait(async r => r.EntryId == (await MappingIDAsync(record.EntryId)).ToString());

            filesDbContext.RemoveRange(query);
        }
        await filesDbContext.SaveChangesAsync();
    }

    public async Task<bool> IsSharedAsync(T entryId, FileEntryType type)
    {
        var mappedId = (entryId is int fid ? MappingIDAsync(fid) : await MappingIDAsync(entryId)).ToString();
        await using var filesDbContext = _dbContextFactory.CreateDbContext();

        return await Queries.IsSharedAsync(filesDbContext, TenantID, mappedId, type);
    }

    public async Task SetShareAsync(FileShareRecord r)
    {
        if (r.Share == FileShare.None)
        {
            var entryId = (r.EntryId is int fid ? MappingIDAsync(fid) : (await MappingIDAsync(r.EntryId) ?? "")).ToString();
            if (string.IsNullOrEmpty(entryId))
            {
                return;
            }

            await using var filesDbContext = _dbContextFactory.CreateDbContext();

            var files = new List<string>();

            if (r.EntryType == FileEntryType.Folder)
            {
                var folders = new List<string>();
                if (int.TryParse(entryId, out var intEntryId))
                {
                    var foldersInt = await Queries.FolderIdsAsync(filesDbContext, entryId).ToListAsync();

                    folders.AddRange(foldersInt.Select(folderInt => folderInt.ToString()));
                    files.AddRange(await Queries.FilesIdsAsync(filesDbContext, TenantID, foldersInt).ToListAsync());
                }
                else
                {
                    folders.Add(entryId);
                }
                var forSetShare = await Queries.ForSetShareAsync(filesDbContext, r.TenantId, r.Subject, folders, FileEntryType.Folder).ToListAsync();
                filesDbContext.Security.RemoveRange(forSetShare);
            }
            else
            {
                files.Add(entryId);
            }

            if (files.Count > 0)
            {
                filesDbContext.Security.RemoveRange(await Queries.ForSetShareAsync(filesDbContext, r.TenantId, r.Subject, files, FileEntryType.File).ToListAsync());
            }

            await filesDbContext.SaveChangesAsync();
        }
        else
        {
            var toInsert = _mapper.Map<FileShareRecord, DbFilesSecurity>(r);
            toInsert.EntryId = (await MappingIDAsync(r.EntryId, true)).ToString();

            await using var filesDbContext = _dbContextFactory.CreateDbContext();
            await filesDbContext.AddOrUpdateAsync(r => r.Security, toInsert);
            await filesDbContext.SaveChangesAsync();
        }
    }

    public async IAsyncEnumerable<FileShareRecord> GetShareForEntryIdsAsync(Guid subject, IEnumerable<string> roomIds)
    {
        var filesDbContext = _dbContextFactory.CreateDbContext();
        var q = Queries.ShareForEntryIdsAsync(filesDbContext, TenantID, subject, roomIds);

        await foreach (var e in q)
        {
            yield return await ToFileShareRecordAsync(e);
        }
    }

    public async IAsyncEnumerable<FileShareRecord> GetSharesAsync(IEnumerable<Guid> subjects)
    {
        var filesDbContext = _dbContextFactory.CreateDbContext();
        var q = Queries.SharesAsync(filesDbContext, TenantID, subjects);

        await foreach (var e in q)
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

    internal async IAsyncEnumerable<FileShareRecord> InternalGetPureShareRecordsAsync(IEnumerable<FileEntry<T>> entries)
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

    public async IAsyncEnumerable<FileShareRecord> GetRoomSharesAsync(Folder<T> room, ShareFilterType filterType, EmployeeActivationStatus? status, int offset = 0, int count = -1)
    {
        if (room == null || !DocSpaceHelper.IsRoom(room.FolderType) || count == 0)
        {
            yield break;
        }

        await using var filesDbContext = _dbContextFactory.CreateDbContext();

        var q = await GetPureSharesQuery(room, filterType, filesDbContext);

        if (filterType == ShareFilterType.User)
        {
            var predicate = ShareCompareHelper.GetCompareExpression<SecurityUserRecord>(s => s.Security.Share);

            var q1 = q.Join(filesDbContext.Users, s => s.Subject, u => u.Id, 
                (s, u) => new SecurityUserRecord { Security = s, User = u });

            if (status.HasValue)
            {
                q = q1.Where(s => s.User.ActivationStatus == status.Value)
                    .OrderBy(predicate)
                    .Select(s => s.Security);
            }
            else
            {
                q = q1.OrderBy(s => s.User.ActivationStatus)
                    .ThenBy(predicate)
                    .Select(s => s.Security);
            }
        }
        else
        {
            var predicate = ShareCompareHelper.GetCompareExpression<DbFilesSecurity>(s => s.Share);
            q = q.OrderBy(predicate);
        }

        if (offset > 0)
        {
            q = q.Skip(offset);
        }

        if (count > 0)
        {
            q = q.Take(count);
        }

        await foreach (var r in q.ToAsyncEnumerable())
        {
            yield return await ToFileShareRecordAsync(r);
        }
    }

    internal async IAsyncEnumerable<FileShareRecord> InternalGetPureShareRecordsAsync(FileEntry<T> entry)
    {
        var files = new List<string>();
        var folders = new List<string>();

        await SelectFilesAndFoldersForShareAsync(entry, files, folders, null);

        await foreach (var r in GetPureShareRecordsDbAsync(files, folders))
        {
            yield return r;
        }
    }

    internal async IAsyncEnumerable<FileShareRecord> GetPureShareRecordsDbAsync(List<string> files, List<string> folders)
    {
        await using var filesDbContext = _dbContextFactory.CreateDbContext();
        var q = Queries.PureShareRecordsDbAsync(filesDbContext, TenantID, files, folders);

        await foreach (var e in q)
        {
            yield return await ToFileShareRecordAsync(e);
        }
    }

    public async Task RemoveSubjectAsync(Guid subject)
    {
        await using var filesDbContext = _dbContextFactory.CreateDbContext();

        await Queries.RemoveSubjectAsync(filesDbContext, TenantID, subject);

        await filesDbContext.SaveChangesAsync();
    }
    
    public async IAsyncEnumerable<FileShareRecord> GetPureSharesAsync(FileEntry<T> entry, IEnumerable<Guid> subjects)
    {
        if (subjects == null || !subjects.Any())
        {
            yield break;
        }
        
        var entryId = await MappingIDAsync(entry.Id);

        await using var filesDbContext = _dbContextFactory.CreateDbContext();

        await foreach (var security in Queries.EntrySharesBySubjectsAsync(filesDbContext, TenantID, entryId.ToString(), entry.FileEntryType, subjects))
        {
            yield return await ToFileShareRecordAsync(security);
        }
    }

    internal async Task SelectFilesAndFoldersForShareAsync(FileEntry<T> entry, ICollection<string> files, ICollection<string> folders, ICollection<int> foldersInt)
    {
        T folderId;
        if (entry.FileEntryType == FileEntryType.File)
        {
            var fileId = entry.Id is int entryId ? MappingIDAsync(entryId) : await MappingIDAsync(entry.Id);
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

        var mappedId = folderId is int fid ? MappingIDAsync(fid) : await MappingIDAsync(folderId);
        if (folders != null)
        {
            folders.Add(mappedId.ToString());
        }
    }

    internal IQueryable<DbFilesSecurity> GetQuery(FilesDbContext filesDbContext, Expression<Func<DbFilesSecurity, bool>> where = null)
    {
        var q = Query(filesDbContext.Security);
        if (q != null)
        {

            q = q.Where(where);
        }
        return q;
    }

    internal async Task<FileShareRecord> ToFileShareRecordAsync(DbFilesSecurity r)
    {
        var result = _mapper.Map<DbFilesSecurity, FileShareRecord>(r);
        result.EntryId = await MappingIDAsync(r.EntryId);

        return result;
    }

    internal async Task<FileShareRecord> ToFileShareRecordAsync(SecurityTreeRecord r)
    {
        var result = await ToFileShareRecordAsync(r.DbFilesSecurity);
        if (r.DbFolderTree != null)
        {
            result.EntryId = r.DbFolderTree.FolderId;
        }

        result.Level = r.DbFolderTree?.Level ?? -1;

        return result;
    }
    
    private async Task<IQueryable<DbFilesSecurity>> GetPureSharesQuery(FileEntry<T> entry, ShareFilterType filterType, FilesDbContext filesDbContext)
    {
        var entryId = await MappingIDAsync(entry.Id);

        var q = filesDbContext.Security.AsNoTracking()
            .Where(s => s.TenantId == TenantID && s.EntryId == entryId.ToString() && s.EntryType == entry.FileEntryType);

        switch (filterType)
        {
            case ShareFilterType.User:
                q = q.Where(s => s.SubjectType == SubjectType.UserOrGroup);
                break;
            case ShareFilterType.InvitationLink:
                q = q.Where(s => s.SubjectType == SubjectType.InvitationLink);
                break;
            case ShareFilterType.ExternalLink:
                q = q.Where(s => s.SubjectType == SubjectType.ExternalLink);
                break;
            case ShareFilterType.Link:
                q = q.Where(s => s.SubjectType == SubjectType.InvitationLink || s.SubjectType == SubjectType.ExternalLink);
                break;
        }

        return q;
    }
}

[Scope]
internal class SecurityDao : SecurityBaseDao<int>, ISecurityDao<int>
{
    public SecurityDao(UserManager userManager,
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
        IMapper mapper) : base(userManager, dbContextFactory, tenantManager, tenantUtil, setupInfo, maxTotalSizeStatistic, coreBaseSettings, coreConfiguration, settingsManager, authContext, serviceProvider, cache, mapper)
    {
    }

    public async Task<IEnumerable<FileShareRecord>> GetSharesAsync(FileEntry<int> entry)
    {
        if (entry == null)
        {
            return Enumerable.Empty<FileShareRecord>();
        }

        var files = new List<string>();
        var foldersInt = new List<int>();

        await SelectFilesAndFoldersForShareAsync(entry, files, null, foldersInt);

        return await SaveFilesAndFoldersForShareAsync(files, foldersInt);
    }

    private async Task<IEnumerable<FileShareRecord>> SaveFilesAndFoldersForShareAsync(List<string> files, List<int> folders)
    {
        await using var filesDbContext = _dbContextFactory.CreateDbContext();

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

        return await q
            .ToAsyncEnumerable()
            .SelectAwait(async e => await ToFileShareRecordAsync(e))
            .OrderBy(r => r.Level)
            .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer())
            .ToListAsync();
    }
}

[Scope]
internal class ThirdPartySecurityDao : SecurityBaseDao<string>, ISecurityDao<string>
{
    private readonly SelectorFactory _selectorFactory;

    public ThirdPartySecurityDao(UserManager userManager,
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
        IMapper mapper,
        SelectorFactory selectorFactory) : base(userManager, dbContextFactory, tenantManager, tenantUtil, setupInfo, maxTotalSizeStatistic, coreBaseSettings, coreConfiguration, settingsManager, authContext, serviceProvider, cache, mapper)
    {
        _selectorFactory = selectorFactory;
    }

    public async Task<IEnumerable<FileShareRecord>> GetSharesAsync(FileEntry<string> entry)
    {
        var result = new List<FileShareRecord>();

        var folders = new List<FileEntry<string>>();
        if (entry is Folder<string> entryFolder)
        {
            folders.Add(entryFolder);
        }

        if (entry is File<string> file)
        {
            await GetFoldersForShareAsync(file.ParentId, folders);

            var pureShareRecords = GetPureShareRecordsAsync(entry);
            await foreach (var pureShareRecord in pureShareRecords)
            {
                if (pureShareRecord == null)
                {
                    continue;
                }

                pureShareRecord.Level = -1;
                result.Add(pureShareRecord);
            }
        }

        result.AddRange(await GetShareForFoldersAsync(folders).ToListAsync());

        return result;
    }

    private async ValueTask GetFoldersForShareAsync(string folderId, ICollection<FileEntry<string>> folders)
    {
        var selector = _selectorFactory.GetSelector(folderId);
        var folderDao = selector.GetFolderDao(folderId);
        if (folderDao == null)
        {
            return;
        }

        var folder = await folderDao.GetFolderAsync(selector.ConvertId(folderId));

        if (folder != null)
        {
            folders.Add(folder);
        }
    }

    private async IAsyncEnumerable<FileShareRecord> GetShareForFoldersAsync(IReadOnlyCollection<FileEntry<string>> folders)
    {
        foreach (var folder in folders)
        {
            var selector = _selectorFactory.GetSelector(folder.Id);
            var folderDao = selector.GetFolderDao(folder.Id);
            if (folderDao == null)
            {
                continue;
            }

            var parentFolders = await folderDao.GetParentFoldersAsync(selector.ConvertId(folder.Id)).ToListAsync();
            if (parentFolders == null || parentFolders.Count == 0)
            {
                continue;
            }

            parentFolders.Reverse();
            var pureShareRecords = await GetPureShareRecordsAsync(parentFolders).ToListAsync();
            if (pureShareRecords == null)
            {
                continue;
            }

            foreach (var pureShareRecord in pureShareRecords)
            {
                if (pureShareRecord == null)
                {
                    continue;
                }

                var f = _serviceProvider.GetService<Folder<string>>();
                f.Id = pureShareRecord.EntryId.ToString();

                pureShareRecord.Level = parentFolders.IndexOf(f);
                pureShareRecord.EntryId = folder.Id;
                yield return pureShareRecord;
            }
        }
    }
}

internal class SecurityTreeRecord
{
    public DbFilesSecurity DbFilesSecurity { get; init; }
    public DbFolderTree DbFolderTree { get; init; }
}

public class SecurityUserRecord
{
    public DbFilesSecurity Security { get; init; }
    public User User { get; init; }
}

static file class Queries
{
    public static readonly Func<FilesDbContext, FileShareRecord, IAsyncEnumerable<DbFilesSecurity>>
        ForDeleteShareRecordsAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (FilesDbContext ctx, FileShareRecord record) =>
                ctx.Security
                    .Where(r => r.TenantId == record.TenantId)
                    .Where(r => r.EntryType == record.EntryType)
                    .Where(r => r.Subject == record.Subject));

    public static readonly Func<FilesDbContext, string, IAsyncEnumerable<int>> FolderIdsAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (FilesDbContext ctx, string entryId) =>
                ctx.Tree.Where(r => r.ParentId.ToString() == entryId)
                    .Select(r => r.FolderId));

    public static readonly Func<FilesDbContext, int, IEnumerable<int>, IAsyncEnumerable<string>> FilesIdsAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, IEnumerable<int> folders) =>
                ctx.Files.Where(r => r.TenantId == tenantId && folders.Contains(r.ParentId))
                    .Select(r => r.Id.ToString()));

    public static readonly
        Func<FilesDbContext, int, Guid, IEnumerable<string>, FileEntryType, IAsyncEnumerable<DbFilesSecurity>>
        ForSetShareAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, Guid subject, IEnumerable<string> entryIds, FileEntryType type) =>
                ctx.Security
                    .Where(a => a.TenantId == tenantId &&
                                entryIds.Contains(a.EntryId) &&
                                a.EntryType == type &&
                                a.Subject == subject));

    public static readonly Func<FilesDbContext, int, string, FileEntryType, Task<bool>> IsSharedAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, string entryId, FileEntryType type) =>
                ctx.Security.Any(r => r.TenantId == tenantId && r.EntryId == entryId && r.EntryType == type
                                      && !(new[] { FileConstant.DenyDownloadId, FileConstant.DenySharingId }).Contains(
                                          r.Subject)));

    public static readonly Func<FilesDbContext, int, Guid, IEnumerable<string>, IAsyncEnumerable<DbFilesSecurity>>
        ShareForEntryIdsAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, Guid subject, IEnumerable<string> roomIds) =>
                ctx.Security
                    .Where(r => r.TenantId == tenantId
                                && (r.Subject == subject || r.Owner == subject)
                                && roomIds.Contains(r.EntryId)));

    public static readonly Func<FilesDbContext, int, IEnumerable<Guid>, IAsyncEnumerable<DbFilesSecurity>> SharesAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, IEnumerable<Guid> subjects) =>
                ctx.Security
                    .Where(r => r.TenantId == tenantId && subjects.Contains(r.Subject)));

    public static readonly
        Func<FilesDbContext, int, IEnumerable<string>, IEnumerable<string>, IAsyncEnumerable<DbFilesSecurity>>
        PureShareRecordsDbAsync = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, IEnumerable<string> files, IEnumerable<string> folders) =>
                ctx.Security.Where(r =>
                    (r.TenantId == tenantId && files.Contains(r.EntryId) && r.EntryType == FileEntryType.File)
                    || (r.TenantId == tenantId && folders.Contains(r.EntryId) && r.EntryType == FileEntryType.Folder)));

    public static readonly Func<FilesDbContext, int, Guid, Task<int>> RemoveSubjectAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, Guid subject) =>
                ctx.Security
                    .Where(r => r.TenantId == tenantId
                                && (r.Subject == subject || r.Owner == subject))
                    .ExecuteDelete());

    public static readonly Func<FilesDbContext, int, string, FileEntryType, IEnumerable<Guid>, IAsyncEnumerable<DbFilesSecurity>> EntrySharesBySubjectsAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, string entryId, FileEntryType entryType, IEnumerable<Guid> subjects) => 
                ctx.Security
                    .Where(r => r.TenantId == tenantId && r.EntryId == entryId && r.EntryType == entryType && subjects.Contains(r.Subject)));
}