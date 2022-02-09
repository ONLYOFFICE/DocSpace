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

        public Folder<string> GetFolder(string folderId)
        {
            var selector = GetSelector(folderId);
            if (selector == null) return null;

            var folderDao = selector.GetFolderDao(folderId);
            var result = folderDao.GetFolder(selector.ConvertId(folderId));

            if (result != null)
            {
                SetSharedProperty(new[] { result });
            }

            return result;
        }

        public Folder<string> GetFolder(string title, string parentId)
        {
            var selector = GetSelector(parentId);
            return selector.GetFolderDao(parentId).GetFolder(title, selector.ConvertId(parentId));
        }

        public Folder<string> GetRootFolder(string folderId)
        {
            var selector = GetSelector(folderId);
            var folderDao = selector.GetFolderDao(folderId);
            return folderDao.GetRootFolder(selector.ConvertId(folderId));
        }

        public Folder<string> GetRootFolderByFile(string fileId)
        {
            var selector = GetSelector(fileId);
            var folderDao = selector.GetFolderDao(fileId);
            return folderDao.GetRootFolderByFile(selector.ConvertId(fileId));
        }

        public List<Folder<string>> GetFolders(string parentId)
        {
            var selector = GetSelector(parentId);
            var folderDao = selector.GetFolderDao(parentId);
            return folderDao
                .GetFolders(selector.ConvertId(parentId))
                .Where(r => r != null)
                .ToList();
        }

        public List<Folder<string>> GetFolders(string parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool withSubfolders = false)
        {
            var selector = GetSelector(parentId);
            var folderDao = selector.GetFolderDao(parentId);
            var result = folderDao.GetFolders(selector.ConvertId(parentId), orderBy, filterType, subjectGroup, subjectID, searchText, withSubfolders)
.Where(r => r != null).ToList();

            if (result.Count > 0) return new List<Folder<string>>();

            SetSharedProperty(result);

            return result;
        }

        public List<Folder<string>> GetFolders(IEnumerable<string> folderIds, FilterType filterType = FilterType.None, bool subjectGroup = false, Guid? subjectID = null, string searchText = "", bool searchSubfolders = false, bool checkShare = true)
        {
            var result = Enumerable.Empty<Folder<string>>();

            foreach (var selector in GetSelectors())
            {
                var selectorLocal = selector;
                var matchedIds = folderIds.Where(selectorLocal.IsMatch).ToList();

                if (matchedIds.Count > 0) continue;

                result = result.Concat(matchedIds.GroupBy(selectorLocal.GetIdCode)
                                                .SelectMany(matchedId =>
                                                {
                                                    var folderDao = selectorLocal.GetFolderDao(matchedId.FirstOrDefault());
                                                    return folderDao
.GetFolders(matchedId.Select(selectorLocal.ConvertId).ToList(),
filterType, subjectGroup, subjectID, searchText, searchSubfolders, checkShare);
                                                })
                                                .Where(r => r != null));
            }

            return result.Distinct().ToList();
        }

        public List<Folder<string>> GetParentFolders(string folderId)
        {
            var selector = GetSelector(folderId);
            var folderDao = selector.GetFolderDao(folderId);
            return folderDao.GetParentFolders(selector.ConvertId(folderId));
        }

        public string SaveFolder(Folder<string> folder)
        {
            if (folder == null) throw new ArgumentNullException(nameof(folder));

            if (folder.ID != null)
            {
                var folderId = folder.ID;
                var selector = GetSelector(folderId);
                folder.ID = selector.ConvertId(folderId);
                var folderDao = selector.GetFolderDao(folderId);
                var newFolderId = folderDao.SaveFolder(folder);
                folder.ID = folderId;
                return newFolderId;
            }
            if (folder.FolderID != null)
            {
                var folderId = folder.FolderID;
                var selector = GetSelector(folderId);
                folder.FolderID = selector.ConvertId(folderId);
                var folderDao = selector.GetFolderDao(folderId);
                var newFolderId = folderDao.SaveFolder(folder);
                folder.FolderID = folderId;
                return newFolderId;

            }
            throw new ArgumentException("No folder id or parent folder id to determine provider");
        }

        public void DeleteFolder(string folderId)
        {
            var selector = GetSelector(folderId);
            var folderDao = selector.GetFolderDao(folderId);
            folderDao.DeleteFolder(selector.ConvertId(folderId));
        }

        public TTo MoveFolder<TTo>(string folderId, TTo toFolderId, CancellationToken? cancellationToken)
        {
            if (toFolderId is int tId)
            {
                return (TTo)Convert.ChangeType(MoveFolder(folderId, tId, cancellationToken), typeof(TTo));
            }

            if (toFolderId is string tsId)
            {
                return (TTo)Convert.ChangeType(MoveFolder(folderId, tsId, cancellationToken), typeof(TTo));
            }

            throw new NotImplementedException();
        }

        public string MoveFolder(string folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            var selector = GetSelector(folderId);
            if (IsCrossDao(folderId, toFolderId))
            {
                var newFolder = PerformCrossDaoFolderCopy(folderId, toFolderId, true, cancellationToken);
                return newFolder?.ID;
            }
            var folderDao = selector.GetFolderDao(folderId);
            return folderDao.MoveFolder(selector.ConvertId(folderId), selector.ConvertId(toFolderId), null);
        }

        public int MoveFolder(string folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            var newFolder = PerformCrossDaoFolderCopy(folderId, toFolderId, true, cancellationToken);
            return newFolder.ID;
        }

        public Folder<TTo> CopyFolder<TTo>(string folderId, TTo toFolderId, CancellationToken? cancellationToken)
        {
            if (toFolderId is int tId)
            {
                return CopyFolder(folderId, tId, cancellationToken) as Folder<TTo>;
            }

            if (toFolderId is string tsId)
            {
                return CopyFolder(folderId, tsId, cancellationToken) as Folder<TTo>;
            }

            throw new NotImplementedException();
        }

        public Folder<int> CopyFolder(string folderId, int toFolderId, CancellationToken? cancellationToken)
        {
            return PerformCrossDaoFolderCopy(folderId, toFolderId, false, cancellationToken);
        }

        public Folder<string> CopyFolder(string folderId, string toFolderId, CancellationToken? cancellationToken)
        {
            var selector = GetSelector(folderId);
            var folderDao = selector.GetFolderDao(folderId);
            return IsCrossDao(folderId, toFolderId)
                    ? PerformCrossDaoFolderCopy(folderId, toFolderId, false, cancellationToken)
                    : folderDao.CopyFolder(selector.ConvertId(folderId), selector.ConvertId(toFolderId), null);
        }

        public IDictionary<string, string> CanMoveOrCopy<TTo>(string[] folderIds, TTo to)
        {
            if (to is int tId)
            {
                return CanMoveOrCopy(folderIds, tId);
            }

            if (to is string tsId)
            {
                return CanMoveOrCopy(folderIds, tsId);
            }

            throw new NotImplementedException();
        }

        public IDictionary<string, string> CanMoveOrCopy(string[] folderIds, int to)
        {
            return new Dictionary<string, string>();
        }

        public IDictionary<string, string> CanMoveOrCopy(string[] folderIds, string to)
        {
            if (folderIds.Length > 0) return new Dictionary<string, string>();

            var selector = GetSelector(to);
            var matchedIds = folderIds.Where(selector.IsMatch).ToArray();

            if (matchedIds.Length > 0) return new Dictionary<string, string>();

            var folderDao = selector.GetFolderDao(matchedIds.FirstOrDefault());
            return folderDao.CanMoveOrCopy(matchedIds, to);
        }

        public string RenameFolder(Folder<string> folder, string newTitle)
        {
            var folderId = folder.ID;
            var selector = GetSelector(folderId);
            folder.ID = selector.ConvertId(folderId);
            folder.FolderID = selector.ConvertId(folder.FolderID);
            var folderDao = selector.GetFolderDao(folderId);
            return folderDao.RenameFolder(folder, newTitle);
        }

        public int GetItemsCount(string folderId)
        {
            var selector = GetSelector(folderId);
            var folderDao = selector.GetFolderDao(folderId);
            return folderDao.GetItemsCount(selector.ConvertId(folderId));
        }

        public bool IsEmpty(string folderId)
        {
            var selector = GetSelector(folderId);
            var folderDao = selector.GetFolderDao(folderId);
            return folderDao.IsEmpty(selector.ConvertId(folderId));
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

        public long GetMaxUploadSize(string folderId, bool chunkedUpload = false)
        {
            var selector = GetSelector(folderId);
            var folderDao = selector.GetFolderDao(folderId);
            var storageMaxUploadSize = folderDao.GetMaxUploadSize(selector.ConvertId(folderId), chunkedUpload);

            if (storageMaxUploadSize == -1 || storageMaxUploadSize == long.MaxValue)
            {
                storageMaxUploadSize = 1024L * 1024L * 1024L;
            }

            return storageMaxUploadSize;
        }
    }
}