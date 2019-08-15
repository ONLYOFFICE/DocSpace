using System.Collections.Generic;
using ASC.Core;
using ASC.Web.Api.Routing;
using ASC.Web.Core;
using ASC.Web.Core.WebZones;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Api.Controllers
{
    [DefaultRoute]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        [Read]
        public IEnumerable<string> GetAll()
        {
            var result = new List<string>();

            foreach (var a in WebItemManager.Instance.GetItems(CoreContext.TenantManager.GetCurrentTenant(), WebZoneType.StartProductList))
            {
                result.Add(a.ApiURL);
            }

            return result;
        }
    }
}
