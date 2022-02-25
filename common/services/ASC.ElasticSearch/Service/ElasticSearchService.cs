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

namespace ASC.ElasticSearch.Service;

[Singletone(Additional = typeof(ServiceExtension))]
public class ElasticSearchService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICacheNotify<ReIndexAction> _cacheNotify;

    public ElasticSearchService(IServiceProvider serviceProvider, ICacheNotify<ReIndexAction> cacheNotify)
    {
        _serviceProvider = serviceProvider;
        _cacheNotify = cacheNotify;
    }

    public void Subscribe()
    {
        _cacheNotify.Subscribe((a) =>
        {
            ReIndex(a.Names.ToList(), a.Tenant);
        }, CacheNotifyAction.Any);
    }

    public bool Support(string table)
    {
        return _serviceProvider.GetService<IEnumerable<IFactoryIndexer>>().Any(r => r.IndexName == table);
    }

    public void ReIndex(List<string> toReIndex, int tenant)
    {
        var allItems = _serviceProvider.GetService<IEnumerable<IFactoryIndexer>>().ToList();
        var tasks = new List<Task>(toReIndex.Count);

        foreach (var item in toReIndex)
        {
            var index = allItems.FirstOrDefault(r => r.IndexName == item);
            if (index == null)
            {
                continue;
            }

            var generic = typeof(BaseIndexer<>);
            var instance = (IIndexer)Activator.CreateInstance(generic.MakeGenericType(index.GetType()), index);
            tasks.Add(instance.ReIndex());
        }

        if (tasks.Count == 0)
        {
            return;
        }

        Task.WhenAll(tasks).ContinueWith(r =>
        {
            using var scope = _serviceProvider.CreateScope();

            var scopeClass = scope.ServiceProvider.GetService<ServiceScope>();
            var (tenantManager, settingsManager) = scopeClass;
            tenantManager.SetCurrentTenant(tenant);
            settingsManager.ClearCache<SearchSettings>();
        });
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

[Scope]
public class ServiceScope
{
    private readonly TenantManager _tenantManager;
    private readonly SettingsManager _settingsManager;

    public ServiceScope(TenantManager tenantManager, SettingsManager settingsManager)
    {
        _tenantManager = tenantManager;
        _settingsManager = settingsManager;
    }

    public void Deconstruct(out TenantManager tenantManager, out SettingsManager settingsManager)
    {
        tenantManager = _tenantManager;
        settingsManager = _settingsManager;
    }
}

internal static class ServiceExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<ServiceScope>();
    }
}
