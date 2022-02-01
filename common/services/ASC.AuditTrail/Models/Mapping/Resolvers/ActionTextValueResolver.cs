using ASC.AuditTrail.Mappers;

using AutoMapper;

namespace ASC.AuditTrail.Models.Mapping.Resolvers
{
    public class ActionTextValueResolver : IValueResolver<LoginEventQuery, LoginEvent, string>
    {
        private readonly AuditActionMapper _actionMapper;

        public ActionTextValueResolver(AuditActionMapper actionMapper) => _actionMapper = actionMapper;

        public string Resolve(LoginEventQuery source, LoginEvent destination, string destMember, ResolutionContext context)
        {
            return _actionMapper.GetActionText(destination);
        }
    }

    public class AuditEventActionTextValueResolver : IValueResolver<AuditEventQuery, AuditEvent, string>
    {
        private readonly AuditActionMapper _actionMapper;

        public AuditEventActionTextValueResolver(AuditActionMapper actionMapper) => _actionMapper = actionMapper;

        public string Resolve(AuditEventQuery source, AuditEvent destination, string destMember, ResolutionContext context)
        {
            return _actionMapper.GetActionText(destination);
        }
    }
}
