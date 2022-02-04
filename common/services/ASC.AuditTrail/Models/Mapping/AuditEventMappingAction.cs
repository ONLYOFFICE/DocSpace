namespace ASC.AuditTrail.Models.Mapping.Actions;

public class AuditEventMappingAction : IMappingAction<AuditEventQuery, AuditEvent>
{
    private readonly MessageTarget _messageTarget;
    private readonly UserFormatter _userFormatter;
    private readonly AuditActionMapper _auditActionMapper;

    public AuditEventMappingAction(
        MessageTarget messageTarget,
        UserFormatter userFormatter,
        AuditActionMapper auditActionMapper)
    {
        _messageTarget = messageTarget;
        _userFormatter = userFormatter;
        _auditActionMapper = auditActionMapper;
    }

    public void Process(AuditEventQuery source, AuditEvent destination, ResolutionContext context)
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