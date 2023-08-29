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

namespace ASC.Web.Api.Controllers.Settings;

public class WhitelabelController : BaseSettingsController
{
    private Tenant Tenant { get { return ApiContext.Tenant; } }

    private readonly PermissionContext _permissionContext;
    private readonly SettingsManager _settingsManager;
    private readonly TenantInfoSettingsHelper _tenantInfoSettingsHelper;
    private readonly TenantWhiteLabelSettingsHelper _tenantWhiteLabelSettingsHelper;
    private readonly TenantLogoManager _tenantLogoManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly IMapper _mapper;
    private readonly CompanyWhiteLabelSettingsHelper _companyWhiteLabelSettingsHelper;

    public WhitelabelController(
        ApiContext apiContext,
        PermissionContext permissionContext,
        SettingsManager settingsManager,
        WebItemManager webItemManager,
        TenantInfoSettingsHelper tenantInfoSettingsHelper,
        TenantWhiteLabelSettingsHelper tenantWhiteLabelSettingsHelper,
        TenantLogoManager tenantLogoManager,
        CoreBaseSettings coreBaseSettings,
        CommonLinkUtility commonLinkUtility,
        IMemoryCache memoryCache,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        CompanyWhiteLabelSettingsHelper companyWhiteLabelSettingsHelper) : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _permissionContext = permissionContext;
        _settingsManager = settingsManager;
        _tenantInfoSettingsHelper = tenantInfoSettingsHelper;
        _tenantWhiteLabelSettingsHelper = tenantWhiteLabelSettingsHelper;
        _tenantLogoManager = tenantLogoManager;
        _coreBaseSettings = coreBaseSettings;
        _commonLinkUtility = commonLinkUtility;
        _mapper = mapper;
        _companyWhiteLabelSettingsHelper = companyWhiteLabelSettingsHelper;
    }

    /// <summary>
    /// Saves the white label settings specified in the request.
    /// </summary>
    /// <short>
    /// Save the white label settings
    /// </short>
    /// <category>Rebranding</category>
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.WhiteLabelRequestsDto, ASC.Web.Api" name="inDto">Request parameters for white label settings</param>
    /// <returns type="System.Boolean, System">Boolean value: true if the operation is sucessful</returns>
    /// <path>api/2.0/settings/whitelabel/save</path>
    /// <httpMethod>POST</httpMethod>
    ///<visible>false</visible>
    [HttpPost("whitelabel/save")]
    public async Task<bool> SaveWhiteLabelSettingsAsync(WhiteLabelRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        await DemandWhiteLabelPermissionAsync();

        var settings = await _settingsManager.LoadAsync<TenantWhiteLabelSettings>();

        if (inDto.Logo != null)
        {
            var logoDict = new Dictionary<int, KeyValuePair<string, string>>();

            foreach (var l in inDto.Logo)
            {
                var key = Int32.Parse(l.Key);

                logoDict.Add(key, new KeyValuePair<string, string>(l.Value.Light, l.Value.Dark));
            }

            await _tenantWhiteLabelSettingsHelper.SetLogo(settings, logoDict, null);
        }

        settings.SetLogoText(inDto.LogoText);
        await _tenantWhiteLabelSettingsHelper.SaveAsync(settings, Tenant.Id, _tenantLogoManager);

        return true;
    }

    /// <summary>
    /// Saves the white label settings from files.
    /// </summary>
    /// <short>
    /// Save the white label settings from files
    /// </short>
    /// <category>Rebranding</category>
    /// <returns type="System.Boolean, System">Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/settings/whitelabel/savefromfiles</path>
    /// <httpMethod>POST</httpMethod>
    ///<visible>false</visible>
    [HttpPost("whitelabel/savefromfiles")]
    public async Task<bool> SaveWhiteLabelSettingsFromFilesAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        await DemandWhiteLabelPermissionAsync();

        if (HttpContext.Request.Form?.Files == null || HttpContext.Request.Form.Files.Count == 0)
        {
            throw new InvalidOperationException("No input files");
        }

        var settings = await _settingsManager.LoadAsync<TenantWhiteLabelSettings>();

        foreach (var f in HttpContext.Request.Form.Files)
        {
            if (f.FileName.Contains("dark"))
            {
                GetParts(f.FileName, out var logoType, out var fileExt);
                await _tenantWhiteLabelSettingsHelper.SetLogoFromStream(settings, logoType, fileExt, f.OpenReadStream(), true, null);
            }
            else
            {
                GetParts(f.FileName, out var logoType, out var fileExt);

                await _tenantWhiteLabelSettingsHelper.SetLogoFromStream(settings, logoType, fileExt, f.OpenReadStream(), false, null);
            }
        }

        await _settingsManager.SaveAsync(settings, Tenant.Id);

        return true;
    }

    private void GetParts(string fileName, out WhiteLabelLogoTypeEnum logoType, out string fileExt)
    {
        var parts = fileName.Split('.');
        logoType = (WhiteLabelLogoTypeEnum)Convert.ToInt32(parts[0]);
        fileExt = parts.Last();
    }

    /// <summary>
    /// Returns the white label logos.
    /// </summary>
    /// <short>
    /// Get the white label logos
    /// </short>
    /// <category>Rebranding</category>
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.WhiteLabelQueryRequestsDto, ASC.Web.Api" name="inDto">White label request parameters</param>
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.WhiteLabelItemDto, ASC.Web.Api">White label logos</returns>
    /// <path>api/2.0/settings/whitelabel/logos</path>
    /// <httpMethod>GET</httpMethod>
    /// <requiresAuthorization>false</requiresAuthorization>
    /// <collection>list</collection>
    /// <visible>false</visible>
    [AllowNotPayment, AllowAnonymous, AllowSuspended]
    [HttpGet("whitelabel/logos")]
    public async IAsyncEnumerable<WhiteLabelItemDto> GetWhiteLabelLogos([FromQuery] WhiteLabelQueryRequestsDto inDto)
    {
        var _tenantWhiteLabelSettings = await _settingsManager.LoadAsync<TenantWhiteLabelSettings>();

        foreach (var logoType in (WhiteLabelLogoTypeEnum[])Enum.GetValues(typeof(WhiteLabelLogoTypeEnum)))
        {
            if (logoType == WhiteLabelLogoTypeEnum.Notification)
            {
                continue;
            }

            var result = new WhiteLabelItemDto
            {
                Name = logoType.ToString(),
                Size = TenantWhiteLabelSettings.GetSize(logoType)
            };

            if (inDto.IsDark.HasValue)
            {
                var path = _commonLinkUtility.GetFullAbsolutePath(await _tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPathAsync(_tenantWhiteLabelSettings, logoType, inDto.IsDark.Value));

                if (inDto.IsDark.Value)
                {
                    result.Path = new WhiteLabelItemPathDto
                    {
                        Dark = path
                    };
                }
                else
                {
                    result.Path = new WhiteLabelItemPathDto
                    {
                        Light = path
                    };
                }
            }
            else
            {
                var lightPath = _commonLinkUtility.GetFullAbsolutePath(await _tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPathAsync(_tenantWhiteLabelSettings, logoType, false));
                var darkPath = _commonLinkUtility.GetFullAbsolutePath(await _tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPathAsync(_tenantWhiteLabelSettings, logoType, true));

                if (lightPath == darkPath)
                {
                    darkPath = null;
                }

                result.Path = new WhiteLabelItemPathDto
                {
                    Light = lightPath,
                    Dark = darkPath
                };
            }

            yield return result;
        }
    }

    /// <summary>
    /// Returns the white label logo text.
    /// </summary>
    /// <short>
    /// Get the white label logo text
    /// </short>
    /// <category>Rebranding</category>
    /// <returns type="System.Object, System">Logo text</returns>
    /// <path>api/2.0/settings/whitelabel/logotext</path>
    /// <httpMethod>GET</httpMethod>
    ///<visible>false</visible>
    [AllowNotPayment]
    [HttpGet("whitelabel/logotext")]
    public async Task<object> GetWhiteLabelLogoTextAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var settings = await _settingsManager.LoadAsync<TenantWhiteLabelSettings>();

        return settings.LogoText ?? TenantWhiteLabelSettings.DefaultLogoText;
    }


    /// <summary>
    /// Restores the white label options.
    /// </summary>
    /// <short>
    /// Restore the white label options
    /// </short>
    /// <category>Rebranding</category>
    /// <returns type="System.Boolean, System">Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/settings/whitelabel/restore</path>
    /// <httpMethod>PUT</httpMethod>
    /// <visible>false</visible>
    [HttpPut("whitelabel/restore")]
    public async Task<bool> RestoreWhiteLabelOptionsAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);
        await DemandWhiteLabelPermissionAsync();

        var settings = await _settingsManager.LoadAsync<TenantWhiteLabelSettings>();

        await _tenantWhiteLabelSettingsHelper.RestoreDefault(settings, _tenantLogoManager, Tenant.Id, null);

        var tenantInfoSettings = await _settingsManager.LoadAsync<TenantInfoSettings>();
        await _tenantInfoSettingsHelper.RestoreDefaultLogoAsync(tenantInfoSettings, _tenantLogoManager);
        await _settingsManager.SaveAsync(tenantInfoSettings);

        return true;
    }

    /// <summary>
    /// Returns the licensor data.
    /// </summary>
    /// <short>Get the licensor data</short>
    /// <category>Rebranding</category>
    /// <returns type="ASC.Web.Core.WhiteLabel.CompanyWhiteLabelSettings, ASC.Web.Core">List of company white label settings</returns>
    /// <path>api/2.0/settings/companywhitelabel</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    /// <visible>false</visible>
    [HttpGet("companywhitelabel")]
    public async Task<List<CompanyWhiteLabelSettings>> GetLicensorDataAsync()
    {
        var result = new List<CompanyWhiteLabelSettings>();

        var instance = await _companyWhiteLabelSettingsHelper.InstanceAsync();

        result.Add(instance);

        if (!_companyWhiteLabelSettingsHelper.IsDefault(instance) && !instance.IsLicensor)
        {
            result.Add(_settingsManager.GetDefault<CompanyWhiteLabelSettings>());
        }

        return result;
    }

    /// <summary>
    /// Saves the company white label settings specified in the request.
    /// </summary>
    /// <category>Rebranding</category>
    /// <short>Save the company white label settings</short>
    /// <param type="ASC.Web.Core.WhiteLabel.CompanyWhiteLabelSettingsWrapper, ASC.Web.Core" name="companyWhiteLabelSettingsWrapper">Company white label settings</param>
    /// <returns type="System.Boolean, System">Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/settings/rebranding/company</path>
    /// <httpMethod>POST</httpMethod>
    /// <visible>false</visible>
    [HttpPost("rebranding/company")]
    public async Task<bool> SaveCompanyWhiteLabelSettingsAsync(CompanyWhiteLabelSettingsWrapper companyWhiteLabelSettingsWrapper)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);
        await DemandRebrandingPermissionAsync();

        if (companyWhiteLabelSettingsWrapper.Settings == null)
        {
            throw new ArgumentNullException("settings");
        }

        companyWhiteLabelSettingsWrapper.Settings.IsLicensor = false;

        await _settingsManager.SaveAsync(companyWhiteLabelSettingsWrapper.Settings);

        return true;
    }

    /// <summary>
    /// Returns the company white label settings.
    /// </summary>
    /// <category>Rebranding</category>
    /// <short>Get the company white label settings</short>
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.CompanyWhiteLabelSettingsDtov, ASC.Web.Api">Company white label settings</returns>
    /// <path>api/2.0/settings/rebranding/company</path>
    /// <httpMethod>GET</httpMethod>
    ///<visible>false</visible>
    [AllowNotPayment]
    [HttpGet("rebranding/company")]
    public async Task<CompanyWhiteLabelSettingsDto> GetCompanyWhiteLabelSettingsAsync()
    {
        return _mapper.Map<CompanyWhiteLabelSettings, CompanyWhiteLabelSettingsDto>(await _settingsManager.LoadAsync<CompanyWhiteLabelSettings>());
    }

    /// <summary>
    /// Deletes the company white label settings.
    /// </summary>
    /// <category>Rebranding</category>
    /// <short>Delete the company white label settings</short>
    /// <returns type="ASC.Web.Core.WhiteLabel.CompanyWhiteLabelSettings, ASC.Web.Core">Default company white label settings</returns>
    /// <path>api/2.0/settings/rebranding/company</path>
    /// <httpMethod>DELETE</httpMethod>
    /// <visible>false</visible>
    [HttpDelete("rebranding/company")]
    public async Task<CompanyWhiteLabelSettings> DeleteCompanyWhiteLabelSettingsAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);
        await DemandRebrandingPermissionAsync();

        var defaultSettings = _settingsManager.GetDefault<CompanyWhiteLabelSettings>();

        await _settingsManager.SaveAsync(defaultSettings);

        return defaultSettings;
    }

    /// <summary>
    /// Saves the additional white label settings specified in the request.
    /// </summary>
    /// <category>Rebranding</category>
    /// <short>Save the additional white label settings</short>
    /// <param type="ASC.Web.Core.WhiteLabel.AdditionalWhiteLabelSettingsWrapper, ASC.Web.Core" name="wrapper">Additional white label settings</param>
    /// <returns type="System.Boolean, System">Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/settings/rebranding/additional</path>
    /// <httpMethod>POST</httpMethod>
    ///<visible>false</visible>
    [HttpPost("rebranding/additional")]
    public async Task<bool> SaveAdditionalWhiteLabelSettingsAsync(AdditionalWhiteLabelSettingsWrapper wrapper)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);
        await DemandRebrandingPermissionAsync();

        if (wrapper.Settings == null)
        {
            throw new ArgumentNullException("settings");
        }

        await _settingsManager.SaveAsync(wrapper.Settings);

        return true;
    }

    /// <summary>
    /// Returns the additional white label settings.
    /// </summary>
    /// <category>Rebranding</category>
    /// <short>Get the additional white label settings</short>
    /// <returns type="ASC.Web.Api.ApiModels.ResponseDto.AdditionalWhiteLabelSettingsDto, ASC.Web.Api">Additional white label settings</returns>
    /// <path>api/2.0/settings/rebranding/additional</path>
    /// <httpMethod>GET</httpMethod>
    ///<visible>false</visible>
    [AllowNotPayment]
    [HttpGet("rebranding/additional")]
    public async Task<AdditionalWhiteLabelSettingsDto> GetAdditionalWhiteLabelSettingsAsync()
    {
        return _mapper.Map<AdditionalWhiteLabelSettings, AdditionalWhiteLabelSettingsDto>(await _settingsManager.LoadAsync<AdditionalWhiteLabelSettings>());
    }

    /// <summary>
    /// Deletes the additional white label settings.
    /// </summary>
    /// <category>Rebranding</category>
    /// <short>Delete the additional white label settings</short>
    /// <returns type="ASC.Web.Core.WhiteLabel.AdditionalWhiteLabelSettings, ASC.Web.Core">Default additional white label settings</returns>
    /// <path>api/2.0/settings/rebranding/additional</path>
    /// <httpMethod>DELETE</httpMethod>
    ///<visible>false</visible>
    [HttpDelete("rebranding/additional")]
    public async Task<AdditionalWhiteLabelSettings> DeleteAdditionalWhiteLabelSettingsAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);
        await DemandRebrandingPermissionAsync();

        var defaultSettings = _settingsManager.GetDefault<AdditionalWhiteLabelSettings>();

        await _settingsManager.SaveAsync(defaultSettings);

        return defaultSettings;
    }

    /// <summary>
    /// Saves the mail white label settings specified in the request.
    /// </summary>
    /// <category>Rebranding</category>
    /// <short>Save the mail white label settings</short>
    /// <param type="ASC.Web.Core.WhiteLabel.MailWhiteLabelSettings, ASC.Web.Core" name="settings">Mail white label settings</param>
    /// <returns type="System.Boolean, System">Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/settings/rebranding/mail</path>
    /// <httpMethod>POST</httpMethod>
    ///<visible>false</visible>
    [HttpPost("rebranding/mail")]
    public async Task<bool> SaveMailWhiteLabelSettingsAsync(MailWhiteLabelSettings settings)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);
        await DemandRebrandingPermissionAsync();

        ArgumentNullException.ThrowIfNull(settings);

        await _settingsManager.SaveAsync(settings);
        return true;
    }

    /// <summary>
    /// Updates the mail white label settings with a paramater specified in the request.
    /// </summary>
    /// <category>Rebranding</category>
    /// <short>Update the mail white label settings</short>
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.MailWhiteLabelSettingsRequestsDto, ASC.Web.Api" name="inDto">Request parameters for mail white label settings</param>
    /// <returns type="System.Boolean, System">Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/settings/rebranding/mail</path>
    /// <httpMethod>PUT</httpMethod>
    ///<visible>false</visible>
    [HttpPut("rebranding/mail")]
    public async Task<bool> UpdateMailWhiteLabelSettings(MailWhiteLabelSettingsRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);
        await DemandRebrandingPermissionAsync();

        await _settingsManager.ManageAsync<MailWhiteLabelSettings>(settings =>
        {
        settings.FooterEnabled = inDto.FooterEnabled;
        });

        return true;
    }

    /// <summary>
    /// Returns the mail white label settings.
    /// </summary>
    /// <category>Rebranding</category>
    /// <short>Get the mail white label settings</short>
    /// <returns type="ASC.Web.Core.WhiteLabel.MailWhiteLabelSettings, ASC.Web.Core">Mail white label settings</returns>
    /// <path>api/2.0/settings/rebranding/mail</path>
    /// <httpMethod>GET</httpMethod>
    ///<visible>false</visible>
    [HttpGet("rebranding/mail")]
    public async Task<MailWhiteLabelSettings> GetMailWhiteLabelSettingsAsync()
    {
        return await _settingsManager.LoadAsync<MailWhiteLabelSettings>();
    }

    /// <summary>
    /// Deletes the mail white label settings.
    /// </summary>
    /// <category>Rebranding</category>
    /// <short>Delete the mail white label settings</short>
    /// <returns type="ASC.Web.Core.WhiteLabel.MailWhiteLabelSettings, ASC.Web.Core">Default mail white label settings</returns>
    /// <path>api/2.0/settings/rebranding/mail</path>
    /// <httpMethod>DELETE</httpMethod>
    ///<visible>false</visible>
    [HttpDelete("rebranding/mail")]
    public async Task<MailWhiteLabelSettings> DeleteMailWhiteLabelSettingsAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);
        await DemandRebrandingPermissionAsync();

        var defaultSettings = _settingsManager.GetDefault<MailWhiteLabelSettings>();

        await _settingsManager.SaveAsync(defaultSettings);

        return defaultSettings;
    }

    /// <summary>
    /// Checks if the white label is enabled or not.
    /// </summary>
    /// <category>Rebranding</category>
    /// <short>Check the white label availability</short>
    /// <returns type="System.Boolean, System">Boolean value: true if the white label is enabled</returns>
    /// <path>api/2.0/settings/enableWhitelabel</path>
    /// <httpMethod>GET</httpMethod>
    ///<visible>false</visible>
    [HttpGet("enableWhitelabel")]
    public async Task<bool> GetEnableWhitelabelAsync()
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        return _coreBaseSettings.Standalone || _tenantLogoManager.WhiteLabelEnabled && await _tenantLogoManager.GetWhiteLabelPaidAsync();
    }

    private async Task DemandWhiteLabelPermissionAsync()
    {
        if (!_coreBaseSettings.Standalone && (!_tenantLogoManager.WhiteLabelEnabled || !await _tenantLogoManager.GetWhiteLabelPaidAsync()))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "WhiteLabel");
        }
    }

    private async Task DemandRebrandingPermissionAsync()
    {
        if (!_coreBaseSettings.Standalone || _coreBaseSettings.CustomMode)
        {
            throw new SecurityException(Resource.ErrorAccessDenied);
        }
        await DemandWhiteLabelPermissionAsync();
    }
}
