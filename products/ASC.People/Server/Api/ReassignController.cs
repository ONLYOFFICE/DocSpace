namespace ASC.People.Api;

public class ReassignController : ApiControllerBase
{
    private readonly ReassignControllerEngine _reassignControllerEngine;

    public ReassignController(ReassignControllerEngine reassignControllerEngine)
    {
        _reassignControllerEngine = reassignControllerEngine;
    }

    [Read(@"reassign/progress")]
    public ReassignProgressItem GetReassignProgress(Guid userId)
    {
        return _reassignControllerEngine.GetReassignProgress(userId);
    }

    [Create(@"reassign/start")]
    public ReassignProgressItem StartReassignFromBody([FromBody] StartReassignRequestDto model)
    {
        return _reassignControllerEngine.StartReassign(model);
    }

    [Create(@"reassign/start")]
    [Consumes("application/x-www-form-urlencoded")]
    public ReassignProgressItem StartReassignFromForm([FromForm] StartReassignRequestDto model)
    {
        return _reassignControllerEngine.StartReassign(model);
    }

    [Update(@"reassign/terminate")]
    public void TerminateReassignFromBody([FromBody] TerminateRequestDto model)
    {
        _reassignControllerEngine.TerminateReassign(model);
    }

    [Update(@"reassign/terminate")]
    [Consumes("application/x-www-form-urlencoded")]
    public void TerminateReassignFromForm([FromForm] TerminateRequestDto model)
    {
        _reassignControllerEngine.TerminateReassign(model);
    }
}
