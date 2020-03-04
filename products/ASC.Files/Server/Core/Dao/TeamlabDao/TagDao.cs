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
    internal class TagDao : AbstractDao, ITagDao<int>
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

        public IEnumerable<Tag> GetTags(TagType tagType, IEnumerable<FileEntry<int>> fileEntries)
        {
            var filesId = fileEntries.Where(e => e.FileEntryType == FileEntryType.File).Select(e => MappingID(e.ID).ToString()).ToList();
            var foldersId = fileEntries.Where(e => e.FileEntryType == FileEntryType.Folder).Select(e => MappingID(e.ID).ToString()).ToList();

            var q = Query(FilesDbContext.Tag)
                .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
                .Where(r => r.Link.TenantId == r.Tag.TenantId)
                .Where(r => r.Tag.Flag == tagType)
                .Where(r => r.Link.EntryType == FileEntryType.File && filesId.Any(f => r.Link.EntryId == f)
                || r.Link.EntryType == FileEntryType.Folder && foldersId.Any(f => r.Link.EntryId == f));

            return FromQuery(q);
        }

        public IEnumerable<Tag> GetTags(int entryID, FileEntryType entryType, TagType tagType)
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
                Query(FilesDbContext.Tag)
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
                Query(FilesDbContext.Tag)
                .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
                .Where(r => r.Link.TenantId == r.Tag.TenantId)
                .Where(r => r.Tag.Flag == TagType.New && r.Link.CreateOn <= TenantUtil.DateTimeNow().AddMonths(-1));

            foreach (var row in mustBeDeleted)
            {
                var linksToRemove = Query(FilesDbContext.TagLink)
                    .Where(r => r.TagId == row.Link.TagId)
                    .Where(r => r.EntryId == row.Link.EntryId)
                    .Where(r => r.EntryType == row.Link.EntryType);
                FilesDbContext.TagLink.RemoveRange(linksToRemove);
            }

            FilesDbContext.SaveChanges();

            var tagsToRemove = Query(FilesDbContext.Tag)
                .Where(r => !Query(FilesDbContext.TagLink).Where(a => a.TagId == r.Id).Any());

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

            var id = Query(FilesDbContext.Tag)
                .Where(r => r.Name == tag.TagName)
                .Where(r => r.Owner == tag.Owner)
                .Where(r => r.Flag == tag.TagType)
                .Select(r => r.Id)
                .FirstOrDefault();

            if (id != 0)
            {
                var toDelete = Query(FilesDbContext.TagLink)
                    .Where(r => r.TagId == id)
                    .Where(r => r.EntryId == MappingID(tag.EntryId).ToString())
                    .Where(r => r.EntryType == tag.EntryType);

                FilesDbContext.TagLink.RemoveRange(toDelete);
                FilesDbContext.SaveChanges();

                var count = Query(FilesDbContext.TagLink).Where(r => r.TagId == id).Count();
                if (count == 0)
                {
                    var tagToDelete = Query(FilesDbContext.Tag).Where(r => r.Id == id);
                    FilesDbContext.Tag.RemoveRange(tagToDelete);
                    FilesDbContext.SaveChanges();
                }
            }
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, FileEntry<int> fileEntry)
        {
            return GetNewTags(subject, new List<FileEntry<int>>(1) { fileEntry });
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, IEnumerable<FileEntry<int>> fileEntries)
        {
            List<Tag> result;

            var tags = fileEntries.Select(r => new DbFilesTagLink
            {
                TenantId = TenantID,
                EntryId = MappingID((r as File<int>)?.ID ?? (r as Folder<int>)?.ID).ToString(),
                EntryType = (r.FileEntryType == FileEntryType.File) ? FileEntryType.File : FileEntryType.Folder
            })
            .ToList();

            var entryIds = tags.Select(r => r.EntryId).ToList();
            var entryTypes = tags.Select(r => (int)r.EntryType).ToList();

            var sqlQuery = Query(FilesDbContext.Tag)
                .Join(FilesDbContext.TagLink, r => r.Id, l => l.TagId, (tag, link) => new TagLinkData { Tag = tag, Link = link })
                .Where(r => r.Link.TenantId == r.Tag.TenantId)
                .Where(r => r.Tag.Flag == TagType.New)
                .Where(x => x.Link.EntryId != null)
                //.Where(r => tags.Any(t => t.TenantId == r.Link.TenantId && t.EntryId == r.Link.EntryId && t.EntryType == (int)r.Link.EntryType)); ;
                .Where(r => entryIds.Any(t => t == r.Link.EntryId) && entryTypes.Any(t => t == (int)r.Link.EntryType));

            if (subject != Guid.Empty)
            {
                sqlQuery = sqlQuery.Where(r => r.Tag.Owner == subject);
            }

            result = FromQuery(sqlQuery);

            return result;
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, Folder<int> parentFolder, bool deepSearch)
        {
            throw new NotImplementedException();
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
            services.TryAddScoped<ITagDao<int>, TagDao>();
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