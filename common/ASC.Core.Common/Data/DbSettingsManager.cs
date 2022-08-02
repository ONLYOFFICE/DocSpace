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
public class DbSettingsManager
{
    private readonly TimeSpan _expirationTimeout = TimeSpan.FromMinutes(5);

    private readonly ILogger<DbSettingsManager> _logger;
    private readonly ICache _cache;
    private readonly IServiceProvider _serviceProvider;
    private readonly DbSettingsManagerCache _dbSettingsManagerCache;
    private readonly AuthContext _authContext;
    private readonly TenantManager _tenantManager;
    private readonly IDbContextFactory<WebstudioDbContext> _dbContextFactory;

    public DbSettingsManager(
        IServiceProvider serviceProvider,
        DbSettingsManagerCache dbSettingsManagerCache,
        ILogger<DbSettingsManager> logger,
        AuthContext authContext,
        TenantManager tenantManager,
        IDbContextFactory<WebstudioDbContext> dbContextFactory)
    {
        _serviceProvider = serviceProvider;
        _dbSettingsManagerCache = dbSettingsManagerCache;
        _authContext = authContext;
        _tenantManager = tenantManager;
        _dbContextFactory = dbContextFactory;
        _cache = dbSettingsManagerCache.Cache;
        _logger = logger;
    }

    private int _tenantID;
    private int TenantID
    {
        get
        {
            if (_tenantID == 0)
            {
                _tenantID = _tenantManager.GetCurrentTenant().Id;
            }

            return _tenantID;
        }
    }
    //
    private Guid? _currentUserID;
    private Guid CurrentUserID
    {
        get
        {
            _currentUserID ??= _authContext.CurrentAccount.ID;

            return _currentUserID.Value;
        }
    }

    public bool SaveSettings<T>(T settings, int tenantId) where T : class, ISettings<T>
    {
        return SaveSettingsFor(settings, tenantId, Guid.Empty);
    }

    public T LoadSettings<T>(int tenantId) where T : class, ISettings<T>
    {
        return LoadSettingsFor<T>(tenantId, Guid.Empty);
    }

    public void ClearCache<T>(int tenantId) where T : class, ISettings<T>
    {
        var settings = LoadSettings<T>(tenantId);
        var key = settings.ID.ToString() + tenantId + Guid.Empty;

        _dbSettingsManagerCache.Remove(key);
    }


    public bool SaveSettingsFor<T>(T settings, int tenantId, Guid userId) where T : class, ISettings<T>
    {
        ArgumentNullException.ThrowIfNull(settings);

        using var webstudioDbContext = _dbContextFactory.CreateDbContext();

        try
        {
            var key = settings.ID.ToString() + tenantId + userId;
            var data = Serialize(settings);
            var def = GetDefault<T>();

            var defaultData = Serialize(def);

            if (data.SequenceEqual(defaultData))
            {
                var strategy = webstudioDbContext.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using var tr = webstudioDbContext.Database.BeginTransaction();
                    // remove default settings
                    var s = webstudioDbContext.WebstudioSettings
                        .Where(r => r.Id == settings.ID)
                        .Where(r => r.TenantId == tenantId)
                        .Where(r => r.UserId == userId)
                        .FirstOrDefault();

                    if (s != null)
                    {
                        webstudioDbContext.WebstudioSettings.Remove(s);
                    }

                    webstudioDbContext.SaveChanges();

                    tr.Commit();
                });



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

                webstudioDbContext.AddOrUpdate(r => r.WebstudioSettings, s);

                webstudioDbContext.SaveChanges();
            }

            _dbSettingsManagerCache.Remove(key);

            _cache.Insert(key, settings, _expirationTimeout);

            return true;
        }
        catch (Exception ex)
        {
            _logger.ErrorSaveSettingsFor(ex);

            return false;
        }
    }

    internal T LoadSettingsFor<T>(int tenantId, Guid userId) where T : class, ISettings<T>
    {
        var def = GetDefault<T>();
        var key = def.ID.ToString() + tenantId + userId;

        try
        {
            var settings = _cache.Get<T>(key);
            if (settings != null)
            {
                return settings;
            }

            using var webstudioDbContext = _dbContextFactory.CreateDbContext();
            var result = webstudioDbContext.WebstudioSettings
                    .Where(r => r.Id == def.ID)
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

            _cache.Insert(key, settings, _expirationTimeout);

            return settings;
        }
        catch (Exception ex)
        {
            _logger.ErrorLoadSettingsFor(ex);
        }

        return def;
    }

    public T GetDefault<T>() where T : class, ISettings<T>
    {
        var settingsInstance = ActivatorUtilities.CreateInstance<T>(_serviceProvider);
        return settingsInstance.GetDefault();
    }

    public T Load<T>() where T : class, ISettings<T>
    {
        return LoadSettings<T>(TenantID);
    }

    public T LoadForCurrentUser<T>() where T : class, ISettings<T>
    {
        return LoadForUser<T>(CurrentUserID);
    }

    public T LoadForUser<T>(Guid userId) where T : class, ISettings<T>
    {
        return LoadSettingsFor<T>(TenantID, userId);
    }

    public T LoadForDefaultTenant<T>() where T : class, ISettings<T>
    {
        return LoadForTenant<T>(Tenant.DefaultTenant);
    }

    public T LoadForTenant<T>(int tenantId) where T : class, ISettings<T>
    {
        return LoadSettings<T>(tenantId);
    }

    public virtual bool Save<T>(T data) where T : class, ISettings<T>
    {
        return SaveSettings(data, TenantID);
    }

    public bool SaveForCurrentUser<T>(T data) where T : class, ISettings<T>
    {
        return SaveForUser(data, CurrentUserID);
    }

    public bool SaveForUser<T>(T data, Guid userId) where T : class, ISettings<T>
    {
        return SaveSettingsFor(data, TenantID, userId);
    }

    public bool SaveForDefaultTenant<T>(T data) where T : class, ISettings<T>
    {
        return SaveForTenant(data, Tenant.DefaultTenant);
    }

    public bool SaveForTenant<T>(T data, int tenantId) where T : class, ISettings<T>
    {
        return SaveSettings(data, tenantId);
    }

    public void ClearCache<T>() where T : class, ISettings<T>
    {
        ClearCache<T>(TenantID);
    }

    private T Deserialize<T>(string data)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        return JsonSerializer.Deserialize<T>(data, options);
    }

    private string Serialize<T>(T settings)
    {
        return JsonSerializer.Serialize(settings);
    }
}
