namespace ASC.People.ApiModels.ResponseDto;

public class GroupDto
{
    public string Description { get; set; }
    public string Name { get; set; }
    public Guid? Parent { get; set; }
    public Guid Category { get; set; }
    public Guid Id { get; set; }
    public EmployeeDto Manager { get; set; }
    public List<EmployeeDto> Members { get; set; }

    public static GroupDto GetSample()
    {
        return new GroupDto
        {
            Id = Guid.NewGuid(),
            Manager = EmployeeDto.GetSample(),
            Category = Guid.NewGuid(),
            Name = "Sample group",
            Parent = Guid.NewGuid(),
            Members = new List<EmployeeDto> { EmployeeDto.GetSample() }
        };
    }
}

[Scope]
public class GroupFullDtoHelper
{
    private readonly UserManager _userManager;
    private readonly EmployeeDtoHelper _employeeWraperHelper;

    public GroupFullDtoHelper(UserManager userManager, EmployeeDtoHelper employeeWraperHelper)
    {
        _userManager = userManager;
        _employeeWraperHelper = employeeWraperHelper;
    }

    public GroupDto Get(GroupInfo group, bool includeMembers)
    {
        var result = new GroupDto
        {
            Id = group.ID,
            Category = group.CategoryID,
            Parent = group.Parent != null ? group.Parent.ID : Guid.Empty,
            Name = group.Name,
            Manager = _employeeWraperHelper.Get(_userManager.GetUsers(_userManager.GetDepartmentManager(group.ID)))
        };

        if (includeMembers)
        {
            result.Members = new List<EmployeeDto>(_userManager.GetUsersByGroup(group.ID).Select(_employeeWraperHelper.Get));
        }

        return result;
    }
}
