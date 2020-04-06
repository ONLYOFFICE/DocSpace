using ASC.Api.Core;
using ASC.Common.Logging;
using ASC.Mail.Core;
using ASC.Web.Api.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ASC.Core;
using ASC.Common;
using ASC.Mail.Core.Engine;

namespace ASC.Mail.Controllers
{
    [DefaultRoute]
    [ApiController]
    public partial class MailController : ControllerBase
    {
        public int TenantId { 
            get { 
                return TenantManager.GetCurrentTenant().TenantId; 
            }
        }

        public string UserId { 
            get { 
                return SecurityContext.CurrentAccount.ID.ToString(); 
            } 
        }

        public TenantManager TenantManager { get; }
        public SecurityContext SecurityContext { get; }
        public AccountEngine AccountEngine { get; }
        public AlertEngine AlertEngine { get; }
        public DisplayImagesAddressEngine DisplayImagesAddressEngine { get; }
        //public SignatureEngine SignatureEngine { get; }
        //public TagEngine TagEngine { get; }

        public ILog Log { get; }

        public MailController(
            TenantManager tenantManager,
            SecurityContext securityContext,
            AccountEngine accountEngine,
            AlertEngine alertEngine,
            DisplayImagesAddressEngine displayImagesAddressEngine,
            //SignatureEngine signatureEngine,
            //TagEngine tagEngine,
            IOptionsMonitor<ILog> option)
        {
            TenantManager = tenantManager;
            SecurityContext = securityContext;
            AccountEngine = accountEngine;
            AlertEngine = alertEngine;
            DisplayImagesAddressEngine = displayImagesAddressEngine;
            //SignatureEngine = signatureEngine;
            //TagEngine = tagEngine;

            Log = option.Get("ASC.Api");
        }

        [Read("info")]
        public Module GetModule()
        {
            var product = new MailProduct();
            product.Init();
            return new Module(product, false);
        }
    }

    public static class MailControllerExtention
    {
        public static DIHelper AddMailController(this DIHelper services)
        {
            return services
                .AddTenantManagerService()
                .AddSecurityContextService()
                .AddAccountEngineService()
                .AddAlertEngineService()
                .AddDisplayImagesAddressEngineService()
                //.AddSignatureEngineService()
                //.AddTagEngineService()
                ;
        }
    }
}