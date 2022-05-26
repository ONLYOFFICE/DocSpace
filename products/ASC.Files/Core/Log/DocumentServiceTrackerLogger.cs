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
internal static partial class DocumentServiceTrackerLogger
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Drop command: fileId '{fileId}' docKey '{fileKey}' for user {user}")]
    public static partial void DebugDropCommand(this ILogger<DocumentServiceTrackerHelper> logger, string fileId, string fileKey, string user, Exception exception);

    [LoggerMessage(Level = LogLevel.Debug, Message = "DocService storing to {path}")]
    public static partial void DebugDocServiceStoring(this ILogger<DocumentServiceTrackerHelper> logger, string path);

    [LoggerMessage(Level = LogLevel.Error, Message = "DocService save error. Version update. File id: '{fileId}'. UserId: {userId}. DocKey '{fileData}'")]
    public static partial void ErrorDocServiceSaveVersionUpdate(this ILogger<DocumentServiceTrackerHelper> logger, string fileId, Guid userId, string fileData, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "DocService save error. File id: '{fileId}'. UserId: {userId}. DocKey '{docKey}'. DownloadUri: {downloadUri}")]
    public static partial void ErrorDocServiceSave(this ILogger<DocumentServiceTrackerHelper> logger, string fileId, Guid userId, string docKey, string downloadUri, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "DocService mailMerge{index} error: userId - {userId}, url - {url}")]
    public static partial void ErrorDocServiceMailMerge(this ILogger<DocumentServiceTrackerHelper> logger, string index, Guid userId, string url, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "DocService Error on save file to temp store")]
    public static partial void ErrorDocServiceSaveFileToTempStore(this ILogger<DocumentServiceTrackerHelper> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "DocService save history error")]
    public static partial void ErrorDocServiceSavehistory(this ILogger<DocumentServiceTrackerHelper> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "DocService drop failed for users {users}")]
    public static partial void ErrorDocServiceDropFailed(this ILogger<DocumentServiceTrackerHelper> logger, List<string> users);

    [LoggerMessage(Level = LogLevel.Error, Message = "DocService saving file {fileId} ({docKey}) with key {fileData}")]
    public static partial void ErrorDocServiceSavingFile(this ILogger<DocumentServiceTrackerHelper> logger, string fileId, string docKey, string fileData);

    [LoggerMessage(Level = LogLevel.Error, Message = "DocService save error. Empty url. File id: '{fileId}'. UserId: {userId}. DocKey '{key}'")]
    public static partial void ErrorDocServiceSave2(this ILogger<DocumentServiceTrackerHelper> logger, string fileId, Guid userId, string key);

    [LoggerMessage(Level = LogLevel.Information, Message = "DocService save error: anonymous author - {userId}")]
    public static partial void InformationDocServiceSaveError(this ILogger<DocumentServiceTrackerHelper> logger, Guid userId, Exception exception);

    [LoggerMessage(Level = LogLevel.Information, Message = "DocService editing file {fileId} ({docKey}) with key {fileKey} for {users}")]
    public static partial void InformationDocServiceEditingFile(this ILogger<DocumentServiceTrackerHelper> logger, string fileId, string docKey, string fileKey, List<string> users);

    [LoggerMessage(Level = LogLevel.Information, Message = "DocService userId is not Guid: {user}")]
    public static partial void InformationDocServiceUserIdIsNotGuid(this ILogger<DocumentServiceTrackerHelper> logger, string user);

    [LoggerMessage(Level = LogLevel.Information, Message = "DocService mailMerge {index}/{count} send: {response}")]
    public static partial void InformationDocServiceMailMerge(this ILogger<DocumentServiceTrackerHelper> logger, int index, int count, string response);
}
