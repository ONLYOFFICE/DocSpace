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

namespace ASC.Data.Storage.Log;
internal static partial class EncryptionOperationLogger
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Storage already {status}")]
    public static partial void DebugStorageAlready(this ILogger logger, EncryprtionStatus status); 
    
    [LoggerMessage(Level = LogLevel.Debug, Message = "Percentage: {percentage}")]
    public static partial void DebugPercentage(this ILogger logger, double percentage);    
    
    [LoggerMessage(Level = LogLevel.Debug, Message = "Save new EncryptionSettings")]
    public static partial void DebugSaveNewEncryptionSettings(this ILogger logger);   
    
    [LoggerMessage(Level = LogLevel.Debug, Message = "Tenant {tenantAlias} SetStatus Active")]
    public static partial void DebugTenantSetStatusActive(this ILogger logger, string tenantAlias);  
    
    [LoggerMessage(Level = LogLevel.Debug, Message = "Tenant {tenantAlias} SendStorageEncryptionSuccess")]
    public static partial void DebugTenantSendStorageEncryptionSuccess(this ILogger logger, string tenantAlias);
    
    [LoggerMessage(Level = LogLevel.Debug, Message = "Tenant {tenantAlias} SendStorageEncryptionError")]
    public static partial void DebugTenantSendStorageEncryptionError(this ILogger logger, string tenantAlias);

    [LoggerMessage(Level = LogLevel.Error, Message = "EncryptionOperation")]
    public static partial void ErrorEncryptionOperation(this ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "{logItem}")]
    public static partial void ErrorLogItem(this ILogger logger, string logItem, Exception exception);
}
