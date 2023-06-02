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

namespace ASC.IPSecurity;

[Singletone]
public class IPRestrictionsServiceCache
{
    public ICache Cache { get; set; }

    private const string CacheKey = "iprestrictions";

    internal readonly ICacheNotify<IPRestrictionItem> Notify;

    public IPRestrictionsServiceCache(ICacheNotify<IPRestrictionItem> notify, ICache cache)
    {
        Cache = cache;
        notify.Subscribe((r) => Cache.Remove(GetCacheKey(r.TenantId)), CacheNotifyAction.InsertOrUpdate);
        Notify = notify;
    }

    public static string GetCacheKey(int tenant)
    {
        return CacheKey + tenant;
    }
}

[Scope]
public class IPRestrictionsService
{
    private readonly ICache _cache;
    private readonly ICacheNotify<IPRestrictionItem> _notify;
    private readonly IPRestrictionsRepository _ipRestrictionsRepository;
    private static readonly TimeSpan _timeout = TimeSpan.FromMinutes(5);

    public IPRestrictionsService(
        IPRestrictionsRepository iPRestrictionsRepository,
        IPRestrictionsServiceCache iPRestrictionsServiceCache)
    {
        _ipRestrictionsRepository = iPRestrictionsRepository;
        _cache = iPRestrictionsServiceCache.Cache;
        _notify = iPRestrictionsServiceCache.Notify;
    }

    public async Task<IEnumerable<IPRestriction>> GetAsync(int tenant)
    {
        var key = IPRestrictionsServiceCache.GetCacheKey(tenant);
        var restrictions = _cache.Get<List<IPRestriction>>(key);
        if (restrictions == null)
        {
            restrictions = await _ipRestrictionsRepository.GetAsync(tenant);
            _cache.Insert(key, restrictions, _timeout);
        }

        return restrictions;
    }

    public async Task<IEnumerable<IpRestrictionBase>> SaveAsync(IEnumerable<IpRestrictionBase> ips, int tenant)
    {
        var restrictions = await _ipRestrictionsRepository.SaveAsync(ips, tenant);
        _notify.Publish(new IPRestrictionItem { TenantId = tenant }, CacheNotifyAction.InsertOrUpdate);

        return restrictions;
    }
}
