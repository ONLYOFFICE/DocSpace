namespace ASC.Notify.Messages;

public partial class NotifyMessage : IMapFrom<NotifyQueue>
{
    public void Mapping(Profile profile)
    {
        profile.CreateMap<NotifyQueue, NotifyMessage>()
            .ForMember(dest => dest.Attachments, opt => opt.Ignore());
    }
}
