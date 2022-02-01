using System.Collections.Generic;

using AutoMapper;

using Newtonsoft.Json;

namespace ASC.AuditTrail.Models.Mapping.Resolvers
{
    public class LoginEventDescriptionValueResolver : IValueResolver<LoginEventQuery, LoginEvent, IList<string>>
    {
        public IList<string> Resolve(LoginEventQuery source, LoginEvent destination, IList<string> destMember, ResolutionContext context)
        {
            if (source.LoginEvents.Description == null) return null;

            return JsonConvert.DeserializeObject<IList<string>>(source.LoginEvents.Description,
                new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Utc });
        }
    }

    public class AuditEventDescriptionValueResolver : IValueResolver<AuditEventQuery, AuditEvent, IList<string>>
    {
        public IList<string> Resolve(AuditEventQuery source, AuditEvent destination, IList<string> destMember, ResolutionContext context)
        {
            if (source.AuditEvent.Description == null) return null;

            return JsonConvert.DeserializeObject<IList<string>>(source.AuditEvent.Description,
                new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Utc });
        }
    }
}
