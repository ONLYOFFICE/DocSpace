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

namespace ASC.Files.Core;

[Scope]
public interface IFolderDao<T>
{
    /// <summary>
    ///     Get folder by id.
    /// </summary>
    /// <param name="folderId">folder id</param>
    /// <returns>folder</returns>
    Task<Folder<T>> GetFolderAsync(T folderId);

    /// <summary>
    ///     Returns the folder with the given name and id of the root
    /// </summary>
    /// <param name="title"></param>
    /// <param name="parentId"></param>
    /// <returns></returns>
    Task<Folder<T>> GetFolderAsync(string title, T parentId);
    /// <summary>
    ///    Gets the root folder
    /// </summary>
    /// <param name="folderId">folder id</param>
    /// <returns>root folder</returns>
    Task<Folder<T>> GetRootFolderAsync(T folderId);

    /// <summary>
    ///    Gets the root folder
    /// </summary>
    /// <param name="fileId">file id</param>
    /// <returns>root folder</returns>
    Task<Folder<T>> GetRootFolderByFileAsync(T fileId);

    IAsyncEnumerable<Folder<T>> GetRoomsAsync(IEnumerable<T> parentsIds, FilterType filterType, IEnumerable<string> tags, Guid subjectId, string searchText, bool withSubfolders,
        bool withoutTags, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter, IEnumerable<string> subjectEntriesIds);

    IAsyncEnumerable<Folder<T>> GetRoomsAsync(IEnumerable<T> parentsIds, IEnumerable<T> roomsIds, FilterType filterType, IEnumerable<string> tags, Guid subjectId, string searchText, bool withSubfolders,
        bool withoutTags, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter, IEnumerable<string> subjectEntriesIds);

    IAsyncEnumerable<Folder<T>> GetFakeRoomsAsync(IEnumerable<T> parentsIds, FilterType filterType, IEnumerable<string> tags, Guid subjectId, string searchText,
        bool withSubfolders, bool withoutTags, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter, IEnumerable<string> subjectEntriesIds);

    IAsyncEnumerable<Folder<T>> GetFakeRoomsAsync(IEnumerable<T> parentsIds, IEnumerable<T> roomsIds, FilterType filterType, IEnumerable<string> tags,
        Guid subjectId, string searchText, bool withSubfolders, bool withoutTags, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter,
        IEnumerable<string> subjectEntriesIds);

    /// <summary>
    ///     Get a list of folders in current folder.
    /// </summary>
    /// <param name="parentId"></param>
    IAsyncEnumerable<Folder<T>> GetFoldersAsync(T parentId);

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
    /// <param name="tagIds"></param>
    /// <returns></returns>
    IAsyncEnumerable<Folder<T>> GetFoldersAsync(T parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText,
        bool withSubfolders = false, bool exludeSubject = false, int offset = 0, int count = -1);

    /// <summary>
    /// Gets the folder (s) by ID (s)
    /// </summary>
    /// <param name="folderIds"></param>
    /// <param name="filterTypes"></param>
    /// <param name="subjectGroup"></param>
    /// <param name="subjectID"></param>
    /// <param name="searchText"></param>
    /// <param name="searchSubfolders"></param>
    /// <param name="checkShare"></param>
    /// <param name="tagIds"></param>
    /// <returns></returns>
    IAsyncEnumerable<Folder<T>> GetFoldersAsync(IEnumerable<T> folderIds, FilterType filterTypes = FilterType.None, bool subjectGroup = false, Guid? subjectID = null, string searchText = "", bool searchSubfolders = false, bool checkShare = true, bool exludeSubject = false);

    /// <summary>
    ///     Get folder, contains folder with id
    /// </summary>
    /// <param name="folderId">folder id</param>
    /// <returns></returns>
    IAsyncEnumerable<Folder<T>> GetParentFoldersAsync(T folderId);
    /// <summary>
    ///     save or update folder
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    Task<T> SaveFolderAsync(Folder<T> folder);
    /// <summary>
    ///     delete folder
    /// </summary>
    /// <param name="folderId">folder id</param>
    Task DeleteFolderAsync(T folderId);
    /// <summary>
    ///  move folder
    /// </summary>
    /// <param name="folderId">folder id</param>
    /// <param name="toFolderId">destination folder id</param>
    /// <param name="cancellationToken"></param>
    Task<T> MoveFolderAsync(T folderId, T toFolderId, CancellationToken? cancellationToken);
    Task<TTo> MoveFolderAsync<TTo>(T folderId, TTo toFolderId, CancellationToken? cancellationToken);
    Task<string> MoveFolderAsync(T folderId, string toFolderId, CancellationToken? cancellationToken);
    Task<int> MoveFolderAsync(T folderId, int toFolderId, CancellationToken? cancellationToken);

    /// <summary>
    ///     copy folder
    /// </summary>
    /// <param name="folderId"></param>
    /// <param name="toFolderId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns> 
    /// </returns>
    Task<Folder<T>> CopyFolderAsync(T folderId, T toFolderId, CancellationToken? cancellationToken);
    Task<Folder<TTo>> CopyFolderAsync<TTo>(T folderId, TTo toFolderId, CancellationToken? cancellationToken);
    Task<Folder<string>> CopyFolderAsync(T folderId, string toFolderId, CancellationToken? cancellationToken);
    Task<Folder<int>> CopyFolderAsync(T folderId, int toFolderId, CancellationToken? cancellationToken);

    /// <summary>
    /// Validate the transfer operation directory to another directory.
    /// </summary>
    /// <param name="folderIds"></param>
    /// <param name="to"></param>
    /// <returns>
    /// Returns pair of file ID, file name, in which the same name.
    /// </returns>
    Task<IDictionary<T, TTo>> CanMoveOrCopyAsync<TTo>(T[] folderIds, T to);
    Task<IDictionary<T, TTo>> CanMoveOrCopyAsync<TTo>(T[] folderIds, TTo to);
    Task<IDictionary<T, TTo>> CanMoveOrCopyAsync<TTo>(T[] folderIds, string to);
    Task<IDictionary<T, TTo>> CanMoveOrCopyAsync<TTo>(T[] folderIds, int to);

    /// <summary>
    ///     Rename folder
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="newTitle">new name</param>
    Task<T> RenameFolderAsync(Folder<T> folder, string newTitle);

    /// <summary>
    ///    Gets the number of files and folders to the container in your
    /// </summary>
    /// <param name="folderId">folder id</param>
    /// <returns></returns>
    Task<int> GetItemsCountAsync(T folderId);

    /// <summary>
    ///    Check folder on emptiness
    /// </summary>
    /// <param name="folderId">folder id</param>
    /// <returns></returns>
    Task<bool> IsEmptyAsync(T folderId);
    /// <summary>
    /// Check the need to use the trash before removing
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    bool UseTrashForRemoveAsync(Folder<T> folder);

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
    Task<long> GetMaxUploadSizeAsync(T folderId, bool chunkedUpload = false);

    IDataWriteOperator CreateDataWriteOperator(
            T folderId,
            CommonChunkedUploadSession chunkedUploadSession,
            CommonChunkedUploadSessionHolder sessionHolder);


    #region Only for TMFolderDao

    /// <summary>
    /// Set created by
    /// </summary>
    /// <param name="folderIds"></param>
    /// <param name="newOwnerId"></param>
    Task ReassignFoldersAsync(T[] folderIds, Guid newOwnerId);


    /// <summary>
    /// Search the list of folders containing text in title
    /// Only in TMFolderDao
    /// </summary>
    /// <param name="text"></param>
    /// <param name="bunch"></param>
    /// <returns></returns>
    IAsyncEnumerable<Folder<T>> SearchFoldersAsync(string text, bool bunch = false);

    /// <summary>
    /// Only in TMFolderDao
    /// </summary>
    /// <param name="module"></param>
    /// <param name="bunch"></param>
    /// <param name="data"></param>
    /// <param name="createIfNotExists"></param>
    /// <returns></returns>
    Task<T> GetFolderIDAsync(string module, string bunch, string data, bool createIfNotExists);

    IAsyncEnumerable<T> GetFolderIDsAsync(string module, string bunch, IEnumerable<string> data, bool createIfNotExists);

    /// <summary>
    ///  Returns id folder "Shared Documents"
    /// Only in TMFolderDao
    /// </summary>
    /// <returns></returns>
    Task<T> GetFolderIDCommonAsync(bool createIfNotExists);

    /// <summary>
    ///  Returns id folder "My Documents"
    /// Only in TMFolderDao
    /// </summary>
    /// <param name="createIfNotExists"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<T> GetFolderIDUserAsync(bool createIfNotExists, Guid? userId = null);

    /// <summary>
    /// Returns id folder "Shared with me"
    /// Only in TMFolderDao
    /// </summary>
    /// <param name="createIfNotExists"></param>
    /// <returns></returns>
    Task<T> GetFolderIDShareAsync(bool createIfNotExists);

    /// <summary>
    /// Returns id folder "Recent"
    /// Only in TMFolderDao
    /// </summary>
    /// <param name="createIfNotExists"></param>
    /// <returns></returns>
    Task<T> GetFolderIDRecentAsync(bool createIfNotExists);

    /// <summary>

    /// <summary>
    /// Returns id folder "Favorites"
    /// Only in TMFolderDao
    /// </summary>
    /// <param name="createIfNotExists"></param>
    /// <returns></returns>
    Task<T> GetFolderIDFavoritesAsync(bool createIfNotExists);

    /// <summary>
    /// Returns id folder "Templates"
    /// Only in TMFolderDao
    /// </summary>
    /// <param name="createIfNotExists"></param>
    /// <returns></returns>
    Task<T> GetFolderIDTemplatesAsync(bool createIfNotExists);

    /// <summary>
    /// Returns id folder "Privacy"
    /// Only in TMFolderDao
    /// </summary>
    /// <param name="createIfNotExists"></param>
    /// <returns></returns>
    Task<T> GetFolderIDPrivacyAsync(bool createIfNotExists, Guid? userId = null);

    /// <summary>
    /// Returns id folder "Trash"
    /// Only in TMFolderDao
    /// </summary>
    /// <param name="createIfNotExists"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<T> GetFolderIDTrashAsync(bool createIfNotExists, Guid? userId = null);

    /// <summary>
    /// Returns id folder "Projects"
    /// Only in TMFolderDao
    /// </summary>
    /// <param name="createIfNotExists"></param>
    /// <returns></returns>
    Task<T> GetFolderIDProjectsAsync(bool createIfNotExists);

    /// <summary>
    /// Returns id folder "VirtualRooms"
    /// Only in TMFolderDao
    /// </summary>
    /// <param name="createIfNotExists"></param>
    /// <returns></returns>
    Task<T> GetFolderIDVirtualRooms(bool createIfNotExists);

    /// <summary>
    /// Returns id folder "Archive"
    /// Only in TMFolderDao
    /// </summary>
    /// <param name="createIfNotExists"></param>
    /// <returns></returns>
    Task<T> GetFolderIDArchive(bool createIfNotExists);

    /// <summary>
    /// Return id of related object
    /// Only in TMFolderDao
    /// </summary>
    /// <param name="folderID"></param>
    /// <returns></returns>
    Task<string> GetBunchObjectIDAsync(T folderID);

    /// <summary>
    /// Return ids of related objects
    /// Only in TMFolderDao
    /// </summary>
    /// <param name="folderIDs"></param>
    /// <returns></returns>
    Task<Dictionary<string, string>> GetBunchObjectIDsAsync(List<T> folderIDs);
    IAsyncEnumerable<FolderWithShare> GetFeedsForRoomsAsync(int tenant, DateTime from, DateTime to);
    IAsyncEnumerable<FolderWithShare> GetFeedsForFoldersAsync(int tenant, DateTime from, DateTime to);
    IAsyncEnumerable<ParentRoomPair> GetParentRoomsAsync(IEnumerable<int> foldersIds);

    IAsyncEnumerable<int> GetTenantsWithFoldersFeedsAsync(DateTime fromTime);
    IAsyncEnumerable<int> GetTenantsWithRoomsFeedsAsync(DateTime fromTime);
    IAsyncEnumerable<OriginData> GetOriginsDataAsync(IEnumerable<T> entriesIds);

    /// <summary>
    /// Tries to return id of the parent virtual room
    /// Only in TMFolderDao
    /// </summary>
    /// <param name="fileEntry"></param>
    /// <returns></returns>
    Task<(int RoomId, string RoomTitle)> GetParentRoomInfoFromFileEntryAsync<TTo>(FileEntry<TTo> fileEntry);
    Task<int> GetFoldersCountAsync(T parentId, FilterType filterType, bool subjectGroup, Guid subjectId, string searchText, bool withSubfolders = false, bool excludeSubject = false);

    #endregion
}