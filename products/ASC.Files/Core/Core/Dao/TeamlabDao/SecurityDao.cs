/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Files.Core.EF;
using ASC.Files.Core.Security;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.Data
{
    [Scope]
    internal class SecurityDao<T> : AbstractDao, ISecurityDao<T>
    {
        public SecurityDao(UserManager userManager,
            DbContextManager<EF.FilesDbContext> dbContextManager,
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
            ICache cache)
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
        }

        public async Task DeleteShareRecordsAsync(IEnumerable<FileShareRecord> records)
        {
            using var tx = await FilesDbContext.Database.BeginTransactionAsync();

            foreach (var record in records)
            {
                var query = await FilesDbContext.Security
                    .AsQueryable()
                    .Where(r => r.TenantId == record.Tenant)
                    .Where(r => r.EntryType == record.EntryType)
                    .Where(r => r.Subject == record.Subject)
                    .AsAsyncEnumerable()
                    .WhereAwait(async r => r.EntryId == (await MappingIDAsync(record.EntryId)).ToString())
                    .ToListAsync();

                FilesDbContext.RemoveRange(query);
            }

            await tx.CommitAsync();
        }

        public ValueTask<bool> IsSharedAsync(object entryId, FileEntryType type)
        {
            return Query(FilesDbContext.Security)
                .AsAsyncEnumerable()
                .AnyAwaitAsync(async r => r.EntryId == (await MappingIDAsync(entryId)).ToString() &&
                          r.EntryType == type);
        }

        public async Task SetShareAsync(FileShareRecord r)
        {
            if (r.Share == FileShare.None)
            {
                var entryId = (await MappingIDAsync(r.EntryId) ?? "").ToString();
                if (string.IsNullOrEmpty(entryId)) return;

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
                        files.AddRange(await Query(FilesDbContext.Files).Where(r => foldersInt.Contains(r.FolderId)).Select(r => r.Id.ToString()).ToListAsync());
                    }
                    else
                    {
                        folders.Add(entryId);
                    }

                    var toDelete = await FilesDbContext.Security
                        .AsQueryable()
                        .Where(a => a.TenantId == r.Tenant &&
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
                        .Where(a => a.TenantId == r.Tenant &&
                                    files.Contains(a.EntryId) &&
                                    a.EntryType == FileEntryType.File &&
                                    a.Subject == r.Subject)
                        .ToListAsync();

                    FilesDbContext.Security.RemoveRange(toDelete);
                    await FilesDbContext.SaveChangesAsync();
                }

                await tx.CommitAsync();
            }
            else
            {
                var toInsert = new DbFilesSecurity
                {
                    TenantId = r.Tenant,
                    EntryId = (await MappingIDAsync(r.EntryId, true)).ToString(),
                    EntryType = r.EntryType,
                    Subject = r.Subject,
                    Owner = r.Owner,
                    Security = r.Share,
                    TimeStamp = DateTime.UtcNow
                };

                await FilesDbContext.AddOrUpdateAsync(r => r.Security, toInsert);
                await FilesDbContext.SaveChangesAsync();
            }
        }

        public ValueTask<List<FileShareRecord>> GetSharesAsync(IEnumerable<Guid> subjects)
        {
            var q = GetQuery(r => subjects.Contains(r.Subject));
            return FromQueryAsync(q);
        }

        public Task<IEnumerable<FileShareRecord>> GetPureShareRecordsAsync(IEnumerable<FileEntry<T>> entries)
        {
            if (entries == null) return Task.FromResult<IEnumerable<FileShareRecord>>(new List<FileShareRecord>());

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
            if (entries == null) return Task.FromResult<IEnumerable<FileShareRecord>>(new List<FileShareRecord>());

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
            if (entry == null) return Task.FromResult<IEnumerable<FileShareRecord>>(new List<FileShareRecord>());

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
            if (entries == null) return Task.FromResult<IEnumerable<FileShareRecord>>(new List<FileShareRecord>());

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
            if (entry == null) return Task.FromResult<IEnumerable<FileShareRecord>>(new List<FileShareRecord>());

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
                var fileId = await MappingIDAsync(entry.ID);
                folderId = ((File<T>)entry).FolderID;
                if (!files.Contains(fileId.ToString())) files.Add(fileId.ToString());
            }
            else
            {
                folderId = entry.ID;
            }

            if (foldersInt != null && int.TryParse(folderId.ToString(), out var folderIdInt) && !foldersInt.Contains(folderIdInt)) foldersInt.Add(folderIdInt);

            var mappedId = await MappingIDAsync(folderId);
            if (folders != null) folders.Add(mappedId.ToString());
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
            using var tr = await FilesDbContext.Database.BeginTransactionAsync();

            var toDelete1 = await FilesDbContext.Security.AsQueryable().Where(r => r.Subject == subject).ToListAsync();
            var toDelete2 = await FilesDbContext.Security.AsQueryable().Where(r => r.Owner == subject).ToListAsync();

            FilesDbContext.RemoveRange(toDelete1);
            await FilesDbContext.SaveChangesAsync();

            FilesDbContext.RemoveRange(toDelete2);
            await FilesDbContext.SaveChangesAsync();

            await tr.CommitAsync();
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

        protected ValueTask<List<FileShareRecord>> FromQueryAsync(IQueryable<DbFilesSecurity> filesSecurities)
        {
            return filesSecurities
                .AsAsyncEnumerable()
                .SelectAwait(async e => await ToFileShareRecordAsync(e))
                .ToListAsync();
        }

        private async Task<FileShareRecord> ToFileShareRecordAsync(DbFilesSecurity r)
        {
            return new FileShareRecord
            {
                Tenant = r.TenantId,
                EntryId = await MappingIDAsync(r.EntryId),
                EntryType = r.EntryType,
                Subject = r.Subject,
                Owner = r.Owner,
                Share = r.Security
            };
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
}