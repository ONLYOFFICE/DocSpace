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

namespace ASC.Data.Storage;

///<summary>
/// Interface for working with files
///</summary>
public interface IDataStore
{
    IDataWriteOperator CreateDataWriteOperator(
            CommonChunkedUploadSession chunkedUploadSession,
            CommonChunkedUploadSessionHolder sessionHolder,
            bool isConsumerStorage = false);
    string GetBackupExtension(bool isConsumerStorage = false);

    IQuotaController QuotaController { get; set; }

    TimeSpan GetExpire(string domain);

    ///<summary>
    /// Get absolute Uri for html links to handler
    ///</summary>
    ///<param name="path"></param>
    ///<returns></returns>
    Task<Uri> GetUriAsync(string path);

    ///<summary>
    /// Get absolute Uri for html links to handler
    ///</summary>
    ///<param name="domain"></param>
    ///<param name="path"></param>
    ///<returns></returns>
    Task<Uri> GetUriAsync(string domain, string path);

    /// <summary>
    /// Get absolute Uri for html links to handler
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="path"></param>
    /// <param name="expire"></param>
    /// <param name="headers"></param>
    /// <returns></returns>
    Task<Uri> GetPreSignedUriAsync(string domain, string path, TimeSpan expire, IEnumerable<string> headers);
    
    ///<summary>
    /// Supporting generate uri to the file
    ///</summary>
    ///<returns></returns>
    bool IsSupportInternalUri { get; }

    ///<summary>
    /// Supporting generate uri to the file
    ///</summary>
    ///<returns></returns>
    bool IsSupportCdnUri { get; }


    /// <summary>
    /// Get absolute Uri for html links
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="path"></param>
    /// <param name="expire"></param>
    /// <param name="headers"></param>
    /// <returns></returns>
    Task<Uri> GetInternalUriAsync(string domain, string path, TimeSpan expire, IEnumerable<string> headers);

    /// <summary>
    /// Get absolute Uri via CDN for html links
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="path"></param>
    /// <param name="expire"></param>
    /// <param name="headers"></param>
    /// <returns></returns>
    Task<Uri> GetCdnPreSignedUriAsync(string domain, string path, TimeSpan expire, IEnumerable<string> headers);

    ///<summary>
    /// A stream of read-only. In the case of the C3 stream NetworkStream general, and with him we have to work
    /// Very carefully as a Jedi cutter groin lightsaber.
    ///</summary>
    ///<param name="domain"></param>
    ///<param name="path"></param>
    ///<returns></returns>
    Task<Stream> GetReadStreamAsync(string domain, string path);

    ///<summary>
    /// A stream of read-only. In the case of the C3 stream NetworkStream general, and with him we have to work
    /// Very carefully as a Jedi cutter groin lightsaber.
    ///</summary>
    ///<param name="domain"></param>
    ///<param name="path"></param>
    ///<param name="offset"></param>
    ///<returns></returns>
    Task<Stream> GetReadStreamAsync(string domain, string path, long offset);

    ///<summary>
    /// Saves the contents of the stream in the repository.
    ///</summary>
    /// <param Name="domain"> </param>
    /// <param Name="path"> </param>
    /// <param Name="stream"> flow. Is read from the current position! Desirable to set to 0 when the transmission MemoryStream instance </param>
    /// <returns> </returns>
    Task<Uri> SaveAsync(string domain, string path, Stream stream);

    /// <summary>
    /// Saves the contents of the stream in the repository.
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="path"></param>
    /// <param name="stream"></param>
    /// <param name="acl"></param>
    /// <returns></returns>
    Task<Uri> SaveAsync(string domain, string path, Stream stream, ACL acl);

    /// <summary>
    /// Saves the contents of the stream in the repository.
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="path"></param>
    /// <param name="stream"></param>
    /// <param name="attachmentFileName"></param>
    /// <returns></returns>
    Task<Uri> SaveAsync(string domain, string path, Stream stream, string attachmentFileName);

    /// <summary>
    /// Saves the contents of the stream in the repository.
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="path"></param>
    /// <param name="stream"></param>
    /// <param name="contentType"></param>
    /// <param name="contentDisposition"></param>
    /// <returns></returns>
    Task<Uri> SaveAsync(string domain, string path, Stream stream, string contentType, string contentDisposition);

    /// <summary>
    /// Saves the contents of the stream in the repository.
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="path"></param>
    /// <param name="stream"></param>
    /// <param name="contentEncoding"></param>
    /// <param name="cacheDays"></param>
    /// <returns></returns>
    Task<Uri> SaveAsync(string domain, string path, Stream stream, string contentEncoding, int cacheDays);

    Task<string> InitiateChunkedUploadAsync(string domain, string path);

    Task<string> UploadChunkAsync(string domain, string path, string uploadId, Stream stream, long defaultChunkSize, int chunkNumber, long chunkLength);

    Task<Uri> FinalizeChunkedUploadAsync(string domain, string path, string uploadId, Dictionary<int, string> eTags);

    Task AbortChunkedUploadAsync(string domain, string path, string uploadId);

    bool IsSupportChunking { get; }

    bool IsSupportedPreSignedUri { get; }

    ///<summary>
    /// Deletes file
    ///</summary>
    ///<param name="domain"></param>
    ///<param name="path"></param>
    Task DeleteAsync(string domain, string path);

    ///<summary>
    /// Deletes file by mask
    ///</summary>
    ///<param name="domain"></param>
    ///<param name="folderPath"></param>
    ///<param name="pattern">Wildcard mask (*.png)</param>
    ///<param name="recursive"></param>
    Task DeleteFilesAsync(string domain, string folderPath, string pattern, bool recursive);

    ///<summary>
    /// Deletes files
    ///</summary>
    ///<param name="domain"></param>
    ///<param name="paths"></param>
    Task DeleteFilesAsync(string domain, List<string> paths);

    ///<summary>
    /// Deletes file by last modified date
    ///</summary>
    ///<param name="domain"></param>
    ///<param name="folderPath"></param>
    ///<param name="fromDate"></param>
    ///<param name="toDate"></param>
    Task DeleteFilesAsync(string domain, string folderPath, DateTime fromDate, DateTime toDate);

    ///<summary>
    /// Moves the contents of one directory to another. s3 for a very expensive procedure.
    ///</summary>
    ///<param name="srcdomain"></param>
    ///<param name="srcdir"></param>
    ///<param name="newdomain"></param>
    ///<param name="newdir"></param>
    Task MoveDirectoryAsync(string srcdomain, string srcdir, string newdomain, string newdir);

    ///<summary>
    /// Moves file
    ///</summary>
    ///<param name="srcdomain"></param>
    ///<param name="srcpath"></param>
    ///<param name="newdomain"></param>
    ///<param name="newpath"></param>
    ///<param name="quotaCheckFileSize"></param>
    ///<returns></returns>
    Task<Uri> MoveAsync(string srcdomain, string srcpath, string newdomain, string newpath, bool quotaCheckFileSize = true);

    ///<summary>
    /// Saves the file in the temp. In fact, almost no different from the usual Save except that generates the file name itself. An inconvenient thing.
    ///</summary>
    ///<param name="domain"></param>
    ///<param name="assignedPath"></param>
    ///<param name="stream"></param>
    ///<returns></returns>
    Task<(Uri, string)> SaveTempAsync(string domain, Stream stream);

    /// <summary>
    ///  Returns a list of links to all subfolders
    /// </summary>
    /// <param name="domain"></param>
    /// <param name="path"></param>
    /// <param name="recursive">iterate subdirectories or not</param>
    /// <returns></returns>
    IAsyncEnumerable<string> ListDirectoriesRelativeAsync(string domain, string path, bool recursive);

    ///<summary>
    /// Returns a list of links to all files
    ///</summary>
    ///<param name="domain"></param>
    ///<param name="path"></param>
    ///<param name="pattern">Wildcard mask (*. jpg for example)</param>
    ///<param name="recursive">iterate subdirectories or not</param>
    ///<returns></returns>
    IAsyncEnumerable<Uri> ListFilesAsync(string domain, string path, string pattern, bool recursive);

    ///<summary>
    /// Returns a list of relative paths for all files
    ///</summary>
    ///<param name="domain"></param>
    ///<param name="path"></param>
    ///<param name="pattern">Wildcard mask (*. jpg for example)</param>
    ///<param name="recursive">iterate subdirectories or not</param>
    ///<returns></returns>
    IAsyncEnumerable<string> ListFilesRelativeAsync(string domain, string path, string pattern, bool recursive);

    ///<summary>
    /// Checks whether a file exists. On s3 it took long time.
    ///</summary>
    ///<param name="domain"></param>
    ///<param name="path"></param>
    ///<returns></returns>

    Task<bool> IsFileAsync(string domain, string path);

    ///<summary>
    /// Checks whether a directory exists. On s3 it took long time.
    ///</summary>
    ///<param name="domain"></param>
    ///<param name="path"></param>
    ///<returns></returns>
    Task<bool> IsDirectoryAsync(string domain, string path);

    Task DeleteDirectoryAsync(string domain, string path);

    Task<long> GetFileSizeAsync(string domain, string path);

    Task<long> GetDirectorySizeAsync(string domain, string path);

    Task<long> ResetQuotaAsync(string domain);

    Task<long> GetUsedQuotaAsync(string domain);

    Task<Uri> CopyAsync(string srcdomain, string path, string newdomain, string newpath);
    Task CopyDirectoryAsync(string srcdomain, string dir, string newdomain, string newdir);

    //Then there are restarted methods without domain. functionally identical to the top

#pragma warning disable 1591
    Task<Stream> GetReadStreamAsync(string path);
    Task<Uri> SaveAsync(string path, Stream stream, string attachmentFileName);
    Task<Uri> SaveAsync(string path, Stream stream);
    Task DeleteAsync(string path);
    Task DeleteFilesAsync(string folderPath, string pattern, bool recursive);
    Task<Uri> MoveAsync(string srcpath, string newdomain, string newpath);
    Task<(Uri, string)> SaveTempAsync(Stream stream);
    IAsyncEnumerable<string> ListDirectoriesRelativeAsync(string path, bool recursive);
    IAsyncEnumerable<Uri> ListFilesAsync(string path, string pattern, bool recursive);
    Task<bool> IsFileAsync(string path);
    Task<bool> IsDirectoryAsync(string path);
    Task DeleteDirectoryAsync(string path);
    Task<long> GetFileSizeAsync(string path);
    Task<long> GetDirectorySizeAsync(string path);
    Task<Uri> CopyAsync(string path, string newdomain, string newpath);
    Task CopyDirectoryAsync(string dir, string newdomain, string newdir);
#pragma warning restore 1591


    IDataStore Configure(string tenant, Handler handlerConfig, Module moduleConfig, IDictionary<string, string> props);
    IDataStore SetQuotaController(IQuotaController controller);

    Task<string> SavePrivateAsync(string domain, string path, Stream stream, DateTime expires);
    Task DeleteExpiredAsync(string domain, string path, TimeSpan oldThreshold);

    string GetUploadForm(string domain, string directoryPath, string redirectTo, long maxUploadSize,
                         string contentType, string contentDisposition, string submitLabel);
    string GetUploadUrl();

    string GetPostParams(string domain, string directoryPath, long maxUploadSize, string contentType,
                         string contentDisposition);

    Task<string> GetFileEtagAsync(string domain, string path);
}
