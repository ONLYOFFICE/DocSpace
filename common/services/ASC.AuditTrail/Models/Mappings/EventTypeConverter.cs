namespace ASC.AuditTrail.Models.Mappings;

public class EventTypeConverter : ITypeConverter<LoginEventQuery, LoginEventDto>, 
                                  ITypeConverter<AuditEventQuery, AuditEventDto>
{
    private readonly UserFormatter _userFormatter;
    private readonly AuditActionMapper _auditActionMapper;
    private readonly MessageTarget _messageTarget;

    public EventTypeConverter(
        UserFormatter userFormatter,
        AuditActionMapper actionMapper,
        MessageTarget messageTarget)
    {
        _userFormatter = userFormatter;
        _auditActionMapper = actionMapper;
        _messageTarget = messageTarget;
    }

    public LoginEventDto Convert(LoginEventQuery source, LoginEventDto destination, ResolutionContext context)
    {
        var result = context.Mapper.Map<LoginEventDto>(source.Event);

        if (source.Event.DescriptionRaw != null)
        {
            result.Description = JsonConvert.DeserializeObject<IList<string>>(source.Event.DescriptionRaw,
                new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });
        }

        result.UserName = (!string.IsNullOrEmpty(source.FirstName) && !string.IsNullOrEmpty(source.LastName))
                        ? _userFormatter.GetUserName(source.FirstName, source.LastName)
                        : !string.IsNullOrWhiteSpace(source.Event.Login)
                                ? source.Event.Login
                                : source.Event.UserId == Core.Configuration.Constants.Guest.ID
                                    ? AuditReportResource.GuestAccount
                                    : AuditReportResource.UnknownAccount;

        result.ActionText = _auditActionMapper.GetActionText(result);

        return result;
    }

    public AuditEventDto Convert(AuditEventQuery source, AuditEventDto destination, ResolutionContext context)
    {
        var result = context.Mapper.Map<AuditEventDto>(source.Event);

        if (source.Event.DescriptionRaw != null)
        {
            result.Description = JsonConvert.DeserializeObject<IList<string>>(
               JsonConvert.ToString(source.Event.DescriptionRaw),
               new JsonSerializerSettings
               {
                   DateTimeZoneHandling = DateTimeZoneHandling.Utc
               });
        }

        result.Target = _messageTarget.Parse(source.Event.Target);

        result.UserName = (source.FirstName != null && source.LastName != null) ? _userFormatter.GetUserName(source.FirstName, source.LastName) :
                source.Event.UserId == Core.Configuration.Constants.CoreSystem.ID ? AuditReportResource.SystemAccount :
                    source.Event.UserId == Core.Configuration.Constants.Guest.ID ? AuditReportResource.GuestAccount :
                        source.Event.Initiator ?? AuditReportResource.UnknownAccount;

        result.ActionText = _auditActionMapper.GetActionText(result);
        result.ActionTypeText = _auditActionMapper.GetActionTypeText(result);
        result.Product = _auditActionMapper.GetProductText(result);
        result.Module = _auditActionMapper.GetModuleText(result);

        return result;
    }
}
