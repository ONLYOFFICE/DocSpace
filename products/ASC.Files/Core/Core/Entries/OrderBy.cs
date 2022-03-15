namespace ASC.Files.Core;

public enum SortedByType
{
    DateAndTime,
    AZ,
    Size,
    Author,
    Type,
    New,
    DateAndTimeCreation
}

[DebuggerDisplay("{SortedBy} {IsAsc}")]
public class OrderBy
{
    [JsonPropertyName("is_asc")]
    public bool IsAsc { get; set; }

    [JsonPropertyName("property")]
    public SortedByType SortedBy { get; set; }

    public OrderBy(SortedByType sortedByType, bool isAsc)
    {
        SortedBy = sortedByType;
        IsAsc = isAsc;
    }
}
