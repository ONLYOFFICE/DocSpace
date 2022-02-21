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

[Singletone]
public class FactoryIndexerHelper
{
    public DateTime LastIndexed { get; set; }
    public string Indexing { get; set; }

    public FactoryIndexerHelper(ICacheNotify<IndexAction> cacheNotify)
    {
        cacheNotify.Subscribe((a) =>
        {
            if (a.LastIndexed != 0)
            {
                LastIndexed = new DateTime(a.LastIndexed);
            }
            Indexing = a.Indexing;
        }, CacheNotifyAction.Any);
    }
}

public interface IFactoryIndexer
{
    string IndexName { get; }
    string SettingsTitle { get; }
    void IndexAll();
    void ReIndex();
}

[Scope]
public class FactoryIndexer<T> : IFactoryIndexer where T : class, ISearchItem
{
    public ILog Logger { get; }
    public string IndexName { get => Indexer.IndexName; }
    public virtual string SettingsTitle => string.Empty;

    protected readonly TenantManager TenantManager;
    protected readonly BaseIndexer<T> Indexer;

    private readonly SearchSettingsHelper _searchSettingsHelper;
    private readonly FactoryIndexer _factoryIndexerCommon;
    private readonly IServiceProvider _serviceProvider;
    private readonly ICache _cache;
    private static readonly TaskScheduler _scheduler
        = new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, 10).ConcurrentScheduler;

    public FactoryIndexer(
        IOptionsMonitor<ILog> options,
        TenantManager tenantManager,
        SearchSettingsHelper searchSettingsHelper,
        FactoryIndexer factoryIndexer,
        BaseIndexer<T> baseIndexer,
        IServiceProvider serviceProvider,
        ICache cache)
    {
        _cache = cache;
        Logger = options.Get("ASC.Indexer");
        TenantManager = tenantManager;
        _searchSettingsHelper = searchSettingsHelper;
        _factoryIndexerCommon = factoryIndexer;
        Indexer = baseIndexer;
        _serviceProvider = serviceProvider;
    }

    public bool TrySelect(Expression<Func<Selector<T>, Selector<T>>> expression, out IReadOnlyCollection<T> result)
    {
        var t = _serviceProvider.GetService<T>();
        if (!Support(t) || !Indexer.CheckExist(t))
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
        var t = _serviceProvider.GetService<T>();
        if (!Support(t) || !Indexer.CheckExist(t))
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
        var t = _serviceProvider.GetService<T>();
        if (!Support(t) || !Indexer.CheckExist(t))
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

    public bool CanIndexByContent(T t)
    {
        return Support(t) && _searchSettingsHelper.CanIndexByContent<T>(TenantManager.GetCurrentTenant().Id);
    }

    public bool Index(T data, bool immediately = true)
    {
        var t = _serviceProvider.GetService<T>();
        if (!Support(t))
        {
            return false;
        }

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

    public void Index(List<T> data, bool immediately = true, int retry = 0)
    {
        var t = _serviceProvider.GetService<T>();
        if (!Support(t) || data.Count == 0)
        {
            return;
        }

        try
        {
            Indexer.Index(data, immediately);
        }
        catch (ElasticsearchClientException e)
        {
            Logger.Error(e);

            if (e.Response != null)
            {
                Logger.Error(e.Response.HttpStatusCode);

                if (e.Response.HttpStatusCode == 413 || e.Response.HttpStatusCode == 403 || e.Response.HttpStatusCode == 408)
                {
                    data.ForEach(r => Index(r, immediately));
                }
                else if (e.Response.HttpStatusCode == 429)
                {
                    Thread.Sleep(60000);
                    if (retry < 5)
                    {
                        Index(data, immediately, retry + 1);
                        return;
                    }

                    throw;
                }
            }
        }
        catch (AggregateException e) //ElasticsearchClientException
        {
            if (e.InnerExceptions.Count == 0)
            {
                throw;
            }

            var inner = e.InnerExceptions.OfType<ElasticsearchClientException>().FirstOrDefault();


            if (inner != null)
            {
                Logger.Error(inner);

                if (inner.Response.HttpStatusCode == 413 || inner.Response.HttpStatusCode == 403)
                {
                    Logger.Error(inner.Response.HttpStatusCode);
                    data.ForEach(r => Index(r, immediately));
                }
                else if (inner.Response.HttpStatusCode == 429)
                {
                    Thread.Sleep(60000);
                    if (retry < 5)
                    {
                        Index(data, immediately, retry + 1);
                        return;
                    }

                    throw;
                }
            }
            else
            {
                throw;
            }
        }
    }

    public Task IndexAsync(List<T> data, bool immediately = true, int retry = 0)
    {
        var t = _serviceProvider.GetService<T>();
        if (!Support(t) || data.Count == 0)
        {
            return Task.CompletedTask;
        }

        return InternalIndexAsync(data, immediately, retry);
    }

    private async Task InternalIndexAsync(List<T> data, bool immediately, int retry)
    {
        try
        {
            await Indexer.IndexAsync(data, immediately).ConfigureAwait(false);
        }
        catch (ElasticsearchClientException e)
        {
            Logger.Error(e);

            if (e.Response != null)
            {
                Logger.Error(e.Response.HttpStatusCode);

                if (e.Response.HttpStatusCode == 413 || e.Response.HttpStatusCode == 403 || e.Response.HttpStatusCode == 408)
                {
                    data.ForEach(r => Index(r, immediately));
                }
                else if (e.Response.HttpStatusCode == 429)
                {
                    await Task.Delay(60000);
                    if (retry < 5)
                    {
                        await IndexAsync(data, immediately, retry + 1);
                        return;
                    }

                    throw;
                }
            }
        }
        catch (AggregateException e) //ElasticsearchClientException
        {
            if (e.InnerExceptions.Count == 0)
            {
                throw;
            }

            var inner = e.InnerExceptions.OfType<ElasticsearchClientException>().FirstOrDefault();


            if (inner != null)
            {
                Logger.Error(inner);

                if (inner.Response.HttpStatusCode == 413 || inner.Response.HttpStatusCode == 403)
                {
                    Logger.Error(inner.Response.HttpStatusCode);
                    data.ForEach(r => Index(r, immediately));
                }
                else if (inner.Response.HttpStatusCode == 429)
                {
                    await Task.Delay(60000);
                    if (retry < 5)
                    {
                        await IndexAsync(data, immediately, retry + 1);
                        return;
                    }

                    throw;
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
        var t = _serviceProvider.GetService<T>();
        if (!Support(t))
        {
            return;
        }

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
        var t = _serviceProvider.GetService<T>();
        if (!Support(t))
        {
            return;
        }

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
        var t = _serviceProvider.GetService<T>();
        if (!Support(t))
        {
            return;
        }

        try
        {
            var tenant = TenantManager.GetCurrentTenant().Id;
            Indexer.Update(data, expression, tenant, immediately, fields);
        }
        catch (Exception e)
        {
            Logger.Error("Update", e);
        }
    }

    public void Update(T data, Expression<Func<Selector<T>, Selector<T>>> expression, UpdateAction action, Expression<Func<T, IList>> fields, bool immediately = true)
    {
        var t = _serviceProvider.GetService<T>();
        if (!Support(t))
        {
            return;
        }

        try
        {
            var tenant = TenantManager.GetCurrentTenant().Id;
            Indexer.Update(data, expression, tenant, action, fields, immediately);
        }
        catch (Exception e)
        {
            Logger.Error("Update", e);
        }
    }

    public void Delete(T data, bool immediately = true)
    {
        var t = _serviceProvider.GetService<T>();
        if (!Support(t))
        {
            return;
        }

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
        var t = _serviceProvider.GetService<T>();
        if (!Support(t))
        {
            return;
        }

        var tenant = TenantManager.GetCurrentTenant().Id;

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
        var t = _serviceProvider.GetService<T>();

        return !Support(t)
            ? Task.FromResult(false)
            : Queue(() => Indexer.Index(data, immediately));
    }

    public Task<bool> UpdateAsync(T data, bool immediately = true, params Expression<Func<T, object>>[] fields)
    {
        var t = _serviceProvider.GetService<T>();

        return !Support(t)
            ? Task.FromResult(false)
            : Queue(() => Indexer.Update(data, immediately, fields));
    }

    public Task<bool> DeleteAsync(T data, bool immediately = true)
    {
        var t = _serviceProvider.GetService<T>();

        return !Support(t)
            ? Task.FromResult(false)
            : Queue(() => Indexer.Delete(data, immediately));
    }

    public async Task<bool> DeleteAsync(Expression<Func<Selector<T>, Selector<T>>> expression, bool immediately = true)
    {
        var t = _serviceProvider.GetService<T>();
        if (!await SupportAsync(t))
        {
            return false;
        }

        var tenant = TenantManager.GetCurrentTenant().Id;

        return await Queue(() => Indexer.Delete(expression, tenant, immediately));
    }

    public void Flush()
    {
        var t = _serviceProvider.GetService<T>();
        if (!Support(t))
        {
            return;
        }

        Indexer.Flush();
    }

    public void Refresh()
    {
        var t = _serviceProvider.GetService<T>();
        if (!Support(t))
        {
            return;
        }

        Indexer.Refresh();
    }

    public virtual void IndexAll()
    {
        return;
    }

    public void ReIndex()
    {
        Indexer.ReIndex();
    }

    public bool Support(T t)
    {
        if (!_factoryIndexerCommon.CheckState())
        {
            return false;
        }

        var cacheTime = DateTime.UtcNow.AddMinutes(15);
        var key = "elasticsearch " + t.IndexName;
        try
        {
            var cacheValue = _cache.Get<string>(key);
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
            _cache.Insert(key, "false", cacheTime);
            Logger.Error("FactoryIndexer CheckState", e);

            return false;
        }
    }

    public Task<bool> SupportAsync(T t)
    {
        return _factoryIndexerCommon.CheckStateAsync();
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
        task.Start(_scheduler);

        return task;
    }
}

[Scope]
public class FactoryIndexer
{
    public ILog Log { get; }

    private readonly ICache _cache;
    private readonly IServiceProvider _serviceProvider;
    private readonly FactoryIndexerHelper _factoryIndexerHelper;
    private readonly Client _client;
    private readonly CoreBaseSettings _coreBaseSettings;

    public FactoryIndexer(
        IServiceProvider serviceProvider,
        FactoryIndexerHelper factoryIndexerHelper,
        Client client,
        IOptionsMonitor<ILog> options,
        CoreBaseSettings coreBaseSettings,
        ICache cache)
    {
        _cache = cache;
        _serviceProvider = serviceProvider;
        _factoryIndexerHelper = factoryIndexerHelper;
        _client = client;
        _coreBaseSettings = coreBaseSettings;

        try
        {
            Log = options.Get("ASC.Indexer");
        }
        catch (Exception e)
        {
            Log.Fatal("FactoryIndexer", e);
        }
    }

    public bool CheckState(bool cacheState = true)
    {
        const string key = "elasticsearch";

        if (cacheState)
        {
            var cacheValue = _cache.Get<string>(key);
            if (!string.IsNullOrEmpty(cacheValue))
            {
                return Convert.ToBoolean(cacheValue);
            }
        }

        var cacheTime = DateTime.UtcNow.AddMinutes(15);

        try
        {
            var isValid = _client.Ping();

            if (cacheState)
            {
                _cache.Insert(key, isValid.ToString(CultureInfo.InvariantCulture).ToLower(), cacheTime);
            }

            return isValid;
        }
        catch (Exception e)
        {
            if (cacheState)
            {
                _cache.Insert(key, "false", cacheTime);
            }

            Log.Error("Ping false", e);

            return false;
        }
    }

    public Task<bool> CheckStateAsync(bool cacheState = true)
    {
        const string key = "elasticsearch";

        if (cacheState)
        {
            var cacheValue = _cache.Get<string>(key);
            if (!string.IsNullOrEmpty(cacheValue))
            {
                return Task.FromResult(Convert.ToBoolean(cacheValue));
            }
        }

        return InternalCheckStateAsync(cacheState, key);    
    }

    private async Task<bool> InternalCheckStateAsync(bool cacheState, string key)
    {
        var cacheTime = DateTime.UtcNow.AddMinutes(15);

        try
        {
            var result = await _client.Instance.PingAsync(new PingRequest());

            var isValid = result.IsValid;

            Log.DebugFormat("CheckState ping {0}", result.DebugInformation);

            if (cacheState)
            {
                _cache.Insert(key, isValid.ToString(CultureInfo.InvariantCulture).ToLower(), cacheTime);
            }

            return isValid;
        }
        catch (Exception e)
        {
            if (cacheState)
            {
                _cache.Insert(key, "false", cacheTime);
            }

            Log.Error("Ping false", e);

            return false;
        }
    }

    public object GetState(TenantUtil tenantUtil)
    {
        State state = null;
        IEnumerable<object> indices = null;
        Dictionary<string, long> count = null;

        if (!_coreBaseSettings.Standalone)
        {
            return new
            {
                state,
                indices,
                status = CheckState()
            };
        }

        state = new State
        {
            Indexing = _factoryIndexerHelper.Indexing,
            LastIndexed = _factoryIndexerHelper.LastIndexed != DateTime.MinValue ? _factoryIndexerHelper.LastIndexed : default(DateTime?)
        };

        if (state.LastIndexed.HasValue)
        {
            state.LastIndexed = tenantUtil.DateTimeFromUtc(state.LastIndexed.Value);
        }

        indices = _client.Instance.Cat.Indices(new CatIndicesRequest { SortByColumns = new[] { "index" } }).Records
            .Select(r => new
            {
                r.Index,
                Count = count.ContainsKey(r.Index) ? count[r.Index] : 0,
                DocsCount = _client.Instance.Count(new CountRequest(r.Index)).Count,
                r.StoreSize
            })
            .Where(r =>
            {
                return r.Count > 0;
            });

        return new
        {
            state,
            indices,
            status = CheckState()
        };
    }

    public void Reindex(string name)
    {
        if (!_coreBaseSettings.Standalone)
        {
            return;
        }

        var generic = typeof(BaseIndexer<>);
        var indexers = _serviceProvider.GetService<IEnumerable<IFactoryIndexer>>()
            .Where(r => string.IsNullOrEmpty(name) || r.IndexName == name)
            .Select(r => (IFactoryIndexer)Activator.CreateInstance(generic.MakeGenericType(r.GetType()), r));

        foreach (var indexer in indexers)
        {
            indexer.ReIndex();
        }
    }
}
