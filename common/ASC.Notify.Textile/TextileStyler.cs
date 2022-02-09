/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using ASC.Common;
using ASC.Common.Notify.Patterns;
using ASC.Core;
using ASC.Core.Common.WhiteLabel;
using ASC.Notify.Messages;
using ASC.Notify.Patterns;
using ASC.Notify.Textile.Resources;
using ASC.Security.Cryptography;
using ASC.Web.Core.WhiteLabel;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

using Textile;
using Textile.Blocks;

namespace ASC.Notify.Textile
{
    [Scope]
    public class TextileStyler : IPatternStyler
    {
        private static readonly Regex VelocityArguments = new Regex(NVelocityPatternFormatter.NoStylePreffix + "(?<arg>.*?)" + NVelocityPatternFormatter.NoStyleSuffix, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        private CoreBaseSettings CoreBaseSettings { get; }
        private IConfiguration Configuration { get; }
        private InstanceCrypto InstanceCrypto { get; }
        private MailWhiteLabelSettingsHelper MailWhiteLabelSettingsHelper { get; }

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
            CoreBaseSettings = coreBaseSettings;
            Configuration = configuration;
            InstanceCrypto = instanceCrypto;
            MailWhiteLabelSettingsHelper = mailWhiteLabelSettingsHelper;
        }

        public void ApplyFormating(NoticeMessage message)
        {
            var output = new StringBuilderTextileFormatter();
            var formatter = new TextileFormatter(output);

            if (!string.IsNullOrEmpty(message.Subject))
            {
                message.Subject = VelocityArguments.Replace(message.Subject, m => m.Result("${arg}"));
            }

            if (string.IsNullOrEmpty(message.Body)) return;

            formatter.Format(message.Body);

            var template = GetTemplate(message);
            var imagePath = GetImagePath(message);
            var logoImg = GetLogoImg(message, imagePath);
            var logoText = GetLogoText(message);
            var mailSettings = GetMailSettings(message);
            var unsubscribeText = GetUnsubscribeText(message, mailSettings);


            InitFooter(message, mailSettings, out var footerContent, out var footerSocialContent);

            message.Body = template
                                   .Replace("%CONTENT%", output.GetFormattedText())
                                   .Replace("%LOGO%", logoImg)
                                   .Replace("%LOGOTEXT%", logoText)
                                   .Replace("%SITEURL%", mailSettings == null ? MailWhiteLabelSettingsHelper.DefaultMailSiteUrl : mailSettings.SiteUrl)
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
                        template = templateValue;
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

            if (CoreBaseSettings.Personal && !CoreBaseSettings.CustomMode)
            {
                logoImg = imagePath + "/mail_logo.png";
            }
            else
            {
                logoImg = Configuration["web:logo:mail"];
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
            var logoText = Configuration["web:logotext:mail"];

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

            if (footer == null) return;

            var footerValue = (string)footer.Value;

            if (string.IsNullOrEmpty(footerValue)) return;

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
                                          .Replace("%SUPPORTURL%", MailWhiteLabelSettingsHelper.DefaultMailSupportUrl)
                                          .Replace("%SALESEMAIL%", MailWhiteLabelSettingsHelper.DefaultMailSalesEmail)
                                          .Replace("%DEMOURL%", MailWhiteLabelSettingsHelper.DefaultMailDemoUrl);
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
                footerSocialContent = NotifyTemplateResource.SocialNetworksFooterV10;
        }

        private string GetUnsubscribeText(NoticeMessage message, MailWhiteLabelSettings settings)
        {
            var withoutUnsubscribe = message.GetArgument("WithoutUnsubscribe");

            if (withoutUnsubscribe != null && (bool)withoutUnsubscribe.Value)
                return string.Empty;

            var rootPathArgument = message.GetArgument("__VirtualRootPath");
            var rootPath = rootPathArgument == null ? string.Empty : (string)rootPathArgument.Value;

            if (string.IsNullOrEmpty(rootPath))
                return string.Empty;

            var unsubscribeLink = CoreBaseSettings.CustomMode && CoreBaseSettings.Personal
                                      ? GetSiteUnsubscribeLink(message, settings)
                                      : GetPortalUnsubscribeLink(message, settings);

            if (string.IsNullOrEmpty(unsubscribeLink))
                return string.Empty;

            return string.Format(NotifyTemplateResource.TextForFooterWithUnsubscribeLink, rootPath, unsubscribeLink);
        }

        private string GetPortalUnsubscribeLink(NoticeMessage message, MailWhiteLabelSettings settings)
        {
            var unsubscribeLinkArgument = message.GetArgument("ProfileUrl");

            if (unsubscribeLinkArgument != null)
            {
                var unsubscribeLink = (string)unsubscribeLinkArgument.Value;

                if (!string.IsNullOrEmpty(unsubscribeLink))
                    return unsubscribeLink;
            }

            return GetSiteUnsubscribeLink(message, settings);
        }

        private string GetSiteUnsubscribeLink(NoticeMessage message, MailWhiteLabelSettings settings)
        {
            var mail = message.Recipient.Addresses.FirstOrDefault(r => r.Contains('@'));

            if (string.IsNullOrEmpty(mail))
                return string.Empty;

            var format = CoreBaseSettings.CustomMode
                             ? "{0}/unsubscribe/{1}"
                             : "{0}/Unsubscribe.aspx?id={1}";

            var site = settings == null
                           ? MailWhiteLabelSettingsHelper.DefaultMailSiteUrl
                           : settings.SiteUrl;

            return string.Format(format,
                                 site,
                                 WebEncoders.Base64UrlEncode(
                                     InstanceCrypto.Encrypt(
                                         Encoding.UTF8.GetBytes(mail.ToLowerInvariant()))));
        }
    }
}