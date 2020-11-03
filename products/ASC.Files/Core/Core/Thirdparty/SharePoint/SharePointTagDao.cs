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

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Files.Core.EF;
using ASC.Files.Thirdparty.Dropbox;
using ASC.Web.Core.Files;
using ASC.Web.Studio.Core;

using Microsoft.Extensions.Options;

namespace ASC.Files.Thirdparty.SharePoint
{
    internal class SharePointTagDao : SharePointDaoBase, ITagDao<string>
    {
        public SharePointTagDao(IServiceProvider serviceProvider, UserManager userManager, TenantManager tenantManager, TenantUtil tenantUtil, DbContextManager<FilesDbContext> dbContextManager, SetupInfo setupInfo, IOptionsMonitor<ILog> monitor, FileUtility fileUtility) : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility)
        {
        }

        public IEnumerable<Tag> GetTags(Guid subject, TagType tagType, IEnumerable<FileEntry<string>> fileEntries)
        {
            return null;
        }

        public IEnumerable<Tag> GetTags(TagType tagType, IEnumerable<FileEntry<string>> fileEntries)
        {
            return new List<Tag>();
        }

        public IEnumerable<Tag> GetTags(Guid owner, TagType tagType)
        {
            return new List<Tag>();
        }

        public IEnumerable<Tag> GetTags(string name, TagType tagType)
        {
            return new List<Tag>();
        }

        public IEnumerable<Tag> GetTags(string[] names, TagType tagType)
        {
            return new List<Tag>();
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, Folder<string> parentFolder, bool deepSearch)
        {

            var folderId = DaoSelector.ConvertId(parentFolder.ID);

            var fakeFolderId = parentFolder.ID.ToString();

            var entryIDs = FilesDbContext.ThirdpartyIdMapping
                       .Where(r => r.Id.StartsWith(fakeFolderId))
                       .Select(r => r.HashId)
                       .ToList();

            if (!entryIDs.Any()) return new List<Tag>();

            var q = FilesDbContext.Tag
                .Join(FilesDbContext.TagLink.DefaultIfEmpty(),
                r => new TagLink { TenantId = r.TenantId, Id = r.Id },
                r => new TagLink { TenantId = r.TenantId, Id = r.TagId },
                (tag, tagLink) => new { tag, tagLink },
                new TagLinkComparer())
                .Where(r => r.tag.TenantId == TenantID)
                .Where(r => r.tag.Flag == TagType.New)
                .Where(r => r.tagLink.TenantId == TenantID)
                .Where(r => entryIDs.Any(a => a == r.tagLink.EntryId));

            if (subject != Guid.Empty)
            {
                q = q.Where(r => r.tag.Owner == subject);
            }

            var tags = q
                .ToList()
                .Select(r => new Tag
                {
                    TagName = r.tag.Name,
                    TagType = r.tag.Flag,
                    Owner = r.tag.Owner,
                    EntryId = MappingID(r.tagLink.EntryId),
                    EntryType = r.tagLink.EntryType,
                    Count = r.tagLink.TagCount,
                    Id = r.tag.Id
                });

            if (deepSearch) return tags;

            var folderFileIds = new[] { fakeFolderId }
                .Concat(ProviderInfo.GetFolderFolders(folderId).Select(x => ProviderInfo.MakeId(x.ServerRelativeUrl)))
                .Concat(ProviderInfo.GetFolderFiles(folderId).Select(x => ProviderInfo.MakeId(x.ServerRelativeUrl)));

            return tags.Where(tag => folderFileIds.Contains(tag.EntryId.ToString()));
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, IEnumerable<FileEntry<string>> fileEntries)
        {
            return new List<Tag>();
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, FileEntry<string> fileEntry)
        {
            return new List<Tag>();
        }

        public IEnumerable<Tag> SaveTags(IEnumerable<Tag> tag)
        {
            return new List<Tag>();
        }

        public IEnumerable<Tag> SaveTags(Tag tag)
        {
            return new List<Tag>();
        }

        public void UpdateNewTags(IEnumerable<Tag> tag)
        {
        }

        public void UpdateNewTags(Tag tag)
        {
        }

        public void RemoveTags(IEnumerable<Tag> tag)
        {
        }

        public void RemoveTags(Tag tag)
        {
        }

        public IEnumerable<Tag> GetTags(string entryID, FileEntryType entryType, TagType tagType)
        {
            return new List<Tag>();
        }
    }

    public static class SharePointTagDaoExtention
    {
        public static DIHelper AddSharePointTagDaoService(this DIHelper services)
        {
            services.TryAddScoped<SharePointTagDao>();

            return services;
        }
    }
}
