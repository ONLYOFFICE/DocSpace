namespace ASC.Files.Core.Data;

[Scope]
internal class LinkDao : AbstractDao, ILinkDao
{
    public DbContextManager<FilesDbContext> DbContextManager { get; }

    public LinkDao(
        UserManager userManager,
        DbContextManager<EF.FilesDbContext> dbContextManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil,
        SetupInfo setupInfo,
        TenantExtra tenantExtra,
        TenantStatisticsProvider tenantStatisticProvider,
        CoreBaseSettings coreBaseSettings,
        CoreConfiguration coreConfiguration,
        SettingsManager settingsManager,
        AuthContext authContext,
        IServiceProvider serviceProvider,
        ICache cache)
        : base(
              dbContextManager,
              userManager,
              tenantManager,
              tenantUtil,
              setupInfo,
              tenantExtra,
              tenantStatisticProvider,
              coreBaseSettings,
              coreConfiguration,
              settingsManager,
              authContext,
              serviceProvider,
              cache)
    { }

    public async Task AddLinkAsync(string sourceId, string linkedId)
    {
        await FilesDbContext.AddOrUpdateAsync(r => r.FilesLink, new DbFilesLink()
        {
            TenantId = TenantID,
            SourceId = sourceId,
            LinkedId = linkedId,
            LinkedFor = AuthContext.CurrentAccount.ID
        });

        await FilesDbContext.SaveChangesAsync();
    }

    public Task<string> GetSourceAsync(string linkedId)
    {
        return FilesDbContext.FilesLink
            .Where(r => r.TenantId == TenantID && r.LinkedId == linkedId && r.LinkedFor == AuthContext.CurrentAccount.ID)
            .Select(r => r.SourceId)
            .SingleOrDefaultAsync();
    }

    public Task<string> GetLinkedAsync(string sourceId)
    {
        return FilesDbContext.FilesLink
            .Where(r => r.TenantId == TenantID && r.SourceId == sourceId && r.LinkedFor == AuthContext.CurrentAccount.ID)
            .Select(r => r.LinkedId)
            .SingleOrDefaultAsync();
    }

    public async Task DeleteLinkAsync(string sourceId)
    {
        var link = await FilesDbContext.FilesLink
            .Where(r => r.TenantId == TenantID && r.SourceId == sourceId && r.LinkedFor == AuthContext.CurrentAccount.ID)
            .SingleOrDefaultAsync();

        FilesDbContext.FilesLink.Remove(link);

        await FilesDbContext.SaveChangesAsync();
    }

    public async Task DeleteAllLinkAsync(string fileId)
    {
        var link = await FilesDbContext.FilesLink.Where(r => r.TenantId == TenantID && (r.SourceId == fileId || r.LinkedId == fileId)).ToListAsync();

        FilesDbContext.FilesLink.RemoveRange(link);

        await FilesDbContext.SaveChangesAsync();
    }
}
