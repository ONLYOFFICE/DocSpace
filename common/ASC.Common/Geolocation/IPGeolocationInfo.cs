namespace ASC.Geolocation;

public class IPGeolocationInfo
{
    public string Key { get; set; }
    public string City { get; set; }
    public double TimezoneOffset { get; set; }
    public string TimezoneName { get; set; }
    public string IPStart { get; set; }
    public string IPEnd { get; set; }

    public readonly static IPGeolocationInfo Default = new IPGeolocationInfo
    {
        Key = string.Empty,
        IPStart = string.Empty,
        IPEnd = string.Empty,
        City = string.Empty,
        TimezoneName = string.Empty,
    };
}
