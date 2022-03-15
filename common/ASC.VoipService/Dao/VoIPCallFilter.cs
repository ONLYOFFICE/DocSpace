namespace ASC.VoipService.Dao;

public class VoipCallFilter
{
    public string Type { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public Guid? Agent { get; set; }
    public int? Client { get; set; }
    public int? ContactID { get; set; }
    public string Id { get; set; }
    public string ParentId { get; set; }
    public string SortBy { get; set; }
    public bool SortOrder { get; set; }
    public string SearchText { get; set; }
    public long Offset { get; set; }
    public long Max { get; set; }

    public int? TypeStatus
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Type)) return null;
            if (TypeStatuses.TryGetValue(Type, out var status)) return status;

            return null;
        }
    }

    public string SortByColumn
    {
        get
        {
            if (string.IsNullOrWhiteSpace(SortBy)) return null;
            return SortColumns.ContainsKey(SortBy) ? SortColumns[SortBy] : null;
        }
    }

    private static Dictionary<string, int> TypeStatuses
    {
        get
        {
            return new Dictionary<string, int>
                {
                    {
                        "answered", (int)VoipCallStatus.Answered
                    },
                    {
                        "missed", (int)VoipCallStatus.Missed
                    },
                    {
                        "outgoing", (int)VoipCallStatus.Outcoming
                    }
                };
        }
    }

    private static Dictionary<string, string> SortColumns
    {
        get
        {
            return new Dictionary<string, string>
                {
                    {
                        "date", "dial_date"
                    },
                    {
                        "duration", "dial_duration"
                    },
                    {
                        "price", "price"
                    },
                };
        }
    }
}