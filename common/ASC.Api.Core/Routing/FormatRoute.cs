using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Api.Routing
{
    public class FormatRouteAttribute : RouteAttribute
    {
        public FormatRouteAttribute(string template) : base($"[controller]/{template}.{{format?}}")
        {
        }
    }
}
