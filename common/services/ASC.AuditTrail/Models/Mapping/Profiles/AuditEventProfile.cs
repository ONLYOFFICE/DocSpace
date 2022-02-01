using ASC.AuditTrail.Models.Mapping.Resolvers;
using ASC.Common.Mapping;

namespace ASC.AuditTrail.Models.Profiles
{
    public class AuditEventProfile : MappingProfile
    {
        public AuditEventProfile()
        {
            CreateMap<AuditEventQuery, AuditEvent>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AuditEvent.Id))
                .ForMember(dest => dest.Ip, opt => opt.MapFrom(src => src.AuditEvent.Ip))
                .ForMember(dest => dest.Initiator, opt => opt.MapFrom(src => src.AuditEvent.Initiator))
                .ForMember(dest => dest.Browser, opt => opt.MapFrom(src => src.AuditEvent.Browser))
                .ForMember(dest => dest.Platform, opt => opt.MapFrom(src => src.AuditEvent.Platform))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.AuditEvent.Date))
                .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.AuditEvent.TenantId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.AuditEvent.UserId))
                .ForMember(dest => dest.Page, opt => opt.MapFrom(src => src.AuditEvent.Page))
                .ForMember(dest => dest.Action, opt => opt.MapFrom(src => src.AuditEvent.Action))
                .ForMember(dest => dest.Description, opt => opt.MapFrom<AuditEventDescriptionValueResolver>())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom<AuditEventUserNameValueResolver>())
                .ForMember(dest => dest.ActionText, opt => opt.MapFrom<AuditEventActionTextValueResolver>())
                .ForMember(dest => dest.ActionTypeText, opt => opt.MapFrom<AuditEventActionTypeValueResolver>())
                .ForMember(dest => dest.Product, opt => opt.MapFrom<ProductValueResolver>())
                .ForMember(dest => dest.Module, opt => opt.MapFrom<ModuleValueResolver>());
        }
    }
}
