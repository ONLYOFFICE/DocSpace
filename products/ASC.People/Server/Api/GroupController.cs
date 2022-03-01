namespace ASC.Employee.Core.Controllers;

[Scope]
[DefaultRoute]
[ApiController]
public class GroupController : ControllerBase
{
    private readonly GroupControllerEngine _groupControllerEngine;

    public GroupController(GroupControllerEngine groupControllerEngine)
    {
        _groupControllerEngine = groupControllerEngine;
    }

    [Read]
    public IEnumerable<GroupSummaryDto> GetAll()
    {
       return _groupControllerEngine.GetAll();
    }

    [Read("full")]
    public IEnumerable<GroupDto> GetAllWithMembers()
    {
        return _groupControllerEngine.GetAllWithMembers();
    }

    [Read("{groupid}")]
    public GroupDto GetById(Guid groupid)
    {
        return _groupControllerEngine.GetById(groupid);
    }

    [Read("user/{userid}")]
    public IEnumerable<GroupSummaryDto> GetByUserId(Guid userid)
    {
        return _groupControllerEngine.GetByUserId(userid);
    }

    [Create]
    public GroupDto AddGroupFromBody([FromBody] GroupRequestDto groupModel)
    {
        return _groupControllerEngine.AddGroup(groupModel);
    }

    [Create]
    [Consumes("application/x-www-form-urlencoded")]
    public GroupDto AddGroupFromForm([FromForm] GroupRequestDto groupModel)
    {
        return _groupControllerEngine.AddGroup(groupModel);
    }

    [Update("{groupid}")]
    public GroupDto UpdateGroupFromBody(Guid groupid, [FromBody] GroupRequestDto groupModel)
    {
        return _groupControllerEngine.UpdateGroup(groupid, groupModel);
    }

    [Update("{groupid}")]
    [Consumes("application/x-www-form-urlencoded")]
    public GroupDto UpdateGroupFromForm(Guid groupid, [FromForm] GroupRequestDto groupModel)
    {
        return _groupControllerEngine.UpdateGroup(groupid, groupModel);
    }

    [Delete("{groupid}")]
    public GroupDto DeleteGroup(Guid groupid)
    {
        return _groupControllerEngine.DeleteGroup(groupid);
    }

    [Update("{groupid}/members/{newgroupid}")]
    public GroupDto TransferMembersTo(Guid groupid, Guid newgroupid)
    {
        return _groupControllerEngine.TransferMembersTo(groupid, newgroupid);
    }

    [Create("{groupid}/members")]
    public GroupDto SetMembersToFromBody(Guid groupid, [FromBody] GroupRequestDto groupModel)
    {
        return _groupControllerEngine.SetMembersTo(groupid, groupModel);
    }

    [Create("{groupid}/members")]
    [Consumes("application/x-www-form-urlencoded")]
    public GroupDto SetMembersToFromForm(Guid groupid, [FromForm] GroupRequestDto groupModel)
    {
        return _groupControllerEngine.SetMembersTo(groupid, groupModel);
    }

    [Update("{groupid}/members")]
    public GroupDto AddMembersToFromBody(Guid groupid, [FromBody] GroupRequestDto groupModel)
    {
        return _groupControllerEngine.AddMembersTo(groupid, groupModel);
    }

    [Update("{groupid}/members")]
    [Consumes("application/x-www-form-urlencoded")]
    public GroupDto AddMembersToFromForm(Guid groupid, [FromForm] GroupRequestDto groupModel)
    {
        return _groupControllerEngine.AddMembersTo(groupid, groupModel);
    }

    [Update("{groupid}/manager")]
    public GroupDto SetManagerFromBody(Guid groupid, [FromBody] SetManagerRequestDto setManagerModel)
    {
        return _groupControllerEngine.SetManager(groupid, setManagerModel);
    }

    [Update("{groupid}/manager")]
    [Consumes("application/x-www-form-urlencoded")]
    public GroupDto SetManagerFromForm(Guid groupid, [FromForm] SetManagerRequestDto setManagerModel)
    {
        return _groupControllerEngine.SetManager(groupid, setManagerModel);
    }

    [Delete("{groupid}/members")]
    public GroupDto RemoveMembersFromFromBody(Guid groupid, [FromBody] GroupRequestDto groupModel)
    {
        return _groupControllerEngine.RemoveMembersFrom(groupid, groupModel);
    }

    [Delete("{groupid}/members")]
    [Consumes("application/x-www-form-urlencoded")]
    public GroupDto RemoveMembersFromFromForm(Guid groupid, [FromForm] GroupRequestDto groupModel)
    {
        return _groupControllerEngine.RemoveMembersFrom(groupid, groupModel);
    }
}
