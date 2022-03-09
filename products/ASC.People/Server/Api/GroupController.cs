namespace ASC.Employee.Core.Controllers;

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

    [Read]
    public IEnumerable<GroupSummaryDto> GetAll()
    {
        var result = _userManager.GetDepartments().Select(r => r);
        if (!string.IsNullOrEmpty(_apiContext.FilterValue))
        {
            result = result.Where(r => r.Name.Contains(_apiContext.FilterValue, StringComparison.InvariantCultureIgnoreCase));
        }

        return result.Select(x => new GroupSummaryDto(x, _userManager));
    }

    [Read("full")]
    public IEnumerable<GroupDto> GetAllWithMembers()
    {
        var result = _userManager.GetDepartments().Select(r => r);
        if (!string.IsNullOrEmpty(_apiContext.FilterValue))
        {
            result = result.Where(r => r.Name.Contains(_apiContext.FilterValue, StringComparison.InvariantCultureIgnoreCase));
        }

        return result.Select(r => _groupFullDtoHelper.Get(r, true));
    }

    [Read("{groupid}")]
    public GroupDto GetById(Guid groupid)
    {
        return _groupFullDtoHelper.Get(GetGroupInfo(groupid), true);
    }

    [Read("user/{userid}")]
    public IEnumerable<GroupSummaryDto> GetByUserId(Guid userid)
    {
        return _userManager.GetUserGroups(userid).Select(x => new GroupSummaryDto(x, _userManager));
    }

    [Create]
    public GroupDto AddGroupFromBody([FromBody] GroupRequestDto inDto)
    {
        return AddGroup(inDto);
    }

    [Create]
    [Consumes("application/x-www-form-urlencoded")]
    public GroupDto AddGroupFromForm([FromForm] GroupRequestDto inDto)
    {
        return AddGroup(inDto);
    }

    [Update("{groupid}")]
    public GroupDto UpdateGroupFromBody(Guid groupid, [FromBody] GroupRequestDto inDto)
    {
        return UpdateGroup(groupid, inDto);
    }

    [Update("{groupid}")]
    [Consumes("application/x-www-form-urlencoded")]
    public GroupDto UpdateGroupFromForm(Guid groupid, [FromForm] GroupRequestDto inDto)
    {
        return UpdateGroup(groupid, inDto);
    }

    [Delete("{groupid}")]
    public GroupDto DeleteGroup(Guid groupid)
    {
        _permissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);

        var @group = GetGroupInfo(groupid);
        var groupWrapperFull = _groupFullDtoHelper.Get(group, false);

       _userManager.DeleteGroup(groupid);

       _messageService.Send(MessageAction.GroupDeleted, _messageTarget.Create(group.ID), group.Name);

       return groupWrapperFull;
    }

    [Update("{groupid}/members/{newgroupid}")]
    public GroupDto TransferMembersTo(Guid groupid, Guid newgroupid)
    {
        _permissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);

        var oldgroup = GetGroupInfo(groupid);

        var newgroup = GetGroupInfo(newgroupid);

        var users = _userManager.GetUsersByGroup(oldgroup.ID);
        foreach (var userInfo in users)
        {
            TransferUserToDepartment(userInfo.Id, newgroup, false);
        }

        return GetById(newgroupid);
    }

    [Create("{groupid}/members")]
    public GroupDto SetMembersToFromBody(Guid groupid, [FromBody] GroupRequestDto inDto)
    {
        return SetMembersTo(groupid, inDto);
    }

    [Create("{groupid}/members")]
    [Consumes("application/x-www-form-urlencoded")]
    public GroupDto SetMembersToFromForm(Guid groupid, [FromForm] GroupRequestDto inDto)
    {
        return SetMembersTo(groupid, inDto);
    }

    [Update("{groupid}/members")]
    public GroupDto AddMembersToFromBody(Guid groupid, [FromBody] GroupRequestDto inDto)
    {
        return AddMembersTo(groupid, inDto);
    }

    [Update("{groupid}/members")]
    [Consumes("application/x-www-form-urlencoded")]
    public GroupDto AddMembersToFromForm(Guid groupid, [FromForm] GroupRequestDto inDto)
    {
        return AddMembersTo(groupid, inDto);
    }

    [Update("{groupid}/manager")]
    public GroupDto SetManagerFromBody(Guid groupid, [FromBody] SetManagerRequestDto inDto)
    {
        return SetManager(groupid, inDto);
    }

    [Update("{groupid}/manager")]
    [Consumes("application/x-www-form-urlencoded")]
    public GroupDto SetManagerFromForm(Guid groupid, [FromForm] SetManagerRequestDto inDto)
    {
        return SetManager(groupid, inDto);
    }

    [Delete("{groupid}/members")]
    public GroupDto RemoveMembersFromFromBody(Guid groupid, [FromBody] GroupRequestDto inDto)
    {
        return RemoveMembersFrom(groupid, inDto);
    }

    [Delete("{groupid}/members")]
    [Consumes("application/x-www-form-urlencoded")]
    public GroupDto RemoveMembersFromFromForm(Guid groupid, [FromForm] GroupRequestDto inDto)
    {
        return RemoveMembersFrom(groupid, inDto);
    }

    private GroupInfo GetGroupInfo(Guid groupid)
    {
        var group = _userManager.GetGroups().SingleOrDefault(x => x.ID == groupid).NotFoundIfNull("group not found");
        if (group.ID == Constants.LostGroupInfo.ID)
        {
            throw new ItemNotFoundException("group not found");
        }

        return @group;
    }

    private GroupDto AddGroup(GroupRequestDto inDto)
    {
        _permissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);

        var group = _userManager.SaveGroupInfo(new GroupInfo { Name = inDto.GroupName });

        TransferUserToDepartment(inDto.GroupManager, @group, true);

        if (inDto.Members != null)
        {
            foreach (var member in inDto.Members)
            {
                TransferUserToDepartment(member, group, false);
            }
        }

        _messageService.Send(MessageAction.GroupCreated, _messageTarget.Create(group.ID), group.Name);

        return _groupFullDtoHelper.Get(group, true);
    }

    private GroupDto UpdateGroup(Guid groupid, GroupRequestDto inDto)
    {
        _permissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
        var group = _userManager.GetGroups().SingleOrDefault(x => x.ID == groupid).NotFoundIfNull("group not found");
        if (groupid == Constants.LostGroupInfo.ID)
        {
            throw new ItemNotFoundException("group not found");
        }

        group.Name = inDto.GroupName ?? group.Name;
        _userManager.SaveGroupInfo(group);

        RemoveMembersFrom(groupid, new GroupRequestDto { Members = _userManager.GetUsersByGroup(groupid, EmployeeStatus.All).Select(u => u.Id).Where(id => !inDto.Members.Contains(id)) });

        TransferUserToDepartment(inDto.GroupManager, @group, true);

        if (inDto.Members != null)
        {
            foreach (var member in inDto.Members)
            {
                TransferUserToDepartment(member, group, false);
            }
        }

        _messageService.Send(MessageAction.GroupUpdated, _messageTarget.Create(groupid), group.Name);

        return GetById(groupid);
    }

    private void TransferUserToDepartment(Guid userId, GroupInfo group, bool setAsManager)
    {
        if (!_userManager.UserExists(userId) && userId != Guid.Empty)
        {
            return;
        }

        if (setAsManager)
        {
            _userManager.SetDepartmentManager(@group.ID, userId);
        }
        _userManager.AddUserIntoGroup(userId, @group.ID);
    }

    private GroupDto SetMembersTo(Guid groupid, GroupRequestDto inDto)
    {
        RemoveMembersFrom(groupid, new GroupRequestDto { Members = _userManager.GetUsersByGroup(groupid).Select(x => x.Id) });
        AddMembersTo(groupid, inDto);

        return GetById(groupid);
    }

    private GroupDto RemoveMembersFrom(Guid groupid, GroupRequestDto inDto)
    {
        _permissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);

        var group = GetGroupInfo(groupid);

        foreach (var userId in inDto.Members)
        {
            RemoveUserFromDepartment(userId, group);
        }

        return GetById(group.ID);
    }

    private GroupDto AddMembersTo(Guid groupid, GroupRequestDto inDto)
    {
        _permissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);

        var group = GetGroupInfo(groupid);

        foreach (var userId in inDto.Members)
        {
            TransferUserToDepartment(userId, group, false);
        }

        return GetById(group.ID);
    }

    private GroupDto SetManager(Guid groupid, SetManagerRequestDto inDto)
    {
        var group = GetGroupInfo(groupid);
        if (_userManager.UserExists(inDto.UserId))
        {
            _userManager.SetDepartmentManager(group.ID, inDto.UserId);
        }
        else
        {
            throw new ItemNotFoundException("user not found");
        }

        return GetById(groupid);
    }

    private void RemoveUserFromDepartment(Guid userId, GroupInfo @group)
    {
        if (!_userManager.UserExists(userId))
        {
            return;
        }

        var user = _userManager.GetUsers(userId);
        _userManager.RemoveUserFromGroup(user.Id, @group.ID);
        _userManager.SaveUserInfo(user);
    }
}
