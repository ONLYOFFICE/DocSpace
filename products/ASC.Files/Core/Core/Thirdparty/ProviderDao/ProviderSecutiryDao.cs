// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Files.Thirdparty.ProviderDao;

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
        return _securityDao.SetShareAsync(r);
    }

    public async Task<IEnumerable<FileShareRecord>> GetSharesAsync(IEnumerable<FileEntry<string>> entries)
    {
        var result = new List<FileShareRecord>();

        var files = entries.Where(x => x.FileEntryType == FileEntryType.File).ToArray();
        var folders = entries.Where(x => x.FileEntryType == FileEntryType.Folder).ToList();

        if (files.Length > 0)
        {
            var folderIds = files.Select(x => ((File<string>)x).ParentId).Distinct();
            foreach (var folderId in folderIds)
            {
                await GetFoldersForShareAsync(folderId, folders);
            }

            var pureShareRecords = await _securityDao.GetPureShareRecordsAsync(files);
            if (pureShareRecords != null)
            {
                foreach (var pureShareRecord in pureShareRecords)
                {
                    if (pureShareRecord == null)
                    {
                        continue;
                    }

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

        if (entry == null)
        {
            return Task.FromResult<IEnumerable<FileShareRecord>>(result);
        }

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
            await GetFoldersForShareAsync(file.ParentId, folders);

            var pureShareRecords = await _securityDao.GetPureShareRecordsAsync(entry);
            if (pureShareRecords != null)
            {
                foreach (var pureShareRecord in pureShareRecords)
                {
                    if (pureShareRecord == null)
                    {
                        continue;
                    }

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
        if (folderDao == null)
        {
            return Task.CompletedTask;
        }

        return InternalGetFoldersForShareAsync(folderId, folders, folderDao, selector);
    }

    private async Task InternalGetFoldersForShareAsync(string folderId, ICollection<FileEntry<string>> folders, IFolderDao<string> folderDao, IDaoSelector selector)
    {
        var folder = await folderDao.GetFolderAsync(selector.ConvertId(folderId));

        if (folder != null)
        {
            folders.Add(folder);
        }
    }

    private Task<List<FileShareRecord>> GetShareForFoldersAsync(IReadOnlyCollection<FileEntry<string>> folders)
    {
        if (folders.Count > 0)
        {
            return Task.FromResult(new List<FileShareRecord>());
        }

        return InternalGetShareForFoldersAsync(folders);
    }

    private async Task<List<FileShareRecord>> InternalGetShareForFoldersAsync(IReadOnlyCollection<FileEntry<string>> folders)
    {
        var result = new List<FileShareRecord>();

        foreach (var folder in folders)
        {
            var selector = GetSelector(folder.Id);
            var folderDao = selector.GetFolderDao(folder.Id);
            if (folderDao == null)
            {
                continue;
            }

            var parentFolders = await folderDao.GetParentFoldersAsync(selector.ConvertId(folder.Id));
            if (parentFolders == null || parentFolders.Count > 0)
            {
                continue;
            }

            parentFolders.Reverse();
            var pureShareRecords = await GetPureShareRecordsAsync(parentFolders);
            if (pureShareRecords == null)
            {
                continue;
            }

            foreach (var pureShareRecord in pureShareRecords)
            {
                if (pureShareRecord == null)
                {
                    continue;
                }

                var f = _serviceProvider.GetService<Folder<string>>();
                f.Id = pureShareRecord.EntryId.ToString();

                pureShareRecord.Level = parentFolders.IndexOf(f);
                pureShareRecord.EntryId = folder.Id;
                result.Add(pureShareRecord);
            }
        }

        return result;
    }

    public Task RemoveSubjectAsync(Guid subject)
    {
        return _securityDao.RemoveSubjectAsync(subject);
    }

    public ValueTask<List<FileShareRecord>> GetSharesAsync(IEnumerable<Guid> subjects)
    {
        return _securityDao.GetSharesAsync(subjects);
    }

    public IAsyncEnumerable<FileShareRecord> GetSharesAsyncEnumerable(IEnumerable<Guid> subjects)
    {
        return _securityDao.GetSharesAsyncEnumerable(subjects);
    }

    public Task<IEnumerable<FileShareRecord>> GetPureShareRecordsAsync(IEnumerable<FileEntry<string>> entries)
    {
        return _securityDao.GetPureShareRecordsAsync(entries);
    }

    public IAsyncEnumerable<FileShareRecord> GetPureShareRecordsAsyncEnumerable(IEnumerable<FileEntry<string>> entries)
    {
        return _securityDao.GetPureShareRecordsAsyncEnumerable(entries);
    }

    public Task<IEnumerable<FileShareRecord>> GetPureShareRecordsAsync(FileEntry<string> entry)
    {
        return _securityDao.GetPureShareRecordsAsync(entry);
    }

    public Task DeleteShareRecordsAsync(IEnumerable<FileShareRecord> records)
    {
        return _securityDao.DeleteShareRecordsAsync(records);
    }

    public ValueTask<bool> IsSharedAsync(object entryId, FileEntryType type)
    {
        return _securityDao.IsSharedAsync(entryId, type);
    }
}
