using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Api.Routing
{
    public class ReadAttribute : HttpGetAttribute
    {
        public ReadAttribute(bool format = true) :
            base($"[controller]{(format ? ".{format}" : "")}")
        {
        }
        public ReadAttribute(string template, bool format = true, int order = 1) : 
            base($"[controller]/{template}{(!format ? "": (template.EndsWith("}") ? ".{format?}" : ".{format}"))}")
        {
            Order = order;
        }
    }
    public class CreateAttribute : HttpPostAttribute
    {
        public CreateAttribute(bool format = true) :
            base($"[controller]{(format ? ".{format}" : "")}")
        {
        }
        public CreateAttribute(string template, bool format = true, int order = 1) : 
            base($"[controller]/{template}{(!format ? "": (template.EndsWith("}") ? ".{format?}" : ".{format}"))}")
        {
            Order = order;
        }
    }
    public class UpdateAttribute : HttpPutAttribute
    {
        public UpdateAttribute(bool format = true) :
            base($"[controller]{(format ? ".{format}" : "")}")
        {
        }
        public UpdateAttribute(string template, bool format = true, int order = 1) : 
            base($"[controller]/{template}{(!format ? "": (template.EndsWith("}") ? ".{format?}" : ".{format}"))}")
        {
            Order = order;
        }
    }
    public class DeleteAttribute : HttpDeleteAttribute
    {
        public DeleteAttribute(bool format = true) :
            base($"[controller]{(format ? ".{format}" : "")}")
        {
        }
        public DeleteAttribute(string template, bool format = true, int order = 1) : 
            base($"[controller]/{template}{(!format ? "": (template.EndsWith("}") ? ".{format?}" : ".{format}"))}")
        {
            Order = order;
        }
    }
}
