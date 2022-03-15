namespace ASC.Common.Web;

public class DisposableHttpContext : IDisposable
{
    private const string Key = "disposable.key";

    public object this[string key]
    {
        get => Items.ContainsKey(key) ? Items[key] : null;
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }

            if (!(value is IDisposable))
            {
                throw new ArgumentException("Only IDisposable may be added!");
            }

            Items[key] = (IDisposable)value;
        }
    }

    private Dictionary<string, IDisposable> Items
    {
        get
        {
            var table = (Dictionary<string, IDisposable>)_context.Items[Key];

            if (table == null)
            {
                table = new Dictionary<string, IDisposable>(1);
                _context.Items.Add(Key, table);
            }

            return table;
        }
    }

    private readonly HttpContext _context;
    private bool _isDisposed;

    public DisposableHttpContext(HttpContext ctx)
    {
        ArgumentNullException.ThrowIfNull(ctx);
        _context = ctx;
    }

    public void Dispose()
    {
        if (!_isDisposed)
        {
            foreach (var item in Items.Values)
            {
                try
                {
                    item.Dispose();
                }
                catch { }
            }

            _isDisposed = true;
        }
    }
}