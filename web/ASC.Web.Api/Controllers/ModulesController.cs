using System.Collections.Generic;

using ASC.Common;
using ASC.Web.Api.Routing;
using ASC.Web.Core;
using ASC.Web.Core.WebZones;

using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Api.Controllers
{
    [Scope]
    [DefaultRoute]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private WebItemManagerSecurity WebItemManagerSecurity { get; }

        public ModulesController(
            WebItemManagerSecurity webItemManagerSecurity)
        {
            WebItemManagerSecurity = webItemManagerSecurity;
        }

        [Read]
        public IEnumerable<string> GetAll()
        {
            var result = new List<string>();

            foreach (var a in WebItemManagerSecurity.GetItems(WebZoneType.StartProductList))
            {
                result.Add(a.ApiURL);
            }

            return result;
        }
    }
}
