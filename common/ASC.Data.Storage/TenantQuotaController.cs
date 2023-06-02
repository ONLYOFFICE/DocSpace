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

namespace ASC.Data.Storage;

[Transient]
public class TenantQuotaController : IQuotaController
{
    private long CurrentSize
    {
        get
        {
            if (!_lazyCurrentSize.IsValueCreated)
            {
                return _currentSize = _lazyCurrentSize.Value;
            }

            return _currentSize;
        }
        set => _currentSize = value;
    }

    private int _tenant;
    private readonly TenantManager _tenantManager;
    private readonly AuthContext _authContext;
    private readonly TenantQuotaFeatureChecker<MaxFileSizeFeature, long> _maxFileSizeChecker;
    private readonly TenantQuotaFeatureChecker<MaxTotalSizeFeature, long> _maxTotalSizeChecker;
    private Lazy<long> _lazyCurrentSize;
    private long _currentSize;

    public TenantQuotaController(TenantManager tenantManager, AuthContext authContext, TenantQuotaFeatureChecker<MaxFileSizeFeature, long> maxFileSizeChecker, TenantQuotaFeatureChecker<MaxTotalSizeFeature, long> maxTotalSizeChecker)
    {
        _tenantManager = tenantManager;
        _maxFileSizeChecker = maxFileSizeChecker;
        _maxTotalSizeChecker = maxTotalSizeChecker;
        _authContext = authContext;
    }
    
    public void Init(int tenant)
    {
        _tenant = tenant;
        _lazyCurrentSize = new Lazy<long>(() => _tenantManager.FindTenantQuotaRowsAsync(tenant).Result
            .Where(r => UsedInQuota(r.Tag))
            .Sum(r => r.Counter));
    }

    public async Task QuotaUsedAddAsync(string module, string domain, string dataTag, long size, bool quotaCheckFileSize = true)
    {
        size = Math.Abs(size);
        if (UsedInQuota(dataTag))
        {
            await QuotaUsedCheckAsync(size, quotaCheckFileSize);
            CurrentSize += size;
        }

        await SetTenantQuotaRowAsync(module, domain, size, dataTag, true, Guid.Empty);
        await SetTenantQuotaRowAsync(module, domain, size, dataTag, true, _authContext.CurrentAccount.ID);
    }

    public async Task QuotaUsedDeleteAsync(string module, string domain, string dataTag, long size)
    {
        size = -Math.Abs(size);
        if (UsedInQuota(dataTag))
        {
            CurrentSize += size;
        }

        await SetTenantQuotaRowAsync(module, domain, size, dataTag, true, Guid.Empty);
        await SetTenantQuotaRowAsync(module, domain, size, dataTag, true, _authContext.CurrentAccount.ID);
    }

    public async Task QuotaUsedSetAsync(string module, string domain, string dataTag, long size)
    {
        size = Math.Max(0, size);
        if (UsedInQuota(dataTag))
        {
            CurrentSize += size;
        }

        await SetTenantQuotaRowAsync(module, domain, size, dataTag, false, Guid.Empty);
    }

    public async Task QuotaUsedCheckAsync(long size)
    {
        await QuotaUsedCheckAsync(size, true);
    }

    public async Task QuotaUsedCheckAsync(long size, bool quotaCheckFileSize)
    {
        var quota = await _tenantManager.GetTenantQuotaAsync(_tenant);
        if (quota != null)
        {
            if (quota.MaxFileSize != 0 && quotaCheckFileSize)
            {
                await _maxFileSizeChecker.CheckAddAsync(_tenant, size);
            }

            if (quota.MaxTotalSize != 0)
            {
                await _maxTotalSizeChecker.CheckAddAsync(_tenant, CurrentSize + size);
            }
        }
    }


    private async Task SetTenantQuotaRowAsync(string module, string domain, long size, string dataTag, bool exchange, Guid userId)
    {
        await _tenantManager.SetTenantQuotaRowAsync(
            new TenantQuotaRow { Tenant = _tenant, Path = $"/{module}/{domain}", Counter = size, Tag = dataTag, UserId = userId, LastModified = DateTime.UtcNow },
            exchange);

    }

    private bool UsedInQuota(string tag)
    {
        return !string.IsNullOrEmpty(tag) && new Guid(tag) != Guid.Empty;
    }
}
