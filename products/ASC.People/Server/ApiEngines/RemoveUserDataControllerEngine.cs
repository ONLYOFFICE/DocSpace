using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.People.ApiHelpers;

public class RemoveUserDataControllerEngine : ApiControllerEngineBase
{
    private readonly QueueWorkerRemove _queueWorkerRemove;

    public RemoveUserDataControllerEngine(
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

    public RemoveProgressItem GetRemoveProgress(Guid userId)
    {
        PermissionContext.DemandPermissions(Constants.Action_EditUser);

        return _queueWorkerRemove.GetProgressItemStatus(Tenant.Id, userId);
    }

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

    public void TerminateRemove(TerminateRequestDto model)
    {
        PermissionContext.DemandPermissions(Constants.Action_EditUser);

        _queueWorkerRemove.Terminate(Tenant.Id, model.UserId);
    }

    public RemoveProgressItem StartRemove(TerminateRequestDto model)
    {
        PermissionContext.DemandPermissions(Constants.Action_EditUser);

        var user = UserManager.GetUsers(model.UserId);

        if (user == null || user.Id == Constants.LostUser.Id)
        {
            throw new ArgumentException("User with id = " + model.UserId + " not found");
        }

        if (user.IsOwner(Tenant) || user.IsMe(AuthContext) || user.Status != EmployeeStatus.Terminated)
        {
            throw new ArgumentException("Can not delete user with id = " + model.UserId);
        }

        return _queueWorkerRemove.Start(Tenant.Id, user, SecurityContext.CurrentAccount.ID, true);
    }
}
