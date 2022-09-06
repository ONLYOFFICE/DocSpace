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
internal static partial class GoogleDriveAppLogger
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: get file {fileId}")]
    public static partial void DebugGoogleDriveAppGetFile(this ILogger<GoogleDriveApp> logger, string fileId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: get file stream url {fileId}")]
    public static partial void DebugGoogleDriveAppGetFileStreamUrl(this ILogger<GoogleDriveApp> logger, string fileId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: save file stream {fileId} from {from}")]
    public static partial void DebugGoogleDriveAppSaveFileStream(this ILogger<GoogleDriveApp> logger, string fileId, string from);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: GetConvertedUri from {fileType} to {currentType} - {downloadUrl}")]
    public static partial void DebugGoogleDriveAppGetConvertedUri(this ILogger<GoogleDriveApp> logger, string fileType, string currentType, string downloadUrl);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: save file stream response - {result}")]
    public static partial void DebugGoogleDriveAppSaveFileStream2(this ILogger<GoogleDriveApp> logger, string result);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: state - {state}")]
    public static partial void DebugGoogleDriveAppState(this ILogger<GoogleDriveApp> logger, string state);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: logout for {googleUserId}")]
    public static partial void DebugGoogleDriveAppLogout(this ILogger<GoogleDriveApp> logger, string googleUserId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: file must be converted")]
    public static partial void DebugGoogleDriveAppFileMustBeConverted(this ILogger<GoogleDriveApp> logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: get file stream {fileId}")]
    public static partial void DebugGoogleDriveAppGetFileStream(this ILogger<GoogleDriveApp> logger, string fileId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: get file stream downloadUrl - {downloadUrl}")]
    public static partial void DebugGoogleDriveAppGetFileStreamDownloadUrl(this ILogger<GoogleDriveApp> logger, string downloadUrl);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: ConfirmConvertFile - {fileId}")]
    public static partial void DebugGoogleDriveAppConfirmConvertFile(this ILogger<GoogleDriveApp> logger, string fileId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: CreateFile folderId - {folderId} fileName - {fileName}")]
    public static partial void DebugGoogleDriveAppCreateFile(this ILogger<GoogleDriveApp> logger, string folderId, string fileName);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: GetAccessToken by code {code}")]
    public static partial void DebugGoogleDriveAppGetAccessTokenByCode(this ILogger<GoogleDriveApp> logger, string code);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: AddLinker {googleUserId}")]
    public static partial void DebugGoogleDriveApAddLinker(this ILogger<GoogleDriveApp> logger, string googleUserId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "From google app new personal user '{email}' without culture {culture}")]
    public static partial void DebugFromGoogleAppNewPersonalUser(this ILogger<GoogleDriveApp> logger, string email, string culture);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: new user {userId}")]
    public static partial void DebugGoogleDriveAppNewUser(this ILogger<GoogleDriveApp> logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: file response - {response}")]
    public static partial void DebugGoogleDriveAppFileResponse(this ILogger<GoogleDriveApp> logger, string response);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: create from - {url}")]
    public static partial void DebugGoogleDriveAppCreateFrom(this ILogger<GoogleDriveApp> logger, string url);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: create file")]
    public static partial void DebugGoogleDriveAppCreateFile2(this ILogger<GoogleDriveApp> logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: create file totalSize - {size}")]
    public static partial void DebugGoogleDriveAppCreateFileTotalSize(this ILogger<GoogleDriveApp> logger, long size);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: create file response - {response}")]
    public static partial void DebugGoogleDriveAppCreateFileResponse(this ILogger<GoogleDriveApp> logger, string response);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: convert file")]
    public static partial void DebugGoogleDriveAppConvertFile(this ILogger<GoogleDriveApp> logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: GetConvertedUri- {downloadUrl}")]
    public static partial void DebugGoogleDriveAppGetConvertedUri2(this ILogger<GoogleDriveApp> logger, string downloadUrl);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: download exportLink - {downloadUrl}")]
    public static partial void DebugGoogleDriveAppDownloadExportLink(this ILogger<GoogleDriveApp> logger, string downloadUrl);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: Try GetCorrectExt - {ext} for - {mimeType}")]
    public static partial void DebugGoogleDriveAppTryGetCorrectExt(this ILogger<GoogleDriveApp> logger, string ext, string mimeType);

    [LoggerMessage(Level = LogLevel.Debug, Message = "GoogleDriveApp: get file stream contentLength - {contentLength}")]
    public static partial void DebugGoogleDriveAppGetFileStreamcontentLength(this ILogger<GoogleDriveApp> logger, string contentLength);

    [LoggerMessage(Level = LogLevel.Error, Message = "GoogleDriveApp: Error convert")]
    public static partial void ErrorGoogleDriveAppConvert(this ILogger<GoogleDriveApp> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "GoogleDriveApp: Error save file stream")]
    public static partial void ErrorGoogleDriveAppSaveFileStream(this ILogger<GoogleDriveApp> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "GoogleDriveApp: validate error {authKey} {validateResult}: {url}")]
    public static partial void ErrorGoogleDriveAppValidate(this ILogger<GoogleDriveApp> logger, string authKey, EmailValidationKeyProvider.ValidationResult validateResult, Uri url, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "GoogleDriveApp: Error request {url}")]
    public static partial void ErrorGoogleDriveAppRequest(this ILogger<GoogleDriveApp> logger, Uri url, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "GoogleDriveApp StreamFile")]
    public static partial void ErrorGoogleDriveAppStreamFile(this ILogger<GoogleDriveApp> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "GoogleDriveApp: userinfo request")]
    public static partial void ErrorGoogleDriveAppUserInfoRequest(this ILogger<GoogleDriveApp> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "GoogleDriveApp: file request")]
    public static partial void ErrorGoogleDriveAppFileRequest(this ILogger<GoogleDriveApp> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "GoogleDriveApp: Error create file")]
    public static partial void ErrorGoogleDriveAppCreateFile(this ILogger<GoogleDriveApp> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "GoogleDriveApp: Error GetConvertedUri")]
    public static partial void ErrorGoogleDriveAppGetConvertedUri(this ILogger<GoogleDriveApp> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "GoogleDriveApp: Error download exportLink")]
    public static partial void ErrorGoogleDriveAppDownLoadExportLink(this ILogger<GoogleDriveApp> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "GoogleDriveApp: file is null")]
    public static partial void ErrorGoogleDriveAppFileIsNull(this ILogger<GoogleDriveApp> logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "GoogleDriveApp: empty state")]
    public static partial void ErrorGoogleDriveAppEmptyIsNull(this ILogger<GoogleDriveApp> logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "GoogleDriveApp: token is null")]
    public static partial void ErrorGoogleDriveAppTokenIsNull(this ILogger<GoogleDriveApp> logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "GoogleDriveApp: UserInfo is null")]
    public static partial void ErrorGoogleDriveAppUserInfoIsNull(this ILogger<GoogleDriveApp> logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "GoogleDriveApp: ids is empty")]
    public static partial void ErrorGoogleDriveAppIdsIsNull(this ILogger<GoogleDriveApp> logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "GoogleDriveApp: Action not identified")]
    public static partial void ErrorGoogleDriveAppActionNotIdentified(this ILogger<GoogleDriveApp> logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "GoogleDriveApp: downloadUrl is null")]
    public static partial void ErrorGoogleDriveAppDownloadUrlIsNull(this ILogger<GoogleDriveApp> logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "GoogleDriveApp: Error convertUrl. size {size}")]
    public static partial void ErrorGoogleDriveAppConvertUrl(this ILogger<GoogleDriveApp> logger, string size);

    [LoggerMessage(Level = LogLevel.Information, Message = "GoogleDriveApp: create copy - {fileName}")]
    public static partial void InformationGoogleDriveAppCreateCopy(this ILogger<GoogleDriveApp> logger, string fileName);

    [LoggerMessage(Level = LogLevel.Error, Message = "GetToken")]
    public static partial void ErrorGetToken(this ILogger<GoogleDriveApp> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error in userinfo request")]
    public static partial void ErrorInUserInfoRequest(this ILogger<GoogleDriveApp> logger);
}
