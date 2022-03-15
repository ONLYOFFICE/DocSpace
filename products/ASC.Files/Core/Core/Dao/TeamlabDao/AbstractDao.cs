namespace ASC.Files.Core.Data;

public class AbstractDao
{
    protected readonly ICache Cache;

    private readonly Lazy<FilesDbContext> _lazyFilesDbContext;
    public FilesDbContext FilesDbContext => _lazyFilesDbContext.Value;
    private int _tenantID;
    protected internal int TenantID
    {
        get
        {
            if (_tenantID == 0)
            {
                _tenantID = TenantManager.GetCurrentTenant().Id;
            }

            return _tenantID;
        }
    }
    protected readonly UserManager UserManager;
    protected readonly TenantManager TenantManager;
    protected readonly TenantUtil TenantUtil;
    protected readonly SetupInfo SetupInfo;
    protected readonly TenantExtra TenantExtra;
    protected readonly TenantStatisticsProvider TenantStatisticProvider;
    protected readonly CoreBaseSettings CoreBaseSettings;
    protected readonly CoreConfiguration CoreConfiguration;
    protected readonly SettingsManager SettingsManager;
    protected readonly AuthContext AuthContext;
    protected readonly IServiceProvider ServiceProvider;

    protected AbstractDao(
        DbContextManager<FilesDbContext> dbContextManager,
        UserManager userManager,
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
    {
        Cache = cache;
        _lazyFilesDbContext = new Lazy<FilesDbContext>(() => dbContextManager.Get(FileConstant.DatabaseId));
        UserManager = userManager;
        TenantManager = tenantManager;
        TenantUtil = tenantUtil;
        SetupInfo = setupInfo;
        TenantExtra = tenantExtra;
        TenantStatisticProvider = tenantStatisticProvider;
        CoreBaseSettings = coreBaseSettings;
        CoreConfiguration = coreConfiguration;
        SettingsManager = settingsManager;
        AuthContext = authContext;
        ServiceProvider = serviceProvider;
    }


    protected IQueryable<T> Query<T>(DbSet<T> set) where T : class, IDbFile
    {
        var tenantId = TenantID;

        return set.AsQueryable().Where(r => r.TenantId == tenantId);
    }

    protected internal IQueryable<DbFile> GetFileQuery(Expression<Func<DbFile, bool>> where)
    {
        return Query(FilesDbContext.Files)
            .Where(where);
    }

    protected async Task GetRecalculateFilesCountUpdateAsync(int folderId)
    {
        var folders = await FilesDbContext.Folders
            .AsQueryable()
            .Where(r => r.TenantId == TenantID)
            .Where(r => FilesDbContext.Tree.AsQueryable().Where(r => r.FolderId == folderId).Select(r => r.ParentId).Any(a => a == r.Id))
            .ToListAsync();

        foreach (var f in folders)
        {
            f.FilesCount = await
                FilesDbContext.Files
                .AsQueryable()
                .Join(FilesDbContext.Tree, a => a.FolderId, b => b.FolderId, (file, tree) => new { file, tree })
                .Where(r => r.file.TenantId == f.TenantId)
                .Where(r => r.tree.ParentId == f.Id)
                .Select(r => r.file.Id)
                .Distinct()
                .CountAsync();
        }

        await FilesDbContext.SaveChangesAsync();
    }

    protected ValueTask<object> MappingIDAsync(object id, bool saveIfNotExist = false)
    {
        if (id == null)
        {
            return ValueTask.FromResult<object>(null);
        }

        var isNumeric = int.TryParse(id.ToString(), out var n);

        if (isNumeric)
        {
            return ValueTask.FromResult<object>(n);
        }

        return InternalMappingIDAsync(id, saveIfNotExist);
    }

    private async ValueTask<object> InternalMappingIDAsync(object id, bool saveIfNotExist = false)
    {
        object result;

        if (id.ToString().StartsWith("sbox")
            || id.ToString().StartsWith("box")
            || id.ToString().StartsWith("dropbox")
            || id.ToString().StartsWith("spoint")
            || id.ToString().StartsWith("drive")
            || id.ToString().StartsWith("onedrive"))
        {
            result = Regex.Replace(BitConverter.ToString(Hasher.Hash(id.ToString(), HashAlg.MD5)), "-", "").ToLower();
        }
        else
        {
            result = await Query(FilesDbContext.ThirdpartyIdMapping)
                .Where(r => r.HashId == id.ToString())
                .Select(r => r.Id)
                .FirstOrDefaultAsync();
        }

        if (saveIfNotExist)
        {
            var newItem = new DbFilesThirdpartyIdMapping
            {
                Id = id.ToString(),
                HashId = result.ToString(),
                TenantId = TenantID
            };

            await FilesDbContext.AddOrUpdateAsync(r => r.ThirdpartyIdMapping, newItem);
        }

        return result;
    }

    protected ValueTask<object> MappingIDAsync(object id)
    {
        return MappingIDAsync(id, false);
    }

    internal static IQueryable<T> BuildSearch<T>(IQueryable<T> query, string text, SearhTypeEnum searhTypeEnum) where T : IDbSearch
    {
        var lowerText = GetSearchText(text);

        return searhTypeEnum switch
        {
            SearhTypeEnum.Start => query.Where(r => r.Title.ToLower().StartsWith(lowerText)),
            SearhTypeEnum.End => query.Where(r => r.Title.ToLower().EndsWith(lowerText)),
            SearhTypeEnum.Any => query.Where(r => r.Title.ToLower().Contains(lowerText)),
            _ => query,
        };
    }

    internal static string GetSearchText(string text) => (text ?? "").ToLower().Trim().Replace("%", "\\%").Replace("_", "\\_");

    internal enum SearhTypeEnum
    {
        Start,
        End,
        Any
    }
}
