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