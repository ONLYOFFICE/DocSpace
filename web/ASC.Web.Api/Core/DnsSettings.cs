using System;
using System.Text;

using ASC.Common;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.MessagingSystem;
using ASC.Web.Core.PublicResources;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Api.Core
{
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

        protected bool EnableDomain
        {
            get { return _coreBaseSettings.Standalone || _tenantExtra.GetTenantQuota().HasDomain; }
        }

        public string SaveDnsSettings(string dnsName, bool enableDns)
        {
            try
            {
                if (!EnableDomain || !SetupInfo.IsVisibleSettings<DnsSettings>()) throw new Exception(Resource.ErrorNotAllowedOption);

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
                        var portalAddress = $"http://{tenant.TenantAlias ?? string.Empty}.{ _coreSettings.BaseDomain}";

                        var u = _userManager.GetUsers(tenant.OwnerId);
                        _studioNotifyService.SendMsgDnsChange(tenant, GenerateDnsChangeConfirmUrl(u.Email, dnsName, tenant.TenantAlias, ConfirmType.DnsChange), portalAddress, dnsName);

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
            if (!string.IsNullOrEmpty(TenantBaseDomain) &&
                (domain.EndsWith(TenantBaseDomain, StringComparison.InvariantCultureIgnoreCase) || domain.Equals(TenantBaseDomain.TrimStart('.'), StringComparison.InvariantCultureIgnoreCase)))
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
}
