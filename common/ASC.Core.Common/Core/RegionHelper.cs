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

namespace ASC.Core;

[Scope]
public class RegionHelper
{
    private readonly TenantManager _tenantManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly GeolocationHelper _geolocationHelper;
    private readonly UserManager _userManager;

    public RegionHelper(
        TenantManager tenantManager,
        IHttpContextAccessor httpContextAccessor,
        GeolocationHelper geolocationHelper,
        UserManager userManager)
    {
        _tenantManager = tenantManager;
        _httpContextAccessor = httpContextAccessor;
        _geolocationHelper = geolocationHelper;
        _userManager = userManager;
    }

    public async Task<RegionInfo> GetCurrentRegionInfoAsync(IDictionary<string, Dictionary<string, decimal>> priceInfo = null)
    {
        var defaultRegion = GetDefaultRegionInfo();
        var geoinfo = await _geolocationHelper.GetIPGeolocationFromHttpContextAsync();

        var countryCode = _httpContextAccessor.HttpContext?.Request.Query["country"];

        var currentRegion = GetRegionInfo(countryCode);

        if (currentRegion == null)
        {
            if (geoinfo != null)
            {
                currentRegion = GetRegionInfo(geoinfo.Key);
            }

            if (currentRegion == null)
            {
                var tenant = _tenantManager.GetCurrentTenant(false);
                if (tenant != null)
                {
                    var owner = _userManager.GetUsers(tenant.OwnerId);
                    var culture = string.IsNullOrEmpty(owner.CultureName) ? tenant.GetCulture() : owner.GetCulture();
                    currentRegion = GetRegionInfo(culture.Name);
                }
            }
        }

        if (currentRegion != null && !currentRegion.Name.Equals(defaultRegion.Name))
        {
            priceInfo ??= await _tenantManager.GetProductPriceInfoAsync();

            if (priceInfo.Values.Any(value => value.ContainsKey(currentRegion.ISOCurrencySymbol)))
            {
                return currentRegion;
            }

            if (geoinfo != null && !string.IsNullOrEmpty(geoinfo.Continent) && geoinfo.Continent == "EU")
            {
                return GetRegionInfo("ES");
            }
        }

        return defaultRegion;
    }

    public RegionInfo GetDefaultRegionInfo()
    {
        return GetRegionInfo("US");
    }

    public async Task<string> GetCurrencyFromRequestAsync()
    {
        var defaultRegion = GetDefaultRegionInfo();
        var geoinfo = await _geolocationHelper.GetIPGeolocationFromHttpContextAsync();

        if (!string.IsNullOrEmpty(geoinfo.Key))
        {
            try
            {
                var currentRegion = new RegionInfo(geoinfo.Key);

                if (currentRegion != null && !currentRegion.Name.Equals(defaultRegion.Name))
                {
                    var priceInfo = await _tenantManager.GetProductPriceInfoAsync();

                    if (priceInfo.Values.Any(value => value.ContainsKey(currentRegion.ISOCurrencySymbol)))
                    {
                        return currentRegion.ISOCurrencySymbol;
                    }

                    if (!string.IsNullOrEmpty(geoinfo.Continent) && geoinfo.Continent == "EU")
                    {
                        return "EUR";
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        return defaultRegion.ISOCurrencySymbol;
    }

    private RegionInfo GetRegionInfo(string isoTwoLetterCountryCode)
    {
        RegionInfo regionInfo = null;

        if (!string.IsNullOrEmpty(isoTwoLetterCountryCode))
        {
            try
            {
                regionInfo = new RegionInfo(isoTwoLetterCountryCode);
            }
            catch
            {
            }
        }

        return regionInfo;
    }
}
