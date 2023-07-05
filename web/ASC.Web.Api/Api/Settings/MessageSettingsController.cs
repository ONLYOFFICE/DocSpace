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

namespace ASC.Web.Api.Controllers.Settings;

public class MessageSettingsController : BaseSettingsController
{
    private Tenant Tenant { get { return ApiContext.Tenant; } }

    private readonly MessageService _messageService;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly CustomNamingPeople _customNamingPeople;
    private readonly IPSecurity.IPSecurity _ipSecurity;
    private readonly TenantManager _tenantManager;
    private readonly CookiesManager _cookiesManager;
    private readonly CountPaidUserChecker _countPaidUserChecker;
    private readonly UserManager _userManager;
    private readonly TenantExtra _tenantExtra;
    private readonly PermissionContext _permissionContext;
    private readonly SettingsManager _settingsManager;
    private readonly CoreBaseSettings _coreBaseSettings;

    public MessageSettingsController(
        MessageService messageService,
        StudioNotifyService studioNotifyService,
        ApiContext apiContext,
        UserManager userManager,
        TenantExtra tenantExtra,
        PermissionContext permissionContext,
        SettingsManager settingsManager,
        WebItemManager webItemManager,
        CoreBaseSettings coreBaseSettings,
        CustomNamingPeople customNamingPeople,
        IPSecurity.IPSecurity ipSecurity,
        IMemoryCache memoryCache,
        IHttpContextAccessor httpContextAccessor,
        TenantManager tenantManager,
        CookiesManager cookiesManager,
        CountPaidUserChecker countPaidUserChecker) : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _customNamingPeople = customNamingPeople;
        _ipSecurity = ipSecurity;
        _tenantManager = tenantManager;
        _cookiesManager = cookiesManager;
        _countPaidUserChecker = countPaidUserChecker;
        _messageService = messageService;
        _studioNotifyService = studioNotifyService;
        _userManager = userManager;
        _tenantExtra = tenantExtra;
        _permissionContext = permissionContext;
        _settingsManager = settingsManager;
        _coreBaseSettings = coreBaseSettings;
    }

    [HttpPost("messagesettings")]
    public object EnableAdminMessageSettings(AdminMessageSettingsRequestsDto inDto)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        _settingsManager.Save(new StudioAdminMessageSettings { Enable = inDto.TurnOn });

        _messageService.Send(MessageAction.AdministratorMessageSettingsUpdated);

        return Resource.SuccessfullySaveSettingsMessage;
    }

    [HttpGet("cookiesettings")]
    public CookieSettingsDto GetCookieSettings()
    {
        var result = _cookiesManager.GetLifeTime(_tenantManager.GetCurrentTenant().Id);

        return new CookieSettingsDto
        {
            Enabled = result.Enabled,
            LifeTime = result.LifeTime
        };
    }

    [HttpPut("cookiesettings")]
    public async Task<object> UpdateCookieSettings(CookieSettingsRequestsDto model)
    {
        _permissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

        if (!SetupInfo.IsVisibleSettings("CookieSettings"))
        {
            throw new BillingException(Resource.ErrorNotAllowedOption, "CookieSettings");
        }

        await _cookiesManager.SetLifeTime(model.LifeTime, model.Enabled);

        _messageService.Send(MessageAction.CookieSettingsUpdated);

        return Resource.SuccessfullySaveSettingsMessage;
    }

    [AllowAnonymous]
    [HttpPost("sendadmmail")]
    public object SendAdmMail(AdminMessageSettingsRequestsDto inDto)
    {
        var studioAdminMessageSettings = _settingsManager.Load<StudioAdminMessageSettings>();
        var enableAdmMess = studioAdminMessageSettings.Enable || _tenantExtra.IsNotPaid();

        if (!enableAdmMess)
        {
            throw new MethodAccessException("Method not available");
        }

        if (!inDto.Email.TestEmailRegex())
        {
            throw new Exception(Resource.ErrorNotCorrectEmail);
        }

        if (string.IsNullOrEmpty(inDto.Message))
        {
            throw new Exception(Resource.ErrorEmptyMessage);
        }

        CheckCache("sendadmmail");

        _studioNotifyService.SendMsgToAdminFromNotAuthUser(inDto.Email, inDto.Message);
        _messageService.Send(MessageAction.ContactAdminMailSent);

        return Resource.AdminMessageSent;
    }

    [AllowAnonymous]
    [HttpPost("sendjoininvite")]
    public async Task<object> SendJoinInviteMail(AdminMessageSettingsRequestsDto inDto)
    {
        try
        {
            var email = inDto.Email;
            if (!(
                (Tenant.TrustedDomainsType == TenantTrustedDomainsType.Custom &&
                Tenant.TrustedDomains.Count > 0) ||
                Tenant.TrustedDomainsType == TenantTrustedDomainsType.All))
            {
                throw new MethodAccessException("Method not available");
            }

            if (!email.TestEmailRegex())
            {
                throw new Exception(Resource.ErrorNotCorrectEmail);
            }

            CheckCache("sendjoininvite");

            var user = _userManager.GetUserByEmail(email);
            if (!user.Id.Equals(Constants.LostUser.Id))
            {
                throw new Exception(_customNamingPeople.Substitute<Resource>("ErrorEmailAlreadyExists"));
            }

            var settings = _settingsManager.Load<IPRestrictionsSettings>();

            if (settings.Enable && !_ipSecurity.Verify())
            {
                throw new Exception(Resource.ErrorAccessRestricted);
            }

            var trustedDomainSettings = _settingsManager.Load<StudioTrustedDomainSettings>();
            var emplType = trustedDomainSettings.InviteAsUsers ? EmployeeType.User : EmployeeType.RoomAdmin;
            if (!_coreBaseSettings.Personal)
            {
                var enableInviteUsers = true;
                try
                {
                    await _countPaidUserChecker.CheckAppend();
                }
                catch (Exception)
                {
                    enableInviteUsers = false;
                }

                if (!enableInviteUsers)
                {
                    emplType = EmployeeType.User;
                }
            }

            switch (Tenant.TrustedDomainsType)
            {
                case TenantTrustedDomainsType.Custom:
                    {
                        var address = new MailAddress(email);
                        if (Tenant.TrustedDomains.Any(d => address.Address.EndsWith("@" + d.Replace("*", ""), StringComparison.InvariantCultureIgnoreCase)))
                        {
                            _studioNotifyService.SendJoinMsg(email, emplType);
                            _messageService.Send(MessageInitiator.System, MessageAction.SentInviteInstructions, email);
                            return Resource.FinishInviteJoinEmailMessage;
                        }

                        throw new Exception(Resource.ErrorEmailDomainNotAllowed);
                    }
                case TenantTrustedDomainsType.All:
                    {
                        _studioNotifyService.SendJoinMsg(email, emplType);
                        _messageService.Send(MessageInitiator.System, MessageAction.SentInviteInstructions, email);
                        return Resource.FinishInviteJoinEmailMessage;
                    }
                default:
                    throw new Exception(Resource.ErrorNotCorrectEmail);
            }
        }
        catch (FormatException)
        {
            throw new Exception(Resource.ErrorNotCorrectEmail);
        }
    }
}
