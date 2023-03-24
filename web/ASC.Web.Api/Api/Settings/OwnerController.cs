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

public class OwnerController : BaseSettingsController
{
    private readonly MessageService _messageService;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly AuthContext _authContext;
    private readonly PermissionContext _permissionContext;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly MessageTarget _messageTarget;

    public OwnerController(
        MessageService messageService,
        CommonLinkUtility commonLinkUtility,
        StudioNotifyService studioNotifyService,
        ApiContext apiContext,
        UserManager userManager,
        TenantManager tenantManager,
        AuthContext authContext,
        PermissionContext permissionContext,
        WebItemManager webItemManager,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        MessageTarget messageTarget,
        IMemoryCache memoryCache,
        IHttpContextAccessor httpContextAccessor) : base(apiContext, memoryCache, webItemManager, httpContextAccessor)
    {
        _messageService = messageService;
        _commonLinkUtility = commonLinkUtility;
        _studioNotifyService = studioNotifyService;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _authContext = authContext;
        _permissionContext = permissionContext;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _messageTarget = messageTarget;
    }

    [HttpPost("owner")]
    public async Task<object> SendOwnerChangeInstructionsAsync(SettingsRequestsDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(SecutiryConstants.EditPortalSettings);

        var curTenant = await _tenantManager.GetCurrentTenantAsync();
        var owner = await _userManager.GetUsersAsync(curTenant.OwnerId);
        var newOwner = await _userManager.GetUsersAsync(inDto.OwnerId);

        if (await _userManager.IsUserAsync(newOwner))
        {
            throw new SecurityException("Collaborator can not be an owner");
        }

        if (!owner.Id.Equals(_authContext.CurrentAccount.ID) || Guid.Empty.Equals(newOwner.Id))
        {
            return new { Status = 0, Message = Resource.ErrorAccessDenied };
        }

        var confirmLink = await _commonLinkUtility.GetConfirmationEmailUrlAsync(owner.Email, ConfirmType.PortalOwnerChange, newOwner.Id, newOwner.Id);
        await _studioNotifyService.SendMsgConfirmChangeOwnerAsync(owner, newOwner, confirmLink);

        await _messageService.SendAsync(MessageAction.OwnerSentChangeOwnerInstructions, _messageTarget.Create(owner.Id), owner.DisplayUserName(false, _displayUserSettingsHelper));

        var emailLink = $"<a href=\"mailto:{owner.Email}\">{owner.Email}</a>";
        return new { Status = 1, Message = Resource.ChangePortalOwnerMsg.Replace(":email", emailLink) };
    }

    [HttpPut("owner")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "PortalOwnerChange")]
    public async Task OwnerAsync(SettingsRequestsDto inDto)
    {
        var newOwner = Constants.LostUser;
        try
        {
            newOwner = await _userManager.GetUsersAsync(inDto.OwnerId);
        }
        catch
        {
        }
        if (Constants.LostUser.Equals(newOwner))
        {
            throw new Exception(Resource.ErrorUserNotFound);
        }

        if (await _userManager.IsUserInGroupAsync(newOwner.Id, Constants.GroupUser.ID))
        {
            throw new Exception(Resource.ErrorUserNotFound);
        }

        var curTenant = await _tenantManager.GetCurrentTenantAsync();
        curTenant.OwnerId = newOwner.Id;
        await _tenantManager.SaveTenantAsync(curTenant);

        await _messageService.SendAsync(MessageAction.OwnerUpdated, newOwner.DisplayUserName(false, _displayUserSettingsHelper));
    }
}
