namespace ASC.Data.Backup.Tasks.Modules;

[Scope]
public class ModuleProvider
{
    public List<IModuleSpecifics> AllModules { get; }

    public ModuleProvider(IOptionsMonitor<ILog> options, Helpers helpers, CoreSettings coreSettings)
    {
        AllModules = new List<IModuleSpecifics>
            {
                new TenantsModuleSpecifics(coreSettings,helpers),
                new AuditModuleSpecifics(helpers),
                new CommunityModuleSpecifics(helpers),
                new CalendarModuleSpecifics(helpers),
                new ProjectsModuleSpecifics(helpers),
                new CrmModuleSpecifics(helpers),
                new FilesModuleSpecifics(options,helpers),
                new MailModuleSpecifics(options,helpers),
                new CrmModuleSpecifics2(helpers),
                new FilesModuleSpecifics2(helpers),
                new CrmInvoiceModuleSpecifics(helpers),
                new WebStudioModuleSpecifics(helpers),
                new CoreModuleSpecifics(helpers)
            }
        .ToList();
    }
    public IModuleSpecifics GetByStorageModule(string storageModuleName, string storageDomainName = null)
    {
        return storageModuleName switch
        {
            "files" => AllModules.FirstOrDefault(m => m.ModuleName == ModuleName.Files),
            "projects" => AllModules.FirstOrDefault(m => m.ModuleName == ModuleName.Projects),
            "crm" => AllModules.FirstOrDefault(m => m.ModuleName == (storageDomainName == "mail_messages" ? ModuleName.Crm2 : ModuleName.Crm)),
            "forum" => AllModules.FirstOrDefault(m => m.ModuleName == ModuleName.Community),
            "mailaggregator" => AllModules.FirstOrDefault(m => m.ModuleName == ModuleName.Mail),
            _ => null,
        };
    }
}
