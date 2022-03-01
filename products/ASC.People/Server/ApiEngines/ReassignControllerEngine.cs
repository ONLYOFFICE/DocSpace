using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.People.ApiHelpers;

public class ReassignControllerEngine : ApiControllerEngineBase
{
    private readonly QueueWorkerReassign _queueWorkerReassign;

    public ReassignControllerEngine(
        UserManager userManager,
        AuthContext authContext,
        ApiContext apiContext,
        PermissionContext permissionContext,
        SecurityContext securityContext,
        MessageService messageService,
        MessageTarget messageTarget,
        StudioNotifyService studioNotifyService,
        QueueWorkerReassign queueWorkerReassign)
        : base(
            userManager,
            authContext,
            apiContext,
            permissionContext,
            securityContext,
            messageService,
            messageTarget,
            studioNotifyService)
    {
        _queueWorkerReassign = queueWorkerReassign;
    }

    public ReassignProgressItem GetReassignProgress(Guid userId)
    {
        PermissionContext.DemandPermissions(Constants.Action_EditUser);

        return _queueWorkerReassign.GetProgressItemStatus(Tenant.TenantId, userId);
    }

    public void TerminateReassign(TerminateRequestDto model)
    {
        PermissionContext.DemandPermissions(Constants.Action_EditUser);

        _queueWorkerReassign.Terminate(Tenant.TenantId, model.UserId);
    }

    public ReassignProgressItem StartReassign(StartReassignRequestDto model)
    {
        PermissionContext.DemandPermissions(Constants.Action_EditUser);

        var fromUser = UserManager.GetUsers(model.FromUserId);

        if (fromUser == null || fromUser.ID == Constants.LostUser.ID)
        {
            throw new ArgumentException("User with id = " + model.FromUserId + " not found");
        }

        if (fromUser.IsOwner(Tenant) || fromUser.IsMe(AuthContext) || fromUser.Status != EmployeeStatus.Terminated)
        {
            throw new ArgumentException("Can not delete user with id = " + model.FromUserId);
        }

        var toUser = UserManager.GetUsers(model.ToUserId);

        if (toUser == null || toUser.ID == Constants.LostUser.ID)
        {
            throw new ArgumentException("User with id = " + model.ToUserId + " not found");
        }

        if (toUser.IsVisitor(UserManager) || toUser.Status == EmployeeStatus.Terminated)
        {
            throw new ArgumentException("Can not reassign data to user with id = " + model.ToUserId);
        }

        return _queueWorkerReassign.Start(Tenant.TenantId, model.FromUserId, model.ToUserId, SecurityContext.CurrentAccount.ID, model.DeleteProfile);
    }
}
