namespace ASC.Web.Api.ApiModel.RequestsDto;

public class TfaRequestsDto
{
    public string Type { get; set; }
    public Guid? Id { get; set; }
}

public class TfaValidateRequestsDto
{
    public string Code { get; set; }
}
