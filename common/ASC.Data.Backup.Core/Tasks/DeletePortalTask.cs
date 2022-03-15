namespace ASC.Data.Backup.Tasks;

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
