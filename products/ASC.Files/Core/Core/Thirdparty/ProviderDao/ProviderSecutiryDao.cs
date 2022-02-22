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
using ASC.Files.Core.Security;
using ASC.Files.Core.Thirdparty;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Files.Thirdparty.ProviderDao
{
    [Scope]
    internal class ProviderSecurityDao : ProviderDaoBase, ISecurityDao<string>
    {
        public ProviderSecurityDao(
            IServiceProvider serviceProvider,
            TenantManager tenantManager,
            SecurityDao<string> securityDao,
            TagDao<string> tagDao,
            CrossDao crossDao)
            : base(serviceProvider, tenantManager, securityDao, tagDao, crossDao)
        {
        }

        public Task SetShareAsync(FileShareRecord r)
        {
            return SecurityDao.SetShareAsync(r);
        }

        public async Task<IEnumerable<FileShareRecord>> GetSharesAsync(IEnumerable<FileEntry<string>> entries)
        {
            var result = new List<FileShareRecord>();

            var files = entries.Where(x => x.FileEntryType == FileEntryType.File).ToArray();
            var folders = entries.Where(x => x.FileEntryType == FileEntryType.Folder).ToList();

            if (files.Length > 0)
            {
                var folderIds = files.Select(x => ((File<string>)x).FolderID).Distinct();
                foreach (var folderId in folderIds)
                {
                    await GetFoldersForShareAsync(folderId, folders);
                }

                var pureShareRecords = await SecurityDao.GetPureShareRecordsAsync(files);
                if (pureShareRecords != null)
                {
                    foreach (var pureShareRecord in pureShareRecords)
                    {
                        if (pureShareRecord == null) continue;
                        pureShareRecord.Level = -1;
                        result.Add(pureShareRecord);
                    }
                }
            }

            result.AddRange(await GetShareForFoldersAsync(folders));

            return result;
        }

        public Task<IEnumerable<FileShareRecord>> GetSharesAsync(FileEntry<string> entry)
        {
            var result = new List<FileShareRecord>();

            if (entry == null) return Task.FromResult<IEnumerable<FileShareRecord>>(result);


            return InternalGetSharesAsync(entry);
        }

        private async Task<IEnumerable<FileShareRecord>> InternalGetSharesAsync(FileEntry<string> entry)
        {
            var result = new List<FileShareRecord>();

            var folders = new List<FileEntry<string>>();
            if (entry is Folder<string> entryFolder)
            {
                folders.Add(entryFolder);
            }

            if (entry is File<string> file)
            {
                await GetFoldersForShareAsync(file.FolderID, folders);

                var pureShareRecords = await SecurityDao.GetPureShareRecordsAsync(entry);
                if (pureShareRecords != null)
                {
                    foreach (var pureShareRecord in pureShareRecords)
                    {
                        if (pureShareRecord == null) continue;
                        pureShareRecord.Level = -1;
                        result.Add(pureShareRecord);
                    }
                }
            }

            result.AddRange(await GetShareForFoldersAsync(folders));

            return result;
        }

        private Task GetFoldersForShareAsync(string folderId, ICollection<FileEntry<string>> folders)
        {
            var selector = GetSelector(folderId);
            var folderDao = selector.GetFolderDao(folderId);
            if (folderDao == null) return Task.CompletedTask;

            return InternalGetFoldersForShareAsync(folderId, folders, folderDao, selector);
        }

        private async Task InternalGetFoldersForShareAsync(string folderId, ICollection<FileEntry<string>> folders, IFolderDao<string> folderDao, IDaoSelector selector)
        {
            var folder = await folderDao.GetFolderAsync(selector.ConvertId(folderId));
            if (folder != null) folders.Add(folder);
        }

        private Task<List<FileShareRecord>> GetShareForFoldersAsync(IReadOnlyCollection<FileEntry<string>> folders)
        {
            if (folders.Count > 0) return Task.FromResult(new List<FileShareRecord>());

            return InternalGetShareForFoldersAsync(folders);
        }

        private async Task<List<FileShareRecord>> InternalGetShareForFoldersAsync(IReadOnlyCollection<FileEntry<string>> folders)
        {
            var result = new List<FileShareRecord>();

            foreach (var folder in folders)
            {
                var selector = GetSelector(folder.ID);
                var folderDao = selector.GetFolderDao(folder.ID);
                if (folderDao == null) continue;

                var parentFolders = await folderDao.GetParentFoldersAsync(selector.ConvertId(folder.ID));
                if (parentFolders == null || parentFolders.Count > 0) continue;

                parentFolders.Reverse();
                var pureShareRecords = await GetPureShareRecordsAsync(parentFolders);
                if (pureShareRecords == null) continue;

                foreach (var pureShareRecord in pureShareRecords)
                {
                    if (pureShareRecord == null) continue;
                    var f = ServiceProvider.GetService<Folder<string>>();
                    f.ID = pureShareRecord.EntryId.ToString();

                    pureShareRecord.Level = parentFolders.IndexOf(f);
                    pureShareRecord.EntryId = folder.ID;
                    result.Add(pureShareRecord);
                }
            }

            return result;
        }

        public Task RemoveSubjectAsync(Guid subject)
        {
            return SecurityDao.RemoveSubjectAsync(subject);
        }

        public ValueTask<List<FileShareRecord>> GetSharesAsync(IEnumerable<Guid> subjects)
        {
            return SecurityDao.GetSharesAsync(subjects);
        }

        public Task<IEnumerable<FileShareRecord>> GetPureShareRecordsAsync(IEnumerable<FileEntry<string>> entries)
        {
            return SecurityDao.GetPureShareRecordsAsync(entries);
        }

        public Task<IEnumerable<FileShareRecord>> GetPureShareRecordsAsync(FileEntry<string> entry)
        {
            return SecurityDao.GetPureShareRecordsAsync(entry);
        }

        public Task DeleteShareRecordsAsync(IEnumerable<FileShareRecord> records)
        {
            return SecurityDao.DeleteShareRecordsAsync(records);
        }

        public ValueTask<bool> IsSharedAsync(object entryId, FileEntryType type)
        {
            return SecurityDao.IsSharedAsync(entryId, type);
        }
    }
}