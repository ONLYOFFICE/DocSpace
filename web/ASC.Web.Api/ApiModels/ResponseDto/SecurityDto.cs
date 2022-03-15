namespace ASC.Web.Api.ApiModel.ResponseDto;

public class SecurityDto
{
    public string WebItemId { get; set; }
    public IEnumerable<EmployeeDto> Users { get; set; }
    public IEnumerable<GroupSummaryDto> Groups { get; set; }
    public bool Enabled { get; set; }
    public bool IsSubItem { get; set; }

    public static SecurityDto GetSample()
    {
        return new SecurityDto
        {
            WebItemId = Guid.Empty.ToString(),
            Enabled = true,
            IsSubItem = false,
            Groups = new List<GroupSummaryDto>
                    {
                        GroupSummaryDto.GetSample()
                    },
            Users = new List<EmployeeDto>
                    {
                        EmployeeDto.GetSample()
                    }
        };
    }
}