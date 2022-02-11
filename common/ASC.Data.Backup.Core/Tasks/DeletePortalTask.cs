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


using System.Linq;

using ASC.Common.Logging;
using ASC.Data.Backup.Exceptions;
using ASC.Data.Backup.Extensions;
using ASC.Data.Backup.Tasks.Data;
using ASC.Data.Backup.Tasks.Modules;
using ASC.Data.Storage;

using Microsoft.Extensions.Options;

namespace ASC.Data.Backup.Tasks
{
    public class DeletePortalTask : PortalTaskBase
    {
        public DeletePortalTask(DbFactory dbFactory, IOptionsMonitor<ILog> options, int tenantId, string configPath, StorageFactory storageFactory, StorageFactoryConfig storageFactoryConfig, ModuleProvider moduleProvider)
            : base(dbFactory, options, storageFactory, storageFactoryConfig, moduleProvider)
        {
            Init(tenantId, configPath);
        }

        public override void RunJob()
        {
            Logger.DebugFormat("begin delete {0}", TenantId);
            var modulesToProcess = GetModulesToProcess().Reverse().ToList();
            SetStepsCount(ProcessStorage ? modulesToProcess.Count + 1 : modulesToProcess.Count);
            foreach (var module in modulesToProcess)
            {
                DoDeleteModule(module);
            }
            if (ProcessStorage)
            {
                DoDeleteStorage();
            }
            Logger.DebugFormat("end delete {0}", TenantId);
        }

        private void DoDeleteModule(IModuleSpecifics module)
        {
            Logger.DebugFormat("begin delete data for module ({0})", module.ModuleName);
            var tablesCount = module.Tables.Count();
            var tablesProcessed = 0;
            using (var connection = DbFactory.OpenConnection())
            {
                foreach (var table in module.GetTablesOrdered().Reverse().Where(t => !IgnoredTables.Contains(t.Name)))
                {
                    ActionInvoker.Try(state =>
                        {
                            var t = (TableInfo)state;
                            module.CreateDeleteCommand(connection.Fix(), TenantId, t).WithTimeout(120).ExecuteNonQuery();
                        }, table, 5, onFailure: error => { throw ThrowHelper.CantDeleteTable(table.Name, error); });
                    SetCurrentStepProgress((int)(++tablesProcessed * 100 / (double)tablesCount));
                }
            }
            Logger.DebugFormat("end delete data for module ({0})", module.ModuleName);
        }

        private void DoDeleteStorage()
        {
            Logger.Debug("begin delete storage");
            var storageModules = StorageFactoryConfig.GetModuleList(ConfigPath).Where(IsStorageModuleAllowed).ToList();
            var modulesProcessed = 0;
            foreach (var module in storageModules)
            {
                var storage = StorageFactory.GetStorage(ConfigPath, TenantId.ToString(), module);
                var domains = StorageFactoryConfig.GetDomainList(ConfigPath, module);
                foreach (var domain in domains)
                {
                    ActionInvoker.Try(state => storage.DeleteFilesAsync((string)state, "\\", "*.*", true).Wait(), domain, 5,
                                      onFailure: error => Logger.WarnFormat("Can't delete files for domain {0}: \r\n{1}", domain, error));
                }
                storage.DeleteFilesAsync("\\", "*.*", true).Wait();
                SetCurrentStepProgress((int)(++modulesProcessed * 100 / (double)storageModules.Count));
            }
            Logger.Debug("end delete storage");
        }
    }
}
