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
using System.IO;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Files.Core.Security;
using ASC.Web.Files.Services.DocumentService;

namespace ASC.Files.Core
{
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
        /// <returns></returns>
        IAsyncEnumerable<File<T>> GetFilesFilteredAsync(IEnumerable<T> fileIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool checkShared = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        Task<List<T>> GetFilesAsync(T parentId);

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
        /// <returns>list of files</returns>
        /// <remarks>
        ///    Return only the latest versions of files of a folder
        /// </remarks>
        IAsyncEnumerable<File<T>> GetFilesAsync(T parentId, OrderBy orderBy, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent, bool withSubfolders = false);

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
        Task AbortUploadSessionAsync(ChunkedUploadSession<T> uploadSession);
        #endregion

        #region Only in TMFileDao

        /// <summary>
        /// Set created by
        /// </summary>
        /// <param name="fileIds"></param>
        /// <param name="newOwnerId"></param>
        Task ReassignFilesAsync(T[] fileIds, Guid newOwnerId);

        /// <summary>
        /// Search files in SharedWithMe & Projects
        /// </summary>
        /// <param name="parentIds"></param>
        /// <param name="filterType"></param>
        /// <param name="subjectGroup"></param>
        /// <param name="subjectID"></param>
        /// <param name="searchText"></param>
        /// <param name="searchInContent"></param>
        /// <returns></returns>
        Task<List<File<T>>> GetFilesAsync(IEnumerable<T> parentIds, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText, bool searchInContent);
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

        Task<List<EditHistory>> GetEditHistoryAsync(DocumentServiceHelper documentServiceHelper, T fileId, int fileVersion = 0);

        Task<Stream> GetDifferenceStreamAsync(File<T> file);

        Task<bool> ContainChangesAsync(T fileId, int fileVersion);

        Task SaveThumbnailAsync(File<T> file, Stream thumbnail);

        Task<Stream> GetThumbnailAsync(File<T> file);

        Task<IEnumerable<(File<int>, SmallShareRecord)>> GetFeedsAsync(int tenant, DateTime from, DateTime to);

        Task<IEnumerable<int>> GetTenantsWithFeedsAsync(DateTime fromTime);

        #endregion
    }
}