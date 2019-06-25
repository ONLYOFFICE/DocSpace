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
        public ReadAttribute(bool check = true) :
            base("GET", $"[controller]", check)
        {
        }
        public ReadAttribute(string template, bool check = true, int order = 1) :
            base("GET", $"[controller]/{template}", check) => Order = order;
    }
    public class CreateAttribute : CustomHttpMethodAttribute
    {
        public CreateAttribute(bool check = true) :
            base("POST", $"[controller]", check)
        {
        }
        public CreateAttribute(string template, bool check = true, int order = 1) :
            base("POST", $"[controller]/{template}", check) => Order = order;
    }
    public class UpdateAttribute : CustomHttpMethodAttribute
    {
        public UpdateAttribute(bool check = true) :
            base("PUT", $"[controller]", check)
        {
        }

        public UpdateAttribute(string template, bool check = true, int order = 1) :
            base("PUT" ,$"[controller]/{template}", check) => Order = order;
    }
    public class DeleteAttribute : CustomHttpMethodAttribute
    {
        public DeleteAttribute(bool check = true) :
            base("DELETE", $"[controller]", check)
        {
        }
        public DeleteAttribute(string template, bool check = true, int order = 1) :
            base("DELETE", $"[controller]/{template}", check) => Order = order;
    }
}
