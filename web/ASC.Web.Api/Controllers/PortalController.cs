using System;
using System.Linq;
using ASC.Api.Core;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.MessagingSystem;
using ASC.Web.Api.Routing;
using ASC.Web.Core.Utility;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Api.Controllers
{
    [DefaultRoute]
    [ApiController]
    public class PortalController : ControllerBase
    {
        public Tenant Tenant { get { return ApiContext.Tenant; } }

        public ApiContext ApiContext { get; }
        public UserManager UserManager { get; }
        public AuthContext AuthContext { get; }
        public LogManager LogManager { get; }
        public MessageService MessageService { get; }
        public StudioNotifyService StudioNotifyService { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }


        public PortalController(LogManager logManager,
            MessageService messageService,
            StudioNotifyService studioNotifyService,
            ApiContext apiContext,
            UserManager userManager,
            AuthContext authContext
            )
        {
            LogManager = logManager;
            MessageService = messageService;
            StudioNotifyService = studioNotifyService;
            ApiContext = apiContext;
            UserManager = userManager;
            AuthContext = authContext;
        }

        [Read("")]
        public Tenant Get()
        {
            return Tenant;
        }

        [Read("users/{userID}")]
        public UserInfo GetUser(Guid userID)
        {
            return UserManager.GetUsers(Tenant.TenantId, userID);
        }

        [Read("users/invite/{employeeType}")]
        public string GeInviteLink(EmployeeType employeeType)
        {
            return CommonLinkUtility.GetConfirmationUrl(Tenant.TenantId, string.Empty, ConfirmType.LinkInvite, (int)employeeType, AuthContext.CurrentAccount.ID)
                   + $"&emplType={employeeType:d}";
        }

        [Update("getshortenlink")]
        public string GetShortenLink(string link)
        {
            try
            {
                return UrlShortener.Instance.GetShortenLink(link);
            }
            catch (Exception ex)
            {
                LogManager.Get("ASC.Web").Error("getshortenlink", ex);
                return link;
            }
        }


        [Read("usedspace")]
        public double GetUsedSpace()
        {
            return Math.Round(
                CoreContext.TenantManager.FindTenantQuotaRows(new TenantQuotaRowQuery(Tenant.TenantId))
                           .Where(q => !string.IsNullOrEmpty(q.Tag) && new Guid(q.Tag) != Guid.Empty)
                           .Sum(q => q.Counter) / 1024f / 1024f / 1024f, 2);
        }


        [Read("userscount")]
        public long GetUsersCount()
        {
            return UserManager.GetUserNames(Tenant, EmployeeStatus.Active).Count();
        }

        [Read("tariff")]
        public Tariff GetTariff()
        {
            return CoreContext.PaymentManager.GetTariff(Tenant.TenantId);
        }

        [Read("quota")]
        public TenantQuota GetQuota()
        {
            return CoreContext.TenantManager.GetTenantQuota(Tenant.TenantId);
        }

        [Read("quota/right")]
        public TenantQuota GetRightQuota()
        {
            var usedSpace = GetUsedSpace();
            var needUsersCount = GetUsersCount();

            return CoreContext.TenantManager.GetTenantQuotas().OrderBy(r => r.Price)
                              .FirstOrDefault(quota =>
                                              quota.ActiveUsers > needUsersCount
                                              && quota.MaxTotalSize > usedSpace
                                              && !quota.Year);
        }


        [Read("path")]
        public string GetFullAbsolutePath(string virtualPath)
        {
            return CommonLinkUtility.GetFullAbsolutePath(ApiContext.HttpContext, virtualPath);
        }
    }
}