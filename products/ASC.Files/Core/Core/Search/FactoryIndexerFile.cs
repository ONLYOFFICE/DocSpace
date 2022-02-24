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
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF.Model;
using ASC.ElasticSearch;
using ASC.ElasticSearch.Core;
using ASC.ElasticSearch.Service;
using ASC.Files.Core;
using ASC.Files.Core.Data;
using ASC.Files.Core.EF;
using ASC.Files.Core.Resources;

using Microsoft.Extensions.Options;
namespace ASC.Web.Files.Core.Search
{
    [Scope(Additional = typeof(FactoryIndexerFileExtension))]
    public class FactoryIndexerFile : FactoryIndexer<DbFile>
    {
        private IDaoFactory DaoFactory { get; }
        private Settings Settings { get; }

        public FactoryIndexerFile(
            IOptionsMonitor<ILog> options,
            TenantManager tenantManager,
            SearchSettingsHelper searchSettingsHelper,
            FactoryIndexer factoryIndexer,
            BaseIndexer<DbFile> baseIndexer,
            IServiceProvider serviceProvider,
            IDaoFactory daoFactory,
            ICache cache,
            Settings settings)
            : base(options, tenantManager, searchSettingsHelper, factoryIndexer, baseIndexer, serviceProvider, cache)
        {
            DaoFactory = daoFactory;
            Settings = settings;
        }

        public override void IndexAll()
        {
            var fileDao = DaoFactory.GetFileDao<int>() as FileDao;

            (int, int, int) getCount(DateTime lastIndexed)
            {
                var dataQuery = GetBaseQuery(lastIndexed)
                    .Where(r => r.DbFile.Version == 1)
                    .OrderBy(r => r.DbFile.Id)
                    .Select(r => r.DbFile.Id);

                var minid = dataQuery.FirstOrDefault();

                dataQuery = GetBaseQuery(lastIndexed)
                    .Where(r => r.DbFile.Version == 1)
                    .OrderByDescending(r => r.DbFile.Id)
                    .Select(r => r.DbFile.Id);

                var maxid = dataQuery.FirstOrDefault();

                var count = GetBaseQuery(lastIndexed)
                    .Where(r => r.DbFile.Version == 1)
                    .Count();

                return new(count, maxid, minid);
            }

            List<DbFile> getData(long start, long stop, DateTime lastIndexed)
            {
                return GetBaseQuery(lastIndexed)
                    .Where(r => r.DbFile.Id >= start && r.DbFile.Id <= stop && r.DbFile.CurrentVersion)
                    .Select(r => r.DbFile)
                    .ToList();

            }

            List<int> getIds(DateTime lastIndexed)
            {
                var start = 0;
                var result = new List<int>();
                while (true)
                {
                    var dataQuery = GetBaseQuery(lastIndexed)
                        .Where(r => r.DbFile.Id >= start)
                        .Where(r => r.DbFile.Version == 1)
                        .OrderBy(r => r.DbFile.Id)
                        .Select(r => r.DbFile.Id)
                        .Skip(BaseIndexer<DbFile>.QueryLimit);

                    var id = dataQuery.FirstOrDefault();
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

            IQueryable<FileTenant> GetBaseQuery(DateTime lastIndexed) => fileDao.FilesDbContext.Files
                .AsQueryable()
                .Where(r => r.ModifiedOn >= lastIndexed)
                .Join(fileDao.FilesDbContext.Tenants, r => r.TenantId, r => r.Id, (f, t) => new FileTenant { DbFile = f, DbTenant = t })
                .Where(r => r.DbTenant.Status == ASC.Core.Tenants.TenantStatus.Active);

            try
            {
                var j = 0;
                var tasks = new List<Task>();

                foreach (var data in Indexer.IndexAll(getCount, getIds, getData))
                {
                    if (Settings.Threads == 1)
                    {
                        data.ForEach(r =>
                        {
                            TenantManager.SetCurrentTenant(r.TenantId);
                            fileDao.InitDocumentAsync(r).Wait();
                        });
                        Index(data);
                    }
                    else
                    {
                        //TODO: refactoring
                        data.ForEach(r =>
                        {
                            TenantManager.SetCurrentTenant(r.TenantId);
                            fileDao.InitDocumentAsync(r).Wait();
                        });

                        tasks.Add(IndexAsync(data));
                        j++;
                        if (j >= Settings.Threads)
                        {
                            Task.WaitAll(tasks.ToArray());
                            tasks = new List<Task>();
                            j = 0;
                        }
                    }
                }

                if (tasks.Count > 0)
                {
                    Task.WaitAll(tasks.ToArray());
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw;
            }
        }


        public override string SettingsTitle
        {
            get { return FilesCommonResource.IndexTitle; }
        }
    }

    public class FileTenant
    {
        public DbTenant DbTenant { get; set; }
        public DbFile DbFile { get; set; }
    }

    public static class FactoryIndexerFileExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<DbFile>();
        }
    }
}
