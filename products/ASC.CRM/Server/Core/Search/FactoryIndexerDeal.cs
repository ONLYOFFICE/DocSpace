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
            var dealDao = DaoFactory.GetDealDao();

            (int, int, int) getCount(DateTime lastIndexed)
            {
                var q = dealDao.CrmDbContext.ContactsInfo
                        .Where(r => r.LastModifedOn >= lastIndexed);

                var count = q.GroupBy(a => a.Id).Count();
                var min = count > 0 ? q.Min(r => r.Id) : 0;
                var max = count > 0 ? q.Max(r => r.Id) : 0;

                return (count, max, min);
            }

            List<DbDeal> getData(long i, long step, DateTime lastIndexed) =>
                    dealDao.CrmDbContext.Deals
                    .Where(r => r.LastModifedOn >= lastIndexed)
                    .Where(r => r.Id >= i && r.Id <= i + step)
                    .Select(r => r)
                    .ToList();

            try
            {
                foreach (var data in Indexer.IndexAll(getCount, getData))
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