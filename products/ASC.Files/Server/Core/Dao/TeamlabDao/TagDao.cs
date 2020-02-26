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

using ASC.Common;
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
    internal class TagDao : AbstractDao, ITagDao
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
            IServiceProvider serviceProvider)
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
                  serviceProvider)
        {
        }

        public IEnumerable<Tag> GetTags(TagType tagType, IEnumerable<FileEntry> fileEntries)
        {
            var filesId = fileEntries.Where(e => e.FileEntryType == FileEntryType.File).Select(e => MappingID(e.ID)).ToList();
            var foldersId = fileEntries.Where(e => e.FileEntryType == FileEntryType.Folder).Select(e => MappingID(e.ID)).ToList();

            var q = Query(r => r.Tag)
                .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
                .Where(r => r.Link.TenantId == r.Tag.TenantId)
                .Where(r => r.Tag.Flag == tagType)
                .Where(r => r.Link.EntryType == FileEntryType.File && filesId.Any(f => r.Link.EntryId == f.ToString())
                || r.Link.EntryType == FileEntryType.Folder && foldersId.Any(f => r.Link.EntryId == f.ToString()));

            return FromQuery(q);
        }

        public IEnumerable<Tag> GetTags(object entryID, FileEntryType entryType, TagType tagType)
        {
            var q = Query(r => r.Tag)
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

            var q = Query(r => r.Tag)
                .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
                .Where(r => r.Link.TenantId == r.Tag.TenantId)
                .Where(r => r.Tag.Owner == Guid.Empty)
                .Where(r => names.Any(n => r.Tag.Name == n))
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
                Query(r => r.Tag)
                .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
                .Where(r => r.Link.TenantId == r.Tag.TenantId)
                .Where(r => r.Tag.Flag == tagType);

            if (owner != Guid.Empty)
            {
                q = q.Where(r => r.Tag.Owner == owner);
            }

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
                Query(r => r.Tag)
                .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
                .Where(r => r.Link.TenantId == r.Tag.TenantId)
                .Where(r => r.Tag.Flag == TagType.New && r.Link.CreateOn <= TenantUtil.DateTimeNow().AddMonths(-1));

            foreach (var row in mustBeDeleted)
            {
                var linksToRemove = Query(r => r.TagLink)
                    .Where(r => r.TagId == row.Link.TagId)
                    .Where(r => r.EntryId == row.Link.EntryId)
                    .Where(r => r.EntryType == row.Link.EntryType);
                FilesDbContext.TagLink.RemoveRange(linksToRemove);
            }

            FilesDbContext.SaveChanges();

            var tagsToRemove = Query(r => r.Tag)
                .Where(r => !Query(a => a.TagLink).Where(a => a.TagId == r.Id).Any());

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
                using (var tx = FilesDbContext.Database.BeginTransaction())
                {
                    var createOn = TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow());

                    foreach (var tag in tags)
                    {
                        UpdateNewTagsInDb(tag, createOn);
                    }
                    tx.Commit();
                }
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

            var forUpdate = Query(r => r.TagLink)
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
                using (var tx = FilesDbContext.Database.BeginTransaction())
                {
                    foreach (var t in tags)
                    {
                        RemoveTagInDb(t);
                    }
                    tx.Commit();
                }
            }
        }

        public void RemoveTags(Tag tag)
        {
            if (tag == null) return;

            lock (syncRoot)
            {
                using (var tx = FilesDbContext.Database.BeginTransaction())
                {
                    RemoveTagInDb(tag);

                    tx.Commit();
                }
            }
        }

        private void RemoveTagInDb(Tag tag)
        {
            if (tag == null) return;

            var id = Query(r => r.Tag)
                .Where(r => r.Name == tag.TagName)
                .Where(r => r.Owner == tag.Owner)
                .Where(r => r.Flag == tag.TagType)
                .Select(r => r.Id)
                .FirstOrDefault();

            if (id != 0)
            {
                var toDelete = Query(r => r.TagLink)
                    .Where(r => r.TagId == id)
                    .Where(r => r.EntryId == MappingID(tag.EntryId).ToString())
                    .Where(r => r.EntryType == tag.EntryType);

                FilesDbContext.TagLink.RemoveRange(toDelete);
                FilesDbContext.SaveChanges();

                var count = Query(r => r.TagLink).Where(r => r.TagId == id).Count();
                if (count == 0)
                {
                    var tagToDelete = Query(r => r.Tag).Where(r => r.Id == id);
                    FilesDbContext.Tag.RemoveRange(tagToDelete);
                    FilesDbContext.SaveChanges();
                }
            }
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, FileEntry fileEntry)
        {
            return GetNewTags(subject, new List<FileEntry>(1) { fileEntry });
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, IEnumerable<FileEntry> fileEntries)
        {
            List<Tag> result;

            var tags = fileEntries.Select(r => new DbFilesTagLink
            {
                TenantId = TenantID,
                EntryId = MappingID(r.ID).ToString(),
                EntryType = (r.FileEntryType == FileEntryType.File) ? FileEntryType.File : FileEntryType.Folder
            });

            var sqlQuery = Query(r => r.Tag)
                .Distinct()
                .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
                .Where(r => r.Link.TenantId == r.Tag.TenantId)
                .Where(r => r.Tag.Flag == TagType.New)
                .Where(x => x.Link.EntryId != null)
                .Where(r => tags.Any(t => t.TenantId == r.Link.TenantId && t.EntryId == r.Link.EntryId && t.EntryType == r.Link.EntryType));

            if (subject != Guid.Empty)
            {
                sqlQuery = sqlQuery.Where(r => r.Tag.Owner == subject);
            }

            result = FromQuery(sqlQuery);

            return result;
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, Folder parentFolder, bool deepSearch)
        {
            if (parentFolder == null || parentFolder.ID == null)
                throw new ArgumentException("folderId");

            var result = new List<Tag>();

            var monitorFolderIds = new[] { parentFolder.ID }.AsEnumerable();

            var getBaseSqlQuery = new Func<IQueryable<TagLinkData>>(() =>
            {
                var fnResult = Query(r => r.Tag)
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
                        .Where(a => a.TenantId == r.Link.TenantId)
                        .Where(a => a.EntryId == r.Link.EntryId)
                        .Where(a => a.EntryType == r.Link.EntryType)
                        .Any()));

                var tmpShareFileTags =
                    shareQuery()
                    .Join(FilesDbContext.Files, r => r.Link.EntryId, f => f.Id.ToString(), (tagLink, file) => new { tagLink, file })
                    .Where(r => r.file.TenantId == r.tagLink.Link.TenantId)
                    .Where(r => r.file.CreateBy != subject)
                    .Where(r => r.tagLink.Link.EntryType == FileEntryType.File)
                    .Select(r => new
                    {
                        r.tagLink,
                        root = FilesDbContext.Folders
                            .Join(FilesDbContext.Tree, a => a.Id, b => b.ParentId, (folder, tree) => new { folder, tree })
                            .Where(x => x.folder.TenantId == r.file.TenantId)
                            .Where(x => x.tree.FolderId == r.file.FolderId)
                            .OrderByDescending(r => r.tree.Level)
                            .Select(r => r.folder)
                            .FirstOrDefault()
                    })
                    .Where(r => r.root.FolderType == FolderType.USER)
                    .Select(r => r.tagLink);

                tempTags = tempTags.Concat(FromQuery(tmpShareFileTags));


                var tmpShareFolderTags =
                    shareQuery()
                    .Join(FilesDbContext.Folders, r => r.Link.EntryId, f => f.Id.ToString(), (tagLink, folder) => new { tagLink, folder })
                    .Where(r => r.folder.TenantId == r.tagLink.Link.TenantId)
                    .Where(r => r.folder.CreateBy != subject)
                    .Where(r => r.tagLink.Link.EntryType == FileEntryType.Folder)
                    .Select(r => new
                    {
                        r.tagLink,
                        root = FilesDbContext.Folders
                            .Join(FilesDbContext.Tree, a => a.Id, b => b.ParentId, (folder, tree) => new { folder, tree })
                            .Where(x => x.folder.TenantId == r.folder.TenantId)
                            .Where(x => x.tree.FolderId == r.folder.ParentId)
                            .OrderByDescending(r => r.tree.Level)
                            .Select(r => r.folder)
                            .FirstOrDefault()
                    })
                    .Where(r => r.root.FolderType == FolderType.USER)
                    .Select(r => r.tagLink);

                tempTags = tempTags.Concat(FromQuery(tmpShareFolderTags));

                var tmpShareSboxTags =
                    shareQuery()
                    .Join(FilesDbContext.ThirdpartyIdMapping, r => r.Link.EntryId, r => r.HashId, (tagLink, mapping) => new { tagLink, mapping })
                    .Where(r => r.mapping.TenantId == r.tagLink.Link.TenantId)
                    .Join(FilesDbContext.ThirdpartyAccount, r => r.mapping.TenantId, r => r.TenantId, (tagLinkMapping, account) => new { tagLinkMapping.tagLink, tagLinkMapping.mapping, account })
                    .Where(r => r.account.UserId != subject)
                    .Where(r => r.account.FolderType == FolderType.USER)
                    .Where(r =>
                    r.mapping.Id.StartsWith($"'sbox-'{r.account.Id}") ||
                    r.mapping.Id.StartsWith($"'box-'{r.account.Id}") ||
                    r.mapping.Id.StartsWith($"'dropbox-'{r.account.Id}") ||
                    r.mapping.Id.StartsWith($"'spoint-'{r.account.Id}") ||
                    r.mapping.Id.StartsWith($"'drive-'{r.account.Id}") ||
                    r.mapping.Id.StartsWith($"'onedrive-'{r.account.Id}")
                    )
                    .Select(r => r.tagLink);

                tempTags = tempTags.Concat(FromQuery(tmpShareSboxTags));
            }
            else if (parentFolder.FolderType == FolderType.Projects)
            {
                var q = getBaseSqlQuery()
                    .Join(FilesDbContext.BunchObjects, r => r.Link.TenantId, r => r.TenantId, (tagLink, bunch) => new { tagLink, bunch })
                    .Where(r => r.bunch.LeftNode == r.tagLink.Link.EntryId)
                    .Where(r => r.tagLink.Link.EntryType == FileEntryType.Folder)
                    .Where(r => r.bunch.RightNode.StartsWith("projects/project/"))
                    .Select(r => r.tagLink);
                tempTags = tempTags.Concat(FromQuery(q));
            }

            if (tempTags.Any())
            {
                if (!deepSearch) return tempTags;

                monitorFolderIds = monitorFolderIds.Concat(tempTags.Where(x => x.EntryType == FileEntryType.Folder).Select(x => x.EntryId));
                result.AddRange(tempTags);
            }

            var monitorFolderIdsInt = monitorFolderIds.Select(r => Convert.ToInt32(r)).ToList();
            var subFoldersSqlQuery =
                FilesDbContext.Tree
                .Where(r => monitorFolderIdsInt.Any(a => r.ParentId == a));

            if (!deepSearch)
            {
                subFoldersSqlQuery = subFoldersSqlQuery.Where(r => r.Level == 1);
            }

            monitorFolderIds = monitorFolderIds.Concat(subFoldersSqlQuery.Select(r => r.FolderId).ToList().ConvertAll(r => (object)r));

            var newTagsForFolders = getBaseSqlQuery()
                .Where(r => monitorFolderIds.Any(a => r.Link.EntryId == a.ToString()))
                .Where(r => r.Link.EntryType == FileEntryType.Folder);

            result.AddRange(FromQuery(newTagsForFolders));

            var newTagsForFiles =
                getBaseSqlQuery()
                .Join(FilesDbContext.Files, r => r.Link.EntryId, r => r.Id.ToString(), (tagLink, file) => new { tagLink, file })
                .Where(r => r.file.TenantId == r.tagLink.Link.TenantId)
                .Where(r => (deepSearch ? monitorFolderIds.ToArray() : new[] { parentFolder.ID }).Any(a => r.file.FolderId == (int)a))
                .Where(r => r.tagLink.Link.EntryType == FileEntryType.File)
                .Select(r => r.tagLink);

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

                var newTagsForSBox = getBaseSqlQuery()
                    .Join(FilesDbContext.ThirdpartyIdMapping, r => r.Link.EntryId, r => r.HashId, (tagLink, mapping) => new { tagLink, mapping })
                    .Where(r => r.mapping.TenantId == r.tagLink.Link.TenantId)
                    .Where(r => thirdpartyFolderIds.Any(a => r.mapping.Id == a))
                    .Where(r => r.tagLink.Tag.Owner == subject)
                    .Where(r => r.tagLink.Link.EntryType == FileEntryType.Folder)
                    .Select(r => r.tagLink);

                result.AddRange(FromQuery(newTagsForSBox));
            }

            return result;
        }

        protected List<Tag> FromQuery(IQueryable<TagLinkData> dbFilesTags)
        {
            return dbFilesTags
                .ToList()
                .Select(ToTag)
                .ToList();
        }

        private Tag ToTag(TagLinkData r)
        {
            var result = new Tag(r.Tag.Name, r.Tag.Flag, r.Tag.Owner, null, r.Link.TagCount)
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
    public static class TagDaoExtention
    {
        public static DIHelper AddTagDaoService(this DIHelper services)
        {
            services.TryAddScoped<ITagDao, TagDao>();
            return services
                .AddUserManagerService()
                .AddFilesDbContextService()
                .AddTenantManagerService()
                .AddTenantUtilService()
                .AddSetupInfo()
                .AddTenantExtraService()
                .AddTenantStatisticsProviderService()
                .AddCoreBaseSettingsService()
                .AddCoreConfigurationService()
                .AddSettingsManagerService()
                .AddAuthContextService();
        }
    }
}