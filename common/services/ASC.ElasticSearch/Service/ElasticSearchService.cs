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
