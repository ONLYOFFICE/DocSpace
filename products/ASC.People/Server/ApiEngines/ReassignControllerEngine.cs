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
        _permissionContext.DemandPermissions(Constants.Action_EditUser);

        return _queueWorkerReassign.GetProgressItemStatus(Tenant.Id, userId);
    }

    public void TerminateReassign(TerminateRequestDto model)
    {
        _permissionContext.DemandPermissions(Constants.Action_EditUser);

        _queueWorkerReassign.Terminate(Tenant.Id, model.UserId);
    }

    public ReassignProgressItem StartReassign(StartReassignRequestDto model)
    {
        _permissionContext.DemandPermissions(Constants.Action_EditUser);

        var fromUser = _userManager.GetUsers(model.FromUserId);

        if (fromUser == null || fromUser.Id == Constants.LostUser.Id)
        {
            throw new ArgumentException("User with id = " + model.FromUserId + " not found");
        }

        if (fromUser.IsOwner(Tenant) || fromUser.IsMe(_authContext) || fromUser.Status != EmployeeStatus.Terminated)
        {
            throw new ArgumentException("Can not delete user with id = " + model.FromUserId);
        }

        var toUser = _userManager.GetUsers(model.ToUserId);

        if (toUser == null || toUser.Id == Constants.LostUser.Id)
        {
            throw new ArgumentException("User with id = " + model.ToUserId + " not found");
        }

        if (toUser.IsVisitor(_userManager) || toUser.Status == EmployeeStatus.Terminated)
        {
            throw new ArgumentException("Can not reassign data to user with id = " + model.ToUserId);
        }

        return _queueWorkerReassign.Start(Tenant.Id, model.FromUserId, model.ToUserId, _securityContext.CurrentAccount.ID, model.DeleteProfile);
    }
}
