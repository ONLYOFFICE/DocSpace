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

namespace ASC.Files.Core.Log;
internal static partial class FileHandlerLogger
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "DocService StreamFile payload: {payload}")]
    public static partial void DebugDocServiceStreamFilePayload(this ILogger<FileHandlerService> logger, string payload);

    [LoggerMessage(Level = LogLevel.Debug, Message = "DocService track fileid: {fileId}")]
    public static partial void DebugDocServiceTrackFileid(this ILogger<FileHandlerService> logger, string fileId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "DocService track body: {body}")]
    public static partial void DebugDocServiceTrackBody(this ILogger<FileHandlerService> logger, string body);

    [LoggerMessage(Level = LogLevel.Debug, Message = "DocService track payload: {payload}")]
    public static partial void DebugDocServiceTrackPayload(this ILogger<FileHandlerService> logger, string payload);

    [LoggerMessage(Level = LogLevel.Error, Message = "BulkDownloadFile failed for user {id}")]
    public static partial void ErrorBulkDownloadFileFailed(this ILogger<FileHandlerService> logger, Guid id, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "DownloadFile")]
    public static partial void ErrorDownloadFile(this ILogger<FileHandlerService> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Url: {url} IsClientConnected:{isCancellationRequested}, line number:{line} frame:{stackFrame}")]
    public static partial void ErrorUrl(this ILogger<FileHandlerService> logger, Uri url, bool isCancellationRequested, int line, StackFrame stackFrame, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "{authKey} {validateResult}: {url}")]
    public static partial void Error(this ILogger<FileHandlerService> logger, string authKey, EmailValidationKeyProvider.ValidationResult validateResult, Uri url, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Download stream header {url}")]
    public static partial void ErrorDownloadStreamHeader(this ILogger<FileHandlerService> logger, Uri url, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error for: {url}")]
    public static partial void ErrorForUrl(this ILogger<FileHandlerService> logger, Uri url, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "StreamFile")]
    public static partial void ErrorStreamFile(this ILogger<FileHandlerService> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "EmptyFile")]
    public static partial void ErrorEmptyFile(this ILogger<FileHandlerService> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "TempFile")]
    public static partial void ErrorTempFile(this ILogger<FileHandlerService> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "DifferenceFile")]
    public static partial void ErrorDifferenceFile(this ILogger<FileHandlerService> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Thumbnail")]
    public static partial void ErrorThumbnail(this ILogger<FileHandlerService> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "FileHandler")]
    public static partial void ErrorFileHandler(this ILogger<FileHandlerService> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "DocService track error read body")]
    public static partial void ErrorDocServiceTrackReadBody(this ILogger<FileHandlerService> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "DocService track header")]
    public static partial void ErrorDocServiceTrackHeader(this ILogger<FileHandlerService> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "DocService track:")]
    public static partial void ErrorDocServiceTrack(this ILogger<FileHandlerService> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "BulkDownload file error. File is not exist on storage. UserId: {userId}.")]
    public static partial void ErrorBulkDownloadFile(this ILogger<FileHandlerService> logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Download file error. File is not exist on storage. File id: {fileId}.")]
    public static partial void ErrorDownloadFile2(this ILogger<FileHandlerService> logger, string fileId);

    [LoggerMessage(Level = LogLevel.Error, Message = "DocService track auth error: {validateResult}, {authKey}: {auth}")]
    public static partial void ErrorDocServiceTrackAuth(this ILogger<FileHandlerService> logger, EmailValidationKeyProvider.ValidationResult validateResult, string authKey, string auth);

    [LoggerMessage(Level = LogLevel.Error, Message = "DocService track header is null")]
    public static partial void ErrorDocServiceTrackHeaderIsNull(this ILogger<FileHandlerService> logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Converting {fileTitle} (fileId: {fileId}) to mp4")]
    public static partial void InformationConvertingToMp4(this ILogger<FileHandlerService> logger, string fileTitle, string fileId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Starting file download (chunk {offset}-{endOffset})")]
    public static partial void InformationStartingFileDownLoad(this ILogger<FileHandlerService> logger, long offset, long endOffset);
}
