namespace ASC.VoipService;

public class VoipSettings
{
    public string VoiceUrl { get; set; }
    public string Name { get; set; }
    public List<Agent> Operators { get; set; }
    public Queue Queue { get; set; }
    public Agent Caller { get { return Operators.FirstOrDefault(r => r.Id == AuthContext.CurrentAccount.ID); } }
    public WorkingHours WorkingHours { get; set; }
    public string VoiceMail { get; set; }
    public string GreetingAudio { get; set; }
    public string HoldAudio { get; set; }
    public bool AllowOutgoingCalls { get; set; }
    public bool Pause { get; set; }
    public bool Record { get; set; }
    internal string JsonSettings
    {
        get
        {
            return JsonConvert.SerializeObject(
                new
                {
                    Operators,
                    GreetingAudio,
                    Name,
                    Queue,
                    WorkingHours,
                    VoiceMail,
                    HoldAudio,
                    AllowOutgoingCalls,
                    Pause,
                    Record
                },
                new JsonSerializerSettings { ContractResolver = CustomSerializeContractResolver.Instance });
        }
        set
        {
            try
            {
                var settings = JsonConvert.DeserializeObject<VoipSettings>(value, new JsonSerializerSettings { ContractResolver = CustomSerializeContractResolver.Instance });

                Operators = settings.Operators ?? new List<Agent>();
                Name = settings.Name;
                Queue = settings.Queue;
                WorkingHours = settings.WorkingHours;
                GreetingAudio = settings.GreetingAudio;
                VoiceMail = settings.VoiceMail;
                HoldAudio = settings.HoldAudio;
                AllowOutgoingCalls = settings.AllowOutgoingCalls;
                Pause = settings.Pause;
                Record = settings.Record;
            }
            catch (Exception)
            {

            }

        }
    }

    protected AuthContext AuthContext { get; }
    protected TenantUtil TenantUtil { get; }
    protected SecurityContext SecurityContext { get; }
    protected BaseCommonLinkUtility BaseCommonLinkUtility { get; }

    public VoipSettings(AuthContext authContext, TenantUtil tenantUtil, SecurityContext securityContext, BaseCommonLinkUtility baseCommonLinkUtility)
    {
        Operators = new List<Agent>();
        AuthContext = authContext;
        TenantUtil = tenantUtil;
        SecurityContext = securityContext;
        BaseCommonLinkUtility = baseCommonLinkUtility;
    }

    public VoipSettings(string settings, AuthContext authContext)
    {
        JsonSettings = settings;
        AuthContext = authContext;
    }

    public virtual string Connect(bool user = true, string contactId = null)
    {
        throw new NotImplementedException();
    }

    public virtual string Redirect(string to)
    {
        throw new NotImplementedException();
    }

    public virtual string Dequeue(bool reject)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return JsonSettings;
    }

    public VoipSettings GetSettings(string settings)
    {
        return new VoipSettings(AuthContext, TenantUtil, SecurityContext, BaseCommonLinkUtility) { JsonSettings = settings };
    }
}

class CustomSerializeContractResolver : CamelCasePropertyNamesContractResolver
{
    public static readonly CustomSerializeContractResolver Instance = new CustomSerializeContractResolver();

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);

        if (property.PropertyName == "voiceMail")
        {
            property.Converter = new VoiceMailConverter();
        }

        return property;
    }
}

class VoiceMailConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.ValueType != null && reader.ValueType.Name == "String")
        {
            return reader.Value;
        }

        var jObject = JObject.Load(reader);
        var url = jObject.Value<string>("url");

        return !string.IsNullOrEmpty(url) ? url : "";
    }

    public override bool CanConvert(Type objectType)
    {
        return true;
    }
}