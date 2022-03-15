namespace ASC.Core.Tenants;

[Serializable]
public class TenantQuotaException : Exception
{
    public TenantQuotaException(string message)
        : base(message) { }

    protected TenantQuotaException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
