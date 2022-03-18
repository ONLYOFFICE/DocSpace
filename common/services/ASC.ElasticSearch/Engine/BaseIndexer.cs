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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.EF.Model;
using ASC.ElasticSearch.Service;

using Autofac;

using Elasticsearch.Net;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Nest;

namespace ASC.ElasticSearch
{
    [Singletone]
    public class BaseIndexerHelper
    {
        public ConcurrentDictionary<string, bool> IsExist { get; set; }
        private readonly ICacheNotify<ClearIndexAction> Notify;

        public BaseIndexerHelper(ICacheNotify<ClearIndexAction> cacheNotify)
        {
            IsExist = new ConcurrentDictionary<string, bool>();
            Notify = cacheNotify;
            Notify.Subscribe((a) =>
            {
                IsExist.AddOrUpdate(a.Id, false, (q, w) => false);
            }, CacheNotifyAction.Any);
        }

        public void Clear<T>(T t) where T : class, ISearchItem
        {
            Notify.Publish(new ClearIndexAction() { Id = t.IndexName }, CacheNotifyAction.Any);
        }
    }

    [Scope]
    public class BaseIndexer<T> where T : class, ISearchItem
    {
        private static readonly object Locker = new object();

        protected internal T Wrapper { get { return ServiceProvider.GetService<T>(); } }

        internal string IndexName { get { return Wrapper.IndexName; } }

        public const int QueryLimit = 10000;

        private bool IsExist { get; set; }
        private Client Client { get; }
        private ILog Log { get; }
        protected TenantManager TenantManager { get; }
        private BaseIndexerHelper BaseIndexerHelper { get; }
        private Settings Settings { get; }
        private IServiceProvider ServiceProvider { get; }
        private Lazy<WebstudioDbContext> LazyWebstudioDbContext { get; }
        private WebstudioDbContext WebstudioDbContext { get => LazyWebstudioDbContext.Value; }

        public BaseIndexer(
            Client client,
            IOptionsMonitor<ILog> log,
            DbContextManager<WebstudioDbContext> dbContextManager,
            TenantManager tenantManager,
            BaseIndexerHelper baseIndexerHelper,
            Settings settings,
            IServiceProvider serviceProvider)
        {
            Client = client;
            Log = log.CurrentValue;
            TenantManager = tenantManager;
            BaseIndexerHelper = baseIndexerHelper;
            Settings = settings;
            ServiceProvider = serviceProvider;
            LazyWebstudioDbContext = new Lazy<WebstudioDbContext>(() => dbContextManager.Value);
        }

        internal void Index(T data, bool immediately = true)
        {
            if (!BeforeIndex(data)) return;

            Client.Instance.Index(data, idx => GetMeta(idx, data, immediately));
        }

        internal void Index(List<T> data, bool immediately = true)
        {
            if (data.Count == 0) return;

            if (!CheckExist(data[0])) return;

            if (data[0] is ISearchItemDocument)
            {
                var currentLength = 0L;
                var portion = new List<T>();
                var portionStart = 0;

                for (var i = 0; i < data.Count; i++)
                {
                    var t = data[i];
                    var runBulk = i == data.Count - 1;

                    BeforeIndex(t);

                    if (!(t is ISearchItemDocument wwd) || wwd.Document == null || string.IsNullOrEmpty(wwd.Document.Data))
                    {
                        portion.Add(t);
                    }
                    else
                    {
                        var dLength = wwd.Document.Data.Length;
                        if (dLength >= Settings.MaxContentLength)
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
                                Log.Error(e);
                            }
                            catch (Exception e)
                            {
                                Log.Error(e);
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

                        if (currentLength + dLength < Settings.MaxContentLength)
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
                        Client.Instance.Bulk(r => r.IndexMany(portion1, GetMeta).SourceExcludes("attachments"));
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
                    BeforeIndex(item);
                }

                Client.Instance.Bulk(r => r.IndexMany(data, GetMeta));
            }
        }

        internal async Task IndexAsync(List<T> data, bool immediately = true)
        {
            if (!CheckExist(data[0])) return;

            if (data is ISearchItemDocument)
            {
                var currentLength = 0L;
                var portion = new List<T>();
                var portionStart = 0;

                for (var i = 0; i < data.Count; i++)
                {
                    var t = data[i];
                    var runBulk = i == data.Count - 1;

                    await BeforeIndexAsync(t);

                    var wwd = t as ISearchItemDocument;

                    if (wwd == null || wwd.Document == null || string.IsNullOrEmpty(wwd.Document.Data))
                    {
                        portion.Add(t);
                    }
                    else
                    {
                        var dLength = wwd.Document.Data.Length;
                        if (dLength >= Settings.MaxContentLength)
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
                                Log.Error(e);
                            }
                            catch (Exception e)
                            {
                                Log.Error(e);
                            }
                            finally
                            {
                                wwd.Document.Data = null;
                                wwd.Document = null;
                                GC.Collect();
                            }
                            continue;
                        }

                        if (currentLength + dLength < Settings.MaxContentLength)
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
                        await Client.Instance.BulkAsync(r => r.IndexMany(portion1, GetMeta).SourceExcludes("attachments"));
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
                foreach (var item in data)
                {
                    await BeforeIndexAsync(item);
                }

                await Client.Instance.BulkAsync(r => r.IndexMany(data, GetMeta));
            }
        }

        internal void Update(T data, bool immediately = true, params Expression<Func<T, object>>[] fields)
        {
            if (!CheckExist(data)) return;
            Client.Instance.Update(DocumentPath<T>.Id(data), r => GetMetaForUpdate(r, data, immediately, fields));
        }

        internal void Update(T data, UpdateAction action, Expression<Func<T, IList>> fields, bool immediately = true)
        {
            if (!CheckExist(data)) return;
            Client.Instance.Update(DocumentPath<T>.Id(data), r => GetMetaForUpdate(r, data, action, fields, immediately));
        }

        internal void Update(T data, Expression<Func<Selector<T>, Selector<T>>> expression, int tenantId, bool immediately = true, params Expression<Func<T, object>>[] fields)
        {
            if (!CheckExist(data)) return;
            Client.Instance.UpdateByQuery(GetDescriptorForUpdate(data, expression, tenantId, immediately, fields));
        }

        internal void Update(T data, Expression<Func<Selector<T>, Selector<T>>> expression, int tenantId, UpdateAction action, Expression<Func<T, IList>> fields, bool immediately = true)
        {
            if (!CheckExist(data)) return;
            Client.Instance.UpdateByQuery(GetDescriptorForUpdate(data, expression, tenantId, action, fields, immediately));
        }

        internal void Delete(T data, bool immediately = true)
        {
            Client.Instance.Delete<T>(data, r => GetMetaForDelete(r, immediately));
        }

        internal void Delete(Expression<Func<Selector<T>, Selector<T>>> expression, int tenantId, bool immediately = true)
        {
            Client.Instance.DeleteByQuery(GetDescriptorForDelete(expression, tenantId, immediately));
        }

        public void Flush()
        {
            Client.Instance.Indices.Flush(new FlushRequest(IndexName));
        }

        public void Refresh()
        {
            Client.Instance.Indices.Refresh(new RefreshRequest(IndexName));
        }

        internal bool CheckExist(T data)
        {
            try
            {
                var isExist = BaseIndexerHelper.IsExist.GetOrAdd(data.IndexName, (k) => Client.Instance.Indices.Exists(k).Exists);
                if (isExist) return true;

                lock (Locker)
                {
                    isExist = Client.Instance.Indices.Exists(data.IndexName).Exists;

                    BaseIndexerHelper.IsExist.TryUpdate(data.IndexName, IsExist, false);

                    if (isExist) return true;
                }
            }
            catch (Exception e)
            {
                Log.Error("CheckExist " + data.IndexName, e);
            }
            return false;
        }

        public Task ReIndex()
        {
            Clear();
            return Task.CompletedTask;
            //((IIndexer) this).IndexAll();
        }

        private void Clear()
        {
            var index = WebstudioDbContext.WebstudioIndex.Where(r => r.IndexName == Wrapper.IndexName).FirstOrDefault();

            if (index != null)
            {
                WebstudioDbContext.WebstudioIndex.Remove(index);
            }

            WebstudioDbContext.SaveChanges();

            Log.DebugFormat("Delete {0}", Wrapper.IndexName);
            Client.Instance.Indices.Delete(Wrapper.IndexName);
            BaseIndexerHelper.Clear(Wrapper);
        }

        internal IReadOnlyCollection<T> Select(Expression<Func<Selector<T>, Selector<T>>> expression, bool onlyId = false)
        {
            var func = expression.Compile();
            var selector = new Selector<T>(ServiceProvider);
            var descriptor = func(selector).Where(r => r.TenantId, TenantManager.GetCurrentTenant().TenantId);
            return Client.Instance.Search(descriptor.GetDescriptor(this, onlyId)).Documents;
        }

        internal IReadOnlyCollection<T> Select(Expression<Func<Selector<T>, Selector<T>>> expression, bool onlyId, out long total)
        {
            var func = expression.Compile();
            var selector = new Selector<T>(ServiceProvider);
            var descriptor = func(selector).Where(r => r.TenantId, TenantManager.GetCurrentTenant().TenantId);
            var result = Client.Instance.Search(descriptor.GetDescriptor(this, onlyId));
            total = result.Total;
            return result.Documents;
        }

        protected virtual bool BeforeIndex(T data)
        {
            return CheckExist(data);
        }

        protected virtual Task<bool> BeforeIndexAsync(T data)
        {
            return Task.FromResult(CheckExist(data));
        }

        public void CreateIfNotExist(T data)
        {
            try
            {
                if (CheckExist(data)) return;

                lock (Locker)
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
                            if (c == nameof(CharFilter.io)) continue;

                            var charFilters = new List<string>() { nameof(CharFilter.io), c };
                            var c1 = c;
                            b.Custom(c1 + "custom", ca => ca.Tokenizer(nameof(Analyzer.whitespace)).Filters(nameof(Filter.lowercase)).CharFilters(charFilters));
                        }

                        if (data is ISearchItemDocument)
                        {
                            b.Custom("document", ca => ca.Tokenizer(nameof(Analyzer.whitespace)).Filters(nameof(Filter.lowercase)).CharFilters(nameof(CharFilter.io)));
                        }

                        return b;
                    }

                    Client.Instance.Indices.Create(data.IndexName,
                       c =>
                       c.Map<T>(m => m.AutoMap())
                       .Settings(r => r.Analysis(a =>
                                       a.Analyzers(analyzers)
                                       .CharFilters(d => d.HtmlStrip(CharFilter.html.ToString())
                                       .Mapping(CharFilter.io.ToString(), m => m.Mappings("ё => е", "Ё => Е"))))));

                    IsExist = true;
                }
            }
            catch (Exception e)
            {
                Log.Error("CreateIfNotExist", e);
            }
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
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
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
            var selector = new Selector<T>(ServiceProvider);
            var descriptor = func(selector).Where(r => r.TenantId, tenantId);
            return descriptor.GetDescriptorForDelete(this, immediately);
        }

        private Func<UpdateByQueryDescriptor<T>, IUpdateByQueryRequest> GetDescriptorForUpdate(T data, Expression<Func<Selector<T>, Selector<T>>> expression, int tenantId, bool immediately = true, params Expression<Func<T, object>>[] fields)
        {
            var func = expression.Compile();
            var selector = new Selector<T>(ServiceProvider);
            var descriptor = func(selector).Where(r => r.TenantId, tenantId);
            return descriptor.GetDescriptorForUpdate(this, GetScriptUpdateByQuery(data, fields), immediately);
        }

        private Func<UpdateByQueryDescriptor<T>, IUpdateByQueryRequest> GetDescriptorForUpdate(T data, Expression<Func<Selector<T>, Selector<T>>> expression, int tenantId, UpdateAction action, Expression<Func<T, IList>> fields, bool immediately = true)
        {
            var func = expression.Compile();
            var selector = new Selector<T>(ServiceProvider);
            var descriptor = func(selector).Where(r => r.TenantId, tenantId);
            return descriptor.GetDescriptorForUpdate(this, GetScriptForUpdate(data, action, fields), immediately);
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
                CreateIfNotExist(ServiceProvider.GetService<T>());
            }

            var (count, max, min) = getCount(lastIndexed);
            Log.Debug($"Index: {IndexName}, Count {count}, Max: {max}, Min: {min}");

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

            Log.Debug($"index completed {Wrapper.IndexName}");
        }
    }

    static class CamelCaseExtension
    {
        internal static string ToLowerCamelCase(this string str)
        {
            return str.ToLowerInvariant()[0] + str.Substring(1);
        }
    }


    public enum UpdateAction
    {
        Add,
        Replace,
        Remove
    }
}