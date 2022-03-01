namespace ASC.People.ApiHelpers;

[Scope]
public class GroupControllerEngine
{
    private readonly ApiContext _apiContext;
    private readonly MessageService _messageService;
    private readonly UserManager _userManager;
    private readonly PermissionContext _permissionContext;
    private readonly MessageTarget _messageTarget;
    private readonly GroupFullDtoHelper _groupWraperFullHelper;

    public GroupControllerEngine(
        ApiContext apiContext,
        MessageService messageService,
        UserManager userManager,
        PermissionContext permissionContext,
        MessageTarget messageTarget,
        GroupFullDtoHelper groupWraperFullHelper)
    {
        _apiContext = apiContext;
        _messageService = messageService;
        _userManager = userManager;
        _permissionContext = permissionContext;
        _messageTarget = messageTarget;
        _groupWraperFullHelper = groupWraperFullHelper;
    }

    public IEnumerable<GroupSummaryDto> GetAll()
    {
        var result = _userManager.GetDepartments().Select(r => r);
        if (!string.IsNullOrEmpty(_apiContext.FilterValue))
        {
            result = result.Where(r => r.Name.Contains(_apiContext.FilterValue, StringComparison.InvariantCultureIgnoreCase));
        }

        return result.Select(x => new GroupSummaryDto(x, _userManager));
    }

    public IEnumerable<GroupDto> GetAllWithMembers()
    {
        var result = _userManager.GetDepartments().Select(r => r);
        if (!string.IsNullOrEmpty(_apiContext.FilterValue))
        {
            result = result.Where(r => r.Name.Contains(_apiContext.FilterValue, StringComparison.InvariantCultureIgnoreCase));
        }

        return result.Select(r => _groupWraperFullHelper.Get(r, true));
    }

    public GroupDto GetById(Guid groupid)
    {
        return _groupWraperFullHelper.Get(GetGroupInfo(groupid), true);
    }

    public IEnumerable<GroupSummaryDto> GetByUserId(Guid userid)
    {
        return _userManager.GetUserGroups(userid).Select(x => new GroupSummaryDto(x, _userManager));
    }

    public GroupDto AddGroup(GroupRequestDto groupModel)
    {
        _permissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);

        var group = _userManager.SaveGroupInfo(new GroupInfo { Name = groupModel.GroupName });

        TransferUserToDepartment(groupModel.GroupManager, @group, true);
        if (groupModel.Members != null)
        {
            foreach (var member in groupModel.Members)
            {
                TransferUserToDepartment(member, group, false);
            }
        }

        _messageService.Send(MessageAction.GroupCreated, _messageTarget.Create(group.ID), group.Name);

        return _groupWraperFullHelper.Get(group, true);
    }

    public GroupDto UpdateGroup(Guid groupid, GroupRequestDto groupModel)
    {
        _permissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
        var group = _userManager.GetGroups().SingleOrDefault(x => x.ID == groupid).NotFoundIfNull("group not found");
        if (groupid == Constants.LostGroupInfo.ID)
        {
            throw new ItemNotFoundException("group not found");
        }

        group.Name = groupModel.GroupName ?? group.Name;
        _userManager.SaveGroupInfo(group);

        RemoveMembersFrom(groupid, new GroupRequestDto { Members = _userManager.GetUsersByGroup(groupid, EmployeeStatus.All).Select(u => u.ID).Where(id => !groupModel.Members.Contains(id)) });

        TransferUserToDepartment(groupModel.GroupManager, @group, true);
        if (groupModel.Members != null)
        {
            foreach (var member in groupModel.Members)
            {
                TransferUserToDepartment(member, group, false);
            }
        }

        _messageService.Send(MessageAction.GroupUpdated, _messageTarget.Create(groupid), group.Name);

        return GetById(groupid);
    }

    public GroupDto DeleteGroup(Guid groupid)
    {
        _permissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
        var @group = GetGroupInfo(groupid);
        var groupWrapperFull = _groupWraperFullHelper.Get(group, false);

        _userManager.DeleteGroup(groupid);

        _messageService.Send(MessageAction.GroupDeleted, _messageTarget.Create(group.ID), group.Name);

        return groupWrapperFull;
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

    public GroupDto TransferMembersTo(Guid groupid, Guid newgroupid)
    {
        _permissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
        var oldgroup = GetGroupInfo(groupid);

        var newgroup = GetGroupInfo(newgroupid);

        var users = _userManager.GetUsersByGroup(oldgroup.ID);
        foreach (var userInfo in users)
        {
            TransferUserToDepartment(userInfo.ID, newgroup, false);
        }

        return GetById(newgroupid);
    }

    public GroupDto SetMembersTo(Guid groupid, GroupRequestDto groupModel)
    {
        RemoveMembersFrom(groupid, new GroupRequestDto { Members = _userManager.GetUsersByGroup(groupid).Select(x => x.ID) });
        AddMembersTo(groupid, groupModel);

        return GetById(groupid);
    }

    public GroupDto AddMembersTo(Guid groupid, GroupRequestDto groupModel)
    {
        _permissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
        var group = GetGroupInfo(groupid);

        foreach (var userId in groupModel.Members)
        {
            TransferUserToDepartment(userId, group, false);
        }

        return GetById(group.ID);
    }

    public GroupDto SetManager(Guid groupid, SetManagerRequestDto setManagerModel)
    {
        var group = GetGroupInfo(groupid);
        if (_userManager.UserExists(setManagerModel.UserId))
        {
            _userManager.SetDepartmentManager(group.ID, setManagerModel.UserId);
        }
        else
        {
            throw new ItemNotFoundException("user not found");
        }

        return GetById(groupid);
    }

    public GroupDto RemoveMembersFrom(Guid groupid, GroupRequestDto groupModel)
    {
        _permissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
        var group = GetGroupInfo(groupid);

        foreach (var userId in groupModel.Members)
        {
            RemoveUserFromDepartment(userId, group);
        }

        return GetById(group.ID);
    }

    private void RemoveUserFromDepartment(Guid userId, GroupInfo @group)
    {
        if (!_userManager.UserExists(userId))
        {
            return;
        }

        var user = _userManager.GetUsers(userId);
        _userManager.RemoveUserFromGroup(user.ID, @group.ID);
        _userManager.SaveUserInfo(user);
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
}