using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using ASC.Webhooks.Core;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ASC.Api.Core.Middleware
{
    public class WebhooksMiddleware
    {
        private readonly RequestDelegate _next;
        private IWebhookPublisher WebhookPublisher { get; }

        public WebhooksMiddleware(RequestDelegate next, IWebhookPublisher webhookPublisher)
        {
            _next = next;
            WebhookPublisher = webhookPublisher;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var methodList = new List<string> { "POST", "UPDATE", "DELETE" };

            var method = context.Request.Method;
            var endpoint = (RouteEndpoint)context.GetEndpoint();
            var routePattern = endpoint?.RoutePattern.RawText;

            if (!methodList.Contains(method) || routePattern == null)
            {
                await _next(context);
                return;
            }

            string responseContent;
            var originalResponseBody = context.Response.Body;
            using (var ms = new MemoryStream())
            {
                context.Response.Body = ms;
                await _next(context);

                ms.Position = 0;
                var responseReader = new StreamReader(ms);

                responseContent = responseReader.ReadToEnd();

                ms.Position = 0;
                await ms.CopyToAsync(originalResponseBody);
                context.Response.Body = originalResponseBody;
            }
            var eventName = $"method: {method}, route: {routePattern}";

            WebhookPublisher.Publish(eventName, responseContent);
        }
    }

    public static class WebhooksMiddlewareExtensions
    {
        public static IApplicationBuilder UseWebhooksMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebhooksMiddleware>();
        }
    }
}
