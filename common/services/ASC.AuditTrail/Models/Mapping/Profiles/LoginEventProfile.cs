using ASC.AuditTrail.Models.Mapping.Resolvers;
using ASC.Common.Mapping;

namespace ASC.AuditTrail.Models.Mapping.Profiles
{
    public class LoginEventProfile : MappingProfile
    {
        public LoginEventProfile()
        {
            CreateMap<LoginEventQuery, LoginEvent>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.LoginEvents.Id))
                .ForMember(dest => dest.Ip, opt => opt.MapFrom(src => src.LoginEvents.Ip))
                .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.LoginEvents.Login))
                .ForMember(dest => dest.Browser, opt => opt.MapFrom(src => src.LoginEvents.Browser))
                .ForMember(dest => dest.Platform, opt => opt.MapFrom(src => src.LoginEvents.Platform))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.LoginEvents.Date))
                .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.LoginEvents.TenantId))
                .ForMember(dest => dest.Page, opt => opt.MapFrom(src => src.LoginEvents.Page))
                .ForMember(dest => dest.Action, opt => opt.MapFrom(src => src.LoginEvents.Action))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.LoginEvents.UserId))
                .ForMember(dest => dest.Description, opt => opt.MapFrom<LoginEventDescriptionValueResolver>())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom<LoginEventUserNameValueResolver>())
                .ForMember(dest => dest.ActionText, opt => opt.MapFrom<ActionTextValueResolver>());
        }
    }
}
