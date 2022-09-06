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
internal static partial class BackupPortalTaskLogger
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "begin backup {tenantId}")]
    public static partial void DebugBeginBackup(this ILogger<BackupPortalTask> logger, int tenantId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "end backup {tenantId}")]
    public static partial void DebugEndBackup(this ILogger<BackupPortalTask> logger, int tenantId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "files: {count}")]
    public static partial void DebugFilesCount(this ILogger<BackupPortalTask> logger, int count);

    [LoggerMessage(Level = LogLevel.Debug, Message = "dir remove start {subDir}")]
    public static partial void DebugDirRemoveStart(this ILogger<BackupPortalTask> logger, string subDir);

    [LoggerMessage(Level = LogLevel.Debug, Message = "dir remove end {subDir}")]
    public static partial void DebugDirRemoveEnd(this ILogger<BackupPortalTask> logger, string subDir);

    [LoggerMessage(Level = LogLevel.Debug, Message = "dump table scheme start {table}")]
    public static partial void DebugDumpTableSchemeStart(this ILogger<BackupPortalTask> logger, string table);

    [LoggerMessage(Level = LogLevel.Debug, Message = "dump table scheme stop {table}")]
    public static partial void DebugDumpTableSchemeStop(this ILogger<BackupPortalTask> logger, string table);

    [LoggerMessage(Level = LogLevel.Debug, Message = "dump table data stop {table}")]
    public static partial void DebugDumpTableDataStop(this ILogger<BackupPortalTask> logger, string table);

    [LoggerMessage(Level = LogLevel.Debug, Message = "dump table data start {table}")]
    public static partial void DebugDumpTableDataStart(this ILogger<BackupPortalTask> logger, string table);

    [LoggerMessage(Level = LogLevel.Debug, Message = "save to file {table}")]
    public static partial void DebugSaveTable(this ILogger<BackupPortalTask> logger, string table);

    [LoggerMessage(Level = LogLevel.Debug, Message = "begin backup storage")]
    public static partial void DebugBeginBackupStorage(this ILogger<BackupPortalTask> logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "end backup storage")]
    public static partial void DebugEndBackupStorage(this ILogger<BackupPortalTask> logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "backup file {path}")]
    public static partial void DebugBackupFile(this ILogger<BackupPortalTask> logger, string path);

    [LoggerMessage(Level = LogLevel.Debug, Message = "archive dir start {subDir}")]
    public static partial void DebugArchiveDirStart(this ILogger<BackupPortalTask> logger, string subDir);

    [LoggerMessage(Level = LogLevel.Debug, Message = "archive dir end {subDir}")]
    public static partial void DebugArchiveDirEnd(this ILogger<BackupPortalTask> logger, string subDir);

    [LoggerMessage(Level = LogLevel.Debug, Message = "begin saving data for module {name}")]
    public static partial void DebugBeginSavingDataForModule(this ILogger<BackupPortalTask> logger, ModuleName name);

    [LoggerMessage(Level = LogLevel.Debug, Message = "begin load table {table}")]
    public static partial void DebugBeginLoadTable(this ILogger<BackupPortalTask> logger, string table);

    [LoggerMessage(Level = LogLevel.Debug, Message = "end load table {table}")]
    public static partial void DebugEndLoadTable(this ILogger<BackupPortalTask> logger, string table);

    [LoggerMessage(Level = LogLevel.Debug, Message = "begin saving table {table}")]
    public static partial void DebugBeginSavingTable(this ILogger<BackupPortalTask> logger, string table);

    [LoggerMessage(Level = LogLevel.Debug, Message = "end saving table {table}")]
    public static partial void DebugEndSavingTable(this ILogger<BackupPortalTask> logger, string table);

    [LoggerMessage(Level = LogLevel.Debug, Message = "end saving data for module {table}")]
    public static partial void DebugEndSavingDataForModule(this ILogger<BackupPortalTask> logger, ModuleName table);

    [LoggerMessage(Level = LogLevel.Error, Message = "DumpTableScheme")]
    public static partial void ErrorDumpTableScheme(this ILogger<BackupPortalTask> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "SelectCount")]
    public static partial void ErrorSelectCount(this ILogger<BackupPortalTask> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "DumpTableData")]
    public static partial void ErrorDumpTableData(this ILogger<BackupPortalTask> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Warning, Message = "backup attempt failure")]
    public static partial void WarningBackupAttemptFailure(this ILogger<BackupPortalTask> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Warning, Message = "can't backup file ({module}:{path})")]
    public static partial void WarningCanNotBackupFile(this ILogger<BackupPortalTask> logger, string module, string path, Exception exception);
}
