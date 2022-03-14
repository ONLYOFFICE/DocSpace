/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Data.Common;
using System.IO;
using System.Linq;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Core.Tenants;
using ASC.Data.Backup.Extensions;
using ASC.Data.Backup.Tasks.Modules;
using ASC.Data.Storage;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Data.Backup.Tasks
{
    [Scope]
    public class TransferPortalTask : PortalTaskBase
    {
        public const string DefaultDirectoryName = "backup";

        public string ToConfigPath { get; private set; }

        public string BackupDirectory { get; set; }
        public bool DeleteBackupFileAfterCompletion { get; set; }
        public bool BlockOldPortalAfterStart { get; set; }
        public bool DeleteOldPortalAfterCompletion { get; set; }
        private IOptionsMonitor<ILog> Options { get; set; }
        private TempStream TempStream { get; }
        private TempPath TempPath { get; }
        private IServiceProvider ServiceProvider { get; set; }
        public int ToTenantId { get; private set; }
        public int Limit { get; private set; }

        public TransferPortalTask(
            DbFactory dbFactory, 
            IServiceProvider serviceProvider,
            IOptionsMonitor<ILog> options, 
            StorageFactory storageFactory, 
            StorageFactoryConfig storageFactoryConfig, 
            ModuleProvider moduleProvider,
            TempStream tempStream,
            TempPath tempPath)
            : base(dbFactory, options, storageFactory, storageFactoryConfig, moduleProvider)
        {
            DeleteBackupFileAfterCompletion = true;
            BlockOldPortalAfterStart = true;
            DeleteOldPortalAfterCompletion = true;
            Options = options;
            TempStream = tempStream;
            TempPath = tempPath;
            ServiceProvider = serviceProvider;
        }

        public void Init(int tenantId, string fromConfigPath, string toConfigPath, int limit, string backupDirectory)
        {
            Limit = limit;
            ToConfigPath = toConfigPath ?? throw new ArgumentNullException(nameof(toConfigPath));
            Init(tenantId, fromConfigPath);

            BackupDirectory = backupDirectory;
        }

        public override void RunJob()
        {
            Logger.DebugFormat("begin transfer {0}", TenantId);
            var fromDbFactory = new DbFactory(null, null);
            var toDbFactory = new DbFactory(null, null);
            var tenantAlias = GetTenantAlias(fromDbFactory);
            var backupFilePath = GetBackupFilePath(tenantAlias);
            var columnMapper = new ColumnMapper();
            try
            {
                //target db can have error tenant from the previous attempts
                SaveTenant(toDbFactory, tenantAlias, TenantStatus.RemovePending, tenantAlias + "_error", "status = " + TenantStatus.Restoring.ToString("d"));

                if (BlockOldPortalAfterStart)
                {
                    SaveTenant(fromDbFactory, tenantAlias, TenantStatus.Transfering);
                }

                SetStepsCount(ProcessStorage ? 3 : 2);

                //save db data to temporary file
                var backupTask = ServiceProvider.GetService<BackupPortalTask>();
                backupTask.Init(TenantId, ConfigPath, backupFilePath, Limit);
                backupTask.ProcessStorage = false;
                backupTask.ProgressChanged += (sender, args) => SetCurrentStepProgress(args.Progress);
                foreach (var moduleName in IgnoredModules)
                {
                    backupTask.IgnoreModule(moduleName);
                }
                backupTask.RunJob();

                //restore db data from temporary file
                var restoreTask = ServiceProvider.GetService<RestorePortalTask>();
                restoreTask.Init(ToConfigPath, backupFilePath, columnMapper: columnMapper);
                restoreTask.ProcessStorage = false;
                restoreTask.ProgressChanged += (sender, args) => SetCurrentStepProgress(args.Progress);
                foreach (var moduleName in IgnoredModules)
                {
                    restoreTask.IgnoreModule(moduleName);
                }
                restoreTask.RunJob();

                //transfer files
                if (ProcessStorage)
                {
                    DoTransferStorage(columnMapper);
                }

                SaveTenant(toDbFactory, tenantAlias, TenantStatus.Active);
                if (DeleteOldPortalAfterCompletion)
                {
                    SaveTenant(fromDbFactory, tenantAlias, TenantStatus.RemovePending, tenantAlias + "_deleted");
                }
                else if (BlockOldPortalAfterStart)
                {
                    SaveTenant(fromDbFactory, tenantAlias, TenantStatus.Active);
                }

                ToTenantId = columnMapper.GetTenantMapping();
            }
            catch
            {
                SaveTenant(fromDbFactory, tenantAlias, TenantStatus.Active);
                if (columnMapper.GetTenantMapping() > 0)
                {
                    SaveTenant(toDbFactory, tenantAlias, TenantStatus.RemovePending, tenantAlias + "_error");
                }
                throw;
            }
            finally
            {
                if (DeleteBackupFileAfterCompletion)
                {
                    File.Delete(backupFilePath);
                }
                Logger.DebugFormat("end transfer {0}", TenantId);
            }
        }

        private void DoTransferStorage(ColumnMapper columnMapper)
        {
            Logger.Debug("begin transfer storage");
            var fileGroups = GetFilesToProcess(TenantId).GroupBy(file => file.Module).ToList();
            var groupsProcessed = 0;
            foreach (var group in fileGroups)
            {
                var baseStorage = StorageFactory.GetStorage(ConfigPath, TenantId.ToString(), group.Key);
                var destStorage = StorageFactory.GetStorage(ToConfigPath, columnMapper.GetTenantMapping().ToString(), group.Key);
                var utility = new CrossModuleTransferUtility(Options, TempStream, TempPath, baseStorage, destStorage);

                foreach (var file in group)
                {
                    var adjustedPath = file.Path;

                    var module = ModuleProvider.GetByStorageModule(file.Module, file.Domain);
                    if (module == null || module.TryAdjustFilePath(false, columnMapper, ref adjustedPath))
                    {
                        try
                        {
                            utility.CopyFileAsync(file.Domain, file.Path, file.Domain, adjustedPath).Wait();
                        }
                        catch (Exception error)
                        {
                            Logger.WarnFormat("Can't copy file ({0}:{1}): {2}", file.Module, file.Path, error);
                        }
                    }
                    else
                    {
                        Logger.WarnFormat("Can't adjust file path \"{0}\".", file.Path);
                    }
                }
                SetCurrentStepProgress((int)(++groupsProcessed * 100 / (double)fileGroups.Count));
            }

            if (fileGroups.Count == 0)
                SetStepCompleted();

            Logger.Debug("end transfer storage");
        }

        private void SaveTenant(DbFactory dbFactory, string alias, TenantStatus status, string newAlias = null, string whereCondition = null)
        {
            using var connection = dbFactory.OpenConnection();
            if (newAlias == null)
            {
                newAlias = alias;
            }
            else if (newAlias != alias)
            {
                newAlias = GetUniqAlias(connection, newAlias);
            }

            var commandText = "update tenants_tenants " +
                "set " +
                $"  status={status.ToString("d")}, " +
                $"  alias = '{newAlias}', " +
                $"  last_modified='{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}', " +
                $"  statuschanged='{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}' " +
                $"where alias = '{alias}'";

            if (!string.IsNullOrEmpty(whereCondition))
                commandText += " and " + whereCondition;
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            command.WithTimeout(120).ExecuteNonQuery();
        }

        private string GetTenantAlias(DbFactory dbFactory)
        {
            using var connection = dbFactory.OpenConnection();
            var command = connection.CreateCommand();
            command.CommandText = "select alias from tenants_tenants where id = " + TenantId;
            return (string)command.WithTimeout(120).ExecuteScalar();
        }

        private static string GetUniqAlias(DbConnection connection, string alias)
        {
            var command = connection.CreateCommand();
            command.CommandText = "select count(*) from tenants_tenants where alias like '" + alias + "%'";
            return alias + command.WithTimeout(120).ExecuteScalar();
        }

        private string GetBackupFilePath(string tenantAlias)
        {
            if (!Directory.Exists(BackupDirectory ?? DefaultDirectoryName))
                Directory.CreateDirectory(BackupDirectory ?? DefaultDirectoryName);

            return CrossPlatform.PathCombine(BackupDirectory ?? DefaultDirectoryName, tenantAlias + DateTime.UtcNow.ToString("(yyyy-MM-dd HH-mm-ss)") + ".backup");
        }

    }
}
