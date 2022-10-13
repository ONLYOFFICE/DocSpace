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
        IHttpContextAccessor httpContextAccessor) : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _permissionContext = permissionContext;
        _settingsManager = settingsManager;
        _tenantInfoSettingsHelper = tenantInfoSettingsHelper;
        _tenantWhiteLabelSettingsHelper = tenantWhiteLabelSettingsHelper;
        _tenantLogoManager = tenantLogoManager;
        _coreBaseSettings = coreBaseSettings;
        _commonLinkUtility = commonLinkUtility;
    }

    ///<visible>false</visible>
    [HttpPost("whitelabel/save")]
    public bool SaveWhiteLabelSettings(WhiteLabelRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        DemandWhiteLabelPermission();

        var settings = _settingsManager.Load<TenantWhiteLabelSettings>();

        if (inDto.Logo != null)
        {
            var logoDict = new Dictionary<int, string>();

            foreach (var l in inDto.Logo)
            {
                logoDict.Add(Int32.Parse(l.Key), l.Value);
            }

            _tenantWhiteLabelSettingsHelper.SetLogo(settings, logoDict, null);
        }

        settings.SetLogoText(inDto.LogoText);
        _tenantWhiteLabelSettingsHelper.Save(settings, Tenant.Id, _tenantLogoManager);

        return true;
    }

    ///<visible>false</visible>
    [HttpPost("whitelabel/savefromfiles")]
    public bool SaveWhiteLabelSettingsFromFiles()
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
            var parts = f.FileName.Split('.');
            var logoType = (WhiteLabelLogoTypeEnum)Convert.ToInt32(parts[0]);
            var fileExt = parts[1];
            _tenantWhiteLabelSettingsHelper.SetLogoFromStream(settings, logoType, fileExt, f.OpenReadStream(), null);
        }

        _settingsManager.SaveForTenant(settings, Tenant.Id);

        return true;
    }

    ///<visible>false</visible>
    [HttpGet("whitelabel/sizes")]
    public object GetWhiteLabelSizes()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        DemandWhiteLabelPermission();

        return
        new[]
        {
            new {type = (int)WhiteLabelLogoTypeEnum.LightSmall, name = nameof(WhiteLabelLogoTypeEnum.LightSmall), height = TenantWhiteLabelSettings.LogoLightSmallSize.Height, width = TenantWhiteLabelSettings.LogoLightSmallSize.Width},
            new {type = (int)WhiteLabelLogoTypeEnum.Dark, name = nameof(WhiteLabelLogoTypeEnum.Dark), height = TenantWhiteLabelSettings.LogoDarkSize.Height, width = TenantWhiteLabelSettings.LogoDarkSize.Width},
            new {type = (int)WhiteLabelLogoTypeEnum.Favicon, name = nameof(WhiteLabelLogoTypeEnum.Favicon), height = TenantWhiteLabelSettings.LogoFaviconSize.Height, width = TenantWhiteLabelSettings.LogoFaviconSize.Width},
            new {type = (int)WhiteLabelLogoTypeEnum.DocsEditor, name = nameof(WhiteLabelLogoTypeEnum.DocsEditor), height = TenantWhiteLabelSettings.LogoDocsEditorSize.Height, width = TenantWhiteLabelSettings.LogoDocsEditorSize.Width},
            new {type = (int)WhiteLabelLogoTypeEnum.DocsEditorEmbed, name = nameof(WhiteLabelLogoTypeEnum.DocsEditorEmbed), height = TenantWhiteLabelSettings.LogoDocsEditorEmbedSize.Height, width = TenantWhiteLabelSettings.LogoDocsEditorEmbedSize.Width},
            new {type = (int)WhiteLabelLogoTypeEnum.LeftMenu, name =  nameof(WhiteLabelLogoTypeEnum.LeftMenu), height = TenantWhiteLabelSettings.LogoLeftMenuSize.Height, width = TenantWhiteLabelSettings.LogoLeftMenuSize.Width},
            new {type = (int)WhiteLabelLogoTypeEnum.AboutPage, name =  nameof(WhiteLabelLogoTypeEnum.AboutPage), height = TenantWhiteLabelSettings.LogoAboutPageSize.Height, width = TenantWhiteLabelSettings.LogoAboutPageSize.Width}
        };
    }



    ///<visible>false</visible>
    [HttpGet("whitelabel/logos")]
    public Dictionary<string, string> GetWhiteLabelLogos([FromQuery] WhiteLabelQueryRequestsDto inDto)
    {
        Dictionary<string, string> result;

        var _tenantWhiteLabelSettings = _settingsManager.Load<TenantWhiteLabelSettings>();

        result = new Dictionary<string, string>
            {
                { ((int)WhiteLabelLogoTypeEnum.LightSmall).ToString(), _commonLinkUtility.GetFullAbsolutePath(_tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.LightSmall, !inDto.IsRetina)) },
                { ((int)WhiteLabelLogoTypeEnum.Dark).ToString(), _commonLinkUtility.GetFullAbsolutePath(_tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.Dark, !inDto.IsRetina)) },
                { ((int)WhiteLabelLogoTypeEnum.Favicon).ToString(), _commonLinkUtility.GetFullAbsolutePath(_tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.Favicon, !inDto.IsRetina)) },
                { ((int)WhiteLabelLogoTypeEnum.DocsEditor).ToString(), _commonLinkUtility.GetFullAbsolutePath(_tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.DocsEditor, !inDto.IsRetina)) },
                { ((int)WhiteLabelLogoTypeEnum.DocsEditorEmbed).ToString(), _commonLinkUtility.GetFullAbsolutePath(_tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings,WhiteLabelLogoTypeEnum.DocsEditorEmbed, !inDto.IsRetina)) },
                { ((int)WhiteLabelLogoTypeEnum.LeftMenu).ToString(), _commonLinkUtility.GetFullAbsolutePath(_tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings,WhiteLabelLogoTypeEnum.LeftMenu, !inDto.IsRetina)) },
                { ((int)WhiteLabelLogoTypeEnum.AboutPage).ToString(), _commonLinkUtility.GetFullAbsolutePath(_tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings,WhiteLabelLogoTypeEnum.AboutPage, !inDto.IsRetina)) }
            };

        return result;
    }

    ///<visible>false</visible>
    [HttpGet("whitelabel/logotext")]
    public object GetWhiteLabelLogoText()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        DemandWhiteLabelPermission();

        var settings = _settingsManager.Load<TenantWhiteLabelSettings>();

        return settings.LogoText ?? TenantWhiteLabelSettings.DefaultLogoText;
    }


    ///<visible>false</visible>
    [HttpPut("whitelabel/restore")]
    public bool RestoreWhiteLabelOptions()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);
        DemandWhiteLabelPermission();

        var settings = _settingsManager.Load<TenantWhiteLabelSettings>();

        _tenantWhiteLabelSettingsHelper.RestoreDefault(settings, _tenantLogoManager, Tenant.Id, null);

        var tenantInfoSettings = _settingsManager.Load<TenantInfoSettings>();
        _tenantInfoSettingsHelper.RestoreDefaultLogo(tenantInfoSettings, _tenantLogoManager);
        _settingsManager.Save(tenantInfoSettings);

        return true;
    }

    ///<visible>false</visible>
    [HttpGet("companywhitelabel")]
    public List<CompanyWhiteLabelSettings> GetLicensorData()
    {
        var result = new List<CompanyWhiteLabelSettings>();

        var instance = CompanyWhiteLabelSettings.Instance(_settingsManager);

        result.Add(instance);

        if (!instance.IsDefault && !instance.IsLicensor)
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

    ///<visible>false</visible>
    [HttpGet("rebranding/company")]
    public CompanyWhiteLabelSettings GetCompanyWhiteLabelSettings()
    {
        return _settingsManager.Load<CompanyWhiteLabelSettings>();
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

    ///<visible>false</visible>
    [HttpGet("rebranding/additional")]
    public AdditionalWhiteLabelSettings GetAdditionalWhiteLabelSettings()
    {
        return _settingsManager.Load<AdditionalWhiteLabelSettings>();
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

        var settings = _settingsManager.Load<MailWhiteLabelSettings>();

        settings.FooterEnabled = inDto.FooterEnabled;

        _settingsManager.Save(settings);

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
