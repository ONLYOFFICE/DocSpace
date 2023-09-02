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

namespace ASC.Core.Common.Quota;
public interface ITenantQuotaFeatureChecker
{
    public Task CheckUsed(TenantQuota value);
    string Exception { get; }
}


public abstract class TenantQuotaFeatureChecker<T, T1> : ITenantQuotaFeatureChecker where T : TenantQuotaFeature<T1> where T1 : IComparable<T1>
{
    private static readonly object _lock = new object();
    protected readonly ITenantQuotaFeatureStat<T, T1> _tenantQuotaFeatureStatistic;
    protected readonly TenantManager _tenantManager;

    public abstract string Exception { get; }

    public TenantQuotaFeatureChecker(ITenantQuotaFeatureStat<T, T1> tenantQuotaFeatureStatistic, TenantManager tenantManager)
    {
        _tenantQuotaFeatureStatistic = tenantQuotaFeatureStatistic;
        _tenantManager = tenantManager;
    }

    public virtual async Task CheckUsed(TenantQuota quota)
    {
        var used = await _tenantQuotaFeatureStatistic.GetValueAsync();
        Check(quota, used);
    }

    public async Task CheckAddAsync(T1 newValue)
    {
        await CheckAddAsync(await _tenantManager.GetCurrentTenantIdAsync(), newValue);
    }

    public virtual async Task CheckAddAsync(int tenantId, T1 newValue)
    {
        var quota = await _tenantManager.GetTenantQuotaAsync(tenantId);
        Check(quota, newValue);
    }

    protected void Check(TenantQuota quota, T1 newValue)
    {
        lock (_lock)
        {
            var val = quota.GetFeature<T>().Value;

            if (newValue.CompareTo(val) > 0)
            {
                throw new TenantQuotaException(string.Format(Exception, val));
            }
        }
    }
}

public abstract class TenantQuotaFeatureCheckerCount<T> : TenantQuotaFeatureChecker<T, int> where T : TenantQuotaFeature<int>
{
    protected TenantQuotaFeatureCheckerCount(ITenantQuotaFeatureStat<T, int> tenantQuotaFeatureStatistic, TenantManager tenantManager) : base(tenantQuotaFeatureStatistic, tenantManager)
    {
    }

    public async Task CheckAppend()
    {
        await CheckAddAsync((await _tenantQuotaFeatureStatistic.GetValueAsync()) + 1);
    }
}