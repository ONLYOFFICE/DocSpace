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
internal static partial class DocuSignHelperLogger
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "DocuSing userInfo: {userInfo}")]
    public static partial void DebugDocuSingUserInfo(this ILogger<DocuSignHelper> logger, string userInfo);

    [LoggerMessage(Level = LogLevel.Debug, Message = "DocuSign hook url: {url}")]
    public static partial void DebugDocuSingHookUrl(this ILogger<DocuSignHelper> logger, string url);

    [LoggerMessage(Level = LogLevel.Debug, Message = "DocuSign createdEnvelope: {envelopeId}")]
    public static partial void DebugDocuSingCreatedEnvelope(this ILogger<DocuSignHelper> logger, string envelopeId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "DocuSign senderView: {url}")]
    public static partial void DebugDocuSingSenderView(this ILogger<DocuSignHelper> logger, string url);

    [LoggerMessage(Level = LogLevel.Error, Message = "DocuSign refresh token for user {id}")]
    public static partial void ErrorDocuSignRefreshToken(this ILogger<DocuSignHelper> logger, Guid id, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Signer is undefined")]
    public static partial void ErrorSignerIsUndefined(this ILogger<DocuSignHelper> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Information, Message = "DocuSign refresh token for user {userId}")]
    public static partial void InformationDocuSignRefreshToken(this ILogger<DocuSignHelper> logger, Guid userId);

    [LoggerMessage(Level = LogLevel.Information, Message = "DocuSign webhook get stream: {documentId}")]
    public static partial void InformationDocuSignWebhookGetStream(this ILogger<DocuSignHelper> logger, string documentId);
}
