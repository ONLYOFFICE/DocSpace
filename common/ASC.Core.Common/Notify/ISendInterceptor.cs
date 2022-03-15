namespace ASC.Notify;

public interface ISendInterceptor
{
    string Name { get; }
    InterceptorPlace PreventPlace { get; }
    InterceptorLifetime Lifetime { get; }
    bool PreventSend(NotifyRequest request, InterceptorPlace place, IServiceScope serviceScope);
}
