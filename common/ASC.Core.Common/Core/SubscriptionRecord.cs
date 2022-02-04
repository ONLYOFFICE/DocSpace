
using ASC.Common.Mapping;
using ASC.Core.Common.EF;

using AutoMapper;

namespace ASC.Core
{
    public partial class SubscriptionRecord : IMapFrom<Subscription>
    {
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Subscription, SubscriptionRecord>()
                .ForMember(dest => dest.Subscribed, opt => opt.MapFrom(src => !src.Unsubscribed));
        }
    }
}
