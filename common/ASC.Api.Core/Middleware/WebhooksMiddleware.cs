using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using ASC.Webhooks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace ASC.Api.Core.Middleware
{
    public class WebhooksMiddleware
    {
        private readonly RequestDelegate _next;
        private WebhookPublisher WebhookPublisher { get; }
        private WebhooksIdentifier WebhooksIdentifier { get; }

        public WebhooksMiddleware(RequestDelegate next, WebhookPublisher webhookPublisher, WebhooksIdentifier webhooksIdentifier)
        {
            _next = next;
            WebhookPublisher = webhookPublisher;
            WebhooksIdentifier = webhooksIdentifier;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var methodList = new List<string> { "POST", "UPDATE", "DELETE" };

            var method = context.Request.Method;
            var endpoint = (RouteEndpoint)context.GetEndpoint();
            var routePattern = endpoint.RoutePattern.RawText;

            if (!methodList.Contains(method) && !WebhooksIdentifier.Identify(routePattern))
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

            var eventName = "method: " + method + ", " + "route: " + routePattern;

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
