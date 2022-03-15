namespace ASC.Web.Api.ApiModel.ResponseDto;

public class UsageSpaceStatItemDto
{
    public string Name { get; set; }
    public string Icon { get; set; }
    public bool Disabled { get; set; }
    public string Size { get; set; }
    public string Url { get; set; }

    public static UsageSpaceStatItemDto GetSample()
    {
        return new UsageSpaceStatItemDto
        {
            Name = "Item name",
            Icon = "Item icon path",
            Disabled = false,
            Size = "0 Byte",
            Url = "Item url"
        };
    }
}

public class ChartPointDto
{
    public string DisplayDate { get; set; }
    public DateTime Date { get; set; }
    public int Hosts { get; set; }
    public int Hits { get; set; }

    public static ChartPointDto GetSample()
    {
        return new ChartPointDto
        {
            DisplayDate = DateTime.Now.ToShortDateString(),
            Date = DateTime.Now,
            Hosts = 0,
            Hits = 0
        };
    }
}