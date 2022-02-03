using System.Collections.Generic;
using System;

using AutoMapper;

using Newtonsoft.Json;
using ASC.MessagingSystem;
using ASC.Core.Users;
using ASC.AuditTrail.Mappers;

namespace ASC.AuditTrail.Models.Mapping.Actions
{
    public class AuditEventMappingAction : IMappingAction<AuditEventQuery, AuditEventDto>
    {
        private MessageTarget _messageTarget;
        private UserFormatter _userFormatter;
        private AuditActionMapper _auditActionMapper;

        public AuditEventMappingAction(
            MessageTarget messageTarget, 
            UserFormatter userFormatter, 
            AuditActionMapper auditActionMapper)
        {
            _messageTarget = messageTarget;
            _userFormatter = userFormatter;
            _auditActionMapper = auditActionMapper;
        }

        public void Process(AuditEventQuery source, AuditEventDto destination, ResolutionContext context)
        {
            if (source.AuditEvent.Description != null)
                destination.Description = JsonConvert.DeserializeObject<IList<string>>(
                   Convert.ToString(source.AuditEvent.Description),
                   new JsonSerializerSettings { DateTimeZoneHandling = DateTimeZoneHandling.Utc });

            destination.Target = _messageTarget.Parse(source.AuditEvent.Target);

            destination.UserName = (source.User.FirstName != null && source.User.LastName != null) ? _userFormatter.GetUserName(source.User.FirstName, source.User.LastName) :
                    source.AuditEvent.UserId == Core.Configuration.Constants.CoreSystem.ID ? AuditReportResource.SystemAccount :
                        source.AuditEvent.UserId == Core.Configuration.Constants.Guest.ID ? AuditReportResource.GuestAccount :
                            source.AuditEvent.Initiator ?? AuditReportResource.UnknownAccount;

            destination.ActionText = _auditActionMapper.GetActionText(destination);
            destination.ActionTypeText = _auditActionMapper.GetActionTypeText(destination);
            destination.Product = _auditActionMapper.GetProductText(destination);
            destination.Module = _auditActionMapper.GetModuleText(destination);
        }
    }
}