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

namespace ASC.Web.Core.Mobile;

public class CachedMobileAppInstallRegistrator : IMobileAppInstallRegistrator
{
    private readonly ICache _cache;
    private readonly TimeSpan _cacheExpiration;
    private readonly IMobileAppInstallRegistrator _registrator;
    private readonly TenantManager _tenantManager;

    public CachedMobileAppInstallRegistrator(MobileAppInstallRegistrator registrator, TenantManager tenantManager, ICache cache)
        : this(registrator, TimeSpan.FromMinutes(30), tenantManager, cache)
    {
    }

    public CachedMobileAppInstallRegistrator(MobileAppInstallRegistrator registrator, TimeSpan cacheExpiration, TenantManager tenantManager, ICache cache)
    {
        _cache = cache;
        _tenantManager = tenantManager;
        this._registrator = registrator ?? throw new ArgumentNullException(nameof(registrator));
        this._cacheExpiration = cacheExpiration;
    }

    public async Task RegisterInstallAsync(string userEmail, MobileAppType appType)
    {
        if (string.IsNullOrEmpty(userEmail))
        {
            return;
        }

        await _registrator.RegisterInstallAsync(userEmail, appType);
        _cache.Insert(await GetCacheKeyAsync(userEmail, null), true, _cacheExpiration);
        _cache.Insert(await GetCacheKeyAsync(userEmail, appType), true, _cacheExpiration);
    }

    public async Task<bool> IsInstallRegisteredAsync(string userEmail, MobileAppType? appType)
    {
        if (string.IsNullOrEmpty(userEmail))
        {
            return false;
        }

        var fromCache = _cache.Get<string>(await GetCacheKeyAsync(userEmail, appType));


        if (bool.TryParse(fromCache, out var cachedValue))
        {
            return cachedValue;
        }

        var isRegistered = await _registrator.IsInstallRegisteredAsync(userEmail, appType);
        _cache.Insert(await GetCacheKeyAsync(userEmail, appType), isRegistered.ToString(), _cacheExpiration);
        return isRegistered;
    }

    private async Task<string> GetCacheKeyAsync(string userEmail, MobileAppType? appType)
    {
        var cacheKey = appType.HasValue ? userEmail + "/" + appType.ToString() : userEmail;

        return string.Format("{0}:mobile:{1}", await _tenantManager.GetCurrentTenantIdAsync(), cacheKey);
    }
}
