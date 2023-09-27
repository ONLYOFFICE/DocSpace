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
public interface IFileDao<T>
{
    /// <summary>
    ///     Clear the application cache for the specific file
    /// </summary>
    Task InvalidateCacheAsync(T fileId);
    /// <summary>
    ///     Receive file
    /// </summary>
    /// <param name="fileId">file id</param>
    /// <returns></returns>
    Task<File<T>> GetFileAsync(T fileId);

    /// <summary>
    ///     Receive file
    /// </summary>
    /// <param name="fileId">file id</param>
    /// <param name="fileVersion">file version</param>
    /// <returns></returns>
    Task<File<T>> GetFileAsync(T fileId, int fileVersion);

    /// <summary>
    ///     Receive file
    /// </summary>
    /// <param name="parentId">folder id</param>
    /// <param name="title">file name</param>
    /// <returns>
    ///   file
    /// </returns>
    Task<File<T>> GetFileAsync(T parentId, string title);
    /// <summary>
    ///     Receive last file without forcesave
    /// </summary>
    /// <param name="fileId">file id</param>
    /// <param name="fileVersion"></param>
    /// <returns></returns>
    Task<File<T>> GetFileStableAsync(T fileId, int fileVersion = -1);
    /// <summary>
    ///  Returns all versions of the file
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    IAsyncEnumerable<File<T>> GetFileHistoryAsync(T fileId);

    /// <summary>
    ///     Gets the file (s) by ID (s)
    /// </summary>
    /// <param name="fileIds">id file</param>
    /// <returns></returns>
    IAsyncEnumerable<File<T>> GetFilesAsync(IEnumerable<T> fileIds);

    /// <summary>
    ///     Gets the file (s) by ID (s) for share
    /// </summary>
    /// <param name="fileIds">id file</param>
    /// <param name="filterType"></param>
    /// <param name="subjectGroup"></param>
    /// <param name="subjectID"></param>
    /// <param name="searchText"></param>
    /// <param name="searchInContent"></param>
    /// <param name="checkShared"></param>
    /// <returns></returns>
    IAsyncEnumerable<File<T>> GetFilesFilteredAsync(IEnumerable<T> fileIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool checkShared = false);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parentId"></param>
    /// <returns></returns>
    IAsyncEnumerable<T> GetFilesAsync(T parentId);

    /// <summary>
    ///     Get files in folder
    /// </summary>
    /// <param name="parentId">folder id</param>
    /// <param name="orderBy"></param>
    /// <param name="filterType">filterType type</param>
    /// <param name="subjectGroup"></param>
    /// <param name="subjectID"></param>
    /// <param name="searchText"> </param>
    /// <param name="searchInContent"></param>
    /// <param name="withSubfolders"> </param>
    /// <param name="exludeSubject"> </param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <param name="roomId"></param>
    /// <returns>list of files</returns>
    /// <remarks>
    ///    Return only the latest versions of files of a folder
    /// </remarks>
    IAsyncEnumerable<File<T>> GetFilesAsync(T parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent,
        bool withSubfolders = false, bool exludeSubject = false, int offset = 0, int count = -1, T roomId = default);

    /// <summary>
    /// Get stream of file
    /// </summary>
    /// <param name="file"></param>
    /// <returns>Stream</returns>
    Task<Stream> GetFileStreamAsync(File<T> file);

    /// <summary>
    /// Get stream of file
    /// </summary>
    /// <param name="file"></param>
    /// <param name="offset"></param>
    /// <returns>Stream</returns>
    Task<Stream> GetFileStreamAsync(File<T> file, long offset);

    /// <summary>
    /// Get presigned uri
    /// </summary>
    /// <param name="file"></param>
    /// <param name="expires"></param>
    /// <returns>Stream uri</returns>
    Task<Uri> GetPreSignedUriAsync(File<T> file, TimeSpan expires);

    /// <summary>
    ///  Check is supported PreSignedUri
    /// </summary>
    /// <param name="file"></param>
    /// <returns>Stream uri</returns>
    Task<bool> IsSupportedPreSignedUriAsync(File<T> file);

    /// <summary>
    ///  Saves / updates the version of the file
    ///  and save stream of file
    /// </summary>
    /// <param name="file"></param>
    /// <param name="fileStream"> </param>
    /// <returns></returns>
    /// <remarks>
    /// Updates the file if:
    /// - The file comes with the given id
    /// - The file with that name in the folder / container exists
    ///
    /// Save in all other cases
    /// </remarks>
    Task<File<T>> SaveFileAsync(File<T> file, Stream fileStream);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="file"></param>
    /// <param name="fileStream"></param>
    /// <returns></returns>
    Task<File<T>> ReplaceFileVersionAsync(File<T> file, Stream fileStream);
    /// <summary>
    ///   Deletes a file including all previous versions
    /// </summary>
    /// <param name="fileId">file id</param>
    Task DeleteFileAsync(T fileId);
    /// <summary>
    ///     Checks whether or not file
    /// </summary>
    /// <param name="title">file name</param>
    /// <param name="folderId">folder id</param>
    /// <returns>Returns true if the file exists, otherwise false</returns>
    Task<bool> IsExistAsync(string title, object folderId);
    /// <summary>
    ///   Moves a file or set of files in a folder
    /// </summary>
    /// <param name="fileId">file id</param>
    /// <param name="toFolderId">The ID of the destination folder</param>
    Task<T> MoveFileAsync(T fileId, T toFolderId);
    Task<TTo> MoveFileAsync<TTo>(T fileId, TTo toFolderId);
    Task<string> MoveFileAsync(T fileId, string toFolderId);
    Task<int> MoveFileAsync(T fileId, int toFolderId);

    /// <summary>
    ///  Copy the files in a folder
    /// </summary>
    /// <param name="fileId">file id</param>
    /// <param name="toFolderId">The ID of the destination folder</param>
    Task<File<T>> CopyFileAsync(T fileId, T toFolderId);
    Task<File<TTo>> CopyFileAsync<TTo>(T fileId, TTo toFolderId);
    Task<File<string>> CopyFileAsync(T fileId, string toFolderId);
    Task<File<int>> CopyFileAsync(T fileId, int toFolderId);

    /// <summary>
    ///   Rename file
    /// </summary>
    /// <param name="file"></param>
    /// <param name="newTitle">new name</param>
    Task<T> FileRenameAsync(File<T> file, string newTitle);

    /// <summary>
    ///   Update comment file
    /// </summary>
    /// <param name="fileId">file id</param>
    /// <param name="fileVersion">file version</param>
    /// <param name="comment">new comment</param>
    Task<string> UpdateCommentAsync(T fileId, int fileVersion, string comment);
    /// <summary>
    ///   Complete file version
    /// </summary>
    /// <param name="fileId">file id</param>
    /// <param name="fileVersion">file version</param>
    Task CompleteVersionAsync(T fileId, int fileVersion);
    /// <summary>
    ///   Continue file version
    /// </summary>
    /// <param name="fileId">file id</param>
    /// <param name="fileVersion">file version</param>
    Task ContinueVersionAsync(T fileId, int fileVersion);
    /// <summary>
    /// Check the need to use the trash before removing
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    bool UseTrashForRemove(File<T> file);
    string GetUniqFilePath(File<T> file, string fileTitle);

    #region chunking

    Task<ChunkedUploadSession<T>> CreateUploadSessionAsync(File<T> file, long contentLength);
    Task<File<T>> UploadChunkAsync(ChunkedUploadSession<T> uploadSession, Stream chunkStream, long chunkLength);
    Task<File<T>> FinalizeUploadSessionAsync(ChunkedUploadSession<T> uploadSession);
    Task AbortUploadSessionAsync(ChunkedUploadSession<T> uploadSession);
    #endregion

    #region Only in TMFileDao

    /// <summary>
    /// Set created by
    /// </summary>
    /// <param name="oldOwnerId"></param>
    /// <param name="newOwnerId"></param>
    /// <param name="exceptFolderIds"></param>
    Task ReassignFilesAsync(Guid oldOwnerId, Guid newOwnerId, IEnumerable<T> exceptFolderIds);

    /// <summary>
    /// Search files in SharedWithMe &amp; Projects
    /// </summary>
    /// <param name="parentIds"></param>
    /// <param name="filterType"></param>
    /// <param name="subjectGroup"></param>
    /// <param name="subjectID"></param>
    /// <param name="searchText"></param>
    /// <param name="searchInContent"></param>
    /// <returns></returns>
    IAsyncEnumerable<File<T>> GetFilesAsync(IEnumerable<T> parentIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent);
    /// <summary>
    /// Search the list of files containing text
    /// Only in TMFileDao
    /// </summary>
    /// <param name="text">search text</param>
    /// <param name="bunch"></param>
    /// <returns>list of files</returns>
    IAsyncEnumerable<File<T>> SearchAsync(string text, bool bunch = false);
    /// <summary>
    ///   Checks whether file exists on storage
    /// </summary>
    /// <param name="file">file</param>
    /// <returns></returns>

    Task<bool> IsExistOnStorageAsync(File<T> file);

    Task SaveEditHistoryAsync(File<T> file, string changes, Stream differenceStream);

    IAsyncEnumerable<EditHistory> GetEditHistoryAsync(DocumentServiceHelper documentServiceHelper, T fileId, int fileVersion = 0);

    Task<Stream> GetDifferenceStreamAsync(File<T> file);

    Task<bool> ContainChangesAsync(T fileId, int fileVersion);

    Task SetThumbnailStatusAsync(File<T> file, Thumbnail status);

    string GetUniqThumbnailPath(File<T> file, int width, int height);

    Task<Stream> GetThumbnailAsync(File<T> file, int width, int height);

    Task<Stream> GetThumbnailAsync(T fileId, int width, int height);

    IAsyncEnumerable<FileWithShare> GetFeedsAsync(int tenant, DateTime from, DateTime to);

    IAsyncEnumerable<int> GetTenantsWithFeedsAsync(DateTime fromTime, bool includeSecurity);

    Task<EntryProperties> GetProperties(T fileId);

    Task SaveProperties(T fileId, EntryProperties entryProperties);

    Task<int> GetFilesCountAsync(T parentId, FilterType filterType, bool subjectGroup, Guid subjectId, string searchText, bool searchInContent, bool withSubfolders = false,
        bool excludeSubject = false, T roomId = default);

    #endregion
}