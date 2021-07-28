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

        public WebhooksMiddleware(RequestDelegate next, WebhookPublisher webhookPublisher)
        {
            _next = next;
            WebhookPublisher = webhookPublisher;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            EventName eventName;
            var action = context.GetRouteValue("action").ToString();

            switch (action)
            {
                case "CreateFileFromBody":
                case "CreateFileFromForm":
                    eventName = EventName.NewFileCreated;
                    break;
                default:
                    eventName = EventName.UntrackedAction;
                    break;
            }

            if(eventName == EventName.UntrackedAction)
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

                var pos = ms.Position;
                ms.Position = 0;
                var responseReader = new StreamReader(ms);

                responseContent = responseReader.ReadToEnd();

                ms.Position = 0;
                await ms.CopyToAsync(originalResponseBody);
                context.Response.Body = originalResponseBody;
            }

            WebhookPublisher.Publish(eventName, responseContent);
        }

        private bool isWebhooks(HttpRequest request)
        {
            return true;
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
