using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ASC.Api.Core.Middleware
{
    public class ResponseWrapper
    {
        private readonly RequestDelegate next;

        public ResponseWrapper(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Exception error = null;
            var currentBody = context.Response.Body;

            using var memoryStream = new MemoryStream();

            context.Response.Body = memoryStream;

            try
            {
                await next(context);
            }
            catch(Exception exception)
            {
                context.Response.StatusCode = 500;
                error = exception;
            }

            context.Response.Body = currentBody;
            memoryStream.Seek(0, SeekOrigin.Begin);

            ResponseParser responseParser;

            switch (context.Request.RouteValues["format"])
            {
                case "xml":
                    responseParser = new XmlResponseParser();
                    break;
                case "json":
                default:
                    responseParser = new JsonResponseParser();
                    break;
            }

            var readToEnd = new StreamReader(memoryStream).ReadToEnd();
            if(error != null)
            {
                await context.Response.WriteAsync(responseParser.WrapAndWrite((HttpStatusCode)context.Response.StatusCode, error));
            }
            else
            {
                await context.Response.WriteAsync(responseParser.WrapAndWrite((HttpStatusCode)context.Response.StatusCode, readToEnd));
            }
        }

    }

    public static class ResponseWrapperExtensions
    {
        public static IApplicationBuilder UseResponseWrapper(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseWrapper>();
        }
    }
}