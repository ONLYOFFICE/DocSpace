using ASC.AuditTrail.Data;

using AutoMapper;

namespace ASC.AuditTrail.Models.Profiles
{
    public class LoginEventProfile : Profile
    {
        public LoginEventProfile()
        {
            CreateMap<LoginEvent, LoginEventQuery>();
        }
    }
}
