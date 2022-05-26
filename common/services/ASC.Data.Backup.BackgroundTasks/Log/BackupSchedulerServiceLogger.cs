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

internal static partial class BackupSchedulerServiceLogger
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "BackupSchedulerService is starting.")]
    public static partial void DebugBackupSchedulerServiceStarting(this ILogger<BackupSchedulerService> logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "BackupSchedulerService background task is stopping.")]
    public static partial void DebugBackupSchedulerServiceStopping(this ILogger<BackupSchedulerService> logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "BackupSchedulerService background task with instance id {instanceId} is't active.")]
    public static partial void DebugBackupSchedulerServiceIsNotActive(this ILogger<BackupSchedulerService> logger, string instanceId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "BackupSchedulerService background task is doing background work.")]
    public static partial void DebugBackupSchedulerServiceDoingWork(this ILogger<BackupSchedulerService> logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "started to schedule backups")]
    public static partial void DebugStartedToSchedule(this ILogger<BackupSchedulerService> logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "{count} backups are to schedule")]
    public static partial void DebugBackupsSchedule(this ILogger<BackupSchedulerService> logger, int count);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Start scheduled backup: {tenantId}, {backupMail}, {storageType}, {storageBasePath}")]
    public static partial void DebugStartScheduledBackup(this ILogger<BackupSchedulerService> logger, int tenantId, bool backupMail, BackupStorageType storageType, string storageBasePath);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Skip portal {tenantId} not paid")]
    public static partial void DebugNotPaid(this ILogger<BackupSchedulerService> logger, int tenantId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Skip portal {tenantId} haven't access")]
    public static partial void DebugHaveNotAccess(this ILogger<BackupSchedulerService> logger, int tenantId);

    [LoggerMessage(Level = LogLevel.Error, Message = "error while scheduling backups")]
    public static partial void ErrorBackups(this ILogger<BackupSchedulerService> logger, Exception exception);
}
