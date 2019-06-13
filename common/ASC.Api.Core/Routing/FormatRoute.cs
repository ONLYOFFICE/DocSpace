using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Api.Routing
{
    public class FormatRouteAttribute : RouteAttribute
    {
        public FormatRouteAttribute(bool format = true) :
            base($"[controller]{(format ? ".{format}" : "")}")
        {
        }
        public FormatRouteAttribute(string template, bool format = true) : 
            base($"[controller]/{template}{(!format ? "": (template.EndsWith("}") ? ".{format?}" : ".{format}"))}")
        {
        }
    }
}
