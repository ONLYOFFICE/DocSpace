using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ASC.Webhooks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

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
            var action = context.GetRouteValue("action").ToString();

            var eventName = WebhooksIdentifier.Identify(action);

            if (eventName == EventName.UntrackedAction)
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
