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
    public ReassignProgressItem StartReassignFromBody([FromBody] StartReassignRequestDto inDto)
    {
        return StartReassign(inDto);
    }

    [Create(@"reassign/start")]
    [Consumes("application/x-www-form-urlencoded")]
    public ReassignProgressItem StartReassignFromForm([FromForm] StartReassignRequestDto inDto)
    {
        return StartReassign(inDto);
    }

    [Update(@"reassign/terminate")]
    public void TerminateReassignFromBody([FromBody] TerminateRequestDto inDto)
    {
        TerminateReassign(inDto);
    }

    [Update(@"reassign/terminate")]
    [Consumes("application/x-www-form-urlencoded")]
    public void TerminateReassignFromForm([FromForm] TerminateRequestDto inDto)
    {
        TerminateReassign(inDto);
    }

    private ReassignProgressItem StartReassign(StartReassignRequestDto inDto)
    {
        _permissionContext.DemandPermissions(Constants.Action_EditUser);

        var fromUser = _userManager.GetUsers(inDto.FromUserId);

        if (fromUser == null || fromUser.Id == Constants.LostUser.Id)
        {
            throw new ArgumentException("User with id = " + inDto.FromUserId + " not found");
        }

        if (fromUser.IsOwner(Tenant) || fromUser.IsMe(_authContext) || fromUser.Status != EmployeeStatus.Terminated)
        {
            throw new ArgumentException("Can not delete user with id = " + inDto.FromUserId);
        }

        var toUser = _userManager.GetUsers(inDto.ToUserId);

        if (toUser == null || toUser.Id == Constants.LostUser.Id)
        {
            throw new ArgumentException("User with id = " + inDto.ToUserId + " not found");
        }

        if (toUser.IsVisitor(_userManager) || toUser.Status == EmployeeStatus.Terminated)
        {
            throw new ArgumentException("Can not reassign data to user with id = " + inDto.ToUserId);
        }

        return _queueWorkerReassign.Start(Tenant.Id, inDto.FromUserId, inDto.ToUserId, _securityContext.CurrentAccount.ID, inDto.DeleteProfile);
    }

    private void TerminateReassign(TerminateRequestDto inDto)
    {
        _permissionContext.DemandPermissions(Constants.Action_EditUser);

        _queueWorkerReassign.Terminate(Tenant.Id, inDto.UserId);
    }
}
