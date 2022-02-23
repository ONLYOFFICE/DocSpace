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
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Files.Core.EF;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.Data
{
    [Scope]
    internal class TagDao<T> : AbstractDao, ITagDao<T>
    {
        private static readonly object syncRoot = new object();

        public TagDao(
            UserManager userManager,
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

        public async IAsyncEnumerable<Tag> GetTagsAsync(Guid subject, TagType tagType, IEnumerable<FileEntry<T>> fileEntries)
        {
            var filesId = new HashSet<string>();
            var foldersId = new HashSet<string>();

            foreach (var f in fileEntries)
            {
                var idObj = await MappingIDAsync(f.ID).ConfigureAwait(false);
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
                .Where(r => r.Tag.Flag == tagType)
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

        static Func<EF.FilesDbContext, int, Guid, IEnumerable<TagType>, HashSet<string>, HashSet<string>, IAsyncEnumerable<TagLinkData>> getTagsQuery =
Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((EF.FilesDbContext ctx, int tenantId, Guid subject, IEnumerable<TagType> tagType, HashSet<string> filesId, HashSet<string> foldersId) =>
ctx.Tag
.AsNoTracking()
.Where(r => r.TenantId == tenantId)
.Where(r => tagType.Contains(r.Flag))
.Join(ctx.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
.Where(r => r.Link.TenantId == r.Tag.TenantId)
.Where(r => r.Link.EntryType == FileEntryType.File && filesId.Contains(r.Link.EntryId)
|| r.Link.EntryType == FileEntryType.Folder && foldersId.Contains(r.Link.EntryId))
.Where(r => subject == Guid.Empty || r.Link.CreateBy == subject)
);

        public async Task<IDictionary<object, IEnumerable<Tag>>> GetTagsAsync(Guid subject, IEnumerable<TagType> tagType, IEnumerable<FileEntry<T>> fileEntries)
        {
            var filesId = new HashSet<string>();
            var foldersId = new HashSet<string>();

            foreach (var f in fileEntries)
            {
                var idObj = await MappingIDAsync(f.ID).ConfigureAwait(false);
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
                var fromQuery = await FromQueryAsync(getTagsQuery(FilesDbContext, TenantID, subject, tagType, filesId, foldersId))
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
                .Where(r => r.Tag.Flag == tagType);

            await foreach (var e in FromQueryAsync(q).ConfigureAwait(false))
            {
                yield return e;
            }
        }

        public IAsyncEnumerable<Tag> GetTagsAsync(string[] names, TagType tagType)
        {
            if (names == null) throw new ArgumentNullException(nameof(names));

            return InternalGetTagsAsync(names, tagType);
        }

        public async IAsyncEnumerable<Tag> InternalGetTagsAsync(string[] names, TagType tagType)
        {
            var q = Query(FilesDbContext.Tag)
                .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
                .Where(r => r.Link.TenantId == r.Tag.TenantId)
                .Where(r => r.Tag.Owner == Guid.Empty)
                .Where(r => names.Contains(r.Tag.Name))
                .Where(r => r.Tag.Flag == tagType);

            await foreach (var e in FromQueryAsync(q).ConfigureAwait(false))
            {
                yield return e;
            }
        }

        public IAsyncEnumerable<Tag> GetTagsAsync(string name, TagType tagType)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            return GetTagsAsync(new[] { name }, tagType);
        }

        public async IAsyncEnumerable<Tag> GetTagsAsync(Guid owner, TagType tagType)
        {
            var q =
                Query(FilesDbContext.Tag)
                .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
                .Where(r => r.Link.TenantId == r.Tag.TenantId)
                .Where(r => r.Tag.Flag == tagType);

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

            if (tags == null) return result;

            tags = tags.Where(x => x != null && !x.EntryId.Equals(null) && !x.EntryId.Equals(0)).ToArray();

            if (!tags.Any()) return result;

            lock (syncRoot)
            {
                using var tx = FilesDbContext.Database.BeginTransaction();
                DeleteTagsBeforeSave();

                var createOn = TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow());
                var cacheTagId = new Dictionary<string, int>();

                result.AddRange(tags.Select(t => SaveTagAsync(t, cacheTagId, createOn).Result));

                tx.Commit();
            }

            return result;
        }

        public IEnumerable<Tag> SaveTags(Tag tag)
        {
            var result = new List<Tag>();

            if (tag == null) return result;

            if (tag.EntryId.Equals(null) || tag.EntryId.Equals(0)) return result;

            lock (syncRoot)
            {
                using var tx = FilesDbContext.Database.BeginTransaction();
                DeleteTagsBeforeSave();

                var createOn = TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow());
                var cacheTagId = new Dictionary<string, int>();

                result.Add(SaveTagAsync(tag, cacheTagId, createOn).Result);

                tx.Commit();
            }

            return result;
        }

        private void DeleteTagsBeforeSave()
        {
            var mustBeDeleted =
                Query(FilesDbContext.Tag)
                .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
                .Where(r => r.Link.TenantId == r.Tag.TenantId)
                .Where(r => (r.Tag.Flag == TagType.New || r.Tag.Flag == TagType.Recent) && r.Link.CreateOn <= TenantUtil.DateTimeNow().AddMonths(-1))
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
            var cacheTagIdKey = string.Join("/", new[] { TenantID.ToString(), t.Owner.ToString(), t.TagName, ((int)t.TagType).ToString(CultureInfo.InvariantCulture) });

            if (!cacheTagId.TryGetValue(cacheTagIdKey, out var id))
            {
                id = await FilesDbContext.Tag
                    .AsQueryable()
                    .Where(r => r.Owner == t.Owner)
                    .Where(r => r.Name == t.TagName)
                    .Where(r => r.Flag == t.TagType)
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync();

                if (id == 0)
                {
                    var toAdd = new DbFilesTag
                    {
                        Id = 0,
                        Name = t.TagName,
                        Owner = t.Owner,
                        Flag = t.TagType,
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
                CreateBy = AuthContext.CurrentAccount.ID,
                CreateOn = createOn,
                TagCount = t.Count
            };

            await FilesDbContext.AddOrUpdateAsync(r => r.TagLink, linkToInsert);
            await FilesDbContext.SaveChangesAsync();

            return t;
        }

        public void UpdateNewTags(IEnumerable<Tag> tags)
        {
            if (tags == null || !tags.Any()) return;

            lock (syncRoot)
            {
                using var tx = FilesDbContext.Database.BeginTransaction();
                var createOn = TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow());

                foreach (var tag in tags)
                {
                    UpdateNewTagsInDbAsync(tag, createOn).Wait();
                }
                tx.Commit();
            }
        }

        public void UpdateNewTags(Tag tag)
        {
            if (tag == null) return;

            lock (syncRoot)
            {
                var createOn = TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow());

                UpdateNewTagsInDbAsync(tag, createOn).Wait();
            }
        }

        private Task UpdateNewTagsInDbAsync(Tag tag, DateTime createOn)
        {
            if (tag == null) return Task.CompletedTask;

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
                f.CreateBy = AuthContext.CurrentAccount.ID;
                f.CreateOn = createOn;
                f.TagCount = tag.Count;
            }

            await FilesDbContext.SaveChangesAsync();
        }

        public void RemoveTags(IEnumerable<Tag> tags)
        {
            if (tags == null || !tags.Any()) return;

            lock (syncRoot)
            {
                using var tx = FilesDbContext.Database.BeginTransaction();
                foreach (var t in tags)
                {
                    RemoveTagInDbAsync(t).Wait();
                }
                tx.Commit();
            }
        }

        public void RemoveTags(Tag tag)
        {
            if (tag == null) return;

            lock (syncRoot)
            {
                using var tx = FilesDbContext.Database.BeginTransaction();
                RemoveTagInDbAsync(tag).Wait();

                tx.Commit();
            }
        }

        private Task RemoveTagInDbAsync(Tag tag)
        {
            if (tag == null) return Task.CompletedTask;

            return InternalRemoveTagInDbAsync(tag);
        }

        private async Task InternalRemoveTagInDbAsync(Tag tag)
        {
            var id = await Query(FilesDbContext.Tag)
                .Where(r => r.Name == tag.TagName &&
                            r.Owner == tag.Owner &&
                            r.Flag == tag.TagType)
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

        static Func<EF.FilesDbContext, int, Guid, List<string>, IAsyncEnumerable<TagLinkData>> newTagsForFilesQuery =
            Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((EF.FilesDbContext ctx, int tenantId, Guid subject, List<string> where) =>
            ctx.Tag
            .AsNoTracking()
            .Where(r => r.TenantId == tenantId)
            .Where(r => subject == Guid.Empty || r.Owner == subject)
            .Where(r => r.Flag == TagType.New)
            .Join(ctx.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData
            {
                Tag = tag,
                Link = link
            })
            .Where(r => r.Link.TenantId == r.Tag.TenantId)
            .Join(ctx.Files, r => Regex.IsMatch(r.Link.EntryId, "^[0-9]+$") ? Convert.ToInt32(r.Link.EntryId) : -1, r => r.Id, (tagLink, file) => new { tagLink, file })
            .Where(r => r.file.TenantId == r.tagLink.Link.TenantId)
            .Where(r => where.Contains(r.file.FolderId.ToString()))
            .Where(r => r.tagLink.Link.EntryType == FileEntryType.File)
            .Select(r => r.tagLink)
            .Distinct()
            );

        static Func<EF.FilesDbContext, int, Guid, List<string>, IAsyncEnumerable<TagLinkData>> newTagsForFoldersQuery =
            Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((EF.FilesDbContext ctx, int tenantId, Guid subject, List<string> monitorFolderIdsStrings) =>
            ctx.Tag
            .AsNoTracking()
            .Where(r => r.TenantId == tenantId)
            .Where(r => subject == Guid.Empty || r.Owner == subject)
            .Where(r => r.Flag == TagType.New)
            .Join(ctx.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData
            {
                Tag = tag,
                Link = link
            })
            .Where(r => r.Link.TenantId == r.Tag.TenantId)
            .Where(r => monitorFolderIdsStrings.Contains(r.Link.EntryId))
            .Where(r => r.Link.EntryType == FileEntryType.Folder)
            );

        static Func<EF.FilesDbContext, int, Guid, FolderType, IAsyncEnumerable<TagLinkData>> tmpShareFileTagsQuery =
            Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((EF.FilesDbContext ctx, int tenantId, Guid subject, FolderType folderType) =>
            ctx.Tag
            .AsNoTracking()
            .Where(r => r.TenantId == tenantId)
            .Where(r => subject == Guid.Empty || r.Owner == subject)
            .Where(r => r.Flag == TagType.New)
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
                    .Where(x => x.folder.TenantId == tenantId && x.tree.FolderId == r.file.FolderId)
                    .OrderByDescending(r => r.tree.Level)
                    .Select(r => r.folder)
                    .Take(1)
                    .FirstOrDefault()
            })
            .Where(r => r.root.FolderType == folderType)
            .Select(r => r.tagLink)
            .Distinct()
            );

        static Func<EF.FilesDbContext, int, Guid, FolderType, IAsyncEnumerable<TagLinkData>> tmpShareFolderTagsQuery =
    Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((EF.FilesDbContext ctx, int tenantId, Guid subject, FolderType folderType) =>
                ctx.Tag
                .AsNoTracking()
                .Where(r => r.TenantId == tenantId)
                .Where(r => subject == Guid.Empty || r.Owner == subject)
                .Where(r => r.Flag == TagType.New)
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

        static Func<EF.FilesDbContext, int, Guid, IAsyncEnumerable<TagLinkData>> tmpShareSboxTagsQuery =
    Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((EF.FilesDbContext ctx, int tenantId, Guid subject) =>
                ctx.Tag
                .AsNoTracking()
                .Where(r => r.TenantId == tenantId)
                .Where(r => subject == Guid.Empty || r.Owner == subject)
                .Where(r => r.Flag == TagType.New)
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

        static Func<EF.FilesDbContext, int, Guid, IAsyncEnumerable<TagLinkData>> projectsQuery =
    Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((EF.FilesDbContext ctx, int tenantId, Guid subject) =>
                ctx.Tag
                .AsNoTracking()
                .Where(r => r.TenantId == tenantId)
                .Where(r => subject == Guid.Empty || r.Owner == subject)
                .Where(r => r.Flag == TagType.New)
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

        static Func<EF.FilesDbContext, int, Guid, List<string>, IAsyncEnumerable<TagLinkData>> newTagsForSBoxQuery =
    Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((EF.FilesDbContext ctx, int tenantId, Guid subject, List<string> thirdpartyFolderIds) =>
                ctx.Tag
                .AsNoTracking()
                .Where(r => r.TenantId == tenantId)
                .Where(r => subject == Guid.Empty || r.Owner == subject)
                .Where(r => r.Flag == TagType.New)
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

        static Func<EF.FilesDbContext, List<int>, bool, IAsyncEnumerable<int>> getFolderQuery = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((EF.FilesDbContext ctx, List<int> monitorFolderIdsInt, bool deepSearch) =>
              ctx.Tree
                  .AsNoTracking()
                  .Where(r => monitorFolderIdsInt.Contains(r.ParentId))
                  .Where(r => deepSearch || r.Level == 1)
                  .Select(r => r.FolderId));

        static Func<EF.FilesDbContext, int, FolderType, Guid, IAsyncEnumerable<int>> getThirdpartyAccountQuery = Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery((EF.FilesDbContext ctx, int tenantId, FolderType folderType, Guid subject) =>
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
                var idObj = await MappingIDAsync(r.ID).ConfigureAwait(false);
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
                    .Where(r => r.Tag.Flag == TagType.New)
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
            if (parentFolder == null || EqualityComparer<T>.Default.Equals(parentFolder.ID, default(T)))
                throw new ArgumentException("folderId");

            return InternalGetNewTagsAsync(subject, parentFolder, deepSearch);
        }

        private async IAsyncEnumerable<Tag> InternalGetNewTagsAsync(Guid subject, Folder<T> parentFolder, bool deepSearch)
        {
            var result = AsyncEnumerable.Empty<Tag>();

            var monitorFolderIds = new object[] { parentFolder.ID }.ToAsyncEnumerable();

            var tenantId = TenantID;

            var tempTags = AsyncEnumerable.Empty<Tag>();

            if (parentFolder.FolderType == FolderType.SHARE)
            {
                tempTags = tempTags.Concat(FromQueryAsync(tmpShareFileTagsQuery(FilesDbContext, tenantId, subject, FolderType.USER)));
                tempTags = tempTags.Concat(FromQueryAsync(tmpShareFolderTagsQuery(FilesDbContext, tenantId, subject, FolderType.USER)));
                tempTags = tempTags.Concat(FromQueryAsync(tmpShareSboxTagsQuery(FilesDbContext, tenantId, subject)));
            }
            else if (parentFolder.FolderType == FolderType.Privacy)
            {
                tempTags = tempTags.Concat(FromQueryAsync(tmpShareFileTagsQuery(FilesDbContext, tenantId, subject, FolderType.Privacy)));
                tempTags = tempTags.Concat(FromQueryAsync(tmpShareFolderTagsQuery(FilesDbContext, tenantId, subject, FolderType.Privacy)));
            }
            else if (parentFolder.FolderType == FolderType.Projects)
            {
                tempTags = tempTags.Concat(FromQueryAsync(projectsQuery(FilesDbContext, tenantId, subject)));
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
                result.Concat(tempTags);
            }

            var monitorFolderIdsInt = await monitorFolderIds.OfType<int>().ToListAsync();
            var subFoldersSqlQuery = getFolderQuery(FilesDbContext, monitorFolderIdsInt, deepSearch);

            monitorFolderIds = monitorFolderIds.Concat(subFoldersSqlQuery.Select(r => (object)r));

            var monitorFolderIdsStrings = await monitorFolderIds.Select(r => r.ToString()).ToListAsync();

            result.Concat(FromQueryAsync(newTagsForFoldersQuery(FilesDbContext, tenantId, subject, monitorFolderIdsStrings)));

            var where = (deepSearch ? await monitorFolderIds.ToArrayAsync().ConfigureAwait(false) : new object[] { parentFolder.ID })
                .Select(r => r.ToString())
                .ToList();

            result.Concat(FromQueryAsync(newTagsForFilesQuery(FilesDbContext, tenantId, subject, where)));

            if (parentFolder.FolderType == FolderType.USER || parentFolder.FolderType == FolderType.COMMON)
            {
                var folderIds = await getThirdpartyAccountQuery(FilesDbContext, tenantId, parentFolder.FolderType, subject).ToListAsync().ConfigureAwait(false);

                var thirdpartyFolderIds = folderIds.ConvertAll(r => "sbox-" + r)
                                                    .Concat(folderIds.ConvertAll(r => $"box-{r}"))
                                                    .Concat(folderIds.ConvertAll(r => $"dropbox-{r}"))
                                                    .Concat(folderIds.ConvertAll(r => $"spoint-{r}"))
                                                    .Concat(folderIds.ConvertAll(r => $"drive-{r}"))
                                                    .Concat(folderIds.ConvertAll(r => $"onedrive-{r}"))
                                                    .ToList();

                if (thirdpartyFolderIds.Count > 0)
                {
                    result.Concat(FromQueryAsync(newTagsForSBoxQuery(FilesDbContext, tenantId, subject, thirdpartyFolderIds)));
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
                .Select(r => new TagLinkData()
                {
                    Tag = new DbFilesTag
                    {
                        Name = r.Tag.Name,
                        Flag = r.Tag.Flag,
                        Owner = r.Tag.Owner,
                        Id = r.Tag.Id
                    },
                    Link = new DbFilesTagLink
                    {
                        TagCount = r.Link.TagCount,
                        EntryId = r.Link.EntryId,
                        EntryType = r.Link.EntryType
                    }
                })
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
                .Select(r => new TagLinkData()
                {
                    Tag = new DbFilesTag
                    {
                        Name = r.Tag.Name,
                        Flag = r.Tag.Flag,
                        Owner = r.Tag.Owner,
                        Id = r.Tag.Id
                    },
                    Link = new DbFilesTagLink
                    {
                        TagCount = r.Link.TagCount,
                        EntryId = r.Link.EntryId,
                        EntryType = r.Link.EntryType
                    }
                })
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var file in files)
            {
                yield return await ToTagAsync(file).ConfigureAwait(false);
            }
        }

        private async ValueTask<Tag> ToTagAsync(TagLinkData r)
        {
            var result = new Tag(r.Tag.Name, r.Tag.Flag, r.Tag.Owner, r.Link.TagCount)
            {
                EntryId = await MappingIDAsync(r.Link.EntryId).ConfigureAwait(false),
                EntryType = r.Link.EntryType,
                Id = r.Tag.Id,
            };

            return result;
        }
    }

    public class TagLinkData
    {
        public DbFilesTag Tag { get; set; }

        public DbFilesTagLink Link { get; set; }
    }
}