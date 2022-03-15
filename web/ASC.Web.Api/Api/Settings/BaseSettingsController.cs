namespace ASC.Web.Api.Controllers.Settings;

[Scope]
[DefaultRoute]
[ApiController]
[ControllerName("settings")]
public partial class BaseSettingsController : ControllerBase
{
    //private const int ONE_THREAD = 1;

    //private static readonly DistributedTaskQueue quotaTasks = new DistributedTaskQueue("quotaOperations", ONE_THREAD);
    //private static DistributedTaskQueue LDAPTasks { get; } = new DistributedTaskQueue("ldapOperations");
    //private static DistributedTaskQueue SMTPTasks { get; } = new DistributedTaskQueue("smtpOperations");

    internal readonly ApiContext _apiContext;
    internal readonly IMemoryCache _memoryCache;
    internal readonly WebItemManager _webItemManager;

    private readonly int _maxCount = 10;
    private readonly int _expirationMinutes = 2;

    public BaseSettingsController(ApiContext apiContext, IMemoryCache memoryCache, WebItemManager webItemManager)
    {
        _apiContext = apiContext;
        _memoryCache = memoryCache;
        _webItemManager = webItemManager;
    }

    internal void CheckCache(string basekey)
    {
        var key = _apiContext.HttpContextAccessor.HttpContext.Request.GetUserHostAddress() + basekey;
        if (_memoryCache.TryGetValue<int>(key, out var count))
        {
            if (count > _maxCount)
                throw new Exception(Resource.ErrorRequestLimitExceeded);
        }

        _memoryCache.Set(key, count + 1, TimeSpan.FromMinutes(_expirationMinutes));
    }

    internal string GetProductName(Guid productId)
    {
        var product = _webItemManager[productId];
        return productId == Guid.Empty ? "All" : product != null ? product.Name : productId.ToString();
    }
}