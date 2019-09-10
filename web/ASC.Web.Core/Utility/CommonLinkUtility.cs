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
using ASC.Core;
using ASC.Core.Common;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Security.Cryptography;
using ASC.Web.Core;
using ASC.Web.Core.WhiteLabel;
using Microsoft.AspNetCore.Http;

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
        Storage = 21
    }

    //  emp-invite - confirm ivite by email
    //  portal-suspend - confirm portal suspending - Tenant.SetStatus(TenantStatus.Suspended)
    //  portal-continue - confirm portal continuation  - Tenant.SetStatus(TenantStatus.Active)
    //  portal-remove - confirm portal deletation - Tenant.SetStatus(TenantStatus.RemovePending)
    //  DnsChange - change Portal Address and/or Custom domain name
    public enum ConfirmType
    {
        EmpInvite,
        LinkInvite,
        PortalSuspend,
        PortalContinue,
        PortalRemove,
        DnsChange,
        PortalOwnerChange,
        Activation,
        EmailChange,
        EmailActivation,
        PasswordChange,
        ProfileRemove,
        PhoneActivation,
        PhoneAuth,
        Auth,
        TfaActivation,
        TfaAuth
    }

    public static class CommonLinkUtility
    {
        private static readonly Regex RegFilePathTrim = new Regex("/[^/]*\\.aspx", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public const string ParamName_ProductSysName = "product";
        public const string ParamName_UserUserName = "user";
        public const string ParamName_UserUserID = "uid";

        public static void Initialize(string serverUri)
        {
            BaseCommonLinkUtility.Initialize(serverUri);
        }

        public static string VirtualRoot
        {
            get { return BaseCommonLinkUtility.VirtualRoot; }
        }

        public static string ServerRootPath(HttpContext context)
        {
            return BaseCommonLinkUtility.ServerRootPath(context);
        }

        public static string GetFullAbsolutePath(string virtualPath)
        {
            return GetFullAbsolutePath(ASC.Common.HttpContext.Current, virtualPath);
        }
        public static string GetFullAbsolutePath(HttpContext context, string virtualPath)
        {
            return BaseCommonLinkUtility.GetFullAbsolutePath(context, virtualPath);
        }

        public static string ToAbsolute(string virtualPath)
        {
            return BaseCommonLinkUtility.ToAbsolute(virtualPath);
        }

        public static string Logout
        {
            get { return ToAbsolute("~/auth.aspx") + "?t=logout"; }
        }

        public static string GetDefault()
        {
            return VirtualRoot;
        }

        public static string GetMyStaff()
        {
            return CoreContext.Configuration.Personal ? ToAbsolute("~/my.aspx") : ToAbsolute("~/products/people/profile.aspx");
        }

        public static string GetEmployees()
        {
            return GetEmployees(EmployeeStatus.Active);
        }

        public static string GetEmployees(EmployeeStatus empStatus)
        {
            return ToAbsolute("~/products/people/") +
                   (empStatus == EmployeeStatus.Terminated ? "#type=disabled" : string.Empty);
        }

        public static string GetDepartment(Guid depId)
        {
            return depId != Guid.Empty ? ToAbsolute("~/products/people/#group=") + depId.ToString() : GetEmployees();
        }

        #region user profile link

        public static string GetUserProfile(int tenantId, Guid userID, UserManager userManager)
        {
            if (!userManager.UserExists(tenantId, userID))
                return GetEmployees();

            return GetUserProfile(tenantId, userID.ToString(), userManager);
        }

        public static string GetUserProfile(UserInfo user, UserManager userManager)
        {
            if (!userManager.UserExists(user))
                return GetEmployees();

            return GetUserProfile(user, userManager, true);
        }

        public static string GetUserProfile(int tenantId, string user, UserManager userManager, bool absolute = true)
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

                queryParams = guid != Guid.Empty ? GetUserParamsPair(tenantId, guid, userManager) : ParamName_UserUserName + "=" + HttpUtility.UrlEncode(user);
            }

            var url = absolute ? ToAbsolute("~/products/people/") : "/products/people/";
            url += "profile.aspx?";
            url += queryParams;

            return url;
        }

        public static string GetUserProfile(UserInfo user, UserManager userManager, bool absolute = true)
        {
            var queryParams = user.ID != Guid.Empty ? GetUserParamsPair(user, userManager) : ParamName_UserUserName + "=" + HttpUtility.UrlEncode(user.UserName);

            var url = absolute ? ToAbsolute("~/products/people/") : "/products/people/";
            url += "profile.aspx?";
            url += queryParams;

            return url;
        }
        public static string GetUserProfile(int tenantId, Guid user, UserManager userManager, bool absolute = true)
        {
            var queryParams = GetUserParamsPair(tenantId, user, userManager);

            var url = absolute ? ToAbsolute("~/products/people/") : "/products/people/";
            url += "profile.aspx?";
            url += queryParams;

            return url;
        }

        #endregion

        public static Guid GetProductID(Tenant tenant, HttpContext context, UserManager userManager, WebItemSecurity webItemSecurity, AuthContext authContext)
        {
            var productID = Guid.Empty;

            if (context != null)
            {
                GetLocationByRequest(tenant, userManager, webItemSecurity, authContext, out var product, out _, context);
                if (product != null) productID = product.ID;
            }

            return productID;
        }

        public static void GetLocationByRequest(Tenant tenant, UserManager userManager, WebItemSecurity webItemSecurity, AuthContext authContext, out IProduct currentProduct, out IModule currentModule, HttpContext context)
        {
            var currentURL = string.Empty;
            if (context != null && context.Request != null)
            {
                currentURL = context.Request.GetUrlRewriter().AbsoluteUri;

                //TODO ?
                // http://[hostname]/[virtualpath]/[AjaxPro.Utility.HandlerPath]/[assembly],[classname].ashx
                //if (currentURL.Contains("/" + AjaxPro.Utility.HandlerPath + "/") && HttpContext.Current.Request.Headers["Referer"].FirstOrDefault() != null)
                //{
                //    currentURL = HttpContext.Current.Request.Headers["Referer"].FirstOrDefault().ToString();
                //}
            }

            GetLocationByUrl(tenant, currentURL, userManager, webItemSecurity, authContext, out currentProduct, out currentModule);
        }

        public static IWebItem GetWebItemByUrl(string currentURL)
        {
            if (!string.IsNullOrEmpty(currentURL))
            {

                var itemName = GetWebItemNameFromUrl(currentURL);
                if (!string.IsNullOrEmpty(itemName))
                {
                    foreach (var item in WebItemManager.Instance.GetItemsAll())
                    {
                        var _itemName = GetWebItemNameFromUrl(item.StartURL);
                        if (string.Compare(itemName, _itemName, StringComparison.InvariantCultureIgnoreCase) == 0)
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
                        return WebItemManager.Instance[pid];
                }
            }

            return null;
        }

        public static void GetLocationByUrl(Tenant tenant, string currentURL, UserManager userManager, WebItemSecurity webItemSecurity, AuthContext authContext, out IProduct currentProduct, out IModule currentModule)
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
                foreach (var product in WebItemManager.Instance.GetItemsAll<IProduct>())
                {
                    var _productName = GetProductNameFromUrl(product.StartURL);
                    if (!string.IsNullOrEmpty(_productName))
                    {
                        if (string.Compare(productName, _productName, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            currentProduct = product;

                            if (!string.IsNullOrEmpty(moduleName))
                            {
                                foreach (var module in WebItemManager.Instance.GetSubItems(tenant, product.ID, webItemSecurity, authContext).OfType<IModule>())
                                {
                                    var _moduleName = GetModuleNameFromUrl(module.StartURL);
                                    if (!string.IsNullOrEmpty(_moduleName))
                                    {
                                        if (string.Compare(moduleName, _moduleName, StringComparison.InvariantCultureIgnoreCase) == 0)
                                        {
                                            currentModule = module;
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (var module in WebItemManager.Instance.GetSubItems(tenant, product.ID, webItemSecurity, authContext).OfType<IModule>())
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
                currentProduct = WebItemManager.Instance[pid] as IProduct;
        }

        private static string GetWebItemNameFromUrl(string url)
        {
            var name = GetModuleNameFromUrl(url);
            if (string.IsNullOrEmpty(name))
            {
                name = GetProductNameFromUrl(url);
                if (string.IsNullOrEmpty(name))
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

            }

            return name;
        }

        private static string GetProductNameFromUrl(string url)
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

        private static IProduct GetProductBySysName(string sysName)
        {
            IProduct result = null;

            if (!string.IsNullOrEmpty(sysName))
                foreach (var product in WebItemManager.Instance.GetItemsAll<IProduct>())
                {
                    if (string.CompareOrdinal(sysName, WebItemExtension.GetSysName(product as IWebItem)) == 0)
                    {
                        result = product;
                        break;
                    }
                }

            return result;
        }

        public static string GetUserParamsPair(int tenantId, Guid userID, UserManager userManager)
        {
            return GetUserParamsPair(userManager.GetUsers(tenantId, userID), userManager);
        }

        public static string GetUserParamsPair(UserInfo user, UserManager userManager)
        {
            if (user == null || string.IsNullOrEmpty(user.UserName) || !userManager.UserExists(user))
                return "";

            return string.Format("{0}={1}", ParamName_UserUserName, HttpUtility.UrlEncode(user.UserName.ToLowerInvariant()));
        }

        #region Help Centr

        public static string GetHelpLink(bool inCurrentCulture = true)
        {
            if (!AdditionalWhiteLabelSettings.Instance.HelpCenterEnabled)
                return string.Empty;

            var url = AdditionalWhiteLabelSettings.DefaultHelpCenterUrl;

            if (string.IsNullOrEmpty(url))
                return string.Empty;

            return GetRegionalUrl(url, inCurrentCulture ? CultureInfo.CurrentCulture.TwoLetterISOLanguageName : null);
        }

        public static string GetRegionalUrl(string url, string lang)
        {
            return BaseCommonLinkUtility.GetRegionalUrl(url, lang);
        }

        #endregion

        #region management links

        public static string GetAdministration(ManagementType managementType)
        {
            if (managementType == ManagementType.General)
                return ToAbsolute("~/management.aspx") + string.Empty;

            return ToAbsolute("~/management.aspx") + "?" + "type=" + ((int)managementType).ToString();
        }

        #endregion

        #region confirm links

        public static string GetConfirmationUrl(int tenantId, string email, ConfirmType confirmType, object postfix = null, Guid userId = default)
        {
            return GetFullAbsolutePath(GetConfirmationUrlRelative(tenantId, email, confirmType, postfix, userId));
        }

        public static string GetConfirmationUrlRelative(int tenantId, string email, ConfirmType confirmType, object postfix = null, Guid userId = default)
        {
            var validationKey = EmailValidationKeyProvider.GetEmailKey(tenantId, email + confirmType + (postfix ?? ""));

            var link = $"confirm?key={validationKey}&type={confirmType}";

            if (!string.IsNullOrEmpty(email))
            {
                link += $"&email={HttpUtility.UrlEncode(email)}";
            }

            if (userId != default)
            {
                link += $"&uid={userId}";
            }

            if (postfix != null)
            {
                link += "&p=1";
            }

            return link;
        }

        #endregion

    }
}
