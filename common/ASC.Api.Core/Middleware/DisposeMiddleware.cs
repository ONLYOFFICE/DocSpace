namespace ASC.Api.Core.Middleware
{
    public class DisposeMiddleware
    {
        private readonly RequestDelegate next;

        public DisposeMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.RegisterForDispose(new DisposableHttpContext(context));

            await next.Invoke(context);
        }
    }

    public static class DisposeMiddlewareExtensions
    {
        public static IApplicationBuilder UseDisposeMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DisposeMiddleware>();
        }
    }
}
