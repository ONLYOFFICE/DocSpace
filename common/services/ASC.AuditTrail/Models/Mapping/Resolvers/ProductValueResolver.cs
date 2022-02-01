
using ASC.AuditTrail.Mappers;

using AutoMapper;

namespace ASC.AuditTrail.Models.Mapping.Resolvers
{
    public class ProductValueResolver : IValueResolver<AuditEventQuery, AuditEvent, string>
    {
        private readonly AuditActionMapper _actionMapper;

        public ProductValueResolver(AuditActionMapper actionMapper) => _actionMapper = actionMapper;

        public string Resolve(AuditEventQuery source, AuditEvent destination, string destMember, ResolutionContext context) =>
            _actionMapper.GetProductText(destination);
    }
}
