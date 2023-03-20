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

namespace ASC.Web.Core.Sms;

[Singletone]
public class SmsKeyStorageCache
{
    private readonly ICacheNotify<SmsKeyCacheKey> _keyCacheNotify;
    private readonly ICache _keyCache;

    public SmsKeyStorageCache(ICacheNotify<SmsKeyCacheKey> keyCacheNotify, ICache cache)
    {
        _keyCache = cache;
        _keyCacheNotify = keyCacheNotify;
        _keyCacheNotify.Subscribe(r => _keyCache.Remove(r.Key), CacheNotifyAction.Remove);
    }

    public void RemoveFromCache(string cacheKey)
    {
        _keyCacheNotify.Publish(new SmsKeyCacheKey { Key = cacheKey }, CacheNotifyAction.Remove);
    }
}

[Scope]
public class SmsKeyStorage
{
    private readonly int _keyLength;
    public readonly TimeSpan StoreInterval;
    private readonly int _attemptCount;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

    private readonly ICache _keyCache;
    private readonly ICache _checkCache;
    private readonly TenantManager _tenantManager;
    private readonly SmsKeyStorageCache _smsKeyStorageCache;

    public SmsKeyStorage(TenantManager tenantManager, IConfiguration configuration, SmsKeyStorageCache smsKeyStorageCache, ICache cache)
    {
        _keyCache = cache;
        _checkCache = cache;

        _tenantManager = tenantManager;
        _smsKeyStorageCache = smsKeyStorageCache;
        if (!int.TryParse(configuration["sms:keylength"], out _keyLength))
        {
            _keyLength = 6;
        }

        if (!int.TryParse(configuration["sms:keystore"], out var store))
        {
            store = 10;
        }
        StoreInterval = TimeSpan.FromMinutes(store);

        if (!int.TryParse(configuration["sms:keycount"], out _attemptCount))
        {
            _attemptCount = 5;
        }
    }

    private async Task<string> BuildCacheKeyAsync(string phone)
    {
        var tenant = await _tenantManager.GetCurrentTenantAsync(false);
        var tenantCache = tenant == null ? Tenant.DefaultTenant : tenant.Id;
        return "smskey" + phone + tenantCache;
    }

    public async Task<(bool, string)> GenerateKeyAsync(string phone)
    {
        string key = null;
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(phone);

        try
        {
            await _semaphore.WaitAsync();
            var cacheKey = await BuildCacheKeyAsync(phone);
            var phoneKeys = _keyCache.Get<Dictionary<string, DateTime>>(cacheKey) ?? new Dictionary<string, DateTime>();
            if (phoneKeys.Count > _attemptCount)
            {
                key = null;
                return (false, key);
            }

            key = RandomNumberGenerator.GetInt32((int)Math.Pow(10, _keyLength - 1), (int)Math.Pow(10, _keyLength)).ToString(CultureInfo.InvariantCulture);
            phoneKeys[key] = DateTime.UtcNow;

            _keyCache.Insert(cacheKey, phoneKeys, DateTime.UtcNow.Add(StoreInterval));
            return (true, key);
        }
        catch
        {
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> ExistsKeyAsync(string phone)
    {
        if (string.IsNullOrEmpty(phone))
        {
            return false;
        }

        try
        {
            await _semaphore.WaitAsync();
            var cacheKey = await BuildCacheKeyAsync(phone);
            var phoneKeys = _keyCache.Get<Dictionary<string, DateTime>>(cacheKey);
            return phoneKeys != null;
        }
        catch
        {
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<Result> ValidateKeyAsync(string phone, string key)
    {
        key = (key ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(key))
        {
            return Result.Empty;
        }

        var cacheCheck = await BuildCacheKeyAsync("check" + phone);
        int.TryParse(_checkCache.Get<string>(cacheCheck), out var counter);
        if (++counter > _attemptCount)
        {
            return Result.TooMuch;
        }

        _checkCache.Insert(cacheCheck, counter.ToString(CultureInfo.InvariantCulture), DateTime.UtcNow.Add(StoreInterval));

        try 
        {
            await _semaphore.WaitAsync();
            var cacheKey = await BuildCacheKeyAsync(phone);
            var phoneKeys = _keyCache.Get<Dictionary<string, DateTime>>(cacheKey);
            if (phoneKeys == null)
            {
                return Result.Timeout;
            }

            if (!phoneKeys.ContainsKey(key))
            {
                return Result.Invalide;
            }

            var createDate = phoneKeys[key];
            _smsKeyStorageCache.RemoveFromCache(cacheKey);
            if (createDate.Add(StoreInterval) < DateTime.UtcNow)
            {
                return Result.Timeout;
            }

            _checkCache.Insert(cacheCheck, (counter - 1).ToString(CultureInfo.InvariantCulture), DateTime.UtcNow.Add(StoreInterval));
            return Result.Ok;
        }
        catch
        {
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public enum Result
    {
        Ok,
        Invalide,
        Empty,
        TooMuch,
        Timeout,
    }
}
