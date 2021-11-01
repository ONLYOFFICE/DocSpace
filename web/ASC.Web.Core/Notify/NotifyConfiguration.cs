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


using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Notify.Engine;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Notify;
using ASC.Notify.Engine;
using ASC.Notify.Messages;
using ASC.Notify.Patterns;
using ASC.Notify.Textile;
using ASC.Web.Core;
using ASC.Web.Core.Users;
using ASC.Web.Core.WhiteLabel;
using ASC.Web.Studio.Utility;

using Google.Protobuf;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using MimeKit.Utils;

namespace ASC.Web.Studio.Core.Notify
{
    [Singletone(Additional = typeof(WorkContextExtension))]
    public class NotifyConfiguration
    {
        private static bool configured;
        private static readonly object locker = new object();
        private static readonly Regex urlReplacer = new Regex(@"(<a [^>]*href=(('(?<url>[^>']*)')|(""(?<url>[^>""]*)""))[^>]*>)|(<img [^>]*src=(('(?<url>(?![data:|cid:])[^>']*)')|(""(?<url>(?![data:|cid:])[^>""]*)""))[^/>]*/?>)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex textileLinkReplacer = new Regex(@"""(?<text>[\w\W]+?)"":""(?<link>[^""]+)""", RegexOptions.Singleline | RegexOptions.Compiled);

        private IServiceProvider ServiceProvider { get; }

        public NotifyConfiguration(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public void Configure()
        {
            lock (locker)
            {
                if (!configured)
                {
                    configured = true;
                    WorkContext.NotifyStartUp(ServiceProvider);
                    WorkContext.NotifyContext.NotifyClientRegistration += NotifyClientRegisterCallback;
                    WorkContext.NotifyContext.NotifyEngine.BeforeTransferRequest += BeforeTransferRequest;
                }
            }
        }

        private static void NotifyClientRegisterCallback(Context context, INotifyClient client)
        {
            #region url correction

            var absoluteUrl = new SendInterceptorSkeleton(
                "Web.UrlAbsoluter",
                InterceptorPlace.MessageSend,
                InterceptorLifetime.Global,
                (r, p, scope) =>
                {
                    if (r != null && r.CurrentMessage != null && r.CurrentMessage.ContentType == Pattern.HTMLContentType)
                    {
                        var commonLinkUtility = scope.ServiceProvider.GetService<CommonLinkUtility>();

                        var body = r.CurrentMessage.Body;

                        body = urlReplacer.Replace(body, m =>
                        {
                            var url = m.Groups["url"].Value;
                            var ind = m.Groups["url"].Index - m.Index;
                            return string.IsNullOrEmpty(url) && ind > 0 ?
                                m.Value.Insert(ind, commonLinkUtility.GetFullAbsolutePath(string.Empty)) :
                                m.Value.Replace(url, commonLinkUtility.GetFullAbsolutePath(url));
                        });

                        body = textileLinkReplacer.Replace(body, m =>
                        {
                            var url = m.Groups["link"].Value;
                            var ind = m.Groups["link"].Index - m.Index;
                            return string.IsNullOrEmpty(url) && ind > 0 ?
                                m.Value.Insert(ind, commonLinkUtility.GetFullAbsolutePath(string.Empty)) :
                                m.Value.Replace(url, commonLinkUtility.GetFullAbsolutePath(url));
                        });

                        r.CurrentMessage.Body = body;
                    }
                    return false;
                });
            client.AddInterceptor(absoluteUrl);

            #endregion

            #region security and culture

            var securityAndCulture = new SendInterceptorSkeleton(
                "ProductSecurityInterceptor",
                 InterceptorPlace.DirectSend,
                 InterceptorLifetime.Global,
                 (r, p, scope) =>
                 {
                     var scopeClass = scope.ServiceProvider.GetService<NotifyConfigurationScope>();
                     var coreBaseSettings = scope.ServiceProvider.GetService<CoreBaseSettings>();
                     var (tenantManager, webItemSecurity, userManager, options, _, _, _, _, _, _, _, _, _, _, _) = scopeClass;
                     try
                     {
                         // culture
                         var u = Constants.LostUser;
                         if (!(coreBaseSettings.Personal && r.NotifyAction.ID == Actions.PersonalConfirmation.ID))
                         {
                             var tenant = tenantManager.GetCurrentTenant();

                             if (32 <= r.Recipient.ID.Length)
                             {
                                 var guid = default(Guid);
                                 try
                                 {
                                     guid = new Guid(r.Recipient.ID);
                                 }
                                 catch (FormatException) { }
                                 catch (OverflowException) { }

                                 if (guid != default)
                                 {
                                     u = userManager.GetUsers(guid);
                                 }
                             }

                             if (Constants.LostUser.Equals(u))
                             {
                                 u = userManager.GetUserByEmail(r.Recipient.ID);
                             }

                             if (Constants.LostUser.Equals(u))
                             {
                                 u = userManager.GetUserByUserName(r.Recipient.ID);
                             }

                             if (!Constants.LostUser.Equals(u))
                             {
                                 var culture = !string.IsNullOrEmpty(u.CultureName) ? u.GetCulture() : tenant.GetCulture();
                                 Thread.CurrentThread.CurrentCulture = culture;
                                 Thread.CurrentThread.CurrentUICulture = culture;

                                 // security
                                 var tag = r.Arguments.Find(a => a.Tag == CommonTags.ModuleID);
                                 var productId = tag != null ? (Guid)tag.Value : Guid.Empty;
                                 if (productId == Guid.Empty)
                                 {
                                     tag = r.Arguments.Find(a => a.Tag == CommonTags.ProductID);
                                     productId = tag != null ? (Guid)tag.Value : Guid.Empty;
                                 }
                                 if (productId == Guid.Empty)
                                 {
                                     productId = (Guid)(CallContext.GetData("asc.web.product_id") ?? Guid.Empty);
                                 }
                                 if (productId != Guid.Empty && productId != new Guid("f4d98afdd336433287783c6945c81ea0") /* ignore people product */)
                                 {
                                     return !webItemSecurity.IsAvailableForUser(productId, u.ID);
                                 }
                             }
                         }

                         var tagCulture = r.Arguments.FirstOrDefault(a => a.Tag == CommonTags.Culture);
                         if (tagCulture != null)
                         {
                             var culture = CultureInfo.GetCultureInfo((string)tagCulture.Value);
                             Thread.CurrentThread.CurrentCulture = culture;
                             Thread.CurrentThread.CurrentUICulture = culture;
                         }
                     }
                     catch (Exception error)
                     {
                         options.CurrentValue.Error(error);
                     }
                     return false;
                 });
            client.AddInterceptor(securityAndCulture);

            #endregion

            #region white label correction

            var whiteLabel = new SendInterceptorSkeleton(
                "WhiteLabelInterceptor",
                 InterceptorPlace.MessageSend,
                 InterceptorLifetime.Global,
                 (r, p, scope) =>
                 {
                     try
                     {
                         var tags = r.Arguments;

                         var logoTextTag = tags.FirstOrDefault(a => a.Tag == CommonTags.LetterLogoText);
                         var logoText = logoTextTag != null ? (string)logoTextTag.Value : string.Empty;

                         if (!string.IsNullOrEmpty(logoText))
                         {
                             r.CurrentMessage.Body = r.CurrentMessage.Body
                                 .Replace(string.Format("${{{0}}}", CommonTags.LetterLogoText), logoText);
                         }
                     }
                     catch (Exception error)
                     {
                         scope.ServiceProvider.GetService<IOptionsMonitor<ILog>>().CurrentValue.Error(error);
                     }
                     return false;
                 });
            client.AddInterceptor(whiteLabel);

            #endregion
        }

        private static void BeforeTransferRequest(NotifyEngine sender, NotifyRequest request, IServiceScope scope)
        {
            var aid = Guid.Empty;
            var aname = string.Empty;
            var tenant = scope.ServiceProvider.GetService<TenantManager>().GetCurrentTenant();
            var authContext = scope.ServiceProvider.GetService<AuthContext>();
            var userManager = scope.ServiceProvider.GetService<UserManager>();
            var displayUserSettingsHelper = scope.ServiceProvider.GetService<DisplayUserSettingsHelper>();

            if (authContext.IsAuthenticated)
            {
                aid = authContext.CurrentAccount.ID;
                var user = userManager.GetUsers(aid);
                if (userManager.UserExists(user))
                {
                    aname = user.DisplayUserName(false, displayUserSettingsHelper)
                        .Replace(">", "&#62")
                        .Replace("<", "&#60");
                }
            }
            var scopeClass = scope.ServiceProvider.GetService<NotifyConfigurationScope>();
            var (_, _, _, options, tenantExtra, _, webItemManager, configuration, tenantLogoManager, additionalWhiteLabelSettingsHelper, tenantUtil, coreBaseSettings, commonLinkUtility, settingsManager, studioNotifyHelper) = scopeClass;
            var log = options.CurrentValue;

            commonLinkUtility.GetLocationByRequest(out var product, out var module);
            if (product == null && CallContext.GetData("asc.web.product_id") != null)
            {
                product = webItemManager[(Guid)CallContext.GetData("asc.web.product_id")] as IProduct;
            }

            var logoText = TenantWhiteLabelSettings.DefaultLogoText;
            if ((tenantExtra.Enterprise || coreBaseSettings.CustomMode) && !MailWhiteLabelSettings.IsDefault(settingsManager, configuration))
            {
                logoText = tenantLogoManager.GetLogoText();
            }

            request.Arguments.Add(new TagValue(CommonTags.AuthorID, aid));
            request.Arguments.Add(new TagValue(CommonTags.AuthorName, aname));
            request.Arguments.Add(new TagValue(CommonTags.AuthorUrl, commonLinkUtility.GetFullAbsolutePath(commonLinkUtility.GetUserProfile(aid))));
            request.Arguments.Add(new TagValue(CommonTags.VirtualRootPath, commonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/')));
            request.Arguments.Add(new TagValue(CommonTags.ProductID, product != null ? product.ID : Guid.Empty));
            request.Arguments.Add(new TagValue(CommonTags.ModuleID, module != null ? module.ID : Guid.Empty));
            request.Arguments.Add(new TagValue(CommonTags.ProductUrl, commonLinkUtility.GetFullAbsolutePath(product != null ? product.StartURL : "~")));
            request.Arguments.Add(new TagValue(CommonTags.DateTime, tenantUtil.DateTimeNow()));
            request.Arguments.Add(new TagValue(CommonTags.RecipientID, Context.SYS_RECIPIENT_ID));
            request.Arguments.Add(new TagValue(CommonTags.ProfileUrl, commonLinkUtility.GetFullAbsolutePath(commonLinkUtility.GetMyStaff())));
            request.Arguments.Add(new TagValue(CommonTags.RecipientSubscriptionConfigURL, commonLinkUtility.GetMyStaff()));
            request.Arguments.Add(new TagValue(CommonTags.HelpLink, commonLinkUtility.GetHelpLink(settingsManager, additionalWhiteLabelSettingsHelper, false)));
            request.Arguments.Add(new TagValue(CommonTags.LetterLogoText, logoText));
            request.Arguments.Add(new TagValue(CommonTags.MailWhiteLabelSettings, MailWhiteLabelSettings.Instance(settingsManager)));
            request.Arguments.Add(new TagValue(CommonTags.SendFrom, tenant.Name));
            request.Arguments.Add(new TagValue(CommonTags.ImagePath, studioNotifyHelper.GetNotificationImageUrl("").TrimEnd('/')));

            AddLetterLogo(request, tenantExtra, tenantLogoManager, coreBaseSettings, commonLinkUtility, log);
        }

        private static void AddLetterLogo(NotifyRequest request, TenantExtra tenantExtra, TenantLogoManager tenantLogoManager, CoreBaseSettings coreBaseSettings, CommonLinkUtility commonLinkUtility, ILog Log)
        {
            if (tenantExtra.Enterprise || coreBaseSettings.CustomMode)
            {
                try
                {
                    var logoData = tenantLogoManager.GetMailLogoDataFromCache();

                    if (logoData == null)
                    {
                        var logoStream = tenantLogoManager.GetWhitelabelMailLogo();
                        logoData = ReadStreamToByteArray(logoStream) ?? GetDefaultMailLogo();

                        if (logoData != null)
                            tenantLogoManager.InsertMailLogoDataToCache(logoData);
                    }

                    if (logoData != null)
                    {
                        var attachment = new NotifyMessageAttachment
                        {
                            FileName = "logo.png",
                            Content = ByteString.CopyFrom(logoData),
                            ContentId = MimeUtils.GenerateMessageId()
                        };

                        request.Arguments.Add(new TagValue(CommonTags.LetterLogo, "cid:" + attachment.ContentId));
                        request.Arguments.Add(new TagValue(CommonTags.EmbeddedAttachments, new[] { attachment }));
                        return;
                    }
                }
                catch (Exception error)
                {
                    Log.Error(error);
                }
            }

            var logoUrl = commonLinkUtility.GetFullAbsolutePath(tenantLogoManager.GetLogoDark(true));

            request.Arguments.Add(new TagValue(CommonTags.LetterLogo, logoUrl));
        }

        private static byte[] ReadStreamToByteArray(Stream inputStream)
        {
            if (inputStream == null) return null;

            using (inputStream)
            {
                using var memoryStream = new MemoryStream();
                inputStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static byte[] GetDefaultMailLogo()
        {
            var filePath = CrossPlatform.PathCombine(AppDomain.CurrentDomain.BaseDirectory, "skins", "default", "images", "logo", "dark_general.png");

            return File.Exists(filePath) ? File.ReadAllBytes(filePath) : null;
        }
    }

    [Scope]
    public class NotifyConfigurationScope
    {
        private TenantManager TenantManager { get; }
        private WebItemSecurity WebItemSecurity { get; }
        private UserManager UserManager { get; }
        private IOptionsMonitor<ILog> Options { get; }
        private TenantExtra TenantExtra { get; }
        private WebItemManagerSecurity WebItemManagerSecurity { get; }
        private WebItemManager WebItemManager { get; }
        private IConfiguration Configuration { get; }
        private TenantLogoManager TenantLogoManager { get; }
        private AdditionalWhiteLabelSettingsHelper AdditionalWhiteLabelSettingsHelper { get; }
        private TenantUtil TenantUtil { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private CommonLinkUtility CommonLinkUtility { get; }
        private SettingsManager SettingsManager { get; }
        private StudioNotifyHelper StudioNotifyHelper { get; }

        public NotifyConfigurationScope(TenantManager tenantManager,
            WebItemSecurity webItemSecurity,
            UserManager userManager,
            IOptionsMonitor<ILog> options,
            TenantExtra tenantExtra,
            WebItemManagerSecurity webItemManagerSecurity,
            WebItemManager webItemManager,
            IConfiguration configuration,
            TenantLogoManager tenantLogoManager,
            AdditionalWhiteLabelSettingsHelper additionalWhiteLabelSettingsHelper,
            TenantUtil tenantUtil,
            CoreBaseSettings coreBaseSettings,
            CommonLinkUtility commonLinkUtility,
            SettingsManager settingsManager,
            StudioNotifyHelper studioNotifyHelper
            )
        {
            TenantManager = tenantManager;
            WebItemSecurity = webItemSecurity;
            UserManager = userManager;
            Options = options;
            TenantExtra = tenantExtra;
            WebItemManagerSecurity = webItemManagerSecurity;
            WebItemManager = webItemManager;
            Configuration = configuration;
            TenantLogoManager = tenantLogoManager;
            AdditionalWhiteLabelSettingsHelper = additionalWhiteLabelSettingsHelper;
            TenantUtil = tenantUtil;
            CoreBaseSettings = coreBaseSettings;
            CommonLinkUtility = commonLinkUtility;
            SettingsManager = settingsManager;
            StudioNotifyHelper = studioNotifyHelper;
        }

        public void Deconstruct(out TenantManager tenantManager,
            out WebItemSecurity webItemSecurity,
            out UserManager userManager,
            out IOptionsMonitor<ILog> optionsMonitor,
            out TenantExtra tenantExtra,
            out WebItemManagerSecurity webItemManagerSecurity,
            out WebItemManager webItemManager,
            out IConfiguration configuration,
            out TenantLogoManager tenantLogoManager,
            out AdditionalWhiteLabelSettingsHelper additionalWhiteLabelSettingsHelper,
            out TenantUtil tenantUtil,
            out CoreBaseSettings coreBaseSettings,
            out CommonLinkUtility commonLinkUtility,
            out SettingsManager settingsManager,
            out StudioNotifyHelper studioNotifyHelper)
        {
            tenantManager = TenantManager;
            webItemSecurity = WebItemSecurity;
            userManager = UserManager;
            optionsMonitor = Options;
            tenantExtra = TenantExtra;
            webItemManagerSecurity = WebItemManagerSecurity;
            webItemManager = WebItemManager;
            configuration = Configuration;
            tenantLogoManager = TenantLogoManager;
            additionalWhiteLabelSettingsHelper = AdditionalWhiteLabelSettingsHelper;
            tenantUtil = TenantUtil;
            coreBaseSettings = CoreBaseSettings;
            commonLinkUtility = CommonLinkUtility;
            settingsManager = SettingsManager;
            studioNotifyHelper = StudioNotifyHelper;
        }
    }

    public class NotifyConfigurationExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<NotifyConfigurationScope>();
            services.TryAdd<TextileStyler>();
            services.TryAdd<JabberStyler>();
            services.TryAdd<PushStyler>();
        }
    }
}
