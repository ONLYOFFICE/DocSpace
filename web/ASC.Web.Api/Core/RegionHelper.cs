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

namespace ASC.Web.Api.Core;

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

    public RegionInfo GetCurrentRegionInfo()
    {
        var priceInfo = _tenantManager.GetProductPriceInfo();
        var defaultRegion = GetDefaultRegionInfo();

        var countryCode = _httpContextAccessor.HttpContext.Request.Query["country"];

        var currentRegion = GetRegionInfo(countryCode) ?? FindRegionInfo();

        if (currentRegion != null && !currentRegion.Name.Equals(defaultRegion.Name))
        {
            if (priceInfo.Values.Any(value => value.ContainsKey(currentRegion.ISOCurrencySymbol)))
            {
                return currentRegion;
            }
        }

        return defaultRegion;
    }
    public RegionInfo GetDefaultRegionInfo()
    {
        return GetRegionInfo("US");
    }

    public string GetCurrencyFromRequest()
    {
        var regionInfo = GetDefaultRegionInfo();
        var geoinfo = _geolocationHelper.GetIPGeolocationFromHttpContext();

        if (!string.IsNullOrEmpty(geoinfo.Key))
        {
            try
            {
                regionInfo = new RegionInfo(geoinfo.Key);
            }
            catch (Exception)
            {
            }
        }
        return regionInfo.ISOCurrencySymbol;
    }

    private RegionInfo FindRegionInfo()
    {
        RegionInfo regionInfo = null;

        var tenant = _tenantManager.GetCurrentTenant();
        var geoinfo = _geolocationHelper.GetIPGeolocationFromHttpContext();

        if (geoinfo != null)
        {
            regionInfo = GetRegionInfo(geoinfo.Key);
        }

        if (regionInfo == null)
        {
            var owner = _userManager.GetUsers(tenant.OwnerId);
            var culture = string.IsNullOrEmpty(owner.CultureName) ? tenant.GetCulture() : owner.GetCulture();
            regionInfo = GetRegionInfo(culture.Name);
        }

        return regionInfo;
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
