namespace ASC.Files.Thirdparty.Box;

[Scope]
internal class BoxTagDao : BoxDaoBase, ITagDao<string>
{
    public BoxTagDao(
        IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        DbContextManager<FilesDbContext> dbContextManager,
        SetupInfo setupInfo,
        IOptionsMonitor<ILog> monitor,
        FileUtility fileUtility,
        TempPath tempPath) : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath)
    {
    }
}
