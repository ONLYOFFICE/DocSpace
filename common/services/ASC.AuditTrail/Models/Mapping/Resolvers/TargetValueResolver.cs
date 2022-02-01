using ASC.MessagingSystem;

using AutoMapper;

namespace ASC.AuditTrail.Models.Mapping.Resolvers
{
    public class TargetValueResolver : IValueResolver<AuditEventQuery, AuditEvent, MessageTarget>
    {
        private readonly MessageTarget _target;

        public TargetValueResolver(MessageTarget messageTarget) => _target = messageTarget;

        public MessageTarget Resolve(AuditEventQuery source, AuditEvent destination, MessageTarget destMember, ResolutionContext context) =>
            _target.Parse(source.AuditEvent.Target);
    }
}
