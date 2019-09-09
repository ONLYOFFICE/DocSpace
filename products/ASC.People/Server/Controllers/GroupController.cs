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
using ASC.Web.Api.Models;
using ASC.Web.Api.Routing;
using ASC.Web.Core.Users;
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

        public UserManager UserManager { get; }
        public UserPhotoManager UserPhotoManager { get; }
        public SecurityContext SecurityContext { get; }
        public PermissionContext PermissionContext { get; }

        public GroupController(
            Common.Logging.LogManager logManager,
            MessageService messageService,
            ApiContext apiContext,
            UserManager userManager,
            UserPhotoManager userPhotoManager,
            SecurityContext securityContext,
            PermissionContext permissionContext)
        {
            LogManager = logManager;
            MessageService = messageService;
            ApiContext = apiContext;
            UserManager = userManager;
            UserPhotoManager = userPhotoManager;
            SecurityContext = securityContext;
            PermissionContext = permissionContext;
        }

        [Read]
        public IEnumerable<GroupWrapperSummary> GetAll()
        {
            return UserManager.GetDepartments(Tenant.TenantId).Select(x => new GroupWrapperSummary(x, ApiContext, UserManager));
        }

        [Read("{groupid}")]
        public GroupWrapperFull GetById(Guid groupid)
        {
            return new GroupWrapperFull(GetGroupInfo(groupid), true, ApiContext, UserManager, UserPhotoManager);
        }

        [Read("user/{userid}")]
        public IEnumerable<GroupWrapperSummary> GetByUserId(Guid userid)
        {
            return UserManager.GetUserGroups(Tenant, userid).Select(x => new GroupWrapperSummary(x, ApiContext, UserManager));
        }

        [Create]
        public GroupWrapperFull AddGroup(Guid groupManager, string groupName, IEnumerable<Guid> members)
        {
            PermissionContext.DemandPermissions(Tenant, Constants.Action_EditGroups, Constants.Action_AddRemoveUser);

            var group = UserManager.SaveGroupInfo(Tenant, new GroupInfo { Name = groupName });

            TransferUserToDepartment(groupManager, @group, true);
            if (members != null)
            {
                foreach (var member in members)
                {
                    TransferUserToDepartment(member, group, false);
                }
            }

            MessageService.Send(MessageAction.GroupCreated, MessageTarget.Create(group.ID), group.Name);

            return new GroupWrapperFull(group, true, ApiContext, UserManager, UserPhotoManager);
        }

        [Update("{groupid}")]
        public GroupWrapperFull UpdateGroup(Guid groupid, Guid groupManager, string groupName, IEnumerable<Guid> members)
        {
            PermissionContext.DemandPermissions(Tenant, Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
            var group = UserManager.GetGroups(Tenant.TenantId).SingleOrDefault(x => x.ID == groupid).NotFoundIfNull("group not found");
            if (group.ID == Constants.LostGroupInfo.ID)
            {
                throw new ItemNotFoundException("group not found");
            }

            group.Name = groupName ?? group.Name;
            UserManager.SaveGroupInfo(Tenant, group);

            RemoveMembersFrom(groupid, UserManager.GetUsersByGroup(Tenant, groupid, EmployeeStatus.All).Select(u => u.ID).Where(id => !members.Contains(id)));

            TransferUserToDepartment(groupManager, @group, true);
            if (members != null)
            {
                foreach (var member in members)
                {
                    TransferUserToDepartment(member, group, false);
                }
            }

            MessageService.Send(MessageAction.GroupUpdated, MessageTarget.Create(group.ID), group.Name);

            return GetById(groupid);
        }

        [Delete("{groupid}")]
        public GroupWrapperFull DeleteGroup(Guid groupid)
        {
            PermissionContext.DemandPermissions(Tenant, Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
            var @group = GetGroupInfo(groupid);
            var groupWrapperFull = new GroupWrapperFull(group, false, ApiContext, UserManager, UserPhotoManager);

            UserManager.DeleteGroup(Tenant, groupid);

            MessageService.Send(MessageAction.GroupDeleted, MessageTarget.Create(group.ID), group.Name);

            return groupWrapperFull;
        }

        private GroupInfo GetGroupInfo(Guid groupid)
        {
            var group = UserManager.GetGroups(Tenant.TenantId).SingleOrDefault(x => x.ID == groupid).NotFoundIfNull("group not found");
            if (group.ID == Constants.LostGroupInfo.ID)
                throw new ItemNotFoundException("group not found");
            return @group;
        }

        [Update("{groupid}/members/{newgroupid}")]
        public GroupWrapperFull TransferMembersTo(Guid groupid, Guid newgroupid)
        {
            PermissionContext.DemandPermissions(Tenant, Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
            var oldgroup = GetGroupInfo(groupid);

            var newgroup = GetGroupInfo(newgroupid);

            var users = UserManager.GetUsersByGroup(Tenant, oldgroup.ID);
            foreach (var userInfo in users)
            {
                TransferUserToDepartment(userInfo.ID, newgroup, false);
            }
            return GetById(newgroupid);
        }

        [Create("{groupid}/members")]
        public GroupWrapperFull SetMembersTo(Guid groupid, IEnumerable<Guid> members)
        {
            RemoveMembersFrom(groupid, UserManager.GetUsersByGroup(Tenant, groupid).Select(x => x.ID));
            AddMembersTo(groupid, members);
            return GetById(groupid);
        }

        [Update("{groupid}/members")]
        public GroupWrapperFull AddMembersTo(Guid groupid, IEnumerable<Guid> members)
        {
            PermissionContext.DemandPermissions(Tenant, Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
            var group = GetGroupInfo(groupid);

            foreach (var userId in members)
            {
                TransferUserToDepartment(userId, group, false);
            }
            return GetById(group.ID);
        }

        [Update("{groupid}/manager")]
        public GroupWrapperFull SetManager(Guid groupid, Guid userid)
        {
            var group = GetGroupInfo(groupid);
            if (UserManager.UserExists(Tenant.TenantId, userid))
            {
                UserManager.SetDepartmentManager(Tenant.TenantId, group.ID, userid);
            }
            else
            {
                throw new ItemNotFoundException("user not found");
            }
            return GetById(groupid);
        }

        [Delete("{groupid}/members")]
        public GroupWrapperFull RemoveMembersFrom(Guid groupid, IEnumerable<Guid> members)
        {
            PermissionContext.DemandPermissions(Tenant, Constants.Action_EditGroups, Constants.Action_AddRemoveUser);
            var group = GetGroupInfo(groupid);

            foreach (var userId in members)
            {
                RemoveUserFromDepartment(userId, group);
            }
            return GetById(group.ID);
        }

        private void RemoveUserFromDepartment(Guid userId, GroupInfo @group)
        {
            if (!UserManager.UserExists(Tenant.TenantId, userId)) return;

            var user = UserManager.GetUsers(Tenant.TenantId, userId);
            UserManager.RemoveUserFromGroup(Tenant, user.ID, @group.ID);
            UserManager.SaveUserInfo(Tenant, user);
        }

        private void TransferUserToDepartment(Guid userId, GroupInfo group, bool setAsManager)
        {
            if (!UserManager.UserExists(Tenant.TenantId, userId) && userId != Guid.Empty) return;

            if (setAsManager)
            {
                UserManager.SetDepartmentManager(Tenant.TenantId, @group.ID, userId);
            }
            UserManager.AddUserIntoGroup(Tenant, userId, @group.ID);
        }
    }
}