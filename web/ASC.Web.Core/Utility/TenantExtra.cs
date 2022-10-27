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

[Singletone]
public class TenantExtraConfig
{
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly LicenseReaderConfig _licenseReaderConfig;

    public TenantExtraConfig(CoreBaseSettings coreBaseSettings, LicenseReaderConfig licenseReaderConfig)
    {
        _coreBaseSettings = coreBaseSettings;
        _licenseReaderConfig = licenseReaderConfig;
    }

    public bool Saas
    {
        get { return !_coreBaseSettings.Standalone; }
    }

    public bool Enterprise
    {
        get { return _coreBaseSettings.Standalone && !string.IsNullOrEmpty(_licenseReaderConfig.LicensePath); }
    }

    public bool Opensource
    {
        get { return _coreBaseSettings.Standalone && string.IsNullOrEmpty(_licenseReaderConfig.LicensePath); }
    }
}

[Scope]
public class TenantExtra
{
    private readonly TenantExtraConfig _tenantExtraConfig;
    private readonly CountRoomAdminStatistic _countRoomAdminStatistic;
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
        CountRoomAdminStatistic countManagerStatistic,
        MaxTotalSizeStatistic maxTotalSizeStatistic)
    {
        _tenantManager = tenantManager;
        _tariffService = tariffService;
        _coreBaseSettings = coreBaseSettings;
        _licenseReader = licenseReader;
        _setupInfo = setupInfo;
        _settingsManager = settingsManager;
        _tenantExtraConfig = tenantExtraConfig;
        _countRoomAdminStatistic = countManagerStatistic;
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

    public bool EnterprisePaid(bool withRequestToPaymentSystem = true)
    {
        return Enterprise && GetCurrentTariff(withRequestToPaymentSystem).State < TariffState.NotPaid;
    }

    public Tariff GetCurrentTariff(bool withRequestToPaymentSystem = true)
    {
        return _tariffService.GetTariff(_tenantManager.GetCurrentTenant().Id, withRequestToPaymentSystem);
    }

    public IEnumerable<TenantQuota> GetTenantQuotas()
    {
        return _tenantManager.GetTenantQuotas();
    }


    public async Task<TenantQuota> GetRightQuota()
    {
        var usedSpace = await _maxTotalSizeStatistic.GetValue();
        var needUsersCount = await _countRoomAdminStatistic.GetValue();
        var quotas = GetTenantQuotas();

        return quotas.OrderBy(q => q.CountUser)
                     .FirstOrDefault(q =>
                                     q.CountUser > needUsersCount
                                     && q.MaxTotalSize > usedSpace
                                     && !q.Free
                                     && !q.Trial);
    }

    public bool IsNotPaid(bool withRequestToPaymentSystem = true)
    {
        Tariff tariff;
        return EnableTariffSettings
               && ((tariff = GetCurrentTariff(withRequestToPaymentSystem)).State >= TariffState.NotPaid
                   || Enterprise && !EnterprisePaid(withRequestToPaymentSystem) && tariff.LicenseDate == DateTime.MaxValue);
    }

    /// <summary>
    /// Max possible file size for not chunked upload. Less or equal than 100 mb.
    /// </summary>
    public long MaxUploadSize
    {
        get { return Math.Min(_setupInfo.AvailableFileSize, MaxChunkedUploadSize); }
    }

    /// <summary>
    /// Max possible file size for chunked upload.
    /// </summary>
    public long MaxChunkedUploadSize
    {
        get
        {
            var diskQuota = _tenantManager.GetCurrentTenantQuota();
            if (diskQuota != null)
            {
                var usedSize = _maxTotalSizeStatistic.GetValue().Result;
                var freeSize = Math.Max(diskQuota.MaxTotalSize - usedSize, 0);
                return Math.Min(freeSize, diskQuota.MaxFileSize);
            }
            return _setupInfo.ChunkUploadSize;
        }
    }
}
