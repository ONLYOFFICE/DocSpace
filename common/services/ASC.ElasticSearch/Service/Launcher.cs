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


using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.ElasticSearch.Service;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ASC.ElasticSearch
{
    [Singletone(Additional = typeof(ServiceLauncherExtension))]
    public class ServiceLauncher : IHostedService
    {
        private ILog Log { get; }
        private ICacheNotify<AscCacheItem> Notify { get; }
        private ICacheNotify<IndexAction> IndexNotify { get; }
        private IServiceProvider ServiceProvider { get; }
        private bool IsStarted { get; set; }
        private CancellationTokenSource CancellationTokenSource { get; set; }
        private Timer Timer { get; set; }
        private TimeSpan Period { get; set; }

        public ServiceLauncher(
            IOptionsMonitor<ILog> options,
            ICacheNotify<AscCacheItem> notify,
            ICacheNotify<IndexAction> indexNotify,
            IServiceProvider serviceProvider,
            ConfigurationExtension configurationExtension)
        {
            Log = options.Get("ASC.Indexer");
            Notify = notify;
            IndexNotify = indexNotify;
            ServiceProvider = serviceProvider;
            CancellationTokenSource = new CancellationTokenSource();
            var settings = Settings.GetInstance(configurationExtension);
            Period = TimeSpan.FromMinutes(settings.Period.Value);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                Notify.Subscribe(async (item) =>
                {
                    while (IsStarted)
                    {
                        await Task.Delay(10000);
                    }
                    IndexAll(true);
                }, CacheNotifyAction.Any);
            }
            catch (Exception e)
            {
                Log.Error("Subscribe on start", e);
            }

            var task = new Task(async () =>
            {
                using var scope = ServiceProvider.CreateScope();
                var scopeClass = scope.ServiceProvider.GetService<ServiceLauncherScope>();
                var (factoryIndexer, service) = scopeClass;
                while (!factoryIndexer.CheckState(false))
                {
                    if (CancellationTokenSource.IsCancellationRequested)
                    {
                        return;
                    }

                    await Task.Delay(10000);
                }

                service.Subscribe();
                Timer = new Timer(_ => IndexAll(), null, TimeSpan.Zero, TimeSpan.Zero);

            }, CancellationTokenSource.Token, TaskCreationOptions.LongRunning);

            task.ConfigureAwait(false);
            task.Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            IsStarted = false;

            if (Timer != null)
            {
                Timer.Dispose();
            }

            CancellationTokenSource.Cancel();

            return Task.CompletedTask;
        }

        private void IndexAll(bool reindex = false)
        {
            try
            {
                Timer.Change(Timeout.Infinite, Timeout.Infinite);
                IsStarted = true;

                using (var scope = ServiceProvider.CreateScope())
                {
                    var wrappers = scope.ServiceProvider.GetService<IEnumerable<IFactoryIndexer>>();

                    Parallel.ForEach(wrappers, wrapper =>
                    {
                        using (var scope = ServiceProvider.CreateScope())
                        {
                            var w = (IFactoryIndexer)scope.ServiceProvider.GetService(wrapper.GetType());
                            IndexProduct(w, reindex);
                        }
                    });
                }

                Timer.Change(Period, Period);
                IndexNotify.Publish(new IndexAction() { Indexing = "", LastIndexed = DateTime.Now.Ticks }, CacheNotifyAction.Any);
                IsStarted = false;
            }
            catch (Exception e)
            {
                Log.Fatal("IndexAll", e);
                throw;
            }
        }

        public void IndexProduct(IFactoryIndexer product, bool reindex)
        {
            if (reindex)
            {
                try
                {
                    if (!IsStarted) return;

                    Log.DebugFormat("Product reindex {0}", product.IndexName);
                    product.ReIndex();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    Log.ErrorFormat("Product reindex {0}", product.IndexName);
                }
            }

            try
            {
                if (!IsStarted) return;

                Log.DebugFormat("Product {0}", product.IndexName);
                IndexNotify.Publish(new IndexAction() { Indexing = product.IndexName, LastIndexed = 0 }, CacheNotifyAction.Any);
                product.IndexAll();
            }
            catch (Exception e)
            {
                Log.Error(e);
                Log.ErrorFormat("Product {0}", product.IndexName);
            }
        }
    }

    [Scope]
    public class ServiceLauncherScope
    {
        private FactoryIndexer FactoryIndexer { get; }
        private Service.Service Service { get; }

        public ServiceLauncherScope(FactoryIndexer factoryIndexer, Service.Service service)
        {
            FactoryIndexer = factoryIndexer;
            Service = service;
        }

        public void Deconstruct(out FactoryIndexer factoryIndexer, out Service.Service service)
        {
            factoryIndexer = FactoryIndexer;
            service = Service;
        }
    }

    public class ServiceLauncherExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<ServiceLauncherScope>();
        }
    }
}
