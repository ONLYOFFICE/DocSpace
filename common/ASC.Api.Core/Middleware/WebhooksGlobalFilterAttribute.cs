using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ASC.Api.Core.Middleware;

[Scope]
public class WebhooksGlobalFilterAttribute : ResultFilterAttribute
{
    private readonly IWebhookPublisher _webhookPublisher;
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private static List<string> _methodList = new List<string> { "POST", "UPDATE", "DELETE" };

    public WebhooksGlobalFilterAttribute(IWebhookPublisher webhookPublisher, Action<JsonOptions> projectJsonOptions)
    {
        _webhookPublisher = webhookPublisher;

        var jsonOptions = new JsonOptions();
        projectJsonOptions.Invoke(jsonOptions);
        _jsonSerializerOptions = jsonOptions.JsonSerializerOptions;
    }

    public override void OnResultExecuted(ResultExecutedContext context)
    {
        var method = context.HttpContext.Request.Method;

        if (!_methodList.Contains(method) || context.Canceled)
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
            var resultContent = JsonSerializer.Serialize(objectResult.Value, _jsonSerializerOptions);

            var eventName = $"method: {method}, route: {routePattern}";

            _webhookPublisher.Publish(eventName, resultContent);
        }

        base.OnResultExecuted(context);
    }
}