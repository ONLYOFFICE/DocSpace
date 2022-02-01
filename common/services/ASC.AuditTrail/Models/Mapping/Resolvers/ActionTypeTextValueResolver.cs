
using ASC.AuditTrail.Mappers;

using AutoMapper;

namespace ASC.AuditTrail.Models.Mapping.Resolvers
{
    public class LoginEventActionTypeTextValueResolver : IValueResolver<AuditEventQuery, AuditEvent, string>
    {
        private readonly AuditActionMapper _actionMapper;

        public LoginEventActionTypeTextValueResolver(AuditActionMapper actionMapper) => _actionMapper = actionMapper;

        public string Resolve(AuditEventQuery source, AuditEvent destination, string destMember, ResolutionContext context) =>
            _actionMapper.GetActionTypeText(destination);
    }

    public class AuditEventActionTypeValueResolver : IValueResolver<AuditEventQuery, AuditEvent, string>
    {
        private readonly AuditActionMapper _actionMapper;

        public AuditEventActionTypeValueResolver(AuditActionMapper actionMapper) => _actionMapper = actionMapper;

        public string Resolve(AuditEventQuery source, AuditEvent destination, string destMember, ResolutionContext context) =>
            _actionMapper.GetActionTypeText(destination);
    }
}
