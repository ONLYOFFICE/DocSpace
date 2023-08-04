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

namespace ASC.Employee.Core.Controllers;

///<summary>
/// Groups API.
///</summary>
///<name>group</name>
[Scope]
[DefaultRoute]
[ApiController]
public class GroupController : ControllerBase
{
    private readonly UserManager _userManager;
    private readonly ApiContext _apiContext;
    private readonly GroupFullDtoHelper _groupFullDtoHelper;
    private readonly MessageService _messageService;
    private readonly MessageTarget _messageTarget;
    private readonly PermissionContext _permissionContext;

    public GroupController(
        UserManager userManager,
        ApiContext apiContext,
        GroupFullDtoHelper groupFullDtoHelper,
        MessageService messageService,
        MessageTarget messageTarget,
        PermissionContext permissionContext)
    {
        _userManager = userManager;
        _apiContext = apiContext;
        _groupFullDtoHelper = groupFullDtoHelper;
        _messageService = messageService;
        _messageTarget = messageTarget;
        _permissionContext = permissionContext;
    }

    /// <summary>
    /// Returns the general information about all the groups, such as group ID and group manager.
    /// </summary>
    /// <short>
    /// Get groups
    /// </short>
    /// <returns type="ASC.Web.Api.Models.GroupSummaryDto, ASC.Api.Core">List of groups</returns>
    /// <remarks>
    /// This method returns partial group information.
    /// </remarks>
    /// <path>api/2.0/group</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet]
    public async Task<IEnumerable<GroupSummaryDto>> GetAllAsync()
    {
        var result = (await _userManager.GetDepartmentsAsync()).Select(r => r);
        if (!string.IsNullOrEmpty(_apiContext.FilterValue))
        {
            result = result.Where(r => r.Name.Contains(_apiContext.FilterValue, StringComparison.InvariantCultureIgnoreCase));
        }

        return result.Select(x => new GroupSummaryDto(x, _userManager));
    }

    /// <summary>
    /// Returns the detailed information about all the groups.
    /// </summary>
    /// <short>
    /// Get groups information
    /// </short>
    /// <returns type="ASC.People.ApiModels.ResponseDto.GroupDto, ASC.People">List of groups with the following parameters</returns>
    /// <remarks>
    /// This method returns full group information.
    /// </remarks>
    /// <path>api/2.0/group/full</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("full")]
    public async IAsyncEnumerable<GroupDto> GetAllWithMembersAsync()
    {
        var result = (await _userManager.GetDepartmentsAsync()).Select(r => r);
        if (!string.IsNullOrEmpty(_apiContext.FilterValue))
        {
            result = result.Where(r => r.Name.Contains(_apiContext.FilterValue, StringComparison.InvariantCultureIgnoreCase));
        }

        foreach (var item in result)
        {
            yield return await _groupFullDtoHelper.Get(item, true);
        }
    }

    /// <summary>
    /// Returns a list of all the groups by the group name specified in the request.
    /// </summary>
    /// <short>
    /// Get groups by a group name
    /// </short>
    /// <param type="System.String, System" name="groupName">Group name</param>
    /// <returns type="ASC.Web.Api.Models.GroupSummaryDto, ASC.Api.Core">List of groups</returns>
    /// <path>api/2.0/group/search</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("search")]
    public async Task<IEnumerable<GroupSummaryDto>> GetTagsByNameAsync(string groupName)
    {
        groupName = (groupName ?? "").Trim();

        if (string.IsNullOrEmpty(groupName))
        {
            return new List<GroupSummaryDto>();
        }

        return (await _userManager.GetDepartmentsAsync())
            .Where(x => x.Name.Contains(groupName))
            .Select(x => new GroupSummaryDto(x, _userManager));
    }

    /// <summary>
    /// Returns the detailed information about the selected group.
    /// </summary>
    /// <short>
    /// Get a group
    /// </short>
    /// <param type="System.Guid, System" method="url" name="groupid">Group ID</param>
    /// <returns type="ASC.People.ApiModels.ResponseDto.GroupDto, ASC.People">Group with the following parameters</returns>
    /// <remarks>
    /// This method returns full group information.
    /// </remarks>
    /// <path>api/2.0/group/{groupid}</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("{groupid}")]
    public async Task<GroupDto> GetById(Guid groupid)
    {
        return await _groupFullDtoHelper.Get(await GetGroupInfoAsync(groupid), true);
    }

    /// <summary>
    /// Returns a list of groups for the user with the ID specified in the request.
    /// </summary>
    /// <short>
    /// Get user groups
    /// </short>
    /// <param type="System.Guid, System" method="url" name="userid">User ID</param>
    /// <returns type="ASC.Web.Api.Models.GroupSummaryDto, ASC.Api.Core">List of groups</returns>
    /// <path>api/2.0/group/user/{userid}</path>
    /// <httpMethod>GET</httpMethod>
    /// <collection>list</collection>
    [HttpGet("user/{userid}")]
    public async Task<IEnumerable<GroupSummaryDto>> GetByUserIdAsync(Guid userid)
    {
        return (await _userManager.GetUserGroupsAsync(userid)).Select(x => new GroupSummaryDto(x, _userManager));
    }

    /// <summary>
    /// Adds a new group with the group manager, name, and members specified in the request.
    /// </summary>
    /// <short>
    /// Add a new group
    /// </short>
    /// <param type="ASC.People.ApiModels.RequestDto.GroupRequestDto, ASC.People" name="inDto">Group request parameters</param>
    /// <returns type="ASC.People.ApiModels.ResponseDto.GroupDto, ASC.People">Newly created group with the following parameters</returns>
    /// <path>api/2.0/group</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost]
    public async Task<GroupDto> AddGroup(GroupRequestDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);

        var group = await _userManager.SaveGroupInfoAsync(new GroupInfo { Name = inDto.GroupName });

        await TransferUserToDepartment(inDto.GroupManager, @group, true);

        if (inDto.Members != null)
        {
            foreach (var member in inDto.Members)
            {
                await TransferUserToDepartment(member, group, false);
            }
        }

        await _messageService.SendAsync(MessageAction.GroupCreated, _messageTarget.Create(group.ID), group.Name);

        return await _groupFullDtoHelper.Get(group, true);
    }

    /// <summary>
    /// Updates the existing group changing the group manager, name, and/or members.
    /// </summary>
    /// <short>
    /// Update a group
    /// </short>
    /// <param type="System.Guid, System" method="url" name="groupid">Group ID</param>
    /// <param type="ASC.People.ApiModels.RequestDto.GroupRequestDto, ASC.People" name="inDto">Group request parameters</param>
    /// <returns type="ASC.People.ApiModels.ResponseDto.GroupDto, ASC.People">Updated group with the following parameters</returns>
    /// <path>api/2.0/group/{groupid}</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("{groupid}")]
    public async Task<GroupDto> UpdateGroup(Guid groupid, GroupRequestDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
        var group = (await _userManager.GetGroupsAsync()).SingleOrDefault(x => x.ID == groupid).NotFoundIfNull("group not found");
        if (groupid == Constants.LostGroupInfo.ID)
        {
            throw new ItemNotFoundException("group not found");
        }

        group.Name = inDto.GroupName ?? group.Name;
        await _userManager.SaveGroupInfoAsync(group);

        await RemoveMembersFrom(groupid, new GroupRequestDto { Members = (await _userManager.GetUsersByGroupAsync(groupid, EmployeeStatus.All)).Select(u => u.Id).Where(id => !inDto.Members.Contains(id)) });

        await TransferUserToDepartment(inDto.GroupManager, @group, true);

        if (inDto.Members != null)
        {
            foreach (var member in inDto.Members)
            {
                await TransferUserToDepartment(member, group, false);
            }
        }

        await _messageService.SendAsync(MessageAction.GroupUpdated, _messageTarget.Create(groupid), group.Name);

        return await GetById(groupid);
    }

    /// <summary>
    /// Deletes a group with the ID specified in the request from the list of groups on the portal.
    /// </summary>
    /// <short>
    /// Delete a group
    /// </short>
    /// <param type="System.Guid, System" method="url" name="groupid">Group ID</param>
    /// <returns type="ASC.People.ApiModels.ResponseDto.GroupDto, ASC.People">Group with the following parameters</returns>
    /// <path>api/2.0/group/{groupid}</path>
    /// <httpMethod>DELETE</httpMethod>
    [HttpDelete("{groupid}")]
    public async Task<GroupDto> DeleteGroup(Guid groupid)
    {
         await _permissionContext.DemandPermissionsAsync(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);

        var @group = await GetGroupInfoAsync(groupid);

        await _userManager.DeleteGroupAsync(groupid);

        await _messageService.SendAsync(MessageAction.GroupDeleted, _messageTarget.Create(group.ID), group.Name);

        return await _groupFullDtoHelper.Get(group, false);
    }

    /// <summary>
    /// Moves all the members from the selected group to another one specified in the request.
    /// </summary>
    /// <short>
    /// Move group members
    /// </short>
    /// <param type="System.Guid, System" method="url" name="groupid">Group ID to move from</param>
    /// <param type="System.Guid, System" method="url" name="newgroupid">Group ID to move to</param>
    /// <returns type="ASC.People.ApiModels.ResponseDto.GroupDto, ASC.People">Group with the following parameters</returns>
    /// <path>api/2.0/group/{groupid}/members/{newgroupid}</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("{groupid}/members/{newgroupid}")]
    public async Task<GroupDto> TransferMembersTo(Guid groupid, Guid newgroupid)
    {
        await _permissionContext.DemandPermissionsAsync(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);

        var oldgroup = await GetGroupInfoAsync(groupid);

        var newgroup = await GetGroupInfoAsync(newgroupid);

        var users = await _userManager.GetUsersByGroupAsync(oldgroup.ID);
        foreach (var userInfo in users)
        {
            await TransferUserToDepartment(userInfo.Id, newgroup, false);
        }

        return await GetById(newgroupid);
    }

    /// <summary>
    /// Replaces the group members with those specified in the request.
    /// </summary>
    /// <short>
    /// Replace group members
    /// </short>
    /// <param type="System.Guid, System" method="url" name="groupid">Group ID</param>
    /// <param type="ASC.People.ApiModels.RequestDto.GroupRequestDto, ASC.People" name="inDto">Group request parameters</param>
    /// <returns type="ASC.People.ApiModels.ResponseDto.GroupDto, ASC.People">Group with the following parameters</returns>
    /// <path>api/2.0/group/{groupid}/members</path>
    /// <httpMethod>POST</httpMethod>
    [HttpPost("{groupid}/members")]
    public async Task<GroupDto> SetMembersTo(Guid groupid, GroupRequestDto inDto)
    {
        await RemoveMembersFrom(groupid, new GroupRequestDto { Members = (await _userManager.GetUsersByGroupAsync(groupid)).Select(x => x.Id) });
        await AddMembersTo(groupid, inDto);

        return await GetById(groupid);
    }

    /// <summary>
    /// Adds new group members to the group with the ID specified in the request.
    /// </summary>
    /// <short>
    /// Add group members
    /// </short>
    /// <param type="System.Guid, System" method="url" name="groupid">Group ID</param>
    /// <param type="ASC.People.ApiModels.RequestDto.GroupRequestDto, ASC.People" name="inDto">Group request parameters</param>
    /// <returns type="ASC.People.ApiModels.ResponseDto.GroupDto, ASC.People">Group with the following parameters</returns>
    /// <path>api/2.0/group/{groupid}/members</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("{groupid}/members")]
    public async Task<GroupDto> AddMembersTo(Guid groupid, GroupRequestDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);

        var group = await GetGroupInfoAsync(groupid);

        foreach (var userId in inDto.Members)
        {
            await TransferUserToDepartment(userId, group, false);
        }

        return await GetById(group.ID);
    }

    /// <summary>
    /// Sets a user with the ID specified in the request as a group manager.
    /// </summary>
    /// <short>
    /// Set a group manager
    /// </short>
    /// <param type="System.Guid, System" method="url" name="groupid">Group ID</param>
    /// <param type="ASC.People.ApiModels.RequestDto.SetManagerRequestDto, ASC.People" name="inDto">Request parameters for setting a group manager</param>
    /// <returns type="ASC.People.ApiModels.ResponseDto.GroupDto, ASC.People">Group with the following parameters</returns>
    /// <path>api/2.0/group/{groupid}/manager</path>
    /// <httpMethod>PUT</httpMethod>
    [HttpPut("{groupid}/manager")]
    public async Task<GroupDto> SetManager(Guid groupid, SetManagerRequestDto inDto)
    {
        var group = await GetGroupInfoAsync(groupid);
        if (await _userManager.UserExistsAsync(inDto.UserId))
        {
            await _userManager.SetDepartmentManagerAsync(group.ID, inDto.UserId);
        }
        else
        {
            throw new ItemNotFoundException("user not found");
        }

        return await GetById(groupid);
    }

    /// <summary>
    /// Removes the group members specified in the request from the selected group.
    /// </summary>
    /// <short>
    /// Remove group members
    /// </short>
    /// <param type="System.Guid, System" method="url" name="groupid">Group ID</param>
    /// <param type="ASC.People.ApiModels.RequestDto.GroupRequestDto, ASC.People" name="inDto">Group request parameters</param>
    /// <returns type="ASC.People.ApiModels.ResponseDto.GroupDto, ASC.People">Group with the following parameters</returns>
    /// <path>api/2.0/group/{groupid}/members</path>
    /// <httpMethod>DELETE</httpMethod>
    [HttpDelete("{groupid}/members")]
    public async Task<GroupDto> RemoveMembersFrom(Guid groupid, GroupRequestDto inDto)
    {
        await _permissionContext.DemandPermissionsAsync(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);

        var group = await GetGroupInfoAsync(groupid);

        foreach (var userId in inDto.Members)
        {
            await RemoveUserFromDepartmentAsync(userId, group);
        }

        return await GetById(group.ID);
    }

    private async Task<GroupInfo> GetGroupInfoAsync(Guid groupid)
    {
        var group = (await _userManager.GetGroupsAsync()).SingleOrDefault(x => x.ID == groupid).NotFoundIfNull("group not found");
        if (group.ID == Constants.LostGroupInfo.ID)
        {
            throw new ItemNotFoundException("group not found");
        }

        return @group;
    }

    private async Task TransferUserToDepartment(Guid userId, GroupInfo group, bool setAsManager)
    {
        if (!await _userManager.UserExistsAsync(userId) && userId != Guid.Empty)
        {
            return;
        }

        if (setAsManager)
        {
            await _userManager.SetDepartmentManagerAsync(@group.ID, userId);
        }
        await _userManager.AddUserIntoGroupAsync(userId, @group.ID);
    }

    private async Task RemoveUserFromDepartmentAsync(Guid userId, GroupInfo @group)
    {
        if (!await _userManager.UserExistsAsync(userId))
        {
            return;
        }

        var user = await _userManager.GetUsersAsync(userId);
        await _userManager.RemoveUserFromGroupAsync(user.Id, @group.ID);
        await _userManager.UpdateUserInfoAsync(user);
    }
}
