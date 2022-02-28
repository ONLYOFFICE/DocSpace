namespace ASC.Employee.Core.Controllers
{
    [Scope]
    [DefaultRoute]
    [ApiController]
    public class GroupController : ControllerBase
    {
        public ApiContext ApiContext { get; }
        private MessageService MessageService { get; }

        private UserManager UserManager { get; }
        private PermissionContext PermissionContext { get; }
        private MessageTarget MessageTarget { get; }
        private GroupWraperFullHelper GroupWraperFullHelper { get; }

        public GroupController(
            ApiContext apiContext,
            MessageService messageService,
            UserManager userManager,
            PermissionContext permissionContext,
            MessageTarget messageTarget,
            GroupWraperFullHelper groupWraperFullHelper)
        {
            ApiContext = apiContext;
            MessageService = messageService;
            UserManager = userManager;
            PermissionContext = permissionContext;
            MessageTarget = messageTarget;
            GroupWraperFullHelper = groupWraperFullHelper;
        }

        [Read]
        public IEnumerable<GroupSummaryDto> GetAll()
        {
            var result = UserManager.GetDepartments().Select(r => r);
            if (!string.IsNullOrEmpty(ApiContext.FilterValue))
            {
                result = result.Where(r => r.Name.Contains(ApiContext.FilterValue, StringComparison.InvariantCultureIgnoreCase));
            }
            return result.Select(x => new GroupSummaryDto(x, UserManager));
        }

        [Read("full")]
        public IEnumerable<GroupDto> GetAllWithMembers()
        {
            var result = UserManager.GetDepartments().Select(r => r);
            if (!string.IsNullOrEmpty(ApiContext.FilterValue))
            {
                result = result.Where(r => r.Name.Contains(ApiContext.FilterValue, StringComparison.InvariantCultureIgnoreCase));
            }
            return result.Select(r=> GroupWraperFullHelper.Get(r, true));
        }

        [Read("{groupid}")]
        public GroupDto GetById(Guid groupid)
        {
            return GroupWraperFullHelper.Get(GetGroupInfo(groupid), true);
        }

        [Read("user/{userid}")]
        public IEnumerable<GroupSummaryDto> GetByUserId(Guid userid)
        {
            return UserManager.GetUserGroups(userid).Select(x => new GroupSummaryDto(x, UserManager));
        }

        [Create]
        public GroupDto AddGroupFromBody([FromBody]GroupRequestDto groupModel)
        {
            return AddGroup(groupModel);
        }

        [Create]
        [Consumes("application/x-www-form-urlencoded")]
        public GroupDto AddGroupFromForm([FromForm] GroupRequestDto groupModel)
        {
            return AddGroup(groupModel);
        }

        private GroupDto AddGroup(GroupRequestDto groupModel)
        {
            PermissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);

            var group = UserManager.SaveGroupInfo(new GroupInfo { Name = groupModel.GroupName });

            TransferUserToDepartment(groupModel.GroupManager, @group, true);
            if (groupModel.Members != null)
            {
                foreach (var member in groupModel.Members)
                {
                    TransferUserToDepartment(member, group, false);
                }
            }

            MessageService.Send(MessageAction.GroupCreated, MessageTarget.Create(group.ID), group.Name);

            return GroupWraperFullHelper.Get(group, true);
        }

        [Update("{groupid}")]
        public GroupDto UpdateGroupFromBody(Guid groupid, [FromBody]GroupRequestDto groupModel)
        {
            return UpdateGroup(groupid, groupModel);
        }

        [Update("{groupid}")]
        [Consumes("application/x-www-form-urlencoded")]
        public GroupDto UpdateGroupFromForm(Guid groupid, [FromForm] GroupRequestDto groupModel)
        {
            return UpdateGroup(groupid, groupModel);
        }

        private GroupDto UpdateGroup(Guid groupid, GroupRequestDto groupModel)
        {
            PermissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
            var group = UserManager.GetGroups().SingleOrDefault(x => x.ID == groupid).NotFoundIfNull("group not found");
            if (groupid == Constants.LostGroupInfo.ID)
            {
                throw new ItemNotFoundException("group not found");
            }

            group.Name = groupModel.GroupName ?? group.Name;
            UserManager.SaveGroupInfo(group);

            RemoveMembersFrom(groupid, new GroupRequestDto {Members = UserManager.GetUsersByGroup(groupid, EmployeeStatus.All).Select(u => u.ID).Where(id => !groupModel.Members.Contains(id)) });

            TransferUserToDepartment(groupModel.GroupManager, @group, true);
            if (groupModel.Members != null)
            {
                foreach (var member in groupModel.Members)
                {
                    TransferUserToDepartment(member, group, false);
                }
            }

            MessageService.Send(MessageAction.GroupUpdated, MessageTarget.Create(groupid), group.Name);

            return GetById(groupid);
        }

        [Delete("{groupid}")]
        public GroupDto DeleteGroup(Guid groupid)
        {
            PermissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
            var @group = GetGroupInfo(groupid);
            var groupWrapperFull = GroupWraperFullHelper.Get(group, false);

            UserManager.DeleteGroup(groupid);

            MessageService.Send(MessageAction.GroupDeleted, MessageTarget.Create(group.ID), group.Name);

            return groupWrapperFull;
        }

        private GroupInfo GetGroupInfo(Guid groupid)
        {
            var group = UserManager.GetGroups().SingleOrDefault(x => x.ID == groupid).NotFoundIfNull("group not found");
            if (group.ID == Constants.LostGroupInfo.ID)
                throw new ItemNotFoundException("group not found");
            return @group;
        }

        [Update("{groupid}/members/{newgroupid}")]
        public GroupDto TransferMembersTo(Guid groupid, Guid newgroupid)
        {
            PermissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
            var oldgroup = GetGroupInfo(groupid);

            var newgroup = GetGroupInfo(newgroupid);

            var users = UserManager.GetUsersByGroup(oldgroup.ID);
            foreach (var userInfo in users)
            {
                TransferUserToDepartment(userInfo.ID, newgroup, false);
            }
            return GetById(newgroupid);
        }

        [Create("{groupid}/members")]
        public GroupDto SetMembersToFromBody(Guid groupid, [FromBody]GroupRequestDto groupModel)
        {
            return SetMembersTo(groupid, groupModel);
        }

        [Create("{groupid}/members")]
        [Consumes("application/x-www-form-urlencoded")]
        public GroupDto SetMembersToFromForm(Guid groupid, [FromForm] GroupRequestDto groupModel)
        {
            return SetMembersTo(groupid, groupModel);
        }

        private GroupDto SetMembersTo(Guid groupid, GroupRequestDto groupModel)
        {
            RemoveMembersFrom(groupid, new GroupRequestDto {Members = UserManager.GetUsersByGroup(groupid).Select(x => x.ID) });
            AddMembersTo(groupid, groupModel);
            return GetById(groupid);
        }

        [Update("{groupid}/members")]
        public GroupDto AddMembersToFromBody(Guid groupid, [FromBody]GroupRequestDto groupModel)
        {
            return AddMembersTo(groupid, groupModel);
        }

        [Update("{groupid}/members")]
        [Consumes("application/x-www-form-urlencoded")]
        public GroupDto AddMembersToFromForm(Guid groupid, [FromForm] GroupRequestDto groupModel)
        {
            return AddMembersTo(groupid, groupModel);
        }

        private GroupDto AddMembersTo(Guid groupid, GroupRequestDto groupModel)
        {
            PermissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
            var group = GetGroupInfo(groupid);

            foreach (var userId in groupModel.Members)
            {
                TransferUserToDepartment(userId, group, false);
            }
            return GetById(group.ID);
        }

        [Update("{groupid}/manager")]
        public GroupDto SetManagerFromBody(Guid groupid, [FromBody]SetManagerRequestDto setManagerModel)
        {
            return SetManager(groupid, setManagerModel);
        }

        [Update("{groupid}/manager")]
        [Consumes("application/x-www-form-urlencoded")]
        public GroupDto SetManagerFromForm(Guid groupid, [FromForm] SetManagerRequestDto setManagerModel)
        {
            return SetManager(groupid, setManagerModel);
        }

        private GroupDto SetManager(Guid groupid, SetManagerRequestDto setManagerModel)
        {
            var group = GetGroupInfo(groupid);
            if (UserManager.UserExists(setManagerModel.UserId))
            {
                UserManager.SetDepartmentManager(group.ID, setManagerModel.UserId);
            }
            else
            {
                throw new ItemNotFoundException("user not found");
            }
            return GetById(groupid);
        }

        [Delete("{groupid}/members")]
        public GroupDto RemoveMembersFromFromBody(Guid groupid, [FromBody]GroupRequestDto groupModel)
        {
            return RemoveMembersFrom(groupid, groupModel);
        }

        [Delete("{groupid}/members")]
        [Consumes("application/x-www-form-urlencoded")]
        public GroupDto RemoveMembersFromFromForm(Guid groupid, [FromForm] GroupRequestDto groupModel)
        {
            return RemoveMembersFrom(groupid, groupModel);
        }

        private GroupDto RemoveMembersFrom(Guid groupid, GroupRequestDto groupModel)
        {
            PermissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
            var group = GetGroupInfo(groupid);

            foreach (var userId in groupModel.Members)
            {
                RemoveUserFromDepartment(userId, group);
            }
            return GetById(group.ID);
        }

        private void RemoveUserFromDepartment(Guid userId, GroupInfo @group)
        {
            if (!UserManager.UserExists(userId)) return;

            var user = UserManager.GetUsers(userId);
            UserManager.RemoveUserFromGroup(user.ID, @group.ID);
            UserManager.SaveUserInfo(user);
        }

        private void TransferUserToDepartment(Guid userId, GroupInfo group, bool setAsManager)
        {
            if (!UserManager.UserExists(userId) && userId != Guid.Empty) return;

            if (setAsManager)
            {
                UserManager.SetDepartmentManager(@group.ID, userId);
            }
            UserManager.AddUserIntoGroup(userId, @group.ID);
        }
    }
}