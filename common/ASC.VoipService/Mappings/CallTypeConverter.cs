namespace ASC.VoipService.Mappings;

[Scope]
public class CallTypeConverter : ITypeConverter<CallContact, VoipCall>
{
    public VoipCall Convert(CallContact source, VoipCall destination, ResolutionContext context)
    {
        var result = context.Mapper.Map<DbVoipCall, VoipCall>(source.DbVoipCall);
        result.VoipRecord = context.Mapper.Map<DbVoipCall, VoipRecord>(source.DbVoipCall);

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
        services.TryAdd<CallTypeConverter>();
    }
}