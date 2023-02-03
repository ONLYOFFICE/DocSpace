﻿// (c) Copyright Ascensio System SIA 2010-2022
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

    public QuotaDto GetCurrentQuota()
    {
        var quota = _tenantManager.GetCurrentTenantQuota();
        var priceInfo = _tenantManager.GetProductPriceInfo();
        var currentRegion = _regionHelper.GetCurrentRegionInfo();

        return ToQuotaDto(quota, priceInfo, currentRegion, true);
    }

    private QuotaDto ToQuotaDto(TenantQuota quota, IDictionary<string, Dictionary<string, decimal>> priceInfo, RegionInfo currentRegion, bool getUsed = false)
    {
        var price = GetPrice(quota, priceInfo, currentRegion);
        var features = GetFeatures(quota, getUsed);

        return new QuotaDto
        {
            Id = quota.TenantId,
            Title = Resource.ResourceManager.GetString($"Tariffs_{quota.Name}"),

            NonProfit = quota.NonProfit,
            Free = quota.Free,
            Trial = quota.Trial,

            Price = new PriceDto
            {
                Value = price,
                CurrencySymbol = currentRegion.CurrencySymbol
            },

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

    private async IAsyncEnumerable<TenantQuotaFeatureDto> GetFeatures(TenantQuota quota, bool getUsed)
    {
        var assembly = GetType().Assembly;

        var features = quota.Features.Split(' ', ',', ';');

        foreach (var feature in quota.TenantQuotaFeatures.Where(r => r.Visible).OrderBy(r => r.Order))
        {
            var result = new TenantQuotaFeatureDto();

            if (feature.Paid)
            {
                result.PriceTitle = Resource.ResourceManager.GetString($"TariffsFeature_{feature.Name}_price_count");
            }

            result.Id = feature.Name;

            object used = null;

            if (feature is TenantQuotaFeatureSize size)
            {
                result.Value = size.Value == long.MaxValue ? -1 : size.Value;
                result.Type = "size";

                await GetStat<long>();
            }
            else if (feature is TenantQuotaFeatureCount count)
            {
                result.Value = count.Value == int.MaxValue ? -1 : count.Value;
                result.Type = "count";

                await GetStat<int>();
            }
            else if (feature is TenantQuotaFeatureFlag flag)
            {
                result.Value = flag.Value;
                result.Type = "flag";
            }

            if (getUsed)
            {
                if (used != null)
                {
                    result.Used = new FeatureUsedDto
                    {
                        Value = used,
                        Title = Resource.ResourceManager.GetString($"TariffsFeature_used_{feature.Name}")
                    };
                }
            }
            else
            {
                result.Title = Resource.ResourceManager.GetString($"TariffsFeature_{feature.Name}");
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
            }

            yield return result;

            async Task GetStat<T>()
            {
                var statisticProvider = (ITenantQuotaFeatureStat<T>)_serviceProvider.GetService(typeof(ITenantQuotaFeatureStat<,>).MakeGenericType(feature.GetType(), typeof(T)));

                if (statisticProvider != null)
                {
                    used = await statisticProvider.GetValue();
                }
            }
        }
    }
}
