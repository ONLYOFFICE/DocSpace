using System.Collections.Generic;
using System.Net.Http;

using Microsoft.AspNetCore.Mvc.Routing;

namespace ASC.Web.Api.Routing
{
    public abstract class CustomHttpMethodAttribute : HttpMethodAttribute
    {
        public bool Check { get; set; }
        public bool DisableFormat { get; set; }

        protected CustomHttpMethodAttribute(string method, string template = null, bool check = true, int order = 1)
            : base(new List<string>() { method }, $"[controller]{(template != null ? $"/{template}" : "")}")
        {
            Check = check;
            Order = order;
        }
    }

    public class ReadAttribute : CustomHttpMethodAttribute
    {
        public ReadAttribute(bool check = true, int order = 1) :
            this(null, check, order)
        { }

        public ReadAttribute(string template, bool check = true, int order = 1) :
            base(HttpMethod.Get.Method, template, check, order)
        { }
    }
    public class CreateAttribute : CustomHttpMethodAttribute
    {
        public CreateAttribute(bool check = true, int order = 1) :
           this(null, check, order)
        { }
        public CreateAttribute(string template, bool check = true, int order = 1) :
           base(HttpMethod.Post.Method, template, check, order)
        { }
    }
    public class UpdateAttribute : CustomHttpMethodAttribute
    {
        public UpdateAttribute(bool check = true, int order = 1) :
            this(null, check, order)
        { }
        public UpdateAttribute(string template, bool check = true, int order = 1) :
            base(HttpMethod.Put.Method, template, check, order)
        { }
    }
    public class DeleteAttribute : CustomHttpMethodAttribute
    {
        public DeleteAttribute(bool check = true, int order = 1) :
            this(null, check, order)
        { }
        public DeleteAttribute(string template, bool check = true, int order = 1) :
            base(HttpMethod.Delete.Method, template, check, order)
        { }
    }
}
