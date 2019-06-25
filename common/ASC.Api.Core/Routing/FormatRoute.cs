using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Routing;

namespace ASC.Web.Api.Routing
{
    public abstract class CustomHttpMethodAttribute : HttpMethodAttribute
    {
        public bool Check { get; set; }

        public CustomHttpMethodAttribute(string method, string template, bool check = true)
            : base(new List<string>() { method }, template)
        {
            Check = check;
        }
    }

    public class ReadAttribute : CustomHttpMethodAttribute
    {
        public ReadAttribute(bool format = true, bool check = true) :
            base("GET", $"[controller]{(format ? ".{format}" : "")}", check)
        {
        }
        public ReadAttribute(string template, bool format = true, int order = 1, bool check = true) :
            base("GET", $"[controller]/{template}{(!format ? "": (template.EndsWith("}") ? ".{format?}" : ".{format}"))}", check)
        {
            Order = order;
        }
    }
    public class CreateAttribute : CustomHttpMethodAttribute
    {
        public CreateAttribute(bool format = true, bool check = true) :
            base("POST", $"[controller]{(format ? ".{format}" : "")}", check)
        {
        }
        public CreateAttribute(string template, bool format = true, int order = 1, bool check = true) :
            base("POST", $"[controller]/{template}{(!format ? "": (template.EndsWith("}") ? ".{format?}" : ".{format}"))}", check)
        {
            Order = order;
        }
    }
    public class UpdateAttribute : CustomHttpMethodAttribute
    {
        public UpdateAttribute(bool format = true, bool check = true) :
            base("PUT", $"[controller]{(format ? ".{format}" : "")}", check)
        {
        }

        public UpdateAttribute(string template, bool format = true, int order = 1, bool check = true) :
            base("PUT" ,$"[controller]/{template}{(!format ? "": (template.EndsWith("}") ? ".{format?}" : ".{format}"))}", check)
        {
            Order = order;
        }
    }
    public class DeleteAttribute : CustomHttpMethodAttribute
    {
        public DeleteAttribute(bool format = true, bool check = true) :
            base("DELETE", $"[controller]{(format ? ".{format}" : "")}", check)
        {
        }
        public DeleteAttribute(string template, bool format = true, int order = 1, bool check = true) :
            base("DELETE", $"[controller]/{template}{(!format ? "": (template.EndsWith("}") ? ".{format?}" : ".{format}"))}", check)
        {
            Order = order;
        }
    }
}
