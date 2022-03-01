namespace ASC.People.Api;

public class RemoveUserDataController : ApiControllerBase
{
    private readonly RemoveUserDataControllerEngine _removeUserDataControllerEngine;

    public RemoveUserDataController(RemoveUserDataControllerEngine removeUserDataControllerEngine)
    {
        _removeUserDataControllerEngine = removeUserDataControllerEngine;
    }

    [Read(@"remove/progress")]
    public RemoveProgressItem GetRemoveProgress(Guid userId)
    {
       return _removeUserDataControllerEngine.GetRemoveProgress(userId);
    }

    [Update("self/delete")]
    public object SendInstructionsToDelete()
    {
        return _removeUserDataControllerEngine.SendInstructionsToDelete();
    }

    [Create(@"remove/start")]
    public RemoveProgressItem StartRemoveFromBody([FromBody] TerminateRequestDto model)
    {
        return _removeUserDataControllerEngine.StartRemove(model);
    }

    [Create(@"remove/start")]
    [Consumes("application/x-www-form-urlencoded")]
    public RemoveProgressItem StartRemoveFromForm([FromForm] TerminateRequestDto model)
    {
        return _removeUserDataControllerEngine.StartRemove(model);
    }

    [Update(@"remove/terminate")]
    public void TerminateRemoveFromBody([FromBody] TerminateRequestDto model)
    {
        _removeUserDataControllerEngine.TerminateRemove(model);
    }

    [Update(@"remove/terminate")]
    [Consumes("application/x-www-form-urlencoded")]
    public void TerminateRemoveFromForm([FromForm] TerminateRequestDto model)
    {
        _removeUserDataControllerEngine.TerminateRemove(model);
    }
}
