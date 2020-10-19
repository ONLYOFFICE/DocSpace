using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Api.Utils;
using ASC.Common;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.MessagingSystem;
using ASC.People.Models;
using ASC.Web.Api.Models;
using ASC.Web.Api.Routing;

using Microsoft.AspNetCore.Mvc;

namespace ASC.Employee.Core.Controllers
{
    [Scope]
    [DefaultRoute]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private MessageService MessageService { get; }

        private UserManager UserManager { get; }
        private PermissionContext PermissionContext { get; }
        private MessageTarget MessageTarget { get; }
        private GroupWraperFullHelper GroupWraperFullHelper { get; }

        public GroupController(
            MessageService messageService,
            UserManager userManager,
            PermissionContext permissionContext,
            MessageTarget messageTarget,
            GroupWraperFullHelper groupWraperFullHelper)
        {
            MessageService = messageService;
            UserManager = userManager;
            PermissionContext = permissionContext;
            MessageTarget = messageTarget;
            GroupWraperFullHelper = groupWraperFullHelper;
        }

        [Read]
        public IEnumerable<GroupWrapperSummary> GetAll()
        {
            return UserManager.GetDepartments().Select(x => new GroupWrapperSummary(x, UserManager));
        }

        [Read("{groupid}")]
        public GroupWrapperFull GetById(Guid groupid)
        {
            return GroupWraperFullHelper.Get(GetGroupInfo(groupid), true);
        }

        [Read("user/{userid}")]
        public IEnumerable<GroupWrapperSummary> GetByUserId(Guid userid)
        {
            return UserManager.GetUserGroups(userid).Select(x => new GroupWrapperSummary(x, UserManager));
        }

        [Create]
        public GroupWrapperFull AddGroup(GroupModel groupModel)
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
        public GroupWrapperFull UpdateGroup(Guid groupid, GroupModel groupModel)
        {
            PermissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
            var group = UserManager.GetGroups().SingleOrDefault(x => x.ID == groupid).NotFoundIfNull("group not found");
            if (groupid == Constants.LostGroupInfo.ID)
            {
                throw new ItemNotFoundException("group not found");
            }

            group.Name = groupModel.GroupName ?? group.Name;
            UserManager.SaveGroupInfo(group);

            RemoveMembersFrom(new GroupModel { Groupid = groupid, Members = UserManager.GetUsersByGroup(groupid, EmployeeStatus.All).Select(u => u.ID).Where(id => !groupModel.Members.Contains(id)) });

            TransferUserToDepartment(groupModel.GroupManager, @group, true);
            if (groupModel.Members != null)
            {
                foreach (var member in groupModel.Members)
                {
                    TransferUserToDepartment(member, group, false);
                }
            }

            MessageService.Send(MessageAction.GroupUpdated, MessageTarget.Create(groupid), group.Name);

            return GetById(groupModel.Groupid);
        }

        [Delete("{groupid}")]
        public GroupWrapperFull DeleteGroup(Guid groupid)
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
        public GroupWrapperFull TransferMembersTo(TransferGroupMembersModel transferGroupMembersModel)
        {
            PermissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
            var oldgroup = GetGroupInfo(transferGroupMembersModel.GroupId);

            var newgroup = GetGroupInfo(transferGroupMembersModel.NewGroupId);

            var users = UserManager.GetUsersByGroup(oldgroup.ID);
            foreach (var userInfo in users)
            {
                TransferUserToDepartment(userInfo.ID, newgroup, false);
            }
            return GetById(transferGroupMembersModel.NewGroupId);
        }

        [Create("{groupid}/members")]
        public GroupWrapperFull SetMembersTo(GroupModel groupModel)
        {
            RemoveMembersFrom(new GroupModel { Groupid = groupModel.Groupid, Members = UserManager.GetUsersByGroup(groupModel.Groupid).Select(x => x.ID) });
            AddMembersTo(groupModel);
            return GetById(groupModel.Groupid);
        }

        [Update("{groupid}/members")]
        public GroupWrapperFull AddMembersTo(GroupModel groupModel)
        {
            PermissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
            var group = GetGroupInfo(groupModel.Groupid);

            foreach (var userId in groupModel.Members)
            {
                TransferUserToDepartment(userId, group, false);
            }
            return GetById(group.ID);
        }

        [Update("{groupid}/manager")]
        public GroupWrapperFull SetManager(SetManagerModel setManagerModel)
        {
            var group = GetGroupInfo(setManagerModel.GroupId);
            if (UserManager.UserExists(setManagerModel.UserId))
            {
                UserManager.SetDepartmentManager(group.ID, setManagerModel.UserId);
            }
            else
            {
                throw new ItemNotFoundException("user not found");
            }
            return GetById(setManagerModel.GroupId);
        }

        [Delete("{groupid}/members")]
        public GroupWrapperFull RemoveMembersFrom(GroupModel groupModel)
        {
            PermissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
            var group = GetGroupInfo(groupModel.Groupid);

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