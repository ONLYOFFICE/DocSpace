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

namespace ASC.Data.Backup.Core.Log;
internal static partial class PortalTaskBaseLogger
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Steps: {stepsCount}")]
    public static partial void DebugCountSteps(this ILogger logger, int stepsCount);

    [LoggerMessage(Level = LogLevel.Debug, Message = "run mysql file {file} {args}")]
    public static partial void DebugRunMySQlFile(this ILogger logger, string file, string args);

    [LoggerMessage(Level = LogLevel.Debug, Message = "complete mysql file {file}")]
    public static partial void DebugCompleteMySQlFile(this ILogger logger, string file);

    [LoggerMessage(Level = LogLevel.Error, Message = "Restore")]
    public static partial void ErrorRestore(this ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Debug, Message = "begin delete storage")]
    public static partial void DebugBeginDeleteStorage(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "end delete storage")]
    public static partial void DebugEndDeleteStorage(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Can't delete files for domain {domain}")]
    public static partial void WarningCanNotDeleteFilesForDomain(this ILogger logger, string domain, Exception exception);
}
