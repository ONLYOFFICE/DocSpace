namespace ASC.Web.Api.ApiModel.RequestsDto;

public class SecurityRequestsDto
{
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public bool Administrator { get; set; }
}