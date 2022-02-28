namespace ASC.VoipService.Mappings;

[Scope]
public class CallConverter : ITypeConverter<CallContact, VoipCall>
{
    public VoipCall Convert(CallContact source, VoipCall destination, ResolutionContext context)
    {
        var result = context.Mapper.Map<VoipCall>(source.DbVoipCall);
        result.ParentID = source.DbVoipCall.ParentCallId;
        result.To = source.DbVoipCall.NumberTo;
        result.VoipRecord = new VoipRecord
        {
            Id = source.DbVoipCall.RecordSid,
            Uri = source.DbVoipCall.RecordUrl,
            Duration = source.DbVoipCall.RecordDuration,
            Price = source.DbVoipCall.RecordPrice
        };

        if (source.CrmContact != null)
        {
            result.ContactId = source.CrmContact.Id;
            result.ContactIsCompany = source.CrmContact.IsCompany;
            result.ContactTitle = result.ContactIsCompany
                                    ? source.CrmContact.CompanyName
                                    : source.CrmContact.FirstName == null || source.CrmContact.LastName == null ? null : $"{source.CrmContact.FirstName} {source.CrmContact.LastName}";
        }
        else
        {
            result.ContactId = 0;
        }

        return result;
    }
}

public class EventTypeConverterExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<CallConverter>();
    }
}