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

using Microsoft.EntityFrameworkCore;

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

    private bool _isExist;
    private readonly Client _client;
    private readonly ILogger _logger;
    private readonly IDbContextFactory<WebstudioDbContext> _dbContextFactory;
    protected readonly TenantManager _tenantManager;
    private readonly BaseIndexerHelper _baseIndexerHelper;
    private readonly Settings _settings;
    private readonly IServiceProvider _serviceProvider;
    private static readonly object _locker = new object();

    public BaseIndexer(
        Client client,
        ILogger<BaseIndexer<T>> logger,
        IDbContextFactory<WebstudioDbContext> dbContextFactory,
        TenantManager tenantManager,
        BaseIndexerHelper baseIndexerHelper,
        Settings settings,
        IServiceProvider serviceProvider)
    {
        _client = client;
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _tenantManager = tenantManager;
        _baseIndexerHelper = baseIndexerHelper;
        _settings = settings;
        _serviceProvider = serviceProvider;
    }

    public async Task<IEnumerable<List<T>>> IndexAllAsync(
        Func<DateTime, (int, int, int)> getCount,
        Func<DateTime, List<int>> getIds,
        Func<long, long, DateTime, List<T>> getData)
    {
        await using var webstudioDbContext = _dbContextFactory.CreateDbContext();
        var now = DateTime.UtcNow;
        var lastIndexed = await Queries.LastIndexedAsync(webstudioDbContext, Wrapper.IndexName);

        if (lastIndexed.Equals(DateTime.MinValue))
        {
            CreateIfNotExist(_serviceProvider.GetService<T>());
        }

        var (count, max, min) = getCount(lastIndexed);
        _logger.DebugIndex(IndexName, count, max, min);

        var ids = new List<int>() { min };
        ids.AddRange(getIds(lastIndexed));
        ids.Add(max);

        await webstudioDbContext.AddOrUpdateAsync(q => q.WebstudioIndex, new DbWebstudioIndex()
        {
            IndexName = Wrapper.IndexName,
            LastModified = now
        });

        await webstudioDbContext.SaveChangesAsync();

        _logger.DebugIndexCompleted(Wrapper.IndexName);

        var list = new List<List<T>>();
        for (var i = 0; i < ids.Count - 1; i++)
        {
            list.Add(getData(ids[i], ids[i + 1], lastIndexed));
        }
        return list;
    }

    public async Task ReIndrexAsync()
    {
        await ClearAsync();
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
                    foreach (var c in AnalyzerExtensions.GetNames())
                    {
                        var c1 = c;
                        b.Custom(c1 + "custom", ca => ca.Tokenizer(c1).Filters(nameof(Filter.lowercase)).CharFilters(nameof(CharFilter.io)));
                    }

                    foreach (var c in CharFilterExtensions.GetNames())
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
            _logger.ErrorCreateIfNotExist(e);
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

    internal async Task IndexAsync(T data, bool immediately = true)
    {
        if (!(await BeforeIndex(data)))
        {
            return;
        }

        await _client.Instance.IndexAsync(data, idx => GetMeta(idx, data, immediately));
    }

    internal async Task IndexAsync(List<T> data, bool immediately = true)
    {
        if (data.Count == 0)
        {
            return;
        }

        if (!CheckExist(data[0]))
        {
            return;
        }

        if (data[0] is ISearchItemDocument)
        {
            var currentLength = 0L;
            var portion = new List<T>();
            var portionStart = 0;

            for (var i = 0; i < data.Count; i++)
            {
                var t = data[i];
                var runBulk = i == data.Count - 1;

                await BeforeIndex(t);

                if (t is not ISearchItemDocument wwd || wwd.Document == null || string.IsNullOrEmpty(wwd.Document.Data))
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
                            await IndexAsync(t, immediately);
                        }
                        catch (ElasticsearchClientException e)
                        {
                            if (e.Response.HttpStatusCode == 429)
                            {
                                throw;
                            }
                            _logger.ErrorIndex(e);
                        }
                        catch (Exception e)
                        {
                            _logger.ErrorIndex(e);
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
            foreach (var item in data)
            {
                await BeforeIndex(item);
            }

            _client.Instance.Bulk(r => r.IndexMany(data, GetMeta));
        }
    }

    internal void Update(T data, bool immediately = true, params Expression<Func<T, object>>[] fields)
    {
        if (!CheckExist(data))
        {
            return;
        }

        _client.Instance.Update(DocumentPath<T>.Id(data), r => GetMetaForUpdate(r, data, immediately, fields));
    }

    internal void Update(T data, UpdateAction action, Expression<Func<T, IList>> fields, bool immediately = true)
    {
        if (!CheckExist(data))
        {
            return;
        }

        _client.Instance.Update(DocumentPath<T>.Id(data), r => GetMetaForUpdate(r, data, action, fields, immediately));
    }

    internal void Update(T data, Expression<Func<Selector<T>, Selector<T>>> expression, int tenantId, bool immediately = true, params Expression<Func<T, object>>[] fields)
    {
        if (!CheckExist(data))
        {
            return;
        }

        _client.Instance.UpdateByQuery(GetDescriptorForUpdate(data, expression, tenantId, immediately, fields));
    }

    internal void Update(T data, Expression<Func<Selector<T>, Selector<T>>> expression, int tenantId, UpdateAction action, Expression<Func<T, IList>> fields, bool immediately = true)
    {
        if (!CheckExist(data))
        {
            return;
        }

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
            _logger.ErrorCheckExist(data.IndexName, e);
        }

        return false;
    }

    internal async Task<IReadOnlyCollection<T>> SelectAsync(Expression<Func<Selector<T>, Selector<T>>> expression, bool onlyId = false)
    {
        var func = expression.Compile();
        var selector = new Selector<T>(_serviceProvider);
        var tenant = await _tenantManager.GetCurrentTenantAsync();
        var descriptor = func(selector).Where(r => r.TenantId, tenant.Id);

        return _client.Instance.Search(descriptor.GetDescriptor(this, onlyId)).Documents;
    }

    internal async Task<(IReadOnlyCollection<T>, long)> SelectWithTotalAsync(Expression<Func<Selector<T>, Selector<T>>> expression, bool onlyId)
    {
        var func = expression.Compile();
        var selector = new Selector<T>(_serviceProvider);
        var tenant = await _tenantManager.GetCurrentTenantAsync();
        var descriptor = func(selector).Where(r => r.TenantId, tenant.Id);
        var result = _client.Instance.Search(descriptor.GetDescriptor(this, onlyId));
        var total = result.Total;

        return (result.Documents, total);
    }

    protected virtual Task<bool> BeforeIndex(T data)
    {
        return Task.FromResult(CheckExist(data));
    }

    protected virtual Task<bool> BeforeIndexAsync(T data)
    {
        return Task.FromResult(CheckExist(data));
    }

    private async Task ClearAsync()
    {
        await using var webstudioDbContext = _dbContextFactory.CreateDbContext();
        var index = await Queries.IndexAsync(webstudioDbContext, Wrapper.IndexName);

        if (index != null)
        {
            webstudioDbContext.WebstudioIndex.Remove(index);
            await webstudioDbContext.SaveChangesAsync();
        }

        _logger.DebugIndexDeleted(Wrapper.IndexName);
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

        if (data is ISearchItemDocument doc && doc.Document != null)
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
                expression = member.Expression;
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


static file class Queries
{
    public static readonly Func<WebstudioDbContext, string, Task<DateTime>> LastIndexedAsync =
        EF.CompileAsyncQuery(
            (WebstudioDbContext ctx, string indexName) =>
                ctx.WebstudioIndex
                    .Where(r => r.IndexName == indexName)
                    .Select(r => r.LastModified)
                    .FirstOrDefault());

    public static readonly Func<WebstudioDbContext, string, Task<DbWebstudioIndex>> IndexAsync =
        EF.CompileAsyncQuery(
            (WebstudioDbContext ctx, string indexName) =>
                ctx.WebstudioIndex
                    .FirstOrDefault(r => r.IndexName == indexName));
}