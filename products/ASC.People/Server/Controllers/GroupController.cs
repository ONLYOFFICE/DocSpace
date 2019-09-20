using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Api.Core;
using ASC.Api.Utils;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.MessagingSystem;
using ASC.People.Models;
using ASC.Web.Api.Models;
using ASC.Web.Api.Routing;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Utility;
using Microsoft.AspNetCore.Mvc;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Employee.Core.Controllers
{
    [DefaultRoute]
    [ApiController]
    public class GroupController : ControllerBase
    {
        public Common.Logging.LogManager LogManager { get; }
        public ApiContext ApiContext { get; }
        public MessageService MessageService { get; }

        public UserManager UserManager { get; }
        public UserPhotoManager UserPhotoManager { get; }
        public SecurityContext SecurityContext { get; }
        public PermissionContext PermissionContext { get; }
        public CommonLinkUtility CommonLinkUtility { get; }

        public GroupController(
            Common.Logging.LogManager logManager,
            MessageService messageService,
            ApiContext apiContext,
            UserManager userManager,
            UserPhotoManager userPhotoManager,
            SecurityContext securityContext,
            PermissionContext permissionContext,
            CommonLinkUtility commonLinkUtility)
        {
            LogManager = logManager;
            MessageService = messageService;
            ApiContext = apiContext;
            UserManager = userManager;
            UserPhotoManager = userPhotoManager;
            SecurityContext = securityContext;
            PermissionContext = permissionContext;
            CommonLinkUtility = commonLinkUtility;
        }

        [Read]
        public IEnumerable<GroupWrapperSummary> GetAll()
        {
            return UserManager.GetDepartments().Select(x => new GroupWrapperSummary(x, UserManager));
        }

        [Read("{groupid}")]
        public GroupWrapperFull GetById(Guid groupid)
        {
            return new GroupWrapperFull(GetGroupInfo(groupid), true, ApiContext, UserManager, UserPhotoManager, CommonLinkUtility);
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

            return new GroupWrapperFull(group, true, ApiContext, UserManager, UserPhotoManager, CommonLinkUtility);
        }

        [Update("{groupid}")]
        public GroupWrapperFull UpdateGroup(GroupModel groupModel)
        {
            PermissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
            var group = UserManager.GetGroups().SingleOrDefault(x => x.ID == groupModel.Groupid).NotFoundIfNull("group not found");
            if (group.ID == Constants.LostGroupInfo.ID)
            {
                throw new ItemNotFoundException("group not found");
            }

            group.Name = groupModel.GroupName ?? group.Name;
            UserManager.SaveGroupInfo(group);

            RemoveMembersFrom(new GroupModel { Groupid = groupModel.Groupid, Members = UserManager.GetUsersByGroup(groupModel.Groupid, EmployeeStatus.All).Select(u => u.ID).Where(id => !groupModel.Members.Contains(id)) });

            TransferUserToDepartment(groupModel.GroupManager, @group, true);
            if (groupModel.Members != null)
            {
                foreach (var member in groupModel.Members)
                {
                    TransferUserToDepartment(member, group, false);
                }
            }

            MessageService.Send(MessageAction.GroupUpdated, MessageTarget.Create(group.ID), group.Name);

            return GetById(groupModel.Groupid);
        }

        [Delete("{groupid}")]
        public GroupWrapperFull DeleteGroup(Guid groupid)
        {
            PermissionContext.DemandPermissions(Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
            var @group = GetGroupInfo(groupid);
            var groupWrapperFull = new GroupWrapperFull(group, false, ApiContext, UserManager, UserPhotoManager, CommonLinkUtility);

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