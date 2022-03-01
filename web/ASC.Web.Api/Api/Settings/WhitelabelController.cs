using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Api.Controllers.Settings;

public class WhitelabelController: BaseSettingsController
{
    private Tenant Tenant { get { return _apiContext.Tenant; } }

    private readonly IServiceProvider _serviceProvider;
    private readonly TenantManager _tenantManager;
    private readonly TenantExtra _tenantExtra;
    private readonly PermissionContext _permissionContext;
    private readonly SettingsManager _settingsManager;
    private readonly TenantInfoSettingsHelper _tenantInfoSettingsHelper;
    private readonly TenantWhiteLabelSettingsHelper _tenantWhiteLabelSettingsHelper;
    private readonly TenantLogoManager _tenantLogoManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly IConfiguration _configuration;
    private readonly CoreSettings _coreSettings;
    private readonly StorageFactory _storageFactory;

    public WhitelabelController(
        ApiContext apiContext,
        TenantManager tenantManager,
        TenantExtra tenantExtra,
        PermissionContext permissionContext,
        SettingsManager settingsManager,
        WebItemManager webItemManager,
        TenantInfoSettingsHelper tenantInfoSettingsHelper,
        TenantWhiteLabelSettingsHelper tenantWhiteLabelSettingsHelper,
        TenantLogoManager tenantLogoManager,
        CoreBaseSettings coreBaseSettings,
        CommonLinkUtility commonLinkUtility,
        IConfiguration configuration,
        CoreSettings coreSettings,
        IServiceProvider serviceProvider,
        IMemoryCache memoryCache,
        StorageFactory storageFactory) : base(apiContext, memoryCache, webItemManager)
    {
        _serviceProvider = serviceProvider;
        _tenantManager = tenantManager;
        _tenantExtra = tenantExtra;
        _permissionContext = permissionContext;
        _settingsManager = settingsManager;
        _tenantInfoSettingsHelper = tenantInfoSettingsHelper;
        _tenantWhiteLabelSettingsHelper = tenantWhiteLabelSettingsHelper;
        _tenantLogoManager = tenantLogoManager;
        _coreBaseSettings = coreBaseSettings;
        _commonLinkUtility = commonLinkUtility;
        _configuration = configuration;
        _coreSettings = coreSettings;
        _storageFactory = storageFactory;
    }

    ///<visible>false</visible>
    [Create("whitelabel/save")]
    public bool SaveWhiteLabelSettingsFromBody([FromBody] WhiteLabelDto model, [FromQuery] WhiteLabelQuery query)
    {
        return SaveWhiteLabelSettings(model, query);
    }

    [Create("whitelabel/save")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool SaveWhiteLabelSettingsFromForm([FromForm] WhiteLabelDto model, [FromQuery] WhiteLabelQuery query)
    {
        return SaveWhiteLabelSettings(model, query);
    }

    private bool SaveWhiteLabelSettings(WhiteLabelDto model, WhiteLabelQuery query)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (!_tenantLogoManager.WhiteLabelEnabled || !_tenantLogoManager.WhiteLabelPaid)
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "WhiteLabel");
        }

        if (query.IsDefault)
        {
            DemandRebrandingPermission();
            SaveWhiteLabelSettingsForDefaultTenant(model);
        }
        else
        {
            SaveWhiteLabelSettingsForCurrentTenant(model);
        }
        return true;
    }

    private void SaveWhiteLabelSettingsForCurrentTenant(WhiteLabelDto model)
    {
        var settings = _settingsManager.Load<TenantWhiteLabelSettings>();

        SaveWhiteLabelSettingsForTenant(settings, null, Tenant.Id, model);
    }

    private void SaveWhiteLabelSettingsForDefaultTenant(WhiteLabelDto model)
    {
        var settings = _settingsManager.LoadForDefaultTenant<TenantWhiteLabelSettings>();
        var storage = _storageFactory.GetStorage(string.Empty, "static_partnerdata");

        SaveWhiteLabelSettingsForTenant(settings, storage, Tenant.DefaultTenant, model);
    }

    private void SaveWhiteLabelSettingsForTenant(TenantWhiteLabelSettings settings, IDataStore storage, int tenantId, WhiteLabelDto model)
    {
        if (model.Logo != null)
        {
            var logoDict = new Dictionary<int, string>();

            foreach (var l in model.Logo)
            {
                logoDict.Add(Int32.Parse(l.Key), l.Value);
            }

            _tenantWhiteLabelSettingsHelper.SetLogo(settings, logoDict, storage);
        }

        settings.SetLogoText(model.LogoText);
        _tenantWhiteLabelSettingsHelper.Save(settings, tenantId, _tenantLogoManager);

    }

    ///<visible>false</visible>
    [Create("whitelabel/savefromfiles")]
    public bool SaveWhiteLabelSettingsFromFiles([FromQuery] WhiteLabelQuery query)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (!_tenantLogoManager.WhiteLabelEnabled || !_tenantLogoManager.WhiteLabelPaid)
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "WhiteLabel");
        }

        if (HttpContext.Request.Form?.Files == null || HttpContext.Request.Form.Files.Count == 0)
        {
            throw new InvalidOperationException("No input files");
        }

        if (query.IsDefault)
        {
            DemandRebrandingPermission();
            SaveWhiteLabelSettingsFromFilesForDefaultTenant();
        }
        else
        {
            SaveWhiteLabelSettingsFromFilesForCurrentTenant();
        }
        return true;
    }

    private void SaveWhiteLabelSettingsFromFilesForCurrentTenant()
    {
        var settings = _settingsManager.Load<TenantWhiteLabelSettings>();

        SaveWhiteLabelSettingsFromFilesForTenant(settings, null, Tenant.Id);
    }

    private void SaveWhiteLabelSettingsFromFilesForDefaultTenant()
    {
        var settings = _settingsManager.LoadForDefaultTenant<TenantWhiteLabelSettings>();
        var storage = _storageFactory.GetStorage(string.Empty, "static_partnerdata");

        SaveWhiteLabelSettingsFromFilesForTenant(settings, storage, Tenant.DefaultTenant);
    }

    private void SaveWhiteLabelSettingsFromFilesForTenant(TenantWhiteLabelSettings settings, IDataStore storage, int tenantId)
    {
        foreach (var f in HttpContext.Request.Form.Files)
        {
            var parts = f.FileName.Split('.');
            var logoType = (WhiteLabelLogoTypeEnum)Convert.ToInt32(parts[0]);
            var fileExt = parts[1];
            _tenantWhiteLabelSettingsHelper.SetLogoFromStream(settings, logoType, fileExt, f.OpenReadStream(), storage);
        }

        _settingsManager.SaveForTenant(settings, tenantId);
    }


    ///<visible>false</visible>
    [Read("whitelabel/sizes")]
    public object GetWhiteLabelSizes()
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (!_tenantLogoManager.WhiteLabelEnabled)
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "WhiteLabel");
        }

        return
        new[]
        {
            new {type = (int)WhiteLabelLogoTypeEnum.LightSmall, name = nameof(WhiteLabelLogoTypeEnum.LightSmall), height = TenantWhiteLabelSettings.logoLightSmallSize.Height, width = TenantWhiteLabelSettings.logoLightSmallSize.Width},
            new {type = (int)WhiteLabelLogoTypeEnum.Dark, name = nameof(WhiteLabelLogoTypeEnum.Dark), height = TenantWhiteLabelSettings.logoDarkSize.Height, width = TenantWhiteLabelSettings.logoDarkSize.Width},
            new {type = (int)WhiteLabelLogoTypeEnum.Favicon, name = nameof(WhiteLabelLogoTypeEnum.Favicon), height = TenantWhiteLabelSettings.logoFaviconSize.Height, width = TenantWhiteLabelSettings.logoFaviconSize.Width},
            new {type = (int)WhiteLabelLogoTypeEnum.DocsEditor, name = nameof(WhiteLabelLogoTypeEnum.DocsEditor), height = TenantWhiteLabelSettings.logoDocsEditorSize.Height, width = TenantWhiteLabelSettings.logoDocsEditorSize.Width},
            new {type = (int)WhiteLabelLogoTypeEnum.DocsEditorEmbed, name =  nameof(WhiteLabelLogoTypeEnum.DocsEditorEmbed), height = TenantWhiteLabelSettings.logoDocsEditorEmbedSize.Height, width = TenantWhiteLabelSettings.logoDocsEditorEmbedSize.Width}

        };
    }



    ///<visible>false</visible>
    [Read("whitelabel/logos")]
    public Dictionary<string, string> GetWhiteLabelLogos([FromQuery] WhiteLabelQuery query)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (!_tenantLogoManager.WhiteLabelEnabled)
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "WhiteLabel");
        }

        Dictionary<string, string> result;

        if (query.IsDefault)
        {
            result = new Dictionary<string, string>
            {
                { ((int)WhiteLabelLogoTypeEnum.LightSmall).ToString(), _commonLinkUtility.GetFullAbsolutePath(_tenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.LightSmall, !query.IsRetina)) },
                { ((int)WhiteLabelLogoTypeEnum.Dark).ToString(), _commonLinkUtility.GetFullAbsolutePath(_tenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.Dark, !query.IsRetina)) },
                { ((int)WhiteLabelLogoTypeEnum.Favicon).ToString(), _commonLinkUtility.GetFullAbsolutePath(_tenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.Favicon, !query.IsRetina)) },
                { ((int)WhiteLabelLogoTypeEnum.DocsEditor).ToString(), _commonLinkUtility.GetFullAbsolutePath(_tenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.DocsEditor, !query.IsRetina)) },
                { ((int)WhiteLabelLogoTypeEnum.DocsEditorEmbed).ToString(), _commonLinkUtility.GetFullAbsolutePath(_tenantWhiteLabelSettingsHelper.GetAbsoluteDefaultLogoPath(WhiteLabelLogoTypeEnum.DocsEditorEmbed, !query.IsRetina)) }
            };
        }
        else
        {
            var _tenantWhiteLabelSettings = _settingsManager.Load<TenantWhiteLabelSettings>();

            result = new Dictionary<string, string>
                {
                    { ((int)WhiteLabelLogoTypeEnum.LightSmall).ToString(), _commonLinkUtility.GetFullAbsolutePath(_tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.LightSmall, !query.IsRetina)) },
                    { ((int)WhiteLabelLogoTypeEnum.Dark).ToString(), _commonLinkUtility.GetFullAbsolutePath(_tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.Dark, !query.IsRetina)) },
                    { ((int)WhiteLabelLogoTypeEnum.Favicon).ToString(), _commonLinkUtility.GetFullAbsolutePath(_tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.Favicon, !query.IsRetina)) },
                    { ((int)WhiteLabelLogoTypeEnum.DocsEditor).ToString(), _commonLinkUtility.GetFullAbsolutePath(_tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings, WhiteLabelLogoTypeEnum.DocsEditor, !query.IsRetina)) },
                    { ((int)WhiteLabelLogoTypeEnum.DocsEditorEmbed).ToString(), _commonLinkUtility.GetFullAbsolutePath(_tenantWhiteLabelSettingsHelper.GetAbsoluteLogoPath(_tenantWhiteLabelSettings,WhiteLabelLogoTypeEnum.DocsEditorEmbed, !query.IsRetina)) }
                };
        }

        return result;
    }

    ///<visible>false</visible>
    [Read("whitelabel/logotext")]
    public object GetWhiteLabelLogoText([FromQuery] WhiteLabelQuery query)
    {
        if (!_tenantLogoManager.WhiteLabelEnabled)
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "WhiteLabel");
        }

        var settings = query.IsDefault ? _settingsManager.LoadForDefaultTenant<TenantWhiteLabelSettings>() : _settingsManager.Load<TenantWhiteLabelSettings>();

        return settings.LogoText ?? TenantWhiteLabelSettings.DefaultLogoText;
    }


    ///<visible>false</visible>
    [Update("whitelabel/restore")]
    public bool RestoreWhiteLabelOptions(WhiteLabelQuery query)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (!_tenantLogoManager.WhiteLabelEnabled || !_tenantLogoManager.WhiteLabelPaid)
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "WhiteLabel");
        }
        if (query.IsDefault)
        {
            DemandRebrandingPermission();
            RestoreWhiteLabelOptionsForDefaultTenant();
        }
        else
        {
            RestoreWhiteLabelOptionsForCurrentTenant();
        }
        return true;
    }

    private void RestoreWhiteLabelOptionsForCurrentTenant()
    {
        var settings = _settingsManager.Load<TenantWhiteLabelSettings>();

        RestoreWhiteLabelOptionsForTenant(settings, null, Tenant.Id);

        var tenantInfoSettings = _settingsManager.Load<TenantInfoSettings>();
        _tenantInfoSettingsHelper.RestoreDefaultLogo(tenantInfoSettings, _tenantLogoManager);
        _settingsManager.Save(tenantInfoSettings);
    }

    private void RestoreWhiteLabelOptionsForDefaultTenant()
    {
        var settings = _settingsManager.LoadForDefaultTenant<TenantWhiteLabelSettings>();
        var storage = _storageFactory.GetStorage(string.Empty, "static_partnerdata");

        RestoreWhiteLabelOptionsForTenant(settings, storage, Tenant.DefaultTenant);
    }

    private void RestoreWhiteLabelOptionsForTenant(TenantWhiteLabelSettings settings, IDataStore storage, int tenantId)
    {
        _tenantWhiteLabelSettingsHelper.RestoreDefault(settings, _tenantLogoManager, tenantId, storage);
    }

    ///<visible>false</visible>
    [Read("companywhitelabel")]
    public List<CompanyWhiteLabelSettings> GetLicensorData()
    {
        var result = new List<CompanyWhiteLabelSettings>();

        var instance = CompanyWhiteLabelSettings.Instance(_settingsManager);

        result.Add(instance);

        if (!instance.IsDefault(_coreSettings) && !instance.IsLicensor)
        {
            result.Add(instance.GetDefault(_serviceProvider) as CompanyWhiteLabelSettings);
        }

        return result;
    }

    ///<visible>false</visible>
    [Create("rebranding/company")]
    public bool SaveCompanyWhiteLabelSettingsFromBody([FromBody] CompanyWhiteLabelSettingsWrapper companyWhiteLabelSettingsWrapper)
    {
        return SaveCompanyWhiteLabelSettings(companyWhiteLabelSettingsWrapper);
    }

    [Create("rebranding/company")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool SaveCompanyWhiteLabelSettingsFromForm([FromForm] CompanyWhiteLabelSettingsWrapper companyWhiteLabelSettingsWrapper)
    {
        return SaveCompanyWhiteLabelSettings(companyWhiteLabelSettingsWrapper);
    }

    private bool SaveCompanyWhiteLabelSettings(CompanyWhiteLabelSettingsWrapper companyWhiteLabelSettingsWrapper)
    {
        if (companyWhiteLabelSettingsWrapper.Settings == null) throw new ArgumentNullException("settings");

        DemandRebrandingPermission();

        companyWhiteLabelSettingsWrapper.Settings.IsLicensor = false; //TODO: CoreContext.TenantManager.GetTenantQuota(TenantProvider.CurrentTenantID).Branding && settings.IsLicensor

        _settingsManager.SaveForDefaultTenant(companyWhiteLabelSettingsWrapper.Settings);
        return true;
    }

    ///<visible>false</visible>
    [Read("rebranding/company")]
    public CompanyWhiteLabelSettings GetCompanyWhiteLabelSettings()
    {
        return _settingsManager.LoadForDefaultTenant<CompanyWhiteLabelSettings>();
    }

    ///<visible>false</visible>
    [Delete("rebranding/company")]
    public CompanyWhiteLabelSettings DeleteCompanyWhiteLabelSettings()
    {
        DemandRebrandingPermission();

        var defaultSettings = (CompanyWhiteLabelSettings)_settingsManager.LoadForDefaultTenant<CompanyWhiteLabelSettings>().GetDefault(_coreSettings);

        _settingsManager.SaveForDefaultTenant(defaultSettings);

        return defaultSettings;
    }
    ///<visible>false</visible>
    [Create("rebranding/additional")]
    public bool SaveAdditionalWhiteLabelSettingsFromBody([FromBody] AdditionalWhiteLabelSettingsWrapper wrapper)
    {
        return SaveAdditionalWhiteLabelSettings(wrapper);
    }

    [Create("rebranding/additional")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool SaveAdditionalWhiteLabelSettingsFromForm([FromForm] AdditionalWhiteLabelSettingsWrapper wrapper)
    {
        return SaveAdditionalWhiteLabelSettings(wrapper);
    }

    private bool SaveAdditionalWhiteLabelSettings(AdditionalWhiteLabelSettingsWrapper wrapper)
    {
        if (wrapper.Settings == null) throw new ArgumentNullException("settings");

        DemandRebrandingPermission();

        _settingsManager.SaveForDefaultTenant(wrapper.Settings);
        return true;
    }

    ///<visible>false</visible>
    [Read("rebranding/additional")]
    public AdditionalWhiteLabelSettings GetAdditionalWhiteLabelSettings()
    {
        return _settingsManager.LoadForDefaultTenant<AdditionalWhiteLabelSettings>();
    }

    ///<visible>false</visible>
    [Delete("rebranding/additional")]
    public AdditionalWhiteLabelSettings DeleteAdditionalWhiteLabelSettings()
    {
        DemandRebrandingPermission();

        var defaultSettings = (AdditionalWhiteLabelSettings)_settingsManager.LoadForDefaultTenant<AdditionalWhiteLabelSettings>().GetDefault(_configuration);

        _settingsManager.SaveForDefaultTenant(defaultSettings);

        return defaultSettings;
    }

    ///<visible>false</visible>
    [Create("rebranding/mail")]
    public bool SaveMailWhiteLabelSettingsFromBody([FromBody] MailWhiteLabelSettings settings)
    {
        return SaveMailWhiteLabelSettings(settings);
    }

    ///<visible>false</visible>
    [Create("rebranding/mail")]
    public bool SaveMailWhiteLabelSettingsFromForm([FromForm] MailWhiteLabelSettings settings)
    {
        return SaveMailWhiteLabelSettings(settings);
    }

    private bool SaveMailWhiteLabelSettings(MailWhiteLabelSettings settings)
    {
        if (settings == null) throw new ArgumentNullException(nameof(settings));

        DemandRebrandingPermission();

        _settingsManager.SaveForDefaultTenant(settings);
        return true;
    }

    ///<visible>false</visible>
    [Update("rebranding/mail")]
    public bool UpdateMailWhiteLabelSettingsFromBody([FromBody] MailWhiteLabelSettingsDto model)
    {
        return UpdateMailWhiteLabelSettings(model);
    }

    [Update("rebranding/mail")]
    [Consumes("application/x-www-form-urlencoded")]
    public bool UpdateMailWhiteLabelSettingsFromForm([FromForm] MailWhiteLabelSettingsDto model)
    {
        return UpdateMailWhiteLabelSettings(model);
    }

    private bool UpdateMailWhiteLabelSettings(MailWhiteLabelSettingsDto model)
    {
        DemandRebrandingPermission();

        var settings = _settingsManager.LoadForDefaultTenant<MailWhiteLabelSettings>();

        settings.FooterEnabled = model.FooterEnabled;

        _settingsManager.SaveForDefaultTenant(settings);

        return true;
    }

    ///<visible>false</visible>
    [Read("rebranding/mail")]
    public MailWhiteLabelSettings GetMailWhiteLabelSettings()
    {
        return _settingsManager.LoadForDefaultTenant<MailWhiteLabelSettings>();
    }

    ///<visible>false</visible>
    [Delete("rebranding/mail")]
    public MailWhiteLabelSettings DeleteMailWhiteLabelSettings()
    {
        DemandRebrandingPermission();

        var defaultSettings = (MailWhiteLabelSettings)_settingsManager.LoadForDefaultTenant<MailWhiteLabelSettings>().GetDefault(_configuration);

        _settingsManager.SaveForDefaultTenant(defaultSettings);

        return defaultSettings;
    }

    private void DemandRebrandingPermission()
    {
        _tenantExtra.DemandControlPanelPermission();

        if (!_tenantManager.GetTenantQuota(Tenant.Id).SSBranding)
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "SSBranding");
        }

        if (_coreBaseSettings.CustomMode)
        {
            throw new SecurityException();
        }
    }
}
