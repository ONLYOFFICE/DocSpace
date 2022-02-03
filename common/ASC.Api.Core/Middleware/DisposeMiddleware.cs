namespace ASC.Api.Core.Middleware
{
    public class DisposeMiddleware
    {
        private readonly RequestDelegate _next;

        public DisposeMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            context.Response.RegisterForDispose(new DisposableHttpContext(context));

            await _next.Invoke(context);
        }
    }

    public static class DisposeMiddlewareExtensions
    {
        public static IApplicationBuilder UseDisposeMiddleware(this IApplicationBuilder builder) =>
            builder.UseMiddleware<DisposeMiddleware>();
    }
}