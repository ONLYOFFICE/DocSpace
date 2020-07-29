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

using ASC.Common.Caching;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.ElasticSearch.Core;

using Autofac;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.ElasticSearch.Service
{
    public class Service
    {
        public IContainer Container { get; }
        public IServiceProvider ServiceProvider { get; }
        public ICacheNotify<ReIndexAction> CacheNotify { get; }

        public Service(IContainer container, IServiceProvider serviceProvider, ICacheNotify<ReIndexAction> cacheNotify)
        {
            Container = container;
            ServiceProvider = serviceProvider;
            CacheNotify = cacheNotify;
        }

        public void Subscribe()
        {
            CacheNotify.Subscribe((a) =>
            {
                ReIndex(a.Names.ToList(), a.Tenant);
            }, CacheNotifyAction.Any);
        }

        public bool Support(string table)
        {
            return Container.Resolve<IEnumerable<IFactoryIndexer>>().Any(r => r.IndexName == table);
        }

        public void ReIndex(List<string> toReIndex, int tenant)
        {
            var allItems = Container.Resolve<IEnumerable<IFactoryIndexer>>().ToList();
            var tasks = new List<Task>(toReIndex.Count);

            foreach (var item in toReIndex)
            {
                var index = allItems.FirstOrDefault(r => r.IndexName == item);
                if (index == null) continue;

                var generic = typeof(BaseIndexer<>);
                var instance = (IIndexer)Activator.CreateInstance(generic.MakeGenericType(index.GetType()), index);
                tasks.Add(instance.ReIndex());
            }

            if (!tasks.Any()) return;

            Task.WhenAll(tasks).ContinueWith(r =>
            {
                using var scope = ServiceProvider.CreateScope();

                var scopeClass = scope.ServiceProvider.GetService<Scope>();
                scopeClass.TenantManager.SetCurrentTenant(tenant);
                scopeClass.SettingsManager.ClearCache<SearchSettings>();
            });
        }

        class Scope
        {
            internal TenantManager TenantManager { get; }
            internal SettingsManager SettingsManager { get; }

            public Scope(TenantManager tenantManager, SettingsManager settingsManager)
            {
                TenantManager = tenantManager;
                SettingsManager = settingsManager;
            }
        }
        //public State GetState()
        //{
        //    return new State
        //    {
        //        Indexing = Launcher.Indexing,
        //        LastIndexed = Launcher.LastIndexed
        //    };
        //}
    }
}
