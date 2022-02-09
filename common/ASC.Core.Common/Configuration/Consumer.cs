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

namespace ASC.Core.Common.Configuration;

public class Consumer : IDictionary<string, string>
{
    public bool CanSet { get; private set; }
    public int Order { get; private set; }
    public string Name { get; private set; }
    public int Count => AllProps.Count;
    public bool IsReadOnly => true;
    public bool IsSet => _props.Any() && !_props.All(r => string.IsNullOrEmpty(this[r.Key]));
    public IEnumerable<string> ManagedKeys => _props.Select(r => r.Key).ToList();
    public ICollection<string> Keys => AllProps.Keys;
    public ICollection<string> Values => AllProps.Values;
    public virtual IEnumerable<string> AdditionalKeys => _additional.Select(r => r.Key).ToList();

    public string this[string key]
    {
        get => Get(key);
        set => Set(key, value);
    }

    internal protected TenantManager TenantManager { get; set; }
    internal protected CoreBaseSettings CoreBaseSettings { get; set; }
    internal protected CoreSettings CoreSettings { get; set; }
    internal protected ConsumerFactory ConsumerFactory { get; set; }
    internal protected IConfiguration Configuration { get; }
    internal protected ICacheNotify<ConsumerCacheItem> EventBusConsumerItem { get; }

    private Dictionary<string, string> AllProps
    {
        get
        {
            var result = _props.ToDictionary(item => item.Key, item => item.Value);

            foreach (var item in _additional.Where(item => !result.ContainsKey(item.Key)))
            {
                result.Add(item.Key, item.Value);
            }

            return result;
        }
    }

    protected readonly Dictionary<string, string> _props;
    protected readonly Dictionary<string, string> _additional;
    private readonly bool _onlyDefault;

    static Consumer() { }

    public Consumer()
    {
        _props = new Dictionary<string, string>();
        _additional = new Dictionary<string, string>();
    }

    public Consumer(
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        ICacheNotify<ConsumerCacheItem> cache,
        ConsumerFactory consumerFactory) : this()
    {
        TenantManager = tenantManager;
        CoreBaseSettings = coreBaseSettings;
        CoreSettings = coreSettings;
        Configuration = configuration;
        EventBusConsumerItem = cache;
        ConsumerFactory = consumerFactory;
        _onlyDefault = configuration["core:default-consumers"] == "true";
        Name = string.Empty;
        Order = int.MaxValue;
    }

    public Consumer(
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        ICacheNotify<ConsumerCacheItem> cache,
        ConsumerFactory consumerFactory,
        string name, int order, Dictionary<string, string> additional)
        : this(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory)
    {
        Name = name;
        Order = order;
        _props = new Dictionary<string, string>();
        _additional = additional;
    }

    public Consumer(
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        ICacheNotify<ConsumerCacheItem> cache,
        ConsumerFactory consumerFactory,
        string name, int order, Dictionary<string, string> props, Dictionary<string, string> additional = null)
        : this(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory)
    {
        Name = name;
        Order = order;
        _props = props ?? new Dictionary<string, string>();
        _additional = additional ?? new Dictionary<string, string>();

        if (props != null && props.Any())
        {
            CanSet = props.All(r => string.IsNullOrEmpty(r.Value));
        }
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        return AllProps.GetEnumerator();
    }

    public void Add(KeyValuePair<string, string> item) { }

    public void Clear()
    {
        if (!CanSet)
        {
            throw new NotSupportedException("Key for read only. Consumer " + Name);
        }

        foreach (var providerProp in _props)
        {
            this[providerProp.Key] = null;
        }

        EventBusConsumerItem.Publish(new ConsumerCacheItem() { Name = this.Name }, CacheNotifyAction.Remove);
    }

    public bool Contains(KeyValuePair<string, string> item)
    {
        return AllProps.Contains(item);
    }

    public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) { }

    public bool Remove(KeyValuePair<string, string> item)
    {
        return AllProps.Remove(item.Key);
    }

    public bool ContainsKey(string key)
    {
        return AllProps.ContainsKey(key);
    }

    public void Add(string key, string value) { }

    public bool Remove(string key)
    {
        return false;
    }

    public bool TryGetValue(string key, out string value)
    {
        return AllProps.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private string Get(string name)
    {
        string value = null;

        if (!_onlyDefault && CanSet)
        {
            var tenant = CoreBaseSettings.Standalone
                             ? Tenant.DefaultTenant
                             : TenantManager.GetCurrentTenant().Id;

            value = CoreSettings.GetSetting(GetSettingsKey(name), tenant);
        }

        if (string.IsNullOrEmpty(value) && AllProps.ContainsKey(name))
        {
            value = AllProps[name];
        }

        return value;
    }

    private void Set(string name, string value)
    {
        if (!CanSet)
        {
            throw new NotSupportedException("Key for read only. Key " + name);
        }

        if (!ManagedKeys.Contains(name))
        {
            if (_additional.ContainsKey(name))
            {
                _additional[name] = value;
            }

            else _additional.Add(name, value);

            return;
        }

        var tenant = CoreBaseSettings.Standalone
                         ? Tenant.DefaultTenant
                         : TenantManager.GetCurrentTenant().Id;
        CoreSettings.SaveSetting(GetSettingsKey(name), value, tenant);
    }

    protected virtual string GetSettingsKey(string name) => $"AuthKey_{name}";
}

public class DataStoreConsumer : Consumer, ICloneable
{
    public const string HandlerTypeKey = "handlerType";
    public const string CdnKey = "cdn";

    public Type HandlerType { get; private set; }
    public DataStoreConsumer Cdn { get; private set; }
    public override IEnumerable<string> AdditionalKeys =>
        base.AdditionalKeys.Where(r => r != HandlerTypeKey && r != "cdn").ToList();

    public DataStoreConsumer() : base() { }

    public DataStoreConsumer(
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        ICacheNotify<ConsumerCacheItem> cache,
        ConsumerFactory consumerFactory)
        : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory) { }

    public DataStoreConsumer(
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        ICacheNotify<ConsumerCacheItem> cache,
        ConsumerFactory consumerFactory,
        string name, int order, Dictionary<string, string> additional)
        : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, name, order, additional)
    {
        Init(additional);
    }

    public DataStoreConsumer(
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        ICacheNotify<ConsumerCacheItem> cache,
        ConsumerFactory consumerFactory,
        string name, int order, Dictionary<string, string> props, Dictionary<string, string> additional)
        : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, name, order, props, additional)
    {
        Init(additional);
    }

    public object Clone()
    {
        return new DataStoreConsumer(TenantManager, CoreBaseSettings, CoreSettings, Configuration,
            EventBusConsumerItem, ConsumerFactory, Name, Order, _props.ToDictionary(r => r.Key, r => r.Value),
            _additional.ToDictionary(r => r.Key, r => r.Value));
    }

    protected override string GetSettingsKey(string name)
    {
        return base.GetSettingsKey(Name + name);
    }

    private void Init(IReadOnlyDictionary<string, string> additional)
    {
        if (additional == null || !additional.ContainsKey(HandlerTypeKey))
        {
            throw new ArgumentException(HandlerTypeKey);
        }

        HandlerType = Type.GetType(additional[HandlerTypeKey]);

        if (additional.ContainsKey(CdnKey))
        {
            Cdn = GetCdn(additional[CdnKey]);
        }
    }

    private DataStoreConsumer GetCdn(string cdn)
    {
        var fromConfig = ConsumerFactory.GetByKey<Consumer>(cdn);
        if (string.IsNullOrEmpty(fromConfig.Name))
        {
            return null;
        }

        var props = ManagedKeys.ToDictionary(prop => prop, prop => this[prop]);
        var additional = fromConfig.AdditionalKeys.ToDictionary(prop => prop, prop => fromConfig[prop]);
        additional.Add(HandlerTypeKey, HandlerType.AssemblyQualifiedName);

        return new DataStoreConsumer(fromConfig.TenantManager, fromConfig.CoreBaseSettings, fromConfig.CoreSettings,
            fromConfig.Configuration, fromConfig.EventBusConsumerItem, fromConfig.ConsumerFactory, fromConfig.Name,
            fromConfig.Order, props, additional);
    }
}

[Scope]
public class ConsumerFactory : IDisposable
{
    public ILifetimeScope Builder { get; set; }

    public ConsumerFactory(IContainer builder)
    {
        Builder = builder.BeginLifetimeScope();
    }

    public ConsumerFactory(ILifetimeScope builder)
    {
        Builder = builder;
    }

    public Consumer GetByKey(string key)
    {
        if (Builder.TryResolveKeyed(key, typeof(Consumer), out var result))
        {
            return (Consumer)result;
        }

        return new Consumer();
    }

    public T GetByKey<T>(string key) where T : Consumer, new()
    {
        if (Builder.TryResolveKeyed(key, typeof(T), out var result))
        {
            return (T)result;
        }

        return new T();
    }

    public T Get<T>() where T : Consumer, new()
    {
        if (Builder.TryResolve(out T result))
        {
            return result;
        }

        return new T();
    }

    public IEnumerable<T> GetAll<T>() where T : Consumer, new()
    {
        return Builder.Resolve<IEnumerable<T>>();
    }

    public void Dispose()
    {
        Builder.Dispose();
    }
}
