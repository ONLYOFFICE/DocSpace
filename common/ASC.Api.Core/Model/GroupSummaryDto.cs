using GroupInfo = ASC.Core.Users.GroupInfo;

namespace ASC.Web.Api.Models;

public class GroupSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Manager { get; set; }

    protected GroupSummaryDto() { }

    public GroupSummaryDto(GroupInfo group, UserManager userManager)
    {
        Id = group.ID;
        Name = group.Name;
        Manager = userManager.GetUsers(userManager.GetDepartmentManager(group.ID)).UserName;
    }

    public static GroupSummaryDto GetSample()
    {
        return new GroupSummaryDto 
        {
            Id = Guid.Empty, 
            Manager = "Jake.Zazhitski", 
            Name = "Group Name" 
        };
    }
}