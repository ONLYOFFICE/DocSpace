/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

namespace ASC.ElasticSearch;

[Singletone(Additional = typeof(ServiceLauncherExtension))]
public class ElasticSearchIndexService : IHostedService, IDisposable
{
    private readonly ILog _logger;
    private readonly ICacheNotify<AscCacheItem> _notify;
    private readonly ICacheNotify<IndexAction> _indexNotify;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly TimeSpan _period;
    private Timer _timer;
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
        _cancellationTokenSource = new CancellationTokenSource();
        _period = TimeSpan.FromMinutes(settings.Period.Value);
    }

    public Task StartAsync(CancellationToken cancellationToken)
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

        var task = new Task(async () =>
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<ElasticSearchIndexServiceScope>();
            var (factoryIndexer, service) = scopeClass;
            while (!factoryIndexer.CheckState(false))
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                await Task.Delay(10000);
            }

            service.Subscribe();
            _timer = new Timer(_ => IndexAll(), null, TimeSpan.Zero, TimeSpan.Zero);

        }, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);

        task.ConfigureAwait(false);
        task.Start();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.Info("ElasticSearch Index Service is stopping.");

        _isStarted = false;

        _timer?.Change(Timeout.Infinite, 0);

        _cancellationTokenSource.Cancel();

        return Task.CompletedTask;
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
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _isStarted = true;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var wrappers = scope.ServiceProvider.GetService<IEnumerable<IFactoryIndexer>>();

                Parallel.ForEach(wrappers, wrapper =>
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var w = (IFactoryIndexer)scope.ServiceProvider.GetService(wrapper.GetType());
                        IndexProduct(w, reindex);
                    }
                });
            }

            _timer.Change(_period, _period);
            _indexNotify.Publish(new IndexAction() { Indexing = "", LastIndexed = DateTime.Now.Ticks }, CacheNotifyAction.Any);
            _isStarted = false;
        }
        catch (Exception e)
        {
            _logger.Fatal("IndexAll", e);

            throw;
        }
    }

    public void Dispose()
    {
        if (_timer == null)
        {
            return;
        }

        var handle = new AutoResetEvent(false);

        if (!_timer.Dispose(handle))
        {
            throw new Exception("Timer already disposed");
        }   

        handle.WaitOne();
    }
}

[Scope]
public class ElasticSearchIndexServiceScope
{
    private readonly FactoryIndexer _factoryIndexer;
    private readonly ElasticSearchService _service;

    public ElasticSearchIndexServiceScope(FactoryIndexer factoryIndexer, ElasticSearchService service)
    {
        _factoryIndexer = factoryIndexer;
        _service = service;
    }

    public void Deconstruct(out FactoryIndexer factoryIndexer, out ElasticSearchService service)
    {
        factoryIndexer = _factoryIndexer;
        service = _service;
    }
}

public class ServiceLauncherExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<ElasticSearchIndexServiceScope>();
    }
}
