namespace ASC.Web.Api.ApiModel.RequestsDto;

public class WhiteLabelRequestsDto
{
    public string LogoText { get; set; }
    public IEnumerable<ItemKeyValuePair<string, string>> Logo { get; set; }
}

public class WhiteLabelQueryRequestsDto
{
    public bool IsDefault { get; set; }
    public bool IsRetina { get; set; }
}
