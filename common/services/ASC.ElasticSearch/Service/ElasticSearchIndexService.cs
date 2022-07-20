// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.ElasticSearch;

public class ElasticSearchIndexService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly ICacheNotify<AscCacheItem> _notify;
    private readonly ICacheNotify<IndexAction> _indexNotify;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TimeSpan _period;
    private bool _isStarted;

    public ElasticSearchIndexService(
        ILoggerProvider options,
        ICacheNotify<AscCacheItem> notify,
        ICacheNotify<IndexAction> indexNotify,
        IServiceScopeFactory serviceScopeFactory,
        Settings settings)
    {
        _logger = options.CreateLogger("ASC.Indexer");
        _notify = notify;
        _indexNotify = indexNotify;
        _serviceScopeFactory = serviceScopeFactory;
        _period = TimeSpan.FromMinutes(settings.Period.Value);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ElasticSearch Index Service running.");

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
            _logger.ErrorSubscribeOnStart(e);
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

                _logger.DebugProductReindex(product.IndexName);
                product.ReIndex();
            }
            catch (Exception e)
            {
                _logger.ErrorProductReindex(product.IndexName, e);
            }
        }

        try
        {
            if (!_isStarted)
            {
                return;
            }

            _logger.DebugProduct(product.IndexName);
            _indexNotify.Publish(new IndexAction() { Indexing = product.IndexName, LastIndexed = 0 }, CacheNotifyAction.Any);
            product.IndexAll();
        }
        catch (Exception e)
        {
            _logger.ErrorProductReindex(product.IndexName, e);
        }
    }

    private void IndexAll(bool reindex = false)
    {
        try
        {
            _isStarted = true;

            IEnumerable<Type> wrappers;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                wrappers = scope.ServiceProvider.GetService<IEnumerable<IFactoryIndexer>>().Select(r => r.GetType()).ToList();
            }

            Parallel.ForEach(wrappers, wrapper =>
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    IndexProduct((IFactoryIndexer)scope.ServiceProvider.GetRequiredService(wrapper), reindex);
                }
            });


            _indexNotify.Publish(new IndexAction() { Indexing = "", LastIndexed = DateTime.Now.Ticks }, CacheNotifyAction.Any);
            _isStarted = false;
        }
        catch (Exception e)
        {
            _logger.CriticalIndexAll(e);

            throw;
        }
    }
}