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
    public sealed class FactoryIndexerTask : FactoryIndexer<DbTask>
    {
        public FactoryIndexerTask(
            IOptionsMonitor<ILog> options,
            TenantManager tenantManager,
            SearchSettingsHelper searchSettingsHelper,
            FactoryIndexer factoryIndexer,
            BaseIndexer<DbTask> baseIndexer,
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
            var taskDao = DaoFactory.GetTaskDao();

            (int, int, int) getCount(DateTime lastIndexed)
            {
                var q = taskDao.CRMDbContext.Tasks
                        .Where(r => r.LastModifedOn >= lastIndexed);

                var count = q.GroupBy(a => a.Id).Count();
                var min = count > 0 ? q.Min(r => r.Id) : 0;
                var max = count > 0 ? q.Max(r => r.Id) : 0;

                return (count, max, min);
            }

            List<DbTask> getData(long i, long step, DateTime lastIndexed) =>
                    taskDao.CRMDbContext.Tasks
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