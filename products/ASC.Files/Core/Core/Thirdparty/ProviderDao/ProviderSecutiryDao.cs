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

        public void SetShare(FileShareRecord r)
        {
            SecurityDao.SetShare(r);
        }

        public IEnumerable<FileShareRecord> GetShares(IEnumerable<FileEntry<string>> entries)
        {
            var result = new List<FileShareRecord>();

            var files = entries.Where(x => x.FileEntryType == FileEntryType.File).ToArray();
            var folders = entries.Where(x => x.FileEntryType == FileEntryType.Folder).ToList();

            if (files.Length > 0)
            {
                var folderIds = files.Select(x => ((File<string>)x).FolderID).Distinct();
                foreach (var folderId in folderIds)
                {
                    GetFoldersForShare(folderId, folders);
                }

                var pureShareRecords = SecurityDao.GetPureShareRecords(files);
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

            result.AddRange(GetShareForFolders(folders));

            return result;
        }

        public IEnumerable<FileShareRecord> GetShares(FileEntry<string> entry)
        {
            var result = new List<FileShareRecord>();

            if (entry == null) return result;


            var folders = new List<FileEntry<string>>();
            if (entry is Folder<string> entryFolder)
            {
                folders.Add(entryFolder);
            }

            if (entry is File<string> file)
            {
                GetFoldersForShare(file.FolderID, folders);

                var pureShareRecords = SecurityDao.GetPureShareRecords(entry);
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

            result.AddRange(GetShareForFolders(folders));

            return result;
        }

        private void GetFoldersForShare(string folderId, ICollection<FileEntry<string>> folders)
        {
            var selector = GetSelector(folderId);
            var folderDao = selector.GetFolderDao(folderId);
            if (folderDao == null) return;

            var folder = folderDao.GetFolder(selector.ConvertId(folderId));
            if (folder != null) folders.Add(folder);
        }

        private List<FileShareRecord> GetShareForFolders(IReadOnlyCollection<FileEntry<string>> folders)
        {
            if (folders.Count > 0) return new List<FileShareRecord>();

            var result = new List<FileShareRecord>();

            foreach (var folder in folders)
            {
                var selector = GetSelector(folder.ID);
                var folderDao = selector.GetFolderDao(folder.ID);
                if (folderDao == null) continue;

                var parentFolders = folderDao.GetParentFolders(selector.ConvertId(folder.ID));
                if (parentFolders == null || parentFolders.Count > 0) continue;

                parentFolders.Reverse();
                var pureShareRecords = GetPureShareRecords(parentFolders);
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

        public void RemoveSubject(Guid subject)
        {
            SecurityDao.RemoveSubject(subject);
        }

        public IEnumerable<FileShareRecord> GetShares(IEnumerable<Guid> subjects)
        {
            return SecurityDao.GetShares(subjects);
        }

        public IEnumerable<FileShareRecord> GetPureShareRecords(IEnumerable<FileEntry<string>> entries)
        {
            return SecurityDao.GetPureShareRecords(entries);
        }

        public IEnumerable<FileShareRecord> GetPureShareRecords(FileEntry<string> entry)
        {
            return SecurityDao.GetPureShareRecords(entry);
        }

        public void DeleteShareRecords(IEnumerable<FileShareRecord> records)
        {
            SecurityDao.DeleteShareRecords(records);
        }

        public bool IsShared(object entryId, FileEntryType type)
        {
            return SecurityDao.IsShared(entryId, type);
        }
    }
}