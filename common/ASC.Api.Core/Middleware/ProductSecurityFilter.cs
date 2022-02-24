using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Notify.Engine;
using ASC.Core;
using ASC.Web.Core;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;

namespace ASC.Api.Core.Middleware
{
    [Scope]
    public class ProductSecurityFilter : IResourceFilter
    {
        private static readonly IDictionary<string, Guid> products;
        private readonly ILog log;

        private WebItemSecurity WebItemSecurity { get; }
        private AuthContext AuthContext { get; }

        static ProductSecurityFilter()
        {
            var blog = new Guid("6a598c74-91ae-437d-a5f4-ad339bd11bb2");
            var bookmark = new Guid("28b10049-dd20-4f54-b986-873bc14ccfc7");
            var forum = new Guid("853b6eb9-73ee-438d-9b09-8ffeedf36234");
            var news = new Guid("3cfd481b-46f2-4a4a-b55c-b8c0c9def02c");
            var wiki = new Guid("742cf945-cbbc-4a57-82d6-1600a12cf8ca");
            var photo = new Guid("9d51954f-db9b-4aed-94e3-ed70b914e101");

            products = new Dictionary<string, Guid>
                {
                    { "blog", blog },
                    { "bookmark", bookmark },
                    { "event", news },
                    { "forum", forum },
                    { "photo", photo },
                    { "wiki", wiki },
                    { "birthdays", WebItemManager.BirthdaysProductID },
                    { "community", WebItemManager.CommunityProductID },
                    { "crm", WebItemManager.CRMProductID },
                    { "files", WebItemManager.DocumentsProductID },
                    { "project", WebItemManager.ProjectsProductID },
                    { "calendar", WebItemManager.CalendarProductID },
                    { "mail", WebItemManager.MailProductID },
                };
        }


        public ProductSecurityFilter(
            IOptionsMonitor<ILog> options,
            WebItemSecurity webItemSecurity,
            AuthContext authContext)
        {
            log = options.CurrentValue;
            WebItemSecurity = webItemSecurity;
            AuthContext = authContext;
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (!AuthContext.IsAuthenticated) return;

            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                var pid = FindProduct(controllerActionDescriptor);
                if (pid != Guid.Empty)
                {
                    if (CallContext.GetData("asc.web.product_id") == null)
                    {
                        CallContext.SetData("asc.web.product_id", pid);
                    }
                    if (!WebItemSecurity.IsAvailableForMe(pid))
                    {
                        context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
                        log.WarnFormat("Product {0} denied for user {1}", controllerActionDescriptor.ControllerName, AuthContext.CurrentAccount);
                    }
                }
            }
        }

        private static Guid FindProduct(ControllerActionDescriptor method)
        {
            if (method == null || string.IsNullOrEmpty(method.ControllerName))
            {
                return default;
            }
            var name = method.ControllerName.ToLower();
            if (name == "community")
            {
                var url = method.MethodInfo.GetCustomAttribute<HttpMethodAttribute>().Template;
                if (!string.IsNullOrEmpty(url))
                {
                    var module = url.Split('/')[0];
                    if (products.TryGetValue(module, out var communityProduct))
                    {
                        return communityProduct;
                    }
                }
            }

            if (products.TryGetValue(name, out var product))
            {
                return product;
            }
            return default;
        }
    }
}
