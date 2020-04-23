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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.ElasticSearch;
using ASC.Web.Files.Core.Search;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ASC.Files.Service
{
    public class ServiceLauncher : IHostedService
    {
        private ILog Log { get; }
        private ICacheNotify<AscCacheItem> Notify { get; }
        public IServiceProvider ServiceProvider { get; }
        private bool IsStarted { get; set; }
        private string Indexing { get; set; }
        private CancellationTokenSource CancellationTokenSource { get; set; }
        private Timer Timer { get; set; }
        private DateTime? LastIndexed { get; set; }
        private TimeSpan Period { get { return TimeSpan.FromMinutes(1); } }//Settings.Default.Period

        public ServiceLauncher(IOptionsMonitor<ILog> options, ICacheNotify<AscCacheItem> notify, IServiceProvider serviceProvider)
        {
            Log = options.Get("ASC.Indexer");
            Notify = notify;
            ServiceProvider = serviceProvider;
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

            var task = new Task(() =>
            {
                using var scope = ServiceProvider.CreateScope();
                var factoryIndexer = scope.ServiceProvider.GetService<FactoryIndexer>();

                while (!factoryIndexer.CheckState(false))
                {
                    if (CancellationTokenSource.IsCancellationRequested)
                    {
                        return;
                    }
                    Thread.Sleep(10000);
                }

                CheckIfChange();
            }, CancellationTokenSource.Token, TaskCreationOptions.LongRunning);

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

        private void CheckIfChange()
        {
            using var scope = ServiceProvider.CreateScope();
            var filesWrapper = scope.ServiceProvider.GetService<FactoryIndexer<FilesWrapper>>();
            IsStarted = true;

            var products = new[] { filesWrapper }.ToList();

            products.ForEach(product =>
            {
                try
                {
                    if (!IsStarted) return;

                    Log.DebugFormat("Product check {0}", product.Indexer.IndexName);
                    Indexing = product.Indexer.IndexName;
                    product.Indexer.Check();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    Log.ErrorFormat("Product check {0}", product.Indexer.IndexName);
                }
            });

            IsStarted = false;
            Indexing = null;

            Timer = new Timer(_ => IndexAll(), null, TimeSpan.Zero, TimeSpan.Zero);
        }

        private void IndexAll(bool reindex = false)
        {
            Timer.Change(-1, -1);
            IsStarted = true;

            using var scope = ServiceProvider.CreateScope();
            var filesWrapper = scope.ServiceProvider.GetService<FactoryIndexer<FilesWrapper>>();
            var foldersWrapper = scope.ServiceProvider.GetService<FactoryIndexer<FoldersWrapper>>();
            var products = new[] { (IIndexer)filesWrapper, (IIndexer)foldersWrapper }.ToList();

            if (reindex)
            {
                products.ForEach(product =>
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
                });
            }

            products.ForEach(product =>
            {
                try
                {
                    if (!IsStarted) return;

                    Log.DebugFormat("Product {0}", product.IndexName);
                    Indexing = product.IndexName;
                    product.IndexAll();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    Log.ErrorFormat("Product {0}", product.IndexName);
                }
            });

            Timer.Change(Period, Period);
            LastIndexed = DateTime.UtcNow;
            IsStarted = false;
            Indexing = null;
        }
    }

    public static class ServiceLauncherExtension
    {
        public static DIHelper AddServiceLauncher(this DIHelper services)
        {
            services.TryAddSingleton<ServiceLauncher>();

            return services;
        }
    }
}
