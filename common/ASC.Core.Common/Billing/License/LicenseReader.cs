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

namespace ASC.Core.Billing;

[Singletone]
public class LicenseReaderConfig
{
    public readonly string LicensePath;
    public LicenseReaderConfig(IConfiguration configuration)
    {
        LicensePath = configuration["license:file:path"] ?? "";
    }
}

[Scope]
public class LicenseReader
{
    private readonly TenantManager _tenantManager;
    private readonly ITariffService _tariffService;
    private readonly CoreSettings _coreSettings;
    private readonly ILogger<LicenseReader> _logger;
    private readonly Users.Constants _constants;
    public readonly string LicensePath;
    private readonly string _licensePathTemp;

    public const string CustomerIdKey = "CustomerId";

    public LicenseReader(
        TenantManager tenantManager,
        ITariffService tariffService,
        CoreSettings coreSettings,
        LicenseReaderConfig licenseReaderConfig,
        ILogger<LicenseReader> logger,
        Users.Constants constants)
    {
        _tenantManager = tenantManager;
        _tariffService = tariffService;
        _coreSettings = coreSettings;
        LicensePath = licenseReaderConfig.LicensePath;
        _licensePathTemp = LicensePath + ".tmp";
        _logger = logger;
        _constants = constants;
    }

    public async Task SetCustomerIdAsync(string value)
    {
        await _coreSettings.SaveSettingAsync(CustomerIdKey, value);
    }

    private Stream GetLicenseStream(bool temp = false)
    {
        var path = temp ? _licensePathTemp : LicensePath;
        if (!File.Exists(path))
        {
            throw new BillingNotFoundException("License not found");
        }

        return File.OpenRead(path);
    }

    public async Task RejectLicenseAsync()
    {
        if (File.Exists(_licensePathTemp))
        {
            File.Delete(_licensePathTemp);
        }

        if (File.Exists(LicensePath))
        {
            File.Delete(LicensePath);
        }

        await _tariffService.DeleteDefaultBillingInfoAsync();
    }

    public async Task RefreshLicenseAsync()
    {
        try
        {
            var temp = true;
            if (!File.Exists(_licensePathTemp))
            {
                _logger.DebugTempLicenseNotFound();

                if (!File.Exists(LicensePath))
                {
                    throw new BillingNotFoundException("License not found");
                }

                temp = false;
            }

            await using (var licenseStream = GetLicenseStream(temp))
            using (var reader = new StreamReader(licenseStream))
            {
                var licenseJsonString = reader.ReadToEnd();
                var license = License.Parse(licenseJsonString);

                await LicenseToDBAsync(license);

                if (temp)
                {
                    SaveLicense(licenseStream, LicensePath);
                }
            }

            if (temp)
            {
                File.Delete(_licensePathTemp);
            }
        }
        catch (Exception ex)
        {
            LogError(ex);

            throw;
        }
    }

    public DateTime SaveLicenseTemp(Stream licenseStream)
    {
        try
        {
            using var reader = new StreamReader(licenseStream);
            var licenseJsonString = reader.ReadToEnd();
            var license = License.Parse(licenseJsonString);

            var dueDate = Validate(license);

            SaveLicense(licenseStream, _licensePathTemp);

            return dueDate;
        }
        catch (Exception ex)
        {
            LogError(ex);

            throw;
        }
    }

    private static void SaveLicense(Stream licenseStream, string path)
    {
        ArgumentNullException.ThrowIfNull(licenseStream);

        if (licenseStream.CanSeek)
        {
            licenseStream.Seek(0, SeekOrigin.Begin);
        }

        using var fs = File.Open(path, FileMode.Create);
        licenseStream.CopyTo(fs);
    }

    private DateTime Validate(License license)
    {
        if (string.IsNullOrEmpty(license.CustomerId)
            || string.IsNullOrEmpty(license.Signature))
        {
            throw new BillingNotConfiguredException("License not correct", license.OriginalLicense);
        }

        return license.DueDate.Date;
    }

    private async Task LicenseToDBAsync(License license)
    {
        Validate(license);

        await SetCustomerIdAsync(license.CustomerId);

        var defaultQuota = await _tenantManager.GetTenantQuotaAsync(Tenant.DefaultTenant);

        var quota = new TenantQuota(-1000)
        {
            Name = "license",
            Trial = license.Trial,
            Audit = true,
            Ldap = true,
            Sso = true,
            WhiteLabel = true,
            ThirdParty = true,
            AutoBackupRestore = true,
            Oauth = true,
            ContentSearch = true,
            MaxFileSize = defaultQuota.MaxFileSize,
            MaxTotalSize = defaultQuota.MaxTotalSize,
            DocsEdition = true,
            Customization = license.Customization
        };

        await _tenantManager.SaveTenantQuotaAsync(quota);

        var tariff = new Tariff
        {
            Quotas = new List<Quota> { new Quota(quota.TenantId, 1) },
            DueDate = license.DueDate,
        };

        await _tariffService.SetTariffAsync(Tenant.DefaultTenant, tariff, new List<TenantQuota> { quota });
    }

    private void LogError(Exception error)
    {
        if (error is BillingNotFoundException)
        {
            _logger.DebugLicenseNotFound(error.Message);
        }
        else
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.ErrorWithException(error);
            }
            else
            {
                _logger.ErrorWithException(error);
            }
        }
    }
}
