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

public class RemoveUserDataController : ApiControllerBase
{
    private Tenant Tenant => _apiContext.Tenant;

    private readonly PermissionContext _permissionContext;
    private readonly UserManager _userManager;
    private readonly QueueWorkerRemove _queueWorkerRemove;
    private readonly SecurityContext _securityContext;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly MessageService _messageService;
    private readonly AuthContext _authContext;
    private readonly ApiContext _apiContext;

    public RemoveUserDataController(
        PermissionContext permissionContext,
        UserManager userManager,
        QueueWorkerRemove queueWorkerRemove,
        SecurityContext securityContext,
        StudioNotifyService studioNotifyService,
        MessageService messageService,
        AuthContext authContext,
        ApiContext apiContext)
    {
        _permissionContext = permissionContext;
        _userManager = userManager;
        _queueWorkerRemove = queueWorkerRemove;
        _securityContext = securityContext;
        _studioNotifyService = studioNotifyService;
        _messageService = messageService;
        _authContext = authContext;
        _apiContext = apiContext;
    }

    /// <summary>
    /// Returns the progress of the started data deletion for the user with the ID specified in the request.
    /// </summary>
    /// <short>Get the deletion progress</short>
    /// <param type="System.Guid, System" name="userId">User ID</param>
    /// <category>User data</category>
    /// <returns type="ASC.People.ApiModels.ResponseDto.TaskProgressResponseDto, ASC.People">Deletion progress</returns>
    /// <path>api/2.0/people/remove/progress/{userid}</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("remove/progress/{userid}")]
    public async Task<TaskProgressResponseDto> GetRemoveProgressAsync(Guid userId)
    {
        await _permissionContext.DemandPermissionsAsync(Constants.Action_EditUser);

        var progressItem = _queueWorkerRemove.GetProgressItemStatus(Tenant.Id, userId);

        return TaskProgressResponseDto.Get(progressItem);
    }

    /// <summary>
    /// Sends instructions for deleting a user profile.
    /// </summary>
    /// <short>
    /// Send the deletion instructions
    /// </short>
    /// <category>Profiles</category>
    /// <returns type="System.Object, System">Information message</returns>
    /// <path>api/2.0/people/self/delete</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("self/delete")]
    public async Task<object> SendInstructionsToDeleteAsync()
    {
        var user = await _userManager.GetUsersAsync(_securityContext.CurrentAccount.ID);

        if (user.IsLDAP() || user.IsOwner(Tenant))
        {
            throw new SecurityException();
        }

        await _studioNotifyService.SendMsgProfileDeletionAsync(user);
        await _messageService.SendAsync(MessageAction.UserSentDeleteInstructions);

        return string.Format(Resource.SuccessfullySentNotificationDeleteUserInfoMessage, "<b>" + user.Email + "</b>");
    }

    /// <summary>
    /// Starts the data deletion for the user with the ID specified in the request.
    /// </summary>
    /// <short>Start the data deletion</short>
    /// <param type="ASC.People.ApiModels.RequestDto.TerminateRequestDto, ASC.People" name="inDto">Request parameters for starting the deletion process</param>
    /// <category>User data</category>
    /// <returns type="ASC.People.ApiModels.ResponseDto.TaskProgressResponseDto, ASC.People">Deletion progress</returns>
    /// <path>api/2.0/people/remove/start</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("remove/start")]
    public async Task<TaskProgressResponseDto> StartRemoveAsync(TerminateRequestDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(Constants.Action_EditUser);

        var user = await _userManager.GetUsersAsync(inDto.UserId);

        if (user == null || user.Id == Constants.LostUser.Id)
        {
            throw new ArgumentException("User with id = " + inDto.UserId + " not found");
        }

        if (user.IsOwner(Tenant) || user.IsMe(_authContext) || user.Status != EmployeeStatus.Terminated)
        {
            throw new ArgumentException("Can not delete user with id = " + inDto.UserId);
        }

        var progressItem = _queueWorkerRemove.Start(Tenant.Id, user, _securityContext.CurrentAccount.ID, true, true);

        return TaskProgressResponseDto.Get(progressItem);
    }

    /// <summary>
    /// Terminates the data deletion for the user with the ID specified in the request.
    /// </summary>
    /// <short>Terminate the data deletion</short>
    /// <param type="ASC.People.ApiModels.RequestDto.TerminateRequestDto, ASC.People" name="inDto">Request parameters for terminating the deletion process</param>
    /// <category>User data</category>
    /// <path>api/2.0/people/remove/terminate</path>
    /// <httpMethod>PUT</httpMethod>
    /// <returns></returns>
    [HttpPut("remove/terminate")]
    public async Task TerminateRemoveAsync(TerminateRequestDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(Constants.Action_EditUser);

        _queueWorkerRemove.Terminate(Tenant.Id, inDto.UserId);
    }
}
