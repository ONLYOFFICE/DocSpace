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
using System.Threading.Tasks;

using ASC.Common;
using ASC.Core;
using ASC.Files.Core;
using ASC.Files.Core.Data;
using ASC.Files.Core.Thirdparty;

namespace ASC.Files.Thirdparty.ProviderDao
{
    [Scope]
    internal class ProviderTagDao : ProviderDaoBase, ITagDao<string>
    {
        public ProviderTagDao(
            IServiceProvider serviceProvider,
            TenantManager tenantManager,
            SecurityDao<string> securityDao,
            TagDao<string> tagDao,
            CrossDao crossDao)
            : base(serviceProvider, tenantManager, securityDao, tagDao, crossDao)
        {
        }

        public IEnumerable<Tag> GetTags(Guid subject, TagType tagType, IEnumerable<FileEntry<string>> fileEntries)
        {
            return GetTagsAsync(subject, tagType, fileEntries).ToListAsync().Result;
        }

        public IAsyncEnumerable<Tag> GetTagsAsync(Guid subject, TagType tagType, IEnumerable<FileEntry<string>> fileEntries)
        {
            return TagDao.GetTagsAsync(subject, tagType, fileEntries);
        }


        public IEnumerable<Tag> GetTags(TagType tagType, IEnumerable<FileEntry<string>> fileEntries)
        {
            return GetTagsAsync(tagType, fileEntries).ToListAsync().Result;
        }

        public IAsyncEnumerable<Tag> GetTagsAsync(TagType tagType, IEnumerable<FileEntry<string>> fileEntries)
        {
            return TagDao.GetTagsAsync(tagType, fileEntries);
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, Folder<string> parentFolder, bool deepSearch)
        {
            return GetNewTagsAsync(subject, parentFolder, deepSearch).ToListAsync().Result;
        }

        public IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, Folder<string> parentFolder, bool deepSearch)
        {
            return GetSelector(parentFolder.ID)
                .GetTagDao(parentFolder.ID)
                .GetNewTagsAsync(subject, parentFolder, deepSearch);
        }

        #region Only for Teamlab Documents

        public IEnumerable<Tag> GetNewTags(Guid subject, IEnumerable<FileEntry<string>> fileEntries)
        {
            return GetNewTagsAsync(subject, fileEntries).ToListAsync().Result;
        }

        public  IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, IEnumerable<FileEntry<string>> fileEntries)
        {
            return TagDao.GetNewTagsAsync(subject, fileEntries);
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, FileEntry<string> fileEntry)
        {
            return GetNewTagsAsync(subject, fileEntry).ToListAsync().Result;
        }

        public IAsyncEnumerable<Tag> GetNewTagsAsync(Guid subject, FileEntry<string> fileEntry)
        {
            return TagDao.GetNewTagsAsync(subject, fileEntry);
        }

        public IEnumerable<Tag> GetTags(Guid owner, TagType tagType)
        {
            return GetTagsAsync(owner, tagType).ToListAsync().Result;
        }

        public IAsyncEnumerable<Tag> GetTagsAsync(Guid owner, TagType tagType)
        {
            return TagDao.GetTagsAsync(owner, tagType);
        }

        public IEnumerable<Tag> GetTags(string name, TagType tagType)
        {
            return GetTagsAsync(name, tagType).ToListAsync().Result;
        }

        public IAsyncEnumerable<Tag> GetTagsAsync(string name, TagType tagType)
        {
            return TagDao.GetTagsAsync(name, tagType);
        }

        public IEnumerable<Tag> GetTags(string[] names, TagType tagType)
        {
            return GetTagsAsync(names, tagType).ToListAsync().Result;
        }

        public IAsyncEnumerable<Tag> GetTagsAsync(string[] names, TagType tagType)
        {
            return TagDao.GetTagsAsync(names, tagType);
        }


        public IEnumerable<Tag> SaveTags(IEnumerable<Tag> tag)
        {
            return TagDao.SaveTags(tag);
        }

        public IEnumerable<Tag> SaveTags(Tag tag)
        {
            return TagDao.SaveTags(tag);
        }

        public void UpdateNewTags(IEnumerable<Tag> tag)
        {
            TagDao.UpdateNewTags(tag);
        }

        public void UpdateNewTags(Tag tag)
        {
            TagDao.UpdateNewTags(tag);
        }

        public void RemoveTags(IEnumerable<Tag> tag)
        {
            TagDao.RemoveTags(tag);
        }

        public void RemoveTags(Tag tag)
        {
            TagDao.RemoveTags(tag);
        }

        public IEnumerable<Tag> GetTags(string entryID, FileEntryType entryType, TagType tagType)
        {
            return GetTagsAsync(entryID, entryType, tagType).ToListAsync().Result;
        }

        public IAsyncEnumerable<Tag> GetTagsAsync(string entryID, FileEntryType entryType, TagType tagType)
        {
            return TagDao.GetTagsAsync(entryID, entryType, tagType);
        }

        public IDictionary<object, IEnumerable<Tag>> GetTags(Guid subject, IEnumerable<TagType> tagType, IEnumerable<FileEntry<string>> fileEntries)
        {
            return GetTagsAsync(subject, tagType, fileEntries).Result;
        }

        public async Task<IDictionary<object, IEnumerable<Tag>>> GetTagsAsync(Guid subject, IEnumerable<TagType> tagType, IEnumerable<FileEntry<string>> fileEntries)
        {
            return await TagDao.GetTagsAsync(subject, tagType, fileEntries).ConfigureAwait(false);
        }

        #endregion
    }
}