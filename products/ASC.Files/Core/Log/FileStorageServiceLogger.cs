﻿// (c) Copyright Ascensio System SIA 2010-2022
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
internal static partial class FileStorageServiceLogger
{
    [LoggerMessage(Level = LogLevel.Error, Message = "DocEditor")]
    public static partial void ErrorDocEditor(this ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "CreateThumbnails")]
    public static partial void ErrorCreateThumbnails(this ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "FileStorageService")]
    public static partial void ErrorFileStorageService(this ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Information, Message = "Reassign provider {providerId} from {fromUser} to {toUser}")]
    public static partial void InformationReassignProvider(this ILogger logger, int providerId, Guid fromUser, Guid toUser);

    [LoggerMessage(Level = LogLevel.Information, Message = "Delete provider {providerId} for {userId}")]
    public static partial void InformationDeleteProvider(this ILogger logger, int providerId, Guid userId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Reassign folders from {fromUser} to {toUser}")]
    public static partial void InformationReassignFolders(this ILogger logger, Guid fromUser, Guid toUser);

    [LoggerMessage(Level = LogLevel.Information, Message = "Reassign files from {fromUser} to {toUser}")]
    public static partial void InformationReassignFiles(this ILogger logger, Guid fromUser, Guid toUser);

    [LoggerMessage(Level = LogLevel.Information, Message = "Delete personal data for {userId}")]
    public static partial void InformationDeletePersonalData(this ILogger logger, Guid userId);
}
