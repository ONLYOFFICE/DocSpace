namespace ASC.Notify.Engine;

class SingleRecipientInterceptor : ISendInterceptor
{
    private const string Prefix = "__singlerecipientinterceptor";
    private readonly List<IRecipient> _sendedTo = new List<IRecipient>(10);

    public string Name { get; private set; }
    public InterceptorPlace PreventPlace => InterceptorPlace.GroupSend | InterceptorPlace.DirectSend;
    public InterceptorLifetime Lifetime => InterceptorLifetime.Call;


    internal SingleRecipientInterceptor(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException(nameof(name));
        }

        Name = name;
    }

    public bool PreventSend(NotifyRequest request, InterceptorPlace place, IServiceScope serviceScope)
    {
        var sendTo = request.Recipient;
        if (!_sendedTo.Exists(rec => Equals(rec, sendTo)))
        {
            _sendedTo.Add(sendTo);

            return false;
        }

        return true;
    }
}
