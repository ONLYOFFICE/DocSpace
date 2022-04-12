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
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly PaymentManager _paymentManager;
    private readonly CoreSettings _coreSettings;
    private readonly ILog _logger;
    public readonly string LicensePath;
    private readonly string _licensePathTemp;

    public const string CustomerIdKey = "CustomerId";
    public const int MaxUserCount = 10000;

    public LicenseReader(
        UserManager userManager,
        TenantManager tenantManager,
        PaymentManager paymentManager,
        CoreSettings coreSettings,
        LicenseReaderConfig licenseReaderConfig,
        ILog logger)
    {
        _userManager = userManager;
        _tenantManager = tenantManager;
        _paymentManager = paymentManager;
        _coreSettings = coreSettings;
        LicensePath = licenseReaderConfig.LicensePath;
        _licensePathTemp = LicensePath + ".tmp";
        _logger = logger;
    }

    public string CustomerId
    {
        get => _coreSettings.GetSetting(CustomerIdKey);
        private set => _coreSettings.SaveSetting(CustomerIdKey, value);
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

    public void RejectLicense()
    {
        if (File.Exists(_licensePathTemp))
        {
            File.Delete(_licensePathTemp);
        }

        if (File.Exists(LicensePath))
        {
            File.Delete(LicensePath);
        }

        _paymentManager.DeleteDefaultTariff();
    }

    public void RefreshLicense()
    {
        try
        {
            var temp = true;
            if (!File.Exists(_licensePathTemp))
            {
                _logger.Debug("Temp license not found");

                if (!File.Exists(LicensePath))
                {
                    throw new BillingNotFoundException("License not found");
                }

                temp = false;
            }

            using (var licenseStream = GetLicenseStream(temp))
            using (var reader = new StreamReader(licenseStream))
            {
                var licenseJsonString = reader.ReadToEnd();
                var license = License.Parse(licenseJsonString);

                LicenseToDB(license);

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

        if (license.DueDate.Date < VersionReleaseDate)
        {
            throw new LicenseExpiredException("License expired", license.OriginalLicense);
        }

        if (license.ActiveUsers.Equals(default) || license.ActiveUsers < 1)
            license.ActiveUsers = MaxUserCount;

        if (license.ActiveUsers < _userManager.GetUsers(EmployeeStatus.Default, EmployeeType.User).Length)
        {
            throw new LicenseQuotaException("License quota", license.OriginalLicense);
        }

        if (license.PortalCount <= 0)
        {
            license.PortalCount = _tenantManager.GetTenantQuota(Tenant.DefaultTenant).CountPortals;
        }
        var activePortals = _tenantManager.GetTenants().Count;
        if (activePortals > 1 && license.PortalCount < activePortals)
        {
            throw new LicensePortalException("License portal count", license.OriginalLicense);
        }

        return license.DueDate.Date;
    }

    private void LicenseToDB(License license)
    {
        Validate(license);

        CustomerId = license.CustomerId;

        var defaultQuota = _tenantManager.GetTenantQuota(Tenant.DefaultTenant);

        var quota = new TenantQuota(-1000)
        {
            ActiveUsers = license.ActiveUsers,
            MaxFileSize = defaultQuota.MaxFileSize,
            MaxTotalSize = defaultQuota.MaxTotalSize,
            Name = "license",
            DocsEdition = true,
            HasDomain = true,
            Audit = true,
            ControlPanel = true,
            HealthCheck = true,
            Ldap = true,
            Sso = true,
            Customization = license.Customization,
            WhiteLabel = license.WhiteLabel || license.Customization,
            Branding = license.Branding,
            SSBranding = license.SSBranding,
            Update = true,
            Support = true,
            Trial = license.Trial,
            CountPortals = license.PortalCount,
            DiscEncryption = true,
            PrivacyRoom = true,
            Restore = true,
            ContentSearch = true
        };

        if (defaultQuota.Name != "overdue" && !defaultQuota.Trial)
        {
            quota.WhiteLabel |= defaultQuota.WhiteLabel;
            quota.Branding |= defaultQuota.Branding;
            quota.SSBranding |= defaultQuota.SSBranding;

            quota.CountPortals = Math.Max(defaultQuota.CountPortals, quota.CountPortals);
        }

        _tenantManager.SaveTenantQuota(quota);

        var tariff = new Tariff
        {
            QuotaId = quota.Tenant,
            DueDate = license.DueDate,
        };

        _paymentManager.SetTariff(-1, tariff);

        if (!string.IsNullOrEmpty(license.AffiliateId))
        {
            var tenant = _tenantManager.GetCurrentTenant();
            tenant.AffiliateId = license.AffiliateId;
            _tenantManager.SaveTenant(tenant);
        }
    }

    private void LogError(Exception error)
    {
        if (error is BillingNotFoundException)
        {
            _logger.DebugFormat("License not found: {0}", error.Message);
        }
        else
        {
            if (_logger.IsDebugEnabled)
            {
                _logger.Error(error);
            }
            else
            {
                _logger.Error(error.Message);
            }
        }
    }

    private static readonly DateTime _date = DateTime.MinValue;

    public DateTime VersionReleaseDate
    {
        get
        {
            // release sign is not longer requered
            return _date;

            //if (_date != DateTime.MinValue) return _date;

            //_date = DateTime.MaxValue;
            //try
            //{
            //    var versionDate = Configuration["version:release:date"];
            //    var sign = Configuration["version:release:sign"];

            //    if (!sign.StartsWith("ASC "))
            //    {
            //        throw new Exception("sign without ASC");
            //    }

            //    var splitted = sign.Substring(4).Split(':');
            //    var pkey = splitted[0];
            //    if (pkey != versionDate)
            //    {
            //        throw new Exception("sign with different date");
            //    }

            //    var date = splitted[1];
            //    var orighash = splitted[2];

            //var skey = MachinePseudoKeys.GetMachineConstant();

            //using (var hasher = new HMACSHA1(skey))
            //    {
            //        var data = string.Join("\n", date, pkey);
            //        var hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(data));
            //        if (WebEncoders.Base64UrlEncode(hash) != orighash && Convert.ToBase64String(hash) != orighash)
            //        {
            //            throw new Exception("incorrect hash");
            //        }
            //    }

            //    var year = int.Parse(versionDate.Substring(0, 4));
            //    var month = int.Parse(versionDate.Substring(4, 2));
            //    var day = int.Parse(versionDate.Substring(6, 2));
            //    _date = new DateTime(year, month, day);
            //}
            //catch (Exception ex)
            //{
            //    Log.Error("VersionReleaseDate", ex);
            //}
            //return _date;
        }
    }
}
