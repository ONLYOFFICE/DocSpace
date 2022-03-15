namespace ASC.Files.Thirdparty.GoogleDrive;

[Scope]
internal class GoogleDriveTagDao : GoogleDriveDaoBase, ITagDao<string>
{
    public GoogleDriveTagDao(
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
