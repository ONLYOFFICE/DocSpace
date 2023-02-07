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

namespace ASC.Core.Common.Log;
internal static partial class SecurityContextLogger
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "cookie {cookie}, tenant {tenant}, userid {userid}")]
    public static partial void AuthenticateDebug(this ILogger<SecurityContext> logger, string cookie, int tenant, Guid userId, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Authenticate error: cookie {cookie}, tenant {tenant}, userid {userid}")]
    public static partial void AuthenticateError(this ILogger<SecurityContext> logger, string cookie, int tenant, Guid userId, Exception exception);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Couldn't test api connection")]
    public static partial void DebugCouldNotTest(this ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Information, Message = "Empty Bearer cookie: {ipFrom} {address}")]
    public static partial void InformationEmptyBearer(this ILogger<SecurityContext> logger, string ipFrom, string address);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Can not decrypt cookie: {cookie} {ipFrom} {address}")]
    public static partial void WarningCanNotDecrypt(this ILogger<SecurityContext> logger, string cookie, string ipFrom, string address);
}
