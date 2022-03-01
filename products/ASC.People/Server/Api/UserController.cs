using Module = ASC.Api.Core.Module;

namespace ASC.People.Api;

public class UserController : ApiControllerBase
{
    private readonly UserControllerEngine _userControllerEngine;

    public UserController(UserControllerEngine userControllerEngine)
    {
        _userControllerEngine = userControllerEngine;
    }

    [Create("active")]
    public EmployeeFullDto AddMemberAsActivatedFromBody([FromBody] MemberRequestDto memberModel)
    {
        return _userControllerEngine.AddMemberAsActivated(memberModel);
    }

    [Create("active")]
    [Consumes("application/x-www-form-urlencoded")]
    public EmployeeFullDto AddMemberAsActivatedFromForm([FromForm] MemberRequestDto memberModel)
    {
        return _userControllerEngine.AddMemberAsActivated(memberModel);
    }

    [Create]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "LinkInvite,Everyone")]
    public EmployeeFullDto AddMemberFromBody([FromBody] MemberRequestDto memberModel)
    {
        return _userControllerEngine.AddMember(memberModel);
    }

    [Create]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "LinkInvite,Everyone")]
    [Consumes("application/x-www-form-urlencoded")]
    public EmployeeFullDto AddMemberFromForm([FromForm] MemberRequestDto memberModel)
    {
        return _userControllerEngine.AddMember(memberModel);
    }

    [Update("{userid}/password")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "PasswordChange,EmailChange,Activation,EmailActivation,Everyone")]
    public EmployeeFullDto ChangeUserPasswordFromBody(Guid userid, [FromBody] MemberRequestDto memberModel)
    {
        return _userControllerEngine.ChangeUserPassword(userid, memberModel);
    }

    [Update("{userid}/password")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "PasswordChange,EmailChange,Activation,EmailActivation,Everyone")]
    [Consumes("application/x-www-form-urlencoded")]
    public EmployeeFullDto ChangeUserPasswordFromForm(Guid userid, [FromForm] MemberRequestDto memberModel)
    {
        return _userControllerEngine.ChangeUserPassword(userid, memberModel);
    }

    [Delete("{userid}")]
    public EmployeeFullDto DeleteMember(string userid)
    {
        return _userControllerEngine.DeleteMember(userid);
    }

    [Delete("@self")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "ProfileRemove")]
    public EmployeeFullDto DeleteProfile()
    {
        return _userControllerEngine.DeleteProfile();
    }

    [Read("status/{status}/search")]
    public IEnumerable<EmployeeFullDto> GetAdvanced(EmployeeStatus status, [FromQuery] string query)
    {
        return _userControllerEngine.GetAdvanced(status, query);
    }

    [Read]
    public IEnumerable<EmployeeDto> GetAll()
    {
        return GetByStatus(EmployeeStatus.Active);
    }

    [Read("email")]
    public EmployeeFullDto GetByEmail([FromQuery] string email)
    {
        return _userControllerEngine.GetByEmail(email);
    }

    [Read("{username}", order: int.MaxValue)]
    public EmployeeFullDto GetById(string username)
    {
        return _userControllerEngine.GetById(username);
    }

    [Read("status/{status}")]
    public IEnumerable<EmployeeDto> GetByStatus(EmployeeStatus status)
    {
        return _userControllerEngine.GetByStatus(status);
    }

    [Read("filter")]
    public IEnumerable<EmployeeFullDto> GetFullByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator)
    {
        return _userControllerEngine.GetFullByFilter(employeeStatus, groupId, activationStatus, employeeType, isAdministrator);
    }

    [Read("info")]
    public Module GetModule()
    {
        return _userControllerEngine.GetModule();
    }

    [Read("search")]
    public IEnumerable<EmployeeFullDto> GetPeopleSearch([FromQuery] string query)
    {
        return GetSearch(query);
    }

    [Read("@search/{query}")]
    public IEnumerable<EmployeeFullDto> GetSearch(string query)
    {
        return _userControllerEngine.GetSearch(query);
    }

    [Read("simple/filter")]
    public IEnumerable<EmployeeDto> GetSimpleByFilter(EmployeeStatus? employeeStatus, Guid? groupId, EmployeeActivationStatus? activationStatus, EmployeeType? employeeType, bool? isAdministrator)
    {
        return _userControllerEngine.GetSimpleByFilter(employeeStatus, groupId, activationStatus, employeeType, isAdministrator);
    }

    [AllowAnonymous]
    [Create(@"register")]
    public Task<string> RegisterUserOnPersonalAsync(RegisterPersonalUserRequestDto model)
    {
        return _userControllerEngine.RegisterUserOnPersonalAsync(model, Request);
    }

    [Update("delete", Order = -1)]
    public IEnumerable<EmployeeFullDto> RemoveUsersFromBody([FromBody] UpdateMembersRequestDto model)
    {
        return _userControllerEngine.RemoveUsers(model);
    }

    [Update("delete", Order = -1)]
    [Consumes("application/x-www-form-urlencoded")]
    public IEnumerable<EmployeeFullDto> RemoveUsersFromForm([FromForm] UpdateMembersRequestDto model)
    {
        return _userControllerEngine.RemoveUsers(model);
    }

    [Update("invite")]
    public IEnumerable<EmployeeFullDto> ResendUserInvitesFromBody([FromBody] UpdateMembersRequestDto model)
    {
        return _userControllerEngine.ResendUserInvites(model);
    }

    [Update("invite")]
    [Consumes("application/x-www-form-urlencoded")]
    public IEnumerable<EmployeeFullDto> ResendUserInvitesFromForm([FromForm] UpdateMembersRequestDto model)
    {
        return _userControllerEngine.ResendUserInvites(model);
    }

    [Read("@self")]
    public EmployeeDto Self()
    {
        return _userControllerEngine.Self();
    }

    [Create("email", false)]
    public object SendEmailChangeInstructionsFromBody([FromBody] UpdateMemberRequestDto model)
    {
        return _userControllerEngine.SendEmailChangeInstructions(model);
    }

    [Create("email", false)]
    [Consumes("application/x-www-form-urlencoded")]
    public object SendEmailChangeInstructionsFromForm([FromForm] UpdateMemberRequestDto model)
    {
        return _userControllerEngine.SendEmailChangeInstructions(model);
    }

    [AllowAnonymous]
    [Create("password", false)]
    public object SendUserPasswordFromBody([FromBody] MemberRequestDto memberModel)
    {
        return _userControllerEngine.SendUserPassword(memberModel);
    }

    [AllowAnonymous]
    [Create("password", false)]
    [Consumes("application/x-www-form-urlencoded")]
    public object SendUserPasswordFromForm([FromForm] MemberRequestDto memberModel)
    {
        return _userControllerEngine.SendUserPassword(memberModel);
    }

    [Update("activationstatus/{activationstatus}")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "Activation,Everyone")]
    public IEnumerable<EmployeeFullDto> UpdateEmployeeActivationStatusFromBody(EmployeeActivationStatus activationstatus, [FromBody] UpdateMembersRequestDto model)
    {
        return _userControllerEngine.UpdateEmployeeActivationStatus(activationstatus, model);
    }

    [Update("activationstatus/{activationstatus}")]
    [Authorize(AuthenticationSchemes = "confirm", Roles = "Activation,Everyone")]
    [Consumes("application/x-www-form-urlencoded")]
    public IEnumerable<EmployeeFullDto> UpdateEmployeeActivationStatusFromForm(EmployeeActivationStatus activationstatus, [FromForm] UpdateMembersRequestDto model)
    {
        return _userControllerEngine.UpdateEmployeeActivationStatus(activationstatus, model);
    }

    [Update("{userid}/culture")]
    public EmployeeFullDto UpdateMemberCultureFromBody(string userid, [FromBody] UpdateMemberRequestDto memberModel)
    {
        return _userControllerEngine.UpdateMemberCulture(userid, memberModel);
    }

    [Update("{userid}/culture")]
    [Consumes("application/x-www-form-urlencoded")]
    public EmployeeFullDto UpdateMemberCultureFromForm(string userid, [FromForm] UpdateMemberRequestDto memberModel)
    {
        return _userControllerEngine.UpdateMemberCulture(userid, memberModel);
    }

    [Update("{userid}")]
    public EmployeeFullDto UpdateMemberFromBody(string userid, [FromBody] UpdateMemberRequestDto memberModel)
    {
        return _userControllerEngine.UpdateMember(userid, memberModel);
    }

    [Update("{userid}")]
    [Consumes("application/x-www-form-urlencoded")]
    public EmployeeFullDto UpdateMemberFromForm(string userid, [FromForm] UpdateMemberRequestDto memberModel)
    {
        return _userControllerEngine.UpdateMember(userid, memberModel);
    }

    [Update("status/{status}")]
    public IEnumerable<EmployeeFullDto> UpdateUserStatusFromBody(EmployeeStatus status, [FromBody] UpdateMembersRequestDto model)
    {
        return _userControllerEngine.UpdateUserStatus(status, model);
    }

    [Update("status/{status}")]
    [Consumes("application/x-www-form-urlencoded")]
    public IEnumerable<EmployeeFullDto> UpdateUserStatusFromForm(EmployeeStatus status, [FromForm] UpdateMembersRequestDto model)
    {
        return _userControllerEngine.UpdateUserStatus(status, model);
    }

    [Update("type/{type}")]
    public IEnumerable<EmployeeFullDto> UpdateUserTypeFromBody(EmployeeType type, [FromBody] UpdateMembersRequestDto model)
    {
        return _userControllerEngine.UpdateUserType(type, model);
    }

    [Update("type/{type}")]
    [Consumes("application/x-www-form-urlencoded")]
    public IEnumerable<EmployeeFullDto> UpdateUserTypeFromForm(EmployeeType type, [FromForm] UpdateMembersRequestDto model)
    {
        return _userControllerEngine.UpdateUserType(type, model);
    } 

    ///// <summary>
    ///// Adds a new portal user from import with the first and last name, email address
    ///// </summary>
    ///// <short>
    ///// Add new import user
    ///// </short>
    ///// <param name="userList">The list of users to add</param>
    ///// <param name="importUsersAsCollaborators" optional="true">Add users as guests (bool type: false|true)</param>
    ///// <returns>Newly created users</returns>
    //[Create("import/save")]
    //public void SaveUsers(string userList, bool importUsersAsCollaborators)
    //{
    //    lock (progressQueue.SynchRoot)
    //    {
    //        var task = progressQueue.GetItems().OfType<ImportUsersTask>().FirstOrDefault(t => (int)t.Id == TenantProvider.CurrentTenantID);
    //        if (task != null && task.IsCompleted)
    //        {
    //            progressQueue.Remove(task);
    //            task = null;
    //        }
    //        if (task == null)
    //        {
    //            progressQueue.Add(new ImportUsersTask(userList, importUsersAsCollaborators, GetHttpHeaders(HttpContext.Current.Request))
    //            {
    //                Id = TenantProvider.CurrentTenantID,
    //                UserId = SecurityContext.CurrentAccount.ID,
    //                Percentage = 0
    //            });
    //        }
    //    }
    //}

    //[Read("import/status")]
    //public object GetStatus()
    //{
    //    lock (progressQueue.SynchRoot)
    //    {
    //        var task = progressQueue.GetItems().OfType<ImportUsersTask>().FirstOrDefault(t => (int)t.Id == TenantProvider.CurrentTenantID);
    //        if (task == null) return null;

    //        return new
    //        {
    //            Completed = task.IsCompleted,
    //            Percents = (int)task.Percentage,
    //            UserCounter = task.GetUserCounter,
    //            Status = (int)task.Status,
    //            Error = (string)task.Error,
    //            task.Data
    //        };
    //    }
    //}
}
