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

namespace ASC.Web.Core.WhiteLabel;

[Serializable]
public class TenantInfoSettings : ISettings<TenantInfoSettings>
{
    [JsonPropertyName("LogoSize")]
    public Size CompanyLogoSize { get; internal set; }

    [JsonPropertyName("LogoFileName")]
    public string CompanyLogoFileName { get; set; }

    [JsonPropertyName("Default")]
    internal bool IsDefault { get; set; }

    public TenantInfoSettings GetDefault()
    {
        return new TenantInfoSettings()
        {
            IsDefault = true
        };
    }

    [JsonIgnore]
    public Guid ID
    {
        get { return new Guid("{5116B892-CCDD-4406-98CD-4F18297C0C0A}"); }
    }
}

[Scope]
public class TenantInfoSettingsHelper
{
    private readonly WebImageSupplier _webImageSupplier;
    private readonly StorageFactory _storageFactory;
    private readonly TenantManager _tenantManager;
    private readonly IConfiguration _configuration;

    public TenantInfoSettingsHelper(
        WebImageSupplier webImageSupplier,
        StorageFactory storageFactory,
        TenantManager tenantManager,
        IConfiguration configuration)
    {
        _webImageSupplier = webImageSupplier;
        _storageFactory = storageFactory;
        _tenantManager = tenantManager;
        _configuration = configuration;
    }
    public void RestoreDefault(TenantInfoSettings tenantInfoSettings, TenantLogoManager tenantLogoManager)
    {
        RestoreDefaultTenantName();
        RestoreDefaultLogo(tenantInfoSettings, tenantLogoManager);
    }

    public void RestoreDefaultTenantName()
    {
        var currentTenant = _tenantManager.GetCurrentTenant();
        currentTenant.Name = _configuration["web:portal-name"] ?? "";
        _tenantManager.SaveTenant(currentTenant);
    }

    public void RestoreDefaultLogo(TenantInfoSettings tenantInfoSettings, TenantLogoManager tenantLogoManager)
    {
        tenantInfoSettings.IsDefault = true;

        var store = _storageFactory.GetStorage(_tenantManager.GetCurrentTenant().Id.ToString(), "logo");
        try
        {
            store.DeleteFilesAsync("", "*", false).Wait();
        }
        catch
        {
        }
        tenantInfoSettings.CompanyLogoSize = default;

        tenantLogoManager.RemoveMailLogoDataFromCache();
    }

    public void SetCompanyLogo(string companyLogoFileName, byte[] data, TenantInfoSettings tenantInfoSettings, TenantLogoManager tenantLogoManager)
    {
        var store = _storageFactory.GetStorage(_tenantManager.GetCurrentTenant().Id.ToString(), "logo");

        if (!tenantInfoSettings.IsDefault)
        {
            try
            {
                store.DeleteFilesAsync("", "*", false).Wait();
            }
            catch
            {
            }
        }
        using (var memory = new MemoryStream(data))
        using (var image = Image.Load(memory))
        {
            tenantInfoSettings.CompanyLogoSize = image.Size();
            memory.Seek(0, SeekOrigin.Begin);
            store.SaveAsync(companyLogoFileName, memory).Wait();
            tenantInfoSettings.CompanyLogoFileName = companyLogoFileName;
        }
        tenantInfoSettings.IsDefault = false;

        tenantLogoManager.RemoveMailLogoDataFromCache();
    }

    public string GetAbsoluteCompanyLogoPath(TenantInfoSettings tenantInfoSettings)
    {
        if (tenantInfoSettings.IsDefault)
        {
            return _webImageSupplier.GetAbsoluteWebPath("logo/dark_general.png");
        }

        var store = _storageFactory.GetStorage(_tenantManager.GetCurrentTenant().Id.ToString(), "logo");
        return store.GetUriAsync(tenantInfoSettings.CompanyLogoFileName ?? "").Result.ToString();
    }

    /// <summary>
    /// Get logo stream or null in case of default logo
    /// </summary>
    public Stream GetStorageLogoData(TenantInfoSettings tenantInfoSettings)
    {
        if (tenantInfoSettings.IsDefault)
        {
            return null;
        }

        var storage = _storageFactory.GetStorage(_tenantManager.GetCurrentTenant().Id.ToString(CultureInfo.InvariantCulture), "logo");

        if (storage == null)
        {
            return null;
        }

        var fileName = tenantInfoSettings.CompanyLogoFileName ?? "";

        return storage.IsFileAsync(fileName).Result ? storage.GetReadStreamAsync(fileName).Result : null;
    }
}
