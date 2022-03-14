using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.EF;
using ASC.ElasticSearch;
using ASC.ElasticSearch.Core;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ASC.Web.CRM.Core.Search
{
    [Scope]
    public sealed class FactoryIndexerDeal : FactoryIndexer<DbDeal>
    {
        public FactoryIndexerDeal(
            IOptionsMonitor<ILog> options,
            TenantManager tenantManager,
            SearchSettingsHelper searchSettingsHelper,
            FactoryIndexer factoryIndexer,
            BaseIndexer<DbDeal> baseIndexer,
            IServiceProvider serviceProvider,
            DaoFactory daoFactory,
            ICache cache)
            : base(options, tenantManager, searchSettingsHelper, factoryIndexer, baseIndexer, serviceProvider, cache)
        {
            DaoFactory = daoFactory;
        }

        public DaoFactory DaoFactory { get; }

        public override void IndexAll()
        {
            var entityDao = DaoFactory.GetDealDao();

            IQueryable<DbDeal> GetBaseQuery(DateTime lastIndexed) =>
                                entityDao.CrmDbContext.Deals
                                        .AsQueryable()
                                        .Where(r => r.LastModifedOn >= lastIndexed)
                                        .Join(entityDao.CrmDbContext.Tenants, r => r.TenantId, r => r.Id, (f, t) => new { DbEntity = f, DbTenant = t })
                                        .Where(r => r.DbTenant.Status == ASC.Core.Tenants.TenantStatus.Active)
                                        .Select(r => r.DbEntity);

            (int, int, int) getCount(DateTime lastIndexed)
            {
                var q = GetBaseQuery(lastIndexed);

                var count = q.Count();
                var min = count > 0 ? q.Min(r => r.Id) : 0;
                var max = count > 0 ? q.Max(r => r.Id) : 0;

                return (count, max, min);
            }

            List<DbDeal> getData(long start, long stop, DateTime lastIndexed) =>
                    GetBaseQuery(lastIndexed)
                    .Where(r => r.Id >= start && r.Id <= stop)
                    .ToList();

            List<int> getIds(DateTime lastIndexed)
            {
                long start = 0;

                var result = new List<int>();

                while (true)
                {
                    var id = GetBaseQuery(lastIndexed)
                                .AsNoTracking()
                                .Where(r => r.Id >= start)
                                .OrderBy(x => x.Id)
                                .Skip(BaseIndexer<DbDeal>.QueryLimit)
                                .Select(x => x.Id)
                                .FirstOrDefault();

                    if (id != 0)
                    {
                        start = id;
                        result.Add(id);
                    }
                    else
                    {
                        break;
                    }
                }

                return result;
            }
            try
            {
                foreach (var data in Indexer.IndexAll(getCount, getIds, getData))
                {
                    Index(data);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
        }
    }
}