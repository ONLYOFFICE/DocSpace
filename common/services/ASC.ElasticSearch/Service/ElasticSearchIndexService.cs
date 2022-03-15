namespace ASC.ElasticSearch;

public class ElasticSearchIndexService : BackgroundService
{
    private readonly ILog _logger;
    private readonly ICacheNotify<AscCacheItem> _notify;
    private readonly ICacheNotify<IndexAction> _indexNotify;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TimeSpan _period;
    private bool _isStarted;

    public ElasticSearchIndexService(
        IOptionsMonitor<ILog> options,
        ICacheNotify<AscCacheItem> notify,
        ICacheNotify<IndexAction> indexNotify,
        IServiceScopeFactory serviceScopeFactory,
        Settings settings)
    {
        _logger = options.Get("ASC.Indexer");
        _notify = notify;
        _indexNotify = indexNotify;
        _serviceScopeFactory = serviceScopeFactory;
        _period = TimeSpan.FromMinutes(settings.Period.Value);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Info("ElasticSearch Index Service running.");

        try
        {
            _notify.Subscribe(async (item) =>
            {
                while (_isStarted)
                {
                    await Task.Delay(10000);
                }
                IndexAll(true);
            }, CacheNotifyAction.Any);
        }
        catch (Exception e)
        {
            _logger.Error("Subscribe on start", e);
        }

        using var scope = _serviceScopeFactory.CreateScope();
        var factoryIndexer = scope.ServiceProvider.GetService<FactoryIndexer>();

        while (!factoryIndexer.CheckState(false))
        {
            if (stoppingToken.IsCancellationRequested)
            {
                return;
            }

            await Task.Delay(10000);
        }

        var service = scope.ServiceProvider.GetService<ElasticSearchService>();
        service.Subscribe();

        while (!stoppingToken.IsCancellationRequested)
        {
            IndexAll();

            await Task.Delay(_period, stoppingToken);
        }
    }

    public void IndexProduct(IFactoryIndexer product, bool reindex)
    {
        if (reindex)
        {
            try
            {
                if (!_isStarted)
                {
                    return;
                }

                _logger.DebugFormat("Product reindex {0}", product.IndexName);
                product.ReIndex();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                _logger.ErrorFormat("Product reindex {0}", product.IndexName);
            }
        }

        try
        {
            if (!_isStarted)
            {
                return;
            }

            _logger.DebugFormat("Product {0}", product.IndexName);
            _indexNotify.Publish(new IndexAction() { Indexing = product.IndexName, LastIndexed = 0 }, CacheNotifyAction.Any);
            product.IndexAll();
        }
        catch (Exception e)
        {
            _logger.Error(e);
            _logger.ErrorFormat("Product {0}", product.IndexName);
        }
    }

    private void IndexAll(bool reindex = false)
    {
        try
        {
            _isStarted = true;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var wrappers = scope.ServiceProvider.GetService<IEnumerable<IFactoryIndexer>>();

                Parallel.ForEach(wrappers, wrapper =>
                {
                    IndexProduct(wrapper, reindex);
                });
            }

            _indexNotify.Publish(new IndexAction() { Indexing = "", LastIndexed = DateTime.Now.Ticks }, CacheNotifyAction.Any);
            _isStarted = false;
        }
        catch (Exception e)
        {
            _logger.Fatal("IndexAll", e);

            throw;
        }
    }
}