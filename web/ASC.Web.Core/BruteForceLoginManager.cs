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

using Microsoft.Extensions.Caching.Distributed;

using ProtoBuf;

namespace ASC.Web.Core;
[Scope]
public class BruteForceLoginManager
{
    private readonly SettingsManager _settingsManager;
    private readonly IDistributedCache _distributedCache;
    private static readonly object _lock = new object();

    public BruteForceLoginManager(SettingsManager settingsManager, IDistributedCache distributedCache)
    {
        _settingsManager = settingsManager;
        _distributedCache = distributedCache;
    }

    public bool Increment(string login, string requestIp, out bool showRecaptcha)
    {
        showRecaptcha = true;

        lock (_lock)
        {
            var blockCacheKey = GetBlockCacheKey(login, requestIp);

            if (GetFromCache<bool>(blockCacheKey))
            {
                return false;
            }

            var historyCacheKey = GetHistoryCacheKey(login, requestIp);

            var history = GetFromCache<List<DateTime>>(historyCacheKey) ?? new List<DateTime>();

            var now = DateTime.UtcNow;

            var settings = _settingsManager.Load<LoginSettings>();

            var checkTime = now.Subtract(TimeSpan.FromSeconds(settings.CheckPeriod));

            history = history.Where(item => item > checkTime).ToList();

            history.Add(now);

            showRecaptcha = history.Count > settings.AttemptCount - 1;

            if (history.Count > settings.AttemptCount)
            {
                SetToCache(blockCacheKey, "block", now.Add(TimeSpan.FromSeconds(settings.BlockTime)));
                _distributedCache.Remove(historyCacheKey);
                return false;
            }

            SetToCache(historyCacheKey, history, now.Add(TimeSpan.FromSeconds(settings.CheckPeriod)));
        }
        return true;
    }

    public void Decrement(string login, string requestIp)
    {
        var historyCacheKey = GetHistoryCacheKey(login, requestIp);

        lock (_lock)
        {
            var history = GetFromCache<List<DateTime>>(historyCacheKey) ?? new List<DateTime>();

            if (history.Count > 0)
            {
                history.RemoveAt(history.Count - 1);
            }

            var settings = _settingsManager.Load<LoginSettings>();

            SetToCache(historyCacheKey, history, DateTime.UtcNow.Add(TimeSpan.FromSeconds(settings.CheckPeriod)));
        }
    }

    private T GetFromCache<T>(string key)
    {
        var serializedObject = _distributedCache.Get(key);

        if (serializedObject == null)
        {
            return default(T);
        }

        using var ms = new MemoryStream(serializedObject);

        return Serializer.Deserialize<T>(ms);
    }

    private void SetToCache<T>(string key, T obj, DateTime ExpirationPeriod)
    {
        using var ms = new MemoryStream();

        Serializer.Serialize(ms, obj);

        _distributedCache.Set(key, ms.ToArray(), new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = ExpirationPeriod
        });
    }

    private string GetBlockCacheKey(string login, string requestIp)
    {
        return "loginblock/" + login + requestIp;
    }

    private string GetHistoryCacheKey(string login, string requestIp)
    {
        return "loginsec/" + login + requestIp;
    }
}
