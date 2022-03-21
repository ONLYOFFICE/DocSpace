using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;

using ASC.Api.Core;
using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Common.Notify.Push;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Web.Api.Models;
using ASC.Web.Api.Routing;
using ASC.Web.Core;
using ASC.Web.Core.Files;
using ASC.Web.Core.Mobile;
using ASC.Web.Core.Utility;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Api.Controllers
{
    [Scope]
    [DefaultRoute]
    [ApiController]
    public class PortalController : ControllerBase
    {

        private Tenant Tenant { get { return ApiContext.Tenant; } }

        private ApiContext ApiContext { get; }
        private UserManager UserManager { get; }
        private TenantManager TenantManager { get; }
        private PaymentManager PaymentManager { get; }
        private CommonLinkUtility CommonLinkUtility { get; }
        private UrlShortener UrlShortener { get; }
        private AuthContext AuthContext { get; }
        private WebItemSecurity WebItemSecurity { get; }
        private SecurityContext SecurityContext { get; }
        private SettingsManager SettingsManager { get; }
        private IMobileAppInstallRegistrator MobileAppInstallRegistrator { get; }
        private IConfiguration Configuration { get; set; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private LicenseReader LicenseReader { get; }
        private SetupInfo SetupInfo { get; }
        private DocumentServiceLicense DocumentServiceLicense { get; }
        private TenantExtra TenantExtra { get; set; }
        public ILog Log { get; }
        public IHttpClientFactory ClientFactory { get; }


        public PortalController(
            IOptionsMonitor<ILog> options,
            ApiContext apiContext,
            UserManager userManager,
            TenantManager tenantManager,
            PaymentManager paymentManager,
            CommonLinkUtility commonLinkUtility,
            UrlShortener urlShortener,
            AuthContext authContext,
            WebItemSecurity webItemSecurity,
            SecurityContext securityContext,
            SettingsManager settingsManager,
            IMobileAppInstallRegistrator mobileAppInstallRegistrator,
            TenantExtra tenantExtra,
            IConfiguration configuration,
            CoreBaseSettings coreBaseSettings,
            LicenseReader licenseReader,
            SetupInfo setupInfo,
            DocumentServiceLicense documentServiceLicense,
            IHttpClientFactory clientFactory
            )
        {
            Log = options.CurrentValue;
            ApiContext = apiContext;
            UserManager = userManager;
            TenantManager = tenantManager;
            PaymentManager = paymentManager;
            CommonLinkUtility = commonLinkUtility;
            UrlShortener = urlShortener;
            AuthContext = authContext;
            WebItemSecurity = webItemSecurity;
            SecurityContext = securityContext;
            SettingsManager = settingsManager;
            MobileAppInstallRegistrator = mobileAppInstallRegistrator;
            Configuration = configuration;
            CoreBaseSettings = coreBaseSettings;
            LicenseReader = licenseReader;
            SetupInfo = setupInfo;
            DocumentServiceLicense = documentServiceLicense;
            TenantExtra = tenantExtra;
            ClientFactory = clientFactory;
        }

        [Read("")]
        public Tenant Get()
        {
            return Tenant;
        }

        [Read("users/{userID}")]
        public UserInfo GetUser(Guid userID)
        {
            return UserManager.GetUsers(userID);
        }

        [Read("users/invite/{employeeType}")]
        public object GeInviteLink(EmployeeType employeeType)
        {
            if (!WebItemSecurity.IsProductAdministrator(WebItemManager.PeopleProductID, AuthContext.CurrentAccount.ID))
            {
                throw new SecurityException("Method not available");
            }

            return CommonLinkUtility.GetConfirmationUrl(string.Empty, ConfirmType.LinkInvite, (int)employeeType)
                   + $"&emplType={employeeType:d}";
        }

        [Update("getshortenlink")]
        public async Task<object> GetShortenLinkAsync(ShortenLinkModel model)
        {
            try
            {
                return await UrlShortener.Instance.GetShortenLinkAsync(model.Link);
            }
            catch (Exception ex)
            {
                Log.Error("getshortenlink", ex);
                return model.Link;
            }
        }

        [Read("tenantextra")]
        public async Task<object> GetTenantExtraAsync()
        {
            return new
            {
                customMode = CoreBaseSettings.CustomMode,
                opensource = TenantExtra.Opensource,
                enterprise = TenantExtra.Enterprise,
                tariff = TenantExtra.GetCurrentTariff(),
                quota = TenantExtra.GetTenantQuota(),
                notPaid = TenantExtra.IsNotPaid(),
                licenseAccept = SettingsManager.LoadForCurrentUser<TariffSettings>().LicenseAcceptSetting,
                enableTariffPage = //TenantExtra.EnableTarrifSettings - think about hide-settings for opensource
                    (!CoreBaseSettings.Standalone || !string.IsNullOrEmpty(LicenseReader.LicensePath))
                    && string.IsNullOrEmpty(SetupInfo.AmiMetaUrl)
                    && !CoreBaseSettings.CustomMode,
                DocServerUserQuota = await DocumentServiceLicense.GetLicenseQuotaAsync(),
                DocServerLicense = await DocumentServiceLicense.GetLicenseAsync()
            };
        }


        [Read("usedspace")]
        public double GetUsedSpace()
        {
            return Math.Round(
                TenantManager.FindTenantQuotaRows(Tenant.TenantId)
                           .Where(q => !string.IsNullOrEmpty(q.Tag) && new Guid(q.Tag) != Guid.Empty)
                           .Sum(q => q.Counter) / 1024f / 1024f / 1024f, 2);
        }


        [Read("userscount")]
        public long GetUsersCount()
        {
            return CoreBaseSettings.Personal ? 1 : UserManager.GetUserNames(EmployeeStatus.Active).Length;
        }

        [Read("tariff")]
        public Tariff GetTariff()
        {
            return PaymentManager.GetTariff(Tenant.TenantId);
        }

        [Read("quota")]
        public TenantQuota GetQuota()
        {
            return TenantManager.GetTenantQuota(Tenant.TenantId);
        }

        [Read("quota/right")]
        public TenantQuota GetRightQuota()
        {
            var usedSpace = GetUsedSpace();
            var needUsersCount = GetUsersCount();

            return TenantManager.GetTenantQuotas().OrderBy(r => r.Price)
                              .FirstOrDefault(quota =>
                                              quota.ActiveUsers > needUsersCount
                                              && quota.MaxTotalSize > usedSpace
                                              && !quota.Year);
        }


        [Read("path")]
        public object GetFullAbsolutePath(string virtualPath)
        {
            return CommonLinkUtility.GetFullAbsolutePath(virtualPath);
        }

        [Read("thumb")]
        public FileResult GetThumb(string url)
        {
            if (!SecurityContext.IsAuthenticated || Configuration["bookmarking:thumbnail-url"] == null)
            {
                return null;
            }

            url = url.Replace("&amp;", "&");
            url = WebUtility.UrlEncode(url);

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(string.Format(Configuration["bookmarking:thumbnail-url"], url));

            var httpClient = ClientFactory.CreateClient();
            using var response = httpClient.Send(request);
            using var stream = response.Content.ReadAsStream();
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);

            string type;
            if (response.Headers.TryGetValues("Content-Type", out var values))
            {
                type = values.First();
            }
            else
            {
                type = "image/png";
            }
            return File(bytes, type);
        }

        [Create("present/mark")]
        public void MarkPresentAsReaded()
        {
            try
            {
                var settings = SettingsManager.LoadForCurrentUser<OpensourceGiftSettings>();
                settings.Readed = true;
                SettingsManager.SaveForCurrentUser(settings);
            }
            catch (Exception ex)
            {
                Log.Error("MarkPresentAsReaded", ex);
            }
        }

        [Create("mobile/registration")]
        public void RegisterMobileAppInstallFromBody([FromBody] MobileAppModel model)
        {
            var currentUser = UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            MobileAppInstallRegistrator.RegisterInstall(currentUser.Email, model.Type);
        }

        [Create("mobile/registration")]
        [Consumes("application/x-www-form-urlencoded")]
        public void RegisterMobileAppInstallFromForm([FromForm] MobileAppModel model)
        {
            var currentUser = UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            MobileAppInstallRegistrator.RegisterInstall(currentUser.Email, model.Type);
        }

        [Create("mobile/registration")]
        public void RegisterMobileAppInstall(MobileAppType type)
        {
            var currentUser = UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            MobileAppInstallRegistrator.RegisterInstall(currentUser.Email, type);
        }
    }
}