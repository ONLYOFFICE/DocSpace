namespace ASC.Web.Files.Core.Search;

[Scope(Additional = typeof(FactoryIndexerFolderExtension))]
public class FactoryIndexerFolder : FactoryIndexer<DbFolder>
{
    private readonly IDaoFactory _daoFactory;
    private readonly Settings _settings;

    public FactoryIndexerFolder(
        IOptionsMonitor<ILog> options,
        TenantManager tenantManager,
        SearchSettingsHelper searchSettingsHelper,
        FactoryIndexer factoryIndexer,
        BaseIndexer<DbFolder> baseIndexer,
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
        var folderDao = _daoFactory.GetFolderDao<int>() as FolderDao;

        (int, int, int) getCount(DateTime lastIndexed)
        {
            var dataQuery = GetBaseQuery(lastIndexed)
                .OrderBy(r => r.DbFolder.Id)
                .Select(r => r.DbFolder.Id);

            var minid = dataQuery.FirstOrDefault();

            dataQuery = GetBaseQuery(lastIndexed)
                .OrderByDescending(r => r.DbFolder.Id)
                .Select(r => r.DbFolder.Id);

            var maxid = dataQuery.FirstOrDefault();

            var count = GetBaseQuery(lastIndexed).Count();

            return new(count, maxid, minid);
        }

        List<DbFolder> getData(long start, long stop, DateTime lastIndexed)
        {
            return GetBaseQuery(lastIndexed)
                .Where(r => r.DbFolder.Id >= start && r.DbFolder.Id <= stop)
                .Select(r => r.DbFolder)
                .ToList();

        }

        List<int> getIds(DateTime lastIndexed)
        {
            var start = 0;
            var result = new List<int>();
            while (true)
            {
                var dataQuery = GetBaseQuery(lastIndexed)
                    .Where(r => r.DbFolder.Id >= start)
                    .OrderBy(r => r.DbFolder.Id)
                    .Select(r => r.DbFolder.Id)
                    .Skip(BaseIndexer<DbFolder>.QueryLimit);

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

        IQueryable<FolderTenant> GetBaseQuery(DateTime lastIndexed) => folderDao.FilesDbContext.Folders
                .AsQueryable()
                .Where(r => r.ModifiedOn >= lastIndexed)
                .Join(folderDao.FilesDbContext.Tenants, r => r.TenantId, r => r.Id, (f, t) => new FolderTenant { DbFolder = f, DbTenant = t })
                .Where(r => r.DbTenant.Status == TenantStatus.Active);

        try
        {
            var j = 0;
            var tasks = new List<Task>();

            foreach (var data in Indexer.IndexAll(getCount, getIds, getData))
            {
                if (_settings.Threads == 1)
                {
                    Index(data);
                }
                else
                {
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
}

class FolderTenant
{
    public DbTenant DbTenant { get; set; }
    public DbFolder DbFolder { get; set; }
}

public static class FactoryIndexerFolderExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<DbFolder>();
    }
}
