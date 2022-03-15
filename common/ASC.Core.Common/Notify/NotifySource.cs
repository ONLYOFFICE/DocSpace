namespace ASC.Core.Notify;

public abstract class NotifySource : INotifySource
{
    private readonly IDictionary<CultureInfo, IActionProvider> _actions = new Dictionary<CultureInfo, IActionProvider>();
    private readonly IDictionary<CultureInfo, IPatternProvider> _patterns = new Dictionary<CultureInfo, IPatternProvider>();

    protected ISubscriptionProvider SubscriprionProvider;
    protected IRecipientProvider RecipientsProvider;
    protected IActionProvider ActionProvider => GetActionProvider();
    protected IPatternProvider PatternProvider => GetPatternProvider();
    public string Id { get; private set; }

    private readonly UserManager _userManager;
    private readonly SubscriptionManager _subscriptionManager;

    protected NotifySource(string id, UserManager userManager, IRecipientProvider recipientsProvider, SubscriptionManager subscriptionManager)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(id);

        Id = id;
        _userManager = userManager;
        RecipientsProvider = recipientsProvider;
        _subscriptionManager = subscriptionManager;
    }

    protected NotifySource(Guid id, UserManager userManager, IRecipientProvider recipientsProvider, SubscriptionManager subscriptionManager)
        : this(id.ToString(), userManager, recipientsProvider, subscriptionManager)
    {
    }

    public IActionProvider GetActionProvider()
    {
        lock (_actions)
        {
            var culture = Thread.CurrentThread.CurrentCulture;
            if (!_actions.ContainsKey(culture))
            {
                _actions[culture] = CreateActionProvider();
            }

            return _actions[culture];
        }
    }

    public IPatternProvider GetPatternProvider()
    {
        lock (_patterns)
        {
            var culture = Thread.CurrentThread.CurrentCulture;
            if (Thread.CurrentThread.CurrentUICulture != culture)
            {
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            if (!_patterns.ContainsKey(culture))
            {
                _patterns[culture] = CreatePatternsProvider();
            }

            return _patterns[culture];
        }
    }

    public IRecipientProvider GetRecipientsProvider()
    {
        return CreateRecipientsProvider();
    }

    public ISubscriptionProvider GetSubscriptionProvider()
    {
        return CreateSubscriptionProvider();
    }

    protected abstract IPatternProvider CreatePatternsProvider();

    protected abstract IActionProvider CreateActionProvider();


    protected virtual ISubscriptionProvider CreateSubscriptionProvider()
    {
        var subscriptionProvider = new DirectSubscriptionProvider(Id, _subscriptionManager, RecipientsProvider);

        return new TopSubscriptionProvider(RecipientsProvider, subscriptionProvider, WorkContext.DefaultClientSenders)
            ?? throw new NotifyException("Provider ISubscriprionProvider not instanced.");
    }

    protected virtual IRecipientProvider CreateRecipientsProvider()
    {
        return new RecipientProviderImpl(_userManager)
            ?? throw new NotifyException("Provider IRecipientsProvider not instanced.");
    }
}
