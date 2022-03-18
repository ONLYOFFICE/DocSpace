namespace ASC.Web.Api.ApiModel.RequestsDto;

public class AuthServiceRequestsDto
{
    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Instruction { get; set; }
    public bool CanSet { get; set; }
    public List<AuthKey> Props { get; set; }

    public AuthServiceRequestsDto()
    {

    }

    public AuthServiceRequestsDto(Consumer consumer)
    {
        var authService = new AuthService(consumer);

        Name = authService.Name;
        Title = authService.Title;
        Description = authService.Description;
        Instruction = authService.Instruction;
        CanSet = authService.CanSet;

        if (consumer.CanSet)
        {
            Props = authService.Props;
            CanSet = authService.CanSet;
        }
    }
}