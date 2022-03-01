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

public enum UpdateAction
{
    Add,
    Replace,
    Remove
}

[Singletone]
public class BaseIndexerHelper
{
    public ConcurrentDictionary<string, bool> IsExist { get; set; }

    private readonly ICacheNotify<ClearIndexAction> _notify;

    public BaseIndexerHelper(ICacheNotify<ClearIndexAction> cacheNotify)
    {
        IsExist = new ConcurrentDictionary<string, bool>();
        _notify = cacheNotify;
        _notify.Subscribe((a) =>
        {
            IsExist.AddOrUpdate(a.Id, false, (string q, bool w) => false);
        }, CacheNotifyAction.Any);
    }

    public void Clear<T>(T t) where T : class, ISearchItem
    {
        _notify.Publish(new ClearIndexAction() { Id = t.IndexName }, CacheNotifyAction.Any);
    }
}

[Scope]
public class BaseIndexer<T> where T : class, ISearchItem
{
    public const int QueryLimit = 10000;

    protected internal T Wrapper => _serviceProvider.GetService<T>();
    internal string IndexName => Wrapper.IndexName;
    private WebstudioDbContext WebstudioDbContext => _lazyWebstudioDbContext.Value;

    private bool _isExist;
    private readonly Client _client;
    private readonly ILog _logger;
    private readonly TenantManager _tenantManager;
    private readonly BaseIndexerHelper _baseIndexerHelper;
    private readonly Settings _settings;
    private readonly IServiceProvider _serviceProvider;
    private readonly Lazy<WebstudioDbContext> _lazyWebstudioDbContext;
    private static readonly object _locker = new object();

    public BaseIndexer(
        Client client,
        IOptionsMonitor<ILog> log,
        DbContextManager<WebstudioDbContext> dbContextManager,
        TenantManager tenantManager,
        BaseIndexerHelper baseIndexerHelper,
        Settings settings,
        IServiceProvider serviceProvider)
    {
        _client = client;
        _logger = log.CurrentValue;
        _tenantManager = tenantManager;
        _baseIndexerHelper = baseIndexerHelper;
        _settings = settings;
        _serviceProvider = serviceProvider;
        _lazyWebstudioDbContext = new Lazy<WebstudioDbContext>(() => dbContextManager.Value);
    }

    public IEnumerable<List<T>> IndexAll(
        Func<DateTime, (int, int, int)> getCount,
        Func<DateTime, List<int>> getIds,
        Func<long, long, DateTime, List<T>> getData)
    {
        var now = DateTime.UtcNow;
        var lastIndexed = WebstudioDbContext.WebstudioIndex
            .Where(r => r.IndexName == Wrapper.IndexName)
            .Select(r => r.LastModified)
            .FirstOrDefault();

        if (lastIndexed.Equals(DateTime.MinValue))
        {
            CreateIfNotExist(_serviceProvider.GetService<T>());
        }

        var (count, max, min) = getCount(lastIndexed);
        _logger.Debug($"Index: {IndexName}, Count {count}, Max: {max}, Min: {min}");

        var ids = new List<int>() { min };
        ids.AddRange(getIds(lastIndexed));
        ids.Add(max);

        for (var i = 0; i < ids.Count - 1; i++)
        {
            yield return getData(ids[i], ids[i + 1], lastIndexed);
        }

        WebstudioDbContext.AddOrUpdate(r => r.WebstudioIndex, new DbWebstudioIndex()
        {
            IndexName = Wrapper.IndexName,
            LastModified = now
        });

        WebstudioDbContext.SaveChanges();

        _logger.Debug($"index completed {Wrapper.IndexName}");
    }

    public Task ReIndex()
    {
        Clear();

        return Task.CompletedTask;
        //((IIndexer) this).IndexAll();
    }

    public void CreateIfNotExist(T data)
    {
        try
        {
            if (CheckExist(data))
            {
                return;
            }

            lock (_locker)
            {
                IPromise<IAnalyzers> analyzers(AnalyzersDescriptor b)
                {
                    foreach (var c in Enum.GetNames(typeof(Analyzer)))
                    {
                        var c1 = c;
                        b.Custom(c1 + "custom", ca => ca.Tokenizer(c1).Filters(nameof(Filter.lowercase)).CharFilters(nameof(CharFilter.io)));
                    }

                    foreach (var c in Enum.GetNames(typeof(CharFilter)))
                    {
                        if (c == nameof(CharFilter.io))
                        {
                            continue;
                        }

                        var charFilters = new List<string>() { nameof(CharFilter.io), c };
                        var c1 = c;
                        b.Custom(c1 + "custom", ca => ca.Tokenizer(nameof(Analyzer.whitespace)).Filters(nameof(Filter.lowercase)).CharFilters(charFilters));
                    }

                    if (data is ISearchItemDocument)
                    {
                        b.Custom("document", ca => ca.Tokenizer(Analyzer.whitespace.ToString()).Filters(nameof(Filter.lowercase)).CharFilters(nameof(CharFilter.io)));
                    }

                    return b;
                }

                _client.Instance.Indices.Create(data.IndexName,
                    c =>
                    c.Map<T>(m => m.AutoMap())
                    .Settings(r => r.Analysis(a =>
                                    a.Analyzers(analyzers)
                                    .CharFilters(d => d.HtmlStrip(CharFilter.html.ToString())
                                    .Mapping(CharFilter.io.ToString(), m => m.Mappings("ё => е", "Ё => Е"))))));

                _isExist = true;
            }
        }
        catch (Exception e)
        {
            _logger.Error("CreateIfNotExist", e);
        }
    }

    public void Flush()
    {
        _client.Instance.Indices.Flush(new FlushRequest(IndexName));
    }

    public void Refresh()
    {
        _client.Instance.Indices.Refresh(new RefreshRequest(IndexName));
    }

    internal void Index(T data, bool immediately = true)
    {
        CreateIfNotExist(data);
        _client.Instance.Index(data, idx => GetMeta(idx, data, immediately));
    }

    internal void Index(List<T> data, bool immediately = true)
    {
        if (data.Count == 0)
        {
            return;
        }

        CreateIfNotExist(data[0]);

        if (data[0] is ISearchItemDocument)
        {
            var currentLength = 0L;
            var portion = new List<T>();
            var portionStart = 0;

            for (var i = 0; i < data.Count; i++)
            {
                var t = data[i];
                var runBulk = i == data.Count - 1;

                if (!(t is ISearchItemDocument wwd) || wwd.Document == null || string.IsNullOrEmpty(wwd.Document.Data))
                {
                    portion.Add(t);
                }
                else
                {
                    var dLength = wwd.Document.Data.Length;
                    if (dLength >= _settings.MaxContentLength)
                    {
                        try
                        {
                            Index(t, immediately);
                        }
                        catch (ElasticsearchClientException e)
                        {
                            if (e.Response.HttpStatusCode == 429)
                            {
                                throw;
                            }
                            _logger.Error(e);
                        }
                        catch (Exception e)
                        {
                            _logger.Error(e);
                        }
                        finally
                        {
                            wwd.Document.Data = null;
                            wwd.Document = null;
                            wwd = null;
                            GC.Collect();
                        }

                        continue;
                    }

                    if (currentLength + dLength < _settings.MaxContentLength)
                    {
                        portion.Add(t);
                        currentLength += dLength;
                    }
                    else
                    {
                        runBulk = true;
                        i--;
                    }
                }

                if (runBulk)
                {
                    var portion1 = portion.ToList();
                    _client.Instance.Bulk(r => r.IndexMany(portion1, GetMeta).SourceExcludes("attachments"));
                    for (var j = portionStart; j < i; j++)
                    {
                        if (data[j] is ISearchItemDocument doc && doc.Document != null)
                        {
                            doc.Document.Data = null;
                            doc.Document = null;
                        }
                        doc = null;
                    }

                    portionStart = i;
                    portion = new List<T>();
                    currentLength = 0L;
                    GC.Collect();
                }
            }
        }
        else
        {
            _client.Instance.Bulk(r => r.IndexMany(data, GetMeta));
        }
    }

    internal async Task IndexAsync(List<T> data, bool immediately = true)
    {
        CreateIfNotExist(data[0]);

        if (data is ISearchItemDocument)
        {
            var currentLength = 0L;
            var portion = new List<T>();
            var portionStart = 0;

            for (var i = 0; i < data.Count; i++)
            {
                var t = data[i];
                var runBulk = i == data.Count - 1;

                var wwd = t as ISearchItemDocument;

                if (wwd == null || wwd.Document == null || string.IsNullOrEmpty(wwd.Document.Data))
                {
                    portion.Add(t);
                }
                else
                {
                    var dLength = wwd.Document.Data.Length;
                    if (dLength >= _settings.MaxContentLength)
                    {
                        try
                        {
                            Index(t, immediately);
                        }
                        catch (ElasticsearchClientException e)
                        {
                            if (e.Response.HttpStatusCode == 429)
                            {
                                throw;
                            }
                            _logger.Error(e);
                        }
                        catch (Exception e)
                        {
                            _logger.Error(e);
                        }
                        finally
                        {
                            wwd.Document.Data = null;
                            wwd.Document = null;
                            GC.Collect();
                        }
                        continue;
                    }

                    if (currentLength + dLength < _settings.MaxContentLength)
                    {
                        portion.Add(t);
                        currentLength += dLength;
                    }
                    else
                    {
                        runBulk = true;
                        i--;
                    }
                }

                if (runBulk)
                {
                    var portion1 = portion.ToList();
                    await _client.Instance.BulkAsync(r => r.IndexMany(portion1, GetMeta).SourceExcludes("attachments"));
                    for (var j = portionStart; j < i; j++)
                    {
                        var doc = data[j] as ISearchItemDocument;
                        if (doc != null && doc.Document != null)
                        {
                            doc.Document.Data = null;
                            doc.Document = null;
                        }
                    }

                    portionStart = i;
                    portion = new List<T>();
                    currentLength = 0L;
                    GC.Collect();
                }
            }
        }
        else
        {
            await _client.Instance.BulkAsync(r => r.IndexMany(data, GetMeta));
        }
    }

    internal void Update(T data, bool immediately = true, params Expression<Func<T, object>>[] fields)
    {
        CreateIfNotExist(data);
        _client.Instance.Update(DocumentPath<T>.Id(data), r => GetMetaForUpdate(r, data, immediately, fields));
    }

    internal void Update(T data, UpdateAction action, Expression<Func<T, IList>> fields, bool immediately = true)
    {
        CreateIfNotExist(data);
        _client.Instance.Update(DocumentPath<T>.Id(data), r => GetMetaForUpdate(r, data, action, fields, immediately));
    }

    internal void Update(T data, Expression<Func<Selector<T>, Selector<T>>> expression, int tenantId, bool immediately = true, params Expression<Func<T, object>>[] fields)
    {
        CreateIfNotExist(data);
        _client.Instance.UpdateByQuery(GetDescriptorForUpdate(data, expression, tenantId, immediately, fields));
    }

    internal void Update(T data, Expression<Func<Selector<T>, Selector<T>>> expression, int tenantId, UpdateAction action, Expression<Func<T, IList>> fields, bool immediately = true)
    {
        CreateIfNotExist(data);
        _client.Instance.UpdateByQuery(GetDescriptorForUpdate(data, expression, tenantId, action, fields, immediately));
    }

    internal void Delete(T data, bool immediately = true)
    {
        _client.Instance.Delete<T>(data, r => GetMetaForDelete(r, immediately));
    }

    internal void Delete(Expression<Func<Selector<T>, Selector<T>>> expression, int tenantId, bool immediately = true)
    {
        _client.Instance.DeleteByQuery(GetDescriptorForDelete(expression, tenantId, immediately));
    }

    internal bool CheckExist(T data)
    {
        try
        {
            var isExist = _baseIndexerHelper.IsExist.GetOrAdd(data.IndexName, (k) => _client.Instance.Indices.Exists(k).Exists);
            if (isExist)
            {
                return true;
            }

            lock (_locker)
            {
                isExist = _client.Instance.Indices.Exists(data.IndexName).Exists;

                _baseIndexerHelper.IsExist.TryUpdate(data.IndexName, _isExist, false);

                if (isExist)
                {
                    return true;
                }
            }
        }
        catch (Exception e)
        {
            _logger.Error("CheckExist " + data.IndexName, e);
        }

        return false;
    }

    internal IReadOnlyCollection<T> Select(Expression<Func<Selector<T>, Selector<T>>> expression, bool onlyId = false)
    {
        var func = expression.Compile();
        var selector = new Selector<T>(_serviceProvider);
        var descriptor = func(selector).Where(r => r.TenantId, _tenantManager.GetCurrentTenant().Id);

        return _client.Instance.Search(descriptor.GetDescriptor(this, onlyId)).Documents;
    }

    internal IReadOnlyCollection<T> Select(Expression<Func<Selector<T>, Selector<T>>> expression, bool onlyId, out long total)
    {
        var func = expression.Compile();
        var selector = new Selector<T>(_serviceProvider);
        var descriptor = func(selector).Where(r => r.TenantId, _tenantManager.GetCurrentTenant().Id);
        var result = _client.Instance.Search(descriptor.GetDescriptor(this, onlyId));
        total = result.Total;

        return result.Documents;
    }

    private void Clear()
    {
        var index = WebstudioDbContext.WebstudioIndex.Where(r => r.IndexName == Wrapper.IndexName).FirstOrDefault();

        if (index != null)
        {
            WebstudioDbContext.WebstudioIndex.Remove(index);
        }

        WebstudioDbContext.SaveChanges();

        _logger.DebugFormat("Delete {0}", Wrapper.IndexName);
        _client.Instance.Indices.Delete(Wrapper.IndexName);
        _baseIndexerHelper.Clear(Wrapper);
        CreateIfNotExist(Wrapper);
    }

    private IIndexRequest<T> GetMeta(IndexDescriptor<T> request, T data, bool immediately = true)
    {
        var result = request.Index(data.IndexName).Id(data.Id);

        if (immediately)
        {
            result.Refresh(Elasticsearch.Net.Refresh.True);
        }

        if (data is ISearchItemDocument)
        {
            result.Pipeline("attachments");
        }

        return result;
    }
    private IBulkIndexOperation<T> GetMeta(BulkIndexDescriptor<T> desc, T data)
    {
        var result = desc.Index(IndexName).Id(data.Id);

        if (data is ISearchItemDocument)
        {
            result.Pipeline("attachments");
        }

        return result;
    }

    private IUpdateRequest<T, T> GetMetaForUpdate(UpdateDescriptor<T, T> request, T data, bool immediately = true, params Expression<Func<T, object>>[] fields)
    {
        var result = request.Index(IndexName);

        if (fields.Length > 0)
        {
            result.Script(GetScriptUpdateByQuery(data, fields));
        }
        else
        {
            result.Doc(data);
        }

        if (immediately)
        {
            result.Refresh(Elasticsearch.Net.Refresh.True);
        }

        return result;
    }

    private Func<ScriptDescriptor, IScript> GetScriptUpdateByQuery(T data, params Expression<Func<T, object>>[] fields)
    {
        var source = new StringBuilder();
        var parameters = new Dictionary<string, object>();

        for (var i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            var func = field.Compile();
            var newValue = func(data);
            string name;

            var expression = field.Body;
            var isList = expression.Type.IsGenericType && expression.Type.GetGenericTypeDefinition() == typeof(List<>);


            var sourceExprText = "";

            while (!string.IsNullOrEmpty(name = TryGetName(expression, out var member)))
            {
                sourceExprText = "." + name + sourceExprText;
            }

            if (isList)
            {
                UpdateByAction(UpdateAction.Add, (IList)newValue, sourceExprText, parameters, source);
            }
            else
            {
                if (newValue == default(T))
                {
                    source.Append($"ctx._source.remove('{sourceExprText.Substring(1)}');");
                }
                else
                {
                    var pkey = "p" + sourceExprText.Replace(".", "");
                    source.Append($"ctx._source{sourceExprText} = params.{pkey};");
                    parameters.Add(pkey, newValue);
                }
            }
        }

        var sourceData = source.ToString();

        return r => r.Source(sourceData).Params(parameters);
    }

    private IUpdateRequest<T, T> GetMetaForUpdate(UpdateDescriptor<T, T> request, T data, UpdateAction action, Expression<Func<T, IList>> fields, bool immediately = true)
    {
        var result = request.Index(IndexName).Script(GetScriptForUpdate(data, action, fields));

        if (immediately)
        {
            result.Refresh(Elasticsearch.Net.Refresh.True);
        }

        return result;
    }

    private Func<ScriptDescriptor, IScript> GetScriptForUpdate(T data, UpdateAction action, Expression<Func<T, IList>> fields)
    {
        var source = new StringBuilder();

        var func = fields.Compile();
        var newValue = func(data);
        string name;

        var expression = fields.Body;

        var sourceExprText = "";

        while (!string.IsNullOrEmpty(name = TryGetName(expression, out var member)))
        {
            sourceExprText = "." + name + sourceExprText;
            expression = member.Expression;
        }

        var parameters = new Dictionary<string, object>();

        UpdateByAction(action, newValue, sourceExprText, parameters, source);

        return r => r.Source(source.ToString()).Params(parameters);
    }

    private void UpdateByAction(UpdateAction action, IList newValue, string key, Dictionary<string, object> parameters, StringBuilder source)
    {
        var paramKey = "p" + key.Replace(".", "");
        switch (action)
        {
            case UpdateAction.Add:
                for (var i = 0; i < newValue.Count; i++)
                {
                    parameters.Add(paramKey + i, newValue[i]);
                    source.Append($"if (!ctx._source{key}.contains(params.{paramKey + i})){{ctx._source{key}.add(params.{paramKey + i})}}");
                }
                break;
            case UpdateAction.Replace:
                parameters.Add(paramKey, newValue);
                source.Append($"ctx._source{key} = params.{paramKey};");
                break;
            case UpdateAction.Remove:
                for (var i = 0; i < newValue.Count; i++)
                {
                    parameters.Add(paramKey + i, newValue[i]);
                    source.Append($"ctx._source{key}.removeIf(item -> item.id == params.{paramKey + i}.id)");
                }
                break;
            default:
                throw new ArgumentOutOfRangeException("action", action, null);
        }
    }

    private string TryGetName(Expression expr, out MemberExpression member)
    {
        member = expr as MemberExpression;
        if (member == null)
        {
            if (expr is UnaryExpression unary)
            {
                member = unary.Operand as MemberExpression;
            }
        }

        return member == null ? "" : member.Member.Name.ToLowerCamelCase();
    }

    private IDeleteRequest GetMetaForDelete(DeleteDescriptor<T> request, bool immediately = true)
    {
        var result = request.Index(IndexName);
        if (immediately)
        {
            result.Refresh(Elasticsearch.Net.Refresh.True);
        }

        return result;
    }

    private Func<DeleteByQueryDescriptor<T>, IDeleteByQueryRequest> GetDescriptorForDelete(Expression<Func<Selector<T>, Selector<T>>> expression, int tenantId, bool immediately = true)
    {
        var func = expression.Compile();
        var selector = new Selector<T>(_serviceProvider);
        var descriptor = func(selector).Where(r => r.TenantId, tenantId);

        return descriptor.GetDescriptorForDelete(this, immediately);
    }

    private Func<UpdateByQueryDescriptor<T>, IUpdateByQueryRequest> GetDescriptorForUpdate(T data, Expression<Func<Selector<T>, Selector<T>>> expression, int tenantId, bool immediately = true, params Expression<Func<T, object>>[] fields)
    {
        var func = expression.Compile();
        var selector = new Selector<T>(_serviceProvider);
        var descriptor = func(selector).Where(r => r.TenantId, tenantId);

        return descriptor.GetDescriptorForUpdate(this, GetScriptUpdateByQuery(data, fields), immediately);
    }

    private Func<UpdateByQueryDescriptor<T>, IUpdateByQueryRequest> GetDescriptorForUpdate(T data, Expression<Func<Selector<T>, Selector<T>>> expression, int tenantId, UpdateAction action, Expression<Func<T, IList>> fields, bool immediately = true)
    {
        var func = expression.Compile();
        var selector = new Selector<T>(_serviceProvider);
        var descriptor = func(selector).Where(r => r.TenantId, tenantId);

        return descriptor.GetDescriptorForUpdate(this, GetScriptForUpdate(data, action, fields), immediately);
    }
}

static class CamelCaseExtension
{
    internal static string ToLowerCamelCase(this string str)
    {
        return str.ToLowerInvariant()[0] + str.Substring(1);
    }
}
