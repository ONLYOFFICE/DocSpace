using System.Collections.Generic;

using ASC.AuditTrail.Mappers;
using ASC.Core.Users;

using AutoMapper;

using Newtonsoft.Json;

namespace ASC.AuditTrail.Models.Mapping.Actions
{
    public class LoginEventMappingAction : IMappingAction<LoginEventQuery, LoginEventDTO>
    {
        private readonly UserFormatter _userFormatter;
        private readonly AuditActionMapper _auditActionMapper;

        public LoginEventMappingAction(UserFormatter userFormatter, AuditActionMapper auditActionMapper)
        {
            _userFormatter = userFormatter;
            _auditActionMapper = auditActionMapper;
        }

        public void Process(LoginEventQuery source, LoginEventDTO destination, ResolutionContext context)
        {
            if (source.LoginEvents.Description != null)
                destination.Description = JsonConvert.DeserializeObject<IList<string>>(
                    source.LoginEvents.Description,
                    new JsonSerializerSettings
                    {
                        DateTimeZoneHandling = DateTimeZoneHandling.Utc
                    });

            destination.UserName = (!string.IsNullOrEmpty(source.User?.FirstName) && !string.IsNullOrEmpty(source.User?.LastName))
                                ? _userFormatter.GetUserName(source.User.FirstName, source.User.LastName)
                                : !string.IsNullOrWhiteSpace(source.LoginEvents.Login)
                                        ? source.LoginEvents.Login
                                        : source.LoginEvents.UserId == Core.Configuration.Constants.Guest.ID
                                            ? AuditReportResource.GuestAccount
                                            : AuditReportResource.UnknownAccount;

            destination.ActionText = _auditActionMapper.GetActionText(destination);
        }
    }
}