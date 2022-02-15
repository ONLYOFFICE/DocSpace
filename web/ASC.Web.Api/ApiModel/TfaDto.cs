namespace ASC.Web.Api.ApiModel;

public class TfaDto
{
    public string Type { get; set; }
    public Guid? Id { get; set; }
}

public class TfaValidateDto
{
    public string Code { get; set; }
}
