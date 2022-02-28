using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.People.Api;

public class RemoveUserDataController : BaseApiController
{
    private readonly QueueWorkerRemove _queueWorkerRemove;

    public RemoveUserDataController(
        UserManager userManager,
        AuthContext authContext,
        ApiContext apiContext,
        PermissionContext permissionContext,
        SecurityContext securityContext,
        MessageService messageService,
        MessageTarget messageTarget,
        StudioNotifyService studioNotifyService,
        QueueWorkerRemove queueWorkerRemove)
        : base(userManager,
               authContext,
               apiContext,
               permissionContext,
               securityContext,
               messageService,
               messageTarget,
               studioNotifyService)
    {
        _queueWorkerRemove = queueWorkerRemove;
    }

    [Read(@"remove/progress")]
    public RemoveProgressItem GetRemoveProgress(Guid userId)
    {
        PermissionContext.DemandPermissions(Constants.Action_EditUser);

        return _queueWorkerRemove.GetProgressItemStatus(Tenant.TenantId, userId);
    }

    [Update("self/delete")]
    public object SendInstructionsToDelete()
    {
        var user = UserManager.GetUsers(SecurityContext.CurrentAccount.ID);

        if (user.IsLDAP())
        {
            throw new SecurityException();
        }

        StudioNotifyService.SendMsgProfileDeletion(user);
        MessageService.Send(MessageAction.UserSentDeleteInstructions);

        return string.Format(Resource.SuccessfullySentNotificationDeleteUserInfoMessage, "<b>" + user.Email + "</b>");
    }

    [Create(@"remove/start")]
    public RemoveProgressItem StartRemoveFromBody([FromBody] TerminateRequestDto model)
    {
        return StartRemove(model);
    }

    [Create(@"remove/start")]
    [Consumes("application/x-www-form-urlencoded")]
    public RemoveProgressItem StartRemoveFromForm([FromForm] TerminateRequestDto model)
    {
        return StartRemove(model);
    }

    [Update(@"remove/terminate")]
    public void TerminateRemoveFromBody([FromBody] TerminateRequestDto model)
    {
        PermissionContext.DemandPermissions(Constants.Action_EditUser);

        _queueWorkerRemove.Terminate(Tenant.TenantId, model.UserId);
    }

    [Update(@"remove/terminate")]
    [Consumes("application/x-www-form-urlencoded")]
    public void TerminateRemoveFromForm([FromForm] TerminateRequestDto model)
    {
        PermissionContext.DemandPermissions(Constants.Action_EditUser);

        _queueWorkerRemove.Terminate(Tenant.TenantId, model.UserId);
    }

    private RemoveProgressItem StartRemove(TerminateRequestDto model)
    {
        PermissionContext.DemandPermissions(Constants.Action_EditUser);

        var user = UserManager.GetUsers(model.UserId);

        if (user == null || user.ID == Constants.LostUser.ID)
        {
            throw new ArgumentException("User with id = " + model.UserId + " not found");
        }

        if (user.IsOwner(Tenant) || user.IsMe(AuthContext) || user.Status != EmployeeStatus.Terminated)
        {
            throw new ArgumentException("Can not delete user with id = " + model.UserId);
        }

        return _queueWorkerRemove.Start(Tenant.TenantId, user, SecurityContext.CurrentAccount.ID, true);
    }
}
