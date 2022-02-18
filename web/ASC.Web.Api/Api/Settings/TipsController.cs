using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Api.Controllers.Settings;

public class TipsController: BaseSettingsController
{
    private Tenant Tenant { get { return _apiContext.Tenant; } }

    private readonly AuthContext _authContext;
    private readonly StudioNotifyHelper _studioNotifyHelper;
    private readonly SettingsManager _settingsManager;
    private readonly SetupInfo _setupInfo;
    private readonly ILog _log;
    private readonly IHttpClientFactory _clientFactory;

    public TipsController(
        IOptionsMonitor<ILog> option,
        ApiContext apiContext,
        AuthContext authContext,
        StudioNotifyHelper studioNotifyHelper,
        SettingsManager settingsManager,
        WebItemManager webItemManager,
        SetupInfo setupInfo,
        IMemoryCache memoryCache,
        IHttpClientFactory clientFactory) : base(apiContext, memoryCache, webItemManager)
    {
        _log = option.Get("ASC.Api");
        _authContext = authContext;
        _studioNotifyHelper = studioNotifyHelper;
        _settingsManager = settingsManager;
        _setupInfo = setupInfo;
        _clientFactory = clientFactory;
    }

    [Update("tips")]
    public TipsSettings UpdateTipsSettingsFromBody([FromBody] SettingsDto model)
    {
        return UpdateTipsSettings(model);
    }

    [Update("tips")]
    [Consumes("application/x-www-form-urlencoded")]
    public TipsSettings UpdateTipsSettingsFromForm([FromForm] SettingsDto model)
    {
        return UpdateTipsSettings(model);
    }

    private TipsSettings UpdateTipsSettings(SettingsDto model)
    {
        var settings = new TipsSettings { Show = model.Show };
        _settingsManager.SaveForCurrentUser(settings);

        if (!model.Show && !string.IsNullOrEmpty(_setupInfo.TipsAddress))
        {
            try
            {
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri($"{_setupInfo.TipsAddress}/tips/deletereaded");

                var data = new NameValueCollection
                {
                    ["userId"] = _authContext.CurrentAccount.ID.ToString(),
                    ["tenantId"] = Tenant.TenantId.ToString(CultureInfo.InvariantCulture)
                };
                var body = JsonSerializer.Serialize(data);//todo check
                request.Content = new StringContent(body);

                var httpClient = _clientFactory.CreateClient();
                using var response = httpClient.Send(request);

            }
            catch (Exception e)
            {
                _log.Error(e.Message, e);
            }
        }

        return settings;
    }

    [Update("tips/change/subscription")]
    public bool UpdateTipsSubscription()
    {
        return StudioPeriodicNotify.ChangeSubscription(_authContext.CurrentAccount.ID, _studioNotifyHelper);
    }
}
