using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Api.Routing
{
    public class FormatIndexRouteAttribute : RouteAttribute
    {
        public FormatIndexRouteAttribute(bool format = true) : base($"[controller]{(format ? ".{format}" : "")}")
        {
        }
    }
}
