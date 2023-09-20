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

    /// <summary>
    /// Returns the progress of the started data reassignment for the user with the ID specified in the request.
    /// </summary>
    /// <short>Get the reassignment progress</short>
    /// <param type="System.Guid, System" name="userId">User ID whose data is reassigned</param>
    /// <category>User data</category>
    /// <returns type="ASC.People.ApiModels.ResponseDto.TaskProgressResponseDto, ASC.People">Reassignment progress</returns>
    /// <path>api/2.0/people/reassign/progress/{userid}</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("reassign/progress/{userid}")]
    public async Task<TaskProgressResponseDto> GetReassignProgressAsync(Guid userId)
    {
        await _permissionContext.DemandPermissionsAsync(Constants.Action_EditUser);

        var progressItem = _queueWorkerReassign.GetProgressItemStatus(Tenant.Id, userId);

        return TaskProgressResponseDto.Get(progressItem);
    }

    /// <summary>
    /// Starts the data reassignment for the user with the ID specified in the request.
    /// </summary>
    /// <short>Start the data reassignment</short>
    /// <param type="ASC.People.ApiModels.RequestDto.StartReassignRequestDto, ASC.People" name="inDto">Request parameters for starting the reassignment process</param>
    /// <category>User data</category>
    /// <returns type="ASC.People.ApiModels.ResponseDto.TaskProgressResponseDto, ASC.People">Reassignment progress</returns>
    /// <path>api/2.0/people/reassign/start</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("reassign/start")]
    public async Task<TaskProgressResponseDto> StartReassignAsync(StartReassignRequestDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(Constants.Action_EditUser);

        var toUser = await _userManager.GetUsersAsync(inDto.ToUserId);

        if (_userManager.IsSystemUser(toUser.Id)
            || await _userManager.IsUserAsync(toUser)
            || toUser.Status == EmployeeStatus.Terminated)
        {
            throw new ArgumentException("Can not reassign data to user with id = " + toUser.Id);
        }

        var fromUser = await _userManager.GetUsersAsync(inDto.FromUserId);

        if (_userManager.IsSystemUser(fromUser.Id)
            || fromUser.IsOwner(Tenant)
            || fromUser.IsMe(_authContext)
            || await _userManager.IsUserAsync(toUser)
            || fromUser.Status != EmployeeStatus.Terminated)
        {
            throw new ArgumentException("Can not reassign data from user with id = " + fromUser.Id);
        }

        var progressItem = _queueWorkerReassign.Start(Tenant.Id, fromUser.Id, toUser.Id, _securityContext.CurrentAccount.ID, true, inDto.DeleteProfile);

        return TaskProgressResponseDto.Get(progressItem);
    }

    /// <summary>
    /// Terminates the data reassignment for the user with the ID specified in the request.
    /// </summary>
    /// <short>Terminate the data reassignment</short>
    /// <param type="ASC.People.ApiModels.RequestDto.TerminateRequestDto, ASC.People" name="inDto">Request parameters for terminating the reassignment process</param>
    /// <category>User data</category>
    /// <path>api/2.0/people/reassign/terminate</path>
    /// <httpMethod>PUT</httpMethod>
    /// <returns></returns>
    [HttpPut("reassign/terminate")]
    public async Task<TaskProgressResponseDto> TerminateReassignAsync(TerminateRequestDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(Constants.Action_EditUser);

        var progressItem = _queueWorkerReassign.GetProgressItemStatus(Tenant.Id, inDto.UserId);

        if (progressItem != null)
        {
            _queueWorkerReassign.Terminate(Tenant.Id, inDto.UserId);

            progressItem.Status = DistributedTaskStatus.Canceled;
            progressItem.IsCompleted = true;
        }

        return TaskProgressResponseDto.Get(progressItem);
    }
}
