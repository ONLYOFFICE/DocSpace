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

using ASC.Common;
using ASC.Common.Caching;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Files.Core.EF;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;
namespace ASC.Files.Core.Data
{
    [Scope]
    internal class TagDao<T> : AbstractDao, ITagDao<T>
    {
        private static readonly object syncRoot = new object();

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

        public IEnumerable<Tag> GetTags(Guid subject, TagType tagType, IEnumerable<FileEntry<T>> fileEntries)
        {
            var filesId = new HashSet<string>();
            var foldersId = new HashSet<string>();

            foreach (var f in fileEntries)
            {
                var id = MappingID(f.ID).ToString();
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

            return FromQuery(q);
        }

        public IDictionary<object, IEnumerable<Tag>> GetTags(Guid subject, IEnumerable<TagType> tagType, IEnumerable<FileEntry<T>> fileEntries)
        {
            var filesId = new HashSet<string>();
            var foldersId = new HashSet<string>();

            foreach (var f in fileEntries)
            {
                var id = MappingID(f.ID).ToString();
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
                var q = Query(FilesDbContext.Tag)
                    .Where(r => tagType.Contains(r.Flag))
                    .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
                    .Where(r => r.Link.TenantId == r.Tag.TenantId)
                    .Where(r => r.Link.EntryType == FileEntryType.File && filesId.Contains(r.Link.EntryId)
                    || r.Link.EntryType == FileEntryType.Folder && foldersId.Contains(r.Link.EntryId));

                if (subject != Guid.Empty)
                {
                    q = q.Where(r => r.Link.CreateBy == subject);
                }

                return FromQuery(q)
                    .GroupBy(r => r.EntryId)
                    .ToDictionary(r => r.Key, r => r.AsEnumerable());
            }

            return new Dictionary<object, IEnumerable<Tag>>();
        }

        public IEnumerable<Tag> GetTags(TagType tagType, IEnumerable<FileEntry<T>> fileEntries)
        {
            return GetTags(Guid.Empty, tagType, fileEntries);
        }


        public IEnumerable<Tag> GetTags(T entryID, FileEntryType entryType, TagType tagType)
        {
            var q = Query(FilesDbContext.Tag)
                .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
                .Where(r => r.Link.TenantId == r.Tag.TenantId)
                .Where(r => r.Link.EntryType == entryType)
                .Where(r => r.Link.EntryId == MappingID(entryID).ToString())
                .Where(r => r.Tag.Flag == tagType);

            return FromQuery(q);
        }

        public IEnumerable<Tag> GetTags(string[] names, TagType tagType)
        {
            if (names == null) throw new ArgumentNullException("names");

            var q = Query(FilesDbContext.Tag)
                .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
                .Where(r => r.Link.TenantId == r.Tag.TenantId)
                .Where(r => r.Tag.Owner == Guid.Empty)
                .Where(r => names.Contains(r.Tag.Name))
                .Where(r => r.Tag.Flag == tagType);

            return FromQuery(q);
        }

        public IEnumerable<Tag> GetTags(string name, TagType tagType)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            return GetTags(new[] { name }, tagType);
        }

        public IEnumerable<Tag> GetTags(Guid owner, TagType tagType)
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

            return FromQuery(q);
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

                result.AddRange(tags.Select(t => SaveTag(t, cacheTagId, createOn)));

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

                result.Add(SaveTag(tag, cacheTagId, createOn));

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

            var tagsToRemove = Query(FilesDbContext.Tag)
                .Where(r => !Query(FilesDbContext.TagLink).Any(a => a.TagId == r.Id));

            FilesDbContext.Tag.RemoveRange(tagsToRemove);
            FilesDbContext.SaveChanges();
        }

        private Tag SaveTag(Tag t, Dictionary<string, int> cacheTagId, DateTime createOn)
        {
            var cacheTagIdKey = string.Join("/", new[] { TenantID.ToString(), t.Owner.ToString(), t.TagName, ((int)t.TagType).ToString(CultureInfo.InvariantCulture) });

            if (!cacheTagId.TryGetValue(cacheTagIdKey, out var id))
            {
                id = FilesDbContext.Tag
                    .Where(r => r.Owner == t.Owner)
                    .Where(r => r.Name == t.TagName)
                    .Where(r => r.Flag == t.TagType)
                    .Select(r => r.Id)
                    .FirstOrDefault();

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
                    FilesDbContext.SaveChanges();
                    id = toAdd.Id;
                }

                cacheTagId.Add(cacheTagIdKey, id);
            }

            t.Id = id;

            var linkToInsert = new DbFilesTagLink
            {
                TenantId = TenantID,
                TagId = id,
                EntryId = MappingID(t.EntryId, true).ToString(),
                EntryType = t.EntryType,
                CreateBy = AuthContext.CurrentAccount.ID,
                CreateOn = createOn,
                TagCount = t.Count
            };

            FilesDbContext.AddOrUpdate(r => r.TagLink, linkToInsert);
            FilesDbContext.SaveChanges();

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
                    UpdateNewTagsInDb(tag, createOn);
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

                UpdateNewTagsInDb(tag, createOn);
            }
        }

        private void UpdateNewTagsInDb(Tag tag, DateTime createOn)
        {
            if (tag == null) return;

            var forUpdate = Query(FilesDbContext.TagLink)
                .Where(r => r.TagId == tag.Id)
                .Where(r => r.EntryType == tag.EntryType)
                .Where(r => r.EntryId == MappingID(tag.EntryId).ToString());

            foreach (var f in forUpdate)
            {
                f.CreateBy = AuthContext.CurrentAccount.ID;
                f.CreateOn = createOn;
                f.TagCount = tag.Count;
            }

            FilesDbContext.SaveChanges();
        }

        public void RemoveTags(IEnumerable<Tag> tags)
        {
            if (tags == null || !tags.Any()) return;

            lock (syncRoot)
            {
                using var tx = FilesDbContext.Database.BeginTransaction();
                foreach (var t in tags)
                {
                    RemoveTagInDb(t);
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
                RemoveTagInDb(tag);

                tx.Commit();
            }
        }

        private void RemoveTagInDb(Tag tag)
        {
            if (tag == null) return;

            var id = Query(FilesDbContext.Tag)
                .Where(r => r.Name == tag.TagName &&
                            r.Owner == tag.Owner &&
                            r.Flag == tag.TagType)
                .Select(r => r.Id)
                .FirstOrDefault();

            if (id != 0)
            {
                var entryId = MappingID(tag.EntryId).ToString();
                var toDelete = Query(FilesDbContext.TagLink)
                    .Where(r => r.TagId == id &&
                                r.EntryId == entryId &&
                                r.EntryType == tag.EntryType);

                FilesDbContext.TagLink.RemoveRange(toDelete);
                FilesDbContext.SaveChanges();

                var count = Query(FilesDbContext.TagLink).Count(r => r.TagId == id);
                if (count == 0)
                {
                    var tagToDelete = Query(FilesDbContext.Tag).Where(r => r.Id == id);
                    FilesDbContext.Tag.RemoveRange(tagToDelete);
                    FilesDbContext.SaveChanges();
                }
            }
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, FileEntry<T> fileEntry)
        {
            return GetNewTags(subject, new List<FileEntry<T>>(1) { fileEntry });
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, IEnumerable<FileEntry<T>> fileEntries)
        {
            var result = new List<Tag>();
            var tags = new List<DbFilesTagLink>();
            var entryIds = new HashSet<string>();
            var entryTypes = new HashSet<int>();

            foreach (var r in fileEntries)
            {
                var id = MappingID(r.ID).ToString();
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

            if (entryIds.Any())
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

                result = FromQuery(sqlQuery);
            }

            return result;
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, Folder<T> parentFolder, bool deepSearch)
        {
            if (parentFolder == null || EqualityComparer<T>.Default.Equals(parentFolder.ID, default(T)))
                throw new ArgumentException("folderId");

            var result = new List<Tag>();

            var monitorFolderIds = new object[] { parentFolder.ID }.AsEnumerable();

            var getBaseSqlQuery = new Func<IQueryable<TagLinkData>>(() =>
            {
                var fnResult = Query(FilesDbContext.Tag)
                    .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
                    .Where(r => r.Link.TenantId == r.Tag.TenantId)
                    .Where(r => r.Tag.Flag == TagType.New)
                    .Distinct();

                if (subject != Guid.Empty)
                {
                    fnResult = fnResult.Where(r => r.Tag.Owner == subject);
                }

                return fnResult;
            });

            var tempTags = Enumerable.Empty<Tag>();

            if (parentFolder.FolderType == FolderType.SHARE)
            {
                var shareQuery =
                    new Func<IQueryable<TagLinkData>>(() => getBaseSqlQuery().Where(
                        r => FilesDbContext.Security
                        .Any(a => a.TenantId == TenantID && a.EntryId == r.Link.EntryId && a.EntryType == r.Link.EntryType)));

                var tmpShareFileTags =
                    shareQuery()
                    .Join(FilesDbContext.Files, r => Regex.IsMatch(r.Link.EntryId, "^[0-9]+$") ? Convert.ToInt32(r.Link.EntryId) : -1, f => f.Id, (tagLink, file) => new { tagLink, file })
                    .Where(r => r.file.TenantId == TenantID && r.file.CreateBy != subject && r.tagLink.Link.EntryType == FileEntryType.File)
                    .Select(r => new
                    {
                        r.tagLink,
                        root = FilesDbContext.Folders
                            .Join(FilesDbContext.Tree, a => a.Id, b => b.ParentId, (folder, tree) => new { folder, tree })
                            .Where(x => x.folder.TenantId == TenantID && x.tree.FolderId == r.file.FolderId)
                            .OrderByDescending(r => r.tree.Level)
                            .Select(r => r.folder)
                            .Take(1)
                            .FirstOrDefault()
                    })
                    .Where(r => r.root.FolderType == FolderType.USER)
                    .Select(r => r.tagLink)
                    .Distinct();

                tempTags = tempTags.Concat(FromQuery(tmpShareFileTags));


                var tmpShareFolderTags =
                    shareQuery()
                    .Join(FilesDbContext.Folders, r => Regex.IsMatch(r.Link.EntryId, "^[0-9]+$") ? Convert.ToInt32(r.Link.EntryId) : -1, f => f.Id, (tagLink, folder) => new { tagLink, folder })
                    .Where(r => r.folder.TenantId == TenantID && r.folder.CreateBy != subject && r.tagLink.Link.EntryType == FileEntryType.Folder)
                    .Select(r => new
                    {
                        r.tagLink,
                        root = FilesDbContext.Folders
                            .Join(FilesDbContext.Tree, a => a.Id, b => b.ParentId, (folder, tree) => new { folder, tree })
                            .Where(x => x.folder.TenantId == TenantID)
                            .Where(x => x.tree.FolderId == r.folder.ParentId)
                            .OrderByDescending(r => r.tree.Level)
                            .Select(r => r.folder)
                            .Take(1)
                            .FirstOrDefault()
                    })
                    .Where(r => r.root.FolderType == FolderType.USER)
                    .Select(r => r.tagLink)
                    .Distinct();

                tempTags = tempTags.Concat(FromQuery(tmpShareFolderTags));

                var tmpShareSboxTags =
                    shareQuery()
                    .Join(FilesDbContext.ThirdpartyIdMapping, r => r.Link.EntryId, r => r.HashId, (tagLink, mapping) => new { tagLink, mapping })
                    .Where(r => r.mapping.TenantId == r.tagLink.Link.TenantId)
                    .Join(FilesDbContext.ThirdpartyAccount, r => r.mapping.TenantId, r => r.TenantId, (tagLinkMapping, account) => new { tagLinkMapping.tagLink, tagLinkMapping.mapping, account })
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
                    .Distinct();

                tempTags = tempTags.Concat(FromQuery(tmpShareSboxTags));
            }
            else if (parentFolder.FolderType == FolderType.Privacy)
            {
                var shareQuery =
                    new Func<IQueryable<TagLinkData>>(() => getBaseSqlQuery().Where(
                        r => FilesDbContext.Security
                        .Any(a => a.TenantId == TenantID &&
                                  a.EntryId == r.Link.EntryId &&
                                  a.EntryType == r.Link.EntryType)));

                var tmpShareFileTags =
                    shareQuery()
                    .Join(FilesDbContext.Files, r => Regex.IsMatch(r.Link.EntryId, "^[0-9]+$") ? Convert.ToInt32(r.Link.EntryId) : -1, f => f.Id, (tagLink, file) => new { tagLink, file })
                    .Where(r => r.file.TenantId == TenantID &&
                                r.file.CreateBy != subject &&
                                r.tagLink.Link.EntryType == FileEntryType.File)
                    .Select(r => new
                    {
                        r.tagLink,
                        root = FilesDbContext.Folders
                            .Join(FilesDbContext.Tree, a => a.Id, b => b.ParentId, (folder, tree) => new { folder, tree })
                            .Where(x => x.folder.TenantId == TenantID && x.tree.FolderId == r.file.FolderId)
                            .OrderByDescending(r => r.tree.Level)
                            .Select(r => r.folder)
                            .Take(1)
                            .FirstOrDefault()
                    })
                    .Where(r => r.root.FolderType == FolderType.Privacy)
                    .Select(r => r.tagLink)
                    .Distinct();

                tempTags = tempTags.Concat(FromQuery(tmpShareFileTags));


                var tmpShareFolderTags =
                                    shareQuery()
                                    .Join(FilesDbContext.Folders, r => Regex.IsMatch(r.Link.EntryId, "^[0-9]+$") ? Convert.ToInt32(r.Link.EntryId) : -1, f => f.Id, (tagLink, folder) => new { tagLink, folder })
                                    .Where(r => r.folder.TenantId == TenantID
                                             && r.folder.CreateBy != subject
                                             && r.tagLink.Link.EntryType == FileEntryType.Folder)
                                    .Select(r => new
                                    {
                                        r.tagLink,
                                        root = FilesDbContext.Folders
                                            .Join(FilesDbContext.Tree, a => a.Id, b => b.ParentId, (folder, tree) => new { folder, tree })
                                            .Where(x => x.folder.TenantId == TenantID && x.tree.FolderId == r.folder.ParentId)
                                            .OrderByDescending(r => r.tree.Level)
                                            .Select(r => r.folder)
                                            .Take(1)
                                            .FirstOrDefault()
                                    })
                                    .Where(r => r.root.FolderType == FolderType.Privacy)
                                    .Select(r => r.tagLink)
                                    .Distinct();

                tempTags = tempTags.Concat(FromQuery(tmpShareFolderTags));
            }
            else if (parentFolder.FolderType == FolderType.Projects)
            {
                var q = getBaseSqlQuery()
                    .Join(FilesDbContext.BunchObjects, r => r.Link.TenantId, r => r.TenantId, (tagLink, bunch) => new { tagLink, bunch })
                    .Where(r => r.bunch.LeftNode == r.tagLink.Link.EntryId &&
                                r.tagLink.Link.EntryType == FileEntryType.Folder &&
                                r.bunch.RightNode.StartsWith("projects/project/"))
                    .Select(r => r.tagLink)
                    .Distinct();
                tempTags = tempTags.Concat(FromQuery(q));
            }

            if (tempTags.Any())
            {
                if (!deepSearch) return tempTags;

                monitorFolderIds = monitorFolderIds.Concat(tempTags.Where(x => x.EntryType == FileEntryType.Folder).Select(x => x.EntryId));
                result.AddRange(tempTags);
            }

            var monitorFolderIdsInt = monitorFolderIds.OfType<int>().ToList();
            var subFoldersSqlQuery =
                FilesDbContext.Tree
                .Where(r => monitorFolderIdsInt.Contains(r.ParentId));

            if (!deepSearch)
            {
                subFoldersSqlQuery = subFoldersSqlQuery.Where(r => r.Level == 1);
            }

            monitorFolderIds = monitorFolderIds.Concat(subFoldersSqlQuery.Select(r => r.FolderId).ToList().ConvertAll(r => (object)r));

            var monitorFolderIdsStrings = monitorFolderIds.Select(r => r.ToString()).ToList();

            var newTagsForFolders = getBaseSqlQuery()
                .Where(r => monitorFolderIdsStrings.Contains(r.Link.EntryId))
                .Where(r => r.Link.EntryType == FileEntryType.Folder);

            result.AddRange(FromQuery(newTagsForFolders));

            var where = (deepSearch ? monitorFolderIds.ToArray() : new object[] { parentFolder.ID })
                .Select(r => r.ToString())
                .ToList();

            var newTagsForFiles =
                getBaseSqlQuery()
                .Join(FilesDbContext.Files, r => Regex.IsMatch(r.Link.EntryId, "^[0-9]+$") ? Convert.ToInt32(r.Link.EntryId) : -1, r => r.Id, (tagLink, file) => new { tagLink, file })
                .Where(r => r.file.TenantId == r.tagLink.Link.TenantId)
                .Where(r => where.Contains(r.file.FolderId.ToString()))
                .Where(r => r.tagLink.Link.EntryType == FileEntryType.File)
                .Select(r => r.tagLink)
                .Distinct();

            result.AddRange(FromQuery(newTagsForFiles));

            if (parentFolder.FolderType == FolderType.USER || parentFolder.FolderType == FolderType.COMMON)
            {
                var folderType = parentFolder.FolderType;

                var querySelect = FilesDbContext.ThirdpartyAccount
                    .Where(r => r.TenantId == TenantID)
                    .Where(r => r.FolderType == folderType);

                if (folderType == FolderType.USER)
                {
                    querySelect = querySelect.Where(r => r.UserId == subject);
                }

                var folderIds = querySelect.Select(r => r.Id).ToList();
                var thirdpartyFolderIds = folderIds.ConvertAll(r => "sbox-" + r)
                                                    .Concat(folderIds.ConvertAll(r => $"box-{r}"))
                                                    .Concat(folderIds.ConvertAll(r => $"dropbox-{r}"))
                                                    .Concat(folderIds.ConvertAll(r => $"spoint-{r}"))
                                                    .Concat(folderIds.ConvertAll(r => $"drive-{r}"))
                                                    .Concat(folderIds.ConvertAll(r => $"onedrive-{r}"));

                if (thirdpartyFolderIds.Any())
                {
                    var newTagsForSBox = getBaseSqlQuery()
                        .Join(FilesDbContext.ThirdpartyIdMapping, r => r.Link.EntryId, r => r.HashId, (tagLink, mapping) => new { tagLink, mapping })
                        .Where(r => r.mapping.TenantId == TenantID &&
                                    thirdpartyFolderIds.Contains(r.mapping.Id) &&
                                    r.tagLink.Tag.Owner == subject &&
                                    r.tagLink.Link.EntryType == FileEntryType.Folder)
                        .Select(r => r.tagLink)
                        .Distinct();

                    result.AddRange(FromQuery(newTagsForSBox));
                }
            }

            return result;
        }

        protected List<Tag> FromQuery(IQueryable<TagLinkData> dbFilesTags)
        {
            return dbFilesTags
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
                .AsEnumerable()
                .ToList()
                .ConvertAll(ToTag);
        }

        private Tag ToTag(TagLinkData r)
        {
            var result = new Tag(r.Tag.Name, r.Tag.Flag, r.Tag.Owner, r.Link.TagCount)
            {
                EntryId = MappingID(r.Link.EntryId),
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