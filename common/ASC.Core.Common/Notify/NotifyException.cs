namespace ASC.Notify;

public class NotifyException : ApplicationException
{
    public NotifyException(string message)
        : base(message)
    {
    }

    public NotifyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    protected NotifyException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
