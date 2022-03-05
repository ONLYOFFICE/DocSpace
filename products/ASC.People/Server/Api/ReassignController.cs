using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.People.Api;

public class ReassignController : ApiControllerBase
{
    private Tenant Tenant => _apiContext.Tenant;

    private readonly PermissionContext _permissionContext;
    private readonly QueueWorkerReassign _queueWorkerReassign;
    private readonly UserManager _userManager;
    private readonly AuthContext _authContext;
    private readonly ApiContext _apiContext;
    private readonly SecurityContext _securityContext;

    public ReassignController(
        PermissionContext permissionContext,
        QueueWorkerReassign queueWorkerReassign,
        UserManager userManager,
        AuthContext authContext,
        ApiContext apiContext,
        SecurityContext securityContext)
    {
        _permissionContext = permissionContext;
        _queueWorkerReassign = queueWorkerReassign;
        _userManager = userManager;
        _authContext = authContext;
        _apiContext = apiContext;
        _securityContext = securityContext;
    }

    [Read(@"reassign/progress")]
    public ReassignProgressItem GetReassignProgress(Guid userId)
{
        _permissionContext.DemandPermissions(Constants.Action_EditUser);

        return _queueWorkerReassign.GetProgressItemStatus(Tenant.Id, userId);
    }

    [Create(@"reassign/start")]
    public ReassignProgressItem StartReassignFromBody([FromBody] StartReassignRequestDto model)
    {
        return StartReassign(model);
    }

    [Create(@"reassign/start")]
    [Consumes("application/x-www-form-urlencoded")]
    public ReassignProgressItem StartReassignFromForm([FromForm] StartReassignRequestDto model)
    {
        return StartReassign(model);
    }

    [Update(@"reassign/terminate")]
    public void TerminateReassignFromBody([FromBody] TerminateRequestDto model)
    {
        TerminateReassign(model);
    }

    [Update(@"reassign/terminate")]
    [Consumes("application/x-www-form-urlencoded")]
    public void TerminateReassignFromForm([FromForm] TerminateRequestDto model)
    {
        TerminateReassign(model);
    }

    private ReassignProgressItem StartReassign(StartReassignRequestDto model)
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

    private void TerminateReassign(TerminateRequestDto model)
    {
        _permissionContext.DemandPermissions(Constants.Action_EditUser);

        _queueWorkerReassign.Terminate(Tenant.Id, model.UserId);
    }
}
