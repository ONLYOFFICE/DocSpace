using AutoMapper;

namespace ASC.VoipService;

public class VoipPhone
{
    public string Id { get; set; }
    public string Number { get; set; }
    public string Alias { get; set; }
    public VoipSettings Settings { get; set; }
    public Agent Caller
    {
        get { return Settings.Caller; }
    }

    public VoipPhone(AuthContext authContext, TenantUtil tenantUtil, SecurityContext securityContext, BaseCommonLinkUtility baseCommonLinkUtility)
    {
        Settings = new VoipSettings(authContext, tenantUtil, securityContext, baseCommonLinkUtility);
    }

    public virtual VoipCall Call(string to, string contactId = null)
    {
        throw new NotImplementedException();
    }

    public virtual VoipCall LocalCall(string to)
    {
        throw new NotImplementedException();
    }

    public virtual VoipCall RedirectCall(string callId, string to)
    {
        throw new NotImplementedException();
    }

    public virtual VoipCall HoldUp(string callId)
    {
        throw new NotImplementedException();
    }

    public virtual void AnswerQueueCall(string callId)
    {
        throw new NotImplementedException();
    }

    public virtual void RejectQueueCall(string callId)
    {
        throw new NotImplementedException();
    }
}

public class VoipRecord : IMapFrom<DbVoipCall>
{
    public string Sid { get; set; }
    public string Uri { get; set; }
    public int Duration { get; set; }
    public decimal Price { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<DbVoipCall, VoipRecord>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.RecordPrice));
    }
}