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

namespace ASC.Web.Core.Sms
{
    [Singletone]
    public class SmsKeyStorageCache
    {
        private ICacheNotify<SmsKeyCacheKey> KeyCacheNotify { get; }
        public ICache KeyCache { get; }
        public ICache CheckCache { get; }

        public SmsKeyStorageCache(ICacheNotify<SmsKeyCacheKey> keyCacheNotify, ICache cache)
        {
            CheckCache = cache;
            KeyCache = cache;
            KeyCacheNotify = keyCacheNotify;
            KeyCacheNotify.Subscribe(r => KeyCache.Remove(r.Key), Common.Caching.CacheNotifyAction.Remove);
        }

        public void RemoveFromCache(string cacheKey)
        {
            KeyCacheNotify.Publish(new SmsKeyCacheKey { Key = cacheKey }, Common.Caching.CacheNotifyAction.Remove);
        }
    }

    [Scope]
    public class SmsKeyStorage
    {
        public readonly int KeyLength;
        public readonly TimeSpan StoreInterval;
        public readonly int AttemptCount;
        private static readonly object KeyLocker = new object();
        private ICache KeyCache { get; }
        private ICache CheckCache { get; }

        private TenantManager TenantManager { get; }
        public SmsKeyStorageCache SmsKeyStorageCache { get; }

        public SmsKeyStorage(TenantManager tenantManager, IConfiguration configuration, SmsKeyStorageCache smsKeyStorageCache)
        {
            KeyCache = smsKeyStorageCache.KeyCache;
            CheckCache = smsKeyStorageCache.CheckCache;

            TenantManager = tenantManager;
            SmsKeyStorageCache = smsKeyStorageCache;
            if (!int.TryParse(configuration["sms:keylength"], out KeyLength))
            {
                KeyLength = 6;
            }

            if (!int.TryParse(configuration["sms:keystore"], out var store))
            {
                store = 10;
            }
            StoreInterval = TimeSpan.FromMinutes(store);

            if (!int.TryParse(configuration["sms:keycount"], out AttemptCount))
            {
                AttemptCount = 5;
            }
        }

        private string BuildCacheKey(string phone)
        {
            var tenant = TenantManager.GetCurrentTenant(false);
            var tenantCache = tenant == null ? Tenant.DefaultTenant : tenant.Id;
            return "smskey" + phone + tenantCache;
        }


        public bool GenerateKey(string phone, out string key)
        {
            ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(phone);

            lock (KeyLocker)
            {
                var cacheKey = BuildCacheKey(phone);
                var phoneKeys = KeyCache.Get<Dictionary<string, DateTime>>(cacheKey) ?? new Dictionary<string, DateTime>();
                if (phoneKeys.Count > AttemptCount)
                {
                    key = null;
                    return false;
                }

                key = RandomNumberGenerator.GetInt32((int)Math.Pow(10, KeyLength - 1), (int)Math.Pow(10, KeyLength)).ToString(CultureInfo.InvariantCulture);
                phoneKeys[key] = DateTime.UtcNow;

                KeyCache.Insert(cacheKey, phoneKeys, DateTime.UtcNow.Add(StoreInterval));
                return true;
            }
        }

        public bool ExistsKey(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return false;
            }

            lock (KeyLocker)
            {
                var cacheKey = BuildCacheKey(phone);
                var phoneKeys = KeyCache.Get<Dictionary<string, DateTime>>(cacheKey);
                return phoneKeys != null;
            }
        }


        public Result ValidateKey(string phone, string key)
        {
            key = (key ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(key))
            {
                return Result.Empty;
            }

            var cacheCheck = BuildCacheKey("check" + phone);
            int.TryParse(CheckCache.Get<string>(cacheCheck), out var counter);
            if (++counter > AttemptCount)
                return Result.TooMuch;
            CheckCache.Insert(cacheCheck, counter.ToString(CultureInfo.InvariantCulture), DateTime.UtcNow.Add(StoreInterval));

            lock (KeyLocker)
            {
                var cacheKey = BuildCacheKey(phone);
                var phoneKeys = KeyCache.Get<Dictionary<string, DateTime>>(cacheKey);
                if (phoneKeys == null)
                    return Result.Timeout;

                if (!phoneKeys.ContainsKey(key))
                    return Result.Invalide;

                var createDate = phoneKeys[key];
                SmsKeyStorageCache.RemoveFromCache(cacheKey);
                if (createDate.Add(StoreInterval) < DateTime.UtcNow)
                    return Result.Timeout;

                CheckCache.Insert(cacheCheck, (counter - 1).ToString(CultureInfo.InvariantCulture), DateTime.UtcNow.Add(StoreInterval));
                return Result.Ok;
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
}