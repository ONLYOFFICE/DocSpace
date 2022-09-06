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

namespace ASC.Data.Backup.BackgroundTasks.Log;

internal static partial class BackupCleanerServiceLogger
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "BackupCleanerService is starting.")]
    public static partial void DebugBackupCleanerServiceStarting(this ILogger<BackupCleanerService> logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "BackupCleanerService background task is stopping.")]
    public static partial void DebugBackupCleanerServiceStopping(this ILogger<BackupCleanerService> logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "BackupCleanerService background task is doing background work.")]
    public static partial void DebugBackupCleanerServiceDoingWork(this ILogger<BackupCleanerService> logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "BackupCleanerService background task with instance id {instanceId} is't active.")]
    public static partial void DebugBackupCleanerServiceIsNotActive(this ILogger<BackupCleanerService> logger, string instanceId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "started to clean expired backups")]
    public static partial void DebugStartedClean(this ILogger<BackupCleanerService> logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "found {count} backups which are expired")]
    public static partial void DebugFoundBackups(this ILogger<BackupCleanerService> logger, int count);

    [LoggerMessage(Level = LogLevel.Debug, Message = "only last {storedCount} scheduled backup records are to keep for tenant {tenantId} so {removeCount} records must be removed")]
    public static partial void DebugOnlyLast(this ILogger<BackupCleanerService> logger, int storedCount, int tenantId, int removeCount);

    [LoggerMessage(Level = LogLevel.Warning, Message = "can't remove backup record {id}")]
    public static partial void WarningCanNotRemoveBackup(this ILogger<BackupCleanerService> logger, Guid id, Exception exception);
}
