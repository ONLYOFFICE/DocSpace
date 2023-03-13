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

    ///<visible>false</visible>
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

    ///<visible>false</visible>
    [AllowNotPayment]
    [HttpGet("whitelabel/logotext")]
    public object GetWhiteLabelLogoText()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        var settings = _settingsManager.Load<TenantWhiteLabelSettings>();

        return settings.LogoText ?? TenantWhiteLabelSettings.DefaultLogoText;
    }


    ///<visible>false</visible>
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

    ///<visible>false</visible>
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

    ///<visible>false</visible>
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
    [AllowNotPayment]
    ///<visible>false</visible>
    [HttpGet("rebranding/company")]
    public CompanyWhiteLabelSettingsDto GetCompanyWhiteLabelSettings()
    {
        return _mapper.Map<CompanyWhiteLabelSettings, CompanyWhiteLabelSettingsDto>(_settingsManager.Load<CompanyWhiteLabelSettings>());
    }

    ///<visible>false</visible>
    [HttpDelete("rebranding/company")]
    public CompanyWhiteLabelSettings DeleteCompanyWhiteLabelSettings()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        DemandWhiteLabelPermission();

        var defaultSettings = _settingsManager.GetDefault<CompanyWhiteLabelSettings>();

        _settingsManager.Save(defaultSettings);

        return defaultSettings;
    }

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
    [AllowNotPayment]
    ///<visible>false</visible>
    [HttpGet("rebranding/additional")]
    public AdditionalWhiteLabelSettingsDto GetAdditionalWhiteLabelSettings()
    {
        return _mapper.Map<AdditionalWhiteLabelSettings, AdditionalWhiteLabelSettingsDto>(_settingsManager.Load<AdditionalWhiteLabelSettings>());
    }

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

    ///<visible>false</visible>
    [HttpGet("rebranding/mail")]
    public MailWhiteLabelSettings GetMailWhiteLabelSettings()
    {
        return _settingsManager.Load<MailWhiteLabelSettings>();
    }

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
