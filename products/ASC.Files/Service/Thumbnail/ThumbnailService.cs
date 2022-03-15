namespace ASC.Files.ThumbnailBuilder;

[Singletone]
public class ThumbnailService : IHostedService
{
    private readonly ICacheNotify<ThumbnailRequest> _cacheNotify;
    private readonly ILog _logger;

    public ThumbnailService(ICacheNotify<ThumbnailRequest> cacheNotify, IOptionsMonitor<ILog> options)
    {
        _cacheNotify = cacheNotify;
        _logger = options.Get("ASC.Files.ThumbnailService");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.Info("Thumbnail Service running.");

        _cacheNotify.Subscribe(BuildThumbnails, CacheNotifyAction.Insert);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.Info("Thumbnail Service is stopping.");

        _cacheNotify.Unsubscribe(CacheNotifyAction.Insert);

        return Task.CompletedTask;
    }

    public void BuildThumbnails(ThumbnailRequest request)
    {
        foreach (var fileId in request.Files)
        {
            var fileData = new FileData<int>(request.Tenant, fileId, request.BaseUrl);

            FileDataQueue.Queue.TryAdd(fileId, fileData);
        }
    }
}
