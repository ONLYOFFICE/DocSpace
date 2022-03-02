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
        _permissionContext.DemandPermissions(Constants.Action_EditUser);

        return _queueWorkerRemove.GetProgressItemStatus(Tenant.Id, userId);
    }

    public object SendInstructionsToDelete()
    {
        var user = _userManager.GetUsers(_securityContext.CurrentAccount.ID);

        if (user.IsLDAP())
        {
            throw new SecurityException();
        }

        _studioNotifyService.SendMsgProfileDeletion(user);
        _messageService.Send(MessageAction.UserSentDeleteInstructions);

        return string.Format(Resource.SuccessfullySentNotificationDeleteUserInfoMessage, "<b>" + user.Email + "</b>");
    }

    public void TerminateRemove(TerminateRequestDto model)
    {
        _permissionContext.DemandPermissions(Constants.Action_EditUser);

        _queueWorkerRemove.Terminate(Tenant.Id, model.UserId);
    }

    public RemoveProgressItem StartRemove(TerminateRequestDto model)
    {
        _permissionContext.DemandPermissions(Constants.Action_EditUser);

        var user = _userManager.GetUsers(model.UserId);

        if (user == null || user.Id == Constants.LostUser.Id)
        {
            throw new ArgumentException("User with id = " + model.UserId + " not found");
        }

        if (user.IsOwner(Tenant) || user.IsMe(_authContext) || user.Status != EmployeeStatus.Terminated)
        {
            throw new ArgumentException("Can not delete user with id = " + model.UserId);
        }

        return _queueWorkerRemove.Start(Tenant.Id, user, _securityContext.CurrentAccount.ID, true);
    }
}
