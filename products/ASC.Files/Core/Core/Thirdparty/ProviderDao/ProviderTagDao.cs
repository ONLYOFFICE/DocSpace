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
            return TagDao.GetTags(subject, tagType, fileEntries);
        }


        public IEnumerable<Tag> GetTags(TagType tagType, IEnumerable<FileEntry<string>> fileEntries)
        {
            return TagDao.GetTags(tagType, fileEntries);
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, Folder<string> parentFolder, bool deepSearch)
        {
            return GetSelector(parentFolder.ID)
                .GetTagDao(parentFolder.ID)
                .GetNewTags(subject, parentFolder, deepSearch);
        }

        #region Only for Teamlab Documents

        public IEnumerable<Tag> GetNewTags(Guid subject, IEnumerable<FileEntry<string>> fileEntries)
        {
            return TagDao.GetNewTags(subject, fileEntries);
        }

        public IEnumerable<Tag> GetNewTags(Guid subject, FileEntry<string> fileEntry)
        {
            return TagDao.GetNewTags(subject, fileEntry);
        }

        public IEnumerable<Tag> GetTags(Guid owner, TagType tagType)
        {
            return TagDao.GetTags(owner, tagType);
        }

        public IEnumerable<Tag> GetTags(string name, TagType tagType)
        {
            return TagDao.GetTags(name, tagType);
        }

        public IEnumerable<Tag> GetTags(string[] names, TagType tagType)
        {
            return TagDao.GetTags(names, tagType);
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
            return TagDao.GetTags(entryID, entryType, tagType);
        }

        #endregion
    }
}