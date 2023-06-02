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

namespace ASC.Web.Studio.Utility;

public enum ManagementType
{
    General = 0,
    Customization = 1,
    ProductsAndInstruments = 2,
    PortalSecurity = 3,
    AccessRights = 4,
    Backup = 5,
    LoginHistory = 6,
    AuditTrail = 7,
    LdapSettings = 8,
    ThirdPartyAuthorization = 9,
    SmtpSettings = 10,
    Statistic = 11,
    Monitoring = 12,
    SingleSignOnSettings = 13,
    Migration = 14,
    DeletionPortal = 15,
    HelpCenter = 16,
    DocService = 17,
    FullTextSearch = 18,
    WhiteLabel = 19,
    MailService = 20,
    Storage = 21,
    PrivacyRoom = 22
}

[Scope]
public class CommonLinkUtility : BaseCommonLinkUtility
{
    private static readonly Regex _regFilePathTrim = new Regex("/[^/]*\\.aspx", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public const string ParamName_ProductSysName = "product";
    public const string ParamName_UserUserID = "uid";
    public const string AbsoluteAccountsPath = "/accounts/";
    public const string VirtualAccountsPath = "~/accounts/";

    private readonly UserManager _userManager;
    private readonly WebItemManagerSecurity _webItemManagerSecurity;
    private readonly WebItemManager _webItemManager;
    private readonly EmailValidationKeyProvider _emailValidationKeyProvider;

    public CommonLinkUtility(
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        TenantManager tenantManager,
        UserManager userManager,
        WebItemManagerSecurity webItemManagerSecurity,
        WebItemManager webItemManager,
        EmailValidationKeyProvider emailValidationKeyProvider,
        ILoggerProvider options) :
        this(null, coreBaseSettings, coreSettings, tenantManager, userManager, webItemManagerSecurity, webItemManager, emailValidationKeyProvider, options)
    {
    }

    public CommonLinkUtility(
        IHttpContextAccessor httpContextAccessor,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        TenantManager tenantManager,
        UserManager userManager,
        WebItemManagerSecurity webItemManagerSecurity,
        WebItemManager webItemManager,
        EmailValidationKeyProvider emailValidationKeyProvider,
        ILoggerProvider options) :
        base(httpContextAccessor, coreBaseSettings, coreSettings, tenantManager, options) =>
        (_userManager, _webItemManagerSecurity, _webItemManager, _emailValidationKeyProvider) = (userManager, webItemManagerSecurity, webItemManager, emailValidationKeyProvider);

    public string Logout
    {
        get { return ToAbsolute("~/auth.aspx") + "?t=logout"; }
    }

    public string GetDefault()
    {
        return VirtualRoot;
    }

    public string GetMyStaff()
    {
        return _coreBaseSettings.Personal ? ToAbsolute("~/my") : ToAbsolute("~/accounts/view/@self");
    }

    public string GetUnsubscribe()
    {
        return _coreBaseSettings.Personal ? ToAbsolute("~/my?unsubscribe=tips") : ToAbsolute("~/products/people/view/@self");
    }

    public string GetEmployees()
    {
        return GetEmployees(EmployeeStatus.Active);
    }

    public string GetEmployees(EmployeeStatus empStatus)
    {
        return ToAbsolute("~/products/people/") +
               (empStatus == EmployeeStatus.Terminated ? "#type=disabled" : string.Empty);
    }

    public string GetDepartment(Guid depId)
    {
        return depId != Guid.Empty ? ToAbsolute("~/products/people/#group=") + depId.ToString() : GetEmployees();
    }

    #region user profile link

    public async Task<string> GetUserProfileAsync(Guid userID)
    {
        if (!await _userManager.UserExistsAsync(userID))
        {
            return GetEmployees();
        }

        return await GetUserProfileAsync(userID.ToString());
    }

    public string GetUserProfile(UserInfo user)
    {
        if (!_userManager.UserExists(user))
        {
            return GetEmployees();
        }

        return GetUserProfile(user, true);
    }

    public async Task<string> GetUserProfileAsync(string user, bool absolute = true)
    {
        var queryParams = "";

        if (!string.IsNullOrEmpty(user))
        {
            var guid = Guid.Empty;
            if (!string.IsNullOrEmpty(user) && 32 <= user.Length && user[8] == '-')
            {
                try
                {
                    guid = new Guid(user);
                }
                catch
                {
                }
            }

            queryParams = guid != Guid.Empty ? await GetUserParamsPairAsync(guid) : HttpUtility.UrlEncode(user.ToLowerInvariant());
        }

        var url = absolute ? ToAbsolute(VirtualAccountsPath) : AbsoluteAccountsPath;
        url += "view/";
        url += queryParams;

        return url;
    }

    public string GetUserProfile(UserInfo user, bool absolute = true)
    {
        var queryParams = user.Id != Guid.Empty ? GetUserParamsPair(user) : HttpUtility.UrlEncode(user.UserName.ToLowerInvariant());

        var url = absolute ? ToAbsolute(VirtualAccountsPath) : AbsoluteAccountsPath;
        url += "view/";
        url += queryParams;

        return url;
    }

    public async Task<string> GetUserProfileAsync(Guid user, bool absolute = true)
    {
        var queryParams = await GetUserParamsPairAsync(user);

        var url = absolute ? ToAbsolute(VirtualAccountsPath) : AbsoluteAccountsPath;
        url += "view/";
        url += queryParams;

        return url;
    }

    #endregion

    public Guid GetProductID()
    {
        var productID = Guid.Empty;

        if (_httpContextAccessor?.HttpContext != null)
        {
            GetLocationByRequest(out var product, out _);
            if (product != null)
            {
                productID = product.ID;
            }
        }

        return productID;
    }

    public Guid GetAddonID()
    {
        var addonID = Guid.Empty;

        if (_httpContextAccessor?.HttpContext != null)
        {
            var addonName = GetAddonNameFromUrl(_httpContextAccessor.HttpContext.Request.Url().AbsoluteUri);

            switch (addonName)
            {
                case "mail":
                    addonID = WebItemManager.MailProductID;
                    break;
                case "talk":
                    addonID = WebItemManager.TalkProductID;
                    break;
                case "calendar":
                    addonID = WebItemManager.CalendarProductID;
                    break;
                default:
                    break;
            }
        }

        return addonID;
    }

    public void GetLocationByRequest(out IProduct currentProduct, out IModule currentModule)
    {
        var currentURL = string.Empty;
        if (_httpContextAccessor?.HttpContext?.Request != null)
        {
            currentURL = _httpContextAccessor.HttpContext.Request.Url().AbsoluteUri;

            //TODO ?
            // http://[hostname]/[virtualpath]/[AjaxPro.Utility.HandlerPath]/[assembly],[classname].ashx
            //if (currentURL.Contains("/" + AjaxPro.Utility.HandlerPath + "/") && HttpContext.Current.Request.Headers["Referer"].FirstOrDefault() != null)
            //{
            //    currentURL = HttpContext.Current.Request.Headers["Referer"].FirstOrDefault().ToString();
            //}
        }

        GetLocationByUrl(currentURL, out currentProduct, out currentModule);
    }

    public IWebItem GetWebItemByUrl(string currentURL)
    {
        if (!string.IsNullOrEmpty(currentURL))
        {

            var itemName = GetWebItemNameFromUrl(currentURL);
            if (!string.IsNullOrEmpty(itemName))
            {
                foreach (var item in _webItemManager.GetItemsAll())
                {
                    var _itemName = GetWebItemNameFromUrl(item.StartURL);
                    if (itemName.Equals(_itemName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return item;
                    }
                }
            }
            else
            {
                var urlParams = HttpUtility.ParseQueryString(new Uri(currentURL).Query);
                var productByName = GetProductBySysName(urlParams[ParamName_ProductSysName]);
                var pid = productByName == null ? Guid.Empty : productByName.ID;

                if (pid == Guid.Empty && !string.IsNullOrEmpty(urlParams["pid"]))
                {
                    try
                    {
                        pid = new Guid(urlParams["pid"]);
                    }
                    catch
                    {
                        pid = Guid.Empty;
                    }
                }

                if (pid != Guid.Empty)
                {
                    return _webItemManager[pid];
                }
            }
        }

        return null;
    }

    public void GetLocationByUrl(string currentURL, out IProduct currentProduct, out IModule currentModule)
    {
        currentProduct = null;
        currentModule = null;

        if (string.IsNullOrEmpty(currentURL))
        {
            return;
        }

        var urlParams = HttpUtility.ParseQueryString(new Uri(currentURL).Query);
        var productByName = GetProductBySysName(urlParams[ParamName_ProductSysName]);
        var pid = productByName == null ? Guid.Empty : productByName.ID;

        if (pid == Guid.Empty && !string.IsNullOrEmpty(urlParams["pid"]))
        {
            try
            {
                pid = new Guid(urlParams["pid"]);
            }
            catch
            {
                pid = Guid.Empty;
            }
        }

        var productName = GetProductNameFromUrl(currentURL);
        var moduleName = GetModuleNameFromUrl(currentURL);

        if (!string.IsNullOrEmpty(productName) || !string.IsNullOrEmpty(moduleName))
        {
            foreach (var product in _webItemManager.GetItemsAll<IProduct>())
            {
                var _productName = GetProductNameFromUrl(product.StartURL);
                if (!string.IsNullOrEmpty(_productName))
                {
                    if (string.Equals(productName, _productName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        currentProduct = product;

                        if (!string.IsNullOrEmpty(moduleName))
                        {
                            foreach (var module in _webItemManagerSecurity.GetSubItems(product.ID).OfType<IModule>())
                            {
                                var _moduleName = GetModuleNameFromUrl(module.StartURL);
                                if (!string.IsNullOrEmpty(_moduleName))
                                {
                                    if (string.Equals(moduleName, _moduleName, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        currentModule = module;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var module in _webItemManagerSecurity.GetSubItems(product.ID).OfType<IModule>())
                            {
                                if (!module.StartURL.Equals(product.StartURL) && currentURL.Contains(_regFilePathTrim.Replace(module.StartURL, string.Empty)))
                                {
                                    currentModule = module;
                                    break;
                                }
                            }
                        }

                        break;
                    }
                }
            }
        }

        if (pid != Guid.Empty)
        {
            currentProduct = _webItemManager[pid] as IProduct;
        }
    }

    private string GetWebItemNameFromUrl(string url)
    {
        var name = GetModuleNameFromUrl(url);
        if (string.IsNullOrEmpty(name))
        {
            name = GetProductNameFromUrl(url);
            if (string.IsNullOrEmpty(name))
            {
                return GetAddonNameFromUrl(url);
            }

        }

        return name;
    }

    private string GetProductNameFromUrl(string url)
    {
        try
        {
            var pos = url.IndexOf("/products/", StringComparison.InvariantCultureIgnoreCase);
            if (0 <= pos)
            {
                url = url.Substring(pos + 10).ToLower();
                pos = url.IndexOf('/');
                return 0 < pos ? url.Substring(0, pos) : url;
            }
        }
        catch
        {
        }
        return null;
    }

    private static string GetAddonNameFromUrl(string url)
    {
        try
        {
            var pos = url.IndexOf("/addons/", StringComparison.InvariantCultureIgnoreCase);
            if (0 <= pos)
            {
                url = url.Substring(pos + 8).ToLower();
                pos = url.IndexOf('/');
                return 0 < pos ? url.Substring(0, pos) : url;
            }
        }
        catch
        {
        }
        return null;
    }

    private static string GetModuleNameFromUrl(string url)
    {
        try
        {
            var pos = url.IndexOf("/modules/", StringComparison.InvariantCultureIgnoreCase);
            if (0 <= pos)
            {
                url = url.Substring(pos + 9).ToLower();
                pos = url.IndexOf('/');
                return 0 < pos ? url.Substring(0, pos) : url;
            }
        }
        catch
        {
        }
        return null;
    }

    private IProduct GetProductBySysName(string sysName)
    {
        IProduct result = null;

        if (!string.IsNullOrEmpty(sysName))
        {
            foreach (var product in _webItemManager.GetItemsAll<IProduct>())
            {
                if (string.Equals(sysName, WebItemExtension.GetSysName(product)))
                {
                    result = product;
                    break;
                }
            }
        }

        return result;
    }

    public async Task<string> GetUserParamsPairAsync(Guid userID)
    {
        return GetUserParamsPair(await _userManager.GetUsersAsync(userID));
    }

    public string GetUserParamsPair(UserInfo user)
    {
        if (user == null || string.IsNullOrEmpty(user.UserName) || !_userManager.UserExists(user))
        {
            return "";
        }

        return HttpUtility.UrlEncode(user.UserName.ToLowerInvariant());
    }

    #region Help Centr

    public async Task<string> GetHelpLinkAsync(SettingsManager settingsManager, AdditionalWhiteLabelSettingsHelperInit additionalWhiteLabelSettingsHelper, bool inCurrentCulture = true)
    {
        if (!(await settingsManager.LoadForDefaultTenantAsync<AdditionalWhiteLabelSettings>()).HelpCenterEnabled)
        {
            return string.Empty;
        }

        var url = additionalWhiteLabelSettingsHelper.DefaultHelpCenterUrl;

        if (string.IsNullOrEmpty(url))
        {
            return string.Empty;
        }

        return GetRegionalUrl(url, inCurrentCulture ? CultureInfo.CurrentCulture.TwoLetterISOLanguageName : null);
    }

    public string GetHelpLink(SettingsManager settingsManager, AdditionalWhiteLabelSettingsHelperInit additionalWhiteLabelSettingsHelper, bool inCurrentCulture = true)
    {
        if (!settingsManager.LoadForDefaultTenant<AdditionalWhiteLabelSettings>().HelpCenterEnabled)
        {
            return string.Empty;
        }

        var url = additionalWhiteLabelSettingsHelper.DefaultHelpCenterUrl;

        if (string.IsNullOrEmpty(url))
        {
            return string.Empty;
        }

        return GetRegionalUrl(url, inCurrentCulture ? CultureInfo.CurrentCulture.TwoLetterISOLanguageName : null);
    }

    public string GetFeedbackAndSupportLink(SettingsManager settingsManager, bool inCurrentCulture = true)
    {
        var settings = settingsManager.LoadForDefaultTenant<AdditionalWhiteLabelSettings>();

        return !settings.FeedbackAndSupportEnabled || string.IsNullOrEmpty(settings.FeedbackAndSupportUrl)
            ? string.Empty
            : GetRegionalUrl(settings.FeedbackAndSupportUrl, inCurrentCulture ? CultureInfo.CurrentCulture.TwoLetterISOLanguageName : null);
    }

    #endregion

    #region management links

    public string GetAdministration(ManagementType managementType)
    {
        if (managementType == ManagementType.General)
        {
            return ToAbsolute("~/management.aspx") + string.Empty;
        }

        return ToAbsolute("~/management.aspx") + "?" + "type=" + ((int)managementType).ToString();
    }

    #endregion

    #region confirm links

    public async Task<string> GetConfirmationEmailUrlAsync(string email, ConfirmType confirmType, object postfix = null, Guid userId = default)
    {
        return GetFullAbsolutePath(await GetConfirmationUrlRelativeAsync(email, confirmType, postfix, userId));
    }

    public string GetConfirmationUrl(string key, ConfirmType confirmType, Guid userId = default)
    {
        return GetFullAbsolutePath(GetConfirmationUrlRelative(key, confirmType, userId));
    }

    public async Task<string> GetConfirmationUrlRelativeAsync(string email, ConfirmType confirmType, object postfix = null, Guid userId = default)
    {
        return GetConfirmationUrlRelative(await _tenantManager.GetCurrentTenantIdAsync(), email, confirmType, postfix, userId);
    }

    public string GetConfirmationUrlRelative(int tenantId, string email, ConfirmType confirmType, object postfix = null, Guid userId = default)
    {
        return $"confirm/{confirmType}?{GetToken(tenantId, email, confirmType, postfix, userId)}";
    }

    public string GetConfirmationUrlRelative(string key, ConfirmType confirmType, Guid userId = default)
    {
        return $"confirm/{confirmType}?type={confirmType}&key={key}&uid={userId}";
    }

    public string GetToken(int tenantId, string email, ConfirmType confirmType, object postfix = null, Guid userId = default)
    {
        var validationKey = _emailValidationKeyProvider.GetEmailKey(tenantId, email + confirmType + (postfix ?? ""));

        var link = $"type={confirmType}&key={validationKey}";

        if (!string.IsNullOrEmpty(email))
        {
            link += $"&email={HttpUtility.UrlEncode(email)}";
        }

        if (userId != default)
        {
            link += $"&uid={userId}";
        }

        return link;
    }

    #endregion

}
