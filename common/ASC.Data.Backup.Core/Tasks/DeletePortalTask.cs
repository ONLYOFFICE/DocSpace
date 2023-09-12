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

namespace ASC.Data.Backup.Tasks;

public class DeletePortalTask : PortalTaskBase
{
    private readonly ILogger<DeletePortalTask> _logger;

    public DeletePortalTask(
        DbFactory dbFactory,
        ILogger<DeletePortalTask> logger,
        int tenantId,
        StorageFactory storageFactory,
        StorageFactoryConfig storageFactoryConfig,
        ModuleProvider moduleProvider)
        : base(dbFactory, logger, storageFactory, storageFactoryConfig, moduleProvider)
    {
        Init(tenantId);
        _logger = logger;
    }

    public override async Task RunJob()
    {
        _logger.DebugBeginDelete(TenantId);
        var modulesToProcess = GetModulesToProcess().Reverse().ToList();
        SetStepsCount(ProcessStorage ? modulesToProcess.Count + 1 : modulesToProcess.Count);

        foreach (var module in modulesToProcess)
        {
            DoDeleteModule(module);
        }

        if (ProcessStorage)
        {
            await DoDeleteStorageAsync();
        }

        _logger.DebugEndDelete(TenantId);
    }

    private void DoDeleteModule(IModuleSpecifics module)
    {
        _logger.DebugBeginDeleteDataForModule(module.ModuleName);
        var tablesCount = module.Tables.Count();
        var tablesProcessed = 0;
        using (var connection = DbFactory.OpenConnection())
        {
            foreach (var table in module.GetTablesOrdered().Reverse().Where(t => !_ignoredTables.Contains(t.Name)))
            {
                ActionInvoker.Try(state =>
                    {
                        var t = (TableInfo)state;
                        module.CreateDeleteCommand(connection.Fix(), TenantId, t).WithTimeout(120).ExecuteNonQuery();
                    }, table, 5, onFailure: error => { throw ThrowHelper.CantDeleteTable(table.Name, error); });
                SetCurrentStepProgress((int)(++tablesProcessed * 100 / (double)tablesCount));
            }
        }

        _logger.DebugEndDeleteDataForModule(module.ModuleName);
    }

    private async Task DoDeleteStorageAsync()
    {
        _logger.DebugBeginDeleteStorage();
        var storageModules = StorageFactoryConfig.GetModuleList().Where(IsStorageModuleAllowed).ToList();
        var modulesProcessed = 0;
        foreach (var module in storageModules)
        {
            var storage = await StorageFactory.GetStorageAsync(TenantId, module);
            var domains = StorageFactoryConfig.GetDomainList(module);
            foreach (var domain in domains)
            {
                await ActionInvoker.TryAsync(async state => await storage.DeleteFilesAsync((string)state, "\\", "*.*", true), domain, 5,
                              onFailure: error => _logger.WarningCanNotDeleteFilesForDomain(domain, error));
            }
            await storage.DeleteFilesAsync("\\", "*.*", true);
            SetCurrentStepProgress((int)(++modulesProcessed * 100 / (double)storageModules.Count));
        }

        _logger.DebugEndDeleteStorage();
    }
}
