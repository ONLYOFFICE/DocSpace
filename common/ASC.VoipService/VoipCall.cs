namespace ASC.VoipService;

public class VoipCall : IMapFrom<CallContact>
{
    public string Id { get; set; }
    public string ParentCallId { get; set; }
    public string NumberFrom { get; set; }
    public string NumberTo { get; set; }
    public Guid AnsweredBy { get; set; }
    public DateTime DialDate { get; set; }
    public int DialDuration { get; set; }
    public VoipCallStatus? Status { get; set; }
    public decimal Price { get; set; }
    public int ContactId { get; set; }
    public bool ContactIsCompany { get; set; }
    public string ContactTitle { get; set; }
    public DateTime Date { get; set; }
    public DateTime EndDialDate { get; set; }
    public VoipRecord VoipRecord { get; set; }
    public List<VoipCall> ChildCalls { get; set; }

    public VoipCall()
    {
        ChildCalls = new List<VoipCall>();
        VoipRecord = new VoipRecord();
    }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<DbVoipCall, VoipCall>();

        profile.CreateMap<CallContact, VoipCall>()
            .ConvertUsing<CallTypeConverter>();
    }
}

public enum VoipCallStatus
{
    Incoming,
    Outcoming,
    Answered,
    Missed
}