namespace ASC.Notify;

public class InitiatorInterceptor : SendInterceptorSkeleton
{
    public InitiatorInterceptor(params IRecipient[] initiators)
        : base("Sys.InitiatorInterceptor", InterceptorPlace.GroupSend | InterceptorPlace.DirectSend, InterceptorLifetime.Call,
            (r, p, scope) => (initiators ?? Enumerable.Empty<IRecipient>()).Any(recipient => r.Recipient.Equals(recipient)))
    {
    }
}
