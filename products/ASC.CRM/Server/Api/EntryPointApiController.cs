
using ASC.Api.Core;
using ASC.Api.Core.Convention;
using ASC.Common;
using ASC.Web.Api.Routing;
using ASC.Web.CRM.Configuration;

using Microsoft.AspNetCore.Mvc;

namespace ASC.CRM.Api
{
    [Scope]
    [DefaultRoute]
    [ApiController]
    [ControllerName("crm")]
    public class EntryPointApiController : ControllerBase
    {
        private ProductEntryPoint ProductEntryPoint { get; }

        public EntryPointApiController(ProductEntryPoint productEntryPoint)
        {
            ProductEntryPoint = productEntryPoint;
        }

        [HttpGet("info")]
        public Module GetModule()
        {
            ProductEntryPoint.Init();
            return new Module(ProductEntryPoint);
        }
    }
}
