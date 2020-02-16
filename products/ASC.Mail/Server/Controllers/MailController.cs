using ASC.Api.Core;
using ASC.Common.Logging;
using ASC.Core.Tenants;
using ASC.Mail.Models;
using ASC.Mail.Core;
using ASC.Web.Api.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using ASC.Mail.Extensions;
using ASC.Core;

namespace ASC.Mail.Controllers
{
    [DefaultRoute]
    [ApiController]
    public class MailController : ControllerBase
    {
        public int TenantId { 
            get { 
                return ApiContext.Tenant.TenantId; 
            }
        }

        public string UserId { 
            get { 
                return SecurityContext.CurrentAccount.ID.ToString(); 
            } 
        }

        public SecurityContext SecurityContext { get; }

        public ApiContext ApiContext { get; }

        public EngineFactory MailEngineFactory { get; }

        public ILog Log { get; }

        public MailController(
            ApiContext apiContext,
            SecurityContext securityContext,
            IOptionsMonitor<ILog> option,
            EngineFactory engine)
        {
            ApiContext = apiContext;
            SecurityContext = securityContext;
            Log = option.Get("ASC.Api");
            MailEngineFactory = engine; //new EngineFactory(TenantId, UserId, Log);
        }

        [Read("info")]
        public Module GetModule()
        {
            var product = new MailProduct();
            product.Init();
            return new Module(product, false);
        }

        [Read("accounts")]
        public IEnumerable<MailAccountData> GetAccounts()
        {
            var accounts = MailEngineFactory.AccountEngine.GetAccountInfoList();
            return accounts.ToAccountData();
        }
    }

    public static class MailControllerExtention
    {
        public static IServiceCollection AddMailController(this IServiceCollection services)
        {
            return services
                .AddApiContextService()
                .AddSecurityContextService()
                .AddEngineFactoryService();
        }
    }
}