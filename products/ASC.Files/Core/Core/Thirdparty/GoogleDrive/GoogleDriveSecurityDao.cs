namespace ASC.Files.Thirdparty.GoogleDrive;

[Scope]
internal class GoogleDriveSecurityDao : GoogleDriveDaoBase, ISecurityDao<string>
{
    public GoogleDriveSecurityDao(
        IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        DbContextManager<FilesDbContext> dbContextManager,
        SetupInfo setupInfo,
        IOptionsMonitor<ILog> monitor,
        FileUtility fileUtility,
        TempPath tempPath)
        : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextManager, setupInfo, monitor, fileUtility, tempPath)
    {
    }
}
