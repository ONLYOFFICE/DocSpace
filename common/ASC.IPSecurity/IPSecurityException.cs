namespace ASC.IPSecurity;

public class IPSecurityException : Exception
{
    public IPSecurityException() { }

    public IPSecurityException(string message)
        : base(message) { }

    public IPSecurityException(string message, Exception innerException)
        : base(message, innerException) { }

    public IPSecurityException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}
