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

using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Studio.Core.Notify;

[Singletone(Additional = typeof(WorkContextExtension))]
public class NotifyConfiguration
{
    private static bool _configured;
    private static readonly object _locker = new object();
    private static readonly Regex _urlReplacer = new Regex(@"(<a [^>]*href=(('(?<url>[^>']*)')|(""(?<url>[^>""]*)""))[^>]*>)|(<img [^>]*src=(('(?<url>(?![data:|cid:])[^>']*)')|(""(?<url>(?![data:|cid:])[^>""]*)""))[^/>]*/?>)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex _textileLinkReplacer = new Regex(@"""(?<text>[\w\W]+?)"":""(?<link>[^""]+)""", RegexOptions.Singleline | RegexOptions.Compiled);
    private readonly NotifyEngine _notifyEngine;
    private readonly WorkContext _workContext;

    public NotifyConfiguration(NotifyEngine notifyEngine, WorkContext workContext)
    {
        _notifyEngine = notifyEngine;
        _workContext = workContext;
    }

    public void Configure()
    {
        lock (_locker)
        {
            if (!_configured)
            {
                _configured = true;
                _workContext.NotifyStartUp();
                _workContext.NotifyContext.NotifyClientRegistration += NotifyClientRegisterCallback;
                _notifyEngine.AddAction<NotifyTransferRequest>();
            }
        }
    }

    private void NotifyClientRegisterCallback(Context context, INotifyClient client)
    {
        #region url correction

        var absoluteUrl = new SendInterceptorSkeleton(
            "Web.UrlAbsoluter",
            InterceptorPlace.MessageSend,
            InterceptorLifetime.Global,
            (r, p, scope) =>
            {
                if (r != null && r.CurrentMessage != null && r.CurrentMessage.ContentType == Pattern.HtmlContentType)
                {
                    var commonLinkUtility = scope.ServiceProvider.GetService<CommonLinkUtility>();

                    var body = r.CurrentMessage.Body;

                    body = _urlReplacer.Replace(body, m =>
                    {
                        var url = m.Groups["url"].Value;
                        var ind = m.Groups["url"].Index - m.Index;
                        return string.IsNullOrEmpty(url) && ind > 0 ?
                            m.Value.Insert(ind, commonLinkUtility.GetFullAbsolutePath(string.Empty)) :
                            m.Value.Replace(url, commonLinkUtility.GetFullAbsolutePath(url));
                    });

                    body = _textileLinkReplacer.Replace(body, m =>
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
                 var scopeClass = scope.ServiceProvider.GetRequiredService<ProductSecurityInterceptor>();
                 return scopeClass.Intercept(r, p);
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
                             .Replace("${{" + CommonTags.LetterLogoText + "}}", logoText);
                     }
                 }
                 catch (Exception error)
                 {
                     scope.ServiceProvider.GetService<ILogger<ProductSecurityInterceptor>>().ErrorNotifyClientRegisterCallback(error);
                 }
                 return false;
             });
        client.AddInterceptor(whiteLabel);

        #endregion
    }
}

[Scope]
public class ProductSecurityInterceptor
{
    private readonly TenantManager _tenantManager;
    private readonly WebItemSecurity _webItemSecurity;
    private readonly UserManager _userManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly ILogger<ProductSecurityInterceptor> _log;

    public ProductSecurityInterceptor(
        TenantManager tenantManager,
        WebItemSecurity webItemSecurity,
        UserManager userManager,
        CoreBaseSettings coreBaseSettings,
        ILogger<ProductSecurityInterceptor> logger
        )
    {
        _tenantManager = tenantManager;
        _webItemSecurity = webItemSecurity;
        _userManager = userManager;
        _coreBaseSettings = coreBaseSettings;
        _log = logger;
    }

    public bool Intercept(NotifyRequest r, InterceptorPlace p)
    {
        try
        {
            // culture
            var u = Constants.LostUser;
            if (!(_coreBaseSettings.Personal && r.NotifyAction.ID == Actions.PersonalConfirmation.ID))
            {
                var tenant = _tenantManager.GetCurrentTenant();

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
                        u = _userManager.GetUsers(guid);
                    }
                }

                if (Constants.LostUser.Equals(u))
                {
                    u = _userManager.GetUserByEmail(r.Recipient.ID);
                }

                if (Constants.LostUser.Equals(u))
                {
                    u = _userManager.GetUserByUserName(r.Recipient.ID);
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
                        return !_webItemSecurity.IsAvailableForUser(productId, u.Id);
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
            _log.ErrorProductSecurityInterceptor(error);
        }

        return false;
    }
}

public static class NotifyConfigurationExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<NotifyTransferRequest>();
        services.TryAdd<ProductSecurityInterceptor>();
        services.TryAdd<TextileStyler>();
        services.TryAdd<JabberStyler>();
        services.TryAdd<PushStyler>();
    }
}

[Scope]
public class NotifyTransferRequest : INotifyEngineAction
{
    private readonly TenantManager _tenantManager;
    private readonly AuthContext _authContext;
    private readonly UserManager _userManager;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly TenantExtra _tenantExtra;
    private readonly WebItemManager _webItemManager;
    private readonly TenantLogoManager _tenantLogoManager;
    private readonly AdditionalWhiteLabelSettingsHelper _additionalWhiteLabelSettingsHelper;
    private readonly TenantUtil _tenantUtil;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly SettingsManager _settingsManager;
    private readonly StudioNotifyHelper _studioNotifyHelper;
    private readonly ILogger<ProductSecurityInterceptor> _log;

    public NotifyTransferRequest(
        TenantManager tenantManager,
        AuthContext authContext,
        UserManager userManager,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        ILogger<ProductSecurityInterceptor> logger,
        TenantExtra tenantExtra,
        WebItemManager webItemManager,
        TenantLogoManager tenantLogoManager,
        AdditionalWhiteLabelSettingsHelper additionalWhiteLabelSettingsHelper,
        TenantUtil tenantUtil,
        CoreBaseSettings coreBaseSettings,
        CommonLinkUtility commonLinkUtility,
        SettingsManager settingsManager,
        StudioNotifyHelper studioNotifyHelper)
    {
        _tenantManager = tenantManager;
        _authContext = authContext;
        _userManager = userManager;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _tenantExtra = tenantExtra;
        _webItemManager = webItemManager;
        _tenantLogoManager = tenantLogoManager;
        _additionalWhiteLabelSettingsHelper = additionalWhiteLabelSettingsHelper;
        _tenantUtil = tenantUtil;
        _coreBaseSettings = coreBaseSettings;
        _commonLinkUtility = commonLinkUtility;
        _settingsManager = settingsManager;
        _studioNotifyHelper = studioNotifyHelper;
        _log = logger;
    }

    public void BeforeTransferRequest(NotifyRequest request)
    {
        var aid = Guid.Empty;
        var aname = string.Empty;
        var tenant = _tenantManager.GetCurrentTenant();

        if (_authContext.IsAuthenticated)
        {
            aid = _authContext.CurrentAccount.ID;
            var user = _userManager.GetUsers(aid);
            if (_userManager.UserExists(user))
            {
                aname = user.DisplayUserName(false, _displayUserSettingsHelper)
                .Replace(">", "&#62")
                .Replace("<", "&#60");
            }
        }

        _commonLinkUtility.GetLocationByRequest(out var product, out var module);
        if (product == null && CallContext.GetData("asc.web.product_id") != null)
        {
            product = _webItemManager[(Guid)CallContext.GetData("asc.web.product_id")] as IProduct;
        }

        var logoText = TenantWhiteLabelSettings.DefaultLogoText;
        if ((_tenantExtra.Enterprise || _coreBaseSettings.CustomMode) && !MailWhiteLabelSettings.IsDefault(_settingsManager))
        {
            logoText = _tenantLogoManager.GetLogoText();
        }

        request.Arguments.Add(new TagValue(CommonTags.AuthorID, aid));
        request.Arguments.Add(new TagValue(CommonTags.AuthorName, aname));
        request.Arguments.Add(new TagValue(CommonTags.AuthorUrl, _commonLinkUtility.GetFullAbsolutePath(_commonLinkUtility.GetUserProfile(aid))));
        request.Arguments.Add(new TagValue(CommonTags.VirtualRootPath, _commonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/')));
        request.Arguments.Add(new TagValue(CommonTags.ProductID, product != null ? product.ID : Guid.Empty));
        request.Arguments.Add(new TagValue(CommonTags.ModuleID, module != null ? module.ID : Guid.Empty));
        request.Arguments.Add(new TagValue(CommonTags.ProductUrl, _commonLinkUtility.GetFullAbsolutePath(product != null ? product.StartURL : "~")));
        request.Arguments.Add(new TagValue(CommonTags.DateTime, _tenantUtil.DateTimeNow()));
        request.Arguments.Add(new TagValue(CommonTags.RecipientID, Context.SysRecipient));
        request.Arguments.Add(new TagValue(CommonTags.ProfileUrl, _commonLinkUtility.GetFullAbsolutePath(_commonLinkUtility.GetMyStaff())));
        request.Arguments.Add(new TagValue(CommonTags.RecipientSubscriptionConfigURL, _commonLinkUtility.GetUnsubscribe()));
        request.Arguments.Add(new TagValue(CommonTags.HelpLink, _commonLinkUtility.GetHelpLink(_settingsManager, _additionalWhiteLabelSettingsHelper, false)));
        request.Arguments.Add(new TagValue(CommonTags.LetterLogoText, logoText));
        request.Arguments.Add(new TagValue(CommonTags.MailWhiteLabelSettings, MailWhiteLabelSettings.Instance(_settingsManager)));
        request.Arguments.Add(new TagValue(CommonTags.SendFrom, tenant.Name == "" ? Resource.PortalName : tenant.Name));
        request.Arguments.Add(new TagValue(CommonTags.ImagePath, _studioNotifyHelper.GetNotificationImageUrl("").TrimEnd('/')));

        AddLetterLogo(request);
    }
    public void AfterTransferRequest(NotifyRequest request)
    {
    }

    private void AddLetterLogo(NotifyRequest request)
    {
        if (_tenantExtra.Enterprise || _coreBaseSettings.CustomMode)
        {
            try
            {
                var logoData = _tenantLogoManager.GetMailLogoDataFromCache();

                if (logoData == null)
                {
                    var logoStream = _tenantLogoManager.GetWhitelabelMailLogo();
                    logoData = ReadStreamToByteArray(logoStream) ?? GetDefaultMailLogo();

                    if (logoData != null)
                    {
                        _tenantLogoManager.InsertMailLogoDataToCache(logoData);
                    }
                }

                if (logoData != null)
                {
                    var attachment = new NotifyMessageAttachment
                    {
                        FileName = "logo.png",
                        Content = logoData,
                        ContentId = MimeUtils.GenerateMessageId()
                    };

                    request.Arguments.Add(new TagValue(CommonTags.LetterLogo, "cid:" + attachment.ContentId));
                    request.Arguments.Add(new TagValue(CommonTags.EmbeddedAttachments, new[] { attachment }));
                    return;
                }
            }
            catch (Exception error)
            {
                _log.ErrorAddLetterLogo(error);
            }
        }

        var logoUrl = _commonLinkUtility.GetFullAbsolutePath(_tenantLogoManager.GetLogoDark(true));

        request.Arguments.Add(new TagValue(CommonTags.LetterLogo, logoUrl));
    }

    private static byte[] ReadStreamToByteArray(Stream inputStream)
    {
        if (inputStream == null)
        {
            return null;
        }

        using (inputStream)
        {
            using var memoryStream = new MemoryStream();
            inputStream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }

    private static byte[] GetDefaultMailLogo()
    {
        var filePath = CrossPlatform.PathCombine(AppDomain.CurrentDomain.BaseDirectory, "skins", "default", "images", "logo", "dark_general.png");

        return File.Exists(filePath) ? File.ReadAllBytes(filePath) : null;
    }
}
