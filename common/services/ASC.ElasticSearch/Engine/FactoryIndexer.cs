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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Common.Threading;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.ElasticSearch.Core;

using Autofac;

using Elasticsearch.Net;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Nest;

namespace ASC.ElasticSearch
{
    public class FactoryIndexerHelper
    {
        public ICache Cache { get; }
        public ILog Logger { get; }
        public FactoryIndexer FactoryIndexer { get; }

        public FactoryIndexerHelper(IOptionsMonitor<ILog> options, FactoryIndexer factoryIndexer)
        {
            Cache = AscCache.Memory;
            Logger = options.Get("ASC.Indexer");
            FactoryIndexer = factoryIndexer;
        }

        public bool Support<T>(T t) where T : class, ISearchItem
        {
            if (!FactoryIndexer.CheckState()) return false;

            var cacheTime = DateTime.UtcNow.AddMinutes(15);
            var key = "elasticsearch " + t.IndexName;
            try
            {
                var cacheValue = Cache.Get<string>(key);
                if (!string.IsNullOrEmpty(cacheValue))
                {
                    return Convert.ToBoolean(cacheValue);
                }

                //TODO:
                //var service = new Service.Service();

                //var result = service.Support(t.IndexName);

                //Cache.Insert(key, result.ToString(CultureInfo.InvariantCulture).ToLower(), cacheTime);

                return true;
            }
            catch (Exception e)
            {
                Cache.Insert(key, "false", cacheTime);
                Logger.Error("FactoryIndexer CheckState", e);
                return false;
            }
        }

    }

    public class FactoryIndexer<T> where T : class, ISearchItem
    {
        private static readonly TaskScheduler Scheduler = new LimitedConcurrencyLevelTaskScheduler(10);

        public ILog Logger { get; }

        public FactoryIndexerHelper FactoryIndexerHelper { get; }
        public TenantManager TenantManager { get; }
        public SearchSettingsHelper SearchSettingsHelper { get; }
        public FactoryIndexer FactoryIndexerCommon { get; }
        public BaseIndexer<T> Indexer { get; }
        public Client Client { get; }
        public IServiceProvider ServiceProvider { get; }
        public string IndexName { get => Indexer.IndexName; }


        public FactoryIndexer(
            IOptionsMonitor<ILog> options,
            FactoryIndexerHelper factoryIndexerSupport,
            TenantManager tenantManager,
            SearchSettingsHelper searchSettingsHelper,
            FactoryIndexer factoryIndexer,
            BaseIndexer<T> baseIndexer,
            Client client,
            IServiceProvider serviceProvider)
        {
            Logger = options.Get("ASC.Indexer");
            FactoryIndexerHelper = factoryIndexerSupport;
            TenantManager = tenantManager;
            SearchSettingsHelper = searchSettingsHelper;
            FactoryIndexerCommon = factoryIndexer;
            Indexer = baseIndexer;
            Client = client;
            ServiceProvider = serviceProvider;

            Indexer.CreateIfNotExist(ServiceProvider.GetService<T>());
        }

        public bool TrySelect(Expression<Func<Selector<T>, Selector<T>>> expression, out IReadOnlyCollection<T> result)
        {
            var t = ServiceProvider.GetService<T>();
            if (!FactoryIndexerHelper.Support(t) || !Indexer.CheckExist(t))
            {
                result = new List<T>();
                return false;
            }

            try
            {
                result = Indexer.Select(expression);
            }
            catch (Exception e)
            {
                Logger.Error("Select", e);
                result = new List<T>();
                return false;
            }
            return true;
        }

        public bool TrySelectIds(Expression<Func<Selector<T>, Selector<T>>> expression, out List<int> result)
        {
            var t = ServiceProvider.GetService<T>();
            if (!FactoryIndexerHelper.Support(t) || !Indexer.CheckExist(t))
            {
                result = new List<int>();
                return false;
            }

            try
            {
                result = Indexer.Select(expression, true).Select(r => r.Id).ToList();
            }
            catch (Exception e)
            {
                Logger.Error("Select", e);
                result = new List<int>();
                return false;
            }

            return true;
        }

        public bool TrySelectIds(Expression<Func<Selector<T>, Selector<T>>> expression, out List<int> result, out long total)
        {
            var t = ServiceProvider.GetService<T>();
            if (!FactoryIndexerHelper.Support(t) || !Indexer.CheckExist(t))
            {
                result = new List<int>();
                total = 0;
                return false;
            }

            try
            {
                result = Indexer.Select(expression, true, out total).Select(r => r.Id).ToList();
            }
            catch (Exception e)
            {
                Logger.Error("Select", e);
                total = 0;
                result = new List<int>();
                return false;
            }

            return true;
        }

        public bool CanSearchByContent()
        {
            return SearchSettingsHelper.CanSearchByContent<T>(TenantManager.GetCurrentTenant().TenantId);
        }

        public bool Index(T data, bool immediately = true)
        {
            var t = ServiceProvider.GetService<T>();
            if (!FactoryIndexerHelper.Support(t)) return false;

            try
            {
                Indexer.Index(data, immediately);
                return true;
            }
            catch (Exception e)
            {
                Logger.Error("Index", e);
            }
            return false;
        }

        public void Index(List<T> data, bool immediately = true)
        {
            var t = ServiceProvider.GetService<T>();
            if (!FactoryIndexerHelper.Support(t) || !data.Any()) return;

            try
            {
                Indexer.Index(data, immediately);
            }
            catch (AggregateException e)
            {
                if (e.InnerExceptions.Count == 0) throw;

                var inner = e.InnerExceptions.OfType<ElasticsearchClientException>().FirstOrDefault();
                Logger.Error(inner);

                if (inner != null)
                {
                    Logger.Error("inner", inner.Response.OriginalException);

                    if (inner.Response.HttpStatusCode == 413)
                    {
                        data.ForEach(r => Index(r, immediately));
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        public void Update(T data, bool immediately = true, params Expression<Func<T, object>>[] fields)
        {
            var t = ServiceProvider.GetService<T>();
            if (!FactoryIndexerHelper.Support(t)) return;

            try
            {
                Indexer.Update(data, immediately, fields);
            }
            catch (Exception e)
            {
                Logger.Error("Update", e);
            }
        }

        public void Update(T data, UpdateAction action, Expression<Func<T, IList>> field, bool immediately = true)
        {
            var t = ServiceProvider.GetService<T>();
            if (!FactoryIndexerHelper.Support(t)) return;

            try
            {
                Indexer.Update(data, action, field, immediately);
            }
            catch (Exception e)
            {
                Logger.Error("Update", e);
            }
        }

        public void Update(T data, Expression<Func<Selector<T>, Selector<T>>> expression, bool immediately = true, params Expression<Func<T, object>>[] fields)
        {
            var t = ServiceProvider.GetService<T>();
            if (!FactoryIndexerHelper.Support(t)) return;

            try
            {
                var tenant = TenantManager.GetCurrentTenant().TenantId;
                Indexer.Update(data, expression, tenant, immediately, fields);
            }
            catch (Exception e)
            {
                Logger.Error("Update", e);
            }
        }

        public void Update(T data, Expression<Func<Selector<T>, Selector<T>>> expression, UpdateAction action, Expression<Func<T, IList>> fields, bool immediately = true)
        {
            var t = ServiceProvider.GetService<T>();
            if (!FactoryIndexerHelper.Support(t)) return;

            try
            {
                var tenant = TenantManager.GetCurrentTenant().TenantId;
                Indexer.Update(data, expression, tenant, action, fields, immediately);
            }
            catch (Exception e)
            {
                Logger.Error("Update", e);
            }
        }

        public void Delete(T data, bool immediately = true)
        {
            var t = ServiceProvider.GetService<T>();
            if (!FactoryIndexerHelper.Support(t)) return;

            try
            {
                Indexer.Delete(data, immediately);
            }
            catch (Exception e)
            {
                Logger.Error("Delete", e);
            }
        }

        public void Delete(Expression<Func<Selector<T>, Selector<T>>> expression, bool immediately = true)
        {
            var t = ServiceProvider.GetService<T>();
            if (!FactoryIndexerHelper.Support(t)) return;

            var tenant = TenantManager.GetCurrentTenant().TenantId;

            try
            {
                Indexer.Delete(expression, tenant, immediately);
            }
            catch (Exception e)
            {
                Logger.Error("Index", e);
            }
        }

        public Task<bool> IndexAsync(T data, bool immediately = true)
        {
            var t = ServiceProvider.GetService<T>();
            if (!FactoryIndexerHelper.Support(t)) return Task.FromResult(false);
            return Queue(() => Indexer.Index(data, immediately));
        }

        public Task<bool> IndexAsync(List<T> data, bool immediately = true)
        {
            var t = ServiceProvider.GetService<T>();
            if (!FactoryIndexerHelper.Support(t)) return Task.FromResult(false);
            return Queue(() => Indexer.Index(data, immediately));
        }

        public Task<bool> UpdateAsync(T data, bool immediately = true, params Expression<Func<T, object>>[] fields)
        {
            var t = ServiceProvider.GetService<T>();
            if (!FactoryIndexerHelper.Support(t)) return Task.FromResult(false);
            return Queue(() => Indexer.Update(data, immediately, fields));
        }

        public Task<bool> DeleteAsync(T data, bool immediately = true)
        {
            var t = ServiceProvider.GetService<T>();
            if (!FactoryIndexerHelper.Support(t)) return Task.FromResult(false);
            return Queue(() => Indexer.Delete(data, immediately));
        }

        public Task<bool> DeleteAsync(Expression<Func<Selector<T>, Selector<T>>> expression, bool immediately = true)
        {
            var t = ServiceProvider.GetService<T>();
            if (!FactoryIndexerHelper.Support(t)) return Task.FromResult(false);
            var tenant = TenantManager.GetCurrentTenant().TenantId;
            return Queue(() => Indexer.Delete(expression, tenant, immediately));
        }


        public void Flush()
        {
            var t = ServiceProvider.GetService<T>();
            if (!FactoryIndexerHelper.Support(t)) return;
            Indexer.Flush();
        }

        public void Refresh()
        {
            var t = ServiceProvider.GetService<T>();
            if (!FactoryIndexerHelper.Support(t)) return;
            Indexer.Refresh();
        }

        private Task<bool> Queue(Action actionData)
        {
            var task = new Task<bool>(() =>
            {
                try
                {
                    actionData();
                    return true;
                }
                catch (AggregateException agg)
                {
                    foreach (var e in agg.InnerExceptions)
                    {
                        Logger.Error(e);
                    }
                    throw;
                }

            }, TaskCreationOptions.LongRunning);

            task.ConfigureAwait(false);
            task.Start(Scheduler);
            return task;
        }

        public virtual void IndexAll()
        {
            return;
        }
    }

    public class FactoryIndexer
    {
        private static ICache cache = AscCache.Memory;
        internal IContainer Builder { get; set; }
        internal static bool Init { get; set; }
        public ILog Log { get; }
        public Client Client { get; }
        public CoreBaseSettings CoreBaseSettings { get; }

        public FactoryIndexer(
            IContainer container,
            Client client,
            IOptionsMonitor<ILog> options,
            CoreBaseSettings coreBaseSettings)
        {
            Client = client;
            CoreBaseSettings = coreBaseSettings;

            try
            {
                Log = options.Get("ASC.Indexer");

                if (container != null)
                {
                    Builder = container;
                    Init = true;
                }
            }
            catch (Exception e)
            {
                Log.Fatal("FactoryIndexer", e);
            }
        }

        public bool CheckState(bool cacheState = true)
        {
            if (!Init) return false;

            const string key = "elasticsearch";

            if (cacheState)
            {
                var cacheValue = cache.Get<string>(key);
                if (!string.IsNullOrEmpty(cacheValue))
                {
                    return Convert.ToBoolean(cacheValue);
                }
            }

            var cacheTime = DateTime.UtcNow.AddMinutes(15);

            try
            {
                var result = Client.Instance.Ping(new PingRequest());

                var isValid = result.IsValid;

                Log.DebugFormat("CheckState ping {0}", result.DebugInformation);

                if (cacheState)
                {
                    cache.Insert(key, isValid.ToString(CultureInfo.InvariantCulture).ToLower(), cacheTime);
                }

                return isValid;
            }
            catch (Exception e)
            {
                if (cacheState)
                {
                    cache.Insert(key, "false", cacheTime);
                }

                Log.Error("Ping false", e);
                return false;
            }
        }

        public object GetState(TenantUtil tenantUtil)
        {
            var indices = CoreBaseSettings.Standalone ?
                Client.Instance.Cat.Indices(new CatIndicesRequest { SortByColumns = new[] { "index" } }).Records.Select(r => new
                {
                    r.Index,
                    r.DocsCount,
                    r.StoreSize
                }) :
                null;

            State state = null;

            if (CoreBaseSettings.Standalone)
            {
                //TODO
                //using (var service = new ServiceClient())
                //{
                //    state = service.GetState();
                //}

                if (state.LastIndexed.HasValue)
                {
                    state.LastIndexed = tenantUtil.DateTimeFromUtc(state.LastIndexed.Value);
                }
            }

            return new
            {
                state,
                //indices,
                status = CheckState()
            };
        }

        public void Reindex(string name)
        {
            if (!CoreBaseSettings.Standalone) return;

            var generic = typeof(BaseIndexer<>);
            var indexers = Builder.Resolve<IEnumerable<Wrapper>>()
                .Where(r => string.IsNullOrEmpty(name) || r.IndexName == name)
                .Select(r => (IIndexer)Activator.CreateInstance(generic.MakeGenericType(r.GetType()), r));

            foreach (var indexer in indexers)
            {
                indexer.ReIndex();
            }
        }
    }

    public static class FactoryIndexerExtention
    {
        public static DIHelper AddFactoryIndexerService(this DIHelper services)
        {
            services.TryAddScoped<FactoryIndexer>();
            return services
                .AddClientService()
                .AddCoreBaseSettingsService();
        }

        public static DIHelper AddFactoryIndexerHelperService(this DIHelper services)
        {
            services.TryAddScoped<FactoryIndexerHelper>();
            return services
                .AddFactoryIndexerService();
        }

        public static DIHelper AddFactoryIndexerService<T>(this DIHelper services, bool addBase = true) where T : class, ISearchItem
        {
            if (addBase)
            {
                services.TryAddScoped<FactoryIndexer<T>>();
            }

            services.TryAddScoped<Selector<T>>();

            return services
                .AddFactoryIndexerHelperService()
                .AddTenantManagerService()
                .AddFactoryIndexerService()
                .AddClientService()
                .AddSearchSettingsHelperService()
                .AddBaseIndexerService<T>();
        }
    }
}