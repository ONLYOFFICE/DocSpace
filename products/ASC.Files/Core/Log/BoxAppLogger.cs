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
internal static partial class BoxAppLogger
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "BoxApp: get file {fileId}")]
    public static partial void DebugBoxAppGetFile(this ILogger<BoxApp> logger, string fileId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "BoxApp: locked by {userId}")]
    public static partial void DebugBoxAppLockedBy(this ILogger<BoxApp> logger, string userId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "BoxApp: get file stream url {url}")]
    public static partial void DebugBoxAppGetFileStreamUrl(this ILogger<BoxApp> logger, string url);

    [LoggerMessage(Level = LogLevel.Debug, Message = "BoxApp: save file stream {fileId} from {from}")]
    public static partial void DebugBoxAppSaveFileStream(this ILogger<BoxApp> logger, string fileId, string from);

    [LoggerMessage(Level = LogLevel.Debug, Message = "BoxApp: GetConvertedUri from {fileType} to {currentType} - {downloadUrl}")]
    public static partial void DebugBoxAppGetConvertedUri(this ILogger<BoxApp> logger, string fileType, string currentType, string downloadUrl);

    [LoggerMessage(Level = LogLevel.Debug, Message = "BoxApp: save file totalSize - {streamLength}")]
    public static partial void DebugBoxAppSaveFileTotalSize(this ILogger<BoxApp> logger, long streamLength);

    [LoggerMessage(Level = LogLevel.Debug, Message = "BoxApp: save file response - {response}")]
    public static partial void DebugBoxAppSaveFileResponse(this ILogger<BoxApp> logger, string response);

    [LoggerMessage(Level = LogLevel.Debug, Message = "BoxApp: logout for {boxUserId}")]
    public static partial void DebugBoxAppLogout(this ILogger<BoxApp> logger, string boxUserId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "BoxApp: get file stream {fileId}")]
    public static partial void DebugBoxAppGetFileStream(this ILogger<BoxApp> logger, string fileId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "BoxApp: AddLinker {boxUserId}")]
    public static partial void DebugBoxAppAddLinker(this ILogger<BoxApp> logger, string boxUserId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "BoxApp: userinfo response - {response}")]
    public static partial void DebugBoxAppUserInfoResponse(this ILogger<BoxApp> logger, string response);

    [LoggerMessage(Level = LogLevel.Debug, Message = "From box app new personal user '{email}' without culture {culture}")]
    public static partial void DebugBoxAppFromBoxAppNewPersonalUser(this ILogger<BoxApp> logger, string email, string culture);

    [LoggerMessage(Level = LogLevel.Debug, Message = "BoxApp: new user {userId}")]
    public static partial void DebugBoxAppNewUser(this ILogger<BoxApp> logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "BoxApp: file response - {response}")]
    public static partial void DebugBoxAppFileResponse(this ILogger<BoxApp> logger, string response);

    [LoggerMessage(Level = LogLevel.Debug, Message = "BoxApp: GetAccessToken by code {code}")]
    public static partial void DebugBoxAppGetAccessTokenByCode(this ILogger<BoxApp> logger, string code);

    [LoggerMessage(Level = LogLevel.Error, Message = "BoxApp: Error convert")]
    public static partial void ErrorBoxAppConvert(this ILogger<BoxApp> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "BoxApp: Error save file")]
    public static partial void ErrorBoxAppSaveFile(this ILogger<BoxApp> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "BoxApp: validate error {authKey} {validateResult}: {url}")]
    public static partial void ErrorBoxAppValidateError(this ILogger<BoxApp> logger, string authKey, EmailValidationKeyProvider.ValidationResult validateResult, Uri url, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "BoxApp: Error request {url}")]
    public static partial void ErrorBoxAppErrorRequest(this ILogger<BoxApp> logger, Uri url, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "BoxApp StreamFile")]
    public static partial void ErrorBoxAppStreamFile(this ILogger<BoxApp> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "BoxApp: userinfo request")]
    public static partial void ErrorBoxAppUserinfoRequest(this ILogger<BoxApp> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "BoxApp: file request")]
    public static partial void ErrorBoxAppFileRequest(this ILogger<BoxApp> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "GetToken")]
    public static partial void ErrorGetToken(this ILogger<BoxApp> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "BoxApp: file is null")]
    public static partial void ErrorBoxAppFileIsNull(this ILogger<BoxApp> logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "BoxApp: token is null")]
    public static partial void ErrorBoxAppTokenIsNull(this ILogger<BoxApp> logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "BoxApp: UserInfo is null")]
    public static partial void ErrorBoxAppUserInfoIsNull(this ILogger<BoxApp> logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error in userinfo request")]
    public static partial void ErrorInUserInfoRequest(this ILogger<BoxApp> logger);
}
