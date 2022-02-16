/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

using Constants = ASC.Core.Users.Constants;

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
    internal Tenant Tenant { get { return _apiContext.Tenant; } }

    internal readonly ApiContext _apiContext;
    internal readonly MessageService _messageService;
    internal readonly StudioNotifyService _studioNotifyService;
    internal readonly IWebHostEnvironment _webHostEnvironment;
    internal readonly IServiceProvider _serviceProvider;
    internal readonly EmployeeWraperHelper _employeeWraperHelper;
    internal readonly ConsumerFactory _consumerFactory;
    internal readonly SmsProviderManager _smsProviderManager;
    internal readonly TimeZoneConverter _timeZoneConverter;
    internal readonly CustomNamingPeople _customNamingPeople;
    internal readonly IPSecurity.IPSecurity _ipSecurity;
    internal readonly IMemoryCache _memoryCache;
    internal readonly ProviderManager _providerManager;
    internal readonly FirstTimeTenantSettings _firstTimeTenantSettings;
    internal readonly UserManager _userManager;
    internal readonly TenantManager _tenantManager;
    internal readonly TenantExtra _tenantExtra;
    internal readonly TenantStatisticsProvider _tenantStatisticsProvider;
    internal readonly AuthContext _authContext;
    internal readonly CookiesManager _cookiesManager;
    internal readonly WebItemSecurity _webItemSecurity;
    internal readonly StudioNotifyHelper _studioNotifyHelper;
    internal readonly LicenseReader _licenseReader;
    internal readonly PermissionContext _permissionContext;
    internal readonly SettingsManager _settingsManager;
    internal readonly TfaManager _tfaManager;
    internal readonly WebItemManager _webItemManager;
    internal readonly WebItemManagerSecurity _webItemManagerSecurity;
    internal readonly TenantInfoSettingsHelper _tenantInfoSettingsHelper;
    internal readonly TenantWhiteLabelSettingsHelper _tenantWhiteLabelSettingsHelper;
    internal readonly StorageHelper _storageHelper;
    internal readonly TenantLogoManager _tenantLogoManager;
    internal readonly TenantUtil _tenantUtil;
    internal readonly CoreBaseSettings _coreBaseSettings;
    internal readonly CommonLinkUtility _commonLinkUtility;
    internal readonly ColorThemesSettingsHelper _colorThemesSettingsHelper;
    internal readonly IConfiguration _configuration;
    internal readonly SetupInfo _setupInfo;
    internal readonly BuildVersion _buildVersion;
    internal readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    internal readonly StatisticManager _statisticManager;
    internal readonly IPRestrictionsService _iPRestrictionsService;
    internal readonly CoreConfiguration _coreConfiguration;
    internal readonly MessageTarget _messageTarget;
    internal readonly StudioSmsNotificationSettingsHelper _studioSmsNotificationSettingsHelper;
    internal readonly CoreSettings _coreSettings;
    internal readonly StorageSettingsHelper _storageSettingsHelper;
    internal readonly ServiceClient _serviceClient;
    internal readonly StorageFactory _storageFactory;
    internal readonly UrlShortener _urlShortener;
    internal readonly EncryptionServiceClient _encryptionServiceClient;
    internal readonly EncryptionSettingsHelper _encryptionSettingsHelper;
    internal readonly BackupAjaxHandler _backupAjaxHandler;
    internal readonly ICacheNotify<DeleteSchedule> _cacheDeleteSchedule;
    internal readonly EncryptionWorker _encryptionWorker;
    internal readonly PasswordHasher _passwordHasher;
    internal readonly ILog _log;
    internal readonly TelegramHelper _telegramHelper;
    internal readonly PaymentManager _paymentManager;
    internal readonly Constants _constants;
    internal readonly InstanceCrypto _instanceCrypto;
    internal readonly Signature _signature;
    internal readonly DbWorker _webhookDbWorker;
    internal readonly IHttpClientFactory _clientFactory;

    public BaseSettingsController(
        IOptionsMonitor<ILog> option,
        MessageService messageService,
        StudioNotifyService studioNotifyService,
        ApiContext apiContext,
        UserManager userManager,
        TenantManager tenantManager,
        TenantExtra tenantExtra,
        TenantStatisticsProvider tenantStatisticsProvider,
        AuthContext authContext,
        CookiesManager cookiesManager,
        WebItemSecurity webItemSecurity,
        StudioNotifyHelper studioNotifyHelper,
        LicenseReader licenseReader,
        PermissionContext permissionContext,
        SettingsManager settingsManager,
        TfaManager tfaManager,
        WebItemManager webItemManager,
        WebItemManagerSecurity webItemManagerSecurity,
        TenantInfoSettingsHelper tenantInfoSettingsHelper,
        TenantWhiteLabelSettingsHelper tenantWhiteLabelSettingsHelper,
        StorageHelper storageHelper,
        TenantLogoManager tenantLogoManager,
        TenantUtil tenantUtil,
        CoreBaseSettings coreBaseSettings,
        CommonLinkUtility commonLinkUtility,
        ColorThemesSettingsHelper colorThemesSettingsHelper,
        IConfiguration configuration,
        SetupInfo setupInfo,
        BuildVersion buildVersion,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        StatisticManager statisticManager,
        IPRestrictionsService iPRestrictionsService,
        CoreConfiguration coreConfiguration,
        MessageTarget messageTarget,
        StudioSmsNotificationSettingsHelper studioSmsNotificationSettingsHelper,
        CoreSettings coreSettings,
        StorageSettingsHelper storageSettingsHelper,
        IWebHostEnvironment webHostEnvironment,
        IServiceProvider serviceProvider,
        EmployeeWraperHelper employeeWraperHelper,
        ConsumerFactory consumerFactory,
        SmsProviderManager smsProviderManager,
        TimeZoneConverter timeZoneConverter,
        CustomNamingPeople customNamingPeople,
        IPSecurity.IPSecurity ipSecurity,
        IMemoryCache memoryCache,
        ProviderManager providerManager,
        FirstTimeTenantSettings firstTimeTenantSettings,
        ServiceClient serviceClient,
        TelegramHelper telegramHelper,
        StorageFactory storageFactory,
        UrlShortener urlShortener,
        EncryptionServiceClient encryptionServiceClient,
        EncryptionSettingsHelper encryptionSettingsHelper,
        BackupAjaxHandler backupAjaxHandler,
        ICacheNotify<DeleteSchedule> cacheDeleteSchedule,
        EncryptionWorker encryptionWorker,
        PasswordHasher passwordHasher,
        PaymentManager paymentManager,
        Constants constants,
        InstanceCrypto instanceCrypto,
        Signature signature,
        DbWorker dbWorker,
        IHttpClientFactory clientFactory)
    {
        _log = option.Get("ASC.Api");
        _webHostEnvironment = webHostEnvironment;
        _serviceProvider = serviceProvider;
        _employeeWraperHelper = employeeWraperHelper;
        _consumerFactory = consumerFactory;
        _smsProviderManager = smsProviderManager;
        _timeZoneConverter = timeZoneConverter;
        _customNamingPeople = customNamingPeople;
        _ipSecurity = ipSecurity;
        _memoryCache = memoryCache;
        _providerManager = providerManager;
        _firstTimeTenantSettings = firstTimeTenantSettings;
        _messageService = messageService;
        _studioNotifyService = studioNotifyService;
        _apiContext = apiContext;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _tenantExtra = tenantExtra;
        _tenantStatisticsProvider = tenantStatisticsProvider;
        _authContext = authContext;
        _cookiesManager = cookiesManager;
        _webItemSecurity = webItemSecurity;
        _studioNotifyHelper = studioNotifyHelper;
        _licenseReader = licenseReader;
        _permissionContext = permissionContext;
        _settingsManager = settingsManager;
        _tfaManager = tfaManager;
        _webItemManager = webItemManager;
        _webItemManagerSecurity = webItemManagerSecurity;
        _tenantInfoSettingsHelper = tenantInfoSettingsHelper;
        _tenantWhiteLabelSettingsHelper = tenantWhiteLabelSettingsHelper;
        _storageHelper = storageHelper;
        _tenantLogoManager = tenantLogoManager;
        _tenantUtil = tenantUtil;
        _coreBaseSettings = coreBaseSettings;
        _commonLinkUtility = commonLinkUtility;
        _colorThemesSettingsHelper = colorThemesSettingsHelper;
        _configuration = configuration;
        _setupInfo = setupInfo;
        _buildVersion = buildVersion;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _statisticManager = statisticManager;
        _iPRestrictionsService = iPRestrictionsService;
        _coreConfiguration = coreConfiguration;
        _messageTarget = messageTarget;
        _studioSmsNotificationSettingsHelper = studioSmsNotificationSettingsHelper;
        _coreSettings = coreSettings;
        _storageSettingsHelper = storageSettingsHelper;
        _serviceClient = serviceClient;
        _encryptionServiceClient = encryptionServiceClient;
        _encryptionSettingsHelper = encryptionSettingsHelper;
        _backupAjaxHandler = backupAjaxHandler;
        _cacheDeleteSchedule = cacheDeleteSchedule;
        _encryptionWorker = encryptionWorker;
        _passwordHasher = passwordHasher;
        _storageFactory = storageFactory;
        _urlShortener = urlShortener;
        _telegramHelper = telegramHelper;
        _paymentManager = paymentManager;
        _webhookDbWorker = dbWorker;
        _constants = constants;
        _instanceCrypto = instanceCrypto;
        _signature = signature;
        _clientFactory = clientFactory;
    }

    private readonly int maxCount = 10;
    private readonly int expirationMinutes = 2;
    internal void CheckCache(string basekey)
    {
        var key = _apiContext.HttpContextAccessor.HttpContext.Request.GetUserHostAddress() + basekey;
        if (_memoryCache.TryGetValue<int>(key, out var count))
        {
            if (count > maxCount)
                throw new Exception(Resource.ErrorRequestLimitExceeded);
        }

        _memoryCache.Set(key, count + 1, TimeSpan.FromMinutes(expirationMinutes));
    }

    internal string GetProductName(Guid productId)
    {
        var product = _webItemManager[productId];
        return productId == Guid.Empty ? "All" : product != null ? product.Name : productId.ToString();
    }
}