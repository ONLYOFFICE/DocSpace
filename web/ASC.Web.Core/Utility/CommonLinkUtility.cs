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
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common;
using ASC.Core.Common.Settings;
using ASC.Core.Users;
using ASC.Security.Cryptography;
using ASC.Web.Core;
using ASC.Web.Core.WhiteLabel;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ASC.Web.Studio.Utility
{
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
        private static readonly Regex RegFilePathTrim = new Regex("/[^/]*\\.aspx", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public const string ParamName_ProductSysName = "product";
        public const string ParamName_UserUserID = "uid";

        public CommonLinkUtility(
            CoreBaseSettings coreBaseSettings,
            CoreSettings coreSettings,
            TenantManager tenantManager,
            UserManager userManager,
            WebItemManagerSecurity webItemManagerSecurity,
            WebItemManager webItemManager,
            EmailValidationKeyProvider emailValidationKeyProvider,
            IOptionsMonitor<ILog> options,
            CommonLinkUtilitySettings settings) :
            this(null, coreBaseSettings, coreSettings, tenantManager, userManager, webItemManagerSecurity, webItemManager, emailValidationKeyProvider, options, settings)
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
            IOptionsMonitor<ILog> options,
            CommonLinkUtilitySettings settings) :
            base(httpContextAccessor, coreBaseSettings, coreSettings, tenantManager, options, settings) =>
            (UserManager, WebItemManagerSecurity, WebItemManager, EmailValidationKeyProvider) = (userManager, webItemManagerSecurity, webItemManager, emailValidationKeyProvider);

        public string Logout
        {
            get { return ToAbsolute("~/auth.aspx") + "?t=logout"; }
        }

        private UserManager UserManager { get; }
        private WebItemManagerSecurity WebItemManagerSecurity { get; }
        private WebItemManager WebItemManager { get; }
        private EmailValidationKeyProvider EmailValidationKeyProvider { get; }

        public string GetDefault()
        {
            return VirtualRoot;
        }

        public string GetMyStaff()
        {
            return CoreBaseSettings.Personal ? ToAbsolute("~/my") : ToAbsolute("~/products/people/view/@self");
        }

        public string GetUnsubscribe()
        {
            return CoreBaseSettings.Personal ? ToAbsolute("~/my?unsubscribe=tips") : ToAbsolute("~/products/people/view/@self");
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

        public string GetUserProfile(Guid userID)
        {
            if (!UserManager.UserExists(userID))
                return GetEmployees();

            return GetUserProfile(userID.ToString());
        }

        public string GetUserProfile(UserInfo user)
        {
            if (!UserManager.UserExists(user))
                return GetEmployees();

            return GetUserProfile(user, true);
        }

        public string GetUserProfile(string user, bool absolute = true)
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

                queryParams = guid != Guid.Empty ? GetUserParamsPair(guid) : HttpUtility.UrlEncode(user.ToLowerInvariant());
            }

            var url = absolute ? ToAbsolute("~/products/people/") : "/products/people/";
            url += "view/";
            url += queryParams;

            return url;
        }

        public string GetUserProfile(UserInfo user, bool absolute = true)
        {
            var queryParams = user.ID != Guid.Empty ? GetUserParamsPair(user) : HttpUtility.UrlEncode(user.UserName.ToLowerInvariant());

            var url = absolute ? ToAbsolute("~/products/people/") : "/products/people/";
            url += "view/";
            url += queryParams;

            return url;
        }
        public string GetUserProfile(Guid user, bool absolute = true)
        {
            var queryParams = GetUserParamsPair(user);

            var url = absolute ? ToAbsolute("~/products/people/") : "/products/people/";
            url += "view/";
            url += queryParams;

            return url;
        }

        #endregion

        public Guid GetProductID()
        {
            var productID = Guid.Empty;

            if (HttpContextAccessor?.HttpContext != null)
            {
                GetLocationByRequest(out var product, out _);
                if (product != null) productID = product.ID;
            }

            return productID;
        }

        public Guid GetAddonID()
        {
            var addonID = Guid.Empty;

            if (HttpContextAccessor?.HttpContext != null)
            {
                var addonName = GetAddonNameFromUrl(HttpContextAccessor.HttpContext.Request.Url().AbsoluteUri);

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
            if (HttpContextAccessor?.HttpContext?.Request != null)
            {
                currentURL = HttpContextAccessor.HttpContext.Request.GetUrlRewriter().AbsoluteUri;

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
                    foreach (var item in WebItemManager.GetItemsAll())
                    {
                        var _itemName = GetWebItemNameFromUrl(item.StartURL);
                        if (itemName.Equals(_itemName, StringComparison.InvariantCultureIgnoreCase))
                            return item;
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
                        return WebItemManager[pid];
                }
            }

            return null;
        }

        public void GetLocationByUrl(string currentURL, out IProduct currentProduct, out IModule currentModule)
        {
            currentProduct = null;
            currentModule = null;

            if (string.IsNullOrEmpty(currentURL)) return;

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
                foreach (var product in WebItemManager.GetItemsAll<IProduct>())
                {
                    var _productName = GetProductNameFromUrl(product.StartURL);
                    if (!string.IsNullOrEmpty(_productName))
                    {
                        if (string.Equals(productName, _productName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            currentProduct = product;

                            if (!string.IsNullOrEmpty(moduleName))
                            {
                                foreach (var module in WebItemManagerSecurity.GetSubItems(product.ID).OfType<IModule>())
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
                                foreach (var module in WebItemManagerSecurity.GetSubItems(product.ID).OfType<IModule>())
                                {
                                    if (!module.StartURL.Equals(product.StartURL) && currentURL.Contains(RegFilePathTrim.Replace(module.StartURL, string.Empty)))
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
                currentProduct = WebItemManager[pid] as IProduct;
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
                foreach (var product in WebItemManager.GetItemsAll<IProduct>())
                {
                    if (string.Equals(sysName, WebItemExtension.GetSysName(product)))
                    {
                        result = product;
                        break;
                    }
                }

            return result;
        }

        public string GetUserParamsPair(Guid userID)
        {
            return GetUserParamsPair(UserManager.GetUsers(userID));
        }

        public string GetUserParamsPair(UserInfo user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserName) || !UserManager.UserExists(user))
                return "";

            return HttpUtility.UrlEncode(user.UserName.ToLowerInvariant());
        }

        #region Help Centr

        public string GetHelpLink(SettingsManager settingsManager, AdditionalWhiteLabelSettingsHelper additionalWhiteLabelSettingsHelper, bool inCurrentCulture = true)
        {
            if (!settingsManager.LoadForDefaultTenant<AdditionalWhiteLabelSettings>().HelpCenterEnabled)
                return string.Empty;

            var url = additionalWhiteLabelSettingsHelper.DefaultHelpCenterUrl;

            if (string.IsNullOrEmpty(url))
                return string.Empty;

            return GetRegionalUrl(url, inCurrentCulture ? CultureInfo.CurrentCulture.TwoLetterISOLanguageName : null);
        }

        #endregion

        #region management links

        public string GetAdministration(ManagementType managementType)
        {
            if (managementType == ManagementType.General)
                return ToAbsolute("~/management.aspx") + string.Empty;

            return ToAbsolute("~/management.aspx") + "?" + "type=" + ((int)managementType).ToString();
        }

        #endregion

        #region confirm links

        public string GetConfirmationUrl(string email, ConfirmType confirmType, object postfix = null, Guid userId = default)
        {
            return GetFullAbsolutePath(GetConfirmationUrlRelative(email, confirmType, postfix, userId));
        }

        public string GetConfirmationUrlRelative(string email, ConfirmType confirmType, object postfix = null, Guid userId = default)
        {
            return GetConfirmationUrlRelative(TenantManager.GetCurrentTenant().TenantId, email, confirmType, postfix, userId);
        }

        public string GetConfirmationUrlRelative(int tenantId, string email, ConfirmType confirmType, object postfix = null, Guid userId = default)
        {
            return $"confirm/{confirmType}?{GetToken(tenantId, email, confirmType, postfix, userId)}";
        }

        public string GetToken(int tenantId, string email, ConfirmType confirmType, object postfix = null, Guid userId = default)
        {
            var validationKey = EmailValidationKeyProvider.GetEmailKey(tenantId, email + confirmType + (postfix ?? ""));

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
}
