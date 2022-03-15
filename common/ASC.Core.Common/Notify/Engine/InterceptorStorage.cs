namespace ASC.Notify.Engine;

class InterceptorStorage
{
    private readonly string _callContextPrefix = "InterceptorStorage.CALLCONTEXT_KEY." + Guid.NewGuid();
    private readonly object _syncRoot = new object();
    private readonly Dictionary<string, ISendInterceptor> _globalInterceptors = new Dictionary<string, ISendInterceptor>(10);

    private Dictionary<string, ISendInterceptor> CallInterceptors
    {
        get
        {
            if (!(CallContext.GetData(_callContextPrefix) is Dictionary<string, ISendInterceptor> storage))
            {
                storage = new Dictionary<string, ISendInterceptor>(10);
                CallContext.SetData(_callContextPrefix, storage);
            }

            return storage;
        }
    }

    public void Add(ISendInterceptor interceptor)
    {
        ArgumentNullException.ThrowIfNull(interceptor);
        if (string.IsNullOrEmpty(interceptor.Name)) throw new ArgumentException("empty name property", nameof(interceptor));

        switch (interceptor.Lifetime)
        {
            case InterceptorLifetime.Call:
                AddInternal(interceptor, CallInterceptors);
                break;
            case InterceptorLifetime.Global:
                AddInternal(interceptor, _globalInterceptors);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public ISendInterceptor Get(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("empty name", nameof(name));
        }

        var result = GetInternal(name, CallInterceptors);
        if (result == null)
        {
            result = GetInternal(name, _globalInterceptors);
        }

        return result;
    }

    public void Remove(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("empty name", nameof(name));
        }

        RemoveInternal(name, CallInterceptors);
        RemoveInternal(name, _globalInterceptors);
    }

    public void Clear()
    {
        Clear(InterceptorLifetime.Call | InterceptorLifetime.Global);
    }

    public void Clear(InterceptorLifetime lifetime)
    {
        lock (_syncRoot)
        {
            if ((lifetime & InterceptorLifetime.Call) == InterceptorLifetime.Call)
            {
                CallInterceptors.Clear();
            }

            if ((lifetime & InterceptorLifetime.Global) == InterceptorLifetime.Global)
            {
                _globalInterceptors.Clear();
            }
        }
    }

    public List<ISendInterceptor> GetAll()
    {
        var result = new List<ISendInterceptor>();
        result.AddRange(CallInterceptors.Values);
        result.AddRange(_globalInterceptors.Values);

        return result;
    }


    private void AddInternal(ISendInterceptor interceptor, Dictionary<string, ISendInterceptor> storage)
    {
        lock (_syncRoot)
        {
            storage[interceptor.Name] = interceptor;
        }
    }

    private ISendInterceptor GetInternal(string name, Dictionary<string, ISendInterceptor> storage)
    {
        ISendInterceptor interceptor;
        lock (_syncRoot)
        {
            storage.TryGetValue(name, out interceptor);
        }

        return interceptor;
    }

    private void RemoveInternal(string name, Dictionary<string, ISendInterceptor> storage)
    {
        lock (_syncRoot)
        {
            storage.Remove(name);
        }
    }
}
