namespace ASC.VoipService;

public class Agent
{
    public Guid Id { get; set; }
    public AnswerType Answer { get; set; }
    public string ClientID { get { return PhoneNumber + PostFix; } }
    public bool Record { get; set; }
    public int TimeOut { get; set; }
    public AgentStatus Status { get; set; }
    public bool AllowOutgoingCalls { get; set; }
    public string PostFix { get; set; }
    public string PhoneNumber { get; set; }
    public string RedirectToNumber { get; set; }

    public Agent()
    {
        Status = AgentStatus.Offline;
        TimeOut = 30;
        AllowOutgoingCalls = true;
        Record = true;
    }

    public Agent(Guid id, AnswerType answer, VoipPhone phone, string postFix)
        : this()
    {
        Id = id;
        Answer = answer;
        PhoneNumber = phone.Number;
        AllowOutgoingCalls = phone.Settings.AllowOutgoingCalls;
        Record = phone.Settings.Record;
        PostFix = postFix;
    }
}

public class Queue
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Size { get; set; }
    public string WaitUrl { get; set; }
    public int WaitTime { get; set; }

    public Queue() { }

    public Queue(string id, string name, int size, string waitUrl, int waitTime)
    {
        Id = id;
        Name = name;
        WaitUrl = waitUrl;
        WaitTime = waitTime;
        Size = size;
    }
}

public sealed class WorkingHours
{
    public bool Enabled { get; set; }
    public TimeSpan? From { get; set; }
    public TimeSpan? To { get; set; }

    public WorkingHours() { }

    public WorkingHours(TimeSpan from, TimeSpan to)
    {
        From = from;
        To = to;
    }


    private bool Equals(WorkingHours other)
    {
        return Enabled.Equals(other.Enabled) && From.Equals(other.From) && To.Equals(other.To);
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((WorkingHours)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Enabled, From, To);
    }
}

public class VoipUpload
{
    public string Name { get; set; }
    public string Path { get; set; }
    public AudioType AudioType { get; set; }
    public bool IsDefault { get; set; }
}

public enum AnswerType
{
    Number,
    Sip,
    Client
}

public enum GreetingMessageVoice
{
    Man,
    Woman,
    Alice
}

public enum AgentStatus
{
    Online,
    Paused,
    Offline
}

public enum AudioType
{
    Greeting,
    HoldUp,
    VoiceMail,
    Queue
}