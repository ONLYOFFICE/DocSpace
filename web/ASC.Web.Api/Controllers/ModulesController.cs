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
        public UserManager UserManager { get; }
        public TenantManager TenantManager { get; }
        public WebItemSecurity WebItemSecurity { get; }
        public AuthContext AuthContext { get; }

        public ModulesController(
            UserManager userManager, 
            TenantManager tenantManager, 
            WebItemSecurity webItemSecurity,
            AuthContext authContext)
        {
            UserManager = userManager;
            TenantManager = tenantManager;
            WebItemSecurity = webItemSecurity;
            AuthContext = authContext;
        }

        [Read]
        public IEnumerable<string> GetAll()
        {
            var result = new List<string>();

            foreach (var a in WebItemManager.Instance.GetItems(TenantManager.GetCurrentTenant(), WebZoneType.StartProductList, WebItemSecurity, AuthContext))
            {
                result.Add(a.ApiURL);
            }

            return result;
        }
    }
}
