namespace ASC.Web.Api.ApiModel;

public class SecurityDto
{
    public Guid ProductId { get; set; }

    public Guid UserId { get; set; }

    public bool Administrator { get; set; }
}