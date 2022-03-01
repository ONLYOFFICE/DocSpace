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

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ASC.Core.Data;

[Singletone]
public class DbSettingsManagerCache
{
    public ICache Cache { get; }
    private readonly ICacheNotify<SettingsCacheItem> _notify;

    public DbSettingsManagerCache(ICacheNotify<SettingsCacheItem> notify, ICache cache)
    {
        Cache = cache;
        _notify = notify;
        _notify.Subscribe((i) => Cache.Remove(i.Key), CacheNotifyAction.Remove);
    }

    public void Remove(string key)
    {
        _notify.Publish(new SettingsCacheItem { Key = key }, CacheNotifyAction.Remove);
    }
}

[Scope]
class ConfigureDbSettingsManager : IConfigureNamedOptions<DbSettingsManager>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DbSettingsManagerCache _dbSettingsManagerCache;
    private readonly IOptionsMonitor<ILog> _logger;
    private readonly AuthContext _authContext;
    private readonly IOptionsSnapshot<TenantManager> _tenantManager;
    private readonly DbContextManager<WebstudioDbContext> _dbContextManager;

    public ConfigureDbSettingsManager(
        IServiceProvider serviceProvider,
        DbSettingsManagerCache dbSettingsManagerCache,
        IOptionsMonitor<ILog> iLog,
        AuthContext authContext,
        IOptionsSnapshot<TenantManager> tenantManager,
        DbContextManager<WebstudioDbContext> dbContextManager
        )
    {
        _serviceProvider = serviceProvider;
        _dbSettingsManagerCache = dbSettingsManagerCache;
        _logger = iLog;
        _authContext = authContext;
        _tenantManager = tenantManager;
        _dbContextManager = dbContextManager;
    }

    public void Configure(string name, DbSettingsManager options)
    {
        Configure(options);

        options.TenantManager = _tenantManager.Get(name);
        options.LazyWebstudioDbContext = new Lazy<WebstudioDbContext>(() => _dbContextManager.Get(name));
    }

    public void Configure(DbSettingsManager options)
    {
        options.ServiceProvider = _serviceProvider;
        options.DbSettingsManagerCache = _dbSettingsManagerCache;
        options.AuthContext = _authContext;
        options.Logger = _logger.CurrentValue;

        options.TenantManager = _tenantManager.Value;
        options.LazyWebstudioDbContext = new Lazy<WebstudioDbContext>(() => _dbContextManager.Value);
    }
}

[Scope(typeof(ConfigureDbSettingsManager))]
public class DbSettingsManager
{
    private readonly TimeSpan _expirationTimeout = TimeSpan.FromMinutes(5);

    internal ILog Logger;
    internal ICache Cache;
    internal IServiceProvider ServiceProvider;
    internal DbSettingsManagerCache DbSettingsManagerCache;
    internal AuthContext AuthContext;
    internal TenantManager TenantManager;
    internal Lazy<WebstudioDbContext> LazyWebstudioDbContext;
    internal WebstudioDbContext WebstudioDbContext => LazyWebstudioDbContext.Value;

    public DbSettingsManager() { }

    public DbSettingsManager(
        IServiceProvider serviceProvider,
        DbSettingsManagerCache dbSettingsManagerCache,
        IOptionsMonitor<ILog> option,
        AuthContext authContext,
        TenantManager tenantManager,
        DbContextManager<WebstudioDbContext> dbContextManager)
    {
        ServiceProvider = serviceProvider;
        DbSettingsManagerCache = dbSettingsManagerCache;
        AuthContext = authContext;
        TenantManager = tenantManager;
        Cache = dbSettingsManagerCache.Cache;
        Logger = option.CurrentValue;
        LazyWebstudioDbContext = new Lazy<WebstudioDbContext>(() => dbContextManager.Value);
    }

    private int _tenantID;
    private int TenantID
    {
        get
        {
            if (_tenantID == 0) _tenantID = TenantManager.GetCurrentTenant().Id;

            return _tenantID;
        }
    }
    //
    private Guid? _currentUserID;
    private Guid CurrentUserID
    {
        get
        {
            _currentUserID ??= AuthContext.CurrentAccount.ID;

            return _currentUserID.Value;
        }
    }

    public bool SaveSettings<T>(T settings, int tenantId) where T : ISettings
    {
        return SaveSettingsFor(settings, tenantId, Guid.Empty);
    }

    public T LoadSettings<T>(int tenantId) where T : class, ISettings
    {
        return LoadSettingsFor<T>(tenantId, Guid.Empty);
    }

    public void ClearCache<T>(int tenantId) where T : class, ISettings
    {
        var settings = LoadSettings<T>(tenantId);
        var key = settings.ID.ToString() + tenantId + Guid.Empty;

        DbSettingsManagerCache.Remove(key);
    }


    public bool SaveSettingsFor<T>(T settings, int tenantId, Guid userId) where T : ISettings
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        try
        {
            var key = settings.ID.ToString() + tenantId + userId;
            var data = Serialize(settings);

            var def = (T)settings.GetDefault(ServiceProvider);

            var defaultData = Serialize(def);

            if (data.SequenceEqual(defaultData))
            {
                using var tr = WebstudioDbContext.Database.BeginTransaction();
                // remove default settings
                var s = WebstudioDbContext.WebstudioSettings
                    .Where(r => r.Id == settings.ID)
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.UserId == userId)
                    .FirstOrDefault();

                if (s != null)
                {
                    WebstudioDbContext.WebstudioSettings.Remove(s);
                }

                WebstudioDbContext.SaveChanges();
                tr.Commit();
            }
            else
            {
                var s = new DbWebstudioSettings
                {
                    Id = settings.ID,
                    UserId = userId,
                    TenantId = tenantId,
                    Data = data
                };

                WebstudioDbContext.AddOrUpdate(r => r.WebstudioSettings, s);

                WebstudioDbContext.SaveChanges();
            }

            DbSettingsManagerCache.Remove(key);

            Cache.Insert(key, settings, _expirationTimeout);

            return true;
        }
        catch (Exception ex)
        {
            Logger.Error(ex);

            return false;
        }
    }

    internal T LoadSettingsFor<T>(int tenantId, Guid userId) where T : class, ISettings
    {
        var settingsInstance = ActivatorUtilities.CreateInstance<T>(ServiceProvider);
        var key = settingsInstance.ID.ToString() + tenantId + userId;
        var def = (T)settingsInstance.GetDefault(ServiceProvider);

        try
        {
            var settings = Cache.Get<T>(key);
            if (settings != null)
            {
                return settings;
            }

            var result = WebstudioDbContext.WebstudioSettings
                    .Where(r => r.Id == settingsInstance.ID)
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.UserId == userId)
                    .Select(r => r.Data)
                    .FirstOrDefault();

            if (result != null)
            {
                settings = Deserialize<T>(result);
            }
            else
            {
                settings = def;
            }

            Cache.Insert(key, settings, _expirationTimeout);

            return settings;
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }

        return def;
    }

    public T Load<T>() where T : class, ISettings
    {
        return LoadSettings<T>(TenantID);
    }

    public T LoadForCurrentUser<T>() where T : class, ISettings
    {
        return LoadForUser<T>(CurrentUserID);
    }

    public T LoadForUser<T>(Guid userId) where T : class, ISettings
    {
        return LoadSettingsFor<T>(TenantID, userId);
    }

    public T LoadForDefaultTenant<T>() where T : class, ISettings
    {
        return LoadForTenant<T>(Tenant.DefaultTenant);
    }

    public T LoadForTenant<T>(int tenantId) where T : class, ISettings
    {
        return LoadSettings<T>(tenantId);
    }

    public virtual bool Save<T>(T data) where T : class, ISettings
    {
        return SaveSettings(data, TenantID);
    }

    public bool SaveForCurrentUser<T>(T data) where T : class, ISettings
    {
        return SaveForUser(data, CurrentUserID);
    }

    public bool SaveForUser<T>(T data, Guid userId) where T : class, ISettings
    {
        return SaveSettingsFor(data, TenantID, userId);
    }

    public bool SaveForDefaultTenant<T>(T data) where T : class, ISettings
    {
        return SaveForTenant(data, Tenant.DefaultTenant);
    }

    public bool SaveForTenant<T>(T data, int tenantId) where T : class, ISettings
    {
        return SaveSettings(data, tenantId);
    }

    public void ClearCache<T>() where T : class, ISettings
    {
        ClearCache<T>(TenantID);
    }

    private T Deserialize<T>(string data)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<T>(data, options);
    }

    private string Serialize<T>(T settings)
    {
        return JsonSerializer.Serialize(settings);
    }
}
