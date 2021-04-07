
using ASC.Api.Core;
using ASC.Common;
using ASC.CRM.Configuration;
using ASC.Web.Api.Routing;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.Controllers
{
    [Scope]
    [DefaultRoute]
    [ApiController]
    public class CRMController : ControllerBase
    {
        private ProductEntryPoint ProductEntryPoint { get; }

        public CRMController(ProductEntryPoint productEntryPoint)
        {
            ProductEntryPoint = productEntryPoint;
        }

        [Read("info")]
        public Module GetModule()
        {
            ProductEntryPoint.Init();
            return new Module(ProductEntryPoint);
        }
    }
}
