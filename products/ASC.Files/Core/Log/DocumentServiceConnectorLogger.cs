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
internal static partial class DocumentServiceConnectorLogger
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "DocService convert from {fromExtension} to {toExtension} - {documentUri}, DocServiceConverterUrl:{docServiceConverterUrl}")]
    public static partial void DebugDocServiceConvert(this ILogger<DocumentServiceConnector> logger, string fromExtension, string toExtension, string documentUri, string docServiceConverterUrl);

    [LoggerMessage(Level = LogLevel.Debug, Message = "DocService command {method} fileId '{fileId}' docKey '{docKey}' callbackUrl '{callbackUrl}' users '{users}' meta '{meta}'")]
    public static partial void DebugDocServiceCommand(this ILogger<DocumentServiceConnector> logger, CommandMethod method, string fileId, string docKey, string callbackUrl, string users, string meta);

    [LoggerMessage(Level = LogLevel.Debug, Message = "DocService builder requestKey {requestKey} async {isAsync}")]
    public static partial void DebugDocServiceBuilderRequestKey(this ILogger<DocumentServiceConnector> logger, string requestKey, bool isAsync);

    [LoggerMessage(Level = LogLevel.Debug, Message = "DocService request version")]
    public static partial void DebugDocServiceRequestVersion(this ILogger<DocumentServiceConnector> logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "DocService command error")]
    public static partial void ErrorDocServiceCommandError(this ILogger<DocumentServiceConnector> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Healthcheck DocService check error")]
    public static partial void ErrorDocServiceHealthcheck(this ILogger<DocumentServiceConnector> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Converter DocService check error")]
    public static partial void ErrorConverterDocServiceCheckError(this ILogger<DocumentServiceConnector> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Document DocService check error")]
    public static partial void ErrorDocumentDocServiceCheckError(this ILogger<DocumentServiceConnector> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Command DocService check error")]
    public static partial void ErrorCommandDocServiceCheckError(this ILogger<DocumentServiceConnector> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "DocService check error")]
    public static partial void ErrorDocServiceCheck(this ILogger<DocumentServiceConnector> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "DocService error")]
    public static partial void ErrorDocServiceError(this ILogger<DocumentServiceConnector> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "DocService command response: '{error}' {errorString}")]
    public static partial void ErrorDocServiceCommandResponse(this ILogger<DocumentServiceConnector> logger, ErrorTypes error, string errorString);
}
