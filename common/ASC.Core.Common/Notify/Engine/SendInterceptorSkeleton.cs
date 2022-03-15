namespace ASC.Notify.Engine;

public class SendInterceptorSkeleton : ISendInterceptor
{
    private readonly Func<NotifyRequest, InterceptorPlace, IServiceScope, bool> _method;
    public string Name { get; internal set; }
    public InterceptorPlace PreventPlace { get; internal set; }
    public InterceptorLifetime Lifetime { get; internal set; }

    public SendInterceptorSkeleton(string name, InterceptorPlace preventPlace, InterceptorLifetime lifetime, Func<NotifyRequest, InterceptorPlace, IServiceScope, bool> sendInterceptor)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Empty name.", nameof(name));
        }

        ArgumentNullException.ThrowIfNull(sendInterceptor);

        _method = sendInterceptor;
        Name = name;
        PreventPlace = preventPlace;
        Lifetime = lifetime;
    }

    public bool PreventSend(NotifyRequest request, InterceptorPlace place, IServiceScope serviceScope)
    {
        return _method(request, place, serviceScope);
    }
}
