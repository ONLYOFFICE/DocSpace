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

namespace ASC.Web.Api.Core;

[Scope]
public class DnsSettings
{
    private readonly PermissionContext _permissionContext;
    private readonly TenantManager _tenantManager;
    private readonly UserManager _userManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CoreSettings _coreSettings;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly MessageService _messageService;
    private readonly TenantExtra _tenantExtra;

    public DnsSettings(
        PermissionContext permissionContext,
        TenantManager tenantManager,
        UserManager userManager,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        StudioNotifyService studioNotifyService,
        CommonLinkUtility commonLinkUtility,
        MessageService messageService,
        TenantExtra tenantExtra)
    {
        _permissionContext = permissionContext;
        _tenantManager = tenantManager;
        _userManager = userManager;
        _coreBaseSettings = coreBaseSettings;
        _coreSettings = coreSettings;
        _studioNotifyService = studioNotifyService;
        _commonLinkUtility = commonLinkUtility;
        _messageService = messageService;
        _tenantExtra = tenantExtra;
    }

    public string SaveDnsSettings(string dnsName, bool enableDns)
    {
        try
        {
            if (!SetupInfo.IsVisibleSettings<DnsSettings>())
            {
                throw new Exception(Resource.ErrorNotAllowedOption);
            }

            _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var tenant = _tenantManager.GetCurrentTenant();

            if (!enableDns || string.IsNullOrEmpty(dnsName))
            {
                dnsName = null;
            }

            if (dnsName == null || CheckCustomDomain(dnsName))
            {
                if (_coreBaseSettings.Standalone)
                {
                    tenant.MappedDomain = dnsName;
                    _tenantManager.SaveTenant(tenant);
                    return null;
                }

                if (tenant.MappedDomain != dnsName)
                {
                    var portalAddress = $"http://{tenant.Alias ?? string.Empty}.{_coreSettings.BaseDomain}";

                    var u = _userManager.GetUsers(tenant.OwnerId);
                    _studioNotifyService.SendMsgDnsChange(tenant, GenerateDnsChangeConfirmUrl(u.Email, dnsName, tenant.Alias, ConfirmType.DnsChange), portalAddress, dnsName);

                    _messageService.Send(MessageAction.DnsSettingsUpdated);
                    return string.Format(Resource.DnsChangeMsg, string.Format("<a href=\"mailto:{0}\">{0}</a>", u.Email.HtmlEncode()));
                }

                return null;
            }

            throw new Exception(Resource.ErrorNotCorrectTrustedDomain);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message.HtmlEncode());
        }
    }

    private bool CheckCustomDomain(string domain)
    {
        if (string.IsNullOrEmpty(domain))
        {
            return false;
        }

        var tenantBaseDomain = TenantBaseDomain;

        if (!string.IsNullOrEmpty(tenantBaseDomain) &&
            (domain.EndsWith(tenantBaseDomain, StringComparison.InvariantCultureIgnoreCase) || domain.Equals(tenantBaseDomain.TrimStart('.'), StringComparison.InvariantCultureIgnoreCase)))
        {
            return false;
        }
        Uri test;
        if (Uri.TryCreate(domain.Contains(Uri.SchemeDelimiter) ? domain : Uri.UriSchemeHttp + Uri.SchemeDelimiter + domain, UriKind.Absolute, out test))
        {
            try
            {
                _tenantManager.CheckTenantAddress(test.Host);
            }
            catch (TenantTooShortException ex)
            {
                var minLength = ex.MinLength;
                var maxLength = ex.MaxLength;
                if (minLength > 0 && maxLength > 0)
                {
                    throw new TenantTooShortException(string.Format(Resource.ErrorTenantTooShortFormat, minLength, maxLength));
                }

                throw new TenantTooShortException(Resource.ErrorTenantTooShort);
            }
            catch (TenantIncorrectCharsException)
            {
            }
            return true;
        }
        return false;
    }

    private string GenerateDnsChangeConfirmUrl(string email, string dnsName, string tenantAlias, ConfirmType confirmType)
    {
        var postfix = string.Join(string.Empty, new[] { dnsName, tenantAlias });

        var sb = new StringBuilder();
        sb.Append(_commonLinkUtility.GetConfirmationUrl(email, confirmType, postfix));
        if (!string.IsNullOrEmpty(dnsName))
        {
            sb.AppendFormat("&dns={0}", dnsName);
        }
        if (!string.IsNullOrEmpty(tenantAlias))
        {
            sb.AppendFormat("&alias={0}", tenantAlias);
        }
        return sb.ToString();
    }

    protected string TenantBaseDomain
    {
        get
        {
            return string.IsNullOrEmpty(_coreSettings.BaseDomain)
                       ? string.Empty
                       : $".{_coreSettings.BaseDomain}";
        }
    }
}
