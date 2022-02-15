namespace ASC.Web.Api.ApiModel;

public class WhiteLabelDto
{
    public string LogoText { get; set; }
    public IEnumerable<ItemKeyValuePair<string, string>> Logo { get; set; }
}

public class WhiteLabelQuery
{
    public bool IsDefault { get; set; }
    public bool IsRetina { get; set; }
}
