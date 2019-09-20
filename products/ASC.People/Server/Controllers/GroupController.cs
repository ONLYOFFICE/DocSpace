using System;
using System.Collections.Generic;
using System.Linq;
using ASC.Api.Core;
using ASC.Api.Utils;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.MessagingSystem;
using ASC.People.Models;
using ASC.Web.Api.Models;
using ASC.Web.Api.Routing;
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
        public Tenant Tenant { get { return ApiContext.Tenant; } }
        public MessageService MessageService { get; }

        public GroupController(Common.Logging.LogManager logManager, MessageService messageService, ApiContext apiContext)
        {
            LogManager = logManager;
            MessageService = messageService;
            ApiContext = apiContext;
        }

        [Read]
        public IEnumerable<GroupWrapperSummary> GetAll()
        {
            return CoreContext.UserManager.GetDepartments(Tenant.TenantId).Select(x => new GroupWrapperSummary(x, ApiContext));
        }

        [Read("{groupid}")]
        public GroupWrapperFull GetById(Guid groupid)
        {
            return new GroupWrapperFull(GetGroupInfo(groupid), true, ApiContext);
        }

        [Read("user/{userid}")]
        public IEnumerable<GroupWrapperSummary> GetByUserId(Guid userid)
        {
            return CoreContext.UserManager.GetUserGroups(Tenant, userid).Select(x => new GroupWrapperSummary(x, ApiContext));
        }

        [Create]
        public GroupWrapperFull AddGroup(GroupModel groupModel)
        {
            SecurityContext.DemandPermissions(Tenant, Constants.Action_EditGroups, Constants.Action_AddRemoveUser);

            var group = CoreContext.UserManager.SaveGroupInfo(Tenant, new GroupInfo { Name = groupModel.GroupName });

            TransferUserToDepartment(groupModel.GroupManager, @group, true);
            if (groupModel.Members != null)
            {
                foreach (var member in groupModel.Members)
                {
                    TransferUserToDepartment(member, group, false);
                }
            }

            MessageService.Send(MessageAction.GroupCreated, MessageTarget.Create(group.ID), group.Name);

            return new GroupWrapperFull(group, true, ApiContext);
        }

        [Update("{groupid}")]
        public GroupWrapperFull UpdateGroup(GroupModel groupModel)
        {
            SecurityContext.DemandPermissions(Tenant, Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
            var group = CoreContext.UserManager.GetGroups(Tenant.TenantId).SingleOrDefault(x => x.ID == groupModel.Groupid).NotFoundIfNull("group not found");
            if (group.ID == Constants.LostGroupInfo.ID)
            {
                throw new ItemNotFoundException("group not found");
            }

            group.Name = groupModel.GroupName ?? group.Name;
            CoreContext.UserManager.SaveGroupInfo(Tenant, group);

            RemoveMembersFrom(new GroupModel { Groupid = groupModel.Groupid, Members = CoreContext.UserManager.GetUsersByGroup(Tenant, groupModel.Groupid, EmployeeStatus.All).Select(u => u.ID).Where(id => !groupModel.Members.Contains(id)) });

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
            SecurityContext.DemandPermissions(Tenant, Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
            var @group = GetGroupInfo(groupid);
            var groupWrapperFull = new GroupWrapperFull(group, false, ApiContext);

            CoreContext.UserManager.DeleteGroup(Tenant, groupid);

            MessageService.Send(MessageAction.GroupDeleted, MessageTarget.Create(group.ID), group.Name);

            return groupWrapperFull;
        }

        private GroupInfo GetGroupInfo(Guid groupid)
        {
            var group = CoreContext.UserManager.GetGroups(Tenant.TenantId).SingleOrDefault(x => x.ID == groupid).NotFoundIfNull("group not found");
            if (group.ID == Constants.LostGroupInfo.ID)
                throw new ItemNotFoundException("group not found");
            return @group;
        }

        [Update("{groupid}/members/{newgroupid}")]
        public GroupWrapperFull TransferMembersTo(TransferGroupMembersModel transferGroupMembersModel)
        {
            SecurityContext.DemandPermissions(Tenant, Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
            var oldgroup = GetGroupInfo(transferGroupMembersModel.GroupId);

            var newgroup = GetGroupInfo(transferGroupMembersModel.NewGroupId);

            var users = CoreContext.UserManager.GetUsersByGroup(Tenant, oldgroup.ID);
            foreach (var userInfo in users)
            {
                TransferUserToDepartment(userInfo.ID, newgroup, false);
            }
            return GetById(transferGroupMembersModel.NewGroupId);
        }

        [Create("{groupid}/members")]
        public GroupWrapperFull SetMembersTo(GroupModel groupModel)
        {
            RemoveMembersFrom(new GroupModel { Groupid = groupModel.Groupid, Members = CoreContext.UserManager.GetUsersByGroup(Tenant, groupModel.Groupid).Select(x => x.ID) });
            AddMembersTo(groupModel);
            return GetById(groupModel.Groupid);
        }

        [Update("{groupid}/members")]
        public GroupWrapperFull AddMembersTo(GroupModel groupModel)
        {
            SecurityContext.DemandPermissions(Tenant, Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
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
            if (CoreContext.UserManager.UserExists(Tenant.TenantId, setManagerModel.UserId))
            {
                CoreContext.UserManager.SetDepartmentManager(Tenant.TenantId, group.ID, setManagerModel.UserId);
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
            SecurityContext.DemandPermissions(Tenant, Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
            var group = GetGroupInfo(groupModel.Groupid);

            foreach (var userId in groupModel.Members)
            {
                RemoveUserFromDepartment(userId, group);
            }
            return GetById(group.ID);
        }

        private void RemoveUserFromDepartment(Guid userId, GroupInfo @group)
        {
            if (!CoreContext.UserManager.UserExists(Tenant.TenantId, userId)) return;

            var user = CoreContext.UserManager.GetUsers(Tenant.TenantId, userId);
            CoreContext.UserManager.RemoveUserFromGroup(Tenant, user.ID, @group.ID);
            CoreContext.UserManager.SaveUserInfo(Tenant, user);
        }

        private void TransferUserToDepartment(Guid userId, GroupInfo group, bool setAsManager)
        {
            if (!CoreContext.UserManager.UserExists(Tenant.TenantId, userId) && userId != Guid.Empty) return;

            if (setAsManager)
            {
                CoreContext.UserManager.SetDepartmentManager(Tenant.TenantId, @group.ID, userId);
            }
            CoreContext.UserManager.AddUserIntoGroup(Tenant, userId, @group.ID);
        }
    }
}