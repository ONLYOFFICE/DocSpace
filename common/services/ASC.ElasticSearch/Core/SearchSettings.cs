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

namespace ASC.ElasticSearch.Core;

[Serializable]
public class SearchSettings : ISettings
{
    public string Data { get; set; }
    public Guid ID => new Guid("{93784AB2-10B5-4C2F-9B36-F2662CCCF316}");
    internal List<SearchSettingsItem> Items
    {
        get
        {
            if (_items != null)
            {
                return _items;
            }

            var parsed = JsonConvert.DeserializeObject<List<SearchSettingsItem>>(Data ?? "");

            return _items = parsed ?? new List<SearchSettingsItem>();
        }
        set
        {
            _items = value;
        }
    }

    private List<SearchSettingsItem> _items;

    public ISettings GetDefault(IServiceProvider serviceProvider)
    {
        return new SearchSettings();
    }

    internal bool IsEnabled(string name)
    {
        var wrapper = Items.FirstOrDefault(r => r.ID == name);

        return wrapper != null && wrapper.Enabled;
    }
}

[Scope]
public class SearchSettingsHelper
{
    public IConfiguration Configuration { get; }
    internal IEnumerable<IFactoryIndexer> AllItems =>
        _allItems ??= _serviceProvider.GetService<IEnumerable<IFactoryIndexer>>();

    private readonly TenantManager _tenantManager;
    private readonly SettingsManager _settingsManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly ICacheNotify<ReIndexAction> _cacheNotify;
    private readonly IServiceProvider _serviceProvider;
    private IEnumerable<IFactoryIndexer> _allItems;

    public SearchSettingsHelper(
        TenantManager tenantManager,
        SettingsManager settingsManager,
        CoreBaseSettings coreBaseSettings,
        ICacheNotify<ReIndexAction> cacheNotify,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _tenantManager = tenantManager;
        _settingsManager = settingsManager;
        _coreBaseSettings = coreBaseSettings;
        _cacheNotify = cacheNotify;
        _serviceProvider = serviceProvider;
        Configuration = configuration;
    }

    public List<SearchSettingsItem> GetAllItems()
    {
        if (!_coreBaseSettings.Standalone)
        {
            return new List<SearchSettingsItem>();
        }

        var settings = _settingsManager.Load<SearchSettings>();

        return AllItems.Select(r => new SearchSettingsItem
        {
            ID = r.IndexName,
            Enabled = settings.IsEnabled(r.IndexName),
            Title = r.SettingsTitle
        }).ToList();
    }

    public void Set(List<SearchSettingsItem> items)
    {
        if (!_coreBaseSettings.Standalone)
        {
            return;
        }

        var settings = _settingsManager.Load<SearchSettings>();

        var settingsItems = settings.Items;
        var toReIndex = settingsItems.Count == 0 ? items.Where(r => r.Enabled).ToList() : items.Where(item => settingsItems.Any(r => r.ID == item.ID && r.Enabled != item.Enabled)).ToList();

        settings.Items = items;
        settings.Data = JsonConvert.SerializeObject(items);
        _settingsManager.Save(settings);

        var action = new ReIndexAction() { Tenant = _tenantManager.GetCurrentTenant().Id };
        action.Names.AddRange(toReIndex.Select(r => r.ID).ToList());

        _cacheNotify.Publish(action, CacheNotifyAction.Any);
    }

    public bool CanIndexByContent<T>(int tenantId) where T : class, ISearchItem
    {
        return CanIndexByContent(typeof(T), tenantId);
    }

    public bool CanIndexByContent(Type t, int tenantId)
    {
        if (!typeof(ISearchItemDocument).IsAssignableFrom(t))
        {
            return false;
        }

        if (Convert.ToBoolean(Configuration["core:search-by-content"] ?? "false"))
        {
            return true;
        }

        if (!_coreBaseSettings.Standalone)
        {
            return true;
        }

        var settings = _settingsManager.LoadForTenant<SearchSettings>(tenantId);

        return settings.IsEnabled(((ISearchItemDocument)_serviceProvider.GetService(t)).IndexName);
    }

    public bool CanSearchByContent<T>() where T : class, ISearchItem
    {
        return CanSearchByContent(typeof(T));
    }

    public bool CanSearchByContent(Type t)
    {
        var tenantId = _tenantManager.GetCurrentTenant().Id;
        if (!CanIndexByContent(t, tenantId))
        {
            return false;
        }

        if (_coreBaseSettings.Standalone)
        {
            return true;
        }

        return _tenantManager.GetTenantQuota(tenantId).ContentSearch;
    }
}

[Serializable]
public class SearchSettingsItem
{
    public string ID { get; set; }
    public bool Enabled { get; set; }
    public string Title { get; set; }
}
