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
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.WhiteLabelRequestsDto, ASC.Web.Api.ApiModel.RequestsDto" name="inDto">Request parameters for white label settings: <![CDATA[
    /// <ul>
    ///     <li><b>LogoText</b> (string) - logo text,</li>
    ///     <li><b>Logo</b> (IEnumerable&lt;ItemKeyValuePair&lt;string, LogoRequestsDto&gt;&gt;) - tenant IDs with their logos (light or dark).</li>
    /// </ul>
    /// ]]></param>
    /// <returns>Boolean value: true if the operation is sucessful</returns>
    /// <path>api/2.0/settings/whitelabel/save</path>
    /// <httpMethod>POST</httpMethod>
    ///<visible>false</visible>
    [HttpPost("whitelabel/save")]
    public async Task<bool> SaveWhiteLabelSettings(WhiteLabelRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        DemandWhiteLabelPermission();

        var settings = _settingsManager.Load<TenantWhiteLabelSettings>();

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
        _tenantWhiteLabelSettingsHelper.Save(settings, Tenant.Id, _tenantLogoManager);

        return true;
    }

    /// <summary>
    /// Saves the white label settings from files.
    /// </summary>
    /// <short>
    /// Save the white label settings from files
    /// </short>
    /// <category>Rebranding</category>
    /// <returns>Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/settings/whitelabel/savefromfiles</path>
    /// <httpMethod>POST</httpMethod>
    ///<visible>false</visible>
    [HttpPost("whitelabel/savefromfiles")]
    public async Task<bool> SaveWhiteLabelSettingsFromFiles()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        DemandWhiteLabelPermission();

        if (HttpContext.Request.Form?.Files == null || HttpContext.Request.Form.Files.Count == 0)
        {
            throw new InvalidOperationException("No input files");
        }

        var settings = _settingsManager.Load<TenantWhiteLabelSettings>();

        foreach (var f in HttpContext.Request.Form.Files)
        {
            if (f.FileName.Contains("dark"))
            {
                GetParts(f.FileName, out var logoType, out var fileExt);
                if (HttpContext.Request.Form.Files.Any(f => f.FileName.Contains($"{logoType}")))
                {
                    continue;
                }
                await _tenantWhiteLabelSettingsHelper.SetLogoFromStream(settings, logoType, fileExt, null, f.OpenReadStream(), null);
            }
            else
            {
                GetParts(f.FileName, out var logoType, out var fileExt);
                IFormFile darkFile;
                if (HttpContext.Request.Form.Files.Any(f => f.FileName.Contains($"{logoType}.dark.")))
                {
                    darkFile = HttpContext.Request.Form.Files.Single(f => f.FileName.Contains($"{logoType}.dark."));
                }
                else
                {
                    darkFile = null;
                }
                if (darkFile != null && darkFile.FileName != f.FileName)
                {
                    throw new InvalidOperationException("logo light and logo dark have different extention");
                }

                await _tenantWhiteLabelSettingsHelper.SetLogoFromStream(settings, logoType, fileExt, f.OpenReadStream(), darkFile?.OpenReadStream(), null);
            }
        }

        _settingsManager.Save(settings, Tenant.Id);

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
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.WhiteLabelQueryRequestsDto, ASC.Web.Api.ApiModel.RequestsDto" name="inDto">White label request parameters: IsDark (bool?) - specifies if the logo is for a dark theme or not</param>
    /// <returns>White label logos: file name, size, path</returns>
    /// <path>api/2.0/settings/whitelabel/logos</path>
    /// <httpMethod>GET</httpMethod>
    /// <requiresAuthorization>false</requiresAuthorization>
    /// <visible>false</visible>
    [AllowNotPayment, AllowAnonymous]
    [HttpGet("whitelabel/logos")]
    public async IAsyncEnumerable<WhiteLabelItemDto> GetWhiteLabelLogos([FromQuery] WhiteLabelQueryRequestsDto inDto)
    {
        var _tenantWhiteLabelSettings = _settingsManager.Load<TenantWhiteLabelSettings>();

        foreach (var logoType in (WhiteLabelLogoTypeEnum[])Enum.GetValues(typeof(WhiteLabelLogoTypeEnum)))
        {
            var result = new WhiteLabelItemDto
            {
                Name = logoType.ToString(),
                Size = TenantWhiteLabelSettings.GetSize(logoType)
            };

            if (inDto.IsDark.HasValue)
            {
                var path = _commonLinkUtility.GetFullAbsolutePath(await _tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings, logoType, inDto.IsDark.Value));

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
                var lightPath = _commonLinkUtility.GetFullAbsolutePath(await _tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings, logoType, false));
                var darkPath = _commonLinkUtility.GetFullAbsolutePath(await _tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings, logoType, true));

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
    /// <returns>Logo text</returns>
    /// <path>api/2.0/settings/whitelabel/logotext</path>
    /// <httpMethod>GET</httpMethod>
    ///<visible>false</visible>
    [AllowNotPayment]
    [HttpGet("whitelabel/logotext")]
    public object GetWhiteLabelLogoText()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var settings = _settingsManager.Load<TenantWhiteLabelSettings>();

        return settings.LogoText ?? TenantWhiteLabelSettings.DefaultLogoText;
    }


    /// <summary>
    /// Restores the white label options.
    /// </summary>
    /// <short>
    /// Restore the white label options
    /// </short>
    /// <category>Rebranding</category>
    /// <returns>Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/settings/whitelabel/restore</path>
    /// <httpMethod>PUT</httpMethod>
    /// <visible>false</visible>
    [HttpPut("whitelabel/restore")]
    public async Task<bool> RestoreWhiteLabelOptions()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        DemandWhiteLabelPermission();

        var settings = _settingsManager.Load<TenantWhiteLabelSettings>();

        await _tenantWhiteLabelSettingsHelper.RestoreDefault(settings, _tenantLogoManager, Tenant.Id, null);

        var tenantInfoSettings = _settingsManager.Load<TenantInfoSettings>();
        await _tenantInfoSettingsHelper.RestoreDefaultLogo(tenantInfoSettings, _tenantLogoManager);
        _settingsManager.Save(tenantInfoSettings);

        return true;
    }

    /// <summary>
    /// Returns the licensor data.
    /// </summary>
    /// <short>Get the licensor data</short>
    /// <category>Rebranding</category>
    /// <returns>List of company white label settings: company name, site, email, address, phone, licensor or not</returns>
    /// <path>api/2.0/settings/companywhitelabel</path>
    /// <httpMethod>GET</httpMethod>
    /// <visible>false</visible>
    [HttpGet("companywhitelabel")]
    public List<CompanyWhiteLabelSettings> GetLicensorData()
    {
        var result = new List<CompanyWhiteLabelSettings>();

        var instance = _companyWhiteLabelSettingsHelper.Instance();

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
    /// <param type="ASC.Web.Core.WhiteLabel.CompanyWhiteLabelSettingsWrapper, ASC.Web.Core.WhiteLabel" name="companyWhiteLabelSettingsWrapper">Company white label settings: <![CDATA[
    /// <ul>
    ///     <li><b>Settings</b> (CompanyWhiteLabelSettings) - company white label settings:</li>
    ///     <ul>
    ///         <li><b>CompanyName</b> (string) - company name,</li>
    ///         <li><b>Site</b> (string) - site,</li>
    ///         <li><b>Email</b> (bool) - email,</li>
    ///         <li><b>Address</b> (bool) - address,</li>
    ///         <li><b>Phone</b> (string) - phone,</li>
    ///         <li><b>IsLicensor</b> (bool) - licensor or not.</li>
    ///     </ul>
    /// </ul>
    /// ]]></param>
    /// <returns>Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/settings/rebranding/company</path>
    /// <httpMethod>POST</httpMethod>
    /// <visible>false</visible>
    [HttpPost("rebranding/company")]
    public bool SaveCompanyWhiteLabelSettings(CompanyWhiteLabelSettingsWrapper companyWhiteLabelSettingsWrapper)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        DemandWhiteLabelPermission();

        if (companyWhiteLabelSettingsWrapper.Settings == null)
        {
            throw new ArgumentNullException("settings");
        }

        companyWhiteLabelSettingsWrapper.Settings.IsLicensor = false;

        _settingsManager.Save(companyWhiteLabelSettingsWrapper.Settings);

        return true;
    }

    /// <summary>
    /// Returns the company white label settings.
    /// </summary>
    /// <category>Rebranding</category>
    /// <short>Get the company white label settings</short>
    /// <returns>Company white label settings: company name, site, email, address, phone, licensor or not, default setings or not</returns>
    /// <path>api/2.0/settings/rebranding/company</path>
    /// <httpMethod>GET</httpMethod>
    ///<visible>false</visible>
    [AllowNotPayment]
    [HttpGet("rebranding/company")]
    public CompanyWhiteLabelSettingsDto GetCompanyWhiteLabelSettings()
    {
        return _mapper.Map<CompanyWhiteLabelSettings, CompanyWhiteLabelSettingsDto>(_settingsManager.Load<CompanyWhiteLabelSettings>());
    }

    /// <summary>
    /// Deletes the company white label settings.
    /// </summary>
    /// <category>Rebranding</category>
    /// <short>Delete the company white label settings</short>
    /// <returns>Default company white label settings: company name, site, email, address, phone, licensor or not</returns>
    /// <path>api/2.0/settings/rebranding/company</path>
    /// <httpMethod>DELETE</httpMethod>
    /// <visible>false</visible>
    [HttpDelete("rebranding/company")]
    public CompanyWhiteLabelSettings DeleteCompanyWhiteLabelSettings()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        DemandWhiteLabelPermission();

        var defaultSettings = _settingsManager.GetDefault<CompanyWhiteLabelSettings>();

        _settingsManager.Save(defaultSettings);

        return defaultSettings;
    }

    /// <summary>
    /// Saves the additional white label settings specified in the request.
    /// </summary>
    /// <category>Rebranding</category>
    /// <short>Save the additional white label settings</short>
    /// <param type="ASC.Web.Core.WhiteLabel.AdditionalWhiteLabelSettingsWrapper, ASC.Web.Core.WhiteLabel" name="wrapper">Additional white label settings: <![CDATA[
    /// <ul>
    ///     <li><b>Settings</b> (AdditionalWhiteLabelSettings) - additional white label settings:</li>
    ///     <ul>
    ///         <li><b>StartDocsEnabled</b> (bool) - specifies if the start document is enabled or not,</li>
    ///         <li><b>HelpCenterEnabled</b> (bool) - specifies if the help center is enabled or not,</li>
    ///         <li><b>FeedbackAndSupportEnabled</b> (bool) - specifies if feedback and support are available or not,</li>
    ///         <li><b>FeedbackAndSupportUrl</b> (string) - feedback and support URL,</li>
    ///         <li><b>UserForumEnabled</b> (bool) - specifies if the user forum is enabled or not,</li>
    ///         <li><b>UserForumUrl</b> (string) - user forum URL,</li>
    ///         <li><b>VideoGuidesEnabled</b> (bool) - specifies if the video guides are enabled or not,</li>
    ///         <li><b>VideoGuidesUrl</b> (string) - video guides URL,</li>
    ///         <li><b>SalesEmail</b> (string) - sales email,</li>
    ///         <li><b>BuyUrl</b> (string) - URL to pay for the portal,</li>
    ///         <li><b>LicenseAgreementsEnabled</b> (bool) - specifies if the license agreements are enabled or not,</li>
    ///         <li><b>LicenseAgreementsUrl</b> (string) - license agreements URL.</li>
    ///     </ul>
    /// </ul>
    /// ]]></param>
    /// <returns>Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/settings/rebranding/additional</path>
    /// <httpMethod>POST</httpMethod>
    ///<visible>false</visible>
    [HttpPost("rebranding/additional")]
    public bool SaveAdditionalWhiteLabelSettings(AdditionalWhiteLabelSettingsWrapper wrapper)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        DemandWhiteLabelPermission();

        if (wrapper.Settings == null)
        {
            throw new ArgumentNullException("settings");
        }

        _settingsManager.Save(wrapper.Settings);

        return true;
    }

    /// <summary>
    /// Returns the additional white label settings.
    /// </summary>
    /// <category>Rebranding</category>
    /// <short>Get the additional white label settings</short>
    /// <returns>Additional white label settings: start document is enabled or not, the help center is enabled or not, feedback and support are available or not, feedback and support URL, the user forum is enabled or not, user forum URL, the video guides are enabled or not, video guides URL, sales email, URL to pay for the portal, the license agreements are enabled or not, license agreements URL, default settings or not</returns>
    /// <path>api/2.0/settings/rebranding/additional</path>
    /// <httpMethod>GET</httpMethod>
    ///<visible>false</visible>
    [AllowNotPayment]
    [HttpGet("rebranding/additional")]
    public AdditionalWhiteLabelSettingsDto GetAdditionalWhiteLabelSettings()
    {
        return _mapper.Map<AdditionalWhiteLabelSettings, AdditionalWhiteLabelSettingsDto>(_settingsManager.Load<AdditionalWhiteLabelSettings>());
    }

    /// <summary>
    /// Deletes the additional white label settings.
    /// </summary>
    /// <category>Rebranding</category>
    /// <short>Delete the additional white label settings</short>
    /// <returns>Default additional white label settings: start document is enabled or not, the help center is enabled or not, feedback and support are available or not, feedback and support URL, the user forum is enabled or not, user forum URL, the video guides are enabled or not, video guides URL, sales email, URL to pay for the portal, the license agreements are enabled or not, license agreements URL</returns>
    /// <path>api/2.0/settings/rebranding/additional</path>
    /// <httpMethod>DELETE</httpMethod>
    ///<visible>false</visible>
    [HttpDelete("rebranding/additional")]
    public AdditionalWhiteLabelSettings DeleteAdditionalWhiteLabelSettings()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        DemandWhiteLabelPermission();

        var defaultSettings = _settingsManager.GetDefault<AdditionalWhiteLabelSettings>();

        _settingsManager.Save(defaultSettings);

        return defaultSettings;
    }

    /// <summary>
    /// Saves the mail white label settings specified in the request.
    /// </summary>
    /// <category>Rebranding</category>
    /// <short>Save the mail white label settings</short>
    /// <param type="ASC.Web.Core.WhiteLabel.MailWhiteLabelSettings, ASC.Web.Core.WhiteLabel" name="settings">Mail white label settings: <![CDATA[
    /// <ul>
    ///     <li><b>Settings</b> (AdditionalWhiteLabelSettings) - additional white label settings:</li>
    ///     <ul>
    ///         <li><b>FooterEnabled</b> (bool) - specifies if the mail footer is enabled or not,</li>
    ///         <li><b>FooterSocialEnabled</b> (bool) - specifies if the footer with social media contacts is enabled or not,</li>
    ///         <li><b>SupportUrl</b> (string) - support URL,</li>
    ///         <li><b>SupportEmail</b> (string) - support email,</li>
    ///         <li><b>SalesEmail</b> (string) - sales email,</li>
    ///         <li><b>DemoUrl</b> (string) - demo URL,</li>
    ///         <li><b>SiteUrl</b> (string) - site URL.</li>
    ///     </ul>
    /// </ul>
    /// ]]></param>
    /// <returns>Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/settings/rebranding/mail</path>
    /// <httpMethod>POST</httpMethod>
    ///<visible>false</visible>
    [HttpPost("rebranding/mail")]
    public bool SaveMailWhiteLabelSettings(MailWhiteLabelSettings settings)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        DemandWhiteLabelPermission();

        ArgumentNullException.ThrowIfNull(settings);

        _settingsManager.Save(settings);
        return true;
    }

    /// <summary>
    /// Updates the mail white label settings with a paramater specified in the request.
    /// </summary>
    /// <category>Rebranding</category>
    /// <short>Update the mail white label settings</short>
    /// <param type="ASC.Web.Api.ApiModel.RequestsDto.MailWhiteLabelSettingsRequestsDto, ASC.Web.Api.ApiModel.RequestsDto" name="inDto">Request parameters for mail white label settings: FooterEnabled (bool) - specifies if the mail footer will be enabled or not</param>
    /// <returns>Boolean value: true if the operation is successful</returns>
    /// <path>api/2.0/settings/rebranding/mail</path>
    /// <httpMethod>PUT</httpMethod>
    ///<visible>false</visible>
    [HttpPut("rebranding/mail")]
    public bool UpdateMailWhiteLabelSettings(MailWhiteLabelSettingsRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        DemandWhiteLabelPermission();

        _settingsManager.Manage<MailWhiteLabelSettings>(settings =>
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
    /// <returns>Mail white label settings: the mail footer is enabled or not, the footer with social media contacts is enabled or not, support URL, support email, sales email, demo URL, site URL</returns>
    /// <path>api/2.0/settings/rebranding/mail</path>
    /// <httpMethod>GET</httpMethod>
    ///<visible>false</visible>
    [HttpGet("rebranding/mail")]
    public MailWhiteLabelSettings GetMailWhiteLabelSettings()
    {
        return _settingsManager.Load<MailWhiteLabelSettings>();
    }

    /// <summary>
    /// Deletes the mail white label settings.
    /// </summary>
    /// <category>Rebranding</category>
    /// <short>Delete the mail white label settings</short>
    /// <returns>Default mail white label settings: the mail footer is enabled or not, the footer with social media contacts is enabled or not, support URL, support email, sales email, demo URL, site URL</returns>
    /// <path>api/2.0/settings/rebranding/mail</path>
    /// <httpMethod>DELETE</httpMethod>
    ///<visible>false</visible>
    [HttpDelete("rebranding/mail")]
    public MailWhiteLabelSettings DeleteMailWhiteLabelSettings()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        DemandWhiteLabelPermission();

        var defaultSettings = _settingsManager.GetDefault<MailWhiteLabelSettings>();

        _settingsManager.Save(defaultSettings);

        return defaultSettings;
    }

    /// <summary>
    /// Checks if the white label is enabled or not.
    /// </summary>
    /// <category>Rebranding</category>
    /// <short>Check the white label availability</short>
    /// <returns>Boolean value: true if the white label is enabled</returns>
    /// <path>api/2.0/settings/enableWhitelabel</path>
    /// <httpMethod>GET</httpMethod>
    ///<visible>false</visible>
    [HttpGet("enableWhitelabel")]
    public bool GetEnableWhitelabel()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        return _coreBaseSettings.Standalone || _tenantLogoManager.WhiteLabelEnabled && _tenantLogoManager.WhiteLabelPaid;
    }

    private void DemandWhiteLabelPermission()
    {
        if (!_coreBaseSettings.Standalone && (!_tenantLogoManager.WhiteLabelEnabled || !_tenantLogoManager.WhiteLabelPaid))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "WhiteLabel");
        }
    }
}
