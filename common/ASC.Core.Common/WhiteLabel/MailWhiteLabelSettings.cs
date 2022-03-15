namespace ASC.Web.Core.WhiteLabel;

[Serializable]
public class MailWhiteLabelSettings : ISettings
{
    public bool FooterEnabled { get; set; }
    public bool FooterSocialEnabled { get; set; }
    public string SupportUrl { get; set; }
    public string SupportEmail { get; set; }
    public string SalesEmail { get; set; }
    public string DemoUrl { get; set; }
    public string SiteUrl { get; set; }

    public Guid ID => new Guid("{C3602052-5BA2-452A-BD2A-ADD0FAF8EB88}");

    public ISettings GetDefault(IConfiguration configuration)
    {
        var mailWhiteLabelSettingsHelper = new MailWhiteLabelSettingsHelper(configuration);

        return new MailWhiteLabelSettings
        {
            FooterEnabled = true,
            FooterSocialEnabled = true,
            SupportUrl = mailWhiteLabelSettingsHelper.DefaultMailSupportUrl,
            SupportEmail = mailWhiteLabelSettingsHelper.DefaultMailSupportEmail,
            SalesEmail = mailWhiteLabelSettingsHelper.DefaultMailSalesEmail,
            DemoUrl = mailWhiteLabelSettingsHelper.DefaultMailDemoUrl,
            SiteUrl = mailWhiteLabelSettingsHelper.DefaultMailSiteUrl
        };
    }

    public bool IsDefault(IConfiguration configuration)
    {
        if (!(GetDefault(configuration) is MailWhiteLabelSettings defaultSettings))
        {
            return false;
        }

        return FooterEnabled == defaultSettings.FooterEnabled &&
                FooterSocialEnabled == defaultSettings.FooterSocialEnabled &&
                SupportUrl == defaultSettings.SupportUrl &&
                SupportEmail == defaultSettings.SupportEmail &&
                SalesEmail == defaultSettings.SalesEmail &&
                DemoUrl == defaultSettings.DemoUrl &&
                SiteUrl == defaultSettings.SiteUrl;
    }

    public static MailWhiteLabelSettings Instance(SettingsManager settingsManager)
    {
        return settingsManager.LoadForDefaultTenant<MailWhiteLabelSettings>();
    }
    public static bool IsDefault(SettingsManager settingsManager, IConfiguration configuration)
    {
        return Instance(settingsManager).IsDefault(configuration);
    }

    public ISettings GetDefault(IServiceProvider serviceProvider)
    {
        return GetDefault(serviceProvider.GetService<IConfiguration>());
    }
}

[Singletone]
public class MailWhiteLabelSettingsHelper
{
    public MailWhiteLabelSettingsHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string DefaultMailSupportUrl
    {
        get
        {
            var url = BaseCommonLinkUtility.GetRegionalUrl(_configuration["web:support-feedback"] ?? string.Empty, null);

            return !string.IsNullOrEmpty(url) ? url : "http://helpdesk.onlyoffice.com";
        }
    }

    public string DefaultMailSupportEmail
    {
        get
        {
            var email = _configuration["web:support:email"];

            return !string.IsNullOrEmpty(email) ? email : "support@onlyoffice.com";
        }
    }

    public string DefaultMailSalesEmail
    {
        get
        {
            var email = _configuration["web:payment:email"];

            return !string.IsNullOrEmpty(email) ? email : "sales@onlyoffice.com";
        }
    }

    public string DefaultMailDemoUrl
    {
        get
        {
            var url = BaseCommonLinkUtility.GetRegionalUrl(_configuration["web:demo-order"] ?? string.Empty, null);

            return !string.IsNullOrEmpty(url) ? url : "http://www.onlyoffice.com/demo-order.aspx";
        }
    }

    public string DefaultMailSiteUrl
    {
        get
        {
            var url = _configuration["web:teamlab-site"];

            return !string.IsNullOrEmpty(url) ? url : "http://www.onlyoffice.com";
        }
    }

    private readonly IConfiguration _configuration;
}
