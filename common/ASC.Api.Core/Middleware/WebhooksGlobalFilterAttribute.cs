﻿namespace ASC.Api.Core.Middleware
{
    [Scope]
    public class WebhooksGlobalFilterAttribute : ResultFilterAttribute
    {
        private IWebhookPublisher WebhookPublisher { get; }
        private static List<string> methodList = new List<string> { "POST", "UPDATE", "DELETE" };
        private JsonSerializerOptions jsonSerializerOptions;

        public WebhooksGlobalFilterAttribute(IWebhookPublisher webhookPublisher, Action<JsonOptions> projectJsonOptions)
        {
            WebhookPublisher = webhookPublisher;

            var jsonOptions = new JsonOptions();
            projectJsonOptions.Invoke(jsonOptions);
            jsonSerializerOptions = jsonOptions.JsonSerializerOptions;
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            var method = context.HttpContext.Request.Method;

            if (!methodList.Contains(method) || context.Canceled)
            {
                base.OnResultExecuted(context);
                return;
            }

            var endpoint = (RouteEndpoint)context.HttpContext.GetEndpoint();
            var routePattern = endpoint?.RoutePattern.RawText;

            if (routePattern == null)
            {
                base.OnResultExecuted(context);
                return;
            }

            if (context.Result is ObjectResult objectResult)
            {
                var resultContent = System.Text.Json.JsonSerializer.Serialize(objectResult.Value, jsonSerializerOptions);

                var eventName = $"method: {method}, route: {routePattern}";

                WebhookPublisher.Publish(eventName, resultContent);
            }

            base.OnResultExecuted(context);
        }
    }
}
