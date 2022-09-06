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

using ASC.Core.Common.Quota;

namespace ASC.Web.Api.Core;

[Scope]
public class QuotaHelper
{
    private readonly TenantManager _tenantManager;
    private readonly RegionHelper _regionHelper;
    private readonly IServiceProvider _serviceProvider;

    public QuotaHelper(TenantManager tenantManager, RegionHelper regionHelper, IServiceProvider serviceProvider)
    {
        _tenantManager = tenantManager;
        _regionHelper = regionHelper;
        _serviceProvider = serviceProvider;
    }

    public IEnumerable<QuotaDto> GetQuotas()
    {
        var quotaList = _tenantManager.GetTenantQuotas(false);
        var priceInfo = _tenantManager.GetProductPriceInfo();
        var currentRegion = _regionHelper.GetCurrentRegionInfo();

        return quotaList.Select(x => ToQuotaDto(x, priceInfo, currentRegion)).ToList();
    }

    private QuotaDto ToQuotaDto(TenantQuota tenantQuota, IDictionary<string, Dictionary<string, decimal>> priceInfo, RegionInfo currentRegion)
    {
        var features = GetFeatures(tenantQuota, priceInfo, currentRegion);

        return new QuotaDto
        {
            Id = tenantQuota.Tenant,
            Title = Resource.ResourceManager.GetString($"Tariffs_{tenantQuota.Name}"),

            NonProfit = tenantQuota.NonProfit,
            Free = tenantQuota.Free,
            Trial = tenantQuota.Trial,

            Features = features
        };
    }
    private decimal GetPrice(TenantQuota quota, IDictionary<string, Dictionary<string, decimal>> priceInfo, RegionInfo currentRegion)
    {
        if (!string.IsNullOrEmpty(quota.ProductId) && priceInfo.ContainsKey(quota.ProductId))
        {
            var prices = priceInfo[quota.ProductId];
            if (prices.ContainsKey(currentRegion.ISOCurrencySymbol))
            {
                return prices[currentRegion.ISOCurrencySymbol];
            }
        }
        return quota.Price;
    }

    private string GetPriceString(decimal price, RegionInfo currentRegion)
    {
        var inEuro = "EUR".Equals(currentRegion.ISOCurrencySymbol);

        var priceString = inEuro && Math.Truncate(price) != price ?
            price.ToString(CultureInfo.InvariantCulture) :
            ((int)price).ToString(CultureInfo.InvariantCulture);

        return string.Format("{0}{1}", currentRegion.CurrencySymbol, priceString);
    }

    private IEnumerable<QuotaFeatureDto> GetFeatures(TenantQuota tenantQuota, IDictionary<string, Dictionary<string, decimal>> priceInfo, RegionInfo currentRegion)
    {
        var assembly = GetType().Assembly;

        var features = tenantQuota.Features.Split(' ', ',', ';');

        foreach (var feature in tenantQuota.TenantQuotaFeatures.Where(r => r.Visible).OrderBy(r => r.Order))
        {
            var result = new QuotaFeatureDto();

            if (feature.Paid || features.Length == 1)
            {
                var val = GetPrice(tenantQuota, priceInfo, currentRegion);
                result.Price = new FeaturePriceDto
                {
                    Value = val,
                    CurrencySymbol = currentRegion.CurrencySymbol,
                    Per = string.Format(Resource.ResourceManager.GetString($"TariffsFeature_{feature.Name}_price_per"), GetPriceString(val, currentRegion)),
                    Count = Resource.ResourceManager.GetString($"TariffsFeature_{feature.Name}_price_count"),
                    Range = new FeaturePriceRangeDto
                    {
                        Value = 1,
                        Min = 1,
                        Max = 999,
                    }
                };
            }

            result.Id = feature.Name;
            result.Title = Resource.ResourceManager.GetString($"TariffsFeature_{feature.Name}");

            if (feature.Name == "total_size") //TODO
            {
                result.Title = string.Format(result.Title, FileSizeComment.FilesSizeToString(tenantQuota.MaxTotalSize));
            }

            var img = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.img.{feature.Name}.svg");

            if (img != null)
            {
                try
                {
                    using var memoryStream = new MemoryStream();
                    img.CopyTo(memoryStream);
                    result.Image = Encoding.UTF8.GetString(memoryStream.ToArray());
                }
                catch (Exception)
                {

                }
            }

            var statisticProvider = (ITenantQuotaFeatureStatistic)_serviceProvider.GetService(typeof(ITenantQuotaFeatureStatistic<>).MakeGenericType(feature.GetType()));

            if (statisticProvider != null)
            {
                result.Used = new FeatureUsedDto
                {
                    Value = statisticProvider.GetValue(),
                    Title = Resource.ResourceManager.GetString($"TariffsFeature_used_{feature.Name}")
                };
            }

            yield return result;
        }
    }
}
