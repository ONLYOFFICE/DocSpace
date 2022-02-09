namespace ASC.Core;

public partial class SubscriptionRecord : IMapFrom<Subscription>
{
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Subscription, SubscriptionRecord>()
            .ForMember(dest => dest.Subscribed, opt => opt.MapFrom(src => !src.Unsubscribed));
    }
}
