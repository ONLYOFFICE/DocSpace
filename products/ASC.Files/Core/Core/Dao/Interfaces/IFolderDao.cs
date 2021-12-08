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
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Files.Core.Security;

namespace ASC.Files.Core
{
    [Scope]
    public interface IFolderDao<T>
    {
        /// <summary>
        ///     Get folder by id.
        /// </summary>
        /// <param name="folderId">folder id</param>
        /// <returns>folder</returns>
        Folder<T> GetFolder(T folderId);
        Task<Folder<T>> GetFolderAsync(T folderId);

        /// <summary>
        ///     Returns the folder with the given name and id of the root
        /// </summary>
        /// <param name="title"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        Folder<T> GetFolder(string title, T parentId);
        Task<Folder<T>> GetFolderAsync(string title, T parentId);
        /// <summary>
        ///    Gets the root folder
        /// </summary>
        /// <param name="folderId">folder id</param>
        /// <returns>root folder</returns>
        Folder<T> GetRootFolder(T folderId);
        Task<Folder<T>> GetRootFolderAsync(T folderId);

        /// <summary>
        ///    Gets the root folder
        /// </summary>
        /// <param name="fileId">file id</param>
        /// <returns>root folder</returns>
        Folder<T> GetRootFolderByFile(T fileId);
        Task<Folder<T>> GetRootFolderByFileAsync(T fileId);

        /// <summary>
        ///     Get a list of folders in current folder.
        /// </summary>
        /// <param name="parentId"></param>
        List<Folder<T>> GetFolders(T parentId);
        Task<List<Folder<T>>> GetFoldersAsync(T parentId);
        /// <summary>
        /// Get a list of folders.
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="orderBy"></param>
        /// <param name="filterType"></param>
        /// <param name="subjectGroup"></param>
        /// <param name="subjectID"></param>
        /// <param name="searchText"></param>
        /// <param name="withSubfolders"></param>
        /// <returns></returns>
        List<Folder<T>> GetFolders(T parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool withSubfolders = false);
        Task<List<Folder<T>>> GetFoldersAsync(T parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool withSubfolders = false);

        /// <summary>
        /// Gets the folder (s) by ID (s)
        /// </summary>
        /// <param name="folderIds"></param>
        /// <param name="filterType"></param>
        /// <param name="subjectGroup"></param>
        /// <param name="subjectID"></param>
        /// <param name="searchText"></param>
        /// <param name="searchSubfolders"></param>
        /// <param name="checkShare"></param>
        /// <returns></returns>
        List<Folder<T>> GetFolders(IEnumerable<T> folderIds, FilterType filterType = FilterType.None, bool subjectGroup = false, Guid? subjectID = null, string searchText = "", bool searchSubfolders = false, bool checkShare = true);
        IAsyncEnumerable<Folder<T>> GetFoldersAsync(IEnumerable<T> folderIds, FilterType filterType = FilterType.None, bool subjectGroup = false, Guid? subjectID = null, string searchText = "", bool searchSubfolders = false, bool checkShare = true);

        /// <summary>
        ///     Get folder, contains folder with id
        /// </summary>
        /// <param name="folderId">folder id</param>
        /// <returns></returns>
        List<Folder<T>> GetParentFolders(T folderId);
        Task<List<Folder<T>>> GetParentFoldersAsync(T folderId);
        /// <summary>
        ///     save or update folder
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        T SaveFolder(Folder<T> folder);
        Task<T> SaveFolderAsync(Folder<T> folder);
        /// <summary>
        ///     delete folder
        /// </summary>
        /// <param name="folderId">folder id</param>
        void DeleteFolder(T folderId);
        Task DeleteFolderAsync(T folderId);
        /// <summary>
        ///  move folder
        /// </summary>
        /// <param name="folderId">folder id</param>
        /// <param name="toFolderId">destination folder id</param>
        /// <param name="cancellationToken"></param>
        T MoveFolder(T folderId, T toFolderId, CancellationToken? cancellationToken);
        Task<T> MoveFolderAsync(T folderId, T toFolderId, CancellationToken? cancellationToken);
        TTo MoveFolder<TTo>(T folderId, TTo toFolderId, CancellationToken? cancellationToken);
        Task<TTo> MoveFolderAsync<TTo>(T folderId, TTo toFolderId, CancellationToken? cancellationToken);
        string MoveFolder(T folderId, string toFolderId, CancellationToken? cancellationToken);
        Task<string> MoveFolderAsync(T folderId, string toFolderId, CancellationToken? cancellationToken);
        int MoveFolder(T folderId, int toFolderId, CancellationToken? cancellationToken);
        Task<int> MoveFolderAsync(T folderId, int toFolderId, CancellationToken? cancellationToken);

        /// <summary>
        ///     copy folder
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="toFolderId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns> 
        /// </returns>
        Folder<T> CopyFolder(T folderId, T toFolderId, CancellationToken? cancellationToken);
        Task<Folder<T>> CopyFolderAsync(T folderId, T toFolderId, CancellationToken? cancellationToken);
        Folder<TTo> CopyFolder<TTo>(T folderId, TTo toFolderId, CancellationToken? cancellationToken);
        Task<Folder<TTo>> CopyFolderAsync<TTo>(T folderId, TTo toFolderId, CancellationToken? cancellationToken);
        Folder<string> CopyFolder(T folderId, string toFolderId, CancellationToken? cancellationToken);
        Task<Folder<string>> CopyFolderAsync(T folderId, string toFolderId, CancellationToken? cancellationToken);
        Folder<int> CopyFolder(T folderId, int toFolderId, CancellationToken? cancellationToken);
        Task<Folder<int>> CopyFolderAsync(T folderId, int toFolderId, CancellationToken? cancellationToken);

        /// <summary>
        /// Validate the transfer operation directory to another directory.
        /// </summary>
        /// <param name="folderIds"></param>
        /// <param name="to"></param>
        /// <returns>
        /// Returns pair of file ID, file name, in which the same name.
        /// </returns>
        IDictionary<T, string> CanMoveOrCopy(T[] folderIds, T to);
        Task<IDictionary<T, string>> CanMoveOrCopyAsync(T[] folderIds, T to);
        IDictionary<T, string> CanMoveOrCopy<TTo>(T[] folderIds, TTo to);
        Task<IDictionary<T, string>> CanMoveOrCopyAsync<TTo>(T[] folderIds, TTo to);
        IDictionary<T, string> CanMoveOrCopy(T[] folderIds, string to);
        Task<IDictionary<T, string>> CanMoveOrCopyAsync(T[] folderIds, string to);
        IDictionary<T, string> CanMoveOrCopy(T[] folderIds, int to);
        Task<IDictionary<T, string>> CanMoveOrCopyAsync(T[] folderIds, int to);

        /// <summary>
        ///     Rename folder
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="newTitle">new name</param>
        T RenameFolder(Folder<T> folder, string newTitle);
        Task<T> RenameFolderAsync(Folder<T> folder, string newTitle);

        /// <summary>
        ///    Gets the number of files and folders to the container in your
        /// </summary>
        /// <param name="folderId">folder id</param>
        /// <returns></returns>
        int GetItemsCount(T folderId);
        Task<int> GetItemsCountAsync(T folderId);

        /// <summary>
        ///    Check folder on emptiness
        /// </summary>
        /// <param name="folderId">folder id</param>
        /// <returns></returns>
        bool IsEmpty(T folderId);
        Task<bool> IsEmptyAsync(T folderId);
        /// <summary>
        /// Check the need to use the trash before removing
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        bool UseTrashForRemove(Folder<T> folder);

        /// <summary>
        /// Check the need to use recursion for operations
        /// </summary>
        /// <param name="folderId"> </param>
        /// <param name="toRootFolderId"> </param>
        /// <returns></returns>
        bool UseRecursiveOperation(T folderId, T toRootFolderId);
        bool UseRecursiveOperation<TTo>(T folderId, TTo toRootFolderId);
        bool UseRecursiveOperation(T folderId, string toRootFolderId);
        bool UseRecursiveOperation(T folderId, int toRootFolderId);

        /// <summary>
        /// Check the possibility to calculate the number of subitems
        /// </summary>
        /// <param name="entryId"> </param>
        /// <returns></returns>
        bool CanCalculateSubitems(T entryId);

        /// <summary>
        /// Returns maximum size of file which can be uploaded to specific folder
        /// </summary>
        /// <param name="folderId">Id of the folder</param>
        /// <param name="chunkedUpload">Determines whenever supposed upload will be chunked (true) or not (false)</param>
        /// <returns>Maximum size of file which can be uploaded to folder</returns>
        long GetMaxUploadSize(T folderId, bool chunkedUpload = false);
        Task<long> GetMaxUploadSizeAsync(T folderId, bool chunkedUpload = false);

        #region Only for TMFolderDao

        /// <summary>
        /// Set created by
        /// </summary>
        /// <param name="folderIds"></param>
        /// <param name="newOwnerId"></param>
        void ReassignFolders(T[] folderIds, Guid newOwnerId);

        /// <summary>
        /// Search the list of folders containing text in title
        /// Only in TMFolderDao
        /// </summary>
        /// <param name="text"></param>
        /// <param name="bunch"></param>
        /// <returns></returns>
        IEnumerable<Folder<T>> SearchFolders(string text, bool bunch = false);

        /// <summary>
        /// Only in TMFolderDao
        /// </summary>
        /// <param name="module"></param>
        /// <param name="bunch"></param>
        /// <param name="data"></param>
        /// <param name="createIfNotExists"></param>
        /// <returns></returns>
        T GetFolderID(string module, string bunch, string data, bool createIfNotExists);
        Task<T> GetFolderIDAsync(string module, string bunch, string data, bool createIfNotExists);

        IEnumerable<T> GetFolderIDs(string module, string bunch, IEnumerable<string> data, bool createIfNotExists);
        Task<IEnumerable<T>> GetFolderIDsAsync(string module, string bunch, IEnumerable<string> data, bool createIfNotExists);

        /// <summary>
        ///  Returns id folder "Shared Documents"
        /// Only in TMFolderDao
        /// </summary>
        /// <returns></returns>
        T GetFolderIDCommon(bool createIfNotExists);
        Task<T> GetFolderIDCommonAsync(bool createIfNotExists);

        /// <summary>
        ///  Returns id folder "My Documents"
        /// Only in TMFolderDao
        /// </summary>
        /// <param name="createIfNotExists"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        T GetFolderIDUser(bool createIfNotExists, Guid? userId = null);
        Task<T> GetFolderIDUserAsync(bool createIfNotExists, Guid? userId = null);

        /// <summary>
        /// Returns id folder "Shared with me"
        /// Only in TMFolderDao
        /// </summary>
        /// <param name="createIfNotExists"></param>
        /// <returns></returns>
        T GetFolderIDShare(bool createIfNotExists);
        Task<T> GetFolderIDShareAsync(bool createIfNotExists);

        /// <summary>
        /// Returns id folder "Recent"
        /// Only in TMFolderDao
        /// </summary>
        /// <param name="createIfNotExists"></param>
        /// <returns></returns>
        T GetFolderIDRecent(bool createIfNotExists);
        Task<T> GetFolderIDRecentAsync(bool createIfNotExists);

        /// <summary>

        /// <summary>
        /// Returns id folder "Favorites"
        /// Only in TMFolderDao
        /// </summary>
        /// <param name="createIfNotExists"></param>
        /// <returns></returns>
        T GetFolderIDFavorites(bool createIfNotExists);
        Task<T> GetFolderIDFavoritesAsync(bool createIfNotExists);

        /// <summary>
        /// Returns id folder "Templates"
        /// Only in TMFolderDao
        /// </summary>
        /// <param name="createIfNotExists"></param>
        /// <returns></returns>
        T GetFolderIDTemplates(bool createIfNotExists);
        Task<T> GetFolderIDTemplatesAsync(bool createIfNotExists);

        /// <summary>
        /// Returns id folder "Privacy"
        /// Only in TMFolderDao
        /// </summary>
        /// <param name="createIfNotExists"></param>
        /// <returns></returns>
        T GetFolderIDPrivacy(bool createIfNotExists, Guid? userId = null);
        Task<T> GetFolderIDPrivacyAsync(bool createIfNotExists, Guid? userId = null);

        /// <summary>
        /// Returns id folder "Trash"
        /// Only in TMFolderDao
        /// </summary>
        /// <param name="createIfNotExists"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        T GetFolderIDTrash(bool createIfNotExists, Guid? userId = null);
        Task<T> GetFolderIDTrashAsync(bool createIfNotExists, Guid? userId = null);

        /// <summary>
        /// Returns id folder "Projects"
        /// Only in TMFolderDao
        /// </summary>
        /// <param name="createIfNotExists"></param>
        /// <returns></returns>
        T GetFolderIDProjects(bool createIfNotExists);
        Task<T> GetFolderIDProjectsAsync(bool createIfNotExists);


        /// <summary>
        /// Return id of related object
        /// Only in TMFolderDao
        /// </summary>
        /// <param name="folderID"></param>
        /// <returns></returns>
        string GetBunchObjectID(T folderID);
        Task<string> GetBunchObjectIDAsync(T folderID);

        /// <summary>
        /// Return ids of related objects
        /// Only in TMFolderDao
        /// </summary>
        /// <param name="folderIDs"></param>
        /// <returns></returns>
        Dictionary<string, string> GetBunchObjectIDs(List<T> folderIDs);
        Task<Dictionary<string, string>> GetBunchObjectIDsAsync(List<T> folderIDs);

        IEnumerable<(Folder<T>, SmallShareRecord)> GetFeedsForFolders(int tenant, DateTime from, DateTime to);

        IEnumerable<T> GetTenantsWithFeedsForFolders(DateTime fromTime);
        Task<IEnumerable<T>> GetTenantsWithFeedsForFoldersAsync(DateTime fromTime);

        #endregion
    }
}