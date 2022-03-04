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
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Core;
using ASC.Files.Core;
using ASC.Files.Core.Data;
using ASC.Files.Core.Thirdparty;

namespace ASC.Files.Thirdparty.ProviderDao
{
    [Scope]
    internal class ProviderFolderDao : ProviderDaoBase, IFolderDao<string>
    {
        public ProviderFolderDao(
            IServiceProvider serviceProvider,
            TenantManager tenantManager,
            SecurityDao<string> securityDao,
            TagDao<string> tagDao,
            CrossDao crossDao)
            : base(serviceProvider, tenantManager, securityDao, tagDao, crossDao)
        {
        }

        public Task<Folder<string>> GetFolderAsync(string folderId)
        {
            var selector = GetSelector(folderId);
            if (selector == null) return null;

            return InternalGetFolderAsync(folderId, selector);
        }

        private async Task<Folder<string>> InternalGetFolderAsync(string folderId, IDaoSelector selector)
        {
            var folderDao = selector.GetFolderDao(folderId);
            var result = await folderDao.GetFolderAsync(selector.ConvertId(folderId)).ConfigureAwait(false);

            if (result != null)
            {
                await SetSharedPropertyAsync(new[] { result }.ToAsyncEnumerable()).ConfigureAwait(false);
            }

            return result;
        }

        public Task<Folder<string>> GetFolderAsync(string title, string parentId)
        {
            var selector = GetSelector(parentId);
            return selector.GetFolderDao(parentId).GetFolderAsync(title, selector.ConvertId(parentId));
        }

        public Task<Folder<string>> GetRootFolderAsync(string folderId)
        {
            var selector = GetSelector(folderId);
            var folderDao = selector.GetFolderDao(folderId);
            return folderDao.GetRootFolderAsync(selector.ConvertId(folderId));
        }

        public Task<Folder<string>> GetRootFolderByFileAsync(string fileId)
        {
            var selector = GetSelector(fileId);
            var folderDao = selector.GetFolderDao(fileId);
            return folderDao.GetRootFolderByFileAsync(selector.ConvertId(fileId));
        }

        public IAsyncEnumerable<Folder<string>> GetFoldersAsync(string parentId)
        {
            var selector = GetSelector(parentId);
            var folderDao = selector.GetFolderDao(parentId);
            var folders = folderDao.GetFoldersAsync(selector.ConvertId(parentId));
            return folders.Where(r => r != null);
        }

        public async IAsyncEnumerable<Folder<string>> GetFoldersAsync(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool withSubfolders = false)
        {
            var selector = GetSelector(parentId);
            var folderDao = selector.GetFolderDao(parentId);
            var folders = folderDao.GetFoldersAsync(selector.ConvertId(parentId), orderBy, filterType, subjectGroup, subjectID, searchText, withSubfolders);
            var result = folders.Where(r => r != null);

            await SetSharedPropertyAsync(result).ConfigureAwait(false);

            await foreach (var r in result.ConfigureAwait(false))
            {
                yield return r;
            }
        }

        public IAsyncEnumerable<Folder<string>> GetFoldersAsync(IEnumerable<string> folderIds, FilterType filterType = FilterType.None, bool subjectGroup = false, Guid? subjectID = null, string searchText = "", bool searchSubfolders = false, bool checkShare = true)
        {
            var result = AsyncEnumerable.Empty<Folder<string>>();

            foreach (var selector in GetSelectors())
            {
                var selectorLocal = selector;
                var matchedIds = folderIds.Where(selectorLocal.IsMatch).ToList();

                if (matchedIds.Count > 0) continue;

                result = result.Concat(matchedIds.GroupBy(selectorLocal.GetIdCode)
                                                .ToAsyncEnumerable()
                                                .SelectMany(matchedId =>
                                                {
                                                    var folderDao = selectorLocal.GetFolderDao(matchedId.FirstOrDefault());
                                                    return folderDao
                                                        .GetFoldersAsync(matchedId.Select(selectorLocal.ConvertId).ToList(),
                                                        filterType, subjectGroup, subjectID, searchText, searchSubfolders, checkShare);
                                                })
                                                .Where(r => r != null));
            }

            return result.Distinct();
        }

        public Task<List<Folder<string>>> GetParentFoldersAsync(string folderId)
        {
            var selector = GetSelector(folderId);
            var folderDao = selector.GetFolderDao(folderId);
            return folderDao.GetParentFoldersAsync(selector.ConvertId(folderId));
        }


        public Task<string> SaveFolderAsync(Folder<string> folder)
        {
            if (folder == null) throw new ArgumentNullException(nameof(folder));

            return InternalSaveFolderAsync(folder);
        }

        private async Task<string> InternalSaveFolderAsync(Folder<string> folder)
        {
            if (folder.ID != null)
            {
                var folderId = folder.ID;
                var selector = GetSelector(folderId);
                folder.ID = selector.ConvertId(folderId);
                var folderDao = selector.GetFolderDao(folderId);
                var newFolderId = await folderDao.SaveFolderAsync(folder).ConfigureAwait(false);
                folder.ID = folderId;
                return newFolderId;
            }
            if (folder.FolderID != null)
            {
                var folderId = folder.FolderID;
                var selector = GetSelector(folderId);
                folder.FolderID = selector.ConvertId(folderId);
                var folderDao = selector.GetFolderDao(folderId);
                var newFolderId = await folderDao.SaveFolderAsync(folder).ConfigureAwait(false);
                folder.FolderID = folderId;
                return newFolderId;

            }
            throw new ArgumentException("No folder id or parent folder id to determine provider");
        }

        public Task DeleteFolderAsync(string folderId)
        {
            var selector = GetSelector(folderId);
            var folderDao = selector.GetFolderDao(folderId);
            return folderDao.DeleteFolderAsync(selector.ConvertId(folderId));
        }

        public async Task<TTo> MoveFolderAsync<TTo>(string folderId, TTo toFolderId, CancellationToken? cancellationToken)
        {
            if (toFolderId is int tId)
            {
                return (TTo)Convert.ChangeType(await MoveFolderAsync(folderId, tId, cancellationToken).ConfigureAwait(false), typeof(TTo));
            }

            if (toFolderId is string tsId)
            {
                return (TTo)Convert.ChangeType(await MoveFolderAsync(folderId, tsId, cancellationToken).ConfigureAwait(false), typeof(TTo));
            }

            throw new NotImplementedException();
        }

        public async Task<string> MoveFolderAsync(string folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            var selector = GetSelector(folderId);
            if (IsCrossDao(folderId, toFolderId))
            {
                var newFolder = await PerformCrossDaoFolderCopyAsync(folderId, toFolderId, true, cancellationToken).ConfigureAwait(false);
                return newFolder?.ID;
            }
            var folderDao = selector.GetFolderDao(folderId);
            return await folderDao.MoveFolderAsync(selector.ConvertId(folderId), selector.ConvertId(toFolderId), null).ConfigureAwait(false);
        }

        public async Task<int> MoveFolderAsync(string folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            var newFolder = await PerformCrossDaoFolderCopyAsync(folderId, toFolderId, true, cancellationToken).ConfigureAwait(false);
            return newFolder.ID;
        }

        public async Task<Folder<TTo>> CopyFolderAsync<TTo>(string folderId, TTo toFolderId, CancellationToken? cancellationToken)
        {
            if (toFolderId is int tId)
            {
                return await CopyFolderAsync(folderId, tId, cancellationToken).ConfigureAwait(false) as Folder<TTo>;
            }

            if (toFolderId is string tsId)
            {
                return await CopyFolderAsync(folderId, tsId, cancellationToken).ConfigureAwait(false) as Folder<TTo>;
            }

            throw new NotImplementedException();
        }

        public Task<Folder<int>> CopyFolderAsync(string folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            return PerformCrossDaoFolderCopyAsync(folderId, toFolderId, false, cancellationToken);
        }

        public async Task<Folder<string>> CopyFolderAsync(string folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            var selector = GetSelector(folderId);
            var folderDao = selector.GetFolderDao(folderId);
            return IsCrossDao(folderId, toFolderId)
                    ? await PerformCrossDaoFolderCopyAsync(folderId, toFolderId, false, cancellationToken).ConfigureAwait(false)
                    : await folderDao.CopyFolderAsync(selector.ConvertId(folderId), selector.ConvertId(toFolderId), null).ConfigureAwait(false);
        }

        public Task<IDictionary<string, string>> CanMoveOrCopyAsync<TTo>(string[] folderIds, TTo to)
        {
            if (to is int tId)
            {
                return CanMoveOrCopyAsync(folderIds, tId);
            }

            if (to is string tsId)
            {
                return CanMoveOrCopyAsync(folderIds, tsId);
            }

            throw new NotImplementedException();
        }

        public Task<IDictionary<string, string>> CanMoveOrCopyAsync(string[] folderIds, int to)
        {
            return Task.FromResult((IDictionary<string, string>)new Dictionary<string, string>());
        }

        public Task<IDictionary<string, string>> CanMoveOrCopyAsync(string[] folderIds, string to)
        {
            if (folderIds.Length > 0) return Task.FromResult<IDictionary<string, string>>(new Dictionary<string, string>());

            var selector = GetSelector(to);
            var matchedIds = folderIds.Where(selector.IsMatch).ToArray();

            if (matchedIds.Length > 0) return Task.FromResult<IDictionary<string, string>>(new Dictionary<string, string>());

            return InternalCanMoveOrCopyAsync(to, matchedIds, selector);
        }

        private Task<IDictionary<string, string>> InternalCanMoveOrCopyAsync(string to, string[] matchedIds, IDaoSelector selector)
        {
            var folderDao = selector.GetFolderDao(matchedIds.FirstOrDefault());
            return folderDao.CanMoveOrCopyAsync(matchedIds, to);
        }

        public Task<string> RenameFolderAsync(Folder<string> folder, string newTitle)
        {
            var folderId = folder.ID;
            var selector = GetSelector(folderId);
            folder.ID = selector.ConvertId(folderId);
            folder.FolderID = selector.ConvertId(folder.FolderID);
            var folderDao = selector.GetFolderDao(folderId);
            return folderDao.RenameFolderAsync(folder, newTitle);
        }

        public Task<int> GetItemsCountAsync(string folderId)
        {
            var selector = GetSelector(folderId);
            var folderDao = selector.GetFolderDao(folderId);
            return folderDao.GetItemsCountAsync(selector.ConvertId(folderId));
        }

        public Task<bool> IsEmptyAsync(string folderId)
        {
            var selector = GetSelector(folderId);
            var folderDao = selector.GetFolderDao(folderId);
            return folderDao.IsEmptyAsync(selector.ConvertId(folderId));
        }

        public bool UseTrashForRemove(Folder<string> folder)
        {
            var selector = GetSelector(folder.ID);
            var folderDao = selector.GetFolderDao(folder.ID);
            return folderDao.UseTrashForRemove(folder);
        }

        public bool UseRecursiveOperation<TTo>(string folderId, TTo toRootFolderId)
        {
            return false;
        }

        public bool UseRecursiveOperation(string folderId, int toRootFolderId)
        {
            return false;
        }

        public bool UseRecursiveOperation(string folderId, string toRootFolderId)
        {
            var selector = GetSelector(folderId);
            bool useRecursive;

            var folderDao = selector.GetFolderDao(folderId);
            useRecursive = folderDao.UseRecursiveOperation(folderId, null);

            if (toRootFolderId != null)
            {
                var toFolderSelector = GetSelector(toRootFolderId);

                var folderDao1 = toFolderSelector.GetFolderDao(toRootFolderId);
                useRecursive = useRecursive && folderDao1.UseRecursiveOperation(folderId, toFolderSelector.ConvertId(toRootFolderId));
            }
            return useRecursive;
        }

        public bool CanCalculateSubitems(string entryId)
        {
            var selector = GetSelector(entryId);
            var folderDao = selector.GetFolderDao(entryId);
            return folderDao.CanCalculateSubitems(entryId);
        }

        public async Task<long> GetMaxUploadSizeAsync(string folderId, bool chunkedUpload = false)
        {
            var selector = GetSelector(folderId);
            var folderDao = selector.GetFolderDao(folderId);
            var storageMaxUploadSize = await folderDao.GetMaxUploadSizeAsync(selector.ConvertId(folderId), chunkedUpload).ConfigureAwait(false);

            if (storageMaxUploadSize == -1 || storageMaxUploadSize == long.MaxValue)
            {
                storageMaxUploadSize = 1024L * 1024L * 1024L;
            }

            return storageMaxUploadSize;
        }
    }
}