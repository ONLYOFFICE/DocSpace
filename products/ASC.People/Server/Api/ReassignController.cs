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

namespace ASC.People.Api;

public class ReassignController : ApiControllerBase
{
    private Tenant Tenant => _apiContext.Tenant;

    private readonly PermissionContext _permissionContext;
    private readonly QueueWorkerReassign _queueWorkerReassign;
    private readonly UserManager _userManager;
    private readonly AuthContext _authContext;
    private readonly ApiContext _apiContext;
    private readonly SecurityContext _securityContext;

    public ReassignController(
        PermissionContext permissionContext,
        QueueWorkerReassign queueWorkerReassign,
        UserManager userManager,
        AuthContext authContext,
        ApiContext apiContext,
        SecurityContext securityContext)
    {
        _permissionContext = permissionContext;
        _queueWorkerReassign = queueWorkerReassign;
        _userManager = userManager;
        _authContext = authContext;
        _apiContext = apiContext;
        _securityContext = securityContext;
    }

    [HttpGet("reassign/progress")]
    public async Task<ReassignProgressItem> GetReassignProgressAsync(Guid userId)
    {
        await _permissionContext.DemandPermissionsAsync(Constants.Action_EditUser);

        return _queueWorkerReassign.GetProgressItemStatus(Tenant.Id, userId);
    }

    [HttpPost("reassign/start")]
    public async Task<ReassignProgressItem> StartReassignAsync(StartReassignRequestDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(Constants.Action_EditUser);

        var fromUser = await _userManager.GetUsersAsync(inDto.FromUserId);

        if (fromUser == null || fromUser.Id == Constants.LostUser.Id)
        {
            throw new ArgumentException("User with id = " + inDto.FromUserId + " not found");
        }

        if (fromUser.IsOwner(Tenant) || fromUser.IsMe(_authContext) || fromUser.Status != EmployeeStatus.Terminated)
        {
            throw new ArgumentException("Can not delete user with id = " + inDto.FromUserId);
        }

        var toUser = await _userManager.GetUsersAsync(inDto.ToUserId);

        if (toUser == null || toUser.Id == Constants.LostUser.Id)
        {
            throw new ArgumentException("User with id = " + inDto.ToUserId + " not found");
        }

        if (await _userManager.IsUserAsync(toUser) || toUser.Status == EmployeeStatus.Terminated)
        {
            throw new ArgumentException("Can not reassign data to user with id = " + inDto.ToUserId);
        }

        return _queueWorkerReassign.Start(Tenant.Id, inDto.FromUserId, inDto.ToUserId, _securityContext.CurrentAccount.ID, inDto.DeleteProfile);
    }

    [HttpPut("reassign/terminate")]
    public async Task TerminateReassignAsync(TerminateRequestDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(Constants.Action_EditUser);

        _queueWorkerReassign.Terminate(Tenant.Id, inDto.UserId);
    }
}
