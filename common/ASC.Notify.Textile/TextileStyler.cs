namespace ASC.Notify.Textile;

[Scope]
public class TextileStyler : IPatternStyler
{
    private static readonly Regex _velocityArguments
        = new Regex(NVelocityPatternFormatter.NoStylePreffix + "(?<arg>.*?)" + NVelocityPatternFormatter.NoStyleSuffix,
            RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly IConfiguration _configuration;
    private readonly InstanceCrypto _instanceCrypto;
    private readonly MailWhiteLabelSettingsHelper _mailWhiteLabelSettingsHelper;

    static TextileStyler()
    {
        const string file = "ASC.Notify.Textile.Resources.style.css";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(file);
        using var reader = new StreamReader(stream);
        BlockAttributesParser.Styler = new StyleReader(reader.ReadToEnd().Replace("\n", "").Replace("\r", ""));
    }

    public TextileStyler(
        CoreBaseSettings coreBaseSettings,
        IConfiguration configuration,
        InstanceCrypto instanceCrypto,
        MailWhiteLabelSettingsHelper mailWhiteLabelSettingsHelper)
    {
        _coreBaseSettings = coreBaseSettings;
        _configuration = configuration;
        _instanceCrypto = instanceCrypto;
        _mailWhiteLabelSettingsHelper = mailWhiteLabelSettingsHelper;
    }

    public void ApplyFormating(NoticeMessage message)
    {
        var output = new StringBuilderTextileFormatter();
        var formatter = new TextileFormatter(output);

        if (!string.IsNullOrEmpty(message.Subject))
        {
            message.Subject = _velocityArguments.Replace(message.Subject, m => m.Result("${arg}"));
        }

        if (string.IsNullOrEmpty(message.Body))
        {
            return;
        }

        formatter.Format(message.Body);

        var template = GetTemplate(message);
        var imagePath = GetImagePath(message);
        var logoImg = GetLogoImg(message, imagePath);
        var logoText = GetLogoText(message);
        var mailSettings = GetMailSettings(message);
        var unsubscribeText = GetUnsubscribeText(message, mailSettings);


        InitFooter(message, mailSettings, out var footerContent, out var footerSocialContent);

        message.Body = template.Replace("%CONTENT%", output.GetFormattedText())
                               .Replace("%LOGO%", logoImg)
                               .Replace("%LOGOTEXT%", logoText)
                               .Replace("%SITEURL%", mailSettings == null ? _mailWhiteLabelSettingsHelper.DefaultMailSiteUrl : mailSettings.SiteUrl)
                               .Replace("%FOOTER%", footerContent)
                               .Replace("%FOOTERSOCIAL%", footerSocialContent)
                               .Replace("%TEXTFOOTER%", unsubscribeText)
                               .Replace("%IMAGEPATH%", imagePath);
    }

    private static string GetTemplate(NoticeMessage message)
    {
        var template = NotifyTemplateResource.HtmlMaster;

        var templateTag = message.GetArgument("MasterTemplate");
        if (templateTag != null)
        {
            var templateTagValue = templateTag.Value as string;
            if (!string.IsNullOrEmpty(templateTagValue))
            {
                var templateValue = NotifyTemplateResource.ResourceManager.GetString(templateTagValue);
                if (!string.IsNullOrEmpty(templateValue))
                {
                    template = templateValue;
                }
            }
        }

        return template;
    }

    private static string GetImagePath(NoticeMessage message)
    {
        var imagePathTag = message.GetArgument("ImagePath");

        return imagePathTag == null ? string.Empty : (string)imagePathTag.Value;
    }

    private string GetLogoImg(NoticeMessage message, string imagePath)
    {
        string logoImg;

        if (_coreBaseSettings.Personal && !_coreBaseSettings.CustomMode)
        {
            logoImg = imagePath + "/mail_logo.png";
        }
        else
        {
            logoImg = _configuration["web:logo:mail"];
            if (string.IsNullOrEmpty(logoImg))
            {
                var logo = message.GetArgument("LetterLogo");
                if (logo != null && ((string)logo.Value).Length != 0)
                {
                    logoImg = (string)logo.Value;
                }
                else
                {
                    logoImg = imagePath + "/mail_logo.png";
                }
            }
        }

        return logoImg;
    }

    private string GetLogoText(NoticeMessage message)
    {
        var logoText = _configuration["web:logotext:mail"];

        if (string.IsNullOrEmpty(logoText))
        {
            var llt = message.GetArgument("LetterLogoText");
            if (llt != null && ((string)llt.Value).Length != 0)
            {
                logoText = (string)llt.Value;
            }
            else
            {
                logoText = BaseWhiteLabelSettings.DefaultLogoText;
            }
        }

        return logoText;
    }

    private static MailWhiteLabelSettings GetMailSettings(NoticeMessage message)
    {
        var mailWhiteLabelTag = message.GetArgument("MailWhiteLabelSettings");

        return mailWhiteLabelTag == null ? null : mailWhiteLabelTag.Value as MailWhiteLabelSettings;
    }

    private void InitFooter(NoticeMessage message, MailWhiteLabelSettings settings, out string footerContent, out string footerSocialContent)
    {
        footerContent = string.Empty;
        footerSocialContent = string.Empty;

        var footer = message.GetArgument("Footer");

        if (footer == null)
        {
            return;
        }

        var footerValue = (string)footer.Value;

        if (string.IsNullOrEmpty(footerValue))
        {
            return;
        }

        switch (footerValue)
        {
            case "common":
                InitCommonFooter(settings, out footerContent, out footerSocialContent);
                break;
            case "social":
                InitSocialFooter(settings, out footerSocialContent);
                break;
            case "personal":
                footerSocialContent = NotifyTemplateResource.SocialNetworksFooterV10;
                break;
            case "personalCustomMode":
                break;
            case "opensource":
                footerContent = NotifyTemplateResource.FooterOpensourceV10;
                footerSocialContent = NotifyTemplateResource.SocialNetworksFooterV10;
                break;
        }
    }

    private void InitCommonFooter(MailWhiteLabelSettings settings, out string footerContent, out string footerSocialContent)
    {
        footerContent = string.Empty;
        footerSocialContent = string.Empty;

        if (settings == null)
        {
            footerContent =
                NotifyTemplateResource.FooterCommonV10
                                      .Replace("%SUPPORTURL%", _mailWhiteLabelSettingsHelper.DefaultMailSupportUrl)
                                      .Replace("%SALESEMAIL%", _mailWhiteLabelSettingsHelper.DefaultMailSalesEmail)
                                      .Replace("%DEMOURL%", _mailWhiteLabelSettingsHelper.DefaultMailDemoUrl);
            footerSocialContent = NotifyTemplateResource.SocialNetworksFooterV10;

        }
        else if (settings.FooterEnabled)
        {
            footerContent =
                NotifyTemplateResource.FooterCommonV10
                .Replace("%SUPPORTURL%", string.IsNullOrEmpty(settings.SupportUrl) ? "mailto:" + settings.SalesEmail : settings.SupportUrl)
                .Replace("%SALESEMAIL%", settings.SalesEmail)
                .Replace("%DEMOURL%", string.IsNullOrEmpty(settings.DemoUrl) ? "mailto:" + settings.SalesEmail : settings.DemoUrl);

            footerSocialContent = settings.FooterSocialEnabled ? NotifyTemplateResource.SocialNetworksFooterV10 : string.Empty;
        }
    }

    private static void InitSocialFooter(MailWhiteLabelSettings settings, out string footerSocialContent)
    {
        footerSocialContent = string.Empty;

        if (settings == null || (settings.FooterEnabled && settings.FooterSocialEnabled))
        {
            footerSocialContent = NotifyTemplateResource.SocialNetworksFooterV10;
        }
    }

    private string GetUnsubscribeText(NoticeMessage message, MailWhiteLabelSettings settings)
    {
        var withoutUnsubscribe = message.GetArgument("WithoutUnsubscribe");

        if (withoutUnsubscribe != null && (bool)withoutUnsubscribe.Value)
        {
            return string.Empty;
        }

        var rootPathArgument = message.GetArgument("__VirtualRootPath");
        var rootPath = rootPathArgument == null ? string.Empty : (string)rootPathArgument.Value;

        if (string.IsNullOrEmpty(rootPath))
        {
            return string.Empty;
        }

        var unsubscribeLink = _coreBaseSettings.CustomMode && _coreBaseSettings.Personal
                                  ? GetSiteUnsubscribeLink(message, settings)
                                  : GetPortalUnsubscribeLink(message, settings);

        if (string.IsNullOrEmpty(unsubscribeLink))
        {
            return string.Empty;
        }

        return string.Format(NotifyTemplateResource.TextForFooterWithUnsubscribeLink, rootPath, unsubscribeLink);
    }

    private string GetPortalUnsubscribeLink(NoticeMessage message, MailWhiteLabelSettings settings)
    {
        var unsubscribeLinkArgument = message.GetArgument("ProfileUrl");

        if (unsubscribeLinkArgument != null)
        {
            var unsubscribeLink = (string)unsubscribeLinkArgument.Value;

            if (!string.IsNullOrEmpty(unsubscribeLink))
            {
                return unsubscribeLink + "?unsubscribe=tips";
            }
        }

        return GetSiteUnsubscribeLink(message, settings);
    }

    private string GetSiteUnsubscribeLink(NoticeMessage message, MailWhiteLabelSettings settings)
    {
        var mail = message.Recipient.Addresses.FirstOrDefault(r => r.Contains('@'));

        if (string.IsNullOrEmpty(mail))
        {
            return string.Empty;
        }

        var format = _coreBaseSettings.CustomMode
                         ? "{0}/unsubscribe/{1}"
                         : "{0}/Unsubscribe.aspx?id={1}";

        var site = settings == null
                       ? _mailWhiteLabelSettingsHelper.DefaultMailSiteUrl
                       : settings.SiteUrl;

        return string.Format(format, site,
            WebEncoders.Base64UrlEncode(_instanceCrypto.Encrypt(
                Encoding.UTF8.GetBytes(mail.ToLowerInvariant()))));
    }
}
