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
    private readonly UserManager _userManager;
    private readonly TenantStatisticsProvider _tenantStatisticsProvider;
    private readonly AuthContext _authContext;
    private readonly TenantManager _tenantManager;
    private readonly PaymentManager _paymentManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly LicenseReader _licenseReader;
    private readonly SetupInfo _setupInfo;
    private readonly SettingsManager _settingsManager;

    public TenantExtra(
        UserManager userManager,
        TenantStatisticsProvider tenantStatisticsProvider,
        AuthContext authContext,
        TenantManager tenantManager,
        PaymentManager paymentManager,
        CoreBaseSettings coreBaseSettings,
        LicenseReader licenseReader,
        SetupInfo setupInfo,
        SettingsManager settingsManager,
        TenantExtraConfig tenantExtraConfig)
    {
        _userManager = userManager;
        _tenantStatisticsProvider = tenantStatisticsProvider;
        _authContext = authContext;
        _tenantManager = tenantManager;
        _paymentManager = paymentManager;
        _coreBaseSettings = coreBaseSettings;
        _licenseReader = licenseReader;
        _setupInfo = setupInfo;
        _settingsManager = settingsManager;
        _tenantExtraConfig = tenantExtraConfig;
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

    public bool EnterprisePaid
    {
        get { return Enterprise && GetCurrentTariff().State < TariffState.NotPaid; }
    }

    public bool EnableControlPanel
    {
        get
        {
            return _coreBaseSettings.Standalone &&
                !string.IsNullOrEmpty(_setupInfo.ControlPanelUrl) &&
                GetCurrentTariff().State < TariffState.NotPaid &&
                _userManager.GetUsers(_authContext.CurrentAccount.ID).IsAdmin(_userManager);
        }
    }

    public bool EnableDocbuilder
    {
        get { return !Opensource; }
    }
    public string GetAppsPageLink()
    {
        return VirtualPathUtility.ToAbsolute("~/AppInstall.aspx");
    }

    public string GetTariffPageLink()
    {
        return VirtualPathUtility.ToAbsolute("~/Tariffs.aspx");
    }

    public Tariff GetCurrentTariff()
    {
        return _paymentManager.GetTariff(_tenantManager.GetCurrentTenant().Id);
    }

    public TenantQuota GetTenantQuota()
    {
        return GetTenantQuota(_tenantManager.GetCurrentTenant().Id);
    }

    public TenantQuota GetTenantQuota(int tenant)
    {
        return _tenantManager.GetTenantQuota(tenant);
    }

    public IEnumerable<TenantQuota> GetTenantQuotas()
    {
        return _tenantManager.GetTenantQuotas();
    }

    private TenantQuota GetPrevQuota(TenantQuota curQuota)
    {
        TenantQuota prev = null;
        foreach (var quota in GetTenantQuotas().OrderBy(r => r.ActiveUsers))
        {
            if (quota.Tenant == curQuota.Tenant)
            {
                return prev;
            }

            prev = quota;
        }
        return null;
    }

    public int GetPrevUsersCount(TenantQuota quota)
    {
        var prevQuota = GetPrevQuota(quota);
        if (prevQuota == null || prevQuota.Trial)
        {
            return 1;
        }

        return prevQuota.ActiveUsers + 1;
    }

    public int GetRightQuotaId()
    {
        var q = GetRightQuota();
        return q != null ? q.Tenant : 0;
    }

    public TenantQuota GetRightQuota()
    {
        var usedSpace = _tenantStatisticsProvider.GetUsedSize();
        var needUsersCount = _tenantStatisticsProvider.GetUsersCount();
        var quotas = GetTenantQuotas();

        return quotas.OrderBy(q => q.ActiveUsers)
                     .FirstOrDefault(q =>
                                     q.ActiveUsers > needUsersCount
                                     && q.MaxTotalSize > usedSpace
                                     && !q.Free
                                     && !q.Trial);
    }

    public int GetRemainingCountUsers()
    {
        return GetTenantQuota().ActiveUsers - _tenantStatisticsProvider.GetUsersCount();
    }

    public void DemandControlPanelPermission()
    {
        if (!_coreBaseSettings.Standalone || _settingsManager.Load<TenantControlPanelSettings>().LimitedAccess)
        {
            throw new SecurityException(Resource.ErrorAccessDenied);
        }
    }

    public bool IsNotPaid()
    {
        Tariff tariff;
        return EnableTariffSettings
               && ((tariff = GetCurrentTariff()).State >= TariffState.NotPaid
                   || Enterprise && !EnterprisePaid && tariff.LicenseDate == DateTime.MaxValue);
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
            var diskQuota = GetTenantQuota();
            if (diskQuota != null)
            {
                var usedSize = _tenantStatisticsProvider.GetUsedSize();
                var freeSize = Math.Max(diskQuota.MaxTotalSize - usedSize, 0);
                return Math.Min(freeSize, diskQuota.MaxFileSize);
            }
            return _setupInfo.ChunkUploadSize;
        }
    }
}
