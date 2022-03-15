namespace ASC.Web.Files.Core.Search;

[Scope(Additional = typeof(FactoryIndexerFileExtension))]
public class FactoryIndexerFile : FactoryIndexer<DbFile>
{
    private readonly IDaoFactory _daoFactory;
    private readonly Settings _settings;

    public FactoryIndexerFile(
        IOptionsMonitor<ILog> options,
        TenantManager tenantManager,
        SearchSettingsHelper searchSettingsHelper,
        FactoryIndexer factoryIndexer,
        BaseIndexer<DbFile> baseIndexer,
        IServiceProvider serviceProvider,
        IDaoFactory daoFactory,
        ICache cache,
        Settings settings)
        : base(options, tenantManager, searchSettingsHelper, factoryIndexer, baseIndexer, serviceProvider, cache)
    {
        _daoFactory = daoFactory;
        _settings = settings;
    }

    public override void IndexAll()
    {
        var fileDao = _daoFactory.GetFileDao<int>() as FileDao;

        (int, int, int) getCount(DateTime lastIndexed)
        {
            var dataQuery = GetBaseQuery(lastIndexed)
                .Where(r => r.DbFile.Version == 1)
                .OrderBy(r => r.DbFile.Id)
                .Select(r => r.DbFile.Id);

            var minid = dataQuery.FirstOrDefault();

            dataQuery = GetBaseQuery(lastIndexed)
                .Where(r => r.DbFile.Version == 1)
                .OrderByDescending(r => r.DbFile.Id)
                .Select(r => r.DbFile.Id);

            var maxid = dataQuery.FirstOrDefault();

            var count = GetBaseQuery(lastIndexed)
                .Where(r => r.DbFile.Version == 1)
                .Count();

            return new(count, maxid, minid);
        }

        List<DbFile> getData(long start, long stop, DateTime lastIndexed)
        {
            return GetBaseQuery(lastIndexed)
                .Where(r => r.DbFile.Id >= start && r.DbFile.Id <= stop && r.DbFile.CurrentVersion)
                .Select(r => r.DbFile)
                .ToList();

        }

        List<int> getIds(DateTime lastIndexed)
        {
            var start = 0;
            var result = new List<int>();
            while (true)
            {
                var dataQuery = GetBaseQuery(lastIndexed)
                    .Where(r => r.DbFile.Id >= start)
                    .Where(r => r.DbFile.Version == 1)
                    .OrderBy(r => r.DbFile.Id)
                    .Select(r => r.DbFile.Id)
                    .Skip(BaseIndexer<DbFile>.QueryLimit);

                var id = dataQuery.FirstOrDefault();
                if (id != 0)
                {
                    start = id;
                    result.Add(id);
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        IQueryable<FileTenant> GetBaseQuery(DateTime lastIndexed) => fileDao.FilesDbContext.Files
            .AsQueryable()
            .Where(r => r.ModifiedOn >= lastIndexed)
            .Join(fileDao.FilesDbContext.Tenants, r => r.TenantId, r => r.Id, (f, t) => new FileTenant { DbFile = f, DbTenant = t })
            .Where(r => r.DbTenant.Status == TenantStatus.Active);

        try
        {
            var j = 0;
            var tasks = new List<Task>();

            foreach (var data in Indexer.IndexAll(getCount, getIds, getData))
            {
                if (_settings.Threads == 1)
                {
                    data.ForEach(r =>
                    {
                        TenantManager.SetCurrentTenant(r.TenantId);
                        fileDao.InitDocumentAsync(r).Wait();
                    });
                    Index(data);
                }
                else
                {
                    //TODO: refactoring
                    data.ForEach(r =>
                    {
                        TenantManager.SetCurrentTenant(r.TenantId);
                        fileDao.InitDocumentAsync(r).Wait();
                    });

                    tasks.Add(IndexAsync(data));
                    j++;
                    if (j >= _settings.Threads)
                    {
                        Task.WaitAll(tasks.ToArray());
                        tasks = new List<Task>();
                        j = 0;
                    }
                }
            }

            if (tasks.Count > 0)
            {
                Task.WaitAll(tasks.ToArray());
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
            throw;
        }
    }

    public override string SettingsTitle => FilesCommonResource.IndexTitle;
}

public class FileTenant
{
    public DbTenant DbTenant { get; set; }
    public DbFile DbFile { get; set; }
}

public static class FactoryIndexerFileExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<DbFile>();
    }
}
