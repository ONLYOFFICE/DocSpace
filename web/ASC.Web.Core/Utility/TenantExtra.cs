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

namespace ASC.Web.Studio.Utility;

[Scope]
public class TenantExtra
{
    private readonly TenantExtraConfig _tenantExtraConfig;
    private readonly CountPaidUserStatistic _countPaidUserStatistic;
    private readonly MaxTotalSizeStatistic _maxTotalSizeStatistic;
    private readonly TenantManager _tenantManager;
    private readonly ITariffService _tariffService;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly LicenseReader _licenseReader;
    private readonly SetupInfo _setupInfo;
    private readonly SettingsManager _settingsManager;

    public TenantExtra(
        TenantManager tenantManager,
        ITariffService tariffService,
        CoreBaseSettings coreBaseSettings,
        LicenseReader licenseReader,
        SetupInfo setupInfo,
        SettingsManager settingsManager,
        TenantExtraConfig tenantExtraConfig,
        CountPaidUserStatistic countPaidUserStatistic,
        MaxTotalSizeStatistic maxTotalSizeStatistic)
    {
        _tenantManager = tenantManager;
        _tariffService = tariffService;
        _coreBaseSettings = coreBaseSettings;
        _licenseReader = licenseReader;
        _setupInfo = setupInfo;
        _settingsManager = settingsManager;
        _tenantExtraConfig = tenantExtraConfig;
        _countPaidUserStatistic = countPaidUserStatistic;
        _maxTotalSizeStatistic = maxTotalSizeStatistic;
    }

    public bool EnableTariffSettings
    {
        get
        {
            return
                SetupInfo.IsVisibleSettings<TariffSettings>()
                && !_settingsManager.Load<TenantAccessSettings>().Anyone
                && (!_coreBaseSettings.Standalone || !string.IsNullOrEmpty(_licenseReader.LicensePath))
                && string.IsNullOrEmpty(_setupInfo.AmiMetaUrl);
        }
    }

    public bool Saas
    {
        get => _tenantExtraConfig.Saas;
    }

    public bool Enterprise
    {
        get => _tenantExtraConfig.Enterprise;
    }

    public bool Opensource
    {
        get => _tenantExtraConfig.Opensource;
    }

    public async Task<bool> EnterprisePaidAsync(bool withRequestToPaymentSystem = true)
    {
        return Enterprise && (await GetCurrentTariffAsync(withRequestToPaymentSystem)).State < TariffState.NotPaid;
    }

    public async Task<Tariff> GetCurrentTariffAsync(bool withRequestToPaymentSystem = true)
    {
        return await _tariffService.GetTariffAsync(await _tenantManager.GetCurrentTenantIdAsync(), withRequestToPaymentSystem);
    }

    public async Task<IEnumerable<TenantQuota>> GetTenantQuotasAsync()
    {
        return await _tenantManager.GetTenantQuotasAsync();
    }


    public async Task<TenantQuota> GetRightQuota()
    {
        var usedSpace = await _maxTotalSizeStatistic.GetValueAsync();
        var needUsersCount = await _countPaidUserStatistic.GetValueAsync();
        var quotas = await GetTenantQuotasAsync();

        return quotas.OrderBy(q => q.CountUser)
                     .FirstOrDefault(q =>
                                     q.CountUser > needUsersCount
                                     && q.MaxTotalSize > usedSpace
                                     && !q.Free
                                     && !q.Trial);
    }

    public async Task<bool> IsNotPaidAsync(bool withRequestToPaymentSystem = true)
    {
        Tariff tariff;
        return EnableTariffSettings
               && ((tariff = (await GetCurrentTariffAsync(withRequestToPaymentSystem))).State >= TariffState.NotPaid
                   || Enterprise && !(await EnterprisePaidAsync(withRequestToPaymentSystem)) && tariff.LicenseDate == DateTime.MaxValue);
    }

    /// <summary>
    /// Max possible file size for not chunked upload. Less or equal than 100 mb.
    /// </summary>
    public async Task<long> GetMaxUploadSizeAsync()
    {
        return Math.Min(_setupInfo.AvailableFileSize, await GetMaxChunkedUploadSizeAsync());
    }

    /// <summary>
    /// Max possible file size for chunked upload.
    /// </summary>
    public async Task<long> GetMaxChunkedUploadSizeAsync()
    {
        var diskQuota = await _tenantManager.GetCurrentTenantQuotaAsync();
        if (diskQuota != null)
        {
            var usedSize = await _maxTotalSizeStatistic.GetValueAsync();
            var freeSize = Math.Max(diskQuota.MaxTotalSize - usedSize, 0);
            return Math.Min(freeSize, diskQuota.MaxFileSize);
        }
        return _setupInfo.ChunkUploadSize;
    }
}
