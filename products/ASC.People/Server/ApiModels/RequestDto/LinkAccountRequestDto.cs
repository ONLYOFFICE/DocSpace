namespace ASC.People.ApiModels.RequestDto;

public class LinkAccountRequestDto
{
    public string SerializedProfile { get; set; }
}

public class SignupAccountRequestDto : LinkAccountRequestDto
{
    public EmployeeType? EmplType { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
}
